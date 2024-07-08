/*
=========================================================================================================
  Module      : 会員ランク付与スレッドクラス(ChangeMemberRankThread.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Threading;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.App.Common;
using w2.App.Common.Option;
using w2.App.Common.Util;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	class ChangeMemberRankThread : BaseThread
	{
		//=========================================================================================
		/// <summary>
		/// スレッド作成（タスクスケジュール実行）
		/// </summary>
		/// <param name="dtScheduleDate">スケジュール日付</param>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strMemberRankRuleId">ランク付与ルールID</param>
		/// <param name="iActionNo">アクションNO</param>
		/// <returns>生成スレッド</returns>
		//=========================================================================================
		public static ChangeMemberRankThread CreateAndStart(DateTime dtScheduleDate, string strDeptId, string strMemberRankRuleId, int iActionNo)
		{
			// スレッド作成
			var changeMemberRankThread = new ChangeMemberRankThread(dtScheduleDate, strDeptId, strMemberRankRuleId, iActionNo);
			changeMemberRankThread.Thread = new Thread(new ThreadStart(changeMemberRankThread.Work));

			// スレッドスタート
			changeMemberRankThread.Thread.Start();

			return changeMemberRankThread;
		}

		//=========================================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="dtScheduleDate">スケジュール日付</param>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strMemberRankRuleId">ランク付与ルールID</param>
		/// <param name="iActionNo">アクションNO</param>
		//=========================================================================================
		public ChangeMemberRankThread(DateTime dtScheduleDate, string strDeptId, string strMemberRankRuleId, int iActionNo)
			: base(dtScheduleDate, strDeptId, Constants.FLG_TASKSCHEDULE_ACTION_KBN_CHANGE_MEMBER_RANK, strMemberRankRuleId, iActionNo)
		{
			// 処理しない //
		}

		//=========================================================================================
		/// <summary>
		/// 会員ランク付与
		/// </summary>
		//=========================================================================================
		public void Work()
		{
			long lExtractTotal = 0;			// 抽出人数
			long lChangeRankTotal = 0;		// 付与人数

			try
			{
				//------------------------------------------------------
				// 会員ランク付与タスクステータス更新(開始)
				//------------------------------------------------------
				int iUpdateStatus = UpdateTaskStatusBegin(
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT,
					"");

				//------------------------------------------------------
				// 処理開始宣言
				//------------------------------------------------------
				Form1.WriteInfoLogLine("会員ランク付与[" + this.MasterId + "]付与開始");

				//------------------------------------------------------
				// 会員ランク付与ルール取得（同時にステータス更新、取得できるまで待つ）
				//------------------------------------------------------
				DataRowView drvMemberRankRule = GetMemberRankRuleAndChangeStatus(Constants.FLG_MEMBERRANKRULE_STATUS_UPDATE);

				// 会員ランクの有効チェック
				if (IsValidMemberRank((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID]))
				{
					// ターゲット抽出ステータス更新（「実行中」へ）
					iUpdateStatus = UpdatePrepareTaskStatus(Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE, Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT);

					// 実行タイミングが「スケジュール実行」の場合は、ターゲットを取得するために、会員ランク付与ユーザー抽出スレッドを作成
					if ((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_EXEC_TIMING] == Constants.FLG_MEMBERRANKRULE_EXEC_TIMING_SCHEDULE)
					{
						var memberRankUserExtractThread = MemberRankUserExtractThread.CreateAndStart(Constants.CONST_DEFAULT_DEPT_ID, (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]);
						while (memberRankUserExtractThread.Thread.IsAlive)
						{
							Thread.Sleep(100);
						}

						// ステータスチェック
						using (var accessor = new SqlAccessor())
						using (var statement = new SqlStatement("MemberRankRule", "GetMemberRankRule"))
						{
							var ht = new Hashtable
							{
								{ Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID, (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID] },
							};

							var drv = statement.SelectSingleStatementWithOC(accessor, ht);
							// ユーザー抽出でエラーが起きている場合は、例外発生させる
							if ((string)drv[0][Constants.FIELD_MEMBERRANKRULE_STATUS] == Constants.FLG_MEMBERRANKRULE_STATUS_ERROR)
							{
								throw new Exception("会員ランク付与ユーザー抽出でエラーが発生しました。\n会員ランク付与の処理を中断します。");
							}
						}
					}

					// ターゲット抽出ステータス更新（「実行完了」へ）
					iUpdateStatus = UpdatePrepareTaskStatus(Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_DONE, Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_EXECUTE);

					DataView dvTargetListData = null;
					using (SqlAccessor sqlAccessor = new SqlAccessor())
					{
						sqlAccessor.OpenConnection();

						//------------------------------------------------------
						// 抽出済ターゲットデータ取得
						//------------------------------------------------------
						using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "GetTargetListData") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
						{
							Hashtable htInput = new Hashtable();
							htInput.Add(Constants.FIELD_TARGETLISTDATA_DEPT_ID, this.DeptId);
							htInput.Add(Constants.FIELD_TARGETLISTDATA_TARGET_KBN, Constants.FLG_TARGETLISTDATA_TARGET_KBN_MEMBERRANKRULE);
							htInput.Add(Constants.FIELD_TARGETLISTDATA_MASTER_ID, this.MasterId);

							dvTargetListData = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
						}
						lExtractTotal = dvTargetListData.Count;

						//------------------------------------------------------
						// 会員ランク付与ルールの抽出ターゲット数更新
						//------------------------------------------------------
						using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "UpdateMemberRankRuleLastCount"))
						{
							Hashtable htInput = new Hashtable();
							htInput.Add(Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID, this.MasterId);
							htInput.Add(Constants.FIELD_MEMBERRANKRULE_LAST_COUNT, lExtractTotal);

							int iUpdated = sqlStatement.ExecStatement(sqlAccessor, htInput);
						}
					}

					//------------------------------------------------------
					// 会員ランク付与
					//------------------------------------------------------
					string strTaskScheduleStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE;	// デフォルトは完了へ

					try // 停止例外捕捉用try
					{
						//------------------------------------------------------
						// 会員ランク付与ループ開始
						//------------------------------------------------------
						foreach (DataRowView drvTargetData in dvTargetListData)
						{
							// スケジュール停止フラグチェック＆進捗更新（とりあえず10件ずつチェックする）
							if ((lChangeRankTotal % 10) == 0)
							{
								CheckScheduleStoppedAndUpdateProgress(lChangeRankTotal.ToString() + "/" + lExtractTotal.ToString());
							}

							// 後々のOneToOneを考えると一件ずつ付与した方がよい //

							//------------------------------------------------------
							// 会員ランク付与情報取得＆設定
							//------------------------------------------------------
							string strUserId = (string)drvTargetData[Constants.FIELD_TARGETLISTDATA_USER_ID];
							string strMaiId = (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_MAIL_ID];

							// 更新前・後ランクID
							string strBeforeRankId = null;
							string strAfterRankId = (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID];	// 指定付与ランクIDを設定

							var userService = new UserService();
							// 会員ランク付与ユーザーの会員ランク(変更前ランク)ID、ユーザー情報取得
							var user = userService.Get(strUserId);
							strBeforeRankId = user.MemberRankId;

							// 会員ランク付与(ユーザー毎)
							user.MemberRankId = strAfterRankId;
							user.LastChanged = Constants.FLG_LASTCHANGED_BATCH;

							// 更新（更新履歴とともに）
							userService.UpdateWithUserExtend(user, UpdateHistoryAction.Insert);

							lChangeRankTotal++;		// ランク付与人数をインクリメント

							//------------------------------------------------------
							// メール送信
							//------------------------------------------------------
							if (strMaiId != "")
							{
								// メールテンプレート用ユーザー情報格納
								Hashtable htInput = (Hashtable)user.DataSource.Clone();
								htInput[Constants.FIELD_MEMBERRANK_MEMBER_RANK_NAME] =
									Constants.MEMBER_RANK_OPTION_ENABLED
									? MemberRankOptionUtility.GetMemberRankName(user.MemberRankId)
									: "";

								// 生年月日の時分秒削除
								htInput[Constants.FIELD_USER_BIRTH] = DateTimeUtility.ToString(
									user.Birth,
									DateTimeUtility.FormatType.ShortDate2Letter,
									user.DispLanguageLocaleId);
								// 生年月日についての管理者向けのタグ用追加
								htInput[Constants.FIELD_USER_BIRTH + "_for_operator"] = DateTimeUtility.ToStringForManager(
									user.Birth,
									DateTimeUtility.FormatType.ShortDate2Letter);

								htInput[Constants.FIELD_USER_NAME] = UserModel.CreateComplementUserName(
									(string)htInput[Constants.FIELD_USER_NAME],
									(string)htInput[Constants.FIELD_USER_MAIL_ADDR],
									(string)htInput[Constants.FIELD_USER_MAIL_ADDR2]);

								htInput[Constants.FIELD_USER_NAME1] = (string.IsNullOrEmpty((string)htInput[Constants.FIELD_USER_NAME1]) && string.IsNullOrEmpty((string)htInput[Constants.FIELD_USER_NAME2]))
									? UserModel.CreateComplementUserName(
										(string)htInput[Constants.FIELD_USER_NAME1],
										(string)htInput[Constants.FIELD_USER_MAIL_ADDR],
										(string)htInput[Constants.FIELD_USER_MAIL_ADDR2])
									: htInput[Constants.FIELD_USER_NAME1];

								// メール送信処理
								var pcMailAddress = (string)htInput[Constants.FIELD_USER_MAIL_ADDR];
								var mobileMailAddress = (string)htInput[Constants.FIELD_USER_MAIL_ADDR2];
								var sendMailPc = false;
								var sendMailMobile = false;
								// 双方送信
								if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED)
								{
									sendMailPc = (pcMailAddress != "");
									sendMailMobile = (mobileMailAddress != "");
								}
								else if (pcMailAddress != "")
								{
									// ユーザーがPCアドレスを設定している場合は、PCアドレスへの送信を優先
									sendMailPc = true;
								}
								else
								{
									sendMailMobile = (mobileMailAddress != "");
								}

								if (sendMailPc && IsUnderUserErrorPoint(pcMailAddress))
								{
									SendMailToUser(strMaiId, strUserId, pcMailAddress, htInput, true, user.DispLanguageCode, user.DispLanguageLocaleId);
								}
								if (sendMailMobile && IsUnderUserErrorPoint(mobileMailAddress)) SendMailToUser(strMaiId, strUserId, mobileMailAddress, htInput, false);
							}

							//------------------------------------------------------
							// 会員ランク付与結果を、会員ランク変更履歴に格納（※スケジュール履歴には格納しない）
							//------------------------------------------------------
							using (SqlAccessor sqlAccessor = new SqlAccessor())
							using (SqlStatement sqlStatement = new SqlStatement("UserMemberRank", "InsertUserMemberRankHistory"))
							{
								Hashtable htInput = new Hashtable();
								htInput.Add(Constants.FIELD_USERMEMBERRANKHISTORY_USER_ID, strUserId);				// ユーザID
								htInput.Add(Constants.FIELD_USERMEMBERRANKHISTORY_BEFORE_RANK_ID, strBeforeRankId);	// 更新前ランクID
								htInput.Add(Constants.FIELD_USERMEMBERRANKHISTORY_AFTER_RANK_ID, strAfterRankId);	// 更新後ランクID
								htInput.Add(Constants.FIELD_USERMEMBERRANKHISTORY_MAIL_ID, strMaiId);				// メールテンプレートID
								htInput.Add(Constants.FIELD_USERMEMBERRANKHISTORY_CHANGED_BY, this.MasterId);		// 変更者

								int iUpdate = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
							}

							// 少し休む
							Thread.Sleep(50);
						}
					}
					catch (ScheduleStopException)
					{
						Form1.WriteInfoLogLine("■会員ランク付与停止要求あり■");

						strTaskScheduleStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP;	// 「停止中」へ
					}

					//------------------------------------------------------
					// 会員ランク付与ルールのステータス更新（通常状態へ）
					//------------------------------------------------------
					UpdateMemberRankRuleStatus(this.MasterId, Constants.FLG_MEMBERRANKRULE_STATUS_NORMAL);

					//------------------------------------------------------
					// 処理終了宣言
					//------------------------------------------------------
					Form1.WriteInfoLogLine("会員ランク付与[" + this.MasterId + "]付与完了：" + lChangeRankTotal.ToString() + "/" + lExtractTotal.ToString() + "件");

					//------------------------------------------------------
					// 会員ランク付与タスクステータス更新（停止 or 完了）
					//------------------------------------------------------
					iUpdateStatus = UpdateTaskStatusEnd(
						strTaskScheduleStatus,
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
						lChangeRankTotal.ToString() + "/" + lExtractTotal.ToString());
				}
				else
				{
					//------------------------------------------------------
					// 会員ランク付与ルールのステータス更新（通常状態へ）
					//------------------------------------------------------
					UpdateMemberRankRuleStatus(this.MasterId, Constants.FLG_MEMBERRANKRULE_STATUS_NORMAL);

					//------------------------------------------------------
					// 処理終了宣言
					//------------------------------------------------------
					Form1.WriteInfoLogLine("会員ランク付与[" + this.MasterId + "]付与完了："
						+ (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID] + "は無効です。");

					//------------------------------------------------------
					// 会員ランク付与タスクステータス更新（完了）
					//------------------------------------------------------
					iUpdateStatus = UpdateTaskStatusEnd(
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE,
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
						"");
				}
			}
			catch (Exception ex)
			{
				w2.Common.Logger.FileLogger.WriteError(ex);
				Form1.WriteErrorLogLine(ex.ToString());

				// 管理者にメール送信
				SendAdministratorMail("会員ランク付与失敗", ex.ToString());

				//------------------------------------------------------
				// 会員ランク付与ルールのステータス更新（エラーへ）
				//------------------------------------------------------
				try
				{
					UpdateMemberRankRuleStatus(this.MasterId, Constants.FLG_MEMBERRANKRULE_STATUS_ERROR);
				}
				catch (Exception ex2)
				{
					w2.Common.Logger.FileLogger.WriteError(ex2);
					Form1.WriteErrorLogLine(ex2.ToString());
				}

				//------------------------------------------------------
				// 会員ランク付与タスクステータス更新（エラー）
				//------------------------------------------------------
				try
				{
					UpdateTaskStatusEnd(
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_ERROR,
						null,
						lChangeRankTotal.ToString() + "/" + lExtractTotal.ToString());
				}
				catch (Exception ex2)
				{
					w2.Common.Logger.FileLogger.WriteError(ex2);
					Form1.WriteErrorLogLine(ex2.ToString());
				}
			}
		}

		//=========================================================================================
		/// <summary>
		/// スレッドストップチェック
		/// </summary>
		/// <param name="strProgress">進捗</param>
		//=========================================================================================
		private void CheckScheduleStoppedAndUpdateProgress(string strProgress)
		{
			// タスクスケジュール取得
			DataView dvTaskSchedule = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "GetTaskScheduleAndUpdateProgress"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.DeptId);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, this.ActionKbn);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, this.MasterId);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_NO, this.ActionNo);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_PROGRESS, strProgress);

				dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			if (dvTaskSchedule.Count != 0)
			{
				if ((string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_STOP_FLG] == Constants.FLG_TASKSCHEDULE_STOP_FLG_ON)
				{
					throw new ScheduleStopException();
				}
			}
		}

		//=========================================================================================
		/// <summary>
		/// 会員ランク付与ルール取得＆ステータス変更
		/// </summary>
		/// <param name="strStatus">変更ステータス</param>
		/// <returns>会員ランク付与ルール</returns>
		//=========================================================================================
		private DataRowView GetMemberRankRuleAndChangeStatus(string strStatus)
		{
			DataRowView drvMemberRankRule = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "GetMemberRankRuleAndChangeStatus"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID, this.MasterId);
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_STATUS, strStatus);

				while (true)
				{
					//------------------------------------------------------
					// 現在のステータス取得・チェック
					//------------------------------------------------------
					DataView dvMemberRankRule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
					if (dvMemberRankRule.Count != 0)
					{
						switch ((string)dvMemberRankRule[0][Constants.FIELD_MEMBERRANKRULE_STATUS])
						{
							// ステータスによらず、取得可能
							case Constants.FLG_MEMBERRANKRULE_STATUS_NORMAL:	// 通常
							case Constants.FLG_MEMBERRANKRULE_STATUS_EXTRACT:	// 抽出中
							case Constants.FLG_MEMBERRANKRULE_STATUS_UPDATE:	// 更新中
							case Constants.FLG_MEMBERRANKRULE_STATUS_ERROR:		// 更新エラー（エラーでも実行できる）
								// 次へ（ステータスは既に更新中となっている）
								drvMemberRankRule = dvMemberRankRule[0];
								break;
						}
					}
					if (drvMemberRankRule != null)
					{
						break;	// 無限ループを抜ける
					}

					Thread.Sleep(1000);

					//------------------------------------------------------
					// 停止要求チェック
					//------------------------------------------------------
					try
					{
						CheckScheduleStoppedAndUpdateProgress(null);
					}
					catch (ScheduleStopException)
					{
						// 通常ステータスへ
						int iUpdateStatus = UpdateMemberRankRuleStatus(this.MasterId, Constants.FLG_MEMBERRANKRULE_STATUS_NORMAL);

						// 付与ストップ
						int iUpdateStatusEnd = UpdateTaskStatusEnd(
							Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP,
							null,
							"0/0");
					}
				}
			}

			return drvMemberRankRule;
		}

		/// <summary>
		/// ユーザー向けメール送信処理
		/// </summary>
		/// <param name="mailId">メールテンプレートID</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="mailAddr">メールアドレス</param>
		/// <param name="input">入力パラメタ</param>
		/// <param name="isPc">PCフラグ</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		private void SendMailToUser(string mailId, string userId, string mailAddr, Hashtable input, bool isPc, string languageCode = null, string languageLocaleId = null)
		{
			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				mailId,
				userId,
				input,
				isPc,
				Constants.MailSendMethod.Manual,
				languageCode,
				languageLocaleId,
				mailAddr))
			{
				mailSender.AddTo(StringUtility.ToEmpty(mailAddr));
				if (mailSender.SendMail() == false)
				{
					AppLogger.WriteError(this.GetType().BaseType.ToString() + " : " + mailSender.MailSendException.ToString());
				}
			}
		}
	}
}
