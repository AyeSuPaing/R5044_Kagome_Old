/*
=========================================================================================================
  Module      : 休日モデル (HolidayModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;

namespace w2.Domain.Holiday
{
	/// <summary>
	/// 休日モデル
	/// </summary>
	public partial class HolidayModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary> 年 </summary>
		public int Year { get { return int.Parse(this.YearMonth.Substring(0, 4)); } }
		/// <summary> 月 </summary>
		public int Month { get { return int.Parse(this.YearMonth.Substring(4, 2)); } }
		/// <summary> 休日の日配列 </summary>
		public int[] Holidays 
		{
			get
			{
				return string.IsNullOrEmpty(this.Days)
					? new int[0]
					: this.Days.Split(',').Select(int.Parse).ToArray();
			}
		}
		#endregion
	}
}
