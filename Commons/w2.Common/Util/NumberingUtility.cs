/*
=========================================================================================================
  Module      : 採番モジュール(NumberingUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using w2.Common.Sql;

namespace w2.Common.Util
{
	///**************************************************************************************
	/// <summary>
	/// 採番テーブルを利用し新たな番号を発行する
	/// </summary>
	///**************************************************************************************
	public class NumberingUtility
	{
		const string FILELD_NUMBER_DEPT_ID = "dept_id";
		const string FILELD_NUMBER_NUMBER_CODE = "number_code";
		const string FILELD_NUMBER_NUMBER = "number";
		const string FILELD_NUMBER_DATE_CHANGED = "date_changed";

		const string SELECT = "SELECT	number, \n"
							+ "			date_changed \n"
							+ "	 FROM	w2_Number WITH (UPDLOCK) \n"
							+ "	WHERE	dept_id = @dept_id \n"
							+ "	  AND	number_code = @number_code \n";

		const string UPDATE = "UPDATE	w2_Number \n"
							+ "	  SET	number = @number, \n"
							+ "			date_changed = @date_changed \n"
							+ " WHERE	dept_id = @dept_id \n"
							+ "   AND	number_code = @number_code \n";

		const string INSERT = "INSERT	w2_Number \n"
							+ "			( \n"
							+ "			dept_id, \n"
							+ "			number_code, \n"
							+ "			number, \n"
							+ "			date_changed \n"
							+ "			) \n"
							+ "	VALUES	( \n"
							+ "			@dept_id, \n"
							+ "			@number_code, \n"
							+ "			@number, \n"
							+ "			@date_changed \n"
							+ "			) \n";

		/// <summary>リセットタイミング</summary>
		public enum ResetTiming
		{
			/// <summary>なし</summary>
			None,
			/// <summary>年</summary>
			Year,
			/// <summary>月</summary>
			Month,
			/// <summary>日</summary>
			Day
		};

		/// <summary>
		/// 新番号生成
		/// </summary>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strKey">採番キー</param>
		/// <returns>新番号</returns>
		public static long CreateNewNumber(string strDeptId, string strKey)
		{
			return CreateNewNumber(strDeptId, strKey, ResetTiming.None);
		}
		/// <summary>
		/// 新番号生成
		/// </summary>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strKey">採番キー</param>
		/// <param name="rtTiming">リセットタイミング</param>
		/// <returns>新番号</returns>
		public static long CreateNewNumber(string strDeptId, string strKey, ResetTiming rtTiming)
		{
			DateTime dtNow = DateTime.Now;
			long lNewNumber = 0;

			// WEBアクセサ生成
			// この中ではテーブルに更新ロックをかけつつ、旧番号取得→新番号生成→新番号保存の処理を行っている。
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				try
				{
					//------------------------------------------------------
					// コネクションオープン・トランザクション開始
					//------------------------------------------------------
					sqlAccessor.OpenConnection();
					sqlAccessor.BeginTransaction(System.Data.IsolationLevel.Serializable);	// 直列化（SERIALIZABLE） 

					//------------------------------------------------------
					// 旧番号取得
					//------------------------------------------------------
					// パラメタ設定
					SqlCommand sqlCommand = new SqlCommand();
					sqlCommand.CommandText = SELECT;
					sqlCommand.Parameters.Add("@" + FILELD_NUMBER_DEPT_ID, SqlDbType.NVarChar, 30);
					sqlCommand.Parameters.Add("@" + FILELD_NUMBER_NUMBER_CODE, SqlDbType.NVarChar, 30);
					sqlCommand.Parameters["@" + FILELD_NUMBER_DEPT_ID].Value = strDeptId;
					sqlCommand.Parameters["@" + FILELD_NUMBER_NUMBER_CODE].Value = strKey;

					// 旧番号取得
					DataView dvResult = sqlAccessor.SelectSingle(sqlCommand);

					// データあり？
					if (dvResult.Count != 0)
					{
						// 数リセット
						bool blResetNumber = false;
						DateTime dtLastUpdate = (DateTime)dvResult[0].Row[FILELD_NUMBER_DATE_CHANGED];
						switch (rtTiming)
						{
							case ResetTiming.None:
								break;

							case ResetTiming.Year:
								if (dtLastUpdate.Year != dtNow.Year)
								{
									blResetNumber = true;
								}
								break;

							case ResetTiming.Month:
								if ((dtLastUpdate.Year != dtNow.Year)
									|| (dtLastUpdate.Month != dtNow.Month))
								{
									blResetNumber = true;
								}
								break;

							case ResetTiming.Day:
								if ((dtLastUpdate.Year != dtNow.Year)
									|| (dtLastUpdate.Month != dtNow.Month)
									|| (dtLastUpdate.Day != dtNow.Day))
								{
									blResetNumber = true;
								}
								break;
						}

						// 必要あればリセットし、新番号発行
						long lOldNumber = blResetNumber ? 0 : (long)dvResult[0].Row[FILELD_NUMBER_NUMBER];
						lNewNumber = lOldNumber + 1;

						//------------------------------------------------------
						// アップデート(sqlCommand使いまわし)
						//------------------------------------------------------
						sqlCommand.CommandText = UPDATE;
						sqlCommand.Parameters.Add("@" + FILELD_NUMBER_NUMBER, SqlDbType.BigInt);
						sqlCommand.Parameters["@" + FILELD_NUMBER_NUMBER].Value = lNewNumber;
						sqlCommand.Parameters.Add("@" + FILELD_NUMBER_DATE_CHANGED, SqlDbType.DateTime);
						sqlCommand.Parameters["@" + FILELD_NUMBER_DATE_CHANGED].Value = dtNow;

						sqlAccessor.Execute(sqlCommand);
					}
					// データなし？
					else
					{
						lNewNumber = 1;

						//------------------------------------------------------
						// インサート(sqlCommand使いまわし)
						//------------------------------------------------------
						sqlCommand.CommandText = INSERT;
						sqlCommand.Parameters.Add("@" + FILELD_NUMBER_NUMBER, SqlDbType.BigInt);
						sqlCommand.Parameters["@" + FILELD_NUMBER_NUMBER].Value = lNewNumber;
						sqlCommand.Parameters.Add("@" + FILELD_NUMBER_DATE_CHANGED, SqlDbType.DateTime);
						sqlCommand.Parameters["@" + FILELD_NUMBER_DATE_CHANGED].Value = dtNow;

						sqlAccessor.Execute(sqlCommand);
					}

					//------------------------------------------------------
					// トランザクションコミット
					//------------------------------------------------------
					sqlAccessor.CommitTransaction();
				}
				catch (Exception)
				{
					// エラー時はロールバック
					sqlAccessor.RollbackTransaction();
					throw;
				}
			}

			// 新番号を返す
			return lNewNumber;
		}

		/// <summary>
		/// Create key id
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="key">採番キー</param>
		/// <param name="lengthKey">Length key</param>
		/// <returns>Key Id</returns>
		public static string CreateKeyId(string deptId, string key, int lengthKey)
		{
			return CreateNewNumber(deptId, key).ToString().PadLeft(lengthKey, '0');
		}
	}
}
