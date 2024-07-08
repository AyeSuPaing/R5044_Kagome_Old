/*
=========================================================================================================
  Module      : スコア後払いキャンセルリクエスト(ScoreCancelRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment.Score.ObjectsElement;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Score.Cancel
{
	/// <summary>
	/// キャンセルリクエスト
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class ScoreCancelRequest : BaseScoreRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ScoreCancelRequest()
		{
			this.Transaction = new TransactionElement();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		public ScoreCancelRequest(OrderModel order)
			: this()
		{
			this.Transaction.NissenTransactionId = order.CardTranId;
			this.Transaction.ShopTransactionId = order.PaymentOrderId;
			this.Transaction.BilledAmount = order.OrderPriceTotal.ToPriceString();
		}

		/// <summary>取引情報/summary>
		[XmlElement("transaction")]
		public TransactionElement Transaction { get; set; }
	}
}
