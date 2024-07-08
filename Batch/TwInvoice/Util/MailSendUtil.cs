/*
=========================================================================================================
  Module      : Mail Send Utility (MailSendUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Text;
using w2.App.Common;
using w2.Common.Logger;

namespace w2.Commerce.Batch.TwInvoice.Util
{
	/// <summary>
	/// Mail Send Utility
	/// </summary>
	public class MailSendUtil
	{
		#region Constant
		/// <summary>File name</summary>
		private const string FILE_NAME = "LogResultAPI";
		/// <summary>Message setting key message</summary>
		public const string MESSAGESETTING_KEY_MESSAGE = "message";
		/// <summary>Message setting key file name</summary>
		public const string MESSAGESETTING_KEY_FILE_NAME = "file_name";
		/// <summary>Message setting key result</summary>
		public const string MESSAGESETTING_KEY_RESULT = "result";
		/// <summary>Message setting key invoice remaining count</summary>
		public const string MESSAGESETTING_KEY_INVOICE_REMAINING_COUNT = "invoice_remaining_count";
		#endregion

		#region Constructors
		/// <summary>
		/// Constructor
		/// </summary>
		public MailSendUtil() { }
		#endregion

		#region Methods
		/// <summary>
		/// Send Mail
		/// </summary>
		/// <param name="hasError">hasError</param>
		/// <param name="code">code</param>
		/// <param name="trackInfo">trackInfo</param>
		/// <param name="message">message</param>
		public void SendMail(bool hasError, string code, string trackInfo, string message)
		{
			var mailContent = new StringBuilder();

			if (hasError)
			{
				mailContent.Append(Environment.NewLine)
				.Append("エラーコード：").Append(code).Append(Environment.NewLine)
				.Append("メッセージ：").Append(message);
			}
			else
			{
				var trackInfos = trackInfo.Split(',');
				for (var index = 0; index < trackInfos.Length; index = index + 3)
				{
					var start = decimal.Parse(trackInfos[index + 1]);
					var end = decimal.Parse(trackInfos[index + 2]);

					mailContent.Append(Environment.NewLine)
						.Append("字軌コード：").Append(trackInfos[index]).Append(Environment.NewLine)
						.Append("開始番号：").Append(trackInfos[index + 1]).Append(Environment.NewLine)
						.Append("終了番号：").Append(trackInfos[index + 2]).Append(Environment.NewLine)
						.Append("取得枚数：").Append(end - start + 1).Append("枚").Append(Environment.NewLine);
				}
			}

			var inputConvertKeyMessage = new Hashtable()
			{
				{ MESSAGESETTING_KEY_MESSAGE, mailContent.ToString() },
				{ MESSAGESETTING_KEY_FILE_NAME, string.Format("{0}{1}_{2}", Constants.PHYSICALDIRPATH_LOGFILE, FILE_NAME, DateTime.Now.Date.ToString("yyyyMMdd")) },
				{ MESSAGESETTING_KEY_RESULT, (hasError) ? "失敗" : "成功" }
			};

			using (var mailUtil = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_SEND_OPERATOR_FROM_BATCH_TWINVOICE,
				string.Empty,
				inputConvertKeyMessage,
				true,
				Constants.MailSendMethod.Manual))
			{
				FileLogger.Write(FILE_NAME, mailContent.ToString());

				if (mailUtil.SendMail() == false)
				{
					FileLogger.WriteError(mailUtil.MailSendException);
				}
			}
		}

		/// <summary>
		/// Send Mail Alert
		/// </summary>
		/// <param name="remainCount">Remain count</param>
		public void SendMailAlert(decimal remainCount)
		{
			var inputConvertKeyMessage = new Hashtable()
			{
				{ MESSAGESETTING_KEY_INVOICE_REMAINING_COUNT, remainCount.ToString() },
			};

			using (var mailUtil = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_SEND_ALERT_OPERATOR_FROM_BATCH_TWINVOICE,
				string.Empty,
				inputConvertKeyMessage,
				true,
				Constants.MailSendMethod.Manual))
			{
				if (mailUtil.SendMail() == false)
				{
					FileLogger.WriteError(mailUtil.MailSendException);
				}

				FileLogger.WriteInfo(mailUtil.Body);
			}
		}
		#endregion
	}
}