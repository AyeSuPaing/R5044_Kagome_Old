/*
=========================================================================================================
  Module      : DSK後払い出荷報告レスポンス(DskDeferredShipmentResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.DSKDeferred.Shipment
{
	/// <summary>
	/// DSK後払い出荷報告レスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false)]
	[XmlType(TypeName = "hsoInfoAddResForm")]
	public class DskDeferredShipmentResponse : BaseDskDeferredResponse
	{
		/// <summary>注文登録結果</summary>
		[XmlElement("transactionInfo")]
		public TransactionInfoResultElement TransactionInfoResult { get; set; }
	}

	/// <summary>
	/// 注文登録結果要素
	/// </summary>
	public class TransactionInfoResultElement
	{
		/// <summary>注文ID</summary>
		[XmlElement("transactionId")]
		public string TransactionId { get; set; }
		/// <summary>加盟店注文ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId { get; set; }
	}
}
