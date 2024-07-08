/*
=========================================================================================================
  Module      : MIMEエンコーダー(MIMEEncoder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net.Mime;
using w2.Common.Extensions;

namespace w2.Common.Net
{
	/// <summary>
	/// MIMEエンコーダー
	/// </summary>
	public class MIMEEncoder
	{
		#region ASCII文字定義
		/// <summary>ASCII文字 "B"</summary>
		readonly static byte ASCII_U_B = 66;
		/// <summary>ASCII文字 "Q"</summary>
		readonly static byte ASCII_U_Q = 81;
		/// <summary>ASCII文字 EQUAL</summary>
		readonly static byte ASCII_EQUAL = 61;
		/// <summary>ASCII文字 QUESTION</summary>
		readonly static byte ASCII_QUESTION = 63;
		/// <summary>ASCII文字 CR</summary>
		readonly static byte ASCII_CR = 13;
		/// <summary>ASCII文字 LF</summary>
		readonly static byte ASCII_LF = 10;
		/// <summary>ASCII文字 SP</summary>
		readonly static byte ASCII_SP = 32;
		/// <summary>フォールディング文字列</summary>
		readonly static byte[] FOLDING_WHITE_SPACE = { ASCII_CR, ASCII_LF, ASCII_SP };
		#endregion

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="encoding">エンコーディング</param>
		/// <param name="transferEncoding">TransferEncoding</param>
		internal MIMEEncoder(Encoding encoding, TransferEncoding transferEncoding = TransferEncoding.Base64)
		{
			this.Encoding = encoding;
			this.TransferEncoding = transferEncoding;
		}

		/// <summary>
		/// エンコード
		/// </summary>
		/// <param name="value">値</param>
		/// <param name="prefix">接頭辞（「Subject: 」など）</param>
		/// <param name="suffix">接尾辞（「 &lt;bh@w2s.xyz&gt;」など）</param>
		/// <param name="maxLength">最大長</param>
		/// <returns>エンコード結果</returns>
		public string Encode(string value, string prefix = "", string suffix = "", int? maxLength = null)
		{
			byte[] bytesPrefix = Encoding.GetBytes(prefix);
			byte[] bytesSuffix = Encoding.GetBytes(suffix);

			StringBuilder buffer = new StringBuilder();
			using (MemoryStream stream = new MemoryStream())
			{
				stream.Write(bytesPrefix);

				Queue queue = new Queue(value.ToCharArray());
				while (queue.Count > 0)
				{
					char c = (char)queue.Dequeue();

					// 最大長を超えそうなら出力
					if (maxLength.HasValue
						&& (stream.GetLastLineLength() + ComputeEncodedBytes(this.Encoding.GetBytes(buffer.ToString() + c.ToString())) > maxLength.Value - 3))
					{
						if (buffer.Length > 0)
						{
							stream.Write(EncodeMIME(this.Encoding.GetBytes(buffer.ToString())));
							buffer.Clear();
						}
						stream.Write(FOLDING_WHITE_SPACE);
					}
					buffer.Append(c);
				}

				// footerが最終行に収まらないか判定
				bool addFoldingWhiteSpaceToLastLine = (maxLength.HasValue
					&& (ComputeEncodedBytes(this.Encoding.GetBytes(buffer.ToString())) + bytesSuffix.Length > maxLength.Value - 3));

				if (buffer.Length > 0) stream.Write(EncodeMIME(this.Encoding.GetBytes(buffer.ToString())));
				if (addFoldingWhiteSpaceToLastLine) stream.Write(FOLDING_WHITE_SPACE);
				stream.Write(bytesSuffix);

				return Encoding.ASCII.GetString(stream.ToArray());
			}
		}

		/// <summary>
		/// MIMEエンコード
		/// </summary>
		/// <param name="value">値</param>
		/// <returns>エンコード値</returns>
		private byte[] EncodeMIME(byte[] value)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				stream.WriteByte(ASCII_EQUAL);
				stream.WriteByte(ASCII_QUESTION);
				stream.Write(Encoding.ASCII.GetBytes(GetMIMEEncodeName(this.Encoding)));
				stream.WriteByte(ASCII_QUESTION);
				stream.WriteByte((this.TransferEncoding == TransferEncoding.Base64) ? ASCII_U_B : ASCII_U_Q);
				stream.WriteByte(ASCII_QUESTION);
				if (this.TransferEncoding == TransferEncoding.Base64)	// 現状base64のみ対応
				{
					stream.Write(Encoding.ASCII.GetBytes(Convert.ToBase64String(value)));
				}
				stream.WriteByte(ASCII_QUESTION);
				stream.WriteByte(ASCII_EQUAL);

				return stream.ToArray();
			}
		}

		/// <summary>
		/// MIMEエンコード名取得
		/// </summary>
		/// <param name="encoding">エンコーディング</param>
		/// <returns>MIMEエンコード名</returns>
		private string GetMIMEEncodeName(Encoding encoding)
		{
			switch (encoding.WebName.ToUpper())
			{
				case "SHIFT_JIS":
					return "Shift_JIS";

				default:
					return encoding.WebName.ToUpper();
			}
		}

		/// <summary>
		/// エンコードバイト数計算
		/// </summary>
		/// <param name="value">値</param>
		/// <returns>エンコード後のバイト数</returns>
		public int ComputeEncodedBytes(byte[] value)
		{
			return ComputeBase64EncodedBytes(value) + 7 + this.Encoding.WebName.Length;	// 「=?」「?B?」「?=」 で 合計7
		}

		/// <summary>
		/// base64エンコードバイト数計算
		/// </summary>
		/// <param name="value">値</param>
		/// <returns>エンコード後のバイト数</returns>
		private int ComputeBase64EncodedBytes(byte[] value)
		{
			if (value.Length % 3 > 0)
			{
				return (int)(Math.Ceiling((double)(value.Length / 3)) + 1.0) * 4;
			}
			return value.Length / 3 * 4;
		}

		/// <summary>エンコード</summary>
		internal Encoding Encoding { get; set; }
		/// <summary>TransferEncoding</summary>
		internal TransferEncoding TransferEncoding { get; set; }
	}
}
