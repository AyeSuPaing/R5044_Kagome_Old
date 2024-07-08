/*
=========================================================================================================
  Module      : OrderImporterリクエスト (OrderImporterRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using w2.App.Common.Product;
using w2.Domain.Order;

namespace w2.App.Common.Recustomer.OrderImporter
{
	/// <summary>
	/// OrderImporterリクエスト
	/// </summary>
	[Serializable]
	public class OrderImporterRequest : RecustomerApiPostRequestBase
	{
		/// <summary>発送ステータス:発送済み</summary>
		private const string CONST_RECUSTOMER_STATUS_FULFILLED = "fulfilled";
		/// <summary>支払いステータス:保留中</summary>
		private const string CONST_RECUSTOMER_FINANCIAL_STATUS_PENDING = "pending";
		/// <summary>支払いステータス:支払い済み</summary>
		private const string CONST_RECUSTOMER_FINANCIAL_STATUS_PAID = "paid";
		/// <summary>支払いステータス:一部支払い済み</summary>
		private const string CONST_RECUSTOMER_FINANCIAL_STATUS_PARTIALLY_PAID = "partially_paid";
		/// <summary>発送状況 :処理中</summary>
		private const string CONST_RECUSTOMER_FULFILLMENT_STATUS_OPEN = "open";
		/// <summary>取引ステータス:成功</summary>
		private const string CONST_RECUSTOMER_TRANSACTION_STATUS_SUCCESS = "success";
		/// <summary>通貨単位:日本円</summary>
		private const string CONST_RECUSTOMER_CURRENCY_JPY = "JPY";
		/// <summary>日付形式フォーマット</summary>
		private const string CONST_RECUSTOMER_DATE_FORMAT = "yyyy-MM-ddTHH:mm:sszzz";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderImporterRequest()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="shippedDate">出荷完了日</param>
		public OrderImporterRequest(OrderModel order, string shippedDate)
		{
			this.Id = string.IsNullOrEmpty(Constants.RECUSTOMER_API_PREFIX)
				? order.OrderId
				: Constants.RECUSTOMER_API_PREFIX + "_" + order.OrderId;
			this.OrderNumber = order.OrderId;
			this.Status = CONST_RECUSTOMER_STATUS_FULFILLED;
			this.FulfillmentStatus = CONST_RECUSTOMER_STATUS_FULFILLED;
			this.FinancialStatus = GetFinancialStatus(order.OrderPaymentStatus);
			this.OrderDiscountAmount = ((int)(order.OrderPriceDiscountTotal - order.OrderPointUseYen)).ToString();
			this.OrderPointPrice = ((int)order.OrderPointUseYen).ToString();
			this.TotalItemDiscountAmount = "0";
			this.SubtotalPrice = ((int)order.OrderPriceSubtotal).ToString();
			this.TotalShippingPrice = ((int)order.OrderPriceShipping).ToString();
			// 全ての割引合計
			this.TotalDiscount = ((int)(order.OrderPriceDiscountTotal)).ToString();
			this.AdditionalFeesAmount = ((int)(order.OrderPriceExchange + order.OrderPriceRegulation)).ToString();
			this.TotalPrice = ((int)order.OrderPriceTotal).ToString();
			this.TotalTax = ((int)order.OrderPriceTax).ToString();
			this.IsTaxIncluded = Constants.MANAGEMENT_INCLUDED_TAX_FLAG;
			this.Currency = CONST_RECUSTOMER_CURRENCY_JPY;
			this.Quantity = order.OrderProductCount;
			this.CreatedAt
				= order.OrderDate.HasValue
					? order.OrderDate.Value.ToString(CONST_RECUSTOMER_DATE_FORMAT)
					: order.DateCreated.ToString(CONST_RECUSTOMER_DATE_FORMAT);
			this.Customer = CreateCustomer(order);
			this.LineItems = CreateLineItems(order);
			this.ShippingAddress = CreateShippingAddress(order);
			this.BillingAddress = CreateBillingAddress(order);
			this.Fulfillments = CreateFulfillments(order, shippedDate);
			this.Transactions = CreateTransactions(order);
		}

		/// <summary>id</summary>
		[JsonProperty("id")]
		public string Id { get; set; }
		/// <summary>order_number</summary>
		[JsonProperty("order_number")]
		public string OrderNumber { get; set; }
		/// <summary>status</summary>
		[JsonProperty("status")]
		public string Status { get; set; }
		/// <summary>fulfillment_status</summary>
		[JsonProperty("fulfillment_status")]
		public string FulfillmentStatus { get; set; }
		/// <summary>financial_status</summary>
		[JsonProperty("financial_status")]
		public string FinancialStatus { get; set; }
		/// <summary>order_discount_amount</summary>
		[JsonProperty("order_discount_amount")]
		public string OrderDiscountAmount { get; set; }
		/// <summary>order_point_price</summary>
		[JsonProperty("order_point_price")]
		public string OrderPointPrice { get; set; }
		/// <summary>total_item_discount_amount</summary>
		[JsonProperty("total_item_discount_amount")]
		public string TotalItemDiscountAmount { get; set; }
		/// <summary>subtotal_price</summary>
		[JsonProperty("subtotal_price")]
		public string SubtotalPrice { get; set; }
		/// <summary>total_shipping_price</summary>
		[JsonProperty("total_shipping_price")]
		public string TotalShippingPrice { get; set; }
		/// <summary>total_discount</summary>
		[JsonProperty("total_discount")]
		public string TotalDiscount { get; set; }
		/// <summary>additional_fees_amount</summary>
		[JsonProperty("additional_fees_amount")]
		public string AdditionalFeesAmount { get; set; }
		/// <summary>total_price</summary>
		[JsonProperty("total_price")]
		public string TotalPrice { get; set; }
		/// <summary>total_tax</summary>
		[JsonProperty("total_tax")]
		public string TotalTax { get; set; }
		/// <summary>is_tax_included</summary>
		[JsonProperty("is_tax_included")]
		public bool IsTaxIncluded { get; set; }
		/// <summary>currency</summary>
		[JsonProperty("currency")]
		public string Currency { get; set; }
		/// <summary>quantity</summary>
		[JsonProperty("quantity")]
		public int Quantity { get; set; }
		/// <summary>created_at</summary>
		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }
		/// <summary>customer</summary>
		[JsonProperty("customer")]
		public Customer Customer { get; set; }
		/// <summary>line_items</summary>
		[JsonProperty("line_items")]
		public LineItem[] LineItems { get; set; }
		/// <summary>shipping_address</summary>
		[JsonProperty("shipping_address")]
		public ShippingAddress ShippingAddress { get; set; }
		/// <summary>billing_address</summary>
		[JsonProperty("billing_address")]
		public BillingAddress BillingAddress { get; set; }
		/// <summary>fulfillments</summary>
		[JsonProperty("fulfillments")]
		public Fulfillment[] Fulfillments { get; set; }
		/// <summary>transactions</summary>
		[JsonProperty("transactions")]
		public Transaction[] Transactions { get; set; }

		/// <summary>
		/// 支払いステータス取得
		/// </summary>
		/// <param name="orderPaymentStatus">入金ステータス</param>
		/// <returns>支払いステータス</returns>
		private string GetFinancialStatus(string orderPaymentStatus)
		{
			var status = CONST_RECUSTOMER_FINANCIAL_STATUS_PENDING;
			switch (orderPaymentStatus)
			{
				case Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_CONFIRM:
					status = CONST_RECUSTOMER_FINANCIAL_STATUS_PENDING;
					break;

				case Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE:
					status = CONST_RECUSTOMER_FINANCIAL_STATUS_PAID;
					break;

				case Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_SHORTAGE:
					status = CONST_RECUSTOMER_FINANCIAL_STATUS_PARTIALLY_PAID;
					break;
			}

			return status;
		}

		/// <summary>
		/// Customer作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>Customer</returns>
		private Customer CreateCustomer(OrderModel order)
		{
			var customer = new Customer
			{
				Email = order.Owner.OwnerMailAddr,
				FullName = order.Owner.OwnerName,
				CreatedAt = order.Owner.DateCreated.ToString(CONST_RECUSTOMER_DATE_FORMAT),
				UpdateAt = order.Owner.DateChanged.ToString(CONST_RECUSTOMER_DATE_FORMAT),
			};

			return customer;
		}

		/// <summary>
		/// LineItems作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>LineItems</returns>
		private LineItem[] CreateLineItems(OrderModel order)
		{
			var lineItemsList = new List<LineItem>();
			foreach(var item in order.Items)
			{
				var productOptionSettings = GetProductOptionSettingList(item.ShopId, item.ProductId, item.ProductOptionTexts);
				lineItemsList.Add(new LineItem
				{
					Id = item.OrderId + "_" + item.OrderItemNo,
					ProductId = item.ProductId,
					ProductName = GetProductName(item, productOptionSettings),
					Sku = item.VariationId,
					ItemDiscountPrice = "0",
					ItemDiscountAmount = "0",
					ItemPrice = ((int)item.ProductPrice + (int)productOptionSettings.SelectedOptionTotalPrice).ToString(),
					ItemCompareAtPrice = ((int)item.ProductPrice + (int)productOptionSettings.SelectedOptionTotalPrice).ToString(),
					ItemQuantity = item.ItemQuantity.ToString(),
					ItemPriceAmount = ((int)item.ItemPrice).ToString(),
					ItemTaxPrice = ((int)item.ItemPriceTax).ToString(),
					IsTaxIncluded = Constants.MANAGEMENT_INCLUDED_TAX_FLAG,
					FullfillmentStatus = CONST_RECUSTOMER_STATUS_FULFILLED,
					CreatedAt = item.DateCreated.ToString(CONST_RECUSTOMER_DATE_FORMAT),
					UpdatedAt = item.DateChanged.ToString(CONST_RECUSTOMER_DATE_FORMAT),
				});
			}
			return lineItemsList.ToArray();
		}

		/// <summary>
		/// ShippingAddress作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>ShippingAddress</returns>
		private ShippingAddress CreateShippingAddress(OrderModel order)
		{
			var shippingAddress = new ShippingAddress();
			// ギフトは連携しないため0固定
			var shipping = order.Shippings[0];
			shippingAddress.FullName = shipping.ShippingName;
			shippingAddress.Address = new Address
			{
				PostalCode = shipping.ShippingZip,
				Province = shipping.ShippingAddr1,
				City = shipping.ShippingAddr2,
				Address1 = shipping.ShippingAddr3,
				Address2 = shipping.ShippingAddr4,
			};
			return shippingAddress;
		}

		/// <summary>
		/// BillingAddress作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>BillingAddress</returns>
		private BillingAddress CreateBillingAddress(OrderModel order)
		{
			var billingAddress = new BillingAddress();

			billingAddress.FullName = order.Owner.OwnerName;
			billingAddress.Address = new Address
			{
				PostalCode = order.Owner.OwnerZip,
				Province = order.Owner.OwnerAddr1,
				City = order.Owner.OwnerAddr2,
				Address1 = order.Owner.OwnerAddr3,
				Address2 = order.Owner.OwnerAddr4,
			};
			return billingAddress;
		}

		/// <summary>
		/// Fulfillments作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="shippedDate">出荷完了日</param>
		/// <returns>Fulfillments</returns>
		private Fulfillment[] CreateFulfillments(OrderModel order, string shippedDate)
		{
			// ギフトは連携しないため0固定
			var shipping = order.Shippings[0];
			var fulfillments = new Fulfillment[]
			{
				new Fulfillment
				{
					Id = shipping.OrderId + "_" + shipping.OrderShippingNo,
					Status = CONST_RECUSTOMER_FULFILLMENT_STATUS_OPEN,
					TrackingNumber = shipping.ShippingCheckNo,
					FulfillmentLineItems = CreateFulfillmentLineItems(order),
					CreatedAt = DateTime.Parse(shippedDate).ToString(CONST_RECUSTOMER_DATE_FORMAT),
					UpdatedAt = DateTime.Parse(shippedDate).ToString(CONST_RECUSTOMER_DATE_FORMAT),
				}
			};
			return fulfillments;
		}

		/// <summary>
		/// Transactions作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>Transactions</returns>
		private Transaction[] CreateTransactions(OrderModel order)
		{
			var transactions = new Transaction[]
			{
				new Transaction
				{
					Id = order.OrderId + "_1",
					Amount = ((int)order.OrderPriceTotal).ToString(),
					Currency = CONST_RECUSTOMER_CURRENCY_JPY,
					Status = CONST_RECUSTOMER_TRANSACTION_STATUS_SUCCESS,
					Gateway = order.OrderPaymentKbn,
				}
			};
			return transactions;
		}

		/// <summary>
		/// FulfillmentLineItems作成
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>FulfillmentLineItems</returns>
		private FulfillmentLineItem[] CreateFulfillmentLineItems(OrderModel order)
		{
			var fullFillmentLineItemsList = new List<FulfillmentLineItem>();
			foreach (var item in order.Items)
			{
				fullFillmentLineItemsList.Add(new FulfillmentLineItem
				{
					Id = item.OrderId + "_" + item.OrderItemNo,
					OrderLineItemId = item.OrderId + "_" + item.OrderItemNo,
					Quantity = item.ItemQuantity.ToString(),
				});
			}
			return fullFillmentLineItemsList.ToArray();
		}

		/// <summary>
		/// 商品付帯情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="productOptionValue">商品付帯情報文字列</param>
		/// <returns>商品付帯情報</returns>
		public ProductOptionSettingList GetProductOptionSettingList(string shopId, string productId, string productOptionValue)
		{
			var productOptionListSetting = new ProductOptionSettingList(shopId, productId);
			productOptionListSetting.SetDefaultValueFromProductOptionTexts(productOptionValue, itemsToAddAsSettings:true , onlySelectedItems:true);
			return productOptionListSetting;
		}

		/// <summary>
		/// 商品名取得(付帯情報がある場合は末尾に付与)
		/// </summary>
		/// <param name="item">注文商品情報</param>
		/// <param name="productOptionSettingList">商品付帯情報</param>
		/// <returns>商品名</returns>
		public string GetProductName(OrderItemModel item, ProductOptionSettingList productOptionSettingList)
		{
			var productName = item.ProductName;
			var productOptionText = productOptionSettingList.GetDisplayProductOptionSettingSelectValues();
			var productNameWithProductOptionText
				= (string.IsNullOrEmpty(productOptionText) == false)
					? productName + "　" + productOptionText
					: productName;

			// Recustomer側の文字数制限が256文字のため、超える場合は切り落とす(productNameが最大200文字のため、商品名が欠けることはない)
			var cooperationProductName = (productNameWithProductOptionText.Length > 256) ? productNameWithProductOptionText.Substring(0, 256) : productNameWithProductOptionText;
			return cooperationProductName;
		}
	}

	/// <summary>
	/// Customerクラス
	/// </summary>
	[Serializable]
	public class Customer : RecustomerApiPostRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Customer()
		{
		}

		/// <summary>email</summary>
		[JsonProperty("email")]
		public string Email { get; set; }
		/// <summary>full_name</summary>
		[JsonProperty("full_name")]
		public string FullName { get; set; }
		/// <summary>created_at</summary>
		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }
		/// <summary>updated_at</summary>
		[JsonProperty("updated_at")]
		public string UpdateAt { get; set; }
	}

	/// <summary>
	/// LineItemクラス
	/// </summary>
	[Serializable]
	public class LineItem : RecustomerApiPostRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LineItem()
		{
		}

		/// <summary>id</summary>
		[JsonProperty("id")]
		public string Id { get; set; }
		/// <summary>product_id</summary>
		[JsonProperty("product_id")]
		public string ProductId { get; set; }
		/// <summary>product_name</summary>
		[JsonProperty("product_name")]
		public string ProductName { get; set; }
		/// <summary>sku</summary>
		[JsonProperty("sku")]
		public string Sku { get; set; }
		/// <summary>item_discount_price</summary>
		[JsonProperty("item_discount_price")]
		public string ItemDiscountPrice { get; set; }
		/// <summary>item_discount_amount</summary>
		[JsonProperty("item_discount_amount")]
		public string ItemDiscountAmount { get; set; }
		/// <summary>item_price</summary>
		[JsonProperty("item_price")]
		public string ItemPrice { get; set; }
		/// <summary>item_compare_at_price</summary>
		[JsonProperty("item_compare_at_price")]
		public string ItemCompareAtPrice { get; set; }
		/// <summary>item_quantity</summary>
		[JsonProperty("item_quantity")]
		public string ItemQuantity { get; set; }
		/// <summary>item_price_amount</summary>
		[JsonProperty("item_price_amount")]
		public string ItemPriceAmount { get; set; }
		/// <summary>item_tax_price</summary>
		[JsonProperty("item_tax_price")]
		public string ItemTaxPrice { get; set; }
		/// <summary>is_tax_included</summary>
		[JsonProperty("is_tax_included")]
		public bool IsTaxIncluded { get; set; }
		/// <summary>fulfillment_status</summary>
		[JsonProperty("fulfillment_status")]
		public string FullfillmentStatus { get; set; }
		/// <summary>created_at</summary>
		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }
		/// <summary>updated_at</summary>
		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	/// <summary>
	/// ShippingAddressクラス
	/// </summary>
	[Serializable]
	public class ShippingAddress : RecustomerApiPostRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ShippingAddress()
		{
		}

		/// <summary>full_name</summary>
		[JsonProperty("full_name")]
		public string FullName { get; set; }
		/// <summary>address</summary>
		[JsonProperty("address")]
		public Address Address { get; set; }
	}

	/// <summary>
	/// BillingAddressクラス
	/// </summary>
	[Serializable]
	public class BillingAddress : RecustomerApiPostRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public BillingAddress()
		{
		}

		/// <summary>full_name</summary>
		[JsonProperty("full_name")]
		public string FullName { get; set; }
		/// <summary>address</summary>
		[JsonProperty("address")]
		public Address Address { get; set; }
	}

	/// <summary>
	/// Fulfillmentクラス
	/// </summary>
	[Serializable]
	public class Fulfillment : RecustomerApiPostRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Fulfillment()
		{
		}

		/// <summary>id</summary>
		[JsonProperty("id")]
		public string Id { get; set; }
		/// <summary>status</summary>
		[JsonProperty("status")]
		public string Status { get; set; }
		/// <summary>tracking_number</summary>
		[JsonProperty("tracking_number")]
		public string TrackingNumber { get; set; }
		/// <summary>fulfillment_line_items</summary>
		[JsonProperty("fulfillment_line_items")]
		public FulfillmentLineItem[] FulfillmentLineItems { get; set; }
		/// <summary>created_at</summary>
		[JsonProperty("created_at")]
		public string CreatedAt { get; set; }
		/// <summary>updated_at</summary>
		[JsonProperty("updated_at")]
		public string UpdatedAt { get; set; }
	}

	/// <summary>
	/// Addressクラス
	/// </summary>
	[Serializable]
	public class Address : RecustomerApiPostRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Address()
		{
		}

		/// <summary>postal_code</summary>
		[JsonProperty("postal_code")]
		public string PostalCode { get; set; }
		/// <summary>province</summary>
		[JsonProperty("province")]
		public string Province { get; set; }
		/// <summary>city</summary>
		[JsonProperty("city")]
		public string City { get; set; }
		/// <summary>address1</summary>
		[JsonProperty("address1")]
		public string Address1 { get; set; }
		/// <summary>address2</summary>
		[JsonProperty("address2")]
		public string Address2 { get; set; }
	}

	/// <summary>
	/// FulfillmentLineItemクラス
	/// </summary>
	[Serializable]
	public class FulfillmentLineItem : RecustomerApiPostRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FulfillmentLineItem()
		{
		}

		/// <summary>id</summary>
		[JsonProperty("id")]
		public string Id { get; set; }
		/// <summary>order_line_item_id</summary>
		[JsonProperty("order_line_item_id")]
		public string OrderLineItemId { get; set; }
		/// <summary>quantity</summary>
		[JsonProperty("quantity")]
		public string Quantity { get; set; }
	}

	[Serializable]
	public class Transaction : RecustomerApiPostRequestBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Transaction()
		{
		}

		/// <summary>id</summary>
		[JsonProperty("id")]
		public string Id { get; set; }
		/// <summary>amount</summary>
		[JsonProperty("amount")]
		public string Amount { get; set; }
		/// <summary>currency</summary>
		[JsonProperty("currency")]
		public string Currency { get; set; }
		/// <summary>status</summary>
		[JsonProperty("status")]
		public string Status { get; set; }
		/// <summary>gateway</summary>
		[JsonProperty("gateway")]
		public string Gateway { get; set; }
	}
}
