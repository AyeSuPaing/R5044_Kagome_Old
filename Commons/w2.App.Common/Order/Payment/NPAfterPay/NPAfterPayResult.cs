/*
=========================================================================================================
  Module      : NP After Pay Result(NPAfterPayResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Text;
using w2.App.Common.ShopMessage;

namespace w2.App.Common.Order.Payment.NPAfterPay
{
	/// <summary>
	/// NP After Pay Result
	/// </summary>
	public class NPAfterPayResult
	{
		#region +Constants
		/// <summary>NP after pay : authori result review ok</summary>
		private const string FLG_NPAFTERPAY_AUTHORI_RESULT_REVIEW_OK = "00";
		/// <summary>NP after pay : authori result hold</summary>
		private const string FLG_NPAFTERPAY_AUTHORI_RESULT_HOLD = "10";
		/// <summary>NP after pay : authori result review ng</summary>
		private const string FLG_NPAFTERPAY_AUTHORI_RESULT_REVIEW_NG = "20";
		/// <summary>NP after pay : authori result before review or review in process</summary>
		private const string FLG_NPAFTERPAY_AUTHORI_RESULT_BEFORE_REVIEW_OR_REVIEW_IN_PROCESS = "40";
		/// <summary>NP after pay : payment status has paid</summary>
		private const string FLG_NPAFTERPAY_PAYMENT_STATUS_HAS_PAID = "20";
		#endregion

		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="response">Response</param>
		public NPAfterPayResult(NPAfterPayResponse response)
		{
			this.Response = response;
		}
		#endregion

		#region +Method
		/// <summary>
		/// Get NP Transaction Id
		/// </summary>
		/// <returns>NP Transaction Id</returns>
		public string GetNPTransactionId()
		{
			var result = (this.IsSuccess
				&& (string.IsNullOrEmpty(this.Response.Results[0].NpTransactionId) == false))
					? this.Response.Results[0].NpTransactionId
					: string.Empty;
			return result;
		}

		/// <summary>
		/// Get Api Error Message
		/// </summary>
		/// <param name="isPc">Is PC</param>
		/// <returns>Api Error Message</returns>
		public string GetApiErrorMessage(bool isPc = false)
		{
			var result = (this.HasError)
				? GetApiMessageByCode(isPc)
				: GetAuthoriErrorMessage(isPc);
			return result;
		}

		/// <summary>
		/// Get Authori Error Message
		/// </summary>
		/// <param name="isPc">Is PC</param>
		/// <returns>Error Message</returns>
		public string GetAuthoriErrorMessage(bool isPc)
		{
			var errorMessage = string.Empty;
			if (this.IsAuthoriReviewNg)
			{
				errorMessage = NPAfterPayUtility.GetErrorMessages(this.Response.Results[0].AuthoriNg);
				return errorMessage;
			}
			else if (this.IsAuthoriReviewPending)
			{
				if (isPc)
				{
					errorMessage = NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_1)
							.Replace("@@ 1 @@", ShopMessageUtil.GetMessage("ShopName"));
					return errorMessage;
				}
				else
				{
					var errorMessages = new StringBuilder();
					foreach (var errorCode in this.Response.Results[0].AuthoriHold)
					{
						errorMessage = NPAfterPayUtility.GetErrorMessages(errorCode);
						if (string.IsNullOrEmpty(errorMessage))
						{
							errorMessages.AppendLine(errorCode);
							continue;
						}

						errorMessages.AppendLine(errorMessage);
					}
					errorMessage = NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_4)
						.Replace("@@ 1 @@", errorMessages.ToString().Trim());
					return errorMessage;
				}
			}
			else if (this.IsAuthoriBeforeReviewOrReviewInProcess)
			{
				errorMessage = string.Format("{0}　ＮＰ取引ＩＤ：{1}",
					NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_2),
					GetNPTransactionId());
				return errorMessage;
			}
			return errorMessage;
		}

		/// <summary>
		/// Get Api Message By Code
		/// </summary>
		/// <param name="isPc">Is PC</param>
		/// <returns>Error Message</returns>
		private string GetApiMessageByCode(bool isPc)
		{
			var errorMessage = string.Empty;
			var formattedErrorMessage = string.Empty;
			var errorMessages = new StringBuilder();
			foreach (var errorCode in this.Response.Errors[0].Codes)
			{
				errorMessage = NPAfterPayUtility.GetErrorMessages(errorCode);
				if (string.IsNullOrEmpty(errorMessage))
				{
					formattedErrorMessage = string.Format("エラーコード：{0}　ＮＰ取引ＩＤ：{1}",
						errorCode,
						this.Response.Errors[0].Id);
					errorMessages.AppendLine(formattedErrorMessage);
					continue;
				}

				formattedErrorMessage = string.Format("エラーコード：{0}　エラー内容：{1}　ＮＰ取引ＩＤ：{2}",
					errorCode,
					errorMessage,
					this.Response.Errors[0].Id);
				errorMessages.AppendLine(formattedErrorMessage);
			}

			errorMessage = (isPc)
				? errorMessages.ToString().Trim()
				: NPAfterPayUtility.GetErrorMessages(Constants.FLG_PAYMENT_NP_AFTERPAY_CUSTOM_ERROR_CODE_5)
					.Replace("@@ 1 @@", errorMessages.ToString().Trim())
					.Replace("@@ 2 @@", Constants.PAYMENT_NP_AFTERPAY_MANUALSITE);
			return errorMessage;
		}
		#endregion

		#region +Property
		/// <summary>Is Authori Ok</summary>
		public bool IsAuthoriReviewOk
		{
			get
			{
				return ((this.Response != null)
					&& (this.Response.Results != null)
					&& (this.Response.Results.Length > 0)
					&& (this.Response.Results[0].AuthoriResult == FLG_NPAFTERPAY_AUTHORI_RESULT_REVIEW_OK));
			}
		}
		/// <summary>Is Authori Review Pending</summary>
		public bool IsAuthoriReviewPending
		{
			get
			{
				return ((this.Response != null)
					&& (this.Response.Results != null)
					&& (this.Response.Results.Length > 0)
					&& (this.Response.Results[0].AuthoriResult == FLG_NPAFTERPAY_AUTHORI_RESULT_HOLD));
			}
		}
		/// <summary>Is Authori Review Ng</summary>
		public bool IsAuthoriReviewNg
		{
			get
			{
				return ((this.Response != null)
					&& (this.Response.Results != null)
					&& (this.Response.Results.Length > 0)
					&& (this.Response.Results[0].AuthoriResult == FLG_NPAFTERPAY_AUTHORI_RESULT_REVIEW_NG));
			}
		}
		/// <summary>Is Authori Before Review Or Review In Process</summary>
		public bool IsAuthoriBeforeReviewOrReviewInProcess
		{
			get
			{
				return ((this.Response != null)
					&& (this.Response.Results != null)
					&& (this.Response.Results.Length > 0)
					&& (this.Response.Results[0].AuthoriResult
						== FLG_NPAFTERPAY_AUTHORI_RESULT_BEFORE_REVIEW_OR_REVIEW_IN_PROCESS));
			}
		}
		/// <summary>Has Paid</summary>
		public bool HasPaid
		{
			get
			{
				return ((this.Response != null)
					&& (this.Response.Results != null)
					&& (this.Response.Results.Length > 0)
					&& (this.Response.Results[0].PaymentStatus == FLG_NPAFTERPAY_PAYMENT_STATUS_HAS_PAID));
			}
		}
		/// <summary>Is Success</summary>
		public bool IsSuccess
		{
			get
			{
				return ((this.Response != null)
					&& (this.Response.Results != null)
					&& (this.Response.Results.Length != 0));
			}
		}
		/// <summary>Has Error</summary>
		public bool HasError
		{
			get
			{
				return ((this.Response != null)
					&& (this.Response.Errors != null)
					&& (this.Response.Errors.Length != 0));
			}
		}
		/// <summary>Response</summary>
		private NPAfterPayResponse Response { get; set; }
		#endregion
	}
}
