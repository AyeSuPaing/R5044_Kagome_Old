/*
=========================================================================================================
  Module      : 出荷集計サービス (DailyOrderShipmentForecastService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;

namespace w2.Domain.DailyOrderShipmentForecast
{
	/// <summary>
	/// 出荷集計サービス
	/// </summary>
	public class DailyOrderShipmentForecastService : ServiceBase
	{
		#region +GetDailyShippmentByYearAndMonth 指定年月の日単位売上商品金額と出荷数を取得
		/// <summary>
		/// 指定年月の日単位売上商品金額と出荷数を取得
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		/// <param name="lastDayOfMonth">該当月の最大日数</param>
		/// <returns>該当月の日別売上商品金額と出荷数リスト</returns>
		public List<Hashtable> GetDailyPriceAndShippmentByYearAndMonth(int year, int month, int lastDayOfMonth)
		{
			using (var repository = new DailyOrderShipmentForecastRepository())
			{
				var dailyShipmentList = new List<Hashtable>();
				for (var i = 1; i <= lastDayOfMonth; i++)
				{
					var dailyShipment = new Hashtable();
					var targetDailyShipment = repository.GetDailyPriceAndShippment(year, month, i);
					dailyShipment.Add("monthDay", string.Format("{0}/{1}", month, i));
					dailyShipment.Add(Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_PRICE_SUBTOTAL,
						targetDailyShipment == null
							? "0"
							: targetDailyShipment[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_PRICE_SUBTOTAL]);
					dailyShipment.Add(Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT,
						targetDailyShipment == null
							? "0"
							: targetDailyShipment[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT]);
					dailyShipmentList.Add(dailyShipment);
				}
				return dailyShipmentList;
			}
		}
		#endregion

		#region +GetTotalPriceAndShippedForMonth 指定月での売上金額＆出荷数の合計数を取得する
		/// <summary>
		/// 指定月での売上金額＆出荷数の合計数を取得する
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		/// <returns>該当月の売上金額＆出荷数の合計数</returns>
		public Hashtable GetTotalPriceAndShippedForMonth(int year, int month)
		{
			using (var repository = new DailyOrderShipmentForecastRepository())
			{
				var result = repository.GetTotalPriceAndShippedForMonth(year, month);
				result[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_PRICE_SUBTOTAL]
					= string.IsNullOrEmpty(result[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_PRICE_SUBTOTAL].ToString())
						? "0"
						: result[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_PRICE_SUBTOTAL];
				result[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT]
					= string.IsNullOrEmpty(result[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT].ToString())
						? "0"
						: result[Constants.FIELD_SHIPMENTQUANTITY_SHIPMENT_ORDER_COUNT];
				return result;
			}
		}
		#endregion

		#region +GetLastDataChangedDateForCurrentMonth 指定年月で最終更新日を取得する
		/// <summary>
		/// 指定年月で最終更新日を取得する
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		/// <returns>最終更新日</returns>
		public DateTime? GetLastDataChangedDateForCurrentMonth(int year, int month)
		{
			using (var repository = new DailyOrderShipmentForecastRepository())
			{
				return repository.GetLastDataChangedDateForCurrentMonth(year, month);
			}
		}
		#endregion

		#region +InsertOrUpdate 挿入または更新
		/// <summary>
		/// 挿入または更新
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertOrUpdate(DailyOrderShipmentForecastModel model)
		{
			using (var repository = new DailyOrderShipmentForecastRepository())
			{
				repository.InsertOrUpdate(model);
			}
		}
		#endregion

		#region +DeleteOldShipments ２年前のデータを削除する
		/// <summary>
		/// ２年前のデータを削除する
		/// </summary>
		public void DeleteOldShipments()
		{
			using (var repository = new DailyOrderShipmentForecastRepository())
			{
				repository.DeleteOldShipments();
			}
		}
		#endregion
	}
}
