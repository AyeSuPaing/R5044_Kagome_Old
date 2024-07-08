/*
=========================================================================================================
  Module      : メール振分設定アイテムモデル(CsMailAssignSettingItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.MailAssignSetting
{
	/// <summary>
	/// メール振分設定アイテムモデル
	/// </summary>
	[Serializable]
	public partial class CsMailAssignSettingItemModel : ModelBase<CsMailAssignSettingItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMailAssignSettingItemModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">メール振分設定アイテム</param>
		public CsMailAssignSettingItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">メール振分設定アイテム</param>
		public CsMailAssignSettingItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_DEPT_ID] = value; }
		}
		/// <summary>メール振分設定ID</summary>
		public string MailAssignId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_MAIL_ASSIGN_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_MAIL_ASSIGN_ID] = value; }
		}
		/// <summary>アイテム番号</summary>
		public int ItemNo
		{
			get { return (int)this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_ITEM_NO] = value; }
		}
		/// <summary>マッチング対象</summary>
		public string MatchingTarget
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TARGET] = value; }
		}
		/// <summary>マッチング値</summary>
		public string MatchingValue
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_VALUE]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_VALUE] = value; }
		}
		/// <summary>マッチング種別</summary>
		public string MatchingType
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TYPE]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_MATCHING_TYPE] = value; }
		}
		/// <summary>大文字小文字非区別</summary>
		public string IgnoreCase
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_IGNORE_CASE]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_IGNORE_CASE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTINGITEM_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
