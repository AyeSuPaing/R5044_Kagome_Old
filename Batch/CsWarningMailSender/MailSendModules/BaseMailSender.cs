/*
=========================================================================================================
  Module      : 警告メール送信基底クラス(BaseMailSender.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common;
using w2.App.Common.Cs.CsOperator;

namespace w2.CustomerSupport.Batch.CsWarningMailSender.MailSendModules
{
	/// <summary>
	/// 警告メール送信基底クラス
	/// </summary>
	public class BaseMailSender
	{
		#region -SetBodyAndSend  警告メールのTo/本文組み立て、送信
		/// <summary>
		/// 警告メールのTo/本文組み立て、送信
		/// </summary>
		/// <param name="mailId">送信メールOP</param>
		/// <param name="targetOperator">対象オペレータ</param>
		/// <param name="count">件数</param>
		/// <param name="days">警告日数</param>
		protected void SetBodyAndSend(string mailId, CsOperatorModel targetOperator, int count, int days)
		{
			var mailSender = new MailSendUtility(Constants.CONST_DEFAULT_DEPT_ID, mailId, "", new Hashtable(), true, Constants.MailSendMethod.Auto);
			mailSender.AddTo(targetOperator.MailAddr);
			mailSender.SetBody(mailSender.Body.Replace("@@ to @@", string.Format("To: {0}(ID:{1})", targetOperator.EX_ShopOperatorName, targetOperator.OperatorId)));
			mailSender.SetBody(mailSender.Body.Replace("@@ count @@", count.ToString()));
			mailSender.SetBody(mailSender.Body.Replace("@@ days @@", days.ToString()));
			if (mailSender.SendMail() == false) throw new Exception("警告メールの送信に失敗しました。");
		}
		#endregion
	}
}
