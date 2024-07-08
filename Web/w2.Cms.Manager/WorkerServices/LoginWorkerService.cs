/*
=========================================================================================================
  Module      : ログインワーカーサービス(LoginWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.Manager.Menu;
using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.ManagerCommon;
using w2.Cms.Manager.Codes.Menu;
using w2.Cms.Manager.Input;
using w2.Cms.Manager.ViewModels.Login;
using w2.Common.Web;
using w2.Domain.MenuAuthority.Helper;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// ログインワーカーサービス
	/// </summary>
	public class LoginWorkerService : BaseWorkerService
	{
		/// <summary>
		/// コンストラクタ（テストの時に利用）
		/// </summary>
		public LoginWorkerService()
		{
			this.ManagerLogin = new ManagerLogin(MenuAuthorityHelper.ManagerSiteType.Cms, this.SessionWrapper);
		}

		/// <summary>
		/// ログイン処理
		/// </summary>
		/// <param name="input">ログイン入力</param>
		/// <returns>ビューモデル</returns>
		public IndexViewModel Login(LoginInput input)
		{
			var errorMessage = this.ManagerLogin.LoginAction(
				input.LoginId.Trim(),
				input.Password.Trim());
			var vm = new IndexViewModel
			{
				Input = input,
				ErrorMessage = errorMessage,
			};
			return vm;
		}

		/// <summary>
		/// ログイン成功向けURL取得
		/// </summary>
		/// <param name="nextUrl">遷移先URL</param>
		/// <param name="currentHost">現在のホスト名</param>
		/// <param name="isSuperUser">スーパーユーザーか</param>
		/// <param name="menuAccessInfo">メニューアクセス情報</param>
		/// <param name="loginExpiredFlg">Login expired flag</param>
		/// <returns>補正済み遷移先URL</returns>
		public string GetUrlForSuccess(
			string nextUrl,
			string currentHost,
			bool isSuperUser,
			MenuAccessInfo menuAccessInfo,
			string loginExpiredFlg = "")
		{
			// 他サイトへ遷移しようとしていたらURL補正（オープンリダイレクトアタック考慮）
			var correcrtedNextUrl = UrlValidator.GetAltUrlIfOtherHostUsed(currentHost, nextUrl, null);
			if (string.IsNullOrEmpty(correcrtedNextUrl) == false)
			{
				if ((loginExpiredFlg != Constants.REQUEST_KEY_MANAGER_LOGIN_EXPIRED_FLG_VALID)
					|| IsSingleSignOnUrl(correcrtedNextUrl)
					|| ManagerMenuCache.Instance.IsSmallMenu(correcrtedNextUrl, menuAccessInfo.LargeMenus))
				{
					return correcrtedNextUrl;
				}
			}

			var url = isSuperUser
				? Constants.COOPERATION_SUPPORT_SITE
					? this.UrlHelper.Action("Index", Constants.CONTROLLER_W2CMS_MANAGER_SUPPORT_INFORMATION)
					: this.UrlHelper.Action("List", Constants.CONTROLLER_W2CMS_MANAGER_OPERATOR)
				: menuAccessInfo.LargeMenus.SelectMany(lm => lm.SmallMenus).First(sm => sm.IsAuthorityDefaultDispPage).Href;
			return url;
		}

		/// <summary>
		/// Is single sign on url
		/// </summary>
		/// <param name="nextUrl">Next url</param>
		/// <returns>True: If this next url is single sign on url</returns>
		private bool IsSingleSignOnUrl(string nextUrl)
		{
			var result = nextUrl.Contains(Constants.CONTROLLER_W2CMS_MANAGER_SINGLE_SIGN_ON);
			return result;
		}

		/// <summary>マネージャログインクラス</summary>
		public IManagerLogin ManagerLogin { get; set; }
	}
}