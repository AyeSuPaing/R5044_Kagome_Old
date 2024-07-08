/*
=========================================================================================================
  Module      : PaidyUtility (PaidyUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.ShopMessage;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.Payment.Paidy
{
	/// <summary>
	/// Paidy utility
	/// </summary>
	public class PaidyUtility
	{
		/// <summary>User credit card card disp name paidy customer</summary>
		public const string USERCREDITCARD_CARDDISPNAME_PAIDYCUSTOMER = "PaidyCustomer";
		/// <summary>Sex value: Male</summary>
		private const string FLG_USER_SEX_MALE = "Male";
		/// <summary>Sex value: Female</summary>
		private const string FLG_USER_SEX_FEMALE = "Female";
		/// <summary>Sex value: Unknown</summary>
		private const string FLG_USER_SEX_UNKNOWN = "Unknown";
		/// <summary>Excepted Order Status</summary>
		private static string[] EXCEPTED_ORDER_STATUS = new string[]
		{
			Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED,
			Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED,
		};
		/// <summary>Excepted Payment Ids</summary>
		private static string[] EXCEPTED_PAYMENT_IDS = new string[]
		{
			Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY,
		};

		/// <summary>
		/// Create Paidy payment object
		/// </summary>
		/// <param name="cart">The cart object</param>
		/// <param name="orderData">The order data</param>
		/// <param name="accessor">SQL accessor</param>
		/// <param name="isReturnExchange">Is Return Exchange</param>
		/// <returns>A Paidy payment object</returns>
		public static PaidyPaymentObject CreatePaidyPaymentObject(
			CartObject cart,
			Hashtable orderData,
			SqlAccessor accessor = null,
			bool isReturnExchange = false)
		{
			// Created Billing Address
			var billingAddress = CreatedBillingAddress(
				cart.Owner.Addr1,
				cart.Owner.Addr2,
				cart.Owner.Addr3,
				cart.Owner.Addr4,
				cart.Owner.Zip);

			// Created Buyer Data
			var userId = StringUtility.ToEmpty(orderData[Constants.FIELD_ORDER_USER_ID]);
			var fixedPurchaseId = StringUtility.ToEmpty(orderData[Constants.FIELD_ORDER_FIXED_PURCHASE_ID]);
			var subscriptionCounter = 0;
			if (string.IsNullOrEmpty(fixedPurchaseId) == false)
			{
				subscriptionCounter = (((isReturnExchange == false) && (cart.FixedPurchase != null))
					? (cart.FixedPurchase.OrderCount + 1)
					: GetSubscriptionCounter(fixedPurchaseId, accessor));
			}
			var buyerData = CreatedBuyerData(
				billingAddress,
				userId,
				subscriptionCounter,
				accessor);

			// Created Shipping Address
			var shipping = cart.GetShipping();
			var shippingAddress = CreatedShippingAddress(
				shipping.Addr1,
				shipping.Addr2,
				shipping.Addr3,
				shipping.Addr4,
				shipping.Zip);

			// Created Order
			var items = CreateOrderItems(cart);
			var order = CreatedOrder(
				StringUtility.ToEmpty(orderData[Constants.FIELD_ORDER_ORDER_ID]),
				items,
				cart.PriceSubtotalTax,
				cart.PriceShipping);

			// Created Payment
			var tokenId = (string.IsNullOrEmpty(cart.Payment.PaidyToken) == false)
				? cart.Payment.PaidyToken
				: cart.Payment.UserCreditCard.CooperationId;
			var payment = CreatedPayment(
				tokenId,
				cart.PriceTotal,
				buyerData,
				shippingAddress,
				order);

			return payment;
		}

		/// <summary>
		/// Created Payment
		/// </summary>
		/// <param name="tokenId">Token Id</param>
		/// <param name="priceTotal">Price Total</param>
		/// <param name="buyerData">Buyer Data</param>
		/// <param name="shippingAddress">Shipping Address</param>
		/// <param name="order">Order</param>
		/// <returns>Payment Object</returns>
		private static PaidyPaymentObject CreatedPayment(
			string tokenId,
			decimal priceTotal,
			PaidyPaymentObject.PaidyBuyerDataObject buyerData,
			PaidyPaymentObject.PaidyShippingAddressObject shippingAddress,
			PaidyPaymentObject.PaidyOrderObject order)
		{
			var payment = new PaidyPaymentObject()
			{
				TokenId = tokenId,
				Amount = (int)priceTotal,
				Currency = Constants.CONST_KEY_CURRENCY_CODE,
				Description = string.Empty,
				StoreName = ShopMessageUtil.GetMessage("ShopName"),
				BuyerData = buyerData,
				Order = order,
				ShippingAddress = shippingAddress
			};

			return payment;
		}

		/// <summary>
		/// Created Order
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="items">Items</param>
		/// <param name="priceSubtotalTax">Price Subtotal Tax</param>
		/// <param name="priceShipping">Price Shipping</param>
		/// <returns>Order Object</returns>
		private static PaidyPaymentObject.PaidyOrderObject CreatedOrder(
			string orderId,
			List<PaidyPaymentObject.PaidyOrderItemObject> items,
			decimal priceSubtotalTax,
			decimal priceShipping)
		{
			var tax = Constants.MANAGEMENT_INCLUDED_TAX_FLAG
				? 0
				: (int)priceSubtotalTax;
			var order = new PaidyPaymentObject.PaidyOrderObject()
			{
				Items = items,
				Tax = tax,
				Shipping = (int)priceShipping,
				OrderRef = orderId,
			};

			return order;
		}

		/// <summary>
		/// Created Billing Address
		/// </summary>
		/// <param name="addr1">Address 1</param>
		/// <param name="addr2">Address 2</param>
		/// <param name="addr3">Address 3</param>
		/// <param name="addr4">Address 4</param>
		/// <param name="zip">Zip Code</param>
		/// <returns>Billing Address Object</returns>
		private static PaidyPaymentObject.PaidyBillingAddressObject CreatedBillingAddress(
			string addr1,
			string addr2,
			string addr3,
			string addr4,
			string zip)
		{
			var billingAddress = new PaidyPaymentObject.PaidyBillingAddressObject()
			{
				Line1 = addr4,
				Line2 = addr3,
				State = addr1,
				City = addr2,
				Zip = zip,
			};

			return billingAddress;
		}

		/// <summary>
		/// Created Shipping Address
		/// </summary>
		/// <param name="addr1">Address 1</param>
		/// <param name="addr2">Address 2</param>
		/// <param name="addr3">Address 3</param>
		/// <param name="addr4">Address 4</param>
		/// <param name="zip">Zip Code</param>
		/// <returns>Shipping Address Object</returns>
		private static PaidyPaymentObject.PaidyShippingAddressObject CreatedShippingAddress(
			string addr1,
			string addr2,
			string addr3,
			string addr4,
			string zip)
		{
			var shippingAddress = new PaidyPaymentObject.PaidyShippingAddressObject()
			{
				Line1 = addr4,
				Line2 = addr3,
				State = addr1,
				City = addr2,
				Zip = zip,
			};

			return shippingAddress;
		}

		/// <summary>
		/// Created Buyer Data
		/// </summary>
		/// <param name="billingAddress">Billing Address</param>
		/// <param name="userId">User Id</param>
		/// <param name="subscriptionCounter">Subscription Counter</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Buyer Data Object</returns>
		private static PaidyPaymentObject.PaidyBuyerDataObject CreatedBuyerData(
			PaidyPaymentObject.PaidyBillingAddressObject billingAddress,
			string userId,
			int subscriptionCounter,
			SqlAccessor accessor)
		{
			var orderService = new OrderService();
			var user = new UserService().Get(userId);

			var ordersWithoutReturnExchangeAndRejection = orderService.GetOrdersWithoutReturnExchangeAndRejection(
				userId,
				EXCEPTED_ORDER_STATUS,
				EXCEPTED_PAYMENT_IDS,
				accessor);
			var orderCount = ordersWithoutReturnExchangeAndRejection.Length;
			var ltv = (int)ordersWithoutReturnExchangeAndRejection.Sum(item => item.OrderPriceTotal);

			var lastOrder = (orderCount > 0)
				? ordersWithoutReturnExchangeAndRejection.OrderByDescending(order => order.DateCreated).ToArray()[0]
				: null;

			var ordersLastThreeMonthWithoutReturnExchange = orderService.GetOrdersLastThreeMonthWithoutReturnExchange(
				userId,
				EXCEPTED_ORDER_STATUS,
				EXCEPTED_PAYMENT_IDS,
				accessor);
			var orderAmountLast3Months = (int)ordersLastThreeMonthWithoutReturnExchange.Sum(item => item.OrderPriceTotal);
			var orderCountLast3Months = ordersLastThreeMonthWithoutReturnExchange.Length;

			var buyerData = new PaidyPaymentObject.PaidyBuyerDataObject()
			{
				UserId = userId,
				Age = GetUserCreatedAge(user),
				OrderCount = orderCount,
				Ltv = ltv,
				LastOrderAmount = GetLastOrderAmount(lastOrder),
				LastOrderAt = GetLastOrderAt(lastOrder),
				SubscriptionCounter = subscriptionCounter,
				Gender = GetGender(user),
				NumberOfPoints = GetRealPointsOfUser(userId),
				OrderAmountLast3Months = orderAmountLast3Months,
				OrderCountLast3Months = orderCountLast3Months,
				BillingAddress = billingAddress
			};

			return buyerData;
		}

		/// <summary>
		/// Create order item
		/// </summary>
		/// <param name="cart">The cart object</param>
		/// <returns>A Paidy order items</returns>
		private static List<PaidyPaymentObject.PaidyOrderItemObject> CreateOrderItems(CartObject cart)
		{
			// Create items
			var result = cart.Items.Select(cartProduct => CreateOrderItem(cartProduct)).ToList();

			// 割引額
			var discount = cart.MemberRankDiscount
				+ cart.UseCouponPrice
				+ cart.SetPromotions.ProductDiscountAmount
				+ cart.SetPromotions.ShippingChargeDiscountAmount
				+ cart.SetPromotions.PaymentChargeDiscountAmount
				+ cart.FixedPurchaseDiscount
				+ cart.FixedPurchaseMemberDiscountAmount;
			if (discount > 0)
			{
				var item = CreateOrderItem("割引額", -(int)discount);
				result.Add(item);
			}

			// ポイント利用額
			if (cart.UsePointPrice > 0)
			{
				var item = CreateOrderItem("ポイント利用額", -(int)cart.UsePointPrice);
				result.Add(item);
			}

			// 調整金額
			if (cart.PriceRegulationTotal != 0)
			{
				var item = CreateOrderItem("調整金額", (int)cart.PriceRegulationTotal);
				result.Add(item);
			}

			// 決済手数料
			if ((cart.Payment != null)
				&& (cart.Payment.PriceExchange != 0))
			{
				var item = CreateOrderItem("決済手数料", (int)cart.Payment.PriceExchange);
				result.Add(item);
			}

			return result;
		}

		/// <summary>
		/// Create order item object
		/// </summary>
		/// <param name="cartProduct">The cart product</param>
		/// <returns>An order item object</returns>
		private static PaidyPaymentObject.PaidyOrderItemObject CreateOrderItem(CartProduct cartProduct)
		{
			var orderItem =  new PaidyPaymentObject.PaidyOrderItemObject()
			{
				Id = cartProduct.ProductId,
				Quantity = cartProduct.Count,
				Title = cartProduct.ProductJointName,
				Description = string.Empty,
				UnitPrice = (int)cartProduct.Price,
			};

			return orderItem;
		}

		/// <summary>
		/// Create order item object
		/// </summary>
		/// <param name="name">An order item name</param>
		/// <param name="price">A order item price</param>
		/// <returns>An order item object</returns>
		private static PaidyPaymentObject.PaidyOrderItemObject CreateOrderItem(string name, int price)
		{
			var orderItem = new PaidyPaymentObject.PaidyOrderItemObject()
			{
				Id = "xxxxx",
				Quantity = 1,
				Title = name,
				Description = string.Empty,
				UnitPrice = price
			};

			return orderItem;
		}

		/// <summary>
		/// Created Buyer Data Object For Paidy Payment
		/// </summary>
		/// <param name="userId">user Id</param>
		/// <returns>Buyer Data Object</returns>
		public static string CreatedBuyerDataObjectForPaidyPayment(string userId)
		{
			var user = new UserService().Get(userId);
			if (user == null) return string.Empty;

			var result = string.Format(" {{ "
				+ "email : '{0}',"
				+ "name1 : '{1}',"
				+ "name2 : '{2}',"
				+ "phone : '{3}'"
				+ " }} ",
				user.MailAddr,
				user.Name,
				user.NameKana,
				(user.Tel1 == null) ? "" : user.Tel1.Replace("-", string.Empty));

			return result;
		}

		/// <summary>
		/// Created Buyer Data Object For Paidy Payment
		/// </summary>
		/// <param name="cartList">Cart object list</param>
		/// <returns>Buyer Data Object</returns>
		public static string CreatedBuyerDataObjectForPaidyPayment(CartObjectList cartList)
		{
			if ((cartList == null) || (cartList.Owner == null)) return string.Empty;

			var owner = cartList.Owner;
			var result = string.Format(" {{ "
				+ "email : '{0}',"
				+ "name1 : '{1}',"
				+ "name2 : '{2}',"
				+ "phone : '{3}'"
				+ " }} ",
				owner.MailAddr,
				owner.Name,
				owner.NameKana,
				(owner.Tel1 == null) ? "" : owner.Tel1.Replace("-", string.Empty));

			return result;
		}

		/// <summary>
		/// Register As User Credit Card
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="tokenId">Token Id</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード</returns>
		public static UserCreditCardModel RegisterAsUserCreditCard(
			string userId,
			string tokenId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			var userCreditCard = new UserCreditCardModel
			{
				UserId = userId,
				CooperationId = tokenId,
				CooperationId2 = string.Empty,
				CardDispName = USERCREDITCARD_CARDDISPNAME_PAIDYCUSTOMER,
				LastFourDigit = string.Empty,
				ExpirationMonth = string.Empty,
				ExpirationYear = string.Empty,
				AuthorName = string.Empty,
				DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
				LastChanged = lastChanged,
				CompanyCode = string.Empty,
				CooperationType = Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_PAIDY,
			};

			new UserCreditCardService().Insert(
				userCreditCard,
				updateHistoryAction,
				accessor);

			return userCreditCard;
		}

		/// <summary>
		/// Is Token Id Exist
		/// </summary>
		/// <param name="tokenId">Token Id</param>
		/// <returns>True: token has exist, otherwise: false</returns>
		public static bool IsTokenIdExist(string tokenId)
		{
			var userCreditCart = new UserCreditCardService().GetByCooperationId1(tokenId);
			
			return (userCreditCart != null);
		}

		/// <summary>
		/// Get User Created Age
		/// </summary>
		/// <param name="user">user</param>
		/// <returns>User Created Age</returns>
		private static int GetUserCreatedAge(UserModel user)
		{
			var result = (user != null)
				? (int)(DateTime.Now - user.DateCreated).TotalDays
				: 0;

			return result;
		}

		/// <summary>
		/// Get Gender
		/// </summary>
		/// <param name="user">user</param>
		/// <returns>Gender Of User</returns>
		public static string GetGender(UserModel user)
		{
			var gender = (user != null)
				? user.Sex
				: Constants.FLG_USER_SEX_MALE;
			var result = string.Empty;

			switch (gender)
			{
				case Constants.FLG_USER_SEX_MALE:
					result = FLG_USER_SEX_MALE;
					break;

				case Constants.FLG_USER_SEX_FEMALE:
					result = FLG_USER_SEX_FEMALE;
					break;
					
				default:
					result = FLG_USER_SEX_UNKNOWN;
					break;
			}

			return result;
		}

		/// <summary>
		/// Get Real Points Of User
		/// </summary>
		/// <param name="userId">User Id</param>
		/// <returns>Real Points Of User</returns>
		public static int GetRealPointsOfUser(string userId)
		{
			if (string.IsNullOrEmpty(userId)) return 0;

			var result = new PointService().GetUserPoint(userId, string.Empty)
				.Where(item => item.PointType == Constants.FLG_USERPOINT_POINT_TYPE_COMP)
				.Sum(item => item.Point);

			return decimal.ToInt32(result);
		}

		/// <summary>
		/// Get Last Order Amount
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Last Order Amount</returns>
		private static int GetLastOrderAmount(OrderModel order)
		{
			var result = ((order != null) && (order.OrderPriceTotal > 0))
				? (int)order.OrderPriceTotal
				: 0;

			return result;
		}

		/// <summary>
		/// Get Last Order At
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Last Order At</returns>
		private static int GetLastOrderAt(OrderModel order)
		{
			var result = ((order != null) && order.OrderDate.HasValue)
				? (int)(DateTime.Now - order.OrderDate.Value).TotalDays
				: 0;

			return result;
		}

		/// <summary>
		/// Get Subscription Counter
		/// </summary>
		/// <param name="fixedPurchaseId">Fixed Purchase Id</param>
		/// <param name="accessor">SQL Accessor</param>
		/// <returns>Subscription Counter</returns>
		private static int GetSubscriptionCounter(
			string fixedPurchaseId,
			SqlAccessor accessor)
		{
			var fixedPurchase = new FixedPurchaseService().Get(fixedPurchaseId, accessor);
			var result = (fixedPurchase != null)
				? fixedPurchase.OrderCount
				: 0;

			return result;
		}
	}
}
