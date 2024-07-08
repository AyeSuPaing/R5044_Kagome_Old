/*
=========================================================================================================
  Module      : Payment Boku Reverse Charge API(PaymentBokuReverseChargeApi.cs)
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
	/// Payment Boku Reverse Charge API
	/// </summary>
	public class PaymentBokuReverseChargeApi : PaymentBokuBaseApi
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuReverseChargeApi()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">Settings</param>
		public PaymentBokuReverseChargeApi(PaymentBokuSetting settings)
			: base(settings)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Exec Api
		/// </summary>
		/// <param name="country">Country</param>
		/// <param name="merchantRequestId">Merchant request ID</param>
		/// <returns>Response xml</returns>
		public PaymentBokuReverseChargeResponse Exec(string country, string merchantRequestId)
		{
			// XML作成
			var request = new PaymentBokuReverseChargeRequest(this.Settings);
			request.Country = (string.IsNullOrEmpty(country) == false)
				? country
				: Constants.COUNTRY_ISO_CODE_JP;
			request.MerchantRequestId = merchantRequestId;
			var requestXml = request.CreatePostString();

			// 実行
			var response = this.Post<PaymentBokuReverseChargeResponse>(
				requestXml,
				Constants.PAYMENT_BOKU_OPTIN_URL,
				null);
			return response;
		}
		#endregion
	}
}
