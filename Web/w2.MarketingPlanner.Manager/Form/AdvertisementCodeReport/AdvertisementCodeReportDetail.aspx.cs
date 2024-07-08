/*
=========================================================================================================
  Module      :広告コードレポート詳細ページ処理(AdvertisementCodeReportDetail.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Text;
using w2.Domain.AdvCode;
using w2.Domain.ShopOperator;
using System.Linq;

public partial class Form_AdvertisementCodeReport_AdvertisementCodeReportDetail : AdvertisementCodeReportPage
{
	private const string FIELD_BGN_ROW_NUM = "bgn_row_num";
	private const string FIELD_END_ROW_NUM = "end_row_num";

	// 対象広告コード
	protected string FIELD_ADVC_ADVERTISEMENT_CODE_TARGET = Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE + "_target";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// リクエスト情報取得
			//------------------------------------------------------
			int iPageNum = 0;
			if (int.TryParse(StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PAGE_NO]), out iPageNum) == false)
			{
				iPageNum = 1;
			}

			Hashtable requestParams = GetRequestParams();
			requestParams.Add(FIELD_BGN_ROW_NUM, Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (iPageNum - 1) + 1);
			requestParams.Add(FIELD_END_ROW_NUM, Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * iPageNum);

			DataView dataReport = GetData(requestParams);

			if (dataReport.Count != 0)
			{
				this.TotalCount = int.Parse(dataReport[0].Row["row_count"].ToString());
				trListError.Visible = false;
			}
			else
			{
				this.TotalCount = 0;
				trListError.Visible = true;
				tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			}

			// データソースセット
			switch (StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_ADVCODE_DISP_KBN]))
			{
				case Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY:			// 商品別売れ行きランキング


					rListRank.DataSource = dataReport;
					rListRank.DataBind();

					rListOwner.Visible = false;

					tbodyPager1.Visible = false;

					break;
				case Constants.KBN_ADVCODE_DISP_ORDER: // 購入者詳細
					rListOwner.DataSource = dataReport;
					rListOwner.DataBind();

					rListRank.Visible = false;

					//------------------------------------------------------
					// ページャ作成（一覧処理で総件数を取得）
					//------------------------------------------------------
					tbodyPager1.Visible = true;
					string strNextUrl = CreateAdvertisementCodeDetailUrl(StringUtility.ToEmpty(requestParams[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE]),
																		StringUtility.ToEmpty(requestParams[Constants.FIELD_USER_CAREER_ID]),
																		StringUtility.ToEmpty(requestParams[FIELD_ADVC_TERM1_FROM]),
																		StringUtility.ToEmpty(requestParams[FIELD_ADVC_TERM1_TO]),
																		StringUtility.ToEmpty(requestParams[Constants.REQUEST_KEY_ADVCODE_DISP_KBN]));
					lbPager1.Text = WebPager.CreateDefaultListPager(this.TotalCount, iPageNum, strNextUrl);

					break;
			}

			//------------------------------------------------------
			// 媒体名取得
			//------------------------------------------------------
			var advertisementCode = new AdvCodeService().GetAdvCodeFromAdvertisementCode((string)requestParams[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE]);
			string strMediaName = (advertisementCode != null) ? advertisementCode.MediaName : string.Empty;

			//------------------------------------------------------
			// コンポーネント初期化
			//------------------------------------------------------
			lAdvertisementCode.Text = requestParams[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE].ToString() != "" ? requestParams[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE].ToString() : "全体";
			lMediaName.Text = strMediaName;
			lCareer.Text = ValueText.GetValueText(Constants.TABLE_ADVCODELOG, Constants.FIELD_MOBILEGROUP_CAREER_ID, requestParams[Constants.FIELD_USER_CAREER_ID].ToString());
			hfDsipKbn.Value = requestParams[Constants.REQUEST_KEY_ADVCODE_DISP_KBN].ToString();
			lTerm1From.Text = DateTimeUtility.ToStringForManager(
				requestParams[FIELD_ADVC_TERM1_FROM],
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			lTerm1To.Text = DateTimeUtility.ToStringForManager(
				requestParams[FIELD_ADVC_TERM1_TO],
				DateTimeUtility.FormatType.ShortDateHourMinuteSecond2Letter);
			lAdvertisementCodeTarget.Text = (requestParams[FIELD_ADVC_ADVERTISEMENT_CODE_TARGET].ToString() == KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER) ? "注文時の広告コード" : "初回の広告コード";

			this.DataBind();
		}
	}

	/// <summary>
	/// 一覧ＵＲＬ作成
	/// </summary>
	/// <returns></returns>
	private string CreateAdvertisementCodeDetailUrl(string strAdvertisementCode, string strCareerId, string strTermFrom, string strTermTo, string strDispKbn)
	{
		StringBuilder sbSearchUrl = new StringBuilder();

		sbSearchUrl.Append(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_REPORT_DETAIL);
		sbSearchUrl.Append("?").Append(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE).Append("=").Append(HttpUtility.UrlEncode(strAdvertisementCode));
			sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_ADVCODE_CAREER_ID).Append("=").Append(HttpUtility.UrlEncode(strCareerId));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_ADVCODE_TERM_FROM).Append("=").Append(HttpUtility.UrlEncode(strTermFrom));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_ADVCODE_TERM_TO).Append("=").Append(HttpUtility.UrlEncode(strTermTo));
		sbSearchUrl.Append("&").Append(Constants.REQUEST_KEY_ADVCODE_DISP_KBN).Append("=").Append(HttpUtility.UrlEncode(strDispKbn));

		return sbSearchUrl.ToString();
	}

	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, System.EventArgs e)
	{
		// Get request parameter
		Hashtable requestParams = GetRequestParams();
		requestParams.Add(FIELD_BGN_ROW_NUM, 1);
		requestParams.Add(FIELD_END_ROW_NUM, this.TotalCount);

		DataView data = GetData(requestParams);

		string dispKbn = requestParams[Constants.REQUEST_KEY_ADVCODE_DISP_KBN].ToString();

		// Set data to form export
		SetData(dispKbn);

		switch (dispKbn)
		{
			case Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY:	// 商品別売れ行きランキング
				ExportFileCsv(EXPORT_FILE_NAME_ORDER_PRODUCT, GetDataExport(data, Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY, null, GetTitleRowsData(Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY)));
				break;

			case Constants.KBN_ADVCODE_DISP_ORDER:	// 購入者詳細
				ExportFileCsv(EXPORT_FILE_NAME_ORDER_OWNER, GetDataExport(data, Constants.KBN_ADVCODE_DISP_ORDER, null, GetTitleRowsData(Constants.KBN_ADVCODE_DISP_ORDER)));
				break;

			default:
				ExportFileCsv(EXPORT_FILE_NAME_ORDER_PRODUCT, GetDataExport(data, Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY, null, GetTitleRowsData(Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY)));
				break;
		}
	}

	/// <summary>
	/// Set data to form export
	/// </summary>
	/// <param name="dispKbn"> Display Kbn</param>
	private void SetData(string dispKbn)
	{
		this.AdvCode = lAdvertisementCode.Text;
		this.MediaName = lMediaName.Text;
		this.Term1From = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_TERM_FROM]);
		this.Term1To = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_TERM_TO]);
		this.AdvertisementCodeTarget = StringUtility.ToValue(Request[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE_TARGET], KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER).ToString();
		this.Career = ValueText.GetValueText(Constants.TABLE_ADVCODELOG,
												Constants.FIELD_MOBILEGROUP_CAREER_ID,
												StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_CAREER_ID]));
		switch (dispKbn)
		{
			case Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY:
				// 「売上表示(出荷基準)(商品別詳細)」
				this.TotalKbn = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT,
					Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_DETAIL_SALE_DISPLAY,
					Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_DETAIL_SALE_DISPLAY_DETAIL_PRODUCT);
				break;

			case Constants.KBN_ADVCODE_DISP_ORDER:
				// 「売上表示(出荷基準)(購入者詳細)」
				this.TotalKbn = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT,
					Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_DETAIL_SALE_DISPLAY,
					Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_DETAIL_SALE_DISPLAY_PURCHASER_DETAIL);
				break;

			default:
				// 「売上表示(出荷基準)(商品別詳細)」
				this.TotalKbn = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT,
					Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_DETAIL_SALE_DISPLAY,
					Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_DETAIL_SALE_DISPLAY_DETAIL_PRODUCT);
				break;
		}
	}

	/// <summary>
	/// Get data export
	/// </summary>
	/// <param name="paramInput"> Parameters input</param>
	/// <returns> Data</returns>
	private DataView GetData(Hashtable paramInput)
	{
		//------------------------------------------------------
		// 広告コード詳細情報取得
		//------------------------------------------------------
		string statements = null;
		switch (StringUtility.ToEmpty(paramInput[Constants.REQUEST_KEY_ADVCODE_DISP_KBN]))
		{
			case Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY:				// 商品別売れ行きランキング
				statements = "GetAdvertisementCodeRankingProductBuy";
				break;
			case Constants.KBN_ADVCODE_DISP_ORDER:								// 購入者詳細
				statements = (StringUtility.ToEmpty(paramInput[FIELD_ADVC_ADVERTISEMENT_CODE_TARGET]) == KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER)
														? "GetAdvertisementCodeOrderOrderOwner"
														: "GetAdvertisementCodeFirstOrderOwner";
				break;
			default:
				paramInput[Constants.REQUEST_KEY_ADVCODE_DISP_KBN] = Constants.KBN_ADVCODE_DISP_RANKING_PRODUCT_BUY;	// デフォルト：商品別売れ行きランキング
				statements = "GetAdvertisementCodeRankingProductBuy";
				break;
		}

		if (Validator.IsDate(StringUtility.ToEmpty(paramInput[FIELD_ADVC_TERM1_FROM]), Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false)
		{
			paramInput[FIELD_ADVC_TERM1_FROM] = DBNull.Value;
		}
		else
		{
			paramInput[FIELD_ADVC_TERM1_FROM] = DateTime.Parse(
				StringUtility.ToEmpty(paramInput[FIELD_ADVC_TERM1_FROM]),
				string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
					? CultureInfo.CurrentCulture
					: new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE));
		}

		if (Validator.IsDate(StringUtility.ToEmpty(paramInput[FIELD_ADVC_TERM1_TO]), Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE) == false)
		{
			paramInput[FIELD_ADVC_TERM1_TO] = DBNull.Value;
		}
		else
		{
			paramInput[FIELD_ADVC_TERM1_TO] = DateTime.Parse(
				StringUtility.ToEmpty(paramInput[FIELD_ADVC_TERM1_TO]),
				string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
					? CultureInfo.CurrentCulture
					: new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE));
		}

		var whereCondition = string.Empty;
		var loginOperator = new ShopOperatorService().Get(this.LoginOperatorShopId, this.LoginOperatorId);
		if (loginOperator.UsableAdvcodeNosInReport.Length > 0)
		{
			var updateAdCodeArray = StringUtility.SplitCsvLine(loginOperator.UsableAdvcodeNosInReport);
			var targetUpdateAdvCode = string.Join("', '", updateAdCodeArray);
			var targetAdvCode = string.Empty;

			switch (statements)
			{
				// 商品別売れ行きランキング
				case "GetAdvertisementCodeRankingProductBuy":
					targetAdvCode = (StringUtility.ToEmpty(paramInput[FIELD_ADVC_ADVERTISEMENT_CODE_TARGET]) == KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER)
						? "w2_Order.advcode_new"
						: "w2_Order.advcode_first";
					break;

				// 注文時広告コード購入者詳細
				case "GetAdvertisementCodeOrderOrderOwner":
					targetAdvCode = "w2_Order.advcode_new";
					break;

				// 初回コード付き購入者詳細
				case "GetAdvertisementCodeFirstOrderOwner":
					targetAdvCode = "w2_Order.advcode_first";
					break;
			}

			whereCondition = "AND  " + targetAdvCode + " IN ('" + targetUpdateAdvCode + "')";
		}

		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("AdvertisementCodeReport", statements))
		{
			sqlStatement.Statement = sqlStatement.Statement.Replace("@@ where_adv_code @@", whereCondition);
			return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, paramInput);
		}
	}

	/// <summary>
	/// Get parameters request
	/// </summary>
	/// <returns> Parameters</returns>
	private Hashtable GetRequestParams()
	{
		Hashtable requestParams = new Hashtable();
		requestParams.Add(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE]));
		requestParams.Add(Constants.FIELD_USER_CAREER_ID, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_CAREER_ID]));
		requestParams.Add(FIELD_ADVC_TERM1_FROM, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_TERM_FROM]));
		requestParams.Add(FIELD_ADVC_TERM1_TO, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_TERM_TO]));
		requestParams.Add(FIELD_ADVC_ADVERTISEMENT_CODE_TARGET,StringUtility.ToValue(Request[Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE_TARGET], KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER).ToString());
		requestParams.Add(Constants.REQUEST_KEY_ADVCODE_DISP_KBN, StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_ADVCODE_DISP_KBN]));

		return requestParams;
	}
}
