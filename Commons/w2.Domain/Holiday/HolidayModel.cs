/*
=========================================================================================================
  Module      : 休日モデル (HolidayModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Holiday
{
	/// <summary>
	/// 休日モデル
	/// </summary>
	[Serializable]
	public partial class HolidayModel : ModelBase<HolidayModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public HolidayModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public HolidayModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public HolidayModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
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
}
