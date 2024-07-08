/*
=========================================================================================================
  Module      : 日付入力コントロールビューモデル(RegisterViewModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Web.Mvc;
using w2.App.Common.Util;

namespace w2.Cms.Manager.ViewModels.Shared
{
	/// <summary>日付入力コントロールビューモデル</summary>
	public class DateTimeInputModel
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="baseName">呼び出し元でのプロパティ名</param>
		public DateTimeInputModel(string baseName)
		{
			this.BaseName = baseName;
			this.YearItems = DateTimeUtility.GetShortRangeYearListItem()
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.MonthItems = DateTimeUtility.GetMonthListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.DayItems = DateTimeUtility.GetDayListItem("00", "00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.HourItems = DateTimeUtility.GetHourListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.MinuteItems = DateTimeUtility.GetMinuteListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
			this.SecondItems = DateTimeUtility.GetSecondListItem("00")
				.Select(li => new SelectListItem
				{
					Value = li.Value,
					Text = li.Text
				}).ToArray();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="baseName">呼び出し元でのプロパティ名</param>
		/// <param name="date">初期設定日時</param>
		public DateTimeInputModel(string baseName, DateTime date) : this(baseName)
		{
			this.Year = date.Year.ToString();
			this.Month = date.Month.ToString("00");
			this.Day = date.Day.ToString("00");
			this.Hour = date.Hour.ToString("00");
			this.Minute = date.Minute.ToString("00");
			this.Second = date.Second.ToString("00");
		}

		/// <summary>
		/// Validate
		/// </summary>
		/// <param name="name">name</param>
		/// <returns>Error message</returns>
		public string Validate(string name)
		{
			var error = Validator.CheckDateError(name, this.DateTimeString);
			return error;
		}

		/// <summary>集計期間(FROM)</summary>
		public DateTime ToDateTime
		{
			get
			{
				DateTime dateTime;
				if (DateTime.TryParse(this.DateTimeString, out dateTime) == false)
				{
					return dateTime;
				}
				return DateTime.Parse(this.DateTimeString);
			}
		}
		/// <summary>Date time string</summary>
		public string DateTimeString
		{
			get
			{
				var dateTime = string.Format(
					"{0}-{1}-{2} {3}:{4}:{5}",
					this.Year,
					this.Month,
					this.Day,
					this.Hour,
					this.Minute,
					this.Second);
				return dateTime;
			}
		}
		/// <summary> 利用元のパス </summary>
		public string BaseName { get; set; }
		/// <summary> 時刻部分があるか </summary>
		public bool HasTime { get; set; }
		/// <summary>選択肢群 年</summary>
		public SelectListItem[] YearItems { get; private set; }
		/// <summary>選択肢群 月</summary>
		public SelectListItem[] MonthItems { get; private set; }
		/// <summary>選択肢群 日</summary>
		public SelectListItem[] DayItems { get; private set; }
		/// <summary>選択肢群 時</summary>
		public SelectListItem[] HourItems { get; private set; }
		/// <summary>選択肢群 分</summary>
		public SelectListItem[] MinuteItems { get; private set; }
		/// <summary>選択肢群 秒</summary>
		public SelectListItem[] SecondItems { get; private set; }
		/// <summary>年月日：年</summary>
		public string Year { get; set; }
		/// <summary>年月日：月</summary>
		public string Month { get; set; }
		/// <summary>年月日：日</summary>
		public string Day { get; set; }
		/// <summary>時分秒：時</summary>
		public string Hour { get; set; }
		/// <summary>時分秒：分</summary>
		public string Minute { get; set; }
		/// <summary>時分秒：秒</summary>
		public string Second { get; set; }
	}
}