/*
=========================================================================================================
  Module      : ユーザ登録用プラグインインターフェース(IUserRegisteredPlugin.cs)
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
	public interface IUserRegisteredPlugin : IPlugin
	{
		/// <summary>
		/// 登録時処理
		/// </summary>
		void OnRegistered();
	}
}
