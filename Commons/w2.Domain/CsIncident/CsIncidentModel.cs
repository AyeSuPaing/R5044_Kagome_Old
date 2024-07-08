/*
=========================================================================================================
  Module      : インシデントマスタモデル (CsIncidentModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.CsIncident
{
	/// <summary>
	/// インシデントマスタモデル
	/// </summary>
	[Serializable]
	public partial class CsIncidentModel : ModelBase<CsIncidentModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsIncidentModel()
		{
			this.ValidFlg = Constants.FLG_CSINCIDENTCATEGORY_VALID_FLG_VALID;
			this.DateCompleted = null;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CsIncidentModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CsIncidentModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_DEPT_ID] = value; }
		}
		/// <summary>インシデントID</summary>
		public string IncidentId
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_INCIDENT_ID]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_INCIDENT_ID] = value; }
		}
		/// <summary>ユーザーID</summary>
		public string UserId
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_USER_ID]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_USER_ID] = value; }
		}
		/// <summary>インシデントカテゴリID</summary>
		public string IncidentCategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_INCIDENT_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_INCIDENT_CATEGORY_ID] = value; }
		}
		/// <summary>インシデント件名</summary>
		public string IncidentTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_INCIDENT_TITLE]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_INCIDENT_TITLE] = value; }
		}
		/// <summary>ステータス</summary>
		public string Status
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_STATUS] = value; }
		}
		/// <summary>VOCID</summary>
		public string VocId
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_VOC_ID]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_VOC_ID] = value; }
		}
		/// <summary>VOCメモ</summary>
		public string VocMemo
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_VOC_MEMO]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_VOC_MEMO] = value; }
		}
		/// <summary>コメント</summary>
		public string Comment
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_COMMENT]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_COMMENT] = value; }
		}
		/// <summary>重要度</summary>
		public string Importance
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_IMPORTANCE]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_IMPORTANCE] = value; }
		}
		/// <summary>問合せ元名称</summary>
		public string UserName
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_USER_NAME]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_USER_NAME] = value; }
		}
		/// <summary>問合せ元連絡先</summary>
		public string UserContact
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_USER_CONTACT]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_USER_CONTACT] = value; }
		}
		/// <summary>担当グループ</summary>
		public string CsGroupId
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_CS_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_CS_GROUP_ID] = value; }
		}
		/// <summary>担当オペレータ</summary>
		public string OperatorId
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_OPERATOR_ID]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_OPERATOR_ID] = value; }
		}
		/// <summary>ロック状態</summary>
		public string LockStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_LOCK_STATUS]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_LOCK_STATUS] = value; }
		}
		/// <summary>ロック保持者</summary>
		public string LockOperatorId
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_LOCK_OPERATOR_ID]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_LOCK_OPERATOR_ID] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_VALID_FLG] = value; }
		}
		/// <summary>最終受付日時</summary>
		public DateTime DateLastReceived
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSINCIDENT_DATE_LAST_RECEIVED]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_DATE_LAST_RECEIVED] = value; }
		}
		/// <summary>対応完了日</summary>
		public DateTime? DateCompleted
		{
			get
			{
				if (this.DataSource[Constants.FIELD_CSINCIDENT_DATE_COMPLETED] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_CSINCIDENT_DATE_COMPLETED];
			}
			set { this.DataSource[Constants.FIELD_CSINCIDENT_DATE_COMPLETED] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSINCIDENT_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSINCIDENT_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENT_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENT_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
