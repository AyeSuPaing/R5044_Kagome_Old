/*
=========================================================================================================
  Module      : 大メニュークラス (MenuLarge.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using w2.Common.Web;

namespace w2.App.Common.Manager.Menu
{
	/// <summary>
	/// 大メニュークラス
	/// </summary>
	[Serializable]
	public class MenuLarge : MenuBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="largeMenuElement">大メニュー要素</param>
		/// <param name="pageIndexListUrlPath">ページバスインデックスリストURLパス</param>
		/// <param name="pageIndexListKey">ページインデックスリストキー</param>
		/// <param name="optionInfo">オプション情報</param>
		public MenuLarge(
			XElement largeMenuElement,
			string pageIndexListUrlPath,
			string pageIndexListKey,
			Dictionary<string, bool> optionInfo)
				: base(largeMenuElement)
		{
			this.IconCss = largeMenuElement.Attribute("icon_css").Value;
			this.Key = (largeMenuElement.Attribute("key") != null) ? largeMenuElement.Attribute("key").Value : "";
			this.PageIndexListUrl = new UrlCreator(pageIndexListUrlPath)
				.AddParam(pageIndexListKey, this.Key).CreateUrl();
			this.SmallMenus = largeMenuElement.Elements().Select(sm => new MenuSmall(sm, optionInfo))
				.Where(sm => sm.CheckOption(optionInfo)).ToArray();
			if (largeMenuElement.Attribute("operation") != null)
			{
				this.IsCsOperation = bool.Parse(largeMenuElement.Attribute("operation").Value);
			}
		}

		/// <summary>アイコン名</summary>
		public string IconCss { get; set; }
		/// <summary>小メニュー</summary>
		public MenuSmall[] SmallMenus { get; set; }
		/// <summary>メニューキー名</summary>
		public string Key { get; set; }
		/// <summary>CSオペレーションメニューか</summary>
		public bool IsCsOperation { get; set; }
	}
}
