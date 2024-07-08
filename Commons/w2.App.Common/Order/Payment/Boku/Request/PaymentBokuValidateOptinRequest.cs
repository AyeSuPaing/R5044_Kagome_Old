/*
=========================================================================================================
  Module      : Payment Boku Validate Optin Request(PaymentBokuValidateOptinRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku.Request
{
	/// <summary>
	/// Payment Boku Validate Optin Request
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "validate-optin-request", IsNullable = false, Namespace = "")]
	public class PaymentBokuValidateOptinRequest : PaymentBokuBaseRequest
	{
		#region Construstors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuValidateOptinRequest()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public PaymentBokuValidateOptinRequest(PaymentBokuSetting setting)
			: base(setting)
		{
			this.OptinType = BokuConstants.CONST_BOKU_OPTIN_TYPE_HOSTED;
			this.IncludeAccountProfile = false;
		}
		#endregion

		#region Properties
		/// <summary>Optin id</summary>
		[XmlElement("optin-id")]
		public string OptinId { get; set; }
		/// <summary>Optin type</summary>
		[XmlElement("optin-type")]
		public string OptinType { get; set; }
		/// <summary>Include account profile</summary>
		[XmlElement("include-account-profile")]
		public bool IncludeAccountProfile { get; set; }
		#endregion
	}
}
