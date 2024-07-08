/*
=========================================================================================================
  Module      : 定期購入配送日計算のためのヘルパクラス (FixedPurchaseShippingDateCalculator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Domain.Order;

namespace w2.Domain.FixedPurchase.Helper
{
	#region +列挙体
	/// <summary>定期購入：次回配送日計算モード</summary>
	public enum NextShippingCalculationMode
	{
		/// <summary>月1配送（次回配送日は、基準日の翌月）</summary>
		Monthly,
		/// <summary>N週間隔配送（次回配送日は、基準日のN週後）</summary>
		EveryNWeek,
		/// <summary>最短配送（次回配送日は、基準日と同月でも可）</summary>
		Earliest,
	}
	#endregion

	/// <summary>
	/// 定期購入配送日計算のためのヘルパクラス
	/// </summary>
	/// <remarks>本クラスはinternalであり、外部への公開はServiceクラスを介す</remarks>
	public class FixedPurchaseShippingDateCalculator
	{
		#region +メソッド
		/// <summary>
		/// 配送希望日と配送所要日数を元に、初回配送予定日を計算します。
		/// </summary>
		/// <param name="shippingDate">配送希望日</param>
		/// <param name="daysRequired">配送所要日数</param>
		/// <returns>初回配送予定日</returns>
		public DateTime CalculateFirstShippingDate(DateTime? shippingDate, int daysRequired)
		{
			return shippingDate ?? DateTime.Today.AddDays(daysRequired);
		}

		/// <summary>
		/// 指定された定期購入配送パターンを元に、次回配送日を計算します。
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="shippingDate">配送希望日</param>
		/// <param name="daysRequired">配送所要日数</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="calculationMode">次回配送日計算モード</param>
		/// <returns>次回配送日</returns>
		internal DateTime CalculateNextShippingDate(string fpKbn, string fpSetting, DateTime? shippingDate, int daysRequired, int minSpan, NextShippingCalculationMode calculationMode)
		{
			var firstShippingDate = CalculateFirstShippingDate(shippingDate, daysRequired);
			return CalculateFollowingShippingDate(fpKbn, fpSetting, firstShippingDate, minSpan, calculationMode);
		}

		/// <summary>
		/// 指定された定期購入配送パターンを元に、次々回配送日を計算します。
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="shippingDate">配送希望日</param>
		/// <param name="daysRequired">配送所要日数</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="calculationMode">次回配送日計算モード</param>
		/// <returns>次々回配送日</returns>
		internal DateTime CalculateNextNextShippingDate(string fpKbn, string fpSetting, DateTime? shippingDate, int daysRequired, int minSpan, NextShippingCalculationMode calculationMode)
		{
			var nextShippingDate = CalculateNextShippingDate(fpKbn, fpSetting, shippingDate, daysRequired, minSpan, calculationMode);
			return CalculateFollowingShippingDate(fpKbn, fpSetting, nextShippingDate, minSpan, calculationMode);
		}

		/// <summary>
		/// 指定された基準日を起点とし、次サイクルの配送日を計算して返却します。
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="baseDate">基準日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="calculationMode">配送日計算モード</param>
		/// <returns>次サイクルの配送日</returns>
		public DateTime CalculateFollowingShippingDate(string fpKbn, string fpSetting, DateTime baseDate, int minSpan, NextShippingCalculationMode calculationMode)
		{
			switch (fpKbn)
			{
				// 月間隔日付指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					var monthAndDays = fpSetting.Split(',');
					return CalculateOnMonthlyDate(int.Parse(monthAndDays[0]), int.Parse(monthAndDays[1]), baseDate, minSpan, calculationMode);

				// 月間隔、週、曜日指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					var monthWeekAndDays = fpSetting.Split(',');
					return CalculateOnWeekAndDay(
						int.Parse(monthWeekAndDays[1]),
						int.Parse(monthWeekAndDays[2]),
						baseDate,
						minSpan,
						calculationMode,
						int.Parse(monthWeekAndDays[0]));

				// 配送間隔指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS:
					return CalculateOnDateInterval(int.Parse(fpSetting), baseDate, minSpan);

				// 週間隔・曜日指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY:
					var weekAndDays = fpSetting.Split(',');
					var nextDeliveryDate = CalculateOnEveryNWeekAndDay(
						int.Parse(weekAndDays[0]),
						int.Parse(weekAndDays[1]),
						baseDate,
						minSpan,
						calculationMode);
					return nextDeliveryDate;

				// 存在しない区分であれば基準日を返す
				default: return baseDate;
			}
		}

		/// <summary>
		/// Calculate first shipping date
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="baseDate">基準日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="calculationMode">配送日計算モード</param>
		/// <returns>次サイクルの配送日</returns>
		public DateTime CalculateFirstShippingDate(
			string fpKbn,
			string fpSetting,
			DateTime baseDate,
			int minSpan,
			NextShippingCalculationMode calculationMode)
		{
			switch (fpKbn)
			{
				// 月間隔日付指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					var monthAndDays = fpSetting.Split(',');
					return CalculateOnMonthlyDate(
						int.Parse(monthAndDays[0]),
						int.Parse(monthAndDays[1]),
						baseDate,
						minSpan,
						calculationMode,
						false);

				// 月間隔、週、曜日指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					var monthWeekAndDays = fpSetting.Split(',');
					return CalculateOnWeekAndDay(
						int.Parse(monthWeekAndDays[1]),
						int.Parse(monthWeekAndDays[2]),
						baseDate,
						minSpan,
						calculationMode,
						int.Parse(monthWeekAndDays[0]),
						false);

				// 存在しない区分であれば基準日を返す
				default: return baseDate;
			}
		}

		/// <summary>
		/// 指定された基準日を起点とし、次サイクルの配送日を計算して返却します。 [毎月日付指定]
		/// </summary>
		/// <param name="intervalMonth">月間隔</param>
		/// <param name="dayOfMonth">日付</param>
		/// <param name="baseDate">基準日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="calculationMode">配送日計算モード</param>
		/// <param name="useMinSpan">Use mininum shipping span</param>
		/// <returns>次サイクルの配送日</returns>
		private DateTime CalculateOnMonthlyDate(
			int intervalMonth,
			int dayOfMonth,
			DateTime baseDate,
			int minSpan,
			NextShippingCalculationMode calculationMode,
			bool useMinSpan = true)
		{
			var result = baseDate;

			// 配送日計算モードに応じて翌X月にずらす
			if (calculationMode == NextShippingCalculationMode.Monthly) result = result.AddMonths(intervalMonth);

			// 月末指定かどうかに応じて日付を組み立てる
			var isLastDay = (dayOfMonth == int.Parse(Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1_DAY_LASTDAY));
			result = new DateTime(result.Year, result.Month, isLastDay ? DateTime.DaysInMonth(result.Year, result.Month) : dayOfMonth);

			if (useMinSpan)
			{
				// 最低配送間隔以上あいてなければ翌X月にずらす
				while (result < baseDate.AddDays(Math.Max(minSpan, 1)))
				{
					result = result.AddMonths(intervalMonth);
					result = new DateTime(result.Year, result.Month, isLastDay ? DateTime.DaysInMonth(result.Year, result.Month) : dayOfMonth);
				}
			}

			return result;
		}

		/// <summary>
		/// 指定された基準日を起点とし、次サイクルの配送日を計算して返却します。 [週・曜日指定]
		/// </summary>
		/// <param name="weekNo">週番号(1-4)</param>
		/// <param name="weekDayNo">曜日番号(0-6)</param>
		/// <param name="baseDate">基準日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="calculationMode">配送日計算モード</param>
		/// <param name="intervalMonth">月間隔</param>
		/// <param name="useMinSpan">Use mininum shipping span</param>
		/// <returns>次サイクルの配送日</returns>
		private DateTime CalculateOnWeekAndDay(
			int weekNo,
			int weekDayNo,
			DateTime baseDate,
			int minSpan,
			NextShippingCalculationMode calculationMode,
			int intervalMonth,
			bool useMinSpan = true)
		{
			var result = baseDate;

			// 配送日計算モードに応じて間隔分月にずらす
			if (calculationMode == NextShippingCalculationMode.Monthly) result = result.AddMonths(intervalMonth);

			// 指定された週・曜日を元に日付を組み立てる
			result = GetDate(result.Year, result.Month, weekNo, weekDayNo);

			if (useMinSpan)
			{
				// 最低配送間隔以上あいてなければ翌X月にずらす
				while (result < baseDate.AddDays(Math.Max(minSpan, 1)))
				{
					result = result.AddMonths(intervalMonth);
					result = GetDate(result.Year, result.Month, weekNo, weekDayNo);
				}
			}

			return result;
		}

		/// <summary>
		/// 指定された基準日を起点とし、次サイクルの配送日を計算して返却します。 [週間隔・曜日指定]
		/// </summary>
		/// <param name="weekSpan">週間隔</param>
		/// <param name="weekDayNo">曜日番号</param>
		/// <param name="baseDate">基準日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="calculationMode">配送日計算モード</param>
		/// <returns>次サイクルの配送日</returns>
		private DateTime CalculateOnEveryNWeekAndDay(
			int weekSpan,
			int weekDayNo,
			DateTime baseDate,
			int minSpan,
			NextShippingCalculationMode calculationMode)
		{
			var diffDays = 0;

			// 配送日計算モードに応じて週間隔分ずらす
			if (calculationMode == NextShippingCalculationMode.EveryNWeek) diffDays = 7 * weekSpan;

			// 基準日の週の指定曜日までの日数
			diffDays += weekDayNo -(int)baseDate.AddDays(diffDays).DayOfWeek;

			// 最低配送間隔以上あいてなければ1週間後にずらす
			while (diffDays < Math.Max(minSpan, 1))
			{
				diffDays += 7;
			}

			// 基準日をYYYY/MM/DDの形に変換(時分秒をリセット)してから日付をずらす
			var resultShippingDate = baseDate.Date.AddDays(diffDays);
			return resultShippingDate;
		}

		/// <summary>
		/// 年・月・週番号・曜日番号を元に、DateTime の日付を決定します。
		/// </summary>
		/// <param name="year">年</param>
		/// <param name="month">月</param>
		/// <param name="weekNo">週番号</param>
		/// <param name="weekDayNo">曜日番号</param>
		/// <returns>年月日</returns>
		private DateTime GetDate(int year, int month, int weekNo, int weekDayNo)
		{
			var result = new DateTime(year, month, 1);
			int dateDiff = weekDayNo - (int)result.DayOfWeek;
			if (dateDiff < 0) dateDiff += 7;
			return result.AddDays(7 * (weekNo - 1) + dateDiff);
		}

		/// <summary>
		/// 指定された基準日を起点とし、次サイクルの配送日を計算して返却します。 [配送間隔指定]
		/// ※配送日計算モードは考慮されません。
		/// </summary>
		/// <param name="daySpan">配送間隔日数</param>
		/// <param name="baseDate">基準日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <returns>次サイクルの配送日</returns>
		private DateTime CalculateOnDateInterval(int daySpan, DateTime baseDate, int minSpan)
		{
			// 基準日をYYYY/MM/DDの形に変換(時分秒をリセット)してから日付をずらす
			var result = baseDate.Date.AddDays(daySpan);

			// 最低配送間隔以上あいてなければ次サイクルにずらす
			while (result < baseDate.Date.AddDays(Math.Max(minSpan, 1)))
			{
				result = result.AddDays(daySpan);
			}

			return result;
		}

		/// <summary>
		/// 最終購入注文の配送日を元にして、キャンセル可能な最短の次回配送日を計算して返却します。
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="lastShippedDate">最終配送日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="daysCancel">何日前までキャンセル可能か</param>
		/// <returns>次回配送日</returns>
		internal DateTime CalculateNextShippingDateFromLastShippedDate(
			string fpKbn,
			string fpSetting,
			DateTime lastShippedDate,
			int minSpan,
			int daysCancel)
		{
			// 配送パターンの再変更が可能な日付を基準日として計算する
			var cancelLimitDate = DateTime.Today.AddDays(daysCancel);

			return CalculateNextShippingDateFromLastShippedDate(
				fpKbn,
				fpSetting,
				lastShippedDate,
				minSpan,
				cancelLimitDate);
		}
		/// <summary>
		/// 最終購入注文の配送日を元に、指定されたキャンセル可能基準日以降で最短の次回配送日を計算して返却します。
		/// </summary>
		/// <param name="fpKbn">定期購入区分</param>
		/// <param name="fpSetting">定期購入設定</param>
		/// <param name="lastShippedDate">最終配送日</param>
		/// <param name="minSpan">最低配送間隔</param>
		/// <param name="cancelLimitDate">キャンセル可能基準日</param>
		/// <returns>次回配送日</returns>
		private DateTime CalculateNextShippingDateFromLastShippedDate(
			string fpKbn,
			string fpSetting,
			DateTime lastShippedDate,
			int minSpan,
			DateTime cancelLimitDate)
		{
			var result = CalculateFollowingShippingDate(
				fpKbn,
				fpSetting,
				lastShippedDate,
				minSpan,
				NextShippingCalculationMode.Earliest);

			// 基準日以降になっているかチェック
			while (result < cancelLimitDate)
			{
				if (fpKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS) result = DateTime.Today;
				result = CalculateFollowingShippingDate(
					fpKbn,
					fpSetting,
					result,
					minSpan,
					NextShippingCalculationMode.Earliest);
			}

			return result;
		}

		/// <summary>
		/// Calculate first shipping date option 2
		/// </summary>
		/// <param name="fixedPurchaseKbn">Fixed purchase kbn</param>
		/// <param name="fixedPurchaseSetting">Fixed purchase setting</param>
		/// <param name="firstShippingDate">First shipping date</param>
		/// <param name="minSpan">Minimum shipping span</param>
		/// <param name="calculationMode">Calculation mode</param>
		/// <returns>A first shipping date option 2</returns>
		public DateTime CalculateFirstShippingDateOption2(
			string fixedPurchaseKbn,
			string fixedPurchaseSetting,
			DateTime firstShippingDate,
			int minSpan,
			NextShippingCalculationMode calculationMode)
		{
			switch (fixedPurchaseKbn)
			{
				// 月間隔日付指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE:
					var monthAndDays = fixedPurchaseSetting.Split(',');
					return CalculateOnMonthlyDate(
						1,
						int.Parse(monthAndDays[1]),
						firstShippingDate,
						minSpan,
						calculationMode);

				// 月間隔、週、曜日指定
				case Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY:
					var monthWeekAndDays = fixedPurchaseSetting.Split(',');
					return CalculateOnWeekAndDay(
						int.Parse(monthWeekAndDays[1]),
						int.Parse(monthWeekAndDays[2]),
						firstShippingDate,
						minSpan,
						calculationMode,
						1);

				// 存在しない区分であれば基準日を返す
				default:
					return firstShippingDate;
			}
		}
		#endregion
	}
}
