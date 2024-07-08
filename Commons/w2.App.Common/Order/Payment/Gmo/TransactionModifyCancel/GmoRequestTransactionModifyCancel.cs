/*
=========================================================================================================
  Module      : Gmo Request Transaction Modify Cancel (GmoRequestTransactionModifyCancel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using w2.App.Common.Input.Order;
using w2.App.Common.Option;
using w2.Common.Util;
using w2.App.Common.Extensions.Currency;
using w2.Domain.Order;
using w2.Domain.UserBusinessOwner;
using w2.Domain.User.Helper;

namespace w2.App.Common.Order.Payment.GMO.TransactionModifyCancel
{
	/// <summary>
	///  取引修正・キャンセルのリクエスト値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestTransactionModifyCancel : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestTransactionModifyCancel()
			: base()
		{
			this.Buyer = new BuyerElement();
			this.Taxes = new Taxes[] { new Taxes() };
			this.TaxesSum = new TaxesSum[] { new TaxesSum() };
			this.KindInfo = new KindInfoElement();
			this.Deliveries = new DeliveriesElement();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="input">カート情報</param>
		public GmoRequestTransactionModifyCancel(OrderInput input)
			: base()
		{
			this.KindInfo = new KindInfoElement();
			this.Buyer = new BuyerElement();
			this.Buyer.ShopTransactionId = input.PaymentOrderId;
			this.Buyer.GmoTransactionId = input.CardTranId;
			this.Buyer.ShopOrderDate = DateTime.Now.ToString("yyyy/MM/dd");
			this.Buyer.BuyerNameFamily = input.Owner.OwnerName1;
			this.Buyer.BuyerName = input.Owner.OwnerName2;
			this.Buyer.BuyerNameFamilyKana = StringUtility.ToZenkakuKatakana(input.Owner.OwnerNameKana1);
			this.Buyer.BuyerNameKana = StringUtility.ToZenkakuKatakana(input.Owner.OwnerNameKana2);
			this.Buyer.ZipCode = input.Owner.OwnerZip;
			this.Buyer.Address = AddressHelper.ConcatenateAddressWithCountryName(input.Owner.OwnerAddr1, input.Owner.OwnerAddr2, input.Owner.OwnerAddr3, input.Owner.OwnerAddr4, input.Owner.OwnerAddrCountryName);
			this.Buyer.CompanyName = input.Owner.OwnerCompanyName;
			this.Buyer.DepartmentName = StringUtility.ToEmpty(input.Owner.OwnerCompanyPostName);
			this.Buyer.Tel1 = input.Owner.OwnerTel1;
			this.Buyer.Tel2 = input.Owner.OwnerTel2;
			this.Buyer.Email1 = StringUtility.ToEmpty(input.Owner.OwnerMailAddr);
			this.Buyer.Email2 = input.Owner.OwnerMailAddr2;
			var lastBilledAmount = string.IsNullOrEmpty(input.LastBilledAmount) ? string.Empty : decimal.Parse(input.LastBilledAmount).ToPriceString();
			this.Buyer.BilledAmount = lastBilledAmount;
			this.Buyer.GmoExtend1 = string.Empty;

			var userBusinessOwnerService = new UserBusinessOwnerService();
			var userBusinessOwner = userBusinessOwnerService.GetByUserId(input.UserId);
			this.Buyer.ShopCustomerId = input.UserId;
			if (userBusinessOwner != null)
			{
				this.Buyer.PresidentNameFamily = userBusinessOwner.OwnerName1;
				this.Buyer.PresidentName = userBusinessOwner.OwnerName2;
				this.Buyer.PresidentNameFamilyKana = StringUtility.ToZenkakuKatakana(userBusinessOwner.OwnerNameKana1);
				this.Buyer.PresidentNameKana = StringUtility.ToZenkakuKatakana(userBusinessOwner.OwnerNameKana2);
				this.Buyer.Birthday = userBusinessOwner.Birth.Value.ToString("yyyy/MM/dd");
				if (input.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
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
				if (input.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
				{
					this.Buyer.ShopCustomerId = string.Empty;
				}
			}

			var shipping = input.Shippings[0];
			this.Buyer.BuyCount = shipping.Items.Length;
			var buyAmountTotal = string.IsNullOrEmpty(input.OrderPriceTotal) ? string.Empty : decimal.Parse(input.OrderPriceTotal).ToPriceString();
			this.Buyer.BuyAmountTotal = buyAmountTotal;

			//Taxes
			var listTaxes = new List<OrderPriceByTaxRateModel>();
			foreach (var tax in input.OrderPriceByTaxRates)
			{
				listTaxes.Add(tax.CreateModel());
			}
			this.CreateTaxesParam(listTaxes);

			// Devlieries informations
			this.Deliveries = new DeliveriesElement();
			this.Deliveries.Delivery = new DeliveryElement();
			this.Deliveries.Delivery.DeliveryCustomer = new DeliveryCustomerElement();
			this.Deliveries.Delivery.DeliveryCustomer.CompanyName = shipping.ShippingCompanyName;
			this.Deliveries.Delivery.DeliveryCustomer.DepartmentName = shipping.ShippingCompanyPostName;
			this.Deliveries.Delivery.DeliveryCustomer.FullKanaName = StringUtility.ToZenkakuKatakana(shipping.ShippingNameKana);
			this.Deliveries.Delivery.DeliveryCustomer.FullName = shipping.ShippingName;
			this.Deliveries.Delivery.DeliveryCustomer.Tel = shipping.ShippingTel1;
			this.Deliveries.Delivery.DeliveryCustomer.Address = AddressHelper.ConcatenateAddressWithCountryName(shipping.ShippingAddr1, shipping.ShippingAddr2, shipping.ShippingAddr3, shipping.ShippingAddr4, shipping.ShippingCountryName);
			this.Deliveries.Delivery.DeliveryCustomer.ZipCode = shipping.ShippingZip;

			//Product details
			this.Deliveries.Delivery.Details = new DetailsElement();
			var detailList = new List<DetailElement>();

			foreach (var product in shipping.Items)
			{
				var productdetail = new DetailElement();
				productdetail.DetailName = product.ProductName;
				productdetail.DetailPrice = product.ProductPrice;
				productdetail.DetailQuantity = int.Parse(product.ItemQuantity);
				productdetail.ItemPrice = product.ItemPrice;
				productdetail.FeeTaxType = product.ProductTaxRate == "8.00" ? "1" : "2";
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
		/// Contructor
		/// </summary>
		/// <param name="input">カート情報<</param>
		public GmoRequestTransactionModifyCancel(OrderModel input)
			: base()
		{
			input.Owner = (input.Owner == null) ? new OrderOwnerModel() : input.Owner;
			this.KindInfo = new KindInfoElement();
			this.Buyer = new BuyerElement();
			this.Buyer.ShopTransactionId = input.PaymentOrderId;
			this.Buyer.GmoTransactionId = input.CardTranId;
			this.Buyer.ShopOrderDate = DateTime.Now.ToString("yyyy/MM/dd");
			this.Buyer.BuyerNameFamily = input.Owner.OwnerName1;
			this.Buyer.BuyerName = input.Owner.OwnerName2;
			this.Buyer.BuyerNameFamilyKana = StringUtility.ToZenkakuKatakana(input.Owner.OwnerNameKana1);
			this.Buyer.BuyerNameKana = StringUtility.ToZenkakuKatakana(input.Owner.OwnerNameKana2);
			this.Buyer.ZipCode = input.Owner.OwnerZip;
			this.Buyer.Address = AddressHelper.ConcatenateAddressWithCountryName(input.Owner.OwnerAddr1, input.Owner.OwnerAddr2, input.Owner.OwnerAddr3, input.Owner.OwnerAddr4, input.Owner.OwnerAddrCountryName);
			this.Buyer.CompanyName = input.Owner.OwnerCompanyName;
			this.Buyer.DepartmentName = StringUtility.ToEmpty(input.Owner.OwnerCompanyPostName);
			this.Buyer.Tel1 = input.Owner.OwnerTel1;
			this.Buyer.Tel2 = input.Owner.OwnerTel2;
			this.Buyer.Email1 = StringUtility.ToEmpty(input.Owner.OwnerMailAddr);
			this.Buyer.Email2 = input.Owner.OwnerMailAddr2;
			this.Buyer.BilledAmount = input.LastBilledAmount.ToPriceString();
			this.Buyer.GmoExtend1 = string.Empty;

			var userBusinessOwnerService = new UserBusinessOwnerService();
			var userBusinessOwner = userBusinessOwnerService.GetByUserId(input.UserId);
			this.Buyer.ShopCustomerId = input.UserId;
			if (userBusinessOwner != null)
			{
				this.Buyer.PresidentNameFamily = userBusinessOwner.OwnerName1;
				this.Buyer.PresidentName = userBusinessOwner.OwnerName2;
				this.Buyer.PresidentNameFamilyKana = StringUtility.ToZenkakuKatakana(userBusinessOwner.OwnerNameKana1);
				this.Buyer.PresidentNameKana = StringUtility.ToZenkakuKatakana(userBusinessOwner.OwnerNameKana2);
				this.Buyer.Birthday = userBusinessOwner.Birth.Value.ToString("yyyy/MM/dd");
				if (input.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
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
				if (input.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_FRAMEGUARANTEE)
				{
					this.Buyer.ShopCustomerId = string.Empty;
				}
			}

			//Taxes
			if (input.OrderPriceByTaxRates != null)
			{
				var listTaxes = new List<OrderPriceByTaxRateModel>(input.OrderPriceByTaxRates);
				this.CreateTaxesParam(listTaxes);
			}

			if (input.Shippings != null)
			{
				var shipping = input.Shippings[0];
				this.Buyer.BuyCount = shipping.Items.Length;
				this.Buyer.BuyAmountTotal = input.OrderPriceTotal.ToPriceString();

				// Devlieries informations
				this.Deliveries = new DeliveriesElement();
				this.Deliveries.Delivery = new DeliveryElement();
				this.Deliveries.Delivery.DeliveryCustomer = new DeliveryCustomerElement();
				this.Deliveries.Delivery.DeliveryCustomer.CompanyName = shipping.ShippingCompanyName;
				this.Deliveries.Delivery.DeliveryCustomer.DepartmentName = StringUtility.ToEmpty(shipping.ShippingCompanyPostName);
				this.Deliveries.Delivery.DeliveryCustomer.FullKanaName = StringUtility.ToZenkakuKatakana(shipping.ShippingNameKana);
				this.Deliveries.Delivery.DeliveryCustomer.FullName = shipping.ShippingName;
				this.Deliveries.Delivery.DeliveryCustomer.Tel = shipping.ShippingTel1;
				this.Deliveries.Delivery.DeliveryCustomer.Address = AddressHelper.ConcatenateAddressWithCountryName(shipping.ShippingAddr1, shipping.ShippingAddr2, shipping.ShippingAddr3, shipping.ShippingAddr4, shipping.ShippingCountryName);
				this.Deliveries.Delivery.DeliveryCustomer.ZipCode = shipping.ShippingZip;

				//Product details
				this.Deliveries.Delivery.Details = new DetailsElement();
				var detailList = new List<DetailElement>();

				foreach (var product in shipping.Items)
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
		}

		/// <summary>更新種別情報</summary>
		[XmlElement("kindInfo")]
		public KindInfoElement KindInfo;

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
	}

	#region KindInfoElement 更新種別情報要素
	/// <summary>
	/// 更新種別情報要素
	/// </summary>
	public class KindInfoElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public KindInfoElement()
		{
			this.UpdateKind = UpdateKindType.OrderModify;
		}

		/// <summary>取引更新種別</summary>
		[XmlElement("updateKind")]
		public UpdateKindType UpdateKind;
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
			this.GmoTransactionId = string.Empty;
			this.ShopTransactionId = string.Empty;
			this.ShopOrderDate = string.Empty;
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
		}

		/// <summary>GMO 取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;

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
			this.Detail = new DetailElement[0];
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
			this.DetailBrand = string.Empty;
			this.DetailCategory = string.Empty;
			this.GmoExtend2 = string.Empty;
			this.GmoExtend3 = string.Empty;
			this.GmoExtend4 = string.Empty;
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