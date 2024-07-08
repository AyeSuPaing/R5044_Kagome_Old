/*
=========================================================================================================
  Module      : POP3メッセージモジュール(Pop3Message.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using w2.Common.Util;

namespace w2.Common.Net.Mail
{
	///**************************************************************************************
	/// <summary>
	/// POP3受信メール情報を格納する
	/// </summary>
	///**************************************************************************************
	public class Pop3Message
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private Pop3Message()
		{
			this.Attachment = new List<Pop3Message>();
			this.To = new List<MailAddress>();
			this.Cc = new List<MailAddress>();
			this.References = new List<string>();
			this.IsUpdateMessageId = false;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="src">メールのソース文字列。「ヘッダ部」「空行」「ボディ部」から構成される</param>
		public Pop3Message(string src)
			: this()
		{
			// 1行ずつ読み込んで、ヘッダとボディを分解する
			var headAndBody = SplitHeaderAndBody(src);

			// 初期化
			Initialize(
				new Pop3MessageHeader(headAndBody.Key),
				headAndBody.Value);
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="header">ヘッダ</param>
		/// <param name="bodyRaw">生ボディ</param>
		public Pop3Message(Pop3MessageHeader header, string bodyRaw)
			: this()
		{
			Initialize(header, bodyRaw);
		}
		#endregion

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="header">ヘッダ</param>
		/// <param name="bodyRaw">生ボディ</param>
		private void Initialize(Pop3MessageHeader header, string bodyRaw)
		{
			// 各プロパティにセット
			this.Header = header;
			this.BodyRaw = bodyRaw;
			this.Source = header.Source + "\r\n" + bodyRaw;

			// 最後にパースする（本文のみ）
			ParseHeader();
			ParseBody();
		}

		#region -SplitHeaderAndBody ヘッダとボディ分割
		/// <summary>
		/// ヘッダとボディ分割
		/// </summary>
		/// <param name="src"></param>
		/// <returns></returns>
		private KeyValuePair<string, string> SplitHeaderAndBody(string src)
		{
			var headerString = new StringBuilder();
			string bodyRaw = null;
			using (var reader = new StringReader(src))
			{
				while (reader.Peek() >= 0)
				{
					var line = reader.ReadLine();

					// ボディ判定
					if (line == "")
					{
						bodyRaw = reader.ReadToEnd();
						break;
					}
					// ヘッダ追加
					headerString.AppendLine(line);
				}
			}
			return new KeyValuePair<string, string>(headerString.ToString(), bodyRaw);
		}
		#endregion

		/// <summary>
		/// ヘッダデコード
		/// </summary>
		public void ParseHeader()
		{
			// 各アドレスなど設定
			var values = this.Header.Values.GetValues("from");
			if (values != null)
			{
				foreach (string value in values)
				{
					this.From = MailAddress.GetInstance(value);
				}
			}
			values = this.Header.Values.GetValues("to");
			if (values != null)
			{
				foreach (string value in values)
				{
					foreach (string strVal in value.Split(','))
					{
						this.To.Add(MailAddress.GetInstance(strVal));
					}
				}
			}
			values = this.Header.Values.GetValues("cc");
			if (values != null)
			{
				foreach (string value in values)
				{
					foreach (string v in value.Split(','))
					{
						this.Cc.Add(MailAddress.GetInstance(v));
					}
				}
			}
			values = this.Header.Values.GetValues("In-Reply-To");
			if (values != null)
			{
				var match = Regex.Match(values[0], "<.*>");
				this.InReplyTo = match.Success ? match.Value.Replace("<", "").Replace(">", "").Trim() : values[0];
			}
			values = this.Header.Values.GetValues("Reply-To");
			if (values != null)
			{
				foreach (string value in values)
				{
					this.ReplyTo = MailAddress.GetInstance(value);
				}
			}
			values = this.Header.Values.GetValues("References");
			if (values != null)
			{
				foreach (string value in values[0].Trim().Split(' '))
				{
					var match = Regex.Match(value, "<.*>");
					this.References.Add(match.Success ? match.Value.Replace("<", "").Replace(">", "").Trim() : value);
				}
			}
			values = this.Header.Values.GetValues("Message-Id");
			if (values != null)
			{
				var match = Regex.Match(values[0], "<.*>");
				this.MessageId = match.Success ? match.Value.Replace("<", "").Replace(">", "").Trim() : values[0];
			}
		}

		/// <summary>
		/// ボディデコード
		/// </summary>
		public void ParseBody()
		{
			var bodyString = this.BodyRaw.Replace("\r\n..", "\r\n.");

			// 添付ファイル判定/追加
			if (this.ContentType.MediaType.ToLower().StartsWith("multipart/"))
			{
				// バウンダリの外側に文字列が混入する可能性があるため両端は読み込まないようにするためdummyを設定
				var mimeParts = ("dummy\r\n" + bodyString + "\r\ndummy")
					.Split(new string[]
					{
						"\r\n--" + this.ContentType.Boundary + "\r\n",
						"\r\n--" + this.ContentType.Boundary + "--\r\n"
					}, StringSplitOptions.RemoveEmptyEntries);

				for (int i = 1; i < mimeParts.Length - 1; i++) // 両端はdummyのため読み込まない
				{
					this.Attachment.Add(new Pop3Message(mimeParts[i]));
				}
				var DefaultText = this.Attachment.FirstOrDefault(a => a.BodyDecoded != null);
				if (DefaultText != null) this.BodyDecoded = DefaultText.BodyDecoded;
			}

			// エンコード取得
			Encoding encodingFromHeader = null;
			try
			{
				if (this.ContentType.CharSet != null)
				{
					// TODO: Pop3MessageHeader.GetEncodingは、EncodeHelperなどに入れたほうが良いかも
					encodingFromHeader = Pop3MessageHeader.GetEncoding(this.ContentType.CharSet);
				}
			}
			catch (ArgumentException) { }
			encodingFromHeader = encodingFromHeader ?? Encoding.GetEncoding("ISO-2022-JP");

			// ボディデコード
			string transferEncoding = this.Header.Values["content-transfer-encoding"] ?? "";
			switch (transferEncoding.Trim().ToLower())
			{
				case "quoted-printable":
					this.BodyBytes = EncodeHelper.DecodeQuotedPrintableToBytes(encodingFromHeader, bodyString);
					break;

				case "base64":
					try
					{
						this.BodyBytes = EncodeHelper.DecodeBase64ToBytes(bodyString);
					}
					catch (Exception)
					{
						// 不正メール対策（BASE64で「!\r\n 」が間に挟まれているものがあった件対応）
						if (bodyString.Contains("!\r\n "))
						{
							try
							{
								this.BodyBytes = EncodeHelper.DecodeBase64ToBytes(bodyString.Replace("!\r\n ", ""));
							}
							catch (Exception) { }
						}

						// 変換できていなかったらデフォルトの値をセット
						if (this.BodyBytes == null) this.BodyBytes = new byte[0];
					}
					break;

				default:
					this.BodyBytes = encodingFromHeader.GetBytes(bodyString);
					break;
			}

			// テキストの場合はデコード
			if (this.ContentType.MediaType.ToLower().StartsWith("text/"))
			{
				this.BodyDecoded = encodingFromHeader.GetString(this.BodyBytes);
			}
		}

		/// <summary>
		/// メッセージIDセット
		/// </summary>
		/// <param name="messageId">メッセージID</param>
		public void SetMessageId(string messageId)
		{
			if(this.IsUpdateMessageId == false) this.OrgMessageId = this.MessageId;
			this.MessageId = messageId;
			this.IsUpdateMessageId = true;
		}

		/// <summary>メールのソース</summary>
		public string Source { get; private set; }
		/// <summary>メールヘッダ</summary>
		public Pop3MessageHeader Header { get; set; }
		/// <summary>メールの件名</summary>
		public string Subject
		{
			get { return this.Header.Values["subject"] ?? ""; }
		}
		/// <summary>メールの差出人</summary>
		public MailAddress From { get; set; }
		/// <summary>メールの日付</summary>
		public DateTime? Date
		{
			get
			{
				DateTime date;
				if (DateTime.TryParse(Regex.Replace(StringUtility.ToEmpty(this.Header.Values["date"]), @"\(.*\)", ""), out date)) return date;
				return null;
			}
		}
		/// <summary>メールの宛先</summary>
		public List<MailAddress> To { get; private set; }
		/// <summary>メールのCc</summary>
		public List<MailAddress> Cc { get; private set; }
		/// <summary>メールの返信先</summary>
		public MailAddress ReplyTo { get; set; }
		/// <summary>添付ファイルのコレクション</summary>
		public IList<Pop3Message> Attachment { get; private set; }
		/// <summary>Content-Type ヘッダー</summary>
		public ContentType ContentType { get { return this.Header.ContentType; } }
		/// <summary>メールボディ（生データ）</summary>
		public string BodyRaw { get; private set; }
		/// <summary>メールボディ（本文）</summary>
		public string BodyDecoded { get; set; }
		/// <summary>メールボディバイト列</summary>
		public byte[] BodyBytes { get; private set; }
		/// <summary>返信元MessageId</summary>
		public string InReplyTo { get; set; }
		/// <summary>返信元InReplyTo + MessageId</summary>
		public List<string> References { get; set; }
		/// <summary>メールMessageId</summary>
		public string MessageId { get; private set; }
		/// <summary>編集前メールMessageId</summary>
		public string OrgMessageId { get; private set; }
		/// <summary>MessageIdを編集したか</summary>
		public bool IsUpdateMessageId { get; private set; }
		/// <summary>マルチパートか</summary>
		public bool IsMultipart { get { return this.ContentType.MediaType.ToLower().StartsWith("multipart/"); } }
		/// <summary>添付ファイルか</summary>
		public bool IsAttachmentFile
		{
			get { return (this.Header.Values["Content-Disposition"] != null)
				&& this.Header.Values["Content-Disposition"].Split(';').Any(d => d.Trim().ToLower() == "attachment"); }
		}
		/// <summary>Gmail Message Id</summary>
		public string GmailMessageId { get; set; }
		/// <summary>Exchange Message Id</summary>
		public string ExchangeMessageId { get; set; }
		/// <summary>
		/// オブジェクトの等価性を、日付(Date)とメッセージID(MessageId)で比較します。
		/// </summary>
		/// <param name="obj">対象</param>
		/// <returns>等価か</returns>
		public override bool Equals(object obj)
		{
			if ((obj == null) || (obj.GetType() != this.GetType())) return false;

			Pop3Message target = (Pop3Message)obj;
			var messageId = this.IsUpdateMessageId ? this.OrgMessageId : this.MessageId;
			var targetMessageId = target.IsUpdateMessageId ? target.OrgMessageId : target.MessageId;
			var result = ((this.Date == target.Date) && (messageId == targetMessageId));
			return result;
		}
		/// <summary>
		/// オブジェクトのハッシュ値を、日付(Date)とメッセージID(MessageId)から生成します。
		/// </summary>
		/// <returns>ハッシュ値取得</returns>
		public override int GetHashCode()
		{
			return (this.Date == null ? 0 : this.Date.GetHashCode())
				^ (StringUtility.ToEmpty(this.MessageId).GetHashCode());
		}
	}
}
