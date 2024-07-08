/*
=========================================================================================================
  Module      :管理画面メニューキャッシュ基底クラス(ManagerMenuCacheBase.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using w2.Domain.MenuAuthority;

namespace w2.App.Common.Manager.Menu
{
	/// <summary>
	/// 管理画面メニューキャッシュ基底クラス
	/// </summary>
	public abstract class ManagerMenuCacheBase<T> : IManagerMenuCache
		where T : class, IManagerMenuCache, new()
	{
		/// <summary>オプション情報</summary>
		private Dictionary<string, bool> m_optionInfo = new Dictionary<string, bool>();
		/// <summary>メニューファイル更新日</summary>
		private DateTime m_dtFileUpdate = DateTime.MinValue;

		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static ManagerMenuCacheBase()
		{
			Instance = new T();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected ManagerMenuCacheBase()
		{
			ReloadMenuList();
		}

		/// <summary>
		/// メニューリストリフレッシュ
		/// </summary>
		/// <returns>更新したか</returns>
		public void RefreshMenuList()
		{
			// メニューリスト読み込み
			ReloadMenuList();

			// ファイル更新日設定
			m_dtFileUpdate = File.GetLastWriteTime(Constants.PHYSICALDIRPATH_MANAGER_MENU_XML);
		}

		/// <summary>
		/// メニューリスト再読み込み
		/// </summary>
		private void ReloadMenuList()
		{
			// オプションメニュー用の設定を行う
			m_optionInfo = GetOptionStatus();

			// XML読み込み
			this.MenuList = ReadMenuXml(Constants.PHYSICALDIRPATH_MANAGER_MENU_XML);
		}

		/// <summary>
		/// メニューXMLを読み込み、メニュー配列構築
		/// </summary>
		/// <param name="menuXmlFilePath">メニューXMLファイルパス</param>
		/// <returns>メニューリスト</returns>
		private MenuLarge[] ReadMenuXml(string menuXmlFilePath)
		{
			var menuAll = XDocument.Load(menuXmlFilePath).Root;
			var largeMenus = menuAll.Elements().Select(lme => 
					new MenuLarge(
						lme,
						this.PageIndexListUrl,
						Constants.REQUEST_KEY_PAGE_INDEX_LIST_KEY,
						m_optionInfo))
				.Where(lm => lm.SmallMenus.Length > 0)
				.Where(lm => lm.CheckOption(m_optionInfo)).ToArray();
			return largeMenus;
		}

		/// <summary>
		/// オプションステータス取得
		/// </summary>
		/// <returns>メニュー有効状態</returns>
		protected abstract Dictionary<string, bool> GetOptionStatus();

		/// <summary>
		/// 権限ありメニューリスト取得
		/// </summary>
		/// <param name="menuAuthorities">メニュー権限</param>
		/// <returns>メニューリスト(選択メニュー情報、機能レベル情報)</returns>
		public MenuLarge[] GetAuthorityMenuList(MenuAuthorityModel[] menuAuthorities)
		{
			// 検索しやすいようにDictinary格納
			var menuAuthorityInfoList = menuAuthorities.ToDictionary(ma => ma.MenuPath, ma => ma);

			var result = GetAuthorityMenuList(menuAuthorityInfoList);
			return result;
		}
		/// <summary>
		/// 権限ありメニューリスト取得
		/// </summary>
		/// <param name="menuAuthorityInfoList">メニュー権限リスト</param>
		/// <returns>メニューリスト(選択メニュー情報、機能レベル情報)</returns>
		public MenuLarge[] GetAuthorityMenuList(Dictionary<string, MenuAuthorityModel> menuAuthorityInfoList)
		{
			// メニュー再読み込み
			RefreshMenuList();

			// 権限ありメニュー構築
			var menuLargeTmps = new List<MenuLarge>();
			foreach (var largeMenu in this.MenuList)
			{
				var menuLargeTmp = (MenuLarge)largeMenu.Clone();
				var menuSmallTmps = new List<MenuSmall>();

				foreach (var smallMenu in largeMenu.SmallMenus)
				{
					if (menuAuthorityInfoList.ContainsKey(smallMenu.MenuPath) == false) continue;

					var mai = menuAuthorityInfoList[smallMenu.MenuPath];
					var menusmall = (MenuSmall)smallMenu.Clone();
					menusmall.AuthorityFunctionLevel = mai.FunctionLevel;
					menusmall.IsAuthorityDefaultDispPage = (mai.DefaultDispFlg == Constants.FLG_MENUAUTHORITY_DEFAULT_DISP_FLG_ON);

					var functionTmps = new List<MenuFunction>();
					foreach (var mf in menusmall.Functions)
					{
						if ((mf.Level & menusmall.AuthorityFunctionLevel) != 0) functionTmps.Add(mf);
					}

					menusmall.Functions = functionTmps.ToArray();
					menuSmallTmps.Add(menusmall);
				}

				menuLargeTmp.SmallMenus = menuSmallTmps.ToArray();
				if (menuLargeTmp.SmallMenus.Length != 0) menuLargeTmps.Add(menuLargeTmp);
			}

			return menuLargeTmps.ToArray();
		}

		/// <summary>
		/// 権限があるか　※ここでいいのか
		/// </summary>
		/// <param name="loginOperatorMenu">ログインオペレーターメニュー</param>
		/// <param name="pageUrl">現ページURL</param>
		/// <param name="functionLevel">チェックする権限</param>
		public bool HasAuthority(List<MenuLarge> loginOperatorMenu, string pageUrl, int functionLevel)
		{
			var find = loginOperatorMenu.Any(lm => lm.SmallMenus.Any(sm => 
				(pageUrl.Contains(Constants.PATH_ROOT + sm.MenuPath)
				&& ((functionLevel & sm.AuthorityFunctionLevel) != 0))));
			return find;
		}

		/// <summary>
		/// 比較対象のメニュー権限が包含されている事を確認（大メニュー）
		/// </summary>
		/// <param name="comparedAuthorities">比較対象のメニュー権限</param>
		/// <param name="loginAuthorities">現在のメニュー権限</param>
		/// <returns>比較対象が現在の権限に全て含まれている場合にTrue それ以外False</returns>
		public bool HasOperatorMenuAuthority(IEnumerable<MenuLarge> comparedAuthorities, IEnumerable<MenuLarge> loginAuthorities)
		{
			var find = comparedAuthorities.All(ca =>
				loginAuthorities.Any(
					menuLarge => ((menuLarge.Name == ca.Name)
						&& HasOperatorSmallMenuAuthority(menuLarge.SmallMenus, ca.SmallMenus))));
			return find;
		}

		/// <summary>
		/// 比較対象のメニュー権限が包含されている事を確認（小メニュー）
		/// </summary>
		/// <param name="loginAuthorities">現在のメニュー権限</param>
		/// <param name="comparedAuthorities">比較対象のメニュー権限</param>
		/// <returns>比較対象が現在の権限に全て含まれている場合にTrue それ以外False</returns>
		private bool HasOperatorSmallMenuAuthority(MenuSmall[] loginAuthorities, MenuSmall[] comparedAuthorities)
		{
			var find = comparedAuthorities.All(
				ca => (loginAuthorities.Any(
					menuSmall => ((menuSmall.MenuPath == ca.MenuPath)
						&& HasOperatorFunctionAuthority(menuSmall.Functions, ca.Functions)))));
			return find;
		}

		/// <summary>
		/// 比較対象のメニュー権限が包含されている事を確認（機能単位）
		/// </summary>
		/// <param name="loginAuthorityFunctions">現在のメニュー権限</param>
		/// <param name="comparedAuthorityFunctions">比較対象のメニュー権限</param>
		/// <returns>比較対象が現在の権限に全て含まれている場合にTrue それ以外False</returns>
		private bool HasOperatorFunctionAuthority(
			MenuFunction[] loginAuthorityFunctions,
			MenuFunction[] comparedAuthorityFunctions)
		{
			var find = comparedAuthorityFunctions.All(
				caf => (loginAuthorityFunctions.Any(lmf => lmf.UniqueId == caf.UniqueId)));
			return find;
		}

		/// <summary>
		/// 指オペレータは比較対象URLのメニュー権限を持っているか
		/// </summary>
		/// <param name="url">MenuLargeのURL</param>
		/// <returns>true:含む,false:含まない</returns>
		public bool HasOperatorMenuAuthority(string url)
		{
			// セッションからオペレータメニューを取得
			var operatorMenuLarges = this.LoginOperatorMenus ?? new List<MenuLarge>();

			var result = HasOperatorMenuAuthority(url, operatorMenuLarges);
			return result;
		}
		/// <summary>
		/// オペレータは比較対象URLのメニュー権限を持っているか
		/// </summary>
		/// <param name="menuLargeUrl">MenuLargeのURL</param>
		/// <param name="loginAuthorities">現在のメニュー権限</param>
		/// <returns>true:含む,false:含まない</returns>
		public bool HasOperatorMenuAuthority(string menuLargeUrl, IEnumerable<MenuLarge> loginAuthorities)
		{
			var loginOperatorMenuSmalls = loginAuthorities.SelectMany(lm => lm.SmallMenus).ToArray();
			var contains = loginOperatorMenuSmalls.Any(sm => menuLargeUrl.Contains(Constants.PATH_ROOT + sm.MenuPath));
			return contains;
		}

		/// <summary>
		///  タイトル取得
		/// </summary>
		/// <param name="filePath">仮想パス</param>
		/// <returns>タイトル</returns>
		public string GetTitle(string filePath)
		{
			var filePathTmp = filePath + (filePath.EndsWith("/") ? "" : "/");
			var targetMenu = this.MenuList
				.SelectMany(lm => lm.SmallMenus.Where(sm => ContainsIgonoreCase(filePathTmp, sm.MenuPath.ToLower())))
				.FirstOrDefault();
			return (targetMenu != null) ? targetMenu.Name : "";
		}

		/// <summary>
		/// srcはdestを含むか（大文字小文字区別しない）
		/// </summary>
		/// <param name="src">検査対象</param>
		/// <param name="dest">含むかを調べる文字</param>
		/// <returns>含むか</returns>
		private bool ContainsIgonoreCase(string src, string dest)
		{
			return src.ToLower().Contains(dest.ToLower());
		}

		/// <summary>
		/// Is small menu
		/// </summary>
		/// <param name="url">Url</param>
		/// <param name="loginAuthorities">Login authorities</param>
		/// <returns>True: If the url is small menu</returns>
		public bool IsSmallMenu(string url, IEnumerable<MenuLarge> loginAuthorities)
		{
			// Remove all parameters
			var urlTmp = (url.IndexOf('?') != -1)
				? url.Split('?')[0]
				: url;
			var loginOperatorMenuSmalls = loginAuthorities.SelectMany(lm => lm.SmallMenus);
			var result = loginOperatorMenuSmalls.Any(sm => sm.Href.Contains(urlTmp));
			return result;
		}

		/// <summary>メニューリスト</summary>
		public abstract string PageIndexListUrl { get; }
		/// <summary>メニューリスト</summary>
		public MenuLarge[] MenuList { get; set; }
		/// <summary>インスタンス</summary>
		public static T Instance { get; protected set; }
		/// <summary>ログインオペレータメニュー</summary>
		public abstract IEnumerable<MenuLarge> LoginOperatorMenus { get; }
	}
}
