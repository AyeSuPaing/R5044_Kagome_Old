/*
=========================================================================================================
  Module      : 集計区分モデル(CsSummarySettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.SummarySetting
{
	/// <summary>
	/// 集計区分モデル
	/// </summary>
	[Serializable]
	public partial class CsSummarySettingModel : ModelBase<CsSummarySettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsSummarySettingModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">集計区分情報</param>
		public CsSummarySettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">集計区分情報情報</param>
		public CsSummarySettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTING_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTING_DEPT_ID] = value; }
		}
		/// <summary>集計区分番号</summary>
		public int SummarySettingNo
		{
			get { return (int)this.DataSource[Constants.FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_NO]; }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_NO] = value; }
		}
		/// <summary>集計区分名</summary>
		public string SummarySettingTitle
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_TITLE]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_TITLE] = value; }
		}
		/// <summary>集計区分入力種別</summary>
		public string SummarySettingType
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTING_SUMMARY_SETTING_TYPE] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_CSSUMMARYSETTING_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTING_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTING_VALID_FLG]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTING_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTING_DEL_FLG]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTING_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSSUMMARYSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSSUMMARYSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTING_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTING_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
