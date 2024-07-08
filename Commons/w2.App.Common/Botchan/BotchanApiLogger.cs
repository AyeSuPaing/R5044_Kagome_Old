/*
=========================================================================================================
  Module      : APIロガー(ApiLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using w2.Common.Logger;

namespace w2.App.Common.Botchan
{
	/// <summary>
	/// 各種ファイルログを出力する
	/// </summary>
	public class BotchanApiLogger : FileLogger
	{
		/// <summary>
		/// ログ書き込み処理（ディレクトリパス指定可能）
		/// </summary>
		/// <param name="message">メッセージ</param>
		public static void Write(string message)
		{
			if (Directory.Exists(Constants.DIRECTORY_BOTCHAN_API_LOGFILEPATH) == false)
			{
				Directory.CreateDirectory(Constants.DIRECTORY_BOTCHAN_API_LOGFILEPATH);
			}

			var today = DateTime.Now.ToString("yyyyMMdd");
			var filePath = string.Format(
				"{0}{1}_{2}.{3}",
				Constants.DIRECTORY_BOTCHAN_API_LOGFILEPATH,
				Constants.BOTCHAN_API_LOGFILE_NAME_PREFIX,
				today,
				Constants.BOTCHAN_LOGFILE_EXTENSION);

			try
			{
				// Mutexで排他制御しながらファイル書き込み
				using (var mtx = new Mutex(false, filePath.Replace("\\", "_") + ".FileWrite"))
				{
					try
					{
						mtx.WaitOne();

						using (var sw = new StreamWriter(filePath, true, Encoding.GetEncoding(Constants.BOTCHAN_LOGFILE_ENCODING)))
						{
							var line = string.Format(
								"[{0}] {1:yyyy年M月d日HH:mm:ss}　{2}",
								Constants.BOTCHAN_API_LOGFILE_NAME_PREFIX,
								DateTime.Now,
								message);
							sw.WriteLine(line);
						}

						var prefix = string.Format("{0}_{1}", Constants.BOTCHAN_API_LOGFILE_NAME_PREFIX, today);
						ForkFile(filePath, prefix, Constants.BOTCHAN_LOGFILE_EXTENSION);
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
			var file = new FileInfo(filePath);
			if (Constants.BOTCHAN_API_LOGFILE_THRESHOLD < 0) return;
			if (file.Length < (Constants.BOTCHAN_API_LOGFILE_THRESHOLD * 1024 * 1024)) return;

			var fileName = string.Format("{0}_*", prefix);
			var pattern = string.Format("{0}_{1}.{2}$", prefix, "(?<number>[0-9]+)", suffix);

			var max = Directory.GetFiles(Constants.DIRECTORY_BOTCHAN_API_LOGFILEPATH, fileName)
				.Select(item => Regex.Match(item, pattern))
				.Where(item => item.Success)
				.Select(item => int.Parse(item.Groups["number"].Value))
				.DefaultIfEmpty(0)
				.Max();
			var newFilePath = string.Format(
				"{0}{1}_{2}.{3}",
				Constants.DIRECTORY_BOTCHAN_API_LOGFILEPATH,
				prefix,
				max + 1,
				suffix);
			file.MoveTo(newFilePath);
		}

		/// <summary>
		/// 出力内容を取得
		/// </summary>
		/// <param name="httpStatusCode">ステータスコード</param>
		/// <param name="message">実行結果</param>
		/// <param name="param">リクエストデータ</param>
		/// <param name="response">レスポンスデータ</param>
		/// <returns>出力内容</returns>
		public static string GetOutPut(int httpStatusCode, string message, string param, string response)
		{
			var status = string.Format("[{0}] {1}", httpStatusCode, message);
			var details = string.Format(
				"{0}[RequestParam]{0}{1}{0}[Response]{0}{2}",
				Environment.NewLine,
				param,
				response);
			switch (Constants.BOTCHAN_API_LOG_LEVEL)
			{
				case Constants.BOTCHAN_API_FLG_LOG_LEVEL_INFO:
					return status;

				case Constants.BOTCHAN_API_FLG_LOG_LEVEL_ERROR:
					return (httpStatusCode == 200) ? status : status + details;

				default:
					return status + details;
			}
		}
	}
}
