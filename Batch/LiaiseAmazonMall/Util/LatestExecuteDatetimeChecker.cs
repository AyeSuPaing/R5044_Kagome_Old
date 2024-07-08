/*
=========================================================================================================
  Module      : 前回実行時間チェッククラス(LatestExecuteDatetimeChecker.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Util
{
	/// <summary>
	/// 前回実行時間チェッククラス
	/// </summary>
	public class LatestExecuteDatetimeChecker
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LatestExecuteDatetimeChecker()
		{
			Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp"));
		}

		/// <summary>
		/// 前回実行時間取得
		/// </summary>
		/// <returns>前回実行時間</returns>
		public DateTime? GetLastExecuteDatetime(string mallId)
		{
			SetTempFilePath(mallId);
			if (File.Exists(this.TempFilePath) == false) return null;
			return File.GetLastWriteTime(this.TempFilePath);
		}

		/// <summary>
		/// ファイルパス設定
		/// </summary>
		/// <param name="mallId">モールID</param>
		public void SetTempFilePath(string mallId)
		{
			this.TempFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tmp", mallId + "lastExec.tmp");
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

		#region プロパティ
		/// <summary>tmpファイルパス</summary>
		private string TempFilePath { get; set; }
		#endregion
	}
}
