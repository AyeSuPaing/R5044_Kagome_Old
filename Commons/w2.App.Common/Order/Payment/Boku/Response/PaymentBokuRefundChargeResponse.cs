/*
=========================================================================================================
  Module      : Payment Boku Refund Charge Response(PaymentBokuRefundChargeResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Boku.Response
{
	/// <summary>
	/// Payment Boku Refund Charge Response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "refund-charge-response", IsNullable = false, Namespace = "")]
	public class PaymentBokuRefundChargeResponse : PaymentBokuBaseResponse
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuRefundChargeResponse()
			: base()
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="refund">Refund</param>
		public PaymentBokuRefundChargeResponse(RefundElement refund)
			: this()
		{
			this.Result = refund.Result;
			this.TimeStamp = refund.TimeStamp;
			this.ChargeId = refund.ChargeId;
			this.RefundId = refund.RefundId;
			this.RefundStatus = refund.RefundStatus;
			this.Country = refund.Country;
			this.NetworkId = refund.NetworkId;
		}
		#endregion

		#region Properties
		/// <summary>Time stamp</summary>
		[XmlElement("timestamp")]
		public string TimeStamp { get; set; }
		/// <summary>Charge id</summary>
		[XmlElement("charge-id")]
		public string ChargeId { get; set; }
		/// <summary>Refund id</summary>
		[XmlElement("refund-id")]
		public string RefundId { get; set; }
		/// <summary>Refund status</summary>
		[XmlElement("refund-status")]
		public string RefundStatus { get; set; }
		/// <summary>Country</summary>
		[XmlElement("country")]
		public string Country { get; set; }
		/// <summary>Network id</summary>
		[XmlElement("network-id")]
		public string NetworkId { get; set; }
		#endregion
	}
}
