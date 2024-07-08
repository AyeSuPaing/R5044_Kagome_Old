/*
=========================================================================================================
  Module      : 定期継続分析レポートページ処理(FixedPurchaseRepeatAnalysisReport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain;
using w2.Domain.User.Helper;

/// <summary>
/// 定期継続分析レポートページ処理
/// </summary>
public partial class Form_FixedPurchaseRepeatAnalysisReport_FixedPurchaseRepeatAnalysisReport : BasePage
{
	#region 表示定数
	/// <summary>Display max count</summary>
	private const int DISPLAY_MAX_COUNT = 10;
	/// <summary>Max times of purchase</summary>
	private const int DISPLAY_LTV_MAX_COUNT = 12;

	/// <summary>Request key: report display kbn</summary>
	private const string REQUEST_KEY_REPORT_DISPLAY_KBN_TYPE = "rpdkbnt";
	/// <summary>Request key adv media kbn</summary>
	private const string REQUEST_KEY_ADV_MEDIA_KBN = "amk";
	/// <summary>Request param: payment type</summary>
	private const string REQUEST_KEY_PAYMENT_TYPE = "pt";
	/// <summary>Request param: adv code</summary>
	private const string REQUEST_KEY_ADV_CODE = "ac";
	/// <summary>Request param: user extend 1</summary>
	private const string REQUEST_KEY_USER_EXTEND1 = "ue1";
	/// <summary>Request param: user extend 2</summary>
	private const string REQUEST_KEY_USER_EXTEND2 = "ue2";

	/// <summary>Search param: product id flag: search multi</summary>
	private const string CONST_SEARCH_PARAM_PRODUCT_ID_FLG_SEARCH_MULTI = "1";
	/// <summary>Search param: product id flag: search like</summary>
	private const string CONST_SEARCH_PARAM_PRODUCT_ID_FLG_SEARCH_LIKE = "0";

	/// <summary>Search param: user extend flag: use</summary>
	private const string CONST_SEARCH_PARAM_USER_EXTEND_FLG_USE = "1";
	/// <summary>Search param: user extend flag: none</summary>
	private const string CONST_SEARCH_PARAM_USER_EXTEND_FLG_NONE = "0";

	/// <summary>Summary field: amount subtotal</summary>
	private const string CONST_FIELD_SUMMARY_AMOUNT_SUBTOTAL = "amount_subtotal";
	/// <summary>Summary field: order count total</summary>
	private const string CONST_FIELD_SUMMARY_ORDER_COUNT_TOTAL = "order_count_total";
	/// <summary>Summary field: product count</summary>
	private const string CONST_FIELD_SUMMARY_MONTH_COUNT = "month_count";
	/// <summary>Summary field: consecutive purchase month</summary>
	private const string CONST_FIELD_SUMMARY_CONSECUTIVE_PURCHASE_MONTH = "consecutive_purchase_month";
	/// <summary>Summary field: fixed purchase cancel count</summary>
	private const string CONST_FIELD_SUMMARY_FIXED_PURCHASE_CANCEL_COUNT = "fixed_purchase_cancel_count";
	/// <summary>Summary field: fixed purchase count total</summary>
	private const string CONST_FIELD_SUMMARY_FIXED_PURCHASE_COUNT_TOTAL = "fixed_purchase_count_total";
	/// <summary>Detail field: tgt month</summary>
	private const string CONST_FIELD_TGT_MONTH = "tgt_month";
	/// <summary>Detail field: tgt year</summary>
	private const string CONST_FIELD_TGT_YEAR = "tgt_year";
	/// <summary>Detail field: order count</summary>
	private const string CONST_FIELD_ORDER_COUNT = "order_count";
	/// <summary>Detail field: order cancel</summary>
	private const string CONST_FIELD_FIXED_PURCHASE_CANCEL = "fixed_purchase_cancel";
	/// <summary>Detail field: order price subtotal</summary>
	private const string CONST_FIELD_ORDER_PRICE_SUBTOTAL = "order_price_subtotal";
	/// <summary>Detail field: order date</summary>
	private const string CONST_FIELD_ORDER_DATE = "order_date";

	/// <summary>Tag search param: multi product ids</summary>
	private const string TAG_SEARCH_PARAM_MULTI_PRODUCT_IDS = "@@ multi_product_ids @@";
	/// <summary>Search param: product ids</summary>
	private const string CONST_SEARCH_PARAM_PRODUCT_IDS = "product_ids";
	/// <summary>Search param: begin max count</summary>
	private const string SEARCH_PARAM_BEGIN_MAX_COUNT = "begin_max_count";
	/// <summary>Search param: end max count</summary>
	private const string SEARCH_PARAM_END_MAX_COUNT = "end_max_count";
	/// <summary>Search param: product id flag</summary>
	private const string SEARCH_PARAM_PRODUCT_ID_FLG = "product_id_flg";
	/// <summary>Search param: product id like escaped</summary>
	private const string SEARCH_PARAM_PRODUCT_ID_LIKE_ESCAPED = "product_id_like_escaped";
	/// <summary>Search param: variation id like escaped</summary>
	private const string SEARCH_PARAM_VARIATION_ID_LIKE_ESCAPED = "variation_id_like_escaped";
	/// <summary>Search param: adv code media type id</summary>
	private const string SEARCH_PARAM_ADVCODE_MEDIA_TYPE_ID = "advcode_media_type_id";
	/// <summary>Search param: advertisement code like escaped</summary>
	private const string SEARCH_PARAM_ADVERTISEMENT_CODE_LIKE_ESCAPED = "advcode_like_escaped";
	/// <summary>Search param: order payment kbn</summary>
	private const string SEARCH_PARAM_ORDER_PAYMENT_KBN = "order_payment_kbn";

	/// <summary>Parameter key: total count</summary>
	private const string CONST_PARAM_KEY_TOTAL_COUNT = "total_count";
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			// コンポーネント初期化
			Initialize();

			// 画面表示
			DisplayForm();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
		// Report kbn
		var reportKbns = ValueText.GetValueItemArray(
			Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT,
			Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DISPLAY_KBN_TYPE);
		rblReportKbn.Items.AddRange(reportKbns);

		// 広告媒体区分
		var advCodeMediaTypes = DomainFacade.Instance.AdvCodeService.GetAdvCodeMediaTypeListAll();
		ddlSearchAdvMediaKbn.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlSearchAdvMediaKbn.Items.AddRange(
			advCodeMediaTypes
				.Select(item =>
					new ListItem(
						AbbreviateString(item.AdvcodeMediaTypeName, 10),
						item.AdvcodeMediaTypeId))
				.ToArray());

		// ユーザー拡張項目の設定
		this.UserExtendSettings = new UserExtendSettingList(this.LoginOperatorId);
		ddlUserExtend.Items.Add(new ListItem(string.Empty, string.Empty));
		ddlUserExtend.Items.AddRange(
			this.UserExtendSettings.Items
				.Select(item => new ListItem(item.SettingName, item.SettingId))
				.ToArray());

		// 決済種別
		ddlPaymentType.Items.Add(new ListItem(string.Empty, string.Empty));
		var validPayments = DomainFacade.Instance.PaymentService.GetValidPayments(this.LoginOperatorShopId);
		ddlPaymentType.Items.AddRange(
			validPayments
				.Select(item =>
					new ListItem(
						AbbreviateString(item.PaymentName, 12),
						item.PaymentId))
				.ToArray());

		// 検索条件の復元
		tbProductId.Text = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PRODUCT_ID]);
		tbVariationId.Text = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_VARIATION_ID]);
		rblReportKbn.SelectedValue = StringUtility.ToEmpty(
			string.IsNullOrEmpty(Request[REQUEST_KEY_REPORT_DISPLAY_KBN_TYPE])
				? Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DISPLAY_KBN_TYPE_LTV
				: Request[REQUEST_KEY_REPORT_DISPLAY_KBN_TYPE]);
		ddlSearchAdvMediaKbn.SelectedValue = StringUtility.ToEmpty(Request[REQUEST_KEY_ADV_MEDIA_KBN]);
		tbSearchAdvCode.Text = StringUtility.ToEmpty(Request[REQUEST_KEY_ADV_CODE]);
		ddlUserExtend.SelectedValue = StringUtility.ToEmpty(Request[REQUEST_KEY_USER_EXTEND1]);
		tbUserExtend.Text = StringUtility.ToEmpty(Request[REQUEST_KEY_USER_EXTEND2]);
		ddlPaymentType.SelectedValue = StringUtility.ToEmpty(Request[REQUEST_KEY_PAYMENT_TYPE]);

		// パラメータにより、年の設定
		int year;
		this.CurrentYear = int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_YEAR], out year)
			? year
			: DateTime.Now.Year;
		var dateStartYear = this.CurrentYear.ToString();
		var dateEndYear = int.TryParse(Request[Constants.REQUEST_KEY_TARGET_YEAR], out year)
			? year.ToString()
			: this.CurrentYear.ToString();

		// パラメータにより、月の設定
		int month;
		this.CurrentMonth = int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_MONTH], out month)
			? month
			: DateTime.Now.Month;
		var dateStartMonth = this.CurrentMonth.ToString("00");
		var dateEndMonth = int.TryParse(Request[Constants.REQUEST_KEY_TARGET_MONTH], out month)
			? month.ToString("00")
			: this.CurrentMonth.ToString("00");

		// パラメータにより、日の設定
		int day;
		var dateStartDay = int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_DAY], out day)
			? day.ToString("00")
			: "01";
		var dateEndDay = int.TryParse(Request[Constants.REQUEST_KEY_TARGET_DAY], out day)
			? day.ToString("00")
			: DateTime.DaysInMonth(int.Parse(dateStartYear), int.Parse(dateStartMonth)).ToString("00");

		var dateStart = string.Format(
			"{0}/{1}/{2} {3}",
			dateStartYear,
			dateStartMonth,
			dateStartDay,
			string.IsNullOrEmpty(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CURRENT_TIME]))
				? "00:00:00"
				: StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CURRENT_TIME]));
		var dateEnd = string.Format(
			"{0}/{1}/{2} {3}",
			dateEndYear,
			dateEndMonth,
			dateEndDay,
			string.IsNullOrEmpty(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_TIME]))
				? "23:59:59"
				: StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_TIME]));

		DateTime currentDate;
		DateTime targetDate;
		if (DateTime.TryParse(dateStart, out currentDate)
			&& DateTime.TryParse(dateEnd, out targetDate))
		{
			ucTargetPeriod.SetPeriodDate(currentDate, targetDate);
		}
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	private void DisplayForm()
	{
		if (this.IsFirst || (IsSeachCondition() == false))
		{
			dvLoadingImg.Visible = false;
			return;
		}

		// 集計データ取得
		GetData();
	}

	/// <summary>
	/// データ取得
	/// </summary>
	/// <returns>表示用データ</returns>
	private void GetData()
	{
		// LTVレポート
		if (this.IsLtvReportKbn)
		{
			GetDataForReportLtv();
		}
		// 回数別レポート
		else
		{
			GetDataForReportRegularFrequency();
			// データバイド
			DataBind();
		}
	}

	/// <summary>
	/// データ取得 (回数別レポート)
	/// </summary>
	private void GetDataForReportRegularFrequency()
	{
		// SQL実行
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("FixedPurchaseRepeatAnalysis", "GetReportData"))
		{
			var input = new Hashtable
			{
				{
					Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_PRODUCT_ID + "_like_escaped",
					StringUtility.SqlLikeStringSharpEscape(tbProductId.Text.Trim())
				},
				{
					Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_VARIATION_ID + "_like_escaped",
					StringUtility.SqlLikeStringSharpEscape(tbVariationId.Text.Trim())
				},
			};

			// データ取得
			var dv = statement.SelectSingleStatementWithOC(accessor, input);

			this.DisplayDataList = CreateDisplayDataList(dv);
		}
	}

	/// <summary>
	/// 表示用データ取得 (回数別レポート)
	/// </summary>
	/// <param name="dv">データビュー</param>
	/// <returns>レポートデータリスト</returns>
	private List<ReportData> CreateDisplayDataList(DataView dv)
	{
		var result = new List<ReportData>();
		ReportData current = null;
		int count = 0;
		foreach (DataRowView drv in dv)
		{
			// 一行目、もしくは商品バリエーションが変わった場合
			if ((current == null)
				|| (current.ProductId != (string)drv[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_PRODUCT_ID])
				|| (current.VariationId != (string)drv[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_VARIATION_ID]))
			{
				if (current != null)
				{
					current.SetContents();
					result.Add(current);
					count++;
				}
				current = new ReportData();
				current.Index = count;
				current.ProductId = (string)drv[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_PRODUCT_ID];
				current.VariationId = (string)drv[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_VARIATION_ID];
				current.ProductName = w2.App.Common.Order.ProductCommon.CreateProductJointName(drv);
			}

			// 11回以上は表示対象外
			if ((int)drv[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_COUNT] > DISPLAY_MAX_COUNT) continue;

			// 1回も購入せずキャンセルした場合は対象外
			if ((int)drv[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_COUNT] < 1) continue;

			var index = (int)drv[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_COUNT] - 1;
			var userCount = (int)drv["user_count"];
			current.CountData[index].Index = index;
			switch ((string)drv[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_STATUS])
			{
				case Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_EXISTS:
					current.CountData[index].PlanCount = userCount;
					break;

				case Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_ORDER:
					current.CountData[index].OrderCount = userCount;
					break;

				case Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_DELIVERED:
					current.CountData[index].DeliveredCount = userCount;
					break;

				case Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_FALLOUT:
					current.CountData[index].FallOutCount = userCount;
					break;
			}
		}
		if (current != null)
		{
			current.SetContents();
			result.Add(current);
		}
		return result;
	}

	/// <summary>
	/// データ取得 (LTVレポート)
	/// </summary>
	private void GetDataForReportLtv()
	{
		this.ProcessInfo = new ProcessInfoType();
		tProcessTimer.Enabled = true;
		dvLoadingImg.Visible = true;
		cbLtv.Visible = false;
		lbLtvReportExport.Visible = false;
		trShowLtvContents.Visible = false;

		Task.Run(
			() =>
			{
				this.ProcessInfo.StartTime = DateTime.Now;
				try
				{
					// Get report data from database
					var param = GetSearchParameters();
					var dv = GetDataLtvReportInDataView(param);
					var count = (dv.Count == 0)
						? 0
						: (int)dv[0]["row_count"];
					if (count == 0)
					{
						this.ProcessInfo.IsDone = true;
						return;
					}

					// Create and set report data
					this.ProcessInfo.DisplayLtvDataView = dv;
					this.ProcessInfo.DisplayLtvSummaryDataView = dv;
					this.ProcessInfo.TotalCount = count;
					this.ProcessInfo.CurrentPageNumber = (int)param[Constants.REQUEST_KEY_PAGE_NO];
					var monthDataList = new List<string>();
					var dateFrom = (DateTime)param[Constants.SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DATE_FROM];
					var dateBegin = dateFrom.AddMonths(DISPLAY_LTV_MAX_COUNT * (this.ProcessInfo.CurrentPageNumber - 1));
					var dateEnd = dateFrom.AddMonths((DISPLAY_LTV_MAX_COUNT * this.ProcessInfo.CurrentPageNumber) - 1);
					for (var date = dateBegin; date <= dateEnd; date = date.AddMonths(1))
					{
						var monthString = string.Format(
							ReplaceTag("@@DispText.common_message.OrderDate@@"),
							date.Year,
							date.Month);
						monthDataList.Add(monthString);
					}
					this.ProcessInfo.MonthDataList = monthDataList;
					param[CONST_PARAM_KEY_TOTAL_COUNT] = count;
					this.ProcessInfo.Param = param;
					this.ProcessInfo.IsDone = true;
				}
				catch (Exception ex)
				{
					tProcessTimer.Enabled = false;
					this.ProcessInfo.IsSystemError = true;
					FileLogger.WriteError(ex);
				}
			});
	}

	/// <summary>
	/// Get data LTV report (DataView)
	/// </summary>
	/// <param name="param">Parameters</param>
	/// <returns>LTV report (DataView)</returns>
	private DataView GetDataLtvReportInDataView(Hashtable param)
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("FixedPurchaseRepeatAnalysis", "GetLtvReportData") { CommandTimeout = 600000 })
		{
			if ((string)param[SEARCH_PARAM_PRODUCT_ID_FLG] == CONST_SEARCH_PARAM_PRODUCT_ID_FLG_SEARCH_MULTI)
			{
				var productIds = (string[])param[CONST_SEARCH_PARAM_PRODUCT_IDS];
				statement.ReplaceStatement(
					TAG_SEARCH_PARAM_MULTI_PRODUCT_IDS,
					string.Join(
						",",
						productIds.Select(id =>
							string.Format("'{0}'", id.Replace("'", "''")))));
			}

			var userExtendName = (string)param[Constants.SEARCH_PARAM_USER_EXTEND_NAME];
			statement.ReplaceStatement("@@ user_extend_field_name @@", userExtendName);
			statement.ReplaceStatement(
				TAG_SEARCH_PARAM_MULTI_PRODUCT_IDS,
				StringUtility.SqlLikeStringSharpEscape(param[TAG_SEARCH_PARAM_MULTI_PRODUCT_IDS]));

			// データ取得
			var dv = statement.SelectSingleStatementWithOC(accessor, param);
			return dv;
		}
	}

	/// <summary>
	/// Get search parameters
	/// </summary>
	/// <returns>Search parameters</returns>
	private Hashtable GetSearchParameters()
	{
		var productId = tbProductId.Text.Trim();
		var productIds = productId.Split(',');
		var searchProductIdFlg = (string.IsNullOrEmpty(productId) == false)
			? (productIds.Length > 1)
				? CONST_SEARCH_PARAM_PRODUCT_ID_FLG_SEARCH_MULTI
				: CONST_SEARCH_PARAM_PRODUCT_ID_FLG_SEARCH_LIKE
			: string.Empty;
		var multiProductIds = string.Join(
			",",
			productIds.Select(id =>
				string.Format("'{0}'", id.Replace("'", "''"))));

		var userExtendName = ddlUserExtend.SelectedValue;
		var userExtendText = tbUserExtend.Text.Trim();
		var userExtendFlg = (string.IsNullOrEmpty(userExtendName) == false)
			? (string.IsNullOrEmpty(userExtendText) == false)
				? CONST_SEARCH_PARAM_USER_EXTEND_FLG_USE
				: CONST_SEARCH_PARAM_USER_EXTEND_FLG_NONE
			: string.Empty;

		var pageNo = 0;
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNo) == false)
		{
			pageNo = 1;
		};

		DateTime dateFrom;
		DateTime dateTo;
		CheckDateSearch(out dateFrom, out dateTo);

		var result = new Hashtable
		{
			{ SEARCH_PARAM_PRODUCT_ID_FLG, searchProductIdFlg },
			{ CONST_SEARCH_PARAM_PRODUCT_IDS, productIds },
			{ SEARCH_PARAM_PRODUCT_ID_LIKE_ESCAPED, StringUtility.SqlLikeStringSharpEscape(productId) },
			{ SEARCH_PARAM_VARIATION_ID_LIKE_ESCAPED, StringUtility.SqlLikeStringSharpEscape(tbVariationId.Text.Trim()) },
			{ SEARCH_PARAM_ADVCODE_MEDIA_TYPE_ID, ddlSearchAdvMediaKbn.SelectedValue },
			{ SEARCH_PARAM_ADVERTISEMENT_CODE_LIKE_ESCAPED, StringUtility.SqlLikeStringSharpEscape(tbSearchAdvCode.Text.Trim()) },
			{ Constants.SEARCH_PARAM_USER_EXTEND_NAME, userExtendName },
			{ Constants.SEARCH_PARAM_USER_EXTEND_FLG, userExtendFlg },
			{ SEARCH_PARAM_ORDER_PAYMENT_KBN, ddlPaymentType.SelectedValue },
			{ Constants.SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DATE_FROM, dateFrom },
			{ Constants.SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DATE_TO, dateTo },
			{ SEARCH_PARAM_BEGIN_MAX_COUNT, (pageNo == 1) ? 1 : (DISPLAY_LTV_MAX_COUNT * (pageNo - 1)) },
			{ SEARCH_PARAM_END_MAX_COUNT, DISPLAY_LTV_MAX_COUNT * pageNo },
			{ TAG_SEARCH_PARAM_MULTI_PRODUCT_IDS, multiProductIds },
			{ Constants.REQUEST_KEY_PAGE_NO, pageNo },
		};

		if (string.IsNullOrEmpty(userExtendName))
		{
			result.Add(Constants.SEARCH_PARAM_USER_EXTEND_LIKE_ESCAPED, string.Empty);
		}
		else
		{
			var userExtendSetting =
				this.UserExtendSettings.Items.FirstOrDefault(userExtend =>
					(userExtend.SettingId == userExtendName));
			if (userExtendSetting != null)
			{
				result.Add(Constants.SEARCH_PARAM_USER_EXTEND_LIKE_ESCAPED, StringUtility.SqlLikeStringSharpEscape(userExtendText));
				result.Add(Constants.SEARCH_PARAM_USER_EXTEND_TYPE, userExtendSetting.InputType);
			}
		}

		return result;
	}

	/// <summary>
	/// Create display LTV summary data
	/// </summary>
	/// <param name="data">Data</param>
	/// <returns>Display LTV summary data</returns>
	public ReportLtvSummaryData CreateDisplayLtvSummaryData(DataView data)
	{
		if ((int)data[0][CONST_FIELD_SUMMARY_ORDER_COUNT_TOTAL] == 0) return new ReportLtvSummaryData();

		var report = new ReportLtvSummaryData(
			(decimal)data[0][CONST_FIELD_SUMMARY_AMOUNT_SUBTOTAL],
			(int)data[0][CONST_FIELD_SUMMARY_ORDER_COUNT_TOTAL],
			(int)data[0][CONST_FIELD_SUMMARY_MONTH_COUNT],
			(int)data[0][CONST_FIELD_SUMMARY_CONSECUTIVE_PURCHASE_MONTH],
			(int)data[0][CONST_FIELD_SUMMARY_FIXED_PURCHASE_CANCEL_COUNT],
			(int)data[0][CONST_FIELD_SUMMARY_FIXED_PURCHASE_COUNT_TOTAL],
			cbLtv.Checked);
		return report;
	}

	/// <summary>
	/// 表示用データ取得 (LTVレポート)
	/// </summary>
	/// <param name="dv">データビュー</param>
	/// <param name="currentPageNumber">Current page number</param>
	/// <param name="totalCount">Total count</param>
	/// <param name="isCsvExport">Is Csv export</param>
	/// <returns>レポートデータリスト</returns>
	private List<ReportLtvData> CreateDisplayLtvDataList(
		DataView dv,
		int currentPageNumber,
		int totalCount,
		bool isCsvExport = false)
	{
		var result = new List<ReportLtvData>();
		var maxCount = isCsvExport
			? (totalCount % DISPLAY_LTV_MAX_COUNT == 0)
				? totalCount
				: totalCount + (DISPLAY_LTV_MAX_COUNT - totalCount % DISPLAY_LTV_MAX_COUNT)
			: (currentPageNumber == 1)
				? DISPLAY_LTV_MAX_COUNT
				: DISPLAY_LTV_MAX_COUNT + 1;
		ReportLtvData current = null;
		ReportMonthData rowData = null;
		foreach (DataRowView drv in dv)
		{
			var productId = (string)drv[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_PRODUCT_ID];
			var variationId = (string)drv[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_VARIATION_ID];
			var currentYear = (int)drv[CONST_FIELD_TGT_YEAR];
			var currentMonth = (int)drv[CONST_FIELD_TGT_MONTH];
			if ((current == null)
				|| (current.ProductId != productId)
				|| (current.VariationId != variationId))
			{
				if (current != null)
				{
					current.SetContentsMonthData();
					result.Add(current);
				}

				current = new ReportLtvData
				{
					ProductId = productId,
					VariationId = variationId,
					ProductName = w2.App.Common.Order.ProductCommon.CreateProductJointName(drv),
					ImageProduct = new Dictionary<string, string>
					{
						{ Constants.FIELD_PRODUCT_SHOP_ID, (string)drv[Constants.FIELD_PRODUCT_SHOP_ID] },
						{ Constants.FIELD_PRODUCT_IMAGE_HEAD, (string)drv[Constants.FIELD_PRODUCT_IMAGE_HEAD] },
					},
				};
			}

			if (current.MonthData.Any(item => (item.Month == currentMonth)) == false)
			{
				rowData = new ReportMonthData(maxCount)
				{
					Year = currentYear,
					Month = currentMonth,
				};
				current.MonthData.Add(rowData);
			}

			var nextMonthCount = Convert.ToInt32(drv[Constants.FIELD_FIXEDPURCHASEREPEATANALYSIS_COUNT]);
			var index = isCsvExport
				? nextMonthCount - 1
				: (currentPageNumber == 1)
					? nextMonthCount - DISPLAY_LTV_MAX_COUNT * (currentPageNumber - 1) - 1
					: nextMonthCount - DISPLAY_LTV_MAX_COUNT * (currentPageNumber - 1);
			if ((index + 1) <= maxCount)
			{
				rowData.CountData[index].OrderCount = (int)drv[CONST_FIELD_ORDER_COUNT];
				rowData.CountData[index].FixedPurchaseCancel = (int)drv[CONST_FIELD_FIXED_PURCHASE_CANCEL];
				rowData.CountData[index].TotalPrice = (decimal)drv[CONST_FIELD_ORDER_PRICE_SUBTOTAL];
				rowData.CountData[index].OrderDate = (DateTime)drv[CONST_FIELD_ORDER_DATE];
			}
		}

		if (current != null)
		{
			current.SetContentsMonthData();
			result.Add(current);
		}

		return result;
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// LTVレポート
		if (this.IsLtvReportKbn)
		{
			Response.Redirect(CreateLtvReportListUrl(1));
		}
		// 回数別レポート
		else
		{
			var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT)
				.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, tbProductId.Text.Trim())
				.AddParam(Constants.REQUEST_KEY_VARIATION_ID, tbVariationId.Text.Trim())
				.AddParam(REQUEST_KEY_REPORT_DISPLAY_KBN_TYPE, rblReportKbn.SelectedValue);
			Response.Redirect(urlCreator.CreateUrl());
		}
	}

	/// <summary>
	/// Create LTV report list url
	/// </summary>
	/// <returns>LTV report list url</returns>
	public string CreateLtvReportListUrl()
	{
		DateTime reportDatePeriodCurrent;
		DateTime reportDatePeriodTaget;
		CheckDateSearch(out reportDatePeriodCurrent, out reportDatePeriodTaget);
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT)
			.AddParam(Constants.REQUEST_KEY_PRODUCT_ID, tbProductId.Text.Trim())
			.AddParam(Constants.REQUEST_KEY_VARIATION_ID, tbVariationId.Text.Trim())
			.AddParam(REQUEST_KEY_REPORT_DISPLAY_KBN_TYPE, rblReportKbn.SelectedValue)
			.AddParam(REQUEST_KEY_ADV_MEDIA_KBN, ddlSearchAdvMediaKbn.SelectedValue)
			.AddParam(REQUEST_KEY_ADV_CODE, tbSearchAdvCode.Text.Trim())
			.AddParam(REQUEST_KEY_USER_EXTEND1, ddlUserExtend.SelectedValue)
			.AddParam(REQUEST_KEY_USER_EXTEND2, tbUserExtend.Text.Trim())
			.AddParam(REQUEST_KEY_PAYMENT_TYPE, ddlPaymentType.SelectedValue)
			.AddParam(
				Constants.REQUEST_KEY_CURRENT_YEAR,
				string.IsNullOrEmpty(ucTargetPeriod.StartDateString)
					? string.Empty
					: StringUtility.ToEmpty(reportDatePeriodCurrent.Year))
			.AddParam(
				Constants.REQUEST_KEY_CURRENT_MONTH,
				string.IsNullOrEmpty(ucTargetPeriod.HfStartDate.Value)
					? string.Empty
					: StringUtility.ToEmpty(reportDatePeriodCurrent.Month))
			.AddParam(
				Constants.REQUEST_KEY_CURRENT_DAY,
				string.IsNullOrEmpty(ucTargetPeriod.HfEndDate.Value)
					? string.Empty
					: StringUtility.ToEmpty(reportDatePeriodCurrent.Day))
			.AddParam(
				Constants.REQUEST_KEY_TARGET_YEAR,
				string.IsNullOrEmpty(ucTargetPeriod.HfEndDate.Value)
					? string.Empty
					: StringUtility.ToEmpty(reportDatePeriodTaget.Year))
			.AddParam(
				Constants.REQUEST_KEY_TARGET_MONTH,
				string.IsNullOrEmpty(ucTargetPeriod.HfStartDate.Value)
					? string.Empty
					: StringUtility.ToEmpty(reportDatePeriodTaget.Month))
			.AddParam(
				Constants.REQUEST_KEY_TARGET_DAY,
				string.IsNullOrEmpty(ucTargetPeriod.HfEndDate.Value)
					? string.Empty
					: StringUtility.ToEmpty(reportDatePeriodTaget.Day))
			.AddParam(Constants.REQUEST_KEY_CURRENT_TIME, ucTargetPeriod.HfStartTime.Value)
			.AddParam(Constants.REQUEST_KEY_TARGET_TIME, ucTargetPeriod.HfEndTime.Value)
			.CreateUrl();
		return url;
	}
	/// <summary>
	/// Create LTV report list url
	/// </summary>
	/// <param name="pageNumber">Page number</param>
	/// <returns>LTV report list url</returns>
	private string CreateLtvReportListUrl(int pageNumber)
	{
		var url = new UrlCreator(CreateLtvReportListUrl())
			.AddParam(Constants.REQUEST_KEY_PAGE_NO, pageNumber.ToString())
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// Create default report list url
	/// </summary>
	/// <returns>Default report list url</returns>
	protected string CreateDefaultReportListUrl()
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT)
			.AddParam(REQUEST_KEY_REPORT_DISPLAY_KBN_TYPE, rblReportKbn.SelectedValue)
			.CreateUrl();
		return url;
	}

	/// <summary>
	/// CSVダウンロードリンククリック (回数別レポート)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, System.EventArgs e)
	{
		// レポートデータ取得
		GetData();

		var header = CreateHeaderRecord();
		var contents = CreateContentsRecord();

		// ファイル名
		var fileName = "FixedPurchaseRepeatAnalysisReport_" + DateTime.Now.ToString("yyyyMMddHHmm");

		// ファイル出力
		OutPutFileCsv(fileName, header + "\r\n" + contents);
	}

	/// <summary>
	/// CSVダウンロードリンククリック (LTVレポート)
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbLtvReportExport_Click(object sender, System.EventArgs e)
	{
		// Get search parameters and set values to export all report data
		var param = (Hashtable)Session[Constants.SESSIONPARAM_KEY_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT_SEARCH_INFO];
		var dateFrom = (DateTime)param[Constants.SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DATE_FROM];
		var totalCount = (int)param[CONST_PARAM_KEY_TOTAL_COUNT];
		param[SEARCH_PARAM_BEGIN_MAX_COUNT] = 0;
		param[SEARCH_PARAM_END_MAX_COUNT] = totalCount;

		// Get report data from database
		var dv = GetDataLtvReportInDataView(param);
		// Create export Csv data
		var header = CreateLtvHeaderRecord(dateFrom, totalCount);
		var contents = CreateLtvContentsRecord(
			dv,
			(int)param[Constants.REQUEST_KEY_PAGE_NO],
			totalCount);

		// ファイル名
		var fileName = string.Format(
			"FixedPurchaseRepeatAnalysisLtvReport_{0:yyyyMMddHHmm}",
			DateTime.Now);

		// ファイル出力
		OutPutFileCsv(fileName, header + Environment.NewLine + contents);
	}

	/// <summary>
	/// ヘッダーレコード作成 (回数別レポート)
	/// </summary>
	/// <returns>ヘッダー文字列</returns>
	private string CreateHeaderRecord()
	{
		var result = new StringBuilder(ReplaceTag("@@Product.product_id.name@@"))
			.AppendFormat(",{0}", ReplaceTag("@@Product.variation_id.name@@"))
			.AppendFormat(",{0}", ReplaceTag("@@Product.name.name@@"))
			.AppendFormat(
				",{0}",
				// 「購入回数」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT,
					Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT_TITLE,
					Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT_NUMBER_PURCHASE));
		for (var i = 0; i < DISPLAY_MAX_COUNT; i++)
		{
			result.Append(",").AppendFormat(
				ReplaceTag("@@DispText.common_message.times@@"),
				i + 1);
		}

		return result.ToString();
	}

	/// <summary>
	/// Create LTV header record
	/// </summary>
	/// <param name="dateFrom">Date from</param>
	/// <param name="totalCount">Total count</param>
	/// <returns>ヘッダー文字列</returns>
	private string CreateLtvHeaderRecord(DateTime dateFrom, int totalCount)
	{
		var result = new StringBuilder(ReplaceTag("@@Product.product_id.name@@"))
			.AppendFormat(",{0}", ReplaceTag("@@Product.variation_id.name@@"))
			.AppendFormat(",{0}", ReplaceTag("@@Product.name.name@@"))
			.AppendFormat(",{0}", ReplaceTag("@@DispText.common_message.FirstOrderMonth@@"))
			.AppendFormat(
				",{0}",
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT,
					Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT_TITLE,
					Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT_NUMBER_PURCHASE));

		var addMonthNumber = (totalCount % DISPLAY_LTV_MAX_COUNT == 0)
			? totalCount
			: totalCount + (DISPLAY_LTV_MAX_COUNT - totalCount % DISPLAY_LTV_MAX_COUNT);
		var dateEnd = dateFrom.AddMonths(addMonthNumber - 1);
		for (var date = dateFrom; date <= dateEnd; date = date.AddMonths(1))
		{
			result.Append(",").AppendFormat(
				ReplaceTag("@@DispText.common_message.OrderDate@@"),
				date.Year,
				date.Month);
		}

		return result.ToString();
	}

	/// <summary>
	/// Create LTV contents record
	/// </summary>
	/// <param name="dv">データビュー</param>
	/// <param name="currentPageNumber">Current page number</param>
	/// <param name="totalCount">Total count</param>
	/// <returns>コンテンツ文字列</returns>
	private string CreateLtvContentsRecord(
		DataView dv,
		int currentPageNumber,
		int totalCount)
	{
		// LTVレポート
		var result = new StringBuilder();
		foreach (var variationItem in CreateDisplayLtvDataList(dv, currentPageNumber, totalCount, isCsvExport: true))
		{
			foreach (var rowData in variationItem.MonthData)
			{
				foreach (var ltvData in rowData.LtvData)
				{
					result.Append(StringUtility.EscapeCsvColumn(variationItem.ProductId))
						.AppendFormat(",{0}", StringUtility.EscapeCsvColumn(variationItem.VariationId))
						.AppendFormat(",{0}", StringUtility.EscapeCsvColumn(variationItem.ProductName))
						.Append(",").AppendFormat(
							ReplaceTag("@@DispText.common_message.OrderDate@@"),
							rowData.Year,
							rowData.Month)
						.AppendFormat(",{0}", StringUtility.EscapeCsvColumn(ltvData.TitleLtvReport));

					var details = StringUtility.CreateEscapedCsvString(ltvData.ContentsLtvData);
					result.AppendFormat(",{0}{1}", details, Environment.NewLine);
				}
			}
		}
		return result.ToString();
	}

	/// <summary>
	/// コンテンツレコード作成
	/// </summary>
	/// <returns>コンテンツ文字列</returns>
	private string CreateContentsRecord()
	{
		// 回数別レポート
		var result = new StringBuilder();
		foreach (var variationItem in this.DisplayDataList)
		{
			foreach (var rowData in variationItem.Data)
			{
				result.Append(StringUtility.EscapeCsvColumn(rowData.ProductId))
					.AppendFormat(",{0}", StringUtility.EscapeCsvColumn(rowData.VariationId))
					.AppendFormat(",{0}", StringUtility.EscapeCsvColumn(rowData.ProductName))
					.AppendFormat(",{0}", StringUtility.EscapeCsvColumn(rowData.Title));

				var details = StringUtility.CreateEscapedCsvString(rowData.ContentsData);
				result.AppendFormat(",{0}{1}", details, Environment.NewLine);
			}
		}

		return result.ToString();
	}

	/// <summary>
	/// 期間指定の条件をチェック
	/// </summary>
	/// <param name="paramDateFrom">開始日</param>
	/// <param name="paramDateTo">終了日</param>
	/// <returns>TRUE:適当な期間　FALSE：不正な期間</returns>
	private bool CheckDateSearch(out DateTime paramDateFrom, out DateTime paramDateTo)
	{
		var dateFrom = ucTargetPeriod.StartDateTimeString;
		var dateTo = ucTargetPeriod.EndDateTimeString;
		paramDateFrom = DateTime.Now;
		paramDateTo = DateTime.Now;
		if (Validator.IsDate(dateFrom) && Validator.IsDate(dateTo))
		{
			paramDateFrom = DateTime.Parse(dateFrom);
			paramDateTo = DateTime.Parse(dateTo);
			return true;
		}
		return false;
	}

	/// <summary>
	/// Get display LTV summary data value
	/// </summary>
	/// <param name="value">Value</param>
	/// <returns>Display LTV summary data value</returns>
	protected decimal GetDisplayLtvSummaryDataValue(decimal value)
	{
		// Remove decimal point when value is 0
		return (value == 0m) ? 0m : value;
	}

	/// <summary>
	/// Report kbn selected index changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblReportKbn_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		Response.Redirect(CreateDefaultReportListUrl());
	}

	/// <summary>
	/// 検索できる状態か？
	/// </summary>
	/// <returns>検索できる状態か</returns>
	private bool IsSeachCondition()
	{
		var result = ((string.IsNullOrEmpty(tbProductId.Text) == false)
			|| (string.IsNullOrEmpty(tbVariationId.Text) == false)
			|| (this.IsLtvReportKbn
				&& ((string.IsNullOrEmpty(ddlPaymentType.SelectedValue) == false)
					|| (string.IsNullOrEmpty(ddlSearchAdvMediaKbn.SelectedValue) == false)
					|| ((string.IsNullOrEmpty(ddlUserExtend.SelectedValue) == false)
					|| (string.IsNullOrEmpty(tbUserExtend.Text.Trim()) == false))
					|| (string.IsNullOrEmpty(tbSearchAdvCode.Text.Trim()) == false)
					|| (string.IsNullOrEmpty(ucTargetPeriod.StartDateTimeString) == false))));
		return result;
	}

	/// <summary>
	/// Check box LTV on checked changed
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void cbLtv_OnCheckedChanged(object sender, EventArgs e)
	{
		if (this.DisplayLtvSummaryData != null)
		{
			this.DisplayLtvSummaryData.Calculate(cbLtv.Checked);
		}
	}

	/// <summary>
	/// 実行時のタイマー処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void tProcessTimer_Tick(object sender, EventArgs e)
	{
		if (this.ProcessInfo == null)
		{
			tProcessTimer.Enabled = false;
			lProcessMessage.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PROCESSING_LOST_DATA));
			FileLogger.WriteInfo("定期継続分析レポート(LTVレポート)：LTVレポート情報は空になっています。");
			dvLoadingImg.Visible = false;
			lbLtvReportExport.Enabled = false;
			cbLtv.Visible = false;
			return;
		};

		if (this.ProcessInfo.IsSystemError)
		{
			tProcessTimer.Enabled = false;
			lProcessMessage.Text = WebSanitizer.HtmlEncodeChangeToBr(WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR)
				.Replace("<br />", Environment.NewLine));
			FileLogger.WriteInfo("定期継続分析レポート(LTVレポート)：システムエラーが発生しました。");
			dvLoadingImg.Visible = false;
			lbLtvReportExport.Enabled = false;
			cbLtv.Visible = false;
			return;
		}

		lProcessMessage.Text = WebSanitizer.HtmlEncodeChangeToBr(
			WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ELAPSED_TIME)
				.Replace("\\r\\n", "\r\n"))
				.Replace("@@ 1 @@", DateTime.Parse((DateTime.Now - this.ProcessInfo.StartTime).ToString()).ToString("mm:ss"));

		if (this.ProcessInfo.IsDone)
		{
			if (this.ProcessInfo.TotalCount == 0)
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST));
				cbLtv.Visible = false;
				lbLtvReportExport.Visible = false;
			}
			else
			{
				Session[Constants.SESSION_KEY_PARAM + "MP"] = new Hashtable
				{
					{ Constants.FIELD_TARGETLIST_TARGET_TYPE, Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_REPEAT_ANALYSIS_REPORT },
					{ Constants.TABLE_USER, (Hashtable)this.ProcessInfo.Param.Clone() },
				};
				Session[Constants.SESSIONPARAM_KEY_FIXED_PURCHASE_REPEAT_ANALYSIS_REPORT_SEARCH_INFO] = (Hashtable)this.ProcessInfo.Param.Clone();

				lbPager1.Text = WebPager.CreateReportLtvListPager(
					this.ProcessInfo.TotalCount,
					this.ProcessInfo.CurrentPageNumber,
					CreateLtvReportListUrl(),
					DISPLAY_LTV_MAX_COUNT);
				this.DisplayLtvSummaryData = CreateDisplayLtvSummaryData(this.ProcessInfo.DisplayLtvDataView);
				this.DisplayLtvDataList = CreateDisplayLtvDataList(
					this.ProcessInfo.DisplayLtvDataView,
					this.ProcessInfo.CurrentPageNumber,
					this.ProcessInfo.TotalCount);
				cbLtv.Visible = true;
				lbLtvReportExport.Visible = true;

				DataBind();
			}

			FileLogger.WriteInfo("処理完了");
			lProcessMessage.Text = string.Empty;
			tProcessTimer.Enabled = false;
			dvLoadingImg.Visible = false;
		}
	}

	#region プロパティ宣言
	/// <summary>レポート表示のデータ</summary>
	protected List<ReportData> DisplayDataList { get; set; }
	/// <summary>初期？</summary>
	protected bool IsFirst { get { return (Request.QueryString.AllKeys.Length <= 1); } }
	/// <summary>表示するか？</summary>
	protected bool ShowContents { get { return ((this.IsFirst == false) && IsSeachCondition() && (this.DisplayDataList != null) && (this.DisplayDataList.Count > 0)); } }
	/// <summary>Display LTV data list</summary>
	protected List<ReportLtvData> DisplayLtvDataList
	{
		get
		{
			return (List<ReportLtvData>)ViewState["display_ltv_data_list"];
		}
		set { ViewState["display_ltv_data_list"] = value; }
	}
	/// <summary>Display LTV summary data</summary>
	protected ReportLtvSummaryData DisplayLtvSummaryData
	{
		get { return (ReportLtvSummaryData)ViewState["display_ltv_summary_data"]; }
		set { ViewState["display_ltv_summary_data"] = value; }
	}
	/// <summary>Is show LTV contents</summary>
	protected bool IsShowLtvContents
	{
		get
		{
			var result = (this.DisplayLtvSummaryData != null)
				&& (this.DisplayLtvDataList != null)
				&& (this.DisplayLtvDataList.Count > 0);
			return result;
		}
	}
	/// <summary>Is LTV report kbn</summary>
	protected bool IsLtvReportKbn
	{
		get
		{
			return rblReportKbn.SelectedValue == Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DISPLAY_KBN_TYPE_LTV;
		}
	}
	/// <summary>Current year</summary>
	private int CurrentYear
	{
		get { return (int)ViewState[Constants.REQUEST_KEY_CURRENT_YEAR]; }
		set { ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] = value; }
	}
	/// <summary>Current month</summary>
	private int CurrentMonth
	{
		get { return (int)ViewState[Constants.REQUEST_KEY_CURRENT_MONTH]; }
		set { ViewState[Constants.REQUEST_KEY_CURRENT_MONTH] = value; }
	}
	/// <summary>ユーザー拡張項目リスト</summary>
	private UserExtendSettingList UserExtendSettings
	{
		get { return (UserExtendSettingList)ViewState["user_extend_setting_list"]; }
		set { ViewState["user_extend_setting_list"] = value; }
	}
	/// <summary>処理情報（非同期スレッドでもアクセス可能なようにWEBキャッシュ格納）</summary>
	public ProcessInfoType ProcessInfo
	{
		get
		{
			var val = this.Cache[this.ProcessInfoCacheKey];
			if ((val is ProcessInfoType) == false) val = null;
			return (ProcessInfoType)val;
		}
		set
		{
			if (value != null)
			{
				this.Cache.Insert(
					this.ProcessInfoCacheKey,
					value,
					null,
					System.Web.Caching.Cache.NoAbsoluteExpiration, // 絶対日付期限切れなし
					TimeSpan.FromMinutes(5)); // 5分キャッシュにアクセスが無かったら破棄する
			}
			else
			{
				this.Cache.Remove(this.ProcessInfoCacheKey);
			}
		}
	}
	/// <summary>処理件数情報キャッシュキー</summary>
	private string ProcessInfoCacheKey
	{
		get
		{
			return "ProcessInfo:" + Session.SessionID;
		}
	}
	#endregion

	#region 回数別レポート
	/// <summary>
	/// レポートデータクラス
	/// </summary>
	[Serializable]
	public class ReportData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReportData()
		{
			// 初期化
			this.CountData = new ReportCountData[DISPLAY_MAX_COUNT];
			for (var i = 0; i < DISPLAY_MAX_COUNT; i++)
			{
				this.CountData[i] = new ReportCountData();
			}
		}

		/// <summary>
		/// コンテンツデータ作成
		/// </summary>
		public void SetContents()
		{
			this.Data = new ReportRowData[7];
			for (var i = 0; i < 7; i++)
			{
				// 初期化
				this.Data[i] = new ReportRowData();
				var data = this.Data[i];

				// コンテンツ内容作成
				data.ReportDataIndex = this.Index;
				data.ProductId = this.ProductId;
				data.VariationId = this.VariationId;
				data.ProductName = this.ProductName;
				data.Title = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT,
					Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TITLE_FREQUENCY,
					i);

				for (var j = 0; j < DISPLAY_MAX_COUNT; j++)
				{
					switch (i)
					{
						// 継続顧客数
						case 0:
							data.ContentsData[j] = this.CountData[j].DeliveredCount.ToString();
							break;

						// 継続率
						case 1:
							// 今回継続顧客数 / ひとつ前の継続顧客数
							data.ContentsData[j] = (j == 0) ? "--" : string.Format("{0}%", Math.Round(((this.CountData[j - 1].DeliveredCount == 0) ? 0 : (decimal)this.CountData[j].DeliveredCount / (decimal)this.CountData[j - 1].DeliveredCount) * 100, 2));
							break;

						// 1回目からの残存率
						case 2:
							// 今回継続顧客数 / 1回目の継続顧客数
							data.ContentsData[j] = (j == 0) ? "--" : string.Format("{0}%", Math.Round(((this.CountData[0].DeliveredCount == 0) ? 0 : (decimal)this.CountData[j].DeliveredCount / (decimal)this.CountData[0].DeliveredCount) * 100, 2));
							break;

						// 未注文・未出荷
						case 3:
							data.ContentsData[j] = (this.CountData[j].OrderCount + this.CountData[j].PlanCount).ToString();
							break;

						// 離脱顧客数
						case 4:
							data.ContentsData[j] = this.CountData[j].FallOutCount.ToString();
							break;

						// 離脱率
						case 5:
							// 離脱顧客数 / 継続顧客数
							data.ContentsData[j] = (j == 0) ? "--" : string.Format("{0}%", Math.Round(((this.CountData[j].DeliveredCount == 0) ? 0 : (decimal)this.CountData[j].FallOutCount / (decimal)this.CountData[j].DeliveredCount) * 100, 2));
							break;

						// 1回目からの離脱率
						case 6:
							// 今回までの累計離脱顧客数 / 1回目の継続顧客数
							data.ContentsData[j] = (j == 0) ? "--" : string.Format("{0}%", Math.Round(((this.CountData[0].DeliveredCount == 0) ? 0 : (decimal)this.CountData.Where(item => item.Index <= j).Select(item => item.FallOutCount).Sum() / (decimal)this.CountData[0].DeliveredCount) * 100, 2));
							break;
					}
				}
			}
		}

		/// <summary>インデックス</summary>
		public int Index { get; set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>バリエーションID</summary>
		public string VariationId { get; set; }
		/// <summary>商品名</summary>
		public string ProductName { get; set; }
		/// <summary>カウントデータ</summary>
		public ReportCountData[] CountData { get; set; }
		/// <summary>コンテンツデータ</summary>
		public ReportRowData[] Data { get; set; }
	}

	/// <summary>
	/// カウントデータ
	/// </summary>
	[Serializable]
	public class ReportCountData
	{
		/// <summary>インデックス</summary>
		public int Index { get; set; }
		/// <summary>予定数</summary>
		public int PlanCount { get; set; }
		/// <summary>注文数</summary>
		public int OrderCount { get; set; }
		/// <summary>出荷数</summary>
		public int DeliveredCount { get; set; }
		/// <summary>離脱数</summary>
		public int FallOutCount { get; set; }
		/// <summary>Total price</summary>
		public decimal TotalPrice { get; set; }
		/// <summary>Fixed purchase cancel</summary>
		public int FixedPurchaseCancel { get; set; }
		/// <summary>Order date</summary>
		public DateTime OrderDate { get; set; }
	}

	/// <summary>
	/// 行データ
	/// </summary>
	[Serializable]
	public class ReportRowData
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ReportRowData()
		{
			// 初期化
			this.ContentsData = new string[DISPLAY_MAX_COUNT];
		}
		/// <summary>レポートデータインデックス</summary>
		public int ReportDataIndex { get; set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; set; }
		/// <summary>バリエーションID</summary>
		public string VariationId { get; set; }
		/// <summary>商品名</summary>
		public string ProductName { get; set; }
		/// <summary>タイトル</summary>
		public string Title { get; set; }
		/// <summary>コンテンツデータ</summary>
		public string[] ContentsData { get; set; }
	}
	#endregion

	#region LTVレポート
	/// <summary>
	/// Report LTV Summary Data
	/// </summary>
	[Serializable]
	public class ReportLtvSummaryData
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReportLtvSummaryData()
		{
			this.OrderPriceSubtotal = 0m;
			this.OrderCount = 0m;
			this.FixedPurchaseCancelationCount = 0m;
			this.FixedPurchaseCountTotal = 0m;
			this.MonthCount = 0m;
			this.ConsecutivePurchaseMonth = 0m;
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="orderPriceSubtotal">Order price subtotal</param>
		/// <param name="orderCounttotal">Order count total</param>
		/// <param name="monthCount">Month count</param>
		/// <param name="consecutivePurchaseMonth">Consecutive purchase month</param>
		/// <param name="fixedPurchaseCancelationCount">Fixed purchase cancelation count</param>
		/// <param name="fixedPurchaseCountTotal">Fixed purchase count total</param>
		/// <param name="isIncludedCancelation">Is included cancelation</param>
		public ReportLtvSummaryData(
			decimal orderPriceSubtotal,
			int orderCounttotal,
			int monthCount,
			int consecutivePurchaseMonth,
			int fixedPurchaseCancelationCount,
			int fixedPurchaseCountTotal,
			bool isIncludedCancelation)
		{
			this.OrderPriceSubtotal = orderPriceSubtotal;
			this.OrderCount = orderCounttotal;
			this.FixedPurchaseCancelationCount = fixedPurchaseCancelationCount;
			this.FixedPurchaseCountTotal = fixedPurchaseCountTotal;
			this.MonthCount = monthCount;
			this.ConsecutivePurchaseMonth = consecutivePurchaseMonth;
			Calculate(isIncludedCancelation);
		}

		/// <summary>
		/// Calculate
		/// </summary>
		/// <param name="isIncludedCancelation">Is included cancelation</param>
		public void Calculate(bool isIncludedCancelation)
		{
			this.AverageOrderPriceSubtotal = DecimalUtility.DecimalRound(
				this.OrderPriceSubtotal / ((this.OrderCount != 0) ? this.OrderCount : 1),
				DecimalUtility.Format.RoundUp,
				1);
			this.AverageOrderCount = DecimalUtility.DecimalRound(
				this.OrderCount / DISPLAY_LTV_MAX_COUNT,
				DecimalUtility.Format.RoundUp,
				2);
			this.CancelationRate = DecimalUtility.DecimalRound(
				(this.FixedPurchaseCancelationCount * 100) / ((this.FixedPurchaseCountTotal != 0) ? this.FixedPurchaseCountTotal : 1),
				DecimalUtility.Format.RoundUp,
				2);
			this.AverageConsecutivePurchaseMonth = (this.ConsecutivePurchaseMonth / ((this.MonthCount != 0) ? this.MonthCount : 1));
			this.Ltv = DecimalUtility.DecimalRound(
				(this.AverageOrderPriceSubtotal * this.AverageOrderCount * this.AverageConsecutivePurchaseMonth)
					* (isIncludedCancelation ? ((100 - this.CancelationRate) / 100) : 1),
				DecimalUtility.Format.RoundUp,
				1);
		}

		/// <summary>Order price subtotal</summary>
		public decimal OrderPriceSubtotal { get; set; }
		/// <summary>Order count</summary>
		public decimal OrderCount { get; set; }
		/// <summary>Month count</summary>
		public decimal MonthCount { get; set; }
		/// <summary>Consecutive purchase month</summary>
		public decimal ConsecutivePurchaseMonth { get; set; }
		/// <summary>Average consecutive purchase month</summary>
		public decimal AverageConsecutivePurchaseMonth { get; set; }
		/// <summary>Fixed purchase cancelation count</summary>
		public decimal FixedPurchaseCancelationCount { get; set; }
		/// <summary>Fixed purchase count total</summary>
		public decimal FixedPurchaseCountTotal { get; set; }
		/// <summary>Average order price subtotal</summary>
		public decimal AverageOrderPriceSubtotal { get; set; }
		/// <summary>Average order count</summary>
		public decimal AverageOrderCount { get; set; }
		/// <summary>Cancelation rate</summary>
		public decimal CancelationRate { get; set; }
		/// <summary>LTV</summary>
		public decimal Ltv { get; set; }
	}

	/// <summary>
	/// Report LTV data
	/// </summary>
	[Serializable]
	public class ReportLtvData
	{
		/// <summary>
		/// Constructor
		/// </summary>
		public ReportLtvData()
		{
			this.MonthData = new List<ReportMonthData>();
		}

		/// <summary>
		/// Set contents month data
		/// </summary>
		public void SetContentsMonthData()
		{
			foreach (var item in this.MonthData)
			{
				item.SetContents();
			}
		}

		/// <summary>Product id</summary>
		public string ProductId { get; set; }
		/// <summary>Variation id</summary>
		public string VariationId { get; set; }
		/// <summary>Product name</summary>
		public string ProductName { get; set; }
		/// <summary>Product image</summary>
		public Dictionary<string, string> ImageProduct { get; set; }
		/// <summary>Month data</summary>
		public List<ReportMonthData> MonthData { get; set; }
	}

	/// <summary>
	/// Report month data
	/// </summary>
	[Serializable]
	public class ReportMonthData
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="maxCount">Max count</param>
		public ReportMonthData(int maxCount)
		{
			this.CountData = new ReportCountData[maxCount];
			for (var i = 0; i < maxCount; i++)
			{
				this.CountData[i] = new ReportCountData
				{
					OrderCount = 0,
					TotalPrice = 0,
					FixedPurchaseCancel = 0,
				};
			}
		}

		/// <summary>
		/// Set contents
		/// </summary>
		public void SetContents()
		{
			this.LtvData = new ReportLtvRowData[4];
			for (var i = 0; i < 4; i++)
			{
				// 初期化
				var maxCount = this.CountData.Length;
				this.LtvData[i] = new ReportLtvRowData
				{
					TitleLtvReport = ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT,
						Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TITLE_LTV,
						i),
					ContentsLtvData = new string[maxCount],
				};

				// コンテンツ内容作成
				var data = this.LtvData[i];
				var count = 0;
				for (var j = 0; j < maxCount; j++)
				{
					count++;
					var dateFrom = new DateTime(
						this.Year,
						this.Month,
						DateTimeUtility.GetLastDayOfMonth(this.Year, this.Month));
					if ((dateFrom > this.CountData[j].OrderDate)
						&& (this.CountData[j].OrderDate != DateTime.MinValue))
					{
						data.ContentsLtvData[j] = string.Empty;
						continue;
					}

					switch (i)
					{
						// 受注件数
						case 0:
							data.ContentsLtvData[j] = this.CountData[j].OrderCount.ToString();
							break;

						// 離脱件数
						case 1:
							data.ContentsLtvData[j] = (dateFrom.Year == this.CountData[j].OrderDate.Year)
									&& (dateFrom.Month == this.CountData[j].OrderDate.Month)
									&& (this.CountData[j].FixedPurchaseCancel == 0)
								? "--"
								: this.CountData[j].FixedPurchaseCancel.ToString();
							break;

						// 受注金額
						case 2:
							data.ContentsLtvData[j] = this.CountData[j].TotalPrice.ToPriceString(true);
							break;

						// 残存率
						case 3:
							var remainingRate = 0m;
							if ((j != 0) && (this.CountData[j - 1].OrderCount != 0))
							{
								remainingRate = (decimal)this.CountData[j].OrderCount / this.CountData[j - 1].OrderCount * 100;
							}

							data.ContentsLtvData[j] = (dateFrom.Year == this.CountData[j].OrderDate.Year)
									&& (dateFrom.Month == this.CountData[j].OrderDate.Month)
									&& (remainingRate == 0)
								? "--"
								: string.Format("{0}%", Math.Abs(Math.Round(remainingRate, 0))).ToString();
							break;
					}
				}

				// Remove data of month 12 when switching to page 2 and above
				if ((maxCount == (DISPLAY_LTV_MAX_COUNT + 1)) && (count == (DISPLAY_LTV_MAX_COUNT + 1)))
				{
					var tempData = data.ContentsLtvData.ToList();
					tempData.RemoveAt(0);
					data.ContentsLtvData = tempData.ToArray();
				}
			}
		}

		/// <summary>Report LTV row data</summary>
		public ReportLtvRowData[] LtvData { get; set; }
		/// <summary>Report count data</summary>
		public ReportCountData[] CountData { get; set; }
		/// <summary>Month</summary>
		public int Month { get; set; }
		/// <summary>Year</summary>
		public int Year { get; set; }
	}

	/// <summary>
	/// Report LTV row data
	/// </summary>
	[Serializable]
	public class ReportLtvRowData
	{
		/// <summary>Title LTV report</summary>
		public string TitleLtvReport { get; set; }
		/// <summary>Contents LTV data</summary>
		public string[] ContentsLtvData { get; set; }
	}

	/// <summary>
	/// 処理情報（非同時処理とのやり取りに利用する）
	/// </summary>
	[Serializable]
	public class ProcessInfoType
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ProcessInfoType()
		{
			this.IsSystemError = false;
			this.IsDone = false;
			this.Param = new Hashtable();
			this.DisplayLtvDataView = new DataView();
			this.DisplayLtvSummaryDataView = new DataView();
			this.TotalCount = 0;
			this.CurrentPageNumber = 1;
			this.StartTime = new DateTime();
		}

		/// <summary>Display Ltv (DataView)</summary>
		public DataView DisplayLtvDataView { get; set; }
		/// <summary>Display Ltv Summary (DataView)</summary>
		public DataView DisplayLtvSummaryDataView { get; set; }
		/// <summary>システムエラー発生したか</summary>
		public bool IsSystemError { get; set; }
		/// <summary>処理完了したか</summary>
		public bool IsDone { get; set; }
		/// <summary>検索条件用のパラメタ</summary>
		public Hashtable Param { get; set; }
		/// <summary>実行結果件数</summary>
		public int TotalCount { get; set; }
		/// <summary>ページ番号</summary>
		public int CurrentPageNumber { get; set; }
		/// <summary>処理開始時間</summary>
		public DateTime StartTime { get; set; }
		/// <summary>Month data list</summary>
		public List<string> MonthDataList { get; set; }
	}
	#endregion
}
