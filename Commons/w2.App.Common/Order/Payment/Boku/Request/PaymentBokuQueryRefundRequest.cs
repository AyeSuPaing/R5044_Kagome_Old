/*
=========================================================================================================
  Module      : Payment Boku Query Refund Request(PaymentBokuQueryRefundRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku.Request
{
	/// <summary>
	/// Payment Boku Query Refund Request
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "query-refund-request", IsNullable = false, Namespace = "")]
	public class PaymentBokuQueryRefundRequest : PaymentBokuBaseRequest
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuQueryRefundRequest()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public PaymentBokuQueryRefundRequest(PaymentBokuSetting setting)
			: base(setting)
		{
			this.MerchantRequestId = string.Empty;
		}
		#endregion

		#region Properties
		/// <summary>Country</summary>
		[XmlElement("country")]
		public string Country { get; set; }
		/// <summary>Refund id</summary>
		[XmlElement("refund-id")]
		public string RefundId { get; set; }
		#endregion
	}
}
