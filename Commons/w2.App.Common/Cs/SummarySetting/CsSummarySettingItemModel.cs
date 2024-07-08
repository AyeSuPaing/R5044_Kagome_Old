/*
=========================================================================================================
  Module      : 集計区分アイテムモデル(CsSummarySettingItemModel.cs)
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
	/// 集計区分アイテムモデル
	/// </summary>
	[Serializable]
	public partial class CsSummarySettingItemModel : ModelBase<CsSummarySettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsSummarySettingItemModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">集計区分アイテム情報</param>
		public CsSummarySettingItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">集計区分アイテム情報</param>
		public CsSummarySettingItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_DEPT_ID] = value; }
		}
		/// <summary>集計区分番号</summary>
		public int SummarySettingNo
		{
			get { return (int)this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_SUMMARY_SETTING_NO]; }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_SUMMARY_SETTING_NO] = value; }
		}
		/// <summary>集計区分アイテムID</summary>
		public string SummarySettingItemId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_SUMMARY_SETTING_ITEM_ID]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_SUMMARY_SETTING_ITEM_ID] = value; }
		}
		/// <summary>集計区分アイテム文言</summary>
		public string SummarySettingItemText
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_SUMMARY_SETTING_ITEM_TEXT]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_SUMMARY_SETTING_ITEM_TEXT] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_VALID_FLG]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_DEL_FLG]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSSUMMARYSETTINGITEM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
