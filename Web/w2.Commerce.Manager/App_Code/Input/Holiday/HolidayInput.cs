/*
=========================================================================================================
  Module      : 休日入力クラス (HolidayInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Input;
using w2.Domain.Holiday;

/// <summary>
/// 休日入力クラス
/// </summary>
[Serializable]
public class HolidayInput: InputBase<HolidayModel>
{
	#region コンストラクタ
	/// <summary>
	/// デフォルトコンストラクタ
	/// </summary>
	public HolidayInput()
	{
	}
	/// <summary>
	/// コンストラクタ
	/// </summary>
	/// <param name="model">モデル</param>
	public HolidayInput(HolidayModel model)
		: this()
	{
		this.YearMonth = model.YearMonth;
		this.Days = model.Days;
		this.DateCreated = model.DateCreated;
		this.DateChanged = model.DateChanged;
		this.LastChanged = model.LastChanged;
	}
	#endregion

	#region メソッド
	/// <summary>
	/// モデル作成
	/// </summary>
	/// <returns>モデル</returns>
	public override HolidayModel CreateModel()
	{
		var model = new HolidayModel
		{
			YearMonth = this.YearMonth,
			Days = this.Days,
			LastChanged = this.LastChanged,
		};
		return model;
	}
	#endregion

	#region プロパティ
	/// <summary>年月</summary>
	public string YearMonth
	{
		get { return (string)this.DataSource[Constants.FIELD_HOLIDAY_YEAR_MONTH]; }
		set { this.DataSource[Constants.FIELD_HOLIDAY_YEAR_MONTH] = value; }
	}
	/// <summary>当月の休日（カンマ区切り）</summary>
	public string Days
	{
		get { return (string)this.DataSource[Constants.FIELD_HOLIDAY_DAYS]; }
		set { this.DataSource[Constants.FIELD_HOLIDAY_DAYS] = value; }
	}
	/// <summary>作成日</summary>
	public DateTime DateCreated
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_HOLIDAY_DATE_CREATED]; }
		set { this.DataSource[Constants.FIELD_HOLIDAY_DATE_CREATED] = value; }
	}
	/// <summary>更新日</summary>
	public DateTime DateChanged
	{
		get { return (DateTime)this.DataSource[Constants.FIELD_HOLIDAY_DATE_CHANGED]; }
		set { this.DataSource[Constants.FIELD_HOLIDAY_DATE_CHANGED] = value; }
	}
	/// <summary>最終更新者</summary>
	public string LastChanged
	{
		get { return (string)this.DataSource[Constants.FIELD_HOLIDAY_LAST_CHANGED]; }
		set { this.DataSource[Constants.FIELD_HOLIDAY_LAST_CHANGED] = value; }
	}
	#endregion
}