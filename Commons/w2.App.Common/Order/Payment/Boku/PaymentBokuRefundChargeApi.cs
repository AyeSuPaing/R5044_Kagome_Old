/*
=========================================================================================================
  Module      : Payment Boku Refund Charge API(PaymentBokuRefundChargeApi.cs)
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
	/// Payment Boku Refund Charge API
	/// </summary>
	public class PaymentBokuRefundChargeApi : PaymentBokuBaseApi
	{
		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public PaymentBokuRefundChargeApi()
			: this(PaymentBokuSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settings">Boku settings</param>
		public PaymentBokuRefundChargeApi(PaymentBokuSetting settings)
			: base(settings)
		{
		}
		#endregion

		#region Methods
		/// <summary>
		/// Exec Api
		/// </summary>
		/// <param name="currency">Currency</param>
		/// <param name="chargeId">Charge id</param>
		/// <param name="merchantRefundId">Merchant refund id</param>
		/// <param name="reasonCode">Reason code</param>
		/// <param name="refundAmount">Refund amount</param>
		/// <param name="isIncludeTax">Is include tax</param>
		/// <returns>Refund charge response</returns>
		public PaymentBokuRefundChargeResponse Exec(
			string currency,
			string chargeId,
			string merchantRefundId,
			int reasonCode,
			string refundAmount,
			bool isIncludeTax)
		{
			// XML作成
			var request = new PaymentBokuRefundChargeRequest(this.Settings);
			request.SetValue(
				currency,
				chargeId,
				merchantRefundId,
				reasonCode,
				refundAmount,
				isIncludeTax);
			var requestXml = request.CreatePostString();

			// 実行
			var response = this.Post<PaymentBokuRefundChargeResponse>(
				requestXml,
				Constants.PAYMENT_BOKU_REFUND_CHARGE_URL,
				null);
			if (response == null) return null;

			var execCount = 0;
			var queryXml = new PaymentBokuQueryRefundRequest(this.Settings)
				{ RefundId = response.RefundId }.CreatePostString();
			while (response.RefundStatus == BokuConstants.CONST_BOKU_REFUND_STATUS_IN_PROGRESS)
			{
				var queryRefund = this.Post<PaymentBokuQueryRefundResponse>(
					queryXml,
					Constants.PAYMENT_BOKU_QUERY_REFUND_URL,
					null);
				if ((queryRefund != null) && queryRefund.IsSuccess)
				{
					response = new PaymentBokuRefundChargeResponse(queryRefund.Refunds.Refunds[0]);
				}
				execCount++;
			}
			return response;
		}
		#endregion
	}
}
