/*
=========================================================================================================
  Module      : データベースロガー(DbLogger.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using w2.Common.Util.Security;
using w2.ExternalAPI.Common.FrameWork.DB;

namespace w2.ExternalAPI.Common.Logging
{
	
	/// <summary>
	///	データベースロガー
	/// </summary>
	/// <remarks>
	/// ログ情報をデータベースに登録する
	/// </remarks>
	public class DbLogger : ILogger
	{
		
		#region #Write ログ書き込み
		/// <summary>ログ書き込み</summary>
		///<param name="executedTime"> </param>
		///<param name="level"> </param>
		///<param name="source"> </param>
		///<param name="message"> </param>
		///<param name="data"> </param>
		///<param name="ex"> </param>
		internal void Write(
			DateTime executedTime,
			LogLevel level,
			string source,
			string message,
			string data,
			Exception ex)
		{

			// ログ書き込みDataの暗号化
			string writeData = EncryptLogData(data);
			
			// ｗ２標準ログに出力
			if(ex != null)
			{
				// DBログへ書き込み(Exception情報あり)
				InsertDb(executedTime, level, source, ex.StackTrace, message,writeData, ex.Message);

				// Exceptionが指定された場合のみイベントログに書き込む
				w2.Common.Logger.AppLogger.Write(Enum.GetName(typeof(LogLevel), level), message, ex);
			}
			else
			{
				// DBログへ書き込み(Exception情報なし)
				InsertDb(executedTime, level, source, Environment.StackTrace, message, writeData,"");
			}
			
		}
		/// <summary>
		/// ログ書き込み（インターフェースのオーバーライド）
		/// </summary>
		/// <param name="executedTime"></param>
		/// <param name="level"></param>
		/// <param name="source"></param>
		/// <param name="message"></param>
		/// <param name="data"></param>
		/// <param name="ex"></param>
		void ILogger.Write(
				DateTime executedTime,
				LogLevel level,
				string source,
				string message,
				string data,
				Exception ex)
		{
			this.Write(executedTime, level, source, message, data, ex);
		}
		#endregion

		#region -InsertDB DBログInsert
		/// <summary>DBログInsert</summary>
		///<param name="excutedTime"> </param>
		///<param name="level"> </param>
		///<param name="source"> </param>
		///<param name="stackTrace"> </param>
		///<param name="message"> </param>
		///<param name="data"> </param>
		///<param name="exception"> </param>
		private void InsertDb(DateTime excutedTime, LogLevel level,string source, string stackTrace, string message, string data, string exception)
		{
			
			using (SqlAccesorWrapper sqlAccessorwWrapper = new SqlAccesorWrapper())
			using (SqlStatementWrapper sqlStatementWrapper = new SqlStatementWrapper("ExternalApi", "InsertLog"))
			{
				Hashtable htInput = new Hashtable
				                    	{
				                    		{"date_excuted", excutedTime },
				                    		{"log_level", Enum.GetName(typeof(LogLevel), level)},
											{"source", "" + source},
											{"stacktrace", stackTrace },
				                    		{"message", message},
				                    		{"data",data },	
				                    		{"exception",exception},
				                    	};

				sqlStatementWrapper.ExecStatementWithOC(sqlAccessorwWrapper, htInput);

			}
		}
		#endregion

		#region -EncryptLogData 暗号化
		/// <summary>
		/// 暗号化
		/// </summary>
		/// <param name="data">暗号化対象の文字（ログに書き込むデータ内容）</param>
		/// <returns>指定されたものを暗号化した結果の文字列</returns>
		private string EncryptLogData(string data)
		{	
			// 暗号化クラスのインスタンス化
			RijndaelCrypto rijndaelCrypto =
				new RijndaelCrypto(w2.Common.Constants.ENCRYPTION_USER_PASSWORD_KEY, w2.Common.Constants.ENCRYPTION_USER_PASSWORD_IV);

			return rijndaelCrypto.Encrypt(data);

		}
		#endregion

	}
}
