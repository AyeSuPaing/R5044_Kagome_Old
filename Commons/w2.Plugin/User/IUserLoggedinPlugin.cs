/*
=========================================================================================================
  Module      : ログイン時プラグインインターフェース(IUserLoggedInPlugin.cs)
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
	public interface IUserLoggedInPlugin : IPlugin
	{
		/// <summary>
		/// ログイン時処理
		/// </summary>
		void OnLoggedIn();
	}
}
