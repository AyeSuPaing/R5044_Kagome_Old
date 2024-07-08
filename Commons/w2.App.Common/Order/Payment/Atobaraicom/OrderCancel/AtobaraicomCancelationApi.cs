/*
=========================================================================================================
  Module      : 後払いキャンセルAPI (AtobaraicomCancelationApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.Atobaraicom;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払いキャンセルAPI
	/// </summary>
	public class AtobaraicomCancelationApi : AtobaraicomBaseApi
	{
		/// <summary>
		/// パラメータ作成 後払いキャンセル(後払い) 決済取消
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <returns>List param</returns>
		private string[][] CreateParam(string orderId)
		{
			var lstParam = new List<string[]>();
			if (string.IsNullOrEmpty(orderId) == false)
			{
				lstParam.Add(
					new[] { AtobaraicomConstants.ATOBARAICOM_API_CANCEL_BUSINESS_ID,
						Constants.PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED });
				lstParam.Add(
					new[] { AtobaraicomConstants.ATOBARAICOM_API_CANCEL_API_USER_ID,
						Constants.PAYMENT_SETTING_ATOBARAICOM_API_USER_ID });
				lstParam.Add(
					new[] { AtobaraicomConstants.ATOBARAICOM_API_CANCEL_ORDER_NUMBER_LIST, orderId });
			}
			return lstParam.ToArray();
		}

		/// <summary>
		/// 実行 後払いキャンセル(後払い) 決済取消
		/// </summary>
		/// <param name="input">Input</param>
		/// <returns>True or false</returns>
		public bool ExecCancel(string input)
		{
			var requestLog = new StringBuilder();
			var result = true;
			var responseData = new AtobaraicomCancelResponse();
			try
			{
				var request = CreateParam(input);
				var responseString = this.PostHttpRequest(request,
					Constants.PAYMENT_SETTING_ATOBARAICOM_CANCELATION_URL);
				var breadLine = "\r\n";
				var paramString = string.Join(
					",",
					request.Select(p => string.Format("{0}={1}", p[0], p[1], Encoding.UTF8)));
				requestLog.Append(breadLine).Append("Request API Cancel").Append(breadLine);
				requestLog.Append("URL: " + Constants.PAYMENT_SETTING_ATOBARAICOM_CANCELATION_URL).Append(breadLine);
				requestLog.Append(paramString).Append(breadLine);
				WriteLog(
					requestLog.ToString(),
					null,
					string.Empty,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentProcessingType.Atobaraicom);

				responseData.HandleMessages(responseString);

				if (string.IsNullOrEmpty(responseString)
					|| responseData.IsCancelStatusError)
				{
					this.ErrorCode = responseData.ErrorCode;
					this.ResponseMessage = responseData.ErrorMessage;
					result = false;
				}
				else
				{
					this.ResponseMessage = responseData.Status;
				}
			}
			catch (Exception ex)
			{
				result = false;
				this.ResponseMessage = ex.Message;
				w2.Common.Logger.FileLogger.WriteError(" 実行 後払いキャンセル(後払い) 決済取消に失敗しました。", ex);
			}

			PaymentFileLogger.WritePaymentLog(
				true,
				input,
				PaymentFileLogger.PaymentType.Atobaraicom,
				PaymentFileLogger.PaymentProcessingType.ExecPayment,
				this.ResponseMessage,
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, input },
					{ Constants.FIELD_ORDER_PAYMENT_ORDER_ID, responseData.PaymentId },
				});

			return result;
		}

		/// <summary>応答メッセージ</summary>
		public string ResponseMessage { get; set; }
		/// <summary>エラーコード</summary>
		public string ErrorCode { get; set; }
	}
}
