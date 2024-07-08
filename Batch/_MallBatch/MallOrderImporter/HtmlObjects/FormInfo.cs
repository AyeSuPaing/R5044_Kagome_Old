/*
=========================================================================================================
  Module      : フォーム情報クラス(FormInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;

namespace w2.Commerce.MallBatch.MallOrderImporter.HtmlObjects
{
	class FormInfo
	{
		public enum FormMethod { POST, GET };

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="action"></param>
		/// <param name="method"></param>
		public FormInfo(string action, string method)
			: this(action, (method.ToUpper() == "POST") ? FormMethod.POST : FormMethod.GET)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="action"></param>
		/// <param name="method"></param>
		public FormInfo(string action, FormMethod method)
		{
			this.Action = action;
			this.Method = method;
			this.Params = new Dictionary<string, string>();
			this.Params2 = new Dictionary<string, string>();
		}

		/// <summary>プロパティ：アクション</summary>
		public string Action { get; set; }
		/// <summary>プロパティ：メソッド</summary>
		public FormMethod Method { get; private set; }
		/// <summary>プロパティ：パラメタ</summary>
		public Dictionary<string, string> Params { get; private set; }
		/// <summary>プロパティ：パラメタ（idがキーになるｓ）</summary>
		public Dictionary<string, string> Params2 { get; private set; }
	}
}
