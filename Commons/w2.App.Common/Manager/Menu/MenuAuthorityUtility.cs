/*
=========================================================================================================
  Module      : メニュー権限ユーティリティ(MenuAuthorityUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common;
using w2.App.Common.Manager.Menu;
using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;

/// <summary>
/// メニューユーティリティ
/// </summary>
public class MenuAuthorityUtility : CommonMenuUtility
{
	/// <summary>
	/// メニュー権限一覧リストアイテム作成
	/// </summary>
	/// <param name="shopId">店舗ID</param>
	/// <param name="menuAccessLevel">権限レベル</param>
	/// <param name="managerSiteType">管理画面</param>>
	/// <returns>リストアイテム配列</returns>
	public static ListItem[] CreateMenuAuthorityList(string shopId, string menuAccessLevel, MenuAuthorityHelper.ManagerSiteType managerSiteType)
	{
		var menuAuthorityListItem = new MenuAuthorityService().GetAllByPkgKbn(shopId, managerSiteType);
		var menuAuthorities = new List<ListItem>
		{
			new ListItem
			{
				Text = Constants.FLG_UNACCESSABLEUSER_NAME,
				Value = Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER
			}
		};
		if (menuAccessLevel == Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER)
		{
			menuAuthorities.Add(
				new ListItem(
					Constants.FLG_SUPERUSER_NAME,
					Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_SUPERUSER));
		}
		var menuAuthority = menuAuthorityListItem
			.Select(me => new ListItem(me.MenuAuthorityName, me.MenuAuthorityLevel.ToString()))
			.ToList();
		menuAuthorities.AddRange(menuAuthority);
		var menuAccesses = menuAuthorities
			.Select(ma => (ma.Value == Constants.FLG_MENUAUTHORITY_MENU_AUTHORITY_LEVEL_UNACCESSABLEUSER)
				? new ListItem(ma.Text, Constants.FLG_NO_AUTHORITY_VALUE)
				: ma).ToArray();

		return menuAccesses;
	}
}