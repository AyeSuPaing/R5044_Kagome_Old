/*
=========================================================================================================
  Module      : ユーザ変更用プラグインインターフェース(IUserModifiedPlugin.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Plugin.User
{
	public interface IUserModifiedPlugin : IPlugin
	{
		/// <summary>
		/// 情報変更時処理
		/// </summary>
		void OnModified();
	}
}
