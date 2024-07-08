/*
=========================================================================================================
  Module      : リフレッシュファイルマネージャクラス(RefreshFileManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using w2.Common.Util;

namespace w2.App.Common.RefreshFileManager
{
	/// <summary>
	/// RefreshFileManager の概要の説明です
	/// </summary>
	public class RefreshFileManager
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="refreshFileName"></param>
		internal RefreshFileManager(string refreshFileName)
			: this(Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, @"Refresh") , refreshFileName, true)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="refreshDirPath">監視対象のディレクトリパス</param>
		/// <param name="refreshFileName">監視対象のファイル名（ワイルドカード可）</param>
		/// <param name="isCreateRefleshFile">リフレッシュファイルが存在しなければ作成するか。</param>
		internal RefreshFileManager(string refreshDirPath, string refreshFileName, bool isCreateRefleshFile)
		{
			this.RefreshDirPath = refreshDirPath;
			this.RefreshFileName = refreshFileName;
			this.CreateRefreshFileIfNotExists = isCreateRefleshFile;
		}

		/// <summary>
		/// 監視ファイル追加
		/// </summary>
		/// <param name="actions">実行メソッド</param>
		public void AddObservation(params Action[] actions)
		{
			// 更新用ファイルを作成・更新
			if (this.CreateRefreshFileIfNotExists)
			{
				lock (this.RefreshFileName)
				{
					if (File.Exists(this.RefreshFilePath) == false)
					{
						CreateFile();
					}
				}
			}

			// 監視追加
			FileUpdateObserver.GetInstance().AddObservation(
				this.RefreshDirPath,
				this.RefreshFileName,
				actions);
		}

		/// <summary>
		/// リフレッシュファイル更新
		/// </summary>
		public void CreateUpdateRefreshFile()
		{
			// 更新用ファイルを作成・更新
			lock (this.RefreshFileName)
			{
				if (File.Exists(this.RefreshFilePath) == false)
				{
					CreateFile();
				}
				else
				{
					File.SetLastWriteTime(this.RefreshFilePath, DateTime.Now);
				}
			}
		}

		/// <summary>
		/// リフレッシュファイル作成
		/// </summary>
		public void CreateFile()
		{
			if (this.CreateRefreshFileIfNotExists == false) return;
			if (File.Exists(this.RefreshFilePath)) return;
			if (Directory.Exists(this.RefreshDirPath) == false) Directory.CreateDirectory(this.RefreshDirPath);
			using (var sw = File.CreateText(this.RefreshFilePath))
			{
			}
		}

		/// <summary>
		/// リフレッシュファイル更新日時取得
		/// </summary>
		/// <returns>更新日時</returns>
		public DateTime GetRefreshFileUpdateTime()
		{
			return File.GetLastWriteTime(this.RefreshFilePath);
		}

		/// <summary>リフレッシュファイルパス</summary>
		public string RefreshFilePath
		{
			get { return Path.Combine(this.RefreshDirPath, this.RefreshFileName); }
		}
		/// <summary>リフレッシュファイル名</summary>
		private string RefreshFileName { get; set; }
		/// <summary>リフレッシュディレクトリパス</summary>
		private string RefreshDirPath { get; set; }
		/// <summary>リフレッシュファイルがなければ作成するか</summary>
		private bool CreateRefreshFileIfNotExists { get; set; }
	}
}