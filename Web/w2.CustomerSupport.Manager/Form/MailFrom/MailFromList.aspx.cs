/*
=========================================================================================================
  Module      : メール送信元設定一覧ページ処理(MailFromList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.App.Common.Cs.MailFrom;

public partial class Form_MailFrom_MailFromList : BasePage
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
			// メール送信元情報一覧表示
			ViewMailFromList();
		}
	}

	/// <summary>
	/// メール送信元情報一覧表示
	/// </summary>
	private void ViewMailFromList()
	{
		// メール送信元情報取得＆データバインド
		int bgnRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.PageNum - 1) + 1;
		int endRow = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.PageNum;
		CsMailFromService service = new CsMailFromService(new CsMailFromRepository());
		CsMailFromModel[] models = service.Search(this.LoginOperatorDeptId, bgnRow, endRow);
		rList.DataSource = models;
		rList.DataBind();

		// エラー表示制御
		int totalCount;
		if (models.Length != 0)
		{
			totalCount = models[0].EX_RowCount;
			trListError.Visible = false;
		}
		else
		{
			totalCount = 0;
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// ページャ作成
		string nextUrl = Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILFROM_LIST;
		lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.PageNum, nextUrl);
	}

	/// <summary>
	/// データバインド用：詳細URL作成
	/// </summary>
	/// <param name="mailFromId">メール送信元ID</param>
	/// <returns>メール送信元情報詳細URL</returns>
	protected string CreateDetailUrl(string mailFromId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILFROM_CONFIRM
			+ "?" + Constants.REQUEST_KEY_MAIL_FROM_ID + "=" + mailFromId
			+ "&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_DETAIL;
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILFROM_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	#region プロパティ
	/// <summary>ページ番号</summary>
	private int PageNum
	{
		get
		{
			int pageNum;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNum) ? pageNum : 1;
		}
	}
	#endregion
}
