/*
=========================================================================================================
  Module      : 機能一覧ビューモデル(PageIndexViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.App.Common.Manager.Menu;

namespace w2.Cms.Manager.ViewModels.PageIndexList
{
	/// <summary>
	/// 機能一覧ビューモデル
	/// </summary>
	public class PageIndexViewModel : ViewModelBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="pageIndexLargeMenu">機能一覧大メニュー</param>
		/// <param name="key">キー</param>
		/// <param name="loginOperatorMenus">ログインオペレータメニューリスト</param>
		public PageIndexViewModel(
			PageIndexListUtility.PageIndexLargeMenu pageIndexLargeMenu,
			string key,
			MenuLarge[] loginOperatorMenus)
		{
			this.PageIndexLargeMenu = pageIndexLargeMenu;
			this.Key = key;
			this.LoginOperatorMenus = loginOperatorMenus;
		}

		/// <summary>
		/// 利用可能な機能の抽出
		/// </summary>
		/// <param name="pageIndexSmallMenus">機能情報リスト</param>
		/// <returns>利用可能な機能</returns>
		public PageIndexListUtility.PageIndexSmallMenu[] GetAuthorizedPages(
			PageIndexListUtility.PageIndexSmallMenu[] pageIndexSmallMenus)
		{
			var authorizedPage = pageIndexSmallMenus.Where(
				pageIndexSmallMenu => this.LoginOperatorMenus.First(lmenu => lmenu.Key == this.Key).SmallMenus
					.Any(smenu => smenu.Name == pageIndexSmallMenu.Name)).ToArray();
			return authorizedPage;
		}

		/// <summary>大メニュー</summary>
		public PageIndexListUtility.PageIndexLargeMenu PageIndexLargeMenu { get; private set; }
		/// <summary>キー</summary>
		public string Key { get; private set; }
		/// <summary>ログインオペレータメニュー</summary>
		private MenuLarge[] LoginOperatorMenus { get; set; }
	}
}