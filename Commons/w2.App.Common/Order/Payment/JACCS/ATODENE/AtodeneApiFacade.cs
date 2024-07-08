/*
=========================================================================================================
  Module      : AtodeneのAPIファサード(AtodeneApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net;
using System.Text;
using w2.App.Common.Order.Payment.JACCS.ATODENE.GetAuth;
using w2.App.Common.Order.Payment.JACCS.ATODENE.GetInvoice;
using w2.App.Common.Order.Payment.JACCS.ATODENE.ModifyTransaction;
using w2.App.Common.Order.Payment.JACCS.ATODENE.Shipping;
using w2.App.Common.Order.Payment.JACCS.ATODENE.Transaction;
using w2.Common.Extensions;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE
{
	/// <summary>
	/// AtodeneのAPIファサード
	/// </summary>
	public class AtodeneApiFacade
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AtodeneApiFacade()
		{
			var setting = new AtodeneApiSetting();
			setting.BasicUserId = Constants.PAYMENT_SETTING_ATODENE_BASIC_USER_ID;
			setting.BasicPassword = Constants.PAYMENT_SETTING_ATODENE_BASIC_PASSWORD;
			setting.UrlOrderRegister = Constants.PAYMENT_SETTING_ATODENE_URL_ORDERREGISTER;
			setting.UrlGetAuthResult = Constants.PAYMENT_SETTING_ATODENE_URL_GETAUTHRESULT;
			setting.UrlShipment = Constants.PAYMENT_SETTING_ATODENE_URL_SHIPMENT;
			setting.UrlGetinvoicePrintData = Constants.PAYMENT_SETTING_ATODENE_URL_GETINVOICEPRINTDATA;
			setting.UrlOrderModifyCancel = Constants.PAYMENT_SETTING_ATODENE_URL_ORDERMODIFYCANCEL;
			setting.ApiEncoding = Encoding.UTF8;
			setting.OnAfterRequest = AfterRequestProc;
			setting.OnBeforeRequest = BeforeRequestProc;
			setting.OnExtendWebRequest = ExtendWebRequestProc;
			setting.OnRequestError = RequestErrorProc;
			setting.HttpTimeOutMiriSecond = 0;
			this.ApiSetting = setting;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting">API設定</param>
		public AtodeneApiFacade(AtodeneApiSetting apiSetting)
		{
			this.ApiSetting = apiSetting;
		}

		/// <summary>
		/// 取引登録
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public AtodeneTransactionResponse OrderRegister(AtodeneTransactionRequest request)
		{
			var response = this.CallApi<AtodeneTransactionResponse>(this.ApiSetting.UrlOrderRegister, request);
			return response;
		}

		/// <summary>
		/// 与信結果取得
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public AtodeneGetAuthResponse GetAuth(AtodeneGetAuthRequest request)
		{
			var response = this.CallApi<AtodeneGetAuthResponse>(this.ApiSetting.UrlGetAuthResult, request);
			return response;
		}

		/// <summary>
		/// 出荷報告
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public AtodeneShippingResponse Shipment(AtodeneShippingRequest request)
		{
			var response = this.CallApi<AtodeneShippingResponse>(this.ApiSetting.UrlShipment, request);
			return response;
		}

		/// <summary>
		/// 請求書印字データ取得
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public AtodeneGetInvoiceResponse GetInvoiceData(AtodeneGetInvoiceRequest request)
		{
			var response = this.CallApi<AtodeneGetInvoiceResponse>(this.ApiSetting.UrlGetinvoicePrintData, request);
			return response;
		}

		/// <summary>
		/// 取引変更・取消
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public AtodeneModifyTransactionResponse OrderModifyCancel(AtodeneModifyTransactionRequest request)
		{
			var response = this.CallApi<AtodeneModifyTransactionResponse>(this.ApiSetting.UrlOrderModifyCancel, request);
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
		private T CallApi<T>(string apiUrl, BaseAtodeneRequest requestData)
			where T : BaseAtodeneResponse
		{
			var response = "";
			using (var conn = new HttpApiConnector())
			{
				// 各種処理をセット
				conn.SetBeforeRequestProc(this.ApiSetting.OnBeforeRequest);
				conn.SetAfterRequestProc(this.ApiSetting.OnAfterRequest);
				conn.SetRequestErrorProc(this.ApiSetting.OnRequestError);
				conn.SetExtendWebRequestProc(this.ApiSetting.OnExtendWebRequest);

				response = conn.Do(apiUrl, this.ApiSetting.ApiEncoding, requestData, this.ApiSetting.BasicUserId, this.ApiSetting.BasicPassword);
			}

			T rtn = SerializeHelper.Deserialize<T>(response);

			return rtn;
		}
		#endregion

		/// <summary>
		/// APIリクエスト前の処理
		/// </summary>
		/// <param name="apiRequestData">リクエスト値</param>
		private void BeforeRequestProc(IHttpApiRequestData apiRequestData)
		{
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				null,
				PaymentFileLogger.PaymentType.Atodene.ToText(),
				PaymentFileLogger.PaymentType.Atodene,
				PaymentFileLogger.PaymentProcessingType.BeforeApiRequest,
				LogCreator.CreateRequestMessage(
					apiRequestData.GetType().Name,
					apiRequestData.CreatePostString(),
					PaymentFileLogger.PaymentType.Atodene.ToText()
					+ PaymentFileLogger.PaymentProcessingType.ApiRequestStart.ToText()));
		}

		/// <summary>
		/// APIリクエスト後処理
		/// </summary>
		/// <param name="apiRequestData">リクエスト値</param>
		/// <param name="response">結果</param>
		private void AfterRequestProc(IHttpApiRequestData apiRequestData, string response)
		{
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				null,
				PaymentFileLogger.PaymentType.Atodene.ToText(),
				PaymentFileLogger.PaymentType.Atodene,
				PaymentFileLogger.PaymentProcessingType.AfterApiRequest,
				LogCreator.CreateRequestMessageWithResult(
					apiRequestData.GetType().Name,
					apiRequestData.CreatePostString(),
					PaymentFileLogger.PaymentType.Atodene.ToText()
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
			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				false,
				PaymentFileLogger.PaymentType.Atodene.ToText(),
				PaymentFileLogger.PaymentType.Atodene,
				PaymentFileLogger.PaymentProcessingType.Request,
				LogCreator.CreateRequestMessage(
					apiRequestData.GetType().Name,
					apiRequestData.CreatePostString(),
					PaymentFileLogger.PaymentProcessingType.RequestError.ToText()));
		}

		/// <summary>
		/// Webリクエストの拡張処理
		/// </summary>
		/// <param name="webRequest">HTTPリクエストの際のWebRequest</param>
		private void ExtendWebRequestProc(HttpWebRequest webRequest)
		{
			// ContentType指定
			webRequest.ContentType = "application/xml";

			// タイムアウトの指定
			if (this.ApiSetting.HttpTimeOutMiriSecond > 0)
			{
				webRequest.Timeout = this.ApiSetting.HttpTimeOutMiriSecond;
			}
		}

		/// <summary>API設定</summary>
		protected AtodeneApiSetting ApiSetting { get; set; }
	}
}
