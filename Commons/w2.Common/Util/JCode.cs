/*
=========================================================================================================
  Module      : 日本語文字コードモジュール(JCode.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2014 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.Common.Util
{
	/// <summary>
	/// 日本語文字コードモジュール
	/// </summary>
	public class JCode
	{
		/// <summary>
		/// 文字コードを判別する
		/// </summary>
		/// <remarks>
		/// Jcode.pmのgetcodeメソッドを移植したものです。
		/// Jcode.pm(http://openlab.ring.gr.jp/Jcode/index-j.html)
		/// Jcode.pmのCopyright: Copyright 1999-2005 Dan Kogai
		/// </remarks>
		/// <param name="bytes">文字コードを調べるデータ</param>
		/// <returns>適当と思われるEncodingオブジェクト。
		/// 判断できなかった時はnull。</returns>
		public static Encoding GetEncoding(byte[] bytes)
		{
			const byte CODE_ESCAPE = 0x1B;
			const byte CODE_AT = 0x40;
			const byte CODE_DOLLAR = 0x24;
			const byte CODE_AND = 0x26;
			const byte CODE_OPEN = 0x28;    //'('
			const byte CODE_B = 0x42;
			const byte CODE_D = 0x44;
			const byte CODE_J = 0x4A;
			const byte CODE_I = 0x49;

			int len = bytes.Length;
			byte b1, b2, b3, b4;

			//Encode::is_utf8 は無視

			bool isBinary = false;
			for (var i = 0; i < len; i++)
			{
				b1 = bytes[i];
				if (b1 <= 0x06 || b1 == 0x7F || b1 == 0xFF)
				{
					//'binary'
					isBinary = true;
					if (b1 == 0x00 && i < len - 1 && bytes[i + 1] <= 0x7F)
					{
						//smells like raw unicode
						return Encoding.Unicode;
					}
				}
			}
			if (isBinary)
			{
				return null;
			}

			//not Japanese
			bool notJapanese = true;
			for (var i = 0; i < len; i++)
			{
				b1 = bytes[i];
				if (b1 == CODE_ESCAPE || 0x80 <= b1)
				{
					notJapanese = false;
					break;
				}
			}
			if (notJapanese)
			{
				return Encoding.ASCII;
			}

			for (int i = 0; i < len - 2; i++)
			{
				b1 = bytes[i];
				b2 = bytes[i + 1];
				b3 = bytes[i + 2];

				if (b1 == CODE_ESCAPE)
				{
					if (b2 == CODE_DOLLAR && b3 == CODE_AT)
					{
						//JIS_0208 1978
						//JIS
						return Encoding.GetEncoding(50220);
					}
					else if (b2 == CODE_DOLLAR && b3 == CODE_B)
					{
						//JIS_0208 1983
						//JIS
						return Encoding.GetEncoding(50220);
					}
					else if (b2 == CODE_OPEN && (b3 == CODE_B || b3 == CODE_J))
					{
						//JIS_ASC
						//JIS
						return Encoding.GetEncoding(50220);
					}
					else if (b2 == CODE_OPEN && b3 == CODE_I)
					{
						//JIS_KANA
						//JIS
						return Encoding.GetEncoding(50220);
					}
					if (i < len - 3)
					{
						b4 = bytes[i + 3];
						if (b2 == CODE_DOLLAR && b3 == CODE_OPEN && b4 == CODE_D)
						{
							//JIS_0212
							//JIS
							return Encoding.GetEncoding(50220);
						}
						if (i < len - 5 &&
							b2 == CODE_AND && b3 == CODE_AT && b4 == CODE_ESCAPE &&
							bytes[i + 4] == CODE_DOLLAR && bytes[i + 5] == CODE_B)
						{
							//JIS_0208 1990
							//JIS
							return Encoding.GetEncoding(50220);
						}
					}
				}
			}

			//should be euc|sjis|utf8
			//use of (?:) by Hiroki Ohzaki <ohzaki@iod.ricoh.co.jp>
			int sjis = 0;
			int euc = 0;
			int utf8 = 0;
			for (int i = 0; i < len - 1; i++)
			{
				b1 = bytes[i];
				b2 = bytes[i + 1];
				if (((0x81 <= b1 && b1 <= 0x9F) || (0xE0 <= b1 && b1 <= 0xFC)) &&
					((0x40 <= b2 && b2 <= 0x7E) || (0x80 <= b2 && b2 <= 0xFC)))
				{
					//SJIS_C
					sjis += 2;
					i++;
				}
			}
			for (int i = 0; i < len - 1; i++)
			{
				b1 = bytes[i];
				b2 = bytes[i + 1];
				if (((0xA1 <= b1 && b1 <= 0xFE) && (0xA1 <= b2 && b2 <= 0xFE)) ||
					(b1 == 0x8E && (0xA1 <= b2 && b2 <= 0xDF)))
				{
					//EUC_C
					//EUC_KANA
					euc += 2;
					i++;
				}
				else if (i < len - 2)
				{
					b3 = bytes[i + 2];
					if (b1 == 0x8F && (0xA1 <= b2 && b2 <= 0xFE) &&
						(0xA1 <= b3 && b3 <= 0xFE))
					{
						//EUC_0212
						euc += 3;
						i += 2;
					}
				}
			}
			for (int i = 0; i < len - 1; i++)
			{
				b1 = bytes[i];
				b2 = bytes[i + 1];
				if ((0xC0 <= b1 && b1 <= 0xDF) && (0x80 <= b2 && b2 <= 0xBF))
				{
					//UTF8
					utf8 += 2;
					i++;
				}
				else if (i < len - 2)
				{
					b3 = bytes[i + 2];
					if ((0xE0 <= b1 && b1 <= 0xEF) && (0x80 <= b2 && b2 <= 0xBF) &&
						(0x80 <= b3 && b3 <= 0xBF))
					{
						//UTF8
						utf8 += 3;
						i += 2;
					}
				}
			}

			if (euc > sjis && euc > utf8)
			{
				//EUC
				return Encoding.GetEncoding(51932);
			}
			else if (sjis > euc && sjis > utf8)
			{
				//SJIS
				return Encoding.GetEncoding(932);
			}
			else if (utf8 > euc && utf8 > sjis)
			{
				//UTF8
				return Encoding.UTF8;
			}

			return null;
		}
	}
}
