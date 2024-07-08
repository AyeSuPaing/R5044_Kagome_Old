/*
=========================================================================================================
  Module      : VOC設定確認ページ処理(IncidentVocConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.App.Common.Cs.IncidentVoc;

public partial class Form_IncidentVoc_IncidentVocConfirm : BasePage
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

			// 画面設定処理
			DisplayData();
		}
	}

	/// <summary>
	/// 初期化
	/// </summary>
	private void Initialize()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 新規
			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー新規
				btnInsertTop.Visible = true;
				btnInsertBottom.Visible = true;
				trConfirm.Visible = true;
				break;

			case Constants.ACTION_STATUS_UPDATE:		// 更新
				btnUpdateTop.Visible = true;
				btnUpdateBottom.Visible = true;
				trConfirm.Visible = true;
				break;

			case Constants.ACTION_STATUS_DETAIL:		// 詳細
				btnEditTop.Visible = true;
				btnEditBottom.Visible = true;
				btnCopyInsertTop.Visible = true;
				btnCopyInsertBottom.Visible = true;
				btnDeleteTop.Visible = true;
				btnDeleteBottom.Visible = true;
				trVocId.Visible = true;
				trDateCreated.Visible = true;
				trDateChanged.Visible = true;
				trLastChanged.Visible = true;
				trDetail.Visible = true;
				break;
		}
	}

	/// <summary>
	/// 画面表示データセット
	/// </summary>
	private void DisplayData()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 登録
			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー登録
			case Constants.ACTION_STATUS_UPDATE:		// 更新
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);	// 処理区分チェック
				this.IncidentVocModel = (CsIncidentVocModel)Session[Constants.SESSION_KEY_INCIDENTVOC_INFO];	// VOC情報取得
				break;

			case Constants.ACTION_STATUS_DETAIL:		// 詳細表示
				CsIncidentVocService service = new CsIncidentVocService(new CsIncidentVocRepository());
				CsIncidentVocModel model = service.Get(this.LoginOperatorDeptId, Request[Constants.REQUEST_KEY_VOC_ID]);
				if (model == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
				this.IncidentVocModel = model;
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}

		// データバインド
		DataBind();
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// VOC情報をそのままセッションへセット
		Session[Constants.SESSION_KEY_INCIDENTVOC_INFO] = this.IncidentVocModel;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTVOC_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, System.EventArgs e)
	{
		// VOC情報をそのままセッションへセット
		Session[Constants.SESSION_KEY_INCIDENTVOC_INFO] = this.IncidentVocModel;

		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTVOC_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COPY_INSERT);
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// インシデントで利用されている場合は削除を行えないようにする
		CsIncidentVocService service = new CsIncidentVocService(new CsIncidentVocRepository());
		if (service.CheckDeletable(this.IncidentVocModel) == false)
		{
			Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_INCIDENTVOC_NO_DELETE);
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}

		// 削除
		service.Delete(this.IncidentVocModel);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTVOC_LIST);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 登録
		CsIncidentVocService service = new CsIncidentVocService(new CsIncidentVocRepository());
		service.Register(this.IncidentVocModel);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTVOC_LIST);
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		// 更新
		CsIncidentVocService service = new CsIncidentVocService(new CsIncidentVocRepository());
		service.Update(this.IncidentVocModel);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_INCIDENTVOC_LIST);
	}

	#region プロパティ
	/// <summary>インシデントVOC情報</summary>
	protected CsIncidentVocModel IncidentVocModel
	{
		get { return (CsIncidentVocModel)ViewState[Constants.SESSION_KEY_INCIDENTVOC_INFO]; }
		private set { ViewState[Constants.SESSION_KEY_INCIDENTVOC_INFO] = value; }
	}
	#endregion
}
