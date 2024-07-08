/*
=========================================================================================================
  Module      : アクセスログ加工プロセスクラス(P02_ProcessAccessLog.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;

namespace w2.MarketingPlanner.Batch.AccessLogImporter.Process
{
	class P02_ProcessAccessLog : P00_BaseProcess
	{
		/// <summary>
		/// 加工処理
		/// </summary>
		/// <param name="blTargetPc">ＰＣ取込可否</param>
		public void ProcessAll(bool blTargetPc)
		{
			string strProcessName = "";
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				try
				{
					// コネクションオープン
					sqlAccessor.OpenConnection();

					//------------------------------------------------------
					// １．加工開始ステータス更新
					//------------------------------------------------------
					strProcessName = "２．加工開始ステータス更新";
					string strTargetDate = null;
					{
						DataView dvTargetDate = null;
						using (SqlStatement sqlStatement = new SqlStatement("ProcessAccessLog", "UpdateProcessBgnStatusAndGetTargetDate"))
						{
							dvTargetDate = sqlStatement.SelectSingleStatement(sqlAccessor);
						}
						strTargetDate = (string)dvTargetDate[0][0];
					}

					// 後々利用
					Dictionary<string, string> dicInputTargetDate = new Dictionary<string, string>();
					dicInputTargetDate.Add(Constants.FIELD_ACCESSLOGSTATUS_TARGET_DATE, strTargetDate);

					//------------------------------------------------------
					// ２．加工処理
					//------------------------------------------------------
					strProcessName = "３．加工開始ステータス更新";

					// トランザクション開始
					sqlAccessor.BeginTransaction();

					// 各ストアド実行
					try
					{
						using (SqlStatement sqlStatement = new SqlStatement(""))
						{
							sqlStatement.CommandTimeout = Constants.SQLTIMEOUT_SEC * 5;	// ストアドによっては時間がかかることが多い
							sqlStatement.AddInputParameters("target_date", SqlDbType.NVarChar, 10);

							foreach (string strSetting in Constants.PROCESSACCESSLOGSETTINGS)
							{
								string[] strSplittedSetting = strSetting.Split(':');
								string strKbn = strSplittedSetting[0].ToUpper().Trim();

								// 実行判定
								var blExec = false;
								switch (strKbn)
								{
									case "PM":
									case "PC":
										blExec = blTargetPc;
										break;

									default:
										throw new Exception("不明な区分:" + strSetting);
								}

								// 実行
								if (blExec)
								{
									strProcessName = "３－Ｘ．" + strSplittedSetting[1].Trim();

									sqlStatement.Statement = strSplittedSetting[2].Trim();
									sqlStatement.ExecStatement(sqlAccessor, dicInputTargetDate);
								}
							}
						}

						// トランザクションコミット
						sqlAccessor.CommitTransaction();
					}
					catch (Exception)
					{
						// トランザクションロールバック
						sqlAccessor.RollbackTransaction();
						throw;
					}

					//------------------------------------------------------
					// ３．加工終了ステータス更新
					//------------------------------------------------------
					strProcessName = "３．加工終了ステータス更新";
					{
						using (SqlStatement sqlStatement = new SqlStatement("ProcessAccessLog", "UpdateProcessEndStatus"))
						{
							sqlStatement.ExecStatement(sqlAccessor, dicInputTargetDate);
						}
					}

					//------------------------------------------------------
					// ４．テーブルクリーニング
					//------------------------------------------------------
					strProcessName = "4．テーブルクリーニング";
					{
						if (blTargetPc)
						{
							using (SqlStatement sqlStatement = new SqlStatement("ImportW3cLog", "ClearPCLog"))
							{
								sqlStatement.CommandTimeout = Constants.SQLTIMEOUT_SEC;
								sqlStatement.ExecStatement(sqlAccessor);
							}
						}
					}

					// トランザクションコミット
					sqlAccessor.CommitTransaction();

					this.ProcessResult = PROCESS_RESULT.SUCCESS;
				}
				catch (Exception ex)
				{
					this.ProcessResult = PROCESS_RESULT.FAILED;
					this.ProcessException = new Exception(GetType().Name + " " + strProcessName + " でエラーが発生しました。\r\n", ex);
				}

			} // using (SqlAccessor sqlAccessor = new SqlAccessor())
		}
	}
}
