/*
=========================================================================================================
  Module      : 基底ページ (BasePage.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web;
using w2.Domain.TempDatas;

namespace BasePage
{
	public abstract class BasePage : System.Web.UI.Page
	{
		/// <summary>Httpヘッダー認証</summary>
		public const string HTTP_HEADER_AUTHORIZATION = "Authorization";
		/// <summary>試行回数制御先頭文字</summary>
		public const string NUMBER_OF_TRIALS_PREFIX_STRING_LINE = "line_number_of_trials_";
		/// <summary>試行回数制御先頭文字</summary>
		public const string NUMBER_OF_TRIALS_PREFIX_STRING_LETRO = "letro_number_of_trials_";
		/// <summary>
		/// 認証タイプ
		/// </summary>
		public enum AuthKeyType
		{
			Letro,
			Line
		}

		/// <summary>
		/// 認証の検証
		/// </summary>
		/// <param name="context">コンテキスト</param>
		/// <param name="secretKey">シークレットキー</param>
		/// <returns>認証の結果</returns>
		protected bool IsValidAuthorization(HttpContext context, string secretKey)
		{
			var clientAuth = context.Request.Headers[HTTP_HEADER_AUTHORIZATION];
			var configAuth = string.Format("Bearer {0}", secretKey);
			return clientAuth == configAuth;
		}

		/// <summary>
		/// 認証キーロックチェック
		/// </summary>
		/// <param name="ipAddress">IPアドレス</param>
		/// <param name="authKeyType">認証キータイプ</param>
		/// <returns>ロックしているか</returns>
		protected bool IsLocked(string ipAddress, AuthKeyType authKeyType)
		{
			string prefixString;
			switch (authKeyType)
			{
				case AuthKeyType.Letro:
					prefixString = NUMBER_OF_TRIALS_PREFIX_STRING_LETRO;
					break;

				case AuthKeyType.Line:
					prefixString = NUMBER_OF_TRIALS_PREFIX_STRING_LINE;
					break;

				default:
					throw new ArgumentException("Unsupported AuthKeyType");
			}
			var numberOfTrialsKey = string.Format(
				"{0}{1}",
				prefixString,
				ipAddress);
			var authKeyErrorInfos = GetAuthKeyErrorInfos(numberOfTrialsKey, authKeyType);
			var authKeyErrorCount = authKeyErrorInfos.Item1;

			return ((authKeyErrorCount != null) && ((int)authKeyErrorCount.TempDataDeserialized <= 0));
		}

		/// <summary>
		/// 認証キー試行回数情報取得
		/// </summary>
		/// <param name="numberOfTrialsKey">試行回数キー</param>
		/// <param name="authKeyType">認証キータイプ</param>
		/// <returns>試行回数情報</returns>
		protected Tuple<TempDatasModel> GetAuthKeyErrorInfos(string numberOfTrialsKey, AuthKeyType authKeyType)
		{
			TempDatasService.TempType tempType;
			int lockMinutes;
			switch (authKeyType)
			{
				case AuthKeyType.Letro:
					lockMinutes = Constants.POSSIBLE_LETRO_AUTH_KEY_LOCK_MINUTES;
					tempType = TempDatasService.TempType.LetroApiNumberOfTrials;
					break;

				case AuthKeyType.Line:
					lockMinutes = Constants.POSSIBLE_LINE_AUTH_KEY_LOCK_MINUTES;
					tempType = TempDatasService.TempType.LineApiNumberOfTrials;
					break;
				
				default:
					throw new ArgumentException("Unsupported AuthKeyType");
			}
			return new Tuple<TempDatasModel>(
				new TempDatasService().Resotre(
					tempType,
					numberOfTrialsKey,
					lockMinutes));
		}

		/// <summary>
		/// 認証キーロックデータのログイン試行可能回数更新
		/// </summary>
		/// <param name="ipAddress">IPアドレス</param>
		/// <param name="authKeyType">認証キータイプ</param>
		protected void UpdateLockPossibleTrialLoginCount(string ipAddress, AuthKeyType authKeyType)
		{
			string prefixString;
			int possibleErrorCount;
			TempDatasService.TempType tempType;
			switch (authKeyType)
			{
				case AuthKeyType.Letro:
					prefixString = NUMBER_OF_TRIALS_PREFIX_STRING_LETRO;
					possibleErrorCount = Constants.POSSIBLE_LETRO_AUTH_KEY_ERROR_COUNT;
					tempType = TempDatasService.TempType.LetroApiNumberOfTrials;
					break;

				case AuthKeyType.Line:
					prefixString = NUMBER_OF_TRIALS_PREFIX_STRING_LINE;
					possibleErrorCount = Constants.POSSIBLE_LINE_AUTH_KEY_ERROR_COUNT;
					tempType = TempDatasService.TempType.LineApiNumberOfTrials;
					break;

				default:
					throw new ArgumentException("Unsupported AuthKeyType");
			}

			var numberOfTrialsKey = string.Format(
				"{0}{1}",
				prefixString,
				ipAddress);
			var authKeyErrorInfos = GetAuthKeyErrorInfos(numberOfTrialsKey, authKeyType);
			var authKeyErrorCount = authKeyErrorInfos.Item1;

			new TempDatasService().Save(
				tempType,
				numberOfTrialsKey,
				(authKeyErrorCount == null)
					? (possibleErrorCount - 1)
					: ((int)authKeyErrorCount.TempDataDeserialized - 1));
		}
	}
}
