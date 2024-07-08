/*
=========================================================================================================
  Module      : バッチで利用されるプロパティの辞書配列クラス(BatchProperties.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.ExternalAPI.Common.Extension;

namespace w2.ExternalAPI.Common
{
	/// <summary>
	/// バッチで利用されるプロパティの辞書配列
	/// </summary>
	public class BatchProperties : Dictionary<string, string>
	{
		/// <summary> 分割子 </summary>
		protected const char Delimiter = ';';
		/// <summary> 接続子 </summary>
		protected const char Connector = '=';

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="src">文字列</param>
		/// <example>BatchProperties("key1=value1;key2=value2;key3")</example>
		public BatchProperties(string src)
			:base(src.Split(Delimiter).Convert2Dictionary(Connector))
		{
		}

		/// <summary>
		/// 文字列に変換
		/// </summary>
		/// <returns>;区切り、=つなぎの文字列</returns>
		/// <example>key1=value1;key2=value2;key3 のように返却する</example>
		public override string ToString()
		{
			return string.Join(Delimiter.ToString(), 
				this.Select<KeyValuePair<string,string>,string>
					(pair => string.Concat(pair.Key, Connector, pair.Value)));
		}
	}
}
