/*
=========================================================================================================
  Module      : 自動翻訳共通ページ(AutoTranslationWordPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Web;

/// <summary>
/// 自動翻訳共通ページ
/// </summary>
public class AutoTranslationWordPage : BasePage
{
	/// <summary>
	/// 詳細URL作成
	/// </summary>
	/// <param name="wordHashKey">ワードハッシュキー</param>
	/// <param name="languageCode">言語コード</param>
	/// <returns>url</returns>
	protected string CreateDetailUrl(string wordHashKey, string languageCode)
	{
		var url = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_AUTOTRANSLATIONWORD_REGISTER)
			.AddParam(Constants.REQUEST_KEY_AUTOTRANSLATION_WORD_HASH, wordHashKey)
			.AddParam(Constants.REQUEST_KEY_AUTOTRANSLATION_LANGUAGE_CODE, languageCode)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE)
			.CreateUrl();
		return url;
	}

	/// <summary>検索値格納クラス</summary>
	[Serializable]
	protected class SearchValues
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="wordBefore">翻訳元ワード</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="pageNum">ページ番号</param>
		public SearchValues(string wordBefore, string languageCode, int pageNum)
		{
			this.WordBefore = wordBefore;
			this.LanguageCode = languageCode;
			this.PageNum = pageNum;
		}

		#region メソッド
		/// <summary>
		/// 翻訳情報一覧URL作成
		/// </summary>
		/// <param name="addPageNo">ページを付けるかどうかフラグ（ページャ作成の際はfalse）</param>
		/// <returns>翻訳情報一覧URL作成</returns>
		public string CreateAutoTranslationWordListUrl(bool addPageNo = true)
		{
			var uc = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_AUTOTRANSLATIONWORD_LIST)
				.AddParam(Constants.REQUEST_KEY_AUTOTRANSLATION_WORD_BEFORE, this.WordBefore)
				.AddParam(Constants.REQUEST_KEY_AUTOTRANSLATION_LANGUAGE_CODE, this.LanguageCode)
				.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_UPDATE);
			if (addPageNo)
			{
				uc.AddParam(Constants.REQUEST_KEY_PAGE_NO, this.PageNum.ToString());
			}
			return uc.CreateUrl();
		}
		#endregion

		/// <summary>ページ番号</summary>
		public int PageNum { get; set; }
		/// <summary>翻訳元ワード</summary>
		public string WordBefore { get; set; }
		/// <summary>言語コーﾄﾞ</summary>
		public string LanguageCode { get; set; }
	}

	/// <summary>自動翻訳一覧検索情報</summary>
	protected SearchValues SearchInfo
	{
		get { return Session[Constants.SESSIONPARAM_KEY_AUTOTRANSLATION_WORD_SEARCH_INFO] != null ? (SearchValues)Session[Constants.SESSIONPARAM_KEY_AUTOTRANSLATION_WORD_SEARCH_INFO] : new SearchValues("", "", 1); }
		set { Session[Constants.SESSIONPARAM_KEY_AUTOTRANSLATION_WORD_SEARCH_INFO] = value; }
	}
}