using System;
/*
=========================================================================================================
  Module      : 休日リポジトリ (HolidayRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.Holiday
{
	/// <summary>
	/// 休日リポジトリ
	/// </summary>
	public class HolidayRepository : RepositoryBase
	{
		/// <summary>XMLファイル名</summary>
		private const string XML_KEY_NAME = "Holiday";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public HolidayRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HolidayRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 全ての休日情報取得
		/// </summary>
		/// <returns>休日モデル配列</returns>
		internal HolidayModel[] Get()
		{
			var dv = Get(XML_KEY_NAME, "GetAll", null);
			return dv.Cast<DataRowView>().Select(drv => new HolidayModel(drv)).ToArray();
		}
		/// <summary>
		/// 年月単位で休日情報取得
		/// </summary>
		/// <param name="target">ターゲット日付</param>
		/// <returns>休日モデル</returns>
		internal HolidayModel Get(DateTime target)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_HOLIDAY_YEAR_MONTH, target.ToString("yyyyMM")}
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new HolidayModel(dv[0]);
		}
		/// <summary>
		/// 指定期間の休日情報取得
		/// </summary>
		/// <param name="yearMonthBegin">年月開始</param>
		/// <param name="monthCount">月数</param>
		/// <returns>指定期間の休日モデル配列</returns>
		internal HolidayModel[] Get(DateTime yearMonthBegin, int monthCount)
		{
			if (monthCount < 0) return new HolidayModel[0];

			var ht = new Hashtable
			{
				{Constants.FIELD_HOLIDAY_YEAR_MONTH + "_begin", yearMonthBegin.ToString("yyyyMM")},
				{Constants.FIELD_HOLIDAY_YEAR_MONTH + "_end", yearMonthBegin.AddMonths(monthCount - 1).ToString("yyyyMM")},
			};
			var dv = Get(XML_KEY_NAME, "GetByPeriod", ht);
			return dv.Cast<DataRowView>().Select(drv => new HolidayModel(drv)).ToArray();
		}

		/// <summary>
		/// 年単位で休日情報取得
		/// </summary>
		/// <param name="year">年</param>
		/// <returns>休日モデル配列</returns>
		internal HolidayModel[] GetByYear(string year)
		{
			var ht = new Hashtable
			{
				{"year", year}
			};
			var dv = Get(XML_KEY_NAME, "GetByYear", ht);
			return dv.Cast<DataRowView>().Select(drv => new HolidayModel(drv)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(HolidayModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="year">年</param>
		/// <returns>影響を受けた件数</returns>
		internal int Delete(string year)
		{
			var ht = new Hashtable
			{
				{"year", year},
			};

			return Exec(XML_KEY_NAME, "Delete", ht);
		}
		#endregion
	}
}
