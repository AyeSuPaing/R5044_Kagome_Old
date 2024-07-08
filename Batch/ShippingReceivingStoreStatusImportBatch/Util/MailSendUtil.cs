/*
=========================================================================================================
  Module      : Mail Send Utility (MailSendUtil.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common;
using w2.App.Common.Mail;
using w2.Domain.MailTemplate;
using w2.Domain.Order;

namespace w2.Commerce.Batch.ShippingReceivingStoreStatusImportBatch.Util
{
	/// <summary>
	/// Mail Send Utility
	/// </summary>
	public class MailSendUtil
	{
		#region Fields
		/// <summary>Flag Send Sms</summary>
		private const string FLG_SEND_SMS = "1";
		/// <summary>Flag Send Mail</summary>
		private const string FLG_SEND_MAIL = "2";
		/// <summary>Flag Send Mail And Sms</summary>
		private const string FLG_SEND_SMS_AND_MAIL = "3";
		#endregion

		#region Method
		/// <summary>
		/// Send Mail
		/// </summary>
		/// <param name="orderShipping">Order Shipping Model</param>
		/// <param name="userId">User Id</param>
		/// <param name="mailAddress">Mail Address</param>
		public void SendMail(OrderShippingModel orderShipping, string userId, string mailAddress)
		{
			var service = new MailTemplateService();
			var mailTemplate = service.Get(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_SEND_PRODUCT_RECEIPT);

			if (mailTemplate == null) return;

			switch (Constants.RECEIVINGSTORE_TWPELICAN_MAILSENDMETHOD)
			{
				case FLG_SEND_SMS:
					mailTemplate.SmsUseFlg = MailTemplateModel.SMS_USE_FLG_ON;
					mailTemplate.AutoSendFlg = Constants.FLG_MAILTEMPLATE_AUTOSENDFLG_NOTSEND;
					break;

				case FLG_SEND_MAIL:
					mailTemplate.SmsUseFlg = MailTemplateModel.SMS_USE_FLG_OFF;
					mailTemplate.AutoSendFlg = Constants.FLG_MAILTEMPLATE_AUTOSENDFLG_SEND;
					break;

				case FLG_SEND_SMS_AND_MAIL:
					mailTemplate.SmsUseFlg = MailTemplateModel.SMS_USE_FLG_ON;
					mailTemplate.AutoSendFlg = Constants.FLG_MAILTEMPLATE_AUTOSENDFLG_SEND;
					break;
			}
			service.Update(mailTemplate);
			var input = new MailTemplateDataCreaterForOrder(false)
				.GetOrderMailDatas(orderShipping.OrderId);

			// Send mail
			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				Constants.CONST_MAIL_ID_SEND_PRODUCT_RECEIPT,
				userId,
				input,
				true,
				Constants.MailSendMethod.Auto,
				mailAddress))
			{
				mailSender.AddTo(mailAddress);
				mailSender.SendMail();
			}
		}
		#endregion
	}
}
