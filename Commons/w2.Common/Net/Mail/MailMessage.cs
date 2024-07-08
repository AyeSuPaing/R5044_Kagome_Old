/*
=========================================================================================================
  Module      : メールメッセージモジュール(MailMessage.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Net.Mime;
using w2.Common.Util;

namespace w2.Common.Net.Mail
{
	///*********************************************************************************************
	/// <summary>
	/// メールメッセージクラス
	/// </summary>
	///*********************************************************************************************
	public class MailMessage
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MailMessage()
		{
			this.From = null;
			this.To = new List<MailAddress>();
			this.CC = new List<MailAddress>();
			this.Bcc = new List<MailAddress>();
			this.References = new string[0];
			this.Priority = MailPriority.Normal;
			this.AttachmentFilePath = new List<string>();
			this.HeaderEncoding = Constants.PC_MAIL_DEFAULT_ENCODING;
			this.BodyEncoding = Constants.PC_MAIL_DEFAULT_ENCODING;
			this.BodyTransferEncodig = Constants.PC_MAIL_DEFAULT_TRANSFER_ENCODING;
			this.BodyHtmlEncoding = Constants.PC_MAIL_DEFAULT_ENCODING;
			this.BodyTransferEncodigHtml = Constants.PC_MAIL_DEFAULT_TRANSFER_ENCODING;
			this.DecomeAttachmentFile = new List<DecomeAttachment>();
			this.MultipartRelatedEnable = false;
		}

		/// <summary>
		/// 1行が長すぎる場合に整形をした本文を返却
		/// </summary>
		/// <remarks>
		/// メールは1行1000バイト超えるとダメなので
		/// ちょっと余裕をもって1行が490文字超えたら改行入れるように整形
		/// </remarks>
		public string GetShapingBodyOneLineMaxLen()
		{
			// まず改行で分割
			var lines = this.Body.Split(new string[] { "\r\n" }, StringSplitOptions.None);
			var result = new StringBuilder();

			foreach (var line in lines)
			{
				// 490文字でグループ化してぶん回す
				foreach (var splittedLine in Regex.Split(line, @"(?<=\G.{490})(?!$)"))
				{
					result.Append(splittedLine).Append("\r\n");
				}
			}
			return result.ToString();
		}

		/// <summary>メール送信元</summary>
		public MailAddress From { get; set; }
		/// <summary>メール送信先</summary>
		public List<MailAddress> To { get; private set; }
		/// <summary>メールCC送信先</summary>
		public List<MailAddress> CC { get; private set; }
		/// <summary>メールBcc送信先</summary>
		public List<MailAddress> Bcc { get; private set; }
		/// <summary>メールReply</summary>
		public MailAddress ReplyTo{ get; set; }
		/// <summary>メールSender</summary>
		public MailAddress Sender{ get; set; }
		/// <summary>メールReturn-path</summary>
		public MailAddress ReturnPath{ get; set; }
		/// <summary>返信元MessageId</summary>
		public string InReplyTo { get; set; }
		/// <summary>返信元InReplyTo + 本メールInReplyTo</summary>
		public string[] References { get; set; }
		/// <summary>メールMessageId</summary>
		public string MessageId { get; set; }
		/// <summary>ヘッダエンコーディング</summary>
		public Encoding HeaderEncoding
		{
			get { return m_headerEncoding; }
			set
			{
				m_headerEncoding = value;

				if (this.HeaderMIMEEncoder == null)
				{
					this.HeaderMIMEEncoder = new MIMEEncoder(this.HeaderEncoding);
				}
				this.HeaderMIMEEncoder.Encoding = this.HeaderEncoding;
				this.HeaderMIMEEncoder.TransferEncoding = TransferEncoding.Base64;
			}
		}
		/// <summary>ヘッダエンコーディング</summary>
		private Encoding m_headerEncoding = null;
		/// <summary>ヘッダMIMEエンコーダ</summary>
		public MIMEEncoder HeaderMIMEEncoder { get; private set; }
		/// <summary>件名</summary>
		public string Subject{ get; set; }
		/// <summary>本文エンコーディング</summary>
		public Encoding BodyEncoding{ get; set; }
		/// <summary>本文エンコーディング(Content-Transfer-Encoding用)</summary>
		public TransferEncoding BodyTransferEncodig { get; set; }
		/// <summary>本文</summary>
		public string Body{ get; set; }
		/// <summary>本文(HTML)エンコーディング</summary>
		public Encoding BodyHtmlEncoding{ get; set; }
		/// <summary>本文エンコーディングHTML(Content-Transfer-Encoding用)</summary>
		public TransferEncoding BodyTransferEncodigHtml { get; set; }
		/// <summary>本文(HTML)</summary>
		public string BodyHtml{ get; set; }
		/// <summary>添付ファイルパス</summary>
		public List<string> AttachmentFilePath{ get; private set; }
		/// <summary>重要度</summary>
		public MailPriority Priority{ get; set; }
		/// <summary>本文存在判定</summary>
		public bool HasBody
		{
			get { return (StringUtility.ToEmpty(this.Body) != ""); }
		}
		/// <summary>本文(HTML)存在判定</summary>
		public bool HasBodyHtml
		{
			get { return (StringUtility.ToEmpty(this.BodyHtml) != ""); }
		}
		/// <summary>添付ファイル存在判定</summary>
		public bool HasAttachmentFiles
		{
			get { return (this.AttachmentFilePath.Count > 0); }
		}
		/// <summary>デコメ用添付ファイル存在判定</summary>
		public bool HasDecomeAttachmentFiles
		{
			get { return (this.DecomeAttachmentFile.Count > 0); }
		}

		///**************************************************************************************
		/// <summary>
		/// 送信メール優先順位の列挙体
		/// </summary>
		///**************************************************************************************
		public enum MailPriority
		{
			/// <summary>重要度高</summary>
			Highest = 1,
			/// <summary>重要度やや高</summary>
			High,
			/// <summary>重要度普通</summary>
			Normal,
			/// <summary>重要度やや低</summary>
			Low,
			/// <summary>重要度低</summary>
			Lowest
		}

		/// <summary>デコメ用添付ファイルパス</summary>
		public List<DecomeAttachment> DecomeAttachmentFile { get; private set; }
		/// <summary>デコメ用マルチパート構成設定(auの場合、Relatedは設定しない)</summary>
		public bool MultipartRelatedEnable { get; set; }

		/// <summary>
		/// ContentID用の文字列を取得する
		/// auのContentIDでは＠が１つ必要なので、@を１つ含んだ形で生成
		/// </summary>
		/// <returns>6文字英数@6文字英数 の文字列</returns>
		public static string CreateContentID()
		{
			// ハイフンなし32桁の英数字を取得
			string strUniqueNumber = Guid.NewGuid().ToString("N"); // Nを指定しないとハイフンが入る

			// ContentID用の文字列として整形する
			return strUniqueNumber.Substring(0, 6) + "@" + strUniqueNumber.Substring(6, 6);
		}
	}
}
