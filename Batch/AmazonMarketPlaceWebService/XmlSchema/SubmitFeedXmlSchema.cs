/*
=========================================================================================================
  Module      : SubmitFeedXmlスキーマ定義クラス(SubmitFeedXmlSchema.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Xml.Serialization;

namespace AmazonMarketPlaceWebService.XmlSchema
{
	#region +SubmitFeedRequestXml
	/// <summary>
	/// SubmitFeedRequestXmlスキーマ定義
	/// </summary>
	[XmlRoot(IsNullable = false, Namespace = "", ElementName = "AmazonEnvelope")]
	public class SubmitFeedRequestXmlSchema
	{
		/// <summary>Header</summary>
		[XmlElement("Header")]
		public Header Header { get; set; }

		/// <summary>MessageType</summary>
		[XmlElement("MessageType")]
		public string MessageType { get; set; }

		/// <summary>Message</summary>
		[XmlElement("Message")]
		public List<Message> Message { get; set; }
	}
	#endregion

	#region +Header
	/// <summary>
	/// Header
	/// </summary>
	public class Header
	{
		/// <summary>DocumentVersion</summary>
		[XmlElement("DocumentVersion")]
		public string DocumentVersion { get; set; }

		/// <summary>MerchantIdentifier</summary>
		[XmlElement("MerchantIdentifier")]
		public string MerchantIdentifier { get; set; }
	}
	#endregion

	#region +Message
	/// <summary>
	/// Message
	/// </summary>
	public class Message
	{
		/// <summary>MessageID</summary>
		[XmlElement("MessageID")]
		public string MessageID { get; set; }

		/// <summary>出荷通知（OrderFulfillment）</summary>
		[XmlElement("OrderFulfillment")]
		public OrderFulfillment OrderFulfillment { get; set; }

		/// <summary>OperationType</summary>
		[XmlElement("OperationType")]
		public string OperationType { get; set; }

		/// <summary>在庫（Inventory）</summary>
		[XmlElement("Inventory")]
		public Inventory Inventory { get; set; }
	}
	#endregion

	#region +OrderFulfillment
	/// <summary>
	/// OrderFulfillment
	/// </summary>
	public class OrderFulfillment
	{
		/// <summary>
		/// AmazonOrderID(Amazon注文ID)
		/// </summary>
		[XmlElement("AmazonOrderID")]
		public string AmazonOrderID { get; set; }

		/// <summary>
		/// MerchantOrderID(フルフィルメント用の識別子)
		/// </summary>
		[XmlElement("MerchantOrderID")]
		public string MerchantOrderID { get; set; }

		/// <summary>
		/// FulfillmentDate(商品の出荷日)
		/// </summary>
		[XmlElement("FulfillmentDate")]
		public string FulfillmentDate { get; set; }

		/// <summary>
		/// FulfillmentData(注文フルフィルメント情報)
		/// </summary>
		[XmlElement("FulfillmentData")]
		public FulfillmentData FulfillmentData { get; set; }

		/// <summary>Item</summary>
		[XmlElement("Item")]
		public List<Item> Item { get; set; }
	}
	#endregion

	#region +Inventory
	public class Inventory
	{
		/// <summary>SKU</summary>
		[XmlElement("SKU")]
		public string SKU { get; set; }

		/// <summary>Quantity</summary>
		[XmlElement("Quantity")]
		public int Quantity { get; set; }

		/// <summary>FulfillmentLatency</summary>
		[XmlElement("FulfillmentLatency")]
		public int FulfillmentLatency { get; set; }
	}
	#endregion

	#region +FulfillmentData
	/// <summary>
	/// FulfillmentData(注文フルフィルメント情報)
	/// </summary>
	public class FulfillmentData
	{
		/// <summary>CarrierCode(配送業者コード)</summary>
		[XmlElement("CarrierCode")]
		public string CarrierCode { get; set; }

		/// <summary>CarrierName(配送業者名)</summary>
		[XmlElement("CarrierName")]
		public string CarrierName { get; set; }

		/// <summary>ShippingMethod(配送方法)</summary>
		[XmlElement("ShippingMethod")]
		public string ShippingMethod { get; set; }

		/// <summary>ShipperTrackingNumber(出荷した商品のトラッキング番号)</summary>
		[XmlElement("ShipperTrackingNumber")]
		public string ShipperTrackingNumber { get; set; }
	}
	#endregion

	#region +Item
	/// <summary>
	/// Item
	/// </summary>
	public class Item
	{
		/// <summary>AmazonOrderItemCode</summary>
		[XmlElement("AmazonOrderItemCode")]
		public string AmazonOrderItemCode { get; set; }

		/// <summary>MerchantOrderItemID</summary>
		[XmlElement("MerchantOrderItemID")]
		public string MerchantOrderItemID { get; set; }

		/// <summary>MerchantFulfillmentItemID</summary>
		[XmlElement("MerchantFulfillmentItemID")]
		public string MerchantFulfillmentItemID { get; set; }

		/// <summary>Quantity</summary>
		[XmlElement("Quantity")]
		public string Quantity { get; set; }
	}
	#endregion
}
