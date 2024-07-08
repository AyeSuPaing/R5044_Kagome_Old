/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) APIファサードクラス(TriLinkAfterPayApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System.Linq;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Helper;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Request;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Response;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay
{
	/// <summary>
	///  後付款(TriLink後払い) APIファサードクラス
	/// </summary>
	public static class TriLinkAfterPayApiFacade
	{
		/// <summary>
		/// 注文審査
		/// </summary>
		/// <param name="request">注文審査リクエスト</param>
		/// <returns>注文審査レスポンス</returns>
		public static TriLinkAfterPayAuthResponse Authorization(TriLinkAfterPayAuthRequest request)
		{
			var response = Post<TriLinkAfterPayAuthRequest, TriLinkAfterPayAuthResponse>(request);
			return response;
		}

		/// <summary>
		/// 注文確定依頼
		/// </summary>
		/// <param name="request">注文確定依頼リクエスト</param>
		/// <returns>注文確定依頼レスポンス</returns>
		public static TriLinkAfterPayCommitResponse Commit(TriLinkAfterPayCommitRequest request)
		{
			var response = Post<TriLinkAfterPayCommitRequest, TriLinkAfterPayCommitResponse>(request);
			return response;
		}

		/// <summary>
		/// 注文登録
		/// </summary>
		/// <param name="request">注文登録リクエスト</param>
		/// <returns>注文登録レスポンス</returns>
		public static TriLinkAfterPayRegisterResponse RegisterOrder(TriLinkAfterPayRegisterRequest request)
		{
			var response = Post<TriLinkAfterPayRegisterRequest, TriLinkAfterPayRegisterResponse>(request);
			return response;
		}

		/// <summary>
		/// 注文キャンセル
		/// </summary>
		/// <param name="request">注文キャンセルリクエスト</param>
		/// <returns>注文キャンセルルレスポンス</returns>
		public static TriLinkAfterPayCancelResponse CancelOrder(TriLinkAfterPayCancelRequest request)
		{
			var responseData = WebApiHelper.DeleteHttpRequest(request.RequestUrl);
			var response = JsonConvert.DeserializeObject<TriLinkAfterPayCancelResponse>(responseData);

			WriteTriLinkAfterPayErrorLog(response, PaymentFileLogger.PaymentProcessingType.OrderCancel);

			return response;
		}

		/// <summary>
		/// 注文情報更新
		/// </summary>
		/// <param name="request">注文情報更新リクエスト</param>
		/// <returns>注文情報更新レスポンス</returns>
		public static TriLinkAfterPayUpdateResponse UpdateOrder(TriLinkAfterPayUpdateRequest request)
		{
			var requestData = JsonConvert.SerializeObject(request);
			var responseData = WebApiHelper.PutHttpRequest(request.RequestUrl, requestData);
			var response = JsonConvert.DeserializeObject<TriLinkAfterPayUpdateResponse>(responseData);

			WriteTriLinkAfterPayErrorLog(response, PaymentFileLogger.PaymentProcessingType.OrderInfoUpdate);
			return response;
		}

		/// <summary>
		/// 出荷報告
		/// </summary>
		/// <param name="request">出荷報告リクエスト</param>
		/// <returns>出荷報告レスポンス</returns>
		public static TriLinkAfterPayShipmentCompleteResponse CompleteShipment(TriLinkAfterPayShipmentCompleteRequest request)
		{
			var response = Post<TriLinkAfterPayShipmentCompleteRequest, TriLinkAfterPayShipmentCompleteResponse>(request);
			return response;
		}

		/// <summary>
		/// アクセストークン
		/// </summary>
		/// <param name="request">アクセストークンリクエスト</param>
		/// <returns>アクセストークン</returns>
		public static string AccessToken(TriLinkAfterPayAccessTokenRequest request)
		{
			var response = Post<TriLinkAfterPayAccessTokenRequest, TriLinkAfterPayAccessTokenResponse>(request);
			var accessToken = ((response.ResponseResult)
				&& (string.IsNullOrEmpty(response.AccessToken) == false))
				? response.AccessToken
				: string.Empty;
			return accessToken;
		}

		/// <summary>
		/// Requestに対するResponseを返却する
		/// </summary>
		/// <typeparam name="T1">RequestBase</typeparam>
		/// <typeparam name="T2">ResponseBase</typeparam>
		/// <param name="request">リクエスト</param>
		/// <returns>レスポンス</returns>
		private static T2 Post<T1, T2>(T1 request)
			where T1 : RequestBase
			where T2 : ResponseBase
		{
			var requestData = JsonConvert.SerializeObject(request);
			var responseData = WebApiHelper.PostHttpRequest(request.RequestUrl, requestData, request.AddRequestHeaders);
			var response = JsonConvert.DeserializeObject<T2>(responseData);

			WriteTriLinkAfterPayErrorLog(response, PaymentFileLogger.PaymentProcessingType.Response);

			return response;
		}

		/// <summary>
		/// 後付款(TriLink後払い)ログ書き込み
		/// </summary>
		/// <param name="response">レスポンスデータ</param>
		/// <param name="processingContent">処理内容</param>
		private static void WriteTriLinkAfterPayErrorLog(ResponseBase response, PaymentFileLogger.PaymentProcessingType processingContent)
		{
			var errorMessage = CheckResponseErrorMessage(response);

			PaymentFileLogger.WritePaymentLog(
				null,
				Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY,
				PaymentFileLogger.PaymentType.TriLink,
				processingContent,
				(string.IsNullOrEmpty(errorMessage))
					? string.Format("[成功] {0}", response.Message)
					: errorMessage);
		}

		/// <summary>
		/// レスポンスの中のエラーメッセージをチェックする
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		public static string CheckResponseErrorMessage(ResponseBase response)
		{
			if (response.ResponseResult == false)
			{
				var errorCode = string.Empty;
				var errorMessage = string.Empty;
				if (response.IsHttpStatusCodeBadRequest)
				{
					errorCode = response.Errors.First().ErrorCode;
					errorMessage = response.Errors.First().Message;
				}
				else if ((response.IsHttpStatusCodeUnauthorized) || (response.IsHttpStatusCodeNotFound))
				{
					errorCode = response.ErrorCode;
					errorMessage = response.Message;
				}
				var errorResponseMessage = string.Empty;
				errorResponseMessage = string.Format("[失敗] {0} エラーコード[{1}] - {2}", response.NameForLog, errorCode, errorMessage);
				return errorResponseMessage;
			}

			return "";
		}
	}
}
