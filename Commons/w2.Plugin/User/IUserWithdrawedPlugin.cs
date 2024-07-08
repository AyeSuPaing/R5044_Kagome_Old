/*
=========================================================================================================
  Module      : ユーザ退会用プラグインインターフェース(UserWithdrawedPlugin.cs)
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
	public interface IUserWithdrawedPlugin : IPlugin
	{
		/// <summary>
		/// 退会時処理
		/// </summary>
		void OnWithdrawed();
	}
}
