/*
=========================================================================================================
  Module      : 注文完了時用プラグインインターフェース(IOrderCompletePlugin.cs)
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
	public interface IOrderCompletePlugin : IPlugin
	{
		/// <summary>
		/// 注文完了時処理
		/// </summary>
		void OnCompleted();
	}
}
