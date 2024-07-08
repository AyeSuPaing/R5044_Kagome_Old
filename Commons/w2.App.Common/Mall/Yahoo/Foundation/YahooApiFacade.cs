/*
=========================================================================================================
  Module      : YAHOO API Yahoo APIモジュール クラス(YahooApiFacade.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Text;
using System.Xml.Serialization;
using w2.App.Common.Mall.Yahoo.Dto;
using w2.App.Common.Mall.Yahoo.Interfaces;
using w2.App.Common.Mall.Yahoo.Procedures;
using w2.App.Common.Mall.Yahoo.YahooMallOrders;
using System.IO;
using w2.Common.Logger;
using w2.Common.Web;

namespace w2.App.Common.Mall.Yahoo.Foundation
{
	/// <summary>
	/// Yahoo APIモジュール
	/// </summary>
	public class YahooApiFacade : IYahooApiFacade
	{
		#region フィールド群
		/// <summary>フィールド群</summary>
		private static readonly string[] s_orderFieldsToRequest =
		{
			"OrderId",
			"Version",
			"ParentOrderId",
			"ChildOrderId",
			"DeviceType",
			"MobileCarrierName",
			"IsActive",
			"IsSeen",
			"IsSplit",
			"CancelReason",
			"CancelReasonDetail",
			"IsRoyalty",
			"IsRoyaltyFix",
			"IsSeller",
			"IsAffiliate",
			"IsRatingB2s",
			"NeedSnl",
			"OrderTime",
			"LastUpdateTime",
			"Suspect",
			"SuspectMessage",
			"OrderStatus",
			"StoreStatus",
			"RoyaltyFixTime",
			"SendConfirmTime",
			"SendPayTime",
			"PrintSlipTime",
			"PrintDeliveryTime",
			"PrintBillTime",
			"BuyerComments",
			"SellerComments",
			"Notes",
			"OperationUser",
			"Referer",
			"EntryPoint",
			"Clink",
			"HistoryId",
			"UsageId",
			"UseCouponData",
			"TotalCouponDiscount",
			"ShippingCouponFlg",
			"ShippingCouponDiscount",
			"CampaignPoints",
			"IsMultiShip",
			"MultiShipId",
			"IsReadOnly",
			"IsFirstClassDrugIncludes",
			"IsFirstClassDrugAgreement",
			"IsWelcomeGiftIncludes",
			"YamatoCoopStatus",
			"FraudHoldStatus",
			"PublicationTime",
			"IsYahooAuctionOrder",
			"YahooAuctionMerchantId",
			"YahooAuctionId",
			"IsYahooAuctionDeferred",
			"YahooAuctionCategoryType",
			"YahooAuctionBidType",
			"YahooAuctionBundleType",
			"GoodStoreStatus",
			"CurrentGoodStoreBenefitApply",
			"CurrentPromoPkgApply",
			"LineGiftOrderId",
			"IsLineGiftOrder",
			"PayStatus",
			"SettleStatus",
			"PayType",
			"PayKind",
			"PayMethod",
			"PayMethodName",
			"SellerHandlingCharge",
			"PayActionTime",
			"PayDate",
			"PayNotes",
			"SettleId",
			"CardBrand",
			"CardNumber",
			"CardNumberLast4",
			"CardExpireYear",
			"CardExpireMonth",
			"CardPayType",
			"CardHolderName",
			"CardPayCount",
			"CardBirthDay",
			"UseYahooCard",
			"UseWallet",
			"NeedBillSlip",
			"NeedDetailedSlip",
			"NeedReceipt",
			"AgeConfirmField",
			"AgeConfirmValue",
			"AgeConfirmCheck",
			"BillAddressFrom",
			"BillFirstName",
			"BillFirstNameKana",
			"BillLastName",
			"BillLastNameKana",
			"BillZipCode",
			"BillPrefecture",
			"BillPrefectureKana",
			"BillCity",
			"BillCityKana",
			"BillAddress1",
			"BillAddress1Kana",
			"BillAddress2",
			"BillAddress2Kana",
			"BillPhoneNumber",
			"BillEmgPhoneNumber",
			"BillMailAddress",
			"BillSection1Field",
			"BillSection1Value",
			"BillSection2Field",
			"BillSection2Value",
			"PayNo",
			"PayNoIssueDate",
			"ConfirmNumber",
			"PaymentTerm",
			"IsApplePay",
			"LineGiftPayMethodName",
			"CombinedPayType",
			"CombinedPayKind",
			"CombinedPayMethod",
			"CombinedPayMethodName",
			"ShipStatus",
			"ShipMethod",
			"ShipMethodName",
			"ShipRequestDate",
			"ShipRequestTime",
			"ShipNotes",
			"ShipCompanyCode",
			"ReceiveShopCode",
			"ShipInvoiceNumber1",
			"ShipInvoiceNumber2",
			"ShipInvoiceNumberEmptyReason",
			"ShipUrl",
			"ArriveType",
			"ShipDate",
			"ArrivalDate",
			"IsCashOnDelivery",
			"NeedGiftWrap",
			"GiftWrapCode",
			"GiftWrapType",
			"GiftWrapMessage",
			"NeedGiftWrapPaper",
			"GiftWrapPaperType",
			"GiftWrapName",
			"Option1Field",
			"Option1Type",
			"Option1Value",
			"Option2Field",
			"Option2Type",
			"Option2Value",
			"ShipFirstName",
			"ShipFirstNameKana",
			"ShipLastName",
			"ShipLastNameKana",
			"ShipZipCode",
			"ShipPrefecture",
			"ShipPrefectureKana",
			"ShipCity",
			"ShipCityKana",
			"ShipAddress1",
			"ShipAddress1Kana",
			"ShipAddress2",
			"ShipAddress2Kana",
			"ShipPhoneNumber",
			"ShipEmgPhoneNumber",
			"ShipSection1Field",
			"ShipSection1Value",
			"ShipSection2Field",
			"ShipSection2Value",
			"ReceiveSatelliteType",
			"ReceiveSatelliteSettleMethod",
			"ReceiveSatelliteMethod",
			"ReceiveSatelliteCompanyName",
			"ReceiveSatelliteShopCode",
			"ReceiveSatelliteShopName",
			"ReceiveSatelliteShipKind",
			"ReceiveSatelliteYahooCode",
			"ReceiveSatelliteCertificationNumber",
			"CollectionDate",
			"CashOnDeliveryTax",
			"NumberUnitsShipped",
			"ShipRequestTimeZoneCode",
			"ShipInstructType",
			"ShipInstructStatus",
			"ReceiveShopType",
			"ReceiveShopName",
			"ExcellentDelivery",
			"IsEazy",
			"EazyDeliveryCode",
			"EazyDeliveryName",
			"IsSubscription",
			"SubscriptionId",
			"SubscriptionContinueCount",
			"SubscriptionCycleType",
			"SubscriptionCycleDate",
			"IsLineGiftShippable",
			"ShippingDeadline",
			"UseGiftCardData",
			"PayCharge",
			"ShipCharge",
			"GiftWrapCharge",
			"Discount",
			"Adjustments",
			"SettleAmount",
			"UsePoint",
			"GiftCardDiscount",
			"TotalPrice",
			"SettlePayAmount",
			"TaxRatio",
			"UsePointFixDate",
			"IsUsePointFix",
			"IsGetPointFixAll",
			"TotalMallCouponDiscount",
			"IsGetStoreBonusFixAll",
			"LineGiftCharge",
			"PayMethodAmount",
			"CombinedPayMethodAmount",
			"LineId",
			"ItemId",
			"Title",
			"SubCode",
			"SubCodeOption",
			"ItemOption",
			"Inscription",
			"IsUsed",
			"ImageId",
			"IsTaxable",
			"ItemTaxRatio",
			"Weight",
			"Size",
			"Jan",
			"ProductId",
			"CategoryId",
			"AffiliateRatio",
			"UnitPrice",
			"NonTaxUnitPrice",
			"Quantity",
			"PointAvailQuantity",
			"ReleaseDate",
			"IsShippingFree",
			"IsReturnAccept",
			"LocationPrefecture",
			"LocationCity",
			"HaveReview",
			"IsCampaign",
			"PointFspCode",
			"PointRatioY",
			"PointRatioSeller",
			"UnitGetPoint",
			"IsGetPointFix",
			"GetPointFixDate",
			"CouponData",
			"CouponDiscount",
			"CouponUseNum",
			"OriginalPrice",
			"OriginalNum",
			"LeadTimeText",
			"LeadTimeStart",
			"LeadTimeEnd",
			"PriceType",
			"PickAndDeliveryCode",
			"PickAndDeliveryTransportRuleType",
			"YamatoUndeliverableReason",
			"StoreBonusRatioSeller",
			"UnitGetStoreBonus",
			"IsGetStoreBonusFix",
			"GetStoreBonusFixDate",
			"ItemYahooAucId",
			"ItemYahooAucMerchantId",
			"SellerId",
			"LineGiftAccount",
			"IsLogin",
			"FspLicenseCode",
			"FspLicenseName",
			"GuestAuthId",
		};
		#endregion
		/// <summary>コンテントタイプ JSON</summary>
		private const string CONTENT_TYPE_JSON = "application/json";
		/// <summary>コンテントタイプ URL ENCODED</summary>
		private const string CONTENT_TYPE_URL_ENCODED = "application/x-www-form-urlencoded";
		/// <summary>取得タイプ 認可コード</summary>
		private const string AUTH_ENDPOINT_GRANT_TYPE_AUTH_CODE = "authorization_code";
		/// <summary>取得タイプ リフレッシュトークン</summary>
		private const string AUTH_ENDPOINT_GRANT_TYPE_REFRESH_TOKEN = "refresh_token";
		/// <summary>Yahoo AuthorizationエンドポイントAPI URL</summary>
		private static string s_yahooAuthorizationApiUrl = "";
		/// <summary>Yahoo TokenエンドポイントAPI URL</summary>
		private static string s_yahooTokenApiUrl = "";
		/// <summary>Yahoo 注文詳細API URL</summary>
		private static string s_yahooOrderInfoApiUrl = "";
		/// <summary>Yahoo API 実行基盤</summary>
		private static IYahooApiCallFoundation s_yahooApiCallFoundation;
		/// <summary>実行時間</summary>
		/// <remarks>Dependency Injectionのため</remarks>
		private static DateTime s_runAt;
		/// <summary>ロガー</summary>
		private static YahooApiLogger s_apiLogger = new YahooApiLogger();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YahooApiFacade()
		{
			s_yahooAuthorizationApiUrl = Constants.YAHOO_API_AUTH_API_URL;
			s_yahooTokenApiUrl = Constants.YAHOO_API_TOKEN_API_URL;
			s_yahooOrderInfoApiUrl = Constants.YAHOO_API_ORDERINFO_API_URL;
			s_yahooApiCallFoundation = new YahooApiCallFoundation();
			s_runAt = DateTime.Now;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="yahooAuthorizationApiUrl">Yahoo AuthorizationエンドポイントAPI URL</param>
		/// <param name="yahooTokenApiUrl">Yahoo TokenエンドポイントAPI URL</param>
		/// <param name="yahooOrderInfoApiUrl">Yahoo 注文詳細API URL</param>
		/// <param name="yahooApiCallFoundation">Yahoo API 実行基盤</param>
		/// <param name="runAt">実行時間</param>
		/// <remarks>Dependency Injectionのため</remarks>
		public YahooApiFacade(
			string yahooAuthorizationApiUrl,
			string yahooTokenApiUrl,
			string yahooOrderInfoApiUrl,
			IYahooApiCallFoundation yahooApiCallFoundation,
			DateTime runAt)
		{
			s_yahooAuthorizationApiUrl = yahooAuthorizationApiUrl;
			s_yahooTokenApiUrl = yahooTokenApiUrl;
			s_yahooOrderInfoApiUrl = yahooOrderInfoApiUrl;
			s_yahooApiCallFoundation = yahooApiCallFoundation;
			s_runAt = runAt;
		}

		/// <summary>
		/// AuthorizationエンドポイントAPIのためのURL生成
		/// </summary>
		/// <param name="clientId">クライアントID</param>
		/// <param name="state">ステート</param>
		/// <returns>URL</returns>
		/// <see cref="https://developer.yahoo.co.jp/yconnect/v2/authorization_code/authorization.html"/>
		public string GenerateUrlToGetApiTokens(
			string clientId,
			string state)
		{
			var callbackUrl = Constants.PROTOCOL_HTTPS
				+ Constants.SITE_DOMAIN
				+ Constants.PATH_ROOT_EC
				+ Constants.PAGE_MANAGER_MALL_YAHOO_API_AUTH_CALLBACK;
			var redirectUri = new UrlCreator(callbackUrl).CreateUrl();
			var url = new YahooApiAuthorizationRequestDto(
				responseType: "code",
				clientId: clientId,
				redirectUri: redirectUri,
				bail: "1",
				scope: "openid profile address",
				state: state,
				nonce: "",
				display: "",
				prompt: "",
				maxAge: "",
				codeChallenge: "",
				codeChallengeMethod: "").GenerateUrl(s_yahooAuthorizationApiUrl);
			return url;
		}

		/// <summary>
		/// 認可コードを使用して、アクセストークンとリフレッシュトークンを取得
		/// </summary>
		/// <param name="clientId">クライアントID</param>
		/// <param name="clientSecret">クライアントシークレット</param>
		/// <param name="authCode">認可コード</param>
		/// <param name="redirectUri">リダイレクトURI</param>
		/// <returns>TokenエンドポイントAPIの実行結果</returns>
		public YahooApiTokenResponse GetAccessTokensWithAuthCode(
			string clientId,
			string clientSecret,
			string authCode,
			string redirectUri)
		{
			var dto = YahooApiTokenRequestDto.InstantiateToGetTokenSet(
				grantType: AUTH_ENDPOINT_GRANT_TYPE_AUTH_CODE,
				clientId: clientId,
				clientSecret: clientSecret,
				redirectUri: redirectUri,
				code: authCode,
				codeVerifier: "");
			var queryString = dto.GenerateQueryStringForTokenEndpoint();
			var auth = dto.Base64Encode();
			var result = CallTokenApi(s_yahooTokenApiUrl, queryString, auth);
			return result;
		}

		/// <summary>
		/// 認可コードを使用して、アクセストークンとリフレッシュトークンを取得
		/// </summary>
		/// <param name="clientId">クライアントID</param>
		/// <param name="clientSecret">クライアントシークレット</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <returns>TokenエンドポイントAPIの実行結果</returns>
		public YahooApiTokenResponse GetAccessTokenWithRefreshToken(string clientId, string clientSecret, string refreshToken)
		{
			var dto = YahooApiTokenRequestDto.InstantiateToRefreshAccessToken(
				grantType: AUTH_ENDPOINT_GRANT_TYPE_REFRESH_TOKEN,
				clientId: clientId,
				clientSecret: clientSecret,
				refreshToken: refreshToken);
			var queryString = dto.GenerateQueryString();
			var auth = dto.Base64Encode();
			var result = CallTokenApi(s_yahooTokenApiUrl, queryString, auth);
			return result;
		}

		/// <summary>
		/// TokenエンドポイントAPI実行
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="content">コンテント</param>
		/// <param name="auth">認証ヘッダー値</param>
		/// <returns>TokenエンドポイントAPI結果オブジェクト</returns>
		private YahooApiTokenResponse CallTokenApi(string url, string content, string auth)
		{
			var response = Post(url, content, Encoding.UTF8, CONTENT_TYPE_URL_ENCODED, $"Basic {auth}");
			try
			{
				var dto = JsonConvert.DeserializeObject<YahooApiTokenResponseDto>(response.Content);
				var result = new YahooApiTokenResponse(
					statusCode: response.StatusCode,
					reasonPhrase: response.ReasonPhrase,
					dto: dto,
					calledApiAt: s_runAt);
				return result;
			}
			catch (Exception ex)
			{
				var errDto = JsonConvert.DeserializeObject<YahooApiTokenErrorResponseDto>(response.Content);
				var result = new YahooApiTokenResponse(
					statusCode: response.StatusCode,
					reasonPhrase: response.ReasonPhrase,
					dto: errDto);
				FileLogger.WriteWarn("TokenエンドポイントAPIのエラー値が返却されました。", ex);
				return result;
			}
		}

		/// <summary>
		/// 注文詳細APIでYahoo注文詳細を取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="sellerId">セラーID</param>
		/// <param name="authValue">暗号化した認証情報</param>
		/// <param name="publicKeyVersion">公開鍵バージョン</param>
		/// <returns>Yahoo注文詳細</returns>
		public YahooMallOrder GetYahooMallOrder(
			string orderId,
			string accessToken,
			string sellerId,
			string authValue = "",
			string publicKeyVersion = "")
		{
			// Post
			var requestDto = new YahooApiOrderInfoRequestDto(
				orderId: orderId,
				fields: s_orderFieldsToRequest,
				sellerId: sellerId);
			var rawResponse = Post(
				s_yahooOrderInfoApiUrl,
				requestDto.Serialize(),
				Encoding.UTF8,
				CONTENT_TYPE_JSON,
				$"Bearer {accessToken}",
				signature: authValue,
				publicKeyVersion: publicKeyVersion);

			// Deserialize
			YahooApiOrderInfoResponseDto deserializedResponse;
			using (var xmlReader = new MemoryStream(Encoding.UTF8.GetBytes(rawResponse.Content)))
			{
				try
				{
					deserializedResponse =
						(YahooApiOrderInfoResponseDto)new XmlSerializer(typeof(YahooApiOrderInfoResponseDto))
							.Deserialize(xmlReader);
				}
				catch (Exception ex)
				{
					var errDto =
						(YahooApiOrderInfoErrorResponseDto)new XmlSerializer(typeof(YahooApiOrderInfoErrorResponseDto))
							.Deserialize(xmlReader);
					FileLogger.WriteError(
						$"注文詳細APIのレスポンス値のデシリアライズに失敗しました。code={errDto.Code},message={errDto.Message}",
						ex);
					var errResult = new YahooMallOrder(
						rawResponse.StatusCode,
						rawResponse.ReasonPhrase,
						orderId,
						errDto,
						rawResponse.XSwsAuthorizeStatusHeader);
					return errResult;
				}
			}

			// ドメインオブジェクト作成
			var result = new YahooMallOrder(
				statusCode: rawResponse.StatusCode,
				reasonPhrase: rawResponse.ReasonPhrase,
				orderId: orderId,
				dto: deserializedResponse,
				new YahooMallOrderPaymentMapper(),
				rawResponse.XSwsAuthorizeStatusHeader);

			// 公開鍵認証の結果確認
			if (result.PublicKeyAuthResultCode == YahooApiPublicKeyAuthResponseStatus.Authorized)
			{
				FileLogger.WriteInfo($"公開鍵認証に成功しました。");
			}
			else
			{
				FileLogger.WriteError($"公開鍵認証に失敗しました。");
			}

			return result;
		}

		/// <summary>
		/// POST実行
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="content">コンテント</param>
		/// <param name="encoding">エンコード</param>
		/// <param name="contentType">コンテントタイプ</param>
		/// <param name="authorization">認証ヘッダー</param>
		/// <param name="signature">シグネチャー</param>
		/// <param name="publicKeyVersion">公開鍵バージョン</param>
		/// <returns>API実行結果</returns>
		private SharedApiResponse Post(
			string url,
			string content,
			Encoding encoding,
			string contentType,
			string authorization,
			string signature = "",
			string publicKeyVersion = "")
		{
			try
			{
				s_apiLogger.WriteRequest(url, content);
				var rawResponse = s_yahooApiCallFoundation.HttpPost(
					url: url,
					content: content,
					encoding: encoding,
					contentType: contentType,
					authorization: authorization,
					signature: signature,
					publicKeyVersion: publicKeyVersion).GetAwaiter().GetResult();
				s_apiLogger.WriteResponse(rawResponse.Content);
				return rawResponse;
			}
			catch (Exception ex)
			{
				throw new Exception($"APIの実行に失敗しました。url={url},", ex);
			}
		}
	}
}
