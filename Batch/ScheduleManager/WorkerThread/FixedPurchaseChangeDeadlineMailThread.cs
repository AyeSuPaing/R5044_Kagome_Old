/*
=========================================================================================================
  Module      : 定期購入変更期限案内メール送信スレッドクラス (FixedPurchaseChangeDeadlineMailThread.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using w2.App.Common;
using w2.App.Common.Mail;
using w2.App.Common.Order.FixedPurchase;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.TaskSchedule;
using w2.Domain.User;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	/// <summary>
	/// 定期購入メール送信スレッドクラス
	/// </summary>
	class FixedPurchaseChangeDeadlineMailThread : BaseThread
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="scheduleDate">スケジュール日付</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">定期購入メール実行ID</param>
		/// <param name="actionNo">アクションNO</param>
		public FixedPurchaseChangeDeadlineMailThread(DateTime scheduleDate, string deptId, string masterId, int actionNo)
			: base(scheduleDate, deptId, Constants.FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_CHANGE_DEADLINE_MAIL, masterId, actionNo)
		{
		}

		/// <summary>
		/// スレッド作成（タスクスケジュール実行）
		/// </summary>
		/// <param name="scheduleDate">スケジュール日付</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">定期購入メール実行ID</param>
		/// <param name="actionNo">アクションNO</param>
		/// <returns>生成スレッド</returns>
		public static FixedPurchaseChangeDeadlineMailThread CreateAndStart(DateTime scheduleDate, string deptId, string masterId, int actionNo)
		{
			// スレッド作成
			var fixedPurchaseDeadlineMaiThread = new FixedPurchaseChangeDeadlineMailThread(scheduleDate, deptId, masterId, actionNo);
			fixedPurchaseDeadlineMaiThread.Thread = new Thread(new ThreadStart(fixedPurchaseDeadlineMaiThread.Work));

			// スレッドスタート
			fixedPurchaseDeadlineMaiThread.Thread.Start();

			return fixedPurchaseDeadlineMaiThread;
		}

		/// <summary>
		/// 定期購入変更期限案内メール送信スレッド
		/// </summary>
		public void Work()
		{
			Form1.WriteInfoLogLine("定期購入変更期限案内メール送信を開始 : ID :" + this.MasterId);
			Form1.WriteInfoLogLine("定期購入変更期限案内メール送信を開始 ");

			var fixedPurchaseBatchMailTmpLogModels =
				new FixedPurchaseService().SearchFixedPurchaseBatchMailTmpLogs(this.MasterId)
					.Where(m => (m.MasterType == Constants.FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_DEADLINE))
					.ToArray();

			//定期購入メール実行ID(action_master_id)に紐づくメールを順次送信
			foreach (var fixedPurchaseBatchMailTmpLogModel in fixedPurchaseBatchMailTmpLogModels)
			{
				try
				{
					FixedPurchaseHelper.SendMail(fixedPurchaseBatchMailTmpLogModel.MasterId, Constants.CONST_MAIL_ID_CHANGE_DEADLINE);
				}
				catch (Exception ex)
				{
					var message = string.Format(
						"タスクスケジューラによる定期購入変更期限案内メールの送信に失敗しました。master_id : {0}",
						fixedPurchaseBatchMailTmpLogModel.MasterId);

					FileLogger.WriteError(message, ex);
					Form1.WriteErrorLogLine(message);

				}
				finally
				{
					new FixedPurchaseService().DeleteFixedPurchaseBatchMailTmpLog(fixedPurchaseBatchMailTmpLogModel.TmpLogId);
				}
			}

			//全処理が完了した後、実行したタスクスケジュールを削除
			new TaskScheduleService().Delete(this.DeptId, this.ActionKbn, this.MasterId, this.ActionNo);

			Form1.WriteInfoLogLine("定期購入変更期限案内メール送信を終了 ");
		}
	}
}
