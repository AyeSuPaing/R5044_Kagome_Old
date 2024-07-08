/*
=========================================================================================================
  Module      : Payment Boku Query Charge Response(PaymentBokuQueryChargeResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Boku.Response
{
	/// <summary>
	/// Payment Boku Query Charge Response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "query-charge-response", IsNullable = false, Namespace = "")]
	public class PaymentBokuQueryChargeResponse : PaymentBokuBaseResponse
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuQueryChargeResponse()
			: base()
		{
			this.Charges = new ChargesElement();
		}
		#endregion

		#region Properties
		/// <summary>Charges</summary>
		[XmlElement("charges")]
		public ChargesElement Charges { get; set; }
		#endregion
	}

	/// <summary>
	/// Charges Element
	/// </summary>
	public class ChargesElement
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public ChargesElement()
		{
			this.Charges = new ChargeElement[1];
		}
		#endregion

		#region Properties
		/// <summary>Charges</summary>
		[XmlElement("charge")]
		public ChargeElement[] Charges { get; set; }
		#endregion
	}

	/// <summary>
	/// Charge Element
	/// </summary>
	public class ChargeElement : PaymentBokuBaseResponse
	{
		/// <summary>Time stamp</summary>
		[XmlElement("timestamp")]
		public string TimeStamp { get; set; }
		/// <summary>Charge id</summary>
		[XmlElement("charge-id")]
		public string ChargeId { get; set; }
		/// <summary>Charge status</summary>
		[XmlElement("charge-status")]
		public string ChargeStatus { get; set; }
		/// <summary>Merchant data</summary>
		[XmlElement("merchant-data")]
		public string MerchantData { get; set; }
		/// <summary>Merchant transaction id</summary>
		[XmlElement("merchant-transaction-id")]
		public string MerchantTransactionId { get; set; }
		/// <summary>Optin id</summary>
		[XmlElement("optin-id")]
		public string OptinId { get; set; }
		/// <summary>Country</summary>
		[XmlElement("country")]
		public string Country { get; set; }
		/// <summary>Network id</summary>
		[XmlElement("network-id")]
		public string NetworkId { get; set; }
	}
}
