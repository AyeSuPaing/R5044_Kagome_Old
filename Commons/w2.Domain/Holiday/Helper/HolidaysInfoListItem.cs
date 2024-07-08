/*
=========================================================================================================
  Module      : 休日情報一覧のアイテムクラス (HolidaysInfoListItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.Holiday.Helper
{
	/// <summary>
	/// 休日情報一覧のアイテムクラス
	/// </summary>
	[Serializable]
	public class HolidaysInfoListItem
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="year">年</param>
		/// <param name="holidayNumbers">休日数</param>
		/// <param name="dateChanged">最終更新日</param>
		/// <param name="lastChanged">最終更新者</param>
		public HolidaysInfoListItem(int year, int holidayNumbers, DateTime dateChanged, string lastChanged)
		{
			this.Year = year;
			this.HolidayNumbers = holidayNumbers;
			this.DateChanged = dateChanged;
			this.LastChanged = lastChanged;
		}

		/// <summary>年</summary>
		public int Year { get; set; }
		/// <summary>休日数</summary>
		public int HolidayNumbers { get; set; }
		/// <summary>最終更新日</summary>
		public DateTime DateChanged { get; set; }
		/// <summary>最終更新者</summary>
		public string LastChanged { get; set; }
	}
}
