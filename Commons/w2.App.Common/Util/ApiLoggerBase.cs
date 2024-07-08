/*
=========================================================================================================
  Module      : ApiLog出力基底 (ApiLoggerBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using w2.Common.Util;

namespace w2.App.Common.Util
{
	/// <summary>
	/// 各種ファイルログを出力する
	/// </summary>
	public abstract class ApiLoggerBase
	{
		/// <summary>
		/// ログ書き込み処理（ディレクトリパス指定可能）
		/// </summary>
		/// <param name="messageLine">メッセージ</param>
		/// <param name="logFilePath">ファイル保存先ディレクトリパス</param>
		/// <param name="fileNamePrefix">ファイル名接頭辞</param>
		/// <param name="fileSizeThreshold">ファイルサイズ閾値</param>
		protected static void Write(
			string messageLine,
			string logFilePath,
			string fileNamePrefix,
			int fileSizeThreshold)
		{
			if (Directory.Exists(logFilePath) == false)
			{
				Directory.CreateDirectory(logFilePath);
			}

			var filePath = string.Format(
				"{0}{1}.{2}",
				logFilePath,
				fileNamePrefix,
				Constants.LOGFILE_EXTENSION);

			try
			{
				// Mutexで排他制御しながらファイル書き込み
				using (var mtx = new Mutex(false, filePath.Replace("\\", "_") + ".FileWrite"))
				{
					try
					{
						mtx.WaitOne();

						using (var sw = new StreamWriter(filePath, true, Encoding.GetEncoding(Constants.LOGFILE_ENCODING)))
						{
							sw.WriteLine(messageLine);
						}

						ForkFile(filePath, fileNamePrefix, Constants.LOGFILE_EXTENSION, fileSizeThreshold);
					}
					finally
					{
						mtx.ReleaseMutex();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(StringUtility.ToEmpty(ex));
			}
		}

		/// <summary>
		/// サイズが閾値を超えたファイルに連番を振って分離
		/// </summary>
		/// <param name="filePath">ログファイルのパス</param>
		/// <param name="prefix">ファイル名の接頭部分</param>
		/// <param name="suffix">ファイル名の日付部分</param>
		/// <param name="fileSizeThreshold">ファイルサイズ閾値</param>
		private static void ForkFile(string filePath, string prefix, string suffix, int fileSizeThreshold)
		{
			if (fileSizeThreshold < 0) return;

			var file = new FileInfo(filePath);
			if (file.Length < fileSizeThreshold * 1024 * 1024) return;

			var fileName = string.Format("{0}_*", prefix);
			var pattern = string.Format("{0}_{1}.{2}$", prefix, "(?<number>[0-9]+)", suffix);
			var maxValue = Directory.GetFiles(filePath, fileName)
				.Select(item => Regex.Match(item, pattern))
				.Where(item => item.Success)
				.Select(item => int.Parse(item.Groups["number"].Value))
				.DefaultIfEmpty(0)
				.Max();
			var newFilePath = string.Format(
				"{0}{1}_{2}.{3}",
				filePath,
				prefix,
				(maxValue + 1),
				suffix);
			file.MoveTo(newFilePath);
		}
	}
}
