/*
=========================================================================================================
  Module      : Paidy Result Object (PaidyResultObject.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.Payment.Paidy
{
	/// <summary>
	/// Paidy result object
	/// </summary>
	public class PaidyResultObject
	{
		/// <summary>
		/// Get Api Error Messages
		/// </summary>
		/// <returns>Error Messages</returns>
		public string GetApiErrorMessages()
		{
			var apiErrorMessage = string.Empty;

			if (this.Error != null)
			{
				apiErrorMessage = string.Format("Paidy処理に失敗しました。{0} {1}", this.Error.Code, this.Error.Description);
			}
			else if (this.InnerException != null)
			{
				apiErrorMessage = string.Format("Paidy処理に失敗しました。{0}", this.InnerException);
			}
			else if (this.IsRejectedPayment)
			{
				apiErrorMessage = string.Format("Paidy処理に失敗しました。{0} 決済否決", this.Payment.Status);
			}

			return apiErrorMessage;
		}

		/// <summary>
		/// Get Api Error Code And Messages
		/// </summary>
		/// <returns>Error Code And Messages</returns>
		public string GetApiErrorCodeAndMessages()
		{
			var errorMessages = (this.Error != null)
				? string.Format("Paidy処理に失敗しました。{0} {1}", this.Error.Code, this.Error.Description)
				: string.Empty;

			return errorMessages;
		}

		/// <summary>Has response error</summary>
		public bool HasError
		{
			get
			{
				return (this.Error != null)
					|| (this.InnerException != null)
					|| this.IsRejectedPayment;
			}
		}
		/// <summary>Is rejected payment</summary>
		public bool IsRejectedPayment
		{
			get
			{
				return ((this.Payment != null)
					&& (this.Payment.Status == Constants.FLG_PAYMENT_PAIDY_API_STATUS_REJECTED));
			}
		}
		/// <summary>Paidy payment object</summary>
		public PaidyPaymentObject Payment { get; set; }
		/// <summary>Paidy token object</summary>
		public PaidyTokenObject Token { get; set; }
		/// <summary>Paidy error object</summary>
		public PaidyErrorObject Error { get; set; }
		/// <summary>Inner exception</summary>
		public Exception InnerException { get; set; }
	}
}
