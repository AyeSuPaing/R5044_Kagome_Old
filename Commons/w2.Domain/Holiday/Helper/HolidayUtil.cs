/*
=========================================================================================================
  Module      : 休日設定共通処理クラス(HolidayUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.Domain.DeliveryCompany;
using w2.Domain.DeliveryLeadTime;

namespace w2.Domain.Holiday.Helper
{
	/// <summary>
	/// 休日設定ユーティリティ
	/// </summary>
	public class HolidayUtil
	{
		/// <summary>
		/// 指定営業日前または後の日付を取得
		/// </summary>
		/// <param name="baseDate">基準日</param>
		/// <param name="businessDayCount">指定営業日数</param>
		/// <param name="isGetAfterDate">基準日より先の日付の取得か(true：指定営業日後の日付取得、false：指定営業日前の日付取得)</param>
		/// <returns>指定営業日前または後の日付</returns>
		public static DateTime GetDateOfBusinessDay(DateTime baseDate, int businessDayCount, bool isGetAfterDate)
		{
			var holidayList = new int[0];
			var holidayListDate = DateTime.MinValue;
			var addDay = (isGetAfterDate) ? 1 : -1;
			int passedBusinessDayCount = 0;

			// 配送希望日から１日ずつ未来を調べて営業日分カウントする
			while (passedBusinessDayCount < businessDayCount)
			{
				baseDate = baseDate.AddDays(addDay);

				// 休日リスト更新
				if ((holidayListDate.Year != baseDate.Year) || (holidayListDate.Month != baseDate.Month))
				{
					holidayList = new HolidayService().GetHolidays(baseDate);
					holidayListDate = baseDate;
				}

				// 営業日であれば営業日カウント+1
				if (holidayList.Contains(baseDate.Day) == false) passedBusinessDayCount++;
			}

			return baseDate;
		}

		/// <summary>
		/// 休日かどうか
		/// </summary>
		/// <param name="baseDate">判定する日</param>
		/// <returns>
		/// True：休日
		/// False：休日ではない
		/// </returns>
		public static bool IsHoliday(DateTime baseDate)
		{
			return new HolidayService().GetHolidays(baseDate).Contains(baseDate.Day);
		}

		/// <summary>
		/// 出荷予定日を計算
		/// </summary>
		/// <param name="shippingDate">出荷日</param>
		/// <param name="totalLeadTime">合計リードタイム</param>
		/// <param name="deliveryCompanyId">会社ID</param>
		/// <param name="isUseShortestShippingDate">最短出荷日を利用する(True:利用)</param>
		/// <returns>出荷予定日</returns>
		public static DateTime CalculatorScheduledShippingDate(DateTime shippingDate, int totalLeadTime, string deliveryCompanyId, bool isUseShortestShippingDate = true)
		{
			var scheduleShippingDate = shippingDate.AddDays(-1 * totalLeadTime);
			var shortestShippingDate = GetShortestShippingDateBasedOnToday(deliveryCompanyId);
			if (isUseShortestShippingDate && (shortestShippingDate > scheduleShippingDate)) return shortestShippingDate;

			do
			{
				scheduleShippingDate = IsHoliday(scheduleShippingDate) ? scheduleShippingDate.AddDays(-1) : scheduleShippingDate;
			} while (IsHoliday(scheduleShippingDate));

			return scheduleShippingDate;
		}

		/// <summary>
		/// 最短出荷日を取得(本日をもとに計算)
		/// </summary>
		/// <param name="deliveryCompanyId">会社ID</param>
		/// <returns>最短出荷日</returns>
		public static DateTime GetShortestShippingDateBasedOnToday(string deliveryCompanyId)
		{
			var shortestShippingDate = GetShortestShippingDate(deliveryCompanyId);
			return shortestShippingDate;
		}
		
		/// <summary>
		/// 最短出荷日を取得(初回注文完了日をもとに計算)
		/// </summary>
		/// <param name="firstOrderDate">初回注文完了日</param>
		/// <param name="deliveryCompanyId">会社ID</param>
		/// <returns>最短出荷日</returns>
		public static DateTime GetShortestShippingDateBasedOnFirstOrderDate(DateTime? firstOrderDate, string deliveryCompanyId)
		{
			var shortestShippingDate = GetShortestShippingDate(deliveryCompanyId, firstOrderDate);
			return shortestShippingDate;
		}

		/// <summary>
		/// 最短出荷日を取得
		/// </summary>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="firstOrderDate">初回注文完了日</param>
		/// <returns>最短出荷日</returns>
		private static DateTime GetShortestShippingDate(string deliveryCompanyId, DateTime? firstOrderDate = null)
		{
			//初回注文完了日がなかった場合は今の日時を代入
			var baseDate = (firstOrderDate.HasValue) ? ((DateTime)firstOrderDate) : DateTime.Now;
			var plusDay = Constants.MINIMUM_SHIPMENT_REQUIRED_BUSINESS_DAYS;
			if (Constants.TODAY_SHIPPABLE_DEADLINE_TIME)
			{
				var deadlineTime = new DeliveryCompanyService().Get(deliveryCompanyId).DeliveryCompanyDeadlineTime;
				if ((string.IsNullOrEmpty(deadlineTime) == false) && (DateTime.Parse(deadlineTime) > DateTime.Parse(baseDate.ToString("HH:mm")))) plusDay = 0;
			}

			var shortestShippingDate = baseDate.AddDays(plusDay);

			//休日を挟む場合
			while (IsHoliday(shortestShippingDate))
			{
				shortestShippingDate = shortestShippingDate.AddDays(1);
			}

			return shortestShippingDate.Date;
		}

		/// <summary>
		/// 最短配送日を取得
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="prefecture">県</param>
		/// <param name="zip">zip</param>
		/// <returns>最短配送日</returns>
		public static DateTime GetShortestDeliveryDate(
			string shopId,
			string deliveryCompanyId,
			string prefecture,
			string zip)
		{
			var totalLeadTime = GetTotalLeadTime(shopId, deliveryCompanyId, prefecture, zip);
			var shortestDeliveryDate = GetShortestShippingDateBasedOnToday(deliveryCompanyId).AddDays(totalLeadTime);

			return shortestDeliveryDate.Date;
		}

		/// <summary>
		/// 合計リードタイムを取得
		/// </summary>
		/// <param name="shopId">ショップID</param>
		/// <param name="deliveryCompanyId">会社ID</param>
		/// <param name="prefecture">県名</param>
		/// <param name="zip">ZIP</param>
		/// <returns>合計リードタイム</returns>
		public static int GetTotalLeadTime(
			string shopId,
			string deliveryCompanyId,
			string prefecture,
			string zip)
		{
			// 郵便番号の「-」を除去
			zip = zip.Replace("-", "");

			// 配送会社を取得
			var deliveryCompany = new DeliveryCompanyService().Get(deliveryCompanyId);
			var deliveryLeadTimes = new DeliveryLeadTimeService().GetAll(shopId, deliveryCompanyId);

			// ZIPか県名によるリードタイムを取得
			var deliveryLeadTime =
				deliveryLeadTimes.FirstOrDefault(item => item.Zip.Contains(zip)
					&& (string.IsNullOrEmpty(zip) == false))
				?? deliveryLeadTimes.FirstOrDefault(item => (item.LeadTimeZoneName == prefecture));
			var totalLeadTime = deliveryCompany.ShippingLeadTimeDefault;
			if (deliveryLeadTime != null) totalLeadTime += deliveryLeadTime.ShippingLeadTime;

			return totalLeadTime;
		}

	}
}