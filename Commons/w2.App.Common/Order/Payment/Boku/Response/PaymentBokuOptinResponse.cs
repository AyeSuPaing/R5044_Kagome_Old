/*
=========================================================================================================
  Module      : Payment Boku Optin Response(PaymentBokuOptinResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku.Response
{
	/// <summary>
	/// Payment Boku Optin Response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "optin-response", IsNullable = false, Namespace = "")]
	public class PaymentBokuOptinResponse : PaymentBokuBaseResponse
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuOptinResponse()
			: base()
		{
			this.OptinState = new StateElement();
			this.Hosted = new Hosted();
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
		[XmlElement("hosted")]
		public Hosted Hosted { get; set; }
		#endregion
	}

	/// <summary>
	/// Hosted
	/// </summary>
	public class Hosted
	{
		/// <summary>Optin url</summary>
		[XmlElement("optin-url")]
		public string OptinUrl { get; set; }
	}
}
