/*
=========================================================================================================
  Module      : Payment Boku Refund Charge Request(PaymentBokuRefundChargeRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku.Request
{
	/// <summary>
	/// Payment Boku Refund Charge Request
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "refund-charge-request", IsNullable = false, Namespace = "")]
	public class PaymentBokuRefundChargeRequest : PaymentBokuBaseRequest
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuRefundChargeRequest()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public PaymentBokuRefundChargeRequest(PaymentBokuSetting setting)
			: base(setting)
		{
			this.TimeOut = new TimeOutElement();
			this.TimeOut.After = setting.Timeout;
			this.SkipRetry = setting.SkipRetryFlg;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Set value to request
		/// </summary>
		/// <param name="currency">Currency</param>
		/// <param name="chargeId">Charge id</param>
		/// <param name="merchantRefundId">Merchant refund id</param>
		/// <param name="reasonCode">Reason code</param>
		/// <param name="refundAmount">Refund amount</param>
		/// <param name="isIncludeTax">Is include tax</param>
		public void SetValue(
			string currency,
			string chargeId,
			string merchantRefundId,
			int reasonCode,
			string refundAmount,
			bool isIncludeTax)
		{
			this.Currency = currency;
			this.ChargeId = chargeId;
			this.MerchantRefundId = merchantRefundId;
			this.ReasonCode = reasonCode;
			this.RefundAmount = refundAmount;
			this.TaxMode = (isIncludeTax) ? BokuConstants.CONST_BOKU_TAX_MODE_INCTAX : BokuConstants.CONST_BOKU_TAX_MODE_EXTAX;
		}
		#endregion

		#region Properties
		/// <summary>Charge id</summary>
		[XmlElement("charge-id")]
		public string ChargeId { get; set; }
		/// <summary>Currency</summary>
		[XmlElement("currency")]
		public string Currency { get; set; }
		/// <summary>Merchant refund id</summary>
		[XmlElement("merchant-refund-id")]
		public string MerchantRefundId { get; set; }
		/// <summary>Reason code</summary>
		[XmlElement("reason-code")]
		public int ReasonCode { get; set; }
		/// <summary>Refund amount</summary>
		[XmlElement("refund-amount")]
		public string RefundAmount { get; set; }
		/// <summary>Tax mode</summary>
		[XmlElement("tax-mode")]
		public string TaxMode { get; set; }
		/// <summary>Time out</summary>
		[XmlElement("timeout")]
		public TimeOutElement TimeOut { get; set; }
		/// <summary>Skip retry</summary>
		[XmlElement("skip-retry")]
		public bool SkipRetry { get; set; }
		#endregion
	}
}
