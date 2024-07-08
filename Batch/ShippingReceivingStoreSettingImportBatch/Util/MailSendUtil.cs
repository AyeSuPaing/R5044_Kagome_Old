/*
=========================================================================================================
  Module      : Mail Send Utility (MailSendUtil.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using w2.App.Common;
using w2.Domain.MailTemplate;

namespace w2.Commerce.Batch.ShippingReceivingStoreSettingImportBatch.Util
{
	/// <summary>
	/// Mail Send Utility
	/// </summary>
	public class MailSendUtil
	{
		#region Fields
		/// <summary>Title Mail</summary>
		private const string TITLE_MAIL = "コンビニ店舗情報取込";
		/// <summary>Result Import Success</summary>
		private const string RESULT_IMPORT_SUCCESS = "成功";
		/// <summary>Result Import Fail</summary>
		private const string RESULT_IMPORT_FAIL = "失敗";
		/// <summary>Tag Subject Title Mail</summary>
		private const string TAG_SUBJECT_TITLE_MAIL = "subject";
		/// <summary>Tag Result Message</summary>
		private const string TAG_RESULT_MESSAGE = "message";
		/// <summary>Tag Start DateTime</summary>
		private const string TAG_START_DATETIME = "startDateTime";
		/// <summary>Tag End DateTime</summary>
		private const string TAG_END_DATETIME = "endDateTime";
		#endregion

		#region Method
		/// <summary>
		/// Send Mail
		/// </summary>
		/// <param name="isResultImport">Is Result Import</param>
		/// <param name="startDateTime">Start Date Time</param>
		/// <param name="endDateTime">End Date Time</param>
		public void SendMail(bool isResultImport, string startDateTime, string endDateTime)
		{
			var mailTemplate = new MailTemplateService().Get(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_SEND_SHIPPING_RECEIVINGSTORE_IMPORTBATCH);

			if (mailTemplate == null) return;

			var input = new Hashtable
			{
				{ TAG_SUBJECT_TITLE_MAIL, TITLE_MAIL },
				{ TAG_RESULT_MESSAGE, (isResultImport) ? RESULT_IMPORT_SUCCESS : RESULT_IMPORT_FAIL },
				{ TAG_START_DATETIME, startDateTime },
				{ TAG_END_DATETIME, endDateTime }
			};

			// Send mail
			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_SEND_SHIPPING_RECEIVINGSTORE_IMPORTBATCH,
				string.Empty,
				input,
				true,
				Constants.MailSendMethod.Auto))
			{
				mailSender.SendMail();
			}
		}
		#endregion
	}
}