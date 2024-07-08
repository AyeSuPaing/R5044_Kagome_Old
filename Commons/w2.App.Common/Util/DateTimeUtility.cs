/*
=========================================================================================================
  Module      : 日付ユーティリティモジュール(DateTimeUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Globalization;
using System.Linq;
using System.Web.UI.WebControls;
using w2.App.Common.Global.Config;
using w2.App.Common.Global.Region;
using w2.Common.Util;

namespace w2.App.Common.Util
{
	///*********************************************************************************************
	/// <summary>
	/// 日付ユーティリティ
	/// </summary>
	///*********************************************************************************************
	public class DateTimeUtility
	{
		/// <summary>日付形式区分の定義</summary>
		public enum FormatType
		{
			/// <summary>日付形式：「ymd」のパターン</summary>
			General,
			/// <summary>日付形式：「yyyy/MM/dd」のパターン</summary>
			ShortDate2Letter,
			/// <summary>日付形式：「yyyy/MM/dd HH:mm」のパターン</summary>
			ShortDateHourMinute2Letter,
			/// <summary>日付形式：「yyyy/MM/dd HH:mm:ss」のパターン</summary>
			ShortDateHourMinuteSecond2Letter,
			/// <summary>日付形式：「yyyy/MM/dd(ddd)」のパターン</summary>
			ShortDateWeekOfDay2Letter,
			/// <summary>日付形式：「yyyy/M/d (ddd)」のパターン</summary>
			ShortDateWeekOfDay1Letter,
			/// <summary>日付形式：「yyyy年M月d日」のパターン</summary>
			LongDate1Letter,
			/// <summary>日付形式：「yyyy年M月d日 H時m分」のパターン</summary>
			LongDateHourMinute1Letter,
			/// <summary>日付形式：「yyyy年M月d日(ddd)」のパターン</summary>
			LongDateWeekOfDay1Letter,
			/// <summary>日付形式：「yyyy年MM月dd日(ddd)」のパターン</summary>
			LongDateWeekOfDay2Letter,
			/// <summary>日付形式：「MM/dd(ddd)」のパターン</summary>
			MonthDayWeekOfDay2Letter,
			/// <summary>日付形式：「H時m分s秒」のパターン</summary>
			HourMinuteSecond1Letter,
			/// <summary>日付形式：「yyyy年M月末」のパターン</summary>
			EndOfYearMonth1Letter,
			/// <summary>日付形式：「yyyy年MM月dd日」のパターン</summary>
			LongDate2Letter,
			/// <summary>日付形式：「yyyy/MM/dd」のパターン※サーバータイム付けない</summary>
			ShortDate2LetterNoneServerTime,
			/// <summary>日付形式：「yyyy年M月」のパターン</summary>
			LongYearMonth,
			/// <summary>日付形式：「yyyy年M月d日 H時m分s秒」のパターン</summary>
			LongDateHourMinuteSecond1Letter,
			/// <summary>日付形式：「yyyy年MM月dd日 HH時mm分」のパターン</summary>
			LongDateHourMinute2Letter,
			/// <summary>日付形式：「d(ddd)」のパターン</summary>
			DayWeekOfDay,
			/// <summary>日付形式：「M/d」のパターン</summary>
			ShortMonthDay,
			/// <summary>日付形式：「M月d日」のパターン</summary>
			LongMonthDay,
			/// <summary>日付形式：「yy/MM/dd」のパターン※サーバータイム付けない</summary>
			YearMonthDay2LetterNoneServerTimeForDatePicker,
			/// <summary>日付形式：「yyyy/MM/dd HH:mm」のパターン※サーバータイム付けない</summary>
			ShortDateHourMinuteNoneServerTime,
			/// <summary>日付形式：「yyyy/MM/dd HH:mm:ss」のパターン※サーバータイム付けない</summary>
			ShortDateHourMinuteSecondNoneServerTime,
			/// <summary>日付形式：「yyyy/MM」のパターン</summary>
			ShortYearMonth,
			/// <summary>日付形式：「yyyy年M月d日」のパターン※サーバータイム付けない</summary>
			LongDate1LetterNoneServerTime,
			/// <summary>日付形式：「yyyy年MM月dd日 HH時mm分ss秒」のパターン※サーバータイム付けない</summary>
			LongDateHourMinuteSecondNoneServerTime,
			/// <summary>日付形式：「yyyy年MM月dd日 HH時mm分」のパターン※サーバータイム付けない</summary>
			LongDateHourMinuteNoneServerTime,
			/// <summary>日付形式：「yyyy/MM/dd HH:mm:ss.fff」のパターン※サーバータイム付けない</summary>
			LongFullDateTimeNoneServerTime,
		}

		/// <summary>
		/// 年チェック
		/// </summary>
		/// <param name="objYear">年</param>
		/// <returns>
		/// 正しい年　　：true
		/// 正しくない年：false
		/// </returns>
		public static bool IsYear(object objYear)
		{
			return Validator.IsDate(objYear + "/01/01");
		}

		/// <summary>
		/// 月チェック
		/// </summary>
		/// <param name="objMonth">月</param>
		/// <returns>
		/// 正しい月　　：true
		/// 正しくない月：false
		/// </returns>
		public static bool IsMonth(object objMonth)
		{
			return Validator.IsDate("1900/" + objMonth + "/01");
		}

		/// <summary>
		/// 末日補正チェック
		/// </summary>
		/// <param name="objYear">年</param>
		/// <param name="objMonth">月</param>
		/// <param name="objDay">日</param>
		/// <returns>
		/// 「年」、「月」指定が正しい AND「日」指定が正しくない：true
		/// 上記以外：false
		/// </returns>
		public static bool IsLastDayOfMonth(object objYear, object objMonth, object objDay)
		{
			// 「年」、「月」指定が正しい AND「日」指定が正しくない
			return (IsYear(objYear))
				&& (IsMonth(objMonth))
				&& (Validator.IsDate(objYear + "/" + objMonth + "/" + objDay) == false);
		}

		/// <summary>
		/// 末日取得
		/// </summary>
		/// <param name="objYear">年</param>
		/// <param name="objMonth">月</param>
		/// <returns>末日</returns>
		public static int GetLastDayOfMonth(object objYear, object objMonth)
		{
			return DateTime.DaysInMonth(int.Parse(objYear.ToString()), int.Parse(objMonth.ToString()));
		}

		/// <summary>
		/// 「年」の値を選択
		/// </summary>
		/// <param name="ddl">ドロップダウンリスト</param>
		/// <param name="format">日付表示フォーマット</param>
		public static void SetYear(DropDownList ddl, string format = "")
		{
			ddl.Items.Cast<ListItem>().ToList().ForEach(item => item.Selected = (item.Value == DateTime.Now.Year.ToString()));
		}

		/// <summary>
		/// 「月」の値を選択
		/// </summary>
		/// <param name="ddl">ドロップダウンリスト</param>
		/// <param name="format">日付表示フォーマット</param>
		public static void SetMonth(DropDownList ddl, string format = "")
		{
			ddl.Items.Cast<ListItem>().ToList().ForEach(item => item.Selected = (item.Value == DateTime.Now.Month.ToString("00")));
		}

		/// <summary>
		/// 「日」の値を選択
		/// </summary>
		/// <param name="ddl">ドロップダウンリスト</param>
		/// <param name="format">日付表示フォーマット</param>
		public static void SetDay(DropDownList ddl, string format = "")
		{
			ddl.Items.Cast<ListItem>().ToList().ForEach(item => item.Selected = (item.Value == DateTime.Now.Day.ToString("00")));
		}

		/// <summary>
		/// 引数で年数を指定してリスト取得
		/// </summary>
		/// <param name="begin">開始年</param>
		/// <param name="end">終了年</param>
		/// <param name="format">フォーマット</param>
		/// <returns>begin ～ end 年のリストを取得</returns>
		public static ListItem[] GetYearListItem(int begin, int end, string format = "yyyy")
		{
			List<ListItem> year = new List<ListItem>();

			for (int i = begin; i <= end; i++)
			{
				DateTime date = new DateTime(i, 1, 1);
				year.Add(new ListItem(date.ToString(format)));
			}
			// 降順が指定されていたら並び替える
			if (Constants.YEAR_LIST_ITEM_ORDER.Equals(Constants.YearListItemOrder.DESC))
			{
				year.Reverse();
			}

			return year.ToArray();
		}

		/// <summary>
		/// 2000年～来年のリスト取得
		/// </summary>
		/// <returns>2000年から来年までのリストアイテム</returns>
		public static ListItem[] GetBackwardYearListItem()
		{
			return GetYearListItem(2000, DateTime.Now.Year + 1);
		}

		/// <summary>
		/// 2000年～10年後のリスト取得
		/// </summary>
		/// <returns>2000年から今年+10年後までのリストアイテム</returns>
		public static ListItem[] GetShortRangeYearListItem()
		{
			return GetYearListItem(2000, DateTime.Now.Year + 10);
		}

		/// <summary>
		/// 2000年から2100年のリスト取得
		/// </summary>
		/// <returns>2000年から2100年までのリストアイテム</returns>
		public static ListItem[] GetLongRangeYearListItem()
		{
			return GetYearListItem(2000, 2100);
		}

		/// <summary>
		/// 生年月日用の年リスト取得
		/// </summary>
		/// <returns>生年月日用の年リスト</returns>
		/// <remarks>今年から1900年までのリストアイテム</remarks>
		public static ListItem[] GetBirthYearListItem()
		{
			var year = GetYearListItem(1900, DateTime.Now.Year);

			// 生年月日の年は、常に降順にする。
			if (Constants.YEAR_LIST_ITEM_ORDER.Equals(Constants.YearListItemOrder.ASC))
			{
				return year.Reverse().ToArray();
			}

			return year;
		}

		/// <summary>
		/// 去年から来年までのリストアイテム取得
		/// </summary>
		/// <returns>去年から来年までのリストアイテム</returns>
		public static ListItem[] GetMallMaintenanceYearListItem()
		{
			return GetYearListItem(DateTime.Now.Year - 1, DateTime.Now.Year + 1);
		}

		/// <summary>
		/// 今年から来年までのリストアイテム取得
		/// </summary>
		/// <returns>今年から来年までのリストアイテム</returns>
		public static ListItem[] GetScheduleYearListItem()
		{
			return GetYearListItem(DateTime.Now.Year, DateTime.Now.Year + 1);
		}

		/// <summary>
		/// クレジットカード用の年リスト取得
		/// </summary>
		/// <param name="format">フォーマット</param>
		/// <param name="years">年数</param>
		/// <returns>クレジットカード用の年リスト</returns>
		/// <remarks>今年から10年後の年数（下2桁）</remarks>
		public static ListItem[] GetCreditYearListItem(string format = "yy", int years = 10)
		{
			var year = GetYearListItem(DateTime.Now.Year, DateTime.Now.Year + years, format);
			// クレジットカード用の年は、常に昇順にする。
			if (Constants.YEAR_LIST_ITEM_ORDER.Equals(Constants.YearListItemOrder.DESC))
			{
				return year.Reverse().ToArray();
			}
			return year;
		}

		/// <summary>
		/// 月のリストアイテム取得
		/// </summary>
		/// <param name="textFormat">表示のフォーマット</param>
		/// <param name="valueFormat">値のフォーマット</param>
		/// <returns>1月から12月までのリストアイテム</returns>
		public static ListItem[] GetMonthListItem(string textFormat = "", string valueFormat = "")
		{
			List<ListItem> month = new List<ListItem>();

			for (int i = 1; i <= 12; i++)
			{
				month.Add(new ListItem(i.ToString(textFormat), string.IsNullOrEmpty(valueFormat) ? null : i.ToString(valueFormat)));
			}
			return month.ToArray();
		}

		/// <summary>
		/// クレジットカード月取得
		/// </summary>
		/// <returns>1月から12月までのリストアイテム</returns>
		public static ListItem[] GetCreditMonthListItem()
		{
			return GetMonthListItem("D2");
		}

		/// <summary>
		/// 日のリストアイテムを取得
		/// </summary>
		/// <param name="textFormat">表示のフォーマット</param>
		/// <param name="valueFormat">値のフォーマット</param>
		/// <returns>日のリストアイテム</returns>
		public static ListItem[] GetDayListItem(string textFormat = "", string valueFormat = "")
		{
			List<ListItem> day = new List<ListItem>();

			for (int i = 1; i <= 31; i++)
			{
				day.Add(new ListItem(i.ToString(textFormat), string.IsNullOrEmpty(valueFormat) ? null : i.ToString(valueFormat)));
			}
			return day.ToArray();
		}

		/// <summary>
		/// 時間のリストアイテムを取得
		/// </summary>
		/// <param name="format">フォーマット</param>
		/// <returns>時間のリストアイテム</returns>
		public static ListItem[] GetHourListItem(string format = "")
		{
			List<ListItem> hour = new List<ListItem>();

			for (int i = 0; i <= 23; i++)
			{
				hour.Add(new ListItem(i.ToString(format)));
			}
			return hour.ToArray();
		}

		/// <summary>
		/// 分のリストアイテムを取得
		/// </summary>
		/// <param name="format">フォーマット</param>
		/// <returns>分のリストアイテム</returns>
		public static ListItem[] GetMinuteListItem(string format = "")
		{
			List<ListItem> minute = new List<ListItem>();

			for (int i = 0; i <= 59; i++)
			{
				minute.Add(new ListItem(i.ToString(format)));
			}
			return minute.ToArray();
		}

		/// <summary>
		/// 秒のリストアイテムを取得
		/// </summary>
		/// <param name="format">フォーマット</param>
		/// <returns>秒のリストアイテム</returns>
		public static ListItem[] GetSecondListItem(string format = "")
		{
			List<ListItem> second = new List<ListItem>();

			for (int i = 0; i <= 59; i++)
			{
				second.Add(new ListItem(i.ToString(format)));
			}
			return second.ToArray();
		}

		/// <summary>
		/// ユーザー年月日フォルト値取得
		/// </summary>
		/// <returns>ユーザーのデフォルト年月日</returns>
		public static DateTime? GetDefaultSettingBirthday()
		{
			DateTime defaultBirthday;
			if ((DateTime.TryParseExact(
					Constants.TAG_REPLACER_DATA_SCHEMA.GetValue("@@User.birth.default@@"),
					"yyyyMMdd",
					null,
					DateTimeStyles.None,
					out defaultBirthday))
				&& (GetBirthYearListItem().Any(item => item.Value.Equals(defaultBirthday.Year.ToString())))
				&& (defaultBirthday <= DateTime.Today))
			{
				return defaultBirthday;
			}

			return null;
		}

		/// <summary>
		/// 日付が月の第何週目かを取得
		/// </summary>
		/// <param name="date">日付</param>
		/// <returns>何週目</returns>
		public static int GetWeekNum(DateTime date)
		{
			var firstDay = new DateTime(date.Year, date.Month, 1);
			var firstDayOfWeek = (int)(firstDay.DayOfWeek);
			var weekNum = (date.Day + firstDayOfWeek - 1) / 7 + 1;
			return weekNum;
		}

		/// <summary>
		/// 日付文字へ変換（リージョンに保持している言語ロケールコードによる）
		/// </summary>
		/// <param name="value">変換対象(日付文字列or日付型）</param>
		/// <param name="formatType">日付形式区分</param>
		/// <param name="defaultValue">デフォルト値</param>
		/// <returns>変換後文字列</returns>
		public static string ToStringFromRegion(object value, FormatType formatType, string defaultValue = "")
		{
			// グローバルOPが無効またはクッキーから言語ロケールIDを取得できない場合は空をセット
			var localeId = ((Constants.GLOBAL_OPTION_ENABLE == false) || RegionManager.GetInstance().Region == null)
				? string.Empty
				: RegionManager.GetInstance().Region.LanguageLocaleId;

			return ToString(value, formatType, localeId, defaultValue);
		}

		/// <summary>
		/// 日付文字へ変換（管理側向け）
		/// </summary>
		/// <param name="value">変換対象(日付文字列or日付型）</param>
		/// <param name="formatType">日付形式区分</param>
		/// <param name="defaultValue">デフォルト値</param>
		/// <returns>変換後文字列</returns>
		public static string ToStringForManager(object value, FormatType formatType, string defaultValue = "")
		{
			return ToString(value, formatType, Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE, defaultValue);
		}

		/// <summary>
		/// 日付文字へ変換
		/// </summary>
		/// <param name="value">変換対象(日付文字列or日付型）</param>
		/// <param name="formatType">日付形式区分</param>
		/// <param name="localeId">言語ロケールID</param>
		/// <param name="defaultValue">デフォルト値</param>
		/// <returns>変換後文字列</returns>
		public static string ToString(object value, FormatType formatType, string localeId, string defaultValue = "")
		{
			// グローバル対応なしの場合、無理やり日本日付形式で表示のため、言語ロケールコードを空にセット
			if (Constants.GLOBAL_OPTION_ENABLE == false) localeId = string.Empty;

			// 設定ファイルから日付フォーマットを取得
			var dateFormat = GlobalConfigUtil.GetDateTimeFormatText(localeId, formatType);
			// 変換対象は日付の場合、文字列への変換を行う
			if ((value != DBNull.Value) && (value is DateTime))
			{
				return ToDateString((DateTime)value, dateFormat, localeId);
			}
			// 変換対象は文字列でない場合、デフォールト値を戻す
			if ((value is string) == false) return defaultValue;
			DateTime date;
			var result = DateTime.TryParse((string)value, out date)
				? ToDateString(date, dateFormat, localeId)
				: defaultValue;
			return result;
		}

		/// <summary>
		/// 日付文字へ変換
		/// </summary>
		/// <param name="value">変換対象(日付型）</param>
		/// <param name="dateFormatText">日付フォーマット文字列</param>
		/// <param name="localeId">言語ロケールID</param>
		/// <returns>変換後文字列</returns>
		internal static string ToDateString(DateTime value, string dateFormatText, string localeId)
		{
			// 言語ロケールIDは空の場合、現行のカルチャーで日付を変換
			// 日付がデフォルト値("0001/01/01 00:00:00")の場合はカルチャーによる日付フォーマット変換は行わない
			var result = (string.IsNullOrEmpty(localeId) || (value.CompareTo(new DateTime()) == 0))
				? value.ToString(dateFormatText)
				: value.ToString(dateFormatText, new CultureInfo(localeId));
			return result;
		}

		/// <summary>
		/// 日付形式IDへの変換
		/// </summary>
		/// <param name="type">日付形式区分</param>
		/// <returns>日付形式ID</returns>
		public static string ConvertFormatType(FormatType type)
		{
			switch (type)
			{
				// 日付形式：「ymd」のパターン
				case FormatType.General:
					return "f0_general";

				//日付形式：「yyyy/MM/dd」のパターン
				case FormatType.ShortDate2Letter:
					return "f1_short_yyyyMMdd";

				// 日付形式：「yyyy/MM/dd HH:mm」のパターン
				case FormatType.ShortDateHourMinute2Letter:
					return "f2_short_yyyyMMdd_HHmm";

				// 日付形式：「yyyy/MM/dd HH:mm:ss」のパターン
				case FormatType.ShortDateHourMinuteSecond2Letter:
					return "f3_short_yyyyMMdd_HHmmss";

				// 日付形式：「yyyy/MM/dd(ddd)」のパターン
				case FormatType.ShortDateWeekOfDay2Letter:
					return "f4_short_yyyyMMdd(d)";

				// 日付形式：「yyyy/M/d (ddd)」のパターン
				case FormatType.ShortDateWeekOfDay1Letter:
					return "f5_short_yyyyMd(d)";

				// 日付形式：「yyyy年M月d日」のパターン
				case FormatType.LongDate1Letter:
					return "f6_long_yyyyMd";

				// 日付形式：「yyyy年M月d日 H時m分」のパターン
				case FormatType.LongDateHourMinute1Letter:
					return "f7_long_yyyyMd_Hm";

				// 日付形式：「yyyy年M月d日(ddd)」のパターン
				case FormatType.LongDateWeekOfDay1Letter:
					return "f8_long_yyyyMd(d)";

				// 日付形式：「yyyy年MM月dd日(ddd)」のパターン
				case FormatType.LongDateWeekOfDay2Letter:
					return "f9_long_yyyyMMdd(d)";

				// 日付形式：「MM/dd(ddd)」のパターン
				case FormatType.MonthDayWeekOfDay2Letter:
					return "f10_MMdd(d)";

				// 日付形式：「H時m分s秒」のパターン
				case FormatType.HourMinuteSecond1Letter:
					return "f11_Hms";

				// 日付形式：「yyyy年M月末」のパターン
				case FormatType.EndOfYearMonth1Letter:
					return "f12_end_of_yyyyM";

				// 日付形式：「yyyy年MM月dd日」のパターン
				case FormatType.LongDate2Letter:
					return "f13_long_yyyyMMdd";

				// 日付形式：「yyyy/MM/dd」のパターン※サーバータイム付けない
				case FormatType.ShortDate2LetterNoneServerTime:
					return "f14_short_yyyyMMdd_none_server_time";

				// 日付形式：「yyyy年M月」のパターン
				case FormatType.LongYearMonth:
					return "f15_long_yyyyM";

				// 日付形式：「yyyy年M月d日 H時m分s秒」のパターン
				case FormatType.LongDateHourMinuteSecond1Letter:
					return "f16_long_yyyyMd_Hms";

				// 日付形式：「yyyy年MM月dd日 HH時mm分」のパターン
				case FormatType.LongDateHourMinute2Letter:
					return "f17_long_yyyyMMdd_HHmm";

				// 日付形式：「d(ddd)」のパターン
				case FormatType.DayWeekOfDay:
					return "f18_d(d)";

				// 日付形式：「M/d」のパターン
				case FormatType.ShortMonthDay:
					return "f19_short_Md";

				// 日付形式：「M月d日」のパターン
				case FormatType.LongMonthDay:
					return "f20_long_Md";

				// 日付形式：「yy/MM/dd」のパターン※サーバータイム付けない
				case FormatType.YearMonthDay2LetterNoneServerTimeForDatePicker:
					return "f21_yyMMdd_none_server_time_for_datepicker";

				// 日付形式：「yyyy/MM/dd HH:mm」のパターン※サーバータイム付けない
				case FormatType.ShortDateHourMinuteNoneServerTime:
					return "f22_short_yyyyMMdd_HHmm_none_server_time";

				// 日付形式：「yyyy/MM/dd HH:mm:ss」のパターン※サーバータイム付けない
				case FormatType.ShortDateHourMinuteSecondNoneServerTime:
					return "f23_short_yyyyMMdd_HHmmss_none_server_time";

				// 日付形式：「yyyy/MM」のパターン
				case FormatType.ShortYearMonth:
					return "f24_yyyyMM";

				// 日付形式：「yyyy年M月d日」のパターン※サーバータイム付けない
				case FormatType.LongDate1LetterNoneServerTime:
					return "f25_long_yyyyMd_none_server_time";

				// 日付形式：「yyyy年MM月dd日 HH時mm分ss秒」のパターン※サーバータイム付けない
				case FormatType.LongDateHourMinuteSecondNoneServerTime:
					return "f26_long_yyyyMMdd_HHmmss_none_server_time";

				// 日付形式：「yyyy年MM月dd日 HH時mm分」のパターン※サーバータイム付けない
				case FormatType.LongDateHourMinuteNoneServerTime:
					return "f27_long_yyyyMMdd_HHmm_none_server_time";

				// 日付形式：「yyyy/MM/dd HH:mm:ss.fff」のパターン※サーバータイム付けない
				case FormatType.LongFullDateTimeNoneServerTime:
					return "f28_long_yyyyMMdd_HHmmssfff_none_server_time";
			}
			// 形式区分定義されない場合にはエラーが出ないようにデフォールト値を返す
			return "f1_short_yyyyMMdd";
		}

		/// <summary>
		/// 3ヶ月前まで選択肢取得
		/// </summary>
		/// <returns>3ヶ月前まで選択肢</returns>
		public static ListItem[] DateTermUpTo3Month()
		{
			return ValueText.GetValueItemArray("DateSelectList", "DateTermUpTo3Month");
		}

		/// <summary>
		/// 末日補正処理
		/// </summary>
		/// <param name="year">年</param>
		/// <param name="month">月</param>
		/// <param name="day">日</param>
		/// <returns>補正後の日</returns>
		public static string CorrectLastDayOfMonth(string year, string month, string day)
		{
			var correctedDay = IsLastDayOfMonth(year, month, day)
				? GetLastDayOfMonth(year, month).ToString()
				: day;
			return correctedDay;
		}

		/// <summary>
		/// SqlDateTime相当の日時範囲か
		/// </summary>
		/// <param name="beginDateTime">開始日時</param>
		/// <param name="endDateTime">終了日時</param>
		/// <returns>結果</returns>
		public static bool IsValidSqlDateTimeRange(DateTime beginDateTime, DateTime endDateTime)
		{
			return (IsValidSqlDateTimeTerm(beginDateTime)
				&& IsValidSqlDateTimeTerm(endDateTime)
				&& (beginDateTime <= endDateTime));
		}

		/// <summary>
		/// SqlDateTime範囲内の日時か
		/// </summary>
		/// <param name="dateTime">チェック対象</param>
		/// <returns>結果</returns>
		public static bool IsValidSqlDateTimeTerm(DateTime dateTime)
		{
			return (((DateTime)SqlDateTime.MinValue <= dateTime)
				&& (dateTime <= (DateTime)SqlDateTime.MaxValue));
		}

		/// <summary>
		/// Get begin time of day
		/// </summary>
		/// <param name="value">The date time that need to get begin time</param>
		/// <returns>A date begin time</returns>
		public static DateTime GetBeginTimeOfDay(DateTime value)
		{
			var result = new DateTime(value.Year, value.Month, value.Day);
			return result;
		}

		/// <summary>
		/// Get end time of day
		/// </summary>
		/// <param name="value">The date time that need to get end time</param>
		/// <returns>A date end time</returns>
		public static DateTime GetEndTimeOfDay(DateTime value)
		{
			var result = new DateTime(value.Year, value.Month, value.Day, 23, 59, 59);
			return result;
		}

		/// <summary>
		/// Get first date of current month
		/// </summary>
		/// <returns>First date of current month</returns>
		public static DateTime GetFirstDateOfCurrentMonth()
		{
			var today = DateTime.Today;
			var result = GetFirstDateOfMonth(today.Year, today.Month);
			return result;
		}

		/// <summary>
		/// Get first date of last month
		/// </summary>
		/// <returns>First date of last month</returns>
		public static DateTime GetFirstDateOfLastMonth()
		{
			var lastMonthDate = DateTime.Today.AddMonths(-1);
			var result = GetFirstDateOfMonth(lastMonthDate.Year, lastMonthDate.Month);
			return result;
		}

		/// <summary>
		/// Get first date of next month
		/// </summary>
		/// <returns>First date of current month</returns>
		public static DateTime GetFirstDateOfNextMonth()
		{
			var nextMonthDate = DateTime.Today.AddMonths(1);
			var result = GetFirstDateOfMonth(nextMonthDate.Year, nextMonthDate.Month);
			return result;
		}

		/// <summary>
		/// Get last date of current month
		/// </summary>
		/// <returns>Last date of current month</returns>
		public static DateTime GetLastDateOfCurrentMonth()
		{
			var result = GetLastDateOfMonth(DateTime.Today);
			return result;
		}

		/// <summary>
		/// Get last date of last month
		/// </summary>
		/// <returns>Last date of last month</returns>
		public static DateTime GetLastDateOfLastMonth()
		{
			var now = DateTime.Today.AddMonths(-1);
			var result = GetLastDateOfMonth(now);
			return result;
		}

		/// <summary>
		/// Get first date of current year
		/// </summary>
		/// <returns>First date of current year</returns>
		public static DateTime GetFirstDateOfCurrentYear()
		{
			var result = new DateTime(DateTime.Today.Year, 1, 1);
			return result;
		}

		/// <summary>
		/// Get date past by day
		/// </summary>
		/// <param name="numberOfDays">Number of days</param>
		/// <returns>Date past</returns>
		public static DateTime GetDatePastByDay(int numberOfDays)
		{
			var result = DateTime.Today.AddDays(-1 * numberOfDays);
			return result;
		}

		/// <summary>
		/// Get first date of month
		/// </summary>
		/// <param name="year">The year</param>
		/// <param name="month">The month</param>
		/// <returns>First date of month</returns>
		public static DateTime GetFirstDateOfMonth(int year, int month)
		{
			var result = new DateTime(year, month, 1);
			return result;
		}

		/// <summary>
		/// Get last date of month
		/// </summary>
		/// <param name="date">Date</param>
		/// <returns>Last date of month</returns>
		public static DateTime GetLastDateOfMonth(DateTime date)
		{
			var result = new DateTime(
				date.Year,
				date.Month,
				DateTime.DaysInMonth(date.Year, date.Month));
			return result;
		}

		/// <summary>
		/// Get last date of current year
		/// </summary>
		/// <returns>Last date of current year</returns>
		public static DateTime GetLastDateOfCurrentYear()
		{
			var result = new DateTime(DateTime.Today.Year, 12, 31);
			return result;
		}

		/// <summary>
		/// Get first date of next year
		/// </summary>
		/// <returns>Last date of current year</returns>
		public static DateTime GetFirstDateOfNextYear()
		{
			var result = new DateTime(DateTime.Today.AddYears(1).Year, 1, 1);
			return result;
		}

		/// <summary>
		/// Get display date string
		/// </summary>
		/// <param name="date">A date</param>
		/// <param name="format">A date format string</param>
		/// <returns>The date is of type string</returns>
		public static string GetDisplayDateString(
			DateTime date,
			string format = "yyyy/MM/dd")
		{
			return date.ToString(format);
		}

		/// <summary>
		/// Get start date this month string
		/// </summary>
		/// <returns>The date is of type string</returns>
		public static string GetStartDateThisMonthString()
		{
			var now = DateTime.Now;
			var date = new DateTime(now.Year, now.Month, 1);
			return GetDisplayDateString(date);
		}

		/// <summary>
		/// Get end date this month string
		/// </summary>
		/// <returns>The date is of type string</returns>
		public static string GetEndDateThisMonthString()
		{
			var now = DateTime.Now;
			var date = new DateTime(now.Year, now.Month, DateTime.DaysInMonth(now.Year, now.Month));
			return GetDisplayDateString(date);
		}
	}
}
