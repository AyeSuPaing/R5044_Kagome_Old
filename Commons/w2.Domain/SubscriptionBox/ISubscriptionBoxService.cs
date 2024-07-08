/*
=========================================================================================================
  Module      : 頒布会サービスインターフェース (ISubscriptionBoxService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.SubscriptionBox
{
	/// <summary>
	/// 頒布会サービスインターフェース
	/// </summary>
	public interface ISubscriptionBoxService : IService
	{
		/// <summary>
		/// GetAll
		/// </summary>
		/// <param name="accessor"></param>
		/// <returns>List Subscription Box</returns>
		SubscriptionBoxModel[] GetAll(SqlAccessor accessor = null);

		/// <summary>
		/// 有効なものをすべて取得（子アイテム郡すべて含む）
		/// </summary>
		/// <remarks>
		/// I/O負荷高いのでキャッシュリフレッシュ専用
		/// </remarks>
		/// <returns>頒布会</returns>
		SubscriptionBoxModel[] GetValidOnesWithChildren();

		/// <summary>
		/// Get Subscription Box by course id
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="accessor"></param>
		/// <returns></returns>
		SubscriptionBoxModel GetByCourseId(string courseId, SqlAccessor accessor = null);

		/// <summary>
		/// Update SubscriptionBox
		/// </summary>
		/// <param name="model"></param>
		/// <returns></returns>
		bool Update(SubscriptionBoxModel model);

		/// <summary>
		/// Delete SubscriptionBox by course id
		/// </summary>
		/// <param name="courseId"></param>
		/// <param name="accessor"></param>
		/// <returns>Row affect</returns>
		int Delete(string courseId, SqlAccessor accessor = null);

		/// <summary>
		/// Insert new SubscriptionBox
		/// </summary>
		/// <param name="model"></param>
		bool Insert(SubscriptionBoxModel model);

		/// <summary>
		/// Get Count
		/// </summary>
		/// <param name="htSqlParam"></param>
		/// <returns></returns>
		int? GetCount(Hashtable htSqlParam);

		/// <summary>
		/// Get Valid Subscription Box
		/// </summary>
		/// <param name="accessor"></param>
		/// <returns></returns>
		SubscriptionBoxModel[] GetValidSubscriptionBox(SqlAccessor accessor = null);

		/// <summary>
		/// Get All Hanpukai
		/// </summary>
		/// <param name="htSqlParam"></param>
		/// <returns>Get list hanpukai</returns>
		DataView SearchSubscriptionBoxesAtDataView(Hashtable htSqlParam);

		/// <summary>
		/// 頒布会注文（あるいは頒布会定期）チェック
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>True：頒布会注文（あるいは頒布会定期）、False：頒布会注文（あるいは頒布会定期）以外</returns>
		bool IsSubscriptionBoxOrderOrFixedPurchase(Hashtable condition);

		/// <summary>
		/// Check Hanpukai 
		/// </summary>
		/// <param name="htSqlParam"></param>
		/// <returns></returns>
		string GetSubscriptionBoxCourseIdOfOrderOrFixedPurchase(Hashtable htSqlParam);

		/// <summary>
		/// Get Product Belong To Hanpukai IDs
		/// </summary>
		/// <param name="productId"></param>
		/// <param name="variationId"></param>
		/// <param name="shopId"></param>
		/// <returns>return list products belong to hanpukai id</returns>
		SubscriptionBoxModel[] GetAvailableSubscriptionBoxesByProductId(string productId, string variationId, string shopId);

		/// <summary>
		/// Get NameDisplay 
		/// </summary>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		/// <returns>return name displa</returns>
		string GetDisplayName(string subscriptionBoxCourseId);

		/// <summary>
		/// Get subscription box item available
		/// </summary>
		/// <returns>SubscriptionBoxCourseId</returns>
		SubscriptionBoxItemModel[] GetSubscriptionBoxItemAvailable(string subscriptionBoxCourseId, string date);

		/// <summary>
		/// Get Number of display
		/// </summary>
		/// <param name="htSqlParam"></param>
		/// <returns> return number of display</returns>
		int? GetNumberOfDisplay(Hashtable htSqlParam);

		/// <summary>
		/// Get List Products For Count Total Money
		/// </summary>
		/// <param name="htSqlParam"></param>
		/// <returns>Return list products</returns>
		DataView GetListProduct(Hashtable htSqlParam);

		/// <summary>
		/// Get Next Date
		/// </summary>
		/// <param name="htSqlParam"></param>
		/// <returns>Return Next Delivery date</returns>
		DateTime? GetNextDate(Hashtable htSqlParam);

		/// <summary>
		/// 商品IDに紐づく頒布会コースを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>頒布会コースID</returns>
		SubscriptionBoxItemModel[] GetSubscriptionItemByProductId(
			string shopId,
			string productId,
			SqlAccessor accessor = null);

		/// <summary>
		/// 商品IDに紐づく頒布会コースを取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>頒布会コースID</returns>
		SubscriptionBoxItemModel[] GetSubscriptionItemByProductVariationId(
			string shopId,
			string productId,
			string variationId,
			SqlAccessor accessor = null);
	}
}
