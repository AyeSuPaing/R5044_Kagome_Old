using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using w2.Common.Logger;

namespace w2.App.Common.User
{
	public partial class UserEventBinder : ContextBoundObject,  w2.Plugin.IPluginHost
	{
		/// <summary>
		/// インターフェースプロパティ初期化
		/// </summary>
		private void InitializeInterface()
		{
			this.Context = HttpContext.Current;
		}

		#region IPluginHost メンバー

		/// <summary>
		/// エラーログ書き出し
		/// </summary>
		/// <param name="message">メッセージ</param>
		public void WriteErrorLog(string message)
		{
			FileLogger.WriteError(message);
		}
		/// <summary>
		/// エラーログ書き出し
		/// </summary>
		/// <param name="ex">例外</param>
		/// <param name="message">メッセージ</param>
		public void WriteErrorLog(Exception ex, string message = "")
		{
			FileLogger.WriteError(message, ex);
		}

		/// <summary>
		/// インフォログ書き出し
		/// </summary>
		/// <param name="message">メッセージ</param>
		public void WriteInfoLog(string message)
		{
			FileLogger.WriteInfo(message);
		}
		/// <summary>
		/// インフォログ書き出し
		/// </summary>
		/// <param name="ex">例外</param>
		/// <param name="message">メッセージ</param>
		public void WriteInfoLog(Exception ex, string message = "")
		{
			FileLogger.WriteInfo(message, ex);
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="title">件名</param>
		/// <param name="body">本文</param>
		/// <returns>送信結果</returns>
		public bool SendMail(string subject, string body)
		{
			return SendMail(
				subject,
				body,
				Constants.MAILADDRESS_FROM_FOR_PLUGIN,
				Constants.MAILADDRESS_TO_LIST_FOR_PLUGIN,
				Constants.MAILADDRESS_CC_LIST_FOR_PLUGIN,
				Constants.MAILADDRESS_BCC_LIST_FOR_PLUGIN);
		}
		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="title">件名</param>
		/// <param name="body">本文</param>
		/// <param name="sendFrom">From</param>
		/// <param name="toList">To</param>
		/// <param name="ccList">Cc</param>
		/// <param name="bccList">Bcc</param>
		/// <returns>メール送信結果</returns>
		public bool SendMail(string subject, string body, string from, List<string> toList, List<string> ccList = null, List<string> bccList = null)
		{
			// 初期化
			if (ccList == null) ccList = new List<string>();
			if (bccList == null) bccList = new List<string>();

			MailSendUtility mailSender = new MailSendUtility(Constants.MailSendMethod.Auto);

			// メール設定
			mailSender.SetFrom(from);
			toList.ForEach(address => mailSender.AddTo(address));
			ccList.ForEach(address => mailSender.AddCC(address));
			bccList.ForEach(address => mailSender.AddBcc(address));
			mailSender.SetSubject(subject);
			mailSender.SetBody(body);

			return mailSender.SendMail();
		}

		/// <summary>ユーザ情報 (HACK: 今後UserModelを利用する)</summary>
		public Hashtable Data { get; private set; }
		/// <summary>HTTPリクエスト情報</summary>
		public HttpContext Context { get; private set; }

		#endregion
	}
}
