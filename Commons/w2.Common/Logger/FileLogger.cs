/*
=========================================================================================================
  Module      : ファイルロガー(FileLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading;

namespace w2.Common.Logger
{
	///*********************************************************************************************
	/// <summary>
	/// 各種ファイルログを出力する
	/// </summary>
	///*********************************************************************************************
	public partial class FileLogger : BaseLogger
	{
		/// <summary>
		/// スタティックコンストラクタ
		/// </summary>
		static FileLogger()
		{
			//------------------------------------------------------
			// ログディレクトリ名補正
			//------------------------------------------------------
			if (Constants.PHYSICALDIRPATH_LOGFILE.EndsWith(@"\") == false)
			{
				// 排他制御
				lock (Constants.PHYSICALDIRPATH_LOGFILE)
				{
					// 念のため二重チェック
					if (Constants.PHYSICALDIRPATH_LOGFILE.EndsWith(@"\") == false)
					{
						Constants.PHYSICALDIRPATH_LOGFILE += @"\";
					}
				}
			}

			//------------------------------------------------------
			// ログディレクトリ作成
			//------------------------------------------------------
			if (Directory.Exists(Constants.PHYSICALDIRPATH_LOGFILE) == false)
			{
				Directory.CreateDirectory(Constants.PHYSICALDIRPATH_LOGFILE);
			}
		}

		/// <summary>
		/// ログ書き込み処理
		/// </summary>
		/// <param name="strLogKbn">ログ区分</param>
		/// <param name="strMessage">メッセージ</param>
		/// <param name="monthly">月ごとローテーション（指定なければ日ごと）</param>
		/// <param name="encoding">Encoding</param>
		public static void Write(
			string strLogKbn,
			string strMessage,
			bool monthly = false,
			Encoding encoding = null)
		{
			Write(strLogKbn, strMessage, Constants.PHYSICALDIRPATH_LOGFILE, monthly, encoding);
		}
		
		/// <summary>
		/// ログ書き込み処理（ディレクトリパス指定可能）
		/// </summary>
		/// <param name="strLogKbn">ログ区分</param>
		/// <param name="strMessage">メッセージ</param>
		/// <param name="directoryPath">ディレクトリパス</param>
		/// <param name="monthly">月ごとローテーション（指定なければ日ごと）</param>
		/// <param name="encoding">Encoding</param>
		public static void Write(
			string strLogKbn,
			string strMessage,
			string directoryPath,
			bool monthly = false,
			Encoding encoding = null)
		{
			//------------------------------------------------------
			// 対応しない場合は抜ける（「*」は全てOK）
			//------------------------------------------------------
			if ((KBN_LOGOUTPUT_SETTINGS.Contains(BaseLogger.LOGKBN_WILDCARD) == false)
				&& (KBN_LOGOUTPUT_SETTINGS.Contains(strLogKbn) == false))
			{
				return;
			}

			//------------------------------------------------------
			// ログファイルパス決定
			//------------------------------------------------------
			var sbLogFilePath = new StringBuilder();
			sbLogFilePath
				.Append(directoryPath)
				.Append(strLogKbn)
				.Append("_")
				.Append(DateTime.Now.ToString(monthly ? "yyyyMM" : "yyyyMMdd"))
				.Append(".")
				.Append(Constants.LOGFILE_EXTENSION);

			if (strLogKbn == Constants.LOGFILE_NAME_OPERATIONLOG_NOT_SEND)
			{
				encoding = new UTF8Encoding(false);
			}
			encoding = encoding ?? Encoding.GetEncoding(Constants.LOGFILE_ENCODING);

			//------------------------------------------------------
			// ログ書き込み
			//------------------------------------------------------
			try
			{
				// Mutexで排他制御しながらファイル書き込み
				using (Mutex mtx = new Mutex(false, sbLogFilePath.ToString().Replace("\\", "_") + ".FileWrite"))
				{
					try
					{
						mtx.WaitOne();

						// ファイル書き込み
						using (var sw = new StreamWriter(sbLogFilePath.ToString(), true, encoding))
						{
							var sbMessage = new StringBuilder();
							if (strLogKbn == Constants.LOGFILE_NAME_OPERATIONLOG_NOT_SEND)
							{
								sbMessage
								.Append(strMessage);
								sw.WriteLine(sbMessage.ToString());
							}
							else
							{
								sbMessage
								.Append("[").Append(strLogKbn).Append("] ")
								.Append(DateTime.Now.ToString(monthly ? "yyyy/MM/dd HH:mm:ss" : "yyyy年M月d日HH:mm:ss"))
								.Append("　")
								.Append(strMessage);
								sw.WriteLine(sbMessage.ToString());
							}
						}
					}
					finally
					{
						mtx.ReleaseMutex();	// Dispose()で呼ばれない模様。
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
			}
		}
	}
}
