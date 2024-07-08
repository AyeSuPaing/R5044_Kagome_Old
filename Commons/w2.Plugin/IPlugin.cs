/*
=========================================================================================================
  Module      : プラグインインターフェース(IPlugin.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Plugin
{
	/// <summary>
	/// プラグインインターフェース
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="host">プラグインホスト</param>
		void Initialize(IPluginHost host);

		/// <summary>プラグインホスト</summary>
		IPluginHost Host { get; }
	}
}
