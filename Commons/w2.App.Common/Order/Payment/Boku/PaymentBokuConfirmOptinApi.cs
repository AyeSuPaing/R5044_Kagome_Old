/*
=========================================================================================================
  Module      : Payment Boku Confirm Optin API(PaymentBokuConfirmOptinApi.cs)
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
	/// Payment Boku Confirm Optin API
	/// </summary>
	public class PaymentBokuConfirmOptinApi : PaymentBokuBaseApi
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuConfirmOptinApi()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">Boku settings</param>
		public PaymentBokuConfirmOptinApi(PaymentBokuSetting settings)
			: base(settings)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Exec Api
		/// </summary>
		/// <param name="optinId">Optin id</param>
		/// <returns>Confirm optin response</returns>
		public PaymentBokuConfirmResponse Exec(string optinId)
		{
			var request = new PaymentBokuConfirmRequest(this.Settings);
			request.OptinId = optinId;
			var requestXml = request.CreatePostString();

			// 実行
			var response = this.Post<PaymentBokuConfirmResponse>(
				requestXml,
				Constants.PAYMENT_BOKU_CONFIRM_OPTIN_URL,
				null);
			return response;
		}
		#endregion
	}
}
