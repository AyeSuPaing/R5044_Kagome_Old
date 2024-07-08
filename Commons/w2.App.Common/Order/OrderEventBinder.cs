/*
=========================================================================================================
  Module      : 注文イベントバインダークラス(OrderEventBinder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Plugin;
using w2.App.Common.Proxy;
using w2.Common.Logger;
using w2.Plugin;
using w2.Plugin.Order;

namespace w2.App.Common.Order
{
	[ErrorHandleProxy()]
	public partial class OrderEventBinder : ContextBoundObject, IPluginHost
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderEventBinder()
		{
			InitializeInterface();
		}

		/// <summary>
		/// 検証時イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnOrderValidating(object sender, OrderEventArgs e)
		{
			ExecPluginOnValidating(e);
		}

		/// <summary>
		/// 検証時プラグイン処理
		/// </summary>
		/// <param name="e"></param>
		[ExceptionCanceler]
		private void ExecPluginOnValidating(OrderEventArgs e)
		{
			// プラグインからの参照用データを設定
			this.Data = e.OrderData;

			// プラグイン探索
			List<PluginInfo> pluginInfoList = PluginInfo.FindPlugins(typeof(IOrderValidatingPlugin));

			// プラグイン読込
			List<IOrderValidatingPlugin> plugins = pluginInfoList
				.Select(plugin => plugin.CreateInstance(this)).Cast<IOrderValidatingPlugin>().ToList();

			// プラグイン処理実行
			plugins.ForEach(plugin => plugin.OnValidating());

			// 検証結果取得
			//   他のイベントで処理されている可能性が有るため、既に失敗している場合は処理しない
			if (e.IsSuccess) e.IsSuccess = plugins.TrueForAll(plugin => plugin.IsSuccess);

			// メッセージを連結する
			e.ReturnMessage += string.Join("\r\n", plugins.Where(plugin => plugin.IsSuccess == false).Select(plugin => plugin.Message)) + "\r\n\r\n";
		}

		/// <summary>
		/// 注文失敗イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnOrderFailed(object sender, OrderEventArgs e)
		{
			ExecPluginOnFailed(e);
		}

		/// <summary>
		/// 注文失敗時プラグイン処理
		/// </summary>
		/// <param name="e"></param>
		[ExceptionCanceler]
		private void ExecPluginOnFailed(OrderEventArgs e)
		{
			// プラグインからの参照用データを設定
			this.Data = e.OrderData;

			// プラグイン探索
			List<PluginInfo> pluginInfoList = PluginInfo.FindPlugins(typeof(IOrderFailedPlugin));

			// プラグイン読込
			List<IOrderFailedPlugin> plugins = pluginInfoList
				.Select(plugin => plugin.CreateInstance(this)).Cast<IOrderFailedPlugin>().ToList();

			// プラグイン処理実行
			plugins.ForEach(plugin => plugin.OnFailed());
		}

		/// <summary>
		/// 注文完了イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnOrderCompleted(object sender, OrderEventArgs e)
		{
			ExecPluginOnCompleted(e);
		}

		/// <summary>
		/// 注文完了時プラグイン処理
		/// </summary>
		/// <param name="e"></param>
		[ExceptionCanceler]
		private void ExecPluginOnCompleted(OrderEventArgs e)
		{
			// プラグインからの参照用データを設定
			this.Data = e.OrderData;

			// プラグイン探索
			List<PluginInfo> pluginInfoList = PluginInfo.FindPlugins(typeof(IOrderCompletePlugin));

			// プラグイン読込
			List<IOrderCompletePlugin> plugins = pluginInfoList
				.Select(plugin => plugin.CreateInstance(this)).Cast<IOrderCompletePlugin>().ToList();

			// プラグイン処理実行
			plugins.ForEach(plugin => plugin.OnCompleted());
		}
	}
}
