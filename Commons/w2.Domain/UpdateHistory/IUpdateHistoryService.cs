/*
=========================================================================================================
  Module      : 更新履歴情報サービスのインタフェース(IUpdateHistoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;

namespace w2.Domain.UpdateHistory
{
	/// <summary>
	/// 更新履歴情報サービスのインタフェース
	/// </summary>
	public interface IUpdateHistoryService : IService
	{
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(UpdateHistoryListSearchCondition condition);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		UpdateHistoryListSearchResult[] Search(UpdateHistoryListSearchCondition condition);

		/// <summary>
		/// 取得（変更前後）
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		UpdateHistoryBeforeAndAfterSearchResult BeforeAfterSearch(UpdateHistoryBeforeAndAfterSearchCondition condition);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="updateHistoryNo">更新履歴No</param>
		/// <returns>モデル</returns>
		UpdateHistoryModel Get(long updateHistoryNo);

		/// <summary>
		/// 注文IDから全て登録
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertAllForOrder(
			string orderId,
			string lastChanged,
			SqlAccessor accessor = null);

		/// <summary>
		/// 登録（ユーザー）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		decimal InsertForUser(
			string userId,
			string lastChanged,
			SqlAccessor accessor = null);

		/// <summary>
		/// 登録（注文）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		decimal InsertEmptyForOrder(
			string orderId,
			string userId,
			string lastChanged,
			SqlAccessor accessor = null);

		/// <summary>
		/// 登録（注文）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		decimal InsertForOrder(
			string orderId,
			string lastChanged,
			SqlAccessor accessor = null);

		/// <summary>
		/// 登録（注文）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateUserId">更新ユーザーID（※ユーザー統合時に利用）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		decimal InsertForOrder(
			string orderId,
			string lastChanged,
			string updateUserId,
			SqlAccessor accessor = null);

		/// <summary>
		/// 登録（注文）
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="userCreditCard">ユーザークレジットカードモデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateUserId">更新ユーザーID（※ユーザー統合時に利用）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		decimal InsertForOrder(
			OrderModel order,
			UserCreditCardModel userCreditCard,
			string lastChanged,
			string updateUserId = null,
			SqlAccessor accessor = null);

		/// <summary>
		/// 登録（定期）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		decimal InsertForFixedPurchase(
			string fixedPurchaseId,
			string lastChanged,
			SqlAccessor accessor = null);

		/// <summary>
		/// 登録（定期）
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateUserId">更新ユーザーID（※ユーザー統合時に利用）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新履歴NO（更新しなかったら0）</returns>
		decimal InsertForFixedPurchase(
			string fixedPurchaseId,
			string lastChanged,
			string updateUserId,
			SqlAccessor accessor = null);
	}
}