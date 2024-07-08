/*
=========================================================================================================
  Module      : 管理画面メニューキャッシュ(ManagerMenuCache.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web;
using w2.App.Common.Manager.Menu;

/// <summary>
/// 管理画面メニューキャッシュ
/// </summary>
public class ManagerMenuCache : ManagerMenuCacheBase<ManagerMenuCache>
{
	/// <summary>
	/// オプションステータス取得
	/// </summary>
	/// <returns>メニュー有効状態</returns>
	protected override Dictionary<string, bool> GetOptionStatus()
	{
		return new Dictionary<string, bool>
		{
			{"support", Constants.COOPERATION_SUPPORT_SITE},	// サポートサイト用
			{
				"data_migration_setting",
				(Constants.DATAMIGRATION_OPTION_ENABLED
					&& (DateTime.Now <= Constants.DATAMIGRATION_END_DATETIME))
			}, // データ移行設定
		};
	}

	/// <summary>機能一覧URL</summary>
	public override string PageIndexListUrl
	{
		get { return Constants.PATH_ROOT + Constants.PAGE_MANAGER_PAGE_INDEX_LIST; }
	}
	/// <summary>ログインオペレータメニュー</summary>
	public override IEnumerable<MenuLarge> LoginOperatorMenus
	{
		get { return (List<MenuLarge>)HttpContext.Current.Session[Constants.SESSION_KEY_LOGIN_OPERTOR_MENU]; }
	}
}
