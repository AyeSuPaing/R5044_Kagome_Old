/*
=========================================================================================================
  Module      : OPlux Logger(OPluxLogger.cs)
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
using w2.Common.Helper;
using w2.Common.Helper.Attribute;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.App.Common.Order.OPlux
{
	/// <summary>
	/// O-PLUX Logger
	/// </summary>
	public class OPluxLogger : FileLogger
	{
		/// <summary>
		/// O-PLUX processing type
		/// </summary>
		public enum OPluxProcessingType
		{
			/// <summary>API request begin</summary>
			[EnumTextName("API Request Begin")]
			ApiRequestBegin,
			/// <summary>API request end</summary>
			[EnumTextName("API Request End")]
			ApiRequestEnd,
			/// <summary>API request error</summary>
			[EnumTextName("API Request Error")]
			ApiRequestError,
		}

		/// <summary>
		/// Write O-PLUX log
		/// </summary>
		/// <param name="processingContent">Processing content</param>
		/// <param name="externalLog">External log</param>
		public static void WriteOPluxLog(OPluxProcessingType processingContent, string externalLog)
		{
			if (Directory.Exists(Constants.DIRECTORY_OPLUX_API_LOGFILEPATH) == false)
			{
				Directory.CreateDirectory(Constants.DIRECTORY_OPLUX_API_LOGFILEPATH);
			}

			var today = DateTime.Now.ToString("yyyyMMdd");
			var filePath = string.Format(
				"{0}{1}_{2}.{3}",
				Constants.DIRECTORY_OPLUX_API_LOGFILEPATH,
				Constants.OPLUX_API_LOGFILE_NAME_PREFIX,
				today,
				Constants.OPLUX_LOGFILE_EXTENSION);

			try
			{
				using (var mutex = new Mutex(false, filePath.Replace("\\", "_") + ".FileWrite"))
				{
					try
					{
						mutex.WaitOne();
						using (var streamWriter = new StreamWriter(
							filePath,
							true,
							Encoding.GetEncoding(Constants.OPLUX_LOGFILE_ENCODING)))
						{
							var result = string.Format(
								"[{0}] {1:yyyy年M月d日HH:mm:ss} {2}: {3}",
								Constants.OPLUX_API_LOGFILE_NAME_PREFIX,
								DateTime.Now,
								processingContent.ToText(),
								(string.IsNullOrEmpty(externalLog)
									? string.Empty
									: externalLog.Replace("\r\n", "\t")));

							streamWriter.WriteLine(result);
						}

						var prefix = string.Format("{0}_{1}", Constants.OPLUX_API_LOGFILE_NAME_PREFIX, today);
						ForkFile(filePath, prefix, Constants.OPLUX_LOGFILE_EXTENSION);
					}
					finally
					{
						mutex.ReleaseMutex();
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(StringUtility.ToEmpty(ex));
			}
		}

		/// <summary>
		/// Fork file
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <param name="prefix">Prefix</param>
		/// <param name="suffix">Suffix</param>
		public static void ForkFile(string filePath, string prefix, string suffix)
		{
			var file = new FileInfo(filePath);

			if ((Constants.OPLUX_API_LOGFILE_THRESHOLD < 0)
				|| (file.Length < (Constants.OPLUX_API_LOGFILE_THRESHOLD * 1024 * 1024)))
			{
				return;
			}

			var fileName = string.Format("{0}_*", prefix);
			var pattern = string.Format("{0}_{1}.{2}$", prefix, "(?<number>[0-9]+)", suffix);

			var max = Directory.GetFiles(Constants.DIRECTORY_OPLUX_API_LOGFILEPATH, fileName)
				.Select(item => Regex.Match(item, pattern))
				.Where(item => item.Success)
				.Select(item => int.Parse(item.Groups["number"].Value))
				.DefaultIfEmpty(0)
				.Max();

			var newFilePath = string.Format(
				"{0}{1}_{2}.{3}",
				Constants.DIRECTORY_OPLUX_API_LOGFILEPATH,
				prefix,
				(max + 1),
				suffix);

			file.MoveTo(newFilePath);
		}
	}
}
