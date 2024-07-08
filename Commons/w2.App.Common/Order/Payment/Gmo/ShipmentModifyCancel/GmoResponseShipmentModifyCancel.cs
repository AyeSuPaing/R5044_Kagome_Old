/*
=========================================================================================================
  Module      : 出荷報告修正・キャンセルのレスポンス値(GmoResponseShipmentModifyCancel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.ShipmentModifyCancel
{
	/// <summary>
	/// 出荷報告修正・キャンセルのレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseShipmentModifyCancel : BaseGmoResponse
	{
		/// <summary>
		/// 取引登録結果
		/// </summary>
		[XmlElement("transactionResult")]
		public transactionResultElement TransactionResult;
	}

	#region TransactionInfoElement
	/// <summary>
	/// 取引登録結果要素
	/// </summary>
	public class transactionResultElement
	{
		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;
	}
	#endregion
}
