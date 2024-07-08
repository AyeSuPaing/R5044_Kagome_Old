/*
=========================================================================================================
  Module      : ログインコントローラ(LoginController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Web.Mvc;
using w2.App.Common;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.Login;
using w2.Cms.Manager.WorkerServices;

namespace w2.Cms.Manager.Controllers
{
	/// <summary>
	/// ログインコントローラ
	/// </summary>
	public class LoginController : BaseController
	{
		/// <summary>
		/// インデックス
		/// </summary>
		/// <returns>結果</returns>
		public ActionResult Index()
		{
			w2.App.Common.Web.SessionManager.Clear();
			w2.Common.Web.CookieManager.RemoveCookie(Constants.REQUEST_KEY_COOKIE_NAME_EC, Constants.REQUEST_REMOVE_COOKIE_PATH);
			w2.Common.Web.CookieManager.RemoveCookie(Constants.REQUEST_KEY_COOKIE_NAME_MP, Constants.REQUEST_REMOVE_COOKIE_PATH);
			w2.Common.Web.CookieManager.RemoveCookie(Constants.REQUEST_KEY_COOKIE_NAME_CS, Constants.REQUEST_REMOVE_COOKIE_PATH);
			w2.Common.Web.CookieManager.RemoveCookie(Constants.REQUEST_KEY_COOKIE_NAME_CMS, Constants.REQUEST_REMOVE_COOKIE_PATH);

			var vm = new IndexViewModel
			{
				// Set error message when login session expired
				ErrorMessage = (Request[Constants.REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG] == Constants.REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG_VALID)
					? WebMessages.ShopOperatorLoginSessionExpiredError
					: string.Empty,
			};
			return View(vm);
		}

		/// <summary>
		/// ポスト処理
		/// </summary>
		/// <param name="input">入力クラス</param>
		/// <returns>結果</returns>
		[HttpPost]
		public ActionResult Index(LoginInput input)
		{
			// ログイン実行
			var vm = this.Service.Login(input);
			if (string.IsNullOrEmpty(vm.ErrorMessage) == false) return View(vm);

			// URL作成・リダイレクト
			var url = this.Service.GetUrlForSuccess(
				input.NextUrl,
				this.Request.Url.Host,
				this.SessionWrapper.IsSuperUser,
				this.SessionWrapper.LoginMenuAccessInfo,
				input.LoginExpiredFlg);
			return Redirect(url);
		}

		/// <summary>
		/// Login
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		/// <returns>結果</returns>
		public ActionResult Login(string loginId, string password)
		{
			var login = this.Service.ManagerLogin.Login(loginId, password);
			var result = Json(new
			{
				login,
				JsonRequestBehavior.AllowGet
			});

			return result;
		}

		/// <summary>
		/// Authentication
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="authenticationCode">認証コード</param>
		/// <returns>結果</returns>
		public ActionResult Authentication(string loginId, string authenticationCode)
		{
			var checkAuthCode = this.Service.ManagerLogin.Authentication(loginId, authenticationCode);
			var result = Json(new
			{
				checkAuthCode,
				JsonRequestBehavior.AllowGet
			});

			return result;
		}

		/// <summary>
		/// Resend authentication code
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <returns>結果</returns>
		public ActionResult ResendCode(string loginId)
		{
			var resendAuthCode = this.Service.ManagerLogin.ResendCode(loginId);
			var result = Json(new
			{
				resendAuthCode,
				JsonRequestBehavior.AllowGet
			});

			return result;
		}

		/// <summary>サービス</summary>
		private LoginWorkerService Service { get { return GetDefaultService<LoginWorkerService>(); } }
	}
}
