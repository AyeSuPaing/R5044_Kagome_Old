/*
=========================================================================================================
  Module      : Payment Information Inquiry Result(PaymentInformationInquiryResult.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment.Paygent.Foundation;

namespace w2.App.Common.Order.Payment.Paygent.PaymentInformationInquiry.Dto
{
	/// <summary>
	/// Payment Information Inquiry Result
	/// </summary>
	[Serializable]
	public class PaymentInformationInquiryResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paygentApiResult">レスポンス結果</param>
		public PaymentInformationInquiryResult(PaygentApiResult paygentApiResult)
		{
			this.Response = (PaymentInformationInquiryResponseDataset)paygentApiResult.Response;
			this.ReasonPhrase = paygentApiResult.ReasonPhrase;
			this.IsSuccess = paygentApiResult.IsSuccess();
		}

		/// <summary>
		/// Get error message
		/// </summary>
		/// <returns>Error message</returns>
		public string GetErrorMessage()
		{
			var errorMessage = (this.Response != null)
				? LogCreator.CreateErrorMessage(
					this.Response.ResponseCode,
					this.Response.ResponseDetail)
				: this.ReasonPhrase ?? string.Empty;
			return errorMessage;
		}

		/// <summary>成功したか</summary>
		public bool IsSuccess { get; private set; }
		/// <summary>レスポンス</summary>
		public PaymentInformationInquiryResponseDataset Response { get; private set; }
		/// <summary>結果理由</summary>
		public string ReasonPhrase { get; private set; }
	}
}
