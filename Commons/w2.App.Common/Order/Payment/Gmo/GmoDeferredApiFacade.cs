/*
=========================================================================================================
  Module      : GMO後払いAPIのファサード(GmoDeferredApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common.Order.Payment.GMO.GetAuthResult;
using w2.App.Common.Order.Payment.GMO.GetDefPaymentStatus;
using w2.App.Common.Order.Payment.GMO.GetinvoicePrintData;
using w2.App.Common.Order.Payment.GMO.OrderModifyCancel;
using w2.App.Common.Order.Payment.GMO.OrderRegister;
using w2.App.Common.Order.Payment.GMO.ReduceSales;
using w2.App.Common.Order.Payment.GMO.Reissue;
using w2.App.Common.Order.Payment.GMO.Shipment;
using w2.App.Common.Order.Payment.GMO.ShipmentModifyCancel;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.GMO
{
	/// <summary>
	/// GMO後払いAPIのファサード
	/// </summary>
	public class GmoDeferredApiFacade
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GmoDeferredApiFacade()
		{
			var setting = new GmoDeferredApiSetting();
			setting.OnAfterRequest = AfterRequestProc;
			setting.OnBeforeRequest = BeforeRequestProc;
			setting.OnRequestError = RequestErrorProc;
			setting.OnExtendWebRequest = ExtendWebRequestProc;

			setting.ApiEncoding = Encoding.UTF8;
			setting.BasicUserId = Constants.PAYMENT_SETTING_GMO_DEFERRED_BASICUSERID;
			setting.BasicPassword = Constants.PAYMENT_SETTING_GMO_DEFERRED_BASICPASSWORD;
			setting.HttpTimeOutMiriSecond = 30000;

			setting.UrlOrderRegister = Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_ORDERREGISTER;
			setting.UrlGetAuthResult = Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_GETAUTHRESULT;
			setting.UrlGetinvoicePrintData = Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_GETINVOICEPRINTDATA;
			setting.UrlOrderModifyCancel = Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_ORDERMODIFYCANCEL;
			setting.UrlReduceSales = Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_REDUCESALES;
			setting.UrlShipment = Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_SHIPMENT;
			setting.UrlShipmentModifyCancel = Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_SHIPMENTMODIFYCANCEL;
			setting.UrlGetDefPaymentStatus = Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_GETDEFPAYMENTSTATUS;
			setting.UrlReissue = Constants.PAYMENT_SETTING_GMO_DEFERRED_URL_REISSUE;

			this.Settting = setting;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="setting">GMOAPI設定クラス</param>
		public GmoDeferredApiFacade(GmoDeferredApiSetting setting)
		{
			this.Settting = setting;
		}
		#endregion

		/// <summary>
		/// 取引登録
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public GmoResponseOrderRegister OrderRegister(GmoRequestOrderRegister request)
		{
			var response = this.CallApi<GmoResponseOrderRegister>(this.Settting.UrlOrderRegister, request);
			return response;
		}

		/// <summary>
		/// 与信審査結果取得
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public GmoResponseGetAuthResult GetAuthResult(GmoRequestGetAuthResult request)
		{
			var response = this.CallApi<GmoResponseGetAuthResult>(this.Settting.UrlGetAuthResult, request);
			return response;
		}

		/// <summary>
		/// 請求書印字データ取得
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public GmoResponseGetinvoicePrintData GetinvoicePrintData(GmoRequestGetinvoicePrintData request)
		{
			var response = this.CallApi<GmoResponseGetinvoicePrintData>(this.Settting.UrlGetinvoicePrintData, request);
			return response;
		}

		/// <summary>
		/// 注文変更・キャンセル
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public GmoResponseOrderModifyCancel OrderModifyCancel(GmoRequestOrderModifyCancel request)
		{
			var response = this.CallApi<GmoResponseOrderModifyCancel>(this.Settting.UrlOrderModifyCancel, request);
			return response;
		}

		/// <summary>
		/// 入金状況確認
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public GmoResponseGetDefPaymentStatus GetDefPaymentStatus(GmoRequestGetDefPaymentStatus request)
		{
			var response = this.CallApi<GmoResponseGetDefPaymentStatus>(this.Settting.UrlGetDefPaymentStatus, request);
			return response;
		}

		/// <summary>
		/// 請求減額
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public GmoResponseReduceSales ReduceSales(GmoRequestReduceSales request)
		{
			var response = this.CallApi<GmoResponseReduceSales>(this.Settting.UrlReduceSales, request);
			return response;
		}

		/// <summary>
		/// 出荷報告
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public GmoResponseShipment Shipment(GmoRequestShipment request)
		{
			var response = this.CallApi<GmoResponseShipment>(this.Settting.UrlShipment, request);
			return response;
		}

		/// <summary>
		/// 出荷報告修正・キャンセル
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public GmoResponseShipmentModifyCancel ShipmentModifyCancel(GmoRequestShipmentModifyCancel request)
		{
			var response = this.CallApi<GmoResponseShipmentModifyCancel>(this.Settting.UrlShipmentModifyCancel, request);
			return response;
		}

		/// <summary>
		/// Invoice reissue application
		/// </summary>
		/// <param name="request">Request</param>
		/// <returns>Response</returns>
		public GmoResponseReissue Reissue(GmoRequestReissue request)
		{
			var response = this.CallApi<GmoResponseReissue>(this.Settting.UrlReissue, request);
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
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
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
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
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

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				false,
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
				PaymentFileLogger.PaymentType.Gmo,
				PaymentFileLogger.PaymentProcessingType.ApiError,
				LogCreator.CreateErrorMessage(errorTuple.Item1, errorTuple.Item2, "", url));
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
				@"<errorMessage>(.*)</errorMessage>.*<errorCode>(.*)</errorCode>",
				RegexOptions.Singleline);

			var errorMessage = match.Groups[1].Value;
			var errorCode = match.Groups[2].Value;
			var errorTuple = (string.IsNullOrEmpty(errorMessage) == false) || (string.IsNullOrEmpty(errorCode) == false)
				? new Tuple<string, string>(errorCode, errorMessage)
				: null;
			return errorTuple;
		}

		/// <summary>
		/// xmlのなかから<gmoTransactionId>タグの値を取り出す
		/// </summary>
		/// <param name="xmlStr">xmlの文字列</param>
		/// <returns><gmoTransactionId>タグの値</returns>
		private static string GetGmoTransactionByXml(string xmlStr)
		{
			var match = Regex.Match(
				xmlStr.Replace(Environment.NewLine, string.Empty),
				@"<gmoTransactionId>(.*)</gmoTransactionId>",
				RegexOptions.Singleline);

			var gmoTransactionid = match.Groups[1].Value;
			return gmoTransactionid;
		}

		/// <summary>API設定</summary>
		private GmoDeferredApiSetting Settting { get; set; }
	}
}
