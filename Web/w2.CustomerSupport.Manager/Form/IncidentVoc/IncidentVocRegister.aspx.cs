/*
=========================================================================================================
  Module      : VOC設定登録ページ処理(IncidentVocRegister.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Web.UI.WebControls;
using w2.App.Common.Cs.IncidentVoc;

public partial class Form_IncidentVoc_IncidentVocRegister : BasePage
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
			// 初期化
			Initialize();

			// 処理区分チェック
			CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

			// 表示用値設定処理
			DisplayData();
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
	/// 画面表示データセット
	/// </summary>
	private void DisplayData()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 新規
				break;

			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー新規・編集
			case Constants.ACTION_STATUS_UPDATE:
				CsIncidentVocModel m_VocInfo = (CsIncidentVocModel)Session[Constants.SESSION_KEY_INCIDENTVOC_INFO];	// VOC情報
				lVocId.Text = m_VocInfo.VocId;
				tbVocText.Text = m_VocInfo.VocText;
				ddlDisplayOrder.SelectedValue = m_VocInfo.DisplayOrder.ToString();
				cbValidFlg.Checked = (m_VocInfo.ValidFlg == Constants.FLG_CSINCIDENTVOC_VALID_FLG_VALID);
				this.VocId = m_VocInfo.VocId;	// VOC ID
				break;

			// それ以外の場合はエラーページへ
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
	protected void btnConfirm_Click(object sender, EventArgs e)
	{
		CsIncidentVocModel parameter = GetIncidentVocModel();

		// 必要なデータをセッションへ格納
		Session[Constants.SESSION_KEY_INCIDENTVOC_INFO] = parameter;		// パラメタ
		Session[Constants.SESSION_KEY_ACTION_STATUS] = this.ActionStatus;	// 処理区分

		// VOC確認ページへ遷移			
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTVOC_CONFIRM
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + this.ActionStatus);
	}

	/// <summary>
	/// 入力データをチェックし、OKであればモデルとして取得します。
	/// </summary>
	/// <returns>インシデントVOCモデル</returns>
	private CsIncidentVocModel GetIncidentVocModel()
	{
		Hashtable parameter = new Hashtable();

		// 処理区分に応じてValidator選択
		string validator = null;
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 新規
			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー新規
				validator = "CsIncidentVocRegister";
				break;

			case Constants.ACTION_STATUS_UPDATE:		// 変更
				validator = "CsIncidentVocModify";
				parameter.Add(Constants.FIELD_CSINCIDENTVOC_VOC_ID, this.VocId);
				break;
		}

		// パラメタ格納
		parameter.Add(Constants.FIELD_CSINCIDENTVOC_DEPT_ID, this.LoginOperatorDeptId);
		parameter.Add(Constants.FIELD_CSINCIDENTVOC_VOC_TEXT, tbVocText.Text);
		parameter.Add(Constants.FIELD_CSINCIDENTVOC_DISPLAY_ORDER, ddlDisplayOrder.SelectedValue);
		parameter.Add(Constants.FIELD_CSINCIDENTVOC_VALID_FLG, cbValidFlg.Checked ? Constants.FLG_CSINCIDENTVOC_VALID_FLG_VALID : Constants.FLG_CSINCIDENTVOC_VALID_FLG_INVALID);
		parameter.Add(Constants.FIELD_CSINCIDENTVOC_LAST_CHANGED, this.LoginOperatorName);

		// 入力チェック＆重複チェック＆型変換
		string errorMessages = Validator.Validate(validator, parameter);
		if (errorMessages != "")
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = errorMessages;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
		parameter[Constants.FIELD_CSINCIDENTVOC_DISPLAY_ORDER] = int.Parse((string)parameter[Constants.FIELD_CSINCIDENTVOC_DISPLAY_ORDER]);	// 表示順

		return new CsIncidentVocModel(parameter);
	}

	#region プロパティ
	/// <summary>VOC番号</summary>
	protected string VocId
	{
		get { return (string)ViewState[Constants.FIELD_CSINCIDENTVOC_VOC_ID]; }
		private set { ViewState[Constants.FIELD_CSINCIDENTVOC_VOC_ID] = value; }
	}
	#endregion
}