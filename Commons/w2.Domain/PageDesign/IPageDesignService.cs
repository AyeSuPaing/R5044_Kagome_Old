/*
=========================================================================================================
  Module      : ページデザイン ページ管理サービスのインターフェース (IPageDesignService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Sql;
using w2.Domain.PageDesign.Helper;

namespace w2.Domain.PageDesign
{
	/// <summary>
	/// ページデザイン ページ管理サービスのインターフェース
	/// </summary>
	public interface IPageDesignService : IService
	{
		/// <summary>
		/// ターゲットリストで使われているか
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>ページ管理モデル</returns>
		PageDesignModel[] CheckTargetListUsed(string targetId);

		/// <summary>
		/// グループ削除
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>s
		int DeleteGroup(long groupId, string lastChanged, SqlAccessor accessor = null);

		/// <summary>
		/// ページ削除
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int DeletePage(long pageId, SqlAccessor accessor = null);

		/// <summary>
		/// グループ全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		PageDesignGroupModel[] GetAllGroup();

		/// <summary>
		/// CMS管理のページをすべて取得
		/// </summary>
		/// <param name="accessor">アクセサ</param>
		/// <returns>XMLサイトマップ用ページモデル</returns>
		SitemapPageModel[] GetAllManagedPagesForXmlSitemap(SqlAccessor accessor = null);

		/// <summary>
		/// ページ 全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		PageDesignModel[] GetAllPage();


		/// <summary>
		/// グループ取得
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		PageDesignGroupModel GetGroup(long groupId, SqlAccessor accessor = null);

		/// <summary>
		/// ページ取得
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		PageDesignModel GetPage(long pageId, SqlAccessor accessor = null);

		/// <summary>
		/// ファイル名でページ取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>s
		PageDesignModel GetPageByFileName(string fileName, SqlAccessor accessor = null);

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(PageDesignListSearch condition);

		/// <summary>
		/// グループ登録
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>新規グループID</returns>
		int InsertGroup(PageDesignGroupModel model, SqlAccessor accessor = null);

		/// <summary>
		/// ページ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>新規ページID</returns>
		int InsertPage(PageDesignModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 存在しないグループと紐づいているページのグループIDを抽出
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		long[] NotExistGroupIds(SqlAccessor accessor = null);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		PageDesignListSearchResult[] Search(PageDesignListSearch condition);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="otherPageGroupModel">その他 グループモデル</param>
		/// <returns>検索結果列</returns>
		PageDesignListSearchGroupResult[] SearchGroup(PageDesignListSearch condition, PageDesignGroupModel otherPageGroupModel);

		/// <summary>
		/// グループ更新
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int UpdateGroup(PageDesignGroupModel model, SqlAccessor accessor = null);

		/// <summary>
		/// グループ順序 更新
		/// </summary>
		/// <param name="groupIds">グループ順序</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		void UpdateGroupSort(long[] groupIds, SqlAccessor accessor = null);

		/// <summary>
		///  管理用タイトル更新
		/// </summary>
		/// <param name="pageId">ページID</param>
		/// <param name="title">管理用タイトル</param>
		void UpdateManagementTitle(long pageId, string title);

		/// <summary>
		/// ページ更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int UpdatePage(PageDesignModel model, SqlAccessor accessor = null);

		/// <summary>
		/// ページをその他グループに移動
		/// </summary>
		/// <param name="groupId">変更対象のグループID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdatePageMoveOtherGroup(long groupId, string lastChanged, SqlAccessor accessor = null);

		/// <summary>
		/// ページ順序 更新
		/// </summary>
		/// <param name="models">モデル配列</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdatePageSort(PageDesignModel[] models, SqlAccessor accessor = null);
	}
}