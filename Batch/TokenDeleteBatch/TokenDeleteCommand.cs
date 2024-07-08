/*
=========================================================================================================
  Module      : Token Delete Command (TokenDeleteCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.Order.Payment.Paidy;
using w2.Common.Logger;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.UserCreditCard;

namespace w2.Commerce.Batch.TokenDeleteBatch
{
	/// <summary>
	/// Token Delete Command Class
	/// </summary>
	public class TokenDeleteCommand
	{
		#region +Constructor
		// <summary>
		/// Constructor
		/// </summary>
		/// <param name="paymentPaidyTokenDeleteLimitDay">Payment Paidy Token Delete Limit Day</param>
		public TokenDeleteCommand(int paymentPaidyTokenDeleteLimitDay)
		{
			this.PaymentPaidyTokenDeleteLimitDay = paymentPaidyTokenDeleteLimitDay;
			this.SuccessCount = 0;
			this.FailureCount = 0;
		}
		#endregion

		#region +Method
		/// <summary>
		/// Token Delete Execute
		/// </summary>
		public void Exec()
		{
			var userCreditCardService = new UserCreditCardService();
			var userCreditCardExpiredForPaymentPaidys = userCreditCardService.GetUserCreditCardExpiredForPaymentPaidys(
				this.PaymentPaidyTokenDeleteLimitDay,
				Constants.FLG_USERCREDITCARD_COOPERATION_TYPE_PAIDY,
				Constants.MASK_STRING);

			foreach (var userCreditCard in userCreditCardExpiredForPaymentPaidys)
			{
				var result = PaidyPaymentApiFacade.DeleteToken(userCreditCard.CooperationId);

				if (result.HasError)
				{
					this.FailureCount++;
					continue;
				}

				// Update Cooperation Id
				userCreditCardService.UpdateCooperationId(
					userCreditCard.UserId,
					userCreditCard.BranchNo,
					Constants.MASK_STRING,
					Constants.FLG_LASTCHANGED_BATCH,
					UpdateHistoryAction.Insert);

				this.SuccessCount++;
			}

			// Write Log
			WriteResultLog();
		}

		/// <summary>
		/// Write result log
		/// </summary>
		private void WriteResultLog()
		{
			FileLogger.WriteInfo(
				string.Format("\r\n成功 {0}/{2}件\r\n失敗 {1}/{2}件",
					this.SuccessCount,
					this.FailureCount,
					this.TotalCount));
		}
		#endregion

		#region +Property
		/// <summary>Payment Paidy Token Delete Limit Day</summary>
		private int PaymentPaidyTokenDeleteLimitDay { get; set; }
		/// <summary>Success count</summary>
		private int SuccessCount { get; set; }
		/// <summary>Failure count</summary>
		private int FailureCount { get; set; }
		/// <summary>Total count</summary>
		private int TotalCount
		{
			get { return (this.SuccessCount + this.FailureCount); }
		}
		#endregion
	}
}
