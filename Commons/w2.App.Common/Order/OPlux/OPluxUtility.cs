/*
=========================================================================================================
  Module      : OPlux Utility(OPluxUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Order.OPlux.NameNormalization;
using w2.App.Common.Order.OPlux.RegisterEvent;
using w2.App.Common.Order.OPlux.Requests;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.User;
using static w2.App.Common.Order.Register.OrderRegisterBase;

namespace w2.App.Common.Order.OPlux
{
	/// <summary>
	/// O-PLUX utility
	/// </summary>
	public class OPluxUtility : BaseOPluxRequest
	{
		/// <summary>
		/// Create register event request
		/// </summary>
		/// <param name="cartObject">Cart object</param>
		/// <param name="nameNormalizationResponseOfOwner">Name normalization response of owner</param>
		/// <param name="nameNormalizationResponseOfShipping">Name normalization response of shipping</param>
		/// <param name="advCode">Adv code</param>
		/// <param name="orderExecType">実行タイプ</param>
		/// <returns>Register event request</returns>
		public static RegisterEventRequest CreateRegisterEventRequest(
			CartObject cartObject,
			NameNormalizationResponse.Response nameNormalizationResponseOfOwner,
			NameNormalizationResponse.Response nameNormalizationResponseOfShipping,
			string advCode,
			ExecTypes orderExecType)
		{
			var now = DateTime.Now;

			// Credit card request
			var creditCardRequest = new EventRequest.CreditCardRequest
			{
				Bincode = (cartObject.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
					? cartObject.Payment.CreditBincode
					: null,
			};

			// Pay first request
			var payFirstRequest = new EventRequest.PayFirstRequest
			{
				PayDeadlineDate = (cartObject.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF)
					? DateTime.Now.AddDays(14)
					: (DateTime?)null,
			};

			// Settle request
			var settleRequest = new EventRequest.SettleRequest
			{
				LimitPrice = OPluxConst.SETTLE_LIMIT_PRICE,
				Status = OPluxConst.SETTLE_STATUS_BEFORE_BILLING,
				Date = now,
				Amount = cartObject.PriceTotal,
				Currency = Constants.CONST_KEY_CURRENCY_CODE,
				Method = GetSettleMethod(cartObject.Payment.PaymentId),
				CreditCard = creditCardRequest,
				PayFirst = payFirstRequest,
			};

			// Address of buyer request
			var addressOfBuyerRequest = new CustomersRequest.AddressRequest
			{
				Country = Constants.COUNTRY_ISO_CODE_JP,
				Zipcode = cartObject.Owner.Zip.Replace("-", string.Empty),
				AddressA = cartObject.Owner.Addr1,
				AddressB = string.Empty,
				AddressC = string.Format(
					"{0}{1}{2}",
					cartObject.Owner.Addr2,
					cartObject.Owner.Addr3,
					cartObject.Owner.Addr4),
			};

			var mailAddr = cartObject.Owner.MailAddr.Split(
				new string[] { "@" },
				StringSplitOptions.RemoveEmptyEntries);
			var hashedAccount = (mailAddr.Length > 1)
				? CreateHashSha1(mailAddr[0])
				: null;
			var domain = (mailAddr.Length > 1)
				? mailAddr[1]
				: null;

			// Pc request
			var pcRequest = new CustomersRequest.PCRequest
			{
				HashedAccount = hashedAccount,
				Domain = domain,
			};

			// Buyer request
			var buyerRequest = new CustomersRequest.BuyerRequest
			{
				Type = OPluxConst.BUYER_TYPE,
				ID = cartObject.OrderUserId,
				HashedName = CreateHashedNameRequest(nameNormalizationResponseOfOwner, cartOwner: cartObject.Owner),
				Address = addressOfBuyerRequest,
				Tel = new CustomersRequest.TelRequest
				{
					FixedNumber = cartObject.Owner.Tel1.Replace("-", string.Empty),
				},
				Email = new CustomersRequest.EmailRequest
				{
					PC = pcRequest,
				},
				Company = new CustomersRequest.CompanyRequest
				{
					Name = advCode,
				},
			};

			// Address of shipping request
			var addressOfShippingRequest = new CustomersRequest.AddressRequest
			{
				Country = Constants.COUNTRY_ISO_CODE_JP,
				Zipcode = cartObject.GetShipping().Zip.Replace("-", string.Empty),
				AddressA = cartObject.GetShipping().Addr1,
				AddressB = string.Empty,
				AddressC = string.Format(
					"{0}{1}{2}",
					cartObject.GetShipping().Addr2,
					cartObject.GetShipping().Addr3,
					cartObject.GetShipping().Addr4),
			};

			// Shipping request
			var shippingRequest = new CustomersRequest.ShippingRequest
			{
				SpecifiedDeliverDatetime = cartObject.GetShipping().ShippingDate,
				DeliverySpecifiedExistence = (cartObject.GetShipping().ShippingDate != null)
					? OPluxConst.DELIVERY_SPECIFIED_EXISTENCE_VALID
					: OPluxConst.DELIVERY_SPECIFIED_EXISTENCE_INVALID,
				Items = CreateShippingItemsRequest(cartObject)
			};

			// Delivery request
			var deliveryRequest = new CustomersRequest.DeliveryRequest
			{
				Type = OPluxConst.DELIVERY_TYPE,
				HashedName = CreateHashedNameRequest(
					nameNormalizationResponseOfShipping,
					cartShipping: cartObject.GetShipping()),
				Address = addressOfShippingRequest,
				Tel = new CustomersRequest.TelRequest
				{
					FixedNumber = cartObject.GetShipping().Tel1.Replace("-", string.Empty),
				},
				Shipping = shippingRequest,
			};

			// Customers request
			var customersRequest = new CustomersRequest
			{
				Buyer = buyerRequest,
				Delivery = deliveryRequest,
			};

			// EC request
			var ecRequest = new EventRequest.ECRequest
			{
				BuyDatetime = now,
				Settle = settleRequest,
				Customers = customersRequest,
				Tenant = new EventRequest.TenantRequest
				{
					TenantName = Constants.SITE_DOMAIN,
				},
			};

			var user = new UserService().Get(cartObject.OrderUserId);
			var fraudbusterCookie = CookieManager.Get(Constants.COOKIE_KEY_FRAUDBUSTER);

			// Event request
			var eventRequest = new EventRequest
			{
				ModelId = Constants.OPLUX_REQUEST_EVENT_MODEL_ID,
				EventIdForShop = cartObject.OrderId,
				EventType = OPluxConst.EVENT_TYPE_EC,
				IpAddressOnly = (user != null)
					? user.RemoteAddr
					: null,
				DeviceInfo = cartObject.DeviceInfo,
				CookieOnly = ((fraudbusterCookie != null) && (orderExecType != ExecTypes.CommerceManager))
					? fraudbusterCookie.Value
					: null,
				EC = ecRequest,
			};

			// Register event request
			var registerEventRequest = new RegisterEventRequest
			{
				Version = OPluxConst.VERSION_DEFAULT,
				ShopId = Constants.OPLUX_REQUEST_SHOP_ID,
				RequestDatetime = now,
				Signiture = CreateHashForSignature(now),
				HashMethod = OPluxConst.HASH_METHOD_SHA256,
				Event = eventRequest,
			};

			return registerEventRequest;
		}

		/// <summary>
		/// Create hashed name request
		/// </summary>
		/// <param name="nameNormalizationResponse">Name normalization response</param>
		/// <param name="cartOwner">Cart owner</param>
		/// <param name="cartShipping">Cart shipping</param>
		/// <returns>Hashed name request</returns>
		private static CustomersRequest.HashedNameRequest CreateHashedNameRequest(
			NameNormalizationResponse.Response nameNormalizationResponse,
			CartOwner cartOwner = null,
			CartShipping cartShipping = null)
		{
			var hashFirstName = CreateHashSha1((cartOwner != null)
				? StringUtility.ToEmpty(cartOwner.Name2)
				: (cartShipping != null)
					? StringUtility.ToEmpty(cartShipping.Name2)
					: string.Empty);

			var normalizedFirstName = nameNormalizationResponse.FirstName.Writing;
			var hashNormalizedFirstName = (string.IsNullOrEmpty(normalizedFirstName) == false)
				? CreateHashSha1(normalizedFirstName)
				: hashFirstName;

			var hashLastName = CreateHashSha1((cartOwner != null)
				? StringUtility.ToEmpty(cartOwner.Name1)
				: (cartShipping != null)
					? StringUtility.ToEmpty(cartShipping.Name1)
					: string.Empty);

			var normalizedLastName = nameNormalizationResponse.LastName.Writing;
			var hashNormalizedLastName = (string.IsNullOrEmpty(normalizedLastName) == false)
				? CreateHashSha1(normalizedLastName)
				: hashLastName;

			var hashedNameRequest = new CustomersRequest.HashedNameRequest
			{
				FirstName = hashFirstName,
				LastName = hashLastName,
				NormalizedFirstName = hashNormalizedFirstName,
				NormalizedLastName = hashNormalizedLastName,
				NameLength = nameNormalizationResponse.LetterCount.NameLength,
				KanjiCountInName = nameNormalizationResponse.LetterCount.KanjiCountInName,
				HiraganaCountInName = nameNormalizationResponse.LetterCount.HiraganaCountInName,
				KatakanaCountInName = nameNormalizationResponse.LetterCount.KatakanaCountInName,
				AlphabetCountInName = nameNormalizationResponse.LetterCount.AlphabetCountInName,
				OtherCountInName = nameNormalizationResponse.LetterCount.OtherCountInName,
				ValidName = OPluxConst.HASHED_NAME_VALID_NAME_EXIST,
			};

			return hashedNameRequest;
		}

		/// <summary>
		/// Create shipping items request
		/// </summary>
		/// <param name="cartObject">Cart object</param>
		/// <returns>Shipping item request</returns>
		private static CustomersRequest.ShippingItemRequest[] CreateShippingItemsRequest(CartObject cartObject)
		{
			var shippingItemRequests = new List<CustomersRequest.ShippingItemRequest>();
			foreach (var product in cartObject.Items)
			{
				var shippingItemModel = new CustomersRequest.ShippingItemRequest
				{
					ShopItemId = product.ProductId,
					ItemPrice = product.Price,
					ItemQuantity = product.Count,
					ItemName = product.ProductName,
					ItemCategory = product.CategoryId1,
				};

				shippingItemRequests.Add(shippingItemModel);
			}

			return shippingItemRequests.ToArray();
		}

		/// <summary>
		/// Get settle method
		/// </summary>
		/// <param name="paymentId">Payment id</param>
		/// <returns>Settle method</returns>
		public static string GetSettleMethod(string paymentId)
		{
			switch (paymentId)
			{
				case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF:
				case Constants.FLG_PAYMENT_PAYMENT_ID_SMS_DEF:
				case Constants.FLG_PAYMENT_PAYMENT_ID_BANK_DEF:
				case Constants.FLG_PAYMENT_PAYMENT_ID_POST_DEF:
				case Constants.FLG_PAYMENT_PAYMENT_ID_DSK_DEF:
					return OPluxConst.SETTLE_METHOD_POSTPAY;

				case Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT:
					return OPluxConst.SETTLE_METHOD_CREDIT_CARD_PAYMENT;

				case Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT:
					return OPluxConst.SETTLE_METHOD_CASH_ON_DELIVERY;

				case Constants.FLG_PAYMENT_PAYMENT_ID_CVS_PRE:
				case Constants.FLG_PAYMENT_PAYMENT_ID_BANK_PRE:
				case Constants.FLG_PAYMENT_PAYMENT_ID_POST_PRE:
				case Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY:
				case Constants.FLG_PAYMENT_PAYMENT_ID_NP_AFTERPAY:
					return OPluxConst.SETTLE_METHOD_PREPAID;

				case Constants.FLG_PAYMENT_PAYMENT_ID_PAYPAL:
					return OPluxConst.SETTLE_METHOD_PAYPAL;

				default:
					return OPluxConst.SETTLE_METHOD_OTHERS;
			}
		}
	}
}
