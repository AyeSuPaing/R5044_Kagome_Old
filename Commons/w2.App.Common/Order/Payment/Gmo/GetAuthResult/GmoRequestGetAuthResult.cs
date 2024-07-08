/*
=========================================================================================================
  Module      : 与信審査結果取得のリクエスト値(GmoRequestGetAuthResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.GetAuthResult
{
	/// <summary>
	/// 与信審査結果取得のリクエスト値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestGetAuthResult : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestGetAuthResult()
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
