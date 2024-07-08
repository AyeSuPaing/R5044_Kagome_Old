/*
=========================================================================================================
  Module      : 入金状況確認のレスポンス値(GmoResponseGetDefPaymentStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.GetDefPaymentStatus
{
	/// <summary>
	/// 入金状況確認のレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseGetDefPaymentStatus : BaseGmoResponse
	{
		/// <summary>
		/// 入金状況確認結果
		/// </summary>
		[XmlElement("transactionResult")]
		public TransactionResultElement TransactionResult { get; set; }

		/// <summary>入金済み判定用フラグ</summary>
		public bool IsPaid
		{
			get
			{
				return ((this.TransactionResult.PaymentStatus == PaymentStatusCode.PROMPT)
					|| (this.TransactionResult.PaymentStatus == PaymentStatusCode.DEFINITE));
			}
		}
	}

	#region TransactionResultElement 入金状況確認結果要素
	/// <summary>
	/// 入金状況確認結果要素
	/// </summary>
	public class TransactionResultElement
	{
		/// <summary>GMO取引ID</summary>
		[XmlElement("GmoTransactionId")]
		public string GmoTransactionId {get; set; }

		/// <summary>入金ステータス</summary>
		[XmlElement("paymentStatus")]
		public PaymentStatusCode PaymentStatus { get; set; }

		/// <summary>速報日</summary>
		[XmlElement("promptDate")]
		public string PromptDate {get; set;}

		/// <summary>確報日</summary>
		[XmlElement("decisionDate")]
		public string DecisionDate { get; set; }
	}
	#endregion
}