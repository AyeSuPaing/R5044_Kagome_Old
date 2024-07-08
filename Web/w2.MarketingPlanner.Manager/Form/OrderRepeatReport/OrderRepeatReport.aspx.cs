/*
=========================================================================================================
  Module      : 定期回数別レポートページ処理(OrderRepeatReport.aspx.cs)
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
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Util;
using w2.Domain.Payment;
using w2.Domain.SubscriptionBox;

public partial class Form_OrderRepeatReport_OrderRepeatReport : BasePage
{
	#region 表示定数
	/// <summary> 期間指定数日 </summary>
	protected const string CONST_GET_ORDER_DAYS = "100";
	/// <summary> 売上基準のリクエストキー </summary>
	private const string REQUEST_KEY_SALES_TYPE = "stype";
	/// <summary> 注文基準 </summary>
	private const string SALES_TYPE_ORDER_REPORT = "1";
	/// <summary> 出荷基準 </summary>
	private const string SALES_TYPE_SHIP_REPORT = "0";
	/// <summary> レポート種別のリクエストキー </summary>
	private const string REQUEST_KEY_REPORT_TYPE = "rptype";
	/// <summary> 全体レポート </summary>
	private const string REPORT_TYPE_TOTAL = "0";
	/// <summary> 注文区分別レポート </summary>
	private const string REPORT_TYPE_DETAIL_BY_DEVICE_TYPE = "1";
	/// <summary> 決済区分別レポート </summary>
	private const string REPORT_TYPE_DETAIL_BY_PAYMENT_TYPE = "2";
	/// <summary> モールID（サイト）のリクエストキー </summary>
	private const string REQUEST_KEY_MALL_ID = "mlid";
	/// <summary> 広告コードのリクエストキー </summary>
	private const string REQUEST_KEY_ADVCODE = "advc";
	/// <summary> 注文種別項目名 </summary>
	protected const string FIELD_REPEATORDERREPORT_ORDER_DIVISION = "order_division";
	/// <summary> 注文種別項目名（表示用） </summary>
	protected const string FIELD_REPEATORDERREPORT_ORDER_DIVISION_TEXT = "order_division_text";
	/// <summary> 注文種別項目の説明文 </summary>
	protected const string FIELD_REPEATORDERREPORT_ORDER_DIVISION_TITLE = "order_division_title";
	/// <summary> 注文件数項目名 </summary>
	protected const string FIELD_REPEATORDERREPORT_ORDER_COUNT = "order_count";
	/// <summary> 割合 </summary>
	protected const string CONST_PERCENT = "_percent";
	/// <summary> 平均の定義 </summary>
	private const string CONST_AVERAGE = "average";
	/// <summary> 金額の定義 </summary>
	private const string CONST_AMOUNT = "amount";
	/// <summary> クラス </summary>
	protected const string CONST_CLASS = "class";
	/// <summary> 最初に項目表示かどうかの設定 </summary>
	protected const string CONST_DISPLAY_FIRST_TIME = "display_first_time";
	/// <summary> ボタン表示 </summary>
	protected const string CONST_DISPLAY_BUTTON = "display_button";
	/// <summary> ツールチップ表示 </summary>
	protected const string CONST_DISPLAY_TOOLTIP = "display_tooltip";
	/// <summary> HTMLで項目表示の指示 </summary>
	private const string STYLE_DISPLAY = "";
	/// <summary> HTMLで項目非表示の指示 </summary>
	private const string STYLE_UNDISPLAY = "display:none";
	/// <summary> 合計の種別 </summary>
	private const string CONST_TOTAL_LINE = "00_total";
	/// <summary> 定期注文2回目の種別 </summary>
	private const string CONST_FP_ORDER_2 = "05_fp_order2";
	/// <summary> 定期注文2回目以降の種別 </summary>
	private const string CONST_FP_ORDER_2UP = "24_fp_order2_up";
	/// <summary> リストアイテム背景のクラス名 </summary>
	private const string CLASS_LIST_ITEM_BACKGROUND = "list_item_bg";
	/// <summary> 決済リストの定義 </summary>
	private const string PAYMENT_LIST = "payment_list";
	/// <summary> 注文区分リストの定義 </summary>
	private const string ORDER_DEVICE_TYPE_LIST = "order_device_type_list";
	/// <summary> レポートデータ表示の設定
	/// x列目を最初に表示するか
	/// </summary>
	private static readonly string[] LINE_REPORT_DISPLAY_SETTING ={
		STYLE_DISPLAY,
		STYLE_DISPLAY,
		STYLE_DISPLAY,
		Constants.FIXEDPURCHASE_OPTION_ENABLED ? STYLE_DISPLAY : STYLE_UNDISPLAY,
		Constants.FIXEDPURCHASE_OPTION_ENABLED ? STYLE_DISPLAY : STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		STYLE_UNDISPLAY,
		Constants.FIXEDPURCHASE_OPTION_ENABLED ? STYLE_DISPLAY : STYLE_UNDISPLAY
	};

	// 抽出条件定数
	/// <summary> 決済種別(複数) </summary>
	private const string FIELD_REPEATORDER_ORDER_PAYMENT_DIVISION = "@@ order_payment_division @@";
	/// <summary> 注文区分(複数) </summary>
	private const string FIELD_REPEATORDER_ORDER_DEVICE_TYPE = "@@ order_device_type @@";
	/// <summary> 期間検索条件 </summary>
	private const string FIELD_REPEATORDER_DATE_SEARCH_CONDITION = "@@ date_search_condition @@";
	/// <summary> 広告コード検索条件 </summary>
	private const string FIELD_REPEATORDER_ADVCODE_SEARCH_CONDITION = "@@ advcode_search_condition @@";
	/// <summary> Regular type </summary>
	private const string REQUEST_KEY_REGULAR_TYPE = "rltype";
	/// <summary> すべての定期 </summary>
	protected const string REGULAR_TYPE_ALL = "all";
	/// <summary> 通常の定期 </summary>
	protected const string REGULAR_TYPE_FIXED_PURCHASE = "fixed_purchase";
	/// <summary> 頒布会の定期 </summary>
	protected const string REGULAR_TYPE_SUBSCRIPTION_BOX = "subscription_box";
	/// <summary> Subsciption box id </summary>
	private const string REQUEST_SUBSCRIPTION_BOX_ID = "sbid";
	
	#endregion

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		// イベントハンドラー追加
		ucReportDatePeriod.DateTimeSelectedEvent += ddlDateFromTo_SelectedIndexChanged;

		DateTime begin;
		DateTime end;
		CheckDateSearch(out begin, out end);
		if (!IsPostBack)
		{
			// コンポーネント初期化
			Initialize();
		}

			// 画面表示
			DisplayForm();
		}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
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

		var dateStart = string.Format("{0}/{1}/{2} {3}",
			dateStartYear,
			dateStartMonth,
			dateStartDay,
			string.IsNullOrEmpty(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CURRENT_TIME]))
				? "00:00:00"
				: StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_CURRENT_TIME]));
		var dateEnd = string.Format("{0}/{1}/{2} {3}",
			dateEndYear,
			dateEndMonth,
			dateEndDay,string.IsNullOrEmpty(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_TIME]))
				? "23:59:59"
				: StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_TIME]));

		DateTime currentDate;
		DateTime targetDate;
		if (DateTime.TryParse(dateStart, out currentDate)
			&& DateTime.TryParse(dateEnd, out targetDate))
		{
			ucReportDatePeriod.SetPeriodDate(currentDate, targetDate);
		}

		// 自社サイト
		ddlSiteName.Items.AddRange(ValueText.GetValueItemList("SiteName", "OwnSiteName").Cast<ListItem>().ToArray());
		// 各モール
		ddlSiteName.Items.AddRange(
			GetMallInfo().Cast<DataRowView>().Select(mall => new ListItem(CreateSiteNameForList(
				(string)mall[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID],
				(string)mall[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_NAME]),
				(string)mall[Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID]))
				.ToArray());

		if (Request.Params[REQUEST_KEY_MALL_ID] != null)
		{
			ddlSiteName.Items.Cast<ListItem>().ToList().ForEach(li => li.Selected = (li.Value == Request.Params[REQUEST_KEY_MALL_ID]));
		}
		var subscriptionBoxIds = new SubscriptionBoxService().GetAll();
		ddlSubscriptionBoxId.Items.AddRange(subscriptionBoxIds.Where(x => x.ValidFlg == Constants.FLG_SUBSCRIPTIONBOX_VALID_TRUE).Select(item => new ListItem(item.CourseId)).ToArray());

		if (Request.Params[REQUEST_SUBSCRIPTION_BOX_ID] != null)
		{
			ddlSubscriptionBoxId.Items.Cast<ListItem>().ToList().ForEach(li => li.Selected = (li.Value == Request.Params[REQUEST_SUBSCRIPTION_BOX_ID]));
		}

		// 売上基準項目を設定
		switch (Request[REQUEST_KEY_SALES_TYPE])
		{
			case SALES_TYPE_ORDER_REPORT:
			case SALES_TYPE_SHIP_REPORT:
				rblSalesType.Items.Cast<ListItem>().ToList().ForEach(li => li.Selected = (li.Value == Request[REQUEST_KEY_SALES_TYPE]));
				break;

			default:
				rblSalesType.Items.Cast<ListItem>().ToList().ForEach(li => li.Selected = (li.Value == SALES_TYPE_ORDER_REPORT));
				break;
		}

		// レポート種別項目を設定
		switch (Request[REQUEST_KEY_REPORT_TYPE])
		{
			case REPORT_TYPE_TOTAL:
			case REPORT_TYPE_DETAIL_BY_DEVICE_TYPE:
			case REPORT_TYPE_DETAIL_BY_PAYMENT_TYPE:
				rblReportType.Items.Cast<ListItem>().ToList().ForEach(li => li.Selected = (li.Value == Request[REQUEST_KEY_REPORT_TYPE]));
				break;

			default:
				rblReportType.Items.Cast<ListItem>().ToList().ForEach(li => li.Selected = (li.Value == REPORT_TYPE_TOTAL));
				break;
		}

		// 定期種別項目を設定
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_FIXEDPURCHASE, Constants.FIELD_FIXEDPURCHASE_REGULAR_STATUS))
		{
			rblRegularType.Items.Add(li);
		}
		if (rblRegularType.Items.Count > 0)
		{
			switch (Request[REQUEST_KEY_REGULAR_TYPE])
			{
				case REGULAR_TYPE_ALL:
					rblRegularType.Items[0].Selected = true;
					break;

				case REGULAR_TYPE_FIXED_PURCHASE:
					rblRegularType.Items[1].Selected = true;
					break;

				case REGULAR_TYPE_SUBSCRIPTION_BOX:
					rblRegularType.Items[2].Selected = true;
					break;

				default:
					rblRegularType.Items[0].Selected = true;
					break;
			}
		}

		// 広告コード項目を設定
		tbAdvCode.Text = string.IsNullOrEmpty(Request.Params[REQUEST_KEY_ADVCODE])
			? ""
			: string.Join(",", Request.Params.GetValues(REQUEST_KEY_ADVCODE));
	}

	/// <summary>
	/// モール情報取得
	/// </summary>
	/// <returns>モール情報</returns>
	private DataView GetMallInfo()
	{
		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("MallLiaise", "GetMallCooperationSettingListFromShopId"))
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, this.LoginOperatorShopId}
			};
			var dv = statement.SelectSingleStatement(accessor, ht);
			return dv;
		}
	}

	/// <summary>
	/// 画面表示
	/// </summary>
	private void DisplayForm()
	{
		// カレンダー設定
		SetCalendar();
		// レポートラベル作成
		CreateReportLabel();
		// 集計データ取得
		GetData();
		// データバイド
		DataBind();
	}

	/// <summary>
	/// カレンダー項目を設定
	/// </summary>
	private void SetCalendar()
	{
		var param = REQUEST_KEY_SALES_TYPE + "=" + rblSalesType.SelectedValue
			+ "&" + REQUEST_KEY_REPORT_TYPE + "=" + rblReportType.SelectedValue
			+ "&" + REQUEST_KEY_MALL_ID + "=" + ddlSiteName.SelectedValue
			+ "&" + REQUEST_KEY_REGULAR_TYPE + "=" + rblRegularType.SelectedValue
			+ "&" + REQUEST_SUBSCRIPTION_BOX_ID + "=" + ddlSubscriptionBoxId.SelectedValue
			+ "&" + string.Join("&", tbAdvCode.Text.Trim().Split(',').Select(adv =>
				string.Format("{0}={1}", REQUEST_KEY_ADVCODE, HttpUtility.UrlEncode(adv))));

		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(
			this.CurrentYear,
			this.CurrentMonth,
			Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ORDER_REPEAT_REPORT,
			param,
			Constants.REQUEST_KEY_CURRENT_YEAR,
			Constants.REQUEST_KEY_CURRENT_MONTH);
	}

	/// <summary>
	/// 定期回数別レポートURLを作成
	/// </summary>
	/// <returns>定期回数別レポートURL</returns>
	private string CreateRepeatOrderReportUrl()
	{
		var reportDatePeriodCurrent = DateTime.Parse(ucReportDatePeriod.StartDateTimeString);
		var reportDatePeriodTaget = DateTime.Parse(ucReportDatePeriod.EndDateTimeString);

		var url = Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ORDER_REPEAT_REPORT
			+ "?" + Constants.REQUEST_KEY_CURRENT_YEAR + "=" + reportDatePeriodCurrent.Year
			+ "&" + Constants.REQUEST_KEY_CURRENT_MONTH + "=" + reportDatePeriodCurrent.Month
			+ "&" + Constants.REQUEST_KEY_CURRENT_DAY + "=" + reportDatePeriodCurrent.Day
			+ "&" + Constants.REQUEST_KEY_TARGET_YEAR + "=" + reportDatePeriodTaget.Year
			+ "&" + Constants.REQUEST_KEY_TARGET_MONTH + "=" + reportDatePeriodTaget.Month
			+ "&" + Constants.REQUEST_KEY_TARGET_DAY + "=" + reportDatePeriodTaget.Day
			+ "&" + Constants.REQUEST_KEY_CURRENT_TIME + "=" + ucReportDatePeriod.HfStartTime.Value
			+ "&" + Constants.REQUEST_KEY_TARGET_TIME + "=" + ucReportDatePeriod.HfEndTime.Value
			+ "&" + REQUEST_KEY_SALES_TYPE + "=" + rblSalesType.SelectedValue
			+ "&" + REQUEST_KEY_REPORT_TYPE + "=" + rblReportType.SelectedValue
			+ "&" + REQUEST_KEY_REGULAR_TYPE + "=" + rblRegularType.SelectedValue
			+ "&" + REQUEST_KEY_MALL_ID + "=" + ddlSiteName.SelectedValue
			+ "&" + REQUEST_SUBSCRIPTION_BOX_ID + "=" + ddlSubscriptionBoxId.SelectedValue
			+ "&" + string.Join("&", tbAdvCode.Text.Trim().Split(',').Select(adv =>
				string.Format("{0}={1}", REQUEST_KEY_ADVCODE, HttpUtility.UrlEncode(adv))));
		return url;
	}

	/// <summary>
	/// レポートラベル作成
	/// </summary>
	private void CreateReportLabel()
	{
		spSiteName.Visible = false;
		spAdvCode.Visible = false;

		// 全体サイト以外、レポートタイトルに選択したサイト情報を表示
		if (string.IsNullOrEmpty(ddlSiteName.SelectedValue) == false) spSiteName.Visible = true;
		// 広告コードがあれば、レポートタイトルに表示
		if (string.IsNullOrEmpty(tbAdvCode.Text.Trim()) == false) spAdvCode.Visible = true;
	}

	/// <summary>
	/// 期間指定の条件をチェック
	/// </summary>
	/// <param name="paramDateFrom">開始日</param>
	/// <param name="paramDateTo">終了日</param>
	/// <returns>TRUE:適当な期間　FALSE：不正な期間</returns>
	private bool CheckDateSearch(out DateTime paramDateFrom, out DateTime paramDateTo)
	{
		var dateFrom = ucReportDatePeriod.StartDateTimeString;
		var dateTo = ucReportDatePeriod.EndDateTimeString;
		paramDateFrom = new DateTime();
		paramDateTo = new DateTime();
		var message = string.Empty;
		var displayError = false;
		if (Validator.IsDate(dateFrom) && Validator.IsDate(dateTo))
		{
			paramDateFrom = DateTime.Parse(dateFrom);
			paramDateTo = DateTime.Parse(dateTo);

			// From-Toが CONST_GET_ORDER_DAYS より大きかった場合はエラー表示
			var subtractDate = paramDateTo.Subtract(paramDateFrom);
			if (subtractDate.Days > int.Parse(CONST_GET_ORDER_DAYS))
			{
				displayError = true;
				message = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_REPEAT_REPORT_OUT_OF_RANGE)
					.Replace("@@ 1 @@", CONST_GET_ORDER_DAYS);
			}
			else if (subtractDate.Days < 0)
			{
				displayError = true;
				message = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_REPEAT_REPORT_INCORRECT_DATE);
			}
		}
		else
		{
			displayError = true;
			message = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDER_REPEAT_REPORT_INVALID_DATE);
		}

		// エラーメッセージ表示の制御
		lblConditionError.Visible = displayError;
		lblConditionError.Text = message;

		return (lblConditionError.Visible == false);
	}

	/// <summary>
	/// データ取得・加工
	/// </summary>
	protected void GetData()
	{
		// 表示データリストの宣言
		List<Hashtable> reportData = null;

		// 期間指定のエラーがない場合のみDBからデータ集計する。エラーの場合、レポートに「0」で表示する
		DateTime dateFrom, dateTo;
		if (CheckDateSearch(out dateFrom, out dateTo))
		{
			var ht = new Hashtable
			{
				{"date_from", dateFrom},
				{"date_to", dateTo},
				{Constants.FIELD_ORDER_MALL_ID, ddlSiteName.SelectedValue},
				{"regular_type", (rblRegularType.SelectedValue != null) ? rblRegularType.SelectedValue : REGULAR_TYPE_ALL},
				{Constants.FIELD_ORDER_SUBSCRIPTION_BOX_COURSE_ID, (ddlSubscriptionBoxId.SelectedValue != null) ? ddlSubscriptionBoxId.SelectedValue : ""}
			};

			// SQL実行
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("OrderRepeatReport", "GetOrderRepeatData"))
			{
				// 購買区分を置換
				statement.ReplaceStatement(FIELD_REPEATORDER_ORDER_DEVICE_TYPE, GetListItemValues(this.OrderDeviceTypes));
				// 決済種別を置換
				statement.ReplaceStatement(FIELD_REPEATORDER_ORDER_PAYMENT_DIVISION, GetListItemValues(this.PaymentTypes));
				// 広告コード検索条件を変換
				statement.ReplaceStatement(FIELD_REPEATORDER_ADVCODE_SEARCH_CONDITION, CreateAdvCodeSearchCondition());
				// 売上基準についての条件を変換
				statement.ReplaceStatement(FIELD_REPEATORDER_DATE_SEARCH_CONDITION, CreateDateSearchCondition());

				// データ取得
				var ds = statement.SelectStatementWithOC(accessor, ht);
				var mainData = ds.Tables[0].DefaultView;
				var detailOrderDeviceType = ds.Tables[1].DefaultView;
				var detailPayment = ds.Tables[2].DefaultView;

				// 表示データリスト作成
				if ((mainData.Count > 0) && (detailOrderDeviceType.Count > 0) && (detailPayment.Count > 0))
				{
					reportData = CreateDisplayDataList(mainData, detailOrderDeviceType, detailPayment);
				}
			}
		}

		// レポート表示データリストに入れる
		this.DisplayDataList = reportData ?? InitDisplayDataList();
	}

	/// <summary>
	/// 展開ボタンがあるレポート行のか
	/// </summary>
	/// <param name="orderDivision">レポート行の定義</param>
	/// <returns>TRUE：ボタンがある　FALSE：ボタンがない</returns>
	/// <remarks>定期2回目と定期2回目以降の行だけに展開ボタンを付ける</remarks>
	private bool HasExpandButtonLine(string orderDivision)
	{
		return ((orderDivision == CONST_FP_ORDER_2) || (orderDivision == CONST_FP_ORDER_2UP));
	}

	/// <summary>
	/// 表示データリストの初期化
	/// </summary>
	/// <returns>表示データリスト</returns>
	private List<Hashtable> InitDisplayDataList()
	{
		var reportData = new List<Hashtable>();
		for (int idx = 0; idx < LINE_REPORT_DISPLAY_SETTING.GetLength(0); idx++)
		{
			var reportRow = new Hashtable
			{
				{ FIELD_REPEATORDERREPORT_ORDER_DIVISION,
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT,
						Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT_ORDER_TYPE_NUMBER,
						idx) },
				{ FIELD_REPEATORDERREPORT_ORDER_DIVISION_TEXT,
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT,
						Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT_ORDER_TYPE_TEXT,
						idx) },
				{ FIELD_REPEATORDERREPORT_ORDER_DIVISION_TITLE,
					ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT,
						Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT_TOOL_TIP,
						idx) },
				{ CONST_CLASS, CLASS_LIST_ITEM_BACKGROUND + ((idx == 24) ? 2 : (idx % 2 + 1)) },
				{ CONST_DISPLAY_FIRST_TIME, LINE_REPORT_DISPLAY_SETTING[idx] },
				{ CONST_DISPLAY_BUTTON,
					HasExpandButtonLine(
						ValueText.GetValueText(
							Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT,
							Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT_ORDER_TYPE_NUMBER,
							idx))
						? STYLE_DISPLAY
						: STYLE_UNDISPLAY },
				{ CONST_DISPLAY_TOOLTIP,
					(ValueText.GetValueText(
						Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT,
						Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT_TOOL_TIP,
						idx) == "")
					? STYLE_UNDISPLAY
					: STYLE_DISPLAY }
			};

			// 注文件数、金額、商品数などのカラムを初期化する
			foreach (ListItem col in this.ReportColumnFields)
			{
				// メインデータ
				reportRow.Add(col.Value, 0);
				// 割合
				reportRow.Add(col.Value + CONST_PERCENT, CalculatePercent(col.Value, 0, 0));

				// 注文区分別または決済種別別レポートの場合、詳細カラムを追加
				if (this.HasExpandedColumn)
				{
					InitDetailData(col.Value, this.IsExpandedByDeviceType ? this.OrderDeviceTypes : this.PaymentTypes)
						.Cast<DictionaryEntry>().ToList().ForEach(item => reportRow.Add(item.Key, item.Value));
				}
			}
			reportData.Add(reportRow);
		}

		return reportData;
	}

	/// <summary>
	/// 詳細データを初期化
	/// </summary>
	/// <param name="field">カラム名</param>
	/// <param name="detailTypes">詳細区分</param>
	/// <returns>詳細データのHashtable</returns>
	private Hashtable InitDetailData(string field, List<ListItem> detailTypes)
	{
		var ht = new Hashtable();
		detailTypes.ForEach(li =>
			{
				ht.Add(field + "_" + li.Value, 0);
				ht.Add(field + CONST_PERCENT + "_" + li.Value, CalculatePercent(field, 0, 0));
			});
		return ht;
	}

	/// <summary>
	/// 詳細データを作成
	/// </summary>
	/// <param name="field">カラム名</param>
	/// <param name="detailData">詳細データ</param>
	/// <param name="totalLine">合計行</param>
	/// <param name="isTotalLine">処理している行が合計のか</param>
	/// <returns>詳細データのHashtable</returns>
	private Hashtable CreateDetailData(string field, DataView detailData, Hashtable totalLine, bool isTotalLine)
	{
		var ht = new Hashtable();
		foreach (DataRowView detail in detailData)
		{
			// 詳細区分取得
			var type = (string)detail[this.IsExpandedByDeviceType ? Constants.FIELD_ORDER_ORDER_KBN : Constants.FIELD_ORDER_ORDER_PAYMENT_KBN];
			// 詳細カラム名作成
			var detailField = field + "_" + type;
			// 詳細データ追加
			ht.Add(detailField, detail[field]);
			// 割合追加
			ht.Add(field + CONST_PERCENT + "_" + type,
				CalculatePercent(field, isTotalLine ? ht[detailField] : totalLine[detailField], ht[detailField]));
		}

		return ht;
	}

	/// <summary>
	/// 表示データリスト作成
	/// </summary>
	/// <param name="mainData">集計データ</param>
	/// <param name="detailOrderDeviceType">注文区分別の集計データ</param>
	/// <param name="detailPayment">決済種別の集計データ</param>
	/// <returns>表示データリスト</returns>
	private List<Hashtable> CreateDisplayDataList(DataView mainData, DataView detailOrderDeviceType, DataView detailPayment)
	{
		// 表示データリストの初期化
		var reportData = InitDisplayDataList();

		foreach (DataRowView drv in mainData)
		{
			foreach (var row in reportData)
			{
				// 不該当なレポート行の場合、Skipする
				if ((string)drv[FIELD_REPEATORDERREPORT_ORDER_DIVISION] != (string)row[FIELD_REPEATORDERREPORT_ORDER_DIVISION]) continue;

				var filter = string.Format("{0}='{1}'", FIELD_REPEATORDERREPORT_ORDER_DIVISION, (string)drv[FIELD_REPEATORDERREPORT_ORDER_DIVISION]);
				detailOrderDeviceType.RowFilter = filter;
				detailPayment.RowFilter = filter;

				// 該当なレポート行にデータ更新
				foreach (ListItem col in this.ReportColumnFields)
				{
					var field = col.Value;
					row[field] = (field.Contains(CONST_AMOUNT)) ? drv[field].ToPriceString() : drv[field];
					row[field + CONST_PERCENT] = CalculatePercent(field, reportData[0][field], row[field]);

					// 注文区分別または決済種別別レポートの場合、詳細データを更新
					if (this.HasExpandedColumn)
					{
						var isTotalLine = ((string)row[FIELD_REPEATORDERREPORT_ORDER_DIVISION] == CONST_TOTAL_LINE);
						CreateDetailData(field, this.IsExpandedByDeviceType ? detailOrderDeviceType : detailPayment, reportData[0], isTotalLine)
							.Cast<DictionaryEntry>().ToList().ForEach(item => row[item.Key] = item.Value);
					}
				}

				// クリアフィルター
				detailOrderDeviceType.RowFilter = "";
				detailPayment.RowFilter = "";
				break;
			}
		}

		return reportData;
	}

	/// <summary>
	/// 単位付ける数字に変換
	/// </summary>
	/// <param name="field">カラム名</param>
	/// <param name="value">値</param>
	/// <returns>単位付けた値</returns>
	protected string ConvertValueWithUnit(string field, object value)
	{
		// 金額の場合、「￥999,999」のフォマートで戻す
		if (field.Contains(CONST_AMOUNT)) return value.ToPriceString(true);

		// 注文件数・商品個数の場合、「999,999件」または「999,999個」のフォマートで戻す
		var result = string.Format(
			field.Contains(FIELD_REPEATORDERREPORT_ORDER_COUNT)
				? ReplaceTag("@@DispText.common_message.unit_of_quantity@@")
				: ReplaceTag("@@DispText.common_message.unit_of_quantity2@@"),
			StringUtility.ToNumeric(value));
		return result;
	}

	/// <summary>
	/// 割合計算
	/// </summary>
	/// <param name="field">カラム名</param>
	/// <param name="total">合計値</param>
	/// <param name="value">値</param>
	/// <returns>計算された割合ストリング：（〇〇％）</returns>
	/// <remarks>割合は四捨五入で、小数点2桁をします</remarks>
	private string CalculatePercent(string field, object total, object value)
	{
		//平均の項目には割合を計算しない。 
		if (field.Contains(CONST_AVERAGE)) return "";

		var totalValue = Convert.ToDecimal(total);
		var result = string.Format(
			"（{0}％）",
			StringUtility.ToNumeric((totalValue > 0) ? Math.Round(Convert.ToDecimal(value) * 100 / totalValue, 2).ToString() : "0"));
		return result;
	}

	/// <summary>
	/// リストアイテムの値を取得
	/// </summary>
	/// <param name="list">リスト元</param>
	/// <returns>リストアイテムの値</returns>
	private string GetListItemValues(List<ListItem> list)
	{
		var values = string.Join(",", list.Select(li => string.Format("'{0}'", li.Value.Replace("'", "''"))));
		return string.IsNullOrEmpty(values) ? "''" : values;
	}

	/// <summary>
	/// 広告コードでの検索条件作成
	/// </summary>
	/// <returns>条件内容</returns>
	private string CreateAdvCodeSearchCondition()
	{
		if (string.IsNullOrEmpty(tbAdvCode.Text.Trim())) return "";

		var advCode = string.Join(",",
			tbAdvCode.Text.Trim().Split(',').Select(adv => string.Format("'{0}'", adv.Replace("'", "''"))));

		var result = string.Format("AND  (w2_Order.advcode_first IN ({0}) OR w2_Order.advcode_new IN ({0}))", advCode);
		return result;
	}

	/// <summary>
	/// 期間指定検索条件を作成
	/// </summary>
	/// <returns>条件内容</returns>
	private string CreateDateSearchCondition()
	{
		// 注文基準・出荷基準で検索条件を作成
		var result = (this.IsSaleTypeOrder)
			? "w2_Order.order_date >= @date_from AND w2_Order.order_date < @date_to"
			: "((w2_Order.order_shipped_date IS NOT NULL  AND  w2_Order.order_shipped_date >= @date_from  AND  w2_Order.order_shipped_date < @date_to)"
				+ "  OR  (w2_Order.order_shipped_date IS NULL  AND  w2_Order.order_delivering_date >= @date_from  AND  w2_Order.order_delivering_date < @date_to))";
		return result;
	}

	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, System.EventArgs e)
	{
		// レポートデータ取得
		GetData();

		// タイトル作成
		var titleParams = new List<string>
		{
			string.Format(
				// 「定期回数別レポート {0}」
				ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT,
					Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT_TITLE,
					Constants.VALUETEXT_PARAM_ORDER_REPEAT_REPORT_REPORT_REGULAR_FREQUENCY),
				hdReportInfo.Value.Trim())
		};
		var records = CreateRecordCsv(titleParams) + "\r\n";

		// ヘッダ作成
		records += CreateReportHeaderCsv();
		// データ作成
		records += CreateReportDataCsv();
		// ファイル名作成
		var fileName = "RepeatOrderReport_" + DateTime.Now.ToString("yyyyMMddHHmm");
		// ファイル出力
		OutPutFileCsv(fileName, records);
	}

	/// <summary>
	/// CSVファイルのヘッダ作成
	/// </summary>
	/// <returns>CSVファイルのヘッダ</returns>
	private string CreateReportHeaderCsv()
	{
		var headerParams = new List<string> { "" };
		foreach (ListItem col in this.ReportColumnFields)
		{
			// カラム名取得
			var field = col.Value;
			var fieldText = col.Text;
			// メインカラムのヘッダを出力
			headerParams.Add(fieldText);
			// 平均カラムではない場合、割合カラムのヘッダを出力
			if (field.Contains(CONST_AVERAGE) == false) headerParams.Add(fieldText + "（％）");

			// 注文区分別または決済種別別レポートの場合、詳細データのヘッダを出力
			if (this.HasExpandedColumn)
			{
				headerParams.AddRange(ExportDetailHeaders(field, fieldText, this.IsExpandedByDeviceType ? this.OrderDeviceTypes : this.PaymentTypes));
			}
		}

		var result = CreateRecordCsv(headerParams);
		return result;
	}

	/// <summary>
	/// 詳細カラムのヘッダを出力
	/// </summary>
	/// <param name="field">カラム名</param>
	/// <param name="fieldText">表示用のカラム名</param>
	/// <param name="detailTypes">詳細区分</param>
	/// <returns>詳細カラムのヘッダ</returns>
	private List<string> ExportDetailHeaders(string field, string fieldText, List<ListItem> detailTypes)
	{
		var list = new List<string>();
		detailTypes.ForEach(li =>
			{
				list.Add(string.Format("{0}【{1}】", fieldText, li.Text));
				// 平均カラムではない場合、割合カラムのヘッダを出力する
				if (field.Contains(CONST_AVERAGE) == false) list.Add(string.Format("{0}【{1}】（％）", fieldText, li.Text));
			});
		return list;
	}

	/// <summary>
	/// CSVファイルのデータ作成
	/// </summary>
	/// <returns>CSVファイルのデータ</returns>
	private string CreateReportDataCsv()
	{
		var result = new StringBuilder();
		foreach (var row in this.DisplayDataList)
		{
			// 定期2回目以降のデータを出力しない
			if ((string)row[FIELD_REPEATORDERREPORT_ORDER_DIVISION] == CONST_FP_ORDER_2UP) continue;

			var dataParams = new List<string>();
			dataParams.Add((string)row[FIELD_REPEATORDERREPORT_ORDER_DIVISION_TEXT]);
			foreach (ListItem col in this.ReportColumnFields)
			{
				// カラム名取得
				var field = col.Value;
				// メインデータを出力
				dataParams.Add(row[field].ToString());
				//平均カラムではない場合、 割合のデータを出力
				if (field.Contains(CONST_AVERAGE) == false) dataParams.Add(row[field + CONST_PERCENT].ToString());

				// 注文区分別または決済種別別レポートの場合、詳細データを出力
				if (this.HasExpandedColumn)
				{
					dataParams.AddRange(ExportDetailData(row, field, this.IsExpandedByDeviceType ? this.OrderDeviceTypes : this.PaymentTypes));
				}
			}

			result.Append(CreateRecordCsv(dataParams).Replace("（", "").Replace("％", "").Replace("）", ""));
		}

		return result.ToString();
	}

	/// <summary>
	/// 詳細データを出力
	/// </summary>
	/// <param name="currentLine">カレント行</param>
	/// <param name="field">カラム名</param>
	/// <param name="detailTypes">詳細区分</param>
	/// <returns>詳細データ</returns>
	private List<string> ExportDetailData(Hashtable currentLine, string field, List<ListItem> detailTypes)
	{
		var list = new List<string>();
		detailTypes.ForEach(li =>
			{
				list.Add(currentLine[field + "_" + li.Value].ToString());
				// 平均カラムではない場合、割合カラムのデータを出力する
				if (field.Contains(CONST_AVERAGE) == false) list.Add(currentLine[field + CONST_PERCENT + "_" + li.Value].ToString());
			});
		return list;
	}

	/// <summary>
	/// 売上基準を選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblSalesType_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 条件を変更時、URLも変更して、リダイレクト
		var url = CreateRepeatOrderReportUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// レポート種別を選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblReportType_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 条件を変更時、URLも変更して、リダイレクト
		var url = CreateRepeatOrderReportUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// Regular type change 
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void rblRegularType_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 条件を変更時、URLも変更して、リダイレクト
		var url = CreateRepeatOrderReportUrl();
		Response.Redirect(url);
	}	

	/// <summary>
	/// 期間指定の各ドロップダウンリストを選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlDateFromTo_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 条件を変更時、URLも変更して、リダイレクト
		var url = CreateRepeatOrderReportUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// サイトを選択
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlSiteName_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 条件を変更時、URLも変更して、リダイレクト
		var url = CreateRepeatOrderReportUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// 広告コードを入力したイベント
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void tbAdvCode_TextChanged(object sender, EventArgs e)
	{
		// 条件を変更時、URLも変更して、リダイレクト
		var url = CreateRepeatOrderReportUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// Subscription box id change
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void ddlSubscriptionBoxId_SelectedIndexChanged(object sender, EventArgs e)
	{
		// 条件を変更時、URLも変更して、リダイレクト
		var url = CreateRepeatOrderReportUrl();
		Response.Redirect(url);
	}
	#region プロパティ宣言
	/// <summary> 決済種別リスト </summary>
	protected List<ListItem> PaymentTypes
	{
		get
		{
			return m_paymentTypes
				?? (m_paymentTypes = new PaymentService().GetValidAll(this.LoginOperatorShopId)
					.Select(payment => new ListItem(payment.PaymentName, payment.PaymentId))
					.ToList());
		}
	}
	private List<ListItem> m_paymentTypes = null;
	/// <summary> 注文区分リスト </summary>
	protected List<ListItem> OrderDeviceTypes
	{
		get
		{
			return m_orderDeviceTypes
				?? (m_orderDeviceTypes = ValueText.GetValueItemList(Constants.TABLE_ORDER, Constants.FIELD_ORDER_ORDER_KBN)
					.Cast<ListItem>()
					.ToList());
		}
	}
	private List<ListItem> m_orderDeviceTypes = null;
	/// <summary> 注文基準か </summary>
	protected bool IsSaleTypeOrder { get { return (rblSalesType.SelectedItem.Value == SALES_TYPE_ORDER_REPORT); } }
	/// <summary> レポートカラム項目リスト </summary>
	protected ListItemCollection ReportColumnFields
	{
		get { return m_reportColumns ?? (m_reportColumns = ValueText.GetValueItemList("RepeatOrderReport", "report_column_fields")); }
	}
	private ListItemCollection m_reportColumns = null;
	/// <summary> 購買区分別での詳細表示フラグ </summary>
	protected bool IsExpandedByDeviceType { get { return rblReportType.SelectedValue == REPORT_TYPE_DETAIL_BY_DEVICE_TYPE; } }
	/// <summary> 決済種別での詳細表示フラグ </summary>
	protected bool IsExpandedByPayment { get { return rblReportType.SelectedValue == REPORT_TYPE_DETAIL_BY_PAYMENT_TYPE; } }
	/// <summary> 詳細カラムの開ける数 </summary>
	protected int ColumnSpanCount
	{
		get { return this.IsExpandedByDeviceType ? (this.OrderDeviceTypes.Count + 1) : (this.IsExpandedByPayment ? (this.PaymentTypes.Count + 1) : 1); }
	}
	/// <summary> 詳細カラムが開けるのか </summary>
	protected bool HasExpandedColumn { get { return (this.IsExpandedByDeviceType || this.IsExpandedByPayment); } }
	/// <summary>レポート表示のデータ</summary>
	protected List<Hashtable> DisplayDataList { get; set; }
	/// <summary> 現在の年 </summary>
	private int CurrentYear
	{
		get { return (int)ViewState[Constants.REQUEST_KEY_CURRENT_YEAR]; }
		set { ViewState[Constants.REQUEST_KEY_CURRENT_YEAR] = value; }
	}
	/// <summary> 現在の月 </summary>
	private int CurrentMonth
	{
		get { return (int)ViewState[Constants.REQUEST_KEY_CURRENT_MONTH]; }
		set { ViewState[Constants.REQUEST_KEY_CURRENT_MONTH] = value; }
	}
	/// <summary>開始日時</summary>
	protected string DateFrom
	{
		get
		{
			return DateTimeUtility.ToStringForManager(
				ucReportDatePeriod.HfStartDate.Value,
				DateTimeUtility.FormatType.LongDate2Letter);
		}
	}
	/// <summary>終了日時</summary>
	protected string DateTo
	{
		get
		{
			return DateTimeUtility.ToStringForManager(
				ucReportDatePeriod.HfEndDate.Value,
				DateTimeUtility.FormatType.LongDate2Letter);
		}
	}
	#endregion
}
