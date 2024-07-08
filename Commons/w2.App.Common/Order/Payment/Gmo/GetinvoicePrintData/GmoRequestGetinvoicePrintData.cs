/*
=========================================================================================================
  Module      : 請求書印字データ取得のリクエスト値(GmoRequestGetinvoicePrintData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.GetinvoicePrintData
{
	/// <summary>
	/// 請求書印字データ取得のリクエスト値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestGetinvoicePrintData : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestGetinvoicePrintData()
			: base()
		{
			this.Transaction = new TransactionElement();
		}

		/// <summary>取引情報</summary>
		[XmlElement("transaction")]
		public TransactionElement Transaction;
	}

	#region BuyerElement 取引情報要素
	/// <summary>
	/// 取引情報要素
	/// </summary>
	public class TransactionElement
	{
		/// <summary>コンストラクタ</summary>
		public TransactionElement()
		{
			this.GmoTransactionId = "";
		}

		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;
	}
	#endregion
}
