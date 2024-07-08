/*
=========================================================================================================
  Module      : YahooApiLogger Logger (YahooApiLogger.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using w2.Common.Logger;

namespace w2.App.Common.Mall.Yahoo.Foundation
{
	/// <summary>
	/// ロガークラス
	/// </summary>
	internal class YahooApiLogger : FileLogger
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public YahooApiLogger()
		{
			this.LogDirectoryPath = Constants.PHYSICALDIRPATH_LOGFILE;
			this.LogFilePath = string.Format(
				"{0}/{1}_{2}.{3}",
				this.LogDirectoryPath,
				Constants.YAHOO_API,
				DateTime.Now.ToString(Constants.DATE_FORMAT_SHORT),
				Constants.LOGFILE_EXTENSION);
		}

		/// <summary>
		/// リクエストログ書き込み
		/// </summary>
		/// <param name="url">URL</param>
		/// <param name="reqestData">リクエストデータ</param>
		public void WriteRequest(string url, string reqestData)
		{
			var reqestLogMessage = string.Format("URL:{0}\r\nRequest Param\r\n{1}", url, reqestData);
			Write(reqestLogMessage);
		}

		/// <summary>
		/// レスポンスログ書き込み
		/// </summary>
		/// <param name="responseData">レスポンスデータ</param>
		public void WriteResponse(string responseData)
		{
			var reqestLogMessage = string.Format("Response Param\r\n{0}", responseData);
			Write(reqestLogMessage);
		}

		/// <summary>
		/// ログ書き込み処理（ディレクトリパス指定可能）
		/// </summary>
		/// <param name="message">メッセージ</param>
		private void Write(string message)
		{
			if (Directory.Exists(this.LogDirectoryPath) == false)
			{
				Directory.CreateDirectory(this.LogDirectoryPath);
			}
			FileLogger.Write(Constants.YAHOO_API, message, this.LogDirectoryPath);
		}

		/// <summary>ログファイルパス</summary>
		private string LogFilePath { get; set; }
		/// <summary>ログファイルディレクトリ</summary>
		private string LogDirectoryPath { get; set; }
	}
}
