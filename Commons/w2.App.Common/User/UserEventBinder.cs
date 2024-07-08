/*
=========================================================================================================
  Module      : ユーザーイベントバインダークラス(UserEventBinder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Plugin;
using w2.App.Common.Proxy;
using w2.Domain.User;
using w2.Plugin;
using w2.Plugin.User;

namespace w2.App.Common.User
{
	[ErrorHandleProxy()]
	public partial class UserEventBinder : ContextBoundObject, IPluginHost
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		public UserEventBinder(string lastChanged)
		{
			InitializeInterface();

			this.LastChanged = lastChanged;
		}

		/// <summary>
		/// 登録イベント処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnRegistered(object sender, UserEventArgs e)
		{
			ExecPluginOnRegistered(e.UserData);
		}

		/// <summary>
		/// 登録時用プラグインの実行
		/// </summary>
		/// <param name="user">ユーザ情報</param>
		[ExceptionCanceler]
		private void ExecPluginOnRegistered(UserModel user)
		{
			// プラグインからの参照用データを設定
			this.Data = user.DataSource;

			List<PluginInfo> pluginInfoList = PluginInfo.FindPlugins(typeof(IUserRegisteredPlugin));

			List<IUserRegisteredPlugin> plugins = pluginInfoList
				.Select(plugin => plugin.CreateInstance(this)).Cast<IUserRegisteredPlugin>().ToList();

			plugins.ForEach(plugin => plugin.OnRegistered());
		}

		/// <summary>
		/// 変更イベント処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnModified(object sender, UserEventArgs e)
		{
			ExecPluginOnModified(e.UserData);
		}

		/// <summary>
		/// 変更時用プラグインの実行
		/// </summary>
		/// <param name="user">ユーザ情報</param>
		[ExceptionCanceler]
		private void ExecPluginOnModified(UserModel user)
		{
			// プラグインからの参照用データを設定
			this.Data = user.DataSource;

			List<PluginInfo> pluginInfos = PluginInfo.FindPlugins(typeof(IUserModifiedPlugin));

			List<IUserModifiedPlugin> plugins = new List<IUserModifiedPlugin>(
				pluginInfos.Select(plugin => plugin.CreateInstance(this) as IUserModifiedPlugin).ToList());

			plugins.ForEach(plugin => plugin.OnModified());
		}

		/// <summary>
		/// 退会イベント処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnWithdrawed(object sender, UserEventArgs e)
		{
			ExecPluginOnWithdrawed(e.UserData);
		}

		/// <summary>
		/// 退会時用プラグインの実行
		/// </summary>
		/// <param name="user">ユーザ情報</param>
		[ExceptionCanceler]
		private void ExecPluginOnWithdrawed(UserModel user)
		{
			// プラグインからの参照用データを設定
			this.Data = user.DataSource;

			List<PluginInfo> pluginInfos = PluginInfo.FindPlugins(typeof(IUserWithdrawedPlugin));

			List<IUserWithdrawedPlugin> plugins = new List<IUserWithdrawedPlugin>(
				pluginInfos.Select(plugin => plugin.CreateInstance(this) as IUserWithdrawedPlugin).ToList());

			plugins.ForEach(plugin => plugin.OnWithdrawed());
		}

		/// <summary>
		/// ログイン時イベント処理
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void OnLoggedIn(object sender, UserEventArgs e)
		{
			ExecPluginOnLoggedIn(e.UserData);
		}

		/// <summary>
		/// ログイン時用プラグインの実行
		/// </summary>
		/// <param name="user">ユーザ情報</param>
		[ExceptionCanceler]
		private void ExecPluginOnLoggedIn(UserModel user)
		{
			// プラグインからの参照用データを設定
			this.Data = user.DataSource;

			List<PluginInfo> pluginInfos = PluginInfo.FindPlugins(typeof(IUserLoggedInPlugin));

			List<IUserLoggedInPlugin> plugins = new List<IUserLoggedInPlugin>(
				pluginInfos.Select(plugin => plugin.CreateInstance(this) as IUserLoggedInPlugin).ToList());

			plugins.ForEach(plugin => plugin.OnLoggedIn());
		}

		/// <summary>最終更新者</summary>
		private string LastChanged { get; set; }
	}
}
