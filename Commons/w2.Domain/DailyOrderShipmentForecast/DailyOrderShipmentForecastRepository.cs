/*
=========================================================================================================
  Module      : 出荷集計リポジトリ (DailyOrderShipmentForecastRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using w2.Common.Extensions;
using w2.Common.Sql;

namespace w2.Domain.DailyOrderShipmentForecast
{
	/// <summary>
	/// 出荷集計リポジトリ
	/// </summary>
	internal class DailyOrderShipmentForecastRepository : RepositoryBase
	{
		/// <summary>キー名</summary>
		private const string XML_KEY_NAME = "DailyOrderShipmentForecast";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public DailyOrderShipmentForecastRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DailyOrderShipmentForecastRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetDailyPriceAndShippment 指定日売上合計金額と出荷数を取得
		/// <summary>
		/// 指定日売上合計金額と出荷数を取得
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		/// <param name="date">指定日</param>
		/// <returns>指定日と売上合計金額と出荷数</returns>
		internal Hashtable GetDailyPriceAndShippment(int year, int month, int date)
		{
			var ht = new Hashtable
			{
				{ "year", year },
				{ "month", month },
				{ "date", date },
			};

			var dv = Get(XML_KEY_NAME, "GetDailyPriceAndShippment", ht);
			if (dv.Count == 0) return null;

			return dv[0].ToHashtable();
		}
		#endregion

		#region +GetTotalPriceAndShippedForMonth 指定月での売上金額＆出荷数の合計数を取得する
		/// <summary>
		/// 指定月での売上金額＆出荷数の合計数を取得する
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		/// <returns>売上金額＆出荷数</returns>
		internal Hashtable GetTotalPriceAndShippedForMonth(int year, int month)
		{
			var ht = new Hashtable
			{
				{ "year", year },
				{ "month", month },
			};

			var dv = Get(XML_KEY_NAME, "GetTotalPriceAndShippedForMonth", ht);
			if (dv.Count == 0) return null;

			return dv[0].ToHashtable();
		}
		#endregion

		#region +GetLastDataChangedDateForCurrentMonth 指定年月で最終更新日を取得する
		/// <summary>
		/// 指定年月で最終更新日を取得する
		/// </summary>
		/// <param name="year">指定年</param>
		/// <param name="month">指定月</param>
		/// <returns>最終更新日</returns>
		internal DateTime? GetLastDataChangedDateForCurrentMonth(int year, int month)
		{
			var ht = new Hashtable
			{
				{ "year", year },
				{ "month", month },
			};

			var dv = Get(XML_KEY_NAME, "GetLastDataChangedDateForCurrentMonth", ht);
			if ((dv.Count == 0)
				|| (dv[0][Constants.FIELD_SHIPMENTQUANTITY_DATE_CHANGED] == DBNull.Value)) return null;

			return (DateTime)dv[0][Constants.FIELD_SHIPMENTQUANTITY_DATE_CHANGED];
		}
		#endregion

		#region +InsertOrUpdate 挿入または更新
		/// <summary>
		/// 挿入または更新
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertOrUpdate(DailyOrderShipmentForecastModel model)
		{
			Exec(XML_KEY_NAME, "InsertOrUpdate", model.DataSource);
		}
		#endregion

		#region +DeleteOldShipments ２年前のデータを削除する
		/// <summary>
		/// ２年前のデータを削除する
		/// </summary>
		internal void DeleteOldShipments()
		{
			Exec(XML_KEY_NAME, "DeleteOldShipments");
		}
		#endregion
	}
}
