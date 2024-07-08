/*
=========================================================================================================
  Module      : Payment Boku Cancel Optin Response(PaymentBokuCancelOptinResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Boku.Response
{
	/// <summary>
	/// Payment Boku Cancel Optin Response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "cancel-optin-response", IsNullable = false, Namespace = "")]
	public class PaymentBokuCancelOptinResponse : PaymentBokuBaseResponse
	{
		/// <summary>Optin id</summary>
		[XmlElement("optin-id")]
		public string OptinId { get; set; }
		/// <summary>Optin type</summary>
		[XmlElement("optin-type")]
		public string OptinType { get; set; }
		/// <summary>Optin state</summary>
		[XmlElement("optin-state")]
		public StateElement OptinState { get; set; }
	}
}
