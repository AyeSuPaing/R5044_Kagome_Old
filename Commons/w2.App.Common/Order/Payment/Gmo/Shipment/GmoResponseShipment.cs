/*
=========================================================================================================
  Module      : 出荷報告のレスポンス値(GmoResponseShipment.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.Shipment
{
	/// <summary>
	/// 出荷報告のレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseShipment : BaseGmoResponse
	{
		/// <summary>
		/// 出荷報告結果
		/// </summary>
		[XmlElement("transactionInfo")]
		public TransactionInfoElement TransactionInfo;
	}

	#region TransactionInfoElement
	/// <summary>
	/// 出荷報告結果要素
	/// </summary>
	public class TransactionInfoElement
	{
		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;
	}
	#endregion
}
