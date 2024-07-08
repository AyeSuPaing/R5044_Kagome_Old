/*
=========================================================================================================
  Module      : メニューユーティリティ(MenuUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using System.Web;
using w2.App.Common.Manager.Menu;
using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.ShopOperator;

/// <summary>
/// メニューユーティリティ
/// </summary>
public class MenuUtility : CommonMenuUtility
{
	/// <summary>
	/// 小メニュー検索
	/// </summary>
	/// <param name="loginMenus">ログインメニュー権限</param>
	/// <param name="pageUrl">現ページURL</param>
	/// <param name="functionLevel">チェックする権限</param>
	public static bool HasAuthority(List<MenuLarge> loginMenus, string pageUrl, int functionLevel)
	{
		return ManagerMenuCache.Instance.HasAuthority(loginMenus, pageUrl, functionLevel);
	}

	/// <summary>
	///  タイトル取得
	/// </summary>
	/// <param name="filePath">仮想パス</param>
	/// <returns>タイトル</returns>
	public static string GetTitle(string filePath)
	{
		if (filePath.ToLower().Contains(Constants.PAGE_MANAGER_ERROR.ToLower())) return "エラー情報";

		var title = ManagerMenuCache.Instance.GetTitle(filePath);
		return title;
	}

	/// <summary>
	/// オペレータは比較対象URLのメニュー権限を持っているか
	/// </summary>
	/// <param name="menuLargeUrl">MenuLargeのURL</param>
	/// <returns>true:含む,false:含まない</returns>
	public static bool HasOperatorMenuAuthority(string menuLargeUrl)
	{
		var result = ManagerMenuCache.Instance.HasOperatorMenuAuthority(menuLargeUrl, LoginAuthorities);
		return result;
	}

	/// <summary>ログインオペレータメニュー</summary>
	public static List<MenuLarge> LoginAuthorities
	{
		get
		{
			return (List<MenuLarge>)(HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU]
				?? new List<MenuLarge>());
		}
	}
}
