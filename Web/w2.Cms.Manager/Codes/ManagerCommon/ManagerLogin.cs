/*
=========================================================================================================
  Module      : 管理画面ログイン(ManagerLogin.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.App.Common.Manager;
using w2.App.Common.Manager.Menu;
using w2.App.Common.OperationLog;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Codes.Menu;
using w2.Common.Logger;
using w2.Domain;
using w2.Domain.MenuAuthority.Helper;

namespace w2.Cms.Manager.Codes.ManagerCommon
{
	/// <summary>
	/// 管理画面ログイン
	/// </summary>
	public class ManagerLogin : IManagerLogin
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <param name="sessionWrapper">セッションラッパー</param>
		public ManagerLogin(MenuAuthorityHelper.ManagerSiteType managerSiteType, SessionWrapper sessionWrapper)
		{
			this.ManagerSiteType = managerSiteType;
			this.SessionWrapper = sessionWrapper;
			this.ShopOperatorLoginManager = new ShopOperatorLoginManager();
		}

		/// <summary>
		/// ログインアクション
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		/// <returns>エラーメッセージ</returns>
		public string LoginAction(string loginId, string password)
		{
			// 共通エラーページはテンプレートを含んでいるため表示したくない。
			// そのため、try～catchで囲んでエラー集約ハンドラへ渡さないようにする
			try
			{
				var loginManager = new ShopOperatorLoginManager();

				// オペレーター情報取得・メニュー権限取得
				var loginResult = loginManager.TryLogin(
					Constants.CONST_DEFAULT_SHOP_ID,
					loginId,
					password);
				if (loginResult.IsSuccess == false)
				{
					var errorMessage = loginManager.IncreaseLoginErrorCount(loginId);
					if (string.IsNullOrEmpty(errorMessage) == false)
					{
						loginResult.ErrorReason = ShopOperatorLoginManager.ShopOperatorLoginResult.ErrorReasonType.LoginCountLimited;
					}
					else
					{
						errorMessage = GetLoginErrorMessage(loginResult.ErrorReason);
					}

					return errorMessage;
				}

				// メニュー情報取得・メニューチェック
				loginResult.MenuAccessInfo = new OperatorMenuManager().GetOperatorMenuList(
					this.ManagerSiteType,
					loginResult.ShopOperator,
					ManagerMenuCache.Instance);
				if (loginResult.MenuAccessInfo.LargeMenus.Length == 0)
				{
					var errorMessage = GetLoginErrorMessage(ShopOperatorLoginManager.ShopOperatorLoginResult.ErrorReasonType.MenuUnacessable);
					return errorMessage;
				}

				DomainFacade.Instance.ShopOperatorService.UpdateRemoteAddress(
					Constants.CONST_DEFAULT_SHOP_ID,
					loginId,
					loginManager.GetIpAddress());

				WriteLoginLog(loginResult.ShopOperator.LoginId, OperationKbn.SUCCESS);

				this.SessionWrapper.LoginOperator = loginResult.ShopOperator;
				this.SessionWrapper.LoginMenuAccessInfo = loginResult.MenuAccessInfo;
				this.SessionWrapper.LoginErrorCounts = null;	// 念のためリセットしておく
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
#if DEBUG
				throw;
#endif
#if !DEBUG
				return WebMessages.SystemError;
#endif
			}
			return "";
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="errorReason">エラー理由</param>
		/// <returns>エラーメッセージ</returns>
		private string GetLoginErrorMessage(ShopOperatorLoginManager.ShopOperatorLoginResult.ErrorReasonType errorReason)
		{
			switch (errorReason)
			{
				case ShopOperatorLoginManager.ShopOperatorLoginResult.ErrorReasonType.NoOperatpr:
					return WebMessages.ShopOperatorLoginError;

				case ShopOperatorLoginManager.ShopOperatorLoginResult.ErrorReasonType.LoginCountLimited:
					return WebMessages.ShopOperatorLoginLimitedCountError;

				case ShopOperatorLoginManager.ShopOperatorLoginResult.ErrorReasonType.MenuUnacessable:
					return WebMessages.ShopOperatorUnaccessable;
			}
			return "";
		}

		/// <summary>
		/// オペレータログインログ出力処理
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="operationKbn">ログイン区分</param>
		public void WriteLoginLog(string loginId, OperationKbn operationKbn)
		{
			this.ShopOperatorLoginManager.WriteLoginLog(loginId, operationKbn);
		}

		/// <summary>
		/// Login
		/// </summary>
		/// <param name="loginId">Login id</param>
		/// <param name="password">Password</param>
		/// <returns>A status code or error message</returns>
		public string Login(string loginId, string password)
		{
			var result = this.ShopOperatorLoginManager.LoginWith2StepAuthentication(loginId, password);
			return result;
		}

		/// <summary>
		/// Authentication
		/// </summary>
		/// <param name="loginId">Login id</param>
		/// <param name="authenticationCode">Authentication code</param>
		/// <returns>Error message</returns>
		public string Authentication(string loginId, string authenticationCode)
		{
			var result = this.ShopOperatorLoginManager.CheckAuthenticationCode(
				loginId,
				authenticationCode);
			return result;
		}

		/// <summary>
		/// Resend authentication code
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <returns>An error message or empty string.</returns>
		public string ResendCode(string loginId)
		{
			var result = this.ShopOperatorLoginManager.ResendCode(loginId);
			return result;
		}

		/// <summary>メニューアクセスレベルフィールド名取得</summary>
		public MenuAuthorityHelper.ManagerSiteType ManagerSiteType { get; private set; }
		/// <summary>セッションラッパー</summary>
		public SessionWrapper SessionWrapper { get; private set; }
		/// <summary>店舗オペレータログインマネージャ</summary>
		private ShopOperatorLoginManager ShopOperatorLoginManager { get; set; }
	}
}
