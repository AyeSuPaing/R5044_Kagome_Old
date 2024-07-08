/*
=========================================================================================================
  Module      : レコメンド設定共通ページ(RecommendPage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using w2.Common.Web;
using w2.App.Common.Recommend;

/// <summary>
/// RecommendPage の概要の説明です
/// </summary>
public class RecommendPage : BasePage
{
	#region 定数
	/// <summary>登録・更新完了メッセージ表示用パラメータ名</summary>
	protected string REQUEST_KEY_SUCCESS = "success";
	#endregion

	#region メソッド
	/// <summary>
	/// レコメンド設定登録・編集URL作成
	/// </summary>
	/// <param name="recommendId">レコメンドID</param>
	/// <param name="actionStatus">アクションステータス</param>
	/// <returns>レコメンド設定登録・編集URL</returns>
	protected string CreateRecommendRegisterUrl(string recommendId, string actionStatus)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_RECOMMEND_REGISTER);
		urlCreator
			.AddParam(Constants.REQUEST_KEY_RECOMMEND_ID, recommendId)
			.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, actionStatus);

		var url = urlCreator.CreateUrl();
		return url;
	}

	/// <summary>
	/// レコメンド設定ボタン画像アップロードURL作成
	/// </summary>
	/// <param name="tempRecommendId">一時レコメンドID</param>
	/// <param name="buttonImageType">ボタン画像種別</param>
	/// <returns>レコメンド設定ボタン画像アップロードURL</returns>
	protected string CreateRecommendButtonImageFileUploadUrl(string tempRecommendId, ButtonImageType buttonImageType)
	{
		var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_RECOMMEND_BUTTONIMAGE_FILEUPLOADD);
		urlCreator
			.AddParam(Constants.REQUEST_KEY_TEMP_RECOMMEND_ID, tempRecommendId)
			.AddParam(Constants.REQUEST_KEY_RECOMMEND_BUTTONIMAGE_TYPE, buttonImageType.ToString());

		var url = urlCreator.CreateUrl();
		return url;
	}
	#endregion

	#region プロパティ
	/// <summary>リクエスト：レコメンドID</summary>
	protected string RequestRecommendId
	{
		get { return StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_RECOMMEND_ID]).Trim(); }
	}
	/// <summary>レコメンド設定一覧検索情報</summary>
	protected SearchValues SearchInfo
	{
		get { return Session[Constants.SESSIONPARAM_KEY_RECOMMEND_SEARCH_INFO] != null ? (SearchValues)Session[Constants.SESSIONPARAM_KEY_RECOMMEND_SEARCH_INFO] : new SearchValues("", "", "", "", "", 1); }
		set { Session[Constants.SESSIONPARAM_KEY_RECOMMEND_SEARCH_INFO] = value; }
	}
	/// <summary>登録・更新完了メッセージ表示？</summary>
	protected bool IsDisplaySuccess
	{
		get { return StringUtility.ToEmpty(Request[REQUEST_KEY_SUCCESS]) == "1"; }
	}
	#endregion

	#region 検索結果格納クラス
	/// <summary>検索値格納クラス</summary>
	[Serializable]
	protected class SearchValues
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="recommendName">レコメンド名（管理用）</param>
		/// <param name="recommendKbn">レコメンド区分</param>
		/// <param name="status">開催状態</param>
		/// <param name="sortKbn">並び順</param>
		/// <param name="pageNo">ページ番号</param>
		public SearchValues(string recommendId,
			string recommendName,
			string recommendKbn,
			string status,
			string sortKbn,
			int pageNum)
		{
			this.RecommendId = recommendId;
			this.RecommendName = recommendName;
			this.RecommendKbn = recommendKbn;
			this.Status = status;
			this.SortKbn = sortKbn;
			this.PageNum = pageNum;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// レコメンド設定一覧URL作成
		/// </summary>
		/// <param name="addPageNo">ページを付けるかどうかフラグ（ページャ作成の際はfalse）</param>
		/// <param name="windowKbn">ポップアップ画面化</param>
		/// <returns>レコメンド設定一覧URL</returns>
		public string CreateRecommendListUrl(bool addPageNo = true, string windowKbn = null)
		{
			var urlCreator = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_RECOMMEND_LIST);
			urlCreator
				.AddParam(Constants.REQUEST_KEY_RECOMMEND_ID, this.RecommendId)
				.AddParam(Constants.REQUEST_KEY_RECOMMEND_NAME, this.RecommendName)
				.AddParam(Constants.REQUEST_KEY_RECOMMEND_KBN, this.RecommendKbn)
				.AddParam(Constants.REQUEST_KEY_RECOMMEND_STATUS, this.Status)
				.AddParam(Constants.REQUEST_KEY_SORT_KBN, this.SortKbn);
			if (addPageNo)
			{
				urlCreator.AddParam(Constants.REQUEST_KEY_PAGE_NO, this.PageNum.ToString());
			}

			if (windowKbn == Constants.KBN_WINDOW_POPUP)
			{
				urlCreator.AddParam(Constants.REQUEST_KEY_WINDOW_KBN, Constants.KBN_WINDOW_POPUP);
			}

			var url = urlCreator.CreateUrl();
			return url;
		}
		#endregion

		#region プロパティ
		/// <summary>レコメンドID</summary>
		public string RecommendId { get; set; }
		/// <summary>レコメンド名（表示用）</summary>
		public string RecommendName { get; set; }
		/// <summary>レコメンド区分</summary>
		public string RecommendKbn { get; set; }
		/// <summary>開催状態</summary>
		public string Status { get; set; }
		/// <summary>並び順</summary>
		public string SortKbn { get; set; }
		/// <summary>ページ番号</summary>
		public int PageNum { get; set; }
		#endregion
	}
	#endregion
}