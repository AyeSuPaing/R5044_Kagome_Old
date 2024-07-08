/*
=========================================================================================================
  Module      : 外部リンク設定登録ページ処理(ExternalLinkPreferenceRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Domain.ExternalLink;
using w2.Common.Web;
using w2.Domain.User.Helper;
using w2.App.Common;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using System;
using w2.App.Common.MasterExport;

public partial class Form_ExternalLinkPreference_ExternalLinkPreferenceRegister : BasePage
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
			// 画面初期化
			Initialize();

			// 処理区分チェック
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			// 外部リンク情報表示
			Display();

			// 置換タグ一覧
			ReplaceTagList();

			// データバインド
			DataBind();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		// 表示制御
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 新規
			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー新規
				trRegister.Visible = true;
				break;

			case Constants.ACTION_STATUS_UPDATE:		// 編集
				trEdit.Visible = true;
				break;
		}
	}

	/// <summary>
	/// 外部リンク情報表示
	/// </summary>
	private void Display()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:
				break;

			case Constants.ACTION_STATUS_COPY_INSERT:
			case Constants.ACTION_STATUS_UPDATE:
				// データ取得/設定
				CsExternalLinkModel externalLinkPreferenceInfo = (CsExternalLinkModel)Session[Constants.SESSION_KEY_EXTERNALLINKPREFERENCE_INFO];
				tbExternalLinkTitle.Text = externalLinkPreferenceInfo.LinkTitle;												// 外部リンクタイトル
				tbExternalLinkUrl.Text = externalLinkPreferenceInfo.LinkUrl;													// 外部リンクURL
				tbExternalLinkMemo.Text = externalLinkPreferenceInfo.LinkMemo;													// 外部リンクメモ
				tbDisplayOrder.Text = externalLinkPreferenceInfo.DisplayOrder;													// 表示順
				cbValidFlg.Checked = (externalLinkPreferenceInfo.ValidFlg == Constants.FLG_CSEXTERNALLINK_VALID_FLG_VALID);		// 有効フラグ
				// ViewStateに格納
				this.LinkId = externalLinkPreferenceInfo.LinkId;
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(CreateExternalLinkUrl(Constants.PAGE_MANAGER_ERROR));
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
		var inputData = CheckInputData();

		// 必要なデータをセッションへ格納
		Session[Constants.SESSION_KEY_EXTERNALLINKPREFERENCE_INFO] = new CsExternalLinkModel(inputData);	// 入力データ
		Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;									// 処理区分


		// 外部リンク確認ページへ遷移
		Response.Redirect(CreateExternalLinkUrl(Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_CONFIRM, this.ActionStatus));
	}

	/// <summary>
	/// 入力情報チェック
	/// </summary>
	/// <returns>入力情報</returns>
	private Hashtable CheckInputData()
	{
		var inputData = new Hashtable();
		var validator = "";

		// 処理区分に応じてValidator選択
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 新規
			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー新規
				validator = "CsExternalLinkRegister";
				break;

			case Constants.ACTION_STATUS_UPDATE:		// 変更
				validator = "CsExternalLinkModify";
				inputData.Add(Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_ID, this.LinkId);	// リンクID
				break;
		}

		// 登録/更新データ格納
		inputData.Add(Constants.FIELD_CSEXTERNALLINK_DEPT_ID, this.LoginOperatorDeptId);				// 識別ID
		inputData.Add(Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_TITLE, tbExternalLinkTitle.Text.Trim());	// リンクタイトル
		inputData.Add(Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_URL, tbExternalLinkUrl.Text.Trim());		// リンクURL
		inputData.Add(Constants.FIELD_CSEXTERNALLINK_EXTERNAL_LINK_MEMO, tbExternalLinkMemo.Text.Trim());		// リンクメモ
		inputData.Add(Constants.FIELD_CSEXTERNALLINK_DISPLAY_ORDER, tbDisplayOrder.Text);				// 表示順
		inputData.Add(Constants.FIELD_CSEXTERNALLINK_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_CSEXTERNALLINK_VALID_FLG_VALID : Constants.FLG_CSEXTERNALLINK_VALID_FLG_INVALID);	// 有効フラグ
		inputData.Add(Constants.FIELD_CSEXTERNALLINK_LAST_CHANGED, this.LoginOperatorName);				// 最終更新者

		// 入力チェック＆重複チェック
		var ErrorMessages = Validator.Validate(validator, inputData);
		if (ErrorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = ErrorMessages;
			Response.Redirect(CreateExternalLinkUrl(Constants.PAGE_MANAGER_ERROR));
		}
		return inputData;
	}

	/// <summary>
	/// 置換タグ一覧
	/// </summary>
	private void ReplaceTagList()
	{
		// ユーザーマスタフィールド取得
		var masterFields = MasterExportSettingUtility.GetMasterExportSettingFieldList(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER);

		// ユーザー拡張項目取得
		var userExtendSettingList = new UserExtendSettingList(this.LoginOperatorName);
		foreach (var userExtendSetting in userExtendSettingList.Items)
		{
			var userExtend = new Hashtable
			{
				{ Constants.MASTEREXPORTSETTING_XML_NAME, userExtendSetting.SettingId },
				{ Constants.MASTEREXPORTSETTING_XML_J_NAME, userExtendSetting.SettingName },
			};
			masterFields.Add(userExtend);
		}
		// ユーザー属性マスタフィールド取得
		var userAttributeFields = MasterExportSettingUtility.GetMasterExportSettingFieldList(Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERATTRIBUTE);
		masterFields.AddRange(userAttributeFields);

		// 使用可能な置換タグ一覧
		rReplacrmentTagList.DataSource = MasterFieldSetting.RemoveMasterFields(masterFields, Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USER);
		rReplacrmentTagList.DataBind();
	}

	/// <summary>
	/// URLクリエイター
	/// </summary>
	/// <param name="buttonActionStatus">ページ</param>
	/// <param name="status">キー</param>
	/// <returns>URL</returns>
	private string CreateExternalLinkUrl(string buttonActionStatus, string status = null)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + buttonActionStatus);
		if(status != null)
		{
			url.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, status);
		}
		return url.CreateUrl();
	}
	#region プロパティ
		/// <summary>リンクID</summary>
	private string LinkId
	{
		get { return (string)ViewState["LinkId"]; }
		set { ViewState["LinkId"] = value; }
	}
	#endregion
}