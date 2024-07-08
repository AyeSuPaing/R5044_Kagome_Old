/*
=========================================================================================================
  Module      : ノベルティ共通ページ(NoveltyPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

/// <summary>
/// NoveltyPage の概要の説明です
/// </summary>
public class NoveltyPage : BasePage
{
	/// <summary>
	/// ノベルティ設定一覧URL作成
	/// </summary>
	/// <param name="noveltyId">ノベルティID</param>
	/// <param name="noveltyDisplayName">ノベルティ名（表示用）</param>
	/// <param name="noveltyName">ノベルティ名（管理用）</param>
	/// <param name="noveltyStatus">開催状態</param>
	/// <param name="sortKbn">並び順</param>
	/// <returns>ノベルティ設定一覧URL</returns>
	protected string CreateNoveltyListUrl(string noveltyId, string noveltyDisplayName, string noveltyName, string noveltyStatus, string sortKbn, int pageNo)
	{
		var url = new StringBuilder(CreateNoveltyListUrl(noveltyId, noveltyDisplayName, noveltyName, noveltyStatus, sortKbn));
		url.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(HttpUtility.UrlEncode(pageNo.ToString()));

		return url.ToString();
	}
	/// <summary>
	/// ノベルティ設定一覧URL作成
	/// </summary>
	/// <param name="noveltyId">ノベルティID</param>
	/// <param name="noveltyDisplayName">ノベルティ名（表示用）</param>
	/// <param name="noveltyName">ノベルティ名（管理用）</param>
	/// <param name="noveltyStatus">開催状態</param>
	/// <param name="sortKbn">並び順</param>
	/// <returns>ノベルティ設定一覧URL</returns>
	protected string CreateNoveltyListUrl(string noveltyId, string noveltyDisplayName, string noveltyName, string noveltyStatus, string sortKbn)
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_NOVELTY_LIST);
		url.Append("?").Append(Constants.REQUEST_KEY_NOVELTY_ID).Append("=").Append(HttpUtility.UrlEncode(noveltyId));
		url.Append("&").Append(Constants.REQUEST_KEY_NOVELTY_DISP_NAME).Append("=").Append(HttpUtility.UrlEncode(noveltyDisplayName));
		url.Append("&").Append(Constants.REQUEST_KEY_NOVELTY_NAME).Append("=").Append(HttpUtility.UrlEncode(noveltyName));
		url.Append("&").Append(Constants.REQUEST_KEY_NOVELTY_STATUS).Append("=").Append(HttpUtility.UrlEncode(noveltyStatus));
		url.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode(sortKbn));

		return url.ToString();
	}

	/// <summary>
	/// ノベルティ設定更新URL作成
	/// </summary>
	/// <param name="noveltyId">ノベルティID</param>
	/// <param name="actionStatus">アクションステータス</param>
	/// <returns>ノベルティ設定更新URL</returns>
	protected string CreateNoveltyRegisterUrl(string noveltyId, string actionStatus)
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_NOVELTY_REGISTER);
		url.Append("?").Append(Constants.REQUEST_KEY_NOVELTY_ID).Append("=").Append(HttpUtility.UrlEncode(noveltyId));
		url.Append("&").Append(Constants.REQUEST_KEY_ACTION_STATUS).Append("=").Append(HttpUtility.UrlEncode(actionStatus));

		return url.ToString();
	}

	#region プロパティ
	/// <summary>リクエスト：ノベルティID</summary>
	protected string RequestNoveltyId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_NOVELTY_ID]).Trim(); }
	}
	/// <summary>ノベルティ設定一覧検索情報</summary>
	protected SearchValues SearchInfo
	{
		get { return Session[Constants.SESSIONPARAM_KEY_NOVELTY_SEARCH_INFO] != null ? (SearchValues)Session[Constants.SESSIONPARAM_KEY_NOVELTY_SEARCH_INFO] : new SearchValues("", "", "", "", "", 1); }
		set { Session[Constants.SESSIONPARAM_KEY_NOVELTY_SEARCH_INFO] = value; }
	}
	/// <summary>検索値格納クラス</summary>
	[Serializable]
	protected class SearchValues
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="noveltyId">ノベルティID</param>
		/// <param name="noveltyDisplayName">ノベルティ名（表示用）</param>
		/// <param name="noveltyName">ノベルティ名（管理用）</param>
		/// <param name="noveltyStatus">開催状態</param>
		/// <param name="sortKbn">並び順</param>
		/// <param name="pageNum">ページ番号</param>
		public SearchValues(string noveltyId, string noveltyDisplayName, string noveltyName, string noveltyStatus, string sortKbn, int pageNum)
		{
			this.NoveltyId = noveltyId;
			this.NoveltyDisplayName = noveltyDisplayName;
			this.NoveltyName = noveltyName;
			this.NoveltyStatus = noveltyStatus;
			this.SortKbn = sortKbn;
			this.PageNum = pageNum;
		}
		/// <summary>ノベルティID</summary>
		public string NoveltyId { get; set; }
		/// <summary>ノベルティ名（表示用）</summary>
		public string NoveltyDisplayName { get; set; }
		/// <summary>ノベルティ名（管理用）</summary>
		public string NoveltyName { get; set; }
		/// <summary>開催状態</summary>
		public string NoveltyStatus { get; set; }
		/// <summary>並び順</summary>
		public string SortKbn { get; set; }
		/// <summary>ページ番号</summary>
		public int PageNum { get; set; }
	}
	#endregion
}