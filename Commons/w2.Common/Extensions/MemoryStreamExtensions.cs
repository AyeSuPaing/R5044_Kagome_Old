/*
=========================================================================================================
  Module      : MemoryStream拡張モジュール(MemoryStreamExtensions.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Text;

namespace w2.Common.Extensions
{
	/// <summary>
	/// MemoryStreamExtensionsクラスに拡張メソッドを追加する
	/// </summary>
	public static class MemoryStreamExtensions
	{
		/// <summary>ASCII文字 CR</summary>
		readonly static byte ASCII_CR = 13;
		/// <summary>ASCII文字 LF</summary>
		readonly static byte ASCII_LF = 10;

		/// <summary>
		/// バッファから読み取ったデータを使用して、現在のストリームにバイトのブロックを書き込みます。
		/// </summary>
		/// <param name="stream">本体</param>
		/// <param name="bytes">書き込みバッファ</param>
		public static void Write(this MemoryStream stream, byte[] buffer)
		{
			foreach (byte b in buffer)
			{
				stream.WriteByte(b);
			}
		}

		/// <summary>
		/// 最終行インデックス取得
		/// </summary>
		/// <param name="stream">本体</param>
		/// <returns>最終行インデックス</returns>
		public static long GetLastLineLength(this MemoryStream stream)
		{
			int num = 0;
			for (long l = stream.Position - 1; l > 0; l--)
			{
				byte[] buffer = stream.GetBuffer();
				byte byte1 = buffer[l];
				byte byte2 = buffer[l - 1];
				if ((byte1 == ASCII_LF) && (byte2 == ASCII_CR))
				{
					return num;
				}
				num++;
			}
			return stream.Position;
		}
	}
}
