/*
=========================================================================================================
  Module      : Tw invoice Ec Pay api (TwInvoiceEcPayApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Option;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.TwOrderInvoice;

namespace w2.App.Common.Api
{
	/// <summary>
	/// Tw invoice Ec Pay api
	/// </summary>
	public class TwInvoiceEcPayApi
	{
		/// <summary>Execute types</summary>
		public enum ExecuteTypes
		{
			/// <summary>Issue</summary>
			Issue,
			/// <summary>Check barcode</summary>
			CheckBarcode,
			/// <summary>Check love code</summary>
			CheckLoveCode,
			/// <summary>Allowance</summary>
			Allowance,
			/// <summary>Invalid</summary>
			Invalid
		}

		/// <summary>Reason types</summary>
		public enum ReasonTypes
		{
			/// <summary>Cancel</summary>
			Cancel,
			/// <summary>Return</summary>
			Return,
			/// <summary>Exchange</summary>
			Exchange
		}

		#region Constants
		/// <summary>Tw invoice Ec Pay: Issue url</summary>
		private const string TWINVOICE_ECPAY_ISSUE_URL = "B2CInvoice/Issue";
		/// <summary>Tw invoice Ec Pay: Check barcode url</summary>
		private const string TWINVOICE_ECPAY_CHECK_BARCODE_URL = "B2CInvoice/CheckBarcode";
		/// <summary>Tw invoice Ec Pay: Check love code url</summary>
		private const string TWINVOICE_ECPAY_CHECK_LOVECODE_URL = "B2CInvoice/CheckLoveCode";
		/// <summary>Tw invoice Ec Pay: Allowance url</summary>
		private const string TWINVOICE_ECPAY_ALLOWANCE_URL = "B2CInvoice/Allowance";
		/// <summary>Tw invoice Ec Pay: Invalid url</summary>
		private const string TWINVOICE_ECPAY_INVALID_URL = "B2CInvoice/Invalid";
		/// <summary>Return code success default</summary>
		protected const int RETURN_CODE_SUCCESS_DEFAULT = 1;
		/// <summary>Month 12</summary>
		protected const int MONTH_12 = 12;
		/// <summary>Month 10</summary>
		protected const int MONTH_10 = 10;
		/// <summary>Month 8</summary>
		protected const int MONTH_8 = 8;
		/// <summary>Month 6</summary>
		protected const int MONTH_6 = 6;
		/// <summary>Month 4</summary>
		protected const int MONTH_4 = 4;
		/// <summary>Month 2</summary>
		protected const int MONTH_2 = 2;
		/// <summary>Max period</summary>
		protected const int MAX_PERIOD = 6;
		/// <summary>Period default</summary>
		protected const int PERIOD_DEFAULT = 0;
		/// <summary>Expiration date default</summary>
		protected const int EXPIRATION_DATE_DEFAULT = 13;
		/// <summary>Flag: Reason type order cancel</summary>
		protected const string FLG_REASON_ORDER_CANCEL = "訂單取消";
		/// <summary>Flag: Reason type order return</summary>
		protected const string FLG_REASON_ORDER_RETURN = "退貨";
		/// <summary>Flag: Reason type order exchange</summary>
		protected const string FLG_REASON_ORDER_EXCHANGE = "換貨";
		/// <summary>Item name: Price exchange</summary>
		protected const string ITEM_NAME_PRICE_EXCHANGE = "付款手續費";
		/// <summary>Item name: Price shipping</summary>
		protected const string ITEM_NAME_PRICE_SHIPPING = "運費";
		/// <summary>Item name: Discount</summary>
		protected const string ITEM_NAME_DISCOUNT = "折扣金額";
		/// <summary>Item name: Point use yen</summary>
		protected const string ITEM_NAME_POINT_USE_YEN = "使用點數";
		/// <summary>Item name: Price Regulation</summary>
		protected const string ITEM_NAME_PRICE_REGULATION = "調整金額";
		/// <summary>Flag: Print on</summary>
		protected const string FLG_PRINT_ON = "1";
		/// <summary>Flag: Print off</summary>
		protected const string FLG_PRINT_OFF = "0";
		/// <summary>Flag: Donation on</summary>
		protected const string FLG_DONATION_ON = "1";
		/// <summary>Flag: Donation off</summary>
		protected const string FLG_DONATION_OFF = "0";
		/// <summary>Flag: Vat default</summary>
		protected const string FLG_VAT_DEFAULT = "1";
		/// <summary>Flag: Carrier type certificate</summary>
		protected const string FLG_CARRIER_TYPE_CERTIFICATE = "2";
		/// <summary>Flag: Carrier type mobile</summary>
		protected const string FLG_CARRIER_TYPE_MOBILE = "3";
		/// <summary>Flag: Is exist</summary>
		public const string FLG_IS_EXIST = "Y";
		/// <summary>Flag: Not taxable</summary>
		public const int FLG_NOT_TAXABLE = 2;
		/// <summary>Return code EcPay invoice system maintenance</summary>
		public const int RETURN_CODE_ECPAY_INVOICE_SYSTEM_MAINTENANCE = 10000010;
		/// <summary>Message setting key result</summary>
		public const string MESSAGESETTING_KEY_RESULT = "result";
		/// <summary>Message setting key message</summary>
		public const string MESSAGESETTING_KEY_MESSAGE = "message";
		/// <summary>電子発票キャンセル</summary>
		public const int RETURN_CODE_CANCELED = 5070453;
		/// <summary>電子発票キャンセル(折讓)</summary>
		public const int RETURN_CODE_ALLOWANCED = 5070450;
		/// <summary>電子発票キャンセル(註銷)</summary>
		public const int RETURN_CODE_WTITEOFF = 5070452;
		#endregion

		#region Methods
		/// <summary>
		/// Create request cancel object
		/// </summary>
		/// <param name="executeType">Execute type</param>
		/// <param name="order">Order information</param>
		/// <param name="orderInvoice">Order invoice</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Request object</returns>
		public TwInvoiceEcPayRequestObject CreateRequestCancelObject(
			ExecuteTypes executeType,
			OrderModel order,
			TwOrderInvoiceModel orderInvoice,
			SqlAccessor accessor = null)
		{
			this.Reason = ReasonTypes.Cancel;
			var requestObject = CreateRequestObject(executeType, order, orderInvoice, accessor);
			return requestObject;
		}

		/// <summary>
		/// Create request return object
		/// </summary>
		/// <param name="executeType">Execute type</param>
		/// <param name="order">Order information</param>
		/// <param name="orderInvoice">Order invoice</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Request object</returns>
		public TwInvoiceEcPayRequestObject CreateRequestReturnObject(
			ExecuteTypes executeType,
			OrderModel order,
			TwOrderInvoiceModel orderInvoice,
			SqlAccessor accessor = null)
		{
			this.Reason = ReasonTypes.Return;
			var requestObject = CreateRequestObject(executeType, order, orderInvoice, accessor);
			return requestObject;
		}

		/// <summary>
		/// Create request exchange object
		/// </summary>
		/// <param name="executeType">Execute type</param>
		/// <param name="order">Order information</param>
		/// <param name="orderInvoice">Order invoice</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Request object</returns>
		public TwInvoiceEcPayRequestObject CreateRequestExchangeObject(
			ExecuteTypes executeType,
			OrderModel order,
			TwOrderInvoiceModel orderInvoice,
			SqlAccessor accessor = null)
		{
			this.Reason = ReasonTypes.Exchange;
			var requestObject = CreateRequestObject(executeType, order, orderInvoice, accessor);
			return requestObject;
		}

		/// <summary>
		/// Create request object
		/// </summary>
		/// <param name="executeType">Execute type</param>
		/// <param name="order">Order information</param>
		/// <param name="orderInvoice">Order invoice</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Request object</returns>
		public TwInvoiceEcPayRequestObject CreateRequestObject(
			ExecuteTypes executeType,
			OrderModel order,
			TwOrderInvoiceModel orderInvoice,
			SqlAccessor accessor = null)
		{
			// Create data object
			var dataObject = CreateDataObject(executeType, order, orderInvoice, accessor);

			// Encode data object
			var dataEncode = EncodeDataObject(dataObject);

			var requestObject = new TwInvoiceEcPayRequestObject
			{
				PlatformId = Constants.PAYMENT_ECPAY_SPECIAL_MERCHANTFLAG
					? Constants.PAYMENT_ECPAY_MERCHANTID
					: string.Empty,
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				RqHeader = CreateRqHeaderRequestObject(),
				Data = dataEncode
			};
			return requestObject;
		}

		/// <summary>
		/// Receive response object
		/// </summary>
		/// <param name="executeType">Execute type</param>
		/// <param name="requestObject">Request object</param>
		/// <returns>Response</returns>
		public TwInvoiceEcPayResponse ReceiveResponseObject(
			ExecuteTypes executeType,
			TwInvoiceEcPayRequestObject requestObject)
		{
			// Convert data object to Json
			var requestData = JsonConvert.SerializeObject(
				requestObject,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});

			// Write infomation log
			FileLogger.WriteInfo("TW ECPAY GET INVOICE");
			FileLogger.WriteInfo("Request call api: " + requestData);

			// Post and get data
			var url = GetEndPointUrl(executeType);
			var responseBody = PostAndReceiveData(url, Encoding.ASCII.GetBytes(requestData));
			var resultObject = JsonConvert.DeserializeObject<TwInvoiceEcPayResultObject>(responseBody);

			// Convert to respone
			var response = ConvertToResponse(resultObject);

			// Write error log
			if (response.IsSuccess == false)
			{
				FileLogger.WriteError(string.Format(
					"{0} - {1}",
					Constants.TWINVOICE_ECPAY_API_URL,
					response.Message));
			}
			return response;
		}

		/// <summary>
		/// Create issue data request object
		/// </summary>
		/// <param name="order">Order information</param>
		/// <param name="orderInvoice">Order invoice</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Data object</returns>
		private TwInvoiceEcPayRequestObject.DataObject CreateIssueDataRequestObject(
			OrderModel order,
			TwOrderInvoiceModel orderInvoice,
			SqlAccessor accessor)
		{
			var isInvoiceCompany = (orderInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_COMPANY);
			var isInvoiceDonate = (orderInvoice.TwUniformInvoice == Constants.FLG_TW_UNIFORM_INVOICE_DONATE);
			var carrierType = GetCarrierType(orderInvoice.TwCarryType);
			var salesAmount = int.Parse(order.OrderPriceTotal.ToString("0"));
			var relateNumber = string.Format(
				"{0}{1}",
				order.OrderId,
				DateTime.Now.ToString("MMddHHmm"));
			var address = string.Format(
				"{0}{1}",
				order.Owner.ConcatenateAddressWithoutCountryName(),
				order.Owner.OwnerAddr5);

			var dataObject = new TwInvoiceEcPayRequestObject.DataObject
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				RelateNumber = relateNumber,
				CustomerId = order.UserId,
				CustomerName = isInvoiceCompany
					? orderInvoice.TwUniformInvoiceOption2
					: order.Owner.OwnerName,
				CustomerAddr = (address.Length > 100)
					? address.Substring(0, 100)
					: address,
				CustomerPhone = order.Owner.OwnerTel1.Replace("-", string.Empty),
				CustomerEmail = order.Owner.OwnerMailAddr,
				TaxType = Constants.TWINVOICE_ECPAY_TAX_TYPE.ToString(),
				Items = CreateItemsObjects(order, ref salesAmount, accessor),
				SalesAmount = salesAmount,
				InvType = Constants.TWINVOICE_ECPAY_INV_TYPE,
				Vat = FLG_VAT_DEFAULT,
			};

			if (isInvoiceCompany)
			{
				dataObject.CustomerIdentifier = orderInvoice.TwUniformInvoiceOption1;
			}

			if (Constants.TWINVOICE_ECPAY_TAX_TYPE == FLG_NOT_TAXABLE)
			{
				dataObject.ClearanceMark = Constants.TWINVOICE_ECPAY_CLEARANCE_MARK;
			}

			if ((dataObject.Print == FLG_PRINT_ON)
				|| (string.IsNullOrEmpty(dataObject.CustomerIdentifier) == false)
				|| string.IsNullOrEmpty(orderInvoice.TwCarryType))
			{
				dataObject.CarrierType = string.Empty;
			}
			else
			{
				dataObject.CarrierType = carrierType;
			}

			dataObject.Donation = isInvoiceDonate
				? FLG_DONATION_ON
				: FLG_DONATION_OFF;

			if ((dataObject.Donation == FLG_DONATION_ON)
				|| (string.IsNullOrEmpty(dataObject.CarrierType) == false))
			{
				dataObject.Print = FLG_PRINT_OFF;
			}
			else
			{
				dataObject.Print = FLG_PRINT_ON;
			}

			if (dataObject.Donation == FLG_DONATION_ON)
			{
				dataObject.LoveCode = orderInvoice.TwUniformInvoiceOption1;
			}

			switch (dataObject.CarrierType)
			{
				case FLG_CARRIER_TYPE_CERTIFICATE:
				case FLG_CARRIER_TYPE_MOBILE:
					dataObject.CarrierNum = orderInvoice.TwCarryTypeOption;
					break;

				default:
					dataObject.CarrierNum = string.Empty;
					break;
			}
			return dataObject;
		}

		/// <summary>
		/// Create check barcode data request object
		/// </summary>
		/// <param name="orderInvoice">Order invoice</param>
		/// <returns>Data object</returns>
		private TwInvoiceEcPayRequestObject.DataObject CreateCheckBarcodeDataRequestObject(
			TwOrderInvoiceModel orderInvoice)
		{
			var dataObject = new TwInvoiceEcPayRequestObject.DataObject
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				BarCode = orderInvoice.TwCarryTypeOption,
			};
			return dataObject;
		}

		/// <summary>
		/// Create check love code data request object
		/// </summary>
		/// <param name="orderInvoice">Order invoice</param>
		/// <returns>Data object</returns>
		private TwInvoiceEcPayRequestObject.DataObject CreateCheckLoveCodeDataRequestObject(
			TwOrderInvoiceModel orderInvoice)
		{
			var dataObject = new TwInvoiceEcPayRequestObject.DataObject
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				LoveCode = orderInvoice.TwUniformInvoiceOption1,
			};
			return dataObject;
		}

		/// <summary>
		/// Create allowance data request object
		/// </summary>
		/// <param name="order">Order information</param>
		/// <param name="orderInvoice">Order invoice</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Data object</returns>
		private TwInvoiceEcPayRequestObject.DataObject CreateAllowanceDataRequestObject(
			OrderModel order,
			TwOrderInvoiceModel orderInvoice,
			SqlAccessor accessor)
		{
			var allowanceAmount = int.Parse(Math.Round(order.OrderPriceRepayment).ToString());
			var dataObject = new TwInvoiceEcPayRequestObject.DataObject
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				InvoiceNo = (orderInvoice.TwInvoiceNo.Length > 10)
					? orderInvoice.TwInvoiceNo.Substring(0, 10)
					: orderInvoice.TwInvoiceNo,
				InvoiceDate = DateTimeUtility.ToString(
					orderInvoice.TwInvoiceDate,
					DateTimeUtility.FormatType.ShortDate2Letter,
					string.Empty),
				AllowanceNotify = Constants.TWINVOICE_ECPAY_ALLOWANCE_NOTIFY,
				CustomerName = order.Owner.OwnerName,
				NotifyMail = order.Owner.OwnerMailAddr,
				Reason = GetReasonType(this.Reason),
				Items = CreateItemsObjects(order, ref allowanceAmount, accessor),
				AllowanceAmount = (order.OrderPriceRepayment != 0)
					? int.Parse(Math.Round(order.OrderPriceRepayment).ToString())
					: int.Parse(Math.Round(order.OrderPriceTotal).ToString())
			};
			return dataObject;
		}

		/// <summary>
		/// Create invalid data request object
		/// </summary>
		/// <param name="orderInvoice">Order invoice</param>
		/// <returns>Data object</returns>
		private TwInvoiceEcPayRequestObject.DataObject CreateInvalidDataRequestObject(
			TwOrderInvoiceModel orderInvoice)
		{
			var dataObject = new TwInvoiceEcPayRequestObject.DataObject
			{
				MerchantId = Constants.PAYMENT_ECPAY_MERCHANTID,
				InvoiceNo = (orderInvoice.TwInvoiceNo.Length > 10)
					? orderInvoice.TwInvoiceNo.Substring(0, 10)
					: orderInvoice.TwInvoiceNo,
				InvoiceDate = DateTimeUtility.ToString(
					orderInvoice.TwInvoiceDate,
					DateTimeUtility.FormatType.ShortDate2Letter,
					string.Empty),
				Reason = GetReasonType(this.Reason)
			};
			return dataObject;
		}

		/// <summary>
		/// Create items objects
		/// </summary>
		/// <param name="order">Order information</param>
		/// <param name="totalAmount">Total amount</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Items objects</returns>
		private TwInvoiceEcPayRequestObject.ItemsObject[] CreateItemsObjects(
			OrderModel order,
			ref int totalAmount,
			SqlAccessor accessor)
		{
			var itemsObjects = new List<TwInvoiceEcPayRequestObject.ItemsObject>();

			if (order.IsNotReturnExchangeOrder)
			{
				foreach (var item in order.Items)
				{
					if (item.IsReturnItem) continue;

					var itemObject = new TwInvoiceEcPayRequestObject.ItemsObject
					{
						ItemSeq = item.OrderItemNo,
						ItemName = item.ProductName,
						ItemCount = item.ItemQuantity,
						ItemWord = Constants.TWINVOICE_ECPAY_ITEM_WORD,
						ItemPrice = GetItemPrice(item),
						ItemAmount = int.Parse(item.ItemPrice.ToString("0"))
					};
					itemsObjects.Add(itemObject);
				}
			}
			else
			{
				itemsObjects = CreateItemsObjectsForReturnExchange(order, accessor);
			}

			// Price exchange
			if (order.OrderPriceExchange > 0)
			{
				var itemObject = new TwInvoiceEcPayRequestObject.ItemsObject
				{
					ItemSeq = (itemsObjects.Count() + 1),
					ItemName = ITEM_NAME_PRICE_EXCHANGE,
					ItemCount = 1,
					ItemWord = Constants.TWINVOICE_ECPAY_ITEM_WORD,
					ItemPrice = int.Parse(order.OrderPriceExchange.ToString("0")),
					ItemAmount = int.Parse(order.OrderPriceExchange.ToString("0"))
				};
				itemsObjects.Add(itemObject);
			}

			// Price shipping
			if (order.OrderPriceShipping > 0)
			{
				var itemObject = new TwInvoiceEcPayRequestObject.ItemsObject
				{
					ItemSeq = (itemsObjects.Count() + 1),
					ItemName = ITEM_NAME_PRICE_SHIPPING,
					ItemCount = 1,
					ItemWord = Constants.TWINVOICE_ECPAY_ITEM_WORD,
					ItemPrice = int.Parse(order.OrderPriceShipping.ToString("0")),
					ItemAmount = int.Parse(order.OrderPriceShipping.ToString("0"))
				};
				itemsObjects.Add(itemObject);
			}

			// Point use
			if (order.OrderPointUseYen > 0)
			{
				var itemObject = new TwInvoiceEcPayRequestObject.ItemsObject
				{
					ItemSeq = (itemsObjects.Count() + 1),
					ItemName = ITEM_NAME_POINT_USE_YEN,
					ItemCount = 1,
					ItemWord = Constants.TWINVOICE_ECPAY_ITEM_WORD,
					ItemPrice = (int.Parse(order.OrderPointUseYen.ToString("0")) * -1),
					ItemAmount = (int.Parse(order.OrderPointUseYen.ToString("0")) * -1)
				};
				itemsObjects.Add(itemObject);
			}

			// Create discount price
			var discountPrice = (order.MemberRankDiscountPrice
				+ order.OrderCouponUse
				+ order.FixedPurchaseDiscountPrice
				+ order.FixedPurchaseMemberDiscountAmount);

			if ((order.SetPromotions != null)
				&& (order.SetPromotions.Length > 0))
			{
				var setPromotionDiscountAmount =
					order.SetPromotions.Sum(item
						=> (item.ShippingChargeDiscountAmount + item.ProductDiscountAmount));
				discountPrice += setPromotionDiscountAmount;
			}

			// Discount price
			if (discountPrice > 0)
			{
				var itemObject = new TwInvoiceEcPayRequestObject.ItemsObject
				{
					ItemSeq = (itemsObjects.Count() + 1),
					ItemName = ITEM_NAME_DISCOUNT,
					ItemCount = 1,
					ItemWord = Constants.TWINVOICE_ECPAY_ITEM_WORD,
					ItemPrice = (int.Parse(discountPrice.ToString("0")) * -1),
					ItemAmount = (int.Parse(discountPrice.ToString("0")) * -1)
				};
				itemsObjects.Add(itemObject);
			}

			// Check total amount order match with sum total amount of item
			var totalAmountItem = itemsObjects.Sum(item =>
				(int.Parse(item.ItemCount.ToString()) * int.Parse(item.ItemPrice.ToString())));
			if (order.IsNotReturnExchangeOrder)
			{
				totalAmount = (int)CurrencyManager.GetSettlementAmount(
					order.LastBilledAmount,
					order.SettlementRate,
					order.SettlementCurrency);
			}

			if (totalAmountItem != totalAmount)
			{
				var itemObject = new TwInvoiceEcPayRequestObject.ItemsObject
				{
					ItemSeq = (itemsObjects.Count() + 1),
					ItemName = ITEM_NAME_PRICE_REGULATION,
					ItemCount = 1,
					ItemWord = Constants.TWINVOICE_ECPAY_ITEM_WORD,
					ItemPrice = int.Parse((totalAmount - totalAmountItem).ToString("0")),
					ItemAmount = int.Parse((totalAmount - totalAmountItem).ToString("0"))
				};
				itemsObjects.Add(itemObject);
			}

			// Convert amount
			itemsObjects.ForEach(item
				=> item.ItemPrice =
					int.Parse(CurrencyManager.GetSettlementAmount(
						decimal.Parse(item.ItemPrice.ToString()),
						order.SettlementRate,
						order.SettlementCurrency)
					.ToString()));
			return itemsObjects.ToArray();
		}

		/// <summary>
		/// Create items objects for return exchange
		/// </summary>
		/// <param name="orderNew">New order</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>List item</returns>
		private List<TwInvoiceEcPayRequestObject.ItemsObject> CreateItemsObjectsForReturnExchange(
			OrderModel orderNew,
			SqlAccessor accessor)
		{
			var orderService = new OrderService();
			var itemsObjects = new List<TwInvoiceEcPayRequestObject.ItemsObject>();
			var orderItemsAll = new List<OrderItemModel>();

			// Get all item from order original and order return exchange
			var orderOld = orderService.Get(orderNew.OrderIdOrg, accessor);
			if (orderOld.IsNotReturnExchangeOrder)
			{
				orderItemsAll = orderOld.Items.ToList();
			}
			else
			{
				var orderOrg = orderService.GetRelatedOrders(orderOld.OrderIdOrg);
				foreach (var order in orderOrg)
				{
					orderItemsAll.AddRange(order.Items);
				}
			}

			// Get all item exchange
			var isExchangeItems = orderNew.Items.Where(item => item.IsExchangeItem);
			orderItemsAll.AddRange(isExchangeItems);

			// Get all item return
			var isReturnItems = orderOld.Items.Where(item => item.IsReturnItem);
			var isReturnAllItems = orderItemsAll.Where(item => item.IsReturnItem);
			var productReturn = orderNew.Items.Where(item => item.IsReturnItem).ToList();
			productReturn.AddRange(isReturnItems);
			productReturn.AddRange(isReturnAllItems);

			// Filter item for call api
			foreach (var product in orderItemsAll)
			{
				if (product.IsReturnItem) continue;

				var itemsReturn = productReturn.Where(
					item => ((string.Format("{0}{1}", item.ProductId, item.VariationId)
						== string.Format("{0}{1}", product.ProductId, product.VariationId)))).ToList();

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
				var itemAddSendToApi = itemsObjects.FirstOrDefault(
					product => (product.ItemName == item.ProductName)
						&& (product.ItemPrice == int.Parse(item.ProductPrice.ToString())));

				if (itemAddSendToApi != null)
				{
					itemAddSendToApi.ItemCount = (itemAddSendToApi.ItemCount + item.ItemQuantitySingle);
					continue;
				}

				var itemObject = new TwInvoiceEcPayRequestObject.ItemsObject
				{
					ItemSeq = item.OrderItemNo,
					ItemName = item.ProductName,
					ItemCount = item.ItemQuantity,
					ItemWord = Constants.TWINVOICE_ECPAY_ITEM_WORD,
					ItemPrice = GetItemPrice(item),
					ItemAmount = (GetItemPrice(item) * item.ItemQuantity)
				};
				itemsObjects.Add(itemObject);
			}
			return itemsObjects;
		}

		/// <summary>
		/// Create rq header request object
		/// </summary>
		/// <returns>Rq header object</returns>
		private TwInvoiceEcPayRequestObject.RqHeaderObject CreateRqHeaderRequestObject()
		{
			var rqHeaderObject = new TwInvoiceEcPayRequestObject.RqHeaderObject
			{
				Timestamp = GetTimeStamp(),
				Revision = Constants.TWINVOICE_ECPAY_VISION
			};
			return rqHeaderObject;
		}

		/// <summary>
		/// Get item price
		/// </summary>
		/// <param name="item">Order item information</param>
		/// <returns>Item price</returns>
		private int GetItemPrice(OrderItemModel item)
		{
			var productPriceExcludeTax = 0m;
			var isIncludedTax = (item.ProductTaxIncludedFlg == Constants.FLG_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG_ON);
			if (isIncludedTax)
			{
				var productPriceTax = TaxCalculationUtility.GetTaxPrice(
					item.ProductPrice,
					item.ProductTaxRate,
					Constants.TAX_EXCLUDED_FRACTION_ROUNDING,
					true);
				productPriceExcludeTax = (item.ProductPrice - productPriceTax);
			}

			var itemPrice = isIncludedTax
				? int.Parse(item.ProductPrice.ToString("0"))
				: int.Parse(productPriceExcludeTax.ToString("0"));
			return itemPrice;
		}

		/// <summary>
		/// Get timestamp
		/// </summary>
		/// <returns>Timestamp</returns>
		private string GetTimeStamp()
		{
			var timestamp = (long)(DateTime.Now.ToUniversalTime()
				- new DateTime(1970, 1, 1, 0, 0, 0)).TotalMilliseconds;
			return timestamp.ToString();
		}

		/// <summary>
		/// Create data object
		/// </summary>
		/// <param name="executeType">Execute type</param>
		/// <param name="order">Order information</param>
		/// <param name="orderInvoice">Order ivnoice</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Data object</returns>
		private TwInvoiceEcPayRequestObject.DataObject CreateDataObject(
			ExecuteTypes executeType,
			OrderModel order,
			TwOrderInvoiceModel orderInvoice,
			SqlAccessor accessor)
		{
			var dataObject = new TwInvoiceEcPayRequestObject.DataObject();
			switch (executeType)
			{
				case ExecuteTypes.Issue:
					dataObject = CreateIssueDataRequestObject(order, orderInvoice, accessor);
					break;

				case ExecuteTypes.CheckBarcode:
					dataObject = CreateCheckBarcodeDataRequestObject(orderInvoice);
					break;

				case ExecuteTypes.CheckLoveCode:
					dataObject = CreateCheckLoveCodeDataRequestObject(orderInvoice);
					break;

				case ExecuteTypes.Allowance:
					dataObject = CreateAllowanceDataRequestObject(order, orderInvoice, accessor);
					break;

				case ExecuteTypes.Invalid:
					dataObject = CreateInvalidDataRequestObject(orderInvoice);
					break;
			}
			return dataObject;
		}

		/// <summary>
		/// Get carrier type
		/// </summary>
		/// <param name="carryType">Carry type</param>
		/// <returns>Carrier type</returns>
		private string GetCarrierType(string carryType)
		{
			switch (carryType)
			{
				case Constants.FLG_ORDER_TW_CARRY_TYPE_CERTIFICATE:
					return FLG_CARRIER_TYPE_CERTIFICATE;

				case Constants.FLG_ORDER_TW_CARRY_TYPE_MOBILE:
					return FLG_CARRIER_TYPE_MOBILE;

				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// Get reason type
		/// </summary>
		/// <param name="reason">Reason types</param>
		/// <returns>Reason as string</returns>
		private string GetReasonType(ReasonTypes reason)
		{
			var reasonType = string.Empty;
			switch (reason)
			{
				case ReasonTypes.Cancel:
					reasonType = FLG_REASON_ORDER_CANCEL;
					break;

				case ReasonTypes.Return:
					reasonType = FLG_REASON_ORDER_RETURN;
					break;

				case ReasonTypes.Exchange:
					reasonType = FLG_REASON_ORDER_EXCHANGE;
					break;
			}
			return reasonType;
		}

		/// <summary>
		/// Convert to response
		/// </summary>
		/// <param name="resultObject">Result object</param>
		/// <returns>Response</returns>
		private TwInvoiceEcPayResponse ConvertToResponse(TwInvoiceEcPayResultObject resultObject)
		{
			var rpHeaderObject = new TwInvoiceEcPayResponseObject.RpHeaderObject
			{
				Timestamp = resultObject.RpHeader.Timestamp
			};
			var dataObject = DecodeDataObject(resultObject.Data);
			var responseObject = new TwInvoiceEcPayResponseObject
			{
				PlatformId = resultObject.PlatformId,
				MerchantId = resultObject.MerchantId,
				RpHeader = rpHeaderObject,
				TransCode = resultObject.TransCode,
				TransMsg = resultObject.TransMsg,
				Data = dataObject
			};
			var response = new TwInvoiceEcPayResponse(responseObject);
			return response;
		}

		/// <summary>
		/// Encode data object
		/// </summary>
		/// <param name="dataObject">Data object</param>
		/// <returns>Hash string</returns>
		private string EncodeDataObject(TwInvoiceEcPayRequestObject.DataObject dataObject)
		{
			var requestData = JsonConvert.SerializeObject(
				dataObject,
				new JsonSerializerSettings
				{
					Formatting = Formatting.Indented,
					NullValueHandling = NullValueHandling.Ignore
				});
			var encodedUrl = WebUtility.UrlEncode(requestData);
			var encrypt = EncryptAES128(encodedUrl);
			return encrypt;
		}

		/// <summary>
		/// Decode data object
		/// </summary>
		/// <param name="hashString">Hash string</param>
		/// <returns>Data object</returns>
		private TwInvoiceEcPayResponseObject.DataObject DecodeDataObject(string hashString)
		{
			var dataObject = new TwInvoiceEcPayResponseObject.DataObject();
			if (hashString == null) return dataObject;

			var decrypt = DecryptAES128(hashString);
			var decodedUrl = WebUtility.UrlDecode(decrypt);
			dataObject = JsonConvert.DeserializeObject<TwInvoiceEcPayResponseObject.DataObject>(decodedUrl);
			return dataObject;
		}

		/// <summary>
		/// Encrypt AES128
		/// </summary>
		/// <param name="source">Source</param>
		/// <returns>Hash string</returns>
		private string EncryptAES128(string source)
		{
			var sourceBytes = Encoding.ASCII.GetBytes(source);
			var aes128 = new RijndaelManaged()
			{
				Key = Encoding.ASCII.GetBytes(Constants.TWINVOICE_ECPAY_HASH_KEY),
				IV = Encoding.ASCII.GetBytes(Constants.TWINVOICE_ECPAY_HASH_IV),
				Mode = CipherMode.CBC,
				Padding = PaddingMode.PKCS7,
				BlockSize = 128
			};
			var transform = aes128.CreateEncryptor();
			var hashString = Convert.ToBase64String(transform
				.TransformFinalBlock(
					sourceBytes,
					0,
					sourceBytes.Length));
			return hashString;
		}

		/// <summary>
		/// Decrypt AES128
		/// </summary>
		/// <param name="source">Source</param>
		/// <returns>Hash string</returns>
		private string DecryptAES128(string source)
		{
			var sourceBytes = Convert.FromBase64String(source);
			var aes128 = new RijndaelManaged()
			{
				Key = Encoding.ASCII.GetBytes(Constants.TWINVOICE_ECPAY_HASH_KEY),
				IV = Encoding.ASCII.GetBytes(Constants.TWINVOICE_ECPAY_HASH_IV),
				Mode = CipherMode.CBC,
				Padding = PaddingMode.PKCS7,
				BlockSize = 128
			};
			var transform = aes128.CreateDecryptor();
			var hashString = Encoding.ASCII.GetString(transform
				.TransformFinalBlock(
					sourceBytes,
					0,
					sourceBytes.Length));
			return hashString;
		}

		/// <summary>
		/// Post and receive data
		/// </summary>
		/// <param name="url">Url</param>
		/// <param name="byteData">Post data</param>
		/// <returns>Json string</returns>
		private string PostAndReceiveData(string url, byte[] byteData)
		{
			try
			{
				// Set request header
				var webRequest = (HttpWebRequest)WebRequest.Create(Constants.TWINVOICE_ECPAY_API_URL + url);
				webRequest.Method = "POST";
				webRequest.ContentLength = byteData.Length;

				// Write POST data
				var responseFromServer = string.Empty;
				using (var stream = webRequest.GetRequestStream())
				{
					// Get response
					stream.Write(byteData, 0, byteData.Length);
					using (var response = (HttpWebResponse)webRequest.GetResponse())
					{
						responseFromServer = new StreamReader(response.GetResponseStream()).ReadToEnd();
					}
				}
				return responseFromServer;
			}
			catch (Exception exception)
			{
				FileLogger.WriteError(string.Format(
					"{0} - {1}",
					Constants.TWINVOICE_ECPAY_API_URL,
					exception.Message));
				return string.Empty;
			}
		}

		/// <summary>
		/// Get end point url
		/// </summary>
		/// <param name="executeType">Execute type</param>
		/// <returns>End point url</returns>
		private string GetEndPointUrl(ExecuteTypes executeType)
		{
			switch (executeType)
			{
				case ExecuteTypes.Issue:
					return TWINVOICE_ECPAY_ISSUE_URL;

				case ExecuteTypes.CheckBarcode:
					return TWINVOICE_ECPAY_CHECK_BARCODE_URL;

				case ExecuteTypes.CheckLoveCode:
					return TWINVOICE_ECPAY_CHECK_LOVECODE_URL;

				case ExecuteTypes.Allowance:
					return TWINVOICE_ECPAY_ALLOWANCE_URL;

				case ExecuteTypes.Invalid:
					return TWINVOICE_ECPAY_INVALID_URL;

				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// Is same period
		/// </summary>
		/// <param name="beginDate">Begin date</param>
		/// <param name="endDate">End date</param>
		/// <returns>Is same timing</returns>
		public bool IsSamePeriod(DateTime beginDate, DateTime endDate)
		{
			var year = ((beginDate.Month == MONTH_12)
				? (beginDate.Year + 1)
				: beginDate.Year);

			var period = (((beginDate.Month / 2) == MAX_PERIOD)
				? PERIOD_DEFAULT
				: ((beginDate.Month % 2 == 0)
					? (beginDate.Month / 2 - 1)
					: (beginDate.Month / 2)));

			var invoiceDateEnd = GetInvoiceDateEnd(year, period);
			if (beginDate.Month == MONTH_12) invoiceDateEnd = invoiceDateEnd.AddMonths(-2);

			var result = (endDate <= invoiceDateEnd);
			return result;
		}

		/// <summary>
		/// Get invoice date end
		/// </summary>
		/// <param name="year">Year</param>
		/// <param name="period">Period</param>
		/// <returns>Invoice Date End</returns>
		private DateTime GetInvoiceDateEnd(int year, int period)
		{
			var month = GetMonthEnd(period);
			var result = new DateTime(year, month, EXPIRATION_DATE_DEFAULT).AddMonths(1);
			return result;
		}

		/// <summary>
		/// Get Month End
		/// </summary>
		/// <param name="period">Period</param>
		/// <returns>Month</returns>
		private int GetMonthEnd(int period)
		{
			switch (period)
			{
				case 1:
					return MONTH_4;

				case 2:
					return MONTH_6;

				case 3:
					return MONTH_8;

				case 4:
					return MONTH_10;

				case 5:
					return MONTH_12;

				default:
					return MONTH_2;
			}
		}
		#endregion

		#region Properties
		/// <summary>Reason type</summary>
		private ReasonTypes Reason { get; set; }
		#endregion

		#region TwInvoiceEcPayResponse
		/// <summary>
		/// Tw invoice Ec Pay response
		/// </summary>
		public class TwInvoiceEcPayResponse : ModelBase<TwInvoiceEcPayResponse>
		{
			#region Constructor
			/// <summary>
			/// Constructor
			/// </summary>
			/// <param name="response">Response</param>
			public TwInvoiceEcPayResponse(TwInvoiceEcPayResponseObject response)
			{
				this.Response = response;
			}
			#endregion

			#region Properties
			/// <summary>Message</summary>
			public string Message
			{
				get
				{
					return string.Format(
						"{0}{1}",
						(this.Response.TransCode != RETURN_CODE_SUCCESS_DEFAULT)
							? this.Response.TransMsg
							: string.Empty,
						(this.Response.TransCode != RETURN_CODE_SUCCESS_DEFAULT)
							? string.Format("\r\n{0}", this.Response.Data.RtnMsg)
							: this.Response.Data.RtnMsg);
				}
			}
			/// <summary>Is success</summary>
			public bool IsSuccess
			{
				get
				{
					return ((this.Response.TransCode == RETURN_CODE_SUCCESS_DEFAULT)
						&& (this.Response.Data.RtnCode == RETURN_CODE_SUCCESS_DEFAULT));
				}
			}
			/// <summary>Response</summary>
			public TwInvoiceEcPayResponseObject Response { get; set; }
			/// <summary>電子発票キャンセル</summary>
			public bool IsInvoiceCanceled
			{
				get
				{
					return ((this.Response.Data.RtnCode == RETURN_CODE_CANCELED)
						&& (this.Response.Data.RtnCode == RETURN_CODE_ALLOWANCED)
						&& (this.Response.Data.RtnCode == RETURN_CODE_WTITEOFF));
				}
			}
			#endregion
		}
		#endregion

		#region TwInvoiceEcPayRequestObject
		/// <summary>
		/// Tw invoice Ec Pay request object
		/// </summary>
		[Serializable]
		public class TwInvoiceEcPayRequestObject
		{
			/// <summary>Platform ID</summary>
			[JsonProperty(PropertyName = "PlatformID")]
			public string PlatformId { get; set; }
			/// <summary>Merchant ID</summary>
			[JsonProperty(PropertyName = "MerchantID")]
			public string MerchantId { get; set; }
			/// <summary>Rq header</summary>
			[JsonProperty(PropertyName = "RqHeader")]
			public RqHeaderObject RqHeader { get; set; }
			/// <summary>Data</summary>
			[JsonProperty(PropertyName = "Data")]
			public string Data { get; set; }

			/// <summary>
			/// Rq header object
			/// </summary>
			[Serializable]
			public class RqHeaderObject
			{
				/// <summary>Timestamp</summary>
				[JsonProperty(PropertyName = "Timestamp")]
				public string Timestamp { get; set; }
				/// <summary>Revision</summary>
				[JsonProperty(PropertyName = "Revision")]
				public string Revision { get; set; }
			}

			/// <summary>
			/// Data object
			/// </summary>
			[Serializable]
			public class DataObject
			{
				/// <summary>Merchant ID</summary>
				[JsonProperty(PropertyName = "MerchantID")]
				public string MerchantId { get; set; }
				/// <summary>Relate number</summary>
				[JsonProperty(PropertyName = "RelateNumber")]
				public string RelateNumber { get; set; }
				/// <summary>Customer ID</summary>
				[JsonProperty(PropertyName = "CustomerID")]
				public string CustomerId { get; set; }
				/// <summary>Customer identifier</summary>
				[JsonProperty(PropertyName = "CustomerIdentifier")]
				public string CustomerIdentifier { get; set; }
				/// <summary>Customer name</summary>
				[JsonProperty(PropertyName = "CustomerName")]
				public string CustomerName { get; set; }
				/// <summary>Customer addr</summary>
				[JsonProperty(PropertyName = "CustomerAddr")]
				public string CustomerAddr { get; set; }
				/// <summary>Customer phone</summary>
				[JsonProperty(PropertyName = "CustomerPhone")]
				public string CustomerPhone { get; set; }
				/// <summary>Customer email</summary>
				[JsonProperty(PropertyName = "CustomerEmail")]
				public string CustomerEmail { get; set; }
				/// <summary>Clearance mark</summary>
				[JsonProperty(PropertyName = "ClearanceMark")]
				public string ClearanceMark { get; set; }
				/// <summary>Print</summary>
				[JsonProperty(PropertyName = "Print")]
				public string Print { get; set; }
				/// <summary>Donation</summary>
				[JsonProperty(PropertyName = "Donation")]
				public string Donation { get; set; }
				/// <summary>Love code</summary>
				[JsonProperty(PropertyName = "LoveCode")]
				public string LoveCode { get; set; }
				/// <summary>Carrier type</summary>
				[JsonProperty(PropertyName = "CarrierType")]
				public string CarrierType { get; set; }
				/// <summary>Carrier num</summary>
				[JsonProperty(PropertyName = "CarrierNum")]
				public string CarrierNum { get; set; }
				/// <summary>Tax type</summary>
				[JsonProperty(PropertyName = "TaxType")]
				public string TaxType { get; set; }
				/// <summary>Sales amount</summary>
				[JsonProperty(PropertyName = "SalesAmount")]
				public int? SalesAmount { get; set; }
				/// <summary>Items</summary>
				[JsonProperty(PropertyName = "Items")]
				public ItemsObject[] Items { get; set; }
				/// <summary>Inv type</summary>
				[JsonProperty(PropertyName = "InvType")]
				public string InvType { get; set; }
				/// <summary>Vat</summary>
				[JsonProperty(PropertyName = "vat")]
				public string Vat { get; set; }
				/// <summary>Bar code</summary>
				[JsonProperty(PropertyName = "BarCode")]
				public string BarCode { get; set; }
				/// <summary>Invoice no</summary>
				[JsonProperty(PropertyName = "InvoiceNo")]
				public string InvoiceNo { get; set; }
				/// <summary>Invoice date</summary>
				[JsonProperty(PropertyName = "InvoiceDate")]
				public string InvoiceDate { get; set; }
				/// <summary>Allowance notify</summary>
				[JsonProperty(PropertyName = "AllowanceNotify")]
				public string AllowanceNotify { get; set; }
				/// <summary>Notify mail</summary>
				[JsonProperty(PropertyName = "NotifyMail")]
				public string NotifyMail { get; set; }
				/// <summary>Notify phone</summary>
				[JsonProperty(PropertyName = "NotifyPhone")]
				public string NotifyPhone { get; set; }
				/// <summary>Allowance amount</summary>
				[JsonProperty(PropertyName = "AllowanceAmount")]
				public int? AllowanceAmount { get; set; }
				/// <summary>Reason</summary>
				[JsonProperty(PropertyName = "Reason")]
				public string Reason { get; set; }
			}

			/// <summary>
			/// Items object
			/// </summary>
			[Serializable]
			public class ItemsObject
			{
				/// <summary>Item seq</summary>
				[JsonProperty(PropertyName = "ItemSeq")]
				public int? ItemSeq { get; set; }
				/// <summary>Item name</summary>
				[JsonProperty(PropertyName = "ItemName")]
				public string ItemName { get; set; }
				/// <summary>Item count</summary>
				[JsonProperty(PropertyName = "ItemCount")]
				public int? ItemCount { get; set; }
				/// <summary>Item word</summary>
				[JsonProperty(PropertyName = "ItemWord")]
				public string ItemWord { get; set; }
				/// <summary>Item price</summary>
				[JsonProperty(PropertyName = "ItemPrice")]
				public int? ItemPrice { get; set; }
				/// <summary>Item amount</summary>
				[JsonProperty(PropertyName = "ItemAmount")]
				public int? ItemAmount { get; set; }
			}
		}
		#endregion

		#region TwInvoiceEcPayResultObject
		/// <summary>
		/// Tw invoice Ec Pay result object
		/// </summary>
		[Serializable]
		public class TwInvoiceEcPayResultObject
		{
			/// <summary>Platform ID</summary>
			[JsonProperty(PropertyName = "PlatformID")]
			public string PlatformId { get; set; }
			/// <summary>Merchant ID</summary>
			[JsonProperty(PropertyName = "MerchantID")]
			public string MerchantId { get; set; }
			/// <summary>Rp header</summary>
			[JsonProperty(PropertyName = "RpHeader")]
			public RpHeaderObject RpHeader { get; set; }
			/// <summary>Trans code</summary>
			[JsonProperty(PropertyName = "TransCode")]
			public int TransCode { get; set; }
			/// <summary>Trans msg</summary>
			[JsonProperty(PropertyName = "TransMsg")]
			public string TransMsg { get; set; }
			/// <summary>Data</summary>
			[JsonProperty(PropertyName = "Data")]
			public string Data { get; set; }

			/// <summary>
			/// Rp header object
			/// </summary>
			[Serializable]
			public class RpHeaderObject
			{
				/// <summary>Timestamp</summary>
				[JsonProperty(PropertyName = "Timestamp")]
				public string Timestamp { get; set; }
			}
		}
		#endregion

		#region TwInvoiceEcPayResponseObject
		/// <summary>
		/// Tw invoice Ec Pay response object
		/// </summary>
		[Serializable]
		public class TwInvoiceEcPayResponseObject
		{
			/// <summary>Platform ID</summary>
			[JsonProperty(PropertyName = "PlatformID")]
			public string PlatformId { get; set; }
			/// <summary>Merchant ID</summary>
			[JsonProperty(PropertyName = "MerchantID")]
			public string MerchantId { get; set; }
			/// <summary>Rp header</summary>
			[JsonProperty(PropertyName = "RpHeader")]
			public RpHeaderObject RpHeader { get; set; }
			/// <summary>Trans code</summary>
			[JsonProperty(PropertyName = "TransCode")]
			public int TransCode { get; set; }
			/// <summary>Trans msg</summary>
			[JsonProperty(PropertyName = "TransMsg")]
			public string TransMsg { get; set; }
			/// <summary>Data</summary>
			[JsonProperty(PropertyName = "Data")]
			public DataObject Data { get; set; }

			/// <summary>
			/// Rp header object
			/// </summary>
			[Serializable]
			public class RpHeaderObject
			{
				/// <summary>Timestamp</summary>
				[JsonProperty(PropertyName = "Timestamp")]
				public string Timestamp { get; set; }
			}

			/// <summary>
			/// Data object
			/// </summary>
			[Serializable]
			public class DataObject
			{
				/// <summary>Rtn code</summary>
				[JsonProperty(PropertyName = "RtnCode")]
				public int RtnCode { get; set; }
				/// <summary>Rtn msg</summary>
				[JsonProperty(PropertyName = "RtnMsg")]
				public string RtnMsg { get; set; }
				/// <summary>Invoice no</summary>
				[JsonProperty(PropertyName = "InvoiceNo")]
				public string InvoiceNo { get; set; }
				/// <summary>Invoice date</summary>
				[JsonProperty(PropertyName = "InvoiceDate")]
				public string InvoiceDate { get; set; }
				/// <summary>Random number</summary>
				[JsonProperty(PropertyName = "RandomNumber")]
				public string RandomNumber { get; set; }
				/// <summary>Is exist</summary>
				[JsonProperty(PropertyName = "IsExist")]
				public string IsExist { get; set; }
				/// <summary>IA allow no</summary>
				[JsonProperty(PropertyName = "IA_Allow_No")]
				public string IAAllowNo { get; set; }
				/// <summary>IA invoice no</summary>
				[JsonProperty(PropertyName = "IA_Invoice_No")]
				public string IAInvoiceNo { get; set; }
				/// <summary>IA date</summary>
				[JsonProperty(PropertyName = "IA_Date")]
				public string IADate { get; set; }
				/// <summary>IA remain allowance amt</summary>
				[JsonProperty(PropertyName = "IA_Remain_Allowance_Amt")]
				public int? IARemainAllowanceAmt { get; set; }
			}
		}
		#endregion
	}
}
