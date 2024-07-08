/*
=========================================================================================================
  Module      : 休日サービスクラス (HolidayService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.Holiday
{
	/// <summary>
	/// 休日サービスクラス
	/// </summary>
	public class HolidayService : ServiceBase
	{
		#region +休日情報取得
		/// <summary>
		/// 全ての休日情報取得
		/// </summary>
		/// <returns>休日モデル配列</returns>
		public HolidayModel[] Get()
		{
			using (var repository = new HolidayRepository())
			{
				var models = repository.Get();
				return models;
			}
		}
		/// <summary>
		/// 年月単位で休日情報取得
		/// </summary>
		/// <param name="target">ターゲット日付</param>
		/// <returns>休日モデル</returns>
		public HolidayModel Get(DateTime target)
		{
			using (var repository = new HolidayRepository())
			{
				var model = repository.Get(target);
				return model;
			}
		}
		/// <summary>
		/// 指定期間の休日情報取得
		/// </summary>
		/// <param name="yearMonthBegin">年月開始</param>
		/// <param name="monthCount">月数</param>
		/// <returns>指定期間の休日モデル配列</returns>
		public HolidayModel[] Get(DateTime yearMonthBegin, int monthCount)
		{
			using (var repository = new HolidayRepository())
			{
				var models = repository.Get(yearMonthBegin, monthCount);
				return models;
			}
		}

		/// <summary>
		/// 年単位で休日情報取得
		/// </summary>
		/// <param name="year">年</param>
		/// <returns>休日モデル配列</returns>
		public HolidayModel[] GetByYear(string year)
		{
			using (var repository = new HolidayRepository())
			{
				var models = repository.GetByYear(year);
				return models;
			}
		}

		/// <summary>
		/// 指定日の月に関する全て休日の日を取得
		/// </summary>
		/// <param name="target">日付</param>
		/// <returns>休日の日のint型配列</returns>
		public int[] GetHolidays(DateTime target)
		{
			var model = Get(target);
			if (model == null) return new int[0];

			return model.Holidays;
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(HolidayModel model)
		{
			using (var repository = new HolidayRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="year">年</param>
		/// <returns>件数</returns>
		public int Delete(string year)
		{
			using (var repository = new HolidayRepository())
			{
				return repository.Delete(year);
			}
		}
		#endregion
	}
}
