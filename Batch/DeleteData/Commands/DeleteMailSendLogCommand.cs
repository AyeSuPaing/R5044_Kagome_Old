/*
=========================================================================================================
  Module      : メール送信ログ削除コマンド(DeleteMailSendLogCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Domain.MailSendLog;

namespace w2.Commerce.Batch.DeleteData.Commands
{
	/// <summary>
	/// メール送信ログ削除コマンド
	/// </summary>
	public class DeleteMailSendLogCommand
	{
		#region +実行
		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			// CSオプションが有効の場合はメール送信ログ削除
			if (Constants.CS_OPTION_ENABLED)
			{
				// 削除対象日付取得
				var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
				var date = now.AddMonths(Constants.MAILSENDLOG_RETENTION_PERIOD_MONTHS * -1);

				// 最も古い送信日時取得
				var targetDate = date;
				var dateOldest = new MailSendLogService().GetDateSendMailOldest();
				if (dateOldest.HasValue)
				{
					// 1回で削除する対象の日付時刻を取得
					targetDate = GetNextDeleteTargetDateTime(dateOldest.Value);
				}

				// SQLタイムアウト秒を取得(ブランクの場合、Nullとし指定なし)
				int parsedTimeOutSec;
				var timeOutSec = int.TryParse(Constants.DELETE_DATA_SQL_TIMEOUT_SECOND, out parsedTimeOutSec)
					? (int?)parsedTimeOutSec
					: null;

				// 削除対象メール送信日時まで指定した日付時間分ずつ削除する
				// ※一括で1月分削除するとタイムアウトエラーが発生するため
				while (targetDate <= date)
				{
					new MailSendLogService().DeleteByDateTime(targetDate, timeOutSec);
					targetDate = GetNextDeleteTargetDateTime(targetDate);
				}
			}
		}
		#endregion

		/// <summary>
		/// 指定日付時刻の次に削除する日付時刻を取得
		/// </summary>
		/// <param name="TargetDateTime">指定日時</param>
		/// <returns>次の削除対象日付時刻</returns>
		private DateTime GetNextDeleteTargetDateTime(DateTime targetDateTime)
		{
			var nextDeleteTargetDateTime = targetDateTime
				.AddDays(Constants.DELETE_TARGET_INTERVAL_DAYS)
				.AddHours(Constants.DELETE_TARGET_INTERVAL_HOUR)
				.AddMinutes(Constants.DELETE_TARGET_INTERVAL_MINUTE);

			var nextDeleteTargetDateTimeAdjust = new DateTime(
				nextDeleteTargetDateTime.Year,
				nextDeleteTargetDateTime.Month,
				nextDeleteTargetDateTime.Day,
				nextDeleteTargetDateTime.Hour,
				nextDeleteTargetDateTime.Minute,
				0);

			return nextDeleteTargetDateTimeAdjust;
		}
	}
}