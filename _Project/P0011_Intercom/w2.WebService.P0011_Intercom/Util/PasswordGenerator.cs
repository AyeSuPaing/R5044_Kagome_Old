/*
=========================================================================================================
  Module      : パスワード用ユーティリティクラス(PassUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2011 All Rights Reserved.
  MEMO        : パスワードユーティリティ。
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Data;

namespace w2.Plugin.P0011_Intercom.WebService.Util
{
	public class PasswordGenerator
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PasswordGenerator()
		{

		}

		/// <summary>
		/// ワンタイムパスワード生成
		/// </summary>
		/// <returns></returns>
		public string GenerateOnetimePass()
		{
			//GUID返すだけ
			return System.Guid.NewGuid().ToString().Replace("-","");
		}
	}
}


