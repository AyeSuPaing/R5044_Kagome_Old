/*
=========================================================================================================
  Module      : Exchange Receiver (ExchangeReceiver.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Common.Net.Mail;

namespace w2.CustomerSupport.Batch.CsMailReceiver
{
	/// <summary>
	/// Exchange receiver
	/// </summary>
	public class ExchangeReceiver : MailReceiver
	{
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="setting">Setting</param>
		public ExchangeReceiver(ExchangeAccountSetting setting)
			: base(setting)
		{
		}

		/// <summary>
		/// Receive and import
		/// </summary>
		/// <returns>Amount mail import success</returns>
		public new int ReceiveAndImport()
		{
			var client = new ExchangeClient(this.ExchangeAccountSetting);
			var messages = client.GetMailMessages();
			if (messages == null) return 0;

			var deletedMailIds = new List<string>();
			foreach (var message in messages)
			{
				var success = ImportMail(message);
				if (success)
				{
					deletedMailIds.Add(message.ExchangeMessageId);
				}
			}

			client.DeleteMailMessages(deletedMailIds.ToArray());

			return deletedMailIds.Count;
		}
	}
}