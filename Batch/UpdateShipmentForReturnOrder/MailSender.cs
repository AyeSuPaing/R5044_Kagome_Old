/*
=========================================================================================================
  Module      : メール送信モジュール(MailSender.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Common.Logger;
using w2.App.Common;

namespace w2.Commerce.Batch.UpdateShipmentForReturnOrder
{
	/// <summary>
	/// メール送信モジュール
	/// </summary>
	public class MailSender
	{
		/// <summary>
		/// メールを送信する
		/// </summary>
		/// <param name="subjectFooter">タイトル</param>
		/// <param name="messages">メッセージ</param>
		/// <param name="attachmentFilePath">添付ファイルパス</param>
		public void SendMail(string subjectFooter, string messages, params string[] attachmentFilePath)
		{
			using (var sendMail = new MailSendUtility(Constants.MailSendMethod.Auto))
			{
				sendMail.SetSubject(Constants.MAIL_SUBJECTHEAD + subjectFooter);
				sendMail.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => sendMail.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => sendMail.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => sendMail.AddBcc(mail.Address));
				sendMail.SetBody(messages);
				attachmentFilePath.ToList().ForEach(path => sendMail.AttachmentFilePath.Add(path));
				if (sendMail.SendMail() == false)
				{
					FileLogger.WriteError(sendMail.Body, sendMail.MailSendException);
				}
			}
		}
	}
}
