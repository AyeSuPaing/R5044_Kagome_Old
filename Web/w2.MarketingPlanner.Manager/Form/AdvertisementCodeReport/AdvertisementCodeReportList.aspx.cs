/*
=========================================================================================================
  Module      :広告コードレポート一覧ページ処理(AdvertisementCodeReportList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI.WebControls;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Util;
using w2.Domain.AdvCode;
using w2.Domain.ShopOperator;

public partial class Form_AdvertisementCodeReport_AdvertisementCodeReportList : AdvertisementCodeReportPage
{
	// 期間指定1,2
	protected string FIELD_ADVC_TERM_FLG = "term_flg";
	protected string FIELD_ADVC_TERM2_FROM = "term2_from";
	protected string FIELD_ADVC_TERM2_TO = "term2_to";

	protected string KBN_ADVC_TERM_FLG_TERM1 = "0";	// 期間1指定
	protected string KBN_ADVC_TERM_FLG_TERM2 = "1";	// 期間1,2指定

	// 対象広告コード
	protected string FIELD_ADVC_ADVERTISEMENT_CODE_TARGET = Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE + "_target";

	// コンバージョンレート集計基準
	protected string FIELD_ADVC_ADVERTISEMENT_CONVERSION_RATE_KBN = "advertisement_conversion_rate_kbn";

	// Total value
	private string m_OrderPriceTotal = "";
	private string m_CRUserRegistTotalCvr = "";
	private string m_CRUserWithdrawalTotalCvr = "";
	private string m_CRUserCountTotalCvr = "";
	private string m_CROrderPriceTotal = "";
	private string m_CROrderPriceTotalAvg = "";

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
			// コンポーネント初期化
			//------------------------------------------------------
			Initialize();
		}

		//------------------------------------------------------
		// 集計データ取得
		//------------------------------------------------------
		GetData();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
		// 集計区分
		rblTotalKbn.Items.Add(new ListItem(
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ADVC_REPORT,
				Constants.VALUETEXT_PARAM_ADVC_REPORT_KBN,
				KBN_ADVC_TOTAL_KBN_USER),
			KBN_ADVC_TOTAL_KBN_USER));
		rblTotalKbn.Items.Add(new ListItem(
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ADVC_REPORT,
				Constants.VALUETEXT_PARAM_ADVC_REPORT_KBN,
				KBN_ADVC_TOTAL_KBN_ORDER),
			KBN_ADVC_TOTAL_KBN_ORDER));
		rblTotalKbn.Items.Add(new ListItem(
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ADVC_REPORT,
				Constants.VALUETEXT_PARAM_ADVC_REPORT_KBN,
				KBN_ADVC_TOTAL_KBN_PV),
			KBN_ADVC_TOTAL_KBN_PV));
		rblTotalKbn.Items.Add(new ListItem(
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ADVC_REPORT,
				Constants.VALUETEXT_PARAM_ADVC_REPORT_KBN,
				KBN_ADVC_TOTAL_KBN_CVR),
			KBN_ADVC_TOTAL_KBN_CVR));
		rblTotalKbn.Items[0].Selected = true;

		// キャリア
		foreach (ListItem li in ValueText.GetValueItemList(Constants.TABLE_ADVCODELOG, Constants.FIELD_MOBILEGROUP_CAREER_ID))
		{
			rblCareer.Items.Add(li);
		}
		rblCareer.Items[0].Selected = true;

		// 期間指定1(デフォルト当月)
		DateTime dt = DateTime.Parse(DateTime.Now.Year + "/" + DateTime.Now.Month + "/01");
		var term1DateFrom = DateTimeUtility.ToStringForManager(
			dt,
			DateTimeUtility.FormatType.ShortDate2LetterNoneServerTime);
		var term1DateTo = DateTimeUtility.ToStringForManager(
			dt.AddMonths(1).AddDays(-1),
			DateTimeUtility.FormatType.ShortDate2LetterNoneServerTime);
		ucTerm1Period.SetPeriodDate(DateTime.Parse(term1DateFrom), DateTime.Parse(string.Format("{0} 23:59:59", term1DateTo)));

		// ▽ここで定義すると初期選択がされないため、aspx側で記述。
		//   ※原因は、「<% if(rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_ORDER){ %><% } %>」内に記述しているため。
		//// 対象広告コード
		//rblAdvertisementCodeTarget.Items.Add(new ListItem("注文時の広告コード", KBN_ADVC_ADVERTISEMENT_CODE_TARGET_ORDER));
		//rblAdvertisementCodeTarget.Items.Add(new ListItem("初回の広告コード", KBN_ADVC_ADVERTISEMENT_CODE_TARGET_FIRST_ORDER));
		//rblAdvertisementCodeTarget.Items[0].Selected = true;

		// Advertisement Code Media Type
		var advCodeMediaTypeList = new AdvCodeService().GetAdvCodeMediaTypeListAll();

		// Set data to dropdown list
		ddlAdvCodeMediaType.Items.Add(new ListItem("", ""));
		foreach (AdvCodeMediaTypeModel item in advCodeMediaTypeList)
		{
			ddlAdvCodeMediaType.Items.Add(new ListItem(item.AdvcodeMediaTypeName, item.AdvcodeMediaTypeId));
		}
	}

	/// <summary>
	/// データ取得・加工
	/// </summary>
	private void GetData()
	{
		if (CheckValidate() == false) return;

		DataView dvDetail = null;

		//------------------------------------------------------
		// 入力値取得
		//------------------------------------------------------
		Hashtable htInput = new Hashtable();
		htInput.Add(Constants.FIELD_USER_CAREER_ID, rblCareer.SelectedValue);
		htInput.Add(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE, rbAdvertisementCode.Checked ? tbAdvertisementCode.Text.Trim() : "");
		htInput.Add(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE + "_like_escaped", rbAdvertisementCode.Checked ? StringUtility.SqlLikeStringSharpEscape(tbAdvertisementCode.Text.Trim()) : "");
		htInput.Add(Constants.FIELD_ADVCODE_ADVCODE_MEDIA_TYPE_ID, ddlAdvCodeMediaType.SelectedValue);
		htInput.Add(FIELD_ADVC_TERM_FLG, KBN_ADVC_TERM_FLG_TERM1);
		htInput.Add(FIELD_ADVC_TERM1_FROM, DBNull.Value);
		htInput.Add(FIELD_ADVC_TERM1_TO, DBNull.Value);
		htInput.Add(FIELD_ADVC_TERM2_FROM, DBNull.Value);
		htInput.Add(FIELD_ADVC_TERM2_TO, DBNull.Value);
		htInput.Add("sort_kbn", ddlSortKbn.SelectedValue);
		htInput.Add(FIELD_ADVC_ADVERTISEMENT_CODE_TARGET, rblAdvertisementCodeTarget.SelectedValue);
		htInput.Add(FIELD_ADVC_ADVERTISEMENT_CONVERSION_RATE_KBN, rblAdvertisementConversionRateKbn.SelectedValue);

		//------------------------------------------------------
		// 期間指定1,2チェック
		//------------------------------------------------------
		if (string.IsNullOrEmpty(ucTerm1Period.StartDateTimeString) == false)
		{
			htInput[FIELD_ADVC_TERM1_FROM] = DateTime.Parse(
				ucTerm1Period.StartDateTimeString,
				string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
					? CultureInfo.CurrentCulture
					: new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE));
		}
		if (string.IsNullOrEmpty(ucTerm1Period.EndDateTimeString) == false)
		{
			htInput[FIELD_ADVC_TERM1_TO] = DateTime.Parse(
				ucTerm1Period.EndDateTimeString,
				string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
					? CultureInfo.CurrentCulture
					: new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE))
						.ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
		}

		// ユーザ集計?
		if (rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_USER)
		{
			if (string.IsNullOrEmpty(ucTerm2Period.StartDateTimeString) == false)
			{
				htInput[FIELD_ADVC_TERM2_FROM] = DateTime.Parse(
					ucTerm2Period.StartDateTimeString,
					string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
						? CultureInfo.CurrentCulture
						: new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE));
				htInput[FIELD_ADVC_TERM_FLG] = KBN_ADVC_TERM_FLG_TERM2;
			}
			if (string.IsNullOrEmpty(ucTerm2Period.EndDateTimeString) == false)
			{
				htInput[FIELD_ADVC_TERM2_TO] = DateTime.Parse(
					ucTerm2Period.EndDateTimeString,
					string.IsNullOrEmpty(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)
						? CultureInfo.CurrentCulture
						: new CultureInfo(Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE)).AddSeconds(1)
							.ToString(Constants.CONST_SHORTDATETIME_2LETTER_FORMAT);
				htInput[FIELD_ADVC_TERM_FLG] = KBN_ADVC_TERM_FLG_TERM2;
			}
		}

		// 正常入力の場合
		//------------------------------------------------------
		// データ取得
		//------------------------------------------------------
		string statementName = null;
		switch (rblTotalKbn.SelectedValue)
		{
			case KBN_ADVC_TOTAL_KBN_USER:	// ユーザ集計
				statementName = "GetAdvertisementCodeUser";
				break;

			case KBN_ADVC_TOTAL_KBN_ORDER:	// 売上表示?
				statementName = "GetAdvertisementCodeOrder";
				break;

			case KBN_ADVC_TOTAL_KBN_PV:	// PV
				statementName = "GetAdvertisementCodePV";
				break;

			case KBN_ADVC_TOTAL_KBN_CVR:	//コンバージョンレート
				statementName = "GetAdvertisementCodeConversionRate";
				break;
		}

		var whereCondition = string.Empty;
		var loginOperator = new ShopOperatorService().Get(this.LoginOperatorShopId, this.LoginOperatorId);
		if (loginOperator.UsableAdvcodeNosInReport.Length > 0)
		{
			var updateAdCodeArray = StringUtility.SplitCsvLine(loginOperator.UsableAdvcodeNosInReport);
			var targetUpdateAdvCode = string.Join("', '", updateAdCodeArray.ToArray());
			var targetAdvCode = string.Empty;
			switch (rblTotalKbn.SelectedValue)
			{
				case KBN_ADVC_TOTAL_KBN_USER: // ユーザ集計
					targetAdvCode = "w2_User.advcode_first";
					break;

				case KBN_ADVC_TOTAL_KBN_ORDER: // 売上表示?
					targetAdvCode = (rblAdvertisementCodeTarget.SelectedValue == "0") ? "w2_Order.advcode_new" : "w2_Order.advcode_first";
					break;

				case KBN_ADVC_TOTAL_KBN_PV: // PV
					targetAdvCode = "w2_AdvCodeLog.advertisement_code";
					break;

				case KBN_ADVC_TOTAL_KBN_CVR: //コンバージョンレート
					targetAdvCode = "temp.adv_code";
					break;
			}
			whereCondition = "AND  " + targetAdvCode + " IN ('" + targetUpdateAdvCode + "')";
		}

		using (var accessor = new SqlAccessor())
		using (var statement = new SqlStatement("AdvertisementCodeReport", statementName))
		{
			statement.UseLiteralSql = true;
			// CVRレポート取得に時間がかかるため、タイムアウトを延ばす
			if (rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_CVR) statement.CommandTimeout = 300;
			statement.Statement = statement.Statement.Replace("@@ where_adv_code @@", whereCondition);

			var visibleAdvCodeReport = statement.SelectSingleStatementWithOC(accessor, htInput);

			var dvOperator = new ShopOperatorService().Get(this.LoginOperatorShopId, this.LoginOperatorId);
			var updateAdvCodeArray = StringUtility.SplitCsvLine(dvOperator.UsableAdvcodeNosInReport);

			dvDetail = (dvOperator.UsableAdvcodeNosInReport.Length != 0)
				? visibleAdvCodeReport.Table.AsEnumerable().Where(
					i => updateAdvCodeArray.Contains((string)i[Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE])).AsDataView()
				: visibleAdvCodeReport;
		}

		this.TotalCount = dvDetail.Count;

		// 該当データありの場合
		if (dvDetail.Count > 0)
		{
			// ユーザ集計?
			if (rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_USER)
			{
				rUserDataList.DataSource = dvDetail;
				rUserDataList.DataBind();

				lbUserRegistTotal.Text = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_REGIST_COUNT + "_total"]);
				lbUserWithdrawalTotal.Text = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_WITHDRAWAL_COUNT + "_total"]);
			}
			// 売上表示?
			else if (rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_ORDER)
			{
				rOrderDataList.DataSource = dvDetail;
				rOrderDataList.DataBind();

				lbUserCountTotal.Text = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_USER_COUNT + "_total"]);
				lbOrderCountTotal.Text = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_ORDER_COUNT + "_total"]);
				lbOrderPriceTotal.Text = dvDetail[0][FIELD_ADVC_ORDER_PRICE + "_total"].ToPriceString(true);

				// Total value
				m_OrderPriceTotal = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_ORDER_PRICE + "_total"]);
			}
			// PV?
			else if (rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_PV)
			{
				rPVDataList.DataSource = dvDetail;
				rPVDataList.DataBind();

				lbPVClickCountTotal.Text = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_CLICK_COUNT + "_total"]);
				lbPVClickUniqueCountTotal.Text = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_CLICK_UNIQUE_COUNT + "_total"]);
			}
			// コンバージョンレート?
			else if (rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_CVR)
			{
				rConversionRateDataList.DataSource = dvDetail;
				rConversionRateDataList.DataBind();

				lbCRClickUniqueCountTotal.Text = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_CLICK_UNIQUE_COUNT + "_total"]);
				lbCRUserRegistTotal.Text = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_REGIST_COUNT + "_total"]);
				lbCRUserRegistTotalCvr.Text = DispRate(dvDetail[0][FIELD_ADVC_REGIST_COUNT + "_total"], dvDetail[0][FIELD_ADVC_CLICK_UNIQUE_COUNT + "_total"]) + "%";
				lbCRUserWithdrawalTotal.Text = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_WITHDRAWAL_COUNT + "_total"]);
				lbCRUserWithdrawalTotalCvr.Text = DispRate(dvDetail[0][FIELD_ADVC_WITHDRAWAL_COUNT + "_total"], dvDetail[0][FIELD_ADVC_CLICK_UNIQUE_COUNT + "_total"]) + "%";
				lbCRUserCountTotal.Text = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_USER_COUNT + "_total"]);
				lbCRUserCountTotalCvr.Text = DispRate(dvDetail[0][FIELD_ADVC_USER_COUNT + "_total"], dvDetail[0][FIELD_ADVC_CLICK_UNIQUE_COUNT + "_total"]) + "%";
				lbCROrderPriceTotal.Text = dvDetail[0][FIELD_ADVC_ORDER_PRICE + "_total"].ToPriceString(true);
				lbCROrderPriceTotalAvg.Text = dvDetail[0][FIELD_ADVC_ORDER_PRICE_AVG + "_total"].ToPriceString(true);

				// Total value
				m_CRUserRegistTotalCvr = DispRate(dvDetail[0][FIELD_ADVC_REGIST_COUNT + "_total"], dvDetail[0][FIELD_ADVC_CLICK_UNIQUE_COUNT + "_total"]);
				m_CRUserWithdrawalTotalCvr = DispRate(dvDetail[0][FIELD_ADVC_WITHDRAWAL_COUNT + "_total"], dvDetail[0][FIELD_ADVC_CLICK_UNIQUE_COUNT + "_total"]);
				m_CRUserCountTotalCvr = DispRate(dvDetail[0][FIELD_ADVC_USER_COUNT + "_total"], dvDetail[0][FIELD_ADVC_CLICK_UNIQUE_COUNT + "_total"]);
				m_CROrderPriceTotal = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_ORDER_PRICE + "_total"]);
				m_CROrderPriceTotalAvg = StringUtility.ToNumeric(dvDetail[0][FIELD_ADVC_ORDER_PRICE_AVG + "_total"]);
			}
			// エラー表示制御
			trConversionListError.Visible = false;
			trOrderListError.Visible = false;
			trPVListError.Visible = false;
			trUserListError.Visible = false;
		}
		// 該当データなしの場合
		else
		{
			// ユーザ集計?
			if (rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_USER)
			{
				rUserDataList.DataSource = null;
				rUserDataList.DataBind();

				lbUserRegistTotal.Text = StringUtility.ToNumeric(0);
				lbUserWithdrawalTotal.Text = StringUtility.ToNumeric(0);
			}
			// 売上表示?
			else if (rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_ORDER)
			{
				rOrderDataList.DataSource = null;
				rOrderDataList.DataBind();

				lbUserCountTotal.Text = StringUtility.ToNumeric(0);
				lbOrderCountTotal.Text = StringUtility.ToNumeric(0);
				lbOrderPriceTotal.Text = 0.ToPriceString(true);
			}
			// PV?
			else if (rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_PV)
			{
				rPVDataList.DataSource = null;
				rPVDataList.DataBind();

				lbPVClickCountTotal.Text = StringUtility.ToNumeric(0);
				lbPVClickUniqueCountTotal.Text = StringUtility.ToNumeric(0);
			}
			// コンバージョンレート?
			else if (rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_CVR)
			{
				rConversionRateDataList.DataSource = null;
				rConversionRateDataList.DataBind();

				lbCRClickUniqueCountTotal.Text = StringUtility.ToNumeric(0);
				lbCRUserRegistTotal.Text = StringUtility.ToNumeric(0);
				lbCRUserRegistTotalCvr.Text = DispRate(0, 0) + "%";
				lbCRUserWithdrawalTotal.Text = StringUtility.ToNumeric(0);
				lbCRUserWithdrawalTotalCvr.Text = DispRate(0, 0) + "%";
				lbCRUserCountTotal.Text = StringUtility.ToNumeric(0);
				lbCRUserCountTotalCvr.Text = DispRate(0, 0) + "%";
				lbCROrderPriceTotal.Text = 0.ToPriceString(true);
				lbCROrderPriceTotalAvg.Text = 0.ToPriceString(true);
			}
			// エラー表示制御
			trConversionListError.Visible = true;
			trOrderListError.Visible = true;
			trPVListError.Visible = true;
			trUserListError.Visible = true;

			string strErrorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			tdConversionErrorMessage.InnerHtml = strErrorMessage;
			tdOrderErrorMessage.InnerHtml = strErrorMessage;
			tdPvErrorMessage.InnerHtml = strErrorMessage;
			tdUserErrorMessage.InnerHtml = strErrorMessage;
		}

		this.DataBind();
	}

	/// <summary>
	/// ユーザ購入単価表示
	/// </summary>
	/// <param name="iUerCountTotal">注文ユーザ数</param>
	/// <param name="dOrderPriceTotal">注文金額</param>
	/// <returns>ユーザ購入単価</returns>
	protected int DispUnitOrderPrice(int iUerCountTotal, decimal dOrderPriceTotal)
	{
		return (int)(dOrderPriceTotal / (iUerCountTotal == 0 ? 1 : iUerCountTotal));
	}

	/// <summary>
	/// 広告コードレポート詳細URL作成
	/// </summary>
	/// <param name="strAdvertisementCode">広告コード</param>
	/// <param name="strDispKbn">広告コード分析詳細区分</param>
	/// <returns>広告コードレポート詳細URL</returns>
	protected string CreateAdvertisementCodeReportDetailUrl(string strAdvertisementCode, string strDispKbn)
	{
		StringBuilder sbResult = new StringBuilder();

		sbResult.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_ADVERTISMENT_CODE_REPORT_DETAIL);
		sbResult.Append("?").Append(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE).Append("=").Append(HttpUtility.UrlEncode(strAdvertisementCode));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ADVCODE_CAREER_ID).Append("=").Append(HttpUtility.UrlEncode(rblCareer.SelectedValue));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ADVCODE_TERM_FROM)
			.Append("=").Append(HttpUtility.UrlEncode(ucTerm1Period.StartDateTimeString));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ADVCODE_TERM_TO)
			.Append("=").Append(HttpUtility.UrlEncode(ucTerm1Period.EndDateTimeString));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ADVCODE_ADVERTISEMENT_CODE_TARGET).Append("=").Append(HttpUtility.UrlEncode(rblAdvertisementCodeTarget.SelectedValue));
		sbResult.Append("&").Append(Constants.REQUEST_KEY_ADVCODE_DISP_KBN).Append("=").Append(HttpUtility.UrlEncode(strDispKbn));

		return StringUtility.ToEmpty(sbResult.ToString());
	}

	///=============================================================================================
	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	///=============================================================================================
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// Page_Load内で処理
	}

	/// <summary>
	/// 置換用文字列をから日付エラーメッセージを作成する
	/// </summary>
	/// <param name="strErrorDateTerm">置換用文字列</param>
	/// <returns>置換後の日付エラーメッセージ</returns>
	protected string ReplaceDateErrorMessage(string strErrorDateTerm)
	{
		// 置換対象のデフォルトエラーメッセージ
		var errorMessage = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ENTER_VALID_DATE)
			.Replace("@@ 1 @@", strErrorDateTerm);
		return errorMessage;
	}

	/// <summary>
	/// CSVダウンロードリンククリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void lbReportExport_Click(object sender, System.EventArgs e)
	{
		if (CheckValidate())
		{
			// Set data to form export
			SetData();

			string fileName = "";
			DataView dataReport = null;

			switch (rblTotalKbn.SelectedValue)
			{
				case KBN_ADVC_TOTAL_KBN_USER:
					fileName = EXPORT_FILE_NAME_USER;
					dataReport = (DataView)rUserDataList.DataSource;
					break;

				case KBN_ADVC_TOTAL_KBN_ORDER:
					fileName = EXPORT_FILE_NAME_ORDER;
					dataReport = (DataView)rOrderDataList.DataSource;
					break;

				case KBN_ADVC_TOTAL_KBN_PV:
					fileName = EXPORT_FILE_NAME_PV;
					dataReport = (DataView)rPVDataList.DataSource;
					break;

				case KBN_ADVC_TOTAL_KBN_CVR:
					fileName = EXPORT_FILE_NAME_CVR;
					dataReport = (DataView)rConversionRateDataList.DataSource;
					break;
			}

			ExportFileCsv(fileName,
							GetDataExport(dataReport,
											rblTotalKbn.SelectedValue,
											GetTotalParams(rblTotalKbn.SelectedValue),
											GetTitleRowsData(rblTotalKbn.SelectedValue)));
		}
	}

	/// <summary>
	/// Set data to form export
	/// </summary>
	private void SetData()
	{
		this.Term1From = ucTerm1Period.StartDateTimeString;
		this.Term1To = ucTerm1Period.EndDateTimeString;
		this.Term2From = ucTerm2Period.StartDateTimeString;
		this.Term2To = ucTerm2Period.EndDateTimeString;
		this.TotalKbn = rblTotalKbn.SelectedItem.Text;
		this.Career = rblCareer.SelectedItem.Text;
		this.AdvertisementCodeTarget = rblAdvertisementCodeTarget.SelectedValue;
		this.AdvertisementConversionRateKbn = rblAdvertisementConversionRateKbn.SelectedItem.Text;
	}

	/// <summary>
	/// Get total parameters
	/// </summary>
	/// <param name="totalKbn"> Total Kbn</param>
	/// <returns> Total parameters</returns>
	private List<string> GetTotalParams(string totalKbn)
	{
		List<string> totalParams = new List<string>();
		totalParams.Add(
			// 「合計」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT,
				Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_LIST_TITLE,
				Constants.VALUETEXT_PARAM_ADVERTISEMENT_CODE_REPORT_LIST_TOTAL));

		switch (totalKbn)
		{
			case KBN_ADVC_TOTAL_KBN_USER:
				totalParams.Add("");
				totalParams.Add("");
				totalParams.Add(lbUserRegistTotal.Text);
				totalParams.Add(lbUserWithdrawalTotal.Text);
				break;

			case KBN_ADVC_TOTAL_KBN_ORDER:
				totalParams.Add("");
				totalParams.Add("");
				totalParams.Add(lbUserCountTotal.Text);
				totalParams.Add(lbOrderCountTotal.Text);
				totalParams.Add(m_OrderPriceTotal);
				break;

			case KBN_ADVC_TOTAL_KBN_PV:
				totalParams.Add("");
				totalParams.Add("");
				totalParams.Add(lbPVClickCountTotal.Text);
				totalParams.Add(lbPVClickUniqueCountTotal.Text);
				break;

			case KBN_ADVC_TOTAL_KBN_CVR:
				totalParams.Add("");
				totalParams.Add("");
				totalParams.Add(lbCRClickUniqueCountTotal.Text);
				totalParams.Add(lbCRUserRegistTotal.Text);
				totalParams.Add(m_CRUserRegistTotalCvr);
				totalParams.Add(lbCRUserWithdrawalTotal.Text);
				totalParams.Add(m_CRUserWithdrawalTotalCvr);
				totalParams.Add(lbCRUserCountTotal.Text);
				totalParams.Add(m_CRUserCountTotalCvr);
				totalParams.Add(m_CROrderPriceTotal);
				totalParams.Add(m_CROrderPriceTotalAvg);
				break;
		}

		return totalParams;
	}

	/// <summary>
	/// Check validate
	/// </summary>
	/// <returns> True: If valid False: If Invalid</returns>
	private bool CheckValidate()
	{
		StringBuilder errorMessageTerm1 = new StringBuilder();
		StringBuilder errorMessageTerm2 = new StringBuilder();

		//------------------------------------------------------
		// 期間指定1,2チェック
		//------------------------------------------------------
		if (string.IsNullOrEmpty(ucTerm1Period.HfStartDate.Value) == false)
		{
			if (Validator.IsDate(ucTerm1Period.HfStartDate.Value, Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE))
			{
				if (CheckDateYear(ucTerm1Period.HfStartDate.Value) == false)
				{
					errorMessageTerm1.Append(ReplaceDateErrorMessage(string.Format(
						"{0}1(From)",
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PERIOD_DESIGNATION))));
				}
			}
			else
			{
				errorMessageTerm1
					.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_HALFWIDTH_DATE)
					.Replace("@@ 1 @@", string.Format(
						"{0}1(From)",
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PERIOD_DESIGNATION))));
			}
		}
		if (string.IsNullOrEmpty(ucTerm1Period.HfEndDate.Value) == false)
		{
			if (Validator.IsDate(ucTerm1Period.HfEndDate.Value, Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE))
			{
				if (CheckDateYear(ucTerm1Period.HfEndDate.Value) == false)
				{
					errorMessageTerm1.Append(ReplaceDateErrorMessage(string.Format(
						"{0}1(To)",
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PERIOD_DESIGNATION))));
				}
			}
			else
			{
				errorMessageTerm1
					.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_HALFWIDTH_DATE)
					.Replace("@@ 1 @@", string.Format(
						"{0}1(To)",
						WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PERIOD_DESIGNATION))));
			}
		}

		// ユーザ集計?
		if (rblTotalKbn.SelectedValue == KBN_ADVC_TOTAL_KBN_USER)
		{
			if (string.IsNullOrEmpty(ucTerm2Period.HfStartDate.Value) == false)
			{
				if (Validator.IsDate(ucTerm2Period.HfStartDate.Value, Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE))
				{
					if (CheckDateYear(ucTerm2Period.HfStartDate.Value) == false)
					{
						errorMessageTerm2.Append(ReplaceDateErrorMessage(string.Format(
							"{0}2(From)",
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PERIOD_DESIGNATION))));
					}
				}
				else
				{
					errorMessageTerm2
						.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_HALFWIDTH_DATE)
						.Replace("@@ 1 @@", string.Format(
							"{0}2(From)",
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PERIOD_DESIGNATION))));
				}
			}
			if (string.IsNullOrEmpty(ucTerm2Period.HfEndDate.Value) == false)
			{
				if (Validator.IsDate(ucTerm2Period.HfEndDate.Value, Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE))
				{
					if (CheckDateYear(ucTerm2Period.HfEndDate.Value) == false)
					{
						errorMessageTerm2.Append(ReplaceDateErrorMessage(string.Format(
							"{0}2(To)",
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PERIOD_DESIGNATION))));
					}
				}
				else
				{
					errorMessageTerm2
						.Append(WebMessages.GetMessages(WebMessages.INPUTCHECK_HALFWIDTH_DATE)
						.Replace("@@ 1 @@", string.Format(
							"{0}2(To)",
							WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_PERIOD_DESIGNATION))));
				}
			}
		}

		// エラーメッセージ設定
		spErrorMessageTerm1.InnerHtml = errorMessageTerm1.Length > 0 ? "<br/>" + errorMessageTerm1.ToString() : "";
		spErrorMessageTerm2.InnerHtml = errorMessageTerm2.Length > 0 ? "<br/>" + errorMessageTerm2.ToString() : "";

		return ((errorMessageTerm1.Length + errorMessageTerm2.Length) == 0);
	}
}
