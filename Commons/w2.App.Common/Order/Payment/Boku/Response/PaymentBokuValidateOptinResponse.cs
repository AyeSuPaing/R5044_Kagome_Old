/*
=========================================================================================================
  Module      : Payment Boku Validate Optin Response(PaymentBokuValidateOptinResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku.Response
{
	/// <summary>
	/// Payment Boku Validate Optin Response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "validate-optin-response", IsNullable = false, Namespace = "")]
	public class PaymentBokuValidateOptinResponse : PaymentBokuBaseResponse
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuValidateOptinResponse()
			: base()
		{
			this.OptinState = new StateElement();
		}
		#endregion

		#region Properties
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
		#endregion
	}
}
