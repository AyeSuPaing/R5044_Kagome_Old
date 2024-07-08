/*
=========================================================================================================
  Module      : 取引登録のリクエスト値(GmoRequestTransactionRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using w2.App.Common.Option;
using w2.App.Common.Extensions.Currency;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UserBusinessOwner;
using w2.Domain.User.Helper;

namespace w2.App.Common.Order.Payment.GMO.TransactionRegister
{
	/// <summary>
	/// 取引登録のリクエスト値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestTransactionRegister : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestTransactionRegister()
			: base()
		{
			this.HttpInfo = new HttpInfoElement();
			this.Buyer = new BuyerElement();
			this.Taxes = new Taxes[] { new Taxes() };
			this.TaxesSum = new TaxesSum[] { new TaxesSum() };
			this.Deliveries = new DeliveriesElement();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート情報</param>
		public GmoRequestTransactionRegister(CartObject cart)
			: base()
		{
			this.HttpInfo = new HttpInfoElement();
			this.HttpInfo.DeviceInfo = TrimDeviceInfo(cart.DeviceInfo);
			this.Buyer = new BuyerElement();
			this.Buyer.ShopTransactionId = string.Empty;
			this.Buyer.ShopOrderDate = DateTime.Now.ToString("yyyy/MM/dd");
			this.Buyer.BuyerNameFamily = cart.Owner.Name1;
			this.Buyer.BuyerName = cart.Owner.Name2;
			this.Buyer.BuyerNameFamilyKana = StringUtility.ToZenkakuKatakana(cart.Owner.NameKana1);
			this.Buyer.BuyerNameKana = StringUtility.ToZenkakuKatakana(cart.Owner.NameKana2);
			this.Buyer.ZipCode = cart.Owner.Zip;
			this.Buyer.Address = AddressHelper.ConcatenateAddressWithCountryName(cart.Owner.Addr1, cart.Owner.Addr2, cart.Owner.Addr3, cart.Owner.Addr4, cart.Owner.AddrCountryName);
			this.Buyer.CompanyName = cart.Owner.CompanyName;
			this.Buyer.DepartmentName = cart.Owner.CompanyPostName;
			this.Buyer.Tel1 = cart.Owner.Tel1;
			this.Buyer.Tel2 = cart.Owner.Tel2;
			this.Buyer.Email1 = cart.Owner.MailAddr;
			this.Buyer.Email2 = cart.Owner.MailAddr2;
			this.Buyer.BilledAmount = cart.PriceTotal.ToPriceString() ;
			this.Buyer.GmoExtend1 = string.Empty;

			var userBusinessOwnerService = new UserBusinessOwnerService();
			var userBusinessOwner = userBusinessOwnerService.GetByUserId(cart.CartUserId);
			this.Buyer.ShopCustomerId = cart.OrderUserId;
			if (userBusinessOwner != null)
			{
				this.Buyer.PresidentNameFamily = userBusinessOwner.OwnerName1;
				this.Buyer.PresidentName = userBusinessOwner.OwnerName2;
				this.Buyer.PresidentNameFamilyKana = StringUtility.ToZenkakuKatakana(userBusinessOwner.OwnerNameKana1);
				this.Buyer.PresidentNameKana = StringUtility.ToZenkakuKatakana(userBusinessOwner.OwnerNameKana2);
				this.Buyer.Birthday = userBusinessOwner.Birth.Value.ToString("yyyy/MM/dd");
				if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
				{
					this.Buyer.ShopCustomerId = userBusinessOwner.ShopCustomerId;
				}
			}
			else
			{
				this.Buyer.PresidentNameFamily = string.Empty;
				this.Buyer.PresidentName = string.Empty;
				this.Buyer.PresidentNameFamilyKana = string.Empty;
				this.Buyer.PresidentNameKana = string.Empty;
				this.Buyer.Birthday = string.Empty;
				if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
				{
					this.Buyer.ShopCustomerId = string.Empty;
				}
			}
			this.Buyer.BuyCount = cart.Items.Count;
			this.Buyer.BuyAmountTotal = cart.PriceTotal.ToPriceString();

			// Devlieries informations
			this.Deliveries = new DeliveriesElement();
			this.Deliveries.Delivery = new DeliveryElement();
			var shipping = cart.GetShipping();
			this.Deliveries.Delivery.DeliveryCustomer = new DeliveryCustomerElement();
			this.Deliveries.Delivery.DeliveryCustomer.Address = AddressHelper.ConcatenateAddressWithCountryName(shipping.Addr1, shipping.Addr2, shipping.Addr3, shipping.Addr4, shipping.ShippingCountryName);
			this.Deliveries.Delivery.DeliveryCustomer.CompanyName = shipping.CompanyName;
			this.Deliveries.Delivery.DeliveryCustomer.DepartmentName = shipping.CompanyPostName;
			this.Deliveries.Delivery.DeliveryCustomer.FullKanaName = StringUtility.ToZenkakuKatakana(shipping.NameKana);
			this.Deliveries.Delivery.DeliveryCustomer.FullName = shipping.Name;
			this.Deliveries.Delivery.DeliveryCustomer.Tel = shipping.Tel1;
			this.Deliveries.Delivery.DeliveryCustomer.ZipCode = shipping.Zip;

			//Taxes
			var listTaxes = new List<OrderPriceByTaxRateModel>();
			foreach (var tax in cart.PriceInfoByTaxRate)
			{
				listTaxes.Add(tax.CreateModel());
			}
			this.CreateTaxesParam(listTaxes);

			//Product details
			this.Deliveries.Delivery.Details = new DetailsElement();
			var detailList = new List<DetailElement>();
			foreach (var product in cart.Items)
			{
				var productdetail = new DetailElement();
				productdetail.DetailName = product.ProductName;
				productdetail.DetailPrice = product.Price.ToPriceString();
				productdetail.DetailQuantity = product.Count;
				productdetail.ItemPrice = product.PriceSubtotal.ToPriceString();
				productdetail.FeeTaxType = product.TaxRate == 8 ? "1" : "2";
				productdetail.GmoExtend2 = string.Empty;
				productdetail.GmoExtend3 = string.Empty;
				productdetail.GmoExtend4 = string.Empty;
				productdetail.DetailBrand = string.Empty;
				productdetail.DetailCategory = string.Empty;
				detailList.Add(productdetail);
			}

			this.Deliveries.Delivery.Details.Detail = detailList.ToArray();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public GmoRequestTransactionRegister(OrderModel order)
			: base()
		{
			this.HttpInfo = new HttpInfoElement();
			this.HttpInfo.DeviceInfo = TrimDeviceInfo(order.DeviceInfo);
			this.Buyer = new BuyerElement();
			this.Buyer.Address = AddressHelper.ConcatenateAddressWithCountryName(order.Owner.OwnerAddr1, order.Owner.OwnerAddr2, order.Owner.OwnerAddr3, order.Owner.OwnerAddr4, order.Owner.OwnerAddrCountryName);
			this.Buyer.BilledAmount = order.LastBilledAmount.ToPriceString();
			this.Buyer.CompanyName = order.Owner.OwnerCompanyName;
			this.Buyer.DepartmentName = order.Owner.OwnerCompanyPostName;
			this.Buyer.Email1 = order.Owner.OwnerMailAddr;
			this.Buyer.Email2 = order.Owner.OwnerMailAddr2;
			this.Buyer.ShopOrderDate = DateTime.Now.ToString("yyyy/MM/dd");
			this.Buyer.ShopTransactionId = order.PaymentOrderId;
			this.Buyer.Tel1 = order.Owner.OwnerTel1;
			this.Buyer.Tel2 = order.Owner.OwnerTel2;
			this.Buyer.ZipCode = order.Owner.OwnerZip;
			this.Buyer.BuyerNameFamily = order.Owner.OwnerName1;
			this.Buyer.BuyerName = order.Owner.OwnerName2;
			this.Buyer.BuyerNameFamilyKana = StringUtility.ToZenkakuKatakana(order.Owner.OwnerNameKana1);
			this.Buyer.BuyerNameKana = StringUtility.ToZenkakuKatakana(order.Owner.OwnerNameKana2);
			this.Buyer.GmoExtend1 = string.Empty;
			
			this.Buyer.BuyCount = order.Items.Length;
			this.Buyer.BuyAmountTotal = order.OrderPriceTotal.ToPriceString();
			this.Buyer.MemberRegistDate = string.Empty;

			var userBusinessOwnerService = new UserBusinessOwnerService();
			var userBusinessOwner = userBusinessOwnerService.GetByUserId(order.UserId);
			this.Buyer.ShopCustomerId = order.UserId;
			if (userBusinessOwner != null)
			{
				this.Buyer.PresidentNameFamily = userBusinessOwner.OwnerName1;
				this.Buyer.PresidentName = userBusinessOwner.OwnerName2;
				this.Buyer.PresidentNameFamilyKana = StringUtility.ToZenkakuKatakana(userBusinessOwner.OwnerNameKana1);
				this.Buyer.PresidentNameKana = StringUtility.ToZenkakuKatakana(userBusinessOwner.OwnerNameKana2);
				this.Buyer.Birthday = userBusinessOwner.Birth.Value.ToString("yyyy/MM/dd");
				if (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
				{
					this.Buyer.ShopCustomerId = userBusinessOwner.ShopCustomerId;
				}
			}
			else
			{
				this.Buyer.PresidentNameFamily = string.Empty;
				this.Buyer.PresidentName = string.Empty;
				this.Buyer.PresidentNameFamilyKana = string.Empty;
				this.Buyer.PresidentNameKana = string.Empty;
				this.Buyer.Birthday = string.Empty;
				if (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
				{
					this.Buyer.ShopCustomerId = string.Empty;
				}
			}

			this.Deliveries = new DeliveriesElement();
			this.Deliveries.Delivery = new DeliveryElement();
			this.Deliveries.Delivery.DeliveryCustomer = new DeliveryCustomerElement();
			var orderShipping = order.Shippings[0];
			this.Deliveries.Delivery.DeliveryCustomer.Address = AddressHelper.ConcatenateAddressWithCountryName(orderShipping.ShippingAddr1, orderShipping.ShippingAddr2, orderShipping.ShippingAddr3, orderShipping.ShippingAddr4, orderShipping.ShippingCountryName);
			this.Deliveries.Delivery.DeliveryCustomer.CompanyName = orderShipping.ShippingCompanyName;
			this.Deliveries.Delivery.DeliveryCustomer.DepartmentName = orderShipping.ShippingCompanyPostName;
			this.Deliveries.Delivery.DeliveryCustomer.FullKanaName = StringUtility.ToZenkakuKatakana(orderShipping.ShippingNameKana);
			this.Deliveries.Delivery.DeliveryCustomer.FullName = orderShipping.ShippingName;
			this.Deliveries.Delivery.DeliveryCustomer.Tel = orderShipping.ShippingTel1;
			this.Deliveries.Delivery.DeliveryCustomer.ZipCode = orderShipping.ShippingZip;

			var orderItems = new OrderService().GetItemAll(order.OrderId);
			var priceInfoByTaxRate = new OrderService().GetPriceInfoByTaxRateAll(order.OrderId);
			this.CreateTaxesParam(priceInfoByTaxRate);

			this.Deliveries.Delivery.Details = new DetailsElement();
			var detailList = new List<DetailElement>();
			foreach (var product in orderItems)
			{
				var productdetail = new DetailElement();
				productdetail.DetailName = product.ProductName;
				productdetail.DetailPrice = product.ProductPrice.ToPriceString();
				productdetail.DetailQuantity = product.ItemQuantity;
				productdetail.ItemPrice = product.ItemPrice.ToPriceString();
				productdetail.FeeTaxType = product.ProductTaxRate == 8 ? "1" : "2";
				productdetail.GmoExtend2 = string.Empty;
				productdetail.GmoExtend3 = string.Empty;
				productdetail.GmoExtend4 = string.Empty;
				productdetail.DetailBrand = string.Empty;
				productdetail.DetailCategory = string.Empty;
				detailList.Add(productdetail);
			}

			this.Deliveries.Delivery.Details.Detail = detailList.ToArray();
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

		///// <summary>税金</summary>
		[XmlElement("taxes")]
		public Taxes[] Taxes;

		///// <summary>税金の合計額</summary>
		[XmlElement("taxesSum")]
		public TaxesSum[] TaxesSum;

		/// <summary>
		/// 税率パラメータ生成
		/// </summary>
		/// <param name="listTaxes">税率毎の注文金額情報モデル</param>
		private void CreateTaxesParam(List<OrderPriceByTaxRateModel> listTaxes)
		{
			// Create default Taxes
			this.Taxes = new Taxes[2] 
			{ 
				new Taxes()
				{
					TaxType = Constants.FLG_GMO_REQUEST_TAX_TYPE_8,
					TaxAmount = "0",
				},
				new Taxes()
				{
					TaxType = Constants.FLG_GMO_REQUEST_TAX_TYPE_10,
					TaxAmount = "0"
				}
			};

			// Create default TaxesSum
			this.TaxesSum = new TaxesSum[2] 
			{ 
				new TaxesSum()
				{
					TaxType = Constants.FLG_GMO_REQUEST_TAX_TYPE_8,
					TaxSumAmount = "0"
				},
				new TaxesSum()
				{
					TaxType = Constants.FLG_GMO_REQUEST_TAX_TYPE_10,
					TaxSumAmount = "0"
				}
			};

			// Update taxes
			foreach (var tax in listTaxes)
			{
				if (tax.KeyTaxRate == 8)
				{
					this.Taxes[0].TaxAmount = tax.TaxPriceByRate.ToPriceString();
					this.TaxesSum[0].TaxSumAmount = tax.PriceTotalByRate.ToPriceString();
				}
				if (tax.KeyTaxRate == 10)
				{
					this.Taxes[1].TaxAmount = tax.TaxPriceByRate.ToPriceString();
					this.TaxesSum[1].TaxSumAmount = tax.PriceTotalByRate.ToPriceString();
				}
			}
		}

		/// <summary>
		/// デバイス情報文字列をトリム（undefined、nullを取り除く）
		/// </summary>
		/// <param name="deviceInfo">デバイス情報文字列</param>
		/// <returns>デバイス情報文字列</returns>
		private string TrimDeviceInfo(string deviceInfo)
		{
			if (string.IsNullOrEmpty(deviceInfo)) return deviceInfo;
			var result = string.Join(
				";:",
				deviceInfo
					.Split(new[] { ";:" }, StringSplitOptions.None)
					.Select(s => ((s == "undefined") || (s == "null")) ? string.Empty : s));
			return result;
		}
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
			this.HttpHeader = string.Empty;
			this.DeviceInfo = string.Empty;
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
			this.ShopTransactionId = string.Empty;
			this.ShopOrderDate = string.Empty;
			this.BuyerNameFamily = string.Empty;
			this.BuyerName = string.Empty;
			this.BuyerNameFamilyKana = string.Empty;
			this.BuyerNameKana = string.Empty;
			this.ZipCode = string.Empty;
			this.Address = string.Empty;
			this.CompanyName = string.Empty;
			this.DepartmentName = string.Empty;
			this.Tel1 = string.Empty;
			this.Tel2 = string.Empty;
			this.Email1 = string.Empty;
			this.Email2 = string.Empty;
			this.BilledAmount = string.Empty;
			this.GmoExtend1 = string.Empty;
			this.PresidentNameFamily = string.Empty;
			this.PresidentName = string.Empty;
			this.PresidentNameFamilyKana = string.Empty;
			this.PresidentNameKana = string.Empty;
			this.Birthday = string.Empty;
			this.MemberRegistDate = string.Empty;
			this.BuyCount = 0;
			this.BuyAmountTotal = string.Empty;
			this.ShopCustomerId = string.Empty;
		}

		/// <summary>加盟店取引ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId;

		/// <summary>加盟店注文日</summary>
		[XmlElement("shopOrderDate")]
		public string ShopOrderDate;

		/// <summary>購入者の名前（姓）</summary>
		[XmlElement("buyerNameFamily")]
		public string BuyerNameFamily;

		/// <summary>購入者の名前（名）</summary>
		[XmlElement("buyerName")]
		public string BuyerName;

		/// <summary>購入者の名前（姓）（カナ）</summary>
		[XmlElement("buyerNameFamilyKana")]
		public string BuyerNameFamilyKana;

		/// <summary>購入者の名前（名）（カナ）</summary>
		[XmlElement("buyerNameKana")]
		public string BuyerNameKana;

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
		public string BilledAmount;

		/// <summary>GMO拡張項目１</summary>
		[XmlElement("gmoExtend1")]
		public string GmoExtend1;

		/// <summary>社長の名前（姓）</summary>
		[XmlElement("presidentNameFamily")]
		public string PresidentNameFamily;

		/// <summary>社長の名前（名）</summary>
		[XmlElement("presidentName")]
		public string PresidentName;

		/// <summary>社長の名前（姓）（カナ）</summary>
		[XmlElement("presidentNameFamilyKana")]
		public string PresidentNameFamilyKana;

		/// <summary>社長の名前（名）（カナ）</summary>
		[XmlElement("presidentNameKana")]
		public string PresidentNameKana;

		/// <summary>誕生日</summary>
		[XmlElement("birthday")]
		public string Birthday;

		/// <summary>メンバー登録日</summary>
		[XmlElement("memberRegistDate")]
		public string MemberRegistDate;

		/// <summary>購入個数</summary>
		[XmlElement("buyCount")]
		public decimal BuyCount;

		/// <summary>購入総量</summary>
		[XmlElement("buyAmountTotal")]
		public string BuyAmountTotal;

		/// <summary>ショップの顧客ID</summary>
		[XmlElement("shopCustomerId")]
		public string ShopCustomerId;
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
			this.FullName = string.Empty;
			this.FullKanaName = string.Empty;
			this.ZipCode = string.Empty;
			this.Address = string.Empty;
			this.CompanyName = string.Empty;
			this.DepartmentName = string.Empty;
			this.Tel = string.Empty;
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
			this.DetailName = string.Empty;
			this.DetailPrice = string.Empty;
			this.DetailQuantity = 0;
			this.ItemPrice = string.Empty;
			this.FeeTaxType = string.Empty;
			this.GmoExtend2 = string.Empty;
			this.GmoExtend3 = string.Empty;
			this.GmoExtend4 = string.Empty;
			this.DetailBrand = string.Empty;
			this.DetailCategory = string.Empty;
		}

		/// <summary>明細名</summary>
		[XmlElement("detailName")]
		public string DetailName;

		/// <summary>単価</summary>
		[XmlElement("detailPrice")]
		public string DetailPrice;

		/// <summary>数量</summary>
		[XmlElement("detailQuantity")]
		public decimal DetailQuantity;

		/// <summary>商品の値段</summary>
		[XmlElement("itemPrice")]
		public string ItemPrice;

		/// <summary>税金の種類</summary>
		[XmlElement("feeTaxType")]
		public string FeeTaxType;

		/// <summary>GMO拡張項目2</summary>
		[XmlElement("gmoExtend2")]
		public string GmoExtend2;

		/// <summary>GMO拡張項目3</summary>
		[XmlElement("gmoExtend3")]
		public string GmoExtend3;

		/// <summary>GMO拡張項目4</summary>
		[XmlElement("gmoExtend4")]
		public string GmoExtend4;

		/// <summary>ブランド詳細</summary>
		[XmlElement("detailBrand")]
		public string DetailBrand;

		/// <summary>カテゴリー詳細</summary>
		[XmlElement("detailCategory")]
		public string DetailCategory;

	}
	#endregion

	#region TaxesElement
	/// <summary>
	/// 税金
	/// </summary>
	public class Taxes
	{
		/// <summary>税金</summary>
		public Taxes()
		{
			this.TaxType = string.Empty;
			this.TaxAmount = string.Empty;
		}

		/// <summary>税金の種類</summary>
		[XmlElement("taxType")]
		public string TaxType;

		/// <summary>税額</summary>
		[XmlElement("taxAmount")]
		public string TaxAmount;
	}
	#endregion

	#region TaxesSumElement
	/// <summary>
	/// 税金の合計額
	/// </summary>
	public class TaxesSum
	{
		/// <summary>税金の合計額</summary>
		public TaxesSum()
		{
			this.TaxType = string.Empty;
			this.TaxSumAmount = string.Empty;
		}

		/// <summary>税金の種類</summary>
		[XmlElement("taxType")]
		public string TaxType;

		/// <summary>税金の合計額</summary>
		[XmlElement("taxSumAmount")]
		public string TaxSumAmount;
	}
	#endregion
}
