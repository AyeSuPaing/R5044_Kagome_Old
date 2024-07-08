/*
=========================================================================================================
  Module      : スコア後払いAPIのファサード(ScoreApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using w2.App.Common.Order.Payment.Score.Cancel;
using w2.App.Common.Order.Payment.Score.Delivery;
using w2.App.Common.Order.Payment.Score.GetAuth;
using w2.App.Common.Order.Payment.Score.GetInvoice;
using w2.App.Common.Order.Payment.Score.Order;
using w2.Common.Extensions;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.Score
{
	/// <summary>
	/// スコア後払いAPIのファサード
	/// </summary>
	public class ScoreApiFacade
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ScoreApiFacade()
		{
			var setting = new ScoreApiSetting(
				onAfterRequest: AfterRequestProc,
				onBeforeRequest: BeforeRequestProc,
				onRequestError: RequestErrorProc,
				onExtendWebRequest: ExtendWebRequestProc,
				apiEncoding: Encoding.UTF8,
				basicUserId: string.Empty,
				basicPassword: string.Empty,
				httpTimeOutMiriSecond: 30000,
				orderRegisterUrl: Constants.PAYMENT_SCORE_AFTER_PAY_URL_TRANSACTION,
				orderModifyUrl: Constants.PAYMENT_SCORE_AFTER_PAY_URL_MODIFY_TRANSACTION,
				orderCancelUrl: Constants.PAYMENT_SCORE_AFTER_PAY_URL_CANCEL,
				getAuthResultUrl: Constants.PAYMENT_SCORE_AFTER_PAY_URL_GET_AUTHOR,
				deliveryRegisterUrl: Constants.PAYMENT_SCORE_AFTER_PAY_URL_PD_REQUEST,
				getInvoiceUrl: Constants.PAYMENT_SCORE_AFTER_PAY_URL_GET_INVOICE);

			this.Settting = setting;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="setting">スコアAPI設定クラス</param>
		public ScoreApiFacade(ScoreApiSetting setting)
		{
			this.Settting = setting;
		}

		/// <summary>
		/// 取引登録
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public ScoreResponseOrderRegisterModify OrderRegister(ScoreRequestOrderRegisterModify request)
		{
			var response = this.CallApi<ScoreResponseOrderRegisterModify>(this.Settting.OrderRegisterUrl, request);
			return response;
		}

		/// <summary>
		/// 注文変更
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public ScoreResponseOrderRegisterModify OrderModify(ScoreRequestOrderRegisterModify request)
		{
			var response = this.CallApi<ScoreResponseOrderRegisterModify>(this.Settting.OrderModifyUrl, request);
			return response;
		}

		/// <summary>
		/// 発送情報登録
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public ScoreDeliveryRegisterResponse DeliveryRegister(ScoreDeliveryRegisterRequest request)
		{
			var response = this.CallApi<ScoreDeliveryRegisterResponse>(this.Settting.DeliveryRegisterUrl, request);
			return response;
		}

		/// <summary>
		/// 与信審査結果取得
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public ScoreGetAuthResultResponse GetAuthResult(ScoreGetAuthResultRequest request)
		{
			var response = this.CallApi<ScoreGetAuthResultResponse>(this.Settting.GetAuthResultUrl, request);
			return response;
		}

		/// <summary>
		/// 払込票印字データ取得
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public ScoreGetInvoiceResponse GetInvoiceResult(ScoreGetInvoiceRequest request)
		{
			var response = this.CallApi<ScoreGetInvoiceResponse>(this.Settting.GetInvoiceUrl, request);
			return response;
		}

		/// <summary>
		/// 注文変更
		/// </summary>
		/// <param name="request">リクエスト値</param>
		/// <returns>レスポンス値</returns>
		public ScoreCancelResponse OrderCancel(ScoreCancelRequest request)
		{
			var response = this.CallApi<ScoreCancelResponse>(this.Settting.OrderCancelUrl, request);
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
		private T CallApi<T>(string apiUrl, BaseScoreRequest requestData)
			where T : BaseScoreResponse
		{
			string response = string.Empty;
			using (var conn = new HttpApiConnector())
			{
				// 各種処理をセット
				conn.SetBeforeRequestProc(this.Settting.OnBeforeRequest);
				conn.SetAfterRequestProc(this.Settting.OnAfterRequest);
				conn.SetRequestErrorProc(this.Settting.OnRequestError);
				conn.SetExtendWebRequestProc(this.Settting.OnExtendWebRequest);

				response = conn.Do(apiUrl, this.Settting.ApiEncoding, requestData, this.Settting.BasicUserId, this.Settting.BasicPassword, true);
			}

			// Attribute属性をクリアする
			var xDoc = XDocument.Parse(response);
			foreach (var element in xDoc.Elements())
			{
				element.RemoveAttributes();
			}

			T rtn = SerializeHelper.Deserialize<T>(xDoc.ToString());

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
				PaymentFileLogger.PaymentType.Score,
				PaymentFileLogger.PaymentProcessingType.ApiRequestStart,
				LogCreator.CreateRequestMessage(
					apiRequestData.GetType().Name,
					apiRequestData.CreatePostString().RemoveWhiteSpaceChars(),
					$"{PaymentFileLogger.PaymentType.Score}後払い"
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
				PaymentFileLogger.PaymentType.Score,
				PaymentFileLogger.PaymentProcessingType.ApiRequestEnd,
					LogCreator.CreateRequestMessageWithResult(
						apiRequestData.GetType().Name,
						string.Empty,
						$"{PaymentFileLogger.PaymentType.Score}後払い",
						response.RemoveWhiteSpaceChars()
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
				PaymentFileLogger.PaymentType.Score,
				PaymentFileLogger.PaymentProcessingType.ApiError,
				LogCreator.CreateErrorMessage(errorTuple?.Item1, errorTuple?.Item2, string.Empty, url));
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

		/// <summary>API設定</summary>
		private ScoreApiSetting Settting { get; }
	}
}
