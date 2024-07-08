/*
=========================================================================================================
  Module      : Payment Information Inquiry API(PaymentInformationInquiry.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.Paygent.Foundation;
using w2.App.Common.Order.Payment.Paygent.PaymentInformationInquiry.Dto;

namespace w2.App.Common.Order.Payment.Paygent.PaymentInformationInquiry
{
	/// <summary>
	/// Payment Information Inquiry API
	/// </summary>
	public class PaymentInformationInquiryApi
	{
		/// <summary>決済情報照会電文</summary>
		private const string PAYGENT_CVS_API_NAME = "決済情報照会電文";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="paymentId">Payment id</param>
		/// <param name="tradingId">Trading id</param>
		public PaymentInformationInquiryApi(string paymentId, string tradingId)
		{
			var postBody = new PaymentInformationInquiryPostBody(paymentId, tradingId);
			this.PostBody = postBody;
		}

		/// <summary>
		/// Get
		/// </summary>
		/// <returns>Result</returns>
		public PaymentInformationInquiryResult Get()
		{
			var response = new PaygentApiService(
					PAYGENT_CVS_API_NAME,
					this.PostBody.GenerateKeyValues())
				.GetResult<PaymentInformationInquiryResponseDataset>();
			var result = new PaymentInformationInquiryResult(response);
			return result;
		}

		/// <summary>Post body</summary>
		public PaymentInformationInquiryPostBody PostBody { get; private set; }
	}
}
