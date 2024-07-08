/*
=========================================================================================================
  Module      : 最終実行日処理クラス(LastExecDate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using w2.Common.Logger;

namespace w2.Commerce.Batch.FixedPurchaseBatch
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
			if (File.Exists(Constants.FILE_LAST_SEND_DATE_FOR_CHANGE_DEADLINE_MAIL) == false)
			{
				return DateTime.Today.Date;
			}

			// 最終実行日ファイル読み込み
			using (var sr = new StreamReader(Constants.FILE_LAST_SEND_DATE_FOR_CHANGE_DEADLINE_MAIL, System.Text.Encoding.Default))
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
				using (StreamWriter sw = new StreamWriter(Constants.FILE_LAST_SEND_DATE_FOR_CHANGE_DEADLINE_MAIL, false, System.Text.Encoding.Default))
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
