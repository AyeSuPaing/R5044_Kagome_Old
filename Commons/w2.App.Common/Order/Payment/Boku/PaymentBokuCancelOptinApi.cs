/*
=========================================================================================================
  Module      : Payment Boku Cancel Optin API(PaymentBokuCancelOptinApi.cs)
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
	/// Payment Boku Cancel Optin API
	/// </summary>
	public class PaymentBokuCancelOptinApi : PaymentBokuBaseApi
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuCancelOptinApi()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">Boku settings</param>
		public PaymentBokuCancelOptinApi(PaymentBokuSetting settings)
			: base(settings)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Exec Api
		/// </summary>
		/// <param name="optinId">Optin id</param>
		/// <returns>Cancel optin response</returns>
		public PaymentBokuCancelOptinResponse Exec(string optinId)
		{
			var request = new PaymentBokuCancelOptinRequest(this.Settings);
			request.OptinId = optinId;
			var requestXml = request.CreatePostString();

			// 実行
			var response = this.Post<PaymentBokuCancelOptinResponse>(
				requestXml,
				Constants.PAYMENT_BOKU_CANCEL_OPTIN_URL,
				null);
			return response;
		}
		#endregion
	}
}
