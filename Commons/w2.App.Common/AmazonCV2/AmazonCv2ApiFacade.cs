/*
=========================================================================================================
  Module      : AmazonCv2APIのファサード(AmazonCv2ApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.IO;
using System.Text;
using Amazon.Pay.API;
using Amazon.Pay.API.Types;
using Amazon.Pay.API.WebStore;
using Amazon.Pay.API.WebStore.Buyer;
using Amazon.Pay.API.WebStore.Charge;
using Amazon.Pay.API.WebStore.ChargePermission;
using Amazon.Pay.API.WebStore.CheckoutSession;
using Amazon.Pay.API.WebStore.Refund;
using Amazon.Pay.API.WebStore.Types;
using Newtonsoft.Json;
using w2.App.Common.Amazon.Util;
using w2.App.Common.Order.Payment;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;
using Environment = Amazon.Pay.API.Types.Environment;

namespace w2.App.Common.AmazonCv2
{
	/// <summary>
	/// AmazonAPIのファサード
	/// </summary>
	public sealed class AmazonCv2ApiFacade : IAmazonCv2ApiFacade
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AmazonCv2ApiFacade()
		{
			this.Client = CreateClient();
		}

		/// <summary>
		/// Amazonユーザ取得
		/// </summary>
		/// <param name="buyerToken">ユーザ取得トークン</param>
		/// <returns>バイヤーレスポンス</returns>
		public BuyerResponse GetBuyer(string buyerToken)
		{
			var response = this.Client.GetBuyer(buyerToken);

			WriteResponseLog(
				response,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2GetBuyer,
				new Dictionary<string, string>
				{
					{ "buyerToken", buyerToken },
					{ "Response", response.RawResponse }
				});
			return response;
		}

		/// <summary>
		/// チェックアウトセッション取得
		/// </summary>
		/// <param name="checkoutSessionId">チェックアウトセッションID</param>
		/// <returns>チェックアウトセッションレスポンス</returns>
		public CheckoutSessionResponse GetCheckoutSession(string checkoutSessionId)
		{
			var response = this.Client.GetCheckoutSession(checkoutSessionId);

			WriteResponseLog(
				response,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2GetCheckoutSession,
				new Dictionary<string, string>
				{
					{ "checkoutSessionId", checkoutSessionId },
					{ "Response", response.RawResponse }
				});
			return response;
		}

		/// <summary>
		/// チェックアウトセッション更新
		/// </summary>
		/// <param name="checkoutSessionId">チェックアウトセッションID</param>
		/// <param name="callBackUrl">コールバックURL</param>
		/// <param name="totalAmount">合計金額</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="withCapture">同時売上確定</param>
		/// <returns>チェックアウトセッションレスポンス</returns>
		public CheckoutSessionResponse UpdateCheckoutSession(
			string checkoutSessionId,
			string callBackUrl,
			decimal totalAmount,
			string orderId,
			bool withCapture = false)
		{
			var request = new UpdateCheckoutSessionRequest
			{
				WebCheckoutDetails =
				{
					CheckoutResultReturnUrl = CreateCallBackUrlWithProtocol(callBackUrl)
				},
				PaymentDetails =
				{
					ChargeAmount =
					{
						Amount = totalAmount,
						CurrencyCode = Currency.JPY
					},
					PaymentIntent = (Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW || withCapture)
						? PaymentIntent.AuthorizeWithCapture
						: PaymentIntent.Authorize
				},
				MerchantMetadata =
				{
					MerchantReferenceId = orderId,
					MerchantStoreName = Constants.PAYMENT_AMAZON_STORENAME,
					NoteToBuyer = string.Empty
				},
				PlatformId = Constants.PAYMENT_AMAZON_PLATFORMID
			};

			var result = this.Client.UpdateCheckoutSession(checkoutSessionId, request);

			WriteResponseLog(
				result,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2UpdateCheckoutSession,
				new Dictionary<string, string>
				{
					{ "checkoutSessionId", checkoutSessionId },
					{ "Request", request.ToJson() },
					{ "Response", result.RawResponse }
				});
			return result;
		}

		/// <summary>
		/// 定期用チェックアウトセッション更新
		/// </summary>
		/// <param name="checkoutSessionId">チェックアウトセッションID</param>
		/// <param name="callBackUrl">コールバックURL</param>
		/// <param name="totalAmount">合計金額</param>
		/// <param name="fixedPurchaseId">定期ID</param>
		/// <param name="fixedPurchaseKbn">定期購入区分</param>
		/// <param name="fixedPurchaseSetting">定期購入設定</param>
		/// <returns>チェックアウトセッションレスポンス</returns>
		public CheckoutSessionResponse UpdateCheckoutSessionForFixedPurchase(
			string checkoutSessionId,
			string callBackUrl,
			decimal totalAmount,
			string fixedPurchaseId,
			string fixedPurchaseKbn,
			string fixedPurchaseSetting)
		{
			var freqUnit = FrequencyUnit.Variable;
			var freqValue = 0;

			// 配送周期をAmazonAPi用に変換
			switch (fixedPurchaseKbn)
			{
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					freqUnit = FrequencyUnit.Month;
					int.TryParse(fixedPurchaseSetting.Split(',')[0], out freqValue);
					break;

				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					freqUnit = FrequencyUnit.Month;
					int.TryParse(fixedPurchaseSetting.Split(',')[0], out freqValue);
					break;

				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
					freqUnit = FrequencyUnit.Day;
					int.TryParse(fixedPurchaseSetting, out freqValue);
					break;
			}

			var request = new UpdateCheckoutSessionRequest
			{
				WebCheckoutDetails =
				{
					CheckoutResultReturnUrl = CreateCallBackUrlWithProtocol(callBackUrl)
				},
				PaymentDetails =
				{
					ChargeAmount =
					{
						Amount = totalAmount,
						CurrencyCode = Currency.JPY
					},
					PaymentIntent = PaymentIntent.Confirm
				},
				MerchantMetadata =
				{
					MerchantReferenceId = fixedPurchaseId,
					MerchantStoreName = Constants.PAYMENT_AMAZON_STORENAME,
					NoteToBuyer = string.Empty
				},
				RecurringMetadata =
				{
					Frequency =
					{
						Unit = freqUnit,
						Value = freqValue
					}
				},
				PlatformId = Constants.PAYMENT_AMAZON_PLATFORMID
			};

			var result = this.Client.UpdateCheckoutSession(checkoutSessionId, request);

			WriteResponseLog(
				result,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2UpdateCheckoutSessionForFixedPurchase,
				new Dictionary<string, string>
				{
					{ "checkoutSessionId", checkoutSessionId },
					{ "Request", request.ToJson() },
					{ "Response", result.RawResponse }
				});
			return result;
		}

		/// <summary>
		/// チェックアウトセッション完了
		/// </summary>
		/// <param name="checkoutSessionId">チェックアウトセッションID</param>
		/// <param name="totalAmount">合計金額</param>
		/// <returns>チェックアウトセッションレスポンス</returns>
		public CheckoutSessionResponse CompleteCheckoutSession(string checkoutSessionId, decimal totalAmount)
		{
			var request = new CompleteCheckoutSessionRequest(totalAmount, Currency.JPY);

			var result = this.Client.CompleteCheckoutSession(checkoutSessionId, request);

			WriteResponseLog(
				result,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2CompleteCheckoutSession,
				new Dictionary<string, string>
				{
					{ "checkoutSessionId", checkoutSessionId },
					{ "Request", request.ToJson() },
					{ "Response", result.RawResponse }
				});
			return result;
		}

		/// <summary>
		/// チャージパーミッション注文ID更新
		/// </summary>
		/// <param name="chargePermissionId">チャージパーミッション</param>
		/// <param name="newOrderId">注文ID</param>
		/// <returns>チャージパーミッションレスポンス</returns>
		public ChargePermissionResponse UpdateChargePermissionOrderId(string chargePermissionId, string newOrderId)
		{
			var request = new UpdateChargePermissionRequest
			{
				MerchantMetadata =
				{
					MerchantReferenceId = newOrderId
				}
			};

			var result = this.Client.UpdateChargePermission(chargePermissionId, request);

			WriteResponseLog(
				result,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2UpdateChargePermissionOrderId,
				new Dictionary<string, string>
				{
					{ "chargePermissionId", chargePermissionId },
					{ "Request", request.ToJson() },
					{ "Response", result.RawResponse }
				});
			return result;
		}

		/// <summary>
		/// 注文取消
		/// </summary>
		/// <param name="chargePermissionId">チャージパーミッションID</param>
		/// <param name="closureReason">取消理由</param>
		/// <returns>チャージパーミッションレスポンス</returns>
		public ChargePermissionResponse CloseChargePermission(string chargePermissionId, string closureReason = null)
		{
			var validatedClosureReason = StringUtility.ToEmpty(closureReason);
			var request = new CloseChargePermissionRequest(validatedClosureReason)
			{
				CancelPendingCharges = true
			};

			var result = this.Client.CloseChargePermission(chargePermissionId, request);

			WriteResponseLog(
				result,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2CloseChargePermission,
				new Dictionary<string, string>
				{
					{ "chargePermissionId", chargePermissionId },
					{ "Request", request.ToJson() },
					{ "Response", result.RawResponse }
				});
			return result;
		}

		/// <summary>
		/// チャージ生成
		/// </summary>
		/// <param name="chargePermissionId">チャージパーミッションID</param>
		/// <param name="amount">金額</param>
		/// <param name="orderId">注文ID</param>
		/// <param name="captureNow">即時売確するか</param>
		/// <returns>チャージレスポンス</returns>
		public ChargeResponse CreateCharge(string chargePermissionId, decimal amount, string orderId = null, bool captureNow = false)
		{
			var request = new CreateChargeRequest(chargePermissionId, amount, Currency.JPY)
			{
				CaptureNow = (Constants.PAYMENT_AMAZON_PAYMENTCAPTURENOW || captureNow)
			};
			if (string.IsNullOrEmpty(orderId) == false)
			{
				request.MerchantMetadata.MerchantReferenceId = orderId;
			}
			var result = this.Client.CreateCharge(request);

			WriteResponseLog(
				result,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2CreateCharge,
				new Dictionary<string, string>
				{
					{ "Request", request.ToJson() },
					{ "Response", result.RawResponse }
				});
			return result;
		}


		/// <summary>
		/// チャージ取得
		/// </summary>
		/// <param name="chargeId">チャージID</param>
		/// <returns>チャージレスポンス</returns>
		public ChargeResponse GetCharge(string chargeId)
		{
			var result = this.Client.GetCharge(ConvertCv1IdToCv2Id(chargeId));

			WriteResponseLog(
				result,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2GetCharge,
				new Dictionary<string, string>
				{
					{ "chargeId", ConvertCv1IdToCv2Id(chargeId) },
					{ "Response", result.RawResponse }
				});
			return result;
		}

		/// <summary>
		/// 売上確定
		/// </summary>
		/// <param name="chargeId">チャージID</param>
		/// <param name="totalAmount">合計金額</param>
		/// <returns>チャージレスポンス</returns>
		public ChargeResponse CaptureCharge(string chargeId, decimal totalAmount)
		{
			var request = new CaptureChargeRequest(totalAmount, Currency.JPY);

			var result = this.Client.CaptureCharge(ConvertCv1IdToCv2Id(chargeId), request);

			WriteResponseLog(
				result,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2CaptureCharge,
				new Dictionary<string, string>
				{
					{ "chargeId", ConvertCv1IdToCv2Id(chargeId) },
					{ "Request", request.ToJson() },
					{ "Response", result.RawResponse }
				});
			return result;
		}

		/// <summary>
		/// チャージキャンセル（注文取消）
		/// </summary>
		/// <param name="chargeId">チャージID</param>
		/// <param name="cancelReason">キャンセル理由</param>
		/// <returns>チャージレスポンス</returns>
		public ChargeResponse CancelCharge(string chargeId, string cancelReason = null)
		{
			var request = new CancelChargeRequest(StringUtility.ToEmpty(cancelReason));

			var result = this.Client.CancelCharge(ConvertCv1IdToCv2Id(chargeId), request);

			WriteResponseLog(
				result,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2CancelCharge,
				new Dictionary<string, string>
				{
					{ "chargeId", ConvertCv1IdToCv2Id(chargeId) },
					{ "Request", request.ToJson() },
					{ "Response", result.RawResponse }
				});
			return result;
		}

		/// <summary>
		/// 返金
		/// </summary>
		/// <param name="chargeId">チャージID</param>
		/// <param name="refundAmount">返金額</param>
		/// <returns>リファンドレスポンス</returns>
		public RefundResponse CreateRefund(string chargeId, decimal refundAmount)
		{
			var request = new CreateRefundRequest(ConvertCv1IdToCv2Id(chargeId), refundAmount, Currency.JPY);

			var result = this.Client.CreateRefund(request);

			WriteResponseLog(
				result,
				PaymentFileLogger.PaymentProcessingType.AmazonCv2CreateRefund,
				new Dictionary<string, string>
				{
					{ "Request", request.ToJson() },
					{ "Response", result.RawResponse }
				});
			return result;
		}

		/// <summary>
		/// クライアント生成
		/// </summary>
		/// <returns>クライアント</returns>
		public static WebStoreClient CreateClient()
		{
			try
			{
				var payConfiguration = new ApiConfiguration(
					region: Region.Japan,
					environment: Constants.PAYMENT_AMAZON_ISSANDBOX ? Environment.Sandbox : Environment.Live,
					publicKeyId: Constants.PAYMENT_AMAZON_PUBLIC_KEY_ID,
					privateKey: Constants.PAYMENT_AMAZON_PRIVATE_KEY);

				var client = new WebStoreClient(payConfiguration);

				return client;
			}
			catch (FileNotFoundException)
			{
				FileLogger.WriteError("提供されたAmazonPayCV2秘密鍵ファイルが見つかりませんでした。");
			}

			return null;
		}

		/// <summary>
		/// レスポンスエラー抽出
		/// </summary>
		/// <param name="response">アマゾンレスポンス</param>
		/// <returns>エラー情報</returns>
		public static AmazonResponseError GetErrorCodeAndMessage(AmazonPayResponse response)
		{
			var result = JsonConvert.DeserializeObject<AmazonResponseError>(response.RawResponse);
			return result;
		}

		/// <summary>
		/// プロトコル付コールバックURL生成
		/// </summary>
		/// <param name="callBackPath">パス</param>
		/// <returns>コールバックURL</returns>
		public static string CreateCallBackUrlWithProtocol(string callBackPath)
		{
			var result = new StringBuilder(Constants.PROTOCOL_HTTPS)
				.Append(Constants.SITE_DOMAIN)
				.Append(AmazonUtil.CreateCallbackUrl(callBackPath))
				.ToString();
			return result;
		}

		/// <summary>
		/// コールバックパス生成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="orderId">注文ID</param>
		/// <returns>遷移先パス</returns>
		public static string CreateCallbackPathForOneTime(
			string actionStatus,
			string orderId)
		{
			var callbackPath = new UrlCreator(Constants.PAGE_FRONT_ORDER_HISTORY_AMAZONPAY_CV2_CHANGE_ACTION_REDIRECT)
			.AddParam(
				AmazonCv2Constants.REQUEST_KEY_AMAZON_ACTION_STATUS,
				actionStatus)
			.AddParam(
				AmazonCv2Constants.REQUEST_KEY_ORDER_ID,
				orderId)
			.CreateUrl();

			return callbackPath;
		}

		/// <summary>
		/// コールバックパス生成
		/// </summary>
		/// <param name="actionStatus">アクションステータス</param>
		/// <param name="orderId">定期ID</param>
		/// <returns></returns>
		public static string CreateCallbackPathForFixedPurchase(
			string actionStatus,
			string orderId)
		{
			var callbackPath = new UrlCreator(Constants.PAGE_FRONT_FIXED_PURCHASE_DETAIL)
			.AddParam(
				AmazonCv2Constants.REQUEST_KEY_AMAZON_ACTION_STATUS,
				actionStatus)
			.AddParam(AmazonCv2Constants.REQUEST_KEY_FIXED_PURCHASE_ID,
				orderId)
			.CreateUrl();

			return callbackPath;
		}

		/// <summary>
		/// CV1のAmazonAuthorizationIdをCV2のChargeIdに変換する。
		/// </summary>
		/// <param name="authId">CV1用AmazonAuthorizationId</param>
		/// <returns>CV2用ChargeId（引数がAmazonAuthorizationId出ない場合は、引数をそのまま返す）</returns>
		private static string ConvertCv1IdToCv2Id(string authId)
		{
			//　21番目のAをCに置き換える
			if (string.IsNullOrEmpty(authId) || authId.IndexOf("A") != 20) return authId;
			var result = string.Format(
				"{0}C{1}",
				authId.Substring(0, 20),
				authId.Substring(21));
			return result;
		}

		/// <summary>
		/// レスポンスログ出力
		/// </summary>
		/// <param name="response">レスポンス</param>
		/// <param name="transactionName">処理名</param>
		/// <param name="dictionary">ディクショナリ</param>
		/// <remarks>
		/// リクエストとレスポンスはdictionaryで出力。例：｛"Request",リクエスト値｝、{"Response",レスポンス値}<br/>
		/// レスポンス処理用のリクエスト値が複数ある場合、それぞれのリクエスト値を出力。
		/// </remarks>
		private static void WriteResponseLog(
			AmazonPayResponse response,
			PaymentFileLogger.PaymentProcessingType transactionName,
			Dictionary<string, string> dictionary = null)
		{
			if (Constants.PAYMENT_AMAZON_CV2_WRITE_API_RESPONSE_LOG)
			{
				var error = GetErrorCodeAndMessage(response);
				// ログ格納処理
				PaymentFileLogger.WritePaymentLog(
					null,
					Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2,
					PaymentFileLogger.PaymentType.AmazonCv2,
					transactionName,
					LogCreator.CreateErrorMessage(error.ReasonCode, error.Message),
					dictionary);
			}
		}

		/// <summary>接続クライアント</summary>
		private WebStoreClient Client { get; set; }
	}

	[JsonObject]
	public class AmazonResponseError
	{
		/// <summary>理由コード</summary>
		[JsonProperty("reasonCode")]
		public string ReasonCode { get; set; }
		/// <summary>エラーメッセージ</summary>
		[JsonProperty("message")]
		public string Message { get; set; }
	}
}
