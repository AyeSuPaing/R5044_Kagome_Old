/*
=========================================================================================================
  Module      : DSK後払い与信結果取得レスポンス(DskDeferredGetAuthResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.DSKDeferred.GetAuth
{
	/// <summary>
	/// DSK後払い与信結果取得レスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false)]
	[XmlType(TypeName = "ysnGetKekkaResForm")]
	public class DskDeferredGetAuthResultResponse : BaseDskDeferredResponse
	{
		/// <summary>取得結果</summary>
		[XmlElement("transactionResult")]
		public TransactionResultElement Transaction;
	}

	/// <summary>
	/// 取得結果要素
	/// </summary>
	public class TransactionResultElement
	{
		/// <summary>注文ID</summary>
		[XmlElement("transactionId")]
		public string TransactionId { get; set; }
		/// <summary>加盟店注文ID</summary>
		[XmlElement("shopOrderId")]
		public string ShopOrderId { get; set; }
		/// <summary>審査時刻</summary>
		[XmlElement("authoriDate")]
		public string AuthoriDate { get; set; }
		/// <summary>審査結果</summary>
		[XmlElement("authorResult")]
		public string AuthorResult { get; set; }
		/// <summary>保留理由要素</summary>
		[XmlElement("holdReason")]
		public HoldReasonElement[] HoldReason;
	}

	/// <summary>
	/// 保留理由要素
	/// </summary>
	public class HoldReasonElement
	{
		/// <summary>理由コード</summary>
		[XmlElement("reasonCode")]
		public string ReasonCode { get; set; }
		/// <summary>保留理由</summary>
		[XmlElement("reason")]
		public string Reason;
	}
}
