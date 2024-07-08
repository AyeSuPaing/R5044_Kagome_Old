/*
=========================================================================================================
  Module      : メニューファンクション (MenuFunction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Xml.Linq;

namespace w2.App.Common.Manager.Menu
{
	/// <summary>
	/// メニューファンクション
	/// </summary>
	[Serializable]
	public class MenuFunction : MenuBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="xeFunction"></param>
		public MenuFunction(XElement xeFunction)
			: base(xeFunction)
		{
			int level;
			this.Level = int.TryParse(xeFunction.Attribute("value").Value, out level) ? level : 0;
			this.Name = xeFunction.Attribute("text").Value;
		}

		/// <summary>メニュー名</summary>
		public new string Name { get; set; }
		/// <summary>機能レベル</summary>
		public int Level { get; set; }
		/// <summary>ユニークID</summary>
		public string UniqueId { get; set; }
	}
}
