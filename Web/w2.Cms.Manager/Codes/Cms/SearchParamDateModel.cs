/*
=========================================================================================================
  Module      : 検索パラメータ日付モデル(SearchParamDateModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Util;

namespace w2.Cms.Manager.Codes.Cms
{
	/// <summary>
	/// 検索パラメータ日付モデル
	/// </summary>
	[Serializable]
	public class SearchParamDateModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SearchParamDateModel()
		{
			this.BeginYearFrom = string.Empty;
			this.BeginMonthFrom = string.Empty;
			this.BeginDayFrom = string.Empty;
			this.BeginYearTo = string.Empty;
			this.BeginMonthTo = string.Empty;
			this.BeginDayTo = string.Empty;
			this.EndYearFrom = string.Empty;
			this.EndMonthFrom = string.Empty;
			this.EndDayFrom = string.Empty;
			this.EndYearTo = string.Empty;
			this.EndMonthTo = string.Empty;
			this.EndDayTo = string.Empty;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 入力値の日付チェック
		/// </summary>
		/// <returns>正当な日付か</returns>
		public bool CheckDate()
		{
			if (CheckDateAllEmptyOrAllFilled(this.BeginYearFrom, this.BeginMonthFrom, this.BeginDayFrom) == false) return false;
			if (CheckDateAllEmptyOrAllFilled(this.BeginYearTo, this.BeginMonthTo, this.BeginDayTo) == false) return false;
			if (CheckDateAllEmptyOrAllFilled(this.EndYearFrom, this.EndMonthFrom, this.EndDayFrom) == false) return false;
			if (CheckDateAllEmptyOrAllFilled(this.EndYearTo, this.EndMonthTo, this.EndDayTo) == false) return false;
			var beginDateFrom = this.BeginYearFrom + this.BeginMonthFrom + this.BeginDayFrom;
			var beginDateTo = this.BeginYearTo + this.BeginMonthTo + this.BeginDayTo;
			var endDateFrom = this.EndYearFrom + this.EndMonthFrom + this.EndDayFrom;
			var endDateTo = this.EndYearTo + this.EndMonthTo + this.EndDayTo;
			return (beginDateTo.CompareTo(beginDateFrom) >= 0) && (endDateTo.CompareTo(endDateFrom) >= 0);
		}

		/// <summary>
		/// 年月日に空欄があるかチェック
		/// </summary>
		/// <param name="year">年</param>
		/// <param name="month">月</param>
		/// <param name="day">日</param>
		/// <returns>空欄があるか</returns>
		private static bool CheckDateAllEmptyOrAllFilled(string year, string month, string day)
		{
			return ((string.IsNullOrEmpty(year)
					&& string.IsNullOrEmpty(month)
					&& string.IsNullOrEmpty(day))
				|| ((string.IsNullOrEmpty(year) == false)
					&& (string.IsNullOrEmpty(month) == false)
					&& string.IsNullOrEmpty(day) == false));
		}

		/// <summary>
		/// 末日補正
		/// </summary>
		public void CorrectLastDayOfMonth()
		{
			this.BeginDayFrom = CoorectLastDayOfMonthImplement(
				this.BeginYearFrom,
				this.BeginMonthFrom,
				this.BeginDayFrom);
			this.BeginDayTo = CoorectLastDayOfMonthImplement(
				this.BeginYearTo,
				this.BeginMonthTo,
				this.BeginDayTo);
			this.EndDayFrom = CoorectLastDayOfMonthImplement(
				this.EndYearFrom,
				this.EndMonthFrom,
				this.EndDayFrom);
			this.EndDayTo = CoorectLastDayOfMonthImplement(
				this.EndYearTo,
				this.EndMonthTo,
				this.EndDayTo);
		}

		/// <summary>
		/// 末日補正実装
		/// </summary>
		/// <param name="year">年</param>
		/// <param name="month">月</param>
		/// <param name="day">日</param>
		/// <returns>補正後の日</returns>
		private static string CoorectLastDayOfMonthImplement(string year, string month, string day)
		{
			if (DateTimeUtility.IsLastDayOfMonth(year, month, day))
			{
				return DateTimeUtility.GetLastDayOfMonth(year, month).ToString();
			}
			else
			{
				return day;
			}
		}
		#endregion

		#region プロパティ
		/// <summary>開始年From</summary>
		public string BeginYearFrom { get; set; }
		/// <summary>開始月From</summary>
		public string BeginMonthFrom { get; set; }
		/// <summary>開始日From</summary>
		public string BeginDayFrom { get; set; }
		/// <summary>開始年To</summary>
		public string BeginYearTo { get; set; }
		/// <summary>開始月To</summary>
		public string BeginMonthTo { get; set; }
		/// <summary>開始日To</summary>
		public string BeginDayTo { get; set; }
		/// <summary>終了年From</summary>
		public string EndYearFrom { get; set; }
		/// <summary>終了月From</summary>
		public string EndMonthFrom { get; set; }
		/// <summary>終了日From</summary>
		public string EndDayFrom { get; set; }
		/// <summary>終了年To</summary>
		public string EndYearTo { get; set; }
		/// <summary>終了月To</summary>
		public string EndMonthTo { get; set; }
		/// <summary>終了日To</summary>
		public string EndDayTo { get; set; }
		#endregion
	}
}