/*
=========================================================================================================
  Module      : Atobaraicom Shipping Registration Response (AtobaraicomShippingRegistrationResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Atobaraicom.Shipping
{
	/// <summary>
	/// Atobaraicom shipping registration response
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class AtobaraicomShippingRegistrationResponse
	{
		/// <summary>Status</summary>
		[XmlElement("status")]
		public string Status { get; set; }
		/// <summary>Order id</summary>
		[XmlElement("orderId")]
		public string OrderId { get; set; }
		/// <summary>Delivery id</summary>
		[XmlElement("delivId")]
		public DelivId DelivId { get; set; }
		/// <summary>Journal num</summary>
		[XmlElement("journalNum")]
		public string JournalNum { get; set; }
		/// <summary>Messages</summary>
		[XmlElement("messages")]
		public Messages[] Messages { get; set; }
		/// <summary>Api messages</summary>
		public string ApiMessages { get; set; }

		/// <summary>
		/// Handle api messages
		/// </summary>
		public void HandleApiMessages()
		{
			this.ApiMessages = string.Empty;

			if ((this.Messages[0].Message == null) || (this.Messages.Any() == false)) return;

			this.ApiMessages = string.Join(
				",",
				this.Messages.Select(message =>
					string.Format(
						"{0} : {1}",
						message.Message.Code,
						message.Message.MessageText)));
		}

		/// <summary>
		/// Is success
		/// </summary>
		public bool IsSuccess
		{
			get { return (this.Status != AtobaraicomConstants.ATOBARAICOM_API_RESULT_STATUS_ERROR); }
		}
	}

	/// <summary>
	/// Delivery id
	/// </summary>
	public class DelivId
	{
		/// <summary>Delivery id text</summary>
		[XmlText]
		public string DelivIdText { get; set; }
		/// <summary>Name</summary>
		[XmlAttribute("name")]
		public string Name { get; set; }
	}

	/// <summary>
	/// Messages
	/// </summary>
	public class Messages
	{
		/// <summary>Message object</summary>
		[XmlElement("message")]
		public Message Message { get; set; }
	}

	/// <summary>
	/// Message
	/// </summary>
	public class Message
	{
		/// <summary>Message text</summary>
		[XmlText]
		public string MessageText { get; set; }
		/// <summary>Code</summary>
		[XmlAttribute("cd")]
		public string Code { get; set; }
	}
}
