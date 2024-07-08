/*
=========================================================================================================
  Module      : Payment Boku Validate Optin API(PaymentBokuValidateOptinApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.Boku.Request;
using w2.App.Common.Order.Payment.Boku.Response;
using w2.App.Common.Order.Payment.Boku.Utils;

namespace w2.App.Common.Order.Payment.Boku
{
	/// <summary>
	/// Payment Boku Validate Optin API
	/// </summary>
	public class PaymentBokuValidateOptinApi : PaymentBokuBaseApi
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuValidateOptinApi()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">Boku settings</param>
		public PaymentBokuValidateOptinApi(PaymentBokuSetting settings)
			: base(settings)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Exec Api
		/// </summary>
		/// <param name="optinId">Optin id</param>
		/// <returns>Validate optin response</returns>
		public PaymentBokuValidateOptinResponse Exec(string optinId)
		{
			var request = new PaymentBokuValidateOptinRequest(this.Settings);
			request.OptinId = optinId;
			var requestXml = request.CreatePostString();

			// 実行
			var response = this.Post<PaymentBokuValidateOptinResponse>(
				requestXml,
				Constants.PAYMENT_BOKU_VALIDATE_OPTIN_URL,
				null);
			return response;
		}
		#endregion
	}
}
