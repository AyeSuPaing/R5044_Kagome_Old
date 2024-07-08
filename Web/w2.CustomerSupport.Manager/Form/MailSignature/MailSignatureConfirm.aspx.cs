/*
=========================================================================================================
  Module      : メール署名設定確認ページ処理(MailSignatureConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.App.Common.Cs.MailSignature;

public partial class Form_MailSignature_MailSignatureConfirm : BasePage
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

			// メール署名情報表示
			DisplayMailSignature();
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
	/// メール署名情報表示
	/// </summary>
	private void DisplayMailSignature()
	{
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 登録
			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー登録
			case Constants.ACTION_STATUS_UPDATE:		// 更新
				// 処理区分チェック
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				// データ格納
				this.MailSignatureInfo = (CsMailSignatureModel)Session[Constants.SESSION_KEY_MAILSIGNATURE_INFO];

				break;

			case Constants.ACTION_STATUS_DETAIL:		// 詳細表示
				// データ取得
				var service = new CsMailSignatureService(new CsMailSignatureRepository());
				var model = service.Get(this.LoginOperatorDeptId, Request[Constants.REQUEST_KEY_MAIL_SIGNATURE_ID], this.LoginOperatorId);
				if (model == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
				// データ格納
				this.MailSignatureInfo = model;
				break;

			default:
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				break;
		}

		// 通常オペレータは自分の署名のみ編集可
		if ((this.LoginOperatorCsInfo.EX_PermitEditSignatureFlg == false) && (this.MailSignatureInfo.OwnerId != this.LoginOperatorId))
		{
			btnEditTop.Visible = false;
			btnEditBottom.Visible = false;
			btnDeleteTop.Visible = false;
			btnDeleteBottom.Visible = false;
		}
	}

	/// <summary>
	/// 編集ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnEdit_Click(object sender, System.EventArgs e)
	{
		// 必要なデータをセッションへセット
		Session[Constants.SESSION_KEY_MAILSIGNATURE_INFO] = this.MailSignatureInfo;		// メール署名情報
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;	// 処理区分

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILSIGNATURE_REGISTER
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
		Session[Constants.SESSION_KEY_MAILSIGNATURE_INFO] = this.MailSignatureInfo;			// メール署名情報
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;	// 処理区分

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILSIGNATURE_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_COPY_INSERT);
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// 削除
		CsMailSignatureService service = new CsMailSignatureService(new CsMailSignatureRepository());
		service.Delete(this.MailSignatureInfo.DeptId, this.MailSignatureInfo.MailSignatureId);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILSIGNATURE_LIST);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 登録
		CsMailSignatureService service = new CsMailSignatureService(new CsMailSignatureRepository());
		service.Register(this.MailSignatureInfo);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILSIGNATURE_LIST);
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		// 更新
		CsMailSignatureService service = new CsMailSignatureService(new CsMailSignatureRepository());
		service.Update(this.MailSignatureInfo);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILSIGNATURE_LIST);
	}

	#region プロパティ
	/// <summary>メール署名情報</summary>
	protected CsMailSignatureModel MailSignatureInfo
	{
		get { return (CsMailSignatureModel)ViewState[Constants.SESSION_KEY_MAILSIGNATURE_INFO]; }
		private set { ViewState[Constants.SESSION_KEY_MAILSIGNATURE_INFO] = value; }
	}
	#endregion
}
