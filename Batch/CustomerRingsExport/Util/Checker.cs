/*
=========================================================================================================
  Module      : チェッククラス(Checker.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;

namespace w2.Commerce.Batch.CustomerRingsExport.Util
{
	public class Checker
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Checker(string target)
		{
			Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp"));
			this.TempFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp", "lastExec" + target + ".tmp");
		}

		/// <summary>
		/// 前回実行時間取得
		/// </summary>
		/// <returns>前回実行時間</returns>
		public DateTime? GetLastExecuteDateTime()
		{
			if (File.Exists(this.TempFilePath) == false) return null;
			return File.GetLastWriteTime(this.TempFilePath);
		}

		/// <summary>
		/// 終了ファイルの出力
		/// </summary>
		public void CreateEndFile()
		{
			CreateEndFile(DateTime.Now);
		}
		/// <summary>
		/// 終了ファイルの出力
		/// </summary>
		/// <param name="date">日付</param>
		public void CreateEndFile(DateTime date)
		{
			if (File.Exists(this.TempFilePath) == false)
			{
				CreateTempFile();
			}

			// 日付更新
			File.SetLastWriteTime(this.TempFilePath, date);
		}

		/// <summary>
		/// tmpファイル作成
		/// </summary>
		private void CreateTempFile()
		{
			using (File.CreateText(this.TempFilePath))
			{
			}
		}

		/// <summary>tmpファイルパス</summary>
		private string TempFilePath { get; set; }
	}
}
