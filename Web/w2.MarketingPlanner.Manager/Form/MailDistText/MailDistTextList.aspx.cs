/*
=========================================================================================================
  Module      : メール配信文章一覧ページ処理(MailDistTextList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Text;

public partial class Form_MailDistText_MailDistTextList : BasePage
{

	protected DataView m_dvMailDistTextList = new DataView();

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, System.EventArgs e)
	{
		int iCurrentPage = 1;
		string strSearchKey = "";
		string strSearchWord = "";
		string strSortKbn = "";

		if (!IsPostBack)
		{
			//------------------------------------------------------
			// パラメタ取得
			//------------------------------------------------------
			strSearchKey = Request[Constants.REQUEST_KEY_SEARCH_KEY];
			strSearchWord = Request[Constants.REQUEST_KEY_SEARCH_WORD];
			strSortKbn = Request[Constants.REQUEST_KEY_SORT_KBN];

			if ((strSearchKey == null) || (strSearchWord == null) || (strSortKbn == null))
			{
				// 初期遷移の場合は、フォームのデフォルト値設定
				strSearchKey = ddlSearchKey.SelectedValue;
				strSearchWord = tbSearchWord.Text;
				strSortKbn = ddlSortKbn.SelectedValue;
			}
			else
			{
				// 初期遷移でない場合は取得したパラメタをフォームへ設定
				foreach (ListItem li in ddlSearchKey.Items)
				{
					li.Selected = (li.Value == strSearchKey);
				}
				tbSearchWord.Text = strSearchWord;
				foreach (ListItem li in ddlSortKbn.Items)
				{
					li.Selected = (li.Value == strSortKbn);
				}
			}

			try
			{
				iCurrentPage = int.Parse(Request[Constants.REQUEST_KEY_PAGE_NO]);
			}
			catch
			{
				// 変換エラーの場合、１ページ目とする
			}
		}
		else
		{
			// ポストバックの場合はフォームの値を取得
			strSearchKey = ddlSearchKey.SelectedValue;
			strSearchWord = tbSearchWord.Text;
			strSortKbn =ddlSortKbn.SelectedValue;
		}

		//------------------------------------------------------
		// 検索
		//------------------------------------------------------
		m_dvMailDistTextList = SearchMailDistTextList(iCurrentPage, strSearchKey, strSearchWord, strSortKbn);

		//------------------------------------------------------
		// 各値設定
		//------------------------------------------------------
		int iTotal = 0;
		if (m_dvMailDistTextList.Count != 0)
		{
			// 合計件数取得
			iTotal = int.Parse(m_dvMailDistTextList[0]["row_count"].ToString());

			// データソースへセット ＆ データバインド
			rMailDistTextList.DataSource = m_dvMailDistTextList;
			rMailDistTextList.DataBind();

			// 文言を隠す
			trListError.Visible = false;
		}
		else
		{
			// データソースへセット ＆ データバインド
			rMailDistTextList.DataSource = null;
			rMailDistTextList.DataBind();

			// リストが取得できなかったとき、文言表示
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// ページャ設定
		lbPager.Text = WebPager.CreateDefaultListPager(iTotal, iCurrentPage, CreateSearchUrl(ddlSearchKey.SelectedValue, tbSearchWord.Text, ddlSortKbn.SelectedValue));
	}

	/// <summary>
	/// メール文章検索
	/// </summary>
	/// <param name="iPageNum">ページ番号</param>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索文</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <returns>メール文章一覧</returns>
	private DataView SearchMailDistTextList(int iPageNum, string strSearchKey, string strSearchWord, string strSortKbn)
	{
		DataView dvResult = null;

		int iBegin = (iPageNum - 1) * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST + 1;
		int iEnd = iPageNum * Constants.CONST_DISP_CONTENTS_DEFAULT_LIST;

		//------------------------------------------------------
		// メール文章一覧を取得
		//------------------------------------------------------
		using (SqlAccessor sqlAccessor = new SqlAccessor())
		using (SqlStatement sqlStatement = new SqlStatement("MailDistText","GetMailDistTextList"))
		{
			Hashtable htInput = new Hashtable();
			htInput.Add(Constants.FIELD_MAILDISTTEXT_DEPT_ID, this.LoginOperatorDeptId);
			htInput.Add("bgn_row_num", iBegin);
			htInput.Add("end_row_num", iEnd);
			htInput.Add("srch_key", strSearchKey);
			htInput.Add("srch_word" + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(strSearchWord));
			htInput.Add("sort_kbn", strSortKbn);

			dvResult = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
		}

		return dvResult;
	}

	/// <summary>
	/// データバインド用詳細画面URL作成
	/// </summary>
	/// <param name="strMailDistTextId">メール文章ID</param>
	/// <returns>詳細画面URL</returns>
	protected string CreateDetailUrl(string strMailDistTextId)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_CONFIRM);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_MAILTEXT_ID).Append("=").Append(Server.UrlEncode(strMailDistTextId));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(Constants.ACTION_STATUS_DETAIL);

		return sbUrl.ToString();
	}

	/// <summary>
	/// 検索パラメタ付きURL作成
	/// </summary>
	/// <param name="strSearchKey">検索キー</param>
	/// <param name="strSearchWord">検索値</param>
	/// <param name="strSortKbn">ソート区分</param>
	/// <returns>ユーザポイント一覧遷移URL</returns>
	private string CreateSearchUrl(string strSearchKey, string strSearchWord, string strSortKbn)
	{
		StringBuilder sbUrl = new StringBuilder();
		sbUrl.Append(Constants.PATH_ROOT).Append(Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_LIST);
		sbUrl.Append("?").Append(Constants.REQUEST_KEY_SEARCH_KEY).Append("=").Append(Server.UrlEncode(strSearchKey));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_SEARCH_WORD).Append("=").Append(Server.UrlEncode(strSearchWord));
		sbUrl.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(Server.UrlEncode(strSortKbn));

		return sbUrl.ToString();
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnNew_Click(object sender, System.EventArgs e)
	{
		// 処理区分をセッションへ格納
		Session[Constants.SESSION_KEY_ACTION_STATUS] = Constants.ACTION_STATUS_INSERT;

		// 新規登録画面へ遷移
		Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_W2MP_MANAGER_MAILDIST_TEXT_REGISTER + "?" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + Constants.ACTION_STATUS_INSERT);
	
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, System.EventArgs e)
	{
		// 検索
		m_dvMailDistTextList = SearchMailDistTextList(1, ddlSearchKey.SelectedValue, tbSearchWord.Text, ddlSortKbn.SelectedValue);

		// 一覧のみデータバインド
		rMailDistTextList.DataSource = m_dvMailDistTextList;
		rMailDistTextList.DataBind();
	}
}