/*
=========================================================================================================
  Module      : 自動翻訳設定情報一覧ページ(AutoTranslationWordList.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Web;
using w2.Domain.AutoTranslationWord;
using w2.Domain.AutoTranslationWord.Helper;

/// <summary>
/// 自動翻訳設定情報一覧ページ
/// </summary>
public partial class Form_AutomaticTranslation_AutomaticTranslation : AutoTranslationWordPage
{
	/// <summary>The google cloud url constants</summary>
	protected const string URL_GOOGLE_TRANSLATE = "https://cloud.google.com/translate/docs/languages";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	private void Page_Load(object sender, System.EventArgs e)
	{
		if (!IsPostBack)
		{
			// 自動翻訳情報一覧表示
			DisplayAutoTranslationWordList();
		}
	}

	/// <summary>
	/// 自動翻訳情報一覧表示
	/// </summary>
	private void DisplayAutoTranslationWordList()
	{
		// 検索フォームにパラメータをセット
		tbWordBefore.Text = this.RequestWordBefore;
		tbLanguageCode.Text = this.RequestLanguageCode;

		// 自動翻訳情報一覧取得
		var searchCondition = CreateSearchCondition();
		var service = new AutoTranslationWordService();
		var totalCount = service.GetSearchHitCount(searchCondition);
		var result = service.Search(searchCondition);
		rList.DataSource = result;
		rList.DataBind();

		// 件数取得、エラー表示制御
		if (totalCount != 0)
		{
			trListError.Visible = false;
		}
		else
		{
			trListError.Visible = true;
			tdErrorMessage.InnerHtml = WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_NO_HIT_LIST);
		}

		// 自動翻訳一覧検索情報を格納
		this.SearchInfo = new SearchValues(this.RequestWordBefore, this.RequestLanguageCode, this.RequestPageNum);

		// ページャ作成
		var nextUrl = this.SearchInfo.CreateAutoTranslationWordListUrl(false);
		lbPager1.Text = WebPager.CreateDefaultListPager(totalCount, this.RequestPageNum, nextUrl);
	}

	/// <summary>
	/// 検索条件作成
	/// </summary>
	/// <returns>検索条件</returns>
	private AutoTranslationWordListSearchCondition CreateSearchCondition()
	{
		var result = new AutoTranslationWordListSearchCondition
		{
			WordBefore = tbWordBefore.Text,
			LanguageCode = tbLanguageCode.Text.Trim(),
			BgnRowNum = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * (this.RequestPageNum - 1) + 1,
			EndRowNum = Constants.CONST_DISP_CONTENTS_DEFAULT_LIST * this.RequestPageNum
		};
		return result;
	}

	/// <summary>
	/// 検索ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnSearch_Click(object sender, EventArgs e)
	{
		var searchValues = new SearchValues(tbWordBefore.Text,tbLanguageCode.Text.Trim(),1);
		Response.Redirect(searchValues.CreateAutoTranslationWordListUrl());
	}

	/// <summary>
	/// 新規登録ボタンクリック
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void btnInsert_Click(object sender, EventArgs e)
	{
		var uc = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_AUTOTRANSLATIONWORD_REGISTER);
		uc.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_INSERT);

		Response.Redirect(uc.CreateUrl());
	}

	/// <summary>
	/// 表示制限
	/// </summary>
	/// <param name="text">テキスト</param>
	/// <param name="length">テキストの長さ</param>
	/// <returns>制限されたテキスト</returns>
	public string DisplayLimit(string text,int length)
	{
		if (text.Length > length) return text.Substring(0, length) + "...";

		return text;
	}

	/// <summary>リクエスト：翻訳元ワード</summary>
	private string RequestWordBefore
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_AUTOTRANSLATION_WORD_BEFORE]).Trim(); }
	}
	/// <summary>リクエスト：言語コード</summary>
	private string RequestLanguageCode
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_AUTOTRANSLATION_LANGUAGE_CODE]).Trim(); }
	}
	/// <summary>リクエスト：ページ番号</summary>
	private int RequestPageNum
	{
		get
		{
			int pageNum;
			return int.TryParse(Request[Constants.REQUEST_KEY_PAGE_NO], out pageNum) ? pageNum : 1;
		}
	}
}
