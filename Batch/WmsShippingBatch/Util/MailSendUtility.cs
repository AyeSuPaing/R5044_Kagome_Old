/*
=========================================================================================================
  Module      : Mail Send Utility (MailSendUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Text;
using w2.App.Common.Elogit;
using w2.App.Common.Mail;
using w2.App.Common.SendMail;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.Commerce.Batch.WmsShippingBatch.Util
{
	/// <summary>
	/// Mail send utility
	/// </summary>
	public class MailSendUtility
	{
		#region Constants
		/// <summary>Constant key message</summary>
		public const string CONST_KEY_MESSAGE = "message";
		#endregion

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public MailSendUtility()
		{
			this.UserMailAddress = string.Empty;
			this.UserId = string.Empty;
			this.Message = string.Empty;
		}
		#endregion

		#region Method
		/// <summary>
		/// Create send mail
		/// </summary>
		/// <param name="input">Input information</param>
		private void CreateSendMail(Hashtable input)
		{
			// Set message
			var mailMessage = new StringBuilder();
			mailMessage.AppendLine(StringUtility.ToEmpty(input[CONST_KEY_MESSAGE]));

			// Check contains key
			if (input.ContainsKey(ElogitConstants.KEY_RESPONSE_MESSAGE))
			{
				mailMessage.AppendLine(StringUtility.ToEmpty(input[ElogitConstants.KEY_RESPONSE_MESSAGE]));
			}
			if (input.ContainsKey(ElogitConstants.KEY_LOG_TEXT))
			{
				mailMessage.AppendLine(StringUtility.ToEmpty(input[ElogitConstants.KEY_LOG_TEXT]));
			}
			if (input.ContainsKey(ElogitConstants.KEY_IF_HISTORY_KEY))
			{
				mailMessage.AppendLine(StringUtility.ToEmpty(input[ElogitConstants.KEY_IF_HISTORY_KEY]));
			}
			if (input.ContainsKey(Constants.FIELD_ORDER_ORDER_ID))
			{
				mailMessage.Append(StringUtility.ToEmpty(input[Constants.FIELD_ORDER_ORDER_ID]));
			}

			this.Message = mailMessage.ToString();

			SendMail();
		}

		/// <summary>
		/// Sends mail
		/// </summary>
		private void SendMail()
		{
			var input = new Hashtable
			{
				{ CONST_KEY_MESSAGE, this.Message },
				{ Constants.FIELD_ORDER_ORDER_ID, StringUtility.ToEmpty(this.OrderId) },
			};

			using (var mailUtil = new w2.App.Common.MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				this.MailTemplateId,
				this.UserId,
				input,
				true,
				Constants.MailSendMethod.Manual))
			{
				// Add mail to address for user
				if (string.IsNullOrEmpty(this.UserMailAddress) == false) mailUtil.AddTo(this.UserMailAddress);

				// Send mail
				if (mailUtil.SendMail() == false) FileLogger.WriteError(mailUtil.MailSendException);
			}
		}

		/// <summary>
		/// Send mail to operator
		/// </summary>
		/// <param name="input">Input information</param>
		public void SendMailToOperator(Hashtable input)
		{
			this.MailTemplateId = Constants.CONST_MAIL_ID_WMS_COOPERATION_FOR_OPERATOR;

			CreateSendMail(input);
		}

		/// <summary>
		/// Send mail to user
		/// </summary>
		/// <param name="orderId">Order id</param>
		public void SendMailToUser(string orderId)
		{
			var dataSendMail = new MailTemplateDataCreaterForOrder(true).GetOrderMailDatas(orderId);
			string languageCode = null;
			string languageLocaleId = null;
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				// 注文者、定期台帳に設定されている言語コードを初期表示する
				languageCode = (string)dataSendMail[Constants.FIELD_USER_DISP_LANGUAGE_CODE];
				languageLocaleId = (string)dataSendMail[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID];
			}

			// ユーザー向け
			SendMailCommon.SendMailToUser(
				Constants.CONST_MAIL_ID_ORDER_SHIPPING,
				(string)dataSendMail[Constants.FIELD_USER_USER_ID],
				dataSendMail,
				(string.IsNullOrEmpty((string)dataSendMail[Constants.FIELD_USER_MAIL_ADDR]) == false),
				languageCode,
				languageLocaleId);
		}
		#endregion

		#region Properties
		/// <summary>User id</summary>
		public string UserId { get; set; }
		/// <summary>Message</summary>
		public string Message { get; set; }
		/// <summary>Mail template id</summary>
		public string MailTemplateId { get; set; }
		/// <summary>User mail address</summary>
		public string UserMailAddress { get; set; }
		/// <summary>Order id</summary>
		public string OrderId { get; set; }
		#endregion
	}
}
