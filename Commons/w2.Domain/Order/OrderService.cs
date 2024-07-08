/*
=========================================================================================================
  Module      : 注文情報サービス (OrderService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.FixedPurchaseRepeatAnalysis;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.Order.Helper;
using w2.Domain.Payment;
using w2.Domain.Product;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文情報サービス
	/// </summary>
	public class OrderService : ServiceBase, IOrderService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public OrderModel Get(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetWithChilds(orderId);
				return model;
			}
		}
		#endregion

		#region +Count 注文情報の件数取得
		/// <summary>
		/// 注文情報の件数取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文情報の件数</returns>
		public int Count(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var count = repository.Count(orderId);
				return count;
			}
		}
		#endregion

		#region +Get order by payment order id
		/// <summary>
		/// Get order by payment order id
		/// </summary>
		/// <param name="paymentOrderId">Payment order id</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>medel</returns>
		public OrderModel GetOrderByPaymentOrderId(string paymentOrderId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetOrderByPaymentOrderId(paymentOrderId);

				// 注文者&配送先&商品&クーポン&セットプロモーションをセット
				SetChildModels(model, repository);

				return model;
			}
		}
		#endregion

		#region +GetRelatedOrders 返品交換含む注文情報取得（全ての注文情報を含む）
		/// <summary>
		/// 返品交換含む注文情報取得（全ての注文情報を含む）
		/// </summary>
		/// <param name="orderIdOrg">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		public OrderModel[] GetRelatedOrders(string orderIdOrg, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var models = repository.GetRelatedOrders(orderIdOrg);
				return models;
			}
		}
		#endregion

		#region +GetRelatedOrders 返品交換含む注文情報取得（全ての注文情報を含む）
		/// <summary>
		/// 返品交換含む注文情報取得（全ての注文情報を含む）
		/// </summary>
		/// <param name="orderIdOrg">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		public DataView GetRelatedOrdersDataView(string orderIdOrg, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var models = repository.GetRelatedOrdersDataView(orderIdOrg);
				return models;
			}
		}
		#endregion

		#region +GetRelatedOrderIds 返品交換含む注文ID取得
		/// <summary>
		/// 返品交換含む注文ID取得
		/// </summary>
		/// <param name="orderIdOrg">注文ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>注文ID列</returns>
		public string[] GetRelatedOrderIds(string orderIdOrg, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orderIds = repository.GetRelatedOrderIds(orderIdOrg);
				return orderIds;
			}
		}
		#endregion

		#region +Get Shipping
		/// <summary>
		/// Get Shipping
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="orderShippingNo">Order Shipping No</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Model</returns>
		public OrderShippingModel GetShipping(string orderId, int orderShippingNo, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				return repository.GetShipping(orderId, orderShippingNo);
			}
		}
		#endregion

		#region +GetShippingAll
		/// <summary>
		/// 配送先取得（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>Model</returns>
		public OrderShippingModel[] GetShippingAll(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				return repository.GetShippingAll(orderId);
			}
		}
		#endregion

		#region +Get Order Info By Shipping Check No
		/// <summary>
		/// Get Order Info By ShippingCheckNo
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="shippingCheckNo">Shipping Check No</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Order Model</returns>
		public OrderModel GetOrderInfoByShippingCheckNo(string orderId, string shippingCheckNo, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orderModels = repository.GetOrderInfoByOrderId(orderId);

				return ((orderModels.Shippings[0].ShippingCheckNo != shippingCheckNo)
					? null
					: orderModels);
			}
		}
		#endregion

		#region ~Get Orders By External Deliverty Status And Update Date
		/// <summary>
		/// Get Orders By External Deliverty Status And Update Date
		/// </summary>
		/// <param name="deadlineDay">Deadline Day</param>
		/// <param name="shippingExternalDelivertyStatus">Shipping External Deliverty Status</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>List Order</returns>
		public List<OrderShippingModel> GetOrdersByExternalDelivertyStatusAndUpdateDate(
			int deadlineDay,
			string shippingExternalDelivertyStatus,
			SqlAccessor accessor = null)
		{
			var deadlineDate = DateTime.Now.AddDays(-deadlineDay);

			using (var repository = new OrderRepository(accessor))
			{
				return repository.GetOrdersByExternalDelivertyStatusAndUpdateDate(
					deadlineDate,
					shippingExternalDelivertyStatus);
			}
		}
		#endregion

		#region +GetLatestOrder
		/// <summary>
		/// Get Latest Order
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <returns>Order Model</returns>
		public OrderModel GetLatestOrder(string userId)
		{
			using (var repository = new OrderRepository())
			{
				var order = repository.GetOrderLatest(userId);
				if (order == null) return null;

				var orderWithChilds = repository.GetWithChilds(order.OrderId);
				return orderWithChilds;
			}
		}
		#endregion

		#region +GetFirstOrder
		/// <summary>
		/// 初回購入注文を取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>初回購入注文</returns>
		public OrderModel GetFirstOrder(string userId)
		{
			using (var repository = new OrderRepository())
			{
				var order = repository.GetFirstOrder(userId);
				return order;
			}
		}
		#endregion

		#region +GetUpdLock 更新ロック取得
		/// <summary>
		/// 更新ロック取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文ID</returns>
		public string GetUpdLock(string orderId, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.GetUpdLock(orderId);
				return result;
			}
		}
		#endregion

		#region +GetAllUpdateLock 更新ロック取得
		/// <summary>
		/// 更新ロック取得
		/// ※テーブルロックのため、トランザクションのコミット・ロールバック利用必須
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文ID</returns>
		public OrderModel GetAllUpdateLock(string orderId, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.GetAllUpdateLock(orderId);
				return result;
			}
		}
		#endregion

		#region +GetCombinedOrders 注文同梱情報取得
		/// <summary>
		/// 注文同梱情報取得
		/// </summary>
		/// <param name="orderIds">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public OrderModel[] GetCombinedOrders(string[] orderIds, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetCombinedOrders(orderIds);
				return model;
			}
		}
		#endregion

		#region +SetChildModels 子モデルをセット(注文者・配送先・商品・クーポン・セットプロモーション)
		/// <summary>
		/// 子モデルをセット(注文者・配送先・商品・クーポン・セットプロモーション)
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="repository">リポジトリ</param>
		private void SetChildModels(OrderModel model, OrderRepository repository)
		{
			if (model == null) return;

			var order = repository.GetOrderInfoByOrderId(model.OrderId);

			// 注文者
			model.Owner = order.Owner;
			// 配送先
			model.Shippings = order.Shippings;
			// 商品
			model.Items = order.Items;
			// クーポン
			model.Coupons = order.Coupons;
			// セットプロモーション
			model.SetPromotions = order.SetPromotions;
			// 税率毎価格情報
			model.OrderPriceByTaxRates = order.OrderPriceByTaxRates;
		}
		#endregion

		#region +UpdateOrderStatus 注文ステータス更新
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
		public int UpdateOrderStatus(
			string orderId,
			string orderStatus,
			DateTime updateDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(model) =>
				{
					model.OrderStatus = orderStatus;
					model.DateChanged = updateDate;
					model.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdatePaymentStatusForCvs 入金ステータス更新（コンビニ向け）
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
		public int UpdatePaymentStatusForCvs(
			string orderId,
			string orderPaymentStatus,
			DateTime? orderPaymentDate,
			string cardTranId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdatePaymentStatusForCvs(
					orderId,
					orderPaymentStatus,
					orderPaymentDate,
					cardTranId,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region -UpdatePaymentStatusForCvs 入金ステータス更新（コンビニ向け）
		/// <summary>
		/// 入金ステータス更新（コンビニ向け）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderPaymentStatus">入金ステータス</param>
		/// <param name="orderPaymentDate">入金日</param>
		/// <param name="cardTranId">決済連携ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 更新件数 </returns>
		private int UpdatePaymentStatusForCvs(
			string orderId,
			string orderPaymentStatus,
			DateTime? orderPaymentDate,
			string cardTranId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				order =>
				{
					order.OrderPaymentStatus = orderPaymentStatus;
					order.OrderPaymentDate = orderPaymentDate;
					order.CardTranId = cardTranId;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor,
				order =>
					((order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE)
						|| (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)));
			return updated;
		}
		#endregion

		#region +UpdatePaymentStatus 入金ステータス更新
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
		public int UpdatePaymentStatus(
			string orderId,
			string orderPaymentStatus,
			DateTime? orderPaymentDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				order =>
				{
					order.OrderPaymentStatus = orderPaymentStatus;
					order.OrderPaymentDate = orderPaymentDate;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateOrderExtendStatus 注文拡張ステータス更新
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
		public int UpdateOrderExtendStatus(
			string orderId,
			int extendStatusNo,
			string extendStatusValue,
			DateTime extendStatusDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdateOrderExtendStatus(
					orderId,
					extendStatusNo,
					extendStatusValue,
					extendStatusDate,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();

				return updated;
			}
		}
		#endregion
		#region +UpdateOrderExtendStatus 注文拡張ステータス更新
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
		public int UpdateOrderExtendStatus(
			string orderId,
			int extendStatusNo,
			string extendStatusValue,
			DateTime extendStatusDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.ExtendStatus[extendStatusNo - 1].Value = extendStatusValue;
					order.ExtendStatus[extendStatusNo - 1].Date = extendStatusDate;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateOrderExtendStatusForCreateFixedPurchaseOrder 注文拡張ステータス更新（※定期購入注文作成時に利用）
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
		public int UpdateOrderExtendStatusForCreateFixedPurchaseOrder(
			string orderId,
			FixedPurchaseModel fixedPurchase,
			string lastChanged,
			int extendStatusUseMaxNo,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					for (var extendStatusNo = 1; extendStatusNo <= extendStatusUseMaxNo; extendStatusNo++)
					{
						if (fixedPurchase.ExtendStatus[extendStatusNo - 1].IsOn == false) continue;

						order.ExtendStatus[extendStatusNo - 1].Value = Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON;
						order.ExtendStatus[extendStatusNo - 1].Date = DateTime.Now.Date;
					}
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateFixedPurchaseIdAndFixedPurchaseOrderCount 定期購入ID + 定期購入回数(注文時点)更新
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
		public int UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
			string orderId,
			string fixedPurchaseId,
			int fixedPurchaseOrderCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.FixedPurchaseId = fixedPurchaseId;
					order.FixedPurchaseOrderCount = fixedPurchaseOrderCount;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region UpdateOrderCountForPreOrder　定期仮注文作成時　定期購入回数(注文基準)更新
		/// <summary>
		/// 仮注文で注文が残る場合
		/// 購入回数(注文基準)を+1する
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="lastChanged">最終更新者</param>
		public void UpdateOrderCountForPreOrder(OrderModel order, string lastChanged)
		{
			// 定期注文ではない
			// または、仮注文で残らなかった場合はスキップ
			if ((order.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP)
				|| string.IsNullOrEmpty(order.FixedPurchaseId)) return;

			var fixedPurchaseService = new FixedPurchaseService();
			var fixedPurchase = fixedPurchaseService.Get(order.FixedPurchaseId);

			// 受注情報が定期注文ではない場合
			// または、すでに定期台帳ステータスが異常系の場合は購入回数(注文基準)を+1しない
			if ((fixedPurchase == null)
				|| (fixedPurchase.FixedPurchaseStatus != Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_STATUS_NORMAL)) return;

			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 仮注文で注文が残る場合
				// 定期台帳の購入回数(注文基準)を+1する
				fixedPurchaseService.Modify(
					fixedPurchase.FixedPurchaseId,
					Constants.FLG_FIXEDPURCHASEHISTORY_FIXED_PURCHASE_HISTORY_KBN_TEMP,
					(model, historyModel) =>
					{
						// 注文ID、購入回数(注文基準)更新、購入回数(注文基準)更新結果をセット
						historyModel.OrderId = order.OrderId;
						historyModel.UpdateOrderCount = 1;
						historyModel.UpdateOrderCountResult = (model.OrderCount + 1);

						// 仮注文登録された場合、購入回数(注文基準)の更新を行う
						model.OrderCount = (model.OrderCount + 1);

						// 最終更新者をセット
						model.LastChanged =
						historyModel.LastChanged = lastChanged;
					},
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				// 仮注文登録された受注情報に定期購入回数(注文基準)を更新する
				UpdateFixedPurchaseIdAndFixedPurchaseOrderCount(
					order.OrderId,
					order.FixedPurchaseId,
					fixedPurchase.OrderCount + 1,
					lastChanged,
					UpdateHistoryAction.DoNotInsert,
					accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +UpdateFixedPurchaseShippedCount 定期購入回数(出荷時点)更新
		/// <summary>
		/// 定期購入回数(出荷時点)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseShippedCount">定期購入回数(出荷時点)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public int UpdateFixedPurchaseShippedCount(
			string orderId,
			int fixedPurchaseShippedCount,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.FixedPurchaseShippedCount = fixedPurchaseShippedCount;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);

			// 定期分析更新
			new FixedPurchaseRepeatAnalysisService()
				.UpdateRepeatAnalysisStatusByOrderId(
					orderId,
					Constants.FLG_FIXEDPURCHASEREPEATANALYSIS_STATUS_DELIVERED,
					lastChanged,
					accessor);

			return updated;
		}
		#endregion

		#region +UpdateUserId ユーザーID更新
		/// <summary>
		/// ユーザーID更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public int UpdateUserId(
			string orderId,
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.UserId = userId;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateCreditBranchNo クレジットカード枝番更新
		/// <summary>
		/// クレジットカード枝番更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="creditBranchNo">クレジットカード枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>件数</returns>
		public int UpdateCreditBranchNo(
			string orderId,
			int creditBranchNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdateCreditBranchNo(orderId, creditBranchNo, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +UpdateCreditBranchNo クレジットカード枝番更新
		/// <summary>
		/// クレジットカード枝番更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="creditBranchNo">クレジットカード枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public int UpdateCreditBranchNo(
			string orderId,
			int creditBranchNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.CreditBranchNo = creditBranchNo;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateCardTransaction 決済取引情報更新
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
		public int UpdateCardTransaction(
			string orderId,
			string paymentOrderId,
			string cardTranId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				model =>
				{
					model.CardTranId = cardTranId;
					model.PaymentOrderId = paymentOrderId;
					model.LastChanged = lastChanged;
				},
				updateHistoryAction);
			return updated;
		}
		#endregion

		#region +UpdateCardTranId 決済取引ID更新
		/// <summary>
		/// 決済取引ID更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		public int UpdateCardTranId(
			string orderId,
			string cardTranId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdateCardTranId(orderId, cardTranId, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +UpdateCardTranId 決済取引ID更新
		/// <summary>
		/// 決済取引ID更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateCardTranId(
			string orderId,
			string cardTranId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.CardTranId = cardTranId;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateOnlinePaymentStatus オンライン決済ステータス更新
		/// <summary>
		/// オンライン決済ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新結果</returns>
		public int UpdateOnlinePaymentStatus(
			string orderId,
			string onlinePaymentStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdateOnlinePaymentStatus(orderId, onlinePaymentStatus, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +UpdateOnlinePaymentStatus オンライン決済ステータス更新
		/// <summary>
		/// オンライン決済ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="onlinePaymentStatus">オンライン決済ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新結果</returns>
		public int UpdateOnlinePaymentStatus(
			string orderId,
			string onlinePaymentStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.OnlinePaymentStatus = onlinePaymentStatus;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +AddPaymentMemo 決済連携メモ追記
		/// <summary>
		/// 決済連携メモ追記
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="addMemoString">追記メモ文字列</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int AddPaymentMemo(
			string orderId,
			string addMemoString,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.PaymentMemo = OrderExternalPaymentUtility.SetExternalPaymentMemo(
						StringUtility.ToEmpty(order.PaymentMemo),
						addMemoString);
					order.LastChanged = lastChanged;
					order.DateChanged = DateTime.Now;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdatePaymentMemo 決済連携メモ更新
		/// <summary>
		/// 決済連携メモ追記
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="addMemoString">更新メモ文字列</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		public int UpdatePaymentMemo(
			string orderId,
			string addMemoString,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				var updated = Modify(
					orderId,
					(order) =>
					{
						order.PaymentMemo = addMemoString;
						order.LastChanged = lastChanged;
						order.DateChanged = DateTime.Now;
					},
					updateHistoryAction,
					accessor);
				return updated;
			}
		}
		#endregion

		#region +UpdateCardTransactionAndPaymentMemo 決済取引情報＋決済連携メモ一括更新
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
		public void UpdateCardTransactionAndPaymentMemo(
			string orderId,
			string paymentOrderId,
			string cardTranId,
			string addMemoString,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			Modify(
				orderId,
				model =>
				{
					model.CardTranId = cardTranId;
					model.PaymentOrderId = paymentOrderId;
					model.PaymentMemo = OrderExternalPaymentUtility.SetExternalPaymentMemo(
						StringUtility.ToEmpty(model.PaymentMemo),
						addMemoString);
					model.LastChanged = lastChanged;
					model.DateChanged = DateTime.Now;
				},
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateOrderShipping
		/// <summary>
		/// Update Order Shipping
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>更新件数</returns>
		public int UpdateOrderShipping(OrderShippingModel model, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				return repository.UpdateShipping(model);
			}
		}
		#endregion

		#region +UpdateOrderDateChangedByOrderId 最終更新日時更新(order_id, accessor)
		/// <summary>
		/// 最終更新日時更新(order_id, accessor)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQL Accessor</param>
		public void UpdateOrderDateChangedByOrderId(string orderId, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				repository.UpdateOrderDateChangedByOrderId(orderId);
			}
		}
		#endregion

		#region +UpdateOrderDateChangedByUserId 最終更新日時更新(user_id, accessor)
		/// <summary>
		/// 最終更新日時更新(user_id, accessor)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">SQL Accessor</param>
		public void UpdateOrderDateChangedByUserId(string userId, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				repository.UpdateOrderDateChangedByUserId(userId);
			}
		}
		#endregion

		#region +UpdateOrderDateChangedForIntegration 最終更新日時更新(user_id, couponId, couponNoOld, accessor)
		/// <summary>
		/// 最終更新日時更新(user_id, couponId, couponNoOld, accessor)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNoOld">旧クーポンNo</param>
		/// <param name="accessor">SQL Accessor</param>
		public void UpdateOrderDateChangedForIntegration(string userId, string couponId, string couponNoOld, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				repository.UpdateOrderDateChangedForIntegration(userId, couponId, couponNoOld);
			}
		}
		#endregion

		#region +UpdateReissueInvoiceStatusAndPaymentMemo
		/// <summary>
		/// Update Reissue Invoice Status And Payment Memo
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="extendStatusNo">Extend status no</param>
		/// <param name="addMemoString">Add memo string</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		public void UpdateReissueInvoiceStatusAndPaymentMemo(
			string orderId,
			int extendStatusNo,
			string addMemoString,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateReissueInvoiceStatusAndPaymentMemo(
					orderId,
					extendStatusNo,
					addMemoString,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +UpdateReissueInvoiceStatusAndPaymentMemo
		/// <summary>
		/// Update Reissue Invoice Status And Payment Memo
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="extendStatusNo">Extend status no</param>
		/// <param name="addMemoString">Add memo string</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql accessor</param>
		public void UpdateReissueInvoiceStatusAndPaymentMemo(
			string orderId,
			int extendStatusNo,
			string addMemoString,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			UpdateOrderExtendStatus(
				orderId,
				extendStatusNo,
				Constants.FLG_ORDER_ORDER_EXTEND_STATUS_OFF,
				DateTime.Now,
				lastChanged,
				updateHistoryAction,
				accessor);

			AddPaymentMemo(
				orderId,
				addMemoString,
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdatePaygentOrder
		/// <summary>
		/// ペイジェント受注情報更新
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <param name="card3DSecureTranId">3DSecureTranId</param>
		/// <param name="cardTranId">PaymentId</param>
		/// <param name="accessor">accessor</param>
		/// <returns>影響件数</returns>
		public int UpdatePaygentOrder(
			string orderId,
			string card3DSecureTranId,
			string cardTranId,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.UpdatePaygentOrder(orderId, card3DSecureTranId, cardTranId);
				return result;
			}
		}
		#endregion

		#region +UpdateOrderPaymentStatusComplete
		/// <summary>
		/// 入金ステータスを入金済みに更新
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <param name="orderPaymentDate">入金確認日時</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">accessor</param>
		/// <returns>影響件数</returns>
		public int UpdateOrderPaymentStatusComplete(
			string orderId,
			DateTime orderPaymentDate,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.UpdateOrderPaymentStatusComplete(orderId, orderPaymentDate, lastChanged);
				return result;
			}
		}
		#endregion

		#region +InspectReturnAllItems
		/// <summary>
		/// 注文商品が全て返品されているか？
		/// </summary>
		/// <param name="relatedOrders">注文（返品交換含む）</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>全て返品している：true、全て返品していない：false</returns>
		public bool InspectReturnAllItems(OrderModel[] relatedOrders, SqlAccessor accessor)
		{
			var service = new OrderStatusInspect();
			return service.InspectReturnAllItems(this, relatedOrders, accessor);
		}
		#endregion

		#region +GetUncancelledOrdersByUserId ユーザーIDから未キャンセル注文情報取得
		/// <summary>
		/// ユーザーIDから未キャンセル注文情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデルリスト</returns>
		public OrderModel[] GetUncancelledOrdersByUserId(string userId, SqlAccessor accessor = null)
		{
			var orders = GetOrdersByUserId(userId, accessor);
			return orders.Where(o =>
				o.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED
				&& o.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)
				.OrderBy(o => o.OrderDate).ToArray();
		}
		#endregion

		#region +GetOrderInfoByOrderId 注文IDから注文情報取得
		/// <summary>
		/// 注文IDから注文情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">トランザクションを内包するアクセサ(指定しなければメソッド内部でトランザクションが完結)</param>
		/// <param name="isGetOrderItem">注文商品情報を取得するか</param>
		/// <returns>モデル</returns>
		public OrderModel GetOrderInfoByOrderId(string orderId, SqlAccessor accessor = null, bool isGetOrderItem = false)
		{
			// トランザクションがあれば引き継ぐ
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetOrderInfoByOrderId(orderId, isGetOrderItem);
				return model;
			}
		}
		#endregion

		#region +GetOrdersByUserId ユーザーIDから注文情報取得
		/// <summary>
		/// ユーザーIDから注文情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しなければメソッド内部でトランザクションが完結)</param>
		/// <returns>モデルリスト</returns>
		public OrderModel[] GetOrdersByUserId(string userId, SqlAccessor sqlAccessor = null)
		{
			// トランザクションがあれば引き継ぐ
			using (var repository = new OrderRepository(sqlAccessor))
			{
				var models = repository.GetOrdersByUserId(userId);
				return models;
			}
		}
		#endregion

		#region +HasUncancelledOrders キャンセル済みでない注文があるか
		/// <summary>
		/// キャンセル済みでない注文があるか
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="exclusionOrderId">除外注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>キャンセル済みでない注文があるか</returns>
		public bool HasUncancelledOrders(string userId, string exclusionOrderId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orderInfo = repository.GetUncancelledOrders(userId);
				var orderIdAndPaymentIdsExlcudeTmpOrder = orderInfo.Where(order =>
					(order.Item2 != Constants.FLG_ORDER_ORDER_STATUS_TEMP)
						|| (order.Item3 == Constants.PAYMENT_CREDIT_PROVISIONAL_CREDITCARD_PAYMENT_ID)).ToArray();
				var result = orderIdAndPaymentIdsExlcudeTmpOrder.Any(order => (order.Item1 != exclusionOrderId));
				return result;
			}
		}
		#endregion

		#region +GetUncancelledOrderInfosByUserId ユーザーIDから未キャンセル注文情報取得（全ての注文情報を含む）
		/// <summary>
		/// ユーザーIDから未キャンセル注文情報取得（全ての注文情報を含む）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデルリスト</returns>
		public OrderModel[] GetUncancelledOrderInfosByUserId(string userId)
		{
			var orders = GetOrderInfosByUserId(userId);
			return orders.Where(o =>
				o.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED
				&& o.OrderStatus != Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED)
				.OrderBy(o => o.OrderDate).ToArray();
		}
		#endregion

		#region +GetOrderInfosByUserId ユーザーIDから注文情報取得（全ての注文情報を含む）
		/// <summary>
		/// ユーザーIDから注文情報取得（全ての注文情報を含む）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しなければメソッド内部でトランザクションが完結)</param>
		/// <returns>モデルリスト</returns>
		public OrderModel[] GetOrderInfosByUserId(string userId, SqlAccessor sqlAccessor = null)
		{
			// トランザクションがあれば引き継ぐ
			using (var repository = new OrderRepository(sqlAccessor))
			{
				var models = repository.GetOrderInfosByUserId(userId);
				return models;
			}
		}
		#endregion

		#region +GetOrderHistoryList ユーザーIDから注文履歴情報取得
		/// <summary>
		/// ユーザーIDから注文履歴情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しなければメソッド内部でトランザクションが完結)</param>
		/// <returns>注文モデルリスト</returns>
		public OrderModel[] GetOrderHistoryList(string userId, SqlAccessor sqlAccessor = null)
		{
			// トランザクションがあれば引き継ぐ
			using (var repository = new OrderRepository(sqlAccessor))
			{
				var orders = repository.GetOrderHistoryList(userId);

				if (orders.Count() > 0)
				{
					// 注文商品一覧取得
					string[] orderIds = orders.Select(order => order.OrderId).ToArray();
					var orderItems = repository.GetItemByOrderIds(orderIds);
					foreach (var order in orders)
					{
						order.Items = orderItems.Where(orderItem => orderItem.OrderId == order.OrderId).ToArray();
					}
				}
				return orders;
			}
		}
		#endregion

		#region +GetOrderHistoryDetailInDataView 注文詳細の取得 HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文詳細の取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="includeTempPaidyPaygentOrder">Include temp paidy paygent order</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public DataView GetOrderHistoryDetailInDataView(
			string orderId,
			string userId,
			string memberRankId,
			bool includeTempPaidyPaygentOrder = false,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var order = repository.GetOrderHistoryDetailInDataView(
					orderId,
					userId,
					memberRankId,
					includeTempPaidyPaygentOrder);
				return order;
			}
		}
		#endregion

		#region +GetOrderHistoryListByOrdersInDataView 注文履歴一覧（注文単位）を取得する HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文履歴一覧（注文単位）を取得する HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public DataView GetOrderHistoryListByOrdersInDataView(
			string userId,
			int startRowNum,
			int endRowNum,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orders = repository.GetOrderHistoryListByOrdersInDataView(userId, startRowNum, endRowNum);
				return orders;
			}
		}
		#endregion

		#region +GetOrderHistoryListByProductsInDataView 注文履歴一覧（商品単位）を取得する HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文履歴一覧（商品単位）を取得する HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public DataView GetOrderHistoryListByProductsInDataView(
			string userId,
			string memberRankId,
			int startRowNum,
			int endRowNum,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var order = repository.GetOrderHistoryListByProductsInDataView(userId, memberRankId, startRowNum, endRowNum);
				return order;
			}
		}
		#endregion

		#region +GetOrderWorkflowListInDataView 注文一覧を取得(ワークフロー) HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文一覧を取得(ワークフロー) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="htSearch"></param>
		/// <param name="iPageNumber"></param>
		/// <returns>注文DataView</returns>
		public DataView GetOrderWorkflowListInDataView(Hashtable htSearch, int iPageNumber = 1)
		{
			using (var repository = new OrderRepository())
			{
				repository.CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT;
				var orders = repository.GetOrderWorkflowListInDataView(htSearch, iPageNumber);
				return orders;
			}
		}
		#endregion

		#region +GetOrderWorkflowListCountInDataView 注文件数を取得(ワークフロー) HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文一覧を取得(ワークフロー) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="htSearch"></param>
		/// <returns>注文DataView</returns>
		public DataView GetOrderWorkflowListCountInDataView(Hashtable htSearch, int iPageNumber = 1)
		{
			using (var repository = new OrderRepository())
			{
				repository.CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT;
				var orders = repository.GetOrderWorkflowListCountInDataView(htSearch, iPageNumber);
				return orders;
			}
		}
		#endregion

		#region GetOrderInDataView 注文情報を取得 HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文情報を取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <returns>注文DataView</returns>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public DataView GetOrderInDataView(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orders = repository.GetOrderInDataView(orderId);
				return orders;
			}
		}
		#endregion

		#region GetOrdersReturnExchangeByOrderId
		/// <summary>
		/// Get Orders Return Exchange By Order Id
		/// </summary>
		/// <param name="orderId">OrderId</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>注文DataView</returns>
		public DataView GetOrdersReturnExchangeByOrderId(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orders = repository.GetOrdersReturnExchangeByOrderId(orderId);

				return orders;
			}
		}
		#endregion

		#region +GetOrderShippingInDataView 配送先情報取得 HACK: 例外的にDataViewを返す
		/// <summary>
		/// 配送先情報取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文DataView</returns>
		public DataView GetOrderShippingInDataView(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orders = repository.GetOrderShippingInDataView(orderId);
				return orders;
			}
		}
		#endregion

		#region +GetOrderIdForOrderReturnExchangeInDataView 最大注文ID(枝番付き)取得 HACK: 例外的にDataViewを返す

		/// <summary>
		/// 最大注文ID(枝番付き)取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文DataView</returns>
		public string GetOrderIdForOrderReturnExchange(string orderId, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orders = repository.GetOrderIdForOrderReturnExchangeInDataView(orderId);
				return (string)orders[0][Constants.FIELD_ORDER_ORDER_ID];
			}
		}
		#endregion

		#region +GetOrderSetPromotionInDataView 注文セットプロモーション情報取得 HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文セットプロモーション情報取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文DataView</returns>
		public DataView GetOrderSetPromotionInDataView(string orderId)
		{
			using (var repository = new OrderRepository())
			{
				var orders = repository.GetOrderSetPromotionInDataView(orderId);
				return orders;
			}
		}
		#endregion

		#region +GetOrderForOrderReturnExchangeInDataView 注文情報取得(※子注文の返品商品も抽出する) HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文情報取得(※子注文の返品商品も抽出する) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文DataView</returns>
		public DataView GetOrderForOrderReturnExchangeInDataView(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orders = repository.GetOrderForOrderReturnExchangeInDataView(orderId);
				return orders;
			}
		}
		#endregion

		#region +GetExchangedOrderInDataView  交換済み注文情報取得(※親注文IDが等しい注文の返品商品も抽出する) HACK: 例外的にDataViewを返す
		/// <summary>
		///  交換済み注文情報取得(※親注文IDが等しい注文の返品商品も抽出する) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="exchangedOrderId">交換済みの注文ID</param>
		/// <param name="orderIdOrg">元注文</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文DataView</returns>
		public DataView GetExchangedOrderInDataView(string exchangedOrderId, string orderIdOrg, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orders = repository.GetExchangedOrderInDataView(exchangedOrderId, orderIdOrg);
				return orders;
			}
		}
		#endregion

		#region +GetExchangedOrderIds 交換注文の注文群を取得
		/// <summary>
		/// 交換注文の注文群を取得
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文モデル</returns>
		public OrderModel[] GetExchangedOrderIds(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orders = repository.GetExchangedOrderIds(orderId);
				return orders;
			}
		}
		#endregion

		#region +GetForOrderReturnInDataView 返品用の受注取得 HACK: 例外的にDataViewを返す

		/// <summary>
		/// 返品用の受注取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="returnExchangeKbn">返品交換区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文DataView</returns>
		public DataView GetForOrderReturnInDataView(string orderId, string returnExchangeKbn, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orders = repository.GetForOrderReturnInDataView(orderId, returnExchangeKbn);
				return orders;
			}
		}
		#endregion

		#region +AdjustAddPoint 利用ポイント調整
		/// <summary>
		/// 利用ポイント調整
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="adjustPoint">調整ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int AdjustAddPoint(
			string orderId,
			decimal adjustPoint,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// ポイント調整
			var updated = AdjustAddPoint(orderId, adjustPoint, lastChanged, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -AdjustAddPoint 利用ポイント調整
		/// <summary>
		/// 利用ポイント調整
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="adjustPoint">調整ポイント数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しなければメソッド内部でトランザクションが完結)</param>
		/// <returns>更新件数</returns>
		private int AdjustAddPoint(string orderId, decimal adjustPoint, string lastChanged, SqlAccessor sqlAccessor)
		{
			// トランザクションがあれば引き継ぐ
			using (var repo = new OrderRepository(sqlAccessor))
			{
				var result = repo.AdjustAddPoint(orderId, adjustPoint, lastChanged);

				//最終更新日時更新
				UpdateOrderDateChangedByOrderId(orderId, sqlAccessor);

				return result;
			}
		}
		#endregion

		#region +GetCoupon クーポン取得
		/// <summary>
		/// クーポン取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns> 注文クーポン情報 </returns>
		public OrderCouponDetailInfo GetCoupon(string orderId)
		{
			using (var repository = new OrderRepository())
			{
				var result = repository.GetCoupon(orderId);
				return result;
			}
		}
		#endregion

		#region +InsertCoupon クーポン登録
		/// <summary>
		/// クーポン登録
		/// </summary>
		/// <param name="orderCoupon">注文クーポンモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 登録件数 </returns>
		public int InsertCoupon(
			OrderCouponModel orderCoupon,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// インサート
			var result = InsertCoupon(orderCoupon, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderCoupon.OrderId, orderCoupon.LastChanged, accessor);
			}
			return result;
		}
		#endregion
		#region -InsertCoupon クーポン登録
		/// <summary>
		/// クーポン登録
		/// </summary>
		/// <param name="orderCoupon">注文クーポンモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 登録件数 </returns>
		private int InsertCoupon(OrderCouponModel orderCoupon, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.InsertCoupon(orderCoupon);
				return result;
			}
		}
		#endregion

		#region +UpdateCoupon クーポン更新
		/// <summary>
		/// クーポン更新
		/// </summary>
		/// <param name="orderCoupon">注文クーポンモデル</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 更新件数 </returns>
		public int UpdateCoupon(
			OrderCouponModel orderCoupon,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 更新
			var updated = UpdateCoupon(orderCoupon, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderCoupon.OrderId, orderCoupon.LastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -UpdateCoupon クーポン更新
		/// <summary>
		/// クーポン更新
		/// </summary>
		/// <param name="orderCoupon">注文クーポンモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 更新件数 </returns>
		private int UpdateCoupon(OrderCouponModel orderCoupon, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.UpdateCoupon(orderCoupon);

				// 最終更新日時更新
				UpdateOrderDateChangedByOrderId(orderCoupon.OrderId, accessor);
				return result;
			}
		}
		#endregion

		#region +DeleteOrderCoupon 注文クーポン情報削除
		/// <summary>
		/// 注文クーポン情報削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 削除件数 </returns>
		public int DeleteOrderCoupon(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 削除
			var result = DeleteOrderCoupon(orderId, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}
			return result;
		}
		#endregion
		#region -DeleteOrderCoupon 注文クーポン情報削除
		/// <summary>
		/// 注文クーポン情報削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns> 削除件数 </returns>
		private int DeleteOrderCoupon(string orderId, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.DeleteOrderCoupon(orderId);
				return result;
			}
		}
		#endregion

		#region +DeleteCouponByCouponNo クーポン情報削除 （注文ID＆注文クーポン枝番指定）
		/// <summary>
		/// クーポン情報削除 （注文ID＆注文クーポン枝番指定）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderCouponNo">注文クーポン枝番</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		public int DeleteCouponByCouponNo(
			string orderId,
			int orderCouponNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 削除
			var updated = DeleteCouponByCouponNo(orderId, orderCouponNo, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -DeleteCouponByCouponNo クーポン情報削除 （注文ID＆注文クーポン枝番指定）
		/// <summary>
		/// クーポン情報削除 （注文ID＆注文クーポン枝番指定）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderCouponNo">注文クーポン枝番</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		private int DeleteCouponByCouponNo(string orderId, int orderCouponNo, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.DeleteCouponByCouponNo(orderId, orderCouponNo);
				return result;
			}
		}
		#endregion

		#region +DeleteSetPromotionAll 注文セットプロモーション情報削除
		/// <summary>
		/// 注文セットプロモーション情報削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>処理件数</returns>
		public int DeleteSetPromotionAll(string orderId, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				return repository.DeleteSetPromotionAll(orderId);
			}
		}
		#endregion

		#region +GetForOrderMail 取得（注文メール用）
		/// <summary>
		/// 取得（注文メール用）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>メール送信用注文情報列</returns>
		public OrderModel[] GetForOrderMail(string orderId)
		{
			using (var repository = new OrderRepository())
			{
				var mailDatas = repository.GetForOrderMail(orderId);
				return mailDatas;
			}
		}
		#endregion

		#region +GetOrderSerialKeyForOrderMail シリアルキー付き注文取得（注文メール用）
		/// <summary>
		/// シリアルキー付き注文取得（注文メール用）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>メール送信用シリアルキー付き注文情報列</returns>
		public OrderSerialKeyForMailSend[] GetOrderSerialKeyForOrderMail(string orderId)
		{
			using (var repository = new OrderRepository())
			{
				var mailDatas = repository.GetOrderSerialKeyForOrderMail(orderId);
				return mailDatas;
			}
		}
		#endregion

		#region 再与信
		#region -UpdateExternalPaymentInfoForAuthError 外部決済ステータスを「与信エラー」更新
		/// <summary>
		/// 外部決済ステータスを「与信エラー」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentErrorMessage">外部決済エラーメッセージ</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>件数</returns>
		public void UpdateExternalPaymentInfoForAuthError(
			string orderId,
			string externalPaymentErrorMessage,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateExternalPaymentInfoForAuthError(
					orderId,
					externalPaymentErrorMessage,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion
		#region -UpdateExternalPaymentInfoForAuthError 外部決済ステータスを「与信エラー」更新
		/// <summary>
		/// 外部決済ステータスを「与信エラー」更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentErrorMessage">外部決済エラーメッセージ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public void UpdateExternalPaymentInfoForAuthError(
			string orderId,
			string externalPaymentErrorMessage,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			UpdateExternalPaymentInfo(
				orderId,
				Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_ERROR,
				false,
				null,
				externalPaymentErrorMessage,
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateExternalPaymentInfoForAuthSuccess 与信成功向け外部決済情報ステータス更新 ※与信日時は更新する
		/// <summary>
		/// 与信成功向け外部決済情報更新 ※与信日時は更新する
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateExternalPaymentInfoForAuthSuccess(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			UpdateExternalPaymentInfo(
				orderId,
				Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_AUTH_COMP,
				true,
				DateTime.Now,
				string.Empty,
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateExternalPaymentStatusSalesComplete 外部決済ステータスを「売上確定済み」更新　※与信日時は更新せずステータス変更のみ
		/// <summary>
		/// 外部決済ステータスを「売上確定済み」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public void UpdateExternalPaymentStatusSalesComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			UpdateExternalPaymentInfo(
				orderId,
				Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP,
				false,
				null,
				string.Empty,
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateExternalPaymentStatusSalesComplete 外部決済ステータスを「売上確定済み」更新　※与信日時は更新せずステータス変更のみ
		/// <summary>
		/// 外部決済ステータスを「売上確定済み」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>件数</returns>
		public void UpdateExternalPaymentStatusSalesComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateExternalPaymentInfo(
					orderId,
					Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SALES_COMP,
					false,
					null,
					string.Empty,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +UpdateExternalPaymentStatusShipmentComplete 外部決済ステータスを「出荷報告済み」更新　※与信日時は更新せずステータス変更のみ
		/// <summary>
		/// 外部決済ステータスを「出荷報告済み」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>件数</returns>
		public void UpdateExternalPaymentStatusShipmentComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateExternalPaymentStatusShipmentComplete(orderId, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion
		#region +UpdateExternalPaymentStatusShipmentComplete 外部決済ステータスを「出荷報告済み」更新　※与信日時は更新せずステータス変更のみ
		/// <summary>
		/// 外部決済ステータスを「出荷報告済み」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public void UpdateExternalPaymentStatusShipmentComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			UpdateExternalPaymentInfo(
				orderId,
				Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_COMP,
				false,
				null,
				string.Empty,
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateExternalPaymentStatusShipmentError 外部決済ステータスを「出荷報告エラー」更新　※与信日時は更新せずステータス変更のみ
		/// <summary>
		/// 外部決済ステータスを「出荷報告エラー」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentErrorMessage">外部決済エラーメッセージ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>件数</returns>
		public void UpdateExternalPaymentStatusShipmentError(
			string orderId,
			string externalPaymentErrorMessage,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateExternalPaymentStatusShipmentError(
					orderId,
					externalPaymentErrorMessage,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion
		#region +UpdateExternalPaymentStatusShipmentError 外部決済ステータスを「出荷報告エラー」更新　※与信日時は更新せずステータス変更のみ
		/// <summary>
		/// 外部決済ステータスを「出荷報告エラー」更新 ※与信日時は更新せずステータス変更のみ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentErrorMessage">外部決済エラーメッセージ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public void UpdateExternalPaymentStatusShipmentError(
			string orderId,
			string externalPaymentErrorMessage,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			UpdateExternalPaymentInfo(
				orderId,
				Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_SHIP_ERROR,
				false,
				null,
				externalPaymentErrorMessage,
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateExternalPaymentInfo 外部決済情報更新
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
		public int UpdateExternalPaymentInfo(
			string orderId,
			string externalPaymentStatus,
			bool updateExternalPaymentAuthDate,
			DateTime? externalPaymentAuthDate,
			string externalPaymentErrorMessage,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.ExternalPaymentStatus = externalPaymentStatus;
					if (updateExternalPaymentAuthDate)
					{
						order.ExternalPaymentAuthDate = externalPaymentAuthDate;
					}
					if (externalPaymentErrorMessage != null)
					{
						order.ExternalPaymentErrorMessage = externalPaymentErrorMessage;
					}
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateExternalPaymentStatusDeliveryComplete 外部決済ステータスを「配送完了」更新
		/// <summary>
		/// 外部決済ステータスを「配送完了」更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateExternalPaymentStatusDeliveryComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_DELI_COMP;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateExternalPaymentStatusPayComplete 外部決済ステータスを「入金済み」更新
		/// <summary>
		/// 外部決済ステータスを「入金済み」更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		public int UpdateExternalPaymentStatusPayComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdateExternalPaymentStatusPayComplete(orderId, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region -UpdateExternalPaymentStatusPayComplete 外部決済ステータスを「入金済み」更新
		/// <summary>
		/// 外部決済ステータスを「入金済み」更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateExternalPaymentStatusPayComplete(
			string orderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_PAY_COMP;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateLastAuthFlg 最終与信フラグ更新
		/// <summary>
		/// 最終与信フラグ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastAuthFlg">最終与信フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateLastAuthFlg(
			string orderId,
			string lastAuthFlg,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			// 更新
			using (var repository = new OrderRepository(accessor))
			{
				var model = Get(orderId, accessor);
				model.LastAuthFlg = lastAuthFlg;
				model.LastChanged = lastChanged;
				model.DateChanged = DateTime.Now;
				repository.Update(model);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}
		}
		#endregion

		#region +GetCvsDefUpdateShipmentCompleteTargeReturnOrder ヤマト後払い出荷報告完了更新対象の返品注文情報取得
		/// <summary>
		/// ヤマト後払い出荷報告完了更新対象の返品注文情報取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		public OrderModel[] GetCvsDefUpdateShipmentCompleteTargeReturnOrder(SqlAccessor accessor = null)
		{
			using (var repository = (accessor != null ? new OrderRepository(accessor) : new OrderRepository()))
			{
				var models = repository.GetUpdateShipmentCompleteTargeReturnOrder(Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF);
				// 注文者&配送先&商品&クーポン&セットプロモーションをセット
				foreach (var model in models)
				{
					SetChildModels(model, repository);
				}
				return models;
			}
		}
		#endregion

		#region +GetCreditUpdateShipmentCompleteTargeReturnOrder ヤマトクレジットカード出荷報告完了更新対象の返品注文情報取得
		/// <summary>
		/// ヤマトクレジットカード出荷報告完了更新対象の返品注文情報取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル列</returns>
		public OrderModel[] GetCreditUpdateShipmentCompleteTargeReturnOrder(SqlAccessor accessor = null)
		{
			using (var repository = (accessor != null ? new OrderRepository(accessor) : new OrderRepository()))
			{
				var models = repository.GetUpdateShipmentCompleteTargeReturnOrder(Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT);
				// 注文者&配送先&商品&クーポン&セットプロモーションをセット
				foreach (var model in models)
				{
					SetChildModels(model, repository);
				}
				return models;
			}
		}
		#endregion
		#endregion

		#region +UpdateOrderStatusForOrderComplete 注文完了向け注文ステータス更新
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
		/// <param name="creditStatus">与信状況</param>
		/// <returns>更新件数</returns>
		public int UpdateOrderStatusForOrderComplete(
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
			string creditStatus = "")
		{
			var updated = Modify(
				orderId,
				model =>
				{
					model.OrderStatus = orderSattus;
					model.CardTranId = cardTranId;
					model.PaymentOrderId = paymentOrderId;
					model.OnlinePaymentStatus = onlinePaymentStatus;
					model.CreditBranchNo = creditBranchNo;
					model.OrderPaymentStatus = orderPaymentStatus;
					model.OrderPaymentDate = orderPaymentDate;
					model.PaymentMemo = paymentMemo;
					model.ExternalPaymentStatus = externalPaymentStatus;
					model.ExternalPaymentAuthDate = externalPaymentAuthDate;
					model.LastChanged = lastChanged;
					if (string.IsNullOrEmpty(cardTranPass) == false)
					{
						model.CardTranPass = cardTranPass;
					}
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateOrderInstallmentCode 支払回数更新
		/// <summary>
		/// 注文完了向け注文ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="installmentsCode">支払回数コード</param>
		/// <param name="installments">支払回数名称</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">アクセッサ</param>
		/// <returns>更新件数</returns>
		public int UpdateOrderInstallmentsCode(
			string orderId,
			string installmentsCode,
			string installments,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			var updated = Modify(
				orderId,
				model =>
				{
					model.CardInstallmentsCode = installmentsCode;
					model.CardInstruments = installments;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +GetAuthExpired 与信期限切れ注文取得
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
		/// <param name="rakutenPayExpireDays">Rakuten Expire Days</param>
		/// <param name="gmoPostExpireDays">Gmo掛け払い与信切れ日数</param>
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
		public OrderModel[] GetAuthExpired(
			DateTime targetDate,
			int cardExpireDays,
			int cvsDefExpireDays,
			int amazonPayExpireDays,
			int paidyPayExpireDays,
			int linePayExpireDate,
			int npafterPayExpireDays,
			int rakutenPayExpireDays,
			int gmoPostExpireDays)
		{
			using (var repository = new OrderRepository())
			{
				var orderIds = repository.GetAuthExpiredOrderIds(
					targetDate.AddDays(-1 * cardExpireDays + 1).Date,
					targetDate.AddDays(-1 * cvsDefExpireDays + 1).Date,
					targetDate.AddDays(-1 * amazonPayExpireDays + 1).Date,
					targetDate.AddDays(-1 * paidyPayExpireDays + 1).Date,
					targetDate.AddDays(-1 * linePayExpireDate + 1).Date,
					targetDate.AddDays(-1 * npafterPayExpireDays + 1).Date,
					targetDate.AddDays(-1 * rakutenPayExpireDays + 1).Date,
					targetDate.AddDays(-1 * gmoPostExpireDays + 1).Date);

				// 配送希望日昇順（※指定なし（=NULLL）を優先する）、注文ID昇順に並び替え
				// 返品交換注文の返品交換完了している受注に関しては除外
				// ※Payment_Credit_Return_AutoSales_Enabled=Falseになっている場合
				//   返品交換時の注文がすべて再与信対象となってしまうため、
				//   既に本フローにて運用しているお客様に影響が出ないように返品交換ステータスが完了している物に関しては再与信が発生しないように修正
				var orders = orderIds.Select(id => Get(id))
					.Where(o => (o.IsAlreadyReturnExchangeCompleted == false))
					.OrderBy(o => o.Shippings[0].ShippingDate.HasValue ? o.Shippings[0].ShippingDate.Value : DateTime.MinValue)
					.ThenBy(o => o.OrderId).ToArray();
				return orders;
			}
		}
		#endregion

		#region +GetOrderForAuth 注文取得（与信用）
		/// <summary>
		/// 注文取得（与信用）
		/// </summary>
		/// <param name="extendStatusNumber">拡張ステータス番号</param>
		/// <returns>対象注文</returns>
		public OrderModel[] GetOrderForAuth(string extendStatusNumber)
		{
			using (var repository = new OrderRepository())
			{
				var orderIds = repository.GetOrderForAuth(extendStatusNumber);
				// 配送希望日昇順（※指定なし（=NULLL）を優先する）、注文ID昇順に並び替え
				// 返品交換注文の返品交換完了している受注に関しては除外
				// ※Payment_Credit_Return_AutoSales_Enabled=Falseになっている場合
				//   返品交換時の注文がすべて再与信対象となってしまうため、
				//   既に本フローにて運用しているお客様に影響が出ないように返品交換ステータスが完了している物に関しては再与信が発生しないように修正
				var orders = orderIds.Select(id => Get(id))
					.Where(o => (o.IsAlreadyReturnExchangeCompleted == false))
					.OrderBy(o => o.Shippings[0].ShippingDate.HasValue ? o.Shippings[0].ShippingDate.Value : DateTime.MinValue)
					.ThenBy(o => o.OrderId).ToArray();
				return orders;
			}
		}
		#endregion

		#region +GetOrderIdForFixedProductOrderLimitCheck 定期商品購入制限チェック用注文ID取得（類似配送先を含む）
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
		public string[] GetOrderIdForFixedProductOrderLimitCheck(
			OrderShippingModel orderShipping,
			OrderModel order,
			OrderOwnerModel orderOwner,
			string shopId,
			string productId,
			string[] notExistsOrderIds,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var ownerList = repository.GetOrderIdForFixedProductOrderLimitCheck(
					orderShipping,
					order,
					orderOwner,
					shopId,
					productId,
					notExistsOrderIds);
				return ownerList;
			}
		}
		#endregion
		#region +GetOrderIdForProductOrderLimitCheck 通常商品購入制限チェック用注文ID取得（類似配送先を含む）
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
		public string[] GetOrderIdForProductOrderLimitCheck(
			OrderShippingModel orderShipping,
			OrderModel order,
			OrderOwnerModel orderOwner,
			string shopId,
			string productId,
			string[] notExistsOrderIds,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var ownerList = repository.GetOrderIdForProductOrderLimitCheck(
					orderShipping,
					order,
					orderOwner,
					shopId,
					productId,
					notExistsOrderIds);
				return ownerList;
			}
		}
		#endregion

		#region 管理サイト受注編集関連

		#region -UpdateForModify 注文情報更新
		/// <summary>
		/// 注文情報更新
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateForModify(
			OrderModel order,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				order.OrderId,
				(model) =>
				{
					model.UserId = order.UserId;
					model.OrderStatus = order.OrderStatus;
					model.OrderPaymentKbn = order.OrderPaymentKbn;
					model.CardTranId = order.CardTranId;
					model.PaymentOrderId = order.PaymentOrderId;
					model.OrderItemCount = order.OrderItemCount;
					model.OrderProductCount = order.OrderProductCount;
					model.OrderPriceSubtotal = order.OrderPriceSubtotal;
					model.OrderPriceShipping = order.OrderPriceShipping;
					model.OrderPriceExchange = order.OrderPriceExchange;
					model.OrderPriceRegulation = order.OrderPriceRegulation;
					model.OrderPriceTotal = order.OrderPriceTotal;
					model.MemberRankDiscountPrice = order.MemberRankDiscountPrice;
					model.OrderPointUse = order.OrderPointUse;
					model.OrderPointUseYen = order.OrderPointUseYen;
					model.OrderPointAdd = order.OrderPointAdd;
					model.OrderCouponUse = order.OrderCouponUse;
					model.RegulationMemo = order.RegulationMemo;
					model.Memo = order.Memo;
					model.PaymentMemo = order.PaymentMemo;
					model.ManagementMemo = order.ManagementMemo;
					model.ShippingMemo = order.ShippingMemo;
					model.RelationMemo = order.RelationMemo;
					model.CardInstruments = order.CardInstruments;
					model.CardInstallmentsCode = order.CardInstallmentsCode;
					model.LastChanged = lastChanged;
					model.ShippingPriceSeparateEstimatesFlg = order.ShippingPriceSeparateEstimatesFlg;
					model.SetpromotionProductDiscountAmount = order.SetpromotionProductDiscountAmount;
					model.SetpromotionShippingChargeDiscountAmount = order.SetpromotionShippingChargeDiscountAmount;
					model.SetpromotionPaymentChargeDiscountAmount = order.SetpromotionPaymentChargeDiscountAmount;
					model.OrderKbn = order.OrderKbn;
					model.ReturnExchangeReasonKbn = order.ReturnExchangeReasonKbn;
					model.ReturnExchangeReasonMemo = order.ReturnExchangeReasonMemo;
					model.RepaymentMemo = order.RepaymentMemo;
					model.AdvcodeFirst = order.AdvcodeFirst;
					model.AdvcodeNew = order.AdvcodeNew;
					model.InflowContentsType = order.InflowContentsType;
					model.InflowContentsId = order.InflowContentsId;
					model.FixedPurchaseMemberDiscountAmount = order.FixedPurchaseMemberDiscountAmount;
					model.FixedPurchaseDiscountPrice = order.FixedPurchaseDiscountPrice;
					model.LastBilledAmount = order.LastBilledAmount;
					model.ExternalPaymentStatus = order.ExternalPaymentStatus;
					model.OnlinePaymentStatus = order.OnlinePaymentStatus;
					model.ExternalPaymentAuthDate = order.ExternalPaymentAuthDate;
					model.ExternalPaymentErrorMessage = order.ExternalPaymentErrorMessage;
					model.LastOrderPointUse = order.LastOrderPointUse;
					model.LastOrderPointUseYen = order.LastOrderPointUseYen;
					model.ExternalOrderId = order.ExternalOrderId;
					model.ShippingId = order.ShippingId;
					model.LastAuthFlg = order.LastAuthFlg;
					model.ExternalImportStatus = order.ExternalImportStatus;
					model.MallLinkStatus = order.MallLinkStatus;
					model.OrderCancelDate = order.OrderCancelDate;
					model.ExtendStatus39 = order.ExtendStatus39;
					model.OrderPriceTax = order.OrderPriceTax;
					model.OrderPriceSubtotalTax = order.OrderPriceSubtotalTax;
					model.SettlementCurrency = order.SettlementCurrency;
					model.SettlementRate = order.SettlementRate;
					model.SettlementAmount = order.SettlementAmount;
					model.ShippingTaxRate = order.ShippingTaxRate;
					model.PaymentTaxRate = order.PaymentTaxRate;
					model.InvoiceBundleFlg = order.InvoiceBundleFlg;
					model.ReceiptFlg = order.ReceiptFlg;
					model.ReceiptOutputFlg = order.ReceiptOutputFlg;
					model.ReceiptAddress = order.ReceiptAddress;
					model.ReceiptProviso = order.ReceiptProviso;
					model.CreditBranchNo = order.CreditBranchNo;
					model.ExternalPaymentType = order.ExternalPaymentType;
					model.Attribute1 = order.Attribute1;
					model.Attribute2 = order.Attribute2;
					model.Attribute3 = order.Attribute3;
					model.Attribute4 = order.Attribute4;
					model.Attribute5 = order.Attribute5;
					model.Attribute6 = order.Attribute6;
					model.Attribute7 = order.Attribute7;
					model.Attribute8 = order.Attribute8;
					model.Attribute9 = order.Attribute9;
					model.Attribute10 = order.Attribute10;
					model.CardTranPass = order.CardTranPass;
					model.SubscriptionBoxCourseId = order.SubscriptionBoxCourseId;
					model.StorePickupStatus = order.StorePickupStatus;
					model.StorePickupStoreArrivedDate = order.StorePickupStoreArrivedDate;
					model.StorePickupDeliveredCompleteDate = order.StorePickupDeliveredCompleteDate;
					model.StorePickupReturnDate = order.StorePickupReturnDate;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateManagementMemo 管理メモ更新
		/// <summary>
		/// 管理メモ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="managementMemo">管理メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		public int UpdateManagementMemo(
			string orderId,
			string managementMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdateManagementMemo(orderId, managementMemo, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region -UpdateManagementMemo 管理メモ更新
		/// <summary>
		/// 管理メモ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="managementMemo">管理メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateManagementMemo(
			string orderId,
			string managementMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.ManagementMemo = managementMemo;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region -AppendManagementMemo 管理メモ追加
		/// <summary>
		/// 管理メモ追加
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="managementMemo">管理メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int AppendManagementMemo(
			string orderId,
			string managementMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.ManagementMemo = (order.ManagementMemo + "\r\n" + managementMemo).Trim();
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateShippingMemo 配送メモ更新
		/// <summary>
		/// 配送メモ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="shippingMemo">配送メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		public int UpdateShippingMemo(
			string orderId,
			string shippingMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdateShippingMemo(orderId, shippingMemo, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region -UpdateShippingMemo 配送メモ更新
		/// <summary>
		/// 配送メモ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="shippingMemo">配送メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateShippingMemo(
			string orderId,
			string shippingMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				(order) =>
				{
					order.ShippingMemo = shippingMemo;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateOwnerForModify 注文者情報更新
		/// <summary>
		/// 注文者情報更新
		/// </summary>
		/// <param name="owner">注文者モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateOwnerForModify(OrderOwnerModel owner, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			// 更新
			var updated = UpdateOwnerForModify(owner, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(owner.OrderId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -UpdateOwnerForModify 注文者情報更新
		/// <summary>
		/// 注文者情報更新
		/// </summary>
		/// <param name="owner">注文者モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private int UpdateOwnerForModify(OrderOwnerModel owner, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetOwner(owner.OrderId);

				// 更新対象項目のみセット
				model.OwnerName = owner.OwnerName;
				model.OwnerName1 = owner.OwnerName1;
				model.OwnerName2 = owner.OwnerName2;
				model.OwnerNameKana = owner.OwnerNameKana;
				model.OwnerNameKana1 = owner.OwnerNameKana1;
				model.OwnerNameKana2 = owner.OwnerNameKana2;
				model.OwnerMailAddr = owner.OwnerMailAddr;
				model.OwnerMailAddr2 = owner.OwnerMailAddr2;
				model.OwnerAddrCountryIsoCode = owner.OwnerAddrCountryIsoCode;
				model.OwnerAddrCountryName = owner.OwnerAddrCountryName;
				model.OwnerZip = owner.OwnerZip;
				model.OwnerAddr1 = owner.OwnerAddr1;
				model.OwnerAddr2 = owner.OwnerAddr2;
				model.OwnerAddr3 = owner.OwnerAddr3;
				model.OwnerAddr4 = owner.OwnerAddr4;
				model.OwnerAddr5 = owner.OwnerAddr5;
				model.OwnerCompanyName = owner.OwnerCompanyName;
				model.OwnerCompanyPostName = owner.OwnerCompanyPostName;
				model.OwnerTel1 = owner.OwnerTel1;
				model.OwnerSex = owner.OwnerSex;
				model.OwnerBirth = owner.OwnerBirth;
				model.AccessCountryIsoCode = owner.AccessCountryIsoCode;
				model.DispLanguageCode = owner.DispLanguageCode;
				model.DispLanguageLocaleId = owner.DispLanguageLocaleId;
				model.DispCurrencyCode = owner.DispCurrencyCode;
				model.DispCurrencyLocaleId = owner.DispCurrencyLocaleId;

				var result = repository.UpdateOwner(model);
				UpdateOrderDateChangedByOrderId(owner.OrderId, accessor);
				return result;
			}
		}
		#endregion

		#region +UpdateShippingForModify 配送先情報更新
		/// <summary>
		/// 配送先情報更新
		/// </summary>
		/// <param name="shippings">配送先モデルリスト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateShippingForModify(OrderShippingModel[] shippings, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			// 更新
			var updated = UpdateShippingForModify(shippings, accessor);

			//最終更新日時更新
			UpdateOrderDateChangedByOrderId(shippings[0].OrderId, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(shippings[0].OrderId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -UpdateShippingForModify 配送先情報更新
		/// <summary>
		/// 配送先情報更新
		/// </summary>
		/// <param name="shippings">配送先モデルリスト</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private int UpdateShippingForModify(OrderShippingModel[] shippings, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				// DELETE>INSERT
				var result = repository.DeleteShippingAll(shippings[0].OrderId);
				if (result == 0) return result;
				result = 0;
				foreach (var shipping in shippings)
				{
					result += repository.InsertShipping(shipping);
				}
				return result;
			}
		}
		#endregion

		#region +UpdateOrderPriceInfoByTaxRateModify 税率毎価格情報更新
		/// <summary>
		/// 税率毎価格情報更新
		/// </summary>
		/// <param name="orderPriceInfoByTaxRates">税率毎価格モデルリスト</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateOrderPriceInfoByTaxRateModify(OrderPriceByTaxRateModel[] orderPriceInfoByTaxRates, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				// DELETE>INSERT
				var result = repository.DeleteOrderPriceByTaxRateAll(orderPriceInfoByTaxRates[0].OrderId);
				if (result == 0) return result;
				result = 0;
				foreach (var orderPriceInfoByTaxRate in orderPriceInfoByTaxRates)
				{
					result += repository.InsertOrderPriceByTaxRate(orderPriceInfoByTaxRate);
				}
				return result;
			}
		}
		#endregion

		#region +UpdateOrderShippingCheckNo 配送伝票番号更新
		/// <summary>
		/// 配送伝票番号更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderShippingNo">注文配送先枝番</param>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>更新件数</returns>
		public int UpdateOrderShippingCheckNo(
			string orderId,
			int orderShippingNo,
			string shippingCheckNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = UpdateOrderShippingCheckNo(orderId, orderShippingNo, shippingCheckNo, lastChanged, updateHistoryAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +UpdateOrderShippingCheckNo 配送伝票番号更新
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
		public int UpdateOrderShippingCheckNo(
			string orderId,
			int orderShippingNo,
			string shippingCheckNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 更新
			var updated = UpdateOrderShippingCheckNo(orderId, orderShippingNo, shippingCheckNo, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -UpdateOrderShippingCheckNo 配送伝票番号更新
		/// <summary>
		/// 配送伝票番号更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderShippingNo">注文配送先枝番</param>
		/// <param name="shippingCheckNo">配送伝票番号</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private int UpdateOrderShippingCheckNo(string orderId, int orderShippingNo, string shippingCheckNo, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetShipping(orderId, orderShippingNo);

				// 配送伝票番号セット
				model.ShippingCheckNo = shippingCheckNo;

				var result = repository.UpdateShipping(model);

				//最終更新日時更新
				UpdateOrderDateChangedByOrderId(orderId, accessor);

				return result;
			}
		}
		#endregion

		#region +UpdateItemForModify 商品情報更新
		/// <summary>
		/// 商品情報更新
		/// </summary>
		/// <param name="items">注文商品モデルリスト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateItemForModify(OrderItemModel[] items, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			// 更新j
			var updated = UpdateItemForModify(items, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(items[0].OrderId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -UpdateItemForModify 商品情報更新
		/// <summary>
		/// 商品情報更新
		/// </summary>
		/// <param name="items">注文商品モデルリスト</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private int UpdateItemForModify(OrderItemModel[] items, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				// DELETE>INSERT
				var result = repository.DeleteItemAll(items[0].OrderId);
				if (result == 0) return result;
				result = 0;
				foreach (var item in items)
				{
					result += repository.InsertItem(item);
				}
				return result;
			}
		}
		#endregion

		#region +UpdateSetPromotionForModify セットプロモーション情報更新
		/// <summary>
		/// セットプロモーション情報更新
		/// </summary>
		/// <param name="setPromotions">注文セットプロモーションモデルリスト</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateSetPromotionForModify(
			OrderSetPromotionModel[] setPromotions,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 更新
			var updated = UpdateSetPromotionForModify(setPromotions, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(setPromotions[0].OrderId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region -UpdateSetPromotionForModify セットプロモーション情報更新
		/// <summary>
		/// セットプロモーション情報更新
		/// </summary>
		/// <param name="setPromotions">注文セットプロモーションモデルリスト</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateSetPromotionForModify(OrderSetPromotionModel[] setPromotions, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				// DELETE>INSERT
				var result = repository.DeleteSetPromotionAll(setPromotions[0].OrderId);

				foreach (var setPromotion in setPromotions)
				{
					result += repository.InsertSetPromotion(setPromotion);
				}
				return result;
			}
		}

		#endregion

		#region +GetCombinableOrder 注文同梱可能な注文取得
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
		public OrderModel[] GetCombinableOrder(
			string shopId,
			string userId,
			string shippingId,
			IOrderCombineSettings orderCombineSettings,
			string[] possiblePaymentIds,
			bool fixedPurchaseSeparate,
			bool isChildOrderFixedPurchase)
		{
			using (var rep = new OrderRepository())
			{
				var models = rep.GetCombinableOrder(
					userId,
					shippingId,
					orderCombineSettings,
					possiblePaymentIds,
					fixedPurchaseSeparate,
					isChildOrderFixedPurchase);

				// 注文者&配送先&商品&クーポン&セットプロモーション&決済種別名をセット
				var shopShipServer = new ShopShippingService();
				var payment = new PaymentService().GetAll(shopId);
				foreach (var model in models)
				{
					this.SetChildModels(model, rep);

					var shopShipModel = shopShipServer.Get(shopId, model.ShippingId);
					model.ShopShippingName = shopShipModel.ShopShippingName;
					model.PaymentName = payment.First(p => p.PaymentId == model.OrderPaymentKbn).PaymentName;
				}

				return models;
			}
		}
		#endregion

		#region +GetCombineOrderCount 注文同梱可能な注文数取得
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
		public int GetCombineOrderCount(
			string userId,
			string shippingId,
			IOrderCombineSettings orderCombineSettings,
			string[] possibleCombinePaymentIds,
			bool fixedPurchaseSeparate,
			bool isChildOrderFixedPurchase)
		{
			using (var rep = new OrderRepository())
			{
				var count = rep.GetCombineOrderCount(
					userId,
					shippingId,
					orderCombineSettings,
					possibleCombinePaymentIds,
					fixedPurchaseSeparate,
					isChildOrderFixedPurchase);
				return count;
			}
		}
		#endregion

		#region +GetCombinableParentOrderWithCondition 注文同梱可能な注文取得(管理画面用)
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
		public OrderModel[] GetCombinableParentOrderWithCondition(
			string shopId,
			IOrderCombineSettings orderCombineSettings,
			bool fixedPurchaseSeparate,
			string userId,
			string userName,
			int startRowNum,
			int endRowNum)
		{
			using (var rep = new OrderRepository())
			{
				var models = rep.GetCombinableParentOrderWithCondition(
					orderCombineSettings,
					fixedPurchaseSeparate,
					userId,
					userName,
					startRowNum,
					endRowNum);

				// 注文者&配送先&商品&クーポン&セットプロモーション&決済種別名をセット
				var shopShipService = new ShopShippingService();
				var payment = new PaymentService().GetAll(shopId);
				foreach (var model in models)
				{
					this.SetChildModels(model, rep);

					var shopShipModel = shopShipService.GetOnlyModel(shopId, model.ShippingId);
					model.ShopShippingName = shopShipModel.ShopShippingName;

					model.PaymentName = payment.First(p => p.PaymentId == model.OrderPaymentKbn).PaymentName;
				}

				return models;
			}
		}
		#endregion

		#region +GetOrderByOrderIdAndCouponUseUser 注文IDとクーポン利用ユーザー(メールアドレスorユーザーID)から注文情報を取得
		/// <summary>
		/// 注文IDとクーポン利用ユーザー(メールアドレスorユーザーID)から注文情報を取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="usedUserJudgeType">利用済みユーザー判定方法</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文情報</returns>
		public OrderModel GetOrderByOrderIdAndCouponUseUser(string orderId, string couponUseUser, string usedUserJudgeType, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var order = repository.GetOrderByOrderIdAndCouponUseUser(orderId, couponUseUser, usedUserJudgeType);
				return order;
			}
		}
		#endregion

		#region +Update3DSecureInfo 3Dセキュア情報更新
		/// <summary>
		/// 3Dセキュア情報更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="tranId">3Dセキュア連携ID</param>
		/// <param name="authUrl">3Dセキュア認証URL</param>
		/// <param name="authKey">3Dセキュア認証キー</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void Update3DSecureInfo(
			string orderId,
			string tranId,
			string authUrl,
			string authKey,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// 更新
				Update3DSecureInfo(orderId, tranId, authUrl, authKey, lastChanged, accessor);

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
				}
				accessor.CommitTransaction();
			}
		}
		#endregion
		#region -Update3DSecureInfo 3Dセキュア情報更新
		/// <summary>
		/// 3Dセキュア情報更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="tranId">3Dセキュア連携ID</param>
		/// <param name="authUrl">3Dセキュア認証URL</param>
		/// <param name="authKey">3Dセキュア認証キー</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="lastChanged">最終更新者</param>
		private void Update3DSecureInfo(
			string orderId,
			string tranId,
			string authUrl,
			string authKey,
			string lastChanged,
			SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				repository.Update3DSecureInfo(orderId, tranId, authUrl, authKey, lastChanged);
			}
		}
		#endregion

		#region +Modify 更新（汎用的に利用）
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="execConditionFunc">実行条件(falseであれば実行しない）</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(
			string orderId,
			Action<OrderModel> updateAction,
			UpdateHistoryAction updateHistoryAction,
			Func<OrderModel, bool> execConditionFunc = null)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = Modify(orderId, updateAction, updateHistoryAction, accessor, execConditionFunc);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +Modify 更新（汎用的に利用）
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <param name="execConditionFunc">実行条件(falseであれば実行しない）</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(
			string orderId,
			Action<OrderModel> updateAction,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor,
			Func<OrderModel, bool> execConditionFunc = null)
		{
			// 最新データ取得
			var order = Get(orderId, accessor);

			// 条件
			var exec = (execConditionFunc == null) || execConditionFunc(order);
			if (exec == false) return 0;

			// モデル内容更新
			updateAction(order);

			// 更新
			int updated;
			using (var repository = new OrderRepository(accessor))
			{
				updated = repository.Update(order);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, order.LastChanged, accessor);
			}
			return updated;
		}
		#endregion

		#region +GetOrderByExternalOrderId 外部連携受注IDから注文情報を取得
		/// <summary>
		/// 外部連携受注IDから注文情報を取得
		/// </summary>
		/// <param name="externalOrderId">外部連携受注ID</param>
		/// <returns>注文情報</returns>
		public OrderModel GetOrderByExternalOrderId(string externalOrderId)
		{
			using (var repository = new OrderRepository())
			{
				var order = repository.GetOrderByExternalOrderId(externalOrderId);
				return order;
			}
		}
		#endregion

		#region +InsertOrder 注文情報登録
		/// <summary>
		/// 注文情報登録
		/// </summary>
		/// <param name="model">注文情報</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>True: Insert order information is successed</returns>
		public bool InsertOrder(OrderModel model, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				// 登録
				var result = repository.InsertOrder(model);
				foreach (var orderItem in model.Items)
				{
					repository.InsertItem(orderItem);
				}
				repository.InsertOwner(model.Owner);
				foreach (var orderShipping in model.Shippings)
				{
					repository.InsertShipping(orderShipping);
				}
				foreach (var orderPriceByTaxRates in model.OrderPriceByTaxRates)
				{
					repository.InsertOrderPriceByTaxRate(orderPriceByTaxRates);
				}

				// 更新履歴登録
				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForOrder(model.OrderId, model.LastChanged, accessor);
				}
				return (result > 0);
			}
		}
		#endregion

		#region +InsertOrderForOrderReturnExchange 返品交換向け注文登録 HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 返品交換向け注文登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertOrderForOrderReturnExchange(Hashtable order, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				repository.InsertOrderForOrderReturnExchange(order);
			}
		}
		#endregion

		#region +InsertOrderOwnerForOrderReturnExchange 返品交換向け注文者情報登録 HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 返品交換向け注文者情報登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertOrderOwnerForOrderReturnExchange(Hashtable order, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				repository.InsertOrderOwnerForOrderReturnExchange(order);
			}
		}
		#endregion

		#region +InsertOrderOwnerForOrderReturnExchange 注文セットプロモーション情報登録 HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 注文セットプロモーション情報登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderSetPromotion">注文セットプロモーション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertOrderSetPromotion(Hashtable orderSetPromotion, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				repository.InsertOrderSetPromotion(orderSetPromotion);
			}
		}
		#endregion

		#region +InsertOrderPriceInfoByTaxRate 税率毎価格情報登録 HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 税率毎価格情報登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderPriceByTaxRate">注文価格レート</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertOrderPriceInfoByTaxRate(Hashtable orderPriceByTaxRate, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				repository.InsertOrderPriceInfoByTaxRate(orderPriceByTaxRate);
			}
		}
		#endregion

		#region +InsertOrderShippingForOrderReturnExchange 注文配送先情報追加処理 HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 注文配送先情報追加処理 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderShipping">注文配送先情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertOrderShippingForOrderReturnExchange(Hashtable orderShipping, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				repository.InsertOrderShippingForOrderReturnExchange(orderShipping);
			}
		}
		#endregion

		#region +InsertOrderItemForOrderReturnExchange 注文商品情報追加処理(返品用：非入力項目は元注文商品から参照) HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 注文商品情報追加処理(返品用：非入力項目は元注文商品から参照) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderItem">注文アイテム</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertOrderItemForOrderReturnExchange(Hashtable orderItem, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				repository.InsertOrderItemForOrderReturnExchange(orderItem);
			}
		}
		#endregion

		#region +UpdateExternalOrderId 外部連携受注ID更新
		/// <summary>
		/// 外部連携受注ID更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalOrderId">外部連携受注ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateExternalOrderId(
			string orderId,
			string externalOrderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			Modify(
				orderId,
				model =>
				{
					model.ExternalOrderId = externalOrderId;
					model.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateExternalOrderImportStatus 外部連携取込ステータス更新
		/// <summary>
		/// 外部連携取込ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalImportStatus">外部連携取込ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateExternalOrderImportStatus(
			string orderId,
			string externalImportStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			Modify(
				orderId,
				model =>
				{
					model.ExternalImportStatus = externalImportStatus;
					model.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateLogiCooperationStatus
		/// <summary>
		/// Update logi cooperation status
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="orderStatus">Order status</param>
		/// <param name="logiCooperationStatus">Logi cooperation status</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql accessor</param>
		public void UpdateLogiCooperationStatus(
			string orderId,
			string orderStatus,
			string logiCooperationStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			Modify(
				orderId,
				model =>
				{
					model.OrderStatus = orderStatus;
					model.LogiCooperationStatus = logiCooperationStatus;
					model.DateChanged = DateTime.Now;
					model.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +GetCombinableParentOrderWithConditionCount 注文同梱可能な注文数取得
		/// <summary>
		/// 注文同梱可能な注文数取得
		/// </summary>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー名</param>
		/// <returns>同梱可能な注文数</returns>
		public int GetCombinableParentOrderWithConditionCount(
			IOrderCombineSettings orderCombineSettings,
			bool fixedPurchaseSeparate,
			string userId,
			string userName)
		{
			using (var rep = new OrderRepository())
			{
				var count = rep.GetCombinableParentOrderWithConditionCount(
					orderCombineSettings,
					fixedPurchaseSeparate,
					userId,
					userName);

				return count;
			}
		}
		#endregion

		#region +GetOrderForOrderCancel 注文情報取得(注文同梱でのキャンセル用)
		/// <summary>
		/// 注文情報取得(注文同梱でのキャンセル用)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>キャンセル用注文情報</returns>
		public DataView GetOrderForOrderCancel(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.GetOrderForOrderCancel(orderId);
				return result;
			}
		}
		#endregion

		#region +GetCombinableChildOrder 注文同梱可能な子注文取得
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
		public OrderModel[] GetCombinableChildOrder(
			string shopId,
			string userId,
			string shippingId,
			IOrderCombineSettings orderCombineSettings,
			bool fixedPurchaseSeparate,
			bool isParentOrderFixedPurchase,
			string parentOrderId,
			string parentPaymentKbn)
		{
			using (var rep = new OrderRepository())
			{
				var models = rep.GetCombinableChildOrder(
					userId,
					shippingId,
					orderCombineSettings,
					fixedPurchaseSeparate,
					isParentOrderFixedPurchase,
					parentOrderId,
					parentPaymentKbn);

				// 注文者&配送先&商品&クーポン&セットプロモーション&決済種別名をセット
				var shopShipServer = new ShopShippingService();
				var payment = new PaymentService().GetAll(shopId);
				foreach (var model in models)
				{
					this.SetChildModels(model, rep);

					var shopShipModel = shopShipServer.Get(shopId, model.ShippingId);
					model.ShopShippingName = shopShipModel.ShopShippingName;
					model.PaymentName = payment.First(p => p.PaymentId == model.OrderPaymentKbn).PaymentName;
				}

				// 親注文の決済種別が利用不可の商品を含む注文を除外する
				var combinableOrders = new List<OrderModel>();
				foreach (var model in models)
				{
					var count = new OrderService().GetCountOrderWithLimitedPaymentIds(model.OrderId, parentPaymentKbn);

					if (count != 0) continue;
					combinableOrders.Add(model);
				}

				return combinableOrders.ToArray();
			}
		}
		#endregion

		#region +UpdateMallLinkStatus モール連携ステータス更新
		/// <summary>
		/// モール連携ステータス更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="mallLinkStatus">モール連携ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateMallLinkStatus(
			string orderId,
			string mallLinkStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			Modify(
				orderId,
				model =>
				{
					model.MallLinkStatus = mallLinkStatus;
					model.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
		}
		#endregion

		#endregion

		#region +GetForAmazonOrderFulfilment Amazon出荷通知対象取得
		/// <summary>
		/// Amazon出荷通知対象取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>Amazon出荷通知対象</returns>
		public DataView GetForAmazonOrderFulfilment(string mallId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var dv = repository.GetForAmazonOrderFulfilment(mallId);
				return dv;
			}
		}
		#endregion

		#region +GetImportedAmazonOrder 取込済みAmazon注文情報取得
		/// <summary>
		/// 取込済みAmazon注文情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文情報リスト</returns>
		public OrderModel[] GetImportedAmazonOrder(string condition, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.GetImportedAmazonOrder(condition);
				return result;
			}
		}
		#endregion

		#region ~UpdateForOrderWorkflow HACK: Modifyでできるようにしたい
		/// <summary>
		/// 更新(受注ワークフロー用) HACK: Modifyでできるようにしたい
		/// </summary>
		/// <param name="statement">ステートメント</param>
		/// /// <param name="order">注文</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateForOrderWorkflow(string statement, Hashtable order, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.UpdateForOrderWorkflow(statement, order);
				return result;
			}
		}
		#endregion

		#region ~UpdateShippedChangedKbn 出荷後変更区分更新(元注文情報) HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 出荷後変更区分更新(元注文情報) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// /// <param name="order">注文</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateShippedChangedKbn(Hashtable order, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.UpdateShippedChangedKbn(order);
				return result;
			}
		}
		#endregion

		#region ~UpdateOrderReturnExchangeStatusReceipt 返品交換ステータス更新(返品交換受付) HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 返品交換ステータス更新(返品交換受付) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// /// <param name="order">注文</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateOrderReturnExchangeStatusReceipt(Hashtable order, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.UpdateOrderReturnExchangeStatusReceipt(order);
				return result;
			}
		}
		#endregion

		#region ~UpdateOrderRepaymentStatusConfrim 返金ステータス更新(未返金) HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 返金ステータス更新(未返金) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// /// <param name="order">注文</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateOrderRepaymentStatusConfrim(Hashtable order, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.UpdateOrderRepaymentStatusConfrim(order);
				return result;
			}
		}
		#endregion

		#region +UpdateAmazonShipNoteProgress Amazon出荷通知中更新
		/// <summary>
		/// Amazon出荷通知中更新
		/// </summary>
		/// <param name="condition">更新条件</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateAmazonShipNoteProgress(string condition, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				repository.UpdateAmazonShipNoteProgress(condition);
			}
		}
		#endregion

		#region +UpdateAmazonShipNoteComplete Amazon出荷通知済み更新
		/// <summary>
		/// Amazon出荷通知済み更新
		/// </summary>
		/// <param name="condition">更新条件</param>
		public void UpdateAmazonShipNoteComplete(string condition)
		{
			using (var respository = new OrderRepository())
			{
				var result = respository.UpdateAmazonShipNoteComplete(condition);
			}
		}
		#endregion

		#region +GetLohacoReserveOrder Lohacoの予約注文一覧の取得
		/// <summary>
		/// Lohaco予約注文一覧の取得
		/// </summary>
		/// <param name="mallId">LohacoモールのモールID</param>
		/// <param name="extendStatusNo">Lohaco予約注文の拡張ステタース番号</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>予約注文一覧</returns>
		public OrderModel[] GetLohacoReserveOrder(string mallId, int extendStatusNo, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.GetLohacoReserveOrder(mallId, extendStatusNo);
				return result;
			}
		}
		#endregion

		#region +GetLastFixedPurchaseOrder 定期台帳の最終注文オブジェクトを取得
		/// <summary>
		/// 定期台帳の最終注文オブジェクトを取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public OrderModel GetLastFixedPurchaseOrder(string fixedPurchaseId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetLastFixedPurchaseOrder(fixedPurchaseId);

				// 注文者&配送先&商品&クーポン&セットプロモーションをセット
				SetChildModels(model, repository);

				return model;
			}
		}
		#endregion

		#region +UpdateRelatedOrdersLastAmount 関連注文最終値(最終請求金額など)更新
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
		public bool UpdateRelatedOrdersLastAmount(
			string orderIdOrg,
			string exceptOrderId,
			decimal lastBilledAmount,
			decimal lastPointUse,
			decimal lastPointUseYen,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var relatedOrderIds = GetRelatedOrderIds(orderIdOrg, accessor).Where(x => (x != exceptOrderId)).ToArray();
			var updated = 0;
			foreach (var orderId in relatedOrderIds)
			{
				updated += UpdateOrderLastAmount(
					orderId,
					lastBilledAmount,
					lastPointUse,
					lastPointUseYen,
					lastChanged,
					updateHistoryAction,
					accessor);
			}
			return (updated == relatedOrderIds.Length);
		}
		#endregion

		#region +UpdateOrderLastAmount 注文情報最終値(最終請求金額など)更新
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
		public int UpdateOrderLastAmount(
			string orderId,
			decimal lastBilledAmount,
			decimal lastPointUse,
			decimal lastPointUseYen,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var updated = repository.UpdateOrderLastAmount(orderId, lastBilledAmount, lastPointUse, lastPointUseYen, lastChanged);

				if (updateHistoryAction == UpdateHistoryAction.Insert)
				{
					new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
				}
				return updated;
			}
		}
		#endregion

		#region +AppendExternalPaymentCooperationLog ある注文の外部決済連携ログを更新
		/// <summary>
		/// 外部決済連携ログを更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentLog">外部決済連携ログ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param> 
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新した行数</returns>
		public int AppendExternalPaymentCooperationLog(
			string orderId,
			string externalPaymentLog,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			if (string.IsNullOrEmpty(externalPaymentLog)) externalPaymentLog = "";

			int result;
			using (var repository = new OrderRepository(accessor))
			{
				result = repository.AppendExternalPaymentCooperationLog(
					orderId,
					externalPaymentLog,
					lastChanged);
			}

			// 更新履歴登録
			if ((result > 0) && (updateHistoryAction == UpdateHistoryAction.Insert))
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}

			return result;

		}
		#endregion

		#region +GetOrderedCountForProductBundle 商品同梱：過去注文回数取得
		/// <summary>
		/// 商品同梱：過去注文回数取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="bundleIds">商品同梱ID</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品同梱情報</returns>
		public Dictionary<string, int> GetOrderedCountForProductBundle(
			string userId,
			List<string> bundleIds,
			IEnumerable<string> excludeOrderIds,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.GetOrderedCountForProductBundle(
					userId, bundleIds, excludeOrderIds);
				return result;
			}
		}
		#endregion

		#region +GetReturnExchangeOrderItems 返品交換注文商品情報を取得
		/// <summary>
		/// 返品交換注文商品情報を取得
		/// </summary>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>返品交換注文商品情報</returns>
		public OrderItemModel[] GetReturnExchangeOrderItems(string orderIdOrg, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var models = repository.GetReturnExchangeOrderItems(orderIdOrg);
				return models;
			}
		}
		#endregion

		#region +UpdateOrderCountByUserId 累計購入回数更新
		/// <summary>
		/// 累計購入回数更新
		/// </summary>
		/// <param name="userId">ユーザーID更新</param>
		/// <param name="orderCountOld">データ移行しない分</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns></returns>
		public int UpdateOrderCountByUserId(string userId, int orderCountOld, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var updated = repository.UpdateOrderCountByUserId(userId, orderCountOld);
				UpdateOrderDateChangedByUserId(userId, accessor);
				return updated;
			}
		}
		#endregion

		#region UpdateInvoiceBundleFlg 請求書同梱フラグ更新
		/// <summary>
		/// 請求書更新フラグ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="invoiceBundleFlg">請求書同梱フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>成功したか</returns>
		public bool UpdateInvoiceBundleFlg(string orderId, string invoiceBundleFlg, string lastChanged, UpdateHistoryAction updateHistoryAction, SqlAccessor accessor)
		{
			var result = UpdateInvoiceBundleFlg(orderId, invoiceBundleFlg, lastChanged, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}

			return result;
		}
		/// <summary>
		/// 請求書更新フラグ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="invoiceBundleFlg">請求書同梱フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>成功したか</returns>
		public bool UpdateInvoiceBundleFlg(string orderId, string invoiceBundleFlg, string lastChanged, SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var updated = repository.UpdateInvoiceBundleFlg(orderId, invoiceBundleFlg, lastChanged);
				return (updated > 0);
			}
		}
		#endregion

		#region +UpdateOrderReceiptInfo 領収書情報更新
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
		public int UpdateOrderReceiptInfo(
			string orderId,
			string receiptFlg,
			string receiptOutputFlg,
			string receiptAddress,
			string receiptProviso,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			// 更新
			var updated = Modify(
				orderId,
				order =>
				{
					order.ReceiptFlg = receiptFlg;
					// 領収書出力フラグがNULLの場合、この値を変更しないため
					order.ReceiptOutputFlg = receiptOutputFlg ?? order.ReceiptOutputFlg;
					order.ReceiptAddress = receiptAddress;
					order.ReceiptProviso = receiptProviso;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +GetPriceInfoByTaxRateAll 注文に紐づく全ての税率毎価格情報を取得
		/// <summary>
		/// 注文に紐づく全ての税率毎価格情報を取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデルの配列</returns>
		public List<OrderPriceByTaxRateModel> GetPriceInfoByTaxRateAll(string orderId)
		{
			using (var repository = new OrderRepository())
			{
				var model = repository.GetPriceInfoByTaxRateAll(orderId);
				return model;
			}
		}
		#endregion

		#region +Get Tax Rate Include Return Exchange
		/// <summary>
		/// 交換注文返品際の税率計算
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデルの配列</returns>
		public List<OrderPriceByTaxRateModel> GetTaxRateIncludeReturnExchange(string orderId)
		{
			using (var repository = new OrderRepository())
			{
				var model = repository.GetTaxRateIncludeReturnExchange(orderId);
				return model;
			}
		}
		#endregion

		#region +GetRelatedOrderItems
		/// <summary>
		/// Get Related Order Items
		/// </summary>
		/// <param name="orderIdOrg">Order Id Org</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Related Order Items</returns>
		public OrderItemModel[] GetRelatedOrderItems(
			string orderIdOrg,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var models = repository.GetRelatedOrderItems(orderIdOrg);

				return models;
			}
		}
		#endregion

		#region +UpdateRelatedPaymentOrderId
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
		public bool UpdateRelatedPaymentOrderId(
			string orderIdOrg,
			string exceptedOrderId,
			string paymentOrderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			var relatedOrderIds = GetRelatedOrderIds(orderIdOrg, accessor)
				.Where(item => item != exceptedOrderId).ToArray();

			var updated = 0;
			foreach (var orderId in relatedOrderIds)
			{
				var result = Modify(
					orderId,
					(model) =>
					{
						model.PaymentOrderId = paymentOrderId;
						model.DateChanged = DateTime.Now;
						model.LastChanged = lastChanged;
					},
					updateHistoryAction,
					accessor);

				updated += result;
			}

			return (updated == relatedOrderIds.Length);
		}
		#endregion

		#region +GetLastOrder
		/// <summary>
		/// Get Last Order
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="exceptedPaymentIds">Excepted Payment Ids</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Last Order</returns>
		public OrderModel GetLastOrder(
			string userId,
			string[] exceptedPaymentIds,
			SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.GetLastOrder(
					userId,
					exceptedPaymentIds);

				return result;
			}
		}
		#endregion

		#region +GetOrdersWithoutReturnExchangeAndRejection
		/// <summary>
		/// Get Orders Without Return Exchange And Rejection
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="exceptedOrderStatus">Excepted Order Status</param>
		/// <param name="exceptedPaymentIds">Excepted Payment Ids</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Orders Without Return Exchange And Rejection</returns>
		public OrderModel[] GetOrdersWithoutReturnExchangeAndRejection(
			string userId,
			string[] exceptedOrderStatus,
			string[] exceptedPaymentIds,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.GetOrdersWithoutReturnExchangeAndRejection(
					userId,
					exceptedOrderStatus,
					exceptedPaymentIds);

				return result;
			}
		}
		#endregion

		#region +GetOrdersLastThreeMonthWithoutReturnExchange
		/// <summary>
		/// Get Orders Last Three Month Without Return Exchange
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="exceptedOrderStatus">Excepted Order Status</param>
		/// <param name="exceptedPaymentIds">Excepted Payment Ids</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Orders Last Three Month Without Return Exchange</returns>
		public OrderModel[] GetOrdersLastThreeMonthWithoutReturnExchange(
			string userId,
			string[] exceptedOrderStatus,
			string[] exceptedPaymentIds,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.GetOrdersLastThreeMonthWithoutReturnExchange(
					userId,
					exceptedOrderStatus,
					exceptedPaymentIds);

				return result;
			}
		}
		#endregion

		#region +GetFirstFixedPurchaseOrder Get First Fixed Purchase Order
		/// <summary>
		/// Get First Fixed Purchase Order
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public OrderModel GetFirstFixedPurchaseOrder(string fixedPurchaseId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetFirstFixedPurchaseOrder(fixedPurchaseId);

				// 注文者&配送先&商品&クーポン&セットプロモーションをセット
				SetChildModels(model, repository);
				return model;
			}
		}
		#endregion

		#region +GetOrderByCardTranId
		/// <summary>
		/// Get Order By Card Tran Id
		/// </summary>
		/// <param name="cardTranId">Card Tran Id</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Model</returns>
		public OrderModel GetOrderByCardTranId(string cardTranId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetOrderByCardTranId(cardTranId);

				// 注文者&配送先&商品&クーポン&セットプロモーションをセット
				SetChildModels(model, repository);
				return model;
			}
		}
		#endregion

		#region +GetAllOrderRelateFixedPurchase
		/// <summary>
		/// Get All Order Relate FixedPurchase
		/// </summary>
		/// <param name="fixedPurchaseId">fixedPurchaseId</param>
		/// <returns>Models Order</returns>
		public OrderModel[] GetAllOrderRelateFixedPurchase(string fixedPurchaseId)
		{
			using (var repository = new OrderRepository())
			{
				var models = repository.GetAllOrderRelateFixedPurchase(fixedPurchaseId);
				return models;
			}
		}
		#endregion

		#region +UpdateDeliveryTransactionIdAndRelationMemo
		/// <summary>
		/// Update Delivery Transaction Id And Relation Memo
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="deliveryTranId">Delivery Tran Id</param>
		/// <param name="relationMemo">Relation Memo</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Sql Accessor</param>
		public void UpdateDeliveryTransactionIdAndRelationMemo(
			string orderId,
			string deliveryTranId,
			string relationMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			Modify(
				orderId,
				order =>
				{
					order.DeliveryTranId = deliveryTranId;
					order.RelationMemo += (string.IsNullOrEmpty(order.RegulationMemo) ? string.Empty : "\r\n")
						+ relationMemo;
					order.LastChanged = lastChanged;
					order.DateChanged = DateTime.Now;
				},
				updateHistoryAction,
				accessor);
		}
		#endregion
		#region +UpdateRelationMemo 連携メモを追記
		/// <summary>
		/// 連携メモを追記
		/// </summary>
		/// <param name="orderId">オーダーID</param>
		/// <param name="relationMemo">連携メモ</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Sql Accessor</param>
		public void AppendRelationMemo(
			string orderId,
			string relationMemo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			// 書き込み
			using (var repository = new OrderRepository(accessor))
			{
				repository.AppendRelationMemo(orderId, relationMemo, lastChanged);
			}

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}
		}
		#endregion

		#region +UpdateOnlineDeliveryStatus
		/// <summary>
		/// Update Online Delivery Status
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="onlineDeliveryStatus">Online Delivery Status</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Sql Accessor</param>
		/// <returns>Update result</returns>
		public int UpdateOnlineDeliveryStatus(
			string orderId,
			string onlineDeliveryStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				order =>
				{
					order.OnlineDeliveryStatus = onlineDeliveryStatus;
					order.LastChanged = lastChanged;
					order.DateChanged = DateTime.Now;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +GetOrdersForLine
		/// <summary>
		/// Get Orders For Line
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="fixedPurchaseId">定期台帳番号</param>
		/// <param name="offset">Offset</param>
		/// <param name="limit">Limit</param>
		/// <param name="updateAt">Update At</param>
		/// <returns>Array Order Model</returns>
		public OrderModel[] GetOrdersForLine(
			string userId,
			string fixedPurchaseId,
			int offset,
			int limit,
			DateTime updateAt)
		{
			using (var repository = new OrderRepository())
			{
				var orders = repository.GetOrdersForLine(
					userId,
					fixedPurchaseId,
					offset,
					limit,
					updateAt);
				var orderList = orders
					.Select(item => (item = repository.GetWithChilds(item.OrderId)))
					.ToArray();
				return orderList;
			}
		}
		#endregion

		#region +GetOrdersByOrderStatus
		/// <summary>
		/// Get orders by order status
		/// </summary>
		/// <param name="orderStatus">Order status</param>
		/// <returns>Orders</returns>
		public OrderModel[] GetOrdersByOrderStatus(string orderStatus)
		{
			using (var repository = new OrderRepository())
			{
				var orders = repository.GetOrdersByOrderStatus(orderStatus);
				foreach (var order in orders)
				{
					this.SetChildModels(order, repository);
				}
				return orders;
			}
		}
		#endregion

		#region +GetMulitpleOrdersByOrderIdsAndPaymentKbn 決済種別と複数の受注IDから複数の受注を取得
		/// <summary>
		/// 決済種別と複数の受注IDから複数の受注を取得
		/// </summary>
		/// <param name="orderIds">複数の受注ID</param>
		/// <param name="paymentKbn">決済種別</param>
		/// <param name="sqlAccessor">トランザクションを内包するアクセサ(指定しなければメソッド内部でトランザクションが完結)</param>
		/// <returns>モデルリスト</returns>
		public OrderModel[] GetMulitpleOrdersByOrderIdsAndPaymentKbn(
			List<string> orderIds,
			string paymentKbn,
			SqlAccessor sqlAccessor = null)
		{
			// トランザクションがあれば引き継ぐ
			using (var repository = new OrderRepository(sqlAccessor))
			{
				var models = repository.GetMulitpleOrdersByOrderIdsAndPaymentKbn(orderIds, paymentKbn);
				return models;
			}
		}
		#endregion

		#region +GetExchangedOrdersInDataView 交換済み注文情報取得
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
		public DataView GetExchangedOrdersInDataView(
			string shopId,
			string orderId,
			string orderIdOrg,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var dv = repository.GetExchangedOrdersInDataView(shopId, orderId, orderIdOrg);
				return dv;
			}
		}
		#endregion

		#region +GetExchangedOrderIds 返品交換対象IDの取得
		/// <summary>
		/// 返品交換対象IDの取得(交換注文済みかつ、注文ステータスが出荷完了、配送完了)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>交換済み注文情報</returns>
		public string[] GetExchangedOrderIds(
			string shopId,
			string orderIdOrg,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orderIds = repository.GetExchangedOrderIds(shopId, orderIdOrg);
				return orderIds;
			}
		}
		#endregion

		#region +UpdateExternalPaymentStatusAndMemoForReauthByNewOrder 新しい受注から再与信した場合の外部決済ステータスやメモを更新
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
		public int UpdateExternalPaymentStatusAndMemoForReauthByNewOrder(
			OrderModel orderOld,
			string lastChanged,
			string newOrderId,
			UpdateHistoryAction updateHistoryAction,
			bool isCombine,
			SqlAccessor accessor = null)
		{
			var reauthReason = isCombine ? "注文同梱" : "アップセル";
			var paymentMemo = string.Format(
				"{0} 決済取引ID：{1}・{2}・このコンビニ後払い与信は{3}によって新しい受注情報に移されました。 新しい受注ID：{4}",
				DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
				orderOld.OrderId,
				orderOld.CardTranId,
				reauthReason,
				newOrderId);

			var updated = Modify(
				orderOld.OrderId,
				(model) =>
				{
					model.PaymentMemo = OrderExternalPaymentUtility.SetExternalPaymentMemo(
						StringUtility.ToEmpty(orderOld.PaymentMemo),
						paymentMemo);
					model.OnlinePaymentStatus = Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_CANCELED;
					model.LastChanged = lastChanged;
					model.ExternalPaymentStatus = Constants.FLG_ORDER_EXTERNAL_PAYMENT_STATUS_CANCEL;
					model.DateChanged = DateTime.Now;
					model.OrderCancelDate = DateTime.Now;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +UpdateCouponForIntegration 注文クーポン情報更新(ユーザー統合時)
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
		public int UpdateCouponForIntegration(
			string userId,
			string couponId,
			string couponNo,
			string couponNoOld,
			string lastChenged,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.UpdateCouponForIntegration(
					userId,
					couponId,
					couponNo,
					couponNoOld,
					lastChenged);

				//最終更新日時更新
				UpdateOrderDateChangedForIntegration(userId, couponId, couponNoOld, accessor);
				return result;
			}
		}
		#endregion

		#region +GetOrderCountByOrderWorkflowSetting
		/// <summary>
		/// Get order count by order workflow setting
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <returns>Order count</returns>
		public int GetOrderCountByOrderWorkflowSetting(Hashtable searchParam)
		{
			using (var repository = new OrderRepository())
			{
				var orderCount = repository.GetOrderCountByOrderWorkflowSetting(searchParam);
				return orderCount;
			}
		}
		#endregion

		#region +GetItemAll
		/// <summary>
		/// Get item all
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <returns>Order item models</returns>
		public OrderItemModel[] GetItemAll(string orderId)
		{
			using (var repository = new OrderRepository())
			{
				var models = repository.GetItemAll(orderId);
				return models;
			}
		}
		#endregion

		#region +GetOrderWorkflowListNoPagination
		/// <summary>
		/// Get order workflow list no pagination
		/// </summary>
		/// <param name="htSearch">Search condition</param>
		/// <returns>Dataview of order list</returns>
		public DataView GetOrderWorkflowListNoPagination(Hashtable htSearch)
		{
			using (var repository = new OrderRepository())
			{
				var orders = repository.GetOrderWorkflowListNoPagination(htSearch);
				return orders;
			}
		}
		#endregion

		#region +GetDeliveryTranIdListOrderWorkFlow
		/// <summary>
		/// Get delivery tran id list order workflow
		/// </summary>
		/// <param name="htSearch">Search condition</param>
		/// <returns>Dataview of delivery tran id list</returns>
		public DataView GetDeliveryTranIdListOrderWorkFlow(Hashtable htSearch)
		{
			using (var repository = new OrderRepository())
			{
				var orders = repository.GetDeliveryTranIdListOrderWorkFlow(htSearch);
				return orders;
			}
		}
		#endregion

		/// <summary>
		/// 仮登録ステータスか確認する
		/// </summary>
		/// <param name="orderId">Order ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public bool CheckTemporaryRegistration(string orderId, SqlAccessor accessor = null)
		{
			if (string.IsNullOrEmpty(orderId)) return false;

			var status = Get(orderId, accessor).OrderStatus;
			return (status == Constants.FLG_ORDER_ORDER_STATUS_TEMP);
		}

		#region +GetOrdersForElogitWmsCooperation
		/// <summary>
		/// Get orders for elogit wms cooperation
		/// </summary>
		/// <returns></returns>
		public OrderModel[] GetOrdersForElogitWmsCooperation()
		{
			using (var repository = new OrderRepository())
			{
				var orders = repository.GetOrdersForElogitWmsCooperation();
				return orders;
			}
		}
		#endregion

		#region +UpdateOrderForWmsShipping
		/// <summary>
		/// Update order for Wms shipping
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="shippingCheckNo">Shipping check no</param>
		/// <param name="orderShippedDate">Order shipped date</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int UpdateOrderForWmsShipping(
			string orderId,
			string shippingCheckNo,
			DateTime? orderShippedDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			// Update shipping check no
			var updatedShipping = UpdateOrderShippingCheckNo(orderId, 1, shippingCheckNo, accessor);

			// Update order shipped date
			var updated = UpdateOrderShippedDate(orderId, orderShippedDate, accessor);

			// Insert history
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion

		#region +UpdateOrderShippedDate
		/// <summary>
		/// Update order shipped date
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="orderShippedDate">Order shipped date</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		private int UpdateOrderShippedDate(string orderId, DateTime? orderShippedDate, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.Get(orderId);

				// Set value for order shipped date
				model.OrderShippedDate = orderShippedDate;

				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +UpdateOrderExtend 注文拡張項目の更新
		/// <summary>
		/// 注文拡張項目の更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChange">最終更新者</param>
		/// <param name="values">変更内容</param>
		/// <param name="updateHistoryAction">更新履歴</param>
		/// <param name="accessor">Sqlアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateOrderExtend(
			string orderId,
			string lastChange,
			Dictionary<string, string> values,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			if (values.Count == 0) return 0;

			var result = Modify(
				orderId,
				model =>
				{
					foreach (var value in values.Where(value => model.DataSource.ContainsKey(value.Key)))
					{
						model.DataSource[value.Key] = value.Value;
					}
					model.LastChanged = lastChange;
				},
				updateHistoryAction,
				accessor);
			return result;
		}
		#endregion

		#region +CheckOrderFirstBuy
		/// <summary>
		/// 初回購入チェック
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="exclusionOrderId">除外注文ID</param>
		/// <param name="accessor">Sqlアクセサ</param>
		/// <returns>True:初回購入 False:初回購入ではない</returns>
		public bool CheckOrderFirstBuy(string userId, string exclusionOrderId = null, SqlAccessor accessor = null)
		{
			var hasOrders = HasUncancelledOrders(userId, exclusionOrderId, accessor);
			return (hasOrders == false);
		}
		#endregion

		#region +UpdateDepositAmountNotMatchError
		/// <summary>
		/// 入金額不整合発生時処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateDepositAmountNotMatchError(
			OrderModel order,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdatePaymentStatus(
					order.OrderId,
					Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_SHORTAGE,
					DateTime.Now,
					lastChanged,
					updateHistoryAction,
					accessor);

				UpdateManagementMemo(
					order.OrderId,
					order.ManagementMemo,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +UpdateDoubleDepositError
		/// <summary>
		/// 二重入金発生時処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="extendStatusNo">拡張ステータスNo</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateDoubleDepositError(
			OrderModel order, int extendStatusNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateOrderExtendStatus(
					order.OrderId,
					extendStatusNo,
					Constants.FLG_ORDER_EXTEND_STATUS_ON,
					DateTime.Now,
					lastChanged,
					updateHistoryAction,
					accessor);

				UpdateManagementMemo(
					order.OrderId,
					order.ManagementMemo,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +UpdateOrderCvsDefDskPreliminaryPaymentCancel
		/// <summary>
		/// 電算コンビニ後払い注文の入金状況を速報キャンセルに更新
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="extendStatusNo">拡張ステータスNo</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateOrderCvsDefDskPreliminaryPaymentCancel(
			OrderModel order,
			int extendStatusNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdatePaymentStatus(
					order.OrderId,
					Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM,
					(DateTime?)null,
					lastChanged,
					updateHistoryAction,
					accessor);

				// (手動編集などで)入金状況が確定になっている場合は未確定に戻す
				if (order.ExtendStatus[extendStatusNo - 1].Value == Constants.FLG_ORDER_EXTEND_STATUS_ON)
				{
					UpdateOrderExtendStatus(
						order.OrderId,
						extendStatusNo,
						Constants.FLG_ORDER_EXTEND_STATUS_OFF,
						DateTime.Now,
						lastChanged,
						updateHistoryAction,
						accessor);
				}

				UpdateManagementMemo(
					order.OrderId,
					order.ManagementMemo,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +UpdateOrderCvsDefDskPaymentConfirmed
		/// <summary>
		/// 電算コンビニ後払い注文の入金状況を確定に更新
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="extendStatusNo">拡張ステータスNo</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public void UpdateOrderCvsDefDskPaymentConfirmed(
			OrderModel order,
			int extendStatusNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				UpdateOrderExtendStatus(
					order.OrderId,
					extendStatusNo,
					Constants.FLG_ORDER_EXTEND_STATUS_ON,
					DateTime.Now,
					lastChanged,
					updateHistoryAction,
					accessor);

				UpdateManagementMemo(
					order.OrderId,
					order.ManagementMemo,
					lastChanged,
					updateHistoryAction,
					accessor);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +UpdateOrderStatusAndExternalPaymentStatus 注文ステータスと外部決済ステータスを更新
		/// <summary>
		/// 注文ステータスと外部決済ステータスを更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderStatus">注文ステータス</param>
		/// <param name="externalPaymentStatus">外部決済ステータス</param>
		/// <param name="externalPaymentAuthDate">外部決済与信日時</param>
		/// <param name="updateDate">更新日時</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateOrderStatusAndExternalPaymentStatus(
			string orderId,
			string orderStatus,
			string externalPaymentStatus,
			DateTime? externalPaymentAuthDate,
			DateTime updateDate,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			var updated = Modify(
				orderId,
				(model) =>
				{
					model.OrderStatus = orderStatus;
					model.ExternalPaymentStatus = externalPaymentStatus;
					model.ExternalPaymentAuthDate = externalPaymentAuthDate;
					model.DateChanged = updateDate;
					model.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +GetCvsDeferredAuthResultHold コンビニ後払いで与信結果がHOLDの注文を取得
		/// <summary>
		/// コンビニ後払いで与信結果がHOLDの注文を取得
		/// </summary>
		/// <returns>注文</returns>
		public OrderModel[] GetCvsDeferredAuthResultHold()
		{
			var result = new OrderRepository().GetCvsDeferredAuthResultHold();
			return result;
		}
		#endregion

		#region +GetOrderForGmoAtokaraAuthResult GMOアトカラで与信結果が審査中の注文を取得
		/// <summary>
		/// GMOアトカラで与信結果が審査中の注文を取得
		/// </summary>
		/// <returns>注文</returns>
		public OrderModel[] GetOrderForGmoAtokaraAuthResult()
		{
			var result = new OrderRepository().GetOrderForGmoAtokaraAuthResult();
			return result;
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		///  CSVへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="replacesValueForOrderby">@@ orderby @@ のreplaces文字列</param>
		/// <param name="replacesValueForMultiOrderId">@@ multi_order_id @@ のreplaces文字列</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		public bool ExportToCsv(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			Stream outputStream,
			string formatDate,
			string replacesValueForOrderby,
			string replacesValueForMultiOrderId,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var accessor = new SqlAccessor())
			using (var repository = new OrderRepository(accessor))
			using (var reader = repository.GetMasterWithReader(
				input,
				statementName,
				new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames),
				new KeyValuePair<string, string>("@@ orderby @@", replacesValueForOrderby),
				new KeyValuePair<string, string>("@@ multi_order_id @@", replacesValueForMultiOrderId),
				new KeyValuePair<string, string>("@@ order_extend_field_name @@",
					string.Format("{0}.{1}", Constants.TABLE_ORDER, StringUtility.ToEmpty(input[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]))),
				new KeyValuePair<string, string>("@@ where @@",
					StringUtility.ToEmpty(input["@@ where @@"])),
				new KeyValuePair<string, string>("@@ order_shipping_addr1 @@", StringUtility.ToEmpty(input[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1]))))
			{
				new MasterExportCsv().Exec(setting, reader, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
			}
			return true;
		}

		/// <summary>
		/// Excelへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="excelTemplateSetting">Excelテンプレート設定</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="replacesValueForOrderby">@@ orderby @@ のreplaces文字列</param>
		/// <param name="replacesValueForMultiOrderId">@@ multi_order_id @@ のreplaces文字列</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か（件数エラーの場合は失敗）</returns>
		public bool ExportToExcel(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			ExcelTemplateSetting excelTemplateSetting,
			Stream outputStream,
			string formatDate,
			string replacesValueForOrderby,
			string replacesValueForMultiOrderId,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var repository = new OrderRepository())
			{
				var dv = repository.GetMaster(input,
					statementName,
					new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames),
					new KeyValuePair<string, string>("@@ orderby @@", replacesValueForOrderby),
					new KeyValuePair<string, string>("@@ multi_order_id @@", replacesValueForMultiOrderId),
					new KeyValuePair<string, string>("@@ order_extend_field_name @@",
						string.Format("{0}.{1}", Constants.TABLE_ORDER, StringUtility.ToEmpty(input[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]))),
					new KeyValuePair<string, string>("@@ where @@",
						StringUtility.ToEmpty(input["@@ where @@"])),
					new KeyValuePair<string, string>("@@ order_shipping_addr1 @@", StringUtility.ToEmpty(input[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1])));
				if (dv.Count >= 20000) return false;

				new MasterExportExcel().Exec(setting, excelTemplateSetting, dv, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				return true;
			}
		}

		/// <summary>
		/// マスタ区分で該当するStatementNameを取得
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>StatementName</returns>
		public string GetStatementNameInfo(string masterKbn)
		{
			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER: // 注文マスタ表示
					return "GetOrderMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM: // 注文商品マスタ表示
					return "GetOrderItemMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION: // 注文セットプロモーションマスタ表示
					return "GetOrderSetPromotionMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING:
					return "GetOrderDataBinding";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER_WORKFLOW: // 注文マスタ表示（ワークフロー）
					return "GetOrderWorkflowMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM_WORKFLOW: // 注文商品マスタ表示（ワークフロー）
					return "GetOrderItemWorkflowMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION_WORKFLOW: // 注文セットプロモーションマスタ表示（ワークフロー）
					return "GetOrderSetPromotionWorkflowMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING_WORKFLOW:
					return "GetOrderDataBindingWorkflowMaster";
			}
			throw new Exception("未対応のマスタ区分：" + masterKbn);
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="shopId">ショップID</param>
		/// <returns>チェックOKか</returns>
		public bool CheckFieldsForGetMaster(string sqlFieldNames, string masterKbn, string shopId)
		{
			try
			{
				using (var repository = new OrderRepository())
				{
					repository.CheckFieldsForGetMaster(
						new Hashtable { { Constants.FIELD_ORDER_SHOP_ID, shopId } },
						masterKbn,
						new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteWarn(ex);
				return false;
			}
			return true;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="shopId">ショップID</param>
		/// <returns>チェックOKか</returns>
		public bool CheckFieldsForGetMasterDataBinding(string sqlFieldNames, string masterKbn, string shopId)
		{
			try
			{
				using (var repository = new OrderRepository())
				{
					repository.CheckFieldsForGetMasterDataBinding(
						new Hashtable { { Constants.FIELD_ORDER_SHOP_ID, shopId } },
						masterKbn,
						new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteWarn(ex);
				return false;
			}
			return true;
		}
		#endregion

		#region +UpdateOrderShippingForImportShipping
		/// <summary>
		/// Update order shipping for import shipping
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="orderShippingNo">Order shipping no</param>
		/// <param name="shippingCheckNo">Shipping check no</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of updates</returns>
		public int UpdateOrderShippingForImportShipping(
			string orderId,
			int orderShippingNo,
			string shippingCheckNo,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// Update
			var updated = UpdateOrderShippingForImportShipping(
				orderId,
				orderShippingNo,
				shippingCheckNo,
				accessor);

			// Update history registration
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(orderId, lastChanged, accessor);
			}
			return updated;
		}
		#endregion
		#region +UpdateOrderShippingForImportShipping
		/// <summary>
		/// Update order shipping for import shipping
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="orderShippingNo">Order shipping no</param>
		/// <param name="shippingCheckNo">Shipping check no</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of updates</returns>
		private int UpdateOrderShippingForImportShipping(
			string orderId,
			int orderShippingNo,
			string shippingCheckNo,
			SqlAccessor accessor)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetShipping(orderId, orderShippingNo);

				// Shipping check no
				model.ShippingCheckNo = shippingCheckNo;

				var result = repository.UpdateShipping(model);
				return result;
			}
		}
		#endregion

		#region +UpdateFixedPerchaseItemOrderCount 定期商品購入回数(注文時点)更新
		/// <summary>
		/// 定期商品購入回数(注文時点)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseItems">定期購入商品</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateFixedPerchaseItemOrderCount(
			string orderId,
			FixedPurchaseItemModel[] fixedPurchaseItems,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var orderItems = Get(orderId, accessor).Shippings.SelectMany(s => s.Items).ToArray();
			foreach (var orderItem in orderItems)
			{
				var fixedPurchaseItemOrder = fixedPurchaseItems.FirstOrDefault(i => (i.ProductId == orderItem.ProductId) && (i.VariationId == orderItem.VariationId));
				orderItem.FixedPurchaseItemOrderCount = (fixedPurchaseItemOrder == null) ? 1 : fixedPurchaseItemOrder.ItemOrderCount;
			}

			// 更新
			UpdateItemForModify(
				orderItems,
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateFixedPerchaseItemShippedCount 定期商品購入回数(出荷時点)更新
		/// <summary>
		/// 定期商品購入回数(出荷時点)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateFixedPerchaseItemShippedCount(
			string orderId,
			string fixedPurchaseId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var fixedPurchaseItems = new FixedPurchaseService().GetAllItem(fixedPurchaseId, accessor);
			var orderItems = Get(orderId, accessor).Shippings.SelectMany(s => s.Items).ToArray();
			foreach (var orderItem in orderItems)
			{
				var fixedPurchaseItemOrder = fixedPurchaseItems.FirstOrDefault(i => (i.ProductId == orderItem.ProductId) && (i.VariationId == orderItem.VariationId));
				orderItem.FixedPurchaseItemShippedCount = (fixedPurchaseItemOrder == null) ? 0 : fixedPurchaseItemOrder.ItemShippedCount;
			}

			// 更新
			UpdateItemForModify(
				orderItems,
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateFixedPerchaseItemOrderCountWhenOrdering 注文時の定期商品購入回数(注文時点)更新
		/// <summary>
		/// 注文時の定期商品購入回数(注文時点)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="fixedPurchaseItems">定期購入商品</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateFixedPerchaseItemOrderCountWhenOrdering(
			string orderId,
			FixedPurchaseItemModel[] fixedPurchaseItems,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 定期商品購入回数（注文時点）+ 1
			foreach (var item in fixedPurchaseItems)
			{
				item.ItemOrderCount++;
			}

			UpdateFixedPerchaseItemOrderCount(
				orderId,
				fixedPurchaseItems,
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +UpdateFixedPerchaseItemOrderCountWhenOrderCombine 定期商品購入回数(注文時点)更新(注文同梱時)
		/// <summary>
		/// 定期商品購入回数(注文時点)更新(注文同梱時)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="combinedOrgOrderId">注文同梱元注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateFixedPerchaseItemOrderCountWhenOrderCombine(
			string orderId,
			string combinedOrgOrderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var combinedOrgOrderIds = combinedOrgOrderId.Split(',');
			// 注文同梱元の注文情報を取得
			var combineOrderItemModels = combinedOrgOrderIds.Select(combineOrderId => Get(combineOrderId, accessor)).Select(orderModel => orderModel.Shippings[0].Items);

			// 注文同梱後の注文情報を取得
			var orderItems = Get(orderId, accessor).Shippings.SelectMany(s => s.Items).ToArray();

			// 注文同梱元の商品購入回数を引き継ぐ
			foreach (var orderItem in orderItems)
			{
				var orderItemCount = new List<int?>();
				foreach (var combineOrderItems in combineOrderItemModels)
				{
					var combineOrderItem = combineOrderItems.FirstOrDefault(i => (i.ProductId == orderItem.ProductId) && (i.VariationId == orderItem.VariationId));
					if (combineOrderItem != null) orderItemCount.Add(combineOrderItem.FixedPurchaseItemOrderCount ?? 1);
				}
				orderItem.FixedPurchaseItemOrderCount = (orderItemCount.Count == 0) ? 1 : orderItemCount.Max();
			}

			// 更新
			UpdateItemForModify(
				orderItems,
				lastChanged,
				updateHistoryAction,
				accessor);
		}
		#endregion

		#region +GetFixedPurchaseItemOrderCount 定期商品購入回数取得
		/// <summary>
		/// 定期商品購入回数取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>定期商品購入回数</returns>
		public int GetFixedPurchaseItemOrderCount(string orderId, string productId, string variationId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetItemByOrderIdAndProductId(orderId, productId, variationId);
				var itemOrderCount = (model == null)
					? 1
					: (model.FixedPurchaseItemOrderCount ?? 1);
				return itemOrderCount;
			}
		}
		#endregion

		#region +UpdatePaypaySBPSInfo
		/// <summary>
		/// Update paypay sbps info
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="paymentOrderId">Payment order id</param>
		/// <param name="lastChanged">Last changed</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int UpdatePaypaySBPSInfo(
			string orderId,
			string paymentOrderId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			var updated = Modify(
				orderId,
				order =>
				{
					order.PaymentOrderId = paymentOrderId;
					order.LastChanged = lastChanged;
				},
				updateHistoryAction,
				accessor);
			return updated;
		}
		#endregion

		#region +GetOrderPaymentIdsForAtobaraicomGetAuthorizeStatus
		/// <summary>
		/// Get order payment ids for atobaraicom get authorize status
		/// </summary>
		public string[] GetOrderPaymentIdsForAtobaraicomGetAuthorizeStatus()
		{
			using (var repository = new OrderRepository())
			{
				var orderPaymentIds = repository.GetOrderPaymentIdsForAtobaraicomGetAuthorizeStatus();
				return orderPaymentIds;
			}
		}
		#endregion

		#region +GetPointGrantOrder ポイント確定処理対象の注文情報を取得
		/// <summary>
		/// Get point grant order
		/// </summary>
		/// <param name="days">Days</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Array of order model</returns>
		public OrderModel[] GetPointGrantOrder(int days, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.GetPointGrantOrder(days);
				return model;
			}
		}
		#endregion

		#region +GetOnlinePaymentStatusByOrderId
		/// <summary>
		/// 注文IDでオンライン決済ステータス取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>オンライン決済ステータス</returns>
		public string GetOnlinePaymentStatusByOrderId(string orderId)
		{
			using (var repository = new OrderRepository())
			{
				var onlinePaymentStatus = repository.GetOnlinePaymentStatusByOrderId(orderId);
				return onlinePaymentStatus;
			}
		}
		#endregion

		#region +GetInReviewGmoOrder
		/// <summary>
		/// 審査中の注文リスト取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>注文一覧</returns>
		public OrderModel[] GetInReviewGmoOrders(SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var listOrder = repository.GetInReviewGmoOrders();
				return listOrder;
			}
		}
		#endregion

		#region +GetOrderHistoryListByFixedPurchaseId 注文履歴一覧（定期台帳単位）を取得する HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文履歴一覧（注文単位）を取得する HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public DataView GetOrderHistoryListByFixedPurchaseId(
			string userId,
			int startRowNum,
			int endRowNum,
			string fixedPurchaseId,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orders = repository.GetOrderHistoryListByFixedPurchaseId(userId, startRowNum, endRowNum, fixedPurchaseId);
				return orders;
			}
		}
		#endregion

		#region +GetOrdersForYahooOrderImport YAHOOモール注文取り込みを行う注文を取得
		/// <summary>
		/// YAHOOモール注文取り込みを行う注文を取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>辞書型注文情報</returns>
		public Dictionary<string, string>[] GetOrdersForYahooOrderImport(string mallId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var orders = repository.GetOrdersForYahooOrderImport(mallId);
				return orders;
			}
		}
		#endregion

		#region +AddYahooMallOrderDetail YAHOOモール注文取り込みした注文情報を更新
		/// <summary>
		/// Update store pickup status
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>モデル</returns>
		public bool AddYahooMallOrderDetail(Hashtable input, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.AddYahooMallOrderDetail(input);
				return (result > 0);
			}
		}
		#endregion

		#region +AddYahooMallOrderOwnerDetail YAHOOモール注文取り込みした注文者情報を更新
		/// <summary>
		/// YAHOOモール注文取り込みした注文者情報を更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>モデル</returns>
		public bool AddYahooMallOrderOwnerDetail(Hashtable input, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.AddYahooMallOrderOwnerDetail(input);
				return (result > 0);
			}
		}
		#endregion

		#region +AddYahooMallOrderShippingDetail YAHOOモール注文取り込みした注文配送情報を更新
		/// <summary>
		/// YAHOOモール注文取り込みした注文配送情報を更新
		/// </summary>
		/// <param name="input">入力値</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>モデル</returns>
		public bool AddYahooMallOrderShippingDetail(Hashtable input, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.AddYahooMallOrderShippingDetail(input);
				return (result > 0);
			}
		}
		#endregion

		#region +GetAllForOrderMail 取得（注文メール用）
		/// <summary>
		/// 取得（注文メール用）
		/// </summary>
		/// <returns>メール送信用注文情報列</returns>
		public OrderModel[] GetAllForOrderMail()
		{
			using (var repository = new OrderRepository())
			{
				var mailDatas = repository.GetAllForOrderMail();
				return mailDatas;
			}
		}
		#endregion

		#region +UpdateStorePickupSatus
		/// <summary>
		/// Update store pickup status
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="storePickupStatus">店舗受取ステータス</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public int UpdateStorePickupSatus(
			string orderId,
			string storePickupStatus,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var model = repository.Get(orderId);

				model.StorePickupStatus = storePickupStatus;
				model.LastChanged = lastChanged;

				switch (storePickupStatus)
				{
					case Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_ARRIVED:
						model.StorePickupStoreArrivedDate = DateTime.Now;
						break;

					case Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_DELIVERED:
						model.StorePickupDeliveredCompleteDate = DateTime.Now;
						break;

					case Constants.FLG_ORDER_ORDER_STORE_PICKUP_STATUS_RETURNED:
						model.StorePickupReturnDate = DateTime.Now;
						break;
				}

				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +UpdateStorePickupStatus
		/// <summary>
		/// Update store pickup status
		/// </summary>
		/// <param name="input">Input</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Number of affected cases</returns>
		public int UpdateStorePickupStatus(Hashtable input, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var result = repository.UpdateStorePickupStatus(input);
				return result;
			}
		}
		#endregion

		#region +GetOrderShoppickUp
		/// <summary>
		/// Get order store pick up count
		/// </summary>
		/// <param name="input">Input</param>
		/// <param name="realShopIds">Real shop ids</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Number of affected cases</returns>
		public int GetOrderStorePickUpCount(
			Hashtable input,
			string realShopIds = "",
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var count = repository.GetOrderStorePickUpCount(input, realShopIds);
				return count;
			}
		}
		#endregion

		#region +GetOrdersForLetro
		/// <summary>
		/// Get orders for Letro
		/// </summary>
		/// <param name="searchInput">Search input</param>
		/// <returns>Orders for Letro</returns>
		public OrderModel[] GetOrdersForLetro(Hashtable searchInput)
		{
			using (var repository = new OrderRepository())
			{
				var orders = repository.GetOrdersForLetro(searchInput);
				return orders;
			}
		}
		#endregion

		#region +GetOrderItemsForLetro
		/// <summary>
		/// Get order items for Letro
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <returns>Order items for Letro</returns>
		public OrderItemModel[] GetOrderItemsForLetro(string orderId)
		{
			using (var repository = new OrderRepository())
			{
				var orders = repository.GetOrderItemsForLetro(orderId);
				return orders;
			}
		}
		#endregion

		#region +GetCombineOrgOrderIds
		/// <summary>
		/// 注文同梱元注文IDを取得
		/// </summary>
		/// <param name="orderId">同梱注文ID</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Number of affected cases</returns>
		public string GetCombineOrgOrderIds(
			string orderId,
			SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var count = repository.GetCombineOrgOrderIds(orderId);
				return count;
			}
		}
		#endregion

		#region +GetShippingOrderForDate 指定期間の出荷注文情報を取得(日次出荷予測レポート用)
		/// <summary>
		/// 指定期間の出荷注文情報を取得
		/// </summary>
		/// <param name="minDate">指定日最小値</param>
		/// <param name="maxDate">指定日最大値</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>注文情報</returns>
		public DataView GetShippingOrderForDate(DateTime minDate, DateTime maxDate, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var dv = repository.GetShippingOrderForDate(minDate, maxDate);
				return dv;
			}
		}
		#endregion

		#region +GetCountOrderWithLimitedPaymentIds 指定決済が利用不可決済になる商品を持つ注文の件数取得(注文同梱で利用)
		/// <summary>
		/// 指定決済が利用不可決済になる商品を持つ注文の件数取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>件数</returns>
		public int GetCountOrderWithLimitedPaymentIds(string orderId, string paymentId, SqlAccessor accessor = null)
		{
			using (var repository = new OrderRepository(accessor))
			{
				var count = repository.GetCountOrderWithLimitedPaymentIds(orderId, paymentId);
				return count;
			}
		}
		#endregion
	}
}
