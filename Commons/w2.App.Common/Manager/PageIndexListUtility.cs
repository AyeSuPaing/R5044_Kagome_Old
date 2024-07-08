/*
=========================================================================================================
  Module      : 機能一覧ユーティリティ(PageIndexListUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace w2.App.Common.Manager.Menu
{
	/// <summary>
	/// 機能一覧ユーティリティ
	/// </summary>
	public class PageIndexListUtility
	{
		/// <summary>機能一覧情報</summary>
		private static PageIndexLargeMenu[] m_pageIndexListMenuList = null;
		/// <summary>機能一覧ファイルの更新日時</summary>
		private static DateTime? m_pageIndexListTimestamp = null;

		/// <summary>
		/// 機能一覧情報取得
		/// </summary>
		/// <param name="pageIndexXmlPath">機能一覧設定ファイルパス</param>
		/// <param name="key">大メニューキー名</param>
		/// <param name="lmenus">大メニュー情報リスト定義</param>
		/// <returns>指定大メニューの機能一覧情報</returns>
		public static PageIndexLargeMenu GetPageIndexList(
			string pageIndexXmlPath,
			string key,
			IEnumerable<MenuLarge> lmenus)
		{
			if ((m_pageIndexListMenuList == null)
				|| (m_pageIndexListTimestamp.GetValueOrDefault(DateTime.MinValue) != File.GetLastWriteTime(pageIndexXmlPath)))
			{
				m_pageIndexListMenuList = ReadPageIndexList(pageIndexXmlPath, lmenus);
				m_pageIndexListTimestamp = File.GetLastWriteTime(pageIndexXmlPath);
			}
			var targetIndexList = m_pageIndexListMenuList.FirstOrDefault(pageIndexLMenu => pageIndexLMenu.Key == key);
			return targetIndexList;
		}

		/// <summary>
		/// 機能一覧情報読み込み
		/// </summary>
		/// <param name="xmlPath">XMLパス</param>
		/// <param name="lmenus">大メニュー情報リスト定義</param>
		/// <returns>機能一覧情報</returns>
		private static PageIndexLargeMenu[] ReadPageIndexList(string xmlPath, IEnumerable<MenuLarge> lmenus)
		{
			var pageIndexLargeMenus = XDocument.Load(xmlPath).Root
				.Elements("lmenu")
				.Select(element => new PageIndexLargeMenu(element))
				.ToArray();
			// hrefはmenu.xmlから取得する
			foreach (var pageIndexLargeMenu in pageIndexLargeMenus)
			{
				foreach (var pageIndexSmallMenu in pageIndexLargeMenu.PageIndexSmallMenuCategories
					.SelectMany(smc => smc.PageIndexSmallMenus))
				{
					var lmTmp = lmenus.FirstOrDefault(lm => lm.Key == pageIndexLargeMenu.Key);
					if (lmTmp == null) continue;

					var smTmp = lmTmp.SmallMenus.FirstOrDefault(sm => sm.Name == pageIndexSmallMenu.Name);
					if (smTmp == null) continue;

					pageIndexSmallMenu.Href = smTmp.Href;
				}
			}
			return pageIndexLargeMenus;
		}

		#region 機能一覧大メニュー
		/// <summary>
		/// 機能一覧大メニュー
		/// </summary>
		public class PageIndexLargeMenu
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="menuLarge">大メニュー</param>
			public PageIndexLargeMenu(XElement menuLarge)
			{
				this.Name = menuLarge.Attribute("name").Value;
				this.Key = menuLarge.Attribute("key").Value;
				this.PageIndexSmallMenuCategories = menuLarge.Elements("smenucat")
					.Select(smenu => new PageIndexSmallMenuCategory(smenu)).ToArray();
			}

			/// <summary>メニュー名</summary>
			public string Name { get; private set; }
			/// <summary>メニューキー名（大メニューのキー）</summary>
			public string Key { get; private set; }
			/// <summary>小メニューリスト</summary>
			public PageIndexSmallMenuCategory[] PageIndexSmallMenuCategories { get; set; }
		}
		#endregion

		#region 機能一覧小メニューカテゴリ
		/// <summary>
		/// 機能一覧小メニューカテゴリ
		/// </summary>
		public class PageIndexSmallMenuCategory
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="menuSmall">小メニュー</param>
			public PageIndexSmallMenuCategory(XElement menuSmall)
			{
				this.Name = menuSmall.Attribute("name").Value;
				this.PageIndexSmallMenus = menuSmall.Elements("smenu")
					.Select(element => new PageIndexSmallMenu(element)).ToArray();
			}

			/// <summary>中メニュー名</summary>
			public string Name { get; private set; }
			/// <summary>機能情報リスト</summary>
			public PageIndexSmallMenu[] PageIndexSmallMenus { get; set; }
		}
		#endregion

		#region 機能一覧小メニュー
		/// <summary>
		/// 機能一覧小メニュー
		/// </summary>
		public class PageIndexSmallMenu
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="smenu">小メニュー情報</param>
			public PageIndexSmallMenu(XElement smenu)
			{
				this.Name = smenu.Attribute("name").Value;
				this.SubText = smenu.Value.Trim();
			}

			/// <summary>リンク（後でセットする）</summary>
			public string Href { get; set; }
			/// <summary>機能名</summary>
			public string Name { get; set; }
			/// <summary>説明文</summary>
			public string SubText { get; private set; }
		}
		#endregion
	}
}