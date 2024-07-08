/*
=========================================================================================================
  Module      : Gmail Receiver(GmailReceiver.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Common.Net.Mail;
using w2.Common.Util;

namespace w2.CustomerSupport.Batch.CsMailReceiver
{
	/// <summary>
	/// Gmail Receiver Class
	/// </summary>
	public class GmailReceiver : MailReceiver
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public GmailReceiver(GmailAccountSetting setting)
			: base(setting)
		{
		}

		/// <summary>
		/// Receive And Import
		/// </summary>
		/// <returns>Amount Mail Import Success</returns>
		public int ReceiveAndImport()
		{
			var client = new GmailClient(this.GmailAccountSetting);
			var messages = client.GetMessagesForAccountGmail();

			var listDeleteIds = new List<string>();
			foreach (var message in messages)
			{
				message.BodyDecoded = StringUtility.ToEmpty(message.BodyDecoded);

				if (message.ReplyTo != null) message.From = message.ReplyTo;
				if (string.IsNullOrEmpty(message.MessageId))
				{
					message.SetMessageId(MessageIdGenerator.Generate());
				}
				var success = ImportMail(message);

				if (success) listDeleteIds.Add(message.GmailMessageId);
			}
			client.DeleteMessagesForGmail(listDeleteIds.ToArray());
			return listDeleteIds.Count;
		}
	}
}