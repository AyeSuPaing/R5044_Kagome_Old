/*
=========================================================================================================
  Module      : Facebook Api Logger (FacebookApiLogger.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using w2.Common.Logger;

namespace w2.App.Common.Facebook.Logger
{
	/// <summary>
	/// 各種ファイルログを出力する
	/// </summary>
	internal class FacebookApiLogger : FileLogger
	{
		/// <summary>Facebook API log path</summary>
		private static string s_facebookApiLogPath = (Constants.PHYSICALDIRPATH_LOGFILE + Constants.FACEBOOK_CONVERSION_API);

		/// <summary>
		/// ログ書き込み処理（ディレクトリパス指定可能）
		/// </summary>
		/// <param name="message">メッセージ</param>
		public static void Write(string message)
		{
			if (Directory.Exists(s_facebookApiLogPath) == false)
			{
				Directory.CreateDirectory(s_facebookApiLogPath);
			}

			var today = DateTime.Now.ToString(Constants.DATE_FORMAT_SHORT);
			var filePath = string.Format(
				"{0}/{1}_{2}.{3}",
				s_facebookApiLogPath,
				Constants.FACEBOOK_CONVERSION_API,
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

						using (var sw = new StreamWriter(filePath, true, Encoding.GetEncoding(Constants.LOGFILE_ENCODING)))
						{
							var line = string.Format(
								"[{0}] {1:yyyy年M月d日HH:mm:ss}　{2}",
								Constants.FACEBOOK_CONVERSION_API,
								DateTime.Now,
								message);
							sw.WriteLine(line);
						}

						var prefix = string.Format(
							"{0}_{1}",
							Constants.FACEBOOK_CONVERSION_API,
							today);
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
		public static void ForkFile(string filePath, string prefix, string suffix)
		{
			if (Constants.FACEBOOK_API_LOGFILE_THRESHOLD < 0) return;

			var file = new FileInfo(filePath);
			if (file.Length < Constants.FACEBOOK_API_LOGFILE_THRESHOLD * 1024 * 1024) return;

			var fileName = string.Format("{0}_*", prefix);
			var pattern = string.Format("{0}_{1}.{2}$", prefix, "(?<number>[0-9]+)", suffix);

			var maxValue = Directory.GetFiles(s_facebookApiLogPath, fileName)
				.Select(item => Regex.Match(item, pattern))
				.Where(item => item.Success)
				.Select(item => int.Parse(item.Groups["number"].Value))
				.DefaultIfEmpty(0)
				.Max();
			var newFilePath = string.Format(
				"{0}{1}_{2}.{3}",
				s_facebookApiLogPath,
				prefix,
				(maxValue + 1),
				suffix);
			file.MoveTo(newFilePath);
		}
	}
}
