/*
=========================================================================================================
  Module      : シングルサインオンワーカーサービス(SingleSignOnWorkerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common;
using w2.App.Common.Manager;
using w2.Cms.Manager.Codes.Common;
using w2.Cms.Manager.ViewModels.SingleSignOn;
using w2.Domain.MenuAuthority.Helper;

namespace w2.Cms.Manager.WorkerServices
{
	/// <summary>
	/// シングルサインオンワーカーサービス
	/// </summary>
	public class SingleSignOnWorkerService : BaseWorkerService
	{
		/// <summary>
		/// シングルサインオン情報作成
		/// </summary>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <param name="nextUrl">遷移先URL</param>
		/// <returns>ビューモデル</returns>
		public IndexViewModel CreateSingleSignOnInfo(
			MenuAuthorityHelper.ManagerSiteType managerSiteType,
			string nextUrl = null)
		{
			// セッション切れの場合、未ログインエラーとする
			if (this.SessionWrapper.IsLoggedIn == false)
			{
				var vm2 = new IndexViewModel
				{
					LoginUrl = Constants.PATH_ROOT_CMS,
					NextUrl = nextUrl,
					ErrorMessage = GetErrorMessage(SingleSignOnInfoCreator.ErrorKbn.UnloggedInError),
				};

				return vm2;
			}

			var info = new SingleSignOnInfoCreator().Create(
				this.SessionWrapper.LoginOperator.ShopId,
				this.SessionWrapper.LoginOperator.OperatorId,
				managerSiteType);

			var vm = new IndexViewModel
			{
				ShopOperator = info.ShopOperator,
				LoginUrl = info.LoginPageUrl,
				NextUrl = nextUrl,
				ErrorMessage = GetErrorMessage(info.ErrorKbn),
			};

			return vm;
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="errorKbn">エラー区分</param>
		/// <returns>エラーメッセージ</returns>
		public string GetErrorMessage(SingleSignOnInfoCreator.ErrorKbn errorKbn)
		{
			switch (errorKbn)
			{
				case SingleSignOnInfoCreator.ErrorKbn.LoginError:
					return WebMessages.ShopOperatorLoginError;

				case SingleSignOnInfoCreator.ErrorKbn.LoginLimitedCountError:
					return WebMessages.ShopOperatorLoginLimitedCountError;

				case SingleSignOnInfoCreator.ErrorKbn.OperatorUnaccessable:
					return WebMessages.ShopOperatorUnaccessable;

				case SingleSignOnInfoCreator.ErrorKbn.UnloggedInError:
					return WebMessages.ShopOperatorUnloggedIn;
			}
			return "";
		}
	}
}