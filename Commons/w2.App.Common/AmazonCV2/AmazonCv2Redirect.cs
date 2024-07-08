/*
=========================================================================================================
  Module      : AmazonCv2リクエスト(AmazonCv2Redirect.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using Amazon.Pay.API.Types;
using Amazon.Pay.API.WebStore.Buyer;
using Amazon.Pay.API.WebStore.CheckoutSession;
using Amazon.Pay.API.WebStore.Types;

namespace w2.App.Common.AmazonCv2
{
	/// <summary>
	/// AmazonCv2遷移準備
	/// </summary>
	public class AmazonCv2Redirect
	{
		/// <summary>
		/// プライベートコンストラクタ(都度払い)
		/// </summary>
		/// <param name="callbackPath">コールバックURL</param>
		private AmazonCv2Redirect(string callbackPath)
		{
			var retUrl = AmazonCv2ApiFacade.CreateCallBackUrlWithProtocol(callbackPath);
			var checkoutSessionScopes = new[]
			{
				CheckoutSessionScope.Name,
				CheckoutSessionScope.Email,
				CheckoutSessionScope.PhoneNumber,
				CheckoutSessionScope.BillingAddress,
			};

			var request = new CreateCheckoutSessionRequest(
				checkoutReviewReturnUrl: retUrl,
				storeId: Constants.PAYMENT_AMAZON_CLIENTID,
				checkoutSessionScopes)
			{
				PaymentDetails =
				{
					AllowOvercharge = true,
					ExtendExpiration = true
				}
			};

			request.DeliverySpecifications.AddressRestrictions.Type = RestrictionType.Allowed;
			var innerRestriction = request
				.DeliverySpecifications
				.AddressRestrictions
				.AddCountryRestriction("JP");
			foreach (var pref in Constants.STR_PREFECTURES_LIST)
			{
				innerRestriction.AddStateOrRegionRestriction(pref);
			}

			var client = AmazonCv2ApiFacade.CreateClient();

			this.Signature = (client != null) ? client.GenerateButtonSignature(request) : string.Empty;
			this.Payload = request.ToJson();
		}
		/// <summary>
		/// プライベートコンストラクタ（継続払い）
		/// </summary>
		/// <param name="callbackPath">コールバックURL</param>
		/// <param name="freqUnit">配送周期単位</param>
		/// <param name="freqValue">配送周期</param>
		/// <param name="amount">金額</param>
		private AmazonCv2Redirect(string callbackPath,
			FrequencyUnit? freqUnit,
			int? freqValue,
			decimal amount)
		{
			var retUrl = AmazonCv2ApiFacade.CreateCallBackUrlWithProtocol(callbackPath);

			var request = new CreateCheckoutSessionRequest(
				checkoutReviewReturnUrl: retUrl,
				storeId: Constants.PAYMENT_AMAZON_CLIENTID)
			{
				ChargePermissionType = ChargePermissionType.Recurring,
				RecurringMetadata =
				{
					Amount =
					{
						Amount = amount,
						CurrencyCode = Currency.JPY
					}
				}
			};

			if ((freqUnit != null) && (freqValue != null))
			{
				request.RecurringMetadata.Frequency.Unit = freqUnit;
				request.RecurringMetadata.Frequency.Value = freqValue;
			}

			request.DeliverySpecifications.AddressRestrictions.Type = RestrictionType.Allowed;
			var innerRestriction = request
				.DeliverySpecifications
				.AddressRestrictions
				.AddCountryRestriction("JP");
			foreach (var pref in Constants.STR_PREFECTURES_LIST)
			{
				innerRestriction.AddStateOrRegionRestriction(pref);
			}

			var client = AmazonCv2ApiFacade.CreateClient();

			this.Signature = (client != null) ? client.GenerateButtonSignature(request) : string.Empty;
			this.Payload = request.ToJson();
		}
		/// <summary>
		/// プライベートコンストラクタ（サインイン）
		/// </summary>
		/// <param name="callbackPath">コールバックURL</param>
		/// <param name="scopes">取得情報スコープ</param>
		private AmazonCv2Redirect(string callbackPath, SignInScope[] scopes)
		{
			var retUrl = AmazonCv2ApiFacade.CreateCallBackUrlWithProtocol(callbackPath);

			var request = new SignInRequest(
				signInReturnUrl: retUrl,
				storeId: Constants.PAYMENT_AMAZON_CLIENTID,
				signInScopes: scopes);

			var client = AmazonCv2ApiFacade.CreateClient();

			this.Signature = (client != null) ? client.GenerateButtonSignature(request) : string.Empty;
			this.Payload = request.ToJson();
		}

		/// <summary>
		/// 都度払いシグネチャ＆ペイロード生成
		/// </summary>
		/// <param name="callbackPath">コールバックURL</param>
		public static AmazonCv2Redirect SignPayloadForOneTime(string callbackPath = Constants.PAGE_FRONT_AMAZON_ORDER_CALLBACK)
		{
			var result = new AmazonCv2Redirect(callbackPath);

			return result;
		}

		/// <summary>
		/// 継続払いシグネチャ＆ペイロード生成
		/// </summary>
		/// <param name="amount">金額</param>
		/// <param name="freqUnit">配送周期単位</param>
		/// <param name="freqValue">配送周期</param>
		/// <param name="callbackPath">コールバックURL</param>
		public static AmazonCv2Redirect SignPayloadForReccuring(
			decimal amount,
			FrequencyUnit? freqUnit = null,
			int? freqValue = null,
			string callbackPath = Constants.PAGE_FRONT_AMAZON_ORDER_CALLBACK)
		{
			var result = new AmazonCv2Redirect(callbackPath, freqUnit, freqValue, amount);

			return result;
		}

		/// <summary>
		/// アマゾンサインインシグネチャ＆ペイロード生成
		/// </summary>
		/// <param name="callbackPath">コールバックURL</param>
		/// <param name="scopes">取得情報スコープ</param>
		public static AmazonCv2Redirect SignPayloadForSignIn(
			string callbackPath = Constants.PAGE_FRONT_AMAZON_LOGIN_CALLBACK,
			SignInScope[] scopes = null)
		{
			if (scopes == null)
			{
				scopes = new[]
				{
					SignInScope.Name,
					SignInScope.Email,
					SignInScope.PostalCode,
					SignInScope.ShippingAddress,
					SignInScope.PhoneNumber,
					SignInScope.BillingAddress,
				};
			}
			var result = new AmazonCv2Redirect(callbackPath, scopes);

			return result;
		}

		/// <summary>シグネチャ</summary>
		public string Signature { get; private set; }
		/// <summary>ペイロード</summary>
		public string Payload { get; private set; }
	}
}
