/*
=========================================================================================================
  Module      : メール送信クラス(SendMail.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Net.Mail;
using w2.App.Common;
using w2.ExternalAPI.Common;
using w2.ExternalAPI.Common.FrameWork.DB;

namespace ExternalAPI.Mail
{
	/// <summary>
	/// メール送信クラス
	/// </summary>
	public class SendMail
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SendMail()
		{
			
		}
		#endregion

		#region +Send メール送信
		/// <summary>
		/// メール送信
		/// <param name="target">ターゲットクラス</param>
		/// /// <param name="endTime">終了時間</param>
		/// </summary>
		public bool Send(ExecuteTarget target, DateTime endTime)
		{
			string body;
			if (target != null)
			{
				body = CreateBody(target, GetApiLog(target), endTime);
			}
			else
			{
				body = "API実行対象の特定に失敗しました。";
			}

			using (MailSendUtility mailer = new MailSendUtility(Constants.MailSendMethod.Auto))
			{
				// 送信設定
				SetSendMailSetting(body, mailer);

				// 送信
				return mailer.SendMail();
			}
		}
		#endregion

		#region -GetApiLog Apiログ取得
		/// <summary>
		/// Apiログ取得
		/// </summary>
		/// <param name="target">ターゲットクラス</param>
		/// <returns>Apiログテーブルデータテーブル</returns>
		private DataTable GetApiLog(ExecuteTarget target)
		{
			DataTable log;
			using (SqlAccesorWrapper sqlAccessor = new SqlAccesorWrapper())
			{
				using (SqlStatementWrapper sqlStatement = new SqlStatementWrapper("ExternalApi", "GetApiLog"))
				{
					Hashtable input = new Hashtable();
					input.Add("executed_time", target.ExecutedTime);

					log = sqlStatement.SelectSingleStatement(sqlAccessor, input).Table;
				}
			}
			return log;
		}

		#endregion

		#region -CreateBody 本文作成

		/// <summary>
		/// 本文作成
		/// </summary>
		/// <param name="target">ターゲットクラス</param>
		/// <param name="log">Apiログデータテーブル</param>
		/// <param name="endTime">終了時間</param>
		/// <returns>引数を下に生成したメール本文用文字列</returns>
		private string CreateBody(ExecuteTarget target, DataTable log, DateTime endTime)
		{
			MailTemplate mail = new MailTemplate();

			mail.FileType = target.FileType;
			mail.FileName = target.TargetFilePath;
			mail.ApiType = target.ApiType;
			mail.ExecutedTime = target.ExecutedTime;
			mail.EndTime = endTime;

			// エラーがなければ正常終了を通知
			if (log.Rows.Count == 0)
			{
				mail.IsFailed = false;
			}
			// エラーあり
			else
			{
				mail.IsFailed = true;
				mail.Log = log;
			}
			// メール本文作成
			return mail.TransformText();
		}
		#endregion

		#region -SetSendMailSetting メール送信設定
		/// <summary>
		/// メール送信設定
		/// </summary>
		/// <param name="body">メール本文</param>
		/// <param name="mailer">メール送信ユーティリティクラス</param>
		private void SetSendMailSetting(string body, MailSendUtility mailer)
		{
			// メール送信設定
			mailer.SetSubject(Constants.MAIL_SUBJECTHEAD);
			mailer.SetFrom(Constants.MAIL_FROM.Address);
			Constants.MAIL_TO_LIST.ForEach(mail => mailer.AddTo(mail.Address));
			Constants.MAIL_CC_LIST.ForEach(mail => mailer.AddCC(mail.Address));
			Constants.MAIL_BCC_LIST.ForEach(mail => mailer.AddBcc(mail.Address));

			// 本文
			mailer.SetBody(body);
		}
		#endregion
	}
}
