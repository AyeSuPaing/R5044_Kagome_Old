/*
=========================================================================================================
  Module      : ユーザー統合共通ページ(UserIntegrationPage.cs)
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
/// UserIntegrationPage の概要の説明です
/// </summary>
public class UserIntegrationPage : BasePage
{
	#region +メソッド
	/// <summary>
	/// ユーザー統合登録URL作成
	/// </summary>
	/// <param name="userIntegrationNo">ユーザー統合No</param>
	/// <param name="actionStatus">アクションステータス</param>
	/// <returns>ユーザー統合登録URL</returns>
	public static string CreateUserIntegrationRegisterlUrl(string userIntegrationNo, string actionStatus)
	{
		return Constants.PATH_ROOT + Constants.PAGE_MANAGER_USER_INTEGRATION_REGISTER
			+ "?" + Constants.REQUEST_KEY_USERINTEGRATION_USER_INTEGRATION_NO + "=" + HttpUtility.UrlEncode(userIntegrationNo)
			+ "&" + Constants.REQUEST_KEY_ACTION_STATUS + "=" + HttpUtility.UrlEncode(actionStatus);
	}

	/// <summary>
	/// ユーザー統合履歴一覧URL作成
	/// </summary>
	/// <param name="userIntegrationNo">ユーザー統合No</param>
	/// <param name="tableName">テーブル名</param>
	/// <param name="userId">ユーザーID</param>
	/// <param name="pageNum">ページNo</param>
	/// <param name="addPageNo">ページを付けるかどうかフラグ（ページャ作成の際はfalse）</param>
	/// <returns>ユーザー統合履歴一覧URL</returns>
	protected string CreateUserIntegrationHistoryListUrl(string userIntegrationNo, string tableName, string userId, int pageNum, bool addPageNo = true)
	{
		var url = new StringBuilder();
		url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_INTEGRATION_HISTORY_LIST);
		url.Append("?").Append(Constants.REQUEST_KEY_USERINTEGRATION_USER_INTEGRATION_NO).Append("=").Append(HttpUtility.UrlEncode(userIntegrationNo));
		url.Append("&").Append(Constants.REQUEST_KEY_USERINTEGRATIONHISTORY_TABLE_NAME).Append("=").Append(HttpUtility.UrlEncode(tableName));
		url.Append("&").Append(Constants.REQUEST_KEY_USERINTEGRATION_USER_ID).Append("=").Append(HttpUtility.UrlEncode(userId));

		if (addPageNo)
		{
			url.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(HttpUtility.UrlEncode(pageNum.ToString()));
		}

		return url.ToString();
	}
	#endregion

	#region +プロパティ
	/// <summary>リクエスト：ユーザー統合No</summary>
	protected string RequestUserIntegrationNo
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_USERINTEGRATION_USER_INTEGRATION_NO]).Trim(); }
	}
	/// <summary>ユーザー統合一覧検索情報</summary>
	protected SearchValues SearchInfo
	{
		get { return Session[Constants.SESSIONPARAM_KEY_USERINTEGRATION_SEARCH_INFO] != null ? (SearchValues)Session[Constants.SESSIONPARAM_KEY_USERINTEGRATION_SEARCH_INFO] : new SearchValues("", "", "", "", "", "", "", "", "", "", 1); }
		set { Session[Constants.SESSIONPARAM_KEY_USERINTEGRATION_SEARCH_INFO] = value; }
	}
	#endregion

	#region +検索値格納クラス
	/// <summary>検索値格納クラス</summary>
	[Serializable]
	protected class SearchValues
	{
		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="status">ステータス</param>
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="name">氏名</param>
		/// <param name="nameKana">氏名かな</param>
		/// <param name="tel">電話番号</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="zip">郵便番号</param>
		/// <param name="addr">住所</param>
		/// <param name="sortKbn">並び順</param>
		/// <param name="pageNum">ページ番号</param>
		public SearchValues(string status,
			string userIntegrationNo,
			string userId,
			string name,
			string nameKana,
			string tel,
			string mailAddr,
			string zip,
			string addr,
			string sortKbn,
			int pageNum)
		{
			this.Status = status;
			this.UserIntegrationNo = userIntegrationNo;
			this.UserId = userId;
			this.Name = name;
			this.NameKana = nameKana;
			this.Tel = tel;
			this.MailAddr = mailAddr;
			this.Zip = zip;
			this.Addr = addr;
			this.SortKbn = sortKbn;
			this.PageNum = pageNum;
		}
		#endregion

		#region +メソッド
		/// <summary>
		/// ユーザー統合一覧URL作成
		/// </summary>
		/// <param name="addPageNo">ページを付けるかどうかフラグ（ページャ作成の際はfalse）</param>
		/// <returns>ユーザー統合一覧URL</returns>
		public string CreateUserIntegrationListUrl(bool addPageNo = true)
		{
			var url = new StringBuilder();
			url.Append(Constants.PATH_ROOT).Append(Constants.PAGE_MANAGER_USER_INTEGRATION_LIST);
			url.Append("?").Append(Constants.REQUEST_KEY_USERINTEGRATION_STATUS).Append("=").Append(HttpUtility.UrlEncode(this.Status));
			url.Append("&").Append(Constants.REQUEST_KEY_USERINTEGRATION_USER_INTEGRATION_NO).Append("=").Append(HttpUtility.UrlEncode(this.UserIntegrationNo));
			url.Append("&").Append(Constants.REQUEST_KEY_USERINTEGRATION_USER_ID).Append("=").Append(HttpUtility.UrlEncode(this.UserId));
			url.Append("&").Append(Constants.REQUEST_KEY_USERINTEGRATION_NAME).Append("=").Append(HttpUtility.UrlEncode(this.Name));
			url.Append("&").Append(Constants.REQUEST_KEY_USERINTEGRATION_NAME_KANA).Append("=").Append(HttpUtility.UrlEncode(this.NameKana));
			url.Append("&").Append(Constants.REQUEST_KEY_USERINTEGRATION_TEL).Append("=").Append(HttpUtility.UrlEncode(this.Tel));
			url.Append("&").Append(Constants.REQUEST_KEY_USERINTEGRATION_MAIL_ADDR).Append("=").Append(HttpUtility.UrlEncode(this.MailAddr));
			url.Append("&").Append(Constants.REQUEST_KEY_USERINTEGRATION_ZIP).Append("=").Append(HttpUtility.UrlEncode(this.Zip));
			url.Append("&").Append(Constants.REQUEST_KEY_USERINTEGRATION_ADDR).Append("=").Append(HttpUtility.UrlEncode(this.Addr));
			url.Append("&").Append(Constants.REQUEST_KEY_SORT_KBN).Append("=").Append(HttpUtility.UrlEncode(this.SortKbn));
			if (addPageNo)
			{
				url.Append("&").Append(Constants.REQUEST_KEY_PAGE_NO).Append("=").Append(HttpUtility.UrlEncode(this.PageNum.ToString()));
			}

			return url.ToString();
		}
		#endregion

		#region +プロパティ
		/// <summary>ステータス</summary>
		public string Status { get; set; }
		/// <summary>ユーザー統合No</summary>
		public string UserIntegrationNo { get; set; }
		/// <summary>ユーザID</summary>
		public string UserId { get; set; }
		/// <summary>氏名</summary>
		public string Name { get; set; }
		/// <summary>氏名かな</summary>
		public string NameKana { get; set; }
		/// <summary>電話番号</summary>
		public string Tel { get; set; }
		/// <summary>メールアドレス</summary>
		public string MailAddr { get; set; }
		/// <summary>郵便番号</summary>
		public string Zip { get; set; }
		/// <summary>住所</summary>
		public string Addr { get; set; }
		/// <summary>並び順</summary>
		public string SortKbn { get; set; }
		/// <summary>ページ番号</summary>
		public int PageNum { get; set; }
		#endregion
	}
	#endregion
}