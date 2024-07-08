/*
=========================================================================================================
  Module      : Payment Boku Charge Response(PaymentBokuChargeResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Boku.Response
{
	/// <summary>
	/// Payment Boku Charge Response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "charge-response", IsNullable = false, Namespace = "")]
	public class PaymentBokuChargeResponse : PaymentBokuBaseResponse
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuChargeResponse()
			: base()
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="charge">Charge</param>
		public PaymentBokuChargeResponse(ChargeElement charge)
			: this()
		{
			this.Result = charge.Result;
			this.TimeStamp = charge.TimeStamp;
			this.ChargeId = charge.ChargeId;
			this.ChargeStatus = charge.ChargeStatus;
			this.MerchantData = charge.MerchantData;
			this.MerchantTransactionId = charge.MerchantTransactionId;
			this.OptinId = charge.OptinId;
			this.Country = charge.Country;
			this.NetworkId = charge.NetworkId;
		}
		#endregion

		#region Properties
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
		#endregion
	}
}
