/*
=========================================================================================================
  Module      : プラグイン情報クラス(PluginInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using w2.Common.Logger;
using w2.Plugin;

namespace w2.App.Common.Plugin
{
	public class PluginInfo
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="filePath">DLLファイルパス</param>
		/// <param name="className">クラス名</param>
		private PluginInfo(string filePath, string className)
		{
			this.FilePath = filePath;
			this.ClassName = className;
		}

		/// <summary>
		/// プラグインを取得する
		/// </summary>
		/// <param name="searchType">対象とするプラグインの型</param>
		/// <returns>プラグイン情報リスト</returns>
		public static List<PluginInfo> FindPlugins(Type searchType)
		{
			List<PluginInfo> plugins = new List<PluginInfo>();

			string pluginType = searchType.FullName;

			// pluginフォルダ
			string folder = Path.GetDirectoryName(Constants.PHYSICALDIRPATH_PLUGINS_STORAGE_LOCATION);

			// 配下のdllをすべて取得
			string[] dlls = Directory.GetFiles(folder, "*.dll");

			foreach (var dll in dlls)
			{
				Assembly assembly;
				if (TryAssemblyLoad(dll, out assembly) == false) continue;

				foreach (Type t in assembly.GetTypes())
				{
					//アセンブリ内のすべての型について、
					//プラグインとして有効であり、対象のインターフェースが実装されているかを検証
					if (t.IsClass && t.IsPublic && !t.IsAbstract &&　t.GetInterface(pluginType) != null)
					{
						//PluginInfoをコレクションに追加する
						plugins.Add(new PluginInfo(dll, t.FullName));
					}
				}
			}

			return plugins;
		}

		/// <summary>
		/// DLLの読み込みを試行し、成否の結果を返します。
		/// </summary>	
		/// <param name="dllPath">DLLファイルパス</param>
		/// <param name="asm">アセンブリ</param>
		/// <returns>読込成否</returns>
		private static bool TryAssemblyLoad(string dllPath, out Assembly asm)
		{
			asm = null;
			try
			{
				asm = Assembly.LoadFrom(dllPath);
				return true;
			}
			catch (Exception ex)
			{
				// ログを残して先へ進む
				FileLogger.WriteError("プラグイン読み込み時にエラーが発生しました。", ex);
				return false;
			}
		}

		/// <summary>
		/// インスタンス作成
		/// </summary>
		/// <param name="host">プラグインホスト</param>
		/// <returns>プラグインインスタンス</returns>
		public IPlugin CreateInstance(IPluginHost host)
		{
			Assembly asm;
			if (TryAssemblyLoad(this.FilePath, out asm) == false) return null;

			IPlugin plugin;
			if (TryCreateInstance(asm, out plugin) == false) return null;

			//　初期化
			plugin.Initialize(host);
			return plugin;
		}

		/// <summary>
		/// インスタンスの作成を試行し、成否の結果を返します。
		/// </summary>
		/// <param name="asm">アセンブリ</param>
		/// <param name="plugin">プラグイン</param>
		/// <returns>作成成否</returns>
		private bool TryCreateInstance(Assembly asm, out IPlugin plugin)
		{
			plugin = null;
			try
			{
				plugin = (IPlugin)asm.CreateInstance(this.ClassName);
				return true;
			}
			catch (Exception ex)
			{
				// ログを残して先へ進む
				FileLogger.WriteError("プラグインのインスタンス作成時にエラーが発生しました。", ex);
				return false;
			}
		}

		/// <summary>プラグインのファイルパス</summary>
		public string FilePath { get; private set; }
		/// <summary>クラス名</summary>
		public string ClassName { get; private set; }
	}
}
