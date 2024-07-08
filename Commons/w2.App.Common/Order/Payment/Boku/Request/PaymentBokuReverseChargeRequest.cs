/*
=========================================================================================================
  Module      : Payment Boku Reverse Charge Request(PaymentBokuReverseChargeRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku.Request
{
	/// <summary>
	/// Payment Boku Reverse Charge Request
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "reverse-charge-request", IsNullable = false, Namespace = "")]
	public class PaymentBokuReverseChargeRequest : PaymentBokuBaseRequest
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuReverseChargeRequest()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public PaymentBokuReverseChargeRequest(PaymentBokuSetting setting)
			: base(setting)
		{
		}
		#endregion

		#region Properties
		/// <summary>Country</summary>
		[XmlElement("country")]
		public string Country { get; set; }
		#endregion
	}
}
