/*
=========================================================================================================
  Module      : メール署名設定一覧ページ処理(MailSignatureList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.App.Common.Cs.MailSignature;
using System.Web;

public partial class Form_MailSignature_MailSignatureList : BasePage
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
			// メール署名情報一覧表示
			ViewMailSignatureList();
		}
	}

	/// <summary>
	/// メール署名情報一覧表示
	/// </summary>
	private void ViewMailSignatureList()
	{
		int listCount = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST / 2;

		// メール署名情報一覧取得＆データセット
		int bgnRow = listCount * (this.RequestPageNum - 1) + 1;
		int endRow = listCount * this.RequestPageNum;
		var service = new CsMailSignatureService(new CsMailSignatureRepository());
		var models = service.Search(this.LoginOperatorDeptId, this.LoginOperatorId, bgnRow, endRow);
		rList.DataSource = models;
		rList.DataBind();

		// 件数取得、エラー表示制御
		int totalCount;
		if (models.Length != 0)
		{
			totalCount = ((CsMailSignatureModel)models[0]).EX_RowCount;
			trListError.Visible = false;
		}
		else
		{
			totalCount = 0;
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// ページャ作成
		string nextUrl = Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILSIGNATURE_LIST;
		lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.RequestPageNum, nextUrl, listCount);
	}

	/// <summary>
	/// データバインド用：詳細URL作成
	/// </summary>
	/// <param name="signatureId">メール署名ID</param>
	/// <returns>メール署名情報詳細URL</returns>
	protected string CreateDetailUrl(string signatureId)
	{
		return Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILSIGNATURE_CONFIRM
			+ "?" + Constants.REQUEST_KEY_MAIL_SIGNATURE_ID + "=" + HttpUtility.UrlEncode(signatureId)
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
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2CS_MANAGER_MAILSIGNATURE_REGISTER
			+ "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	}

	#region プロパティ
	/// <summary>リクエスト：ページ番号</summary>
	private int RequestPageNum
	{
		get
		{
			int pageNum;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNum) ? pageNum : 1;
		}
	}
	#endregion
}
