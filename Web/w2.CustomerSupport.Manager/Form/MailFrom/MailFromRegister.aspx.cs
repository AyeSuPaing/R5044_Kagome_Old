/*
=========================================================================================================
  Module      : メール送信元設定登録ページ処理(MailFromRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.MailFrom;

public partial class Form_MailFrom_MailFromRegister : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 初期化
			Initialize();

			// 処理区分チェック
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			// メール送信元情報表示
			ViewMailFrom();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				trRegister.Visible = true;
				break;

			case Constants.ACTION_STATUS_UPDATE:
				trEdit.Visible = true;
				break;
		}

		// 表示順ドロップダウン作成
		for (int i = 1; i <= 100; i++)
		{
			ddlDisplayOrder.Items.Add(new ListItem(i.ToString(), i.ToString()));
		}
	}

	/// <summary>
	/// メール送信元情報表示
	/// </summary>
	private void ViewMailFrom()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 新規
				break;

			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー新規
			case Constants.ACTION_STATUS_UPDATE:		// 編集

				// セッションよりメール送信元情報取得
				CsMailFromModel mailFromInfo = ((CsMailFromModel)Session[Constants.SESSION_KEY_MAILFROM_INFO]);
				tbFromDisplayName.Text = mailFromInfo.DisplayName;										// 表示名
				tbFromAddress.Text = mailFromInfo.MailAddress;											// メールアドレス
				ddlDisplayOrder.SelectedValue = mailFromInfo.DisplayOrder.ToString();					// 表示順
				cbValidFlg.Checked = (mailFromInfo.ValidFlg == Constants.FLG_CSMAILFROM_VALID_FLG_VALID);	// 有効フラグ

				// ViewStateに格納
				this.MailFromId = mailFromInfo.MailFromId;	// メール送信元ID
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}
	}

	/// <summary>
	/// 確認するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnConfirm_Click(object sender, System.EventArgs e)
	{
		// 入力情報チェック
		Hashtable inputData = CheckInputData();

		// 必要なデータをセッションへ格納
		Session[Constants.SESSION_KEY_MAILFROM_INFO] = new CsMailFromModel(inputData);	// 入力データ
		Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;				// 処理区分

		// メール送信元確認ページへ遷移			
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILFROM_CONFIRM
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + this.ActionStatus);		
	}

	/// <summary>
	/// 入力情報チェック
	/// </summary>
	/// <returns>入力情報</returns>
	private Hashtable CheckInputData()
	{
		// 変数宣言
		Hashtable inputData = new Hashtable();
		string validator = "";

		// 処理区分に応じたValidator選択
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				validator = "CsMailFromRegister";
				break;

			case Constants.ACTION_STATUS_UPDATE:
				validator = "CsMailFromModify";
				inputData.Add(Constants.FIELD_CSMAILFROM_MAIL_FROM_ID, this.MailFromId);
				break;
		}

		// 入力データ格納
		inputData.Add(Constants.FIELD_CSMAILFROM_DEPT_ID, this.LoginOperatorDeptId);			// 識別ID
		inputData.Add(Constants.FIELD_CSMAILFROM_DISPLAY_NAME, tbFromDisplayName.Text);			// 送信元表示名
		inputData.Add(Constants.FIELD_CSMAILFROM_MAIL_ADDRESS, tbFromAddress.Text);				// 送信元アドレス
		inputData.Add(Constants.FIELD_CSMAILFROM_DISPLAY_ORDER, ddlDisplayOrder.SelectedValue);	// 表示順
		inputData.Add(Constants.FIELD_CSMAILFROM_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_CSMAILFROM_VALID_FLG_VALID : Constants.FLG_CSMAILFROM_VALID_FLG_INVALID);	// 有効フラグ
		inputData.Add(Constants.FIELD_CSMAILFROM_LAST_CHANGED, this.LoginOperatorName);			// 最終更新者

		// 入力チェック＆重複チェック＆型変換
		string errorMessages = Validator.Validate(validator, inputData);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		inputData[Constants.FIELD_CSMAILFROM_DISPLAY_ORDER] = int.Parse((string)inputData[Constants.FIELD_CSMAILFROM_DISPLAY_ORDER]);	// 表示順
		return inputData;
	}

	#region プロパティ
	/// <summary>メール送信元ID</summary>
	private string MailFromId
	{
		get { return (string)ViewState["MailFromId"]; }
		set { ViewState["MailFromId"] = value; }
	}
	#endregion
}
