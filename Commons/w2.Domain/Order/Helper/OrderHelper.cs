/*
=========================================================================================================
  Module      : 注文情報のためのヘルパクラス (OrderHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.Domain.Order.Helper
{
	/// <summary>
	/// 注文情報のためのヘルパクラス
	/// </summary>
	internal class OrderHelper
	{
		/// <summary>
		/// 全ての注文情報を含む注文情報データビューからモデルを返す
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="isGetOrderItem">注文商品情報を取得するか</param>
		/// <returns>注文情報モデル</returns>
		internal OrderModel GetOrder(DataView order, bool isGetOrderItem = false)
		{
			if (order.Count == 0) return null;
			var result = this.GetOrders(order, isGetOrderItem);
			return result[0];
		}

		/// <summary>
		/// 全ての注文情報を含む注文情報データビューからモデルリストを返す
		/// </summary>
		/// <param name="orders">注文情報</param>
		/// <param name="isGetOrderItem">注文商品情報を取得するか</param>
		/// <returns>注文情報モデル配列</returns>
		internal OrderModel[] GetOrders(DataView orders, bool isGetOrderItem = false)
		{
			var result = new List<OrderModel>();
			var owners = new List<OrderOwnerModel>();
			var shippings = new List<OrderShippingModel>();
			var items = new List<OrderItemModel>();
			var coupons = new List<OrderCouponModel>();
			var setpromotions = new List<OrderSetPromotionModel>();
			var orderPriceByTaxRates = new List<OrderPriceByTaxRateModel>();
			foreach (DataRowView order in orders)
			{
				var orderId = (string)order[Constants.FIELD_ORDER_ORDER_ID];
				var orderShippingNo = (int)order[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO];
				var taxRate = (decimal)order[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE];
				var orderItemNo = (int)order[Constants.FIELD_ORDERITEM_ORDER_ITEM_NO];
				if (result.Exists(o => o.OrderId == orderId) == false)
				{
					result.Add(new OrderModel(order));
				}
				if (owners.Exists(o => o.OrderId == orderId) == false)
				{
					owners.Add(new OrderOwnerModel(order));
				}
				if (shippings.Exists(o =>
					(o.OrderId == orderId)
					&& (o.OrderShippingNo == orderShippingNo)) == false)
				{
					shippings.Add(new OrderShippingModel(order));
				}
				if (orderPriceByTaxRates.Exists(o =>
					((o.OrderId == orderId) && (o.KeyTaxRate == taxRate))) == false)
				{
					orderPriceByTaxRates.Add(new OrderPriceByTaxRateModel(order));
				}
				if (items.Exists(o =>
					(o.OrderId == orderId)
					&& (o.OrderItemNo == orderItemNo)
					&& (o.OrderShippingNo == orderShippingNo)) == false)
				{
					var item = new OrderItemModel(order);
					item.SupplierId = (string)order["item_supplier_id"];
					item.DateCreated = (DateTime)order["item_date_created"];
					item.DateChanged = (DateTime)order["item_date_changed"];
					item.SubscriptionBoxCourseId = (string)order[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_COURSE_ID_WITH_PREFIX];
					var subscriptionBoxFixedAmount =
						order[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX] != DBNull.Value
							? (decimal?)order[Constants.FIELD_ORDERITEM_SUBSCRIPTION_BOX_FIXED_AMOUNT_WITH_PREFIX]
							: null;
					item.SubscriptionBoxFixedAmount = subscriptionBoxFixedAmount;
					items.Add(item);
				}
				if (order[Constants.FIELD_ORDERCOUPON_COUPON_NO] != DBNull.Value)
				{
					var orderCouponNo = (int)order[Constants.FIELD_ORDERCOUPON_COUPON_NO];
					if (coupons.Exists(o =>
						(o.OrderId == orderId)
						&& (o.OrderCouponNo == orderCouponNo)) == false)
					{
						coupons.Add(new OrderCouponModel(order));
					}
				}
				if (order["promotion_" + Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO] != DBNull.Value)
				{
					var orderSetPromotionNo = (int)order["promotion_" + Constants.FIELD_ORDERSETPROMOTION_ORDER_SETPROMOTION_NO];
					if (setpromotions.Exists(o =>
						(o.OrderId == orderId)
						&& (o.OrderSetpromotionNo == orderSetPromotionNo)) == false)
					{
						var setPromotion = new OrderSetPromotionModel(order);
						setPromotion.OrderSetpromotionNo = orderSetPromotionNo;
						setpromotions.Add(setPromotion);
					}
				}
			}

			// 配送先情報に商品情報セット
			foreach (var shipping in shippings)
			{
				shipping.Items = items.Where(o =>
						(o.OrderId == shipping.OrderId)
						&& (o.OrderShippingNo == shipping.OrderShippingNo))
					.OrderBy(o => o.OrderItemNo)
					.ToArray();
			}
			foreach (var order in result)
			{
				// 注文者情報
				order.Owner = owners.FirstOrDefault(o => o.OrderId == order.OrderId);
				// 配送先情報＆商品情報
				order.Shippings =
					shippings.Where(o => o.OrderId == order.OrderId)
					.OrderBy(o => o.OrderShippingNo)
					.ToArray();
				order.OrderPriceByTaxRates = orderPriceByTaxRates.Where(o => (o.OrderId == order.OrderId)).ToArray();
				if (order.LastAuthFlg == Constants.FLG_ORDER_LAST_AUTH_FLG_ON || isGetOrderItem) order.Items = items.ToArray();
				// クーポン情報
				order.Coupons =
					coupons.Where(o => o.OrderId == order.OrderId)
					.OrderBy(o => o.OrderCouponNo)
					.ToArray();
				// セットプロモーション情報
				order.SetPromotions =
					setpromotions.Where(o => o.OrderId == order.OrderId)
					.OrderBy(o => o.OrderSetpromotionNo)
					.ToArray();
			}

			return result.ToArray();
		}
	}
}
