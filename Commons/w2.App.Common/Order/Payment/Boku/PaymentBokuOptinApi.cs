/*
=========================================================================================================
  Module      : Payment Boku Optin API(PaymentBokuOptinApi.cs)
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
	/// Payment Boku Optin API
	/// </summary>
	public class PaymentBokuOptinApi : PaymentBokuBaseApi
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuOptinApi()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">Boku settings</param>
		public PaymentBokuOptinApi(PaymentBokuSetting settings)
			: base(settings)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Exec Api
		/// </summary>
		/// <param name="country">Country</param>
		/// <param name="forwardUrl">Forward url</param>
		/// <param name="optinPurpose">Optin purpose</param>
		/// <returns>Optin response</returns>
		public PaymentBokuOptinResponse Exec(
			string country,
			string forwardUrl,
			string optinPurpose = BokuConstants.CONST_BOKU_OPTIN_TERMS_OPTIN_PURPOSE_STANDING_APPROVAL)
		{
			// XML作成
			var request = new PaymentBokuOptinRequest(this.Settings);
			request.Country = (string.IsNullOrEmpty(country) == false)
				? country
				: Constants.COUNTRY_ISO_CODE_JP;
			request.Hosted.ForwardUrl = forwardUrl;
			request.Terms.OptinPurpose = optinPurpose;
			request.PaymentMethod = BokuConstants.CONST_BOKU_PAYMENT_METHOD_CARRIERBILLING;
			var requestXml = request.CreatePostString();

			// 実行
			var response = this.Post<PaymentBokuOptinResponse>(
				requestXml,
				Constants.PAYMENT_BOKU_OPTIN_URL,
				BokuConstants.CONST_BOKU_PAYMENT_METHOD_CARRIERBILLING);
			return response;
		}
		#endregion
	}
}
