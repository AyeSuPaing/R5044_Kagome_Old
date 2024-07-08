/*
=========================================================================================================
  Module      : メール振分設定モデル(CsMailAssignSettingModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;
using w2.App.Common.Cs.CsOperator;

namespace w2.App.Common.MailAssignSetting
{
	/// <summary>
	/// メール振分設定モデル
	/// </summary>
	[Serializable]
	public partial class CsMailAssignSettingModel : ModelBase<CsMailAssignSettingModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMailAssignSettingModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">メール振分設定</param>
		public CsMailAssignSettingModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">メール振分設定</param>
		public CsMailAssignSettingModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_DEPT_ID] = value; }
		}
		/// <summary>メール振分設定ID</summary>
		public string MailAssignId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_MAIL_ASSIGN_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_MAIL_ASSIGN_ID] = value; }
		}
		/// <summary>振分優先順</summary>
		public int AssignPriority
		{
			get { return (int)this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_PRIORITY]; }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_PRIORITY] = value; }
		}
		/// <summary>論理演算子</summary>
		public string LogicalOperator
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_LOGICAL_OPERATOR]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_LOGICAL_OPERATOR] = value; }
		}
		/// <summary>振分け停止</summary>
		public string StopFiltering
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_STOP_FILTERING]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_STOP_FILTERING] = value; }
		}
		/// <summary>振分先インシデントカテゴリID</summary>
		public string AssignIncidentCategoryId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_INCIDENT_CATEGORY_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_INCIDENT_CATEGORY_ID] = value; }
		}
		/// <summary>振分後担当オペレータID</summary>
		public string AssignOperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_OPERATOR_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_OPERATOR_ID] = value; }
		}
		/// <summary>振分後担当CSグループID</summary>
		public string AssignCsGroupId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_CS_GROUP_ID]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_CS_GROUP_ID] = value; }
		}
		/// <summary>振分後重要度</summary>
		public string AssignImportance
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_IMPORTANCE]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_IMPORTANCE] = value; }
		}
		/// <summary>振分後ステータス</summary>
		public string AssignStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_STATUS]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_ASSIGN_STATUS] = value; }
		}
		/// <summary>ごみ箱</summary>
		public string Trash
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_TRASH]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_TRASH] = value; }
		}
		/// <summary>オートレスポンス</summary>
		public string AutoResponse
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE] = value; }
		}
		/// <summary>オートレスポンス送信元アドレス</summary>
		public string AutoResponseFrom
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_FROM]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_FROM] = value; }
		}
		/// <summary>オートレスポンスCC</summary>
		public string AutoResponseCc
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_CC]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_CC] = value; }
		}
		/// <summary>オートレスポンスBCC</summary>
		public string AutoResponseBcc
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_BCC]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_BCC] = value; }
		}
		/// <summary>オートレスポンス件名</summary>
		public string AutoResponseSubject
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_SUBJECT]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_SUBJECT] = value; }
		}
		/// <summary>オートレスポンス本文</summary>
		public string AutoResponseBody
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_BODY]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_AUTO_RESPONSE_BODY] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_VALID_FLG]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_LAST_CHANGED] = value; }
		}
		/// <summary>振分設定名</summary>
		public string MailAssignName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_MAIL_ASSIGN_NAME]); }
			set { this.DataSource[Constants.FIELD_CSMAILASSIGNSETTING_MAIL_ASSIGN_NAME] = value; }
		}
		#endregion
	}
}
