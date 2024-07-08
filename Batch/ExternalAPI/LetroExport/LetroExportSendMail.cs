/*
=========================================================================================================
  Module      : LetroExport 結果報告メール送信クラス(LetroExportSendMail.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using ExternalAPI.Mail;
using System;
using System.Data;
using w2.App.Common;
using w2.Common.Logger;
using w2.ExternalAPI.Common;

namespace ExternalAPI.LetroExport
{
	/// <summary>
	/// LetroExport 結果報告メール送信クラス
	/// </summary>
	public class LetroExportSendMail
	{
		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="process">LetroExport 実行設定内容</param>
		/// <param name="isSuccess">成功したか</param>
		public void SendMail(LetroExportProcess process, bool isSuccess)
		{
			using (var mailer = new MailSendUtility(Constants.MailSendMethod.Auto))
			{
				var commandKey = (process.ExportSetting != null)
					? process.ExportSetting.CommandKey
					: "【設定ファイルの読み込みに失敗】";
				var body = CreateBody(process, commandKey, isSuccess);
				SetSendMailSetting(commandKey, isSuccess, body, mailer);
				if (mailer.SendMail() == false)
				{
					FileLogger.WriteError(mailer.MailSendException);
				}
			}
		}

		/// <summary>
		/// メール送信設定
		/// </summary>
		/// <param name="commandKey">実行コマンドキー</param>
		/// <param name="success">成功したか</param>
		/// <param name="body">メール本文</param>
		/// <param name="mailer">メール送信ユーティリティクラス</param>
		private void SetSendMailSetting(
			string commandKey,
			bool success,
			string body,
			MailSendUtility mailer)
		{
			mailer.SetSubject(
				string.Format(
					"{0} 実行:{1} 結果:{2}",
					Constants.MAIL_SUBJECTHEAD,
					commandKey,
					success ? "成功" : "失敗"));
			mailer.SetFrom(Constants.MAIL_FROM.Address);
			Constants.MAIL_TO_LIST.ForEach(mail => mailer.AddTo(mail.Address));
			Constants.MAIL_CC_LIST.ForEach(mail => mailer.AddCC(mail.Address));
			Constants.MAIL_BCC_LIST.ForEach(mail => mailer.AddBcc(mail.Address));
			mailer.SetBody(body);
		}

		/// <summary>
		/// メール内容作成
		/// </summary>
		/// <param name="process">LetroExport 実行設定内容</param>
		/// <param name="commandKey">実行コマンドキー</param>
		/// <param name="isSuccess">成功したか</param>
		/// <returns>メール内容</returns>
		private string CreateBody(LetroExportProcess process, string commandKey, bool isSuccess)
		{
			var mail = new MailTemplate
			{
				FileType = commandKey,
				FileName = process.ExportFileNameWithExtension,
				ApiType = APIType.Export,
				ExecutedTime = process.StartTime,
				EndTime = DateTime.Now,
				IsFailed = isSuccess == false,
				Log = new DataTable()
			};
			return mail.TransformText();
		}
	}
}
