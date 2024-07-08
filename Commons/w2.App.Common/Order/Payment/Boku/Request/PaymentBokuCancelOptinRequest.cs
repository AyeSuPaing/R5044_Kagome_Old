/*
=========================================================================================================
  Module      : Payment Boku Cancel Optin Request(PaymentBokuCancelOptinRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku.Request
{
	/// <summary>
	/// Payment Boku Cancel Optin Request
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "cancel-optin-request", IsNullable = false, Namespace = "")]
	public class PaymentBokuCancelOptinRequest : PaymentBokuBaseRequest
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuCancelOptinRequest()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public PaymentBokuCancelOptinRequest(PaymentBokuSetting setting)
			: base(setting)
		{
		}
		#endregion

		#region Properties
		/// <summary>Optin id</summary>
		[XmlElement("optin-id")]
		public string OptinId { get; set; }
		#endregion
	}
}
