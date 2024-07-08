/*
=========================================================================================================
  Module      : 小メニュー (MenuSmall.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using w2.Common.Helper;
using w2.Domain.MenuAuthority.Helper;

namespace w2.App.Common.Manager.Menu
{
	/// <summary>
	/// 小メニュー
	/// </summary>
	[Serializable]
	public class MenuSmall : MenuBase
	{
		/// <summary>menu.xmlの内部パラメタ</summary>
		public const string PARAM_XML_PAGEPATH = "@@pagepath@@";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="smallMenuElement">小メニューエレメント</param>
		/// <param name="optionInfo">オプション情報</param>
		public MenuSmall(XElement smallMenuElement, Dictionary<string, bool> optionInfo)
			: base(smallMenuElement)
		{
			this.TopPage = smallMenuElement.Attribute("top").Value;
			this.MenuPath = smallMenuElement.Attribute("path").Value;
			this.Site = (smallMenuElement.Attribute("site") != null) ? smallMenuElement.Attribute("site").Value : "";
			if ((this.Site != null)
				&& (this.Site.ToUpper() == MenuAuthorityHelper.ManagerSiteType.Cms.ToText()))
			{
				this.Href = SingleSignOnUrlCreator.CreateForWebForms(
					MenuAuthorityHelper.ManagerSiteType.Cms,
					Constants.PATH_ROOT_CMS + this.MenuPath + this.TopPage);
			}
			else
			{
				this.Href = Constants.PATH_ROOT + this.MenuPath + this.TopPage;
			}

			this.AuthorityFunctionLevel = 0xffff;
			this.Functions = smallMenuElement.Elements().Select(f => new MenuFunction(f)).Where(f => f.CheckOption(optionInfo)).ToArray();
			var descriptionElement = smallMenuElement.Elements().FirstOrDefault(e => e.Name == "description");
			this.Description = (descriptionElement != null) ? descriptionElement.Value.Trim() : null;
		}

		/// <summary>トップページ名</summary>
		public string TopPage { get; set; }
		/// <summary>メニューパス</summary>
		public string MenuPath { get; set; }
		/// <summary>サイト</summary>
		public string Href { get; set; }
		/// <summary>アンカー遷移先</summary>
		public string Site { get; set; }
		/// <summary>機能レベル情報</summary>
		public MenuFunction[] Functions { get; set; }
		/// <summary>説明</summary>
		public string Description { get; set; }
		/// <summary>権限デフォルト表示ページか</summary>
		public bool IsAuthorityDefaultDispPage { get; set; }
		/// <summary>権限機能レベル（ビット管理）</summary>
		public int AuthorityFunctionLevel { get; set; }
	}
}
