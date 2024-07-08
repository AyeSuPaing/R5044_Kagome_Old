/*
=========================================================================================================
  Module      : 外部リンク設定確認ページ処理(ExternalLinkPreferenceConfirm.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Web;
using w2.Domain.ExternalLink;

public partial class Form_ExternalLinkPreference_ExternalLinkPreferenceConfirm : BasePage
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

			// 外部リンク情報表示
			Display();
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
	/// 外部リンク情報表示
	/// </summary>
	private void Display()
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
				this.ExternalLinkInfo = (CsExternalLinkModel)Session[Constants.SESSION_KEY_EXTERNALLINKPREFERENCE_INFO];
				break;

			case Constants.ACTION_STATUS_DETAIL:		// 詳細
														// データセット
				var service = new CsExternalLinkService();
				this.ExternalLinkInfo = service.Get(this.LoginOperatorDeptId, Request[Constants.REQUEST_KEY_EXTERNAL_LINK_PERFERENCE_LINK_ID]);
				if (this.ExternalLinkInfo == null)
				{
					Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_DETAIL);
					Response.Redirect(CreateExternalLinkUrl(Constants.PAGE_MANAGER_ERROR));
				}
				break;

			default:
				// エラーページへ
				Session[Constants.SESSION_KEY_ERROR_MSG] = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_IRREGULAR_PARAMETER_ERROR);
				Response.Redirect(CreateExternalLinkUrl(Constants.PAGE_MANAGER_ERROR));
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
		Session[Constants.SESSION_KEY_EXTERNALLINKPREFERENCE_INFO] = this.ExternalLinkInfo; // 外部リンク情報
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_UPDATE;		// 処理区分

		// 編集画面へ
		Response.Redirect(CreateExternalLinkUrl(
			Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_REGISTER,
			Constants.ACTION_STATUS_UPDATE));
	}

	/// <summary>
	/// コピー新規登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnCopyInsert_Click(object sender, System.EventArgs e)
	{
		// 必要な情報をセッションへセット
		Session[Constants.SESSION_KEY_EXTERNALLINKPREFERENCE_INFO] = this.ExternalLinkInfo; // 外部リンク情報
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_COPY_INSERT; // 処理区分

		// 登録画面へ
		Response.Redirect(CreateExternalLinkUrl(
			Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_REGISTER,
			Constants.ACTION_STATUS_COPY_INSERT));
	}

	/// <summary>
	/// 削除するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnDelete_Click(object sender, System.EventArgs e)
	{
		// 削除
		var service = new CsExternalLinkService();
		service.Delete(this.ExternalLinkInfo);

		// 一覧画面へ戻る
		Response.Redirect(CreateExternalLinkUrl(Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_LIST));
	}

	/// <summary>
	/// 登録するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 登録
		var service = new CsExternalLinkService();
		service.Register(this.ExternalLinkInfo);

		// 一覧画面へ戻る
		Response.Redirect(CreateExternalLinkUrl(Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_LIST));
	}

	/// <summary>
	/// 更新するボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		// 更新
		var service = new CsExternalLinkService();
		service.Update(this.ExternalLinkInfo);

		// 一覧画面へ戻る
		Response.Redirect(CreateExternalLinkUrl(Constants.PAGE_W2CS_MANAGER_EXTERNALLINKPREFERENCE_LIST));
	}

	/// <summary>
	/// URLクリエイター
	/// </summary>
	/// <param name="status">画面ステータス</param>
	/// <param name="value">アクションステータス</param>
	/// <returns>URL</returns>
	private string CreateExternalLinkUrl(string status, string value = null)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + status);
		if (value != null)
		{
			url.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, value);
		}
		return url.CreateUrl();
	}

	#region プロパティ
	/// <summary>外部リンク情報</summary>
	protected CsExternalLinkModel ExternalLinkInfo
	{
		get { return (CsExternalLinkModel)ViewState[Constants.SESSION_KEY_EXTERNALLINKPREFERENCE_INFO]; }
		private set { ViewState[Constants.SESSION_KEY_EXTERNALLINKPREFERENCE_INFO] = value; }
	}
	#endregion
}
