/*
=========================================================================================================
  Module      : 定期購入再開スレッドクラス (FixedPurchaseResumeThread.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Threading;
using w2.App.Common.Order.FixedPurchase;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.FixedPurchase;
using w2.Domain.TaskSchedule;
using w2.Domain.UpdateHistory.Helper;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	/// <summary>
	/// 定期購入再開スレッドクラス
	/// </summary>
	class FixedPurchaseResumeThread : BaseThread
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="scheduleDate">スケジュール日付</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">定期購入メール実行ID</param>
		/// <param name="actionNo">アクションNO</param>
		public FixedPurchaseResumeThread(DateTime scheduleDate, string deptId, string masterId, int actionNo)
			: base(scheduleDate, deptId, Constants.FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_RESUME, masterId, actionNo)
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
		public static FixedPurchaseResumeThread CreateAndStart(DateTime scheduleDate, string deptId, string masterId, int actionNo)
		{
			// スレッド作成
			var fixedPurchaseResumeThread = new FixedPurchaseResumeThread(scheduleDate, deptId, masterId, actionNo);
			fixedPurchaseResumeThread.Thread = new Thread(fixedPurchaseResumeThread.Work);

			// スレッドスタート
			fixedPurchaseResumeThread.Thread.Start();

			return fixedPurchaseResumeThread;
		}

		/// <summary>
		/// 定期購入再開スレッド
		/// </summary>
		public void Work()
		{
			Form1.WriteInfoLogLine("定期購入再開処理を開始 : ID :" + this.MasterId);

			// 再開対象の定期を取得する
			var fixedPurchaseResumeTagets = new FixedPurchaseService().GetTargetsForResume();

			foreach (var fixedPurchase in fixedPurchaseResumeTagets)
			{
				try
				{
					// 定期再開
					var success = new FixedPurchaseService().Resume(
						fixedPurchase.FixedPurchaseId,
						fixedPurchase.UserId,
						Constants.FLG_LASTCHANGED_USER,
						null,
						null,
						UpdateHistoryAction.Insert);

					// 定期購入再開メール送信
					if (success)
					{
						FixedPurchaseHelper.SendMail(
							fixedPurchase.FixedPurchaseId,
							Constants.CONST_MAIL_ID_RESUME_FIXEDPURCHASE);
					}
					
				}
				catch (Exception ex)
				{
					var message = string.Format("定期購入再開処理に失敗しました。定期購入ID：" + fixedPurchase.FixedPurchaseId);

					FileLogger.WriteError(message, ex);
					Form1.WriteErrorLogLine(message);
				}
			}

			//全処理が完了した後、実行したタスクスケジュールを削除
			new TaskScheduleService().Delete(this.DeptId, this.ActionKbn, this.MasterId, this.ActionNo);

			Form1.WriteInfoLogLine("定期購入再開処理を終了 ");
		}
	}
}
