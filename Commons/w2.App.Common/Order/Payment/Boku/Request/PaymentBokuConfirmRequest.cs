/*
=========================================================================================================
  Module      : Payment Boku Confirm Request(PaymentBokuConfirmRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku.Request
{
	/// <summary>
	/// Payment Boku Confirm Request
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "confirm-optin-request", IsNullable = false, Namespace = "")]
	public class PaymentBokuConfirmRequest : PaymentBokuBaseRequest
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuConfirmRequest()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public PaymentBokuConfirmRequest(PaymentBokuSetting setting)
			: base(setting)
		{
			this.IncludeAccountProfile = false;
		}
		#endregion

		#region Properties
		/// <summary>Optin id</summary>
		[XmlElement("optin-id")]
		public string OptinId { get; set; }
		/// <summary>Include account profile</summary>
		[XmlElement("include-account-profile")]
		public bool IncludeAccountProfile { get; set; }
		#endregion
	}
}
