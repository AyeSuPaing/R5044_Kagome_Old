/*
=========================================================================================================
  Module      : 商品別販売数ランキングページ(ProductSaleRankingReport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.Common.Logger;

public partial class Form_OrderConditionReport_ProductSaleRankingReport : BasePage
{
	/// <summary>ページング数</summary>
	const int DISP_PRODUCT_SALE_RANKING_REPORT_LIST = 100;
	/// <summary>商品セール検索条件</summary>
	private const string FIELD_ORDERCONDITION_PRODUCT_SALE_SEARCH_CONDITION = "@@ product_sale @@";
	/// <summary>商品セール区分値：通常</summary>
	private const string VALUETEXT_PARAM_PRODUCTSALE_KBN_NOMAL = "NM";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{        
        if (!IsPostBack)
		{
            //------------------------------------------------------
            // 売上状況レポート検索条件引継
            //------------------------------------------------------
            Hashtable param = GetSearchParameters();
            rblDisplayUnit.SelectedValue = StringUtility.ToEmpty(this.SessionParameter[Constants.REQUEST_KEY_PRODUCT_UNIT_TYPE]);

			//------------------------------------------------------
			// 集計期間表示文言作成
			//------------------------------------------------------
			lTotalPeriod.Text = CreateDisplayTotalPeriod(param);

			//------------------------------------------------------
			// 商品別販売数集計
			//------------------------------------------------------
			CalculateCountByProduct(param);
		}
	}

	/// <summary>
	/// 商品別販売数集計
	/// </summary>
	/// <param name="param">検索条件用のパラメタ</param>
	private void CalculateCountByProduct(Hashtable param)
	{
		this.ProcessInfo = new ProcessInfoType();
		this.ProcessInfo.TotalCount = 0;
		this.ProcessInfo.CurrentPageNumber = 0;
		this.ProcessInfo.OrderProductList = new DataView();
		tProcessTimer.Enabled = true;

		Task.Run(
			() =>
			{
				this.ProcessInfo.StartTime = DateTime.Now;
				try
				{
					// 総件数取得
					this.ProcessInfo.TotalCount = ActionSqlStatement("GetProductSaleRankingReportCount", param).Count;
					if (this.ProcessInfo.TotalCount == 0)
					{
						this.ProcessInfo.IsDone = true;
						return;
					}

					this.ProcessInfo.CurrentPageNumber = (int)param[Constants.REQUEST_KEY_PAGE_NO];
					param["bgn_row_num"] = DISP_PRODUCT_SALE_RANKING_REPORT_LIST * (this.ProcessInfo.CurrentPageNumber - 1) + 1;
					param["end_row_num"] = DISP_PRODUCT_SALE_RANKING_REPORT_LIST * this.ProcessInfo.CurrentPageNumber;
					this.ProcessInfo.OrderProductList = ActionSqlStatement("GetProductSaleRankingReport", param);
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
	/// 実行時のタイマー処理
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void tProcessTimer_Tick(object sender, EventArgs e)
	{
		if (this.ProcessInfo == null)
		{
			tProcessTimer.Enabled = false;
			lProcessMessage.Text = WebSanitizer.HtmlEncode("処理情報が失われました。実行中の可能性があります。");
			FileLogger.WriteInfo("商品別販売数ランキング：商品別販売数取得処理情報は空になっています。");
			dvLoadingImg.Visible = false;
			lbReportExport.Enabled = false;
			return;
		};
		if (this.ProcessInfo.IsSystemError)
		{
			tProcessTimer.Enabled = false;
			lProcessMessage.Text = WebSanitizer.HtmlEncode(WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR));
			FileLogger.WriteInfo("商品別販売数ランキング：システムエラーが発生しました。");
			lbReportExport.Enabled = false;
			dvLoadingImg.Visible = false;
			return;
		}

		lProcessMessage.Text = WebSanitizer.HtmlEncodeChangeToBr(WebMessages
			.GetMessages(WebMessages.ERRMSG_MANAGER_ELAPSED_TIME)
			.Replace("@@ 1 @@", DateTime.Parse((DateTime.Now - this.ProcessInfo.StartTime).ToString()).ToString("mm:ss")));

		if (this.ProcessInfo.IsDone)
		{
			if (this.ProcessInfo.TotalCount == 0)
			{
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}
			else
			{
				// データバインド
				this.OrderProductList = this.ProcessInfo.OrderProductList; // todo dvDetail.ToHashtableList();

				//------------------------------------------------------
				// ページャ作成（一覧処理で総件数を取得）
				//------------------------------------------------------
				lbPager1.Text = WebPager.CreateListPager(
					this.ProcessInfo.TotalCount,
					this.ProcessInfo.CurrentPageNumber,
					CreateProductSaleRankingReportUrl(this.ProcessInfo.Param, null),
					DISP_PRODUCT_SALE_RANKING_REPORT_LIST);

				DataBind();
			}

			FileLogger.WriteInfo("処理完了");
			lProcessMessage.Text = string.Empty;
			tProcessTimer.Enabled = false;
			lbReportExport.Enabled = true;
			dvLoadingImg.Visible = false;
		}
	}

	/// <summary>
	///　SQLステートメントを実行する
	/// </summary>
	/// <param name="statementName">SQLステートメント</param>
	/// <param name="param">検索条件</param>
	/// <returns>実行結果</returns>
	private DataView ActionSqlStatement(string statementName, Hashtable param)
	{
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("OrderConditionReport", statementName) { CommandTimeout = 600000 })
		{
			sqlStatement.ReplaceStatement("@@ order_payment_kbns @@", (string)param["order_payment_kbns"]);
			sqlStatement.ReplaceStatement("@@ order_kbns @@", (string)param["order_kbns"]);
			sqlStatement.ReplaceStatement("@@ order_types @@", (string)param["order_types"]);
			var timeFrom = DateTime.Parse(StringUtility.ToEmpty(param[Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_TIME_FROM]));
			var timeTo = DateTime.Parse(StringUtility.ToEmpty(param[Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_TIME_TO]));

			// 合計指定
			DateTime dateFrom;
			DateTime dateTo;
			if (this.IsTotal)
			{
				// 月別
				if (param["number_of_days"] == null)
				{
					dateFrom = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), 1, 1);
					dateTo = dateFrom.AddYears(1);
				}
				// 日別（期間指定）
				else
				{
					dateFrom = new DateTime(
						int.Parse(param["year"].ToString()), 
						int.Parse(param["month"].ToString()), 
						int.Parse(param["day"].ToString()),
						timeFrom.Hour,
						timeFrom.Minute,
						timeFrom.Second);
					dateTo = new DateTime(
						int.Parse(param["yearto"].ToString()),
						int.Parse(param["monthto"].ToString()),
						int.Parse(param["dayto"].ToString()),
						timeTo.Hour,
						timeTo.Minute,
						timeTo.Second,
						998);
				}
			}
			// 日付指定
			else
			{
				// 月別
				if (string.IsNullOrEmpty((string)param[Constants.REQUEST_KEY_TARGET_DAY]))
				{
					dateFrom = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), int.Parse(param[Constants.REQUEST_KEY_TARGET_MONTH].ToString()), 1);
					dateTo = dateFrom.AddMonths(1);
				}
				// 日別
				else
				{
					dateFrom = new DateTime(
						int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()),
						int.Parse(param[Constants.REQUEST_KEY_TARGET_MONTH].ToString()),
						int.Parse(param[Constants.REQUEST_KEY_TARGET_DAY].ToString()),
						timeFrom.Hour,
						timeFrom.Minute,
						timeFrom.Second);
					dateTo = new DateTime(
						int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()),
						int.Parse(param[Constants.REQUEST_KEY_TARGET_MONTH].ToString()),
						int.Parse(param[Constants.REQUEST_KEY_TARGET_DAY].ToString()),
						timeTo.Hour,
						timeTo.Minute,
						timeTo.Second,
						998);
				}
			}
			param["date_from"] = dateFrom;
			param["date_to"] = DateTimeUtility.ToStringForManager(
				dateTo,
				DateTimeUtility.FormatType.LongFullDateTimeNoneServerTime);

			sqlStatement.ReplaceStatement(
				"@@ search_order_date @@",
				((string)param["sales_type"] == "1")
					? "[[ PRODUCT_SALE_RANKING_SEARCH_WHERE_SEARCH_ORDER_DATE ]]"
					: "[[ PRODUCT_SALE_RANKING_SEARCH_WHERE_SEARCH_SHIPPED_DATE ]]");

			// 検索パラメータの商品セール区分が「通常」であればセール以外の受注を条件にする。それ以外は全検索 or セールID・区分の条件で抽出
			var productSaleStatement = ((string)param[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_KBN] == VALUETEXT_PARAM_PRODUCTSALE_KBN_NOMAL)
					? "w2_OrderItem.productsale_id = ''"
					: "(@productsale_kbn = '' OR w2_ProductSale.productsale_kbn = @productsale_kbn)  "
					+ "AND  (@productsale_id = '' OR w2_OrderItem.productsale_id = @productsale_id)";
			sqlStatement.ReplaceStatement(FIELD_ORDERCONDITION_PRODUCT_SALE_SEARCH_CONDITION, productSaleStatement);

			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, param);
		}
	}
	/// <summary>
	///　集計期間表示文言作成
	/// </summary>
	/// <param name="param">検索用パラメタ</param>
	/// <remarks>
	/// ・合計指定---yyyy年1年1日(曜日) ～ yyyy年12年31日(曜日)
	/// ・月指定---yyyy年MM年1日(曜日) ～ yyy年MM年dd日(曜日)
	/// ・特定日指定---yyyy年MM年dd日(曜日)
	/// </remarks>
	private string CreateDisplayTotalPeriod(Hashtable param)
	{
		DateTime Begin;
		DateTime End;

		if (this.IsTotal)
		{
			// 合計
			if (param["number_of_days"] == null)
			{
				// 月別
				Begin = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), 1, 1);
				End = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), 12, 31);
			}
			else
			{
				// 日別
				Begin = new DateTime(
					int.Parse(param["year"].ToString()),
					int.Parse(param["month"].ToString()),
					int.Parse(param["day"].ToString()));
				End = Begin.Date.AddDays(int.Parse(param[Constants.REQUEST_KEY_NUMBER_OF_DAYS].ToString()) - 1);
			}
		}
		else
		{
			// 指定
			if (StringUtility.ToEmpty(param[Constants.REQUEST_KEY_TARGET_DAY]) == "")
			{
				// 月別
				Begin = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), int.Parse(param[Constants.REQUEST_KEY_TARGET_MONTH].ToString()), 1);
				End = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), int.Parse(param[Constants.REQUEST_KEY_TARGET_MONTH].ToString()), DateTime.DaysInMonth(int.Parse((string)param[Constants.REQUEST_KEY_TARGET_YEAR]), int.Parse((string)param[Constants.REQUEST_KEY_TARGET_MONTH])));
			}
			else
			{
				// 日別
				Begin = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), int.Parse(param[Constants.REQUEST_KEY_TARGET_MONTH].ToString()), int.Parse(param[Constants.REQUEST_KEY_TARGET_DAY].ToString()));

				return DateTimeUtility.ToStringForManager(Begin, DateTimeUtility.FormatType.LongDateWeekOfDay2Letter);
			}
		}

		return string.Format(
			"{0}～{1}",
			DateTimeUtility.ToStringForManager(Begin, DateTimeUtility.FormatType.LongDateWeekOfDay2Letter),
			DateTimeUtility.ToStringForManager(End, DateTimeUtility.FormatType.LongDateWeekOfDay2Letter));
	}
	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	/// <remarks>CSVファイルを出力する</remarks>
	protected void lbReportExport_Click(object sender, System.EventArgs e)
	{
		Hashtable param = GetSearchParameters();
		param["bgn_row_num"] = 0;
		param["end_row_num"] = this.ProcessInfo.TotalCount;
		var dvDetail = ActionSqlStatement("GetProductSaleRankingReport", param);

		StringBuilder csvRecord = new StringBuilder();

		// ヘッダ行
		List<string> lHeadercolumns = new List<string>();
		lHeadercolumns.Add(ReplaceTag("@@Product.product_id.name@@"));
		if (this.IsVariationDisplay)
		{
			lHeadercolumns.Add(ReplaceTag("@@Product.variation_id.name@@"));
		}
		lHeadercolumns.Add(ReplaceTag("@@Product.name.name@@"));

		lHeadercolumns.Add(
			// 「期中販売数」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_DURING_THE_PERIOD,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_DURING_THE_PERIOD_NUMBER_SALE));
		lHeadercolumns.Add(string.Format(
			// 「期中売上計{0}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_DURING_THE_PERIOD,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_DURING_THE_PERIOD_TOTAL_SALE),
			string.Format(
				"（{0}）",
				this.IsIncludedTax
					? ReplaceTag("@@DispText.tax_type.included@@")
					: ReplaceTag("@@DispText.tax_type.excluded@@"))));
		lHeadercolumns.Add(string.Format(
			// 「平均売価{0}」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_AVERAGE_SELLING_PRICE),
			string.Format(
				"（{0}）",
				this.IsIncludedTax
					? ReplaceTag("@@DispText.tax_type.included@@")
					: ReplaceTag("@@DispText.tax_type.excluded@@"))));

		lHeadercolumns.Add(
			// 「期中消化率」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_DURING_THE_PERIOD,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_DURING_THE_PERIOD_DIGESTION_SALE));

		lHeadercolumns.Add(
			// 「総投入数」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TOTAL_KBN,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TOTAL_KBN_INPUT));

		lHeadercolumns.Add(
			// 「総販売数」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TOTAL_KBN,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TOTAL_KBN_SALE));

		lHeadercolumns.Add(
			// 「総消化率」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TOTAL_KBN,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TOTAL_KBN_DIGESTIBILITY));

		lHeadercolumns.Add(
			// 「在庫残」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_TITLE,
				Constants.VALUETEXT_PARAM_PRODUCT_SALE_RANKING_REPORT_OUT_OF_STOCK));
		csvRecord.Append(CreateRecordCsv(lHeadercolumns));

		foreach (DataRowView drv in dvDetail)
		{
			List<string> lColumns = new List<string>();

			foreach (DataColumn dc in drv.Row.Table.Columns)
			{
				if (dc.ColumnName.Contains("icon")) continue;
				if (dc.ColumnName.Contains("image")) continue;
				if (dc.ColumnName.Contains("shop_id")) continue;
				if (this.IsVariationDisplay == false)
				{
					if (dc.ColumnName.Contains("variation_id")) continue;
				}

				if (dc.ColumnName.Contains("product_price_total") || dc.ColumnName.Contains("average_price"))
				{
					drv[dc.ColumnName] = drv[dc.ColumnName].ToPriceString();
				}

				lColumns.Add(drv[dc.ColumnName].ToString());
			}
			csvRecord.Append(CreateRecordCsv(lColumns));
		}

		OutPutFileCsv(CreateCsvFileName(param), csvRecord.ToString());
	}
	/// <summary>
	///　CSVファイル名作成
	/// </summary>
	/// <param name="param">検索用パラメタ</param>
	/// <returns>CSVファイル名</returns>
	/// <remarks>
	/// ・合計指定：yyyy0101_yyyy1231
	/// ・月指定：yyyyMM01_yyyyMMdd
	/// ・特定日指定：yyyyMMdd
	/// </remarks>
	private string CreateCsvFileName(Hashtable param)
	{
		StringBuilder targetDate = new StringBuilder();
		DateTime Begin;
		DateTime End;

		if (this.IsTotal)
		{
			// 合計
			if (param["number_of_days"] == null)
			{
				// 月別
				Begin = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), 1, 1);
				End = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), 12, 31);
			}
			else
			{
				// 日別
				Begin = new DateTime((int)param["year"], (int)param["month"], (int)param["day"]);
				End = Begin.Date.AddDays((int)param["number_of_days"] - 1);
			}

			targetDate.Append(Begin.ToString("yyyyMMdd"));
			targetDate.Append("_");
			targetDate.Append(End.ToString("yyyyMMdd"));
		}
		else
		{
			// 指定
			if (StringUtility.ToEmpty(param[Constants.REQUEST_KEY_TARGET_DAY]) == "")
			{
				// 月別
				Begin = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), int.Parse(param[Constants.REQUEST_KEY_TARGET_MONTH].ToString()), 1);
				End = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), int.Parse(param[Constants.REQUEST_KEY_TARGET_MONTH].ToString()), DateTime.DaysInMonth(int.Parse((string)param[Constants.REQUEST_KEY_TARGET_YEAR]), int.Parse((string)param[Constants.REQUEST_KEY_TARGET_MONTH])));

				targetDate.Append(Begin.ToString("yyyyMMdd"));
				targetDate.Append("_");
				targetDate.Append(End.ToString("yyyyMMdd"));
			}
			else
			{
				// 日別
				Begin = new DateTime(int.Parse(param[Constants.REQUEST_KEY_TARGET_YEAR].ToString()), int.Parse(param[Constants.REQUEST_KEY_TARGET_MONTH].ToString()), int.Parse(param[Constants.REQUEST_KEY_TARGET_DAY].ToString()));

				targetDate.Append(Begin.ToString("yyyyMMdd"));
			}
		}

		return "ProductSaleRankingReport_" + targetDate.ToString();
	}


	/// <summary>
	/// 売上状況レポート検索条件取得
	/// </summary>
	/// <returns>検索条件用のパラメタ</returns>
	/// <remarks>売上状況レポートから引き継いだセッション情報とリクエスト情報から検索条件とページ番号を取得する</remarks>
	private Hashtable GetSearchParameters()
	{
		Hashtable param = (Hashtable)this.SessionParameter.Clone();
		param[Constants.REQUEST_KEY_TARGET_YEAR] = HttpUtility.UrlDecode(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_YEAR]));
		param[Constants.REQUEST_KEY_TARGET_MONTH] = HttpUtility.UrlDecode(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_MONTH]));
		param[Constants.REQUEST_KEY_TARGET_DAY] = HttpUtility.UrlDecode(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_DAY]));
		param["total_flg"] = HttpUtility.UrlDecode(StringUtility.ToEmpty(Request["total_flg"]));  // ※売上状況レポートの合計アンカークリック時にパラメタが渡される
		this.IsTotal = (((string)param["total_flg"]) != "");
		this.IsIncludedTax = (StringUtility.ToEmpty(param["tax_included_flg"]) == "1");

		int iCurrentPageNumber;
		if (int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out iCurrentPageNumber) == false) iCurrentPageNumber = 1;
		param[Constants.REQUEST_KEY_PAGE_NO] = iCurrentPageNumber;
		var timeFrom = StringUtility.ToEmpty(Request.QueryString[Constants.REQUEST_KEY_REPORT_TIME_FROM]);
		var timeTo = StringUtility.ToEmpty(Request.QueryString[Constants.REQUEST_KEY_REPORT_TIME_TO]);

		if (string.IsNullOrEmpty(timeFrom)) timeFrom = Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DEFAULT_START_TIME;
		if (string.IsNullOrEmpty(timeTo)) timeTo = Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_DEFAULT_END_TIME;

		param[Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_TIME_FROM] = timeFrom;
		param[Constants.VALUETEXT_PARAM_ORDER_CONDITION_REPORT_TIME_TO] = timeTo;

		return param;
	}

	/// <summary>
	/// 商品別販売個数ランキングレポートURL作成（合計）
	/// </summary>
	protected string CreateProductSaleRankingReportlUrlForTotal()
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_PRODUCT_SALE_RANKING_REPORT);
		url.Append("?").Append(Constants.REQUEST_KEY_TARGET_YEAR).Append("=").Append(HttpUtility.UrlEncode(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_YEAR])));
		if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_MONTH]) != "") url.Append("&").Append(Constants.REQUEST_KEY_TARGET_MONTH).Append("=").Append(HttpUtility.UrlEncode(Request[Constants.REQUEST_KEY_TARGET_MONTH]));
		if (StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_TARGET_DAY]) != "") url.Append("&").Append(Constants.REQUEST_KEY_TARGET_DAY).Append("=").Append(HttpUtility.UrlEncode(Request[Constants.REQUEST_KEY_TARGET_DAY]));
		if (StringUtility.ToEmpty(Request["total_flg"]) != "") url.Append("&total_flg=1");
		url.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=1");
		url.Append("&").Append(Constants.REQUEST_KEY_WINDOW_KBN).Append("=").Append(HttpUtility.UrlEncode(Constants.KBN_WINDOW_POPUP));

		return url.ToString();
	}

	/// <summary>
	/// データバインド用商品別販売数ランキング覧遷移URL作成
	/// </summary>
	/// <param name="htSearch">検索情報</param>
	/// <param name="iPageNumber">ページNO</param>
	/// <returns>商品別販売数ランキング遷移URL</returns>
	protected string CreateProductSaleRankingReportUrl(Hashtable htSearch, int? iPageNumber)
	{
		StringBuilder url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_PRODUCT_SALE_RANKING_REPORT);
		url.Append("?").Append(Constants.REQUEST_KEY_TARGET_YEAR).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_TARGET_YEAR]));
		url.Append("&").Append(Constants.REQUEST_KEY_TARGET_MONTH).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_TARGET_MONTH]));
		url.Append("&").Append(Constants.REQUEST_KEY_TARGET_DAY).Append("=").Append(HttpUtility.UrlEncode((string)htSearch[Constants.REQUEST_KEY_TARGET_DAY]));
		url.Append("&").Append("total_flg").Append("=").Append(HttpUtility.UrlEncode((string)htSearch["total_flg"]));
		if (iPageNumber != null) url.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(iPageNumber.ToString());

		return url.ToString();
	}

	protected void rblDisplayUnit_SelectedIndexChanged(object sender, EventArgs e)
	{
		this.SessionParameter[Constants.REQUEST_KEY_PAGE_NO] = 1;	// 表示切替時はページ先頭にする
		this.SessionParameter[Constants.REQUEST_KEY_PRODUCT_UNIT_TYPE] = ((RadioButtonList)sender).SelectedValue;

		Response.Redirect(CreateProductSaleRankingReportlUrlForTotal());
	}

	/// <summary>税込表示か</summary>//todo
    protected bool IsIncludedTax
    {
        get
        {
            return (StringUtility.ToEmpty(this.SessionParameter["tax_included_flg"]) == "1");
        }
        private set {}
    }
    /// <summary>合計フラグ（売上状況レポートで合計アンカーを押下した場合に「真」とする）</summary>
    private bool IsTotal { get; set; }
	/// <summary>販売商品情報一覧</summary>
	protected DataView OrderProductList { get; private set; }
	/// <summary>表示軸がバリエーション単位か</summary>
	protected bool IsVariationDisplay { get { return StringUtility.ToEmpty((string)(this.SessionParameter)[Constants.REQUEST_KEY_PRODUCT_UNIT_TYPE]) != "0"; } }
	private Hashtable SessionParameter { get { return (Hashtable)Session[Constants.SESSION_KEY_PARAM]; } }
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
					System.Web.Caching.Cache.NoAbsoluteExpiration,	// 絶対日付期限切れなし
					TimeSpan.FromMinutes(5));	// 5分キャッシュにアクセスが無かったら破棄する
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
		this.OrderProductList = new DataView();
		this.TotalCount = 0;
		this.CurrentPageNumber = 0;
		this.StartTime = new DateTime();
	}
	/// <summary>システムエラー発生したか</summary>
	public bool IsSystemError { get; set; }
	/// <summary>処理完了したか</summary>
	public bool IsDone { get; set; }
	/// <summary>検索条件用のパラメタ</summary>
	public Hashtable Param { get; set; }
	/// <summary>販売商品情報一覧</summary>
	public DataView OrderProductList { get; set; }
	/// <summary>実行結果件数</summary>
	public int TotalCount { get; set; }
	/// <summary>ページ番号</summary>
	public int CurrentPageNumber { get; set; }
	/// <summary>処理開始時間</summary>
	public DateTime StartTime { get; set; }
}