/*
=========================================================================================================
  Module      : FLAPS商品、在庫同期排他制御クラス (FlapsIntegrationLocker.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace w2.App.Common.Flaps
{
	/// <summary>
	/// FLAPS商品、在庫同期排他制御クラス
	/// </summary>
	public class FlapsIntegrationLocker : IDisposable
	{
		/// <summary>ロックに使用するファイルのあるディレクトリパス</summary>
		private static readonly string s_directory =
			Path.Combine(Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + @"Flaps\Lock");
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="replicationData">同期するデータ</param>
		public FlapsIntegrationLocker(string replicationData)
		{
			this.ReplicationData = replicationData;
			this.FilePath = Path.Combine(s_directory, replicationData);

			// 必要であればディレクトリとファイルを作成
			CreateDirectoryIfNecessary();
			CreateLockFileIfNecessary();
		}

		/// <summary>
		/// ディレクトリ作成
		/// </summary>
		private void CreateDirectoryIfNecessary()
		{
			if (Directory.Exists(s_directory) == false)
			{
				Directory.CreateDirectory(s_directory);
			}
		}

		/// <summary>
		/// ファイル作成
		/// </summary>
		private void CreateLockFileIfNecessary()
		{
			if (File.Exists(this.FilePath) == false)
			{
				File.Create(this.FilePath);
			}
		}
		
		/// <summary>
		/// ロックがかかっているかどうか
		/// </summary>
		/// <returns>ロックがかかっているか</returns>
		public bool IsLocked()
		{
			try
			{
				var fs = new FileStream(this.FilePath, FileMode.Open, FileAccess.ReadWrite);
				fs.Close();
				return false;
			}
			catch (Exception exception)
			{
				var isLocked = IsExceptionCausedByLock(exception);
				return isLocked;
			}
		}

		/// <summary>
		/// ロックがかかっていることによる例外かどうか
		/// </summary>
		/// <param name="e">例外</param>
		/// <returns>ロックがかかっていることによる例外か</returns>
		private bool IsExceptionCausedByLock(Exception e)
		{
			var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
			var isLocked = ((errorCode == 32) || (errorCode == 33));
			if (isLocked == false) throw e;
			return true;
		}

		/// <summary>
		/// ロックをかける
		/// </summary>
		/// <returns>結果</returns>
		public bool Lock()
		{
			var isLocked = IsLocked();
			if (isLocked) return false;
			if (this.IsLocking) return true;

			try
			{
				var fs = new FileStream(
					this.FilePath,
					FileMode.Open,
					FileAccess.ReadWrite,
					FileShare.None);
				this.FileStream = fs;
			}
			catch (Exception exception)
			{
				var isLockedBySomeone = IsExceptionCausedByLock(exception);
				if (isLockedBySomeone) return false;
			}

			return true;
		}

		/// <summary>
		/// ロックを解除する
		/// </summary>
		/// <returns>結果</returns>
		public bool Unlock()
		{
			if (this.FileStream == null) return false;
			this.FileStream.Close();
			this.FileStream = null;
			return true;

		}

		/// <summary>
		/// オブジェクト破棄時にロックを解除する
		/// </summary>
		public void Dispose()
		{
			Unlock();
		}

		/// <summary>ファイルストリーム</summary>
		private FileStream FileStream { get; set; }
		/// <summary>ロックに使用するファイルのパス</summary>
		private string FilePath { get; set; }
		/// <summary>このオブジェクトがロックをかけているかどうか</summary>
		private bool IsLocking
		{
			get { return (this.FileStream != null); }
		}
		/// <summary>同期データ</summary>
		private string ReplicationData { get; set; }
		}
}
