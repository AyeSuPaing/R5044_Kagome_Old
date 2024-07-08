/*
=========================================================================================================
  Module      : Payment Boku Query Charge Request(PaymentBokuQueryChargeRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku.Request
{
	/// <summary>
	/// Payment Boku Query Charge Request
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "query-charge-request", IsNullable = false, Namespace = "")]
	public class PaymentBokuQueryChargeRequest : PaymentBokuBaseRequest
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuQueryChargeRequest()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public PaymentBokuQueryChargeRequest(PaymentBokuSetting setting)
			: base(setting)
		{
			this.MerchantRequestId = null;
		}
		#endregion

		#region Properties
		/// <summary>Charge id</summary>
		[XmlElement("charge-id")]
		public string ChargeId { get; set; }
		/// <summary>Country</summary>
		[XmlElement("country")]
		public string Country { get; set; }
		#endregion
	}
}
