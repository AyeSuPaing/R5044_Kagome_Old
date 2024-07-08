/*
=========================================================================================================
  Module      : デコメ添付ファイルモジュール(DecomeAttachment.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using w2.Common.Util;
using System.Net.Mime;
using System.IO;

namespace w2.Common.Net.Mail
{
	///*********************************************************************************************
	/// <summary>
	/// デコメ添付ファイルクラス
	/// </summary>
	///*********************************************************************************************
	public class DecomeAttachment
	{
		/// <summary>デフォルトエンコーディング</summary>
		static Encoding DEFAULT_ENCODING = Encoding.GetEncoding("ISO-2022-JP");

		/// <summary>デフォルトエンコーディング(Content-Transfer-Encoding用)</summary>
		static string DEFAULT_TRANSFER_ENCODING = "base64";

		/// <summary>Content-Disposition用データ属性(デフォルト)</summary>
		const string CONTENT_DISPOSITION_INLINE = "inline";
		/// <summary>Content-Disposition用データ属性(auの場合はこちらを指定推奨)</summary>
		const string CONTENT_DISPOSITION_ATTACHMENT = "attachment";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strFilePath">デコメ用添付ファイルパス</param>
		public DecomeAttachment(string strFilePath)
		{
			// ファイルパスを設定
			this.FilePath = strFilePath;
			// 物理ファイル名を設定
			this.FileName = Path.GetFileName(strFilePath); // ファイル名をデフォルトで設定
			// 拡張子からContentTypeを設定
			this.FileContentType = GetContentsType(strFilePath);			// 拡張子から自動判別
			this.FileContentId = MailMessage.CreateContentID();				// 一意なContentIDを設定する
			this.FileContentTransferEncoding = DEFAULT_TRANSFER_ENCODING;	// BASE64をデフォルト設定
			this.FileContentDisposition = ContentDisposition.Inline;		// Inlineをデフォルト設定
		}

		/// <summary>ファイル取得元パス</summary>
		public string FilePath { get; private set; }
		/// <summary>添付時のファイル名</summary>
		public string FileName { get; set; }
		/// <summary>ファイル形式（Content-Typeの指定）</summary>
		public string FileContentType { get; private set; }
		/// <summary>ContentIdの指定 ※直接指定する場合は、HTML内の内容と一致させる必要あり</summary>
		public string FileContentId { get; private set; }
		/// <summary>格納方法の指定（Content-Transfer-Encoding　※7bit / base64 / quoted-printable）</summary>
		public string FileContentTransferEncoding { get; set; }
		/// <summary>データ属性を指定（Content-Disposition　※inline / attachment）AUの場合はattachment推奨</summary>
		public ContentDisposition FileContentDisposition { get; set; }

		/// <summary>
		/// 拡張子からMediaTypeを取得する
		/// </summary>
		/// <param name="strFilePath">デコメファイルパス</param>
		/// <returns>Content-Typeとして指定する際のMediaTypeを返却</returns>
		public string GetContentsType(string strFilePath)
		{
			switch (Path.GetExtension(strFilePath).ToLower())
			{
				// gif形式の場合
				case ".gif":
					return MediaTypeNames.Image.Gif;

				// jpeg/jpg形式の場合
				case ".jpeg":
				case ".jpg":
					return MediaTypeNames.Image.Jpeg;

				// png形式の場合
				case ".png":
					return "image/png";

				// その他の場合
				default:
					return MediaTypeNames.Application.Octet;
			}
		}

		///**************************************************************************************
		/// <summary>
		/// データ属性の列挙体
		/// デコメの場合にはInlineを利用する。（auもInlineで動作するが、可能であればAttachmentとする）
		/// </summary>
		///**************************************************************************************
		public enum ContentDisposition
		{
			/// <summary>Inline(デフォルト)</summary>
			Inline,
			/// <summary>Attachment(au推奨)</summary>
			Attachment
		}

	}
}
