/*
=========================================================================================================
  Module      : メニュー権限ヘルパ (MenuAuthorityHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace w2.App.Common.Manager.Menu
{
	/// <summary>
	/// メニュー基底クラス
	/// </summary>
	[Serializable]
	public abstract class MenuBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="menuElement">メニューエレメント</param>
		protected MenuBase(XElement menuElement)
			: this((menuElement.Attribute("name") != null) ?  menuElement.Attribute("name").Value : null)
		{
			this.Options = (menuElement.Attribute("option") != null)
				? menuElement.Attribute("option").Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
				: new string[0];
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name"></param>
		protected MenuBase(string name)
		{
			this.Name = name;
		}

		/// <summary>
		/// オプション属性部分の確認を行う
		/// </summary>
		/// <param name="optionInfo">オプション情報</param>
		/// <returns>メニューが有効か</returns>
		public bool CheckOption(Dictionary<string, bool> optionInfo)
		{
			if (this.Options.Length == 0) return true;
			var result = this.Options.Any(op => optionInfo.ContainsKey(op) && optionInfo[op]);
			return result;
		}

		/// <summary>
		/// Clone()
		/// </summary>
		/// <returns>クローン</returns>
		public MenuBase Clone()
		{
			return (MenuBase)MemberwiseClone();
		}

		/// <summary>メニュー名</summary>
		public string Name { get; set; }
		/// <summary>機能一覧画面遷移URL</summary>
		public string PageIndexListUrl { get; set; }
		/// <summary>オプション</summary>
		public string[] Options { get; set; }
	}
}
