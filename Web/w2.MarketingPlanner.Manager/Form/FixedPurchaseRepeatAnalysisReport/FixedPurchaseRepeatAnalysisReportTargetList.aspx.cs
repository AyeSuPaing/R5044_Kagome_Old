/*
=========================================================================================================
  Module      : Fixed Purchase Repeat Analysis Report Target List (FixedPurchaseRepeatAnalysisReportTargetList.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using w2.App.Common.TargetList;
using w2.App.Common.Web.Page;
using w2.Common.Web;

/// <summary>
/// Fixed purchase repeat analysis report target list
/// </summary>
public partial class Form_FixedPurchaseRepeatAnalysisReport_FixedPurchaseRepeatAnalysisReportTargetList : BasePage
{
	/// <summary>Search param: order date from</summary>
	private const string CONST_SEARCH_PARAM_ORDER_DATE_FROM = "order_date_from";
	/// <summary>Search param: order date to</summary>
	private const string CONST_SEARCH_PARAM_ORDER_DATE_TO = "order_date_to";
	/// <summary>Search param: title</summary>
	private const string CONST_SEARCH_PARAM_TITLE = "title";
	/// <summary>Search param: fixed purchase date from</summary>
	private const string SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_FIXED_PURCHASE_DATE_FROM = "fixed_purchase_date_from";
	/// <summary>Search param: fixed purchase date to</summary>
	private const string SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_FIXED_PURCHASE_DATE_TO = "fixed_purchase_date_to";
	/// <summary>Max length target list name</summary>
	private const int MAX_LENGTH_TARGET_LIST_NAME = 30;

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			if (this.IsVaildSessionParameters == false)
			{
				RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR));
			}

			// コンポーネント初期化
			InitializeComponents();
		}
	}


	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		GetSearchParameters();
		var usersCount = UserTargetList.GetUserCount(this.SessionParameters);
		if (usersCount <= 0) RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_USER_TARGET_LIST_NO_TARGET_DATA));

		btnRegisterTop.Enabled = (usersCount > 0);
		lbDataCount.Text = usersCount.ToString();

		var targetType = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT,
			Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TARGET_LIST,
			Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TARGET_LIST_LTV_REPORT);
		var dataType = ValueText.GetValueText(
			Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT,
			Constants.VALUETEXT_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TITLE_LTV,
			this.SearchParameters[CONST_SEARCH_PARAM_TITLE]);

		tbTargetListName.Text = string.Format(
			"{0}{1:yyyyMM}{2}{3:yyyyMM}",
			this.SearchParameters[Constants.FIELD_ORDERITEM_PRODUCT_ID],
			this.SearchParameters[SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_FIXED_PURCHASE_DATE_FROM],
			dataType,
			this.SearchParameters[CONST_SEARCH_PARAM_ORDER_DATE_TO]);

		TrimTargetListName();
		lbSourceName.Text = HtmlSanitizer.HtmlEncode(targetType);
	}

	/// <summary>
	/// 新しいターゲットリストとインポートcsvファイルを作成します
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnRegisterTop_Click(object sender, EventArgs e)
	{
		// データの入力チェックします。
		CheckInputData();

		GetSearchParameters();
		if (this.SearchParameters == null)
		{
			RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_USER_TARGET_LIST_NO_TARGET_DATA));
		}

		TrimTargetListName();

		// データcsvファイルをカウント
		var allUsers = UserTargetList.GetUserInfos(this.SessionParameters);
		var distinctUsers = TargetListUtility.GetAvailableDistinctTargetList(allUsers);
		// 新しいターゲット リストを作成します
		var newTargetId = UserTargetList.CreateNewTargetList(
			StringUtility.ToEmpty(this.SessionParameters[Constants.FIELD_TARGETLIST_TARGET_TYPE]),
			tbTargetListName.Text.Trim(),
			this.LoginOperatorDeptId,
			this.LoginOperatorName);

		// Csv ファイルを作成します。
		int totalDataCount;
		var activeFilePath = TargetListUtility.CreateImportCsvToActiveDirectory(
			this.LoginOperatorShopId,
			this.LoginOperatorDeptId,
			newTargetId,
			distinctUsers,
			string.Format(
				"{0}{1}{2:yyyyMMddHHmmss}",
				this.LoginOperatorId,
				tbTargetListName.Text.Trim(),
				DateTime.Now),
			out totalDataCount);
		if (totalDataCount == 0)
		{
			RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_USER_TARGET_LIST_NO_TARGET_DATA));
		};

		// ターゲット リストのデータ カウントを更新します。
		UserTargetList.UpdateTargetListDataCount(
			newTargetId,
			totalDataCount,
			this.LoginOperatorDeptId);

		// バッチ実行
		ExecuteBatch("\"" + activeFilePath + "\"");

		pnComplete.Visible = true;
		pnRegister.Visible = false;
	}

	/// <summary>
	/// データの入力チェックします。
	/// </summary>
	private void CheckInputData()
	{
		// ターゲットリスト名
		var errorMessage = Validator.CheckNecessaryError(
			CommonPage.ReplaceTag("@@DispText.target_list.TargetListName@@"),
			tbTargetListName.Text.Trim());

		// エラーがあれば画面遷移
		if (string.IsNullOrEmpty(errorMessage) == false)
		{
			RedirectErrorPage(errorMessage);
		}
	}

	/// <summary>
	/// エラーページにリダイレクトします。
	/// <param name="errorMessage">エラーメッセージ</param>
	/// </summary>
	private void RedirectErrorPage(string errorMessage)
	{
		Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessage;
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR)
			.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP)
			.CreateUrl();
		Response.Redirect(url);
	}

	/// <summary>
	/// バッチを実行してバッチの終了を待つ
	/// </summary>
	/// <param name="args">パラメーター</param>
	/// <returns>成功または失敗</returns>
	private void ExecuteBatch(string args)
	{
		var batchFilePath = Constants.PHYSICALDIRPATH_MASTERUPLOAD_EXE;
		if (File.Exists(batchFilePath) == false)
		{
			RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_SYSTEM_ERROR));
		}

		var exeProcess = new Process
		{
			StartInfo = {
				FileName = batchFilePath,
				Arguments = args
			},
		};
		exeProcess.Start();
	}

	/// <summary>
	/// Get search parameters
	/// </summary>
	private void GetSearchParameters()
	{
		int orderMonth;
		int pageNumber;
		int title;
		int month;
		int year;
		if ((int.TryParse(Request[Constants.REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_ORDER_MONTH], out orderMonth) == false)
			|| (int.TryParse(Request[Constants.REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_PAGE_NUMBER], out pageNumber) == false)
			|| (int.TryParse(Request[Constants.REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_TITLE], out title) == false)
			|| (int.TryParse(Request[Constants.REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_FIXEDPURCHASE_ORDER_MONTH], out month) == false)
			|| (int.TryParse(Request[Constants.REQUEST_KEY_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_FIXEDPURCHASE_ORDER_YEAR], out year) == false))
		{
			this.SearchParameters = null;
			return;
		}

		this.SearchParameters = (Hashtable)this.SessionParameters[Constants.TABLE_USER];
		var fixedPurchaseDateFrom = new DateTime(year, month, 1);
		var dateFrom = (DateTime)this.SearchParameters[Constants.SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_DATE_FROM];
		var totalMonth = orderMonth + (pageNumber - 1) * 12;
		var orderDateFrom = dateFrom.AddMonths(totalMonth);
		this.SearchParameters[CONST_SEARCH_PARAM_TITLE] = title;
		this.SearchParameters[Constants.FIELD_ORDERITEM_PRODUCT_ID] = Request[Constants.REQUEST_KEY_PRODUCT_ID];
		this.SearchParameters[Constants.FIELD_ORDERITEM_VARIATION_ID] = Request[Constants.REQUEST_KEY_VARIATION_ID];
		this.SearchParameters[SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_FIXED_PURCHASE_DATE_FROM] = fixedPurchaseDateFrom;
		this.SearchParameters[CONST_SEARCH_PARAM_ORDER_DATE_FROM] = orderDateFrom;
		this.SearchParameters[CONST_SEARCH_PARAM_ORDER_DATE_TO] = orderDateFrom.AddMonths(1).AddSeconds(-1);
		this.SearchParameters[SEARCH_PARAM_FIXED_PURCHASE_REPEATE_ANALYSIS_REPORT_FIXED_PURCHASE_DATE_TO] = fixedPurchaseDateFrom.AddMonths(1).AddSeconds(-1);
	}

	/// <summary>
	/// Trim target list name
	/// </summary>
	private void TrimTargetListName()
	{
		if (tbTargetListName.Text.Length > MAX_LENGTH_TARGET_LIST_NAME)
		{
			tbTargetListName.Text = tbTargetListName.Text.Substring(0, MAX_LENGTH_TARGET_LIST_NAME);
		}
	}

	/// <summary>Is valid session parameters</summary>
	private bool IsVaildSessionParameters
	{
		get
		{
			return (this.SessionParameters != null)
				&& ((string)this.SessionParameters[Constants.FIELD_TARGETLIST_TARGET_TYPE]
					== Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_REPEAT_ANALYSIS_REPORT);
		}
	}
	/// <summary>Session parameters</summary>
	private Hashtable SessionParameters
	{
		get { return (Hashtable)Session[Constants.SESSION_KEY_PARAM + "MP"]; }
	}
	/// <summary>Search parameters</summary>
	private Hashtable SearchParameters
	{
		get { return (Hashtable)ViewState["search_param"]; }
		set { ViewState["search_param"] = value; }
	}
}
