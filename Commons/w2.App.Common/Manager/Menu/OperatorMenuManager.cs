/*
=========================================================================================================
  Module      : オペレーターメニューマネージャ(OperatorMenuManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.ShopOperator;

namespace w2.App.Common.Manager.Menu
{
	/// <summary>
	/// オペレーターメニューマネージャ
	/// </summary>
	public class OperatorMenuManager
	{
		/// <summary>スーパーユーザーレベル区分</summary>
		public const int KBN_OPERATOR_LEVEL_SUPERUSER = 0;

		/// <summary>
		/// オペレータアクセス可能メニューリスト取得
		/// </summary>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <param name="shopOperator">オペレータ</param>
		/// <param name="managerMenuCache"></param>
		/// <returns>権限ありメニューリスト</returns>
		public MenuAccessInfo GetOperatorMenuList(
			MenuAuthorityHelper.ManagerSiteType managerSiteType,
			ShopOperatorModel shopOperator,
			IManagerMenuCache managerMenuCache)
		{
			var menuAccessLevel = shopOperator.GetMenuAccessLevel(managerSiteType);
				
			// 権限なしユーザであればなにも返さない
			if (menuAccessLevel.HasValue == false) return new MenuAccessInfo(new MenuLarge[0], null);
			// スーパーユーザーであればすべて返す
			if (menuAccessLevel == KBN_OPERATOR_LEVEL_SUPERUSER) return new MenuAccessInfo(managerMenuCache.MenuList, menuAccessLevel);

			// それ以外は権限取得
			var menuAuthorities = new MenuAuthorityService().Get(
				shopOperator.ShopId,
				managerSiteType,
				menuAccessLevel.Value);

			var lMenus = managerMenuCache.GetAuthorityMenuList(menuAuthorities);
			return new MenuAccessInfo(lMenus, menuAccessLevel);
		}

		/// <summary>
		/// メニューアクセスレベルフィールド名取得
		/// </summary>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <returns>メニューアクセスレベルフィールド名</returns>
		private string GetMenuAccessLevelFiledName(MenuAuthorityHelper.ManagerSiteType managerSiteType)
		{
			var pkgKbn = MenuAuthorityHelper.GetPkgKbn(managerSiteType);
			var filedNamePrefix = Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1.Substring(
				0,
				Constants.FIELD_SHOPOPERATOR_MENU_ACCESS_LEVEL1.Length - 1);
			return filedNamePrefix + pkgKbn;
		}

		/// <summary>
		/// ページが許可された管理画面のメニューに含まれるか
		/// </summary>
		/// <param name="menuAccessLevel">メニューアクセスレベル</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="targetPath">対象パス</param>
		/// <param name="managerType">管理画面種別</param>
		/// <returns></returns>
		public bool CheckPageMenuAuthorityWithManagetType(int? menuAccessLevel, string shopId, string targetPath, MenuAuthorityHelper.ManagerSiteType managerType)
		{
			// 権限なしの場合はfalseを返す
			if (menuAccessLevel.HasValue == false) return false;
			// スーパーユーザーの場合はtrueを返す
			if (menuAccessLevel.Value == OperatorMenuManager.KBN_OPERATOR_LEVEL_SUPERUSER) return true;

			var menuAuthorities = new MenuAuthorityService().Get(
				shopId,
				managerType,
				menuAccessLevel.Value);

			var result = menuAuthorities.Any(menuAuthoritie => targetPath.Contains(menuAuthoritie.MenuPath));
			return result;
		}
	}

}
