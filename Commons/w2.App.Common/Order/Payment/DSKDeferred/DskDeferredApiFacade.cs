/*
=========================================================================================================
  Module      : DSK後払いのAPIファサード(DskDeferredApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.App.Common.Order.Payment.DSKDeferred.GetAuth;
using w2.App.Common.Order.Payment.DSKDeferred.GetInvoice;
using w2.App.Common.Order.Payment.DSKDeferred.OrderCancel;
using w2.App.Common.Order.Payment.DSKDeferred.OrderModify;
using w2.App.Common.Order.Payment.DSKDeferred.OrderRegister;
using w2.App.Common.Order.Payment.DSKDeferred.Shipment;
using w2.Common.Extensions;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.DSKDeferred
{
	/// <summary>
	/// DSK後払いのAPIファサード
	/// </summary>
	public class DskDeferredApiFacade
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DskDeferredApiFacade()
		{
			var setting = new DskDeferredApiSetting();
			setting.UrlOrderRegister = Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_ORDERREGISTER;
			setting.UrlOrderModify = Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_ORDERMODIFY;
			setting.UrlGetAuthResult = Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_GETAUTHRESULT;
			setting.UrlOrderCancel = Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_ORDERCANCEL;
			setting.UrlGetinvoicePrintData = Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_GETINVOICEPRINTDATA;
			setting.UrlShipment = Constants.PAYMENT_SETTING_DSK_DEFERRED_URL_SHIPMENT;
			setting.ApiEncoding = Encoding.UTF8;
			setting.OnAfterRequest = AfterRequestProc;
			setting.OnBeforeRequest = BeforeRequestProc;
			setting.OnRequestError = RequestErrorProc;
			this.ApiSetting = setting;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting">API設定</param>
		public DskDeferredApiFacade(DskDeferredApiSetting apiSetting)
		{
			this.ApiSetting = apiSetting;
		}

		/// <summary>
		/// 取引登録
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public DskDeferredOrderRegisterResponse OrderRegister(DskDeferredOrderRegisterRequest request)
		{
			var response = CallApi<DskDeferredOrderRegisterResponse>(this.ApiSetting.UrlOrderRegister, request);
			return response;
		}

		/// <summary>
		/// 取引キャンセル
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public DskDeferredOrderCancelResponse OrderCancel(DskDeferredOrderCancelRequest request)
		{
			var response = CallApi<DskDeferredOrderCancelResponse>(this.ApiSetting.UrlOrderCancel, request);
			return response;
		}

		/// <summary>
		/// 取引変更
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public DskDeferredOrderModifyResponse OrderModify(DskDeferredOrderModifyRequest request)
		{
			var response = CallApi<DskDeferredOrderModifyResponse>(this.ApiSetting.UrlOrderModify, request);
			return response;
		}

		/// <summary>
		/// 出荷報告
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public DskDeferredShipmentResponse Shipment(DskDeferredShipmentRequest request)
		{
			var response = CallApi<DskDeferredShipmentResponse>(this.ApiSetting.UrlShipment, request);
			return response;
		}

		/// <summary>
		/// 請求書印字データ取得
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public DskDeferredGetInvoiceResponse GetInvoiceData(DskDeferredGetInvoiceRequest request)
		{
			var response = CallApi<DskDeferredGetInvoiceResponse>(this.ApiSetting.UrlGetinvoicePrintData, request);
			return response;
		}

		/// <summary>
		///与信結果取得
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public DskDeferredGetAuthResultResponse GetAuthResult(DskDeferredGetAuthResultRequest request)
		{
			var response = CallApi<DskDeferredGetAuthResultResponse>(this.ApiSetting.UrlGetAuthResult, request);
			return response;
		}

		#region -CallAPI API実行
		/// <summary>
		/// API実行
		/// </summary>
		/// <typeparam name="T">レスポンスのデータ型</typeparam>
		/// <param name="apiUrl">ApiのURL</param>
		/// <param name="requestData">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		private T CallApi<T>(string apiUrl, BaseDskDeferredRequest requestData)
			where T : BaseDskDeferredResponse
		{
			var response = "";
			using (var connector = new HttpApiConnector())
			{
				connector.SetBeforeRequestProc(this.ApiSetting.OnBeforeRequest);
				connector.SetAfterRequestProc(this.ApiSetting.OnAfterRequest);
				connector.SetRequestErrorProc(this.ApiSetting.OnRequestError);

				response = connector.Do(apiUrl, this.ApiSetting.ApiEncoding, requestData, true);
			}

			T result = SerializeHelper.Deserialize<T>(response);

			return result;
		}
		#endregion

		/// <summary>
		/// APIリクエスト前の処理
		/// </summary>
		/// <param name="apiRequestData">リクエスト値</param>
		private void BeforeRequestProc(IHttpApiRequestData apiRequestData)
		{
			PaymentFileLogger.WritePaymentLog(
				null,
				PaymentFileLogger.PaymentType.DskDeferred.ToText(),
				PaymentFileLogger.PaymentType.DskDeferred,
				PaymentFileLogger.PaymentProcessingType.BeforeApiRequest,
				LogCreator.CreateRequestMessage(
					apiRequestData.GetType().Name,
					apiRequestData.CreatePostString(),
					PaymentFileLogger.PaymentType.DskDeferred.ToText()
					+ PaymentFileLogger.PaymentProcessingType.ApiRequestStart.ToText()));
		}

		/// <summary>
		/// APIリクエスト後処理
		/// </summary>
		/// <param name="apiRequestData">リクエスト値</param>
		/// <param name="response">結果</param>
		private void AfterRequestProc(IHttpApiRequestData apiRequestData, string response)
		{
			PaymentFileLogger.WritePaymentLog(
				null,
				PaymentFileLogger.PaymentType.DskDeferred.ToText(),
				PaymentFileLogger.PaymentType.DskDeferred,
				PaymentFileLogger.PaymentProcessingType.AfterApiRequest,
				LogCreator.CreateRequestMessageWithResult(
					apiRequestData.GetType().Name,
					apiRequestData.CreatePostString(),
					PaymentFileLogger.PaymentType.DskDeferred.ToText()
						+ PaymentFileLogger.PaymentProcessingType.ApiRequestEnd,
					response.RemoveWhiteSpaceChars()));
		}

		/// <summary>
		/// APIエラー時処理
		/// </summary>
		/// <param name="apiRequestData">リクエスト値</param>
		/// <param name="url">リクエストしたURL</param>
		/// <param name="ex">発生Exception</param>
		private void RequestErrorProc(IHttpApiRequestData apiRequestData, string url, Exception ex)
		{
			PaymentFileLogger.WritePaymentLog(
				false,
				PaymentFileLogger.PaymentType.DskDeferred.ToText(),
				PaymentFileLogger.PaymentType.DskDeferred,
				PaymentFileLogger.PaymentProcessingType.Request,
				LogCreator.CreateRequestMessage(
					apiRequestData.GetType().Name,
					apiRequestData.CreatePostString(),
					PaymentFileLogger.PaymentProcessingType.RequestError.ToText()));
		}

		/// <summary>API設定</summary>
		private DskDeferredApiSetting ApiSetting { get; set; }
	}
}
