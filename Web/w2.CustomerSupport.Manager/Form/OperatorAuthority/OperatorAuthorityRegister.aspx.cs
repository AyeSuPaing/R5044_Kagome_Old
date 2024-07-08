/*
=========================================================================================================
  Module      : オペレータ権限設定登録ページ処理(OperatorAuthorityRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Xml;
using w2.App.Common.Cs.CsOperator;

public partial class Form_OperatorAuthority_OperatorAuthorityRegister : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 処理区分チェック
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			// オペレータ権限情報表示
			DisplayOperatorAuthorityInfo();
		}
	}

	/// <summary>
	/// オペレータ権限情報表示
	/// </summary>
	private void DisplayOperatorAuthorityInfo()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
				break;

			case Constants.ACTION_STATUS_UPDATE:

				// セッションよりデータ取得
				var authInfo = (CsOperatorAuthorityModel)Session[Constants.SESSION_KEY_OPERATORAUTHORITY_INFO];
				//ViewState.Add(Constants.SESSION_KEY_OPERATORAUTHORITY_INFO, authInfo);

				// オペレータ権限情報セット
				lbOperatorAuthorityId.Text = authInfo.OperatorAuthorityId;
				tbOperatorAuthorityName.Text = authInfo.OperatorAuthorityName;
				cbPermitEdit.Checked = (authInfo.PermitEditFlg == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG_VALID);
				cbPermitMailSend.Checked = (authInfo.PermitMailSendFlg == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG_VALID);
				cbPermitApproval.Checked = (authInfo.PermitApprovalFlg == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG_VALID);
				cbPermitUnlock.Checked = (authInfo.PermitUnlockFlg == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG_VALID);
				cbPermitEditSignature.Checked = (authInfo.PermitEditSignatureFlg == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_SIGNATURE_FLG_VALID);
				cbReceiveNoAssignWarning.Checked = (authInfo.ReceiveNoAssignWarningFlg == Constants.FLG_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG_VALID);
				cbPermitPermanentDelete.Checked = 
					(authInfo.PermitPermanentDeleteFlg == Constants.FLG_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG_VALID);
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
		CsOperatorAuthorityModel authInfo = GetInput();

		// 処理区分・パラメタをセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;
		Session[Constants.SESSION_KEY_OPERATORAUTHORITY_INFO] = authInfo;

		// 確認画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_OPERATOR_AUTHORITY_CONFIRM + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + this.ActionStatus);
	}

	/// <summary>
	/// 入力情報取得
	/// </summary>
	/// <returns></returns>
	private CsOperatorAuthorityModel GetInput()
	{
		Hashtable inputData = new Hashtable();

		// 処理区分に応じてValidator選択
		string validator;
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 新規
				validator = "CsOperatorAuthorityRegister";
				break;

			case Constants.ACTION_STATUS_UPDATE:		// 変更
				validator = "CsOperatorAuthorityModity";
				inputData.Add(Constants.FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_ID, lbOperatorAuthorityId.Text);
				break;

			default:
				throw new Exception("予期せぬActionStatusが指定されました：" + this.ActionStatus);
		}

		inputData.Add(Constants.FIELD_CSOPERATORAUTHORITY_OPERATOR_AUTHORITY_NAME, tbOperatorAuthorityName.Text);
		inputData.Add(Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG, cbPermitEdit.Checked ? Constants.FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG_VALID : Constants.FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG_INVALID);
		inputData.Add(Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG, cbPermitMailSend.Checked ? Constants.FLG_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG_VALID : Constants.FLG_CSOPERATORAUTHORITY_PERMIT_MAIL_SEND_FLG_INVALID);
		inputData.Add(Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG, cbPermitApproval.Checked ? Constants.FLG_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG_VALID : Constants.FLG_CSOPERATORAUTHORITY_PERMIT_APPROVAL_FLG_INVALID);
		inputData.Add(Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG, cbPermitUnlock.Checked ? Constants.FLG_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG_VALID : Constants.FLG_CSOPERATORAUTHORITY_PERMIT_UNLOCK_FLG_INVALID);
		inputData.Add(Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_EDIT_SIGNATURE_FLG, cbPermitEditSignature.Checked ? Constants.FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG_VALID : Constants.FLG_CSOPERATORAUTHORITY_PERMIT_EDIT_FLG_INVALID);
		inputData.Add(Constants.FIELD_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG, cbReceiveNoAssignWarning.Checked ? Constants.FLG_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG_VALID : Constants.FLG_CSOPERATORAUTHORITY_RECEIVE_NO_ASSIGN_WARNING_FLG_INVALID);
		inputData.Add(
			Constants.FIELD_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG, 
			cbPermitPermanentDelete.Checked 
				? Constants.FLG_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG_VALID 
				: Constants.FLG_CSOPERATORAUTHORITY_PERMIT_PERMANENT_DELETE_FLG_INVALID);

		// 入力チェック
		string errorMsg = Validator.Validate(validator, inputData);
		if (errorMsg != "")
		{
			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMsg;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		return new CsOperatorAuthorityModel(inputData);
	}
}
