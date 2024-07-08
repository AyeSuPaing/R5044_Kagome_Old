/*
=========================================================================================================
  Module      : 会員ランク付与ユーザー抽出スレッドクラス(MemberRankUserExtractThread.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
using w2.App.Common.User;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.MemberRankRule;
using w2.Domain.MemberRankRule;
using w2.Domain.TargetList;
using w2.Domain.User;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	class MemberRankUserExtractThread : BaseThread
	{
		/// <summary> 会員ランク付与ルール：MemberRankRule </summary>
		private const string TABEL_NAME = "MemberRankRule";

		//=========================================================================================
		/// <summary>
		/// スレッド作成（アクション実行）
		/// </summary>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strMemberRankRuleId">ランク付与ルールID</param>
		/// <returns>生成スレッド</returns>
		//=========================================================================================
		public static MemberRankUserExtractThread CreateAndStart(string strDeptId, string strMemberRankRuleId)
		{
			return CreateAndStart(DateTime.Now, strDeptId, strMemberRankRuleId, -1);
		}
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
		public static MemberRankUserExtractThread CreateAndStart(DateTime dtScheduleDate, string strDeptId, string strMemberRankRuleId, int iActionNo)
		{
			// スレッド作成
			var memberRankUserExtractThread = new MemberRankUserExtractThread(dtScheduleDate, strDeptId, strMemberRankRuleId, iActionNo);
			memberRankUserExtractThread.Thread = new Thread(new ThreadStart(memberRankUserExtractThread.Work));

			// スレッドスタート
			memberRankUserExtractThread.Thread.Start();

			return memberRankUserExtractThread;
		}

		/// <summary>
		///  デフォルトコンストラクタ
		/// </summary>
		public MemberRankUserExtractThread()
			:base()
		{
			// 何もしない //
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
		public MemberRankUserExtractThread(DateTime dtScheduleDate, string strDeptId, string strMemberRankRuleId, int iActionNo)
			: base(dtScheduleDate, strDeptId, Constants.FLG_TASKSCHEDULE_ACTION_KBN_MEMBER_RANK_USER_EXTRACT, strMemberRankRuleId, iActionNo)
		{
			// 処理しない //
		}

		//=========================================================================================
		/// <summary>
		/// 会員ランク付与ユーザー抽出
		/// </summary>
		//=========================================================================================
		public void Work()
		{
			// HACK:このクラススレッドじゃなくていい。スレッド内から呼び出されてシーケンシャル(while(thread.IsAlive){thread.wait(300)})に実行されてるだけ
			// HACK:メンバーランククラス？のようなもののインスタンス作って、パラメータ設定→抽出メソッド呼び出しじゃだめなんだろうか。。
			try
			{
				//------------------------------------------------------
				// 会員ランク付与ユーザー抽出タスクステータス更新(開始)
				//------------------------------------------------------
				if (this.ActionNo != -1)
				{
					int iUpdate = UpdateTaskStatusBegin(
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT,
						"");
				}

				//------------------------------------------------------
				// 会員ランク付与ルール取得（同時にステータス更新、取得できるまで待つ）
				//------------------------------------------------------
				DataRowView drvMemberRankRule = GetMemberRankRule(Constants.FLG_MEMBERRANKRULE_STATUS_EXTRACT);

				//------------------------------------------------------
				// 処理開始宣言
				//------------------------------------------------------
				Form1.WriteInfoLogLine("会員ランク付与ユーザー[" + this.MasterId + "]抽出開始");

				if (IsValidMemberRank((string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID]))
				{
					//------------------------------------------------------
					// 会員ランク付与ユーザー抽出
					//------------------------------------------------------
					int iCreateCount = ExtractMemberRankRuleUser(drvMemberRankRule);
					// 抽出でエラーが起きているので、例外を投げる
					if (iCreateCount == -1) throw new Exception();

					//------------------------------------------------------
					// 処理終了宣言
					//------------------------------------------------------
					Form1.WriteInfoLogLine("会員ランク付与ユーザー[" + this.MasterId + "]抽出完了：" + iCreateCount.ToString() + "件");

					//------------------------------------------------------
					// 会員ランク付与ルールのステータス更新（※ステータスは同一ステータスで更新し、最終付与人数・日時も更新）
					//------------------------------------------------------
					UpdateMemberRankRuleStatus(this.MasterId, Constants.FLG_MEMBERRANKRULE_STATUS_EXTRACT, iCreateCount);

					//------------------------------------------------------
					// ターゲット抽出タスクステータス更新(完了)
					//------------------------------------------------------
					if (this.ActionNo != -1)
					{
						int iUpdate = UpdateTaskStatusEnd(
							Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE,
							Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
							iCreateCount + "/" + iCreateCount.ToString());
					}
				}
				else
				{
					//------------------------------------------------------
					// 処理終了宣言
					//------------------------------------------------------
					Form1.WriteInfoLogLine("会員ランク付与ユーザー[" + this.MasterId + "]抽出完了："
						+ (string)drvMemberRankRule[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID] + "は無効です。");

					//------------------------------------------------------
					// ターゲット抽出タスクステータス更新(完了)
					//------------------------------------------------------
					int iUpdate = UpdateTaskStatusEnd(
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE,
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
						"");
				}
			}
			catch (Exception ex)
			{
				w2.Common.Logger.FileLogger.WriteError(ex);
				Form1.WriteInfoLogLine(ex.ToString());

				// エラー発生時はメールで通知する
				var messageHead = "会員ランク付与ユーザー[" + this.MasterId + "]抽出";
				SendDebugMail(messageHead + " エラー発生" + ex.ToString());

				//------------------------------------------------------
				// 会員ランク付与ルールのステータス更新（エラー）
				//------------------------------------------------------
				try
				{
					UpdateMemberRankRuleStatus(
						this.MasterId,
						Constants.FLG_MEMBERRANKRULE_STATUS_ERROR,
						0);
				}
				catch (Exception ex2)
				{
					w2.Common.Logger.FileLogger.WriteError(ex2);
					Form1.WriteInfoLogLine(ex2.ToString());
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
		private DataRowView GetMemberRankRule(string strStatus)
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
							case Constants.FLG_MEMBERRANKRULE_STATUS_ERROR:		// 更新エラー(// エラーでも実行できる)
								// 次へ（ステータスは既に抽出中となっている）
								drvMemberRankRule = dvMemberRankRule[0];
								break;
						}
					}
					if (drvMemberRankRule != null)
					{
						break;	// 無限ループを抜ける
					}

					Thread.Sleep(1000);
				}
			}

			return drvMemberRankRule;
		}

		/// <summary>
		/// 会員ランク付与ユーザー抽出
		/// </summary>
		/// <param name="memberRankRule">会員ランク付与ルール</param>
		/// <returns>抽出完了件数</returns>
		private int ExtractMemberRankRuleUser(DataRowView memberRankRule)
		{
			FileLogger.WriteInfo(
				string.Format(
					"会員ランク変動ルール処理({0}({1}))：開始",
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME],
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]));

			var targetExtractFrom = DateTime.Now;		// ユーザー抽出開始日
			var targetExtractTo = DateTime.Now;		// ユーザー抽出終了日

			// 抽出条件集計期間指定
			switch ((string)memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TYPE])
			{
				case Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DURING:	// 期間指定
					// 抽出条件集計期間開始日
					targetExtractFrom =
						(memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START] != DBNull.Value) ?
						(DateTime)memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_START] : new DateTime(1753, 1, 1);
					// 抽出条件集計期間終了日
					targetExtractTo =
						(memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END] != DBNull.Value) ?
						(DateTime)memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_END] : DateTime.MaxValue;
					break;

				case Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_TYPE_DAYSAGO:	// 前日指定
					// 抽出条件集計期間前日指定
					targetExtractFrom = DateTime.Today.AddDays((-1) * (int)memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_DAYS_AGO]);
					targetExtractTo = DateTime.Today;	// 抽出終了日は本日
					break;
			}

			// 抽出/利用ターゲットリスト取得（カンマ区切り）
			Form1.WriteInfoLogLine("抽出/利用ターゲットリスト取得（カンマ区切り）");
			FileLogger.WriteInfo(
				string.Format(
					"利用ターゲットリスト取得({0}({1}))：開始",
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME],
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]));
			var targetIdsExtract = new StringBuilder();
			var targetIdsUse = new StringBuilder();
			for (var i = 1; i <= 5; i++)
			{
				var targetIdTmp = (string)memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_ID + ((i == 1) ? "" : i.ToString())];
				var targetExtractFlgTmp = (string)memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_FLG + ((i == 1) ? "" : i.ToString())];

				if (targetIdTmp.Length != 0)
				{
					// メール配信設定の5つのターゲット中、1つでもエラーになったら全て送らない
					if (new TargetListService().IsTargetListStatusNormal(targetIdTmp) == false)
					{
						Form1.WriteInfoLogLine("抽出/利用ターゲットリストのステータスがエラーになりました。");
						throw new Exception(
							string.Format("ターゲットリストID:{0}のステータスが「通常」ではないため、メール配信を中止しました。", targetIdTmp));
					}

					targetIdsUse.Append((targetIdsUse.Length != 0) ? "," : "");
					targetIdsUse.Append("'").Append(targetIdTmp).Append("'");

					if (targetExtractFlgTmp == Constants.FLG_MEMBERRANKRULE_TARGET_EXTRACT_FLG_ON)
					{
						targetIdsExtract.Append((targetIdsExtract.Length != 0) ? "," : "");
						targetIdsExtract.Append("'").Append(targetIdTmp).Append("'");
					}
				}
			}
			FileLogger.WriteInfo(
				string.Format(
					"利用ターゲットリスト取得({0}({1}))：終了",
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME],
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]));

			// ターゲット抽出ステータス更新（「実行中」へ。抽出対象なしの場合は「完了」へ）
			var updateStatus = UpdatePrepareTaskStatus(
				(targetIdsExtract.Length == 0)
					? Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE
					: Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
				Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT);
			Form1.WriteInfoLogLine("ターゲット抽出ステータス更新（「実行中」へ。抽出対象なしの場合は「完了」へ）:" + updateStatus);

			// 抽出対象ありの場合は抽出（自動消滅スレッドそれぞれ作成）
			Form1.WriteInfoLogLine("抽出対象ありの場合は抽出（自動消滅スレッドそれぞれ作成）");
			FileLogger.WriteInfo(
				string.Format(
					"利用ターゲットリスト抽出({0}({1}))：開始",
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME],
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]));
			if (targetIdsExtract.Length != 0)
			{
				// ターゲットリスト一覧取得
				var extractedTargetLists = new TargetListService().GetTargetListForAction(this.DeptId, targetIdsExtract.ToString());

				if (extractedTargetLists == null)
				{
					Form1.WriteInfoLogLine("会員ランク変動ルールに設定されているターゲットリストが見つかりませんでした。");
					throw new Exception("会員ランク変動ルールに設定されているターゲットリストが見つかりませんでした。");
				}

				// ターゲットリスト抽出（完了する間で待つ）
				foreach (var extractedTargetList in extractedTargetLists)
				{
					var extractThread = TargetExtractThread.CreateAndStart(
						extractedTargetList.DeptId,
						extractedTargetList.TargetId);
					while (extractThread.Thread.IsAlive)
					{
						Thread.Sleep(100);
					}
				}

				// ターゲット抽出ステータス更新（抽出対象なしの場合は完了ステータスへ）
				UpdatePrepareTaskStatus(
					Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_DONE,
					Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_EXECUTE);
			}
			FileLogger.WriteInfo(
				string.Format(
					"利用ターゲットリスト抽出({0}({1}))：終了",
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME],
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]));

			string strSqlForDebug = null;
			var extractCount = 0;	// 抽出完了件数
			FileLogger.WriteInfo(
				string.Format(
					"会員ランク変動ルール抽出({0}({1}))：開始",
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME],
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]));
			// 抽出開始
			using (var sqlAccessor = new SqlAccessor())
			{
				// トランザクション開始
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				try
				{
					var input = new Hashtable
					{
						{Constants.FIELD_TARGETLISTDATA_DEPT_ID, this.DeptId},
						{Constants.FIELD_TARGETLISTDATA_TARGET_KBN, Constants.FLG_TARGETLISTDATA_TARGET_KBN_MEMBERRANKRULE},
						{Constants.FIELD_TARGETLISTDATA_MASTER_ID, this.MasterId},
						{"target_extract_from", targetExtractFrom},
						{"target_extract_to", targetExtractTo},
						// 抽出条件合計購入金額範囲(From)
						{Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM,
							(memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM] != DBNull.Value) ?
								(decimal)memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_FROM] : 0},
						// 抽出条件合計購入金額範囲(To)
						{Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO,
							(memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO] != DBNull.Value) ?
								(decimal)memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_PRICE_TO] : decimal.MaxValue},
						// 抽出条件合計購入回数範囲(From)
						{Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM,
							(memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM] != DBNull.Value) ?
								(int)memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_FROM] : 0},
						// 抽出条件合計購入回数範囲(To)
						{Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO,
							(memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO] != DBNull.Value) ?
								(int)memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_TOTAL_COUNT_TO] : int.MaxValue},
						{Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG, (string)memberRankRule[Constants.FIELD_MEMBERRANKRULE_TARGET_EXTRACT_OLD_RANK_FLG]}, // 抽出時の旧ランク情報抽出判定
						{Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE, (string)memberRankRule[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_TYPE]}, // ランク付与方法
						{Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID, (string)memberRankRule[Constants.FIELD_MEMBERRANKRULE_RANK_CHANGE_RANK_ID]}, // 指定付与ランクID
						{"intersect_target_list_flg", (targetIdsUse.Length != 0) ? "1" : "0"},
					};

					// ターゲットリストデータの削除処理
					using (var statement = new SqlStatement(TABEL_NAME, "DeleteTargetListData"))
					{
						// SQLタイムアウト値設定
						statement.CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT;
						statement.ExecStatement(sqlAccessor, input);
					}

					// ターゲットリストデータの登録処理
					using (var statement = new SqlStatement(TABEL_NAME, "CreateTargetListData"))
					{
						// SQLタイムアウト値設定
						statement.CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT;
						statement.Statement = statement.Statement.Replace("@@ intersect_target_list @@", targetIdsUse.ToString());
						strSqlForDebug = statement.Statement;
						extractCount = (int)statement.SelectSingleStatement(sqlAccessor, input)[0]["row_count"];
					}

					// コミット
					sqlAccessor.CommitTransaction();
				}
				catch (Exception ex)
				{
					// ロールバック
					sqlAccessor.RollbackTransaction();

					FileLogger.WriteError(ex);
					Form1.WriteInfoLogLine(ex.ToString());
					Form1.WriteInfoLogLine("-----");
					Form1.WriteInfoLogLine(strSqlForDebug);
					Form1.WriteInfoLogLine("-----");

					// エラー検知用
					extractCount = -1;
				}
			}
			FileLogger.WriteInfo(
				string.Format(
					"会員ランク変動ルール抽出({0}({1}))：終了",
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_NAME],
					memberRankRule[Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID]));

			return extractCount;
		}

		// HACK:アクセサ経由だと基底クラスのプロパティにアクセスできなかったので準備。テストコード用のコードです。もっといい方法あれば置き換えてください
		/// <summary>
		/// マスターIDセット
		/// </summary>
		/// <param name="masterId"></param>
		private void SetMasterId(string masterId)
		{
			this.MasterId = masterId;
		}

		/// <summary>
		/// DeptIDセット
		/// </summary>
		/// <param name="deptId"></param>
		private void SetDeptId(string deptId)
		{
			this.DeptId = deptId;
		}
	}
}
