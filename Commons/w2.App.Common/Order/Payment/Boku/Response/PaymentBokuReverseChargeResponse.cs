/*
=========================================================================================================
  Module      : Payment Boku Reverse Charge Response(PaymentBokuReverseChargeResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Boku.Response
{
	/// <summary>
	/// Payment Boku Reverse Charge Response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "reverse-charge-response", IsNullable = false, Namespace = "")]
	public class PaymentBokuReverseChargeResponse : PaymentBokuBaseResponse
	{
		/// <summary>Time stamp</summary>
		[XmlElement("timestamp")]
		public string TimeStamp { get; set; }
		/// <summary>Charge id</summary>
		[XmlElement("charge-id")]
		public string ChargeId { get; set; }
		/// <summary>Reversal ids</summary>
		[XmlElement("reversal-id")]
		public string ReversalId { get; set; }
		/// <summary>Country</summary>
		[XmlElement("country")]
		public string Country { get; set; }
	}
}
