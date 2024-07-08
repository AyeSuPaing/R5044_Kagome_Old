/*
=========================================================================================================
  Module      :  シングルサインオン情報作成(SingleSignOnInfoCreator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.ShopOperator;

namespace w2.App.Common.Manager
{
	/// <summary>
	/// シングルサインオン情報作成
	/// </summary>
	public class SingleSignOnInfoCreator
	{
		/// <summary>エラー区分</summary>
		public enum ErrorKbn
		{
			/// <summary>エラーなし</summary>
			NoError,
			/// <summary>ログインエラー</summary>
			LoginError,
			/// <summary>ログイン試行回数エラー</summary>
			LoginLimitedCountError,
			/// <summary>権限なし</summary>
			OperatorUnaccessable,
			/// <summary>ログインなし</summary>
			UnloggedInError,
		}

		/// <summary>
		/// 作成
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <returns>シングルサインオン情報</returns>
		public SingleSignOnInfo Create(string shopId, string operatorId, MenuAuthorityHelper.ManagerSiteType managerSiteType)
		{
			// オペレータ情報取得・エラー判定
			var shopOperator = new ShopOperatorService().Get(shopId, operatorId);
			var errorKbn = ErrorKbn.NoError;
			if (shopOperator == null)
			{
				// 存在しない場合はログインエラー
				errorKbn = ErrorKbn.LoginError;
			}
			else if (shopOperator.ValidFlgOn == false)
			{
				// 有効フラグが無効の場合はログインカウントエラー
				errorKbn = ErrorKbn.LoginLimitedCountError;
			}
			else
			{
				// スーパーユーザー以外で、アクセス権限なしorメニュー権限が存在しない場合はエラー
				var menuAuthorityLevel = shopOperator.GetMenuAccessLevel(managerSiteType);

				var isAccable = false;
				if (menuAuthorityLevel.HasValue)
				{
					if (menuAuthorityLevel == int.Parse(Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER))
					{
						isAccable = true;
					}
					else
					{
						var menus = new MenuAuthorityService().Get(shopId, managerSiteType, menuAuthorityLevel.Value);
						isAccable = (menus.Length > 0);
					}
				}

				if (isAccable == false)
				{
					errorKbn = ErrorKbn.OperatorUnaccessable;
				}
			}

			return new SingleSignOnInfo(shopOperator, CreateLoginPagePath(managerSiteType), errorKbn);
		}

		/// <summary>
		/// ログインページパス作成
		/// </summary>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <returns>ページパス</returns>
		private string CreateLoginPagePath(MenuAuthorityHelper.ManagerSiteType managerSiteType)
		{
			switch (managerSiteType)
			{
				case MenuAuthorityHelper.ManagerSiteType.Ec:
					return Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_LOGIN;

				case MenuAuthorityHelper.ManagerSiteType.Mp:
					return Constants.PATH_ROOT_MP + Constants.PAGE_W2MP_MANAGER_LOGIN;

				case MenuAuthorityHelper.ManagerSiteType.Cs:
					return Constants.PATH_ROOT_CS + Constants.PAGE_W2CS_MANAGER_LOGIN;

				case MenuAuthorityHelper.ManagerSiteType.Cms:
					return Constants.PATH_ROOT_CMS;
			}
			return "";
		}

		/// <summary>
		/// シングルサインオン情報
		/// </summary>
		public class SingleSignOnInfo
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="shopOperator">店舗オペレータ</param>
			/// <param name="loginPageUrl">ログインページURL</param>
			/// <param name="errorKbn">エラー区分</param>
			public SingleSignOnInfo(ShopOperatorModel shopOperator, string loginPageUrl, ErrorKbn errorKbn)
			{
				this.ShopOperator = shopOperator;
				this.LoginPageUrl = loginPageUrl;
				this.ErrorKbn = errorKbn;
			}

			/// <summary>店舗オペレータ</summary>
			public ShopOperatorModel ShopOperator { get; private set; }
			/// <summary>ログインページURL</summary>
			public string LoginPageUrl { get; private set; }
			/// <summary>エラー区分</summary>
			public ErrorKbn ErrorKbn { get; private set; }
		}
	}
}
