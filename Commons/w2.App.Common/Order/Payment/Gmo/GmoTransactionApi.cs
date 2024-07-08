/*
=========================================================================================================
  Module      : Gmo transaction api (GmoTransactionApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.Order.Payment.GMO.BillingConfirmation;
using w2.App.Common.Order.Payment.GMO.BillingModification;
using w2.App.Common.Order.Payment.GMO.FrameGuarantee;
using w2.App.Common.Order.Payment.GMO.GetCreditStatus;
using w2.App.Common.Order.Payment.GMO.TransactionModifyCancel;
using w2.App.Common.Order.Payment.GMO.TransactionReduce;
using w2.App.Common.Order.Payment.GMO.TransactionRegister;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.GMO
{
	public class GmoTransactionApi
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GmoTransactionApi()
		{
			var setting = new GmoTransactionApiSetting();
			setting.OnAfterRequest = AfterRequestProc;
			setting.OnBeforeRequest = BeforeRequestProc;
			setting.OnRequestError = RequestErrorProc;
			setting.OnExtendWebRequest = ExtendWebRequestProc;

			setting.ApiEncoding = Encoding.UTF8;
			setting.BasicUserId = Constants.PAYMENT_SETTING_GMO_DEFERRED_BASICUSERID;
			setting.BasicPassword = Constants.PAYMENT_SETTING_GMO_DEFERRED_BASICPASSWORD;
			setting.HttpTimeOutMiriSecond = 30000;

			setting.AuthenticationId = Constants.SETTING_GMO_PAYMENT_AUTHENTICATIONID;
			setting.ShopCode = Constants.SETTING_GMO_PAYMENT_SHOPCODE;
			setting.ConnectPassword = Constants.SETTING_GMO_PAYMENT_CONNECTPASSWORD;
			setting.UrlTransactionRegister = Constants.SETTING_GMO_PAYMENT_URL_TRANSACTION_REGISTER;
			setting.UrlGetCreditResult = Constants.SETTING_GMO_PAYMENT_URL_GET_CREDIT_RESULT;
			setting.UrlTransactionModifyCancel = Constants.SETTING_GMO_PAYMENT_URL_MODIFY_CANCEL_TRANSACTION;
			setting.UrlBillingConfirm = Constants.SETTING_GMO_PAYMENT_URL_BILLING_CONFIRM;
			setting.UrlBillingModifyCancel = Constants.SETTING_GMO_PAYMENT_URL_BILLING_MODIFY_CANCEL;
			setting.UrlReducedClaims = Constants.SETTING_GMO_PAYMENT_URL_REDUCED_CLAIMS;

			setting.UrlFrameGuaranteeGetStatus = Constants.SETTING_GMO_PAYMENT_URL_FRAME_GUARANTEE_GET_STATUS;
			setting.UrlFrameGuaranteeRegister = Constants.SETTING_GMO_PAYMENT_URL_FRAME_GUARANTEE_REGISTER;
			setting.UrlFrameGuaranteeUpdate = Constants.SETTING_GMO_PAYMENT_URL_FRAME_GUARANTEE_UPDATE;
			this.Settting = setting;
		}
		#endregion

		/// <summary>
		/// 取引の新規登録
		/// </summary>
		/// <param name="request">GMOリクエスト取引登録</param>
		/// <returns>APIレスポンス</returns>
		public GmoResponseTransactionRegister TransactionRegister(GmoRequestTransactionRegister request)
		{
			request.ShopInfo = new ShopInfoElement(this.Settting.AuthenticationId, this.Settting.ShopCode, this.Settting.ConnectPassword);
			var response = this.CallApi<GmoResponseTransactionRegister>(this.Settting.UrlTransactionRegister, request);
			return response;
		}

		/// <summary>
		/// 枠保証ステータス取得
		/// </summary>
		/// <param name="request">Gmoリクエスト枠保証ステータス</param>
		/// <returns>APIレスポンス</returns>
		public GmoResponseFrameGuaranteeGetStatus FrameGuaranteeGetStatus(GmoRequestFrameGuaranteeGetStatus request)
		{
			request.ShopInfo = new ShopInfoElement(this.Settting.AuthenticationId, this.Settting.ShopCode, this.Settting.ConnectPassword);
			var response = this.CallApi<GmoResponseFrameGuaranteeGetStatus>(this.Settting.UrlFrameGuaranteeGetStatus, request);
			return response;
		}

		/// <summary>
		/// 新規枠保証登録
		/// </summary>
		/// <param name="request">GMOリクエスト枠保証登録</param>
		/// <returns>APIレスポンス</returns>
		public GmoResponseFrameGuarantee FrameGuaranteeRegister(GmoRequestFrameGuaranteeRegister request)
		{
			request.ShopInfo = new ShopInfoElement(this.Settting.AuthenticationId, this.Settting.ShopCode, this.Settting.ConnectPassword);
			var response = this.CallApi<GmoResponseFrameGuarantee>(this.Settting.UrlFrameGuaranteeRegister, request);
			return response;
		}

		/// <summary>
		/// 請求の変更またはキャンセル
		/// </summary>
		/// <param name="request">GMOリクエスト請求変更</param>
		/// <returns>APIレスポンス</returns>
		public GmoResponseBillingModification BillingModifyCancel(GmoRequestBillingModification request)
		{
			request.ShopInfo = new ShopInfoElement(this.Settting.AuthenticationId, this.Settting.ShopCode, this.Settting.ConnectPassword);
			var response = this.CallApi<GmoResponseBillingModification>(this.Settting.UrlBillingModifyCancel, request);
			return response;
		}

		/// <summary>
		/// 枠保証情報更新
		/// </summary>
		/// <param name="request">GMOリクエスト枠保証更新</param>
		/// <returns>APIレスポンス</returns>
		public GmoResponseFrameGuarantee FrameGuaranteeUpdate(GmoRequestFrameGuaranteeUpdate request)
		{
			request.ShopInfo = new ShopInfoElement(this.Settting.AuthenticationId, this.Settting.ShopCode, this.Settting.ConnectPassword);
			var response = this.CallApi<GmoResponseFrameGuarantee>(this.Settting.UrlFrameGuaranteeUpdate, request);
			return response;
		}
		
		/// <summary>
		/// 取引の変更またはキャンセル
		/// </summary>
		/// <param name="request">GMOリクエスト取引変更・キャンセル</param>
		/// <returns>APIレスポンス</returns>
		public GmoResponseTransactionModifyCancel TransactionModifyCancel(GmoRequestTransactionModifyCancel request)
		{
			request.ShopInfo = new ShopInfoElement(this.Settting.AuthenticationId, this.Settting.ShopCode, this.Settting.ConnectPassword);
			var response = this.CallApi<GmoResponseTransactionModifyCancel>(this.Settting.UrlTransactionModifyCancel, request);
			return response;
		}

		/// <summary>
		/// 取引のクレジット結果取得
		/// </summary>
		/// <param name="request">GMOリクエスト認証結果取得</param>
		/// <returns>APIレスポンス</returns>
		public GmoResponseGetCreditStatus GetCreditResult(GmoRequestGetCreditStatus request)
		{
			request.ShopInfo = new ShopInfoElement(this.Settting.AuthenticationId, this.Settting.ShopCode, this.Settting.ConnectPassword);
			var response = this.CallApi<GmoResponseGetCreditStatus>(this.Settting.UrlGetCreditResult, request);
			return response;
		}

		/// <summary>
		/// 請求削減(新規取引の取得)
		/// </summary>
		/// <param name="request">GMOリクエスト取引削減</param>
		/// <returns>APIレスポンス</returns>
		public GmoResponseTransactionReduce ReduceClaims(GmoRequestTransactionReduce request)
		{
			request.ShopInfo = new ShopInfoElement(this.Settting.AuthenticationId, this.Settting.ShopCode, this.Settting.ConnectPassword);
			var response = this.CallApi<GmoResponseTransactionReduce>(this.Settting.UrlReducedClaims, request);
			return response;
		}

		/// <summary>
		/// 請求確認
		/// </summary>
		/// <param name="request">GMOリクエスト請求確認</param>
		/// <returns>APIレスポンス</returns>
		public GmoResponseBillingConfirmation BillingConfirm(GmoRequestBillingConfirmation request)
		{
			request.ShopInfo = new ShopInfoElement(this.Settting.AuthenticationId, this.Settting.ShopCode, this.Settting.ConnectPassword);
			var response = this.CallApi<GmoResponseBillingConfirmation>(this.Settting.UrlBillingConfirm, request);
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
		private T CallApi<T>(string apiUrl, BaseGmoRequest requestData)
			where T : BaseGmoResponse
		{
			string response = string.Empty;
			using (var conn = new HttpApiConnector())
			{
				// 各種処理をセット
				conn.SetBeforeRequestProc(this.Settting.OnBeforeRequest);
				conn.SetAfterRequestProc(this.Settting.OnAfterRequest);
				conn.SetRequestErrorProc(this.Settting.OnRequestError);
				conn.SetExtendWebRequestProc(this.Settting.OnExtendWebRequest);

				response = conn.Do(apiUrl, this.Settting.ApiEncoding, requestData, this.Settting.BasicUserId, this.Settting.BasicPassword);
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
				Constants.SETTING_GMO_PAYMENT_URL_WRITE_LOG,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.ApiRequestStart,
				LogCreator.CreateRequestMessage(
					apiRequestData.GetType().Name,
					apiRequestData.CreatePostString(),
					PaymentFileLogger.PaymentType.Gmo.ToText()
						+ "後払い"
				));
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
				Constants.SETTING_GMO_PAYMENT_URL_WRITE_LOG,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.ApiRequestEnd,
					LogCreator.CreateRequestMessageWithResult(
						apiRequestData.GetType().Name,
						apiRequestData.CreatePostString(),
						PaymentFileLogger.PaymentType.Gmo.ToText()
							+ "後払い",
						response
					));
		}

		/// <summary>
		/// APIエラー時処理
		/// </summary>
		/// <param name="apiRequestData">リクエスト値</param>
		/// <param name="url">リクエストしたURL</param>
		/// <param name="ex">発生Exception</param>
		private void RequestErrorProc(IHttpApiRequestData apiRequestData, string url, Exception ex)
		{
			var errorTuple = GetErrorMessageAndErrorCodeByXml(apiRequestData.CreatePostString());

			if (errorTuple == null)
			{ 
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.SETTING_GMO_PAYMENT_URL_WRITE_LOG,
					PaymentFileLogger.PaymentType.Gmo,
					PaymentFileLogger.PaymentProcessingType.ApiError,
					LogCreator.CreateErrorMessage(ex.Message, ex.StackTrace, apiRequestData.CreatePostString(), url));
			}
			else
			{
				PaymentFileLogger.WritePaymentLog(
					false,
					Constants.SETTING_GMO_PAYMENT_URL_WRITE_LOG,
					PaymentFileLogger.PaymentType.Gmo,
					PaymentFileLogger.PaymentProcessingType.ApiError,
					LogCreator.CreateErrorMessage(errorTuple.Item1, errorTuple.Item2, apiRequestData.CreatePostString(), url));
			}
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
			if (this.Settting.HttpTimeOutMiriSecond > 0)
			{
				webRequest.Timeout = this.Settting.HttpTimeOutMiriSecond;
			}
		}

		/// <summary>
		/// xmlの文字列からerrorMessageとerrorCodeを取得
		/// </summary>
		/// <param name="xmlStr">xml文字列</param>
		/// <returns>Item1がerrorCode,Item2がerrorMessageのTuple</returns>
		private static Tuple<string, string> GetErrorMessageAndErrorCodeByXml(string xmlStr)
		{
			var match = Regex.Match(
				xmlStr.Replace(Environment.NewLine, string.Empty),
				@"<errorMessage>(.*)</errorMessage>.*<errCode>(.*)</errCode>",
				RegexOptions.Singleline);

			var errorMessage = match.Groups[1].Value;
			var errorCode = match.Groups[2].Value;
			var errorTuple = (string.IsNullOrEmpty(errorMessage) == false) || (string.IsNullOrEmpty(errorCode) == false)
				? new Tuple<string, string>(errorCode, errorMessage)
				: null;
			return errorTuple;
		}

		/// <summary>API設定</summary>
		private GmoTransactionApiSetting Settting { get; set; }
	}
}
