/*
=========================================================================================================
  Module      : スコア後払い払込票印字データ取得のリクエスト値(ScoreGetInvoiceRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Input.Order;
using w2.App.Common.Order.Payment.Score.ObjectsElement;

namespace w2.App.Common.Order.Payment.Score.GetInvoice
{
	/// <summary>
	/// 払込票印字データ取得
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class ScoreGetInvoiceRequest : BaseScoreRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ScoreGetInvoiceRequest()
		{
			this.Transaction = new TransactionElement();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public ScoreGetInvoiceRequest(OrderInput order)
			: this()
		{
			this.Transaction.NissenTransactionId = order.CardTranId;
			this.Transaction.ShopTransactionId = order.PaymentOrderId;
			this.Transaction.BilledAmount = order.LastBilledAmount.ToPriceString();
		}

		/// <summary>取引情報</summary>
		[XmlElement("transaction")]
		public TransactionElement Transaction { get; set; }
	}
}
