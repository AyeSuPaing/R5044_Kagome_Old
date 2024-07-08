/*
=========================================================================================================
  Module      : APIロガー(ApiLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using w2.Common.Logger;

namespace w2.App.Common.Flaps.Logger
{
	/// <summary>
	/// 各種ファイルログを出力する
	/// </summary>
	internal class ApiLogger : FileLogger
	{
		/// <summary>
		/// ログ書き込み処理（ディレクトリパス指定可能）
		/// </summary>
		/// <param name="message">メッセージ</param>
		internal static void Write(string message)
		{
			if (Directory.Exists(Constants.PHYSICALDIRPATH_FLAPS_API_LOGFILE) == false)
			{
				Directory.CreateDirectory(Constants.PHYSICALDIRPATH_FLAPS_API_LOGFILE);
			}

			var today = DateTime.Now.ToString("yyyyMMdd");
			var filePath = string.Format(
				"{0}{1}_{2}.{3}",
				Constants.PHYSICALDIRPATH_FLAPS_API_LOGFILE,
				Constants.FLAPS_API_LOGFILE_NAME_PREFIX,
				today,
				Constants.LOGFILE_EXTENSION);

			try
			{
				// Mutexで排他制御しながらファイル書き込み
				using (var mtx = new Mutex(false, filePath.Replace("\\", "_") + ".FileWrite"))
				{
					try
					{
						mtx.WaitOne();

						using (var sw = new StreamWriter(
							filePath,
							true,
							Encoding.GetEncoding(Constants.LOGFILE_ENCODING)))
						{
							var line = string.Format(
								"[{0}] {1:yyyy年M月d日HH:mm:ss}　{2}",
								Constants.FLAPS_API_LOGFILE_NAME_PREFIX,
								DateTime.Now,
								message);
							sw.WriteLine(line);
						}

						var prefix = string.Format("{0}_{1}", Constants.FLAPS_API_LOGFILE_NAME_PREFIX, today);
						ForkFile(filePath, prefix, Constants.LOGFILE_EXTENSION);
					}
					finally
					{
						mtx.ReleaseMutex();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}

		/// <summary>
		/// サイズが閾値を超えたファイルに連番を振って分離
		/// </summary>
		/// <param name="filePath">ログファイルのパス</param>
		/// <param name="prefix">ファイル名の接頭部分</param>
		/// <param name="suffix">ファイル名の日付部分</param>
		private static void ForkFile(string filePath, string prefix, string suffix)
		{
			var file = new FileInfo(filePath);
			if (Constants.FLAPS_API_LOGFILE_THRESHOLD < 0) return;
			if (file.Length < Constants.FLAPS_API_LOGFILE_THRESHOLD * 1024 * 1024) return;

			var fileName = string.Format("{0}_*", prefix);
			var pattern = string.Format("{0}_{1}.{2}$", prefix, "(?<number>[0-9]+)", suffix);

			var max = Directory.GetFiles(Constants.PHYSICALDIRPATH_FLAPS_API_LOGFILE, fileName)
				.Select(item => Regex.Match(item, pattern)).Where(item => item.Success)
				.Select(item => int.Parse(item.Groups["number"].Value)).DefaultIfEmpty(0).Max();
			var newFilePath = string.Format(
				"{0}{1}_{2}.{3}",
				Constants.PHYSICALDIRPATH_FLAPS_API_LOGFILE,
				prefix,
				max + 1,
				suffix);
			file.MoveTo(newFilePath);
		}
	}
}
