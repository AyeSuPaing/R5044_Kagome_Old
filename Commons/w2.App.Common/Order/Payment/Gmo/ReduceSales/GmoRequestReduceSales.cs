/*
=========================================================================================================
  Module      : 請求減額のリクエスト値(GmoRequestReduceSales.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.ReduceSales
{
	/// <summary>
	/// 取引登録のリクエスト値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestReduceSales : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestReduceSales()
			: base()
		{
			this.Buyer = new BuyerElement();
			this.Deliveries = new DeliveriesElement();
		}

		/// <summary>購入者情報</summary>
		[XmlElement("buyer")]
		public BuyerElement Buyer;

		/// <summary>配送先項目</summary>
		[XmlElement("deliveries")]
		public DeliveriesElement Deliveries;
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
			this.GmoTransactionId = "";
			this.ShopTransactionId = "";
			this.BilledAmount = 0;
			this.GmoExtend1 = "";
		}

		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;

		/// <summary>加盟店取引ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId;

		/// <summary>顧客請求金額</summary>
		[XmlElement("billedAmount")]
		public decimal BilledAmount;

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
