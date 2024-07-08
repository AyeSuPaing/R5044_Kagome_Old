/*
=========================================================================================================
  Module      : メニュー権限管理リポジトリ (MenuAuthorityRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.MenuAuthority
{
	/// <summary>
	/// メニュー権限管理リポジトリ
	/// </summary>
	internal class MenuAuthorityRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MenuAuthority";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal MenuAuthorityRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MenuAuthorityRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="pkgKbn">パッケージ区分</param>
		/// <param name="menuAuthorityLevel">表示レベル</param>
		/// <returns>モデル</returns>
		internal MenuAuthorityModel[] Get(string shopId, string pkgKbn, int menuAuthorityLevel)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MENUAUTHORITY_SHOP_ID, shopId},
				{Constants.FIELD_MENUAUTHORITY_PKG_KBN, pkgKbn},
				{Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_LEVEL, menuAuthorityLevel},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			return dv.Cast<DataRowView>().Select(drv => new MenuAuthorityModel(drv)).ToArray();
		}
		#endregion

		#region ~PKG区分からメニュー権限をすべて取得
		/// <summary>
		/// PKG区分からメニュー権限をすべて取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="pkgKbn">パッケージ区分</param>
		/// <returns>モデル</returns>
		internal MenuAuthorityModel[] GetAllByPkgKbn(string shopId, string pkgKbn)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MENUAUTHORITY_SHOP_ID, shopId},
				{Constants.FIELD_MENUAUTHORITY_PKG_KBN, pkgKbn},
			};
			var dv = Get(XML_KEY_NAME, "GetAllByPkgKbn", ht);
			return dv.Cast<DataRowView>().Select(drv => new MenuAuthorityModel(drv)).ToArray();
		}
		#endregion

		/// <summary>
		/// 表示レベルから、権限名を取得
		/// </summary>
		/// <param name="menuAuthorityLevel">表示レベル</param>
		/// <returns>権限名</returns>
		internal string GetNameByLevel(int menuAuthorityLevel)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_LEVEL, menuAuthorityLevel }
			};
			var dv = Get(XML_KEY_NAME, "GetNameByLevel", ht);
			var result = (dv.Count != 0)
				? (string)dv[0][Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_NAME]
				: string.Empty;
			return result;
		}

		#region ~メニュー権限情報に設定されていないかチェック
		/// <summary>
		/// メニュー権限情報に設定されていないかチェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="menuAuthorityLevel">メニュー権限レベル</param>
		/// <param name="replaces">メニューアクセスレベル</param>
		/// <returns>true:設定されている false:設定されていない</returns>
		internal bool CheckMenuAuthorityUsed(string shopId, int menuAuthorityLevel, KeyValuePair<string, string>[] replaces)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MENUAUTHORITY_SHOP_ID, shopId},
				{Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_LEVEL, menuAuthorityLevel},
			};
			var dv = Get(XML_KEY_NAME, "CheckMenuAuthorityUsed", ht, replaces: replaces);
			return dv.Count != 0;
		}
		#endregion

		#region ~登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="authorityMenu">メニュー権限</param>
		internal void Insert(MenuAuthorityModel authorityMenu)
		{
			Exec(XML_KEY_NAME, "Insert", authorityMenu.DataSource);
		}
		#endregion

		#region ~削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="pkgKbn">パッケージ区分</param>
		/// <param name="menuAuthorityLevel">メニュー権限レベル</param>
		internal void Delete(string shopId, string pkgKbn, int menuAuthorityLevel)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MENUAUTHORITY_SHOP_ID, shopId},
				{Constants.FIELD_MENUAUTHORITY_PKG_KBN, pkgKbn},
				{Constants.FIELD_MENUAUTHORITY_MENU_AUTHORITY_LEVEL, menuAuthorityLevel},
			};
			Exec(XML_KEY_NAME, "Delete", ht);
		}
		#endregion
	}
}
