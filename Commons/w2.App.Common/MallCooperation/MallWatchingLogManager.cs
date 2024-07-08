/*
=========================================================================================================
  Module      : モール監視ログマネージャークラス(MallWatchingLogManager.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Sql;

namespace w2.App.Common.MallCooperation
{
	/// <summary>
	/// モール監視ログマネージャ
	/// </summary>
	public class MallWatchingLogManager
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MallWatchingLogManager()
		{
			// なにもしない
		}

		/// <summary>
		/// モール監視ログ登録（SQLアクセサ無）
		/// </summary>
		/// <param name="strBatchId">バッチID</param>
		/// <param name="strMallId">メールID</param>
		/// <param name="strLogKbn">ログ区分</param>
		/// <param name="strLogMessage">メッセージ</param>
		/// <param name="strLogContents1">コンテンツ1</param>
		/// <param name="strLogContents1">コンテンツ2</param>
		/// <param name="strLogContents1">コンテンツ3</param>
		/// <param name="strLogContents1">コンテンツ4</param>
		/// <param name="strLogContents1">コンテンツ5</param>
		/// <returns>取込可否</returns>
		public bool Insert(
			string strBatchId,
			string strMallId,
			string strLogKbn,
			string strLogMessage,
			string strLogContents1 = "",
			string strLogContents2 = "",
			string strLogContents3 = "",
			string strLogContents4 = "",
			string strLogContents5 = "")
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				return Insert(sqlAccessor, strBatchId, strMallId, strLogKbn, strLogMessage, strLogContents1, strLogContents2, strLogContents3, strLogContents4, strLogContents5);
			}
		}
		/// <summary>
		/// モール監視ログ登録（SQLアクセサ有）
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <param name="strBatchId">バッチID</param>
		/// <param name="strMallId">メールID</param>
		/// <param name="strLogKbn">ログ区分</param>
		/// <param name="strLogMessage">メッセージ</param>
		/// <param name="strLogContents1">コンテンツ1</param>
		/// <param name="strLogContents1">コンテンツ2</param>
		/// <param name="strLogContents1">コンテンツ3</param>
		/// <param name="strLogContents1">コンテンツ4</param>
		/// <param name="strLogContents1">コンテンツ5</param>
		/// <returns>取込可否</returns>
		public bool Insert(
			SqlAccessor sqlAccessor,
			string strBatchId,
			string strMallId,
			string strLogKbn,
			string strLogMessage,
			string strLogContents1 = "",
			string strLogContents2 = "",
			string strLogContents3 = "",
			string strLogContents4 = "",
			string strLogContents5 = "")
		{
			int iUpdated = 0;
			using (SqlStatement sqlStatement = CreateSqlStatement(CommandType.Insert))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLWATCHINGLOG_WATCHING_DATE, DateTime.Now.ToString("yyyy/MM/dd"));
				htInput.Add(Constants.FIELD_MALLWATCHINGLOG_WATCHING_TIME, DateTime.Now.ToString("H:mm:ss"));
				htInput.Add(Constants.FIELD_MALLWATCHINGLOG_BATCH_ID, strBatchId);
				htInput.Add(Constants.FIELD_MALLWATCHINGLOG_MALL_ID, strMallId);
				htInput.Add(Constants.FIELD_MALLWATCHINGLOG_LOG_KBN, strLogKbn);
				htInput.Add(Constants.FIELD_MALLWATCHINGLOG_LOG_MESSAGE, strLogMessage);
				htInput.Add(Constants.FIELD_MALLWATCHINGLOG_LOG_CONTENT1, strLogContents1);

				iUpdated = sqlStatement.ExecStatement(sqlAccessor, htInput);
			}
			return (iUpdated > 0);
		}

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="targetDate">対象日時</param>
		/// <returns></returns>
		public bool Delete()
		{
			return Delete(DateTime.Now.AddMonths(-3));	// ３か月前以前のモール監視ログを削除する（大量ログ蓄積回避）
		}
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="targetDate">対象日時</param>
		/// <returns></returns>
		public bool Delete(DateTime target)
		{
			int iDeleted;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = CreateSqlStatement(CommandType.Delete))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLWATCHINGLOG_WATCHING_DATE, target);

				iDeleted = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}
			return (iDeleted > 0);
		}

		/// <summary>
		/// SQLステートメント作成
		/// </summary>
		/// <param name="sqlStatement"></param>
		private SqlStatement CreateSqlStatement(CommandType commandType)
		{
			SqlStatement sqlStatement = null;
			switch (commandType)
			{
				case CommandType.Insert:
					sqlStatement = new SqlStatement(SQL_INSERT);
					sqlStatement.AddInputParameters("watching_date", System.Data.SqlDbType.NVarChar, 20);
					sqlStatement.AddInputParameters("watching_time", System.Data.SqlDbType.NVarChar, 20);
					sqlStatement.AddInputParameters("batch_id", System.Data.SqlDbType.NVarChar, 50);
					sqlStatement.AddInputParameters("mall_id", System.Data.SqlDbType.NVarChar, 30);
					sqlStatement.AddInputParameters("log_kbn", System.Data.SqlDbType.NVarChar, 10);
					sqlStatement.AddInputParameters("log_message", System.Data.SqlDbType.NText);
					sqlStatement.AddInputParameters("log_content1", System.Data.SqlDbType.NText);
					sqlStatement.AddInputParameters("log_content2", System.Data.SqlDbType.NText);
					sqlStatement.AddInputParameters("log_content3", System.Data.SqlDbType.NText);
					sqlStatement.AddInputParameters("log_content4", System.Data.SqlDbType.NText);
					sqlStatement.AddInputParameters("log_content5", System.Data.SqlDbType.NText);
					break;

				case CommandType.Delete:
					sqlStatement = new SqlStatement(SQL_DELETE);
					// タイムアウト時間を300秒に設定
					sqlStatement.CommandTimeout = 300;
					sqlStatement.AddInputParameters("watching_date", System.Data.SqlDbType.NVarChar, 20);
					break;
			}

			return sqlStatement;
		}

		/// <summary>
		/// コマンドタイプ
		/// </summary>
		private enum CommandType { Insert, Delete }

		/// <summary>
		/// SQL文字列「インサート」
		/// </summary>
		static string SQL_INSERT
			= "	INSERT  w2_MallWatchingLog "
			+ " ( "
			+ "		w2_MallWatchingLog.watching_date,  "
			+ "		w2_MallWatchingLog.watching_time,  "
			+ "		w2_MallWatchingLog.batch_id,  "
			+ "		w2_MallWatchingLog.mall_id,  "
			+ "		w2_MallWatchingLog.log_kbn,  "
			+ "		w2_MallWatchingLog.log_message, "
			+ "		w2_MallWatchingLog.log_content1 "
			+ " )  "
			+ " VALUES "
			+ " ( "
			+ "		@watching_date,  "
			+ "		@watching_time,  "
			+ "		@batch_id,  "
			+ "		@mall_id,  "
			+ "		@log_kbn, "
			+ "		@log_message, "
			+ "		@log_content1 "
			+ "	) ";

		/// <summary>
		/// SQL文字列「削除」
		/// </summary>
		static string SQL_DELETE
			= " DELETE  w2_MallWatchingLog "
			+ "  WHERE  w2_MallWatchingLog.watching_date <= @watching_date ";
	}
}
