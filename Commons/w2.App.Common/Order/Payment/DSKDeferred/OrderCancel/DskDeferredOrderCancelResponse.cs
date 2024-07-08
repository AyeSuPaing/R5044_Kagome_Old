/*
=========================================================================================================
  Module      : DSK後払い取引キャンセルレスポンス(DskDeferredOrderCancelResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.DSKDeferred.OrderCancel
{
	/// <summary>
	/// DSK後払い取引キャンセルレスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false)]
	[XmlType(TypeName = "cancelAddResForm")]
	public class DskDeferredOrderCancelResponse : BaseDskDeferredResponse
	{
		/// <summary>取引情報</summary>
		[XmlElement("transactionInfo")]
		public TransactionInfoElement TransactionInfo { get; set; }
	}

	/// <summary>
	/// 取引情報要素
	/// </summary>
	public class TransactionInfoElement
	{
		/// <summary>注文ID</summary>
		[XmlElement("transactionId")]
		public string TransactionId { get; set; }
		/// <summary>加盟店注文ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId { get; set; }
	}
}
