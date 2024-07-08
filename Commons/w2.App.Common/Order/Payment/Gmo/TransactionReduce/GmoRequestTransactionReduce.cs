/*
=========================================================================================================
  Module      : Gmo Request Transaction Reduce (GmoRequestTransactionReduce.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using w2.App.Common.Option;
using w2.App.Common.Extensions.Currency;
using w2.Domain.Order;
using w2.App.Common.Input.Order;

namespace w2.App.Common.Order.Payment.GMO.TransactionReduce
{
	/// <summary>
	/// GMOリクエスト取引削減
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestTransactionReduce : BaseGmoRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GmoRequestTransactionReduce()
			: base()
			{
				this.Buyer = new BuyerElement();
				this.Taxes = new Taxes[0];
				this.TaxesSum = new TaxesSum[0];
				this.Deliveries = new DeliveriesElement();
			}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="orderModel">OrderModel</param>
		public GmoRequestTransactionReduce(OrderModel orderModel)
			: base()
		{
			this.Buyer = new BuyerElement();
			this.Buyer.GmoTransactionId = orderModel.CardTranId;
			this.Buyer.ShopTransactionId = orderModel.PaymentOrderId;
			this.Buyer.BilledAmount = orderModel.LastBilledAmount.ToPriceString();
			this.Buyer.GmoExtend1 = string.Empty;

			//Taxes
			var listTaxes = new List<OrderPriceByTaxRateModel>(orderModel.OrderPriceByTaxRates);
			this.CreateTaxesParam(listTaxes);

			this.Deliveries = new DeliveriesElement();
			var detailList = new List<DetailElement>();

			foreach (var product in orderModel.Items)
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
			this.BilledAmount = string.Empty;
			this.GmoExtend1 = string.Empty;
			this.GmoTransactionId = string.Empty;
		}

		/// <summary>加盟店取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;

		/// <summary>加盟店取引ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId;

		/// <summary>顧客請求額</summary>
		[XmlElement("billedAmount")]
		public string BilledAmount;

		/// <summary>GMO拡張項目１</summary>
		[XmlElement("gmoExtend1")]
		public string GmoExtend1;
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
			this.Details = new DetailsElement();
		}

		/// <summary>明細項目</summary>
		[XmlElement("details")]
		public DetailsElement Details;
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