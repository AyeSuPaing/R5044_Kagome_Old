/*
=========================================================================================================
  Module      : メール送信元設定確認ページ処理(MailFromConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Cs.MailFrom;

public partial class Form_MailFrom_MailFromConfirm : BasePage
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

			// メール送信元情報表示
			DisplayMailFrom();
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
				trDateCreated.Visible = true;
				trDateChanged.Visible = true;
				trLastChanged.Visible = true;
				trDetail.Visible = true;
				break;
		}
	}

	/// <summary>
	/// メール送信元情報表示
	/// </summary>
	private void DisplayMailFrom()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 登録
			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー登録
			case Constants.ACTION_STATUS_UPDATE:		// 更新

				// 処理区分チェック
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);

				// 表示データ格納
				this.MailFromModel = (CsMailFromModel)Session[Constants.SESSION_KEY_MAILFROM_INFO];
				this.MailFromModel.DateCreated = DateTime.Now;	// 作成日にダミーデータ
				this.MailFromModel.DateChanged = DateTime.Now;	// 更新日にダミーデータ
				break;

			case Constants.ACTION_STATUS_DETAIL:		// 詳細

				// 表示データ格納
				CsMailFromService service = new CsMailFromService(new CsMailFromRepository());
				this.MailFromModel = service.Get(this.LoginOperatorDeptId, this.RequestMailFromId);
				if (this.MailFromModel == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}
	}

	/// <summary>
	/// 編集するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 必要なデータをセッションへセット
		Session[Constants.SESSION_KEY_MAILFROM_INFO] = this.MailFromModel;	// メール送信元情報
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;	// 処理区分

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILFROM_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, System.EventArgs e)
	{
		// 必要なデータをセッションへセット
		Session[Constants.SESSION_KEY_MAILFROM_INFO] = this.MailFromModel;		// メール送信元情報
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;	// 処理区分

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILFROM_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COPY_INSERT);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 登録
		CsMailFromService service = new CsMailFromService(new CsMailFromRepository());
		service.Register(this.MailFromModel);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILFROM_LIST);
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		// 更新
		CsMailFromService service = new CsMailFromService(new CsMailFromRepository());
		service.Update(this.MailFromModel);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILFROM_LIST);
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// 削除
		CsMailFromService service = new CsMailFromService(new CsMailFromRepository());
		service.Delete(this.MailFromModel.DeptId, this.MailFromModel.MailFromId);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILFROM_LIST);
	}

	#region プロパティ
	/// <summary>リクエスト：メール送信元ID</summary>
	private string RequestMailFromId
	{
		get { return Request[Constants.REQUEST_KEY_MAIL_FROM_ID]; }
	}
	/// <summary>メール送信元情報</summary>
	protected CsMailFromModel MailFromModel
	{
		get { return (CsMailFromModel)ViewState[Constants.SESSION_KEY_MAILFROM_INFO]; }
		private set { ViewState[Constants.SESSION_KEY_MAILFROM_INFO] = value; }
	}
	#endregion
}
