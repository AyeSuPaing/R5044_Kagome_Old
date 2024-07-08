/*
=========================================================================================================
  Module      : Payment Boku Query Charge API(PaymentBokuQueryChargeApi.cs)
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
	/// Payment Boku Query Charge API
	/// </summary>
	public class PaymentBokuQueryChargeApi : PaymentBokuBaseApi
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuQueryChargeApi()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">Settings</param>
		public PaymentBokuQueryChargeApi(PaymentBokuSetting settings)
			: base(settings)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Exec Api
		/// </summary>
		/// <param name="country">Country</param>
		/// <param name="chargeId">Charge id</param>
		/// <returns>Query charge response</returns>
		public PaymentBokuQueryChargeResponse Exec(string country, string chargeId)
		{
			// XML作成
			var request = new PaymentBokuQueryChargeRequest(this.Settings);
			request.Country = (string.IsNullOrEmpty(country) == false)
				? country
				: Constants.COUNTRY_ISO_CODE_JP;
			request.ChargeId = chargeId;
			var requestXml = request.CreatePostString();

			// 実行
			var response = this.Post<PaymentBokuQueryChargeResponse>(
				requestXml,
				Constants.PAYMENT_BOKU_OPTIN_URL,
				null);
			return response;
		}
		#endregion
	}
}
