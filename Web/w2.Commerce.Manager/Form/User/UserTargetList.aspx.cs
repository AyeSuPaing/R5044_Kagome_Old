/*
=========================================================================================================
  Module      : ターゲットリスト設定登録ページ処理(UserTargetList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using w2.App.Common.TargetList;
using w2.Common.Web;

public partial class Form_User_UserTargetList : BasePage
{
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
			InitializeComponents();
		}
	}

	/// <summary>
	/// コンポーネント初期化
	/// </summary>
	private void InitializeComponents()
	{
		var usersCount = UserTargetList.GetUserCount(this.SessionParameters);
		if (usersCount == 0) RedirectErrorPage(WebMessages.GetMessages(WebMessages.ERRMSG_USER_TARGET_LIST_NO_TARGET_DATA));

		btnRegisterTop.Enabled = (usersCount > 0);
		lbDataCount.Text = usersCount.ToString();

		string targetType;
		switch (StringUtility.ToEmpty(this.SessionParameters[Constants.FIELD_TARGETLIST_TARGET_TYPE]))
		{
			case Constants.FLG_TARGETLIST_TARGET_TYPE_USER_LIST:
				//「ユーザー情報一覧」
				targetType = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER,
					Constants.VALUETEXT_PARAM_USER_TARGET_LIST,
					Constants.VALUETEXT_PARAM_USER_TARGET_TYPE_USER);
				tbTargetListName.Text = targetType;
				lbSourceName.Text = HtmlSanitizer.HtmlEncode(targetType);
				break;

			case Constants.FLG_TARGETLIST_TARGET_TYPE_ORDER_LIST:
				//「受注情報一覧」
				targetType = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER,
					Constants.VALUETEXT_PARAM_USER_TARGET_LIST,
					Constants.VALUETEXT_PARAM_USER_TARGET_TYPE_ORDER);
				tbTargetListName.Text = targetType;
				lbSourceName.Text = HtmlSanitizer.HtmlEncode(targetType);
				break;

			case Constants.FLG_TARGETLIST_TARGET_TYPE_ORDERWORKFLOW_LIST:
				//「受注ワークフロー一覧」
				targetType = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER,
					Constants.VALUETEXT_PARAM_USER_TARGET_LIST,
					Constants.VALUETEXT_PARAM_USER_TARGET_TYPE_ORDERWORKFLOW);
				tbTargetListName.Text = targetType;
				lbSourceName.Text = HtmlSanitizer.HtmlEncode(targetType);
				break;

			case Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_LIST:
				//「定期購入情報一覧」
				targetType = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER,
					Constants.VALUETEXT_PARAM_USER_TARGET_LIST,
					Constants.VALUETEXT_PARAM_USER_TARGET_TYPE_FIXEDPURCHASE);
				tbTargetListName.Text = targetType;
				lbSourceName.Text = HtmlSanitizer.HtmlEncode(targetType);
				break;

			case Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_WORKFLOW_LIST:
				//「定期ワークフロー一覧」
				targetType = ValueText.GetValueText(
					Constants.VALUETEXT_PARAM_USER,
					Constants.VALUETEXT_PARAM_USER_TARGET_LIST,
					Constants.VALUETEXT_PARAM_USER_TARGET_TYPE_FIXEDPURCHASE_WORKFLOW);
				tbTargetListName.Text = targetType;
				lbSourceName.Text = HtmlSanitizer.HtmlEncode(targetType);
				break;
		}
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
		}

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
		var errorMessage = UserTargetList.CheckInputData(tbTargetListName.Text.Trim());

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
	public void ExecuteBatch(string args)
	{
		string batchFilePath = Constants.PHYSICALDIRPATH_MASTERUPLOAD_EXE;
		if (File.Exists(batchFilePath) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_FILE_IMPORT_NOT_FOUND);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_ERROR);
		}

		var exeProcess = new Process { StartInfo = { FileName = batchFilePath, Arguments = args } };
		exeProcess.Start();
	}

	/// <summary>Session parameters</summary>
	private Hashtable SessionParameters
	{
		get { return (Hashtable)Session[Constants.SESSION_KEY_PARAM + "EC"]; }
	}
}
