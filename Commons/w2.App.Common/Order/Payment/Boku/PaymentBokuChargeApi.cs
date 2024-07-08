/*
=========================================================================================================
  Module      : Payment Boku Charge API(PaymentBokuChargeApi.cs)
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
	/// Payment Boku Charge API
	/// </summary>
	public class PaymentBokuChargeApi : PaymentBokuBaseApi
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuChargeApi()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">Boku settings</param>
		public PaymentBokuChargeApi(PaymentBokuSetting settings)
			: base(settings)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Exec Api
		/// </summary>
		/// <param name="currency">Currency</param>
		/// <param name="merchantData">Merchant data</param>
		/// <param name="merchantItemDescription">Merchant item description</param>
		/// <param name="merchantTransactionId">Merchant transaction id</param>
		/// <param name="optinId">Optin id</param>
		/// <param name="totalAmount">Total amount</param>
		/// <param name="isIncludeTax">Is include tax</param>
		/// <param name="consumerIpAddress">Consumer ip address</param>
		/// <param name="isSubscription">Is subscription</param>
		/// <param name="renewal">Renewal</param>
		/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
		/// <param name="fixedPurchaseSetting">Fixed purchase setting</param>
		/// <returns>Response xml</returns>
		public PaymentBokuChargeResponse Exec(
			string currency,
			string merchantData,
			string merchantItemDescription,
			string merchantTransactionId,
			string optinId,
			string totalAmount,
			bool isIncludeTax,
			string consumerIpAddress,
			bool isSubscription,
			bool renewal,
			string fixedPurchaseKbn,
			string fixedPurchaseSetting)
		{
			// XML作成
			var request = new PaymentBokuChargeRequest(this.Settings);
			request.SetValue(
				currency,
				merchantData,
				merchantItemDescription,
				merchantTransactionId,
				optinId,
				totalAmount,
				isIncludeTax,
				consumerIpAddress,
				isSubscription,
				renewal,
				fixedPurchaseKbn,
				fixedPurchaseSetting);
			var requestXml = request.CreatePostString();

			// 実行
			var response = this.Post<PaymentBokuChargeResponse>(
				requestXml,
				Constants.PAYMENT_BOKU_CHARGE_URL,
				null);
			if (response == null) return null;

			var execCount = 0;
			
			while ((execCount <= Constants.BOKU_PAYMENT_QUERY_LIMIT_TIME)
				&& (response.ChargeStatus == BokuConstants.CONST_BOKU_CHARGE_STATUS_IN_PROGRESS))
			{
				var queryXml = new PaymentBokuQueryChargeRequest(this.Settings)
				{
					ChargeId = response.ChargeId,
					Country = response.Country
				}.CreatePostString();
				var queryCharge = this.Post<PaymentBokuQueryChargeResponse>(
					queryXml,
					Constants.PAYMENT_BOKU_QUERY_CHARGE_URL,
					null);
				if ((queryCharge != null) && queryCharge.IsSuccess)
				{
					response = new PaymentBokuChargeResponse(queryCharge.Charges.Charges[0]);
				}
				execCount++;
			}

			if (response.ChargeStatus == BokuConstants.CONST_BOKU_CHARGE_STATUS_IN_PROGRESS)
			{
				var reverseXml = new PaymentBokuReverseChargeRequest(this.Settings)
				{
					Country = response.Country,
					MerchantRequestId = response.MerchantRequestId
				}.CreatePostString();
				var reverseCharge = this.Post<PaymentBokuReverseChargeResponse>(
					reverseXml,
					Constants.PAYMENT_BOKU_REVERSE_CHARGE_URL,
					null);
			}

			return response;
		}
		#endregion
	}
}
