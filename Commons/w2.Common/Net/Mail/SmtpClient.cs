/*
=========================================================================================================
  Module      : SMTPクライアントモジュール(SmtpClient.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Mime;
using System.Net.Sockets;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using w2.Common.Extensions;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Common.Web;

namespace w2.Common.Net.Mail
{
	///**************************************************************************************
	/// <summary>
	/// SMTP接続をしてメールを送信する
	/// </summary>
	///**************************************************************************************
	public class SmtpClient
	{
		/// <summary>base64エンコードの最大文字数</summary>
		const int MAXLENGTH_BASE64_LINE = 76;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="smtpServer">SMTPサーバー</param>
		/// <param name="smtpPort">SMTPポート</param>
		public SmtpClient(string smtpServer, int smtpPort)
		{
			// SMTPサーバセット
			this.SmtpServerName = smtpServer;
			this.SmtpServerPort = smtpPort;

			// Boundaryセット
			Random rand = new Random();
			this.Boundary1 = rand.Next().ToString("x") + rand.Next().ToString("x") + rand.Next().ToString("x");
			this.Boundary2 = rand.Next().ToString("x") + rand.Next().ToString("x") + rand.Next().ToString("x");
			this.BoundaryRelated = rand.Next().ToString("x") + rand.Next().ToString("x") + rand.Next().ToString("x");

			// 日付セット用カルチャー情報セット
			this.USCulture = (CultureInfo)(System.Threading.Thread.CurrentThread.CurrentCulture.Clone());	// newするより高速
			this.USCulture.DateTimeFormat.Calendar = new GregorianCalendar(GregorianCalendarTypes.USEnglish);
		}

		/// <summary>
		/// SMTP認証情報設定
		/// </summary>
		/// <param name="userName">SMTP認証ユーザ名</param>
		/// <param name="password">SMTP認証パスワード</param>
		public void SetSmtpAuthCredential(string userName, string password)
		{
			this.SmtpAuthUserName = userName;
			this.SmtpAuthPassword = password;
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="mailAddress">メールアドレス</param>
		public void SendMail(MailMessage message, string userId, string mailAddress)
		{
			if (message.To.Count == 0)
			{
				throw new w2Exception("メールの送信先(To)が指定されていません。");
			}

			// Validate sender email address
			if ((message.From != null) && (Validator.IsMailAddress(message.From.AddressRaw) == false))
			{
				var errorMessage = new StringBuilder();
				errorMessage.Append(MessageManager.GetMessages(MessageManager.INPUTCHECK_MAILADDRESS).Replace("@@ 1 @@", "送信元メールアドレス")).Append("\r\n");
				errorMessage.Append(string.Format("・送信元のメールアドレス情報: {0}", message.From.AddressRaw)).Append("\r\n");
				errorMessage.Append(string.Format("・メール件名： {0}", message.Subject)).Append("\r\n");
				errorMessage.Append(string.Format("・メッセージID： {0}", message.MessageId));

				throw new w2Exception(errorMessage.ToString());
			}

			using (TcpClient client = new TcpClient(this.SmtpServerName, this.SmtpServerPort))
			using (NetworkStream stream = client.GetStream())
			{
				// 読み込みタイムアウト指定
				stream.ReadTimeout = Constants.MAILSEND_NETWORK_STREAM_READ_TIMEOUT;

				this.IsWritingMailData = false;

				//------------------------------------------------------
				// 初期コマンド
				//------------------------------------------------------
				// 初期読み込み
				Recv(stream, "220");

				if (Constants.SERVER_SMTP_AUTH_TYPE == SmtpAuthType.SmtpAuth)
				{
					// EHLO
					Send(stream, "EHLO " + this.SmtpServerName);
					Recv(stream, "250");

					Send(stream, "auth login");
					Recv(stream, "334");

					Send(stream, GetBase64String(
						this.SmtpAuthUserName,
						Encoding.Default));
					Recv(stream, "334");

					Send(stream, GetBase64String(
						this.SmtpAuthPassword,
						Encoding.Default));
					Recv(stream, "235");
				}
				else
				{
					// HELO
					Send(stream, "HELO " + this.SmtpServerName);
					Recv(stream, "250");
				}

				// リセット
				Send(stream, "RSET");
				Recv(stream, "250");

				//------------------------------------------------------
				// メールのFROM（エラーの場合の返信先）
				//------------------------------------------------------
				if (message.ReturnPath != null)
				{
					Send(stream, "MAIL FROM:<" + message.ReturnPath.Address + ">");
				}
				else
				{
					if (message.From != null)
					{
						Send(stream, "MAIL FROM:<" + message.From.Address + ">");
					}
					else
					{
						Send(stream, "MAIL FROM:<" + ">");
					}
				}
				Recv(stream, "250");

				//------------------------------------------------------
				// メールのTO,CC,BCCをRCPT TOとして送信
				//------------------------------------------------------
				foreach (MailAddress address in message.To)
				{
					if (address.Address.Length == 0)
						continue;

					Send(stream, "RCPT TO:<" + address.Address + ">");
					Recv(stream, "250");
				}
				foreach (MailAddress address in message.CC)
				{
					if (address.Address.Length == 0)
						continue;

					Send(stream, "RCPT TO:<" + address.Address + ">");
					Recv(stream, "250");
				}
				foreach (MailAddress address in message.Bcc)
				{
					if (address.Address.Length == 0)
						continue;

					Send(stream, "RCPT TO:<" + address.Address + ">");
					Recv(stream, "250");
				}

				// データ開始
				Send(stream, "DATA");
				Recv(stream, "354");
				
				this.IsWritingMailData = true;
				this.MailData = new StringBuilder();


				// 署名の問題で作ってすぐSendしてはいけないため、header送信内容をストックしておく
				var headerSendQue = new List<MailSendTextTemp>();

				// FROM（「From: 」を考慮して改行処理を入れる必要あり）
				var from = GetMailaddressString(message.From, message.HeaderMIMEEncoder, "From: ");
				headerSendQue.Add(new MailSendTextTemp(from));

				// Sender
				if (message.Sender != null)
				{
					var sender = "Sender: " + GetMailaddressString(message.Sender, message.HeaderMIMEEncoder);
					headerSendQue.Add(new MailSendTextTemp(sender));
				}

				// ReplyTo
				if (message.ReplyTo != null)
				{
					var replyTo = "Reply-To: " + GetMailaddressString(message.ReplyTo, message.HeaderMIMEEncoder);
					headerSendQue.Add(new MailSendTextTemp(replyTo));
				}

				// To
				if (message.To.Count != 0)
				{
					var to = "To: " + GetMailaddressString(message.To, message.HeaderMIMEEncoder);
					headerSendQue.Add(new MailSendTextTemp(to));
				}

				// Cc
				if (message.CC.Count != 0)
				{
					var cc = "Cc: " + GetMailaddressString(message.CC, message.HeaderMIMEEncoder);
					headerSendQue.Add(new MailSendTextTemp(cc));
				}

				// Bccは記述しない

				// List-Unsubscribe
				// List-Unsubscribe-Post
				if (Constants.MAIL_LISTUNSUBSCRIBE_OPTION_ENABLED)
				{
					// URLは必ず付ける
					// Mailは設定値にあれば付ける
					// UserIDがない場合、URLよりもMailを優先する
					var unsubscribeTexts = new List<string>();

					var unsubscribeUrl = new UrlCreator($"{Constants.PROTOCOL_HTTPS}{Constants.SITE_DOMAIN}{Constants.PATH_ROOT_FRONT_PC}{Constants.MAIL_LISTUNSUBSCRIBE_URL}")
						.AddParam(Constants.MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_USER_ID, userId)
						.AddParam(
							Constants.MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_VERIFICATION_KEY,
							UnsubscribeVarificationHelper.Hash(
								userId,
								string.IsNullOrEmpty(mailAddress)
									? message.To.Last().Address
									: mailAddress))
						.CreateUrl();
					unsubscribeTexts.Add($"<{unsubscribeUrl}>");

					if (string.IsNullOrEmpty(Constants.MAIL_LISTUNSUBSCRIBE_MAILTO) == false)
					{
						if (string.IsNullOrEmpty(userId)) unsubscribeTexts.Insert(index: 0, $"<mailto:{Constants.MAIL_LISTUNSUBSCRIBE_MAILTO}>");
						else unsubscribeTexts.Add($"<mailto:{Constants.MAIL_LISTUNSUBSCRIBE_MAILTO}>");
					}

					var listUnsubscribe = $"List-Unsubscribe: {unsubscribeTexts.JoinToString(", ")}";
					headerSendQue.Add(new MailSendTextTemp(listUnsubscribe));

					var listUnsubscribePost = "List-Unsubscribe-Post: List-Unsubscribe=One-Click";
					headerSendQue.Add(new MailSendTextTemp(listUnsubscribePost));
				}

				// Return-path
				if (message.ReturnPath != null)
				{
					var returnPath = "Return-Path: <" + message.ReturnPath.Address + ">";
					headerSendQue.Add(new MailSendTextTemp(returnPath));
				}

				// Message-Id
				if (string.IsNullOrEmpty(message.MessageId) == false)
				{
					var messageId = "Message-Id: <" + message.MessageId + ">";
					headerSendQue.Add(new MailSendTextTemp(messageId));
				}

				// In-Reply-To
				if (string.IsNullOrEmpty(message.InReplyTo) == false)
				{
					var inReplyTo = "In-Reply-To: <" + message.InReplyTo + ">";
					headerSendQue.Add(new MailSendTextTemp(inReplyTo));
				}

				// References
				if (message.References.Length != 0)
				{
					var references = "References:"
						+ " <" + message.References[0] + ">"
						+ ((message.References.Length > 1) ? " <" + message.References[1] + ">" : "");
					Send(stream, references);
				}

				// 件名（「Subject: 」を考慮して改行処理を入れる必要あり）
				var subject = message.HeaderMIMEEncoder.Encode(message.Subject, "Subject: ", "", MAXLENGTH_BASE64_LINE);
				headerSendQue.Add(new MailSendTextTemp(subject));

				// 日付(Tue, 16 Aug 2005 22:01:42 +0900)
				var date = "Date: " + DateTime.Now.ToString("ddd, d MMM yyyy HH':'mm':'ss zz00", this.USCulture);
				headerSendQue.Add(new MailSendTextTemp(date));

				// その他
				var mimeVersion = "MIME-Version: 1.0";
				headerSendQue.Add(new MailSendTextTemp(mimeVersion));

				var xPriority = "X-Priority: " + (int)message.Priority;
				headerSendQue.Add(new MailSendTextTemp(xPriority));

				// Bodyの文言を作ってハッシュ化してHeaderに埋め込むまでSendしてはいけないため、body送信内容をストックしておく
				var bodySendQue = CreateBody(message);

				// DKIM署名
				// これを作るためにheaderやbodyの送信内容をストックしている
				// 署名対象のheader行より前に入れる必要があるため、先頭に入れている
				// また、設定値によっては失敗しメールが送られなくなるため、TryCatchを記載し、最低限メールの送信は担保する
				// この機能導入直後は全案件でログを注視すること
				if (Constants.MAIL_DKIM_OPTION_ENABLED)
				{
					try
					{
						var dkimSignature = "DKIM-Signature: " + DkimHelper.Sign(message, headerSendQue, bodySendQue);
						headerSendQue.Insert(index: 0, new MailSendTextTemp(dkimSignature));
					}
					catch (Exception e)
					{
						FileLogger.WriteError($"{message.From}から送信するメール「{message.Subject}」のDKIM署名時にエラーが発生したため、DKIM署名処理をスキップしました", e);
					}
				}

				// Header送信
				foreach (var header in headerSendQue)
				{
					if (header.Encodeing == null) Send(stream, header.MailSendText);
					else Send(stream, header.GetSendText(), header.Encodeing);
				}

				// Body送信
				foreach (var body in bodySendQue)
				{
					if (body.Encodeing == null) Send(stream, body.GetSendText());
					else Send(stream, body.GetSendText(), body.Encodeing);
				}

				Send(stream, "");

				// DATA終わり
				Send(stream, ".");
				Recv(stream, "250");

				this.IsWritingMailData = false;

				//------------------------------------------------------
				// 終了
				//------------------------------------------------------
				Send(stream, "QUIT");
				Recv(stream, "221");
			}
		}

		/// <summary>
		/// 本文を作成
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <returns>本文の行ごとのリスト</returns>
		private List<MailSendTextTemp> CreateBody(MailMessage message)
		{
			var sendQue = new List<MailSendTextTemp>();

			// 添付ファイル用のバウンダリ利用を定義
			bool blUseBoundary1 = false;
			if (message.HasAttachmentFiles || message.HasDecomeAttachmentFiles)
			{
				if ((message.AttachmentFilePath.Count >= 2)
					|| message.HasBody
					|| message.HasBodyHtml)
				{
					blUseBoundary1 = true;

					sendQue.Add(new MailSendTextTemp("Content-Type: multipart/mixed; boundary=\"" + this.Boundary1 + "\""));
					sendQue.Add(new MailSendTextTemp(""));
					sendQue.Add(new MailSendTextTemp("--" + this.Boundary1));
				}
			}

			//------------------------------------------------------
			// related パートの開始：docomo/softbank のデコメ用などに利用
			//------------------------------------------------------
			bool blUseBoundaryRelated = false;
			if (message.MultipartRelatedEnable)
			{
				blUseBoundaryRelated = true;

				sendQue.Add(new MailSendTextTemp("Content-Type: multipart/related; boundary=\"" + this.BoundaryRelated + "\""));
				sendQue.Add(new MailSendTextTemp(""));
				sendQue.Add(new MailSendTextTemp("--" + this.BoundaryRelated));
			}

			// 本文/HTMのバウンダリ利用を定義
			bool blUseBoundary2 = false;
			if (message.HasBody && message.HasBodyHtml)
			{
				blUseBoundary2 = true;

				sendQue.Add(new MailSendTextTemp("Content-Type: multipart/alternative; boundary=\"" + this.Boundary2 + "\""));
				sendQue.Add(new MailSendTextTemp(""));
				sendQue.Add(new MailSendTextTemp("--" + this.Boundary2));
			}

			// 本文出力
			if (message.HasBody)
			{

				sendQue.Add(new MailSendTextTemp("Content-Type: text/plain; charset=\"" + message.BodyEncoding.WebName + "\""));
				sendQue.Add(new MailSendTextTemp("Content-Transfer-Encoding: " + GetTransferEncodingString(message.BodyTransferEncodig)));
				// sendQue.Add(new MailSendTextTemp("Content-Disposition: inline"));
				sendQue.Add(new MailSendTextTemp("")); // （空行がBODY開始）

				var bodySend = message.BodyTransferEncodig == TransferEncoding.Base64
					? GetBase64String(StringUtility.ToEmpty(message.GetShapingBodyOneLineMaxLen()), message.BodyEncoding, Base64FormattingOptions.InsertLineBreaks)
					: StringUtility.ToEmpty(message.GetShapingBodyOneLineMaxLen());
				sendQue.Add(new MailSendTextTemp(EscapeData(bodySend), EscapeData(bodySend), message.BodyEncoding));
			}

			// 本文HTML出力
			if (message.HasBodyHtml)
			{
				if (blUseBoundary2)
				{
					sendQue.Add(new MailSendTextTemp("--" + this.Boundary2));
				}

				sendQue.Add(new MailSendTextTemp("Content-Type: text/html; charset=\"" + message.BodyHtmlEncoding.WebName + "\""));
				sendQue.Add(new MailSendTextTemp("Content-Transfer-Encoding: " + GetTransferEncodingString(message.BodyTransferEncodigHtml)));
				sendQue.Add(new MailSendTextTemp(""));

				var bodyHtmlSend = message.BodyTransferEncodigHtml == TransferEncoding.Base64
					? GetBase64String(StringUtility.ToEmpty(message.BodyHtml), message.BodyHtmlEncoding, Base64FormattingOptions.InsertLineBreaks)
					: StringUtility.ToEmpty(message.BodyHtml);

				sendQue.Add(new MailSendTextTemp(EscapeData(bodyHtmlSend), EscapeData(bodyHtmlSend), message.BodyHtmlEncoding));

				if (blUseBoundary2)
				{
					sendQue.Add(new MailSendTextTemp("--" + this.Boundary2 + "--"));
				}
			}

			//------------------------------------------------------
			// デコメ用の添付ファイルを出力
			//------------------------------------------------------
			int iDecomeAttachedCount = 0;
			foreach (var maAttachementFile in message.DecomeAttachmentFile)
			{
				if (blUseBoundary1)
				{
					if ((iDecomeAttachedCount > 0)
						|| message.HasBody
						|| message.HasBodyHtml)
					{
						sendQue.Add(
							blUseBoundaryRelated
								? new MailSendTextTemp("--" + this.BoundaryRelated)
								: new MailSendTextTemp("--" + this.Boundary1));
					}
				}

				//------------------------------------------------------
				// ファイル形式に沿ってContentTypeを出力する
				//------------------------------------------------------
				sendQue.Add(
					maAttachementFile.FileContentType != ""
						? new MailSendTextTemp("Content-Type: " + maAttachementFile.FileContentType)
						: new MailSendTextTemp("Content-Type: application/octet-stream"));

				sendQue.Add(new MailSendTextTemp("Content-Transfer-Encoding: base64"));

				//------------------------------------------------------
				// 添付ファイルに対するDispositionの設定を行う
				//	(docomo/softbank の場合にはinline、au はattachment が推奨される)
				//------------------------------------------------------   
				StringBuilder sbContentDisposition = new StringBuilder();
				if (maAttachementFile.FileContentDisposition == DecomeAttachment.ContentDisposition.Inline)
				{
					sbContentDisposition.Append("Content-Disposition: inline; ");
				}
				else
				{
					sbContentDisposition.Append("Content-Disposition: attachment; ");
				}

				sbContentDisposition.Append("filename=\"").Append(message.HeaderMIMEEncoder.Encode(maAttachementFile.FileName)).Append("\"");

				sendQue.Add(new MailSendTextTemp(sbContentDisposition.ToString()));

				//------------------------------------------------------
				// 画像をHTMLで利用可能とするため、ContentIDを指定
				//------------------------------------------------------
				if (maAttachementFile.FileContentId != "")
				{
					sendQue.Add(new MailSendTextTemp("Content-ID: <" + maAttachementFile.FileContentId + ">"));
				}

				sendQue.Add(new MailSendTextTemp(""));
				sendQue.Add(new MailSendTextTemp(Convert.ToBase64String(File.ReadAllBytes(maAttachementFile.FilePath), Base64FormattingOptions.InsertLineBreaks).TrimEnd()));

				//------------------------------------------------------
				// 添付ファイル部のバウンダリ閉じ
				//------------------------------------------------------
				if (blUseBoundary1)
				{
					if (iDecomeAttachedCount == message.DecomeAttachmentFile.Count - 1)
					{
						if (blUseBoundaryRelated == false)
						{
							sendQue.Add(new MailSendTextTemp("--" + this.Boundary1 + "--"));
						}
					}
				}

				iDecomeAttachedCount++;
			}

			//------------------------------------------------------
			// related パートの終了：docomo/softbank のデコメ用などに利用
			//------------------------------------------------------
			if (blUseBoundaryRelated)
			{
				sendQue.Add(new MailSendTextTemp("--" + this.BoundaryRelated + "--"));
				sendQue.Add(new MailSendTextTemp(""));
			}

			//------------------------------------------------------
			// 以下、通常の添付ファイル
			//------------------------------------------------------
			// 添付ファイル出力
			int iFileAttachedCount = 0;
			foreach (string strAttachementFilePath in message.AttachmentFilePath)
			{
				if (blUseBoundary1)
				{
					if ((iFileAttachedCount > 0)
						|| message.HasBody
						|| message.HasBodyHtml)
					{
						sendQue.Add(new MailSendTextTemp("--" + this.Boundary1));
					}
				}

				sendQue.Add(new MailSendTextTemp("Content-Type: application/octet-stream"));
				sendQue.Add(new MailSendTextTemp("Content-Transfer-Encoding: base64"));

				var sbFileName = new StringBuilder();

				// RFC2231では、以下のような方法で非ASCII文字の添付ファイル名を示すこととしている
				//sbFileName.Append("filename*=iso-2022-jp'ja'").Append(System.Web.HttpUtility.UrlEncode(Path.GetFileName(strAttachementFilePath), Encoding.GetEncoding("ISO-2022-JP")));

				// 一般的に使用されているこの方式はMIME違反だが、OutlookはRFC2231に対応してないのでこちらを採用
				sbFileName.Append("filename=\"").Append(message.HeaderMIMEEncoder.Encode(Path.GetFileName(strAttachementFilePath))).Append("\"");

				sendQue.Add(new MailSendTextTemp("Content-Disposition: attachment; " + sbFileName));
				sendQue.Add(new MailSendTextTemp(""));
				sendQue.Add(new MailSendTextTemp(Convert.ToBase64String(File.ReadAllBytes(strAttachementFilePath), Base64FormattingOptions.InsertLineBreaks).TrimEnd()));

				if (blUseBoundary1)
				{
					if (iFileAttachedCount == message.AttachmentFilePath.Count - 1)
					{
						sendQue.Add(new MailSendTextTemp("--" + this.Boundary1 + "--"));
					}
				}

				iFileAttachedCount++;
			}

			return sendQue;
		}

		/// <summary>
		/// コマンド送信
		/// </summary>
		/// <param name="stream">送信先ストリーム</param>
		/// <param name="strData">送信文字列</param>
		private void Send(NetworkStream stream, string strData)
		{
			Send(stream, strData, Encoding.ASCII);
		}
		/// <summary>
		/// コマンド送信
		/// </summary>
		/// <param name="stream">送信先ストリーム</param>
		/// <param name="strData">送信文字列</param>
		/// <param name="encoding">文字エンコード</param>
		private void Send(NetworkStream stream, string strData, Encoding encoding)
		{
			// コマンドを保管(エラー出力用）
			this.BeforeCommand = strData;

			byte[] data = encoding.GetBytes(strData.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n") + "\r\n");
			stream.Write(data, 0, data.Length);

			if (this.IsWritingMailData) this.MailData.Append(strData).Append("\r\n");
		}

		/// <summary>
		/// コマンド受信
		/// </summary>
		/// <param name="stream">受信先ストリーム</param>
		/// <param name="strCheckCode">チェックするコード</param>
		private string Recv(NetworkStream stream, string strCheckCode)
		{
			StringBuilder sbReceive = new StringBuilder(100);
			while (true)
			{
				int ch;
				try
				{
					ch = stream.ReadByte();
					if (ch == -1)
					{
						throw new NetworkIOException("予期しないEOFが検出されました（接続が切断されました）。", null);
					}
				}
				catch (IOException ex)
				{
					throw new NetworkIOException("SMTPサーバーからのデータ読み取りに失敗しました。", ex);
				}

				sbReceive.Append((char)ch);

				if (sbReceive.ToString().EndsWith("\r\n")
					|| (sbReceive.Length > strCheckCode.Length + 500))
				{
					if (sbReceive.ToString().StartsWith(strCheckCode))
					{
						break;
					}

					// SMTP認証において、冗長なステータスコード250の応答が有りうるためスキップ
					if (sbReceive.ToString().StartsWith("250"))
					{
						sbReceive.Append(Recv(stream, strCheckCode));
						break;
					}

					StringBuilder sbErrorMessage = new StringBuilder();
					sbErrorMessage.Append("SMTPコマンドエラー").Append(":").Append(strCheckCode).Append("\r\n");
					sbErrorMessage.Append("コマンド[").Append(this.BeforeCommand).Append("]\r\n");
					sbErrorMessage.Append("エラー[").Append(sbReceive.ToString().Replace("\r\n","")).Append("]");
						throw new w2Exception(sbErrorMessage.ToString());
				}
			}

			return sbReceive.ToString();
		}

		/// <summary>
		/// メールアドレス文字列取得
		/// </summary>
		/// <param name="lAddresses">メールアドレスリスト</param>
		/// <param name="mimeEncoder">MIMEエンコーダ</param>
		/// <returns>メールアドレス文字列</returns>
		private string GetMailaddressString(List<MailAddress> lAddresses, MIMEEncoder mimeEncoder)
		{
			StringBuilder sbAddresses = new StringBuilder();
			foreach (MailAddress ma in lAddresses)
			{
				if (sbAddresses.Length != 0)
				{
					sbAddresses.Append(" ,");
				}
				sbAddresses.Append(GetMailaddressString(ma, mimeEncoder));
			}

			return sbAddresses.ToString(); ;
		}
		/// <summary>
		/// メールアドレス文字列取得
		/// </summary>
		/// <param name="address">メールアドレス</param>
		/// <param name="mimeEncoder">MIMEエンコーダ</param>
		/// <param name="header">ヘッダ（「Subject: 」など）</param>
		/// <returns>メールアドレス文字列</returns>
		private string GetMailaddressString(MailAddress address, MIMEEncoder mimeEncoder, string header = "", int maxLength = MAXLENGTH_BASE64_LINE)
		{
			StringBuilder sbAddress = new StringBuilder();

			if (address != null)
			{
				if (address.DisplayName.Length != 0)
				{
					sbAddress.Append(mimeEncoder.Encode(address.DisplayName, header, " <" + address.Address + ">", maxLength));
				}
				else
				{
					sbAddress.Append(header + address.Address);
				}
			}

			return sbAddress.ToString();
		}

		/// <summary>
		/// BASE64エンコーディング
		/// </summary>
		/// <param name="src">対象文字列</param>
		/// <param name="encoding">文字エンコーディング</param>
		/// <param name="base64FormattingOptions">Base64フォーマットオプション</param>
		/// <returns>エンコード結果</returns>
		private string GetBase64String(string src, Encoding encoding, Base64FormattingOptions base64FormattingOptions = Base64FormattingOptions.None)
		{
			return Convert.ToBase64String(encoding.GetBytes(StringUtility.ToEmpty(src)), base64FormattingOptions);
		}

		/// <summary>
		/// TransferEncoding文字列取得
		/// </summary>
		/// <param name="transferEncoding">TransferEncoding</param>
		/// <returns>transferEncoding文字列</returns>
		private string GetTransferEncodingString(TransferEncoding transferEncoding)
		{
			switch (transferEncoding)
			{
				case TransferEncoding.Base64:
					return "base64";

				case TransferEncoding.SevenBit:
					return "7bit";

				case TransferEncoding.QuotedPrintable:
					return "quoted-printable";
			}
			return "";
		}

		/// <summary>
		/// DATA部分の.だけの行をエスケープする
		/// </summary>
		/// <param name="str">DATA文字列</param>
		public string EscapeData(string str)
		{
			// 改行コードを[CRLF]に統一する
			str = str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");

			// ドット（.）はメールの終了を示すため、行頭ドット（.）をエスケープ
			str = Regex.Replace(str, @"^\.", "..", RegexOptions.Multiline);

			return str;
		}

		/// <summary>SMTPサーバ名</summary>
		string SmtpServerName { get; set; }
		/// <summary>SMTPサーバポート</summary>
		int SmtpServerPort { get; set; }
		/// <summary>SMTP-AUTHユーザ名</summary>
		string SmtpAuthUserName { get; set; }
		/// <summary>SMTP-AUTHパスワード</summary>
		string SmtpAuthPassword { get; set; }
		/// <summary>boundary1</summary>
		string Boundary1 { get; set; }
		/// <summary>boundary2</summary>
		string Boundary2 { get; set; }
		/// <summary>boundaryRelated</summary>
		string BoundaryRelated { get; set; }
		/// <summary>実行コマンド(エラー出力用)</summary>
		string BeforeCommand { get; set; }
		/// <summary>日付セット用カルチャー情報</summary>
		CultureInfo USCulture { get; set; }
		/// <summary>メール生データ出力中か</summary>
		private bool IsWritingMailData { get; set; }
		/// <summary>メール生データ</summary>
		public StringBuilder MailData { get; private set; }
	}
}
