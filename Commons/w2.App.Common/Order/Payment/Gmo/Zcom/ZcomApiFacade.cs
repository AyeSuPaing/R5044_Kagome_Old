/*
=========================================================================================================
  Module      : ZcomApi実行用の窓口 (ZcomApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Net;
using System.Text;
using w2.App.Common.Order.Payment.GMO.Zcom.Cancel;
using w2.App.Common.Order.Payment.GMO.Zcom.CheckAuth;
using w2.App.Common.Order.Payment.GMO.Zcom.Direct;
using w2.App.Common.Order.Payment.GMO.Zcom.Sales;
using w2.Common.Helper;
using w2.Common.Logger;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.GMO.Zcom
{
	/// <summary>
	/// ZcomApi実行用の窓口
	/// </summary>
	public class ZcomApiFacade : IZcomApiFacade
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZcomApiFacade()
		{
			var setting = new ZcomApiSetting();
			setting.BasicUserId = Constants.PAYMENT_CREDIT_ZCOM_BASIC_USER_ID;
			setting.BasicPassword = Constants.PAYMENT_CREDIT_ZCOM_BASIC_PASSWORD;
			setting.UrlDirectPayment = Constants.PAYMENT_CREDIT_ZCOM_APIURL_DIRECTPAYMENT;
			setting.UrlCancelPayment = Constants.PAYMENT_CREDIT_ZCOM_APIURL_CANCELPAYMENT;
			setting.UrlSalsePayment = Constants.PAYMENT_CREDIT_ZCOM_APIURL_SALESPAYMENT;
			setting.ApiEncoding = Encoding.UTF8;
			setting.OnAfterRequest = AfterRequestProc;
			setting.OnBeforeRequest = BeforeRequestProc;
			setting.OnExtendWebRequest = ExtendWebRequestProc;
			setting.OnRequestError = RequestErrorProc;
			setting.HttpTimeOutMiriSecond = 0;
			this.ApiSetting = setting;
			setting.UrlCheckAuth = Constants.PAYMENT_CREDIT_USE_ZCOM3DS_MOCK
				? Constants.PAYMENT_CREDIT_ZCOM_APIURL_CHECKAUTH_MOCK
				: Constants.PAYMENT_CREDIT_ZCOM_APIURL_CHECKAUTH;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="setting">ZcomAPI設定</param>
		public ZcomApiFacade(ZcomApiSetting setting)
		{
			this.ApiSetting = setting;
		}

		/// <summary>
		/// 決済実施
		/// </summary>
		/// <param name="request">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		public ZcomDirectResponse DirectPayment(ZcomDirectRequest request)
		{
			var response = this.CallApi<ZcomDirectResponse>(this.ApiSetting.UrlDirectPayment, request);
			return response;
		}

		/// <summary>
		/// 取消し実施
		/// </summary>
		/// <param name="request">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		public ZcomCancelResponse CancelPayment(ZcomCancelRequest request)
		{
			var response = this.CallApi<ZcomCancelResponse>(this.ApiSetting.UrlCancelPayment, request);
			return response;
		}

		/// <summary>
		/// 実売上実施
		/// </summary>
		/// <param name="request">リクエストデータ</param>
		/// <returns>レスポンスデータ</returns>
		public ZcomSalesResponse SalesPayment(ZcomSalesRequest request)
		{
			var response = this.CallApi<ZcomSalesResponse>(this.ApiSetting.UrlSalsePayment, request);
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
		private T CallApi<T>(string apiUrl, BaseZcomRequest requestData)
			where T : BaseZcomResponse
		{
			string response;
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
			var request = apiRequestData as BaseZcomRequest;
			var postdata = (request != null)
				? request.CreateMaskedPostString()
				: apiRequestData.CreatePostString();

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Zcom,
				PaymentFileLogger.PaymentProcessingType.AfterApiRequest,
				LogCreator.CreateRequestMessage(
					apiRequestData.GetType().Name,
					postdata,
					PaymentFileLogger.PaymentProcessingType.Request.ToText() + "開始"));
		}

		/// <summary>
		/// APIリクエスト後処理
		/// </summary>
		/// <param name="apiRequestData">リクエスト値</param>
		/// <param name="response">結果</param>
		private void AfterRequestProc(IHttpApiRequestData apiRequestData, string response)
		{
			// ログ書く
			var request = apiRequestData as BaseZcomRequest;
			var postdata = (request != null)
				? request.CreateMaskedPostString()
				: apiRequestData.CreatePostString();

			// ここでResponseを結果としてログに書き込むことも可能
			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Zcom,
				PaymentFileLogger.PaymentProcessingType.AfterApiRequest,
				LogCreator.CreateRequestMessage(
					apiRequestData.GetType().Name, 
					postdata,
					PaymentFileLogger.PaymentProcessingType.Request.ToText() + "終了"));
		}

		/// <summary>
		/// APIエラー時処理
		/// </summary>
		/// <param name="apiRequestData">リクエスト値</param>
		/// <param name="url">リクエストしたURL</param>
		/// <param name="ex">発生Exception</param>
		private void RequestErrorProc(IHttpApiRequestData apiRequestData, string url, Exception ex)
		{
			var request = apiRequestData as BaseZcomRequest;
			var postdata = (request != null)
				? request.CreateMaskedPostString()
				: apiRequestData.CreatePostString();

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				false,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Zcom,
				PaymentFileLogger.PaymentProcessingType.AfterApiRequest,
				LogCreator.CreateRequestMessage(
					apiRequestData.GetType().Name,
					postdata,
					PaymentFileLogger.PaymentProcessingType.Request.ToText() + "エラー",
					BaseLogger.CreateExceptionMessage(ex)));
		}

		/// <summary>
		/// Webリクエストの拡張処理
		/// </summary>
		/// <param name="webRequest">HTTPリクエストの際のWebRequest</param>
		private void ExtendWebRequestProc(HttpWebRequest webRequest)
		{
			// ContentType指定
			webRequest.ContentType = "application/x-www-form-urlencoded";

			// タイムアウトの指定
			if (this.ApiSetting.HttpTimeOutMiriSecond > 0)
			{
				webRequest.Timeout = this.ApiSetting.HttpTimeOutMiriSecond;
			}
		}

		/// <summary>
		/// Zcom credit check auth
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>Response</returns>
		public ZcomCheckAuthResponse ZcomCreditCheckAuth(ZcomCheckAuthRequest request)
		{
			var response = this.CallApi<ZcomCheckAuthResponse>(this.ApiSetting.UrlCheckAuth, request);
			return response;
		}

		/// <summary>API設定</summary>
		protected ZcomApiSetting ApiSetting { get; set; }
	}
}
