﻿/*
=========================================================================================================
  Module      : Aftee Payment Api Facade (AfteePaymentApiFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using w2.App.Common.Global.Region.Currency;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.Domain.User.Helper;

namespace w2.App.Common.Order.Payment.Aftee
{
	/// <summary>
	/// AfteePaymentAPIのファサード
	/// </summary>
	public static class AfteePaymentApiFacade
	{
		#region Constants
		/// <summary>HTTP method: GET</summary>
		private const string m_HTTP_METHOD_GET = "GET";
		/// <summary>HTTP method: POST</summary>
		private const string m_HTTP_METHOD_POST = "POST";
		/// <summary>HTTP method: PATCH</summary>
		private const string m_HTTP_METHOD_PATCH = "PATCH";
		/// <summary>HTTP header: Content-Type</summary>
		private const string m_HTTP_HEADER_CONTENT_TYPE = "Content-Type";
		/// <summary>HTTP header: Content-Type: application/json</summary>
		private const string m_HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON = "application/json";
		/// <summary>HTTP header: Authorization</summary>
		private const string m_HTTP_HEADER_AUTHORIZATION = "Authorization";
		/// <summary>HTTP header: Terminal Id</summary>
		private const string m_HTTP_HEADER_TERMINAL_ID = "X-NP-Terminal-Id";
		/// <summary>Product Id For None Product Item</summary>
		private const string m_CONTS_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM = "xxxxx";
		#endregion

		#region Call API Methods
		/// <summary>
		/// Create a payment
		/// </summary>
		/// <param name="request">Aftee create payment request object</param>
		/// <returns>Aftee response object</returns>
		public static AfteeResponse CreatePayment(AfteeCreatePaymentRequest request)
		{
			using (var client = new WebClient())
			{
				var uri = Constants.PAYMENT_AFTEE_API_URL + "transactions";

				try
				{
					SetHttpHeaders(client);

					var requestData = JsonConvert.SerializeObject(
						request.Data,
						new JsonSerializerSettings
						{
							Formatting = Formatting.Indented,
							NullValueHandling = NullValueHandling.Ignore
						});

					WriteRequestDataLog(uri, requestData);

					// Call API
					var requestBytes = Encoding.UTF8.GetBytes(requestData);
					var responseBytes = client.UploadData(uri, m_HTTP_METHOD_POST, requestBytes);

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);

					var result = JsonConvert.DeserializeObject<AfteeResponse>(responseBody);
					WriteLogResult(result);
					return result;
				}
				catch (WebException exception)
				{
					return HandleWebException(uri, exception);
				}
			}
		}

		/// <summary>
		/// Write Log Result
		/// </summary>
		/// <param name="response">Response</param>
		public static void WriteLogResult(AfteeResponse response)
		{
			var messageWriteLog = response.IsSuccess
				? "OK: " + response.TranId
				: "NG: " + response.Message;
			FileLogger.Write("AfteeApi", messageWriteLog);
		}

		/// <summary>
		/// Capture a payment
		/// </summary>
		/// <param name="tokenId">A token ID</param>
		/// <param name="tranId">A transaction ID</param>
		/// <returns>Aftee response object</returns>
		public static AfteeResponse CapturePayment(string tokenId, string tranId)
		{
			using (var client = new WebClient())
			{
				var uri = string.Format("{0}transactions/{1}/settle", Constants.PAYMENT_AFTEE_API_URL, tranId);

				try
				{
					SetHttpHeaders(client);

					WriteRequestDataLog(uri, string.Empty);

					// Call API
					var responseBytes = client.UploadData(uri, m_HTTP_METHOD_PATCH, Encoding.UTF8.GetBytes(string.Empty));

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);

					var result = JsonConvert.DeserializeObject<AfteeResponse>(responseBody);
					WriteLogResult(result);
					return result;
				}
				catch (WebException exception)
				{
					return HandleWebException(uri, exception);
				}
			}
		}

		/// <summary>
		/// Refund a payment
		/// </summary>
		/// <param name="tranId">A transaction ID</param>
		/// <param name="request">Aftee refund request object</param>
		/// <returns>Aftee response object</returns>
		public static AfteeResponse RefundPayment(string tranId, AfteeRefundPaymentRequest request)
		{
			using (var client = new WebClient())
			{
				var uri = string.Format("{0}transactions/{1}/refund", Constants.PAYMENT_AFTEE_API_URL, tranId);

				try
				{
					SetHttpHeaders(client);

					// Convert Number
					request.AmountRefund = decimal.Parse(request.AmountRefund).ToString("0");

					var requestData = JsonConvert.SerializeObject(
						request,
						new JsonSerializerSettings
						{
							Formatting = Formatting.Indented,
							NullValueHandling = NullValueHandling.Ignore
						});

					WriteRequestDataLog(uri, requestData);

					// Call API
					var responseBytes = client.UploadData(uri, m_HTTP_METHOD_PATCH, Encoding.UTF8.GetBytes(requestData));

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					var result = JsonConvert.DeserializeObject<AfteeResponse>(responseBody);
					WriteLogResult(result);
					return result;
				}
				catch (WebException exception)
				{
					return HandleWebException(uri, exception);
				}
			}
		}

		/// <summary>
		/// Retrieving a payment
		/// </summary>
		/// <param name="tranId">Tran id</param>
		/// <returns>Aftee response object</returns>
		public static AfteeResponse GetPayment(string tranId)
		{
			using (var client = new WebClient())
			{
				var uri = string.Format("{0}transactions/{1}", Constants.PAYMENT_AFTEE_API_URL, tranId);

				try
				{
					SetHttpHeaders(client);

					WriteRequestDataLog(uri, string.Empty);

					// Call API
					var responseBytes = client.DownloadData(uri);

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					var result = JsonConvert.DeserializeObject<AfteeResponse>(
						responseBody,
						new JsonSerializerSettings
						{
							NullValueHandling = NullValueHandling.Ignore
						});
					WriteLogResult(result);
					return result;
				}
				catch (WebException exception)
				{
					return HandleWebException(uri, exception);
				}
			}
		}

		/// <summary>
		/// Get Javascript Payment Aftee
		/// </summary>
		/// <returns>Javascript</returns>
		public static string GetJavascriptPaymentAftee()
		{
			using (var client = new WebClient())
			{
				var uri = Constants.PAYMENT_AFTEE_SCRIPT_URL;
				try
				{
					// Call API
					var responseBytes = client.DownloadData(uri);

					// Handle response
					var responseBody = Encoding.UTF8.GetString(responseBytes);
					return responseBody;
				}
				catch (WebException ex)
				{
					HandleWebException(uri, ex);
					return string.Empty;
				}
			}
		}
		#endregion

		#region Helper methods
		/// <summary>
		/// Create HTTP header authorization data
		/// </summary>
		/// <returns>HTTP header authorization data</returns>
		private static string CreateHttpHeaderAuthorizationData()
		{
			var plainTextBytes = Encoding.UTF8.GetBytes(Constants.PAYMENT_AFTEE_SECRET_KEY);
			return string.Format("Basic {0}", Convert.ToBase64String(plainTextBytes));
		}

		/// <summary>
		/// Set Aftee HTTP headers
		/// </summary>
		/// <param name="client">The web client object</param>
		private static void SetHttpHeaders(WebClient client)
		{
			client.Headers.Add(m_HTTP_HEADER_CONTENT_TYPE, m_HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON);
			client.Headers.Add(m_HTTP_HEADER_AUTHORIZATION, CreateHttpHeaderAuthorizationData());
			client.Headers.Add(m_HTTP_HEADER_TERMINAL_ID, Constants.PAYMENT_AFTEE_TERMINALID);
		}

		/// <summary>
		/// Write request data log
		/// </summary>
		/// <param name="uri">URI</param>
		/// <param name="requestData">A request data</param>
		public static void WriteRequestDataLog(string uri, string requestData)
		{
			FileLogger.Write("AfteeApi", string.Format("{0}\r\n{1}", uri, requestData));
		}

		/// <summary>
		/// Get Response Error
		/// </summary>
		/// <param name="exception">A web exception</param>
		/// <returns>An error message</returns>
		private static string GetResponseError(WebException exception)
		{
			if (exception.Response == null) return null;

			using (var reader = new StreamReader(exception.Response.GetResponseStream()))
			{
				return reader.ReadToEnd();
			}
		}

		/// <summary>
		/// Handle web exception
		/// </summary>
		/// <param name="uri">URI</param>
		/// <param name="exception">A web exception</param>
		/// <returns>Aftee response object</returns>
		private static AfteeResponse HandleWebException(string uri, WebException exception)
		{
			var errorString = GetResponseError(exception);
			FileLogger.Write("AfteeApi", string.Format("{0}\r\n{1}\r\n{2}", uri, errorString, exception));

			return JsonConvert.DeserializeObject<AfteeResponse>(errorString);
		}

		/// <summary>
		/// Create Data For Atone Authorite
		/// </summary>
		/// <param name="orderNew">Order New</param>
		/// <param name="orderOld">Order Old</param>
		/// <returns>Json Data Atone String</returns>
		public static AfteeCreatePaymentRequest CreateDataAfteeAuthoriteForReturnExchange(
			OrderModel orderNew,
			OrderModel orderOld)
		{
			var request = new AfteeCreatePaymentRequest();
			request.Data = new AfteeCreatePaymentRequest.DataResponse();
			var userAttribute = UserAttributeOrderInfoCalculator.GetInstance().Calculate(orderNew.UserId);
			var isPhoneNumber = OrderCommon.CheckValidTelNoForPaymentAtoneAndAftee(orderNew.OrderPaymentKbn, orderNew.Owner.OwnerTel1);
			request.Data.DestCustomers = new List<AfteeCreatePaymentRequest.DestinationCustomer>()
			{
				new AfteeCreatePaymentRequest.DestinationCustomer
				{
					DestCustomerName = orderNew.Shippings[0].ShippingName,
					DestCustomerNameKana = orderNew.Shippings[0].ShippingNameKana,
					DestCompanyName = orderNew.Shippings[0].DeliveryCompanyName,
					DestDepartment = orderNew.Shippings[0].ShippingCompanyPostName,
					DestZipCode = orderNew.Shippings[0].ShippingZip.Replace("-", string.Empty),
					DestAddress = string.Format(
						"{0}{1}{2}{3}",
						orderNew.Shippings[0].ShippingAddr1,
						orderNew.Shippings[0].ShippingAddr2,
						orderNew.Shippings[0].ShippingAddr3,
						orderNew.Shippings[0].ShippingAddr4),
					DestTel = string.IsNullOrEmpty(orderNew.Shippings[0].ShippingTel1)
						? string.Empty
						: orderNew.Shippings[0].ShippingTel1.Replace("-", string.Empty)
				}
			};
			request.Data.Customer = new AfteeCreatePaymentRequest.SourceCustomer
			{
				CustomerName = orderNew.Owner.OwnerName,
				CustomerFamilyName = orderNew.Owner.OwnerName1,
				CustomerGivenName = orderNew.Owner.OwnerName2,
				CustomerNameKana = orderNew.Owner.OwnerNameKana,
				CustomerFamilyNameKana = orderNew.Owner.OwnerNameKana1,
				CustomerGivenNameKana = orderNew.Owner.OwnerNameKana2,
				PhoneNumber = isPhoneNumber
					? orderNew.Owner.OwnerTel1.Replace("-", string.Empty)
					: string.Empty,
				Birthday = orderNew.Owner.OwnerBirth.Value.ToString("yyyy-MM-dd"),
				SexDivision = (orderNew.Owner.OwnerSex == Constants.FLG_USER_SEX_MALE) ? "1" : "2",
				CompanyName = orderNew.Owner.OwnerCompanyName,
				Department = (orderNew.Owner.OwnerCompanyName.Length > 30)
					? orderNew.Owner.OwnerCompanyPostName.Substring(0, 30)
					: orderNew.Owner.OwnerCompanyName,
				ZipCode = orderNew.Owner.OwnerZip.Replace("-", string.Empty),
				Address = string.Format(
					"{0}{1}{2}{3}",
					orderNew.Owner.OwnerAddr1,
					orderNew.Owner.OwnerAddr2,
					orderNew.Owner.OwnerAddr3,
					orderNew.Owner.OwnerAddr4),
				Tel = isPhoneNumber == false
					? orderNew.Owner.OwnerTel1.Replace("-", string.Empty)
					: string.Empty,
				Email = orderNew.Owner.OwnerMailAddr,
				TotalPurchaseCount = userAttribute.OrderCountOrderAll.ToString(),
				TotalPurchaseAmount = userAttribute.OrderAmountOrderAll.ToString("0")
			};

			var items = new List<AfteeCreatePaymentRequest.Item>();
			var discountPrice = orderNew.MemberRankDiscountPrice
				+ orderNew.OrderCouponUse
				+ orderNew.FixedPurchaseDiscountPrice
				+ orderNew.FixedPurchaseMemberDiscountAmount;
			if ((orderNew.SetPromotions != null)
				&& (orderNew.SetPromotions.Length > 0))
			{
				var setPromotionDiscountAmount =
					orderNew.SetPromotions.Sum(item =>
						item.ShippingChargeDiscountAmount + item.ProductDiscountAmount);
				discountPrice += setPromotionDiscountAmount;
			}
			if(orderNew.IsNotReturnExchangeOrder)
			{
				foreach (var item in orderNew.Shippings[0].Items)
				{
					if (item.IsReturnItem) continue;

					items.Add(
						new AfteeCreatePaymentRequest.Item
						{
							ShopItemId = item.ProductId,
							ItemName = item.ProductName,
							ItemPrice = item.ProductPrice.ToString("0"),
							ItemCount = item.ItemQuantitySingle.ToString()
						});
				}
			}
			else
			{
				items = CreateItemsForReturnExchange(orderNew, orderOld);
			}

			// Create Shipping Fee Item
			if (orderNew.OrderPriceShipping > 0)
			{
				var shippingFeeItem = new AfteeCreatePaymentRequest.Item
				{
					ShopItemId = m_CONTS_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "配送料",
					ItemPrice = orderNew.OrderPriceShipping.ToString("0"),
					ItemCount = "1"
				};

				items.Add(shippingFeeItem);
			}

			// Create Payment Fee Item
			if (orderNew.OrderPriceExchange > 0)
			{
				var paymentFeeItem = new AfteeCreatePaymentRequest.Item
				{
					ShopItemId = m_CONTS_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "決済手数料",
					ItemPrice = "-" + orderNew.OrderPriceExchange.ToString("0"),
					ItemCount = "1"
				};
				items.Add(paymentFeeItem);
			}

			// Create Point Use Item
			if (orderNew.OrderPointUseYen > 0)
			{
				var pointUseItem = new AfteeCreatePaymentRequest.Item
				{
					ShopItemId = m_CONTS_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "ポイント利用額",
					ItemPrice = "-" + orderNew.OrderPointUseYen.ToString(),
					ItemCount = "1"
				};
				items.Add(pointUseItem);
			}

			// Create Discount Item
			if (discountPrice > 0)
			{
				var discountItem = new AfteeCreatePaymentRequest.Item
				{
					ShopItemId = m_CONTS_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "割引額",
					ItemPrice = "-" + discountPrice.ToString("0"),
					ItemCount = "1"
				};
				items.Add(discountItem);
			}

			// Convert Taiwan dollar
			items.ForEach(item => item.ItemPrice = CurrencyManager.GetSettlementAmount(
				decimal.Parse(item.ItemPrice),
				orderNew.SettlementRate,
				orderNew.SettlementCurrency).ToString("0"));

			// Check total amount order match with sum total amount of item
			var totalAmountItem = items.Sum(item =>
				int.Parse(item.ItemCount) * int.Parse(item.ItemPrice));
			var totalAmout = (int)CurrencyManager.GetSettlementAmount(
				orderNew.LastBilledAmount,
				orderNew.SettlementRate,
				orderNew.SettlementCurrency);

			if (totalAmountItem != totalAmout)
			{
				items.Add(new AfteeCreatePaymentRequest.Item
				{
					ShopItemId = m_CONTS_PRODUCT_ID_FOR_NONE_PRODUCT_ITEM,
					ItemName = "調整金額",
					ItemPrice = (totalAmout - totalAmountItem).ToString(),
					ItemCount = "1"
				});
			}

			var hasDigitalContent =
				(orderNew.DigitalContentsFlg == Constants.FLG_ORDER_DIGITAL_CONTENTS_FLG_ON);
			var transactionOptions = OrderCommon.CreateTransactionOptionsForUpdateOrder(
				orderNew.ExternalPaymentStatus,
				orderNew.IsOrderSalesSettled,
				orderNew.IsFixedPurchaseOrder,
				false,
				hasDigitalContent);

			request.Data.SalesSettled = hasDigitalContent
				? hasDigitalContent
				: (orderOld.OnlinePaymentStatus == Constants.FLG_ORDER_ONLINE_PAYMENT_STATUS_SETTLED);
			request.Data.Items = items;
			request.Data.Amount = totalAmout.ToString("0");
			request.Data.ShopTransactionNo = orderNew.OrderId;
			request.Data.TransactionOptions = transactionOptions;
			request.Data.DescriptionTrans = string.Empty;

			var returnData = JsonConvert.SerializeObject(new { data = request });
			FileLogger.WriteInfo(returnData);
			return request;
		}

		/// <summary>
		/// Create Items For Return Exchange
		/// </summary>
		/// <param name="orderNew">Order New</param>
		/// <param name="orderOld">Order Old</param>
		/// <returns>List Item</returns>
		private static List<AfteeCreatePaymentRequest.Item> CreateItemsForReturnExchange(
			OrderModel orderNew,
			OrderModel orderOld)
		{
			var result = new List<AfteeCreatePaymentRequest.Item>();

			// Get All Item From Order Original And Order Return Exchange
			var orderItemsAll = new List<OrderItemModel>();
			if (orderOld.IsNotReturnExchangeOrder)
			{
				orderItemsAll = orderOld.Shippings[0].Items.ToList();
			}
			else
			{
				var orderOrg = new OrderService().GetRelatedOrders(orderOld.OrderIdOrg);
				foreach (var order in orderOrg)
				{
					orderItemsAll.AddRange(order.Shippings[0].Items.ToList());
				}
			}

			var isExchangeItems = orderNew.Shippings[0].Items.Where(item => item.IsExchangeItem);
			var isReturnItems = orderOld.Shippings[0].Items.Where(item => item.IsReturnItem);
			var isReturnAllItems = orderItemsAll.Where(item => item.IsReturnItem);
			orderItemsAll.AddRange(isExchangeItems);

			// Get All Item Return
			var productReturn =
				orderNew.Shippings[0].Items.Where(item => item.IsReturnItem).ToList();
			productReturn.AddRange(isReturnItems);
			productReturn.AddRange(isReturnAllItems);

			// Filter Item For Call Api
			foreach (var product in orderItemsAll)
			{
				if (product.IsReturnItem) continue;
				var itemsReturn = productReturn.Where(
					item => (string.Format("{0}{1}", item.ProductId, item.VariationId)
						== string.Format("{0}{1}", product.ProductId, product.VariationId))).ToList();

				if (itemsReturn.Count > 0)
				{
					foreach (var item in itemsReturn)
					{
						product.ItemQuantity += item.ItemQuantity;
						product.ItemQuantitySingle += item.ItemQuantitySingle;
					}
				}
			}
			var orderItemsNew = orderItemsAll
				.Where(item => item.ItemQuantitySingle > 0).ToArray();

			foreach (var item in orderItemsNew)
			{
				// Merge duplicate items
				var itemAddSendToApi = result.FirstOrDefault(
					product => (product.ShopItemId == item.ProductId)
						&& (product.ItemPrice == item.ProductPrice.ToString("0")));

				if (itemAddSendToApi != null)
				{
					itemAddSendToApi.ItemCount =
						(int.Parse(itemAddSendToApi.ItemCount) + item.ItemQuantitySingle).ToString();

					continue;
				}

				result.Add(
					new AfteeCreatePaymentRequest.Item
					{
						ShopItemId = item.ProductId,
						ItemName = item.ProductName,
						ItemPrice = item.ProductPrice.ToString("0"),
						ItemCount = item.ItemQuantitySingle.ToString()
					});
			}
			return result;
		}
		#endregion
	}
}