/*
=========================================================================================================
  Module      : YAHOO API トークン取得進行インターフェース (IYahooApiTokenProcedure.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.MallCooperationSetting;

namespace w2.App.Common.Mall.Yahoo.Interfaces
{
	/// <summary>
	/// トークン取得進行インターフェース
	/// </summary>
	public interface IYahooApiTokenProcedure
	{
		/// <summary>
		/// AuthorizationエンドポイントAPIのURL生成
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="state">ステート</param>
		/// <returns>URL</returns>
		string GenerateYahooApiAuthorizationUrl(string mallId, string state);

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
		void UpdateAccessTokenWithAuthCode(string mallId, string authCode, string redirectUri);

		/// <summary>
		/// リフレッシュトークンを使用して再取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="dateTimeToCompare">有効期限を比較するための日時(基本は実行時の現在時刻)</param>
		/// <remarks>Dependency Injectionのため</remarks>
		/// <param name="forcesAccessTokenRefresh">有効期限が切れていなくても更新を強制するか</param>
		/// <returns>TokenエンドポイントAPIの実行結果</returns>
		YahooApiTokenRefreshResult GetAccessTokenWithRefreshToken(
			string mallId,
			DateTime dateTimeToCompare,
			bool forcesAccessTokenRefresh = false);
		/// <summary>
		/// リフレッシュトークンを使用して再取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		/// <param name="dateTimeToCompare">有効期限を比較するための日時(基本は実行時の現在時刻)</param>
		/// <remarks>Dependency Injectionのため</remarks>
		/// <param name="mallCooperationSetting">モール連携基本設定</param>
		/// <param name="forcesAccessTokenRefresh">有効期限が切れていなくても更新を強制するか</param>
		/// <returns>TokenエンドポイントAPIの実行結果</returns>
		YahooApiTokenRefreshResult GetAccessTokenWithRefreshToken(
			string mallId,
			DateTime dateTimeToCompare,
			MallCooperationSettingModel mallCooperationSetting,
			bool forcesAccessTokenRefresh = false);
	}
}
