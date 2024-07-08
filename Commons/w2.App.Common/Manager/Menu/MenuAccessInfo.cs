/*
=========================================================================================================
  Module      : メニューアクセス情報 (MenuAccessInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Manager.Menu
{
	/// <summary>
	/// メニューアクセス情報
	/// </summary>
	[Serializable]
	public class MenuAccessInfo
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="largeMenus">大メニュー</param>
		/// <param name="menuAccessLevel">メニューアクセルレベル</param>
		public MenuAccessInfo(MenuLarge[] largeMenus, int? menuAccessLevel)
		{
			this.LargeMenus = largeMenus;
			this.MenuAccessLevel = menuAccessLevel;
		}

		/// <summary>大メニュー</summary>
		public MenuLarge[] LargeMenus { get; private set; }
		/// <summary>メニューアクセルレベル</summary>
		public int? MenuAccessLevel { get; private set; }
	}
}
