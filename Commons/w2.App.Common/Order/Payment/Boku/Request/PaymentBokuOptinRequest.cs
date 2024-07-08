/*
=========================================================================================================
  Module      : Payment Boku Optin Request(PaymentBokuOptinRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku.Request
{
	/// <summary>
	/// Payment Boku Optin Request
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "optin-request", IsNullable = false, Namespace = "")]
	public class PaymentBokuOptinRequest : PaymentBokuBaseRequest
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuOptinRequest()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public PaymentBokuOptinRequest(PaymentBokuSetting setting)
			: base(setting)
		{
			this.Hosted = new Hosted();
			this.Terms = new Terms();
			this.OptinType = BokuConstants.CONST_BOKU_OPTIN_TYPE_HOSTED;
			this.MerchantConsumerId = setting.MerchantConsumerId;
			this.IncludeAccountProfile = false;
		}
		#endregion

		#region Properties
		/// <summary>Country</summary>
		[XmlElement("country")]
		public string Country { get; set; }
		/// <summary>Optin type</summary>
		[XmlElement("optin-type")]
		public string OptinType { get; set; }
		/// <summary>Payment method</summary>
		[XmlElement("payment-method")]
		public string PaymentMethod { get; set; }
		/// <summary>Hosted</summary>
		[XmlElement("hosted")]
		public Hosted Hosted { get; set; }
		/// <summary>Include account profile</summary>
		[XmlElement("include-account-profile")]
		public bool IncludeAccountProfile { get; set; }
		/// <summary>Merchant consumer id</summary>
		[XmlElement("merchant-consumer-id")]
		public string MerchantConsumerId { get; set; }
		/// <summary>Terms</summary>
		[XmlElement("terms")]
		public Terms Terms { get; set; }
		#endregion
	}

	/// <summary>
	/// Hosted
	/// </summary>
	public class Hosted
	{
		/// <summary>Foward url</summary>
		[XmlElement("forward-url")]
		public string ForwardUrl { get; set; }
	}

	/// <summary>
	/// Terms
	/// </summary>
	public class Terms
	{
		/// <summary>Optin purpose</summary>
		[XmlElement("optin-purpose")]
		public string OptinPurpose { get; set; }
	}
}
