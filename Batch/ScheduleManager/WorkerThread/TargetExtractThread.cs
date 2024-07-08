/*
=========================================================================================================
  Module      : ターゲット抽出スレッドクラス(TargetExtractThread.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Threading;
using System.Text;
using System.Xml;
using w2.Common.Sql;
using w2.App.Common.TargetList;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	class TargetExtractThread : BaseThread
	{
		// 継承済み
		// private string m_strDeptId = null;
		// private string m_strActionKbn = null;
		// private string m_strMastertId = null;
		// private int m_iActionNo = -1;

		//=========================================================================================
		/// <summary>
		/// スレッド作成（アクション実行）
		/// </summary>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strTargetId">ターゲットID</param>
		/// <param name="iActionNo">アクションNO</param>
		/// <param name="blUpdateTaskStatus">タスクステータス更新可否</param>
		/// <returns>生成スレッド</returns>
		//=========================================================================================
		public static TargetExtractThread CreateAndStart(string strDeptId, string strTargetId)
		{
			return CreateAndStart(DateTime.Now, strDeptId, strTargetId, -1);
		}
		//=========================================================================================
		/// <summary>
		/// スレッド作成（タスクスケジュール実行）
		/// </summary>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strTargetId">ターゲットID</param>
		/// <param name="iActionNo">アクションNO</param>
		/// <param name="blUpdateTaskStatus">タスクステータス更新可否</param>
		/// <returns>生成スレッド</returns>
		//=========================================================================================
		public static TargetExtractThread CreateAndStart(DateTime dtScheduleDate, string strDeptId, string strTargetId, int iActionNo)
		{
			// スレッド作成
			var targetExtractThread = new TargetExtractThread(dtScheduleDate, strDeptId, strTargetId, iActionNo);
			targetExtractThread.Thread = new Thread(new ThreadStart(targetExtractThread.Work));

			// スレッドスタート
			targetExtractThread.Thread.Start();

			return targetExtractThread;
		}

		//=========================================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strTargetId">ターゲットID</param>
		/// <param name="iActionNo">アクションNO</param>
		/// <param name="blUpdateTaskStatus">タスクステータス更新可否</param>
		//=========================================================================================
		public TargetExtractThread(DateTime dtScheduleDate, string strDeptId, string strTargetId, int iActionNo)
			: base(dtScheduleDate, strDeptId, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST, strTargetId, iActionNo)
		{
		}

		//=========================================================================================
		/// <summary>
		/// ターゲットリスト抽出
		/// </summary>
		//=========================================================================================
		public void Work()
		{
			try
			{
				//------------------------------------------------------
				// ターゲット抽出タスクステータス更新(開始)
				//------------------------------------------------------
				if (this.ActionNo != -1)
				{
					int iUpdate = UpdateTaskStatusBegin(
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT,
						"");
				}

				//------------------------------------------------------
				// ターゲットリスト設定取得（同時にステータス更新、取得できるまで待つ）
				//------------------------------------------------------
				DataRowView drvTargetList = GetTargetListAndUpdateStatus(this.MasterId, Constants.FLG_TARGETLIST_STATUS_EXTRACT);

				//------------------------------------------------------
				// 処理開始宣言
				//------------------------------------------------------
				Form1.WriteInfoLogLine("ターゲットリスト[" + this.MasterId + "]抽出開始");

				//------------------------------------------------------
				// 条件解析
				//------------------------------------------------------
				int iCreateCount = 0;
				switch ((string)drvTargetList[Constants.FIELD_TARGETLIST_TARGET_TYPE])
				{
					//------------------------------------------------------
					// ユーザリストのターゲットリスト
					//------------------------------------------------------
					case Constants.FLG_TARGETLIST_TARGET_TYPE_MANUAL:
						using (SqlAccessor sqlAccessor = new SqlAccessor())
						{
							sqlAccessor.OpenConnection();

							// ターゲットリストデータ削除
							TargetListUtility.DeleteTargetListData(this.DeptId, this.MasterId, sqlAccessor);

							try
							{
								// 抽出
								iCreateCount = TargetListUtility.CreateTargetListData(
									this.DeptId,
									this.MasterId,
									(string)drvTargetList[Constants.FIELD_TARGETLIST_TARGET_CONDITION],
									sqlAccessor,
									Constants.AGGREGATE_SQL_TIME_OUT);
							}
							catch (Exception ex)
							{
								w2.Common.Logger.FileLogger.WriteError(ex);
								Form1.WriteInfoLogLine(ex.ToString());
							}
						}

						break;

					case Constants.FLG_TARGETLIST_TARGET_TYPE_CSV:
					case Constants.FLG_TARGETLIST_TARGET_TYPE_MERGE:
					case Constants.FLG_TARGETLIST_TARGET_TYPE_USER_LIST:
					case Constants.FLG_TARGETLIST_TARGET_TYPE_ORDER_LIST:
					case Constants.FLG_TARGETLIST_TARGET_TYPE_ORDERWORKFLOW_LIST:
					case Constants.FLG_TARGETLIST_TARGET_TYPE_FIXEDPURCHASE_LIST:
						int.TryParse(drvTargetList[Constants.FIELD_TARGETLIST_DATA_COUNT].ToString(), out iCreateCount);
						break;
				}

				//------------------------------------------------------
				// 処理終了宣言
				//------------------------------------------------------
				Form1.WriteInfoLogLine("ターゲットリスト[" + this.MasterId + "]抽出完了：" + iCreateCount.ToString() + "件");

				//------------------------------------------------------
				// ターゲットリストステータス更新（通常状態へ）
				//------------------------------------------------------
				UpdateTargetListStatus(this.MasterId, Constants.FLG_TARGETLIST_STATUS_NORMAL, iCreateCount);

				//------------------------------------------------------
				// ターゲット抽出タスクステータス更新(完了)
				//------------------------------------------------------
				if (this.ActionNo != -1)
				{
					var executeStatus = (Constants.TARGET_LIST_IMPORT_TYPE_LIST.Contains((string)drvTargetList[Constants.FIELD_TARGETLIST_TARGET_TYPE]))
						? Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_SKIP
						: Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE;
					int iUpdate = UpdateTaskStatusEnd(
						executeStatus,
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
						iCreateCount + "/" + iCreateCount.ToString());
				}
			}
			catch (Exception ex)
			{
				w2.Common.Logger.FileLogger.WriteError(ex);
				Form1.WriteInfoLogLine(ex.ToString());

				//------------------------------------------------------
				// ターゲットリストステータス更新（エラー）
				//------------------------------------------------------
				try
				{
					UpdateTargetListStatus(
						this.MasterId,
						Constants.FLG_TARGETLIST_STATUS_ERROR, null);
				}
				catch (Exception ex2)
				{
					w2.Common.Logger.FileLogger.WriteError(ex2);
					Form1.WriteInfoLogLine(ex2.ToString());
				}

				//------------------------------------------------------
				// ターゲット抽出タスクステータス更新(エラー)
				//------------------------------------------------------
				UpdateTaskStatusEnd(
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_ERROR,
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
					"");
			}
		}
	}
}
