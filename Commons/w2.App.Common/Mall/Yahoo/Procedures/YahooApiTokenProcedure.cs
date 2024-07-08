/*
=========================================================================================================
  Module      : YAHOO API トークン取得進行クラス (YahooApiTokenProcedure.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Mall.Yahoo.Foundation;
using w2.App.Common.Mall.Yahoo.Interfaces;
using w2.Common.Logger;
using w2.Domain.MallCooperationSetting;

namespace w2.App.Common.Mall.Yahoo.Procedures
{
	/// <summary>
	/// YAHOO API トークン取得進行クラス
	/// </summary>
	public class YahooApiTokenProcedure : IYahooApiTokenProcedure
	{
		private readonly IMallCooperationSettingService _mallCooperationSettingService;
		private readonly IYahooApiFacade _yahooApiFacade;
		private static Random s_random = new Random();
		/// <summary>認証コード文字リスト</summary>
		private const string AUTHENTICATION_CODE_CHARACTERS =
			"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YahooApiTokenProcedure()
		{
			_mallCooperationSettingService = new MallCooperationSettingService();
			_yahooApiFacade = new YahooApiFacade();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YahooApiTokenProcedure(
			IMallCooperationSettingService mallCooperationSettingService,
			IYahooApiFacade yahooApiFacade)
		{
			_mallCooperationSettingService = mallCooperationSettingService;
			_yahooApiFacade = yahooApiFacade;
		}

		/// <summary>
		/// 乱数生成
		/// </summary>
		/// <returns>乱数</returns>
		public static string GenerateRandomStateCode()
		{
			var stateLength = 11;
			var result = new string(
				Enumerable.Repeat(AUTHENTICATION_CODE_CHARACTERS, stateLength)
					.Select(s => s[s_random.Next(s.Length)])
					.ToArray());
			return result;
		}

		/// <summary>
		/// AuthorizationエンドポイントAPIのURL生成
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="state">ステート</param>
		/// <returns>URL</returns>
		public string GenerateYahooApiAuthorizationUrl(string mallId, string state)
		{
			var setting = _mallCooperationSettingService.Get(shopId: Constants.CONST_DEFAULT_SHOP_ID, mallId: mallId);
			var url = _yahooApiFacade.GenerateUrlToGetApiTokens(clientId: setting.YahooApiClientId, state: state);
			return url;
		}

		/// <summary>
		/// 認可コードを使用してアクセストークンを更新
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="authCode">認可コード</param>
		/// <param name="redirectUri">リダイレクトURI</param>
		/// <remarks>
		/// 認可コード: アクセストークンを取得するために必要なコード<br />
		/// AuthorizationエンドポイントAPIを実行して取得できる。
		/// </remarks>
		public void UpdateAccessTokenWithAuthCode(string mallId, string authCode, string redirectUri)
		{
			var setting = _mallCooperationSettingService.Get(Constants.CONST_DEFAULT_SHOP_ID, mallId);
			if (setting == null)
			{
				FileLogger.WriteError($"モール連携基本設定が見つかりません。mall_id={mallId}");
				return;
			}
			var tokenApiResult = _yahooApiFacade.GetAccessTokensWithAuthCode(
				clientId: setting.YahooApiClientId,
				clientSecret: setting.YahooApiClientSecret,
				authCode: authCode,
				redirectUri: redirectUri);
			_mallCooperationSettingService.UpdateYahooApiTokenSet(
				Constants.CONST_DEFAULT_SHOP_ID,
				mallId,
				tokenApiResult.AccessToken,
				tokenApiResult.AccessTokenExpirationDateTime,
				tokenApiResult.RefreshToken,
				tokenApiResult.RefreshTokenExpirationDateTime);
		}

		/// <summary>
		/// リフレッシュトークンを使用して再取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="dateTimeToCompare">有効期限を比較するための日時(基本は実行時の現在時刻)</param>
		/// <remarks>Dependency Injectionのため</remarks>
		/// <param name="forcesAccessTokenRefresh">有効期限が切れていなくても更新を強制するか</param>
		/// <returns>TokenエンドポイントAPIの実行結果</returns>
		public YahooApiTokenRefreshResult GetAccessTokenWithRefreshToken(
			string mallId,
			DateTime dateTimeToCompare,
			bool forcesAccessTokenRefresh = false)
		{
			// モール連携基本設定取得
			var mallCooperationSetting =
				_mallCooperationSettingService.Get(Constants.CONST_DEFAULT_SHOP_ID, mallId: mallId);
			if (mallCooperationSetting == null)
			{
				FileLogger.WriteError($"モール連携基本設定が存在しません。指定したモールIDを確認してください。mall_id={mallId}");
				return new YahooApiTokenRefreshResult(YahooApiTokenRefreshResultCode.FailedToRefresh, accessToken: "");
			}

			var result = GetAccessTokenWithRefreshToken(
				mallId,
				dateTimeToCompare,
				mallCooperationSetting,
				forcesAccessTokenRefresh);
			return result;
		}
		/// <summary>
		/// リフレッシュトークンを使用して再取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="dateTimeToCompare">有効期限を比較するための日時(基本は実行時の現在時刻)</param>
		/// <remarks>Dependency Injectionのため</remarks>
		/// <param name="mallCooperationSetting">モール連携基本設定</param>
		/// <param name="forcesAccessTokenRefresh">有効期限が切れていなくても更新を強制するか</param>
		/// <returns>TokenエンドポイントAPIの実行結果</returns>
		public YahooApiTokenRefreshResult GetAccessTokenWithRefreshToken(
			string mallId,
			DateTime dateTimeToCompare,
			MallCooperationSettingModel mallCooperationSetting,
			bool forcesAccessTokenRefresh = false)
		{
			// トークンセットの取得
			var tokenSet = new YahooApiTokenSet(
				accessToken: mallCooperationSetting.YahooApiAccessToken,
				accessTokenExpirationDateTime: mallCooperationSetting.YahooApiAccessTokenExpirationDatetime,
				refreshToken: mallCooperationSetting.YahooApiRefreshToken,
				refreshTokenExpirationDateTime: mallCooperationSetting.YahooApiRefreshTokenExpirationDatetime);

			// 再取得が不要な場合、何もしない
			if (tokenSet.HasAccessTokenAvailable(dateTimeToCompare) && forcesAccessTokenRefresh == false)
			{
				FileLogger.WriteDebug($"トークンを再取得する必要がありません。mall_id={mallId}");
				return new YahooApiTokenRefreshResult(
					YahooApiTokenRefreshResultCode.NoNeedToRefreshAccessToken,
					accessToken: mallCooperationSetting.YahooApiAccessToken);
			}

			// ブラウザ操作が必要である場合、ブラウザ操作が必要
			if (tokenSet.RequiresToAcquireNewAccessTokenWithAuthCode(dateTimeToCompare))
			{
				// ブラウザ上の操作が必要なAuthorization APIを実行しなければいけないため、ここでは処理しない
				FileLogger.WriteError($"EC管理画面上でトークンを再取得してください。このエラーが頻出する場合は、公開鍵による認証を行っていない可能性があります。mall_id={mallId}");
				return new YahooApiTokenRefreshResult(
					YahooApiTokenRefreshResultCode.RefreshOnBrowserRequired,
					accessToken: "");
			}

			// Refresh Token を使用して、Access Tokenを再取得
			var tokenApiResult = _yahooApiFacade.GetAccessTokenWithRefreshToken(
				clientId: mallCooperationSetting.YahooApiClientId,
				clientSecret: mallCooperationSetting.YahooApiClientSecret,
				refreshToken: tokenSet.RefreshToken);
			if (tokenApiResult.IsSuccessful() == false)
			{
				FileLogger.WriteError("アクセストークンの取得に失敗しました。");
				return new YahooApiTokenRefreshResult(YahooApiTokenRefreshResultCode.FailedToRefresh, accessToken: "");
			}

			// トークンを更新
			_mallCooperationSettingService.UpdateYahooApiTokenSet(
				Constants.CONST_DEFAULT_SHOP_ID,
				mallId,
				tokenApiResult.AccessToken,
				tokenApiResult.AccessTokenExpirationDateTime,
				tokenApiResult.RefreshToken,
				tokenApiResult.RefreshTokenExpirationDateTime);
			return new YahooApiTokenRefreshResult(
				YahooApiTokenRefreshResultCode.SuccessfullyRefreshed,
				tokenApiResult.AccessToken,
				tokenApiResult.AccessTokenExpirationDateTime);
		}
	}
}
