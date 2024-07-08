/*
=========================================================================================================
  Module      : Payment Boku Confirm Response(PaymentBokuConfirmResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku.Response
{
	/// <summary>
	/// Payment Boku Confirm Response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "confirm-optin-response", IsNullable = false, Namespace = "")]
	public class PaymentBokuConfirmResponse : PaymentBokuBaseResponse
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuConfirmResponse()
			: base()
		{
			this.OptinState = new StateElement();
			this.AccountProfile = new AccountProfile();
		}
		#endregion

		#region Properties
		/// <summary>Time stamp</summary>
		[XmlElement("timestamp")]
		public string TimeStamp { get; set; }
		/// <summary>Optin id</summary>
		[XmlElement("optin-id")]
		public string OptinId { get; set; }
		/// <summary>Optin state</summary>
		[XmlElement("optin-state")]
		public StateElement OptinState { get; set; }
		/// <summary>Optin type</summary>
		[XmlElement("optin-type")]
		public string OptinType { get; set; }
		/// <summary>Payment method</summary>
		[XmlElement("payment-method")]
		public string PaymentMethod { get; set; }
		/// <summary>Optin url</summary>
		[XmlElement("account-profile")]
		public AccountProfile AccountProfile { get; set; }
		#endregion
	}

	/// <summary>
	/// Account Profile
	/// </summary>
	public class AccountProfile
	{
		/// <summary>Carrier data</summary>
		[XmlElement("carrier-data")]
		public CarrierData CarrierData { get; set; }
	}

	/// <summary>
	/// Carrier data
	/// </summary>
	public class CarrierData
	{
		/// <summary>Attribute</summary>
		[XmlElement("attribute")]
		public Attribute[] Attributes { get; set; }
	}

	/// <summary>
	/// Attribute
	/// </summary>
	public class Attribute
	{
		/// <summary>Key</summary>
		[XmlAttribute("key")]
		public string Key { get; set; }
		/// <summary>Text</summary>
		[XmlText]
		public string Text { get; set; }
	}
}
