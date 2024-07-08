/*
=========================================================================================================
  Module      : バッチメーラークラス(BatchMailer.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using w2.App.Common;
using w2.Common.Logger;

namespace UpdateSMSState
{
	/// <summary>
	/// バッチメーラークラス
	/// </summary>
	public class BatchMailer
	{
		/// <summary>バッチのメールテンプレートID</summary>
		const string BATCH_MAIL_TEMPLATE_ID = "10000082";

		/// <summary>
		/// アラートメール送信
		/// </summary>
		/// <param name="ex">エラー</param>
		public static void SendAlertMail(Exception ex)
		{
			var ht = new Hashtable();

			ht.Add("message", string.Format("SMS受信確認バッチにて、予期しないエラーが発生しました。\r\n{0}", ex.ToString()));

			SendMail(ht);
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="ht">置換パラメータ用ハッシュ</param>
		private static void SendMail(Hashtable ht)
		{
			using (MailSendUtility msMailSend = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, BATCH_MAIL_TEMPLATE_ID, "", ht))
			{
				if (msMailSend.SendMail() == false)
				{
					// 送信エラーの場合ログ書き込み
					FileLogger.WriteError(msMailSend.MailSendException.ToString());
				}
			}
		}
	}
}
