/*
=========================================================================================================
  Module      : Atobaraicom Shipping Registration Api (AtobaraicomShippingRegistrationApi.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text;
using w2.Common.Helper;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment.Atobaraicom.Shipping
{
	/// <summary>
	/// Atobaraicom shipping registration api
	/// </summary>
	public class AtobaraicomShippingRegistrationApi : AtobaraicomBaseApi
	{
		/// <summary>
		/// Execute shipping registration
		/// </summary>
		/// <param name="orderPaymentId">Order payment id</param>
		/// <param name="shippingCheckNo">Shipping check no</param>
		/// <param name="deliveryId">Delivery id</param>
		/// <param name="invoiceBundleFlg">Invoice bundle flag</param>
		/// <returns>Shipping registration response</returns>
		public AtobaraicomShippingRegistrationResponse ExecShippingRegistration(
			string orderPaymentId,
			string shippingCheckNo,
			string deliveryId,
			string invoiceBundleFlg)
		{
			var responseData = new AtobaraicomShippingRegistrationResponse();
			try
			{
				// Request
				var request = CreateParameters(
					orderPaymentId,
					shippingCheckNo,
					deliveryId,
					invoiceBundleFlg);
				var requestString = string.Join(
					",",
					request.Select(p => string.Format("{0}={1}", p[0], p[1], Encoding.UTF8)));
				var requestLog = string.Format(
					"\r\nRequest Post: \r\nURL: {0}\r\n{1}\r\n",
					Constants.PAYMENT_ATOBARAICOM_SHIPPING_APIURL,
					requestString);

				WriteLog(
					requestLog,
					null,
					string.Empty,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentProcessingType.Atobaraicom);

				// Response
				var responseText = this.PostHttpRequest(request, Constants.PAYMENT_ATOBARAICOM_SHIPPING_APIURL);
				var responseLog = string.Format("\r\nRequest Response: \r\n{0}\r\n", responseText);

				WriteLog(
					responseLog,
					null,
					string.Empty,
					Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
					PaymentFileLogger.PaymentProcessingType.Atobaraicom);

				responseData = SerializeHelper.Deserialize<AtobaraicomShippingRegistrationResponse>(responseText);
				responseData.HandleApiMessages();
				return responseData;
			}
			catch (Exception ex)
			{
				FileLogger.WriteError("伝票番号登録 後払い.com 決済に失敗しました。", ex);
				responseData.Status = AtobaraicomConstants.ATOBARAICOM_API_RESULT_STATUS_ERROR;
				responseData.ApiMessages = "API Fail " + ex.Message;
				return responseData;
			}
		}

		/// <summary>
		/// Create parameters
		/// </summary>
		/// <param name="orderPaymentId">Order payment id</param>
		/// <param name="shippingCheckNo">Shipping check no</param>
		/// <param name="deliveryId">Delivery id</param>
		/// <param name="invoiceBundleFlg">Invoice bundle flag</param>
		/// <returns>Parameters</returns>
		private string[][] CreateParameters(
			string orderPaymentId,
			string shippingCheckNo,
			string deliveryId,
			string invoiceBundleFlg)
		{
			var cSeparateShipment = ((invoiceBundleFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
				? AtobaraicomConstants.FLG_ATOBARAICOM_API_C_SEPARATE_SHIPMENT_INVALID
				: AtobaraicomConstants.FLG_ATOBARAICOM_API_C_SEPARATE_SHIPMENT_VALID);
			var result = new string[][]
			{
				new string [] { AtobaraicomConstants.ATOBARAICOM_API_PARAM_ENTERPRISE_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED },
				new string [] { AtobaraicomConstants.ATOBARAICOM_API_PARAM_API_USER_ID,
					Constants.PAYMENT_SETTING_ATOBARAICOM_API_USER_ID },
				new string [] { AtobaraicomConstants.ATOBARAICOM_API_PARAM_ORDER_ID, orderPaymentId },
				new string [] { AtobaraicomConstants.ATOBARAICOM_API_PARAM_DELIV_ID, deliveryId },
				new string [] { AtobaraicomConstants.ATOBARAICOM_API_PARAM_JOURNAL_NUM , shippingCheckNo },
				new string [] { AtobaraicomConstants.ATOBARAICOM_API_PARAM_C_SEPARATE_SHIPMENT, cSeparateShipment },
			};
			return result;
		}
	}
}
