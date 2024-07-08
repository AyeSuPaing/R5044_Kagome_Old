/*
=========================================================================================================
  Module      : メール送信モジュール(SmtpMailSender.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using w2.Common.Logger;
using w2.Common.Util;

namespace w2.Common.Net.Mail
{
	///**************************************************************************************
	/// <summary>
	/// SmtpClientを利用してメールを送信する
	/// </summary>
	///**************************************************************************************
	public class SmtpMailSender : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SmtpMailSender()
			: this(Constants.SERVER_SMTP, Constants.SERVER_SMTP_PORT)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="smtpServer">SMTPサーバ</param>
		public SmtpMailSender(string smtpServer)
			: this(smtpServer, Constants.SERVER_SMTP_PORT)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="smtpServer">SMTPサーバー</param>
		/// <param name="smtpPort">SMTPポート</param>
		/// <param name="message">メールメッセージ</param>
		public SmtpMailSender(string smtpServer, int smtpPort, MailMessage message = null)
		{
			// SMTPクライアント作成
			this.SmtpClientObject = new SmtpClient(smtpServer, smtpPort);
			this.SmtpAuthType = Constants.SERVER_SMTP_AUTH_TYPE;
			this.SmtpAuthPopServer = Constants.SERVER_SMTP_AUTH_POP_SERVER;
			this.SmtpAuthPopPort = Constants.SERVER_SMTP_AUTH_POP_PORT;
			this.SmtpAuthPopType = Constants.SERVER_SMTP_AUTH_POP_TYPE;
			this.SmtpAuthUserName = Constants.SERVER_SMTP_AUTH_USER_NAME;
			this.SmtpAuthPassword = Constants.SERVER_SMTP_AUTH_PASSOWRD;

			// メールメッセージ作成
			this.Message = message ?? new MailMessage();

			// リトライ回数初期化
			this.RetryCount = 0;
		}

		/// <summary>
		/// SMTP認証情報更新
		/// </summary>
		/// <param name="smtpAuthType">SMTP認証タイプ</param>
		/// <param name="smtpAuthPopServer">SMTP認証Popサーバー</param>
		/// <param name="smtpAuthPopPort">SMTP認証Popポート</param>
		/// <param name="smtpAuthPopType">SMTP認証Popタイプ</param>
		/// <param name="smtpAuthUserName">SMTP認証ユーザー名</param>
		/// <param name="smtpAuthPassword">SMTP認証パスワード</param>
		/// <remarks>別サーバーを利用したい時のみ使用(現在ScheduleManagerからのエラーメール送信に一部利用)</remarks>
		public void UpdateSmtpAuthInfo(
			SmtpAuthType smtpAuthType,
			string smtpAuthPopServer,
			string smtpAuthPopPort,
			PopType smtpAuthPopType,
			string smtpAuthUserName,
			string smtpAuthPassword)
		{
			this.SmtpAuthType = smtpAuthType;
			this.SmtpAuthPopServer = smtpAuthPopServer;
			this.SmtpAuthPopPort = smtpAuthPopPort;
			this.SmtpAuthPopType = smtpAuthPopType;
			this.SmtpAuthUserName = smtpAuthUserName;
			this.SmtpAuthPassword = smtpAuthPassword;
		}

		/// <summary>
		/// メール送信エンコーディング設定
		/// </summary>
		/// <param name="encoding">エンコーディング</param>
		/// <param name="transferEncoding">エンコーディング（Content-Transfer-Encoding用）</param>
		public void SetEncoding(Encoding encoding, TransferEncoding transferEncoding)
		{
			this.Message.HeaderEncoding = encoding;
			this.Message.BodyEncoding = encoding;
			this.Message.BodyTransferEncodig = transferEncoding;
			this.Message.BodyHtmlEncoding = encoding;
			this.Message.BodyTransferEncodigHtml = transferEncoding;
		}

		/// <summary>
		/// 送信元セット
		/// </summary>
		/// <param name="strMailAddress">送信元メールアドレス</param>
		public void SetFrom(string strMailAddress)
		{
			SetFrom(strMailAddress, "");
		}
		/// <summary>
		/// 送信元セット
		/// </summary>
		/// <param name="strMailAddress">送信元メールアドレス</param>
		/// <param name="strName">送信元名</param>
		public void SetFrom(string strMailAddress, string strName)
		{
			if (StringUtility.ToEmpty(strMailAddress) != "")
			{
				this.Message.From = new MailAddress(strMailAddress, strName);
			}
		}

		/// <summary>
		/// 送信先追加
		/// </summary>
		/// <param name="strMailAddress">送信先メールアドレス</param>
		public void AddTo(string strMailAddress)
		{
			AddTo(strMailAddress, "");
		}
		/// <summary>
		/// 送信先追加
		/// </summary>
		/// <param name="strMailAddress">送信先メールアドレス</param>
		/// <param name="strName">送信先名セット</param>
		public void AddTo(string strMailAddress, string strName)
		{
			if (StringUtility.ToEmpty(strMailAddress) != "")
			{
				this.Message.To.Add(new MailAddress(strMailAddress, strName));
			}
		}

		/// <summary>
		/// CC送信先追加
		/// </summary>
		/// <param name="strMailAddress">CC送信先メールアドレス</param>
		public void AddCC(string strMailAddress)
		{
			AddCC(strMailAddress, "");
		}
		/// <summary>
		/// CC送信先追加
		/// </summary>
		/// <param name="strMailAddress">CC送信先メールアドレス</param>
		/// <param name="strName">CC送信先名セット</param>
		public void AddCC(string strMailAddress, string strName)
		{
			if (StringUtility.ToEmpty(strMailAddress) != "")
			{
				this.Message.CC.Add(new MailAddress(strMailAddress, strName));
			}
		}

		/// <summary>
		/// Bcc送信先追加
		/// </summary>
		/// <param name="strMailAddress">Bcc送信先メールアドレス</param>
		public void AddBcc(string strMailAddress)
		{
			AddBcc(strMailAddress, "");
		}
		/// <summary>
		/// Bcc送信先追加
		/// </summary>
		/// <param name="strMailAddress">Bcc送信先メールアドレス</param>
		/// <param name="strName">Bcc送信先名セット</param>
		public void AddBcc(string strMailAddress, string strName)
		{
			if (StringUtility.ToEmpty(strMailAddress) != "")
			{
				this.Message.Bcc.Add(new MailAddress(strMailAddress, strName));
			}
		}

		/// <summary>
		/// ReplyToセット
		/// </summary>
		/// <param name="strMailAddress">ReplyToメールアドレス</param>
		public void SetReplyTo(string strMailAddress)
		{
			SetReplyTo(strMailAddress, "");
		}
		/// <summary>
		/// ReplyToセット
		/// </summary>
		/// <param name="strMailAddress">ReplyToメールアドレス</param>
		/// <param name="strName">ReplyTo名</param>
		public void SetReplyTo(string strMailAddress, string strName)
		{
			if (StringUtility.ToEmpty(strMailAddress) != "")
			{
				this.Message.ReplyTo = new MailAddress(strMailAddress, strName);
			}
		}

		/// <summary>
		/// Return-pathセット
		/// </summary>
		/// <param name="strMailAddress">Return-pathメールアドレス</param>
		public void SetReturnPath(string strMailAddress)
		{
			if (StringUtility.ToEmpty(strMailAddress) != "")
			{
				this.Message.ReturnPath = new MailAddress(strMailAddress);
			}
		}

		/// <summary>
		/// Senderセット
		/// </summary>
		/// <param name="strMailAddress">Senderメールアドレス</param>
		public void SetSender(string strMailAddress)
		{
			SetSender(strMailAddress, "");
		}
		/// <summary>
		/// Senderセット
		/// </summary>
		/// <param name="strMailAddress">Senderメールアドレス</param>
		/// <param name="strName">Sender名</param>
		public void SetSender(string strMailAddress, string strName)
		{
			if (StringUtility.ToEmpty(strMailAddress) != "")
			{
				this.Message.Sender = new MailAddress(strMailAddress, strName);
			}
		}

		/// <summary>
		/// メール重要度セット
		/// </summary>
		/// <param name="mpPriority">メール重要度</param>
		private void SetMailPriority(MailMessage.MailPriority mpPriority)
		{
			this.Message.Priority = mpPriority;
		}

		/// <summary>
		/// 送信先クリア
		/// </summary>
		public void ClearTo()
		{
			this.Message.To.Clear();
		}

		/// <summary>
		/// CC送信先クリア
		/// </summary>
		public void ClearCC()
		{
			this.Message.CC.Clear();
		}

		/// <summary>
		/// Bcc送信元クリア
		/// </summary>
		public void ClearBcc()
		{
			this.Message.Bcc.Clear();
		}

		/// <summary>
		/// メール送信例外クリア
		/// </summary>
		public void ClearMailSendException()
		{
			this.MailSendException = null;
		}

		/// <summary>
		/// 件名セット
		/// </summary>
		/// <param name="strSubject">件名</param>
		public void SetSubject(string strSubject)
		{
			this.Message.Subject = strSubject;
		}


		/// <summary>
		/// 本文セット
		/// </summary>
		/// <param name="strBody">本文</param>
		public void SetBody(string strBody)
		{
			this.Message.Body = strBody;
		}


		/// <summary>
		/// 本文HTMLセット
		/// </summary>
		/// <param name="strBodyHtml">本文HTML</param>
		public void SetBodyHtml(string strBodyHtml)
		{
			this.Message.BodyHtml = strBodyHtml;
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="userMailAddress">ユーザーメールアドレス</param>
		/// <returns>送信結果</returns>
		public bool SendMail(string userId = "", string userMailAddress = "")
		{
			this.UserId = userId;
			this.UserMailAddress = userMailAddress;

			bool blResut = false;

			try
			{
				// POP Before SMTP
				if (this.SmtpAuthType == SmtpAuthType.PopBeforeSmtp)
				{
					// POP接続のみ
					using (Pop3Client pop3
						= new Pop3Client(
							this.SmtpAuthPopServer,
							int.Parse(this.SmtpAuthPopPort),
							this.SmtpAuthUserName,
							this.SmtpAuthPassword,
							this.SmtpAuthPopType))
					{
						pop3.Connect();
					}
				}
				// SMTP AUTH
				else if (Constants.SERVER_SMTP_AUTH_TYPE == SmtpAuthType.SmtpAuth)
				{
					this.SmtpClientObject.SetSmtpAuthCredential(
						this.SmtpAuthUserName,
						this.SmtpAuthPassword);
				}

				// 送信操作を行うごとにメッセージIDを生成する
				// 但し、リトライ時は同じメッセージIDを使用する
				if ((this.RetryCount == 0) && (string.IsNullOrEmpty(this.Message.MessageId)))
				{
					this.Message.MessageId = MessageIdGenerator.Generate();
				}

				// 現状、濁点ありの半角カナ文字が「ｶﾞ」→「カ゛」のように変換されて送信されてしまうので、
				// メール送信前に半角カナ文字を全角カナ文字へ変換しておく。（全角カナ変換後：「ｶﾞ」→「ガ」）
				this.Message.Subject = StringUtility.ToZenkakuKatakanaFromHankaku(this.Message.Subject);
				this.Message.Body = StringUtility.ToZenkakuKatakanaFromHankaku(this.Message.Body);
				this.Message.BodyHtml = StringUtility.ToZenkakuKatakanaFromHankaku(this.Message.BodyHtml);

				try
				{
					// メール送信
					this.SmtpClientObject.SendMail(this.Message, this.UserId, this.UserMailAddress);

					blResut = true;
				}
				catch (SocketException)
				{
					if (this.CanRetry) return Resend();

					throw;
				}
				catch (NetworkIOException)
				{
					if (this.CanRetry) return Resend();

					throw;
				}
			}
			catch (Exception ex)
			{
				blResut = false;

				this.MailSendException = ex;
			}

			this.RetryCount = 0;

			return blResut;
		}

		/// <summary>
		/// 再送処理
		/// </summary>
		/// <returns></returns>
		private bool Resend()
		{
			// Sleep a little
			Thread.Sleep(Constants.MAILSEND_SLEEP_TIME);

			this.RetryCount++;

			AppLogger.Write(AppLogger.LOGKBN_WARN, "SmtpMailSender: " + this.RetryCount.ToString() + "回目の再送信を試みます。", true);

			return SendMail(this.UserId, this.UserMailAddress);
		}

		/// <summary>
		/// Dispose
		/// </summary>
		public void Dispose()
		{
			// 現状は何もしない
		}

		/// <summary>
		/// SMTPクライアント取得
		/// </summary>
		/// <returns>SMTPクライアント</returns>
		public SmtpClient GetSmtpClient()
		{
			return this.SmtpClientObject;
		}

		/// <summary>SMTPクライアント</summary>
		protected SmtpClient SmtpClientObject { get; private set; }
		/// <summary>SMTP認証タイプ</summary>
		private SmtpAuthType SmtpAuthType { get; set; }
		/// <summary>SMTP認証Popサーバー</summary>
		private string SmtpAuthPopServer { get; set; }
		/// <summary>SMTP認証Popポート</summary>
		private string SmtpAuthPopPort { get; set; }
		/// <summary>SMTP認証Popタイプ</summary>
		private PopType SmtpAuthPopType { get; set; }
		/// <summary>SMTP認証ユーザー名</summary>
		private string SmtpAuthUserName { get; set; }
		/// <summary>SMTP認証パスワード</summary>
		private string SmtpAuthPassword { get; set; }
		/// <summary>メールメッセージ</summary>
		public MailMessage Message { get; private set; }
		/// <summary>件名</summary>
		public string Subject
		{
			get { return this.Message.Subject; }
		}
		/// <summary>本文</summary>
		public string Body
		{
			get { return this.Message.Body; }
		}
		/// <summary>本文HTML</summary>
		public string BodyHtml
		{
			get { return this.Message.BodyHtml; }
		}
		/// <summary>添付ファイル</summary>
		public List<string> AttachmentFilePath
		{
			get { return this.Message.AttachmentFilePath; }
		}
		/// <summary>メール送信例外</summary>
		public Exception MailSendException { get; private set; }

		/// <summary>デコメ用添付ファイル</summary>
		public List<DecomeAttachment> DecomeAttachmentFile
		{
			get { return this.Message.DecomeAttachmentFile; }
		}
		/// <summary>マルチパートrelated設定</summary>
		public bool MultipartRelatedEnable
		{
			get { return this.Message.MultipartRelatedEnable; }
		}
		/// <summary>再送回数</summary>
		private int RetryCount { get; set; }
		/// <summary>再送可能判定</summary>
		private bool CanRetry
		{
			get { return (this.RetryCount < Constants.MAILSEND_RETRY_COUNT_MAX); }
		}
		/// <summary>ユーザーID</summary>
		/// <remarks>登録解除の認証情報作成に利用する</remarks>
		public string UserId { get; private set; }
		/// <summary>ユーザーメールアドレス</summary>
		/// <remarks>登録解除の認証情報作成に利用する</remarks>
		public string UserMailAddress { get; private set; }
	}
}
