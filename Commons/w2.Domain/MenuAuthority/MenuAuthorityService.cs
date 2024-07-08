/*
=========================================================================================================
  Module      : メニュー権限管理サービス (MenuAuthorityService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Domain.MenuAuthority.Helper;

namespace w2.Domain.MenuAuthority
{
	/// <summary>
	/// メニュー権限管理サービス
	/// </summary>
	public class MenuAuthorityService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <param name="menuAuthorityLevel">表示レベル</param>
		/// <returns>モデル</returns>
		public MenuAuthorityModel[] Get(
			string shopId,
			MenuAuthorityHelper.ManagerSiteType managerSiteType,
			int menuAuthorityLevel)
		{
			var pkgKbn = MenuAuthorityHelper.GetPkgKbn(managerSiteType);
			using (var repository = new MenuAuthorityRepository())
			{
				var models = repository.Get(shopId, pkgKbn, menuAuthorityLevel);
				return models;
			}
		}
		#endregion

		#region +GetAllByPkgKbn PKG区分からメニュー権限をすべて取得
		/// <summary>
		/// PKG区分からメニュー権限をすべて取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="managerSiteType">管理画面タイプ</param>
		/// <returns>モデル</returns>
		public MenuAuthorityModel[] GetAllByPkgKbn(string shopId, MenuAuthorityHelper.ManagerSiteType managerSiteType)
		{
			var pkgKbn = MenuAuthorityHelper.GetPkgKbn(managerSiteType);
			using (var repository = new MenuAuthorityRepository())
			{
				var models = repository.GetAllByPkgKbn(shopId, pkgKbn);
				return models;
			}
		}
		#endregion

		/// <summary>
		/// 表示レベルから、権限名を取得
		/// </summary>
		/// <param name="menuAuthorityLevel">表示レベル</param>
		/// <returns>権限名</returns>
		public string GetNameByLevel(int menuAuthorityLevel)
		{
			using (var repository = new MenuAuthorityRepository())
			{
				var result = repository.GetNameByLevel(menuAuthorityLevel);
				return result;
			}
		}

		#region +CheckMenuAuthorityUsed メニュー権限情報に設定されていないかチェック
		/// <summary>
		/// メニュー権限情報に設定されていないかチェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="managerSiteType">パッケージ区分</param>
		/// <param name="menuAuthorityLevel">メニュー権限レベル</param>
		/// <returns>true:設定されている false:設定されていない</returns>
		public bool CheckMenuAuthorityUsed(string shopId, MenuAuthorityHelper.ManagerSiteType managerSiteType, int menuAuthorityLevel)
		{
			using (var repository = new MenuAuthorityRepository())
			{
				var pkgKbn = MenuAuthorityHelper.GetPkgKbn(managerSiteType);
				var replace = new[]
				{
					new KeyValuePair<string, string>("@@ access_level @@", "menu_access_level" + pkgKbn)
				};

				return repository.CheckMenuAuthorityUsed(shopId, menuAuthorityLevel, replace);
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="authorityMenuList">メニュー権限リスト</param>
		/// <param name="authorityLevel">表示レベル</param>
		public void Insert(MenuAuthorityModel[] authorityMenuList, long authorityLevel)
		{
			using (var repository = new MenuAuthorityRepository())
			{
				foreach (var authorityMenu in authorityMenuList)
				{
					authorityMenu.MenuAuthorityLevel = (int)authorityLevel;
					repository.Insert(authorityMenu);
				}
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="authorityMenuList">メニュー権限リスト</param>
		public void Update(MenuAuthorityModel[] authorityMenuList)
		{
			using (var repository = new MenuAuthorityRepository())
			{
				repository.Delete(authorityMenuList[0].ShopId, authorityMenuList[0].PkgKbn, authorityMenuList[0].MenuAuthorityLevel);
				
				foreach (var authorityMenu in authorityMenuList)
				{
					repository.Insert(authorityMenu);
				}
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="managerSiteType">パッケージ区分</param>
		/// <param name="menuAuthorityLevel">メニュー権限レベル</param>
		public void Delete(string shopId, MenuAuthorityHelper.ManagerSiteType managerSiteType, int menuAuthorityLevel)
		{
			using (var repository = new MenuAuthorityRepository())
			{
				var pkgKbn = MenuAuthorityHelper.GetPkgKbn(managerSiteType);
				repository.Delete(shopId, pkgKbn, menuAuthorityLevel);
			}
		}
		#endregion
	}
}
