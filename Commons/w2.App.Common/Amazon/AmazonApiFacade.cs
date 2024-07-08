/*
=========================================================================================================
  Module      : AmazonAPIのファサード(AmazonApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using AmazonPay;
using AmazonPay.CommonRequests;
using AmazonPay.RecurringPaymentRequests;
using AmazonPay.Responses;
using AmazonPay.StandardPaymentRequests;
using Newtonsoft.Json;
using w2.App.Common.Amazon.Responses;
using w2.App.Common.Order.Payment;

namespace w2.App.Common.Amazon
{
	/// <summary>
	/// AmazonAPIのファサード
	/// </summary>
	public static class AmazonApiFacade
	{
		/// <summary>
		/// ユーザー情報取得
		/// </summary>
		/// <param name="token">トークン</param>
		/// <returns>ユーザー情報</returns>
		public static GetUserInfoResponse GetUserInfo(string token)
		{
			var client = CreateClient();
			var v = client.GetUserInfo(token);
			var response = JsonConvert.DeserializeObject<GetUserInfoResponse>(v);
			return response;
		}

		/// <summary>
		/// [ワンタイム] Orderリファレンス取得
		/// </summary>
		/// <param name="orderRefId">OrderリファレンスID</param>
		/// <param name="token">トークン</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201751970"/>
		/// <returns>Orderリファレンス</returns>
		public static OrderReferenceDetailsResponse GetOrderReferenceDetails(string orderRefId, string token)
		{
			var client = CreateClient();

			var request = new GetOrderReferenceDetailsRequest();
			request.WithAmazonOrderReferenceId(orderRefId)
				.WithAccessToken(token);

			var response = client.GetOrderReferenceDetails(request);

			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "orderRefId", orderRefId },
					{ "token", token }
				});
			return response;
		}

		/// <summary>
		/// [ワンタイム] 注文情報の設定
		/// </summary>
		/// <param name="orderRefId">OrderリファレンスID</param>
		/// <param name="amount">注文金額</param>
		/// <param name="orderId">注文ID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201751960"/>
		/// <returns>Orderリファレンス</returns>
		public static OrderReferenceDetailsResponse SetOrderReferenceDetails(string orderRefId, decimal amount, string orderId)
		{
			var client = CreateClient();

			var request = new SetOrderReferenceDetailsRequest();
			request.WithAmazonOrderReferenceId(orderRefId)
				.WithAmount(amount)
				.WithCurrencyCode(Regions.currencyCode.JPY)
				.WithSellerOrderId(orderId)
				.WithStoreName(Constants.PAYMENT_AMAZON_STORENAME)
				.WithPlatformId(Constants.PAYMENT_AMAZON_PLATFORMID);

			var response = client.SetOrderReferenceDetails(request);

			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, orderId },
					{ "orderRefId", orderRefId }
				});
			return response;
		}

		/// <summary>
		/// [ワンタイム] 注文情報の承認
		/// </summary>
		/// <param name="orderRefId">OrderリファレンスID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201751980"/>
		/// <returns>確定したOrderリファレンス</returns>
		public static ConfirmOrderReferenceResponse ConfirmOrderReference(string orderRefId)
		{
			var client = CreateClient();

			var request = new ConfirmOrderReferenceRequest();
			request.WithAmazonOrderReferenceId(orderRefId);

			var response = client.ConfirmOrderReference(request);

			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "orderRefId", orderRefId }
				});
			return response;
		}

		/// <summary>
		/// [ワンタイム] Orderリファレンス 取消し
		/// </summary>
		/// <param name="orderRefId">OrderリファレンスID</param>
		/// <param name="cancelationReason">キャンセル理由</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201751990"/>
		/// <returns>取消ししたOrderリファレンス</returns>
		public static CancelOrderReferenceResponse CancelOrderReference(string orderRefId, string cancelationReason)
		{
			var client = CreateClient();

			var request = new CancelOrderReferenceRequest();
			request.WithAmazonOrderReferenceId(orderRefId)
				.WithCancelationReason(cancelationReason);

			var response = client.CancelOrderReference(request);

			var idKeyAndValueDictionary = new Dictionary<string, string>
			{
				{"orderRefId", orderRefId},
				{"cancelationReason", cancelationReason}
			};
			WriteResponseLog(response, idKeyAndValueDictionary);
			return response;
		}

		/// <summary>
		/// [ワンタイム] 支払金額のオーソリ
		/// </summary>
		/// <param name="orderRefId">OrderリファレンスID</param>
		/// <param name="amount">注文金額</param>
		/// <param name="tranId">オーソリを一意にするためのトランザクションID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201752010"/>
		/// <returns>オーソリ結果</returns>
		public static AuthorizeResponse Authorize(string orderRefId, decimal amount, string tranId, bool captureNow)
		{
			var client = CreateClient();

			var request = new AuthorizeRequest();
			request.WithAmazonOrderReferenceId(orderRefId)
				.WithCaptureNow(captureNow)
				.WithAmount(amount)
				.WithCurrencyCode(Regions.currencyCode.JPY)
				.WithTransactionTimeout(0)
				.WithAuthorizationReferenceId(tranId);

			var response = client.Authorize(request);


			var idKeyAndValueDictionary = new Dictionary<string, string>
			{
				{"orderRefId", orderRefId},
				{"tranId", tranId}
			};
			WriteResponseLog(response, idKeyAndValueDictionary);
			return response;
		}

		/// <summary>
		/// [ワンタイム] オーソリオブジェクト取得
		/// </summary>
		/// <param name="amazonAuthorizationId">AmazonオーソリID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201752030"/>
		/// <returns>取得結果</returns>
		public static AuthorizeResponse GetAuthorizationDetails(string amazonAuthorizationId)
		{
			var client = CreateClient();

			var request = new GetAuthorizationDetailsRequest();
			request.WithAmazonAuthorizationId(amazonAuthorizationId);

			var response = client.GetAuthorizationDetails(request);

			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "amazonAuthorizationId", amazonAuthorizationId }
				});
			return response;
		}

		/// <summary>
		/// [ワンタイム] 売上請求オブジェクト取得
		/// </summary>
		/// <param name="amazonCaptureId">Amazon売上請求ID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201752060"/>
		/// <returns>取得結果</returns>
		public static CaptureResponse GetCaptureDetails(string amazonCaptureId)
		{
			var client = CreateClient();

			var request = new GetCaptureDetailsRequest();
			request.WithAmazonCaptureId(amazonCaptureId);

			var response = client.GetCaptureDetails(request);

			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "amazonCaptureId", amazonCaptureId }
				});
			return response;
		}

		/// <summary>
		/// [ワンタイム] 返金オブジェクト取得
		/// </summary>
		/// <param name="amazonCaptureId">Amazon売上請求ID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201752100"/>
		/// <returns>取得結果</returns>
		public static RefundResponse GetRefundDetails(string amazonRefundId)
		{
			var client = CreateClient();

			var request = new GetRefundDetailsRequest();
			request.WithAmazonRefundId(amazonRefundId);

			var response = client.GetRefundDetails(request);

			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "amazonRefundId", amazonRefundId }
				});
			return response;
		}

		/// <summary>
		/// [ワンタイム] 支払金額のオーソリClose
		/// </summary>
		/// <param name="amazonAuthorizationId">AmazonオーソリID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201752070"/>
		/// <returns>オーソリ結果</returns>
		public static CloseAuthorizationResponse CloseAuthorization(string amazonAuthorizationId)
		{
			var client = CreateClient();

			var request = new CloseAuthorizationRequest();
			request.WithAmazonAuthorizationId(amazonAuthorizationId);

			var response = client.CloseAuthorization(request);
			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "amazonAuthorizationId", amazonAuthorizationId }
				});
			return response;
		}

		/// <summary>
		/// [ワンタイム] 支払金額の請求
		/// </summary>
		/// <param name="amazonAuthorizationId">AmazonオーソリID</param>
		/// <param name="amount">請求金額</param>
		/// <param name="tranId">売上請求を一意にするトランザクションID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201752040"/>
		/// <returns>売上請求結果</returns>
		public static CaptureResponse Capture(string amazonAuthorizationId, decimal amount, string tranId)
		{
			var client = CreateClient();

			var request = new CaptureRequest();
			request.WithAmazonAuthorizationId(amazonAuthorizationId)
				.WithCaptureReferenceId(tranId)
				.WithAmount(amount)
				.WithCurrencyCode(Regions.currencyCode.JPY);

			var response = client.Capture(request);

			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "amazonAuthorizationId", amazonAuthorizationId }
				});
			return response;
		}

		/// <summary>
		/// [ワンタイム] 支払金額の返金
		/// </summary>
		/// <param name="amazonCaptureId">Amazon売上請求ID</param>
		/// <param name="amount">返金金額</param>
		/// <param name="tranId">返金を一意にするトランザクションID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201752080"/>
		/// <returns>返金結果</returns>
		public static RefundResponse Refund(string amazonCaptureId, decimal amount, string tranId)
		{
			var client = CreateClient();

			var request = new RefundRequest();
			request.WithAmazonCaptureId(amazonCaptureId)
				.WithRefundReferenceId(tranId)
				.WithAmount(amount)
				.WithCurrencyCode(Regions.currencyCode.JPY);

			var response = client.Refund(request);

			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "amazonCaptureId", amazonCaptureId },
					{ "tranId", tranId }
				});
			return response;
		}

		/// <summary>
		/// [定期] 支払契約情報取得
		/// </summary>
		/// <param name="amazonBillingAgreementId">Amazon支払契約ID</param>
		/// <param name="token">トークン</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201751690"/>
		/// <returns>Orderリファレンス</returns>
		public static BillingAgreementDetailsResponse GetBillingAgreementDetails(string amazonBillingAgreementId, string token)
		{
			var client = CreateClient();

			var request = new GetBillingAgreementDetailsRequest();
			request.WithAmazonBillingAgreementId(amazonBillingAgreementId)
				.WithaddressConsentToken(token);

			var response = client.GetBillingAgreementDetails(request);

			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "amazonBillingAgreementId", amazonBillingAgreementId },
					{ "token", token }
				});
			return response;
		}

		/// <summary>
		/// [定期] お支払い方法設定
		/// </summary>
		/// <param name="amazonBillingAgreementId">Amazon支払契約ID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201751700"/>
		/// <returns></returns>
		public static BillingAgreementDetailsResponse SetBillingAgreementDetails(string amazonBillingAgreementId)
		{
			var client = CreateClient();

			var request = new SetBillingAgreementDetailsRequest();
			request.WithAmazonBillingAgreementId(amazonBillingAgreementId)
				.WithStoreName(Constants.PAYMENT_AMAZON_STORENAME)
				.WithPlatformId(Constants.PAYMENT_AMAZON_PLATFORMID);

			var response = client.SetBillingAgreementDetails(request);
			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "amazonBillingAgreementId", amazonBillingAgreementId }
				});
			return response;
		}

		/// <summary>
		/// [定期] お支払い方法設定の承認
		/// </summary>
		/// <param name="amazonBillingAgreementId">支払契約ID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201751710"/>
		/// <returns>支払契約承認結果</returns>
		public static ConfirmBillingAgreementResponse ConfirmBillingAgreement(string amazonBillingAgreementId)
		{
			var client = CreateClient();

			var request = new ConfirmBillingAgreementRequest();
			request.WithAmazonBillingreementId(amazonBillingAgreementId);

			var response = client.ConfirmBillingAgreement(request);

			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "amazonBillingAgreementId", amazonBillingAgreementId }
				});
			return response;
		}

		/// <summary>
		/// [定期] 支払い契約終了
		/// </summary>
		/// <param name="amazonBillingAgreementId">支払契約ID</param>
		/// <param name="closureReason">理由</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201751950"/>
		/// <returns>注文生成結果</returns>
		public static CloseBillingAgreementResponse CloseBillingAgreement(string amazonBillingAgreementId, string closureReason)
		{
			var client = CreateClient();

			var request = new CloseBillingAgreementRequest();
			request.WithAmazonBillingAgreementId(amazonBillingAgreementId)
				.WithClosureReason(closureReason);

			var response = client.CloseBillingAgreement(request);
			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "amazonBillingAgreementId", amazonBillingAgreementId },
					{ "closureReason", closureReason }
				});
			return response;
		}

		/// <summary>
		/// [定期] 注文生成
		/// </summary>
		/// <param name="amazonBillingAgreementId">支払契約ID</param>
		/// <param name="amount">注文金額</param>
		/// <param name="orderId">注文ID</param>
		/// <see cref="https://pay.amazon.com/jp/developer/documentation/apireference/201751670"/>
		/// <returns>注文生成結果</returns>
		public static OrderReferenceDetailsResponse CreateOrderReferenceForId(string amazonBillingAgreementId, decimal amount, string orderId)
		{
			var client = CreateClient();

			var request = new CreateOrderReferenceForIdRequest();
			request.WithId(amazonBillingAgreementId)
				.WithIdType("BillingAgreement")
				.WithAmount(amount)
				.WithSellerOrderId(orderId)
				.WithPlatformId(Constants.PAYMENT_AMAZON_PLATFORMID);

			request.WithStoreName(Constants.PAYMENT_AMAZON_STORENAME);

			var response = client.CreateOrderReferenceForId(request);
			WriteResponseLog(
				response,
				new Dictionary<string, string>
				{
					{ "amazonBillingAgreementId", amazonBillingAgreementId },
					{ Constants.FIELD_ORDER_ORDER_ID, orderId }
				});
			return response;
		}

		/// <summary>
		/// AmazonClient生成
		/// </summary>
		/// <returns>生成したClient</returns>
		private static Client CreateClient()
		{
			var conf = CreateAmazonConfiguration();
			return new Client(conf);
		}

		/// <summary>
		/// Amazon設定生成
		/// </summary>
		/// <returns>Amazon設定</returns>
		private static Configuration CreateAmazonConfiguration()
		{
			var clientConfig = new Configuration();
			clientConfig.WithMerchantId(Constants.PAYMENT_AMAZON_SELLERID)
				.WithAccessKey(Constants.PAYMENT_AMAZON_AWSACCESSKEY)
				.WithSecretKey(Constants.PAYMENT_AMAZON_AWSSECRET)
				.WithCurrencyCode(Regions.currencyCode.JPY)
				.WithClientId(Constants.PAYMENT_AMAZON_CLIENTID)
				.WithRegion(Regions.supportedRegions.jp)
				.WithSandbox(Constants.PAYMENT_AMAZON_ISSANDBOX)
				.WithPlatformId("")
				.WithCABundleFile("")
				.WithApplicationName("")
				.WithApplicationVersion("")
				.WithProxyHost("")
				.WithProxyPort(-1)
				.WithProxyUserName("")
				.WithProxyUserPassword("")
				.WithAutoRetryOnThrottle(true);
			return clientConfig;
		}

		/// <summary>
		/// レスポンスログ出力
		/// </summary>
		/// <param name="response">レスポンス</param>
		/// <param name="dictionary">ディクショナリ</param>
		private static void WriteResponseLog(AbstractResponse response, Dictionary<string, string> dictionary = null)
		{
			if (Constants.PAYMENT_AMAZON_WRITE_API_RESPONSE_LOG)
			{
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					null,
					Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT,
					PaymentFileLogger.PaymentType.Amazon,
					PaymentFileLogger.PaymentProcessingType.ResponseOutput,
					LogCreator.CreateErrorMessage(response.GetErrorCode(), response.GetErrorMessage()),
					dictionary);
			}
		}
	}
}
