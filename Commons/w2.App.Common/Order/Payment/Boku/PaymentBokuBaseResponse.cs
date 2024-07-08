/*
=========================================================================================================
  Module      : Payment Boku Base Response(PaymentBokuBaseRespone.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.Boku
{
	/// <summary>
	///  Payment Boku Base Response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "result", IsNullable = false, Namespace = "")]
	public abstract class PaymentBokuBaseResponse : IHttpApiResponseData
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuBaseResponse()
		{
			this.Result = new ResultElement();
		}
		#endregion

		#region Methods
		/// <summary>
		/// レスポンス文字列生成
		/// </summary>
		/// <returns>レスポンス文字列</returns>
		public virtual string CreateResponseString()
		{
			return SerializeHelper.Serialize(this);
		}
		#endregion

		#region Properties
		/// <summary>Result</summary>
		[XmlElement("result")]
		public ResultElement Result { get; set; }
		/// <summary>Merchant id</summary>
		[XmlElement("merchant-id")]
		public string MerchantId { get; set; }
		/// <summary>Merchant request id</summary>
		[XmlElement("merchant-request-id")]
		public string MerchantRequestId { get; set; }
		/// <summary>Is success</summary>
		public bool IsSuccess
		{
			get { return (this.Result.Status == "OK"); }
		}
		#endregion
	}

	/// <summary>
	/// Result Element
	/// </summary>
	public class ResultElement
	{
		/// <summary>Status</summary>
		[XmlAttribute("status")]
		public string Status { get; set; }
		/// <summary>Reason code</summary>
		[XmlElement("reason-code")]
		public string ReasonCode { get; set; }
		/// <summary>Message</summary>
		[XmlElement("message")]
		public string Message { get; set; }
		/// <summary>Retriable</summary>
		[XmlElement("retriable")]
		public bool Retriable { get; set; }
		/// <summary>Reversal id</summary>
		[XmlElement("reversal-id")]
		public string ReversalId { get; set; }
		/// <summary>Retry delay</summary>
		[XmlElement("retry-delay")]
		public string RetryDelay { get; set; }
		/// <summary>Invalid request field</summary>
		[XmlElement("invalid-request-fields")]
		public Field[] InvalidRequestField { get; set; }
	}

	/// <summary>
	/// Field
	/// </summary>
	public class Field
	{
		/// <summary>Path</summary>
		[XmlElement("path")]
		public string Path { get; set; }
		/// <summary>Reason</summary>
		[XmlElement("reason")]
		public string Reason { get; set; }
	}

	/// <summary>
	/// State Element
	/// </summary>
	public class StateElement
	{
		/// <summary>Optin status</summary>
		[XmlElement("optin-status")]
		public string OptinStatus { get; set; }
		/// <summary>Country</summary>
		[XmlElement("country")]
		public string Country { get; set; }
		/// <summary>Network id</summary>
		[XmlElement("network-id")]
		public string NetworkId { get; set; }
		/// <summary>Account identifier</summary>
		[XmlElement("account-identifier")]
		public string AccountIdentifier { get; set; }
		/// <summary>Issuer unique user id</summary>
		[XmlElement("issuer-unique-user-id")]
		public string IssuerUniqueUserId { get; set; }
	}
}
