/*
=========================================================================================================
  Module      : 入金状況確認のリクエスト値(GmoRequestGetDefPaymentStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.GetDefPaymentStatus
{
	/// <summary>
	///  取引修正・キャンセルのリクエスト値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestGetDefPaymentStatus : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestGetDefPaymentStatus()
			: base()
		{
			this.Transaction = new TransactionElement();
		}
		/// <summary>コンストラクタ</summary>
		/// <param name="transactionId">取引ID</param>
		public GmoRequestGetDefPaymentStatus(string transactionId)
			: base()
		{
			this.Transaction = new TransactionElement(transactionId);
		}

		/// <summary>取引情報</summary>
		[XmlElement("transaction")]
		public TransactionElement Transaction { get; set; }
	}

	#region TransactionElement 取引情報要素
	/// <summary>
	/// 取引情報要素
	/// </summary>
	public class TransactionElement
	{
		/// <summary>コンストラクタ</summary>
		public TransactionElement()
		{
			this.GmoTransactionId = string.Empty;
		}
		/// <summary>コンストラクタ</summary>
		/// <param name="transactionId">取引ID</param>
		public TransactionElement(string transactionId)
		{
			this.GmoTransactionId = transactionId;
		}

		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId { get; set; }
	}
	#endregion
}