/*
=========================================================================================================
  Module      : Paidy Checkout クラス(PaidyCheckout.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Option;
using w2.App.Common.Order.Payment.Paidy;
using w2.App.Common.Order.Payment.Paygent.Logger;
using w2.App.Common.Order.Payment.Paygent.Paidy.Checkout.Dto;
using w2.Domain;
using w2.Domain.User;

namespace w2.App.Common.Order.Payment.Paygent.Paidy.Checkout
{
	/// <summary>
	/// Paidy Checkout クラス
	/// </summary>
	public class PaidyCheckout
	{
		/// <summary>例外注文ステータス</summary>
		private static string[] EXCEPTED_ORDER_STATUS =
		{
			Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED,
			Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED,
		};
		/// <summary>例外決済ID</summary>
		private static string[] EXCEPTED_PAYMENT_IDS =
		{
			Constants.FLG_PAYMENT_PAYMENT_ID_PAIDY,
		};

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カートデータ</param>
		public PaidyCheckout(CartObject cart)
		{
			if (cart == null) return;

			var owner = cart.Owner;
			var buyer = new PaidyCheckoutResponseBuyer(
				owner.MailAddr,
				owner.Name,
				owner.NameKana,
				owner.Tel1,
				owner.Birth.ToString());

			var shipping = cart.GetShipping();
			var shippingAddress = new PaidyCheckoutResponseShippingAddress(
				shipping.Addr4,
				shipping.Addr3,
				shipping.Addr2,
				shipping.Addr1,
				shipping.Zip);

			var productCategories = new List<string>();
			foreach (var item in cart.Items)
			{
				var addCategory = new Action<string>(categoryId =>
				{
					if ((categoryId == null) || (string.IsNullOrWhiteSpace(categoryId))) return;

					productCategories.Add(categoryId);
				});

				addCategory(item.CategoryId1);
				addCategory(item.CategoryId2);
				addCategory(item.CategoryId3);
				addCategory(item.CategoryId4);
				addCategory(item.CategoryId5);
			}

			var productCategorys = productCategories.Distinct().ToArray();
			var ordersLastThreeMonthWithoutReturnExchange = DomainFacade.Instance.OrderService.GetOrdersLastThreeMonthWithoutReturnExchange(
				cart.CartUserId,
				EXCEPTED_ORDER_STATUS,
				EXCEPTED_PAYMENT_IDS);
			var orderAmountLast3Months = (int)ordersLastThreeMonthWithoutReturnExchange.Sum(item => item.OrderPriceTotal);
			var orderCountLast3Months = ordersLastThreeMonthWithoutReturnExchange.Length;
			var billingAddress = new PaidyCheckoutResponseShippingAddress(
				owner.Addr4,
				owner.Addr3,
				owner.Addr2,
				owner.Addr1,
				owner.Zip);

			PaidyCheckoutResponseBuyerData buyerData;
			var userService = new UserService();
			var user = userService.Get(cart.CartUserId);
			if (user != null)
			{
				var userAttribute = userService.GetUserAttribute(cart.CartUserId);
				if (userAttribute == null)
				{
					userService.InsertUpdateUserAttributeOrderInfo(
						cart.CartUserId,
						DateTime.Now.ToString());
					userAttribute = userService.GetUserAttribute(cart.CartUserId);
				}

				var userPoint = (int)new UserPointObject(DomainFacade.Instance.PointService.GetUserPoint(user.UserId, cart.CartId)).PointComp;
				var age = UserAge(cart.Owner.Birth.Value);
				var agePlatform = (int)(DateTime.Now - user.DateCreated).TotalDays;
				var daysSinceFirstTransaction = (userAttribute.FirstOrderDate.HasValue)
					? (int)(DateTime.Now - userAttribute.FirstOrderDate.Value).TotalDays
					: 0;
				var lastOrderAt = (userAttribute.LastOrderDate.HasValue)
					? (int)(DateTime.Now - userAttribute.LastOrderDate.Value).TotalDays
					: 0;

				buyerData = new PaidyCheckoutResponseBuyerData(
					cart.CartUserId,
					age,
					agePlatform,
					user.DateCreated.ToString(),
					daysSinceFirstTransaction,
					(double)userAttribute.OrderAmountOrderAll,
					userAttribute.OrderCountOrderAll,
					lastOrderAmount: 1,
					lastOrderAt,
					userAttribute.FirstOrderDate.ToString(),
					orderAmountLast3Months,
					orderCountLast3Months,
					shippingAddress,
					billingAddress,
					PaidyUtility.GetGender(user),
					userPoint,
					productCategorys);
			}
			else
			{
				var today = DateTime.Now;
				buyerData = new PaidyCheckoutResponseBuyerData(
					cart.CartUserId,
					today.Year - int.Parse(cart.Owner.BirthYear),
					agePlatform: 0,
					today.ToString(),
					daysSinceFirstTransaction: 0,
					ltv: 0,
					orderCount: 0,
					lastOrderAmount: 1,
					lastOrderAt: 0,
					today.ToString(),
					orderAmountLast3Months,
					orderCountLast3Months,
					shippingAddress,
					billingAddress,
					gender: "Unknown",
					numberOfPoints: 0,
					productCategorys);
			}

			var product = CreateOrderItems(cart);
			var tax = (double)cart.PriceSubtotalTax;
			var order = new PaidyCheckoutResponseOrder(
				product,
				cart.CartId,
				(double)cart.ShippingPriceWithDiscount,
				tax);

			this.PostBody = new PaidyCheckoutPostBody(
				(double)cart.SendingAmount,
				cart.SettlementCurrency,
				Constants.PAYMENT_PAYGENT_MERCHANTNAME,
				buyer,
				buyerData,
				order,
				shippingAddress,
				description: string.Empty);
		}

		/// <summary>
		/// 誕生日から年齢を算出
		/// </summary>
		/// <param name="birthDay">誕生日</param>
		/// <returns>年齢</returns>
		protected int UserAge(DateTime birthDay)
		{
			var age = (DateTime.Today >= birthDay)
				? (new DateTime((DateTime.Today - birthDay).Ticks).Year - 1)
				: 0;
			return age;
		}

		/// <summary>
		/// Create order item
		/// </summary>
		/// <param name="cart">The cart object</param>
		/// <returns>A Paidy order items</returns>
		private static PaidyCheckoutResponseItems[] CreateOrderItems(CartObject cart)
		{
			// Create items
			var totalPrice = cart.SendingAmount
				- cart.ShippingPriceWithDiscount
				- cart.PriceSubtotalTax
				- (int)cart.PriceRegulationTotal
				- (int)cart.Payment.PriceExchange;

			var result = new List<PaidyCheckoutResponseItems>
			{
				CreateOrderItem(totalPrice)
			};

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

			return result.ToArray();
		}

		/// <summary>
		/// Create order item object
		/// </summary>
		/// <param name="totalPrice">Total price</param>
		/// <returns>An order item object</returns>
		private static PaidyCheckoutResponseItems CreateOrderItem(decimal totalPrice)
		{
			var orderItem = new PaidyCheckoutResponseItems(
				Constants.PAYMENT_PAIDY_CHECKOUT_REQUEST_ITEMS_ID,
				quantity: 1,
				Constants.PAYMENT_PAIDY_CHECKOUT_REQUEST_ITEMS_TITLE,
				(double)totalPrice,
				description: string.Empty);

			return orderItem;
		}
		/// <summary>
		/// Create order item object
		/// </summary>
		/// <param name="name">An order item name</param>
		/// <param name="price">A order item price</param>
		/// <returns>An order item object</returns>
		private static PaidyCheckoutResponseItems CreateOrderItem(string name, int price)
		{
			var orderItem = new PaidyCheckoutResponseItems(
				id: "xxxxx",
				quantity: 1,
				name,
				price,
				description: string.Empty);

			return orderItem;
		}

		/// <summary>
		/// Create parameter for paidy checkout
		/// </summary>
		/// <returns>Paidy checkout body</returns>
		public string CreateParameterForPaidyCheckout()
		{
			var paidyCheckoutBody = JsonConvert.SerializeObject(
				this.PostBody,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			return paidyCheckoutBody;
		}

		/// <summary>
		/// ログ出力
		/// </summary>
		/// <param name="request">リクエスト</param>
		/// <param name="response">レスポンス</param>
		public void OutputLog(string request, string response)
		{
			PaygentApiLogger.Write(
				string.Format(
					"API : PaidyCheckout\r\nRequest Param\r\n{0}\r\nResponse Param\r\n{1}",
					request,
					response));
		}

		/// <summary>Paidy checkout post body</summary>
		private PaidyCheckoutPostBody PostBody { get; set; }
	}
}
