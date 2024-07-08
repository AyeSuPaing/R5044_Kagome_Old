/*
=========================================================================================================
  Module      : 注文検証用プラグインインターフェース(IOrderValidatingPlugin.cs)
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
	public interface IOrderValidatingPlugin : IPlugin
	{
		/// <summary>
		/// 検証時処理
		/// </summary>
		void OnValidating();
		/// <summary>成功フラグ</summary>
		bool IsSuccess { get; }
		/// <summary>メッセージ</summary>
		string Message { get; }
	}
}
