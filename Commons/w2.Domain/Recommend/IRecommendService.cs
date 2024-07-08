/*
=========================================================================================================
  Module      : レコメンド設定サービスのインターフェース (IRecommendService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Sql;
using w2.Domain.Recommend.Helper;

namespace w2.Domain.Recommend
{
	/// <summary>
	/// レコメンド設定サービスのインターフェース
	/// </summary>
	public interface IRecommendService : IService
	{
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		int Delete(string shopId, string recommendId);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		RecommendModel Get(string shopId, string recommendId, SqlAccessor accessor = null);

		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル列</returns>
		RecommendModel[] GetAll(string shopId);

		/// <summary>
		/// 取得（表示用）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>モデル</returns>
		RecommendContainer GetContainer(string shopId, string recommendId);

		/// <summary>
		/// 履歴枝番を採番してレコメンド表示履歴登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>履歴枝番</returns>
		int GetNewRecommendHistoryNoAndInsertRecommendHistory(RecommendHistoryModel model);

		/// <summary>
		/// レコメンド表示履歴取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="recommendHistoryNo">履歴枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>レコメンド表示履歴</returns>
		RecommendHistoryModel GetRecommendHistory(string shopId, string recommendId, string userId, int recommendHistoryNo, SqlAccessor accessor = null);

		/// <summary>
		/// Get max history no number
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="userId">User ID</param>
		/// <param name="recommendId">Recommend ID</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Max history no</returns>
		int GetMaxRecommendHistoryNo(
			string shopId,
			string userId,
			string recommendId,
			SqlAccessor accessor = null);

		/// <summary>
		/// ユーザーIDからレコメンド表示履歴を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>レコメンド表示履歴</returns>
		RecommendHistoryModel[] GetRecommendHistoryByUserId(string shopId, string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(RecommendListSearchCondition condition);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(RecommendModel model);

		/// <summary>
		/// レコメンド表示履歴登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertRecommendHistory(RecommendHistoryModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 整合性チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <returns>整合性が取れている場合true</returns>
		bool IsConsistent(string shopId, string recommendId);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		RecommendListSearchResult[] Search(RecommendListSearchCondition condition);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Update(RecommendModel model, SqlAccessor accessor = null);

		/// <summary>
		/// レコメンド表示履歴の購入フラグを購入済に更新（注文ID追加可能）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="recommendHistoryNo">履歴枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="orderId">注文ID</param>
		void UpdateBuyOrderedFlg(string shopId, string recommendId, string userId, int recommendHistoryNo, string lastChanged, string orderId = null);

		/// <summary>
		/// 適用優先順更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="recommendId">レコメンドID</param>
		/// <param name="priority">適用優先順</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int UpdatePriority(string shopId, string recommendId, int priority, string lastChanged, SqlAccessor accessor);

		/// <summary>
		/// レコメンド表示履歴更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateRecommendHistory(RecommendHistoryModel model, SqlAccessor accessor = null);
	}
}