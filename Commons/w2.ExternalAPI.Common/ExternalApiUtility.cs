/*
=========================================================================================================
  Module      : External api utility(ExternalApiUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.IO;
using w2.App.Common;
using w2.Domain.MailTemplate;

namespace w2.ExternalAPI.Common
{
	public static class ExternalApiUtility
	{
		#region MoveFileToDirectory
		/// <summary>
		/// Move file to directory
		/// </summary>
		/// <param name="sourceFilePath">Source file path</param>
		/// <param name="targetDirectoryPath">Target file path</param>
		public static void MoveFileToDirectory(string sourceFilePath, string targetFilePath)
		{
			var targetDirectoryPath = new FileInfo(targetFilePath).DirectoryName;
			if (Directory.Exists(targetDirectoryPath) == false) Directory.CreateDirectory(targetDirectoryPath);

			File.Move(sourceFilePath, targetFilePath);
		}
		#endregion

		#region SendMailToOperator
		/// <summary>
		/// Send mail to operator
		/// </summary>
		/// <param name="replaceContent">Replace content</param>
		/// <param name="mailMessage">Mail message</param>
		/// <param name="shopId">Shop id</param>
		/// <param name="mailTemplateId">Mail template id</param>
		public static void SendMailToOperator(string replaceContent, string mailMessage, string shopId, string mailTemplateId)
		{
			var mailSendUtility = new MailSendUtility(Constants.MailSendMethod.Manual);
			var mailTemplate = new MailTemplateService().Get(shopId, mailTemplateId);
			if (mailTemplate != null)
			{
				mailSendUtility.MailId = mailTemplate.MailId;
				mailSendUtility.SetSubject(mailTemplate.MailSubject);
				mailSendUtility.SetFrom(mailTemplate.MailFrom);
				mailSendUtility.AddTo(mailTemplate.MailTo);
				mailSendUtility.SetBodyHtml(mailTemplate.MailBody.Replace("@@ message @@", mailMessage.Replace("@@ error @@", replaceContent)));

				mailSendUtility.SendMail();
			}
		}
		#endregion
	}
}