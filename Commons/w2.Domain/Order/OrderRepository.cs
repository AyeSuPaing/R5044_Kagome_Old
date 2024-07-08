/*
=========================================================================================================
  Module      : 注文情報リポジトリ (OrderRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order.Helper;
using w2.Domain.TwOrderInvoice;

namespace w2.Domain.Order
{
	/// <summary>
	/// 注文情報リポジトリ
	/// </summary>
	public class OrderRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "Order";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetWithChilds 子も含めて全て取得
		/// <summary>
		/// 子も含めて全て取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		public OrderModel GetWithChilds(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
			};

			var ds = GetWithChilds(
				new[]
				{
					new KeyValuePair<string ,string>(XML_KEY_NAME, "Get"),
					new KeyValuePair<string ,string>(XML_KEY_NAME, "GetOwner"),
					new KeyValuePair<string ,string>(XML_KEY_NAME, "GetShippingAll"),
					new KeyValuePair<string ,string>(XML_KEY_NAME, "GetItemAll"),
					new KeyValuePair<string ,string>(XML_KEY_NAME, "GetAllCoupons"),
					new KeyValuePair<string ,string>(XML_KEY_NAME, "GetSetPromotionAll"),
					new KeyValuePair<string ,string>(XML_KEY_NAME, "GetPriceInfoByTaxRateAll"),
					new KeyValuePair<string ,string>(XML_KEY_NAME, "GetInvoiceAll"),
				},
				ht);

			var order = (ds.Tables[0].DefaultView.Count != 0) ? new OrderModel(ds.Tables[0].DefaultView[0]) : null;
			if (order == null) return null;

			order.Owner = (ds.Tables[1].DefaultView.Count != 0) ? new OrderOwnerModel(ds.Tables[1].DefaultView[0]) : null;
			order.Shippings = ds.Tables[2].DefaultView.Cast<DataRowView>().Select(drv => new OrderShippingModel(drv)).ToArray();
			var items =
				ds.Tables[3].DefaultView.Cast<DataRowView>()
					.Select(drv => new OrderItemModel(drv))
					.OrderBy(i => i.OrderShippingNo)
					.ThenBy(i => i.OrderItemNo)
					.ToArray();
			foreach (var shipping in order.Shippings)
			{
				shipping.Items = items.Where(i => i.OrderShippingNo == shipping.OrderShippingNo).ToArray();
			}
			order.Items = ds.Tables[3].DefaultView.Cast<DataRowView>().Select(drv => new OrderItemModel(drv)).ToArray();
			order.Coupons = ds.Tables[4].DefaultView.Cast<DataRowView>().Select(drv => new OrderCouponModel(drv)).ToArray();
			order.SetPromotions = ds.Tables[5].DefaultView.Cast<DataRowView>().Select(drv => new OrderSetPromotionModel(drv)).ToArray();
			order.OrderPriceByTaxRates = ds.Tables[6].DefaultView.Cast<DataRowView>().Select(drv => new OrderPriceByTaxRateModel(drv)).ToArray();
			order.Invoices = ds.Tables[7].DefaultView.Cast<DataRowView>().Select(drv => new TwOrderInvoiceModel(drv)).ToArray();

			return order;
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		public OrderModel Get(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new OrderModel(dv[0]);
		}
		#endregion

		#region ~Count 注文情報の件数取得
		/// <summary>
		/// 注文情報の件数取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文情報の件数</returns>
		internal int Count(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "Count", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region +Get order by payment order id
		/// <summary>
		/// Get order by payment order id
		/// </summary>
		/// <param name="paymentOrderId">Payment order id</param>
		/// <returns>Model</returns>
		public OrderModel GetOrderByPaymentOrderId(string paymentOrderId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_ORDER_PAYMENT_ORDER_ID, paymentOrderId},
			};
			var data = Get(XML_KEY_NAME, "GetOrderByPaymentOrderId", input);
			if (data.Count == 0) return null;
			return new OrderModel(data[0]);
		}
		#endregion

		#region +GetRelatedOrders 返品交換含む注文情報取得（全ての注文情報を含む）
		/// <summary>
		/// 取得（返品交換含む）
		/// </summary>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <returns>モデル</returns>
		public OrderModel[] GetRelatedOrders(string orderIdOrg)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID_ORG, orderIdOrg},
			};
			var dv = Get(XML_KEY_NAME, "GetRelatedOrders", ht);
			var orders = new OrderHelper().GetOrders(dv);
			return orders;
		}
		#endregion

		#region +GetRelatedOrderIds 返品交換含む注文ID取得
		/// <summary>
		/// 返品交換含む注文ID取得
		/// </summary>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <returns>注文ID列</returns>
		public string[] GetRelatedOrderIds(string orderIdOrg)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID_ORG, orderIdOrg},
			};
			var dv = Get(XML_KEY_NAME, "GetRelatedOrderIds", ht);
			return dv.Cast<DataRowView>().Select(drv => (string)drv[0]).ToArray();
		}
		#endregion

		#region ~Get Orders By External Deliverty Status And Update Date
		/// <summary>
		/// Get Orders By External Deliverty Status And Update Date
		/// </summary>
		/// <param name="deadlineDate">Deadline Date</param>
		/// <param name="shippingExternalDelivertyStatus">Shipping External Deliverty Status</param>
		/// <returns>List Order</returns>
		public List<OrderShippingModel> GetOrdersByExternalDelivertyStatusAndUpdateDate(
			DateTime deadlineDate,
			string shippingExternalDelivertyStatus)
		{
			var input = new Hashtable
			{
				{ "deadline_date", deadlineDate },
				{ "shipping_external_deliverty_status", shippingExternalDelivertyStatus }
			};
			var data = Get(XML_KEY_NAME, "GetOrdersByExternalDelivertyStatusAndUpdateDate", input);

			return data.Cast<DataRowView>().Select(row => new OrderShippingModel(row)).ToList();
		}
		#endregion

		#region +GetOrderLatest
		/// <summary>
		/// Get Order Latest
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <returns>Get Order Latest</returns>
		public OrderModel GetOrderLatest(string userId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_USER_ID, userId }
			};
			var data = Get(XML_KEY_NAME, "GetOrderLatest", input);
			return (data.Count == 0)
				? null
				: new OrderModel(data[0]);
		}
		#endregion

		#region ~GetFirstOrder
		/// <summary>
		/// 初回購入注文を取得
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <returns>初回購入注文</returns>
		internal OrderModel GetFirstOrder(string userId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_USER_ID, userId }
			};
			var dv = Get(XML_KEY_NAME, "GetFirstOrder", input);
			var result = (dv.Count == 0) ? null : new OrderModel(dv[0]);
			return result;
		}
		#endregion

		#region ~GetUpdLock 更新ロック取得
		/// <summary>
		/// 更新ロック取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文ID</returns>
		internal string GetUpdLock(string orderId)
		{
			var dv = Get(XML_KEY_NAME, "GetUpdLock", new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId }
			});
			return (string)dv[0][0];
		}
		#endregion

		#region ~GetAllUpdateLock 更新ロック取得
		/// <summary>
		/// 更新ロック取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文ID</returns>
		internal OrderModel GetAllUpdateLock(string orderId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetAllUpdateLock",
				new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, orderId }
				});
			var result = (dv.Count > 0) ? new OrderModel(dv[0]) : null;
			return result;
		}
		#endregion

		#region +GetCombinedOrders 注文同梱情報取得
		/// <summary>
		/// 注文同梱情報取得
		/// </summary>
		/// <param name="orderIds">orderIds Id</param>
		/// <returns>OrderModel</returns>
		public OrderModel[] GetCombinedOrders(string[] orderIds)
		{
			var replaceKeyValues = new KeyValuePair<string, string>(
				"@@ order_id @@",
				string.Join(",", orderIds.Select(orderId => string.Format("'{0}'", orderId.Replace("'", "''")))));

			var dv = Get(XML_KEY_NAME, "GetCombinedOrders", null, null, replaceKeyValues);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
		}
		#endregion

		#region ~InsertOrder 注文情報登録
		/// <summary>
		/// 注文情報登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int InsertOrder(OrderModel model)
		{
			var result = Exec(XML_KEY_NAME, "InsertOrder", model.DataSource);
			return result;
		}
		#endregion

		#region ~InsertOwner 注文者情報登録
		/// <summary>
		/// 注文者情報登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertOwner(OrderOwnerModel model)
		{
			Exec(XML_KEY_NAME, "InsertOwner", model.DataSource);
		}
		#endregion

		#region ~InsertOrderForOrderReturnExchange 返品交換向け注文登録 HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 返品交換向け注文登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="order">受注</param>
		internal void InsertOrderForOrderReturnExchange(Hashtable order)
		{
			Exec(XML_KEY_NAME, "InsertOrderForOrderReturnExchange", order);
		}
		#endregion

		#region ~InsertOrderOwnerForOrderReturnExchange 返品交換向け注文者情報登録 HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 返品交換向け注文者情報登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="order">受注</param>
		internal void InsertOrderOwnerForOrderReturnExchange(Hashtable order)
		{
			Exec(XML_KEY_NAME, "InsertOrderOwnerForOrderReturnExchange", order);
		}
		#endregion

		#region ~InsertOrderSetPromotion 注文セットプロモーション情報登録 HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 注文セットプロモーション情報登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderSetPromotion">受注セットプロモーション</param>
		internal void InsertOrderSetPromotion(Hashtable orderSetPromotion)
		{
			Exec(XML_KEY_NAME, "InsertOrderSetPromotion", orderSetPromotion);
		}
		#endregion

		#region ~InsertOrderPriceInfoByTaxRate 税率毎価格情報登録 HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 税率毎価格情報登録 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderPriceByTaxRate">注文価格レート</param>
		internal void InsertOrderPriceInfoByTaxRate(Hashtable orderPriceByTaxRate)
		{
			Exec(XML_KEY_NAME, "InsertOrderPriceInfoByTaxRate", orderPriceByTaxRate);
		}
		#endregion

		#region ~InsertOrderShippingForOrderReturnExchange 注文配送先情報追加処理 HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 注文配送先情報追加処理 HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderShipping">注文配送先情報</param>
		internal void InsertOrderShippingForOrderReturnExchange(Hashtable orderShipping)
		{
			Exec(XML_KEY_NAME, "InsertOrderShippingForOrderReturnExchange", orderShipping);
		}
		#endregion

		#region ~InsertOrderItemForOrderReturnExchange 注文商品情報追加処理(返品用：非入力項目は元注文商品から参照) HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 注文商品情報追加処理(返品用：非入力項目は元注文商品から参照) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="orderItem">注文アイテム</param>
		internal void InsertOrderItemForOrderReturnExchange(Hashtable orderItem)
		{
			Exec(XML_KEY_NAME, "InsertOrderItemForOrderReturnExchange", orderItem);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(OrderModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +UpdateOrderDateChangedByOrderId 最終更新日時更新(order_id)
		/// <summary>
		/// 最終更新日時更新(order_id)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		public void UpdateOrderDateChangedByOrderId(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDEROWNER_ORDER_ID, orderId},
			};
			var result = Exec(XML_KEY_NAME, "UpdateOrderDateChangedByOrderId", ht);
		}
		#endregion

		#region +UpdateOrderDateChangedByUserId 最終更新日時更新(user_id)
		/// <summary>
		/// 最終更新日時更新(user_id)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		public void UpdateOrderDateChangedByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
			};
			Exec(XML_KEY_NAME, "UpdateOrderDateChangedByUserId", ht);
		}
		#endregion

		#region +UpdateOrderDateChangedForIntegration 最終更新日時更新(user_id, couponId, couponNoOld)
		/// <summary>
		/// 最終更新日時更新(user_id, couponId, couponNoOld)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNoOld">旧クーポンNo</param>
		public void UpdateOrderDateChangedForIntegration(string userId, string couponId, string couponNoOld)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USER_USER_ID, userId},
				{ Constants.FIELD_ORDERCOUPON_COUPON_ID, couponId },
				{ Constants.FIELD_ORDERCOUPON_COUPON_NO + "_old", couponNoOld },
			};
			Exec(XML_KEY_NAME, "UpdateOrderDateChangedForIntegration", ht);
		}
		#endregion

		#region +UpdateShippedChangedKbn 出荷後変更区分更新(元注文情報) HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 出荷後変更区分更新(元注文情報) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="order">受注</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateShippedChangedKbn(Hashtable order)
		{
			var result = Exec(XML_KEY_NAME, "UpdateShippedChangedKbn", order);
			return result;
		}
		#endregion

		#region +UpdateOrderReturnExchangeStatusReceipt 返品交換ステータス更新(返品交換受付) HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 返品交換ステータス更新(返品交換受付) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="order">受注</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateOrderReturnExchangeStatusReceipt(Hashtable order)
		{
			var result = Exec(XML_KEY_NAME, "UpdateOrderReturnExchangeStatusReceipt", order);
			return result;
		}
		#endregion

		#region +UpdateOrderRepaymentStatusConfrim 返金ステータス更新(未返金) HACK: 例外的にHashtableを渡す
		/// <summary>
		/// 返金ステータス更新(未返金) HACK: 例外的にHashtableを渡す
		/// </summary>
		/// <param name="order">受注</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateOrderRepaymentStatusConfrim(Hashtable order)
		{
			var result = Exec(XML_KEY_NAME, "UpdateOrderRepaymentStatusConfrim", order);
			return result;
		}
		#endregion

		#region +UpdatePaygentOrder
		/// <summary>
		/// ペイジェント受注情報更新
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <param name="card3DSecureTranId">3DSecureTranId</param>
		/// <param name="cardTranId">PaymentId</param>
		/// <returns>影響件数</returns>
		public int UpdatePaygentOrder(
			string orderId,
			string card3DSecureTranId,
			string cardTranId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId },
				{ Constants.FIELD_ORDER_CARD_3DSECURE_TRAN_ID, card3DSecureTranId},
				{ Constants.FIELD_ORDER_CARD_TRAN_ID, cardTranId },
			};
			var result = Exec(XML_KEY_NAME, "UpdatePaygentOrder", ht);
			return result;
		}
		#endregion

		#region +UpdateOrderPaymentStatusComplete
		/// <summary>
		/// 入金ステータスを入金済みに更新
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <param name="orderPaymentDate">入金確認日時</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>影響件数</returns>
		public int UpdateOrderPaymentStatusComplete(
			string orderId,
			DateTime orderPaymentDate,
			string lastChanged)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId },
				{ Constants.FIELD_ORDER_ORDER_PAYMENT_DATE, orderPaymentDate },
				{ Constants.FIELD_ORDER_LAST_CHANGED, lastChanged },
			};
			var result = Exec(XML_KEY_NAME, "UpdateOrderPaymentStatusComplete", ht);
			return result;
		}
		#endregion

		#region +GetOrdersByUserId ユーザーIDから注文情報取得
		/// <summary>
		/// ユーザーIDから注文情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデルリスト</returns>
		public OrderModel[] GetOrdersByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrdersByUserId", ht);
			return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
		}
		#endregion

		#region +GetUncancelledOrders キャンセル済み注文以外を取得
		/// <summary>
		/// キャンセル済み注文以外を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>注文ID、注文ステータス、決済種別IDのリスト</returns>
		internal Tuple<string, string, string>[] GetUncancelledOrders(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetUncancelledOrders", ht);
			var orderIdAndPaymentIds = dv.Cast<DataRowView>()
					.Select(drv =>
						new Tuple<string, string, string>(
							(string)drv[Constants.FIELD_ORDER_ORDER_ID],
							(string)drv[Constants.FIELD_ORDER_ORDER_STATUS],
							(string)drv[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN])).ToArray();
			return orderIdAndPaymentIds;
		}
		#endregion

		#region +ユーザーIDから注文情報取得（全ての注文情報を含む）
		/// <summary>
		/// 注文IDから注文情報取得（全ての注文情報を含む）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="isGetOrderItem">注文商品情報を取得するか</param>
		/// <returns>モデル</returns>
		public OrderModel GetOrderInfoByOrderId(string orderId, bool isGetOrderItem = false)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderInfoByOrderId", ht);
			var order = new OrderHelper().GetOrder(dv, isGetOrderItem);
			return order;
		}
		#endregion

		#region +GetOrderInfosByUserId ユーザーIDから注文情報取得（全ての注文情報を含む）
		/// <summary>
		/// ユーザーIDから注文情報取得（全ての注文情報を含む）
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデルリスト</returns>
		public OrderModel[] GetOrderInfosByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderInfosByUserId", ht);
			var orders = new OrderHelper().GetOrders(dv);
			return orders;
		}
		#endregion

		#region +GetOrderHistory ユーザーIDから注文履歴情報取得
		/// <summary>
		/// ユーザーIDから注文履歴情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>注文モデルリスト</returns>
		public OrderModel[] GetOrderHistoryList(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderHistoryList", ht);
			return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
		}
		#endregion

		#region ~GetOrderHistoryDetailInDataView 注文詳細の取得 HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文詳細の取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="includeTempPaidyPaygentOrder">Include temp paidy paygent order</param>
		/// <returns>モデル</returns>
		internal DataView GetOrderHistoryDetailInDataView(
			string orderId,
			string userId,
			string memberRankId,
			bool includeTempPaidyPaygentOrder)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
				{Constants.FIELD_ORDER_USER_ID, userId},
				{Constants.FIELD_ORDER_MEMBER_RANK_ID, memberRankId},
			};

			if (includeTempPaidyPaygentOrder)
			{
				ht["include_temp_paidy_paygent_order"] = Constants.FLG_ON;
			}
			var dv = Get(XML_KEY_NAME, "GetOrderHistoryDetail", ht);
			return dv;
		}
		#endregion

		#region ~GetOrderHistoryListByOrdersInDataView 注文履歴一覧（注文単位）を取得する HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文履歴一覧（注文単位）を取得する HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <returns>モデル</returns>
		internal DataView GetOrderHistoryListByOrdersInDataView(
			string userId,
			int startRowNum,
			int endRowNum)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_USER_ID, userId},
				{"bgn_row_num", startRowNum},
				{"end_row_num", endRowNum},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderHistoryListByOrders", ht);
			return dv;
		}
		#endregion

		#region ~GetOrderHistoryListByProductsInDataView 注文履歴一覧（商品単位）を取得する HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文履歴一覧（商品単位）を取得する HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <returns></returns>
		internal DataView GetOrderHistoryListByProductsInDataView(
			string userId,
			string memberRankId,
			int startRowNum,
			int endRowNum)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_USER_ID, userId},
				{Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRankId},
				{"bgn_row_num", startRowNum},
				{"end_row_num", endRowNum},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderHistoryListByProducts", ht);
			return dv;
		}
		#endregion

		#region +GetOrderInDataView 注文を取得 HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文を取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <returns>注文DataView</returns>
		public DataView GetOrderInDataView(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrder", ht);
			return dv;
		}
		#endregion

		#region +GetOrderShippingInDataView 配送先情報取得 HACK: 例外的にDataViewを返す
		/// <summary>
		/// 配送先情報取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <returns>注文DataView</returns>
		public DataView GetOrderShippingInDataView(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderShipping", ht);
			return dv;
		}
		#endregion

		#region +GetOrderSetPromotionInDataView 注文セットプロモーション情報取得 HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文セットプロモーション情報取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <returns>注文DataView</returns>
		public DataView GetOrderSetPromotionInDataView(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderSetPromotion", ht);
			return dv;
		}
		#endregion

		#region +GetOrderIdForOrderReturnExchangeInDataView 最大注文ID(枝番付き)取得 HACK: 例外的にDataViewを返す
		/// <summary>
		/// 最大注文ID(枝番付き)取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <returns>注文DataView</returns>
		public DataView GetOrderIdForOrderReturnExchangeInDataView(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID_ORG, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderIdForOrderReturnExchange", ht);
			return dv;
		}
		#endregion

		#region +GetOrderForOrderReturnExchangeInDataView 注文情報取得(※子注文の返品商品も抽出する) HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文情報取得(※子注文の返品商品も抽出する) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <returns>注文DataView</returns>
		public DataView GetOrderForOrderReturnExchangeInDataView(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID_ORG, orderId },
				{ Constants.FIELD_ORDER_ORDER_ID_ORG + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(orderId) }
			};
			var dv = Get(XML_KEY_NAME, "GetOrderForOrderReturnExchange", ht);
			return dv;
		}
		#endregion

		#region +GetExchangedOrderInDataView 注文情報取得(※子注文の返品商品も抽出する) HACK: 例外的にDataViewを返す
		/// <summary>
		/// 交換済み注文情報取得(※親注文IDが等しい注文の返品商品も抽出する) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="exchangedOrderId">交換済みの注文ID</param>
		/// <param name="orderIdOrg">元注文</param>
		/// <returns>注文DataView</returns>
		public DataView GetExchangedOrderInDataView(string exchangedOrderId, string orderIdOrg)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, exchangedOrderId },
				{ Constants.FIELD_ORDER_ORDER_ID_ORG, orderIdOrg },
				{ Constants.FIELD_ORDER_ORDER_ID_ORG + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(orderIdOrg) }
			};
			var dv = Get(XML_KEY_NAME, "GetExchangedOrder", ht);
			return dv;
		}
		#endregion

		#region +GetExchangedOrderIds 交換注文の注文群を取得
		/// <summary>
		/// 交換注文の注文群を取得
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <returns>注文モデル</returns>
		public OrderModel[] GetExchangedOrderIds(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID_ORG, orderId },
				{ Constants.FIELD_ORDER_ORDER_ID_ORG + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(orderId) }
			};
			var dv = Get(XML_KEY_NAME, "GetExchangedOrderIds", ht);
			return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
		}
		#endregion

		#region +GetForOrderReturnInDataView 返品用の受注取得 HACK: 例外的にDataViewを返す

		/// <summary>
		/// 返品用の受注取得 HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="orderId">受注ID</param>
		/// <param name="returnExchangeKbn">返品交換区分</param>
		/// <returns>注文DataView</returns>
		public DataView GetForOrderReturnInDataView(string orderId, string returnExchangeKbn)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId },
				{ Constants.FIELD_ORDER_RETURN_EXCHANGE_KBN, returnExchangeKbn },
			};
			var dv = Get(XML_KEY_NAME, "GetForOrderReturn", ht);
			return dv;
		}
		#endregion

		#region +GetOrderWorkflowListInDataView 注文一覧を取得(ワークフロー) HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文一覧を取得(ワークフロー) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="htSearch"></param>
		/// <param name="iPageNumber"></param>
		/// <returns>注文リストDataView</returns>
		public DataView GetOrderWorkflowListInDataView(Hashtable htSearch, int iPageNumber)
		{
			htSearch.Add("bgn_row_num", (int)htSearch[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT] * (iPageNumber - 1) + 1);
			htSearch.Add("end_row_num", (int)htSearch[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT] * iPageNumber);

			var replace = new[]
			{
				new KeyValuePair<string, string>("@@ where @@", (string) htSearch["@@ where @@"]),
				new KeyValuePair<string, string>(
					"@@ orderby @@",
					GetOrderSearchOrderByStringForOrderListAndWorkflow((string) htSearch["sort_kbn"])),
				new KeyValuePair<string, string>("@@ order_extend_field_name @@",
					((string.IsNullOrEmpty(StringUtility.ToEmpty(htSearch[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME])) == false)
						? string.Format("{0}.{1}", Constants.TABLE_ORDER, StringUtility.ToEmpty(htSearch[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]))
						: string.Empty)),
			};
			htSearch.Remove("@@ where @@");	// 条件文はパラメータではないので削除（パフォーマンスのため）
			var dv = Get(XML_KEY_NAME, "GetOrderWorkflowList", htSearch, replaces: replace);
			return dv;
		}
		#endregion

		#region +GetOrderWorkflowListCountInDataView 注文件数を取得(ワークフロー) HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文件数を取得(ワークフロー) HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="htSearch"></param>
		/// <returns>注文リストDataView</returns>
		public DataView GetOrderWorkflowListCountInDataView(Hashtable htSearch, int iPageNumber)
		{
			htSearch.Add("bgn_row_num", (int)htSearch[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT] * (iPageNumber - 1) + 1);
			htSearch.Add("end_row_num", (int)htSearch[Constants.FIELD_ORDERWORKFLOWSETTING_DISPLAY_COUNT] * iPageNumber);

			var replace = new[]
			{
				new KeyValuePair<string, string>("@@ where @@", (string)htSearch["@@ where @@"]),
				new KeyValuePair<string, string>(
					"@@ orderby @@",
					GetOrderSearchOrderByStringForOrderListAndWorkflow((string)htSearch["sort_kbn"])),
				new KeyValuePair<string, string>("@@ order_extend_field_name @@",
					((string.IsNullOrEmpty(StringUtility.ToEmpty(htSearch[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME])) == false)
						? string.Format("{0}.{1}", Constants.TABLE_ORDER, StringUtility.ToEmpty(htSearch[Constants.SEARCH_FIELD_ORDER_EXTEND_NAME]))
						: string.Empty)),
			};
			htSearch.Remove("@@ where @@"); // 条件文はパラメータではないので削除（パフォーマンスのため）
			var dv = Get(XML_KEY_NAME, "GetOrderWorkflowListCount", htSearch, replaces: replace);
			return dv;
		}
		#endregion

		#region +GetOrderSearchOrderByStringForOrderListAndWorkflow 検索向けORDER BY文字列取得
		/// <summary>
		/// 検索向けORDER BY文字列取得
		/// </summary>
		/// <param name="sortKbn">ソート区分</param>
		/// <returns>ORDER BY文字列</returns>
		private static string GetOrderSearchOrderByStringForOrderListAndWorkflow(string sortKbn)
		{
			const string ORDER_BY = "ORDER BY";
			switch (sortKbn)
			{
				case "0":
					return ORDER_BY + " w2_Order.order_id ASC";

				case "1":
					return ORDER_BY + " w2_Order.order_id DESC";

				case "2":
					return ORDER_BY + " w2_Order.order_date ASC, w2_Order.order_id ASC";

				case "3":
					return ORDER_BY + " w2_Order.order_date DESC, w2_Order.order_id ASC";

				case "4":
					return ORDER_BY + " w2_Order.date_created ASC, w2_Order.order_id ASC";

				case "5":
					return ORDER_BY + " w2_Order.date_created DESC, w2_Order.order_id ASC";

				case "6":
					return ORDER_BY + " w2_Order.date_changed ASC, w2_Order.order_id ASC";

				case "7":
					return ORDER_BY + " w2_Order.date_changed DESC, w2_Order.order_id ASC";

				case "100":
					return ORDER_BY + " w2_Order.order_return_exchange_receipt_date ASC, w2_Order.order_id ASC";

				case "101":
					return ORDER_BY + " w2_Order.order_return_exchange_receipt_date DESC, w2_Order.order_id ASC";

				default:
					return ORDER_BY + " w2_Order.order_id ASC";
			}
		}
		#endregion

		#region +GetOwner 注文者取得
		/// <summary>
		/// 注文者取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		public OrderOwnerModel GetOwner(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDEROWNER_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetOwner", ht);
			if (dv.Count == 0) return null;
			return new OrderOwnerModel(dv[0]);
		}
		#endregion

		#region ~UpdateOwner 注文者更新
		/// <summary>
		/// 注文者更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateOwner(OrderOwnerModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateOwner", model.DataSource);
			return result;
		}
		#endregion

		#region ~UpdateForOrderWorkflow 更新(受注ワークフロー用) HACK: Modifyでできるようにしたい
		/// <summary>
		/// 更新(受注ワークフロー用)HACK: Modifyでできるようにしたい
		/// </summary>
		/// <param name="statement">ステートメント</param>
		/// /// <param name="order">オーダー</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateForOrderWorkflow(string statement, Hashtable order)
		{
			var result = Exec(XML_KEY_NAME, statement, order);
			return result;
		}
		#endregion

		#region ~GetShipping 配送先取得
		/// <summary>
		/// 配送先取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderShippingNo">配送先枝番</param>
		/// <returns>モデル</returns>
		public OrderShippingModel GetShipping(string orderId, int orderShippingNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERSHIPPING_ORDER_ID, orderId},
				{Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO, orderShippingNo},
			};
			var dv = Get(XML_KEY_NAME, "GetShipping", ht);
			if (dv.Count == 0) return null;
			return new OrderShippingModel(dv[0]);
		}
		#endregion

		#region +GetShippingAll 配送先取得（全て）
		/// <summary>
		/// 配送先取得（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		public OrderShippingModel[] GetShippingAll(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERSHIPPING_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetShippingAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new OrderShippingModel(drv)).OrderBy(s => s.OrderShippingNo).ToArray();
		}
		#endregion

		#region ~InsertShipping 配送先登録
		/// <summary>
		/// 配送先登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		internal int InsertShipping(OrderShippingModel model)
		{
			return base.Exec(XML_KEY_NAME, "InsertShipping", model.DataSource);
		}
		#endregion

		#region ~UpdateShipping 配送先更新
		/// <summary>
		/// 配送先更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateShipping(OrderShippingModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateShipping", model.DataSource);
			return result;
		}
		#endregion

		#region ~DeleteShippingAll 配送先削除（全て）
		/// <summary>
		/// 配送先削除（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>削除件数</returns>
		internal int DeleteShippingAll(string orderId)
		{
			return base.Exec(
				XML_KEY_NAME,
				"DeleteShippingAll",
				new Hashtable() { { Constants.FIELD_ORDERSHIPPING_ORDER_ID, orderId } });
		}
		#endregion

		#region ~InsertOrderPriceByTaxRate 税率毎価格情報登録
		/// <summary>
		/// 税率毎価格情報登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		internal int InsertOrderPriceByTaxRate(OrderPriceByTaxRateModel model)
		{
			return base.Exec(XML_KEY_NAME, "InsertOrderPriceByTaxRate", model.DataSource);
		}
		#endregion

		#region ~DeleteShippingAll 税率毎価格情報削除（全て）
		/// <summary>
		/// 税率毎価格情報削除（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>削除件数</returns>
		internal int DeleteOrderPriceByTaxRateAll(string orderId)
		{
			return base.Exec(
				XML_KEY_NAME,
				"DeleteOrderPriceByTaxRateAll",
				new Hashtable() { { Constants.FIELD_ORDERPRICEBYTAXRATE_ORDER_ID, orderId } });
		}
		#endregion

		#region +GetItemAll 商品取得（全て）
		/// <summary>
		/// 商品取得（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		public OrderItemModel[] GetItemAll(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERITEM_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetItemAll", ht);
			return dv.Cast<DataRowView>()
				.Select(drv => new OrderItemModel(drv))
				.OrderBy(i => i.OrderShippingNo)
				.ThenBy(i => i.OrderItemNo)
				.ToArray();
		}
		#endregion

		#region ~InsertItem 注文商品登録
		/// <summary>
		/// 注文商品登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		internal int InsertItem(OrderItemModel model)
		{
			return base.Exec(XML_KEY_NAME, "InsertItem", model.DataSource);
		}
		#endregion

		#region ~DeleteItemAll 注文商品削除（全て）
		/// <summary>
		/// 注文商品削除（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>削除件数</returns>
		internal int DeleteItemAll(string orderId)
		{
			return base.Exec(
				XML_KEY_NAME,
				"DeleteItemAll",
				new Hashtable() { { Constants.FIELD_ORDERITEM_ORDER_ID, orderId } });
		}
		#endregion

		#region +GetItemByOrderIds 注文ID一覧から注文商品取得
		/// <summary>
		/// 注文ID一覧から注文商品取得
		/// </summary>
		/// <param name="orderIds">注文ID一覧</param>
		/// <returns>注文商品モデルリスト</returns>
		public OrderItemModel[] GetItemByOrderIds(string[] orderIds)
		{
			string queryOrderIds = string.Format(@"'{0}'", string.Join("','", orderIds));
			var ht = new KeyValuePair<string, string>[]
			{
				new KeyValuePair<string, string>("@" + Constants.FIELD_ORDERITEM_ORDER_ID, queryOrderIds)
			};
			var dv = Get(XML_KEY_NAME, "GetItemByOrderIds", replaces: ht);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new OrderItemModel(drv)).OrderBy(i => i.OrderShippingNo).ThenBy(i => i.OrderItemNo).ToArray();
		}
		#endregion

		#region ~GetCoupon クーポン取得
		/// <summary>
		/// クーポン取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>登録件数</returns>
		internal OrderCouponDetailInfo GetCoupon(string orderId)
		{
			var ht = new Hashtable { { Constants.FIELD_ORDER_ORDER_ID, orderId } };
			var dv = base.Get(XML_KEY_NAME, "GetCoupon", ht);
			return dv.Cast<DataRowView>().Select(drv => new OrderCouponDetailInfo(drv)).FirstOrDefault();
		}
		#endregion

		#region +GetAllCoupons クーポン取得（全て）
		/// <summary>
		/// クーポン取得（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文クーポンモデル一覧</returns>
		internal OrderCouponModel[] GetAllCoupons(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERCOUPON_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetAllCoupons", ht);
			return dv.Cast<DataRowView>().Select(drv => new OrderCouponModel(drv)).OrderBy(c => c.OrderCouponNo).ToArray();
		}
		#endregion

		#region ~InsertCoupon クーポン登録
		/// <summary>
		/// クーポン登録
		/// </summary>
		/// <param name="model">注文クーポンモデル</param>
		/// <returns>登録件数</returns>
		internal int InsertCoupon(OrderCouponModel model)
		{
			return base.Exec(XML_KEY_NAME, "InsertCoupon", model.DataSource);
		}
		#endregion

		#region ~UpdateCoupon クーポン更新
		/// <summary>
		/// クーポン更新
		/// </summary>
		/// <param name="model">注文クーポンモデル</param>
		/// <returns>更新件数</returns>
		internal int UpdateCoupon(OrderCouponModel model)
		{
			return base.Exec(XML_KEY_NAME, "UpdateCoupon", model.DataSource);
		}
		#endregion

		#region ~DeleteOrderCoupon 注文クーポン情報を削除
		/// <summary>
		/// 注文クーポン情報を削除
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>削除件数</returns>
		internal int DeleteOrderCoupon(string orderId)
		{
			return base.Exec(
				XML_KEY_NAME,
				"DeleteOrderCoupon",
				new Hashtable() { { Constants.FIELD_ORDERCOUPON_ORDER_ID, orderId } });
		}
		#endregion

		#region ~DeleteCouponByCouponNo クーポン情報削除 （注文ID＆注文クーポン枝番指定）
		/// <summary>
		/// クーポン情報削除 （注文ID＆注文クーポン枝番指定）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderCouponNo">注文クーポン枝番</param>
		/// <returns>削除件数</returns>
		internal int DeleteCouponByCouponNo(string orderId, int orderCouponNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERCOUPON_ORDER_ID, orderId},
				{Constants.FIELD_ORDERCOUPON_ORDER_COUPON_NO, orderCouponNo}
			};
			return base.Exec(XML_KEY_NAME, "DeleteCouponByCouponNo", ht);
		}
		#endregion

		#region +GetSetPromotionAll セットプロモーション取得（全て）
		/// <summary>
		/// セットプロモーション取得（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		public OrderSetPromotionModel[] GetSetPromotionAll(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERCOUPON_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetSetPromotionAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new OrderSetPromotionModel(drv)).OrderBy(s => s.OrderSetpromotionNo).ToArray();
		}
		#endregion

		#region +GetPriceInfoByTaxRateAll 税率毎価格情報取得（全て）
		/// <summary>
		/// 税率毎価格情報取得（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		public List<OrderPriceByTaxRateModel> GetPriceInfoByTaxRateAll(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_ORDER_ID, orderId },
			};
			var dv = Get(XML_KEY_NAME, "GetPriceInfoByTaxRateAll", ht);
			var result = dv.Cast<DataRowView>().Select(drv => new OrderPriceByTaxRateModel(drv)).OrderBy(s => s.KeyTaxRate).ToList();
			return result;
		}
		#endregion

		#region +Get Tax Rate Include Return Exchange
		/// <summary>
		/// 交換注文返品際の税率計算
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		public List<OrderPriceByTaxRateModel> GetTaxRateIncludeReturnExchange(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_ORDER_ID, orderId },
			};
			var dv = Get(XML_KEY_NAME, "GetTaxRateIncludeReturnExchange", ht);
			var result = dv.Cast<DataRowView>().Select(drv => new OrderPriceByTaxRateModel(drv)).OrderBy(order => order.KeyTaxRate).ToList();
			return result;
		}
		#endregion

		#region ~InsertSetPromotion 注文セットプロモーション登録
		/// <summary>
		/// 注文セットプロモーション登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>登録件数</returns>
		internal int InsertSetPromotion(OrderSetPromotionModel model)
		{
			return base.Exec(XML_KEY_NAME, "InsertSetPromotion", model.DataSource);
		}
		#endregion

		#region ~DeleteSetPromotionAll 注文セットプロモーション削除（全て）
		/// <summary>
		/// 注文セットプロモーション削除（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>削除件数</returns>
		internal int DeleteSetPromotionAll(string orderId)
		{
			return base.Exec(
				XML_KEY_NAME,
				"DeleteSetPromotionAll",
				new Hashtable() { { Constants.FIELD_ORDERITEM_ORDER_ID, orderId } });
		}
		#endregion

		#region +GetOrderPriceByTaxRateAll 税率毎価格情報取得（全て）
		/// <summary>
		/// 税率毎価格情報取得（全て）
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>モデル</returns>
		public OrderPriceByTaxRateModel[] GetOrderPriceByTaxRateAll(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERPRICEBYTAXRATE_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderPriceByTaxRateAll", ht);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new OrderPriceByTaxRateModel(drv)).ToArray();
		}
		#endregion

		#region ~AdjustAddPoint 追加ポイント調整
		/// <summary>
		/// 追加ポイント調整
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="adjustAddPoint">調整ポイント</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新件数</returns>
		internal int AdjustAddPoint(string orderId, decimal adjustAddPoint, string lastChanged)
		{
			return base.Exec(
				XML_KEY_NAME,
				"AdjustAddPoint",
				new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, orderId },
					{ Constants.FIELD_ORDER_ORDER_POINT_ADD, adjustAddPoint },
					{ Constants.FIELD_ORDER_LAST_CHANGED, lastChanged },
				});
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
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERITEM_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetForOrderMail", ht);

			return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
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
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERITEM_ORDER_ID, orderId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderSerialKeyForOrderMail", ht);

			return dv.Cast<DataRowView>().Select(drv => new OrderSerialKeyForMailSend(drv)).ToArray();
		}
		#endregion

		#region +GetOrderByOrderIdAndCouponUseUser 注文IDとクーポン利用ユーザー(メールアドレスorユーザーID)から注文情報が取得できるか
		/// <summary>
		/// 注文IDとクーポン利用ユーザー(メールアドレスorユーザーID)から注文情報が取得できるか
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="couponUseUser">クーポン利用ユーザー(メールアドレスorユーザーID)</param>
		/// <param name="usedUserJudgeType">利用済みユーザー判定方法</param>
		/// <returns>注文情報</returns>
		internal OrderModel GetOrderByOrderIdAndCouponUseUser(string orderId, string couponUseUser, string usedUserJudgeType)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId },
				{ Constants.FIELD_COUPONUSEUSER_COUPON_USE_USER, couponUseUser },
				{ Constants.FLG_COUPONUSEUSER_USED_USER_JUDGE_TYPE, usedUserJudgeType }
			};
			var dv = Get(XML_KEY_NAME, "GetOrderByOrderIdAndCouponUseUser", ht);
			return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).FirstOrDefault();
		}
		#endregion

		#region +GetUpdateShipmentCompleteTargeReturnOrder ヤマト後払い出荷報告完了更新対象の返品/交換注文情報取得
		/// <summary>
		/// ヤマト後払い出荷報告完了更新対象の返品文情報取得
		/// </summary>
		/// <param name="paymentKbn">決済区分</param>
		/// <returns>モデル列</returns>
		public OrderModel[] GetUpdateShipmentCompleteTargeReturnOrder(string paymentKbn)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, paymentKbn},
			};
			var dv = Get(XML_KEY_NAME, "GetUpdateShipmentCompleteTargeReturnOrder", input);
			return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).OrderBy(o => o.OrderId).ToArray();
		}
		#endregion

		#region +GetAuthExpiredOrderIds
		/// <summary>
		/// 与信期限切れ注文取得
		/// </summary>
		/// <param name="cardExpireDate">カード与信日</param>
		/// <param name="cvsDefExpireDate">コンビニ後払い与信日</param>
		/// <param name="amazonPayExpireDate">AmazonPay与信日</param>
		/// <param name="paidyPayExpireDate">PaidyPay与信日</param>
		/// <param name="linePayExpireDate">LinePay与信日</param>
		/// <param name="npafterPayExpireDate">NPAfterPay与信日</param>
		/// <param name="rakutenPayExpireDate">Rakuten与信日</param>
		/// <param name="gmoPostExpireDate">Gmo掛け払い与信日</param>
		/// <returns>対象注文</returns>
		public string[] GetAuthExpiredOrderIds(
			DateTime cardExpireDate,
			DateTime cvsDefExpireDate,
			DateTime amazonPayExpireDate,
			DateTime paidyPayExpireDate,
			DateTime linePayExpireDate,
			DateTime npafterPayExpireDate,
			DateTime rakutenPayExpireDate,
			DateTime gmoPostExpireDate)
		{
			var ht = new Hashtable
			{
				{ "card_expire_date", cardExpireDate.AddDays(1) },
				{ "cvs_def_expire_date", cvsDefExpireDate.AddDays(1) },
				{ "amazon_pay_expire_date", amazonPayExpireDate.AddDays(1) },
				{ "paidy_pay_expire_date", paidyPayExpireDate.AddDays(1) },
				{ "line_pay_expire_date", linePayExpireDate.AddDays(1) },
				{ "npafter_pay_expire_date", npafterPayExpireDate.AddDays(1) },
				{ "rakuten_pay_expire_date", rakutenPayExpireDate.AddDays(1) },
				{ "gmo_post_expire_date", gmoPostExpireDate.AddDays(1) },
			};
			var dv = Get(XML_KEY_NAME, "GetAuthExpiredOrderIds", ht);
			return dv.Cast<DataRowView>().Select(drv => (string)drv[0]).ToArray();
		}
		#endregion

		#region +GetOrderForAuth
		/// <summary>
		/// 与信期限切れ注文取得
		/// </summary>
		/// <param name="extendStatusNumber">拡張ステータス番号</param>
		/// <returns>対象注文</returns>
		public string[] GetOrderForAuth(string extendStatusNumber)
		{
			var ht = new Hashtable
			{
				{"extend_status_number", extendStatusNumber},
			};
			var replaceValueString = ReplaceStatementForAuth(extendStatusNumber);

			var dv = Get(XML_KEY_NAME, "GetOrderIdsForAuth", ht, replaces: replaceValueString);
			return dv.Cast<DataRowView>().Select(drv => (string)drv[0]).ToArray();
		}
		#endregion
		#region +ReplaceStatementForAuth 注文取得（与信）取得用SQL条件分置換
		/// <summary>
		/// 注文取得（与信）取得用SQL条件分置換
		/// </summary>
		/// <param name="extendStatusNumber">拡張ステータス番号</param>
		/// <returns>置換対象文字列</returns>
		private KeyValuePair<string, string>[] ReplaceStatementForAuth(string extendStatusNumber)
		{
			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>(
					"@@ extend_status_condition @@",
					string.Format("extend_status{0} = '1'", extendStatusNumber)),
			};
			return replaceKeyValues;
		}
		#endregion

		#region ~GetOrderByExternalOrderId 外部連携受注IDから注文情報取得
		/// <summary>
		/// 外部連携受注IDから注文情報取得
		/// </summary>
		/// <param name="externalOrderId">外部連携受注ID</param>
		/// <returns>注文情報</returns>
		internal OrderModel GetOrderByExternalOrderId(string externalOrderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_EXTERNAL_ORDER_ID, externalOrderId }
			};
			var dv = Get(XML_KEY_NAME, "GetOrderByExternalOrderId", ht);
			return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).FirstOrDefault();
		}
		#endregion
		#region +GetCombinableOrder 注文同梱可能な注文取得
		/// <summary>
		/// 注文同梱可能な注文取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="possibleCombinePaymentIds">注文同梱可能な決済種別</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="isChildOrderFixedPurchase">注文同梱の子注文が定期注文か 定期注文の場合TRUE、通常注文の場合FALSE</param>
		/// <returns>注文同梱可能注文</returns>
		internal OrderModel[] GetCombinableOrder(
			string userId,
			string shippingId,
			IOrderCombineSettings orderCombineSettings,
			string[] possibleCombinePaymentIds,
			bool fixedPurchaseSeparate,
			bool isChildOrderFixedPurchase)
		{
			var replaceValueString = ReplaceStatementForGetCombinableOrder(
				orderCombineSettings,
				possibleCombinePaymentIds,
				fixedPurchaseSeparate,
				isChildOrderFixedPurchase);

			var orderDateDeadline = DateTime.Parse(DateTime.Now.AddDays(-orderCombineSettings.AllowOrderDayPassed).ToString("yyyy/MM/dd 00:00:00"));
			var shippingDateDeadline = orderCombineSettings.AllowShippingDayBefore < 0
				? (DateTime?)null
				: DateTime.Today.AddDays(orderCombineSettings.AllowShippingDayBefore);

			var ht = new Hashtable
			{
				{ Constants.FIELD_USER_USER_ID, userId },
				{ Constants.FIELD_ORDER_SHIPPING_ID, shippingId },
				{ "order_date_deadline", orderDateDeadline },
				{ "shipping_date_deadline", shippingDateDeadline },
			};

			var dv = Get(XML_KEY_NAME, "GetCombinableOrder", ht, replaces: replaceValueString);
			var models = dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();

			return models;
		}
		#endregion

		#region +GetCombineOrderCount 注文同梱可能な注文数取得
		/// <summary>
		/// 注文同梱可能な注文数取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="possibleCombinePaymentIds">注文同梱可能な決済種別</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="isChildOrderFixedPurchase">注文同梱の子注文が定期注文か 定期注文の場合TRUE、通常注文の場合FALSE</param>
		/// <returns>同梱可能な注文数</returns>
		internal int GetCombineOrderCount(
			string userId,
			string shippingId,
			IOrderCombineSettings orderCombineSettings,
			string[] possibleCombinePaymentIds,
			bool fixedPurchaseSeparate,
			bool isChildOrderFixedPurchase)
		{
			var replaceValueString = ReplaceStatementForGetCombinableOrder(
				orderCombineSettings,
				possibleCombinePaymentIds,
				fixedPurchaseSeparate,
				isChildOrderFixedPurchase);

			var orderDateDeadline = DateTime.Parse(DateTime.Now.AddDays(-orderCombineSettings.AllowOrderDayPassed).ToString("yyyy/MM/dd 00:00:00"));
			var shippingDateDeadline = orderCombineSettings.AllowShippingDayBefore < 0
				? (DateTime?)null
				: DateTime.Today.AddDays(orderCombineSettings.AllowShippingDayBefore);

			var ht = new Hashtable
			{
				{ Constants.FIELD_USER_USER_ID, userId },
				{ Constants.FIELD_ORDER_SHIPPING_ID, shippingId },
				{ "order_date_deadline", orderDateDeadline },
				{ "shipping_date_deadline", shippingDateDeadline },
			};

			var dv = Get(XML_KEY_NAME, "GetCombinableOrderCount", ht, replaces: replaceValueString);
			return (int)dv[0][0];
		}
		#endregion

		#region +GetCombinableParentOrderWithCondition 注文同梱可能な注文取得(管理画面用)
		/// <summary>
		/// 注文同梱可能な注文取得(管理画面用)
		/// </summary>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー名</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <returns>注文同梱可能注文</returns>
		internal OrderModel[] GetCombinableParentOrderWithCondition(
			IOrderCombineSettings orderCombineSettings,
			bool fixedPurchaseSeparate,
			string userId,
			string userName,
			int startRowNum,
			int endRowNum)
		{
			var replaceValueString = ReplaceStatementForGetCombinableParentOrder(
				orderCombineSettings,
				userId,
				userName,
				fixedPurchaseSeparate);

			var orderDateDeadline = DateTime.Parse(DateTime.Now.AddDays(-orderCombineSettings.AllowOrderDayPassed).ToString("yyyy/MM/dd 00:00:00"));
			var shippingDateDeadline = orderCombineSettings.AllowShippingDayBefore < 0
				? (DateTime?)null
				: DateTime.Today.AddDays(orderCombineSettings.AllowShippingDayBefore);

			var ht = new Hashtable {
				{"order_date_deadline", orderDateDeadline},
				{"shipping_date_deadline", shippingDateDeadline},
				{"start_row_num", startRowNum},
				{"end_row_num", endRowNum}
			};

			var dv = Get(XML_KEY_NAME, "GetCombinableParentOrderWithCondition", ht, replaces: replaceValueString);
			var models = dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();

			return models;
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
		internal int GetCombinableParentOrderWithConditionCount(
			IOrderCombineSettings orderCombineSettings,
			bool fixedPurchaseSeparate,
			string userId,
			string userName)
		{
			var replaceValueString = ReplaceStatementForGetCombinableParentOrder(
				orderCombineSettings,
				userId,
				userName,
				fixedPurchaseSeparate);

			var orderDateDeadline = DateTime.Parse(DateTime.Now.AddDays(-orderCombineSettings.AllowOrderDayPassed).ToString("yyyy/MM/dd 00:00:00"));
			var shippingDateDeadline = orderCombineSettings.AllowShippingDayBefore < 0
				? (DateTime?)null
				: DateTime.Today.AddDays(orderCombineSettings.AllowShippingDayBefore);

			var ht = new Hashtable
			{
				{ "order_date_deadline", orderDateDeadline },
				{ "shipping_date_deadline", shippingDateDeadline },
			};

			var dv = Get(XML_KEY_NAME, "GetCombinableParentOrderWithConditionCount", ht, replaces: replaceValueString);
			return (int)dv[0][0];
		}
		#endregion
		#region
		/// <summary>
		/// 注文情報取得(注文同梱でのキャンセル用)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>キャンセル用注文情報</returns>
		internal DataView GetOrderForOrderCancel(string orderId)
		{
			var ht = new Hashtable { { Constants.FIELD_ORDER_ORDER_ID, orderId } };
			var dv = Get(XML_KEY_NAME, "GetOrderForOrderCancel", ht);
			return dv;
		}
		#endregion

		#region +GetCombinableChildOrder 注文同梱可能な子注文取得
		/// <summary>
		/// 注文同梱可能な注文取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="isParentOrderFixedPurchase">注文同梱の親注文が定期注文か 定期注文の場合TRUE、通常注文の場合FALSE</param>
		/// <param name="parentOrderId">注文同梱の親注文ID</param>
		/// <param name="parentPaymentKbn">注文同梱の親注文の決済種別</param>
		/// <returns>注文同梱可能注文</returns>
		internal OrderModel[] GetCombinableChildOrder(
			string userId,
			string shippingId,
			IOrderCombineSettings orderCombineSettings,
			bool fixedPurchaseSeparate,
			bool isParentOrderFixedPurchase,
			string parentOrderId,
			string parentPaymentKbn)
		{
			var replaceValueString = ReplaceStatementForGetCombinableChildOrder(
				orderCombineSettings,
				fixedPurchaseSeparate,
				isParentOrderFixedPurchase);

			var orderDateDeadline = DateTime.Parse(DateTime.Now.AddDays(-orderCombineSettings.AllowOrderDayPassed).ToString("yyyy/MM/dd 00:00:00"));
			var shippingDateDeadline = orderCombineSettings.AllowShippingDayBefore < 0
				? (DateTime?)null
				: DateTime.Today.AddDays(orderCombineSettings.AllowShippingDayBefore);

			var ht = new Hashtable
			{
				{ Constants.FIELD_USER_USER_ID, userId },
				{ Constants.FIELD_ORDER_SHIPPING_ID, shippingId },
				{ "order_date_deadline", orderDateDeadline },
				{ "shipping_date_deadline", shippingDateDeadline },
				{ Constants.FIELD_ORDER_ORDER_ID, parentOrderId },
				{ Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, parentPaymentKbn },
			};

			var dv = Get(XML_KEY_NAME, "GetCombinableChildOrder", ht, replaces: replaceValueString);
			var models = dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();

			return models;
		}
		#endregion

		#region +ReplaceStatementForGetCombinableOrder 注文同梱可能な注文取得用SQL条件分置換
		/// <summary>
		/// 注文同梱可能な注文取得用SQL条件分置換
		/// </summary>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="possiblePaymentIds">注文同梱可能な決済種別</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="isChildOrderFixedPurchase">注文同梱の子注文が定期注文か 定期注文の場合TRUE、通常注文の場合FALSE</param>
		/// <returns>置換対象文字列</returns>
		private KeyValuePair<string, string>[] ReplaceStatementForGetCombinableOrder(
			IOrderCombineSettings orderCombineSettings,
			string[] possiblePaymentIds,
			bool fixedPurchaseSeparate,
			bool isChildOrderFixedPurchase)
		{
			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>(
					"@@ order_status @@",
					string.Join(",", orderCombineSettings.AllowOrderStatus.Select(status => string.Format("'{0}'", status.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ order_payment_kbn @@",
					string.Join(",", possiblePaymentIds.Select(kbn => string.Format("'{0}'", kbn.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ order_payment_status @@",
					string.Join(",", orderCombineSettings.AllowOrderPaymentStatus.Select(kbn => string.Format("'{0}'", kbn.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ deny_shipping_id @@",
					string.Join(",", orderCombineSettings.DenyShippingIds.Select(id => string.Format("'{0}'", id.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ deny_shipping_method @@",
					string.Join(",", orderCombineSettings.DenyShippingMethods.Select(method => string.Format("'{0}'", method.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ fixed_purchase_condition @@",
					fixedPurchaseSeparate
						? isChildOrderFixedPurchase
							? "AND w2_Order.fixed_purchase_id <> '' AND w2_Order.order_payment_kbn NOT IN ('K80', 'K81')"
							: "AND w2_Order.fixed_purchase_id = ''"
						: "AND NOT(w2_Order.fixed_purchase_id <> '' AND w2_Order.order_payment_kbn IN ('K80', 'K81'))"),
			};
			return replaceKeyValues;
		}
		#endregion

		#region +ReplaceStatementForGetCombinableParentOrder 注文同梱可能な親注文取得用SQL条件分置換
		/// <summary>
		/// 注文同梱可能な親注文取得用SQL条件分置換
		/// </summary>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userName">ユーザー名</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <returns>置換対象文字列</returns>
		private KeyValuePair<string, string>[] ReplaceStatementForGetCombinableParentOrder(
			IOrderCombineSettings orderCombineSettings,
			string userId,
			string userName,
			bool fixedPurchaseSeparate)
		{
			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>(
					"@@ order_status @@",
					string.Join(",", orderCombineSettings.AllowOrderStatus.Select(status => string.Format("'{0}'", status.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ order_status @@", string.Join(",", orderCombineSettings.AllowOrderStatus.Select(status => string.Format("'{0}'", status.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ order_payment_kbn @@",
					string.Join(",", orderCombineSettings.AllowPaymentKbn.Select(id => string.Format("'{0}'", id.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ order_payment_status @@",
					string.Join(",", orderCombineSettings.AllowOrderPaymentStatus.Select(status => string.Format("'{0}'", status.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ user_id_condition @@",
					string.IsNullOrEmpty(userId) ? "" : string.Format("AND w2_Order.user_id = '{0}'", userId.Replace("'", "''"))),
				new KeyValuePair<string, string>(
					"@@ user_name_condition @@",
					string.IsNullOrEmpty(userName)
						? "" : string.Format("AND w2_User.name like '%{0}%' ESCAPE '#'", StringUtility.SqlLikeStringSharpEscape(userName.Replace("'", "''")))),
				new KeyValuePair<string, string>(
					"@@ deny_shipping_id @@",string.Join(",", orderCombineSettings.DenyShippingIds.Select(id => string.Format("'{0}'", id.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ deny_shipping_method @@",
					string.Join(",", orderCombineSettings.DenyShippingMethods.Select(method => string.Format("'{0}'", method.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ fixed_purchase_condition @@",
					fixedPurchaseSeparate ? "AND ((w2_Order.fixed_purchase_id <> '' AND ParentOrder.fixed_purchase_id <> '') OR (w2_Order.fixed_purchase_id = '' AND ParentOrder.fixed_purchase_id = ''))" : "")
			};
			return replaceKeyValues;

		}
		#endregion

		#region +ReplaceStatementForGetCombinableChildOrder 注文同梱可能な子注文取得用SQL条件分置換
		/// <summary>
		/// 注文同梱可能な子注文取得用SQL条件分置換
		/// </summary>
		/// <param name="orderCombineSettings">注文同梱設定</param>
		/// <param name="fixedPurchaseSeparate">通常注文と定期注文を分割するか 分割する場合TRUE、分割しない場合FALSE</param>
		/// <param name="isParentOrderFixedPurchase">親注文が定期注文か 定期注文の場合TRUE、定期注文でない場合FALSE</param>
		/// <returns>置換対象文字列</returns>
		private KeyValuePair<string, string>[] ReplaceStatementForGetCombinableChildOrder(
			IOrderCombineSettings orderCombineSettings,
			bool fixedPurchaseSeparate,
			bool isParentOrderFixedPurchase)
		{
			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>(
					"@@ order_status @@",
					string.Join(",", orderCombineSettings.AllowOrderStatus.Select(status => string.Format("'{0}'", status.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ order_payment_status @@",
					string.Join(",", orderCombineSettings.AllowOrderPaymentStatus.Select(kbn => string.Format("'{0}'", kbn.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ deny_shipping_method @@",
					string.Join(",", orderCombineSettings.DenyShippingMethods.Select(method => string.Format("'{0}'", method.Replace("'", "''"))))),
				new KeyValuePair<string, string>(
					"@@ fixed_purchase_condition @@",
					fixedPurchaseSeparate
						? isParentOrderFixedPurchase
							? "AND  w2_Order.fixed_purchase_id <> ''" : "AND  w2_Order.fixed_purchase_id = ''"
						: ""),
			};
			return replaceKeyValues;
		}
		#endregion

		#region ~Update3DSecureInfo 3Dセキュア情報更新
		/// <summary>
		/// 3Dセキュア情報更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="tranId">3Dセキュア連携ID</param>
		/// <param name="authUrl">3Dセキュア認証URL</param>
		/// <param name="authKey">3Dセキュア認証キー</param>
		/// <param name="lastChanged">最終更新者</param>
		internal void Update3DSecureInfo(string orderId, string tranId, string authUrl, string authKey, string lastChanged)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId },
				{ Constants.FIELD_ORDER_CARD_3DSECURE_TRAN_ID, tranId },
				{ Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_URL, authUrl },
				{ Constants.FIELD_ORDER_CARD_3DSECURE_AUTH_KEY, authKey },
				{ Constants.FIELD_ORDER_LAST_CHANGED, lastChanged },
			};
			Exec(XML_KEY_NAME, "Update3dsecureInfo", ht);
		}
		#endregion

		#region ~GetForAmazonOrderFulfilment Amazon出荷通知対象取得
		/// <summary>
		/// Amazon出荷通知対象取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <returns>注文情報リスト</returns>
		internal DataView GetForAmazonOrderFulfilment(string mallId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_MALL_ID, mallId},
			};
			var dv = Get(XML_KEY_NAME, "GetForAmazonOrderFulfilment", ht);
			return dv;
		}
		#endregion

		#region ~GetImportedAmazonOrder 取込済みAmazon注文情報取得
		/// <summary>
		/// 取込済みAmazon注文情報取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>注文情報リスト</returns>
		internal OrderModel[] GetImportedAmazonOrder(string condition)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "GetImportedAmazonOrder"))
			{
				var ht = new Hashtable();
				statement.ReplaceStatement("@@ order_id_where @@", condition);
				var dv = statement.SelectSingleStatementWithOC(this.Accessor, ht);
				return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)
				{
					Items = new[] { new OrderItemModel(drv) },
					Owner = new OrderOwnerModel(drv),
					Shippings = new[] { new OrderShippingModel(drv) }
				}).ToArray();
			}
		}
		#endregion

		#region ~UpdateAmazonShipNoteProgress Amazon出荷通知中に更新
		/// <summary>
		/// Amazon出荷通知中に更新
		/// </summary>
		/// <param name="condition">更新条件</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateAmazonShipNoteProgress(string condition)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "UpdateAmazonShipNoteProgress"))
			{
				var ht = new Hashtable { };
				statement.ReplaceStatement("@@ order_id_where @@", condition);
				var result = statement.ExecStatementWithOC(this.Accessor, ht);
				return result;
			}
		}
		#endregion

		#region ~UpdateAmazonShipNoteComplete Amazon出荷通知済み更新
		/// <summary>
		/// Amazon出荷通知済み更新
		/// </summary>
		/// <param name="condition">更新条件</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateAmazonShipNoteComplete(string condition)
		{
			using (var statement = new SqlStatement(XML_KEY_NAME, "UpdateAmazonShipNoteComplete"))
			{
				var ht = new Hashtable { };
				statement.ReplaceStatement("@@ order_id_where @@", condition);
				var result = statement.ExecStatementWithOC(this.Accessor, ht);
				return result;
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
		internal string[] GetOrderIdForFixedProductOrderLimitCheck(
			OrderShippingModel orderShipping,
			OrderModel order,
			OrderOwnerModel orderOwner,
			string shopId,
			string productId,
			string[] notExistsOrderIds)
		{
			// 変換処理
			var matching = new OrderShippingMatching();
			var replaceCondition = matching.ReplaceConditionOrderShipping(orderShipping, order, orderOwner);
			var replaceKeyValues = new[]
			{
				// クエリ条件元のデータに対してSQLインジェクション対策のため、シングルクォーテーションをエスケープ
				new KeyValuePair<string, string>(
					"@@ condition @@",
					string.Format(
						replaceCondition,
						matching.QueryValues.Select(query => query.Replace("'", "''")).ToArray())),
				new KeyValuePair<string, string>("@@ productIds @@", (string.Format(productId.Replace("'", "'")))),
				new KeyValuePair<string, string>(
					"@@ notExistsOrderIds @@",
					string.Join(",", notExistsOrderIds.Select(id => string.Format("'{0}'", id)).ToArray()))
			};

			var dv = Get(
				XML_KEY_NAME,
				"GetOrderIdForFixedProductOrderLimitCheck",
				new Hashtable
				{
					{ Constants.FIELD_ORDERITEM_SHOP_ID, shopId },
					{ "table_name" , matching.Setting.ConditionTableName },
					{ "not_include_order", (notExistsOrderIds.Length > 0) ? notExistsOrderIds[0] : "" },
				},
				replaces: replaceKeyValues);
			var orderIds = dv.Cast<DataRowView>().Select(drv => (string)drv[Constants.FIELD_ORDERITEM_ORDER_ID]).ToArray();
			return orderIds;
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
		internal string[] GetOrderIdForProductOrderLimitCheck(
			OrderShippingModel orderShipping,
			OrderModel order,
			OrderOwnerModel orderOwner,
			string shopId,
			string productId,
			string[] notExistsOrderIds)
		{
			// 変換処理
			var matching = new OrderShippingMatching();
			var replaceCondition = matching.ReplaceConditionOrderShipping(orderShipping, order, orderOwner);

			var replaceKeyValues = new[]
			{
				// クエリ条件元のデータに対してSQLインジェクション対策のため、シングルクォーテーションをエスケープ
				new KeyValuePair<string, string>(
					"@@ condition @@",
					string.Format(
						replaceCondition,
						matching.QueryValues.Select(query => query.Replace("'", "''")).ToArray())),
				new KeyValuePair<string, string>(
					"@@ productIds @@",
					(string.Format(productId.Replace("'", "'")))),
				new KeyValuePair<string, string>(
					"@@ notExistsOrderIds @@",
					string.Join(",", notExistsOrderIds.Select(id => string.Format("'{0}'", id)).ToArray()))
			};

			var dv = Get(
				XML_KEY_NAME,
				"GetOrderIdForProductOrderLimitCheck",
				new Hashtable
				{
					{ Constants.FIELD_ORDERITEM_SHOP_ID, shopId },
				},
				replaces: replaceKeyValues);
			var orderIds = dv.Cast<DataRowView>().Select(drv => (string)drv[Constants.FIELD_ORDERITEM_ORDER_ID]).ToArray();
			return orderIds;
		}
		#endregion

		#region ~GetLohacoReserveOrder Lohaco予約注文一覧の取得
		/// <summary>
		/// Lohaco予約注文一覧の取得
		/// </summary>
		/// <param name="mallId">LohacoモールID</param>
		/// <param name="extendStatusNo">Lohaco予約注文の拡張ステタース番号</param>
		/// <returns>Lohaco予約注文一覧</returns>
		internal OrderModel[] GetLohacoReserveOrder(string mallId, int extendStatusNo)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_SHOP_ID, Constants.CONST_DEFAULT_SHOP_ID },
				{ Constants.FIELD_ORDER_MALL_ID, mallId },
				{ Constants.FIELD_ORDER_EXTEND_STATUS_VALUE, Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON },
			};
			var replace = new KeyValuePair<string, string>(
				"@@extend_status_name@@",
				string.Concat(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME, extendStatusNo));
			var dv = Get(XML_KEY_NAME, "GetLohacoReserveOrder", ht, replaces: replace);
			return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
		}
		#endregion

		#region +GetLastFixedPurchaseOrder 定期台帳の最終注文オブジェクトを取得
		/// <summary>
		/// 定期台帳の最終注文オブジェクトを取得
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <returns>モデル</returns>
		public OrderModel GetLastFixedPurchaseOrder(string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_FIXED_PURCHASE_ID, fixedPurchaseId},
			};
			var dv = Get(XML_KEY_NAME, "GetLastFixedPurchaseOrder", ht);
			if (dv.Count == 0) return null;
			return new OrderModel(dv[0]);
		}
		#endregion

		#region +GetReturnOrderItems 返品交換注文商品情報を取得
		/// <summary>
		/// 返品交換注文商品情報を取得する
		/// </summary>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <returns>返品交換注文商品情報</returns>
		public OrderItemModel[] GetReturnExchangeOrderItems(string orderIdOrg)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID_ORG, orderIdOrg }
			};

			var data = Get(XML_KEY_NAME, "GetReturnExchangeOrderItems", input);
			var result = data.Cast<DataRowView>()
				.Select(item => new OrderItemModel(item)).ToArray();

			return result;
		}
		#endregion

		#region +UpdateOrderLastAmount 注文情報の最終値(最終請求金額など)更新
		/// <summary>
		/// 注文情報の最終値(最終請求金額など)更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastBilledAmount">最終請求金額</param>
		/// <param name="lastPointUse">最終利用ポイント</param>
		/// <param name="lastPointUseYen">最終利用ポイント額</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateOrderLastAmount(string orderId, decimal lastBilledAmount, decimal lastPointUse, decimal lastPointUseYen, string lastChanged)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
				{Constants.FIELD_ORDER_LAST_CHANGED, lastChanged},
				{Constants.FIELD_ORDER_LAST_BILLED_AMOUNT, lastBilledAmount},
				{Constants.FIELD_ORDER_LAST_ORDER_POINT_USE, lastPointUse},
				{Constants.FIELD_ORDER_LAST_ORDER_POINT_USE_YEN, lastPointUseYen},
			};
			var result = Exec(XML_KEY_NAME, "UpdateOrderLastAmount", ht);
			return result;
		}
		#endregion

		#region +UpdateExternalPaymentCooperationLog 注文の外部決済連携ログを更新
		/// <summary>
		/// 外部決済連携ログの更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="externalPaymentLog">外部決済連携ログ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新した行数</returns>
		public int AppendExternalPaymentCooperationLog(string orderId, string externalPaymentLog, string lastChanged)
		{
			var ht = new Hashtable()
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId},
				{ Constants.FIELD_ORDER_EXTERNAL_PAYMENT_COOPERATION_LOG, externalPaymentLog },
				{ Constants.FIELD_ORDER_LAST_CHANGED, lastChanged }
			};
			var result = Exec(XML_KEY_NAME, "AppendExternalPaymentCooperationLog", ht);
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
		/// <returns>商品同梱情報</returns>
		public Dictionary<string, int> GetOrderedCountForProductBundle(
			string userId,
			List<string> bundleIds,
			IEnumerable<string> excludeOrderIds)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_USER_ID, userId },
			};

			var orderIds = (excludeOrderIds != null)
				? string.Join(",", excludeOrderIds.Select(id => string.Format("'{0}'", id)))
				: "''";
			var productBundleIds = string.Join(",", bundleIds.Select(id => $"'{StringUtility.EscapeSqlString(id)}'"));

			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>("@@ excludeOrderIds @@", orderIds),
				new KeyValuePair<string, string>("@@ productBundleIds @@", productBundleIds),
			};

			var dv = Get(XML_KEY_NAME, "GetOrderedCountForProductBundle", ht, replaces: replaceKeyValues);
			var result = new Dictionary<string, int>();
			dv.Cast<DataRowView>().ToList().ForEach(
				drv =>
				{
					result.Add((string)drv[Constants.FIELD_ORDERITEM_PRODUCT_BUNDLE_ID], (int)drv["ordered_count"]);
				});

			return result;
		}
		#endregion

		#region +UpdateOrderCountByUserId 累計購入回数更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="orderCountOld">データ移行しない分</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateOrderCountByUserId(string userId, int orderCountOld)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_USER_ID, userId },
				{ Constants.FIELD_USER_ORDER_COUNT_OLD, orderCountOld },
			};
			var result = Exec(XML_KEY_NAME, "UpdateOrderCountByUserId", ht);
			return result;
		}
		#endregion

		#region ~請求書同梱フラグ更新
		/// <summary>
		/// 請求書更新フラグ更新
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="invoiceBundleFlg">請求書同梱フラグ</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>成功したか</returns>
		public int UpdateInvoiceBundleFlg(string orderId, string invoiceBundleFlg, string lastChanged)
		{
			var updated = Exec(
				XML_KEY_NAME,
				"UpdateInvoiceBundleFlg",
				new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, orderId },
					{ Constants.FIELD_ORDER_INVOICE_BUNDLE_FLG, invoiceBundleFlg },
					{ Constants.FIELD_ORDER_LAST_CHANGED, lastChanged }
				});

			return updated;
		}
		#endregion

		#region +GetRelatedOrders 返品交換含む注文情報取得（全ての注文情報を含む）
		/// <summary>
		/// Get Related Orders DataView
		/// </summary>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <returns>モデル</returns>
		public DataView GetRelatedOrdersDataView(string orderIdOrg)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID_ORG, orderIdOrg},
			};

			return Get(XML_KEY_NAME, "GetRelatedOrders", ht);
		}
		#endregion

		#region GetOrdersReturnExchangeByOrderId
		/// <summary>
		/// Get Orders Return Exchange By Order Id
		/// </summary>
		/// <param name="orderId">OrderId</param>
		/// <returns>注文DataView</returns>
		public DataView GetOrdersReturnExchangeByOrderId(string orderId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
			};
			var data = Get(XML_KEY_NAME, "GetOrdersReturnExchangeByOrderId", input);

			return (data.Count == 0)
				? null
				: data;
		}
		#endregion

		#region +GetRelatedOrderItems
		/// <summary>
		/// Get Related Order Items
		/// </summary>
		/// <param name="orderIdOrg">Order Id Org</param>
		/// <returns>Related Order Items</returns>
		public OrderItemModel[] GetRelatedOrderItems(string orderIdOrg)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID_ORG, orderIdOrg }
			};

			var data = Get(XML_KEY_NAME, "GetRelatedOrderItems", input);
			var result = data.Cast<DataRowView>()
				.Select(item => new OrderItemModel(item)).ToArray();

			return result;
		}
		#endregion

		#region +GetLastOrder
		/// <summary>
		/// Get Last Order
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="excludedPaymentIds">Excluded Payment Ids</param>
		/// <returns>Last Order</returns>
		public OrderModel GetLastOrder(
			string userId,
			string[] excludedPaymentIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_USER_ID, userId },
			};
			var replaceKeyValues = ReplaceStatementForGetLastOrder(excludedPaymentIds);

			var result = Get(
				XML_KEY_NAME,
				"GetLastOrder",
				input,
				replaces: replaceKeyValues);
			if (result.Count == 0) return null;

			return new OrderModel(result[0]);
		}
		#endregion

		#region +ReplaceStatementForGetLastOrder
		/// <summary>
		/// Replace Statement For Get Last Order
		/// </summary>
		/// <param name="exceptedPaymentIds">Excepted Payment Ids</param>
		/// <returns>Replace Statement</returns>
		private KeyValuePair<string, string>[] ReplaceStatementForGetLastOrder(
			string[] exceptedPaymentIds)
		{
			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>("@@ payment_ids @@",
					string.Join(",", exceptedPaymentIds.Select(paymentId => string.Format("'{0}'", paymentId.Replace("'", "''"))))),
			};

			return replaceKeyValues;
		}
		#endregion

		#region +ReplaceStatementForGetOrders
		/// <summary>
		/// Replace Statement For Get Orders
		/// </summary>
		/// <param name="exceptedOrderStatus">Excepted Order Status</param>
		/// <param name="exceptedPaymentIds">Excepted Payment Ids</param>
		/// <returns>Replace Statement</returns>
		private KeyValuePair<string, string>[] ReplaceStatementForGetOrders(
			string[] exceptedOrderStatus,
			string[] exceptedPaymentIds)
		{
			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>("@@ order_status @@",
					string.Join(",", exceptedOrderStatus.Select(status => string.Format("'{0}'", status.Replace("'", "''"))))),
				new KeyValuePair<string, string>("@@ payment_ids @@",
					string.Join(",", exceptedPaymentIds.Select(paymentId => string.Format("'{0}'", paymentId.Replace("'", "''"))))),
			};

			return replaceKeyValues;
		}
		#endregion

		#region +GetOrdersWithoutReturnExchangeAndRejection
		/// <summary>
		/// Get Orders Without Return Exchange And Rejection
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="exceptedOrderStatus">Excepted Order Status</param>
		/// <param name="exceptedPaymentIds">Excepted Payment Ids</param>
		/// <returns>Orders Without Return Exchange And Rejection</returns>
		public OrderModel[] GetOrdersWithoutReturnExchangeAndRejection(
			string userId,
			string[] exceptedOrderStatus,
			string[] exceptedPaymentIds)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_USER_ID, userId },
			};
			var replaceKeyValues = ReplaceStatementForGetOrders(
				exceptedOrderStatus,
				exceptedPaymentIds);

			var data = Get(
				XML_KEY_NAME,
				"GetOrdersWithoutReturnExchangeAndRejection",
				input,
				replaces: replaceKeyValues);
			var result = data.Cast<DataRowView>()
				.Select(item => new OrderModel(item))
				.ToArray();

			return result;
		}
		#endregion

		#region +GetOrdersLastThreeMonthWithoutReturnExchange
		/// <summary>
		/// Get Orders Last Three Month Without Return Exchange
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <param name="exceptedOrderStatus">Excepted Order Status</param>
		/// <param name="exceptedPaymentIds">Excepted Payment Ids</param>
		/// <returns>Orders Last Three Month Without Return Exchange</returns>
		public OrderModel[] GetOrdersLastThreeMonthWithoutReturnExchange(
			string userId,
			string[] exceptedOrderStatus,
			string[] exceptedPaymentIds)
		{
			var startDate = DateTime.Now.AddMonths(-3);
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_USER_ID, userId },
				{ "start_date", startDate },
			};
			var replaceKeyValues = ReplaceStatementForGetOrders(
				exceptedOrderStatus,
				exceptedPaymentIds);

			var data = Get(
				XML_KEY_NAME,
				"GetOrdersLastThreeMonthWithoutReturnExchange",
				input,
				replaces: replaceKeyValues);
			var result = data.Cast<DataRowView>()
				.Select(item => new OrderModel(item))
				.ToArray();

			return result;
		}
		#endregion

		#region +GetFirstFixedPurchaseOrder
		/// <summary>
		/// Get First Fixed Purchase Order
		/// </summary>
		/// <param name="fixedPurchaseId">定期台帳ID</param>
		/// <returns>モデル</returns>
		public OrderModel GetFirstFixedPurchaseOrder(string fixedPurchaseId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_ORDER_FIXED_PURCHASE_ID, fixedPurchaseId},
			};
			var data = Get(XML_KEY_NAME, "GetFirstFixedPurchaseOrder", input);
			return (data.Count == 0)
				? null
				: new OrderModel(data[0]);
		}
		#endregion

		#region +GetOrderByCardTranId
		/// <summary>
		/// Get Order By Card Tran Id
		/// </summary>
		/// <param name="cardTranId">Card Tran Id</param>
		/// <returns>Order Model</returns>
		public OrderModel GetOrderByCardTranId(string cardTranId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_ORDER_CARD_TRAN_ID, cardTranId},
			};
			var data = Get(XML_KEY_NAME, "GetOrderByCardTranId", input);
			return (data.Count == 0)
				? null
				: new OrderModel(data[0]);
		}
		#endregion

		#region +GetAllOrderRelateFixedPurchase
		/// <summary>
		/// Get All Order Relate FixedPurchase
		/// </summary>
		/// <param name="fixedPurchaseId">fixedPurchaseId</param>
		/// <returns>Get All Order</returns>
		public OrderModel[] GetAllOrderRelateFixedPurchase(string fixedPurchaseId)
		{
			var hashTable = new Hashtable
			{
				{ Constants.FIELD_ORDER_FIXED_PURCHASE_ID, fixedPurchaseId}
			};
			var data = Get(XML_KEY_NAME, "GetAllOrderRelateFixedPurchase", hashTable);
			var result = data.Cast<DataRowView>()
				.Select(row => new OrderModel(row))
				.OrderBy(item => item.FixedPurchaseId)
				.ToArray();
			return result;
		}
		#endregion

		#region +AppendRelationMemo 連携メモを追記
		/// <summary>
		/// 連携メモを追記
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="relationMemo">関連メモ</param>
		/// <param name="lastChanged">最終更新者</param>
		public void AppendRelationMemo(string orderId, string relationMemo, string lastChanged)
		{
			Exec(
				XML_KEY_NAME,
				"AppendRelationMemo",
				new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_ID, orderId },
					{ Constants.FIELD_ORDER_RELATION_MEMO, relationMemo },
					{ Constants.FIELD_ORDER_LAST_CHANGED, lastChanged },
				});
		}
		# endregion

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
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_USER_ID, userId },
				{ Constants.FIELD_ORDER_DATE_CHANGED, updateAt },
				{ Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId },
				{ "offset", offset },
				{ "limit", limit },
			};
			var dataView = Get(XML_KEY_NAME, "GetOrdersForLine", input);
			var result = dataView.Cast<DataRowView>()
				.Select(row => new OrderModel(row))
				.ToArray();
			return result;
		}
		#endregion

		#region +GetOrdersByOrderStatus
		/// <summary>
		/// Get orders by order status
		/// </summary>
		/// <param name="orderStatus">Order status</param>
		/// <returns>Orders</returns>
		internal OrderModel[] GetOrdersByOrderStatus(string orderStatus)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetOrdersByOrderStatus",
				new Hashtable
				{
					{ Constants.FIELD_ORDER_ORDER_STATUS, orderStatus }
				});
			var orders = dv.Cast<DataRowView>()
				.Select(drv => new OrderModel(drv))
				.ToArray();
			return orders;
		}
		#endregion

		#region +GetMulitpleOrdersByOrderIdsAndPaymentKbn 決済種別と複数の受注IDから複数の受注を取得
		/// <summary>
		/// 決済種別と複数の受注IDから複数の受注を取得
		/// </summary>
		/// <param name="orderIds">複数の注文ID</param>
		/// <param name="paymentKbn">決済種別</param>
		/// <returns>モデル</returns>
		public OrderModel[] GetMulitpleOrdersByOrderIdsAndPaymentKbn(List<string> orderIds, string paymentKbn)
		{
			var orderIdWithQuotation = orderIds.Select(shipping => "'" + shipping + "'");
			var orderIdList = string.Join(",", orderIdWithQuotation).TrimEnd();

			var param = new KeyValuePair<string, string>("@@ orderIds @@", StringUtility.ToEmpty(orderIdList));
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_PAYMENT_KBN, paymentKbn },
			};
			var dv = Get(XML_KEY_NAME, "GetMulitpleOrdersByOrderIdsAndPaymentKbn", ht, replaces: param);
			var orders = dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
			return orders;
		}
		#endregion

		#region +GetExchangedOrdersInDataView 交換済み注文情報取得
		/// <summary>
		/// 交換済み注文情報取得
		/// 表示対象の交換済み注文よりも連番の大きい注文IDの返品商品を合わせて取得する
		/// (表示対象がXXX-001ならばXXX-002以降の返品商品を取得）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <returns>交換済み注文情報</returns>
		public DataView GetExchangedOrdersInDataView(
			string shopId,
			string orderId,
			string orderIdOrg)
		{
			var ht = new Hashtable {
				{ Constants.FIELD_ORDER_ORDER_ID, orderId},
				{ Constants.FIELD_ORDER_ORDER_ID_ORG, orderIdOrg},
				{ Constants.FIELD_ORDER_ORDER_ID_ORG + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(orderIdOrg)},
				{ Constants.FIELD_ORDER_SHOP_ID, shopId},
			};
			var dv = Get(XML_KEY_NAME, "GetExchangedOrders", ht);
			return dv;
		}
		#endregion

		#region +GetExchangedOrderIds 返品交換対象IDの取得
		/// <summary>
		/// 返品交換対象IDの取得(交換注文済みかつ、注文ステータスが出荷完了、配送完了)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="orderIdOrg">元注文ID</param>
		/// <returns>返品交換対象ID</returns>
		public string[] GetExchangedOrderIds(string shopId, string orderIdOrg)
		{
			var ht = new Hashtable {
				{ Constants.FIELD_ORDER_ORDER_ID_ORG, orderIdOrg },
				{ Constants.FIELD_ORDER_ORDER_ID_ORG + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(orderIdOrg) },
				{ Constants.FIELD_ORDER_SHOP_ID, shopId },
			};
			var dv = Get(XML_KEY_NAME, "GetExchangedOrderIds", ht);
			return dv.Cast<DataRowView>().Select(drv => (string)drv[0]).ToArray();
		}
		#endregion

		#region ~UpdateCouponForIntegration 注文クーポン情報更新(ユーザー統合時)
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="couponId">クーポンID</param>
		/// <param name="couponNo">クーポンNo</param>
		/// <param name="couponNoOld">クーポンNo(旧)</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateCouponForIntegration(
			string userId,
			string couponId,
			string couponNo,
			string couponNoOld,
			string lastChanged)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDER_USER_ID, userId },
				{ Constants.FIELD_ORDERCOUPON_COUPON_ID, couponId },
				{ Constants.FIELD_ORDERCOUPON_COUPON_NO, couponNo },
				{ Constants.FIELD_ORDERCOUPON_COUPON_NO + "_old", couponNoOld },
				{ Constants.FIELD_ORDERCOUPON_LAST_CHANGED, lastChanged },
			};
			var result = Exec(XML_KEY_NAME, "UpdateCouponForIntegration", input);
			return result;
		}
		#endregion

		#region +GetOrdersForElogitWmsCooperation
		/// <summary>
		/// Get orders for elogit wms cooperation
		/// </summary>
		/// <returns>Array of order model</returns>
		public OrderModel[] GetOrdersForElogitWmsCooperation()
		{
			var data = Get(XML_KEY_NAME, "GetOrdersForElogitWmsCooperation");
			var result = data.Cast<DataRowView>().Select(item => new OrderModel(item)
			{
				Items = new[] { new OrderItemModel(item) },
				Shippings = new[] { new OrderShippingModel(item) },
				OrderPriceByTaxRates = new[] { new OrderPriceByTaxRateModel(item) },
				Owner = new OrderOwnerModel(item),
			}).ToArray();
			return result;
		}
		#endregion

		#region +GetOrderCountByOrderWorkflowSetting
		/// <summary>
		/// Get order count by order workflow setting
		/// </summary>
		/// <param name="searchParam">Search param</param>
		/// <returns>Order count</returns>
		internal int GetOrderCountByOrderWorkflowSetting(Hashtable searchParam)
		{
			var replace = new[]
			{
				new KeyValuePair<string, string>(
					"@@ where @@",
					(string)searchParam["@@ where @@"])
			};
			searchParam.Remove("@@ where @@");

			var dv = Get(
				XML_KEY_NAME,
				"GetOrderCountByOrderWorkflowSetting",
				searchParam,
				replaces: replace);
			return (int)dv[0][0];
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, statementName, input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, statementName, input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		public void CheckFieldsForGetMaster(Hashtable input, string masterKbn, params KeyValuePair<string, string>[] replaces)
		{
			var statement = string.Empty;

			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER: // 注文マスタ表示
					statement = "CheckOrderFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM: // 注文商品マスタ表示
					statement = "CheckOrderItemFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION: // 注文セットプロモーションマスタ表示
					statement = "CheckOrderSetPromotionFields";
					break;
			}
			Get(XML_KEY_NAME, statement, input, replaces: replaces);
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		public void CheckFieldsForGetMasterDataBinding(Hashtable input, string masterKbn, params KeyValuePair<string, string>[] replaces)
		{
			var statement = string.Empty;
			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDER:
					statement = "CheckOrderFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERITEM:
					statement = "CheckOrderItemFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ORDERSETPROMOTION:
					statement = "CheckOrderSetPromotionFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_DATABINDING:
					statement = "CheckOrderDataBindingFields";
					break;
			}
			Get(XML_KEY_NAME, statement, input, replaces: replaces);
		}
		#endregion

		#region +GetItemByOrderIdAndProductId 注文ID、商品IDから商品取得
		/// <summary>
		/// 注文ID、商品IDから商品取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <returns>モデル</returns>
		public OrderItemModel GetItemByOrderIdAndProductId(string orderId, string productId, string variationId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDERITEM_ORDER_ID, orderId },
				{ Constants.FIELD_ORDERITEM_PRODUCT_ID, productId },
				{ Constants.FIELD_ORDERITEM_VARIATION_ID, variationId },
			};
			var dv = Get(XML_KEY_NAME, "GetItemByOrderIdAndProductId", ht);
			if (dv.Count == 0) return null;
			return new OrderItemModel(dv[0]);
		}
		#endregion

		#region +GetCvsDeferredAuthResultHold コンビニ後払いで与信結果がHOLDの注文を取得
		/// <summary>
		/// コンビニ後払いで与信結果がHOLDの注文を取得
		/// </summary>
		/// <returns>注文</returns>
		internal OrderModel[] GetCvsDeferredAuthResultHold()
		{
			var dv = Get(XML_KEY_NAME, "GetCvsDeferredAuthResultHold");
			var orders = dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
			return orders;
		}
		#endregion

		#region +GetOrderForGmoAtokaraAuthResult GMOアトカラで与信結果が審査中の注文を取得
		/// <summary>
		/// GMOアトカラで与信結果が審査中の注文を取得
		/// </summary>
		/// <returns>注文</returns>
		internal OrderModel[] GetOrderForGmoAtokaraAuthResult()
		{
			var dv = Get(XML_KEY_NAME, "GetOrderForGmoAtokaraAuthResult");
			var orders = dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
			return orders;
		}
		#endregion

		#region +GetOrderWorkflowListNoPagination
		/// <summary>
		/// Get order workflow list no pagination
		/// </summary>
		/// <param name="searchParam">Search condition</param>
		/// <returns>Dataview of order list</returns>
		internal DataView GetOrderWorkflowListNoPagination(Hashtable searchParam)
		{
			var replace = new[]
			{
				new KeyValuePair<string, string>("@@ where @@", (string) searchParam["@@ where @@"]),
				new KeyValuePair<string, string>(
					"@@ orderby @@",
					GetOrderSearchOrderByStringForOrderListAndWorkflow((string) searchParam["sort_kbn"])),
			};
			searchParam.Remove("@@ where @@");
			var dv = Get(
				XML_KEY_NAME,
				"GetOrderWorkflowListNoPagination",
				searchParam,
				replaces: replace);
			return dv;
		}
		#endregion

		#region +GetDeliveryTranIdListOrderWorkFlow
		/// <summary>
		/// Get delivery tran id list order workflow
		/// </summary>
		/// <param name="searchParam">Search condition</param>
		/// <returns>Dataview of delivery tran id list</returns>
		internal DataView GetDeliveryTranIdListOrderWorkFlow(Hashtable searchParam)
		{
			var replace = new[]
			{
				new KeyValuePair<string, string>("@@ where @@", (string) searchParam["@@ where @@"]),
				new KeyValuePair<string, string>(
					"@@ orderby @@",
					GetOrderSearchOrderByStringForOrderListAndWorkflow((string) searchParam["sort_kbn"])),
				new KeyValuePair<string, string>(
					"@@ multi_order_id @@",
					GetOrderSearchMultiOrderId(searchParam)),
			};
			searchParam.Remove("@@ where @@");
			var dv = Get(
				XML_KEY_NAME,
				"GetDeliveryTranIdListOrderWorkFlow",
				searchParam,
				replaces: replace);
			return dv;
		}
		#endregion

		#region +GetOrderSearchMultiOrderId
		/// <summary>
		/// Get Order Search Multi Order Id
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>String Multi Order Id</returns>
		private static string GetOrderSearchMultiOrderId(Hashtable input)
		{
			var result = StringUtility.ToEmpty(input[Constants.FIELD_ORDER_ORDER_ID + "_like_escaped"])
				.Replace("'", "''")
				.Replace(",", "','");

			return result;
		}
		#endregion

		#region +GetOrderPaymentIdsForAtobaraicomGetAuthorizeStatus
		/// <summary>
		/// Get order payment ids for atobaraicom get authorize status
		/// </summary>
		public string[] GetOrderPaymentIdsForAtobaraicomGetAuthorizeStatus()
		{
			var dv = Get(XML_KEY_NAME, "GetOrderPaymentIdsForAtobaraicomGetAuthorizeStatus");
			var orderPaymentIds = dv.Cast<DataRowView>()
				.Select(drv => StringUtility.ToEmpty(drv[0]))
				.ToArray();
			return orderPaymentIds;
		}
		#endregion

		#region +GetPointGrantOrder ポイント確定処理対象の注文情報を取得
		/// <summary>
		/// Get point grant order
		/// </summary>
		/// <param name="days">Days</param>
		/// <returns>Array of order model</returns>
		internal OrderModel[] GetPointGrantOrder(int days)
		{
			var input = new Hashtable
			{
				{ "point_temp_to_comp_days", days },
			};
			var dv = Get(XML_KEY_NAME, "GetPointGrantOrder", input);

			if (dv.Count == 0) return null;

			var result = dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
			return result;
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
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_ORDER_ID, orderId},
			};

			var dv = Get(XML_KEY_NAME, "GetOnlinePaymentStatusByOrderId", ht);
			var onlinePaymentStatus = dv.Cast<DataRowView>()
				.Select(drv => StringUtility.ToEmpty(drv[0]))
				.ToArray();
			return onlinePaymentStatus.FirstOrDefault();
		}
		#endregion

		#region +GetInReviewGmoOrder
		/// <summary>
		/// 審査中の注文リスト取得
		/// </summary>
		/// <returns>注文一覧</returns>
		public OrderModel[] GetInReviewGmoOrders()
		{
			var dv = Get(XML_KEY_NAME, "GetInReviewGmoOrder");
			if (dv.Count == 0) return new OrderModel[] { };
			return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
		}
		#endregion

		#region ~GetOrderHistoryListByFixedPurchaseId 注文履歴一覧（定期台帳単位）を取得する HACK: 例外的にDataViewを返す
		/// <summary>
		/// 注文履歴一覧（注文単位）を取得する HACK: 例外的にDataViewを返す
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="startRowNum">取得開始行番号</param>
		/// <param name="endRowNum">取得終了行番号</param>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <returns>モデル</returns>
		internal DataView GetOrderHistoryListByFixedPurchaseId(
			string userId,
			int startRowNum,
			int endRowNum,
			string fixedPurchaseId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDER_USER_ID, userId},
				{"bgn_row_num", startRowNum},
				{"end_row_num", endRowNum},
				{Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID, fixedPurchaseId},
			};
			var dv = Get(XML_KEY_NAME, "GetOrderHistoryListByFixedPurchaseId", ht);
			return dv;
		}
		#endregion

		#region ~GetOrderIdsForYahooOrderImport YAHOOモール注文取り込みを行う注文を取得
		/// <summary>
		/// YAHOOモール注文取り込みを行う注文を取得
		/// </summary>
		/// <returns>辞書型注文情報</returns>
		internal Dictionary<string, string>[] GetOrdersForYahooOrderImport(string mallId)
		{
			var dv = Get(
				XML_KEY_NAME,
				"GetOrderIdsForYahooOrderImport",
				new Hashtable { { Constants.FIELD_ORDER_MALL_ID, mallId }, });
			return dv.Cast<DataRowView>().Select(
				drv => new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(drv[Constants.FIELD_ORDER_ORDER_ID]) },
					{ Constants.FIELD_ORDER_USER_ID, StringUtility.ToEmpty(drv[Constants.FIELD_ORDER_USER_ID]) },
				}).ToArray();
		}
		#endregion

		#region ~AddYahooMallOrderDetail YAHOOモール注文取り込みした情報を更新
		/// <summary>
		/// YAHOOモール注文取り込みした注文情報を更新
		/// </summary>
		/// <returns>モデル</returns>
		internal int AddYahooMallOrderDetail(Hashtable input)
		{
			var result = Exec(XML_KEY_NAME, "AddYahooMallOrderDetail", input);
			return result;
		}
		#endregion

		#region ~AddYahooMallOrderOwnerDetail YAHOOモール注文取り込みした注文者情報を更新
		/// <summary>
		/// YAHOOモール注文取り込みした注文者情報を更新
		/// </summary>
		/// <returns>モデル</returns>
		internal int AddYahooMallOrderOwnerDetail(Hashtable input)
		{
			var result = Exec(XML_KEY_NAME, "AddYahooMallOrderOwnerDetail", input);
			return result;
		}
		#endregion

		#region ~AddYahooMallOrderShippingDetail YAHOOモール注文取り込みした注文配送情報を更新
		/// <summary>
		/// YAHOOモール注文取り込みした注文配送情報を更新
		/// </summary>
		/// <returns>モデル</returns>
		internal int AddYahooMallOrderShippingDetail(Hashtable input)
		{
			var result = Exec(XML_KEY_NAME, "AddYahooMallOrderShippingDetail", input);
			return result;
		}
		#endregion

		#region +GetAllForOrderMail 取得（注文メール用）
		/// <summary>
		/// 取得（注文メール用）
		/// </summary>
		/// <returns>メール送信用注文情報列</returns>
		public OrderModel[] GetAllForOrderMail()
		{
			var dv = Get(XML_KEY_NAME, "GetAllForOrderMail");

			return dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
		}
		#endregion

		#region +UpdateStorePickupStatus
		/// <summary>
		/// Update store pickup status
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>Number of affected cases</returns>
		public int UpdateStorePickupStatus(Hashtable input)
		{
			var result = Exec(XML_KEY_NAME, "UpdateStorePickupStatus", input);
			return result;
		}
		#endregion

		#region ~OrderShoppickUp
		/// <summary>
		/// Get order store pick up count
		/// </summary>
		/// <param name="input">Input</param>
		/// <param name="realShopIds">Real shop ids</param>
		/// <returns>Number of affected cases</returns>
		internal int GetOrderStorePickUpCount(Hashtable input, string realShopIds)
		{
			var whereRealShopIds = (string.IsNullOrEmpty(realShopIds) == false)
				? string.Format("AND w2_RealShop.real_shop_id IN ({0})", realShopIds)
				: string.Empty;

			var replace = new KeyValuePair<string, string>("@@ real_shop_ids_condition @@", whereRealShopIds);

			var dv = Get(XML_KEY_NAME, "GetOrderStorePickUpCount", input, replaces: replace);
			return (int)dv[0][0];
		}
		#endregion

		#region +GetOrdersForLetro
		/// <summary>
		/// Get orders for Letro
		/// </summary>
		/// <param name="searchInput">Search input</param>
		/// <returns>Orders for Letro</returns>
		internal OrderModel[] GetOrdersForLetro(Hashtable searchInput)
		{
			var dv = Get(XML_KEY_NAME, "GetOrdersForLetro", searchInput);
			if (dv.Count == 0) return new OrderModel[] { };
			var result = dv.Cast<DataRowView>().Select(drv => new OrderModel(drv)).ToArray();
			return result;
		}
		#endregion

		#region +GetOrderItemsForLetro
		/// <summary>
		/// Get order items for Letro
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <returns>Order items for Letro</returns>
		internal OrderItemModel[] GetOrderItemsForLetro(string orderId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_ORDERITEM_ORDER_ID, orderId },
			};

			var dv = Get(XML_KEY_NAME, "GetItemAll", input);
			if (dv.Count == 0) return new OrderItemModel[] { };
			var result = dv.Cast<DataRowView>().Select(drv => new OrderItemModel(drv)).ToArray();
			return result;
		}
		#endregion

		#region +GetCombineOrgOrderIds 注文同梱元注文IDを取得
		/// <summary>
		/// 注文同梱元注文IDを取得
		/// </summary>
		/// <param name="orderId">同梱注文ID</param>
		/// <returns>注文ID列</returns>
		internal string GetCombineOrgOrderIds(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId },
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return string.Empty;
			return (string)dv[0][Constants.FIELD_ORDER_COMBINED_ORG_ORDER_IDS];
		}
		#endregion

		#region +GetShippingOrderForDate 指定期間の出荷注文情報を取得(日次出荷予測レポート用)
		/// <summary>
		/// 指定期間の出荷注文情報を取得
		/// </summary>
		/// <param name="minDate">指定日最小値</param>
		/// <param name="maxDate">指定日最大値</param>
		/// <returns>注文情報</returns>
		internal DataView GetShippingOrderForDate(DateTime minDate, DateTime maxDate)
		{
			var ht = new Hashtable
			{
				{ Constants.MIN_DATE_SHIPPING_ORDER_SEARCH, minDate },
				{ Constants.MAX_DATE_SHIPPING_ORDER_SEARCH, maxDate },
			};
			var dv = Get(XML_KEY_NAME, "GetShippingOrderForDate", ht);
			if (dv.Count == 0) return null;
			return dv;
		}
		#endregion

		#region ~GetCountOrderWithLimitedPaymentIds 指定決済が利用不可決済になる商品を持つ注文の件数取得(注文同梱で利用)
		/// <summary>
		/// 指定決済が利用不可決済になる商品を持つ注文の件数取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="paymentId">決済種別ID</param>
		/// <returns>件数</returns>
		internal int GetCountOrderWithLimitedPaymentIds(string orderId, string paymentId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId },
				{ Constants.FIELD_PAYMENT_PAYMENT_ID + "_like_escaped", StringUtility.SqlLikeStringSharpEscape(paymentId) },
			};
			var dv = Get(XML_KEY_NAME, "GetCountOrderWithLimitedPaymentIds", ht);
			var result = (int)dv[0][0];

			return result;
		}
		#endregion
	}
}
