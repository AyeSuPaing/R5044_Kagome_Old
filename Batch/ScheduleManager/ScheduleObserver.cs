/*
=========================================================================================================
  Module      : スケジュール監視スレッド(ScheduleObserver.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.MarketingPlanner.Win.ScheduleManager
{
	//*****************************************************************************************
	/// <summary>
	/// スケジュール監視スレッドクラス
	/// </summary>
	/// <remarks>１アプリ１スレッドのみ</remarks>
	//*****************************************************************************************
	class ScheduleObserver
	{
		private DateTime m_dtTaskScheduleCreateNext = new DateTime(0);

		private List<ScheduleQueue> m_lScheduleQueueList = new List<ScheduleQueue>();
		private const int SLEEP_MSEC = 5000;
		private Queue m_queue = new Queue();
		private Queue m_waitQueue = new Queue();

		private static bool m_blStopThread = true;

		/// <summary>ワーカースレッド格納リスト</summary>
		/// <remarks>
		/// 現状ワーカースレッドプールとしての利用をしていません。
		/// メール配信再開時に元スレッドがまだ生きていたらAbortするためにリストに格納することにしました。
		/// </remarks>
		private List<WorkerThread.BaseThread> m_workerThreads = new List<WorkerThread.BaseThread>();

		//=========================================================================================
		/// <summary>
		/// スレッド作成
		/// </summary>
		/// <returns>生成スレッド</returns>
		//=========================================================================================
		public static Thread CreateThread()
		{
			m_blStopThread = false;

			// スレッドスタート（無限ループ）
			Thread th = new Thread(new ThreadStart(new ScheduleObserver().Work));
			th.Start();

			return th;
		}

		//=========================================================================================
		/// <summary>
		/// スレッド停止命令
		/// </summary>
		//=========================================================================================
		public static void AbortThread()
		{
			m_blStopThread = true;
		}

		//=========================================================================================
		/// <summary>
		/// ルーチン
		/// </summary>
		//=========================================================================================
		public void Work()
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				while (true)
				{
					// 集約エラーハンドラでキャッチできないのでtry-catch
					try
					{
						//------------------------------------------------------
						// スケジュールタスクの再実行
						//------------------------------------------------------
						ReRunScheduledTask(sqlAccessor);

						//------------------------------------------------------
						// タスクスケジュール作成（一日一回実行）
						//------------------------------------------------------
						CreateScheduledTask(sqlAccessor);

						//------------------------------------------------------
						// タスクスケジュール取得
						//------------------------------------------------------
						DataView dvTaskSchedule = GetTaskSchedule(sqlAccessor);

						//------------------------------------------------------
						// スケジュールタスク実行
						//------------------------------------------------------
						RunScheduledTask(dvTaskSchedule);

						//------------------------------------------------------
						// 終了済みスレッドはワーカープロセスリストから削除
						//------------------------------------------------------
						for (int iLoop = m_workerThreads.Count - 1; iLoop >= 0; iLoop--)
						{
							WorkerThread.BaseThread workerThread = m_workerThreads[iLoop];
							if (workerThread.Thread.IsAlive == false)
							{
								m_workerThreads.Remove(workerThread);
							}
						}
					}
					catch (Exception ex)
					{
						try
						{
							// ログを落としてスルー
							FileLogger.WriteError(ex);
							Form1.WriteInfoLogLine(ex.ToString());
						}
						catch (Exception ex2)
						{
							Form1.WriteInfoLogLine(ex2.ToString());
						}
					}

					// スレッド停止命令が来たらスレッド停止
					if (m_blStopThread)
					{
						break;
					}

					// スリープ
					Thread.Sleep(SLEEP_MSEC);
				}
			}
		}

		/// <summary>
		/// タスクスケジュール取得
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private DataView GetTaskSchedule(SqlAccessor sqlAccessor)
		{
			var taskScheduleActionKbns = string.Empty;

			// Check setting member rank
			if (Constants.MEMBER_RANK_OPTION_ENABLED == false)
			{
				taskScheduleActionKbns = string.Format("AND {0}.{1} NOT IN ('{2}','{3}')",
					Constants.TABLE_TASKSCHEDULE,
					Constants.FIELD_TASKSCHEDULE_ACTION_KBN,
					Constants.FLG_TASKSCHEDULE_ACTION_KBN_MEMBER_RANK_USER_EXTRACT,
					Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK);
			}

			using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "GetTaskScheduleForExecute"))
			{
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ ACTION_KBNS @@", taskScheduleActionKbns);

				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
			}
		}

		/// <summary>
		/// タスクスケジュール作成（一日一回実行）
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private void CreateScheduledTask(SqlAccessor sqlAccessor)
		{
			if (m_dtTaskScheduleCreateNext <= DateTime.Now)
			{
				DateTime dtNextDay = DateTime.Now.AddDays(1);
				m_dtTaskScheduleCreateNext = new DateTime(dtNextDay.Year, dtNextDay.Month, dtNextDay.Day, 0, 0, 0);

				// Create task schedule target list
				var inserted = CreateTaskSchedule(Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST, "CreateTaskScheduleTargetList", sqlAccessor);

				// Create task schedule mail dist
				inserted += CreateTaskSchedule(Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST, "CreateTaskScheduleMailDist", sqlAccessor);

				// 定期購入再開のタスクスケジュールを作成する
				if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
				{
					if (CreateTaskScheduleForFixedPurchaseResume(sqlAccessor)) inserted++;
				}

				// Create task schedules change member rank
				if (Constants.MEMBER_RANK_OPTION_ENABLED)
				{
					inserted += CreateTaskSchedule(Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK, "CreateTaskScheduleChangeMemberRank", sqlAccessor);
				}

				// クーポン発行スケージュール作成
				if (Constants.W2MP_COUPON_OPTION_ENABLED)
				{
					inserted += CreateTaskSchedule(
						Constants.FLG_TASKSCHEDULE_ACTION_KBN_PUBLISH_COUPON,
						"CreateTaskSchedulePublishCoupon",
						sqlAccessor);
				}

				// ポイント付与スケージュール作成
				if (Constants.W2MP_POINT_OPTION_ENABLED)
				{
					inserted += CreateTaskSchedule(
						Constants.FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT,
						"CreateTaskScheduleAddPoint",
						sqlAccessor);
				}

				// ワークフローシナリオ実行スケージュール作成
				inserted += CreateTaskSchedule(
					Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_SCHEDULE,
					"CreateTaskScheduleAddOrderWorkflowScenario",
					sqlAccessor);

				// For case order workflow option enabled
				if (Constants.ORDERWORKFLOW_OPTION_ENABLED)
				{
					// Create task schedule for workflow target count aggregate
					if (CreateTaskScheduleForWorkflowTargetCountAggregate(sqlAccessor)) inserted++;
				}

				if (inserted > 0)
				{
					Form1.WriteInfoLogLine("タスクスケジュール" + inserted + "件作成完了");
				}
			}
		}

		/// <summary>
		/// Create task schedule
		/// </summary>
		/// <param name="actionKbn">Action kbn</param>
		/// <param name="statement">Sql statement</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number task schedule create</returns>
		private int CreateTaskSchedule(string actionKbn, string statementName, SqlAccessor accessor)
		{
			var input = new Hashtable { { "action_kbn_schedule", actionKbn } };

			using (var statement = new SqlStatement("TaskSchedule", statementName))
			{
				return (int)(statement.SelectSingleStatementWithOC(accessor, input)[0]["COUNT"]);
			}
		}

		/// <summary>
		/// 定期購入再開のタスクスケジュール登録
		/// </summary>
		/// <param name="accessor">Sqlアクセサ</param>
		/// <returns>登録成功か</returns>
		private bool CreateTaskScheduleForFixedPurchaseResume(SqlAccessor accessor)
		{
			var input = new Hashtable
			{
				{ "action_kbn_schedule", Constants.FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_RESUME },
				{ "dept_id", Constants.CONST_DEFAULT_DEPT_ID },
				{ "action_master_id", DateTime.Now.ToString("yyyyMMddhhmmssfff") }
			};

			using (var statement = new SqlStatement("TaskSchedule", "CreateTaskScheduleResumeFixedPurchase"))
			{
				return (statement.ExecStatementWithOC(accessor, input) > 0);
			}
		}

		/// <summary>
		/// Create task schedule for workflow target count aggregate
		/// </summary>
		/// <param name="accessor">Sql Accessor</param>
		/// <returns>True: If a new task schedule has been successfully created</returns>
		private bool CreateTaskScheduleForWorkflowTargetCountAggregate(SqlAccessor accessor)
		{
			var now = DateTime.Now;
			var input = new Hashtable
			{
				{ Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE, now },
				{ "action_kbn_schedule", Constants.FLG_TASKSCHEDULE_ACTION_KBN_WORKFLOW_TARGET_COUNT_AGGREGATE },
				{ Constants.FIELD_TASKSCHEDULE_DEPT_ID, Constants.CONST_DEFAULT_DEPT_ID },
				{ Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, now.ToString("yyyyMMddhhmmssfff") }
			};

			using (var statement = new SqlStatement("TaskSchedule", "CreateTaskScheduleIfNotExists"))
			{
				return (statement.ExecStatementWithOC(accessor, input) > 0);
			}
		}

		/// <summary>
		/// スケジュールタスク実行
		/// </summary>
		/// <param name="dvTaskSchedule">スケジュール情報</param>
		private void RunScheduledTask(DataView dvTaskSchedule)
		{
			// 待ちキューにあるものをキューへ入れる
			while (m_waitQueue.Count > 0)
			{
				m_queue.Enqueue((DataRowView)m_waitQueue.Dequeue());
			}

			// スケジュールタスクをキューに貯める
			foreach (DataRowView drv in dvTaskSchedule)
			{
				m_queue.Enqueue(drv);
			}

			// 最大スレッドまで実行
			while ((m_queue.Count > 0) && (m_workerThreads.Count < Constants.THREADS_MAX))
			{
				var drv = (DataRowView)m_queue.Dequeue();
				switch ((string)drv[Constants.FIELD_TASKSCHEDULE_ACTION_KBN])
				{
					// ターゲットリスト作成スレッド作成
					case Constants.FLG_TASKSCHEDULE_ACTION_KBN_CREATE_TARGETLIST:
						m_workerThreads.Add(
							WorkerThread.TargetExtractThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]));
						break;

					// メール配信スレッド作成
					case Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST:
						m_workerThreads.Add(
							WorkerThread.MailSenderThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]));
						break;

					// 会員ランク付与ユーザー抽出スレッド作成
					case Constants.FLG_TASKSCHEDULE_ACTION_KBN_MEMBER_RANK_USER_EXTRACT:
						m_workerThreads.Add(
							WorkerThread.MemberRankUserExtractThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]));
						break;

					// 会員ランク付与スレッド作成
					case Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK:
						// 会員ランク付与スレッドが別で動いている場合はスレッド作成せず待ちキューに入れる
						if (m_workerThreads.Any(thread => ((thread is WorkerThread.ChangeMemberRankThread) && thread.Thread.IsAlive)) == false)
						{
							m_workerThreads.Add(
								WorkerThread.ChangeMemberRankThread.CreateAndStart(
									(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
									(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
									(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
									(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]));
						}
						else
						{
							m_waitQueue.Enqueue(drv);
						}
						break;

					// 定期購入メール送信スレッド作成
					case Constants.FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_MAIL:
						m_workerThreads.Add(
							WorkerThread.FixedPurchaseMailThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]));
						break;

					// 定期購入変更期限案内メール送信スレッド作成
					case Constants.FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_CHANGE_DEADLINE_MAIL:
						m_workerThreads.Add(
							WorkerThread.FixedPurchaseChangeDeadlineMailThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]));
						break;

					// ポイント付与スレッド作成
					case Constants.FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT:
						m_workerThreads.Add(
							WorkerThread.AddPointThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]));
						break;

					// クーポン発行スレッド作成
					case Constants.FLG_TASKSCHEDULE_ACTION_KBN_PUBLISH_COUPON:
						// クーポン発行スレッドが別で動いている場合はスレッド作成せず待ちキューに入れる
						if (m_workerThreads.Any(thread => ((thread is WorkerThread.PublishCouponThread) && thread.Thread.IsAlive)) == false)
						{
							m_workerThreads.Add(
								WorkerThread.PublishCouponThread.CreateAndStart(
									(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
									(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
									(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
									(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO],
									StringUtility.ToEmpty(drv[Constants.FIELD_TASKSCHEDULE_LAST_CHANGED])));
						}
						else
						{
							m_waitQueue.Enqueue(drv);
						}
						break;

					// 定期購入再開スレッド作成
					case Constants.FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_RESUME:
						m_workerThreads.Add(
							WorkerThread.FixedPurchaseResumeThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]));
						break;

					// 受注ワークフロー実行スレッド作成(手動実行)
					case Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_MANUAL:
						m_workerThreads.Add(
							WorkerThread.OrderWorkflowExecThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_MANUAL,
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO],
								Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_MANUAL,
								(string)drv[Constants.FIELD_TASKSCHEDULE_LAST_CHANGED]));
						break;

					// 受注ワークフロー実行スレッド作成(スケジュール実行)
					case Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_SCHEDULE:
						m_workerThreads.Add(
							WorkerThread.OrderWorkflowExecThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								Constants.FLG_TASKSCHEDULE_ACTION_KBN_ORDERWORKFLOW_EXEC_SCENARIO_TO_SCHEDULE,
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO],
								Constants.FLG_ORDERWORKFLOWEXECHISTORY_EXEC_TIMING_SCHEDULE,
								Constants.FLG_LASTCHANGED_BATCH));
						break;

					// Elogit upload execution thread creation (schedule execution)
					case Constants.FLG_TASKSCHEDULE_ACTION_KBN_ELOGIT_UPLOAD:
						m_workerThreads.Add(
							WorkerThread.WmsShippingThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								Constants.FLG_TASKSCHEDULE_ACTION_KBN_ELOGIT_UPLOAD,
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]));
						break;

					// Elogit dowload execution thread creation (schedule execution)
					case Constants.FLG_TASKSCHEDULE_ACTION_KBN_ELOGIT_DOWNLOAD:
						m_workerThreads.Add(
							WorkerThread.WmsShippingThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								Constants.FLG_TASKSCHEDULE_ACTION_KBN_ELOGIT_DOWNLOAD,
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]));
						break;

					// Workflow target count aggregate thread
					case Constants.FLG_TASKSCHEDULE_ACTION_KBN_WORKFLOW_TARGET_COUNT_AGGREGATE:
						m_workerThreads.Add(
							WorkerThread.WorkflowTargetCountAggregateThread.CreateAndStart(
								(DateTime)drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
								(string)drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID],
								(string)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
								(int)drv[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]));
						break;
				}
			}
		}

		#region 暫定対応
		/// <summary>
		/// スケジュールタスクの再実行
		/// </summary>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <remarks>
		/// HACK:前回スレッドが正常終了(完了や停止)せず、ステータスなど正しく終えれない場合
		/// 「メール送信設定が利用可能ではないので暫く待ちます」が表示され次回スレッドの実行が出来ない。 
		/// 暫定対応の為、現状はメール送信(Maildist)のみ使用し原因判明しだい削除
		/// </remarks>
		private void ReRunScheduledTask(SqlAccessor sqlAccessor)
		{
			const string REACTION_KEY = "reaction";
			const int ADD_MINUTE = 120; // HACK:除外リスト内容が多い場合、SQLに時間がかかるため改善するまではこの時間
			Dictionary<string, Hashtable> settingAndTask = new Dictionary<string, Hashtable>();

			// 正常終了していない可能性があるメール配信設定とタスクスケジュールを取得
			DataView mailDistSettingAndTaskSchedule = null;
			using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "GetMailDistSettingAndTaskSchedule"))
			{
				Hashtable input = new Hashtable();
				input.Add(Constants.FIELD_MAILDISTSETTING_DEPT_ID, "0"); // HACK:暫定対応なのでゼロ固定
				mailDistSettingAndTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}

			// 取得情報を保持
			settingAndTask.Clear();
			foreach (DataRowView drv in mailDistSettingAndTaskSchedule)
			{
				string msg = drv[Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID] + "-" + (drv[Constants.FIELD_TASKSCHEDULE_ACTION_NO]).ToString();
				Hashtable row = GetSettingAndTaskOfMailDist(drv);
				row.Add(REACTION_KEY, false); // 再度スレッド作成したかどうかのチェック用
				settingAndTask.Add(msg, row);

				// 念のためログ出力
				FileLogger.Write(Constants.INTERIM, msg + " " + drv[Constants.FIELD_TASKSCHEDULE_ACTION_KBN] + " " + drv[Constants.FIELD_TASKSCHEDULE_PROGRESS] + " WaitTask:作業中orエラー", true);
			}

			// 更新と現在の日時が一定時間差があるとき異常と判断、そうでなければcontinue
			foreach (string key in settingAndTask.Keys)
			{
				if ((settingAndTask.ContainsKey(key) == false)
					|| ((bool)(settingAndTask[key][REACTION_KEY]))
					|| (((DateTime)(settingAndTask[key][Constants.FIELD_TASKSCHEDULE_DATE_CHANGED])).AddMinutes(ADD_MINUTE) > DateTime.Now))
				{
					continue;
				}

				// 送信エラーとしてステータス変更し待ちループ解除
				using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "CloseScheduleTask"))
				{
					Hashtable input = new Hashtable();
					input.Add(Constants.FIELD_MAILDISTSETTING_DEPT_ID, "0"); // HACK:暫定対応なのでゼロ固定
					input.Add(Constants.FIELD_MAILDISTSETTING_MAILDIST_ID, settingAndTask[key][Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID]);
					input.Add(Constants.FIELD_TASKSCHEDULE_ACTION_NO, settingAndTask[key][Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]);
					sqlStatement.ExecStatementWithOC(sqlAccessor, input);
					FileLogger.Write(Constants.INTERIM, "送信エラーとしてステータス変更し待ちループ解除", true);
				}

				// ２重送信しないよう既存スレッド停止し、完了するまで待つ
				for (int iLoop = m_workerThreads.Count - 1; iLoop >= 0; iLoop--)
				{
					var baseThread = m_workerThreads[iLoop];
					if (baseThread is WorkerThread.MailSenderThread)
					{
						if ((baseThread.DeptId == (string)settingAndTask[key][Constants.FIELD_TASKSCHEDULE_DEPT_ID])
							&& (baseThread.ActionKbn == (string)settingAndTask[key][Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_KBN])
							&& (baseThread.MasterId == (string)settingAndTask[key][Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID])
							&& (baseThread.ActionNo == (int)settingAndTask[key][Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO]))
						{
							FileLogger.Write(Constants.INTERIM, "スレッドを停止します。ThreadState:" + baseThread.Thread.ThreadState.ToString(), true);

							baseThread.Thread.Abort();
							while (baseThread.Thread.IsAlive)
							{
								// 停止まで待つ
								Thread.Sleep(100);
							}
						}
					}
				}

				// メール配信スレッド再作成
				m_workerThreads.Add(
					WorkerThread.MailSenderThread.CreateAndStart(
						(DateTime)settingAndTask[key][Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE],
						(string)settingAndTask[key][Constants.FIELD_TASKSCHEDULE_DEPT_ID],
						(string)settingAndTask[key][Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID],
						(int)settingAndTask[key][Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO],
						true));

				settingAndTask[key][REACTION_KEY] = true;

				string msg = key + " 再度スレッドを作成しました";
				Form1.WriteDebugoLogLine(msg);
				FileLogger.Write(Constants.INTERIM, msg, true);
			}
		}

		/// <summary>
		/// 正常終了していない可能性があるメール配信設定とタスクスケジュールをHashtableで取得
		/// </summary>
		/// <param name="drv">スレッド作成に必要な情報</param>
		private Hashtable GetSettingAndTaskOfMailDist(DataRowView drv)
		{
			Hashtable row = new Hashtable();
			row.Add(Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE, drv[Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE]);
			row.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, drv[Constants.FIELD_TASKSCHEDULE_DEPT_ID]);
			row.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, drv[Constants.FIELD_TASKSCHEDULE_ACTION_KBN]);
			row.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, drv[Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID]);
			row.Add(Constants.FIELD_TASKSCHEDULE_ACTION_NO, drv[Constants.FIELD_TASKSCHEDULE_ACTION_NO]);
			row.Add(Constants.FIELD_TASKSCHEDULE_DATE_CHANGED, drv[Constants.FIELD_TASKSCHEDULE_DATE_CHANGED]);
			return row;
		}
		#endregion
	}
}
