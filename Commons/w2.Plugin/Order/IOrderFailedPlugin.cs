/*
=========================================================================================================
  Module      : 注文失敗時用プラグインインターフェース(IOrderFailedPlugin.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Plugin;

namespace w2.Plugin.Order
{
	public interface IOrderFailedPlugin : IPlugin
	{
		/// <summary>
		/// 注文失敗時処理
		/// </summary>
		void OnFailed();
	}
}
