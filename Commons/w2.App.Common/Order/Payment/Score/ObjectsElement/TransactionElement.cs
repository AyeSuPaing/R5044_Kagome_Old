/*
=========================================================================================================
  Module      : 取引要素(TransactionElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;
using w2.Common.Helper;

namespace w2.App.Common.Order.Payment.Score.ObjectsElement
{
	/// <summary>
	/// 取引要素
	/// </summary>
	public class TransactionElement
	{
		/// <summary>スコア注文ID</summary>
		[XmlElement("nissenTransactionId")]
		public string NissenTransactionId { get; set; }
		/// <summary>加盟店注文ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId { get; set; }
		/// <summary>顧客請求額</summary>
		[XmlElement("billedAmount")]
		public string BilledAmount { get; set; }

	}

	/// <summary>
	/// 取引結果要素
	/// </summary>
	public class TransactionResultElement : TransactionElement
	{
		/// <summary>審査時刻</summary>
		[XmlElement("authoriDate")]
		public string AuthoriDate { get; set; }
		/// <summary>審査結果</summary>
		[XmlElement("authorResult")]
		public string AuthorResult { get; set; }
		/// <summary>保留理由</summary>
		[XmlElement("holdReason")]
		public HoldReasonElement HoldReason { get; set; }
		/// <summary>審査結果はNGか</summary>
		public bool IsResultNg
		{
			get
			{
				return this.AuthorResult == ScoreAuthorResult.Ng.ToText();
			}
		}
		/// <summary>審査結果は保留か</summary>
		public bool IsResultHold
		{
			get
			{
				return this.AuthorResult == ScoreAuthorResult.Hold.ToText();
			}
		}
	}

	/// <summary>
	/// 保留理由要素
	/// </summary>
	public class HoldReasonElement
	{
		/// <summary>理由コード</summary>
		[XmlElement("reasonCode")]
		public string ReasonCode { get; set; }
		/// <summary>理由</summary>
		[XmlElement("reason")]
		public string Reason { get; set; }
	}
}
