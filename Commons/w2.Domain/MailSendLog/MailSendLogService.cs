/*
=========================================================================================================
  Module      : メール送信ログサービス (MailSendLogService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;

namespace w2.Domain.MailSendLog
{
	/// <summary>
	/// メール送信ログサービス
	/// </summary>
	public class MailSendLogService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="logNo">ログNO</param>
		/// <returns>モデル</returns>
		public MailSendLogModel Get(long logNo)
		{
			using (var repository = new MailSendLogRepository())
			{
				var model = repository.Get(logNo);
				return model;
			}
		}
		#endregion

		#region +GetMailSendLogByUserId ユーザーIDからメール送信ログ取得
		/// <summary>
		/// ユーザーIDからメール送信ログ取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public MailSendLogModel[] GetMailSendLogByUserId(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new MailSendLogRepository(accessor))
			{
				var models = repository.GetMailSendLogByUserId(userId);
				return models;
			}
		}
		#endregion

		#region +GetByLogNoAndUserId  ログNoとユーザーIDからメール送信ログ取得
		/// <summary>
		/// ログNoとユーザーIDからメール送信ログ取得
		/// </summary>
		/// <param name="logNo">ログNo</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル</returns>
		public MailSendLogModel GetByLogNoAndUserId(long logNo, string userId)
		{
			var model = Get(logNo);
			if(model == null) return null;
			return (model.UserId == userId) ? model : null;
		}
		#endregion

		#region +GetDateSendMailOldest 最も古い送信日時取得
		/// <summary>
		/// 最も古い送信日時取得
		/// </summary>
		public DateTime? GetDateSendMailOldest()
		{
			using (var repository = new MailSendLogRepository())
			{
				return repository.GetDateSendMailOldest();
			}
		}
		#endregion

		#region +Get 取得(画面表示用)
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="logNo">ログNO</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル</returns>
		public MailSendLogModel GetForDisplay(long logNo, string userId = "")
		{
			using (var repository = new MailSendLogRepository())
			{
				var model = repository.GetForDisplay(logNo, userId);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(MailSendLogModel model)
		{
			using (var repository = new MailSendLogRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +UpdateUserId ユーザーID更新
		/// <summary>
		/// ユーザーID更新
		/// </summary>
		/// <param name="logNo">ログNo</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public int UpdateUserId(long logNo, string userId, SqlAccessor accessor)
		{
			using (var repository = new MailSendLogRepository(accessor))
			{
				// 最新インシデント取得
				var model = repository.Get(logNo);
				if (model == null) return 0;
				// ユーザーID
				model.UserId = userId;
				return repository.Update(model);
			}
		}
		#endregion

		#region +UpdateReadFlg 既読フラグ更新
		/// <summary>
		/// 既読フラグ更新
		/// </summary>
		/// <param name="logNo">ログNo</param>
		/// <param name="readFlg">既読か</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>件数</returns>
		public int UpdateReadFlg(long logNo, bool readFlg, SqlAccessor accessor)
		{
			using (var repository = new MailSendLogRepository(accessor))
			{
				// 最新情報取得
				var model = repository.Get(logNo);
				// 更新情報セット
				model.ReadFlg = readFlg ? Constants.FLG_MAILSENDLOG_READ_FLG_READ : Constants.FLG_MAILSENDLOG_READ_FLG_UNREAD;
				model.DateReadMail = readFlg ? DateTime.Now : (DateTime?)null;
				return repository.Update(model);
			}
		}
		#endregion

		#region +DeleteByDate 指定日以前を削除
		/// <summary>
		/// 指定日時以前を削除
		/// </summary>
		/// <param name="dateSendMail">指定日時</param>
		/// <param name="timeOutSec">タイムアウト時間</param>
		public void DeleteByDateTime(DateTime dateSendMail, int? timeOutSec)
		{
			using (var repository = new MailSendLogRepository())
			{
				repository.DeleteByDateTime(dateSendMail, timeOutSec);
			}
		}
		#endregion

		#region +InsertMailSendTextHistoryAndGetTextHistoryNo メール配信時文章履歴登録(登録後、履歴NO取得)
		/// <summary>
		/// メール配信時文章履歴登録(登録後、履歴NO取得)
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>メール配信時文章履歴NO</returns>
		public int InsertTextHistoryAndGetTextHistoryNo(MailSendTextHistoryModel model)
		{
			using (var repository = new MailSendLogRepository())
			{
				var textHistoryNo = repository.InsertTextHistoryAndGetTextHistoryNo(model);
				return textHistoryNo;
			}
		}
		#endregion

	}
}