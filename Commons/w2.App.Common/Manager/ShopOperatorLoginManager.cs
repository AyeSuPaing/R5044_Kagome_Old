/*
=========================================================================================================
  Module      : 店舗オペレータログインマネージャ(ShopOperatorLoginManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using w2.App.Common.Manager.Menu;
using w2.App.Common.OperationLog;
using w2.App.Common.SendMail;
using w2.Common.Logger;
using w2.Domain;
using w2.Domain.ShopOperator;

namespace w2.App.Common.Manager
{
	/// <summary>
	/// 店舗オペレータログインマネージャ
	/// </summary>
	public class ShopOperatorLoginManager
	{
		/// <summary>Login stage</summary>
		public enum LoginStage
		{
			/// <summary>Login failed</summary>
			Failed = 0,
			/// <summary>Login normal</summary>
			Normal = 1,
			/// <summary>Login with 2-step authentication</summary>
			With2StepAuthentication = 2
		}

		/// <summary>Authentication code allowed characters</summary>
		private const string AUTHENTICATION_CODE_ALLOWED_CHARACTERS =
			"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

		/// <summary>ループバックアドレス</summary>
		private const string CONST_ROOPBACK_ADDRESS = "::1";

		/// <summary>リモートアドレス</summary>
		private const string CONST_REMOTE_ADDR = "REMOTE_ADDR";

		#region +TryLogin ログイン試行
		/// <summary>
		/// ログイン試行
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		/// <returns>モデル</returns>
		public ShopOperatorLoginResult TryLogin(
			string shopId,
			string loginId,
			string password)
		{
			// オペレーター情報取得・ログインチェック
			var operatorservice = new ShopOperatorService();
			var shopOperator = operatorservice.GetByLoginId(shopId, loginId);
			var errorReason = ((shopOperator == null) || (shopOperator.Password != password))
				? ShopOperatorLoginResult.ErrorReasonType.NoOperatpr
				: (shopOperator.ValidFlg != Domain.Constants.FLG_SHOPOPERATOR_VALID_FLG_VALID)
					? ShopOperatorLoginResult.ErrorReasonType.LoginCountLimited
					: ShopOperatorLoginResult.ErrorReasonType.NoError;

			return (errorReason == ShopOperatorLoginResult.ErrorReasonType.NoError)
				? new ShopOperatorLoginResult { ShopOperator = shopOperator, }
				: new ShopOperatorLoginResult { ErrorReason = errorReason, };
		}
		#endregion

		/// <summary>
		/// Login with 2-step authentication
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		/// <returns>A status code or error message</returns>
		public string LoginWith2StepAuthentication(string loginId, string password)
		{
			try
			{
				if (IsNecessityTwoFactorAuthentication() == false)
				{
					var loginNormal = GetLoginResult(LoginStage.Normal);
					return loginNormal;
				}

				var loginResult = new ShopOperatorLoginManager().TryLogin(
					Constants.CONST_DEFAULT_SHOP_ID,
					loginId,
					password);

				if (loginResult.IsSuccess == false)
				{
					var loginFailed = GetLoginResult(LoginStage.Failed);
					return loginFailed;
				}

				if (string.IsNullOrEmpty(loginResult.ShopOperator.MailAddr))
				{
					var errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_LOGIN_MAIL_ADDRESS_NOT_SETTING);
					return errorMessage;
				}

				if (Need2StepAuthentication(loginResult.ShopOperator) == false)
				{
					var loginNormal = GetLoginResult(LoginStage.Normal);
					return loginNormal;
				}

				SendAuthenticationCode(loginResult.ShopOperator);

				var loginWith2StepAuthentication = GetLoginResult(LoginStage.With2StepAuthentication);
				return loginWith2StepAuthentication;
			}
			catch (Exception ex)
			{
				var errorMessage = HandleError(ex);
				return errorMessage;
			}
		}

		/// <summary>
		/// 2段階認証が必要か
		/// </summary>
		/// <returns>必要：True、不要：False</returns>
		private bool IsNecessityTwoFactorAuthentication()
		{
			if (Constants.TWO_STEP_AUTHENTICATION_OPTION_ENABLED == false) return false;

			var ipAddresses = Constants.TWO_STEP_AUTHENTICATION_EXCLUSION_IPADDRESS.Split(',');
			var request = HttpContext.Current.Request;

			// 開発環境の場合、ローカルのIPアドレスを取得
			var externalIp = request.IsLocal
				? new WebClient().DownloadString("https://api.ipify.org")
				: request.ServerVariables[CONST_REMOTE_ADDR];

			if (ipAddresses.Contains(externalIp)) return false;

			return true;
		}

		/// <summary>
		/// Check authentication code
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="authenticationCode">認証コード</param>
		/// <returns>A status code or error message</returns>
		public string CheckAuthenticationCode(string loginId, string authenticationCode)
		{
			try
			{
				if (Constants.TWO_STEP_AUTHENTICATION_OPTION_ENABLED == false)
				{
					var errorMessage = CommerceMessages.GetMessages(
						CommerceMessages.ERRMSG_MANAGER_TWO_STEP_AUTHENTICATION_DISABLED);
					return errorMessage;
				}

				if (string.IsNullOrEmpty(loginId))
				{
					var errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SYSTEM_ERROR);
					return errorMessage;
				}

				if (IsValidAuthenticationCode(loginId, authenticationCode) == false)
				{
					var errorMessage = IncreaseLoginErrorCount(loginId);
					if (string.IsNullOrEmpty(errorMessage))
					{
						errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_LOGIN_INVALID_AUTHENTICATION_CODE);
					}

					return errorMessage;
				}

				var loginWith2StepAuthentication = GetLoginResult(LoginStage.With2StepAuthentication);
				return loginWith2StepAuthentication;
			}
			catch (Exception ex)
			{
				var errorMessage = HandleError(ex);
				return errorMessage;
			}
		}

		/// <summary>
		/// Resend code
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <returns>An error message or empty string.</returns>
		public string ResendCode(string loginId)
		{
			try
			{
				var errorMessage = string.Empty;
				if (Constants.TWO_STEP_AUTHENTICATION_OPTION_ENABLED == false)
				{
					errorMessage = CommerceMessages.GetMessages(
						CommerceMessages.ERRMSG_MANAGER_TWO_STEP_AUTHENTICATION_DISABLED);
					return errorMessage;
				}

				SendAuthenticationCode(loginId);
				return errorMessage;
			}
			catch (Exception ex)
			{
				var errorMessage = HandleError(ex);
				return errorMessage;
			}
		}

		/// <summary>
		/// IPアドレスを取得
		/// </summary>
		/// <returns>IPアドレス</returns>
		/// <remarks>複数のプロキシを経由した場合、カンマ区切りでIPアドバイスが格納されるため「,」で分割し、最初の値を返却</remarks>
		public string GetIpAddress()
		{
			var ipAddress = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
			if (string.IsNullOrEmpty(ipAddress))
			{
				ipAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
			}
			var result = ipAddress.Split(',')[0];

			return result;
		}

		/// <summary>
		/// Is valid authentication code
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="authenticationCode">認証コード</param>
		/// <returns>True if authentication is valid, otherwise false</returns>
		private bool IsValidAuthenticationCode(string loginId, string authenticationCode)
		{
			if (string.IsNullOrEmpty(authenticationCode)) return false;

			var isValid = false;
			if (this.AuthenticationCodeSession != null)
			{
				// Check authentication code in session
				isValid = ((string.IsNullOrEmpty(this.AuthenticationCodeSession.Code) == false)
					&& (this.AuthenticationCodeSession.Code == authenticationCode)
					&& (DateTime.Now < this.AuthenticationCodeSession.ExpiredDate));
			}

			if (isValid == false)
			{
				// If authentication code session is not exists, check authentication code store in database.
				var shopOperator = DomainFacade.Instance.ShopOperatorService.GetByLoginId(
					Constants.CONST_DEFAULT_SHOP_ID,
					loginId);

				isValid = ((string.IsNullOrEmpty(shopOperator.AuthenticationCode) == false)
					&& (shopOperator.AuthenticationCode == authenticationCode)
					&& (DateTime.Now < shopOperator.DateCodeSend.AddMinutes(Constants.TWO_STEP_AUTHENTICATION_DEADLINE)));
			}

			return isValid;
		}

		/// <summary>
		/// Create authentication code
		/// </summary>
		/// <returns>Authentication code</returns>
		private string CreateAuthenticationCode()
		{
			var random = new Random();
			var authCode = new string(Enumerable.Repeat(AUTHENTICATION_CODE_ALLOWED_CHARACTERS, 8)
				.Select(item => item[random.Next(item.Length)])
				.ToArray());

			return authCode;
		}

		/// <summary>
		/// Send authentication code via email
		/// </summary>
		/// <param name="loginId">ログインID</param>
		private void SendAuthenticationCode(string loginId)
		{
			var shopOperator = DomainFacade.Instance.ShopOperatorService.GetByLoginId(
				Constants.CONST_DEFAULT_SHOP_ID,
				loginId);

			SendAuthenticationCode(shopOperator);
		}

		/// <summary>
		/// Send authentication code via email
		/// </summary>
		/// <param name="shopOperator">店舗管理者マスタモデル</param>
		private void SendAuthenticationCode(ShopOperatorModel shopOperator)
		{
			var authenticationCode = CreateAuthenticationCode();
			var mailData = new Hashtable
			{
				{ Constants.FIELD_SHOPOPERATOR_MAIL_ADDR, shopOperator.MailAddr },
				{ Constants.CONST_AUTHENTICATION_CODE, authenticationCode },
				{ Constants.CONST_AUTHENTICATION_DEADLINE, Constants.TWO_STEP_AUTHENTICATION_DEADLINE }
			};

			DomainFacade.Instance.ShopOperatorService.UpdateAuthenticationCode(
				Constants.CONST_DEFAULT_SHOP_ID,
				shopOperator.LoginId,
				authenticationCode);

			new SendMailCommon().Send2StepAuthenticationCode(
				Constants.CONST_MAIL_ID_SEND_2_STEP_AUTHENTICATION_CODE,
				mailData);

			this.AuthenticationCodeSession = new AuthenticationCode(
				authenticationCode,
				DateTime.Now.AddMinutes(Constants.TWO_STEP_AUTHENTICATION_DEADLINE));
		}

		/// <summary>
		/// Need 2-step authentication
		/// </summary>
		/// <param name="shopOperator">A shop operator modal</param>
		/// <returns>True if need 2-step authentication, otherwise false</returns>
		private bool Need2StepAuthentication(ShopOperatorModel shopOperator)
		{
			var currentIpAddress = GetIpAddress();
			var isSameIpAddress = ((string.IsNullOrEmpty(shopOperator.RemoteAddr) == false)
				&& (shopOperator.RemoteAddr == currentIpAddress));

			var isSetAuthenticationCode = ((string.IsNullOrEmpty(shopOperator.AuthenticationCode) == false));

			var authenticationExpirationDate =
				shopOperator.DateLastLoggedIn.AddDays(Constants.TWO_STEP_AUTHENTICATION_EXPIRATION_DATE);
			var isExpired = (DateTime.Now > authenticationExpirationDate);

			var isFirstLogin = (shopOperator.DateLastLoggedIn == new DateTime(1753, 1, 1));
			return ((isSameIpAddress == false)
				|| (isSetAuthenticationCode == false)
				|| isFirstLogin
				|| isExpired);
		}

		/// <summary>
		/// Increase login error count
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <returns>Error message</returns>
		public string IncreaseLoginErrorCount(string loginId)
		{
			// 入力ログインIDに関するエラー情報がある？
			if (this.LoginErrorCounts.ContainsKey(loginId))
			{
				this.LoginErrorCounts[loginId]++;
			}
			else
			{
				this.LoginErrorCounts.Add(loginId, 1);
				WriteLoginLog(loginId, OperationKbn.MISS);
			}

			// 制限を越えていれば該当オペレータを無効にする
			var errorMessage = string.Empty;
			if (this.LoginErrorCounts[loginId] >= Constants.ERROR_LOGIN_LIMITED_COUNT)
			{
				DomainFacade.Instance.ShopOperatorService.UpdateValidFlgOffByLoginId(
					Constants.CONST_DEFAULT_SHOP_ID,
					loginId,
					Constants.FLG_LASTCHANGED_SYSTEM);

				// エラー文言を画面へ表示
				WriteLoginLog(loginId, OperationKbn.LOCK);
				errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_MANAGER_LOGIN_LIMITED_COUNT_ERROR);
			}

			return errorMessage;
		}

		/// <summary>
		/// Get login result
		/// </summary>
		/// <param name="loginStage">A login stage</param>
		/// <returns>A login result</returns>
		public static string GetLoginResult(LoginStage loginStage)
		{
			var result = ((int)loginStage).ToString();
			return result;
		}

		/// <summary>
		/// オペレータログインログ出力処理
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="operationKbn">ログイン区分</param>
		public void WriteLoginLog(string loginId, OperationKbn operationKbn)
		{
			var loginShopOperator = (ShopOperatorModel)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_SHOP_OPERTOR]
				?? new ShopOperatorModel();
			OperationLogWriter.WriteLoginLog(
				loginShopOperator.OperatorId,
				loginId,
				HttpContext.Current.Request.UserHostAddress,
				HttpContext.Current.Session.SessionID,
				loginShopOperator.Name,
				operationKbn);
		}

		/// <summary>
		/// Handle error
		/// </summary>
		/// <param name="ex">An exception</param>
		/// <returns>An error message</returns>
		private static string HandleError(Exception ex)
		{
			AppLogger.WriteError(ex);

			var errorMessage = CommerceMessages.GetMessages(CommerceMessages.ERRMSG_SYSTEM_ERROR);
			return errorMessage;
		}

		/// <summary>Authentication code</summary>
		private AuthenticationCode AuthenticationCodeSession
		{
			get
			{
				return (AuthenticationCode)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_AUTHENTICATION_CODE];
			}
			set
			{
				HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_AUTHENTICATION_CODE] = value;
			}
		}
		/// <summary>ログインエラーカウント</summary>
		private Dictionary<string, int> LoginErrorCounts
		{
			get
			{
				if (HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_ERROR_INFO] == null)
				{
					HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_ERROR_INFO] = new Dictionary<string, int>();
				}
				var loginErrorCounts = (Dictionary<string, int>)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_ERROR_INFO];
				return loginErrorCounts;
			}
			set
			{
				HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_ERROR_INFO] = value;
			}
		}

		/// <summary>
		/// 店舗管理者ログイン結果
		/// </summary>
		public class ShopOperatorLoginResult
		{
			/// <summary>
			/// エラーの理由
			/// </summary>
			public enum ErrorReasonType
			{
				/// <summary>エラーなし</summary>
				NoError,
				/// <summary>オペレータなし</summary>
				NoOperatpr,
				/// <summary>メニュー権限なしエラー</summary>
				MenuUnacessable,
				/// <summary>ログイン試行回数エラー</summary>
				LoginCountLimited,
			}

			/// <summary>成功したか</summary>
			public bool IsSuccess
			{
				get { return (this.ErrorReason == ErrorReasonType.NoError); }
			}
			/// <summary>エラー理由がログイン試行回数エラーか</summary>
			public bool IsErrorLoginCountLimit
			{
				get { return (this.ErrorReason == ErrorReasonType.LoginCountLimited); }
			}
			/// <summary>オペレータ情報</summary>
			public ShopOperatorModel ShopOperator { get; set; }
			/// <summary>メニューアクセス情報</summary>
			public MenuAccessInfo MenuAccessInfo { get; set; }
			/// <summary>エラー理由</summary>
			public ErrorReasonType ErrorReason { get; set; }
		}

		/// <summary>
		/// 認証コード
		/// </summary>
		[Serializable]
		public class AuthenticationCode
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public AuthenticationCode()
			{
				this.Code = string.Empty;
			}

			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="code">コード</param>
			/// <param name="expiredDate">有効期限</param>
			public AuthenticationCode(string code, DateTime expiredDate)
			{
				this.Code = code;
				this.ExpiredDate = expiredDate;
			}

			/// <summary>コード</summary>
			public string Code { get; set; }
			/// <summary>有効期限</summary>
			public DateTime ExpiredDate { get; set; }
		}
	}
}
