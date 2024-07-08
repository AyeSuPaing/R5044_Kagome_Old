/*
=========================================================================================================
  Module      : Atobaraicom authorize status api (AtobaraicomAuthorizeStatusApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment.Atobaraicom.OrderStatus
{
	/// <summary>
	/// Atobaraicom authorie status Api
	/// </summary>
	public class AtobaraicomAuthorizeStatusApi : AtobaraicomBaseApi
	{
		/// <summary>
		/// Create params
		/// </summary>
		/// <param name="paymentOrderIds">Payment order ids</param>
		/// <returns>List params</returns>
		private string[][] CreateParams(string[] paymentOrderIds)
		{
			var parammeters = new List<string[]>();
			if (paymentOrderIds.Length == 0) return parammeters.ToArray();

			parammeters.Add(
				new[]
				{
					AtobaraicomConstants.ATOBARAICOM_API_GET_ORDER_STATUS_BUSINESS_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED
				});
			parammeters.Add(
				new[]
				{
					AtobaraicomConstants.ATOBARAICOM_API_GET_ORDER_STATUS_API_USER_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_API_USER_ID
				});
			parammeters.AddRange(
				paymentOrderIds.Select(paymentOrderId =>
					new[]
					{
						AtobaraicomConstants.ATOBARAICOM_API_GET_ORDER_STATUS_ORDER_NUMBER_LIST,
						paymentOrderId
					}));
			return parammeters.ToArray();
		}

		/// <summary>
		/// Execute get authorize status
		/// </summary>
		/// <param name="paymentOrderIds">Payment order ids</param>
		/// <returns>Order status response</returns>
		public AtobaraicomAuthorizeStatusResponse ExecGetAuthorizeStatus(string[] paymentOrderIds)
		{
			var requestLog = new StringBuilder();
			var responseData = new AtobaraicomAuthorizeStatusResponse();
			try
			{
				var request = CreateParams(paymentOrderIds);
				var responseString = this.PostHttpRequest(request,
					Constants.PAYMENT_ATOBARAICOM_GET_AUTHORIZE_STATUS_APIURL);
				var breakLine = "\r\n";
				var paramString = string.Join(
					",",
					request.Select(p => string.Format("{0}={1}", p[0], p[1], Encoding.UTF8)));
				requestLog.Append(breakLine).Append("Request API Get Authorize Status").Append(breakLine);
				requestLog.Append("URL: ").Append(Constants.PAYMENT_ATOBARAICOM_GET_AUTHORIZE_STATUS_APIURL).Append(breakLine);
				requestLog.Append(paramString).Append(breakLine);
				WriteLog(
					requestLog.ToString(),
					null,
					string.Empty,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentProcessingType.Atobaraicom);

				var responseLog = string.Format("\r\nRequest Response: \r\n{0}\r\n", responseString);
				WriteLog(
					responseLog,
					null,
					string.Empty,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentProcessingType.Atobaraicom);

				responseData.HandleMessages(responseString);
			}
			catch (Exception ex)
			{
				this.ResponseErrorMessage = ex.Message;
				FileLogger.WriteError(" 実行 Atobaraicom決済(後払い) 注文状況取得に失敗しました。", ex);
			}

			return responseData;
		}

		/// <summary>Response error message</summary>
		public string ResponseErrorMessage { get; set; }
	}
}
