/*
=========================================================================================================
  Module      : メール署名設定登録ページ処理(MailSignatureRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.MailSignature;

public partial class Form_MailSignature_MailSignatureRegister : BasePage
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

			// メール署名情報表示
			DisplayMailSignature();
		}
	}

	/// <summary>
	/// 表示コンポーネント初期化
	/// </summary>
	private void Initialize()
	{
		// 表示制御
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

		// 所有者ラジオボタンリスト作成
		rblSignatureOwner.Items.Add(new ListItem(
			// 「（共通）」
			ValueText.GetValueText(
				Constants.VALUETEXT_PARAM_MAIL_SIGNATURE,
				Constants.VALUETEXT_PARAM_OWNER_ID,
				Constants.VALUETEXT_PARAM_COMMON),
			string.Empty));
		rblSignatureOwner.Items.Add(new ListItem(WebSanitizer.HtmlEncode(this.LoginOperatorName), this.LoginOperatorId));
		rblSignatureOwner.SelectedValue = "";
		if (this.LoginOperatorCsInfo.EX_PermitEditSignatureFlg == false)
		{
			rblSignatureOwner.SelectedValue = this.LoginOperatorId;
			rblSignatureOwner.Enabled = false;
		}

		// 表示順ドロップダウン作成
		for (int i = 1; i <= 100; i++)
		{
			ddlDisplayOrder.Items.Add(new ListItem(i.ToString(), i.ToString()));
		}
	}

	/// <summary>
	/// メール署名情報表示
	/// </summary>
	private void DisplayMailSignature()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 新規
				break;

			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー新規
			case Constants.ACTION_STATUS_UPDATE:		// 編集
				// 署名情報取得＆セット
				CsMailSignatureModel model = (CsMailSignatureModel)Session[Constants.SESSION_KEY_MAILSIGNATURE_INFO];
				tbSignatureTitle.Text = model.SignatureTitle;
				tbSignatureText.Text = model.SignatureText;
				rblSignatureOwner.SelectedValue = this.LoginOperatorCsInfo.EX_PermitEditSignatureFlg ? model.OwnerId : this.LoginOperatorId; // 権限なければオペレータID（コピー新規用）
				ddlDisplayOrder.SelectedValue = model.DisplayOrder.ToString();
				cbValidFlg.Checked = (model.ValidFlg == Constants.FLG_CSMAILSIGNATURE_VALID_FLG_VALID);
				// メール署名IDをViewStateに格納
				this.MailSignatureId = model.MailSignatureId;	
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
		Hashtable input = new Hashtable();
		string validator = "";

		// Validatorを選択
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
			case Constants.ACTION_STATUS_COPY_INSERT:
				validator = "CsMailSignatureRegister";
				break;

			case Constants.ACTION_STATUS_UPDATE:
				validator = "CsMailSignatureModify";
				input.Add(Constants.FIELD_CSMAILSIGNATURE_MAIL_SIGNATURE_ID, this.MailSignatureId);
				break;
		}

		// 入力データ取得
		input.Add(Constants.FIELD_CSMAILSIGNATURE_DEPT_ID, this.LoginOperatorDeptId);				// 識別ID
		input.Add(Constants.FIELD_CSMAILSIGNATURE_SIGNATURE_TITLE, tbSignatureTitle.Text);			// 署名タイトル
		input.Add(Constants.FIELD_CSMAILSIGNATURE_SIGNATURE_TEXT, tbSignatureText.Text);			// 署名本文
		input.Add(Constants.FIELD_CSMAILSIGNATURE_OWNER_ID, this.LoginOperatorCsInfo.EX_PermitEditSignatureFlg ? rblSignatureOwner.SelectedValue : LoginOperatorId);							// 所有者
		input.Add(Constants.FIELD_CSMAILSIGNATURE_DISPLAY_ORDER, ddlDisplayOrder.SelectedValue);	// 表示順
		input.Add(Constants.FIELD_CSMAILSIGNATURE_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_CSMAILSIGNATURE_VALID_FLG_VALID : Constants.FLG_CSMAILSIGNATURE_VALID_FLG_INVALID);	// 有効フラグ
		input.Add(Constants.FIELD_CSMAILSIGNATURE_LAST_CHANGED, this.LoginOperatorName);			// 最終更新者

		// 入力チェック＆重複チェック＆型変換
		string errorMessages = Validator.Validate(validator, input);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		input[Constants.FIELD_CSMAILSIGNATURE_DISPLAY_ORDER] = int.Parse((string)input[Constants.FIELD_CSMAILSIGNATURE_DISPLAY_ORDER]);

		// 必要なデータをセッションへ格納
		Session[Constants.SESSION_KEY_MAILSIGNATURE_INFO] = new CsMailSignatureModel(input);	// 入力データ
		Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;						// 処理区分

		// メール署名確認ページへ遷移			
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILSIGNATURE_CONFIRM
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + this.ActionStatus);
	}

	#region プロパティ
	/// <summary>メール署名ID</summary>
	private string MailSignatureId
	{
		get { return (string)ViewState["MailSignatureId"]; }
		set { ViewState["MailSignatureId"] = value; }
	}
	#endregion
}
