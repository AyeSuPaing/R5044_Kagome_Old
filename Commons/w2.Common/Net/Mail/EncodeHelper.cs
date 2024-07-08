/*
=========================================================================================================
  Module      : エンコードヘルパ(EncodeHelper.ss)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Common.Net.Mail
{
	/// <summary>
	/// エンコードヘルパ
	/// </summary>
	public class EncodeHelper
	{
		#region DecodeQuotedPrintable Quoted-Printableデコード
		/// <summary>
		/// Quoted-Printableデコード
		/// </summary>
		/// <param name="enc">エンコーディング</param>
		/// <param name="src">ソース</param>
		/// <returns>デコードデータ</returns>
		public static string DecodeQuotedPrintable(Encoding enc, string src)
		{
			return enc.GetString(DecodeQuotedPrintableToBytes(enc, src));
		}
		#endregion

		#region +DecodeQuotedPrintableToBytes Quoted-Printableデコード（バイト配列へ）
		/// <summary>
		/// Quoted-Printableデコード（バイト配列へ）
		/// </summary>
		/// <param name="enc">エンコーディング</param>
		/// <param name="src">ソース</param>
		/// <returns>デコードデータ</returns>
		public static byte[] DecodeQuotedPrintableToBytes(Encoding enc, string src)
		{
			var bytes = new List<byte>();
			for (int index = 0; index < src.Length; index++)
			{
				switch (src.Substring(index, 1))
				{
					// =の次が改行でなければ16進の値セット
					case "=":
						if (index == src.Length - 1) break;	// = が 最後の文字の場合は（正常なのかは不明だが）セットしない

						string next = src.Substring(index + 1, 2);
						if (next != "\r\n")
						{
							bytes.Add(Convert.ToByte(src.Substring(index + 1, 2).Trim(), 16));
						}
						index += 2;
						break;

					// そのままセット
					default:
						bytes.Add(enc.GetBytes(src.ToCharArray(), index, 1)[0]);
						break;
				}
			}
			return bytes.ToArray();
		}
		#endregion

		#region +DecodeQEncode Qエンコード デコード
		/// <summary>
		/// Qエンコード デコード
		/// </summary>
		/// <param name="enc">エンコーディング</param>
		/// <param name="src">ソース</param>
		/// <returns>デコードデータ</returns>
		public static string DecodeQEncode(Encoding enc, string src)
		{
			return enc.GetString(DecodeQEncodeToBytes(enc, src));
		}
		#endregion

		#region +DecodeQEncodeToBytes Qエンコード（バイト配列へ）
		/// <summary>
		/// Qエンコード（バイト配列へ）
		/// </summary>
		/// <param name="enc">エンコーディング</param>
		/// <param name="src">ソース</param>
		/// <returns>デコードデータ</returns>
		public static byte[] DecodeQEncodeToBytes(Encoding enc, string src)
		{
			var bytes = new List<byte>();
			for (int index = 0; index < src.Length; index++)
			{
				switch (src.Substring(index, 1))
				{
					// =の次が改行でなければ16進の値セット
					case "=":
						string next = src.Substring(index + 1, 2);
						if (next != "\r\n")
						{
							bytes.Add(Convert.ToByte(src.Substring(index + 1, 2), 16));
						}
						index += 2;
						break;

					// 空白セット
					case "_":
						bytes.Add(0x20);
						break;

					// そのままセット
					default:
						bytes.Add(enc.GetBytes(src.ToCharArray(), index, 1)[0]);
						break;
				}
			}
			return bytes.ToArray();
		}
		#endregion

		#region +DecodeBase64 Base64デコード
		/// <summary>
		///  Base64デコード
		/// </summary>
		/// <param name="enc">エンコーディング</param>
		/// <param name="src">ソース</param>
		/// <returns>デコードデータ</returns>
		public static string DecodeBase64(Encoding enc, string src)
		{
			return enc.GetString(DecodeBase64ToBytes(src));
		}
		#endregion

		#region +DecodeBase64ToBytes Base64デコード（バイト配列へ）
		/// <summary>
		/// Base64デコード（バイト配列へ）
		/// </summary>
		/// <param name="src">ソース</param>
		/// <returns>デコードデータ</returns>
		public static byte[] DecodeBase64ToBytes(string src)
		{
			return Convert.FromBase64String(src);
		}
		#endregion
	}
}
