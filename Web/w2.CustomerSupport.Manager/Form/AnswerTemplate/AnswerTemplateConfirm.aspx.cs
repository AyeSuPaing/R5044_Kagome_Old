/*
=========================================================================================================
  Module      : 回答例文章設定確認ページ処理(AnswerTemplateConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.App.Common.Cs.AnswerTemplate;

public partial class Form_AnswerTemplate_AnswerTemplateConfirm : BasePage
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

			// 回答例情報表示
			DisplayAnswerTemplate();
		}
	}

	/// <summary>
	/// コンポーネント初期化
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
	/// 回答例情報表示
	/// </summary>
	private void DisplayAnswerTemplate()
	{
		// 画面設定処理
		switch (this.ActionStatus)
		{
			case Constants.ACTION_STATUS_INSERT:		// 登録
			case Constants.ACTION_STATUS_COPY_INSERT:	// コピー登録
			case Constants.ACTION_STATUS_UPDATE:		// 更新
				// 処理区分チェック
				CheckActionStatus((string)Session[Constants.SESSION_KEY_ACTION_STATUS]);
				// データセット
				this.AnswerTemplateInfo = (CsAnswerTemplateModel)Session[Constants.SESSION_KEY_ANSWERTEMPLATE_INFO];
				break;

			case Constants.ACTION_STATUS_DETAIL:		// 詳細
				// データセット
				var service = new CsAnswerTemplateService(new CsAnswerTemplateRepository());
				this.AnswerTemplateInfo = service.Get(this.LoginOperatorDeptId, Request[Constants.REQUEST_KEY_ANSWER_TEMPLATE_ID]);
				if (this.AnswerTemplateInfo == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
				}
				break;

			default:
				// エラーページへ
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
		// 必要な情報をセッションへセット
		Session[Constants.SESSION_KEY_ANSWERTEMPLATE_INFO] = this.AnswerTemplateInfo;	// 回答例情報
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;		// 処理区分

		// 編集画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATE_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_UPDATE);
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, System.EventArgs e)
	{
		// 必要な情報をセッションへセット
		Session[Constants.SESSION_KEY_ANSWERTEMPLATE_INFO] = this.AnswerTemplateInfo;	// 回答例情報
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT;	// 処理区分

		// 登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATE_REGISTER
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
		var service = new CsAnswerTemplateService(new CsAnswerTemplateRepository());
		service.Delete(this.AnswerTemplateInfo);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATE_LIST);
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 登録
		var service = new CsAnswerTemplateService(new CsAnswerTemplateRepository());
		service.Register(this.AnswerTemplateInfo);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATE_LIST);
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		// 更新
		var service = new CsAnswerTemplateService(new CsAnswerTemplateRepository());
		service.Update(this.AnswerTemplateInfo);

		// 一覧画面へ戻る
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_ANSWERTEMPLATE_LIST);
	}

	#region プロパティ
	/// <summary>回答例情報</summary>
	protected CsAnswerTemplateModel AnswerTemplateInfo
	{
		get { return (CsAnswerTemplateModel)ViewState[Constants.SESSION_KEY_ANSWERTEMPLATE_INFO]; }
		private set { ViewState[Constants.SESSION_KEY_ANSWERTEMPLATE_INFO] = value; }
	}
	#endregion
}
