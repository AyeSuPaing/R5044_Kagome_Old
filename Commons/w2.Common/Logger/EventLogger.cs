/*
=========================================================================================================
  Module      : イベントロガー(EventLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace w2.Common.Logger
{
	///*********************************************************************************************
	/// <summary>
	/// 各種イベントログを出力する
	/// </summary>
	///*********************************************************************************************
	public partial class EventLogger : BaseLogger
	{
		/// <summary>
		/// イベントログ出力
		/// </summary>
		/// <param name="strLogKbn">ログ区分</param>
		/// <param name="strMessage">出力メッセージ</param>
		public static void Write(string strLogKbn, string strMessage)
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
			// イベントログエントリータイプ決定
			//------------------------------------------------------
			EventLogEntryType? elType = null;
			switch (strLogKbn)
			{
				case LOGKBN_DEBUG:
					elType = null;	// 出力しない
					break;

				case LOGKBN_INFO:
					elType = EventLogEntryType.Information;
					break;

				case LOGKBN_WARN:
					elType = EventLogEntryType.Warning;
					break;

				case LOGKBN_ERROR:
					elType = EventLogEntryType.Error;
					break;

				case LOGKBN_FATAL:
					elType = EventLogEntryType.Error;	// エラーとして出力
					break;

				default:
					elType = null;	// 出力しない
					break;
			}

			//------------------------------------------------------
			// 書き込みログファイル名決定
			//------------------------------------------------------
			Write(elType, strMessage);
		}
		/// <summary>
		/// イベントログ出力（出力できなかった場合は例外）
		/// </summary>
		/// <param name="elType">ログエントリータイプ</param>
		/// <param name="strMessage">出力メッセージ</param>
		public static void Write(EventLogEntryType? elType, string strMessage)
		{
			if (elType.HasValue)
			{
				try
				{
					if (Constants.APPLICATION_NAME_WITH_PROJECT_NO != "")
					{
						// イベントログ出力
						using (EventLog elEventLog = new EventLog())
						{
							elEventLog.Source = Constants.APPLICATION_NAME_WITH_PROJECT_NO;
							elEventLog.WriteEntry(strMessage, elType.Value);
						}
					}
				}
				catch (Exception)
				{
					// エラー時は何もしない
				}
			}
		}
	}
}
