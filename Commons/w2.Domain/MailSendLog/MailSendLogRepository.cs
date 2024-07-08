/*
=========================================================================================================
  Module      : メール送信ログリポジトリ (MailSendLogRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.MailSendLog
{
	/// <summary>
	/// メール送信ログリポジトリ
	/// </summary>
	public class MailSendLogRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MailSendLog";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MailSendLogRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MailSendLogRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="logNo">ログNO</param>
		/// <returns>モデル</returns>
		public MailSendLogModel Get(long logNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILSENDLOG_LOG_NO, logNo},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new MailSendLogModel(dv[0]);
		}
		#endregion

		#region +GetFixedPurchasesByUserId ユーザーIDから定期購入情報取得
		/// <summary>
		/// ユーザーIDから定期購入情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル列</returns>
		public MailSendLogModel[] GetMailSendLogByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILSENDLOG_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetMailSendLogByUserId", ht);
			if (dv.Count == 0) return new MailSendLogModel[0];
			var models = dv.Cast<DataRowView>().Select(drv => new MailSendLogModel(drv)).ToArray();

			return models;
		}
		#endregion

		#region ~GetDateSendMailOldest 最も古い送信日時取得
		/// <summary>
		/// 最も古い送信日時取得
		/// </summary>
		/// <returns>最も古い送信日時</returns>
		internal DateTime? GetDateSendMailOldest()
		{
			var dv = Get(XML_KEY_NAME, "GetDateSendMailOldest");
			if (dv.Count == 0) return null;
			return (DateTime?)dv[0][Constants.FIELD_MAILSENDLOG_DATE_SEND_MAIL];
		}
		#endregion

		#region +GetForDisplay 取得(画面表示用)
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="logNo">ログNO</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル</returns>
		public MailSendLogModel GetForDisplay(long logNo, string userId = "")
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MAILSENDLOG_LOG_NO, logNo},
				{"user_id", userId}
			};
			var dv = Get(XML_KEY_NAME, "GetForDisplay", ht);
			if (dv.Count == 0) return null;
			return new MailSendLogModel(dv[0]);
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(MailSendLogModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(MailSendLogModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteByDateTime 指定日時以前を削除
		/// <summary>
		/// 指定日時以前を削除
		/// </summary>
		/// <param name="dateSendMail">指定日時</param>
		/// <param name="timeOutSec">タイムアウト時間</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteByDateTime(DateTime dateSendMail, int? timeOutSec)
		{
			var ht = new Hashtable
			{
				{"date_send_mail", dateSendMail},
			};

			// タイムアウト時間を設定し実行
			this.CommandTimeout = timeOutSec;
			var result = Exec(XML_KEY_NAME, "DeleteByDateSendMail", ht);
			return result;
		}
		#endregion

		#region ~InsertTextHistoryAndGetTextHistoryNo メール配信時文章履歴登録(登録後、履歴NO取得)
		/// <summary>
		/// メール配信時文章履歴登録(登録後、履歴NO取得)
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>メール配信時文章履歴NO</returns>
		internal int InsertTextHistoryAndGetTextHistoryNo(MailSendTextHistoryModel model)
		{
			var ht = new Hashtable { };
			var result = Get(XML_KEY_NAME, "InsertTextHistoryAndGetTextHistoryNo", model.DataSource);
			return int.Parse(result[0][0].ToString());
		}
		#endregion
	}
}