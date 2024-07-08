/*
=========================================================================================================
  Module      : DSK後払い出荷報告リクエスト(DskDeferredShipmentRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.DSKDeferred.Shipment
{
	/// <summary>
	/// DSK後払い出荷報告リクエスト
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false)]
	public class DskDeferredShipmentRequest : BaseDskDeferredRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DskDeferredShipmentRequest()
		{
			this.Transaction = new TransactionElement();
			this.PdRequest = new PdRequestElement();
		}

		/// <summary>購入者情報</summary>
		[XmlElement("transaction")]
		public TransactionElement Transaction;
		/// <summary>発送先項目要素</summary>
		[XmlElement("PdRequest")]
		public PdRequestElement PdRequest;
	}

	/// <summary>
	/// 取引情報要素
	/// </summary>
	public class TransactionElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TransactionElement()
		{
			this.TransactionId = "";
			this.ShopTransactionId = "";
			this.BilledAmount = "";
			this.InvoiceStartDate = "";
		}

		/// <summary>注文ID</summary>
		[XmlElement("transactionId")]
		public string TransactionId;
		/// <summary>加盟店注文ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId;
		/// <summary>顧客請求金額</summary>
		[XmlElement("billedAmount")]
		public string BilledAmount;
		/// <summary>請求書発行日付</summary>
		[XmlElement("invoiceStartDate")]
		public string InvoiceStartDate;
	}

	/// <summary>
	/// 発送先項目要素
	/// </summary>
	public class PdRequestElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PdRequestElement()
		{
			this.PdCompanyCode = "";
			this.SlipNo = "";
			this.Address1 = "";
			this.Address2 = "";
			this.Address3 = "";
		}

		/// <summary>運送会社コード</summary>
		[XmlElement("pdcompanycode")]
		public string PdCompanyCode;
		/// <summary>発送伝票番号</summary>
		[XmlElement("slipno")]
		public string SlipNo;
		/// <summary>商品発送先都道府県</summary>
		[XmlElement("address1")]
		public string Address1;
		/// <summary>商品発送先市区町村</summary>
		[XmlElement("address2")]
		public string Address2;
		/// <summary>商品発送先それ以降の住所</summary>
		[XmlElement("address3")]
		public string Address3;
	}
}
