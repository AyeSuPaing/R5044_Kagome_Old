/*
=========================================================================================================
  Module      : 共通メニューユーティリティ (CommonMenuUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.ShopOperator;

namespace w2.App.Common.Manager.Menu
{
	/// <summary>
	/// 共通メニューユーティリティ
	/// </summary>
	public class CommonMenuUtility
	{
		/// <summary>
		/// 権限チェック
		/// </summary>
		/// <param name="shopOperator">店舗オペレータ</param>
		/// <param name="managerSiteType">管理サイトタイプ</param>
		/// <param name="pageUrl">ページURL</param>
		/// <returns>権限の有無</returns>
		private static bool HasAuthority(
			ShopOperatorModel shopOperator,
			MenuAuthorityHelper.ManagerSiteType managerSiteType,
			string pageUrl)
		{
			// スーパーユーザー
			if (shopOperator.IsSuperUser(managerSiteType)) return true;

			// メニュー取得
			var menus = shopOperator.GetMenuAuthorities(managerSiteType);

			// ルートパス取得
			var pathRoot = "";
			switch (managerSiteType)
			{
				case MenuAuthorityHelper.ManagerSiteType.Ec:
					pathRoot = Constants.PATH_ROOT_EC;
					break;

				case MenuAuthorityHelper.ManagerSiteType.Mp:
					pathRoot = Constants.PATH_ROOT_MP;
					break;

				case MenuAuthorityHelper.ManagerSiteType.Cs:
					pathRoot = Constants.PATH_ROOT_CS;
					break;

				case MenuAuthorityHelper.ManagerSiteType.Cms:
					pathRoot = Constants.PATH_ROOT_CMS;
					break;
			}

			// 権限チェック
			var find = menus.Any(menu => pageUrl.Contains(pathRoot + menu.MenuPath));
			return find;
		}

		/// <summary>
		/// 権限チェック（EC）
		/// </summary>
		/// <param name="shopOperator">店舗オペレータ</param>
		/// <param name="pageUrl">ページURL</param>
		/// <returns>権限の有無</returns>
		public static bool HasAuthorityEc(ShopOperatorModel shopOperator, string pageUrl)
		{
			return HasAuthority(shopOperator, MenuAuthorityHelper.ManagerSiteType.Ec, pageUrl);
		}

		/// <summary>
		/// 権限チェック（MP）
		/// </summary>
		/// <param name="shopOperator">店舗オペレータ</param>
		/// <param name="pageUrl">ページURL</param>
		/// <returns>権限の有無</returns>
		public static bool HasAuthorityMp(ShopOperatorModel shopOperator, string pageUrl)
		{
			return HasAuthority(shopOperator, MenuAuthorityHelper.ManagerSiteType.Mp, pageUrl);
		}

		/// <summary>
		/// 権限チェック（CS）
		/// </summary>
		/// <param name="shopOperator">店舗オペレータ</param>
		/// <param name="pageUrl">ページURL</param>
		/// <returns>権限の有無</returns>
		public static bool HasAuthorityCs(ShopOperatorModel shopOperator, string pageUrl)
		{
			return HasAuthority(shopOperator, MenuAuthorityHelper.ManagerSiteType.Cs, pageUrl);
		}

		/// <summary>
		/// 権限チェック（CMS）
		/// </summary>
		/// <param name="shopOperator">店舗オペレータ</param>
		/// <param name="pageUrl">ページURL</param>
		/// <returns>権限の有無</returns>
		public static bool HasAuthorityCms(ShopOperatorModel shopOperator, string pageUrl)
		{
			return HasAuthority(shopOperator, MenuAuthorityHelper.ManagerSiteType.Cms, pageUrl);
		}
	}
}