/*
=========================================================================================================
  Module      : 注文情報サービスのインタフェース(IOrderService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;
using w2.Domain.FixedPurchase;
using w2.Domain.Order.Helper;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文情報サービスのインタフェース
	/// </summary>
	public interface IOrderService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		OrderModel Get(string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// Get order by payment order id
		/// </summary>
		/// <param name="paymentOrderId">Payment order id</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>medel</returns>
		OrderModel GetOrderByPaymentOrderId(string paymentOrderId, SqlAccessor accessor = null);

		/// <summary>
		/// 返品交換含む注文情報取得（全ての注文情報を含む）
		/// </summary>
		/// <param name="orderIdOrg">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		OrderModel[] GetRelatedOrders(string orderIdOrg, SqlAccessor accessor = null);

		/// <summary>
		/// 返品交換含む注文情報取得（全ての注文情報を含む）
		/// </summary>
		/// <param name="orderIdOrg">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		DataView GetRelatedOrdersDataView(string orderIdOrg, SqlAccessor accessor = null);

		/// <summary>
		/// 返品交換含む注文ID取得
		/// </summary>
		/// <param name="orderIdOrg">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>注文ID列</returns>
		string[] GetRelatedOrderIds(string orderIdOrg, SqlAccessor accessor = null);

		/// <summary>
		/// Get Shipping
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="orderShippingNo">Order Shipping No</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Model</returns>
		OrderShippingModel GetShipping(string orderId, int orderShippingNo, SqlAccessor accessor);

		/// <summary>
		/// 配送先取得（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>Model</returns>
		OrderShippingModel[] GetShippingAll(string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// Get Order Info By ShippingCheckNo
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="shippingCheckNo">Shipping Check No</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Order Model</returns>
		OrderModel GetOrderInfoByShippingCheckNo(string orderId, string shippingCheckNo, SqlAccessor accessor);

		/// <summary>
		/// Get Orders By External Deliverty Status And Update Date
		/// </summary>
		/// <param name="deadlineDay">Deadline Day</param>
		/// <param name="shippingExternalDelivertyStatus">Shipping External Deliverty Status</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>List Order</returns>
		List<OrderShippingModel> GetOrdersByExternalDelivertyStatusAndUpdateDate(
			int deadlineDay,
			string shippingExternalDelivertyStatus,
			SqlAccessor accessor = null);

		/// <summary>
		/// Get Latest Order
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <returns>Order Model</returns>
		OrderModel GetLatestOrder(string userId);

		/// <summary>
		/// 更新ロック取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文ID</returns>
		string GetUpdLock(string orderId, SqlAccessor accessor);

		/// <summary>
		/// 注文同梱情報取得
		/// </summary>
		/// <param name="orderIds">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		OrderModel[] GetCombinedOrders(string[] orderIds, SqlAccessor accessor = null);

		/// <summary>
		/// Get order count by order workflow setting
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <returns>Order count</returns>
		int GetOrderCountByOrderWorkflowSetting(Hashtable searchParam);

		/// <summary>
		/// Get item all
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <returns>Order item models</returns>
		OrderItemModel[] GetItemAll(string orderId);

		/// <summary>
		/// Get order workflow list no pagination
		/// </summary>
		/// <param name="htSearch">Search condition</param>
		/// <returns>Dataview of order list</returns>
		DataView GetOrderWorkflowListNoPagination(Hashtable htSearch);

		/// <summary>
		/// Get delivery tran id list order workflow
		/// </summary>
		/// <param name="htSearch">Search condition</param>
		/// <returns>Dataview of delivery tran id list</returns>
		DataView GetDeliveryTranIdListOrderWorkFlow(Hashtable htSearch);

		/// <summary>
		/// 注文ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderStatus">注文ステータス</param>
		/// <param name="updateDate">更新日時</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateOrderStatus(
			string orderId,
			string orderStatus,
			DateTime updateDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 入金ステータス更新（コンビニ向け）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderPaymentStatus">入金ステータス</param>
		/// <param name="orderPaymentDate">入金日</param>
		/// <param name="cardTranId">決済連携ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns> 更新件数 </returns>
		int UpdatePaymentStatusForCvs(
			string orderId,
			string orderPaymentStatus,
			DateTime? orderPaymentDate,
			string cardTranId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 入金ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderPaymentStatus">入金ステータス</param>
		/// <param name="orderPaymentDate">入金日</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 更新件数 </returns>
		int UpdatePaymentStatus(
			string orderId,
			string orderPaymentStatus,
			DateTime? orderPaymentDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文拡張ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="extendStatusNo">注文拡張ステータスNo</param>
		/// <param name="extendStatusValue">注文拡張ステータス値</param>
		/// <param name="extendStatusDate">注文拡張ステータス更新日時</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>件数</returns>
		int UpdateOrderExtendStatus(
			string orderId,
			int extendStatusNo,
			string extendStatusValue,
			DateTime extendStatusDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 注文拡張ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="extendStatusNo">注文拡張ステータスNo</param>
		/// <param name="extendStatusValue">注文拡張ステータス値</param>
		/// <param name="extendStatusDate">注文拡張ステータス更新日時</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		int UpdateOrderExtendStatus(
			string orderId,
			int extendStatusNo,
			string extendStatusValue,
			DateTime extendStatusDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文拡張ステータス更新（※定期購入注文作成時に利用）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchase">定期購入情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="extendStatusUseMaxNo">注文拡張ステータス最大利用数</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		int UpdateOrderExtendStatusForCreateFixedPurchaseOrder(
			string orderId,
			FixedPurchaseModel fixedPurchase,
			string lastChanged,
			int extendStatusUseMaxNo,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 定期購入ID + 定期購入回数(注文時点)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="fixedPurchaseOrderCount">定期購入回数(注文時点)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		int UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
			string orderId,
			string fixedPurchaseId,
			int fixedPurchaseOrderCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 仮注文で注文が残る場合
		/// 購入回数(注文基準)を+1する
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="lastChanged">最終更新者</param>
		void UpdateOrderCountForPreOrder(OrderModel order, string lastChanged);

		/// <summary>
		/// 定期購入回数(出荷時点)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseShippedCount">定期購入回数(出荷時点)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		int UpdateFixedPurchaseShippedCount(
			string orderId,
			int fixedPurchaseShippedCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// ユーザーID更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		int UpdateUserId(
			string orderId,
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// クレジットカード枝番更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="creditBranchNo">クレジットカード枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>件数</returns>
		int UpdateCreditBranchNo(
			string orderId,
			int creditBranchNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// クレジットカード枝番更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="creditBranchNo">クレジットカード枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		int UpdateCreditBranchNo(
			string orderId,
			int creditBranchNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 決済取引情報更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateCardTransaction(
			string orderId,
			string paymentOrderId,
			string cardTranId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 決済取引ID更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		int UpdateCardTranId(
			string orderId,
			string cardTranId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 決済取引ID更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateCardTranId(
			string orderId,
			string cardTranId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// オンライン決済ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新結果</returns>
		int UpdateOnlinePaymentStatus(
			string orderId,
			string onlinePaymentStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// オンライン決済ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新結果</returns>
		int UpdateOnlinePaymentStatus(
			string orderId,
			string onlinePaymentStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 決済連携メモ追記
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="addMemoString">追記メモ文字列</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int AddPaymentMemo(
			string orderId,
			string addMemoString,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 決済取引情報＋決済連携メモ一括更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="addMemoString">追記メモ文字列</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateCardTransactionAndPaymentMemo(
			string orderId,
			string paymentOrderId,
			string cardTranId,
			string addMemoString,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// Update Order Shipping
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>更新件数</returns>
		int UpdateOrderShipping(OrderShippingModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Update Reissue Invoice Status And Payment Memo
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="extendStatusNo">Extend status no</param>
		/// <param name="addMemoString">Add memo string</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		void UpdateReissueInvoiceStatusAndPaymentMemo(
			string orderId,
			int extendStatusNo,
			string addMemoString,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// Update Reissue Invoice Status And Payment Memo
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="extendStatusNo">Extend status no</param>
		/// <param name="addMemoString">Add memo string</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql accessor</param>
		void UpdateReissueInvoiceStatusAndPaymentMemo(
			string orderId,
			int extendStatusNo,
			string addMemoString,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文商品が全て返品されているか？
		/// </summary>
		/// <param name="relatedOrders">注文（返品交換含む）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>全て返品している：true、全て返品していない：false</returns>
		bool InspectReturnAllItems(OrderModel[] relatedOrders, SqlAccessor accessor);

		/// <summary>
		/// ユーザーIDから未キャンセル注文情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデルリスト</returns>
		OrderModel[] GetUncancelledOrdersByUserId(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 注文IDから注文情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">トランザクションを内包するアクセサ(指定しなければメソッド内部でトランザクションが完結)</param>
		/// <param name="isGetOrderItem">注文商品情報を取得するか</param>
		/// <returns>モデル</returns>
		OrderModel GetOrderInfoByOrderId(string orderId, SqlAccessor accessor = null, bool isGetOrderItem = false);

		/// <summary>
		/// ユーザーIDから注文情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しなければメソッド内部でトランザクションが完結)</param>
		/// <returns>モデルリスト</returns>
		OrderModel[] GetOrdersByUserId(string userId, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// キャンセル済みでない注文があるか
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="exclusionOrderId">除外注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>キャンセル済みでない注文があるか</returns>
		bool HasUncancelledOrders(string userId, string exclusionOrderId, SqlAccessor accessor = null);

		/// <summary>
		/// ユーザーIDから未キャンセル注文情報取得（全ての注文情報を含む）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデルリスト</returns>
		OrderModel[] GetUncancelledOrderInfosByUserId(string userId);

		/// <summary>
		/// ユーザーIDから注文情報取得（全ての注文情報を含む）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しなければメソッド内部でトランザクションが完結)</param>
		/// <returns>モデルリスト</returns>
		OrderModel[] GetOrderInfosByUserId(string userId, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// ユーザーIDから注文履歴情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しなければメソッド内部でトランザクションが完結)</param>
		/// <returns>注文モデルリスト</returns>
		OrderModel[] GetOrderHistoryList(string userId, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// 注文一覧を取得(ワークフロー) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="htSearch"></param>
		/// <param name="iPageNumber"></param>
		/// <returns>注文DataView</returns>
		DataView GetOrderWorkflowListInDataView(Hashtable htSearch, int iPageNumber = 1);

		/// <summary>
		/// 注文情報を取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <returns>注文DataView</returns>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		DataView GetOrderInDataView(string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// Get Orders Return Exchange By Order Id
		/// </summary>
		/// <param name="orderId">OrderId</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>注文DataView</returns>
		DataView GetOrdersReturnExchangeByOrderId(string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// 配送先情報取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文DataView</returns>
		DataView GetOrderShippingInDataView(string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// 最大注文ID(枝番付き)取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文DataView</returns>
		string GetOrderIdForOrderReturnExchange(string orderId, SqlAccessor accessor);

		/// <summary>
		/// 注文セットプロモーション情報取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文DataView</returns>
		DataView GetOrderSetPromotionInDataView(string orderId);

		/// <summary>
		/// 注文情報取得(※子注文の返品商品も抽出する) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文DataView</returns>
		DataView GetOrderForOrderReturnExchangeInDataView(string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// 返品用の受注取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="returnExchangeKbn">返品交換区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文DataView</returns>
		DataView GetForOrderReturnInDataView(string orderId, string returnExchangeKbn, SqlAccessor accessor = null);

		/// <summary>
		/// 利用ポイント調整
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="adjustPoint">調整ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int AdjustAddPoint(
			string orderId,
			decimal adjustPoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// クーポン取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns> 注文クーポン情報 </returns>
		OrderCouponDetailInfo GetCoupon(string orderId);

		/// <summary>
		/// クーポン登録
		/// </summary>
		/// <param name="orderCoupon">注文クーポンモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 登録件数 </returns>
		int InsertCoupon(
			OrderCouponModel orderCoupon,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// クーポン更新
		/// </summary>
		/// <param name="orderCoupon">注文クーポンモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 更新件数 </returns>
		int UpdateCoupon(
			OrderCouponModel orderCoupon,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文クーポン情報削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 削除件数 </returns>
		int DeleteOrderCoupon(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// クーポン情報削除 （注文ID＆注文クーポン枝番指定）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderCouponNo">注文クーポン枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		int DeleteCouponByCouponNo(
			string orderId,
			int orderCouponNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文セットプロモーション情報削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		int DeleteSetPromotionAll(string orderId, SqlAccessor accessor);

		/// <summary>
		/// 取得（注文メール用）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>メール送信用注文情報列</returns>
		OrderModel[] GetForOrderMail(string orderId);

		/// <summary>
		/// 取得（注文メール用）
		/// </summary>
		/// <returns>メール送信用注文情報列</returns>
		OrderModel[] GetAllForOrderMail();

		/// <summary>
		/// シリアルキー付き注文取得（注文メール用）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>メール送信用シリアルキー付き注文情報列</returns>
		OrderSerialKeyForMailSend[] GetOrderSerialKeyForOrderMail(string orderId);

		/// <summary>
		/// 外部決済ステータスを「与信エラー」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentErrorMessage">外部決済エラーメッセージ</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>件数</returns>
		void UpdateExternalPaymentInfoForAuthError(
			string orderId,
			string externalPaymentErrorMessage,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 外部決済ステータスを「与信エラー」更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentErrorMessage">外部決済エラーメッセージ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		void UpdateExternalPaymentInfoForAuthError(
			string orderId,
			string externalPaymentErrorMessage,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 与信成功向け外部決済情報更新 ※与信日時は更新する
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateExternalPaymentInfoForAuthSuccess(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 外部決済ステータスを「売上確定済み」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		void UpdateExternalPaymentStatusSalesComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 外部決済ステータスを「売上確定済み」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>件数</returns>
		void UpdateExternalPaymentStatusSalesComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 外部決済ステータスを「出荷報告済み」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>件数</returns>
		void UpdateExternalPaymentStatusShipmentComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 外部決済ステータスを「出荷報告済み」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		void UpdateExternalPaymentStatusShipmentComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 外部決済ステータスを「出荷報告エラー」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentErrorMessage">外部決済エラーメッセージ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>件数</returns>
		void UpdateExternalPaymentStatusShipmentError(
			string orderId,
			string externalPaymentErrorMessage,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 外部決済ステータスを「出荷報告エラー」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentErrorMessage">外部決済エラーメッセージ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		void UpdateExternalPaymentStatusShipmentError(
			string orderId,
			string externalPaymentErrorMessage,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 外部決済情報更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentStatus">外部決済ステータス</param>
		/// <param name="updateExternalPaymentAuthDate">外部決済与信日時を更新するか（ステータスだけ戻すときは更新したくない）</param>
		/// <param name="externalPaymentAuthDate">外部決済与信日時</param>
		/// <param name="externalPaymentErrorMessage">外部決済エラーメッセージ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateExternalPaymentInfo(
			string orderId,
			string externalPaymentStatus,
			bool updateExternalPaymentAuthDate,
			DateTime? externalPaymentAuthDate,
			string externalPaymentErrorMessage,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 外部決済ステータスを「配送完了」更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateExternalPaymentStatusDeliveryComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 外部決済ステータスを「入金済み」更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		int UpdateExternalPaymentStatusPayComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 外部決済ステータスを「入金済み」更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateExternalPaymentStatusPayComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 最終与信フラグ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastAuthFlg">最終与信フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateLastAuthFlg(
			string orderId,
			string lastAuthFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// ヤマト後払い出荷報告完了更新対象の返品注文情報取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		OrderModel[] GetCvsDefUpdateShipmentCompleteTargeReturnOrder(SqlAccessor accessor = null);

		/// <summary>
		/// ヤマトクレジットカード出荷報告完了更新対象の返品注文情報取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		OrderModel[] GetCreditUpdateShipmentCompleteTargeReturnOrder(SqlAccessor accessor = null);

		/// <summary>
		/// 注文完了向け注文ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderSattus">注文ステータス</param>
		/// <param name="cardTranId">決済カード取引ID</param>
		/// <param name="creditBranchNo">クレジットカード枝番</param>
		/// <param name="orderPaymentStatus">入金ステータス</param>
		/// <param name="orderPaymentDate">入金確認日時</param>
		/// <param name="paymentMemo">決済連携メモ</param>
		/// <param name="externalPaymentStatus">外部決済ステータス</param>
		/// <param name="externalPaymentAuthDate">外部決済与信日時</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="timeout">更新時のタイムアウト（秒）</param>
		/// <param name="cardTranPass">Card tran pass</param>
		/// <param name="accessor">最終更新者</param>
		/// <param name="creditStatus">Credit status</param>
		/// <returns>更新件数</returns>
		int UpdateOrderStatusForOrderComplete(
			string orderId,
			string orderSattus,
			string cardTranId,
			int? creditBranchNo,
			string orderPaymentStatus,
			DateTime? orderPaymentDate,
			string paymentMemo,
			string externalPaymentStatus,
			DateTime? externalPaymentAuthDate,
			string lastChanged,
			string paymentOrderId,
			string onlinePaymentStatus,
			UpdateHistoryAction updateHistoryAction,
			int? timeout,
			string cardTranPass = "",
			SqlAccessor accessor = null,
			string creditStatus = "");

		/// <summary>
		/// 注文完了向け注文ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="installmentsCode">支払回数コード</param>
		/// <param name="installments">支払回数名称</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新件数</returns>
		int UpdateOrderInstallmentsCode(
			string orderId,
			string installmentsCode,
			string installments,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// 与信期限切れ注文取得
		/// </summary>
		/// <param name="targetDate">対象日</param>
		/// <param name="cardExpireDays">カード与信切れ日数</param>
		/// <param name="cvsDefExpireDays">コンビニ後払い与信切れ日数</param>
		/// <param name="amazonPayExpireDays">Amazon Pay与信切れ日数</param>
		/// <param name="paidyPayExpireDays">Paidy Pay与信切れ日数</param>
		/// <param name="linePayExpireDate">LinePay与信日</param>
		/// <param name="npafterPayExpireDays">NP After Pay与信切れ日数</param>
		/// <param name="rakutenPayExpireDays">Rakuten 与信切れ日数</param>
		/// <param name="gmoPostExpireDays">Gmo掛け払い 与信切れ日数</param>
		/// <returns>対象注文</returns>
		/// <remarks>
		/// 期限切れがX日後（最終与信日を含まない）のとき
		///
		/// 2/1与信日だった場合、
		/// 期限切れは 2/1+X
		/// 
		/// 2/1+X  の前日、2/1+X-1実行で再与信をかけるべき。2/1+X-1 → TODAY-(X-1) で 2/1がひっかかる。
		/// 
		/// 例 X=5のとき、2/7に実行すると
		///  クレジット→ 2/7-5 +1 = 与信日2/3を再与信 (8に切れる）
		/// </remarks>
		OrderModel[] GetAuthExpired(
			DateTime targetDate,
			int cardExpireDays,
			int cvsDefExpireDays,
			int amazonPayExpireDays,
			int paidyPayExpireDays,
			int linePayExpireDate,
			int npafterPayExpireDays,
			int rakutenPayExpireDays,
			int gmoPostExpireDays);

		/// <summary>
		/// 注文取得（与信用）
		/// </summary>
		/// <param name="extendStatusNumber">拡張ステータス番号</param>
		/// <returns>対象注文</returns>
		OrderModel[] GetOrderForAuth(string extendStatusNumber);

		/// <summary>
		/// 定期商品購入制限チェック用注文ID取得（類似配送先を含む）
		/// </summary>
		/// <param name="orderShipping">配送先情報</param>
		/// <param name="order">注文情報</param>
		/// <param name="orderOwner">注文者情報</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="notExistsOrderIds">結果に含めない注文ID</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>注文IDリスト</returns>
		string[] GetOrderIdForFixedProductOrderLimitCheck(
			OrderShippingModel orderShipping,
			OrderModel order,
			OrderOwnerModel orderOwner,
			string shopId,
			string productId,
			string[] notExistsOrderIds,
			SqlAccessor accessor = null);

		/// <summary>
		/// 通常商品購入制限チェック用注文ID取得（類似配送先を含む）
		/// </summary>
		/// <param name="orderShipping">配送先情報</param>
		/// <param name="order">注文情報</param>
		/// <param name="orderOwner">注文者情報</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="notExistsOrderIds">結果に含めない注文ID</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>注文IDリスト</returns>
		string[] GetOrderIdForProductOrderLimitCheck(
			OrderShippingModel orderShipping,
			OrderModel order,
			OrderOwnerModel orderOwner,
			string shopId,
			string productId,
			string[] notExistsOrderIds,
			SqlAccessor accessor = null);

		/// <summary>
		/// 注文情報更新
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateForModify(
			OrderModel order,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 管理メモ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="managementMemo">管理メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		int UpdateManagementMemo(
			string orderId,
			string managementMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 管理メモ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="managementMemo">管理メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateManagementMemo(
			string orderId,
			string managementMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 配送メモ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="shippingMemo">配送メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		int UpdateShippingMemo(
			string orderId,
			string shippingMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 配送メモ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="shippingMemo">配送メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateShippingMemo(
			string orderId,
			string shippingMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文者情報更新
		/// </summary>
		/// <param name="owner">注文者モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateOwnerForModify(OrderOwnerModel owner, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// 配送先情報更新
		/// </summary>
		/// <param name="shippings">配送先モデルリスト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateShippingForModify(OrderShippingModel[] shippings, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// 税率毎価格情報更新
		/// </summary>
		/// <param name="orderPriceInfoByTaxRates">税率毎価格モデルリスト</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateOrderPriceInfoByTaxRateModify(OrderPriceByTaxRateModel[] orderPriceInfoByTaxRates, SqlAccessor accessor);

		/// <summary>
		/// 配送伝票番号更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderShippingNo">注文配送先枝番</param>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		int UpdateOrderShippingCheckNo(
			string orderId,
			int orderShippingNo,
			string shippingCheckNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 配送伝票番号更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderShippingNo">注文配送先枝番</param>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateOrderShippingCheckNo(
			string orderId,
			int orderShippingNo,
			string shippingCheckNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 商品情報更新
		/// </summary>
		/// <param name="items">注文商品モデルリスト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateItemForModify(OrderItemModel[] items, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// セットプロモーション情報更新
		/// </summary>
		/// <param name="setPromotions">注文セットプロモーションモデルリスト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateSetPromotionForModify(
			OrderSetPromotionModel[] setPromotions,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// セットプロモーション情報更新
		/// </summary>
		/// <param name="setPromotions">注文セットプロモーションモデルリスト</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateSetPromotionForModify(OrderSetPromotionModel[] setPromotions, SqlAccessor accessor);

		/// <summary>
		/// 注文同梱可能な注文取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="possiblePaymentIds">注文同梱可能な決済種別</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="isChildOrderFixedPurchase">注文同梱の子注文が定期注文か 定期注文の場合TRUE、通常注文の場合FALSE</param>
		/// <returns>同梱可能な注文情報</returns>
		OrderModel[] GetCombinableOrder(
			string shopId,
			string userId,
			string shippingId,
			IOrderCombineSettings orderCombineSettings,
			string[] possiblePaymentIds,
			bool fixedPurchaseSeparate,
			bool isChildOrderFixedPurchase);

		/// <summary>
		/// 注文同梱可能な注文数取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="possibleCombinePaymentIds">利用可能な決済種別</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="isChildOrderFixedPurchase">注文同梱の子注文が定期注文か 定期注文の場合TRUE、通常注文の場合FALSE</param>
		/// <returns>同梱可能な注文数</returns>
		int GetCombineOrderCount(
			string userId,
			string shippingId,
			IOrderCombineSettings orderCombineSettings,
			string[] possibleCombinePaymentIds,
			bool fixedPurchaseSeparate,
			bool isChildOrderFixedPurchase);

		/// <summary>
		/// 注文同梱可能な注文取得(管理画面用)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー名</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <returns>同梱可能な注文情報</returns>
		OrderModel[] GetCombinableParentOrderWithCondition(
			string shopId,
			IOrderCombineSettings orderCombineSettings,
			bool fixedPurchaseSeparate,
			string userId,
			string userName,
			int startRowNum,
			int endRowNum);

		/// <summary>
		/// 注文IDとクーポン利用ユーザー(メールアドレスorユーザーID)から注文情報を取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="usedUserJudgeType">利用済みユーザー判定方法</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文情報</returns>
		OrderModel GetOrderByOrderIdAndCouponUseUser(string orderId, string couponUseUser, string usedUserJudgeType, SqlAccessor accessor = null);

		/// <summary>
		/// 3Dセキュア情報更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="tranId">3Dセキュア連携ID</param>
		/// <param name="authUrl">3Dセキュア認証URL</param>
		/// <param name="authKey">3Dセキュア認証キー</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		void Update3DSecureInfo(
			string orderId,
			string tranId,
			string authUrl,
			string authKey,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction);

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="execConditionFunc">実行条件(falseであれば実行しない）</param>
		/// <returns>影響を受けた件数</returns>
		int Modify(
			string orderId,
			Action<OrderModel> updateAction,
			UpdateHistoryAction updateHistoryAction,
			Func<OrderModel, bool> execConditionFunc = null);

		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="execConditionFunc">実行条件(falseであれば実行しない）</param>
		/// <returns>影響を受けた件数</returns>
		int Modify(
			string orderId,
			Action<OrderModel> updateAction,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			Func<OrderModel, bool> execConditionFunc = null);

		/// <summary>
		/// 外部連携受注IDから注文情報を取得
		/// </summary>
		/// <param name="externalOrderId">外部連携受注ID</param>
		/// <returns>注文情報</returns>
		OrderModel GetOrderByExternalOrderId(string externalOrderId);

		/// <summary>
		/// 注文情報登録
		/// </summary>
		/// <param name="model">注文情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>True: Insert order information is successed</returns>
		bool InsertOrder(OrderModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// 返品交換向け注文登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertOrderForOrderReturnExchange(Hashtable order, SqlAccessor accessor);

		/// <summary>
		/// 返品交換向け注文者情報登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertOrderOwnerForOrderReturnExchange(Hashtable order, SqlAccessor accessor);

		/// <summary>
		/// 注文セットプロモーション情報登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderSetPromotion">注文セットプロモーション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertOrderSetPromotion(Hashtable orderSetPromotion, SqlAccessor accessor);

		/// <summary>
		/// 税率毎価格情報登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderPriceByTaxRate">注文価格レート</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertOrderPriceInfoByTaxRate(Hashtable orderPriceByTaxRate, SqlAccessor accessor);

		/// <summary>
		/// 注文配送先情報追加処理 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderShipping">注文配送先情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertOrderShippingForOrderReturnExchange(Hashtable orderShipping, SqlAccessor accessor);

		/// <summary>
		/// 注文商品情報追加処理(返品用：非入力項目は元注文商品から参照) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderItem">注文アイテム</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertOrderItemForOrderReturnExchange(Hashtable orderItem, SqlAccessor accessor);

		/// <summary>
		/// 外部連携受注ID更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalOrderId">外部連携受注ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateExternalOrderId(
			string orderId,
			string externalOrderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// 外部連携取込ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalImportStatus">外部連携取込ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateExternalOrderImportStatus(
			string orderId,
			string externalImportStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// Update logi cooperation status
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="orderStatus">Order status</param>
		/// <param name="logiCooperationStatus">Logi cooperation status</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql accessor</param>
		void UpdateLogiCooperationStatus(
			string orderId,
			string orderStatus,
			string logiCooperationStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文同梱可能な注文数取得
		/// </summary>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー名</param>
		/// <returns>同梱可能な注文数</returns>
		int GetCombinableParentOrderWithConditionCount(
			IOrderCombineSettings orderCombineSettings,
			bool fixedPurchaseSeparate,
			string userId,
			string userName);

		/// <summary>
		/// 注文情報取得(注文同梱でのキャンセル用)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>キャンセル用注文情報</returns>
		DataView GetOrderForOrderCancel(string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// 注文同梱可能な注文取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="isParentOrderFixedPurchase">注文同梱の親注文が定期注文か 定期注文の場合TRUE、通常注文の場合FALSE</param>
		/// <param name="parentOrderId">注文同梱の親注文ID</param>
		/// <param name="parentPaymentKbn">注文同梱の親注文の決済種別</param>
		/// <returns>同梱可能な注文情報</returns>
		OrderModel[] GetCombinableChildOrder(
			string shopId,
			string userId,
			string shippingId,
			IOrderCombineSettings orderCombineSettings,
			bool fixedPurchaseSeparate,
			bool isParentOrderFixedPurchase,
			string parentOrderId,
			string parentPaymentKbn);

		/// <summary>
		/// モール連携ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="mallLinkStatus">モール連携ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateMallLinkStatus(
			string orderId,
			string mallLinkStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// Amazon出荷通知対象取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>Amazon出荷通知対象</returns>
		DataView GetForAmazonOrderFulfilment(string mallId, SqlAccessor accessor = null);

		/// <summary>
		/// 取込済みAmazon注文情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文情報リスト</returns>
		OrderModel[] GetImportedAmazonOrder(string condition, SqlAccessor accessor = null);

		/// <summary>
		/// 更新(受注ワークフロー用) HACK: Modifyでできるようにしたい
		/// </summary>
		/// <param name="statement">ステートメント</param>
		/// /// <param name="order">注文</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>影響を受けた件数</returns>
		int UpdateForOrderWorkflow(string statement, Hashtable order, SqlAccessor accessor = null);

		/// <summary>
		/// 出荷後変更区分更新(元注文情報) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// /// <param name="order">注文</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>影響を受けた件数</returns>
		int UpdateShippedChangedKbn(Hashtable order, SqlAccessor accessor);

		/// <summary>
		/// 返品交換ステータス更新(返品交換受付) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// /// <param name="order">注文</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>影響を受けた件数</returns>
		int UpdateOrderReturnExchangeStatusReceipt(Hashtable order, SqlAccessor accessor);

		/// <summary>
		/// 返金ステータス更新(未返金) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// /// <param name="order">注文</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>影響を受けた件数</returns>
		int UpdateOrderRepaymentStatusConfrim(Hashtable order, SqlAccessor accessor);

		/// <summary>
		/// Amazon出荷通知中更新
		/// </summary>
		/// <param name="condition">更新条件</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateAmazonShipNoteProgress(string condition, SqlAccessor accessor = null);

		/// <summary>
		/// Amazon出荷通知済み更新
		/// </summary>
		/// <param name="condition">更新条件</param>
		void UpdateAmazonShipNoteComplete(string condition);

		/// <summary>
		/// Lohaco予約注文一覧の取得
		/// </summary>
		/// <param name="mallId">LohacoモールのモールID</param>
		/// <param name="extendStatusNo">Lohaco予約注文の拡張ステタース番号</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>予約注文一覧</returns>
		OrderModel[] GetLohacoReserveOrder(string mallId, int extendStatusNo, SqlAccessor accessor = null);

		/// <summary>
		/// 定期台帳の最終注文オブジェクトを取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		OrderModel GetLastFixedPurchaseOrder(string fixedPurchaseId, SqlAccessor accessor = null);

		/// <summary>
		/// 関連注文最終値(最終請求金額など)更新
		/// </summary>
		/// <param name="orderIdOrg">更新対象注文ID</param>
		/// <param name="exceptOrderId">更新除外注文ID</param>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="lastPointUse">最終利用ポイント</param>
		/// <param name="lastPointUseYen">最終利用ポイント額</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>更新 成功/失敗</returns>
		bool UpdateRelatedOrdersLastAmount(
			string orderIdOrg,
			string exceptOrderId,
			decimal lastBilledAmount,
			decimal lastPointUse,
			decimal lastPointUseYen,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 注文情報最終値(最終請求金額など)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="lastPointUse">最終利用ポイント</param>
		/// <param name="lastPointUseYen">最終利用ポイント額</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int UpdateOrderLastAmount(
			string orderId,
			decimal lastBilledAmount,
			decimal lastPointUse,
			decimal lastPointUseYen,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// 外部決済連携ログを更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentLog">外部決済連携ログ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param> 
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新した行数</returns>
		int AppendExternalPaymentCooperationLog(
			string orderId,
			string externalPaymentLog,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// 商品同梱：過去注文回数取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="bundleIds">商品同梱ID</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品同梱情報</returns>
		Dictionary<string, int> GetOrderedCountForProductBundle(
			string userId,
			List<string> bundleIds,
			IEnumerable<string> excludeOrderIds,
			SqlAccessor accessor = null);

		/// <summary>
		/// 返品交換注文商品情報を取得
		/// </summary>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>返品交換注文商品情報</returns>
		OrderItemModel[] GetReturnExchangeOrderItems(string orderIdOrg, SqlAccessor accessor = null);

		/// <summary>
		/// 累計購入回数更新
		/// </summary>
		/// <param name="userId">ユーザーID更新</param>
		/// <param name="orderCountOld">データ移行しない分</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns></returns>
		int UpdateOrderCountByUserId(string userId, int orderCountOld, SqlAccessor accessor);

		/// <summary>
		/// 請求書更新フラグ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="invoiceBundleFlg">請求書同梱フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>成功したか</returns>
		bool UpdateInvoiceBundleFlg(string orderId, string invoiceBundleFlg, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor);

		/// <summary>
		/// 請求書更新フラグ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="invoiceBundleFlg">請求書同梱フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>成功したか</returns>
		bool UpdateInvoiceBundleFlg(string orderId, string invoiceBundleFlg, string lastChanged, SqlAccessor accessor);

		/// <summary>
		/// 領収書情報更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="receiptFlg">領収書希望フラグ</param>
		/// <param name="receiptOutputFlg">領収書出力フラグ</param>
		/// <param name="receiptAddress">宛名</param>
		/// <param name="receiptProviso">但し書き</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateOrderReceiptInfo(
			string orderId,
			string receiptFlg,
			string receiptOutputFlg,
			string receiptAddress,
			string receiptProviso,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// 注文に紐づく全ての税率毎価格情報を取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデルの配列</returns>
		List<OrderPriceByTaxRateModel> GetPriceInfoByTaxRateAll(string orderId);

		/// <summary>
		/// 交換注文返品際の税率計算
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデルの配列</returns>
		List<OrderPriceByTaxRateModel> GetTaxRateIncludeReturnExchange(string orderId);

		/// <summary>
		/// Get Related Order Items
		/// </summary>
		/// <param name="orderIdOrg">Order Id Org</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Related Order Items</returns>
		OrderItemModel[] GetRelatedOrderItems(
			string orderIdOrg,
			SqlAccessor accessor = null);

		/// <summary>
		/// Update Related Payment Order Id
		/// </summary>
		/// <param name="orderIdOrg">Order Id Org</param>
		/// <param name="exceptedOrderId">Excepted Order Id</param>
		/// <param name="paymentOrderId">Payment Order Id</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>True: If updated sucessful, otherwise: false</returns>
		bool UpdateRelatedPaymentOrderId(
			string orderIdOrg,
			string exceptedOrderId,
			string paymentOrderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// Get Last Order
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="exceptedPaymentIds">Excepted Payment Ids</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Last Order</returns>
		OrderModel GetLastOrder(
			string userId,
			string[] exceptedPaymentIds,
			SqlAccessor accessor);

		/// <summary>
		/// Get Orders Without Return Exchange And Rejection
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="exceptedOrderStatus">Excepted Order Status</param>
		/// <param name="exceptedPaymentIds">Excepted Payment Ids</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Orders Without Return Exchange And Rejection</returns>
		OrderModel[] GetOrdersWithoutReturnExchangeAndRejection(
			string userId,
			string[] exceptedOrderStatus,
			string[] exceptedPaymentIds,
			SqlAccessor accessor = null);

		/// <summary>
		/// Get Orders Last Three Month Without Return Exchange
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="exceptedOrderStatus">Excepted Order Status</param>
		/// <param name="exceptedPaymentIds">Excepted Payment Ids</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Orders Last Three Month Without Return Exchange</returns>
		OrderModel[] GetOrdersLastThreeMonthWithoutReturnExchange(
			string userId,
			string[] exceptedOrderStatus,
			string[] exceptedPaymentIds,
			SqlAccessor accessor = null);

		/// <summary>
		/// Get First Fixed Purchase Order
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		OrderModel GetFirstFixedPurchaseOrder(string fixedPurchaseId, SqlAccessor accessor = null);

		/// <summary>
		/// Get Order By Card Tran Id
		/// </summary>
		/// <param name="cardTranId">Card Tran Id</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Model</returns>
		OrderModel GetOrderByCardTranId(string cardTranId, SqlAccessor accessor = null);

		/// <summary>
		/// Get All Order Relate FixedPurchase
		/// </summary>
		/// <param name="fixedPurchaseId">fixedPurchaseId</param>
		/// <returns>Models Order</returns>
		OrderModel[] GetAllOrderRelateFixedPurchase(string fixedPurchaseId);

		/// <summary>
		/// Update Delivery Transaction Id And Relation Memo
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="deliveryTranId">Delivery Tran Id</param>
		/// <param name="relationMemo">Relation Memo</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Sql Accessor</param>
		void UpdateDeliveryTransactionIdAndRelationMemo(
			string orderId,
			string deliveryTranId,
			string relationMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// 連携メモを追記
		/// </summary>
		/// <param name="orderId">オーダーID</param>
		/// <param name="relationMemo">連携メモ</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Sql Accessor</param>
		void AppendRelationMemo(
			string orderId,
			string relationMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// Update Online Delivery Status
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="onlineDeliveryStatus">Online Delivery Status</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Sql Accessor</param>
		/// <returns>Update result</returns>
		int UpdateOnlineDeliveryStatus(
			string orderId,
			string onlineDeliveryStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// Get Orders For Line
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="fixedPurchaseId">定期台帳番号</param>
		/// <param name="offset">Offset</param>
		/// <param name="limit">Limit</param>
		/// <param name="updateAt">Update At</param>
		/// <returns>Array Order Model</returns>
		OrderModel[] GetOrdersForLine(
			string userId,
			string fixedPurchaseId,
			int offset,
			int limit,
			DateTime updateAt);

		/// <summary>
		/// Get orders by order status
		/// </summary>
		/// <param name="orderStatus">Order status</param>
		/// <returns>Orders</returns>
		OrderModel[] GetOrdersByOrderStatus(string orderStatus);

		/// <summary>
		/// 決済種別と複数の受注IDから複数の受注を取得
		/// </summary>
		/// <param name="orderIds">複数の受注ID</param>
		/// <param name="paymentKbn">決済種別</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しなければメソッド内部でトランザクションが完結)</param>
		/// <returns>モデルリスト</returns>
		OrderModel[] GetMulitpleOrdersByOrderIdsAndPaymentKbn(
			List<string> orderIds,
			string paymentKbn,
			SqlAccessor sqlAccessor = null);

		/// <summary>
		/// 交換済み注文情報取得  HACK: 例外的にDataViewを返す
		/// 表示対象の交換済み注文よりも連番の大きい注文IDの返品商品を合わせて取得する
		/// (表示対象がXXX-001ならばXXX-002以降の返品商品を取得）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>交換済み注文情報</returns>
		DataView GetExchangedOrdersInDataView(
			string shopId,
			string orderId,
			string orderIdOrg,
			SqlAccessor accessor = null);

		/// <summary>
		/// 返品交換対象IDの取得(交換注文済みかつ、注文ステータスが出荷完了、配送完了)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>交換済み注文情報</returns>
		string[] GetExchangedOrderIds(
			string shopId,
			string orderIdOrg,
			SqlAccessor accessor = null);

		/// <summary>
		/// 新しい受注から再与信した場合の外部決済ステータスやメモを更新
		/// </summary>
		/// <param name="orderOld">古い受注</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="newOrderId">新しい受注ID</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="isCombine">注文同梱かどうか</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		int UpdateExternalPaymentStatusAndMemoForReauthByNewOrder(
			OrderModel orderOld,
			string lastChanged,
			string newOrderId,
			UpdateHistoryAction updateHistoryAction,
			bool isCombine,
			SqlAccessor accessor = null);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">クーポンNo</param>
		/// <param name="couponNoOld">クーポンNo(旧)</param>
		/// <param name="lastChenged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int UpdateCouponForIntegration(
			string userId,
			string couponId,
			string couponNo,
			string couponNoOld,
			string lastChenged,
			SqlAccessor accessor = null);

		/// <summary>
		/// 仮登録ステータスか確認する
		/// </summary>
		/// <param name="orderId">Order ID</param>
		/// <param name="accessor">SQLアSクセサ</param>
		bool CheckTemporaryRegistration(string orderId, SqlAccessor accessor = null);

		/// <summary>
		/// 初回購入チェック
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="exclusionOrderId">除外注文ID</param>
		/// <param name="accessor">Sqlアクセサ</param>
		/// <returns>True:初回購入 False:初回購入ではない</returns>
		bool CheckOrderFirstBuy(string userId, string exclusionOrderId = null, SqlAccessor accessor = null);

		/// <summary>
		/// 定期商品購入回数取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期商品購入回数</returns>
		int GetFixedPurchaseItemOrderCount(string orderId, string productId, string variationId, SqlAccessor accessor = null);

		/// <summary>
		/// Update paypay sbps info
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="paymentOrderId">Payment order id</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int UpdatePaypaySBPSInfo(
			string orderId,
			string paymentOrderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// Get order payment ids for atobaraicom get authorize status
		/// </summary>
		string[] GetOrderPaymentIdsForAtobaraicomGetAuthorizeStatus();

		/// <summary>
		/// Get point grant order
		/// </summary>
		/// <param name="days">Days</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Array of order model</returns>
		OrderModel[] GetPointGrantOrder(int days, SqlAccessor accessor = null);

		/// <summary>
		/// 審査中の注文リスト取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文一覧</returns>
		OrderModel[] GetInReviewGmoOrders(SqlAccessor accessor = null);

		/// <summary>
		/// YAHOOモール注文取り込みを行う注文を取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>辞書型注文情報</returns>
		Dictionary<string, string>[] GetOrdersForYahooOrderImport(string mallId, SqlAccessor accessor = null);

		/// <summary>
		/// YAHOOモール注文取り込みした注文情報を更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>モデル</returns>
		bool AddYahooMallOrderDetail(Hashtable input, SqlAccessor accessor = null);
		
		/// <summary>
		/// YAHOOモール注文取り込みした注文者情報を更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>モデル</returns>
		bool AddYahooMallOrderOwnerDetail(Hashtable input, SqlAccessor accessor = null);

		/// <summary>
		/// YAHOOモール注文取り込みした注文配送情報を更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>モデル</returns>
		bool AddYahooMallOrderShippingDetail(Hashtable input, SqlAccessor accessor = null);

		/// <summary>
		/// Update store pickup status
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="storePickupStatus">店舗受取ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		int UpdateStorePickupSatus(
			string orderId,
			string storePickupStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null);

		/// <summary>
		/// Update store pickup status
		/// </summary>
		/// <param name="input">Input</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Number of affected cases</returns>
		int UpdateStorePickupStatus(Hashtable input, SqlAccessor accessor = null);

		/// <summary>
		/// Get order store pick up count
		/// </summary>
		/// <param name="input">Input</param>
		/// <param name="realShopIds">Real shop ids</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Number of affected cases</returns>
		int GetOrderStorePickUpCount(
			Hashtable input,
			string realShopIds = "",
			SqlAccessor accessor = null);

		/// <summary>
		/// Get orders for Letro
		/// </summary>
		/// <param name="searchInput">Search input</param>
		/// <returns>Orders for Letro</returns>
		OrderModel[] GetOrdersForLetro(Hashtable searchInput);

		/// <summary>
		/// Get order items for Letro
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <returns>Order items for Letro</returns>
		OrderItemModel[] GetOrderItemsForLetro(string orderId);
	}
}
