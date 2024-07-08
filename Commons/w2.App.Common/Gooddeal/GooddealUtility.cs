/*
=========================================================================================================
  Module      : Gooddeal utility (GooddealUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Domain.DeliveryCompany;
using w2.Domain.Order;

namespace w2.App.Common.Gooddeal
{
	/// <summary>
	/// Gooddeal utility
	/// </summary>
	public static class GooddealUtility
	{
		/// <summary>
		/// Create register shipping delivery request
		/// </summary>
		/// <param name="order">Order information</param>
		/// <param name="timestamp">timestamp</param>
		/// <returns>Gooddeal request</returns>
		public static GooddealRequest CreateRegisterShippingDeliveryRequest(OrderModel order, string timestamp)
		{
			var shipping = order.Shippings.FirstOrDefault();
			var deliveryCompany = new DeliveryCompanyService().Get(shipping.DeliveryCompanyId);

			var request = new GooddealRequest
			{
				CheckTime = timestamp,
				DeliveryTypeId = deliveryCompany.DeliveryCompanyTypeGooddeal,
				RegisterOrderNo = order.OrderId,
				PostId = IsCrossBorderOrder(deliveryCompany.DeliveryCompanyTypeGooddeal) 
					? string.Empty
					: shipping.ShippingZip.Substring(0, 3),
				Address = string.Format(
					"{0}{1}{2}",
					shipping.ShippingAddr2,
					shipping.ShippingAddr3,
					shipping.ShippingAddr4),
				Name = shipping.ShippingName,
				PhoneNo = (shipping.ShippingTel1.StartsWith(GooddealConstants.CONST_TW_PREFIX_TEL_NO) == false)
					? shipping.ShippingTel1
					: string.Empty,
				MobileNo = shipping.ShippingTel1.StartsWith(GooddealConstants.CONST_TW_PREFIX_TEL_NO)
					? shipping.ShippingTel1
					: string.Empty,
				AgencyFee = (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
					? order.OrderPriceTotal.ToString()
					: string.Empty,
				Packstr = string.Empty,
				StoreId = string.Empty,
				StoreName = string.Empty,
				DeliveryNo = string.Empty,
				Note = order.Memo,
				Items = CreateProducts(shipping.Items),
				DeliveryTimeId = (string.IsNullOrEmpty(shipping.ShippingTime) == false)
					? shipping.ShippingTime
					: GooddealConstants.CONST_SHIPPING_TIME_NOT_SPECIFY,
			};

			// 越境注文の場合追加項目セット
			if (IsCrossBorderOrder(deliveryCompany.DeliveryCompanyTypeGooddeal))
			{
				request.Country = shipping.ShippingCountryName;
				request.CountryPost = shipping.ShippingZip.Replace("-", "");
				request.CurrencyId = order.SettlementCurrency;
				request.PostValue = order.OrderPriceTotal.ToString();
				request.PostPaytype = IsCashOnDelivery(order.OrderPaymentKbn)
					? GooddealConstants.PostPayType.CashOnDelivery.ToText()
					: GooddealConstants.PostPayType.SenderPayment.ToText();
			}
			return request;
		}

		/// <summary>
		/// 注文が越境注文か
		/// </summary>
		/// <param name="deliveryCompanyTypeGooddeal">出荷連携配送会社(Gooddeal)</param>
		/// <returns>越境注文か</returns>
		private static bool IsCrossBorderOrder(string deliveryCompanyTypeGooddeal)
		{
			var isCrossBorderOrder = GooddealConstants.TRANSNATIONNAL_NUMBER.Contains(deliveryCompanyTypeGooddeal);
			return isCrossBorderOrder;
		}

		/// <summary>
		/// 注文が着払いか
		/// </summary>
		/// <param name="orderPaymentKbn">注文支払い区分</param>
		/// <returns>着払いか</returns>
		private static bool IsCashOnDelivery(string orderPaymentKbn)
		{
			var isCashOnDelivery = Constants.TwShipping_Gooddeal_Post_PayType.Contains(orderPaymentKbn);
			return isCashOnDelivery;
		}

		/// <summary>
		/// Create products
		/// </summary>
		/// <param name="Items">Items</param>
		/// <returns>Products</returns>
		private static Product[] CreateProducts(OrderItemModel[] Items)
		{
			var products = Items.Select(orderItem =>
				new Product
				{
					ProductId = orderItem.ProductId,
					Price = orderItem.ProductPrice,
					Quantity = orderItem.ItemQuantity
				});
			return products.ToArray();
		}

		/// <summary>
		/// Create cancel shipping delivery request
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="timestamp">timestamp</param>
		/// <returns>Gooddeal request</returns>
		public static GooddealRequest CreateCancelShippingDeliveryRequest(string orderId, string timestamp)
		{
			var request = new GooddealRequest
			{
				CheckTime = timestamp,
				OrderNo = orderId,
			};
			return request;
		}

		/// <summary>
		/// Create get shipping delivery slip request
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="timestamp">timestamp</param>
		/// <returns>Gooddeal request</returns>
		public static GooddealRequest CreateGetShippingDeliverySlipRequest(string orderId, string timestamp)
		{
			var request = new GooddealRequest
			{
				CheckTime = timestamp,
				OrderNo = orderId,
			};
			return request;
		}

		/// <summary>
		/// Send mail for operator
		/// </summary>
		/// <param name="beginTime">Begin time</param>
		/// <param name="endTime">End time</param>
		/// <param name="executeCount">Execute count</param>
		/// <param name="successCount">Success count</param>
		/// <param name="failureCount">Failure count</param>
		/// <param name="messages">Messages</param>
		public static void SendMailForOperator(
			DateTime beginTime,
			DateTime endTime,
			int executeCount,
			int successCount,
			int failureCount,
			string[] messages)
		{
			var input = new Hashtable
			{
				{ GooddealConstants.GOODDEAL_MAIL_TAG_REPLACE_KEY_TIME_BEGIN,beginTime.ToString(GooddealConstants.CONST_LONGDATE_FORMAT) },
				{ GooddealConstants.GOODDEAL_MAIL_TAG_REPLACE_KEY_TIME_END, endTime.ToString(GooddealConstants.CONST_LONGDATE_FORMAT) },
				{ GooddealConstants.GOODDEAL_MAIL_TAG_REPLACE_KEY_EXECUTE_COUNT, executeCount },
				{ GooddealConstants.GOODDEAL_MAIL_TAG_REPLACE_KEY_SUCCESS_COUNT, successCount },
				{ GooddealConstants.GOODDEAL_MAIL_TAG_REPLACE_KEY_FAILURE_COUNT, failureCount },
				{ GooddealConstants.GOODDEAL_MAIL_TAG_REPLACE_KEY_MESSAGE, string.Join(Environment.NewLine, messages) }
			};

			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_GOODDEAL_SHIPPING_FOR_OPERATOR,
				string.Empty,
				input,
				true,
				Constants.MailSendMethod.Manual))
			{
				if (mailSender.SendMail() == false)
				{
					FileLogger.WriteError(mailSender.MailSendException);
				}
			}
		}

		/// <summary>
		/// Timestampを取得する
		/// </summary>
		/// <returns>Timestamp</returns>
		public static string GetTimestamp()
		{
			return new DateTimeOffset(DateTime.UtcNow).ToUnixTimeSeconds().ToString();
		}
	}
}
