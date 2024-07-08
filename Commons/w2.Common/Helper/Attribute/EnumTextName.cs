/*
=========================================================================================================
  Module      : 列挙体テキスト名属性クラス (EnumTextName.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Common.Helper.Attribute
{
	/// <summary>
	/// 列挙体テキスト名属性クラス
	/// </summary>
	public class EnumTextName : System.Attribute
	{
		/// <summary>テキスト名</summary>
		public string TextName { private set; get; }

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="textName">テキスト名</param>
		public EnumTextName(string textName)
		{
			this.TextName = textName;
		}
	}
}
