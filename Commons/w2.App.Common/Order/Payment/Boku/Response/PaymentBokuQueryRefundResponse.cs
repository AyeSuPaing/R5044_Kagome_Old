/*
=========================================================================================================
  Module      : Payment Boku Query Refund Response(PaymentBokuQueryRefundResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Boku.Response
{
	/// <summary>
	/// Payment Boku Query Refund Response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "query-refund-response", IsNullable = false, Namespace = "")]
	public class PaymentBokuQueryRefundResponse : PaymentBokuBaseResponse
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuQueryRefundResponse()
			: base()
		{
			this.Refunds = new RefundsElement();
		}
		#endregion

		#region Properties
		/// <summary>Refunds</summary>
		[XmlElement("refunds")]
		public RefundsElement Refunds { get; set; }
		#endregion
	}

	/// <summary>
	/// Refunds Element
	/// </summary>
	public class RefundsElement
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public RefundsElement()
		{
			this.Refunds = new RefundElement[1];
		}
		#endregion

		#region Properties
		/// <summary>Refunds</summary>
		[XmlElement("refund")]
		public RefundElement[] Refunds { get; set; }
		#endregion
	}

	/// <summary>
	/// Refund Element
	/// </summary>
	public class RefundElement : PaymentBokuBaseResponse
	{
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
	}
}
