/*
=========================================================================================================
  Module      : 最終実行日処理クラス(LastExecDate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using w2.Common.Logger;

namespace w2.MarketingPlanner.Batch.CreateReport
{
	/// <summary>
	/// 最終実行日処理
	/// </summary>
	internal class LastExecDate
	{
		/// <summary>
		/// 最終実行日を取得する
		/// </summary>
		/// <returns>最終実行日</returns>
		internal static DateTime GetLastExecDate()
		{
			if (File.Exists(Constants.FILE_LAST_EXEC_DATE) == false)
			{
				return DateTime.Today.AddDays(-1).Date;
			}

			// 最終実行日ファイル読み込み
			using (var sr = new StreamReader(Constants.FILE_LAST_EXEC_DATE, System.Text.Encoding.Default))
			{
				return DateTime.ParseExact(sr.ReadLine(), Constants.FORMAT_DATE, null);
			}
		}

		/// <summary>
		/// 最終実行日を設定する
		/// </summary>
		internal static void UpdateLastExecDate()
		{
			try
			{
				// 最終実行日ファイル書き込み
				using (StreamWriter sw = new StreamWriter(Constants.FILE_LAST_EXEC_DATE, false, System.Text.Encoding.Default))
				{
					sw.WriteLine(DateTime.Today.Date.ToString(Constants.FORMAT_DATE));
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteInfo("Propertyファイルの書き込みに失敗しました。", ex);
			}
		}
	}
}
