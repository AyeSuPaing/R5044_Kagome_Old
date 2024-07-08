/*
=========================================================================================================
  Module      : DSK後払い注文情報キャンセルリクエスト(DskDeferredOrderCancelRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.DSKDeferred.OrderCancel
{
	/// <summary>
	/// DSK後払い注文情報キャンセルリクエスト
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false)]
	public class DskDeferredOrderCancelRequest : BaseDskDeferredRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DskDeferredOrderCancelRequest()
		{
			this.Transaction = new TransactionElement();
		}

		/// <summary>取引情報</summary>
		[XmlElement("transaction")]
		public TransactionElement Transaction;
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
	}
}
