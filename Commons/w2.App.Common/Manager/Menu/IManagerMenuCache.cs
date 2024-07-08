/*
=========================================================================================================
  Module      :管理画面メニューキャッシュインターフェース(IManagerMenuCache.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using w2.Domain.MenuAuthority;

namespace w2.App.Common.Manager.Menu
{
	/// <summary>
	/// 管理画面メニューキャッシュインターフェース
	/// </summary>
	public interface IManagerMenuCache
	{
		/// <summary>
		/// 権限ありメニューリスト取得
		/// </summary>
		/// <param name="menuAuthorities">メニュー権限</param>
		/// <returns>メニューリスト(選択メニュー情報、機能レベル情報)</returns>
		MenuLarge[] GetAuthorityMenuList(MenuAuthorityModel[] menuAuthorities);
		/// <summary>メニューリスト</summary>
		MenuLarge[] MenuList { get; set; }
	}
}
