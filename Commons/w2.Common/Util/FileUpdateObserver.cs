/*
=========================================================================================================
  Module      : ファイル更新監視モジュール(FileUpdateObserver.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Cryptography;
using System.Threading;

namespace w2.Common.Util
{
	/// <summary>
	/// ファイル更新管理モジュール
	/// </summary>
	public class FileUpdateObserver
	{
		/// <summary>インスタンス</summary>
		static readonly FileUpdateObserver m_instance = new FileUpdateObserver();

		///// <summary>実行アクションリスト</summary>
		readonly Dictionary<string, ActionInfo> m_actionInfos = new Dictionary<string, ActionInfo>();
		///// <summary>ロックオブジェクト</summary>
		readonly object m_lockObject = new object();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private FileUpdateObserver()
		{
		}

		/// <summary>
		/// インスタンス取得
		/// </summary>
		/// <returns>インスタンス</returns>
		public static FileUpdateObserver GetInstance()
		{
			return m_instance;
		}

		/// <summary>
		/// 既に監視されているか
		/// </summary>
		/// <param name="dirPath">対象ディレクトリ</param>
		/// <param name="filePattern">対象ファイル名（ワイルドカード可）</param>
		/// <returns>既に監視されているか</returns>
		public bool Contains(string dirPath, string filePattern)
		{
			var key = CreateKey(dirPath, filePattern);
			lock (m_lockObject)
			{
				return m_actionInfos.ContainsKey(key);
			}
		}

		/// <summary>
		/// ファイル監視オブジェクトのキー作成
		/// </summary>
		/// <param name="dirPath">対象ディレクトリ</param>
		/// <param name="filePattern">対象ファイル名（ワイルドカード可）</param>
		/// <returns>キー</returns>
		private string CreateKey(string dirPath, string filePattern)
		{
			return dirPath + filePattern;
		}

		/// <summary>
		/// 監視ファイル追加
		/// </summary>
		/// <param name="dirPath">対象ディレクトリ</param>
		/// <param name="filePattern">対象ファイル名（ワイルドカード可）</param>
		/// <param name="actions">実行メソッド</param>
		public void AddObservation(string dirPath, string filePattern, params Action[] actions)
		{
			AddObservation(dirPath, filePattern, false, actions);
		}

		/// <summary>
		/// 監視ファイル追加(サブディレクトリ含む）
		/// </summary>
		/// <param name="dirPath">対象ディレクトリ</param>
		/// <param name="filePattern">対象ファイル名（ワイルドカード可）</param>
		/// <param name="actions">実行メソッド</param>
		public void AddObservationWithSubDir(string dirPath, string filePattern, params Action[] actions)
		{
			AddObservation(dirPath, filePattern, true, actions);
		}

		/// <summary>
		/// 監視ファイル追加
		/// </summary>
		/// <param name="dirPath">対象ディレクトリ</param>
		/// <param name="filePattern">対象ファイル名（ワイルドカード可）</param>
		/// <param name="includeSubdir">サブディレクトリを検索するかどうか</param>
		/// <param name="actions">実行メソッド</param>
		private void AddObservation(string dirPath, string filePattern, bool includeSubdir, params Action[] actions)
		{
			var key = CreateKey(dirPath, filePattern);
			lock (m_lockObject)
			{
				// 新規追加？
				if (m_actionInfos.ContainsKey(key) == false)
				{
					// Configファイル監視イベント設定
					var fswWatcher = new FileSystemWatcher
					{
						Path = dirPath,
						Filter = filePattern,
						IncludeSubdirectories = includeSubdir,
						// VisualStudioのエディタからの更新に対応するため「CreationTime」も付与
						// （一度別ファイルに保存して新規作成をしている模様）
						NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.CreationTime,
					};
					fswWatcher.Changed += ExecMethods;

					m_actionInfos.Add(
						key,
						new ActionInfo(actions, DateTime.MinValue, fswWatcher));

					// 監査開始
					fswWatcher.EnableRaisingEvents = true;
				}
				// 既存追加？
				else
				{
					// イベント追加
					foreach (var method in actions.Where(m => (m_actionInfos[key].Actions.Contains(m) == false)))
					{
						m_actionInfos[key].Actions.Add(method);
					}
				}
			}
		}

		/// <summary>
		/// メソッド実行イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ExecMethods(object sender, EventArgs e)
		{
			var watcher = ((FileSystemWatcher)sender);
			var key  = watcher.Path + watcher.Filter;

			// ファイル書き込み後ちょっとの間ロックの解放が遅れる可能性があるため少しスリープ
			Thread.Sleep(500);

			lock (m_lockObject)
			{
				if (DateTime.Now > m_actionInfos[key].LastRaised)
				{
					m_actionInfos[key].LastRaised = DateTime.Now;
					foreach (var mMethod in m_actionInfos[key].Actions)
					{
						mMethod.Invoke();
					}
				}
			}
		}

		#region 内部クラス
		/// <summary>
		/// アクション情報
		/// </summary>
		private class ActionInfo
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="actions">実行アクション</param>
			/// <param name="lastRaised">最終実行日（2重起動防止用）</param>
			/// <param name="watcher">FileSystemWatcher（念のため保持）</param>
			public ActionInfo(IEnumerable<Action> actions, DateTime lastRaised, FileSystemWatcher watcher)
			{
				this.Actions = actions.ToList();
				this.LastRaised = lastRaised;
				this.Watcher = watcher;
			}

			/// <summary>アクションリスト</summary>
			public List<Action> Actions { get; set; }
			/// <summary>最終実行日（2重起動防止用）</summary>
			public DateTime LastRaised { get; set; }
			/// <summary>FileSystemWatcher（念のため保持）</summary>
			private FileSystemWatcher Watcher { get; set; }
		}
		#endregion
	}
}
