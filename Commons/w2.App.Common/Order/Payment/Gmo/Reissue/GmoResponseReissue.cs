/*
=========================================================================================================
  Module      : Gmo Response Reissue(GmoResponseReissue.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Xml.Serialization;
using w2.App.Common.Util;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.GMO.Reissue
{
	/// <summary>
	/// Invoice reissue request API response value
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseReissue : BaseGmoResponse
	{
		/// <summary>
		/// Get Error Message
		/// </summary>
		/// <returns>Error message</returns>
		public string GetErrorMessage()
		{
			if (this.HasError) return string.Empty;

			var errorMessage = string.Join(
				",",
				this.Errors.Error.Select(error
					=> string.Format(
						"{0}:{1}",
						error.ErrorCode,
						error.ErrorMessage)).ToArray());
			return errorMessage;
		}

		/// <summary>
		/// Get Order Payment Memo For Reissue
		/// </summary>
		/// <param name="paymentOrderId">Payment order id</param>
		/// <returns>Payment memo</returns>
		public string GetOrderPaymentMemoForReissue(string paymentOrderId)
		{
			var transIdString = ValueText.GetValueText(
				Constants.TABLE_ORDER,
				Constants.VALUETEXT_PAYMENT_MEMO_CVSDEF_INVOICE_REISSUE,
				Constants.VALUETEXT_ORDER_SETTLEMENT_TRANSACTION_ID);

			var reissueInvoiceString = ValueText.GetValueText(
				Constants.TABLE_ORDER,
				Constants.VALUETEXT_PAYMENT_MEMO_CVSDEF_INVOICE_REISSUE,
				Constants.VALUETEXT_ORDER_INVOICE_REISSUE);

			var paymentMemo = string.Format(
				transIdString,
				DateTimeUtility.ToStringForManager(
					DateTime.Now,
					DateTimeUtility.FormatType.ShortDateHourMinute2Letter),
				paymentOrderId,
				this.TransactionResult.GmoTransactionId,
				reissueInvoiceString);
			return paymentMemo;
		}

		/// <summary>Is success</summary>
		public bool IsSuccess
		{
			get { return (this.Result == ResultCode.OK); }
		}
		/// <summary>Has error</summary>
		private bool HasError
		{
			get
			{
				return ((this.Errors == null)
					|| (this.Errors.Error == null));
			}
		}
		/// <summary>Transaction result</summary>
		[XmlElement("transactionResult")]
		public TransactionResultElement TransactionResult { get; set; }
	}

	#region Transaction result element
	/// <summary>
	/// Transaction result element
	/// </summary>
	public class TransactionResultElement
	{
		/// <summary>GMO transaction id</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId { get; set; }
	}
	#endregion
}
