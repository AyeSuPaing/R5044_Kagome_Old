/*
=========================================================================================================
  Module      : 入力チェックモジュール 単体エラーチェック部分(Validator_Checks.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using w2.Common.Sql;

namespace w2.Common.Util
{
	///**************************************************************************************
	/// <summary>
	/// さまざまな入力チェックを行う
	/// </summary>
	/// <remarks>
	/// インスタンス作成不要のため抽象クラスとして定義する
	/// </remarks>
	///**************************************************************************************
	public abstract partial class Validator
	{
		/// <summary>
		/// NULL・空文字判定
		/// </summary>
		/// <param name="objValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsNullEmpty(object objValue)
		{
			if (objValue == null)
			{
				return true;
			}

			try
			{
				if (objValue.ToString() == "")
				{
					return true;
				}
			}
			catch
			{
				// スルー
			}

			return false;
		}

		/// <summary>
		/// 必須エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckNecessaryError(string strName, string strValue)
		{
			if (IsNullEmpty(strValue))
			{
				return GetErrorMessage(CHECK_NECESSARY, strName);
			}

			return "";
		}

		/// <summary>
		/// 文字数エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="iLength">文字数</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckLengthError(string strName, string strValue, int iLength)
		{
			if (strValue.Length != iLength)
			{
				return GetErrorMessage(CHECK_LENGTH, strName, iLength.ToString());
			}

			return "";
		}

		/// <summary>
		/// 最大文字数エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="iMaxLength">最大文字数</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckLengthMaxError(string strName, string strValue, int iMaxLength)
		{
			if (strValue.Length > iMaxLength)
			{
				return GetErrorMessage(CHECK_LENGTH_MAX, strName, iMaxLength.ToString());
			}

			return "";
		}

		/// <summary>
		/// 最小文字数エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="iMinLength">最小文字数</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckLengthMinError(string strName, string strValue, int iMinLength)
		{
			if (strValue.Length < iMinLength)
			{
				return GetErrorMessage(CHECK_LENGTH_MIN, strName, iMinLength.ToString());
			}

			return "";
		}

		/// <summary>
		/// 文字バイト数(Shift_JIS)エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="iByteLength">文字バイト数</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckByteLengthError(string strName, string strValue, int iByteLength)
		{
			Encoding enc = Encoding.GetEncoding("Shift_JIS");

			if (enc.GetByteCount(strValue) != iByteLength)
			{
				return GetErrorMessage(CHECK_BYTE_LENGTH, strName, iByteLength.ToString(), (double.Parse(iByteLength.ToString()) / 2).ToString());
			}

			return "";
		}

		/// <summary>
		/// 最大文字バイト数(Shift_JIS)エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="iMaxByteLength">最大文字バイト数</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckByteLengthMaxError(string strName, string strValue, int iMaxByteLength)
		{
			Encoding enc = Encoding.GetEncoding("Shift_JIS");

			if (enc.GetByteCount(strValue) > iMaxByteLength)
			{
				return GetErrorMessage(CHECK_BYTE_LENGTH_MAX, strName, iMaxByteLength.ToString(), (double.Parse(iMaxByteLength.ToString()) / 2).ToString());
			}

			return "";
		}

		/// <summary>
		/// 最小文字バイト数(Shift_JIS)エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="iMinByteLength">最小文字バイト数</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckByteLengthMinError(string strName, string strValue, int iMinByteLength)
		{
			Encoding enc = Encoding.GetEncoding("Shift_JIS");

			if (enc.GetByteCount(strValue) < iMinByteLength)
			{
				return GetErrorMessage(CHECK_BYTE_LENGTH_MIN, strName, iMinByteLength.ToString(), (double.Parse(iMinByteLength.ToString()) / 2).ToString());
			}

			return "";
		}

		/// <summary>
		/// 最大数値エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="dMaxLength">最大数値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckNumberMaxError(string strName, string strValue, decimal dMaxLength)
		{
			// 文字種別はデフォルト「数値」とする
			return CheckNumberMaxError(strName, strValue, dMaxLength, STRTYPE_HALFWIDTH_DECIMAL);
		}
		/// <summary>
		/// 最大数値エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="dMaxLength">最大数値</param>
		/// <param name="strType">文字種別</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>
		/// 型チェックとともに利用する事が前提となります。
		/// （渡された値がdecimalにParseできない場合はエラーメッセージは返さない仕様となります。）
		/// </remarks>
		public static string CheckNumberMaxError(string strName, string strValue, decimal dMaxLength, string strType)
		{
			string strResult = "";

			try
			{
				if (strValue.Length != 0)	// 空文字の場合はチェック対象外
				{
					if (decimal.Parse(strValue) > dMaxLength)
					{
						return GetErrorMessage(CHECK_NUMBER_MAX, strName, ((strType == STRTYPE_HALFWIDTH_DECIMAL || (strType == STRTYPE_CURRENCY)) ? StringUtility.ToNumeric(dMaxLength) : dMaxLength.ToString()));
					}
				}
			}
			catch (Exception)
			{
				// なにもしない
			}

			return strResult;
		}

		/// <summary>
		/// 最小数値エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="dMinLength">最小数値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckNumberMinError(string strName, string strValue, decimal dMinLength)
		{
			// 文字種別はデフォルト「数値」とする
			return CheckNumberMinError(strName, strValue, dMinLength, STRTYPE_HALFWIDTH_DECIMAL);
		}
		/// <summary>
		/// 最小数値エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="dMinLength">最小数値</param>
		/// <param name="strType">文字種別</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>
		/// 型チェックとともに利用する事が前提となります。
		/// （渡された値がdecimalにParseできない場合はエラーメッセージは返さない仕様となります。）
		/// </remarks>
		public static string CheckNumberMinError(string strName, string strValue, decimal dMinLength, string strType)
		{
			try
			{
				if (strValue.Length != 0)	// 空文字の場合はチェック対象外
				{
					if (decimal.Parse(strValue) < dMinLength)
					{
						return GetErrorMessage(CHECK_NUMBER_MIN, strName, ((strType == STRTYPE_HALFWIDTH_DECIMAL || (strType == STRTYPE_CURRENCY)) ? StringUtility.ToNumeric(dMinLength) : dMinLength.ToString()));
					}
				}
			}
			catch (Exception)
			{
				// なにもしない
			}

			return "";
		}

		/// <summary>
		/// 上限額エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="dMaxLength">最大数値</param>
		/// <param name="strType">文字種別</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>
		/// 型チェックとともに利用する事が前提となります。
		/// （渡された値がdecimalにParseできない場合はエラーメッセージは返さない仕様となります。）
		/// </remarks>
		public static string CheckPriceMaxError(string strName, string strValue, decimal dMaxLength, string strType)
		{
			string strResult = "";

			try
			{
				if (strValue.Length != 0)	// 空文字の場合はチェック対象外
				{
					if (decimal.Parse(strValue) > dMaxLength)
					{
						return GetErrorMessage(CHECK_PRICE_MAX, strName, String.Format("{0:#,0}", dMaxLength));
					}
				}
			}
			catch (Exception)
			{
				// なにもしない
			}

			return strResult;
		}

		/// <summary>
		/// 全角文字列判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsZenkaku(string strValue)
		{
			try
			{
				if (strValue.Length != 0)	// 空文字の場合はチェック対象外
				{
					foreach (char chChar in strValue)
					{
						// SJISでのバイト数が文字数の2倍でなければNGと判定
						byte[] bChar = Encoding.GetEncoding("Shift_JIS").GetBytes(chChar.ToString());
						if (bChar.Length != 2)
						{
							return false;
						}

						int iChar = (bChar[0] << 8) + bChar[1];	// char型に格納するとunicodeになるためint型とする

						// 各種記号、英数字、かなの場合はOK（次の文字へ）
						if ((0x8140 <= iChar) && (iChar <= 0x84BE))
						{
							continue;
						}
						// NEC拡張外字の場合はOK（次の文字へ）
						if ((0x8740 <= iChar) && (iChar <= 0x879C))
						{
							continue;
						}
						// 第一水準漢字の場合はOK（次の文字へ）
						if ((0x889F <= iChar) && (iChar <= 0x9872))
						{
							continue;
						}
						// 第二水準漢字の場合もOK（次の文字へ）
						if (((0x989F <= iChar) && (iChar <= 0x9FFC))
							|| ((0xE040 <= iChar) && (iChar <= 0xEAA4)))
						{
							continue;
						}
						// IBM拡張文字(115区から119区)の場合もOK(次の文字へ)
						if (((0xFA40 <= iChar) && (iChar <= 0xFA9E)) // 115区
							|| ((0xFA9F <= iChar) && (iChar <= 0xFAFC)) // 116区
							|| ((0xFB40 <= iChar) && (iChar <= 0xFB9E)) // 117区
							|| ((0xFB9F <= iChar) && (iChar <= 0xFBFC)) // 118区
							|| ((0xFC40 <= iChar) && (iChar <= 0xFC4B))) // 119区
						{
							continue;
						}
						// NEC選定IBM拡張文字(89区から92区)の場合もOK（次の文字へ）
						if (((0xED40 <= iChar) && (iChar <= 0xED9E)) // 89区
							|| ((0xED9F <= iChar) && (iChar <= 0xEDFC)) // 90区
							|| ((0xEE40 <= iChar) && (iChar <= 0xEE9E)) // 91区
							|| ((0xEE9F <= iChar) && (iChar <= 0xEEFC))) // 92区
						{
							continue;
						}

						return false;	// 上記チェックでOKが出なかった場合はNG
					}
				}
			}
			catch (Exception)
			{
				// なにもしない
			}

			return true;
		}

		/// <summary>
		/// 全角エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckFullwidthError(string strName, string strValue)
		{
			if ((IsZenkaku(strValue) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_FULLWIDTH, strName);
			}

			return "";
		}

		/// <summary>
		/// 全角ひらがな判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsZenkakuHiragana(string strValue)
		{
			try
			{
				if (strValue.Length != 0)	// 空文字の場合はfalse
				{
					//      JIS  SJIS EUC   +0 +1 +2 +3 +4 +5 +6 +7 +8 +9 +A +B +C +D +E +F
					//04区  2420 829E A4A0     ぁ あ ぃ い ぅ う ぇ え ぉ お か が き ぎ く
					//04区  2430 82AE A4B0  ぐ け げ こ ご さ ざ し じ す ず せ ぜ そ ぞ た
					//04区  2440 82BE A4C0  だ ち ぢ っ つ づ て で と ど な に ぬ ね の は
					//04区  2450 82CE A4D0  ば ぱ ひ び ぴ ふ ぶ ぷ へ べ ぺ ほ ぼ ぽ ま み
					//04区  2460 82DE A4E0  む め も ゃ や ゅ ゆ ょ よ ら り る れ ろ ゎ わ
					//04区  2470 82EE A4F0  ゐ ゑ を ん ・ ・ ・ ・ ・ ・ ・ ・ ・ ・ ・   

					Regex objRegex = new Regex("^[ぁ-んー]+$");

					if (objRegex.IsMatch(strValue))
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
				// なにもしない
			}

			return false;
		}

		/// <summary>
		/// 全角ひらがなエラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckFullwidthHiraganaError(string strName, string strValue)
		{
			if ((IsZenkakuHiragana(strValue) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_FULLWIDTH_HIRAGANA, strName);
			}

			return "";
		}

		/// <summary>
		/// 全角カタカナ判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsZenkakuKatakana(string strValue)
		{
			try
			{
				if (strValue.Length != 0)	// 空文字の場合はfalse
				{
					//      JIS  SJIS EUC   +0 +1 +2 +3 +4 +5 +6 +7 +8 +9 +A +B +C +D +E +F
					//05区  2520 833F A5A0     ァ ア ィ イ ゥ ウ ェ エ ォ オ カ ガ キ ギ ク
					//05区  2530 834F A5B0  グ ケ ゲ コ ゴ サ ザ シ ジ ス ズ セ ゼ ソ ゾ タ
					//05区  2540 835F A5C0  ダ チ ヂ ッ ツ ヅ テ デ ト ド ナ ニ ヌ ネ ノ ハ
					//05区  2550 836F A5D0  バ パ ヒ ビ ピ フ ブ プ ヘ ベ ペ ホ ボ ポ マ ミ
					//05区  2560 8380 A5E0  ム メ モ ャ ヤ ュ ユ ョ ヨ ラ リ ル レ ロ ヮ ワ
					//05区  2570 8390 A5F0  ヰ ヱ ヲ ン ヴ ヵ ヶ ・ ・ ・ ・ ・ ・ ・ ・   

					Regex objRegex = new Regex(@"^[ァ-ヶー]+\z");	// 改行コード排除したいので「$」でなく「\z」を利用

					if (objRegex.IsMatch(strValue))
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
				// なにもしない
			}

			return false;
		}

		/// <summary>
		/// 全角カタカナエラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckFullwidthKatakanaError(string strName, string strValue)
		{
			if ((IsZenkakuKatakana(strValue) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_FULLWIDTH_KATAKANA, strName);
			}

			return "";
		}

		/// <summary>
		/// 半角判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsHalfwidth(string strValue)
		{
			try
			{
				if (strValue.Length != 0)	// 空文字の場合はfalse
				{
					// SJISでのバイト数
					int intSjisByte = Encoding.GetEncoding("Shift_JIS").GetByteCount(strValue);

					if (strValue.Length == intSjisByte)
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
				// なにもしない
			}

			return false;
		}

		/// <summary>
		/// 半角エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckHalfwidthError(string strName, string strValue)
		{
			if ((IsHalfwidth(strValue) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_HALFWIDTH, strName);
			}

			return "";
		}

		/// <summary>
		/// 半角英数判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsHalfwidthAlphNum(string strValue)
		{
			try
			{
				if (strValue.Length != 0)	// 空文字の場合はfalse
				{
					Regex objRegex = new Regex(@"^[0-9a-zA-Z]*\z");	// 改行コード排除したいので「$」でなく「\z」を利用

					if (objRegex.IsMatch(strValue))
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
				// なにもしない
			}

			return false;
		}

		/// <summary>
		/// 半角英数エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckHalfwidthAlphNumError(string strName, string strValue)
		{
			if ((IsHalfwidthAlphNum(strValue) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_HALFWIDTH_ALPHNUM, strName);
			}

			return "";
		}

		/// <summary>
		/// 半角英数記号判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsHalfwidthAlphNumSymbol(string strValue)
		{
			try
			{
				if (strValue.Length != 0)	// 空文字の場合はfalse
				{
					int intUTF8Byte = Encoding.GetEncoding("utf-8").GetByteCount(strValue);

					if (strValue.Length == intUTF8Byte)
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
				// なにもしない
			}

			return false;
		}

		/// <summary>
		/// 半角英数記号エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckHalfwidthAlphNumSymbolError(string strName, string strValue)
		{
			if ((IsHalfwidthAlphNumSymbol(strValue) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_HALFWIDTH_ALPHNUMSYMBOL, strName);
			}

			return "";
		}

		/// <summary>
		/// 半角数字（正数のみ）判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsHalfwidthNumber(string strValue)
		{
			try
			{
				if (strValue.Length != 0)	// 空文字の場合はfalse
				{
					Regex objRegex = new Regex(@"^[0-9]+\z");	// 改行コード排除したいので「$」でなく「\z」を利用

					if (objRegex.IsMatch(strValue))
					{
						return true;
					}
				}
			}
			catch (Exception)
			{
				// なにもしない
			}

			return false;
		}

		/// <summary>
		/// 半角数字（正数のみ）エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckHalfwidthNumberError(string strName, string strValue)
		{
			if ((IsHalfwidthNumber(strValue) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_HALFWIDTH_NUMBER, strName);
			}

			return "";
		}

		/// <summary>
		/// 半角数字（10進数）判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsHalfwidthDecimal(string strValue)
		{
			try
			{
				if (strValue.Length != 0)	// 空文字の場合はfalse
				{
					decimal dBuff = decimal.Parse(strValue);
				}

				// 例外が発生していなければtrue;
				return true;
			}
			catch (Exception)
			{
				// なにもしない
			}

			return false;
		}

		/// <summary>
		/// 半角数字（10進数）エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckHalfwidthDecimalError(string strName, string strValue)
		{
			if ((IsHalfwidthDecimal(strValue) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_HALFWIDTH_DECIMAL, strName);
			}

			return "";
		}

		/// <summary>
		/// 半角日付判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>判定結果</returns>
		public static bool IsHalfwidthDate(string strValue, string languageLocaleId = "")
		{
			var result = (IsDate(strValue, languageLocaleId) && IsHalfwidth(strValue));
			return result;
		}

		/// <summary>
		/// 半角日付エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckHalfwidthDateError(string strName, string strValue, string languageLocaleId = "")
		{
			if ((IsHalfwidthDate(strValue, languageLocaleId) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_HALFWIDTH_DATE, strName);
			}

			return "";
		}

		/// <summary>
		/// 日付型チェック
		/// </summary>
		/// <param name="value">値</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>エラー判定</returns>
		public static bool IsDate(object value, string languageLocaleId = "")
		{
			DateTime date;
			var result = DateTime.TryParse(
				StringUtility.ToEmpty(value),
				string.IsNullOrEmpty(languageLocaleId) ? null : new CultureInfo(languageLocaleId),
				DateTimeStyles.None,
				out date);
			return result;
		}

		/// <summary>
		/// 日付エラーチェック
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="value">値</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckDateError(string name, string value, string languageLocaleId = "")
		{
			if ((IsDate(value, languageLocaleId) == false) && (StringUtility.ToEmpty(value).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_DATE, name);
			}
			return "";
		}

		/// <summary>
		/// 時間エラーチェック
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="value">値</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckTimeError(string name, string value, string languageLocaleId = "")
		{
			var regex = new Regex(@"[/]+");
			if (regex.IsMatch(value)) return GetErrorMessage(CHECK_STRTYPE_DROPDOWN_TIME, name);

			if ((IsDate(value, languageLocaleId) == false) && (StringUtility.ToEmpty(value).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_DROPDOWN_TIME, name);
			}
			return "";
		}

		/// <summary>
		/// 日付型チェック
		/// </summary>
		/// <param name="objValue">値</param>
		/// <param name="strFormat">日付フォーマット</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>エラー判定</returns>
		public static bool IsDateExact(object objValue, string strFormat, string languageLocaleId = "")
		{
			DateTime dtTmp;
			var result = DateTime.TryParseExact(
				StringUtility.ToEmpty(objValue),
				strFormat,
				string.IsNullOrEmpty(languageLocaleId) ? null : new CultureInfo(languageLocaleId),
				DateTimeStyles.None,
				out dtTmp);
			return result;
		}

		/// <summary>
		/// 日付エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="strFormat">日付フォーマット</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckDateExactError(string strName, string strValue, string strFormat, string languageLocaleId = "")
		{
			if ((IsDateExact(strValue, strFormat, languageLocaleId) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_DATE, strName);
			}
			return "";
		}

		/// <summary>
		/// 未来日付判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>判定結果</returns>
		public static bool IsDateFuture(string strValue, string languageLocaleId = "")
		{
			// 未来判定
			DateTime date;
			var result = (DateTime.TryParse(
				strValue,
				string.IsNullOrEmpty(languageLocaleId) ? null : new CultureInfo(languageLocaleId),
				DateTimeStyles.None,
				out date) && (date > DateTime.Now.Date));
			return result;
		}

		/// <summary>
		/// 未来日付エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckDateFutureError(string strName, string strValue, string languageLocaleId = "")
		{
			if ((IsDateFuture(strValue, languageLocaleId) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_DATE_FUTURE, strName);
			}
			return "";
		}

		/// <summary>
		/// 過去日付判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>判定結果</returns>
		public static bool IsDatePast(string strValue, string languageLocaleId = "")
		{
			// 過去判定
			DateTime date;
			var result = (DateTime.TryParse(
				strValue,
				string.IsNullOrEmpty(languageLocaleId) ? null : new CultureInfo(languageLocaleId),
				DateTimeStyles.None,
				out date) && (date < DateTime.Now.Date));
			return result;
		}

		/// <summary>
		/// 過去日付エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckDatePastError(string strName, string strValue, string languageLocaleId = "")
		{
			if ((IsDatePast(strValue, languageLocaleId) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_DATE_PAST, strName);
			}
			return "";
		}

		/// <summary>
		/// メールアドレス判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsMailAddress(string strValue)
		{
			if (strValue.Length != 0)	// 空文字の場合はチェック対象外
			{
				//※KDDIでは先頭に「-」が許される
				//※旧Vodafoneでは「?」「/」「+」の記号も許される
				//※旧DoCoMoでは連続した「.」が許される
				//※一文字ドメインOK
				//※．(ドット)の連続は許されない

				//Regex objRegex = new Regex("^[0-9a-zA-Z][0-9a-zA-Z_\\.\\-]+@[0-9a-zA-Z][0-9a-zA-Z_\\.\\-]+\\.[0-9a-zA-Z]+$");
				//Regex objRegex = new Regex(@"^[0-9a-zA-Z\-_\?\/\+][0-9a-zA-Z~_\.\-\?\/\+]*@[0-9a-zA-Z][0-9a-zA-Z\.\-]+\.[0-9a-zA-Z]+\z");		// 改行コード排除したいので「$」でなく「\z」を利用
				//Regex objRegex = new Regex(@"^[0-9a-zA-Z\-_\?\/\+][0-9a-zA-Z~_\.\-\?\/\+]*@[0-9a-zA-Z][0-9a-zA-Z\-]*([\.][0-9a-zA-Z\-]+)*\.[0-9a-zA-Z]+\z");		// 連続した「.」をチェックする
				//Regex objRegex = new Regex(@"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$");
				Regex objRegex = new Regex(@"^[0-9a-zA-Z\!\#\$\%\&\'\*\+\-\/\=\?\^_\`\{\|\}\~][0-9a-zA-Z\.\!\#\$\%\&\'\*\+\-\/\=\?\^_\`\{\|\}~]*@[0-9a-zA-Z][0-9a-zA-Z\-]*([\.][0-9a-zA-Z\-]+)*\.[0-9a-zA-Z]+\z");

				if (objRegex.IsMatch(strValue))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// メールアドレスエラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckMailAddressError(string strName, string strValue)
		{
			if ((IsMailAddress(strValue) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_MAILADDRESS, strName);
			}

			return "";
		}

		/// <summary>
		/// 都道府県判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsPrefecture(string strValue)
		{
			if (strValue.Length != 0)	// 空文字の場合はチェック対象外
			{
				foreach (string strPrefecture in Constants.STR_PREFECTURES_LIST)
				{
					if (strPrefecture == strValue)
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// 都道府県エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckPrefectureError(string strName, string strValue)
		{
			if ((IsPrefecture(strValue) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_PREFECTURE, strName);
			}

			return "";
		}

		/// <summary>
		/// 郵便番号エラーチェック
		/// </summary>
		/// <param name="zipCodeFront">郵便番号 前</param>
		/// <param name="zipCodeBack">郵便番号 後ろ</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckZipCode(string zipCodeFront, string zipCodeBack)
		{
			var errorMessage = CheckZipCode(StringUtility.ToHankaku(zipCodeFront + zipCodeBack));
			return errorMessage;
		}
		/// <summary>
		/// 郵便番号エラーチェック
		/// </summary>
		/// <param name="zipCode">郵便番号</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckZipCode(string zipCode)
		{
			string errorMessages = null;

			if (zipCode.Contains("-")
				|| zipCode.Contains("ｰ")
				|| (zipCode.Length > 7))
			{
				errorMessages = Validator.CheckRegExpError(
					"郵便番号",
					zipCode,
					"(^[0-9]{3}(-|ｰ)[0-9]{4}$)|^([0-9]{7}$)",
					string.Empty);
				return errorMessages;
			}

			errorMessages = Validator.CheckNecessaryError("郵便番号", zipCode)
				+ Validator.CheckHalfwidthNumberError("郵便番号", zipCode)
				+ Validator.CheckLengthError("郵便番号", zipCode, 7);
			return errorMessages;
		}

		/// <summary>
		/// 税率判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsTaxRate(string strValue)
		{
			if (strValue.Length == 0) return false;	// 空文字の場合はチェック対象外

			var regex = new Regex(@"^([0-9]{1,3}|([0-9]{1,3}\.[0-9]{1,2}))$");		// 整数最大桁3、少数最大桁2をチェックする
			var result = regex.IsMatch(strValue);
			return result;
		}

		/// <summary>
		/// 税率エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckTaxRateError(string strName, string strValue)
		{
			if ((IsTaxRate(strValue) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_TAX_RATE, strName);
			}

			return "";
		}

		/// <summary>
		/// ShiftJis変換エラーチェック
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="value">値</param>
		/// <param name="addRegexPattern">追加で除外する正規表現パターン(ある場合のみ設定)</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckShiftJisConvertError(string name, string value, string addRegexPattern)
		{
			string errorStr;
			if ((IsEnableConvertShiftJis(value, addRegexPattern, out errorStr) == false) && (StringUtility.ToEmpty(value).Length != 0))
			{
				return GetErrorMessage(CHECK_PROHIBITED_CHAR, name, errorStr);
			}

			return "";
		}

		/// <summary>
		/// 禁止文字列エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="chProhibitedChars">禁止文字</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckProhibitedCharError(string strName, string strValue, char[] chProhibitedChars)
		{
			foreach (char ch in chProhibitedChars)
			{
				if (strValue.IndexOf(ch) != -1)
				{
					return GetErrorMessage(CHECK_PROHIBITED_CHAR, strName, ch.ToString());
				}
			}

			return "";
		}

		/// <summary>
		/// 除外文字コード外判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <param name="enc">エンコード</param>
		/// <param name="uiExceptCodeBgn">除外コード開始</param>
		/// <param name="uiExceptCodeEnd">除外コード終了</param>
		/// <returns>判定結果</returns>
		public static bool IsOutOfCharCode(string strValue, Encoding enc, uint uiExceptCodeBgn, uint uiExceptCodeEnd)
		{
			char chTmp;
			return IsOutOfCharCode(strValue, enc, uiExceptCodeBgn, uiExceptCodeEnd, out chTmp);
		}
		/// <summary>
		/// 除外文字コード外判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <param name="enc">エンコード</param>
		/// <param name="uiExceptCodeBgn">除外コード開始</param>
		/// <param name="uiExceptCodeEnd">除外コード終了</param>
		/// <param name="chErrorChar">エラー文字</param>
		/// <returns>判定結果</returns>
		public static bool IsOutOfCharCode(string strValue, Encoding enc, uint uiExceptCodeBgn, uint uiExceptCodeEnd, out char chErrorChar)
		{
			// TODO: 外出ししたいなー
			if (strValue.Length != 0)	// 空文字の場合はチェック対象外
			{
				foreach (char ch in strValue)
				{
					uint uiChar = CharToUint(enc, ch);
					// 範囲内であればアウト
					if ((uiExceptCodeBgn <= uiChar) && (uiChar <= uiExceptCodeEnd))
					{
						chErrorChar = ch;
						return false;
					}
				}
			}

			chErrorChar = ' ';	// ダミー
			return true;
		}

		/// <summary>
		/// 文字からUintへの変換
		/// </summary>
		/// <param name="enc">エンコード</param>
		/// <param name="ch">文字</param>
		/// <returns>文字コード</returns>
		private static uint CharToUint(Encoding enc, char ch)
		{
			byte[] bytes = enc.GetBytes(ch.ToString());
			uint uiChar = 0;
			if ((enc.CodePage == 50220) && (bytes.Length == 8))	//  "ISO-2022-JP"
			{
				uiChar = ((0xffffffff & bytes[3]) << 8) + (0xffffffff & bytes[4]);
			}
			else if (bytes.Length == 1)
			{
				uiChar = 0xffffffff & bytes[0];
			}
			else
			{
				uiChar = ((0xffffffff & bytes[0]) << 8) + (0xffffffff & bytes[1]);
			}

			return uiChar;
		}

		/// <summary>
		/// SiftJis文字への変換ができるかチェック
		/// </summary>
		/// <param name="value">値</param>
		/// <param name="addRegexPattern">追加で除外する正規表現パターン(ある場合のみ設定)</param>
		/// <param name="errorStr">エラー時の文字</param>
		/// <returns>変換できるかどうか</returns>
		public static bool IsEnableConvertShiftJis(string value, string addRegexPattern, out string errorStr)
		{
			// 空文字の場合はチェック対象外
			errorStr = " ";	// ダミー
			if (value.Length == 0) return true;

			var enumerator = StringInfo.GetTextElementEnumerator(value);
			while (enumerator.MoveNext())
			{
				var currentStr = enumerator.GetTextElement();

				if ((currentStr.Length == 2) && (Char.IsSurrogatePair(currentStr, 0) == false))
				{
					// Lengthが2でサロゲートペアでない場合は結合文字
					errorStr = currentStr;
					return false;
				}

				uint uiChar = CharToUint(Encoding.GetEncoding("Shift_JIS"), currentStr[0]);

				//変換できない場合は「?(0x3F)」なのでエラー、もともと「?」は除外
				if ((uiChar == 0x3F) && (currentStr != "?"))
				{
					errorStr = currentStr;
					return false;
				}

				//変換できる文字コード範囲でない場合はエラー
				if (IsEnableConvertShiftJis(uiChar) == false)
				{
					errorStr = currentStr;
					return false;
				}

				//追加で除外する条件がある場合
				if (string.IsNullOrEmpty(addRegexPattern) == false)
				{
					if (Regex.IsMatch(string.Format("0x{0:X}", uiChar), addRegexPattern))
					{
						errorStr = currentStr;
						return false;
					}
				}
			}

			return true;
		}
		/// <summary>
		/// シフトJISへ変換可能な文字か
		/// </summary>
		/// <param name="shiftJisStringCode">シフトJIS文字コード</param>
		/// <returns></returns>
		public static bool IsEnableConvertShiftJis(uint shiftJisStringCode)
		{
			if (((0x00 <= shiftJisStringCode) && (shiftJisStringCode <= 0x1F) || (shiftJisStringCode == 0x7F)) //制御コード
			|| ((0x20 <= shiftJisStringCode) && (shiftJisStringCode <= 0x7e))	//ASCII文字
			|| ((0xa1 <= shiftJisStringCode) && (shiftJisStringCode <= 0xdf))	//半角カタカナ
			|| ((0x8140 <= shiftJisStringCode) && (shiftJisStringCode <= 0x84BE))	//非漢字
			|| ((0x889F <= shiftJisStringCode) && (shiftJisStringCode <= 0x9872))	//第一水準
			|| ((0x989F <= shiftJisStringCode) && (shiftJisStringCode <= 0x9FFC))	//第二水準
			|| ((0xE040 <= shiftJisStringCode) && (shiftJisStringCode <= 0xEAA4))	//第二水準
			|| ((0x8740 <= shiftJisStringCode) && (shiftJisStringCode <= 0x879C))	//NEC特殊文字
			|| ((0xED40 <= shiftJisStringCode) && (shiftJisStringCode <= 0xEEFC))	//NEC選定IBM拡張文字
			|| ((0xFA40 <= shiftJisStringCode) && (shiftJisStringCode <= 0xFC4B))	//IBM拡張文字
			|| ((0x00FD <= shiftJisStringCode) && (shiftJisStringCode <= 0x00FF))	//Apple拡張文字
			|| ((0x8540 <= shiftJisStringCode) && (shiftJisStringCode <= 0x886D))	//Apple拡張文字
			|| ((0xEB40 <= shiftJisStringCode) && (shiftJisStringCode <= 0xED96))	//Apple拡張文字
			)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// 除外文字コード(ISO-2022-JP)エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strTrueValueName">正しい文字の名称</param>
		/// <param name="strValue">値</param>
		/// <param name="uiExceptCodeBgn">除外コード開始</param>
		/// <param name="uiExceptCodeEnd">除外コード終了</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckOutOfISO2202JPCode(string strName, string strTrueValueName, string strValue, uint uiExceptCodeBgn, uint uiExceptCodeEnd)
		{
			char chErrorChar;
			if ((IsOutOfCharCode(strValue, Encoding.GetEncoding("ISO-2022-JP"), uiExceptCodeBgn, uiExceptCodeEnd, out chErrorChar) == false)
				&& (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_OUTOFSHIFTJISCODE, strName, strTrueValueName, chErrorChar.ToString());
			}

			return "";
		}

		/// <summary>
		/// 除外文字コード(Shift_JIS)エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strTrueValueName">正しい文字の名称</param>
		/// <param name="strValue">値</param>
		/// <param name="uiExceptCodeBgn">除外コード開始</param>
		/// <param name="uiExceptCodeEnd">除外コード終了</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckOutOfShiftJISCode(string strName, string strTrueValueName, string strValue, uint uiExceptCodeBgn, uint uiExceptCodeEnd)
		{
			char chErrorChar;
			if ((IsOutOfCharCode(strValue, Encoding.GetEncoding("Shift_JIS"), uiExceptCodeBgn, uiExceptCodeEnd, out chErrorChar) == false)
				&& (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_OUTOFSHIFTJISCODE, strName, strTrueValueName, chErrorChar.ToString());
			}

			return "";
		}

		/// <summary>
		/// 正規表現マッチング判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <param name="strPattern">パターン</param>
		/// <returns>判定結果</returns>
		public static bool IsRegExpMatch(string strValue, string strPattern)
		{
			if (strValue.Length != 0)	// 空文字の場合はチェック対象外
			{
				Regex objRegex = new Regex(strPattern);

				if (objRegex.IsMatch(strValue))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// 正規表現マッチングエラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="strPattern">パターン</param>
		/// <param name="strPatternMessage">エラーメッセージ用パターン名称</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>マッチングしない場合エラーを返す</remarks>
		public static string CheckRegExpError(string strName, string strValue, string strPattern, string strPatternMessage)
		{
			if ((IsRegExpMatch(strValue, strPattern) == false) && (StringUtility.ToEmpty(strValue).Length != 0))
			{
				return GetErrorMessage(CHECK_REGEXP, strName, strPatternMessage);
			}

			return "";
		}

		/// <summary>
		/// 正規表現（除外）マッチングエラーチェック
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="value">値</param>
		/// <param name="pattern">パターン</param>
		/// <param name="patternMessage">エラーメッセージ用パターン名称</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>マッチングした場合エラーを返す</remarks>
		public static string CheckExceptRegExpError(string name, string value, string pattern, string patternMessage)
		{
			if ((IsRegExpMatch(value, pattern)) && (StringUtility.ToEmpty(value).Length != 0))
			{
				return GetErrorMessage(CHECK_EXCEPT_REGEXP, name, patternMessage);
			}

			return "";
		}

		/// <summary>
		/// 確認用入力エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strValue">値</param>
		/// <param name="strConfValue">確認用値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckConfirmError(string strName, string strValue, string strConfValue)
		{
			if (strValue.Equals(strConfValue) == false)
			{
				return GetErrorMessage(CHECK_CONFIRM, strName);
			}

			return "";
		}

		/// <summary>
		/// 同値判定
		/// </summary>
		/// <param name="strValue">値</param>
		/// <param name="strCheckValue">チェック値</param>
		/// <returns>判定結果</returns>
		public static bool IsEquivalence(string strValue, string strCheckValue)
		{
			return (strValue == strCheckValue);
		}

		/// <summary>
		/// 同値エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strEquivalenceName">同値名称</param>
		/// <param name="strValue">値</param>
		/// <param name="strCheckValue">チェック値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckEquivalence(string strName, string strEquivalenceName, string strValue, string strCheckValue)
		{
			if (IsEquivalence(strValue, strCheckValue))
			{
				return GetErrorMessage(CHECK_EQUIVALENCE, strName, strEquivalenceName);
			}

			return "";
		}

		/// <summary>
		/// 異値エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strDifferentValueName">異値名称</param>
		/// <param name="strValue">値</param>
		/// <param name="strCheckValue">チェック値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckDifferentValue(string strName, string strDifferentValueName, string strValue, string strCheckValue)
		{
			if (IsEquivalence(strValue, strCheckValue) == false)
			{
				return GetErrorMessage(CHECK_DIFFERENT_VALUE, strName, strDifferentValueName);
			}

			return "";
		}

		/// <summary>
		/// ＤＢ重複判定
		/// </summary>
		/// <param name="strFileName">ステートメントファイル名</param>
		/// <param name="strStatementName">ステートメント名</param>
		/// <param name="dicParam">チェックパラメタ</param>
		/// <param name="strValue">値</param>
		/// <returns>判定結果</returns>
		public static bool IsDbDuplicated(string strFileName, string strStatementName, IDictionary dicParam, string strValue)
		{
			// ステートメント取得（usingを用いてdisposeを自動的に呼び出す）
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement(strFileName, strStatementName))
			{
				// 値を置換
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ value @@", strValue);

				// ステートメント実行
				System.Data.DataView dv = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, dicParam);

				if (dv.Count != 0)
				{
					if (dv[0].Row[0].ToString() != "0")
					{
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// ＤＢ重複エラーチェック
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strFileName">ステートメントファイル名</param>
		/// <param name="strStatementName">ステートメント名</param>
		/// <param name="dicParam">チェックパラメタ</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckDbDuplication(string strName, string strFileName, string strStatementName, IDictionary dicParam, string strValue)
		{
			if (IsDbDuplicated(strFileName, strStatementName, dicParam, strValue))
			{
				return GetErrorMessage(CHECK_DUPLICATION, strName);
			}

			return "";
		}

		/// <summary>
		/// ＤＢ重複エラーチェック（同一期間内の同一項目重複チェック）
		/// </summary>
		/// <param name="strName">名称</param>
		/// <param name="strFileName">ステートメントファイル名</param>
		/// <param name="strStatementName">ステートメント名</param>
		/// <param name="dicParam">チェックパラメタ</param>
		/// <param name="strValue">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckDbDuplicationDateRange(string strName, string strFileName, string strStatementName, IDictionary dicParam, string strValue)
		{
			if (IsDbDuplicated(strFileName, strStatementName, dicParam, strValue))
			{
				return GetErrorMessage(CHECK_DUPLICATION_DATERANGE, strName);
			}

			return "";
		}

		/// <summary>
		/// エラーメッセージ取得モジュール
		/// </summary>
		/// <param name="strInputCheckType">入力チェックタイプ</param>
		/// <param name="strParam1">エラーメッセージ用パラメータ</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetErrorMessage(string strInputCheckType, string strParam1)
		{
			return GetErrorMessage(strInputCheckType, strParam1, "", "", "");
		}
		/// <summary>
		/// エラーメッセージ取得モジュール
		/// </summary>
		/// <param name="strInputCheckType">入力チェックタイプ</param>
		/// <param name="strParam1">エラーメッセージ用パラメータ１</param>
		/// <param name="strParam2">エラーメッセージ用パラメータ２</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetErrorMessage(string strInputCheckType, string strParam1, string strParam2)
		{
			return GetErrorMessage(strInputCheckType, strParam1, strParam2, "", "");
		}
		/// <summary>
		/// エラーメッセージ取得モジュール
		/// </summary>
		/// <param name="strInputCheckType">入力チェックタイプ</param>
		/// <param name="strParam1">エラーメッセージ用パラメータ１</param>
		/// <param name="strParam2">エラーメッセージ用パラメータ２</param>
		/// <param name="strParam3">エラーメッセージ用パラメータ３</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetErrorMessage(string strInputCheckType, string strParam1, string strParam2, string strParam3)
		{
			return GetErrorMessage(strInputCheckType, strParam1, strParam2, strParam3, "");
		}
		/// <summary>
		/// エラーメッセージ取得モジュール
		/// </summary>
		/// <param name="strInputCheckType">入力チェックタイプ</param>
		/// <param name="strParam1">エラーメッセージ用パラメータ１</param>
		/// <param name="strParam2">エラーメッセージ用パラメータ２</param>
		/// <param name="strParam3">エラーメッセージ用パラメータ３</param>
		/// <param name="strParam4">エラーメッセージ用パラメータ４</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetErrorMessage(string strInputCheckType, string strParam1, string strParam2, string strParam3, string strParam4)
		{
			string strErrorMessages = "";

			switch (strInputCheckType)
			{
				// 必須チェック
				case CHECK_NECESSARY:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_NECESSARY);
					break;

				// 文字数チェック
				case CHECK_LENGTH:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_LENGTH);
					break;

				// 最大文字数チェック
				case CHECK_LENGTH_MAX:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_LENGTH_MAX);
					break;

				// 最小文字数チェック
				case CHECK_LENGTH_MIN:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_LENGTH_MIN);
					break;

				// 文字バイト数チェック
				case CHECK_BYTE_LENGTH:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_BYTE_LENGTH);
					break;

				// 最大文字バイト数チェック
				case CHECK_BYTE_LENGTH_MAX:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_BYTE_LENGTH_MAX);
					break;

				// 最小文字バイト数チェック
				case CHECK_BYTE_LENGTH_MIN:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_BYTE_LENGTH_MIN);
					break;

				// 最大数値チェック
				case CHECK_NUMBER_MAX:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_NUMBER_MAX);
					break;

				// 最小数値チェック
				case CHECK_NUMBER_MIN:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_NUMBER_MIN);
					break;

				// 全角チェック
				case CHECK_STRTYPE_FULLWIDTH:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_FULLWIDTH);
					break;

				// 全角ひらがなチェック
				case CHECK_STRTYPE_FULLWIDTH_HIRAGANA:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_FULLWIDTH_HIRAGANA);
					break;

				// 全角カタカナチェック
				case CHECK_STRTYPE_FULLWIDTH_KATAKANA:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_FULLWIDTH_KATAKANA);
					break;

				// 半角チェック
				case CHECK_STRTYPE_HALFWIDTH:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH);
					break;

				// 半角英数チェック
				case CHECK_STRTYPE_HALFWIDTH_ALPHNUM:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_ALPHNUM);
					break;

				// 半角英数記号チェック
				case CHECK_STRTYPE_HALFWIDTH_ALPHNUMSYMBOL:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_ALPHNUMSYMBOL);
					break;

				// 半角数字チェック
				case CHECK_STRTYPE_HALFWIDTH_NUMBER:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_NUMBER);
					break;

				// 半角数字（正数のみ）チェック
				case CHECK_STRTYPE_HALFWIDTH_DECIMAL:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DECIMAL);
					break;

				// 半角日付チェック
				case CHECK_STRTYPE_HALFWIDTH_DATE:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_DATE);
					break;

				// 半角数字チェック
				case CHECK_STRTYPE_HALFWIDTH_INT:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_HALFWIDTH_NUMERIC);
					break;

				// 日付チェック
				case CHECK_STRTYPE_DATE:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_DATE);
					break;

				// 未来日付チェック
				case CHECK_STRTYPE_DATE_FUTURE:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_DATE_FUTURE);
					break;

				// 過去日付チェック
				case CHECK_STRTYPE_DATE_PAST:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_DATE_PAST);
					break;

				// メールアドレスチェック
				case CHECK_STRTYPE_MAILADDRESS:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_MAILADDRESS);
					break;

				// 都道府県チェック
				case CHECK_STRTYPE_PREFECTURE:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_PREFECTURE);
					break;

				// 都道府県チェック
				case CHECK_STRTYPE_TAX_RATE:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_TAX_RATE);
					break;

				// 禁止文字列チェック
				case CHECK_PROHIBITED_CHAR:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_PROHIBITED_CHAR);
					break;

				// 除外文字コードチェック
				case CHECK_OUTOFSHIFTJISCODE:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_OUTOFCHARCODE);
					break;

				// 正規表現チェック
				case CHECK_REGEXP:
					if (strParam2 != "")
					{
						strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_REGEXP);
					}
					else
					{
						strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_REGEXP2);
					}
					break;

				// 正規表現（除外）チェック
				case CHECK_EXCEPT_REGEXP:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_EXCEPT_REGEXP);
					break;

				// 確認入力チェック
				case CHECK_CONFIRM:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_CONFIRM);
					break;

				// 同値チェック
				case CHECK_EQUIVALENCE:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_EQUIVALENCE);
					break;

				// 異値チェック
				case CHECK_DIFFERENT_VALUE:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_DIFFERENT_VALUE);
					break;

				// 重複チェック
				case CHECK_DUPLICATION:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_DUPLICATION);
					break;

				// 期間内重複チェック
				case CHECK_DUPLICATION_DATERANGE:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_DUPLICATION_DATERANGE);
					break;

				// 開始日と終了日チェック
				case CHECK_DATERANGE:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_DATERANGE);
					break;

				// 通貨チェック
				case CHECK_STRTYPE_CURRENCY:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_CURRENCY);
					break;

				// 時間チェック
				case CHECK_STRTYPE_DROPDOWN_TIME:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_DROPDOWN_TIME);
					break;

				// 上限金額チェック
				case CHECK_PRICE_MAX:
					strErrorMessages = MessageManager.GetMessages(MessageManager.INPUTCHECK_PRICE_MAX);
					break;
			}

			// パラメタのリプレース
			strErrorMessages = strErrorMessages.Replace("@@ 1 @@", strParam1).Replace("@@ 2 @@", strParam2).Replace("@@ 3 @@", strParam3).Replace("@@ 4 @@", strParam4);

			return strErrorMessages;
		}

		/// <summary>
		/// 特定文字を除去する
		/// </summary>
		/// <param name="strValue">対象文字列</param>
		/// <param name="strChars">連続した除去文字</param>
		/// <returns>除去後の文字列</returns>
		private static string RemoveChars(string strValue, string strChars)
		{
			foreach (char ch in strChars)
			{
				strValue = strValue.Replace(ch.ToString(), "");
			}

			return strValue;
		}

		/// <summary>
		/// 開始日と終了日の比較
		/// </summary>
		/// <param name="beginDate">開始日</param>
		/// <param name="endDate">終了日</param>
		/// <returns>終了日 > 開始日:true</returns>
		public static bool CheckDateRange(object beginDate, object endDate)
		{
			DateTime beginDateTmp, endDateTmp;

			if (DateTime.TryParse(StringUtility.ToEmpty(beginDate), out beginDateTmp) == false
				|| DateTime.TryParse(StringUtility.ToEmpty(endDate), out endDateTmp) == false)
			{
				return false;
			}

			return DateTime.Compare(beginDateTmp, endDateTmp) <= 0;
		}

		/// <summary>
		/// メールアドレス入力チェック（カンマ区切り複数可能）
		/// </summary>
		/// <param name="mailInputs">メールアドレス（カンマ区切り複数可能）</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckAllMailAddrInputs(string mailInputs)
		{
			string error = string.Empty;
			foreach (string input in mailInputs.Split(','))
			{
				string message = CheckMailAddressError(string.Format("\"{0}\" ", input.Trim()), input.Trim());
				if (message != "") error += w2.Common.Web.HtmlSanitizer.HtmlEncode(message) + "<br />";
			}

			return error;
		}

		/// <summary>
		/// メールアドレス入力チェック（カンマ区切り複数可能）
		/// </summary>
		/// <param name="itemName">項目名</param>
		/// <param name="mailInputs">メールアドレス（カンマ区切り複数可能）</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>いずれのメールアドレスが不正だったらエラーメッセージを戻る</remarks>
		public static string CheckMailAddrInputs(string itemName, string mailInputs)
		{
			foreach (string input in mailInputs.Split(','))
			{
				string errorMessage = CheckMailAddrInput(itemName, input.Trim());
				if (errorMessage != "") return errorMessage;
			}

			return "";
		}

		/// <summary>
		/// メールアドレス入力チェック
		/// </summary>
		/// <param name="itemName">項目名</param>
		/// <param name="mailInput">メールアドレス</param>	
		/// <returns>エラーメッセージ</returns>
		public static string CheckMailAddrInput(string itemName, string mailInput)
		{
			Match match = Regex.Match(mailInput.Trim(), "<.*>");
			string mailAddr = (match.Success) ? match.Value.Replace("<", "").Replace(">", "").Trim() : mailInput.Trim();

			if((string.IsNullOrEmpty(mailAddr.Trim())) && (mailInput != mailAddr))
			{
				return CheckMailAddressError(itemName, mailInput);
			}

			return CheckMailAddressError(itemName, mailAddr);
			
		}

		/// <summary>
		/// 通貨数値の形式チェック
		/// </summary>
		/// <param name="value">値</param>
		/// <param name="localeId">通貨ロケールID</param>
		/// <param name="currencyDecimalDigits">補助単位 小数点以下の有効桁数</param>
		/// <returns>OK:true NG:false</returns>
		public static bool IsCurrency(string value, string localeId, int? currencyDecimalDigits = null)
		{
			// 通貨の小数点以下の有効桁数と入力値の小数点以下の有効桁数が一致
			var digits = currencyDecimalDigits ?? CultureInfo.CreateSpecificCulture(localeId).NumberFormat.CurrencyDecimalDigits;

			var result = (digits == ((value.Contains("."))
				? value.Substring(value.IndexOf(".") + 1).Length
				: 0));
			return result;
		}

		/// <summary>
		/// 通貨数値の形式チェック
		/// </summary>
		/// <param name="name">項目名</param>
		/// <param name="value">値</param>
		/// <param name="localeId">通貨ロケールID</param>
		/// <param name="currencyDecimalDigits">補助単位 小数点以下の有効桁数</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckCurrency(string name, string value, string localeId, int? currencyDecimalDigits = null)
		{
			// 半角数値チェック
			var errorMessage = CheckHalfwidthDecimalError(name, value);
			if (string.IsNullOrEmpty(errorMessage) == false) return errorMessage;

			if ((StringUtility.ToEmpty(value).Length == 0)
				|| (StringUtility.ToEmpty(localeId).Length == 0)
				|| IsCurrency(value, localeId, currencyDecimalDigits)) return "";

			var digits = currencyDecimalDigits ?? CultureInfo.CreateSpecificCulture(localeId).NumberFormat.CurrencyDecimalDigits;
			return GetErrorMessage(CHECK_STRTYPE_CURRENCY,
				name,
				value,
				digits.ToString(),
				String.Format("{0:F" + digits + "}", 1000));

		}

		/// <summary>
		/// ターゲットリスト抽出用の誕生日付チェック
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="value">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckBirthDataType(string name, string value)
		{
			// 空の値はチェックを行わない
			if (string.IsNullOrEmpty(value)) return "";

			var hasError = false;
			if (value.Contains("today"))
			{
				var reg1 = new Regex("\\(.*\\)");
				if (reg1.IsMatch(value))
				{
					// 抜き出した部分だけで日付チェック
					var msg = CheckDateTimeError(name, reg1.Match(value).Value.Replace("(", "").Replace(")", ""));
					if (string.IsNullOrEmpty(msg) == false) return msg;

					// 抜き出した部分をtodayで置換
					value = reg1.Replace(value, "today");
				}
				var reg2 = new Regex("^today.(year|month|day)([+-][0-9]{1,})?$");
				if (reg2.IsMatch(value) == false) hasError = true;
			}
			else
			{
				hasError = (IsHalfwidthNumber(value) == false);
			}
			var errorMessage = hasError ? GetErrorMessage(CHECK_REGEXP, name) : "";
			return errorMessage;
		}

		/// <summary>
		/// ターゲットリスト抽出用の日付チェック
		/// </summary>
		/// <param name="name">名称</param>
		/// <param name="value">値</param>
		/// <returns>エラーメッセージ</returns>
		public static string CheckDateTimeError(string name, string value)
		{
			// 空の値はチェックを行わない
			if (string.IsNullOrEmpty(value)) return "";

			var hasError = false;
			if (value.Contains("today"))
			{
				var reg = new Regex("^today([+-][0-9]{1,}[ymd]){0,3}$");
				if (reg.IsMatch(value) == false) hasError = true;
			}
			else
			{
				return CheckDateExactError(name, value, "yyyy/M/d");
			}
			var errorMessage = hasError ? GetErrorMessage(CHECK_REGEXP, name) : "";
			return errorMessage;
		}

		/// <summary>
		/// 開始数値と終了数値の比較
		/// </summary>
		/// <param name="beginNumber">開始数値</param>
		/// <param name="endNumber">終了数値</param>
		/// <returns>終了数値 > 開始数値:true</returns>
		public static bool CheckNumberRange(object beginNumber, object endNumber)
		{
			decimal beginNumberTmp, endNumberTmp;

			if ((decimal.TryParse(StringUtility.ToEmpty(beginNumber), out beginNumberTmp) == false)
				|| (decimal.TryParse(StringUtility.ToEmpty(endNumber), out endNumberTmp) == false))
			{
				return false;
			}

			return decimal.Compare(beginNumberTmp, endNumberTmp) <= 0;
		}

		/// <summary>
		/// 携帯電話番号か
		/// </summary>
		/// <param name="ownerTel">電話番号</param>
		/// <returns>携帯電話番号</returns>
		public static bool CheckValidCellPhoneNumber(string ownerTel)
		{
			var regex = new Regex(@"([0]|[8][1]|[+][8][1])[7-9][0](\d{8}|-\d{4}-\d{4})");
			var result = regex.IsMatch(ownerTel);
			return result;
		}

		/// <summary>
		/// Check halfwidth int error
		/// </summary>
		/// <param name="name">Name</param>
		/// <param name="value">Value</param>
		/// <returns>Error message</returns>
		public static string CheckHalfwidthIntError(string name, string value)
		{
			if ((IsHalfwidthInt(value) == false)
				&& (StringUtility.ToEmpty(value).Length != 0))
			{
				return GetErrorMessage(CHECK_STRTYPE_HALFWIDTH_INT, name);
			}

			return string.Empty;
		}

		/// <summary>
		/// Is halfwidth int
		/// </summary>
		/// <param name="value">Value</param>
		/// <returns>Is halfwidth int</returns>
		public static bool IsHalfwidthInt(string value)
		{
			var numberTemp = 0;
			var isHalfwidthInt = int.TryParse(value, out numberTemp);
			return isHalfwidthInt;
		}
	}
}