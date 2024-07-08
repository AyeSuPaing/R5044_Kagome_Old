/*
=========================================================================================================
  Module      : EC Pay Utility(ECPayUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.ShopMessage;
using w2.App.Common.Web.Process;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Web;
using w2.Domain.Order;
using w2.Domain.Order.Helper;
using w2.Domain.ShopShipping;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.Payment.ECPay
{
	/// <summary>
	/// EC Pay Utility Class
	/// </summary>
	public class ECPayUtility
	{
		// 文字変換用マッピングテーブル //
		const string HANKAKU_SYMBOL_MAP = "^'`!@#%&*+\\\"<>｜_[]";	//半角記号
		const string ZENKAKU_SYMBOL_MAP = "＾’｀！＠＃％＆＊＋＼”＜＞｜＿［］";	//全角記号

		#region +Method Convenience Store
		/// <summary>
		/// Create Request Data For Send Order
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="dontNotifyUpdate">フロントに通知を送信しないフラグ</param>
		/// <returns>Request Data</returns>
		public static ECPayConvenienceStoreRequest CreateRequestDataForSendOrder(
			OrderModel order,
			bool dontNotifyUpdate)
		{
			var orderShipping = order.Shippings[0];
			var isReceivingStore = (orderShipping.DeliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID);
			var isCollection = GetIsCollection(orderShipping.ShippingReceivingStoreType);
			var logisticsSubType = GetLogisticsSubType(order);
			var receiverZipCode = orderShipping.ShippingZip;
			var receiverAddress = string.Format("{0}{1}{2}{3}",
				orderShipping.ShippingAddr2,
				orderShipping.ShippingAddr3,
				orderShipping.ShippingAddr4,
				orderShipping.ShippingAddr5);
			var senderAddress = Constants.RECEIVINGSTORE_TWECPAY_SENDNAME[4];
			var temperature = GetTemperature(order, logisticsSubType);
			var request = new ECPayConvenienceStoreRequest
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				SenderName = Constants.RECEIVINGSTORE_TWECPAY_SENDNAME[0],
				SenderPhone = Constants.RECEIVINGSTORE_TWECPAY_SENDNAME[1],
				SenderCellPhone = Constants.RECEIVINGSTORE_TWECPAY_SENDNAME[2],
				PlatformId = Constants.PAYMENT_ECPAY_SPECIAL_MERCHANTFLAG
					? Constants.PAYMENT_ECPAY_MERCHANTID
					: string.Empty,
				MerchantTradeNo = order.OrderId,
				MerchantTradeDate = (order.OrderDate != null)
					? order.OrderDate.Value.ToString(ECPayConstants.CONST_FORMAT_DATE_API)
					: null,
				LogisticsType = isReceivingStore
					? Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_TYPE_CVS
					: Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_TYPE_HOME,
				LogisticsSubType = logisticsSubType,
				GoodsAmount = order.SettlementAmount.ToString("0"),
				IsCollection = isCollection,
				CollectionAmount = (isCollection == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON)
					? order.SettlementAmount.ToString("0")
					: "0",
				GoodsName = GetGoodsName(orderShipping.Items, " "),
				ReceiverName = isReceivingStore
					? StringUtility.ToHankaku(order.Owner.OwnerName)
					: StringUtility.ToHankaku(orderShipping.ShippingName),
				ReceiverPhone = isReceivingStore
					? string.Empty
					: orderShipping.ShippingTel1,
				ReceiverCellPhone = isReceivingStore
					? order.Owner.OwnerTel1.Replace("-", string.Empty)
					: string.Empty,
				ReceiverEmail = order.Owner.OwnerMailAddr,
				ServerReplyUrl = CreateServerReplyUrl(order.OrderId, false, dontNotify: true),
				LogisticsC2CReplyUrl = (logisticsSubType == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_7_ELEVEN_C2C)
					? ECPayConstants.SERVER_LOGISTICS_C2C_REPLY_URL
					: string.Empty,
				ReceiverStoreId = isReceivingStore
					? orderShipping.ShippingReceivingStoreId
					: null,
				SenderZipCode = (isReceivingStore == false)
					? Constants.RECEIVINGSTORE_TWECPAY_SENDNAME[3]
					: null,
				SenderAddress = (isReceivingStore == false)
					? senderAddress
					: null,
				ReceiverZipCode = (isReceivingStore == false)
					? receiverZipCode
					: null,
				ReceiverAddress = (isReceivingStore == false)
					? receiverAddress
					: null,
				Temperature = (isReceivingStore == false)
					? temperature
					: null,
				Distance = (isReceivingStore == false)
					? GetDistance(
						order,
						receiverZipCode,
						receiverAddress,
						senderAddress)
					: null,
				Specification = (isReceivingStore == false)
					? GetSpecification(order, temperature)
					: null,
			};

			// Add check sum data
			var urlParameter = CreateUrlParameters(request);
			request.CheckMacValue = CreateCheckSumValue(urlParameter);
			return request;
		}

		/// <summary>
		/// Create Request Data For Update Order
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Request Data</returns>
		public static ECPayConvenienceStoreRequest CreateRequestDataForUpdateOrder(OrderModel order)
		{
			var orderShipping = order.Shippings[0];
			var shippingCheckNo = orderShipping.ShippingCheckNo;
			var paymentNo = shippingCheckNo;
			var validationNo = shippingCheckNo;
			if (CheckLogisticsSubType7Eleven(orderShipping.ShippingReceivingStoreType)
				&& (shippingCheckNo.Length > 4))
			{
				paymentNo = shippingCheckNo.Substring(0, (shippingCheckNo.Length - 4));
				validationNo = shippingCheckNo.Substring(shippingCheckNo.Length - 4);
			}

			var request = new ECPayConvenienceStoreRequest
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				AllPayLogisticsId = order.DeliveryTranId,
				CvsPaymentNo = paymentNo,
				CvsValidationNo = validationNo,
				ReceiverStoreId = orderShipping.ShippingReceivingStoreId,
				StoreType = ECPayConstants.CONST_DEFAULT_STORE_TYPE,
				PlatformId = Constants.PAYMENT_ECPAY_SPECIAL_MERCHANTFLAG
					? Constants.PAYMENT_ECPAY_MERCHANTID
					: string.Empty
			};

			// Add check sum data
			var urlParameter = CreateUrlParameters(request);
			request.CheckMacValue = CreateCheckSumValue(urlParameter);
			return request;
		}

		/// <summary>
		/// Create Request Data For Send Return At Convenience Store
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="deliveryTranId">Delivery Tran Id</param>
		/// <returns>EC Pay Request</returns>
		public static ECPayConvenienceStoreRequest CreateRequestDataForSendReturnAtConvenienceStore(
			OrderModel order,
			string deliveryTranId)
		{
			var orderShippingItems = order.Shippings[0].Items;
			var request = new ECPayConvenienceStoreRequest
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				AllPayLogisticsId = deliveryTranId,
				ServerReplyUrl = CreateServerReplyUrl(order.OrderId, false),
				GoodsName = GetGoodsName(orderShippingItems, "#"),
				GoodsAmount = GetGoodsAmount(orderShippingItems),
				CollectionAmount = "0",
				ServiceType = ECPayConstants.CONST_DEFAULT_SERVICE_TYPE,
				SenderName = StringUtility.ToHankaku(order.Owner.OwnerName),
				SenderPhone = order.Owner.OwnerTel1,
				PlatformId = Constants.PAYMENT_ECPAY_SPECIAL_MERCHANTFLAG
					? Constants.PAYMENT_ECPAY_MERCHANTID
					: string.Empty
			};

			// Add check sum data
			var urlParameter = CreateUrlParameters(request);
			request.CheckMacValue = CreateCheckSumValue(urlParameter);
			return request;
		}

		/// <summary>
		/// Create Request Data For Order Return Home
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="deliveryTranId">Delivery Tran Id</param>
		/// <returns>EC Pay Request</returns>
		public static ECPayConvenienceStoreRequest CreateRequestDataForOrderReturnHome(
			OrderModel order,
			string deliveryTranId)
		{
			var logisticsSubType = GetLogisticsSubType(order);
			var orderShipping = order.Shippings[0];
			var senderAddress = string.Format("{0}{1}{2}{3}",
				orderShipping.ShippingAddr2,
				orderShipping.ShippingAddr3,
				orderShipping.ShippingAddr4,
				orderShipping.ShippingAddr5);
			var receiverZipCode = Constants.RECEIVINGSTORE_TWECPAY_SENDNAME[3];
			var receiverAddress = Constants.RECEIVINGSTORE_TWECPAY_SENDNAME[4];
			var temperature = GetTemperature(order, logisticsSubType);
			var orderShippingItems = orderShipping.Items;
			var request = new ECPayConvenienceStoreRequest
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				AllPayLogisticsId = deliveryTranId,
				LogisticsSubType = logisticsSubType,
				ServerReplyUrl = CreateServerReplyUrl(order.OrderId, false),
				SenderName = StringUtility.ToHankaku(orderShipping.ShippingName),
				SenderPhone = orderShipping.ShippingTel1,
				SenderCellPhone = string.Empty,
				SenderZipCode = orderShipping.ShippingZip,
				SenderAddress = senderAddress,
				ReceiverName = Constants.RECEIVINGSTORE_TWECPAY_SENDNAME[0],
				ReceiverPhone = Constants.RECEIVINGSTORE_TWECPAY_SENDNAME[1],
				ReceiverCellPhone = Constants.RECEIVINGSTORE_TWECPAY_SENDNAME[2],
				ReceiverZipCode = receiverZipCode,
				ReceiverAddress = receiverAddress,
				ReceiverEmail = Constants.RECEIVINGSTORE_TWECPAY_SENDNAME[5],
				GoodsAmount = GetGoodsAmount(orderShippingItems),
				GoodsName = GetGoodsName(orderShippingItems, " "),
				Temperature = temperature,
				Distance = GetDistance(
					order,
					receiverZipCode,
					receiverAddress,
					senderAddress),
				Specification = GetSpecification(order, temperature),
				PlatformId = Constants.PAYMENT_ECPAY_SPECIAL_MERCHANTFLAG
					? Constants.PAYMENT_ECPAY_MERCHANTID
					: string.Empty
			};

			// Add check sum data
			var urlParameter = CreateUrlParameters(request);
			request.CheckMacValue = CreateCheckSumValue(urlParameter);
			return request;
		}

		/// <summary>
		/// Get Distance
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="receiverZipCode">Receiver Zip Code</param>
		/// <param name="receiverAddress">Receiver Address</param>
		/// <param name="senderAddress">Sender Address</param>
		/// <returns>Distance</returns>
		private static string GetDistance(
			OrderModel order,
			string receiverZipCode,
			string receiverAddress,
			string senderAddress)
		{
			var shippingInfo = new ShopShippingService().Get(order.ShopId, order.ShippingId);
			if ((shippingInfo != null)
				&& (shippingInfo.ZoneList.Length > 0)
				&& (shippingInfo.ZoneList.Any(item => item.Zip.Split(',').Contains(receiverZipCode))))
			{
				return ECPayConstants.CONST_DISTANCE_REMOTE_ISLAND;
			}

			if ((receiverAddress.Length >= 3)
				&& (senderAddress.Length >= 3)
				&& (receiverAddress.Substring(0, 3) == senderAddress.Substring(0, 3)))
			{
				return ECPayConstants.CONST_DISTANCE_SAME_CITY;
			}
			return ECPayConstants.CONST_DISTANCE_DIFFERENT_CITY;
		}

		/// <summary>
		/// Get Goods Amount
		/// </summary>
		/// <param name="orderItems">Order Items</param>
		/// <returns>Goods Amount</returns>
		private static string GetGoodsAmount(OrderItemModel[] orderItems)
		{
			var amount = orderItems.Select(product => product.ProductPrice).Sum();
			var result = CurrencyManager.ConvertPrice(amount);
			return result.ToString("0");
		}

		/// <summary>
		/// Get Goods Name
		/// </summary>
		/// <param name="orderItems">Order Items</param>
		/// <param name="separator">Separator</param>
		/// <returns>Goods Name</returns>
		private static string GetGoodsName(OrderItemModel[] orderItems, string separator)
		{
			//ノベルティ、同梱物を除く1レコード目の商品のみ連携
			var item = orderItems.OrderBy(product => product.OrderItemNo)
				.Where(product => ((string.IsNullOrEmpty(product.NoveltyId)) && (string.IsNullOrEmpty(product.ProductBundleId)))).ToArray().First();
			var source = (string.IsNullOrEmpty(Constants.PRODUCT_FIXED_PURCHASE_STRING)) ? item.ProductName : item.ProductName.Replace(Constants.PRODUCT_FIXED_PURCHASE_STRING, string.Empty);
			var result = ToOtherCharactorMap(string.Join(separator, source), HANKAKU_SYMBOL_MAP, ZENKAKU_SYMBOL_MAP);

			//26文字以上の場合切り捨て
			return (result.Length >= 25) ? result.Substring(0, 25) : result;
		}

		/// <summary>
		/// 半角全角記号変換
		/// </summary>
		/// <param name="source">対象文字列</param>
		/// <param name="beforeMap">変換前キャラクタマップ</param>
		/// <param name="afterMap">変換後キャラクタマップ</param>
		/// <returns>変換後文字列</returns>
		private static string ToOtherCharactorMap(string source, string beforeMap, string afterMap)
		{
			var sourceResult = new StringBuilder(source);
			for (var i = 0; i < beforeMap.Length; i++)
			{
				sourceResult.Replace(beforeMap[i], afterMap[i]);
			}

			return sourceResult.ToString();
		}

		/// <summary>
		/// Create Script For Get Invoice Order
		/// </summary>
		/// <param name="deliveryTranIdList">Delivery Tran Id List</param>
		/// <returns>Script</returns>
		public static string CreateScriptForGetInvoiceOrder(string[] deliveryTranIdList)
		{
			var request = new ECPayConvenienceStoreRequest
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				AllPayLogisticsId = string.Join(",", deliveryTranIdList),
				PlatformId = Constants.PAYMENT_ECPAY_SPECIAL_MERCHANTFLAG
					? Constants.PAYMENT_ECPAY_MERCHANTID
					: string.Empty,
			};

			// Add check sum data
			var urlParameter = CreateUrlParameters(request);
			request.CheckMacValue = CreateCheckSumValue(urlParameter);

			var parameters = request.GetParameters();
			var clientScript = string.Format("open_window_with_post('{0},'webECPayInvoice',['{1}'],['{2}'],'width=1000,height=800,top=120,left=320')",
				string.Format("{0}helper/printTradeDocument'", Constants.RECEIVINGSTORE_TWECPAY_APIURL),
				string.Join("','", parameters.Select(item => item.Key)),
				string.Join("','", parameters.Select(item => HttpUtility.UrlEncode(item.Value))));
			return clientScript;
		}

		/// <summary>
		/// Create Script For Download Payment Slip
		/// </summary>
		/// <param name="deliveryTranIdList">Delivery Tran Id List</param>
		/// <param name="shippingCheckNo">Shipping Check No</param>
		/// <param name="shippingReceivingStoreType">Shipping Receiving Store Type</param>
		/// <returns>Script</returns>
		public static string CreateScriptForDownloadPaymentSlip(
			string deliveryTranIdList,
			string shippingCheckNo,
			string shippingReceivingStoreType)
		{
			var is7ElevenC2C = CheckShippingReceivingStoreType7Eleven(shippingReceivingStoreType);
			var paymentNo = ((shippingCheckNo.Length > 4) && is7ElevenC2C)
				? shippingCheckNo.Substring(0, (shippingCheckNo.Length - 4))
				: shippingCheckNo;
			var validationNo = ((shippingCheckNo.Length > 4) && is7ElevenC2C)
				? shippingCheckNo.Substring(shippingCheckNo.Length - 4)
				: string.Empty;

			var request = new ECPayConvenienceStoreRequest
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				AllPayLogisticsId = string.Join(",", deliveryTranIdList),
				PlatformId = Constants.PAYMENT_ECPAY_SPECIAL_MERCHANTFLAG
					? Constants.PAYMENT_ECPAY_MERCHANTID
					: string.Empty,
				CvsPaymentNo = paymentNo,
				CvsValidationNo = validationNo
			};

			// Add check sum data
			var urlParameter = CreateUrlParameters(request);
			request.CheckMacValue = CreateCheckSumValue(urlParameter);

			var url = CreateUrl(shippingReceivingStoreType);
			var parameters = request.GetParameters();
			var clientScript = string.Format("open_window_with_post('{0},'webECPayInvoice',['{1}'],['{2}'],'width=1000,height=800,top=120,left=320')",
				string.Format("{0}{1}'",
				Constants.RECEIVINGSTORE_TWECPAY_APIURL,
				url),
				string.Join("','", parameters.Select(item => item.Key)),
				string.Join("','", parameters.Select(item => HttpUtility.UrlEncode(item.Value))));
			return clientScript;
		}

		/// <summary>
		/// Create Script For Get Invoice Order
		/// </summary>
		/// <param name="shippingReceivingStoreType">Shipping Receiving Store Type</param>
		/// <param name="isSmartPhone">Is Smart Phone</param>
		/// <returns>Script</returns>
		public static string CreateScriptForOpenConvenienceStoreMap(
			string shippingReceivingStoreType,
			bool isSmartPhone = false)
		{
			var serverReplyUrl = CreateServerReplyUrl(string.Empty, true);
			var parameters = CreateParametersForOpenConvenienceStoreMap(
				shippingReceivingStoreType,
				serverReplyUrl,
				isSmartPhone);
			var clientScript = string.Format("open_window_with_post('{0},'ECPayMap',['{1}'],['{2}'],'width=1000,height=800,top=120,left=320')",
				string.Format("{0}Express/map'", Constants.RECEIVINGSTORE_TWECPAY_APIURL),
				string.Join("','", parameters.Select(item => item.Key)),
				string.Join("','", parameters.Select(item => HttpUtility.UrlEncode(item.Value))));
			return clientScript;
		}

		/// <summary>
		/// Create Request Data For Get Invoice Order
		/// </summary>
		/// <param name="deliveryTranIdList">Delivery Tran Id List</param>
		/// <returns>Request Data</returns>
		public static ECPayConvenienceStoreRequest CreateRequestDataForGetInvoiceOrder(string[] deliveryTranIdList)
		{
			var request = new ECPayConvenienceStoreRequest
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				AllPayLogisticsId = string.Join(",", deliveryTranIdList),
				PlatformId = Constants.PAYMENT_ECPAY_SPECIAL_MERCHANTFLAG
					? Constants.PAYMENT_ECPAY_MERCHANTID
					: string.Empty,
			};

			// Add check sum data
			var urlParameter = CreateUrlParameters(request);
			request.CheckMacValue = CreateCheckSumValue(urlParameter);
			return request;
		}

		/// <summary>
		/// Get Specification
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="temperature">Temperature</param>
		/// <returns>Specification</returns>
		public static string GetSpecification(OrderModel order, string temperature)
		{
			var sumSize = 0;
			foreach (var data in order.Shippings[0].Items)
			{
				switch (data.ShippingSizeKbn)
				{
					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XXS:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_XXS * data.ItemQuantity;
						break;

					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XS:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_XS * data.ItemQuantity;
						break;

					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_S:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_S * data.ItemQuantity;
						break;

					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_M:
					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_L:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_ML * data.ItemQuantity;
						break;

					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XL:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_XL * data.ItemQuantity;
						break;

					case Constants.FLG_PRODUCT_SHIPPING_SIZE_KBN_XXL:
						sumSize += Constants.FLG_SHIPPING_SIZE_KBN_XXL * data.ItemQuantity;
						break;

					default:
						continue;
				}
			}

			var result = GetSpecificationSize(sumSize, temperature);
			return result;
		}

		/// <summary>
		/// Get Specification Size
		/// </summary>
		/// <param name="sumSize">Sum size</param>
		/// <param name="temperature">Temperature</param>
		/// <returns>Specification Size</returns>
		public static string GetSpecificationSize(int sumSize, string temperature)
		{
			if (sumSize > ECPayConstants.CONST_SPECIFICATION_SIZE_BIG) return string.Empty;
			if ((sumSize > ECPayConstants.CONST_SPECIFICATION_SIZE_MEDIUM)
				&& (temperature != ECPayConstants.CONST_TEMPERATURE_FREEZING))
			{
				return ECPayConstants.CONST_SPECIFICATION_BIG;
			}
			if (sumSize > ECPayConstants.CONST_SPECIFICATION_SIZE_NORMAL) return ECPayConstants.CONST_SPECIFICATION_MEDIUM;
			if (sumSize > ECPayConstants.CONST_SPECIFICATION_SIZE_SMALL) return ECPayConstants.CONST_SPECIFICATION_NORMAL;
			return ECPayConstants.CONST_SPECIFICATION_SMALL;
		}

		/// <summary>
		/// Get Logistics Sub Type
		/// </summary>
		/// <param name="order">Order</param>
		/// <returns>Logistics Sub Type</returns>
		public static string GetLogisticsSubType(OrderModel order)
		{
			var orderShipping = order.Shippings[0];
			var deliveryCompanyId = orderShipping.DeliveryCompanyId;
			var shippingType = orderShipping.ShippingReceivingStoreType;
			var logisticsSubType = string.Empty;
			if (deliveryCompanyId == Constants.TWPELICANEXPRESS_CONVENIENCE_STORE_ID)
			{
				logisticsSubType = GetLogisticsSubType(shippingType);
			}
			else if (Constants.SHIPPING_SERVICE_YAMATO_FOR_ECPAY.Contains(deliveryCompanyId))
			{
				logisticsSubType = Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_YAMATO_HOME_DELIVERY;
			}
			else if (Constants.SHIPPING_SERVICE_HOME_DELIVERY_FOR_ECPAY.Contains(deliveryCompanyId))
			{
				logisticsSubType = Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_HOME_DELIVERY;
			}
			return logisticsSubType;
		}

		/// <summary>
		/// Get Temperature
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="logisticSubType">Logistic Sub Type</param>
		/// <returns>Temperature</returns>
		public static string GetTemperature(OrderModel order, string logisticSubType)
		{
			var temperature = string.Empty;
			if (logisticSubType == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_HOME_DELIVERY)
			{
				temperature = ECPayConstants.CONST_TEMPERATURE_NORMAL;
			}
			else if (logisticSubType == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_YAMATO_HOME_DELIVERY)
			{
				var deliveryCompanyId = order.Shippings[0].DeliveryCompanyId;
				switch (deliveryCompanyId)
				{
					case ECPayConstants.CONST_SHIPPING_SERVICE_ID_YAMATO_TEPID:
						temperature = ECPayConstants.CONST_TEMPERATURE_NORMAL;
						break;

					case ECPayConstants.CONST_SHIPPING_SERVICE_ID_YAMATO_REFRIGERATED:
						temperature = ECPayConstants.CONST_TEMPERATURE_REFRIGERATED;
						break;

					case ECPayConstants.CONST_SHIPPING_SERVICE_ID_YAMATO_FROZEN:
						temperature = ECPayConstants.CONST_TEMPERATURE_FREEZING;
						break;
				}
			}
			return temperature;
		}

		/// <summary>
		/// Is Shipping Service Of EC Pay
		/// </summary>
		/// <param name="deliveryCompanyId">Delivery Company Id</param>
		/// <returns>True: This is shipping service of EC Pay, otherwise: false</returns>
		public static bool IsShippingServiceOfECPay(string deliveryCompanyId)
		{
			var result = (Constants.SHIPPING_SERVICE_YAMATO_FOR_ECPAY.Contains(deliveryCompanyId)
				|| Constants.SHIPPING_SERVICE_HOME_DELIVERY_FOR_ECPAY.Contains(deliveryCompanyId));
			return result;
		}

		/// <summary>
		/// Create Check Sum Value
		/// </summary>
		/// <param name="urlParameters">Url Parameters</param>
		/// <returns>Check Sum Value</returns>
		public static string CreateCheckSumValue(string urlParameters)
		{
			var hashKey = Constants.PAYMENT_ECPAY_HASHKEY[0];
			var hashIv = (Constants.PAYMENT_ECPAY_HASHKEY.Length > 1)
				? Constants.PAYMENT_ECPAY_HASHKEY[1]
				: string.Empty;
			urlParameters = string.Format("{0}={1}&{2}&{3}={4}",
				ECPayConstants.CONST_REQUEST_KEY_HASH_KEY,
				hashKey,
				urlParameters,
				ECPayConstants.CONST_REQUEST_KEY_HASH_IV,
				hashIv);

			// Encoding url and convert to lower characters
			var encodedUrl = HttpUtility.UrlEncode(urlParameters).ToLower();
			var checkSum = new StringBuilder();
			using (var md5Hash = MD5.Create())
			{
				var hashBytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(encodedUrl));
				foreach (var byteData in hashBytes)
				{
					checkSum.Append(byteData.ToString("x2"));
				}
			}

			return checkSum.ToString().ToUpper();
		}

		/// <summary>
		/// Create Url Parameters
		/// </summary>
		/// <param name="request">Request data</param>
		/// <returns>Url Parameters</returns>
		public static string CreateUrlParameters(ECPayConvenienceStoreRequest request)
		{
			var parameters = request.CreateParameters();
			var urlParameters = string.Join("&", parameters.Select(item =>
				string.Format("{0}={1}", item.Key, item.Value)));
			return urlParameters;
		}

		/// <summary>
		/// Create Log Message For Send Order Register
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="deliveryTranId">Delivery Tran Id</param>
		/// <param name="returnCode">Return Code</param>
		/// <param name="returnMessage">Return Message</param>
		/// <returns>Log Message</returns>
		public static string CreateLogMessageForSendOrderRegister(
			string orderId,
			string deliveryTranId,
			string returnCode,
			string returnMessage)
		{
			var errorLog = string.Format("注文ID：{0}、ECPayの物流取引ID：{1}、物流状態コード：{2}、物流状態メッセージ ：{3}",
				orderId,
				deliveryTranId,
				returnCode,
				returnMessage);
			return errorLog;
		}

		/// <summary>
		/// Create Log Message For Send Order Return CVS
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="returnMerchantTradeNo">Return Merchant Trade No</param>
		/// <param name="returnCode">Return Code</param>
		/// <param name="isFamilyMart">Is Family Mart</param>
		/// <returns>Log Message</returns>
		public static string CreateLogMessageForSendOrderReturnCVS(
			string orderId,
			string returnMerchantTradeNo,
			string returnCode,
			bool isFamilyMart)
		{
			var logMessage = string.Format("注文ID：{0}、返品物流取引ID：{1}、{2}：{3}",
				orderId,
				returnMerchantTradeNo,
				isFamilyMart
					? "返品番号Famiport Key"
					: "返品番号7-ELEVEN iBon",
				returnCode);
			return logMessage;
		}

		/// <summary>
		/// Create Log Message For Send Order Return Home
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="deliveryTranId">Delivery Tran Id</param>
		/// <param name="errorMessage">Error Message</param>
		/// <param name="isSuccess">Is Success</param>
		/// <returns>Log Message</returns>
		public static string CreateLogMessageForSendOrderReturnHome(
			string orderId,
			string deliveryTranId,
			string errorMessage,
			bool isSuccess)
		{
			var logMessage = string.Format("注文ID：{0}、物流取引ID：{1}、{2}",
				orderId,
				deliveryTranId,
				isSuccess
					? "宅配返品処理成功"
					: "宅配返品処理失敗：" + errorMessage);
			return logMessage;
		}

		/// <summary>
		/// Create Message
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="errorMessage">Error Message</param>
		/// <returns>Message</returns>
		public static string CreateMessage(
			string orderId,
			string errorMessage)
		{
			var message = string.Format("{0}：{1}",
				orderId,
				errorMessage);
			return message;
		}

		/// <summary>
		/// Create Relation Memo
		/// </summary>
		/// <param name="returnCode">Return Code</param>
		/// <param name="returnMessage">Return Message</param>
		/// <param name="returnOrderNo">Return Order No</param>
		/// <param name="isReturnOrder">Is Return Order</param>
		/// <param name="isReturnAtFamilyMart">Is Return At Family Mart</param>
		/// <returns>Memo</returns>
		public static string CreateRelationMemo(
			string returnCode,
			string returnMessage,
			string returnOrderNo,
			bool isReturnOrder = false,
			bool isReturnAtFamilyMart = false)
		{
			if (isReturnOrder)
			{
				return string.Format("{0}：{1}",
					isReturnAtFamilyMart
						? "返品番号Famiport Key"
						: "返品番号7-ELEVEN iBon",
					returnOrderNo);
			}
			return string.Format("{0}：{1}",
				returnCode,
				returnMessage);
		}

		/// <summary>
		/// Send Mail Error
		/// </summary>
		/// <param name="message">Message</param>
		public static void SendMailError(string message)
		{
			var input = new Hashtable
			{
				{ "message", message }
			};

			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_EXTERNAL_ORDER_INFO_FOR_OPERATOR,
				string.Empty,
				input))
			{
				// メールテンプレートでTOが指定されていない場合、メール送信しない
				if (string.IsNullOrEmpty(mailSender.TmpTo)) return;

				if (mailSender.SendMail() == false)
				{
					FileLogger.WriteError(mailSender.MailSendException);
				}
			}
		}

		/// <summary>
		/// Create Parameters For Open Convenience Store Map
		/// </summary>
		/// <param name="shippingReceivingStoreType">Shipping Receiving Store Type</param>
		/// <param name="serverReplyUrl">Server Reply Url</param>
		/// <param name="isSmartPhone">Is Smart Phone</param>
		/// <returns>Parameters</returns>
		public static Dictionary<string, string> CreateParametersForOpenConvenienceStoreMap(
			string shippingReceivingStoreType,
			string serverReplyUrl,
			bool isSmartPhone = false)
		{
			var parameters = new Dictionary<string, string>
			{
				{ ECPayConstants.PARAM_MERCHANT_ID, Constants.PAYMENT_ECPAY_MERCHANTID },
				{ ECPayConstants.PARAM_LOGISTICS_TYPE, Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_TYPE_CVS },
				{ ECPayConstants.PARAM_LOGISTICS_SUB_TYPE, GetLogisticsSubType(shippingReceivingStoreType) },
				{ ECPayConstants.PARAM_IS_COLLECTION, GetIsCollection(shippingReceivingStoreType) },
				{ ECPayConstants.PARAM_DEVICE, isSmartPhone
					? Constants.FLG_RECEIVINGSTORE_TWECPAY_DEVICE_SP
					: Constants.FLG_RECEIVINGSTORE_TWECPAY_DEVICE_PC },
				{ ECPayConstants.PARAM_SERVER_REPLY_URL, serverReplyUrl }
			};
			return parameters;
		}

		/// <summary>
		/// Get logistics sub type
		/// </summary>
		/// <param name="shippingService">Shipping service</param>
		/// <returns>Logistics sub type</returns>
		public static string GetLogisticsSubType(string shippingService)
		{
			var isC2C = (Constants.RECEIVINGSTORE_TWECPAY_CVS_LOGISTIC_METHOD == ECPayConstants.CONST_CVS_LOGISTIC_METHOD_C2C);
			switch (shippingService)
			{
				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART:
				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART_PAYMENT:
					return isC2C
						? Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_FAMILY_MART_C2C
						: Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_FAMILY_MART;

				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT:
				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT_PAYMENT:
					return isC2C
						? Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_7_ELEVEN_C2C
						: Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_7_ELEVEN;

				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE:
				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE_PAYMENT:
					return isC2C
						? Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_HI_LIFE_C2C
						: Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_HI_LIFE;

				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// Get is collection
		/// </summary>
		/// <param name="shippingService">Shipping service</param>
		/// <returns>Is collection: Y(Yes) or N(No)</returns>
		public static string GetIsCollection(string shippingService)
		{
			switch (shippingService)
			{
				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART_PAYMENT:
				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT_PAYMENT:
				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE_PAYMENT:
					return Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_ON;

				default:
					return Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_COLLECTION_OFF;
			}
		}

		/// <summary>
		/// Check logistics sub type 7-ELEVEN
		/// </summary>
		/// <param name="shippingReceivingStoreType">Shipping Receiving Store Type</param>
		/// <returns>If true: 7-ELEVEN or false: not 7-ELEVEN</returns>
		public static bool CheckLogisticsSubType7Eleven(string shippingReceivingStoreType)
		{
			var logisticsSubType = GetLogisticsSubType(shippingReceivingStoreType);
			return ((logisticsSubType == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_7_ELEVEN)
				|| (logisticsSubType == Constants.FLG_RECEIVINGSTORE_TWECPAY_LOGISTICS_SUB_TYPE_7_ELEVEN_C2C));
		}

		/// <summary>
		/// Check Logistics Sub Type 7-ELEVEN And C2C Family Mart
		/// </summary>
		/// <param name="shippingReceivingStoreType">Shipping Receiving Store Type</param>
		/// <returns>True: 7-ELEVEN And C2C Family mart</returns>
		public static bool CheckLogisticsSubType7ElevenAndC2CFamilyMart(string shippingReceivingStoreType)
		{
			var isC2C = (Constants.RECEIVINGSTORE_TWECPAY_CVS_LOGISTIC_METHOD == ECPayConstants.CONST_CVS_LOGISTIC_METHOD_C2C);
			return (CheckShippingReceivingStoreType7Eleven(shippingReceivingStoreType)
				|| ((shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART
						|| shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART_PAYMENT)
					&& isC2C));
		}

		/// <summary>
		/// Check shipping receiving store type 7-ELEVEN
		/// </summary>
		/// <param name="shippingReceivingStoreType">Shipping Receiving Store Type</param>
		/// <returns>True: Shipping receiving store type is 7-ELEVEN</returns>
		public static bool CheckShippingReceivingStoreType7Eleven(string shippingReceivingStoreType)
		{
			var result = ((shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT)
				|| (shippingReceivingStoreType == Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT_PAYMENT));
			return result;
		}

		/// <summary>
		/// Convert Shipping Status
		/// </summary>
		/// <param name="logisticsSubType">Logistics Sub-Type</param>
		/// <param name="returnCode">Return Code</param>
		/// <returns>Shipping Status Has Convert</returns>
		public static string ConvertShippingStatus(string logisticsSubType, string returnCode)
		{
			var shippingStatusConvert = ValueText.GetValueText(
				Constants.TABLE_ORDERSHIPPING,
				string.Format("{0}_{1}",
					Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CONVERT,
					logisticsSubType),
				returnCode);
			return shippingStatusConvert;
		}

		/// <summary>
		/// Create Server Reply Url
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="isOpenMap">Is Open Map</param>
		/// <param name="dontNotify">Don't notify</param>
		/// <returns>Server Reply Url</returns>
		public static string CreateServerReplyUrl(string orderId, bool isOpenMap, bool dontNotify = false)
		{
			var serverReplyUrl = string.Format("{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				(isOpenMap || string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN))
					? Constants.SITE_DOMAIN
					: Constants.WEBHOOK_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC);
			var result = (isOpenMap)
				? string.Format("{0}{1}", serverReplyUrl, ECPayConstants.PAGE_RECEIVING_STORE_FOR_ECPAY_RESPONSE)
				: string.Format(
					"{0}{1}{2}",
					serverReplyUrl,
					(dontNotify == false)
						? ECPayConstants.SERVER_REPLY_URL
						: ECPayConstants.SERVER_REPLY_NO_ACTION_URL,
					orderId);
			return result;
		}

		/// <summary>
		/// Create Url
		/// </summary>
		/// <param name="shippingReceivingStoreType">Shipping Receiving Store Type</param>
		/// <returns>Url</returns>
		public static string CreateUrl(string shippingReceivingStoreType)
		{
			switch (shippingReceivingStoreType)
			{
				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART:
				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_FAMILY_MART_PAYMENT:
					return ECPayConstants.CONST_URL_EXPRESS_PRINT_FAMI_C2C;

				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE:
				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_HI_LIFE_PAYMENT:
					return ECPayConstants.CONST_URL_EXPRESS_PRINT_HILIFE_C2C;

				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT:
				case Constants.FLG_RECEIVINGSTORE_TWECPAY_SHIPPING_SERVICE_7_ELEVENT_PAYMENT:
					return ECPayConstants.CONST_URL_EXPRESS_PRINT_UNIMART_C2C;

				default:
					return string.Empty;
			}
		}
		#endregion

		#region +Method Payment
		/// <summary>
		/// Export Log
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="orderId">Order Id</param>
		/// <param name="paymentOrderId">Payment Order Id</param>
		/// <param name="result">Result</param>
		/// <param name="paymentId">Payment Id</param>
		public static void ExportLog(
			string message,
			string orderId,
			string paymentOrderId,
			bool result,
			string paymentId)
		{
			var idDictionary = new Dictionary<string, string>
			{
				{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(orderId) },
				{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, StringUtility.ToEmpty(paymentOrderId) },
			};
			PaymentFileLogger.WritePaymentLog(
				result,
				paymentId,
				PaymentFileLogger.PaymentType.EcPay,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				message,
				idDictionary);
		}

		/// <summary>
		/// Create Request For Regist Order
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <returns>Request For Regist Order</returns>
		public static ECPayRequest CreateRequestForRegistOrder(CartObject cart)
		{
			// Create Request
			var baseUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				string.IsNullOrEmpty(Constants.WEBHOOK_DOMAIN)
					? Constants.SITE_DOMAIN
					: Constants.WEBHOOK_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC);

			var clientBackUrl = new UrlCreator(baseUrl + Constants.PAGE_FRONT_PAYMENT_ECPAY_ORDER_RESULT)
				.AddParam(Constants.FIELD_ORDER_ORDER_ID, cart.OrderId)
				.CreateUrl();
			var request = new ECPayRequest
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				MerchantTradeNo = cart.OrderId,
				MerchantTradeDate = DateTime.Now.ToString(ECPayConstants.CONST_FORMAT_DATE_API),
				PaymentType = ECPayConstants.CONST_PAYMENT_TYPE,
				TotalAmount = cart.SettlementAmount.ToString("0"),
				TradeDesc = ShopMessageUtil.GetMessage("ShopName"),
				ItemName = CreateItemName(cart),
				ChoosePayment = cart.Payment.ExternalPaymentType,
				EncryptType = ECPayConstants.CONST_ENCRYPT_TYPE,
				ClientBackUrl = clientBackUrl,
				ReturnUrl = CreateUrlApiForReturnOrPayment(baseUrl, cart.CartId, true),
				PaymentInfoUrl = CreateUrlApiForReturnOrPayment(baseUrl, cart.CartId),
				PlatformId = CreatePlatformId(),
			};
			if (request.ChoosePayment == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT)
			{
				if (cart.Payment.IsPaymentEcPayWithCreditInstallment)
				{
					request.CreditInstallment = Constants.ECPAY_PAYMENT_CREDIT_INSTALLMENT;
				}
				request.BindingCard = ECPayConstants.CONST_BINDING_CARD;
				request.MerchantMemberId = CreateMerchantMemberId(cart.OrderUserId);
			}
			request.CheckMacValue = CreateChecksum(request);
			return request;
		}

		/// <summary>
		/// Create Url Api For Return Or Payment
		/// </summary>
		/// <param name="host">Host</param>
		/// <param name="cartId">Cart Id</param>
		/// <param name="isGetReturnUrl">Is Get Return Url</param>
		/// <returns>Url</returns>
		private static string CreateUrlApiForReturnOrPayment(string host, string cartId, bool isGetReturnUrl = false)
		{
			var urlCreator = new UrlCreator(host + Constants.PAGE_FRONT_PAYMENT_ECPAY_RECEIVE)
				.AddParam(Constants.REQUEST_KEY_NO, isGetReturnUrl
					? ECPayConstants.CONST_RETURN_URL_PARAMETER_NO
					: ECPayConstants.CONST_PAYMENT_URL_PARAMETER_NO)
				.AddParam(Constants.REQUEST_KEY_CART_ID, cartId)
				.CreateUrl();
			return urlCreator;
		}

		/// <summary>
		/// Create Platform Id
		/// </summary>
		/// <returns>Platform Id From Config</returns>
		private static string CreatePlatformId()
		{
			return ((Constants.PAYMENT_ECPAY_SPECIAL_MERCHANTFLAG)
				? Constants.PAYMENT_ECPAY_MERCHANTID
				: string.Empty);
		}

		// <summary>
		/// Create Barcode Url
		/// </summary>
		/// <param name="barcode1">Barcode 1</param>
		/// <param name="barcode2">Barcode 2</param>
		/// <param name="barcode3">Barcode 3</param>
		/// <returns>Barcode Url</returns>
		public static string CreateBarcodeUrl(string barcode1, string barcode2, string barcode3)
		{
			var baseUrl = string.Format(
				"{0}{1}{2}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC);
			var barcodeUrl = new UrlCreator(baseUrl + Constants.PAGE_FRONT_PAYMENT_ECPAY_BARCODE)
				.AddParam("code1", barcode1)
				.AddParam("code2", barcode2)
				.AddParam("code3", barcode3)
				.CreateUrl();
			return barcodeUrl;
		}

		/// <summary>
		/// Create Payment Memo
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="memo">Memo</param>
		/// <param name="totalAmount">Total Amount</param>
		/// <returns>Payment Memo</returns>
		public static string CreatePaymentMemo(OrderModel order, string memo, decimal totalAmount)
		{
			var tempMemo = OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
				order.OrderId,
				order.OrderPaymentKbn,
				order.CardTranId,
				memo,
				totalAmount);
			var finalMemo = OrderExternalPaymentUtility.SetExternalPaymentMemo(
				StringUtility.ToEmpty(order.PaymentMemo),
				tempMemo);
			return finalMemo;
		}

		/// <summary>
		/// Create Merchant Member Id
		/// </summary>
		/// <param name="userId">USer Id</param>
		/// <returns>Merchant Member Id</returns>
		private static string CreateMerchantMemberId(string userId)
		{
			var userCreditCard = new UserCreditCardService();
			var branchNo = userCreditCard.GetMaxBranchNoByUserIdAndCooperationType(
				userId,
				Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_ECPAY);
			var userCrediCardInfo = userCreditCard.Get(userId, branchNo);
			return (userCrediCardInfo != null)
				? userCrediCardInfo.CooperationId
				: string.Format("{0}{1}", Constants.PAYMENT_ECPAY_MERCHANTID, userId);
		}

		/// <summary>
		/// Create Request For Cancel Refund And Capture Payment
		/// </summary>
		/// <param name="order">Order</param>
		/// <param name="isCancel">Is Cancel Action</param>
		/// <param name="isRefund">Is Refund Action</param>
		/// <param name="refundAmount">Refund Amount</param>
		/// <returns>Request For Cancel Refund And Capture Payment</returns>
		public static ECPayRequest CreateRequestForCancelRefundAndCapturePayment(
			OrderModel order,
			bool isCancel = false,
			bool isRefund = false,
			decimal refundAmount = 0)
		{
			// Create Request
			var totalAmount = isRefund
				? refundAmount
				: order.SettlementAmount;
			var request = new ECPayRequest
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				MerchantTradeNo = (string.IsNullOrEmpty(order.OrderIdOrg)
					? order.OrderId
					: order.OrderIdOrg),
				TradeNo = order.CardTranId,
				Action = CreateAction(isCancel, isRefund),
				TotalAmount = totalAmount.ToString("0")
			};
			request.PlatformId = CreatePlatformId();
			request.CheckMacValue = CreateChecksum(request, true);
			return request;
		}

		/// <summary>
		/// Create Action
		/// </summary>
		/// <param name="isCancel">Is Cancel Action</param>
		/// <param name="isRefund">Is Refund Action</param>
		/// <returns>Action</returns>
		private static string CreateAction(bool isCancel, bool isRefund)
		{
			return (isCancel)
				? ECPayConstants.CONST_ACTION_CANCEL
				: (isRefund)
					? ECPayConstants.CONST_ACTION_REFUND
					: ECPayConstants.CONST_ACTION_CAPTURE;
		}

		/// <summary>
		/// Create Item Name
		/// </summary>
		/// <param name="cart">Cart</param>
		/// <returns>Item Name</returns>
		private static string CreateItemName(CartObject cart)
		{
			var result = cart.Items
				.Select(item => string.Format(
					"{0}:{1}元",
					item.ProductName,
					item.PriceSubtotal.ToString("0")))
				.ToList();

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
				result.Add(string.Format(
					"{0}:-{1}元",
					CommonPageProcess.ReplaceTag(
						"@@DispText.item_name.discount@@"),
					discount.ToString("0")));
			}

			// ポイント利用額
			if (cart.UsePointPrice > 0)
			{
				result.Add(string.Format(
					"{0}:-{1}元",
					CommonPageProcess.ReplaceTag(
						"@@DispText.item_name.point@@"),
					cart.UsePointPrice.ToString("0")));
			}

			// 配送料
			if (cart.PriceShipping > 0)
				result.Add(string.Format(
					"{0}:{1}元",
					CommonPageProcess.ReplaceTag(
						"@@DispText.item_name.shipping@@"),
					cart.PriceShipping.ToString("0")));

			// 調整金額
			if (cart.PriceRegulationTotal != 0)
			{
				result.Add(string.Format(
					"{0}:{1}元",
					CommonPageProcess.ReplaceTag(
						"@@DispText.item_name.price_regulation@@"),
					cart.PriceRegulationTotal.ToString("0")));
			}

			// 決済手数料
			if ((cart.Payment != null)
				&& (cart.Payment.PriceExchange != 0))
			{
				result.Add(string.Format(
					"{0}:{1}元",
					CommonPageProcess.ReplaceTag(
						"@@DispText.item_name.payment_fee@@"),
					cart.Payment.PriceExchange.ToString("0")));
			}

			var itemName = string.Join("#", result);
			return itemName;
		}

		/// <summary>
		/// Create Checksum
		/// </summary>
		/// <param name="request">Request</param>
		/// <param name="isCallApiDoAction">Is Call Api Do Action</param>
		/// <returns>Checksum</returns>
		private static string CreateChecksum(ECPayRequest request, bool isCallApiDoAction = false)
		{
			var parameters = (isCallApiDoAction
				? CreateParametersForCallApiDoAction(request)
				: CreateParameters(request));

			var encodedUrl = CreateUrl(parameters);

			var result = CreateSHA256(encodedUrl);
			return result.ToUpper();
		}

		/// <summary>
		/// Create SHA256
		/// </summary>
		/// <param name="url">Url</param>
		/// <returns>Value</returns>
		private static string CreateSHA256(string url)
		{
			// Convert To SHA256
			var result = string.Empty;
			using (var hash = SHA256Managed.Create())
			{
				var checkSumEncode = new StringBuilder();
				var datas = hash.ComputeHash(Encoding.UTF8.GetBytes(url));
				foreach (var data in datas)
				{
					checkSumEncode.Append(data.ToString("x2"));
				}
				result = checkSumEncode.ToString();
			}
			return result;
		}

		/// <summary>
		/// Create Url
		/// </summary>
		/// <param name="parameters">Parameters</param>
		/// <returns>Url</returns>
		private static string CreateUrl(KeyValuePair<string, string>[] parameters)
		{
			// Create Url With Hash Key & Hash IV
			var hashKey = string.Format("{0}={1}", ECPayConstants.CONST_REQUEST_KEY_HASH_KEY, Constants.PAYMENT_ECPAY_HASHKEY[0]);
			var hashIV = string.Format("{0}={1}", ECPayConstants.CONST_REQUEST_KEY_HASH_IV, Constants.PAYMENT_ECPAY_HASHKEY[1]);
			var url = string.Format(
				"{0}&{1}&{2}",
				hashKey,
				string.Join("&", parameters
					.Select(item => string.Format("{0}={1}", item.Key, item.Value))),
				hashIV);

			// Url Encoding And Convert To Lower
			var result = HttpUtility.UrlEncode(url).ToLower();
			return result;
		}

		/// <summary>
		/// Create Parameters
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>Parameters</returns>
		private static KeyValuePair<string, string>[] CreateParametersForCallApiDoAction(ECPayRequest request)
		{
			var parameters = new Dictionary<string, string>
			{
				{ "Action", request.Action },
				{ "MerchantID", request.MerchantId },
				{ "MerchantTradeNo", request.MerchantTradeNo },
				{ "TotalAmount", request.TotalAmount },
				{ "TradeNo", request.TradeNo },
				{ "PlatformID", request.PlatformId }
			};
			// Remove item null and sort
			var result = parameters
				.OrderBy(item => item.Key)
				.ToArray();
			return result;
		}

		/// <summary>
		/// Create Parameters
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>Parameters</returns>
		private static KeyValuePair<string, string>[] CreateParameters(ECPayRequest request)
		{
			var parameters = new Dictionary<string, string>
			{
				{ "MerchantID", request.MerchantId },
				{ "MerchantTradeNo", request.MerchantTradeNo },
				{ "MerchantTradeDate", request.MerchantTradeDate },
				{ "PaymentType", request.PaymentType },
				{ "TotalAmount", request.TotalAmount },
				{ "TradeDesc", request.TradeDesc },
				{ "ItemName", request.ItemName },
				{ "ReturnURL", request.ReturnUrl },
				{ "ChoosePayment", request.ChoosePayment },
				{ "ClientBackURL", request.ClientBackUrl },
				{ "EncryptType", request.EncryptType },
				{ "PaymentInfoURL", request.PaymentInfoUrl },
				{ "PlatformID", request.PlatformId }
			};
			if (request.ChoosePayment == Constants.FLG_PAYMENT_TYPE_ECPAY_CREDIT)
			{
				if (string.IsNullOrEmpty(request.CreditInstallment) == false)
				{
					parameters.Add("CreditInstallment", request.CreditInstallment);
				}
				parameters.Add("BindingCard", request.BindingCard);
				parameters.Add("MerchantMemberID", request.MerchantMemberId);
			}
			// Remove item null and sort
			var result = parameters
				.OrderBy(item => item.Key)
				.ToArray();
			return result;
		}

		/// <summary>
		/// Register As User Credit Card
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザークレジットカード</returns>
		public static UserCreditCardModel RegisterAsUserCreditCard(
			string userId,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor = null)
		{
			var userCreditCard = new UserCreditCardModel
			{
				UserId = userId,
				CooperationId = string.Format(Constants.PAYMENT_ECPAY_MERCHANTID + userId),
				CooperationId2 = string.Empty,
				CardDispName = ECPayConstants.USERCREDITCARD_CARDDISPNAME_ECPAYCUSTOMER,
				LastFourDigit = string.Empty,
				ExpirationMonth = string.Empty,
				ExpirationYear = string.Empty,
				AuthorName = string.Empty,
				DispFlg = Constants.FLG_USERCREDITCARD_DISP_FLG_OFF,
				LastChanged = lastChanged,
				CompanyCode = string.Empty,
				CooperationType = Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_ECPAY,
			};

			new UserCreditCardService().Insert(
				userCreditCard,
				updateHistoryAction,
				accessor);
			return userCreditCard;
		}

		/// <summary>
		/// Get Payment Message Text
		/// </summary>
		/// <param name="externalPaymentType">External Payment Type</param>
		/// <param name="paymentMemo">Payment Memo</param>
		/// <returns>Payment Message Text</returns>
		public static string GetPaymentMessageText(string externalPaymentType, string paymentMemo)
		{
			var pattern = string.Empty;
			switch (externalPaymentType)
			{
				case Constants.FLG_PAYMENT_TYPE_ECPAY_ATM:
					pattern = "(?=銀行番号).*(?=・)";
					break;

				case Constants.FLG_PAYMENT_TYPE_ECPAY_CVS:
					pattern = "(?=支払い番号).*(?=・)";
					break;

				case Constants.FLG_PAYMENT_TYPE_ECPAY_BARCODE:
					pattern = "(?=バーコード).*(?=・)";
					break;

				default:
					return string.Empty;
			}
			var result = Regex.Match(paymentMemo, pattern).Value;
			return result;
		}
		#endregion
	}
}
