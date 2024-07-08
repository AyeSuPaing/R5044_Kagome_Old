/*
=========================================================================================================
  Module      : シルバーエッグレコメンド結果レポートページ処理(SilvereggAigentReport.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI.WebControls;
using System.Net;
using Newtonsoft.Json;

public partial class Form_SilvereggAigentReport_SilvereggAigentReport : BasePage
{
	#region 表示定数
	/// <summary> レコメンドレポート区分 </summary>
	protected const string REQUEST_KEY_REPORT_TYPE = "rtype";
	/// <summary> レコメンドレポート区分：通常 </summary>
	protected const string KBN_REPORT_TYPE_DEFAULT_REPORT = "0";
	/// <summary> レコメンドレポート区分：スペックID(レコメンドID)毎 </summary>
	protected const string KBN_REPORT_TYPE_PERSPEC_REPORT = "1";

	/// <summary> 月日別区分 </summary>
	protected const string REQUEST_KEY_DATE_TYPE = "dtype";
	/// <summary> 月日別区分：日別レポート </summary>
	protected const string KBN_DATE_TYPE_DAILY_REPORT = "0";
	/// <summary> 月日別区分：月別レポート </summary>
	protected const string KBN_DATE_TYPE_MONTHLY_REPORT = "1";

	/// <summary> レポート項目：日付 </summary>
	protected const string FIELD_RESPONSE_DATE = "date";
	/// <summary> レポート項目：スペックID(レコメンドID) </summary>
	protected const string FIELD_RESPONSE_SPECID = "specId";
	/// <summary> レポート項目：スペック名(レコメンド名) </summary>
	protected const string FIELD_RESPONSE_SPECNAME = "specName";
	/// <summary> レポート項目：表示回数 </summary>
	protected const string FIELD_RESPONSE_IMPRESSION = "impression";
	/// <summary> レポート項目：クリック数 </summary>
	protected const string FIELD_RESPONSE_CLICK = "click";
	/// <summary> レポート項目：受注件数 </summary>
	protected const string FIELD_RESPONSE_PURCHASE = "purchase";
	/// <summary> レポート項目：受注金額 </summary>
	protected const string FIELD_RESPONSE_SALES = "sales";
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

			// レポート種別の設定
			SetReportType();
		}

		// 年月日の日項目表示制御
		ucDatePeriod.Visible = (rblDateType.SelectedValue == KBN_DATE_TYPE_DAILY_REPORT);

		// カレンダー設定
		SetCalendar();

		// 検索実行
		SearchReport();
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
		//レコメンドIDリスト
		foreach (ListItem li in ValueText.GetValueItemList("SilvereggAigentReport", "spec_id_list"))
		{
			ddlSpecIdList.Items.Add(li);
		}

		// 期間指定ドロップダウン設定(デフォルトは今月)
		var ucDateStart = string.Format("{0}/{1}/01",
			this.InputCurrentYear,
			this.InputCurrentMonth);
		var ucDateEnd = string.Format("{0}/{1}/{2}",
			this.InputCurrentYear,
			this.InputCurrentMonth,
			DateTime.DaysInMonth(this.InputCurrentYear, this.InputCurrentMonth));
		ucDatePeriod.SetPeriodDate(DateTime.Parse(ucDateStart), DateTime.Parse(ucDateEnd));
	}

	/// <summary>
	/// レポート種別の設定
	/// </summary>
	private void SetReportType()
	{
		// レポート種別
		switch (Request[REQUEST_KEY_REPORT_TYPE])
		{
			case KBN_REPORT_TYPE_DEFAULT_REPORT:
			case KBN_REPORT_TYPE_PERSPEC_REPORT:
				foreach (ListItem li in rblReportType.Items)
				{
					li.Selected = (li.Value == Request[REQUEST_KEY_REPORT_TYPE]);
				}
				break;
			default:
				foreach (ListItem li in rblReportType.Items)
				{
					li.Selected = (li.Value == KBN_REPORT_TYPE_DEFAULT_REPORT);
				}
				break;
		}

		// レポート月日種別
		switch (Request[REQUEST_KEY_DATE_TYPE])
		{
			case KBN_DATE_TYPE_DAILY_REPORT:
			case KBN_DATE_TYPE_MONTHLY_REPORT:
				foreach (ListItem li in rblDateType.Items)
				{
					li.Selected = (li.Value == Request[REQUEST_KEY_DATE_TYPE]);
				}
				break;
			default:
				foreach (ListItem li in rblDateType.Items)
				{
					li.Selected = (li.Value == KBN_DATE_TYPE_DAILY_REPORT);
				}
				break;
		}
	}

	/// <summary>
	/// カレンダー設定
	/// </summary>
	private void SetCalendar()
	{
		var paramForCurrent = new StringBuilder();
		paramForCurrent.Append(REQUEST_KEY_REPORT_TYPE).Append("=").Append(rblReportType.SelectedValue);
		paramForCurrent.Append("&").Append(REQUEST_KEY_DATE_TYPE).Append("=").Append(rblDateType.SelectedValue);
		lblCurrentCalendar.Text = CalendarUtility.CreateHtmlYMCalendar(
			this.InputCurrentYear,
			this.InputCurrentMonth,
			Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_SILVEREGG_AIGENT_REPORT,
			paramForCurrent.ToString(),
			Constants.REQUEST_KEY_CURRENT_YEAR,
			Constants.REQUEST_KEY_CURRENT_MONTH);
	}

	/// <summary>
	/// 検索実行
	/// </summary>
	protected void SearchReport()
	{
		// API実行してデータ取得
		var response = GetResponses();

		// データ編集
		var recommendList = (rblReportType.SelectedValue == KBN_REPORT_TYPE_DEFAULT_REPORT) ? SetParamListDefault(response) : SetParamListPerspec(response);
		if (recommendList.Count > 0)
		{
			trListError.Visible = false;
		}
		else
		{
			tdErrorMessage.InnerText = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
			trListError.Visible = true;
		}

		rList.DataSource = recommendList;
		rList.DataBind();
	}

	/// <summary>
	/// シルバーエッグ（アイジェント）レポートAPI実行結果から明細部の取得
	/// </summary>
	/// <param name="response">APIからの返却値</param>
	/// <returns>APIからの返却値の明細部</returns>
	private List<Hashtable> SetParamListDefault(SilvereggAigentReportResponseData response)
	{
		var list = new List<Hashtable>();
		foreach (string[] row in response.Row)
		{
			var ht = new Hashtable
			{
				{FIELD_RESPONSE_DATE, row[0]},
				{FIELD_RESPONSE_IMPRESSION, row[1]},
				{FIELD_RESPONSE_CLICK, row[2]},
				{FIELD_RESPONSE_PURCHASE, row[3]},
				{FIELD_RESPONSE_SALES, StringUtility.ToPrice(row[4])}
			};
			list.Add(ht);
		}
		return list;
	}

	/// <summary>
	/// シルバーエッグ（アイジェント）レポートAPI実行結果から明細部の取得（レコメンドID毎）
	/// </summary>
	/// <param name="response">APIからの返却値</param>
	/// <returns>APIからの返却値の明細部</returns>
	private List<Hashtable> SetParamListPerspec(SilvereggAigentReportResponseData response)
	{
		var list = new List<Hashtable>();
		foreach (string[] row in response.Row)
		{
			var ht = new Hashtable
			{
				{FIELD_RESPONSE_SPECID, row[0]},
				{FIELD_RESPONSE_SPECNAME, ValueText.GetValueText("SilvereggAigentReport", "spec_id_list" , row[0])},
				{FIELD_RESPONSE_IMPRESSION, row[2]},
				{FIELD_RESPONSE_CLICK, row[3]},
				{FIELD_RESPONSE_PURCHASE, row[4]},
				{FIELD_RESPONSE_SALES, StringUtility.ToPrice(row[5])}
			};
			list.Add(ht);
		}
		return list;
	}

	/// <summary>
	/// シルバーエッグ（アイジェント）レポートAPI実行して結果を取得
	/// </summary>
	/// <returns>取得結果</returns>
	private SilvereggAigentReportResponseData GetResponses()
	{
		// APIリクエスト用のデータ取得
		var requestData = GetRequestData();

		// API種別判定
		var apiType = GetApiType();

		// APIリクエスト情報設定
		var requestUrl = Constants.RECOMMEND_SILVEREGG_REPORT_API_URL + apiType + requestData.CreatePostString();
		var request = (HttpWebRequest)WebRequest.Create(requestUrl);
		request.Method = "GET";
		request.Headers.Add("X-Server-Token", Constants.RECOMMEND_SILVEREGG_REPORT_API_TOKEN);

		try
		{
			// APIリクエスト実行
			using (var response = request.GetResponse())
			using (var stream = response.GetResponseStream())
			using (var reader = new StreamReader(stream, Encoding.UTF8))
			{
				var result = JsonConvert.DeserializeObject<SilvereggAigentReportResponseData>(reader.ReadToEnd());
				if (result.Type == "complete") return result;
			}
		}
		catch (WebException ex)
		{
			AppLogger.WriteError(ex);
			Session[Constants.SESSION_KEY_ERROR_MSG] =
				WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_SILVEREGG_GET_REPORT_ERROR);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		return new SilvereggAigentReportResponseData();
	}

	/// <summary>
	/// シルバーエッグ（アイジェント）レポートAPI種別を取得
	/// </summary>
	/// <returns>レポートAPIの種別</returns>
	private string GetApiType()
	{
		var apiType = "";
		if (rblReportType.SelectedValue == KBN_REPORT_TYPE_DEFAULT_REPORT)
		{
			apiType = (rblDateType.SelectedValue == KBN_DATE_TYPE_DAILY_REPORT) ? "daily?" : "monthly?";
		}
		else
		{
			apiType = (rblDateType.SelectedValue == KBN_DATE_TYPE_DAILY_REPORT) ? "per_spec_daily?" : "per_spec_monthly?";
		}
		return apiType;
	}

	/// <summary>
	/// シルバーエッグ（アイジェント）レポートAPIリクエスト用のデータ取得
	/// </summary>
	/// <returns>APIリクエスト用のデータを返却</returns>
	private SilvereggAigentReportRequestData GetRequestData()
	{
		// 末日補正処理
		var ucDateStart = DateTime.Parse(ucDatePeriod.HfStartDate.Value);
		var ucDateEnd = DateTime.Parse(ucDatePeriod.HfEndDate.Value);
		if (DateTimeUtility.IsLastDayOfMonth(ucDateStart.Year, ucDateStart.Month, ucDateStart.Day))
		{
			var dayStart =
				DateTimeUtility.GetLastDayOfMonth(ucDateStart.Year, ucDateStart.Month).ToString();
			ucDatePeriod.HfStartDate.Value = string.Format("{0}{1}{2}",
				ucDateStart.Year,
				ucDateStart.Month,
				dayStart);
		}
		if (DateTimeUtility.IsLastDayOfMonth(ucDateEnd.Year, ucDateEnd.Month, ucDateEnd.Day))
		{
			var dayEnd = DateTimeUtility.GetLastDayOfMonth(ucDateEnd.Year, ucDateEnd.Month).ToString();
			ucDatePeriod.HfEndDate.Value = string.Format("{0}{1}{2}",
				ucDateEnd.Year,
				ucDateEnd.Month,
				dayEnd);
		}

		// リクエスト対象日付の編集
		var selectedFrom = ucDateStart.Year.ToString() + ucDateStart.Month.ToString("00");
		var selectedTo = ucDateEnd.Year.ToString() + ucDateEnd.Month.ToString("00");
		if (rblDateType.SelectedValue == KBN_DATE_TYPE_DAILY_REPORT)
		{
			selectedFrom += ucDateStart.Day.ToString("00");
			selectedTo += ucDateEnd.Day.ToString("00");
		}

		// リクエストデータ生成
		var requestData = new SilvereggAigentReportRequestData()
		{
			Merchant = Constants.RECOMMEND_SILVEREGG_MERCHANT_ID,
			Spec = ddlSpecIdList.SelectedValue,
			Date = selectedFrom,
			From = selectedFrom,
			To = selectedTo
		};
		return requestData;
	}

	/// <summary>入力値：年</summary>
	public int InputCurrentYear
	{
		get
		{
			int currentYear = (int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_YEAR], out currentYear)) ? currentYear : DateTime.Now.Year;
			return currentYear;
		}
	}
	/// <summary>入力値：月</summary>
	public int InputCurrentMonth
	{
		get
		{
			int currentMonth = (int.TryParse(Request[Constants.REQUEST_KEY_CURRENT_MONTH], out currentMonth)) ? currentMonth : DateTime.Now.Month;
			return currentMonth;
		}
	}
}