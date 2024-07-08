/*
=========================================================================================================
  Module      : 取引登録のリクエスト値(GmoRequestOrderRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using w2.App.Common.Option;
using w2.App.Common.Extensions.Currency;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.GMO.OrderRegister
{
	/// <summary>
	/// 取引登録のリクエスト値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestOrderRegister : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestOrderRegister()
			: base()
		{
			this.HttpInfo = new HttpInfoElement();
			this.Buyer = new BuyerElement();
			this.Deliveries = new DeliveriesElement();
		}
		/// <summary>コンストラクタ</summary>
		/// <param name="order">注文情報</param>
		public GmoRequestOrderRegister(OrderModel order)
			: base()
		{
			this.HttpInfo = new HttpInfoElement();
			this.HttpInfo.DeviceInfo = order.DeviceInfo;
			this.Buyer = new BuyerElement();
			this.Buyer.Address = order.Owner.OwnerAddr1 + order.Owner.OwnerAddr2 + order.Owner.OwnerAddr3 + "　" + order.Owner.OwnerAddr4;
			this.Buyer.BilledAmount = order.LastBilledAmount.ToPriceDecimal().Value;
			this.Buyer.CompanyName = order.Owner.OwnerCompanyName;
			this.Buyer.DepartmentName = order.Owner.OwnerCompanyPostName;
			this.Buyer.Email1 = order.Owner.OwnerMailAddr;
			this.Buyer.Email2 = string.Empty;
			this.Buyer.FullKanaName = order.Owner.OwnerNameKana;
			this.Buyer.FullName = order.Owner.OwnerName;
			this.Buyer.GmoExtend1 = string.Empty;
			this.Buyer.ShopOrderDate = DateTime.Now.ToString("yyyy/MM/dd");
			this.Buyer.ShopTransactionId = order.PaymentOrderId;
			this.Buyer.Tel1 = order.Owner.OwnerTel1;
			this.Buyer.Tel2 = string.Empty;
			this.Buyer.ZipCode = order.Owner.OwnerZip;

			this.Deliveries = new DeliveriesElement();
			this.Deliveries.Delivery = new DeliveryElement();
			this.Deliveries.Delivery.DeliveryCustomer = new DeliveryCustomerElement();
			var orderShipping = order.Shippings[0];
			this.Deliveries.Delivery.DeliveryCustomer.Address = orderShipping.ShippingAddr1 + orderShipping.ShippingAddr2 + orderShipping.ShippingAddr3 + "　" + orderShipping.ShippingAddr4;
			this.Deliveries.Delivery.DeliveryCustomer.CompanyName = orderShipping.ShippingCompanyName;
			this.Deliveries.Delivery.DeliveryCustomer.DepartmentName = orderShipping.ShippingCompanyPostName;
			this.Deliveries.Delivery.DeliveryCustomer.FullKanaName = orderShipping.ShippingNameKana;
			this.Deliveries.Delivery.DeliveryCustomer.FullName = orderShipping.ShippingName;
			this.Deliveries.Delivery.DeliveryCustomer.Tel = orderShipping.ShippingTel1;
			this.Deliveries.Delivery.DeliveryCustomer.ZipCode = orderShipping.ShippingZip;

			this.Deliveries.Delivery.Details = new DetailsElement();

			var detailList = new List<DetailElement>();
			var productdetail = new DetailElement();
			productdetail.DetailName = "お買い上げ商品";
			productdetail.DetailPrice = TaxCalculationUtility.GetPriceTaxIncluded(order.OrderPriceSubtotal, order.OrderPriceSubtotalTax).ToPriceDecimal().Value;
			productdetail.DetailQuantity = 1;
			productdetail.GmoExtend2 = string.Empty;
			productdetail.GmoExtend3 = string.Empty;
			productdetail.GmoExtend4 = string.Empty;

			var shipDetail = new DetailElement();
			shipDetail.DetailName = "送料";
			shipDetail.DetailPrice = order.OrderPriceShipping.ToPriceDecimal().Value;
			shipDetail.DetailQuantity = 1;
			shipDetail.GmoExtend2 = string.Empty;
			shipDetail.GmoExtend3 = string.Empty;
			shipDetail.GmoExtend4 = string.Empty;

			var exchangeDetail = new DetailElement();
			exchangeDetail.DetailName = "決済手数料";
			exchangeDetail.DetailPrice = order.OrderPriceExchange.ToPriceDecimal().Value;
			exchangeDetail.DetailQuantity = 1;
			exchangeDetail.GmoExtend2 = string.Empty;
			exchangeDetail.GmoExtend3 = string.Empty;
			exchangeDetail.GmoExtend4 = string.Empty;

			var discountDetail = new DetailElement();
			discountDetail.DetailName = "割引等";
			discountDetail.DetailPrice = (this.Buyer.BilledAmount - (productdetail.DetailPrice + shipDetail.DetailPrice + exchangeDetail.DetailPrice)).ToPriceDecimal().Value;
			discountDetail.DetailQuantity = 1;
			discountDetail.GmoExtend2 = string.Empty;
			discountDetail.GmoExtend3 = string.Empty;
			discountDetail.GmoExtend4 = string.Empty;

			detailList.Add(productdetail);
			detailList.Add(shipDetail);
			detailList.Add(exchangeDetail);
			detailList.Add(discountDetail);

			this.Deliveries.Delivery.Details.Detail = detailList.ToArray();

			// 決済種別（請求書の同梱か別送かどうか）
			this.Buyer.PaymentType = ((order.InvoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
				? GmoPaymentType.IncludeService
				: GmoPaymentType.SeparateService);
		}
		/// <summary>コンストラクタ</summary>
		/// <param name="cart">カート情報</param>
		public GmoRequestOrderRegister(CartObject cart)
			: base()
		{
			this.HttpInfo = new HttpInfoElement();
			this.HttpInfo.DeviceInfo = cart.DeviceInfo;
			this.Buyer = new Payment.GMO.OrderRegister.BuyerElement();
			this.Buyer.Address = cart.Owner.Addr1 + cart.Owner.Addr2 + cart.Owner.Addr3 + "　" + cart.Owner.Addr4;
			this.Buyer.BilledAmount = cart.PriceTotal.ToPriceDecimal().Value;
			this.Buyer.CompanyName = StringUtility.ToEmpty(cart.Owner.CompanyName);
			this.Buyer.DepartmentName = StringUtility.ToEmpty(cart.Owner.CompanyPostName);
			this.Buyer.Email1 = StringUtility.ToEmpty(cart.Owner.MailAddr);
			this.Buyer.Email2 = string.Empty;
			this.Buyer.FullKanaName = cart.Owner.NameKana;
			this.Buyer.FullName = cart.Owner.Name;
			this.Buyer.GmoExtend1 = "";
			this.Buyer.ShopOrderDate = DateTime.Now.ToString("yyyy/MM/dd");
			this.Buyer.ShopTransactionId = string.Empty;
			this.Buyer.Tel1 = cart.Owner.Tel1;
			this.Buyer.Tel2 = "";
			this.Buyer.ZipCode = cart.Owner.Zip;

			this.Deliveries = new Payment.GMO.OrderRegister.DeliveriesElement();
			this.Deliveries.Delivery = new Payment.GMO.OrderRegister.DeliveryElement();
			this.Deliveries.Delivery.DeliveryCustomer = new Payment.GMO.OrderRegister.DeliveryCustomerElement();
			this.Deliveries.Delivery.DeliveryCustomer.Address = cart.GetShipping().Addr1 + cart.GetShipping().Addr2 + cart.GetShipping().Addr3 + "　" + cart.GetShipping().Addr4;
			this.Deliveries.Delivery.DeliveryCustomer.CompanyName = StringUtility.ToEmpty(cart.GetShipping().CompanyName);
			this.Deliveries.Delivery.DeliveryCustomer.DepartmentName = StringUtility.ToEmpty(cart.GetShipping().CompanyPostName);
			this.Deliveries.Delivery.DeliveryCustomer.FullKanaName = cart.GetShipping().NameKana;
			this.Deliveries.Delivery.DeliveryCustomer.FullName = cart.GetShipping().Name;
			this.Deliveries.Delivery.DeliveryCustomer.Tel = cart.GetShipping().Tel1;
			this.Deliveries.Delivery.DeliveryCustomer.ZipCode = cart.GetShipping().Zip;

			this.Deliveries.Delivery.Details = new Payment.GMO.OrderRegister.DetailsElement();

			var detailList = new List<Payment.GMO.OrderRegister.DetailElement>();
			var productdetail = new Payment.GMO.OrderRegister.DetailElement();
			productdetail.DetailName = "お買い上げ商品";
			productdetail.DetailPrice = TaxCalculationUtility.GetPriceTaxIncluded(cart.PriceSubtotal, cart.PriceSubtotalTax).ToPriceDecimal().Value;
			productdetail.DetailQuantity = 1;
			productdetail.GmoExtend2 = "";
			productdetail.GmoExtend3 = "";
			productdetail.GmoExtend4 = "";

			var shipDetail = new Payment.GMO.OrderRegister.DetailElement();
			shipDetail.DetailName = "送料";
			shipDetail.DetailPrice = cart.PriceShipping.ToPriceDecimal().Value;
			shipDetail.DetailQuantity = 1;
			shipDetail.GmoExtend2 = "";
			shipDetail.GmoExtend3 = "";
			shipDetail.GmoExtend4 = "";

			var exchangeDetail = new Payment.GMO.OrderRegister.DetailElement();
			exchangeDetail.DetailName = "決済手数料";
			exchangeDetail.DetailPrice = cart.Payment.PriceExchange.ToPriceDecimal().Value;
			exchangeDetail.DetailQuantity = 1;
			exchangeDetail.GmoExtend2 = "";
			exchangeDetail.GmoExtend3 = "";
			exchangeDetail.GmoExtend4 = "";

			var discountDetail = new Payment.GMO.OrderRegister.DetailElement();
			discountDetail.DetailName = "割引等";
			discountDetail.DetailPrice = (cart.PriceTotal -
				(TaxCalculationUtility.GetPriceTaxIncluded(cart.PriceSubtotal, cart.PriceSubtotalTax)
				+ cart.PriceShipping + cart.Payment.PriceExchange))
				.ToPriceDecimal().Value;
			discountDetail.DetailQuantity = 1;
			discountDetail.GmoExtend2 = "";
			discountDetail.GmoExtend3 = "";
			discountDetail.GmoExtend4 = "";

			detailList.Add(productdetail);
			detailList.Add(shipDetail);
			detailList.Add(exchangeDetail);
			detailList.Add(discountDetail);

			this.Deliveries.Delivery.Details.Detail = detailList.ToArray();

			// 決済種別（請求書の同梱か別送かどうか）
			this.Buyer.PaymentType = ((cart.GetInvoiceBundleFlg() == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
				? GmoPaymentType.IncludeService
				: GmoPaymentType.SeparateService);
		}

		/// <summary>端末識別情報</summary>
		[XmlElement("httpInfo")]
		public HttpInfoElement HttpInfo;

		/// <summary>購入者情報</summary>
		[XmlElement("buyer")]
		public BuyerElement Buyer;

		/// <summary>配送先項目</summary>
		[XmlElement("deliveries")]
		public DeliveriesElement Deliveries;
	}

	#region HttpInfoElement
	/// <summary>
	/// 端末識別情報要素
	/// </summary>
	public class HttpInfoElement
	{
		/// <summary>コンストラクタ</summary>
		public HttpInfoElement()
		{
			this.HttpHeader = "";
			this.DeviceInfo = "";
		}

		/// <summary>HTTPヘッダ情報</summary>
		/// <remarks>ｇｇｇ</remarks>
		[XmlElement("httpHeader")]
		public string HttpHeader;

		/// <summary>デバイス情報</summary>
		[XmlElement("deviceInfo")]
		public string DeviceInfo;
	}
	#endregion

	#region BuyerElement 購入者情報要素
	/// <summary>
	/// 購入者情報要素
	/// </summary>
	public class BuyerElement
	{
		/// <summary>コンストラクタ</summary>
		public BuyerElement()
		{
			this.ShopTransactionId = "";
			this.ShopOrderDate = "";
			this.FullName = "";
			this.FullKanaName = "";
			this.ZipCode = "";
			this.Address = "";
			this.CompanyName = "";
			this.DepartmentName = "";
			this.Tel1 = "";
			this.Tel2 = "";
			this.Email1 = "";
			this.Email2 = "";
			this.BilledAmount = 0;
			this.GmoExtend1 = "";
			this.PaymentType = GmoPaymentType.IncludeService;
		}

		/// <summary>加盟店取引ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId;

		/// <summary>加盟店注文日</summary>
		[XmlElement("shopOrderDate")]
		public string ShopOrderDate;

		/// <summary>氏名（漢字）</summary>
		[XmlElement("fullName")]
		public string FullName;

		/// <summary>氏名（カナ）</summary>
		[XmlElement("FullKanaName")]
		public string FullKanaName;

		/// <summary>郵便番号</summary>
		[XmlElement("zipCode")]
		public string ZipCode;

		/// <summary>住所</summary>
		[XmlElement("address")]
		public string Address;

		/// <summary>会社名</summary>
		[XmlElement("companyName")]
		public string CompanyName;

		/// <summary>部署名</summary>
		[XmlElement("departmentName")]
		public string DepartmentName;

		/// <summary>電話番号１</summary>
		[XmlElement("tel1")]
		public string Tel1;

		/// <summary>電話番号２</summary>
		[XmlElement("tel2")]
		public string Tel2;

		/// <summary>メールアドレス１</summary>
		[XmlElement("email1")]
		public string Email1;

		/// <summary>メールアドレス２</summary>
		[XmlElement("email2")]
		public string Email2;

		/// <summary>顧客請求額</summary>
		[XmlElement("billedAmount")]
		public decimal BilledAmount;

		/// <summary>GMO拡張項目１</summary>
		[XmlElement("gmoExtend1")]
		public string GmoExtend1;

		/// <summary>決済種別</summary>
		[XmlElement("paymentType")]
		public GmoPaymentType PaymentType;
	}
	#endregion

	#region DeliveriesElement 配送先項目要素
	/// <summary>
	/// 配送先項目要素
	/// </summary>
	public class DeliveriesElement
	{
		/// <summary>コンストラクタ</summary>
		public DeliveriesElement()
		{
			this.Delivery = new DeliveryElement();
		}

		/// <summary>配送先情報</summary>
		[XmlElement("delivery")]
		public DeliveryElement Delivery;
	}
	#endregion

	#region DeliveryElement 配送先情報要素
	/// <summary>
	/// 配送先情報要素
	/// </summary>
	public class DeliveryElement
	{
		/// <summary>コンストラクタ</summary>
		public DeliveryElement()
		{
			this.DeliveryCustomer = new DeliveryCustomerElement();
			this.Details = new DetailsElement();
		}

		/// <summary>配送先顧客情報</summary>
		[XmlElement("deliveryCustomer")]
		public DeliveryCustomerElement DeliveryCustomer;

		/// <summary>明細項目</summary>
		[XmlElement("details")]
		public DetailsElement Details;
	}
	#endregion

	#region DeliveryCustomerElement 配送先顧客情報
	/// <summary>
	/// 配送先顧客情報
	/// </summary>
	public class DeliveryCustomerElement
	{
		/// <summary>コンストラクタ</summary>
		public DeliveryCustomerElement()
		{
			this.FullName = "";
			this.FullKanaName = "";
			this.ZipCode = "";
			this.Address = "";
			this.CompanyName = "";
			this.DepartmentName = "";
			this.Tel = "";
		}

		/// <summary>氏名（漢字）</summary>
		[XmlElement("fullName")]
		public string FullName;

		/// <summary>氏名（カナ）</summary>
		[XmlElement("fullKanaName")]
		public string FullKanaName;

		/// <summary>郵便番号</summary>
		[XmlElement("zipCode")]
		public string ZipCode;

		/// <summary>住所</summary>
		[XmlElement("address")]
		public string Address;

		/// <summary>会社名</summary>
		[XmlElement("companyName")]
		public string CompanyName;

		/// <summary>部署名</summary>
		[XmlElement("departmentName")]
		public string DepartmentName;

		/// <summary>電話番号</summary>
		[XmlElement("tel")]
		public string Tel;
	}
	#endregion

	#region DetailsElement 明細項目要素
	/// <summary>
	/// 明細項目要素
	/// </summary>
	public class DetailsElement
	{
		/// <summary>コンストラクタ</summary>
		public DetailsElement()
		{
			this.Detail = new DetailElement[] { new DetailElement() };
		}

		/// <summary>明細詳細情報</summary>
		[XmlElement("detail")]
		public DetailElement[] Detail;
	}
	#endregion

	#region DetailElement 明細詳細情報要素
	/// <summary>
	/// 明細詳細情報要素
	/// </summary>
	public class DetailElement
	{
		/// <summary>コンストラクタ</summary>
		public DetailElement()
		{
			this.DetailName = "";
			this.DetailPrice = 0;
			this.DetailQuantity = 0;
			this.GmoExtend2 = "";
			this.GmoExtend3 = "";
			this.GmoExtend4 = "";
		}

		/// <summary>明細名</summary>
		[XmlElement("detailName")]
		public string DetailName;

		/// <summary>単価</summary>
		[XmlElement("detailPrice")]
		public decimal DetailPrice;

		/// <summary>数量</summary>
		[XmlElement("detailQuantity")]
		public decimal DetailQuantity;

		/// <summary>GMO拡張項目2</summary>
		[XmlElement("gmoExtend2")]
		public string GmoExtend2;

		/// <summary>GMO拡張項目3</summary>
		[XmlElement("gmoExtend3")]
		public string GmoExtend3;

		/// <summary>GMO拡張項目4</summary>
		[XmlElement("gmoExtend4")]
		public string GmoExtend4;

	}
	#endregion
}
