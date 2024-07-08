/*
=========================================================================================================
  Module      : 出荷報告のリクエスト値(GmoRequestShipment.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.Shipment
{
	/// <summary>
	/// 出荷報告のリクエスト値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestShipment : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestShipment()
			: base()
		{
			this.Transaction = new TransactionElement();
		}

		/// <summary>購入者情報</summary>
		[XmlElement("transaction")]
		public TransactionElement Transaction;
	}

	#region BuyerElement 購入者情報要素
	/// <summary>
	/// 購入者情報要素
	/// </summary>
	public class TransactionElement
	{
		/// <summary>コンストラクタ</summary>
		public TransactionElement()
		{
			this.GmoTransactionId = "";
			this.Pdcompanycode = Constants.PAYMENT_SETTING_GMO_DEFERRED_PDCOMPANYCODE;
			this.Slipno = "";
			this.InvoiceIssueDate = "";
		}

		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;

		/// <summary>運送会社コード</summary>
		[XmlElement("pdcompanycode")]
		public string Pdcompanycode;

		/// <summary>発送伝票番号</summary>
		[XmlElement("slipno")]
		public string Slipno;

		/// <summary>請求書発行日</summary>
		[XmlElement("invoiceIssueDate")]
		public string InvoiceIssueDate;
	}
	#endregion
}
