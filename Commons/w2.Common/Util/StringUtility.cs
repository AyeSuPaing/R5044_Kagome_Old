/*
=========================================================================================================
  Module      : 文字列ユーティリティモジュール(StringUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ionic.Zlib;

namespace w2.Common.Util
{
	///**************************************************************************************
	/// <summary>
	/// 文字列関連の変換処理を行う
	/// </summary>
	///**************************************************************************************
	public class StringUtility
	{
		// 文字変換用マッピングテーブル //

		const string HANKAKU_ALPHNUMSYMBOL_MAP = " ､｡,.･:;?!ﾞﾟ´`^~_ｰ/|'\"()[]{}｢｣+-=<>\\$%#&*@0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
		const string ZENKAKU_ALPHNUMSYMBOL_MAP = "　、。，．・：；？！゛゜´｀＾～＿ー／｜’”（）［］｛｝「」＋－＝＜＞￥＄％＃＆＊＠０１２３４５６７８９ＡＢＣＤＥＦＧＨＩＪＫＬＭＮＯＰＱＲＳＴＵＶＷＸＹＺａｂｃｄｅｆｇｈｉｊｋｌｍｎｏｐｑｒｓｔｕｖｗｘｙｚ";
		const string HANKAKU_KANA_MAP = "｡｢｣､･ｦｧｨｩｪｫｬｭｮｯｰｱｲｳｴｵｶｷｸｹｺｻｼｽｾｿﾀﾁﾂﾃﾄﾅﾆﾇﾈﾉﾊﾋﾌﾍﾎﾏﾐﾑﾒﾓﾔﾕﾖﾗﾘﾙﾚﾛﾜﾝﾞﾟ";
		const string ZENKAKU_KATAKANA_MAP = "。「」、・ヲァィゥェォャュョッーアイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワン゛゜";
		const string ZENKAKU_HIRAGANA_MAP = "。「」、・をぁぃぅぇぉゃゅょっーあいうえおかきくけこさしすせそたちつてとなにぬねのはひふへほまみむめもやゆよらりるれろわん゛゜";

		static string[] HANKAKU_KANA_MAP_DAKUTEN = { "ｶﾞ", "ｷﾞ", "ｸﾞ", "ｹﾞ", "ｺﾞ", "ｻﾞ", "ｼﾞ", "ｽﾞ", "ｾﾞ", "ｿﾞ", "ﾀﾞ", "ﾁﾞ", "ﾂﾞ", "ﾃﾞ", "ﾄﾞ", "ﾊﾞ", "ﾋﾞ", "ﾌﾞ", "ﾍﾞ", "ﾎﾞ", "ｳﾞ" };
		static string[] ZENKAKU_KATAKANA_MAP_DAKUTEN = { "ガ", "ギ", "グ", "ゲ", "ゴ", "ザ", "ジ", "ズ", "ゼ", "ゾ", "ダ", "ヂ", "ヅ", "デ", "ド", "バ", "ビ", "ブ", "ベ", "ボ", "ヴ" };
		static string[] ZENKAKU_HIRAGANA_MAP_DAKUTEN = { "が", "ぎ", "ぐ", "げ", "ご", "ざ", "じ", "ず", "ぜ", "ぞ", "だ", "ぢ", "づ", "で", "ど", "ば", "び", "ぶ", "べ", "ぼ", "う゛" };

		static string[] HANKAKU_KANA_MAP_HANDAKUTEN = { "ﾊﾟ", "ﾋﾟ", "ﾌﾟ", "ﾍﾟ", "ﾎﾟ" };
		static string[] ZENKAKU_KATAKANA_MAP_HANDAKUTEN = { "パ", "ピ", "プ", "ペ", "ポ" };
		static string[] ZENKAKU_HIRAGANA_MAP_HANDAKUTEN = { "ぱ", "ぴ", "ぷ", "ぺ", "ぽ" };

		static string[] HANKAKU_KANAETC_MAP_SRC = { "−" };
		static string[] ZENKAKU_KANAETC_MAP_DST = { "－" };

		/// <summary>システム利用不能制御文字（タブ、改行、復帰は利用可能とする）</summary>
		static string[] UNAVAILABLE_CONTROL_CHARACTER_CODE_MAP =
		{
			((char)0x00).ToString(), ((char)0x01).ToString(), ((char)0x02).ToString(), ((char)0x03).ToString(), ((char)0x04).ToString(), ((char)0x05).ToString(), ((char)0x06).ToString(), ((char)0x07).ToString(),
			((char)0x08).ToString(), /*((char)0x09).ToString(),*/ /*((char)0x0A).ToString(),*/ ((char)0x0B).ToString(), ((char)0x0C).ToString(), /*((char)0x0D,*/ ((char)0x0E).ToString(), ((char)0x0F).ToString(),
			((char)0x10).ToString(), ((char)0x11).ToString(), ((char)0x12).ToString(), ((char)0x13).ToString(), ((char)0x14).ToString(), ((char)0x15).ToString(), ((char)0x16).ToString(), ((char)0x17).ToString(),
			((char)0x18).ToString(), ((char)0x19).ToString(), ((char)0x1A).ToString(), ((char)0x1B).ToString(), ((char)0x1C).ToString(), ((char)0x1D).ToString(), ((char)0x1E).ToString(), ((char)0x1F).ToString(),
			((char)0x7f).ToString(), 
		};

		/// <summary>
		/// Nullから空文字へ変換
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <returns>変換後文字列</returns>
		public static string ToEmpty(object objSrc)
		{
			if ((objSrc == null) || (objSrc == DBNull.Value))
			{
				return "";
			}

			return objSrc.ToString();
		}

		/// <summary>
		/// Nullから値へ変換
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <param name="objNullValue">ヌルの場合の変換値</param>
		/// <returns>変換後文字列</returns>
		public static object ToValue(object objSrc, object objNullValue)
		{
			if ((objSrc == null) || (objSrc == DBNull.Value))
			{
				return objNullValue;
			}

			return objSrc;
		}

		/// <summary>
		/// 空文字からNULLへ変換
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <returns>変換後文字列</returns>
		public static string ToNull(object objSrc)
		{
			return (ToEmpty(objSrc) == "") ? null : ToEmpty(objSrc);
		}

		/// <summary>
		/// 日付文字へ変換
		/// </summary>
		/// <param name="objSrc">変換対象(日付文字列or日付型）</param>
		/// <param name="strDateFormat">日付フォーマット文字列</param>
		/// <returns>変換後文字列</returns>
		public static string ToDateString(object objSrc, string strDateFormat)
		{
			return ToDateString(objSrc, strDateFormat, "");
		}
		/// <summary>
		/// 日付文字へ変換
		/// </summary>
		/// <param name="objSrc">変換対象(日付文字列or日付型）</param>
		/// <param name="strDateFormat">日付フォーマット文字列</param>
		/// <param name="strDefault">変換できなかった場合に表示する文字列</param>
		/// <returns>変換後文字列</returns>
		public static string ToDateString(object objSrc, string strDateFormat, string strDefault)
		{
			if ((objSrc != DBNull.Value)
				&& (objSrc is DateTime))
			{
				return ((DateTime)objSrc).ToString(strDateFormat);
			}
			else if (objSrc is string)
			{
				DateTime dtTemp;
				if (DateTime.TryParse((string)objSrc, out dtTemp))
				{
					return dtTemp.ToString(strDateFormat);
				}
			}

			return strDefault;
		}

		/// <summary>
		/// 全角変換
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <returns>全角変換後の値</returns>
		public static string ToZenkaku(object objSrc)
		{
			return ToZenkaku(objSrc, null);
		}
		/// <summary>
		/// 全角変換
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <param name="strExcepts">除外文字列</param>
		/// <returns>全角変換後の値</returns>
		public static string ToZenkaku(object objSrc, params string[] strExcepts)
		{
			string strResult = ToEmpty(objSrc);

			// 半角カナ→全角カタカナ変換 ※先に変換
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP_DAKUTEN, ZENKAKU_KATAKANA_MAP_DAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP_HANDAKUTEN, ZENKAKU_KATAKANA_MAP_HANDAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP, ZENKAKU_KATAKANA_MAP, strExcepts);

			// 半角英数記号→全角英数記号変換
			strResult = ToOtherCharactorMap(strResult, HANKAKU_ALPHNUMSYMBOL_MAP, ZENKAKU_ALPHNUMSYMBOL_MAP, strExcepts);

			// その他かな変換（マックのハイフン「−」など）
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANAETC_MAP_SRC, ZENKAKU_KANAETC_MAP_DST, strExcepts);

			return strResult;
		}

		/// <summary>
		/// 全角カタカナ変換（半角カタカナ対象）
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <returns>全角カタカナ変換後の値</returns>
		public static string ToZenkakuKatakanaFromHankaku(object objSrc)
		{
			return ToZenkakuKatakanaFromHankaku(objSrc, null);
		}
		/// <summary>
		/// 全角カタカナ変換（半角カタカナ対象）
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <param name="strExcepts">除外文字列</param>
		/// <returns>全角カタカナ変換後の値</returns>
		public static string ToZenkakuKatakanaFromHankaku(object objSrc, params string[] strExcepts)
		{
			string strResult = ToEmpty(objSrc);

			// 半角カナ→全角カタカナ変換（濁点・半濁点を先に変換）
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP_DAKUTEN, ZENKAKU_KATAKANA_MAP_DAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP_HANDAKUTEN, ZENKAKU_KATAKANA_MAP_HANDAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP, ZENKAKU_KATAKANA_MAP, strExcepts);

			// その他かな変換（マックのハイフン「−」など）
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANAETC_MAP_SRC, ZENKAKU_KANAETC_MAP_DST, strExcepts);

			return strResult;
		}

		/// <summary>
		/// 全角カタカナ変換
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <returns>全角カタカナ変換後の値</returns>
		public static string ToZenkakuKatakana(object objSrc)
		{
			return ToZenkakuKatakana(objSrc, null);
		}
		/// <summary>
		/// 全角カタカナ変換
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <param name="strExcepts">除外文字列</param>
		/// <returns>全角カタカナ変換後の値</returns>
		public static string ToZenkakuKatakana(object objSrc, params string[] strExcepts)
		{
			string strResult = ToEmpty(objSrc);

			// 半角カナ→全角カタカナ変換（濁点・半濁点を先に変換）
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP_DAKUTEN, ZENKAKU_KATAKANA_MAP_DAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP_HANDAKUTEN, ZENKAKU_KATAKANA_MAP_HANDAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP, ZENKAKU_KATAKANA_MAP, strExcepts);

			// 全角ひらがな→全角カタカナ変換（濁点・半濁点を先に変換）
			strResult = ToOtherCharactorMap(strResult, ZENKAKU_HIRAGANA_MAP_DAKUTEN, ZENKAKU_KATAKANA_MAP_DAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, ZENKAKU_HIRAGANA_MAP_HANDAKUTEN, ZENKAKU_KATAKANA_MAP_HANDAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, ZENKAKU_HIRAGANA_MAP, ZENKAKU_KATAKANA_MAP, strExcepts);

			// その他かな変換（マックのハイフン「−」など）
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANAETC_MAP_SRC, ZENKAKU_KANAETC_MAP_DST, strExcepts);

			return strResult;
		}

		/// <summary>
		/// 全角ひらがな変換
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <returns>全角ひらがな変換後の値</returns>
		/// <remarks>全角変換しますが、半角カナも全角ひらがなへ変換します</remarks>
		public static string ToZenkakuHiragana(object objSrc)
		{
			return ToZenkakuHiragana(objSrc, null);
		}
		/// <summary>
		/// 全角ひらがな変換
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <param name="strExcepts">除外文字列</param>
		/// <returns>全角ひらがな変換後の値</returns>
		/// <remarks>全角変換しますが、半角カナも全角ひらがなへ変換します</remarks>
		public static string ToZenkakuHiragana(object objSrc, params string[] strExcepts)
		{
			string strResult = ToEmpty(objSrc);

			// 半角カナ→全角ひらがな変換（濁点・半濁点を先に変換）
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP_DAKUTEN, ZENKAKU_HIRAGANA_MAP_DAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP_HANDAKUTEN, ZENKAKU_HIRAGANA_MAP_HANDAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANA_MAP, ZENKAKU_HIRAGANA_MAP, strExcepts);

			// 全角カタカナ→全角ひらがな変換（濁点・半濁点を先に変換）
			strResult = ToOtherCharactorMap(strResult, ZENKAKU_KATAKANA_MAP_DAKUTEN, ZENKAKU_HIRAGANA_MAP_DAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, ZENKAKU_KATAKANA_MAP_HANDAKUTEN, ZENKAKU_HIRAGANA_MAP_HANDAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, ZENKAKU_KATAKANA_MAP, ZENKAKU_HIRAGANA_MAP, strExcepts);

			// その他かな変換（マックのハイフン「−」など）
			strResult = ToOtherCharactorMap(strResult, HANKAKU_KANAETC_MAP_SRC, ZENKAKU_KANAETC_MAP_DST, strExcepts);

			return strResult;
		}

		/// <summary>
		/// 半角変換
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <returns>半角変換後の値</returns>
		/// <remarks>半角変換しますが、ひらがなは半角カナへ変換されません</remarks>
		public static string ToHankaku(object objSrc)
		{
			return ToHankaku(objSrc, null);
		}
		/// <summary>
		/// 半角変換
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <param name="strExcepts">除外文字列</param>
		/// <returns>半角変換後の値</returns>
		/// <remarks>半角変換しますが、ひらがなは半角カナへ変換されません</remarks>
		public static string ToHankaku(object objSrc, params string[] strExcepts)
		{
			string strResult = ToEmpty(objSrc);

			// 半角カタカナ変換 ※先に変換
			strResult = ToHankakuKatakana(strResult);

			// 全角英数記号→半角英数記号変換
			strResult = ToOtherCharactorMap(strResult, ZENKAKU_ALPHNUMSYMBOL_MAP, HANKAKU_ALPHNUMSYMBOL_MAP, strExcepts);

			return strResult;
		}

		/// <summary>
		/// 半角カタカナ変換
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <returns>半角変換後の値</returns>
		/// <remarks>半角変換しますが、ひらがなは半角カナへ変換されません</remarks>
		public static string ToHankakuKatakana(object objSrc)
		{
			return ToHankakuKatakana(objSrc, null);
		}
		/// <summary>
		/// 半角カタカナ変換
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <param name="strExcepts">除外文字列</param>
		/// <returns>半角変換後の値</returns>
		/// <remarks>半角変換しますが、ひらがなは半角カナへ変換されません</remarks>
		public static string ToHankakuKatakana(object objSrc, params string[] strExcepts)
		{
			string strResult = ToEmpty(objSrc);

			// 全角カタカナ→半角カナ変換（濁点・半濁点を先に変換）
			strResult = ToOtherCharactorMap(strResult, ZENKAKU_KATAKANA_MAP_DAKUTEN, HANKAKU_KANA_MAP_DAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, ZENKAKU_KATAKANA_MAP_HANDAKUTEN, HANKAKU_KANA_MAP_HANDAKUTEN, strExcepts);
			strResult = ToOtherCharactorMap(strResult, ZENKAKU_KATAKANA_MAP, HANKAKU_KANA_MAP, strExcepts);

			// 全角ひらがな→半角カナ変換（濁点・半濁点を先に変換）
			//strResult = ToOtherCharactorMap(strResult, ZENKAKU_HIRAGANA_MAP_DAKUTEN, HANKAKU_KANA_MAP_DAKUTEN, strExcepts);
			//strResult = ToOtherCharactorMap(strResult, ZENKAKU_HIRAGANA_MAP_HANDAKUTEN, HANKAKU_KANA_MAP_HANDAKUTEN, strExcepts);
			//strResult = ToOtherCharactorMap(strResult, ZENKAKU_HIRAGANA_MAP, HANKAKU_KANA_MAP, strExcepts);

			return strResult;
		}

		/// <summary>
		/// キャラクタ変換（半角全角変換等用）
		/// </summary>
		/// <param name="strSrc">対象文字列</param>
		/// <param name="strSrcMap">変換前キャラクタマップ</param>
		/// <param name="strDstMap">変換後キャラクタマップ</param>
		/// <param name="strExcepts">除外文字</param>
		/// <returns>変換後文字列</returns>
		private static string ToOtherCharactorMap(string strSrc, string strSrcMap, string strDstMap, string[] strExcepts)
		{
			List<string> lExcepts = (strExcepts != null) ? new List<string>(strExcepts) : null;

			StringBuilder sbResult = new StringBuilder(strSrc);
			for (int iLoop = 0; iLoop < strSrcMap.Length; iLoop++)
			{
				if ((lExcepts != null)
					&& (lExcepts.IndexOf(strSrcMap[iLoop].ToString()) != -1))
				{
					continue;
				}

				sbResult.Replace(strSrcMap[iLoop], strDstMap[iLoop]);
			}

			return sbResult.ToString();
		}

		/// <summary>
		/// キャラクタ変換（半角全角変換等用）
		/// </summary>
		/// <param name="strSrc">対象文字列</param>
		/// <param name="strSrcMap">変換前キャラクタマップ</param>
		/// <param name="strDstMap">変換後キャラクタマップ</param>
		/// <param name="strExcepts">除外文字</param>
		/// <returns>変換後文字列</returns>
		private static string ToOtherCharactorMap(string strSrc, string[] strSrcMap, string[] strDstMap, string[] strExcepts)
		{
			List<string> lExcepts = (strExcepts != null) ? new List<string>(strExcepts) : null;

			StringBuilder sbResult = new StringBuilder(strSrc);
			for (int iLoop = 0; iLoop < strSrcMap.Length; iLoop++)
			{
				if ((lExcepts != null)
					&& (lExcepts.IndexOf(strSrcMap[iLoop]) != -1))
				{
					continue;
				}

				sbResult.Replace(strSrcMap[iLoop], strDstMap[iLoop]);
			}

			return sbResult.ToString();
		}

		/// <summary>
		/// 数値を３桁区切りの文字列へ変換
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <returns>変換値</returns>
		public static string ToNumeric(object objSrc)
		{
			string strSrc = StringUtility.ToEmpty(objSrc);

			decimal dSrc;
			if (decimal.TryParse(strSrc, out dSrc))
			{
				// 小数点あり？
				if (strSrc.IndexOf('.') != -1)
				{
					long lSrc = (long)dSrc;

					return string.Format("{0:#,##0}", lSrc) + (dSrc - lSrc).ToString().Substring(1);
				}
				// 小数点なし
				else
				{
					return string.Format("{0:#,##0}", dSrc);
				}
			}

			return "";
		}

		/// <summary>
		/// １以上の時プラス記号を付加して３桁区切りの文字列に変換
		/// </summary>
		/// <param name="source">変換対象</param>
		/// <returns>変換値</returns>
		public static string ToNumericWithPlusSign(object source)
		{
			var src = ToEmpty(source);
			decimal value;
			if (decimal.TryParse(src, out value))
			{
				var numeric = (value >= 1)
					? "+" + ToNumeric(value)
					: ToNumeric(value);
				return numeric;
			}

			return string.Empty;
		}

		/// <summary>
		/// 文字列をＸ文字ごとに分割
		/// </summary>
		/// <param name="sourceObj">変換対象</param>
		/// <param name="length">分割する文字数</param>
		/// <returns>変換値</returns>
		public static string[] SplitByLength(object sourceObj, int length)
		{
			var source = ToEmpty(sourceObj);
			var max = (int)Math.Ceiling((decimal)source.Length / length);

			var result = new List<string>();
			for (var i = 0; i < max; i++)
			{
				var start = length * i;
				if (source.Length <= start) break;
				result.Add(
					(source.Length < start + length)
						? source.Substring(start)
						: source.Substring(start, length));
			}
			return result.ToArray();
		}

		/// <summary>
		/// 数値を０埋めの文字列へ変換（数値チェックしない）
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <param name="ilength">長さ</param>
		/// <returns>変換値</returns>
		public static string ToZeroNumeric(object objSrc, int ilength)
		{
			decimal dSrc;
			if (decimal.TryParse(StringUtility.ToEmpty(objSrc), out dSrc))
			{
				return String.Format("{0:D" + ilength.ToString() + "}", dSrc);
			}

			return "";
		}

		/// <summary>
		/// 書式及び通貨ロケールIDに基づいて通貨文字列へ変換
		/// デフォルトは通貨ロケールID(ja-JP),数値を３桁区切り({0:C})
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <param name="localeId">通貨ロケールID</param>
		/// <param name="format">書式</param>
		/// <returns>変換値</returns>
		public static string ToPrice(object objSrc, string localeId = "ja-JP", string format = "{0:C}")
		{
			decimal priceNum;
			if (decimal.TryParse(StringUtility.ToEmpty(objSrc), out priceNum))
			{
				return String.Format(CultureInfo.CreateSpecificCulture(localeId), format, priceNum);
			}

			return "";
		}

		/// <summary>
		/// 文字を切り詰める
		/// </summary>
		/// <param name="src">変換対象</param>
		/// <param name="length">長さ</param>
		/// <returns>変換値</returns>
		public static string StrTrim(object src, int length)
		{
			return StrTrim(src, length, "");
		}
		/// <summary>
		/// 文字を切り詰める
		/// </summary>
		/// <param name="src">変換対象</param>
		/// <param name="length">長さ</param>
		/// <param name="replaceString">置換文字列</param>
		/// <returns>変換値</returns>
		public static string StrTrim(object src, int length, string replaceString)
		{
			string srcString = ToEmpty(src);
			if (srcString.Length <= length) return srcString;

			// 結合文字列の可能性があるのでStringInfoを利用して再比較・置換
			var info = new System.Globalization.StringInfo(srcString);
			if (info.LengthInTextElements > length)
			{
				srcString = info.SubstringByTextElements(0, length) + replaceString;
			}
			return srcString;
		}

		/// <summary>
		/// アスタリスク変換
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <returns>アスタリスク変換後文字列</returns>
		public static string ChangeToAster(string objSrc)
		{
			if (string.IsNullOrEmpty(objSrc)) return string.Empty;

			StringBuilder sbResult = new StringBuilder();
			for (int iLoop = 0; iLoop < objSrc.Length; iLoop++)
			{
				sbResult.Append("*");
			}

			return sbResult.ToString();
		}

		/// <summary>
		/// 改行コード("\r\n","\r","\n")から"&lt;br&gt;"変換
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <returns>改行コード変換後文字列</returns>
		public static string ChangeToBrTag(string objSrc)
		{
			if (objSrc != null)
			{
				// 改行コードを<br />に変換
				return objSrc.Replace("\r\n", "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
			}

			return "";
		}

		/// <summary>バックスラッシュをスラッシュに変換</summary>
		/// <param name="targetString">対象文字列</param>
		/// <return>変換後文字列</return>
		public static string ConvertBackSlashToSlash(string targetString)
		{
			return (targetString != null) ? targetString.Replace(@"\", "/") : "";
		}

		/// <summary>スラッシュをバックスラッシュに変換</summary>
		/// <param name="targetString">対象文字列</param>
		/// <return>変換後文字列</return>
		public static string ConvertSlachToBackSlash(string targetString)
		{
			return (targetString != null) ? targetString.Replace("/", @"\") : "";
		}

		/// <summary>
		/// CSV出力カラムデータのエスケープ
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <param name="newLineReplaceString">改行文字から変換文字列</param>
		/// <returns>エスケープ後カラムデータ</returns>
		public static string EscapeCsvColumn(string objSrc, string newLineReplaceString = "\n")
		{
			if (objSrc != null)
			{
				// 改行コードを"\n"に変換、ダブルクォートをエスケープ
				return objSrc.Replace("\r\n", newLineReplaceString).Replace("\r", newLineReplaceString).Replace("\"", "\"\"");
			}

			return "";
		}

		/// <summary>
		/// エスケープ済みCSV文字列の生成:
		/// 各要素をダブルクオートで囲んだCSV文字列（"hoge","piyo",...,"XXX"）を返します。
		/// 各要素に含まれる改行コード・ダブルクオートはエスケープ処理されます。
		/// </summary>
		/// <param name="strings">文字列配列</param>
		/// <returns>CSV文字列</returns>
		public static string CreateEscapedCsvString(IEnumerable<string> strings)
		{
			var result = string.Join(",",
				strings.Select(s => string.Format("\"{0}\"", EscapeCsvColumn(s))));
			return result;
		}

		/// <summary>
		/// 特定文字で分割した部分文字列取得
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <param name="chSeparator">セパレータ</param>
		/// <param name="iIndex">分割された要素のインデックス（0～)</param>
		/// <returns>部分文字列</returns>
		public static string GetSplitedValue(object objSrc, char chSeparator, int iIndex)
		{
			string[] strSepareted = (StringUtility.ToEmpty(objSrc)).Split(chSeparator);
			if (iIndex < strSepareted.Length)
			{
				return strSepareted[iIndex];
			}

			return "";
		}

		/// <summary>
		/// CSV行分割
		/// </summary>
		/// <param name="strCsvLine">CSV列</param>
		/// <returns>分割後の文字配列</returns>
		public static string[] SplitCsvLine(string strCsvLine)
		{
			string[] strResults = null;

			//------------------------------------------------------
			// 正規表現１（フィールドマッチング）
			//
			// (?:[^"]|"")* ・・・「"」以外 または 「""」を含む文字列。
			// [^,]*		・・・「,」以外の連続文字列。
			//
			//	("(?:[^"]|"")*"|[^,]*),		・・・（「"」でくくられた（「"」以外 または 「""」を含む文字列））または「,」以外の連続文字列＋「,」
			//------------------------------------------------------
			string strParrtern1 = "(\"(?:[^\"]|\"\")*\"|[^,]*),";

			//------------------------------------------------------
			// 正規表現２（両側のダブルクォート削除）
			//
			// ^"|",$|,$	・・・先頭の「"」もしくは終端の「",」もしくは終端の「,」にマッチング。
			//
			//------------------------------------------------------
			string strParrtern2 = "^\"|\",$|,$";

			// 正規表現にパターン１適用＆マッチング
			System.Text.RegularExpressions.Regex regx = new System.Text.RegularExpressions.Regex(strParrtern1);
			System.Text.RegularExpressions.MatchCollection match = regx.Matches(strCsvLine + ",");

			// マッチングしたそれぞれについて、両端の不要文字「"」、「,」を削除して配列に格納
			strResults = new string[match.Count];
			for (int iLoop = 0; iLoop < match.Count; iLoop++)
			{
				System.Text.RegularExpressions.Regex regx2 = new System.Text.RegularExpressions.Regex(strParrtern2);
				strResults[iLoop] = regx2.Replace(match[iLoop].Value, "").Replace("\"\"", "\"");
			}

			return strResults;
		}

		/// <summary>
		/// SQLのLIKE検索用「#」エスケープ
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <returns>エスケープ済文字列</returns>
		/// <remarks>
		/// SQLのLIKE検索では元々「w2_LikeStringEscape」というSQL関数を用いて
		/// > category_id1 like dbo.w2_LikeStringEscape(@category_id) + '%' ESCAPE '#'
		/// と言うような処理を行ってきた。
		/// だが、WHERE文の中に関数を通した値が利用されると統計情報が正しく作成されず、
		/// 商品データが多い場合に検索に莫大な時間がかかってしまっていた。
		/// これを緩和するため、あらかじめLIKE検索用のエスケープ処理を加えたものを
		/// SQLのパラメタとして渡すようにする。
		/// </remarks>
		public static string SqlLikeStringSharpEscape(object objSrc)
		{
			return ToEmpty(objSrc)
				.Replace("#", "##")
				.Replace("%", "#%")
				.Replace("_", "#_")
				.Replace("[", "#[");
		}

		/// <summary>
		/// ヘッダフッタ付加
		/// </summary>
		/// <param name="strHeader">ヘッダ</param>
		/// <param name="ojbSrc">対象</param>
		/// <param name="strFooter">フッタ</param>
		/// <returns>変換値</returns>
		/// <remarks>対象が空(もしくはnull）の場合は空文字が返る</remarks>
		public static string AddHeaderFooter(string strHeader, object ojbSrc, string strFooter)
		{
			if (ToEmpty(ojbSrc) != "")
			{
				return strHeader + ojbSrc + strFooter;
			}

			return "";
		}

		/// <summary>
		/// 文字コード判定
		/// </summary>
		/// <param name="bytes"></param>
		/// <returns>
		/// 適当と思われるEncodingオブジェクト
		/// 判断できなかった時はnullとなる
		/// </returns>
		public static Encoding GetCode(byte[] bytes)
		{
			if (bytes.Length >= 4 &&
				(bytes[0] == 0xFF && bytes[1] == 0xFE && bytes[2] == 0x00 && bytes[3] == 0x00))
			{
				return Encoding.GetEncoding(12000); // UTF-32
			}
			if (bytes.Length >= 4 &&
				(bytes[0] == 0x00 && bytes[1] == 0x00 && bytes[2] == 0xFE && bytes[3] == 0xFF))
			{
				return Encoding.GetEncoding(12001); // UTF-32 Big Endian
			}
			if (bytes.Length >= 2 && (bytes[0] == 0xFF && bytes[1] == 0xFE))
			{
				return Encoding.GetEncoding(1200);  // UTF-16
			}
			if (bytes.Length >= 2 && (bytes[0] == 0xFE && bytes[1] == 0xFF))
			{
				return Encoding.GetEncoding(1201);  // UTF-16 Big Endian
			}
			if (IsJis(bytes) == true)
			{
				return Encoding.GetEncoding(50220); // 日本語 (JIS)
			}
			if (IsAscii(bytes) == true)
			{
				return Encoding.GetEncoding(20127); // US-ASCII
			}
			int utf8 = 0, sjis = 0, euc = 0;  // 文字コード判定用.
			bool bomFrag = false;             // UTF-8 BOM の判定用.
			bool Utf8Flag = IsUTF8(bytes, ref utf8, ref bomFrag);
			bool SJisFlag = IsShiftJis(bytes, ref sjis);
			bool EucFlag = IsEUC(bytes, ref euc);
			if (Utf8Flag == true && SJisFlag == false && EucFlag == false)
			{
				if (bomFrag == true)
				{
					return new UTF8Encoding(true);		// UTF-8 (BOMあり)
				}
				else
				{
					return new UTF8Encoding(false);		// UTF-8N (BOMなし)
				}
			}
			else if (SJisFlag == true && Utf8Flag == false && EucFlag == false)
			{
				return Encoding.GetEncoding(932);		// 日本語 (シフト JIS)
			}
			else if (EucFlag == true && Utf8Flag == false && SJisFlag == false)
			{
				return Encoding.GetEncoding(51932);		// 日本語 (EUC)
			}
			else
			{
				if (euc > sjis && euc > utf8)
				{
					return Encoding.GetEncoding(51932);		// 日本語 (EUC)
				}
				if (sjis > euc && sjis > utf8)
				{
				}
				else if (utf8 > euc && utf8 > sjis)
				{
					if (bomFrag == true)
					{
						return new UTF8Encoding(true);		// UTF-8 (BOMあり)
					}
					else
					{
						return new UTF8Encoding(false);		// UTF-8N (BOMなし)
					}
				}
			}
			return null;
		}

		/// <summary>
		/// JISコード判定(ISO-2022-JP)
		/// </summary>
		/// <param name="bytes">文字コード判定するバイト配列</param>
		/// <returns></returns>
		private static bool IsJis(byte[] bytes)
		{
			int length = bytes.Length;
			byte[] temp = new byte[6];
			for (int i = 0; i < length; i++)
			{
				temp[0] = bytes[i];
				if (temp[0] > 0x7F)
				{
					return false;   // Not ISO-2022-JP (0x00～0x7F)
				}
				else if (i < length - 2)
				{
					temp[1] = bytes[i + 1];
					temp[2] = bytes[i + 2];
					if (temp[0] == 0x1B && temp[1] == 0x28 && temp[2] == 0x42)
					{
						return true;    // ESC ( B  : JIS ASCII
					}
					else if (temp[0] == 0x1B && temp[1] == 0x28 && temp[2] == 0x4A)
					{
						return true;    // ESC ( J  : JIS X 0201-1976 Roman Set
					}
					else if (temp[0] == 0x1B && temp[1] == 0x28 && temp[2] == 0x49)
					{
						return true;    // ESC ( I  : JIS X 0201-1976 kana
					}
					else if (temp[0] == 0x1B && temp[1] == 0x24 && temp[2] == 0x40)
					{
						return true;    // ESC $ @  : JIS X 0208-1978(old_JIS)
					}
					else if (temp[0] == 0x1B && temp[1] == 0x24 && temp[2] == 0x42)
					{
						return true;    // ESC $ B  : JIS X 0208-1983(new_JIS)
					}
				}
				else if (i < length - 3)
				{
					temp[1] = bytes[i + 1];
					temp[2] = bytes[i + 2];
					temp[3] = bytes[i + 3];
					if (temp[0] == 0x1B && temp[1] == 0x24 && temp[2] == 0x28 && temp[3] == 0x44)
					{
						return true;    // ESC $ ( D  : JIS X 0212-1990（JIS_hojo_kanji）
					}
				}
				else if (i < length - 5)
				{
					temp[1] = bytes[i + 1];
					temp[2] = bytes[i + 2];
					temp[3] = bytes[i + 3];
					temp[4] = bytes[i + 4];
					temp[5] = bytes[i + 5];
					if (temp[0] == 0x1B && temp[1] == 0x26 && temp[2] == 0x40 &&
						temp[3] == 0x1B && temp[4] == 0x24 && temp[5] == 0x42)
					{
						return true;    // ESC & @ ESC $ B  : JIS X 0208-1990, JIS X 0208:1997
					}
				}
				else
				{
					continue;
				}
			}
			return false;
		}

		/// <summary>
		/// ASCIIコード判定
		/// </summary>
		/// <param name="bytes">文字コード判定するバイト配列</param>
		/// <returns></returns>
		private static bool IsAscii(byte[] bytes)      // Check for Ascii
		{
			foreach (byte bt in bytes)
			{
				if (bt <= 0x7F)
				{
					// ASCII : 0x00～0x7F
					continue;
				}
				else
				{
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// S-JIS判定
		/// </summary>
		/// <param name="bytes">文字コード判定するバイト配列</param>
		/// <returns></returns>
		private static bool IsShiftJis(byte[] bytes, ref int sjis)
		{
			int length = bytes.Length;
			byte[] temp = new byte[2];
			for (int i = 0; i < length; i++)
			{
				temp[0] = bytes[i];
				if (temp[0] >= 0x00 && temp[0] <= 0x7F)
				{
					// ASCII : 0x00～0x7F
					continue;
				}
				if (temp[0] >= 0xA1 && temp[0] <= 0xDF)
				{
					// kana  : 0xA1～0xDF
					continue;
				}
				if (i < length - 1)
				{
					temp[1] = bytes[i + 1];
					if (((temp[0] >= 0x81 && temp[0] <= 0x9F) || (temp[0] >= 0xE0 && temp[0] <= 0xFC)) &&
						((temp[1] >= 0x40 && temp[1] <= 0x7E) || (temp[1] >= 0x80 && temp[1] <= 0xFC)))
					{
						// kanji first byte  : 0x81～0x9F or 0xE0～0xFC
						//       second byte : 0x40～0x7E or 0x80～0xFC
						i += 1;
						sjis += 2;
						continue;
					}
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// EUC判定
		/// </summary>
		/// <param name="bytes">文字コード判定するバイト配列</param>
		/// <returns></returns>
		private static bool IsEUC(byte[] bytes, ref int euc)
		{
			int length = bytes.Length;
			byte[] temp = new byte[3];
			for (int i = 0; i < length; i++)
			{
				temp[0] = bytes[i];
				if (temp[0] >= 0x00 && temp[0] <= 0x7F)
				{
					// ASCII : 0x00～0x7F
					continue;
				}
				if (i < length - 1)
				{
					temp[1] = bytes[i + 1];
					if ((temp[0] >= 0xA1 && temp[0] <= 0xFE) &&
						(temp[1] >= 0xA1 && temp[1] <= 0xFE))
					{
						// kanji - 0xA1～0xFE, 0xA1～0xFE
						i += 1;
						euc += 2;
						continue;
					}
					if ((temp[0] == 0x8E) &&
						(temp[1] >= 0xA1 && temp[1] <= 0xDF))
					{
						// kana - 0x8E, 0xA1～0xDF
						i += 1;
						euc += 2;
						continue;
					}
				}
				if (i < length - 2)
				{
					temp[1] = bytes[i + 1];
					temp[2] = bytes[i + 2];
					if ((temp[0] == 0x8F) &&
						(temp[1] >= 0xA1 && temp[1] <= 0xFE) &&
						(temp[2] >= 0xA1 && temp[2] <= 0xFE))
					{
						// hojo kanji - 0x8F, 0xA1～0xFE, 0xA1～0xFE
						i += 2;
						euc += 3;
						continue;
					}
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// UTFコード判定
		/// </summary>
		/// <param name="bytes">文字コード判定するバイト配列</param>
		/// <param name="bBom">BOM有り/無し判定</param>
		/// <returns></returns>
		private static bool IsUTF8(byte[] bytes, ref int utf8, ref bool bomFlg)
		{
			int len = bytes.Length;
			byte[] temp = new byte[4];
			for (int i = 0; i < len; i++)
			{
				temp[0] = bytes[i];
				if (temp[0] >= 0x00 && temp[0] <= 0x7F)
				{
					// ASCII : 0x00～0x7F
					continue;
				}
				if (i < len - 1)
				{
					temp[1] = bytes[i + 1];
					if ((temp[0] >= 0xC0 && temp[0] <= 0xDF) &&
						(temp[1] >= 0x80 && temp[1] <= 0xBF))
					{
						// 2 byte char
						i += 1;
						utf8 += 2;
						continue;
					}
				}
				if (i < len - 2)
				{
					temp[1] = bytes[i + 1]; temp[2] = bytes[i + 2];
					if (temp[0] == 0xEF && temp[1] == 0xBB && temp[2] == 0xBF)
					{
						// BOM : 0xEF 0xBB 0xBF
						bomFlg = true;
						i += 2;
						utf8 += 3;
						continue;
					}
					if ((temp[0] >= 0xE0 && temp[0] <= 0xEF) &&
						(temp[1] >= 0x80 && temp[1] <= 0xBF) &&
						(temp[2] >= 0x80 && temp[2] <= 0xBF))
					{
						// 3 byte char
						i += 2;
						utf8 += 3;
						continue;
					}
				}
				if (i < len - 3)
				{
					temp[1] = bytes[i + 1]; temp[2] = bytes[i + 2]; temp[3] = bytes[i + 3];
					if ((temp[0] >= 0xF0 && temp[0] <= 0xF7) &&
						(temp[1] >= 0x80 && temp[1] <= 0xBF) &&
						(temp[2] >= 0x80 && temp[2] <= 0xBF) &&
						(temp[3] >= 0x80 && temp[3] <= 0xBF))
					{
						// 4 byte char
						i += 3;
						utf8 += 4;
						continue;
					}
				}
				return false;
			}
			return true;
		}

		/// <summary>
		/// 文字列に含まれる長い単語を途中改行可能な形式に変換
		/// ※WebSanitizerの後で呼び出してください。
		/// </summary>
		/// <param name="src">入力文字列</param>
		/// <param name="letterCount">改行文字数</param>
		/// <returns>変換後文字列</returns>
		public static string ToWordBreakString(string src, int letterCount)
		{
			return System.Text.RegularExpressions.Regex.Replace(src, "([\x21-\x7E]{" + letterCount + "})", "$1<br>");
		}

		/// <summary>
		/// システムで利用できないコントロールコード削除
		/// </summary>
		/// <param name="source">変換対象</param>
		/// <returns>変換後文字列</returns>
		public static string RemoveUnavailableControlCode(object source)
		{
			var temp = ToEmpty(source);
			return UNAVAILABLE_CONTROL_CHARACTER_CODE_MAP
				.Aggregate(temp, (current, ch) => current.Replace(ch, ""));
		}

		/// <summary>
		/// To Phone Number Format
		/// </summary>
		/// <param name="phoneNumber">Phone number</param>
		/// <returns>Phone number format</returns>
		/// <remarks>ex."09072481626"→"090-7248-1626"</remarks>
		public static string ToPhoneNumber(string phoneNumber)
		{
			if (string.IsNullOrEmpty(phoneNumber)) return phoneNumber;

			return string.Join("-", Regex.Replace(phoneNumber.Replace("-", string.Empty), @"(\d{0,6})(\d{0,4})(\d{1,4})", "$1-$2-$3", RegexOptions.RightToLeft)
				.Split('-').Where(x => string.IsNullOrEmpty(x) == false).ToList());
		}

		/// <summary>
		/// 指定エンコーディングで文字列の指定バイト長文字列取得
		/// </summary>
		/// <param name="value">バイト長を取得したい文字列</param>
		/// <param name="byteStartPosition">文字列取得の開始バイトインデックス</param>
		/// <param name="byteLength">指定バイト長</param>
		/// <param name="encoding">指定エンコーディング</param>
		/// <returns>指定エンコーディングで指定バイト長の文字列</returns>
		public static string GetWithSpecifiedByteLength(string value, int byteStartPosition, int byteLength, Encoding encoding)
		{
			var result = new StringBuilder();
			var currentLength = 0;
			var byteEndPostion = byteStartPosition + byteLength;
			foreach (var appendChar in value)
			{
				currentLength += GetByteLength(appendChar.ToString(), encoding);
				if (currentLength < byteStartPosition) continue;
				if (currentLength > byteEndPostion) break;
				result.Append(appendChar);
			}
			return result.ToString();
		}

		/// <summary>
		/// 文字列のバイト長取得
		/// </summary>
		/// <param name="value">バイト長を取得したい文字列</param>
		/// <param name="encoding">エンコーディング</param>
		/// <returns>バイト長</returns>
		public static int GetByteLength(string value, Encoding encoding)
		{
			return encoding.GetByteCount(value);
		}

		/// <summary>
		/// Unicode文字列へエンコード
		/// </summary>
		/// <param name="source">ソース</param>
		/// <returns>エンコード後文字列</returns>
		public static string EncodeToUnicodeString(string source)
		{
			var strings = EncodeToUnicodeStrings(source);
			var result = string.Join("", strings);
			return result;
		}
		/// <summary>
		/// Unicode文字列へエンコード
		/// </summary>
		/// <param name="source">ソース</param>
		/// <returns>エンコード後文字列</returns>
		public static string[] EncodeToUnicodeStrings(string source)
		{
			var result = source.Select(
				c =>
				{
					var bytes = Encoding.Unicode.GetBytes(c.ToString());
					var splitted = (BitConverter.ToString(bytes)).Split('-');
					return @"\u" + string.Join("", splitted.Reverse());
				}).ToArray();
			return result;
		}

		/// <summary>
		/// 住所部分一覧からｗ２の住所２～４に分割
		/// </summary>
		/// <param name="addressList">住所一覧（都道府県情報を除く）</param>
		/// <returns>ｗ２の住所２～４一覧（５０文字ごとに分ける）
		/// １５０文字を超える場合、nullに返却する
		/// </returns>
		public static string[] SplitAddress(params string[] addressList)
		{
			var address = StringUtility.ToZenkaku(string.Concat(addressList)).Trim();
			var len = address.Length;
			var resultAddress = new List<string>();
			if (len <= 50)
			{
				resultAddress.Add(address);			// 住所２
				resultAddress.Add(string.Empty);	// 住所３
				resultAddress.Add(string.Empty);	// 住所４
			}
			else if (len <= 50 * 2)
			{
				resultAddress.Add(address.Substring(0, 50));	// 住所２
				resultAddress.Add(address.Substring(50));		// 住所３
				resultAddress.Add(string.Empty);				// 住所４
			}
			else if (len <= 50 * 3)
			{
				resultAddress.Add(address.Substring(0, 50));	// 住所２
				resultAddress.Add(address.Substring(50, 50));	// 住所３
				resultAddress.Add(address.Substring(100));		// 住所４
			}
			else
			{
				return null;
			}
			return resultAddress.ToArray();
		}

		#region +SplitJapanZipCode 日本郵便番号文字列から前・後部分に分割
		/// <summary>
		/// 日本郵便番号文字列（ハイフンあり・なしも対応）から前・後部分に分割
		/// </summary>
		/// <param name="zipCode">日本郵便番号文字列（ハイフンあり・なしも対応）</param>
		/// <returns>日本郵便番号の前・後部分
		/// 7桁・8桁ではない場合、nullに返却する
		/// </returns>
		public static string[] SplitJapanZipCode(object zipCode)
		{
			var target = ToEmpty(zipCode).Trim();
			var resultZipCode = new List<string>();

			if (target.Length == 7)
			{
				// XXXYYYYフォーマットなので、7桁から3桁・4桁に分ける
				resultZipCode.Add(target.Substring(0, 3));
				resultZipCode.Add(target.Substring(3, 4));
			}
			else if (target.Length == 8)
			{
				// XXX-YYYYフォーマットなので、8桁から最初3桁・最後4桁に分ける
				resultZipCode.Add(target.Substring(0, 3));
				resultZipCode.Add(target.Substring(4, 4));
			}
			else
			{
				return null;
			}
			return resultZipCode.ToArray();
		}
		#endregion

		#region +IsAnotherShippingFlagValid 別出荷フラグをチェック
		/// <summary>
		/// 別出荷フラグをチェック
		/// </summary>
		/// <param name="ownerName1">注文者の氏名1</param>
		/// <param name="ownerName2">注文者の氏名2</param>
		/// <param name="ownerZip">注文者の郵便番号</param>
		/// <param name="ownerAddr1">注文者の住所1</param>
		/// <param name="ownerAddr2">注文者の住所2</param>
		/// <param name="ownerAddr3">注文者の住所3</param>
		/// <param name="ownerAddr4">注文者の住所4</param>
		/// <param name="ownerPhone">注文者の電話番号</param>
		/// <param name="shippingName1">配送先の氏名1</param>
		/// <param name="shippingName2">配送先の氏名2</param>
		/// <param name="shippingZip">配送先の郵便番号</param>
		/// <param name="shippingAddr1">配送先の住所1</param>
		/// <param name="shippingAddr2">配送先の住所2</param>
		/// <param name="shippingAddr3">配送先の住所3</param>
		/// <param name="shippingAddr4">配送先の住所4</param>
		/// <param name="shippingPhone">配送先の電話番号</param>
		/// <returns>true：別出荷有効（配送先が注文者と異なる）、false：別出荷無効（配送先が注文者と同じ）</returns>
		public static bool IsAnotherShippingFlagValid(
			string ownerName1,
			string ownerName2,
			string ownerZip,
			string ownerAddr1,
			string ownerAddr2,
			string ownerAddr3,
			string ownerAddr4,
			string ownerPhone,
			string shippingName1,
			string shippingName2,
			string shippingZip,
			string shippingAddr1,
			string shippingAddr2,
			string shippingAddr3,
			string shippingAddr4,
			string shippingPhone)
		{
			return ((ownerName1 != shippingName1)
				|| (ownerName2 != shippingName2)
				|| (ownerZip != shippingZip)
				|| (ownerAddr1 != shippingAddr1)
				|| (ownerAddr2 != shippingAddr2)
				|| (ownerAddr3 != shippingAddr3)
				|| (ownerAddr4 != shippingAddr4)
				|| (ownerPhone != shippingPhone));
		}
		#endregion

		/// <summary>
		/// 文字列を省略する
		/// </summary>
		/// <param name="value">指定文字列</param>
		/// <param name="displayLength">文字列数</param>
		/// <param name="ignoreCharacter">省略文字</param>
		/// <returns>省略文字列</returns>
		public static string AbbreviateString(string value, int displayLength, string ignoreCharacter = "...")
		{
			var result = ToEmpty(value);

			// 指定文字列より大きい場合
			if (result.Length > displayLength)
			{
				result = result.Substring(0, displayLength) + ignoreCharacter;
			}

			return result;
		}

		/// <summary>
		/// Get Byte Length String
		/// </summary>
		/// <param name="value">Value to get byte length</param>
		/// <param name="startByte">Start Byte</param>
		/// <param name="endByte">Length limit</param>
		/// <param name="encoding">Encoding</param>
		/// <returns>Value after get byte length</returns>
		public static string GetByteLengthString(string value, int startByte, int endByte, Encoding encoding)
		{
			var result = new StringBuilder();
			var byteLength = 0;
			foreach (char chAppend in value)
			{
				byteLength += GetByteLength(chAppend.ToString(), encoding);

				if (byteLength > endByte) break;

				if (byteLength >= startByte)
				{
					result.Append(chAppend);
				}
			}

			return result.ToString().Trim();
		}

		/// <summary>
		/// 文字列を圧縮する
		/// </summary>
		/// <param name="value">圧縮前の文字列</param>
		/// <returns>圧縮後文字列</returns>
		public static string CompressString(string value)
		{
			var buffer = Encoding.UTF8.GetBytes(value);
			using (var memoryStream = new MemoryStream())
			{
				using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Compress, true))
				{
					gZipStream.Write(buffer, 0, buffer.Length);
				}

				memoryStream.Position = 0;

				var compressedData = new byte[memoryStream.Length];
				memoryStream.Read(compressedData, 0, compressedData.Length);

				var gZipBuffer = new byte[compressedData.Length + 4];
				Buffer.BlockCopy(compressedData, 0, gZipBuffer, 4, compressedData.Length);
				Buffer.BlockCopy(BitConverter.GetBytes(buffer.Length), 0, gZipBuffer, 0, 4);

				var result = Convert.ToBase64String(gZipBuffer);
				return result;
			}
		}

		/// <summary>
		/// 文字列を解凍する
		/// </summary>
		/// <param name="compressedValue">圧縮した文字列</param>
		/// <returns>解凍後文字列</returns>
		public static string DecompressString(string compressedValue)
		{
			var gZipBuffer = Convert.FromBase64String(compressedValue);
			using (var memoryStream = new MemoryStream())
			{
				var dataLength = BitConverter.ToInt32(gZipBuffer, 0);
				memoryStream.Write(gZipBuffer, 4, gZipBuffer.Length - 4);

				var buffer = new byte[dataLength];

				memoryStream.Position = 0;
				using (var gZipStream = new GZipStream(memoryStream, CompressionMode.Decompress))
				{
					gZipStream.Read(buffer, 0, buffer.Length);
				}

				var result = Encoding.UTF8.GetString(buffer);
				return result;
			}
		}

		/// <summary>
		/// Replace delimiter
		/// </summary>
		/// <param name="value">Target character string</param>
		/// <returns>String without hyphen</returns>
		public static string ReplaceDelimiter(string value)
		{
			var result = value.Replace("-", string.Empty)
				.Replace("ｰ", string.Empty);
			return result;
		}

		/// <summary>
		/// SQL文字列エスケープ
		/// </summary>
		/// <param name="objSrc">対象</param>
		/// <returns>エスケープ済文字列</returns>
		public static string EscapeSqlString(object objSrc)
		{
			return ToEmpty(objSrc).Replace("'", "''");
		}

		/// <summary>
		/// NULLまたは空文字の場合に置換
		/// </summary>
		/// <param name="src">検査対象文字列</param>
		/// <param name="alt">NULLまたは空文字の場合に返す文字列</param>
		/// <returns>文字列</returns>
		public static string ToValueIfNullOrEmpty(string src, string alt)
		{
			return string.IsNullOrEmpty(src) ? alt : src;
		}

		/// <summary>
		/// 半角スペースをnbspに変換
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <returns>半角スペース変換後文字列</returns>
		public static string ChangeToNbsp(string objSrc)
		{
			if (objSrc != null)
			{
				// 半角スペースをnbspに変換
				return objSrc.Replace(" ", "&nbsp;");
			}

			return "";
		}

		/// <summary>
		/// タブを#009に変換
		/// </summary>
		/// <param name="objSrc">変換対象</param>
		/// <returns>タブ変換後文字列</returns>
		public static string ChangeToTab(string objSrc)
		{
			if (objSrc != null)
			{
				// タブを#009に変換
				return objSrc.Replace("\t", "&#009;");
			}

			return "";
		}

		/// <summary>
		/// To date format
		/// </summary>
		/// <param name="data">Object data</param>
		/// <param name="dateFormat">Date format</param>
		/// <returns>A date time string with an specified format</returns>
		public static string ToDateFormat(object data, string dateFormat = "yyyy/MM/dd HH:mm:ss")
		{
			var value = ToEmpty(data);
			if (string.IsNullOrEmpty(value)) return string.Empty;

			var isSuccess = DateTime.TryParse(value, out var dateTime);
			var result = isSuccess ? dateTime.ToString(dateFormat) : string.Empty;

			return result;
		}

		/// <summary>
		/// Truncate utf 8
		/// </summary>
		/// <param name="input">Input</param>
		/// <param name="maxLength">Max length</param>
		/// <returns>Truncated string by length</returns>
		public static string TruncateUtf8(string input, int maxLength)
		{
			var bytes = Encoding.UTF8.GetBytes(input);
			if (bytes.Length <= maxLength)
			{
				// 元の長さがmaxLength以下ならそのまま返す
				return input;
			}

			// maxLengthに収まるようにバイト配列を切り詰める
			var truncatedBytes = new byte[maxLength];
			Array.Copy(bytes, truncatedBytes, maxLength);

			// UTF-8文字の途中で切れないように調整
			var validLength = maxLength;
			while ((validLength > 0) && ((truncatedBytes[validLength - 1] & 0xC0) == 0x80))
			{
				validLength--;
			}

			// 正しいバイト数で文字列に戻す
			var truncatedString = Encoding.UTF8.GetString(truncatedBytes, 0, validLength);

			var result = string.Empty;
			for (var index = 0; index < input.Length; index++)
			{
				if (input[index] != truncatedString[index]) break;
				result += truncatedString[index];
			}
			return result;
		}
	}

	#region Class StringExtention 文字列型拡張クラス
	/// <summary>
	/// 文字列型拡張クラス
	/// </summary>
	public static class StringExtention
	{
		/// <summary>
		/// メール振り分け条件の "含むとき" を判定します。
		/// </summary>
		/// <param name="objectCompare">対象文字列</param>
		/// <param name="value">検索値</param>
		/// <param name="ignoreCase">大文字/小文字を無視するかどうか</param>
		/// <returns>検索値が対象文字列内に存在すればtrue</returns>
		public static bool Contains(this string objectCompare, string value, bool ignoreCase)
		{
			return ignoreCase ? objectCompare.ToLower().Contains(value.ToLower()) : objectCompare.Contains(value);
		}

		/// <summary>
		/// すべての空白を削除
		/// </summary>
		/// <param name="value">対象文字列</param>
		/// <returns>空白なしの文字列</returns>
		public static string TrimAllSpaces(this string value)
		{
			if (string.IsNullOrEmpty(value)) return value;
			var result = value.Replace(" ", "").Replace("　", "");
			return result;
		}
	}
	#endregion
}
