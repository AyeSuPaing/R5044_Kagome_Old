/*
=========================================================================================================
  Module      : メッセージ依頼アイテムモデル(CsMessageRequestItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.Message
{
	/// <summary>
	/// メッセージ依頼アイテムモデル
	/// </summary>
	[Serializable]
	public partial class CsMessageRequestItemModel : ModelBase<CsMessageRequestItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMessageRequestItemModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CsMessageRequestItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CsMessageRequestItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_DEPT_ID] = value; }
		}
		/// <summary>インシデントID</summary>
		public string IncidentId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_INCIDENT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_INCIDENT_ID] = value; }
		}
		/// <summary>メッセージ番号</summary>
		public int MessageNo
		{
			get { return (int)this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_MESSAGE_NO]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_MESSAGE_NO] = value; }
		}
		/// <summary>依頼番号</summary>
		public int RequestNo
		{
			get { return (int)this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_REQUEST_NO]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_REQUEST_NO] = value; }
		}
		/// <summary>枝番</summary>
		public int BranchNo
		{
			get { return (int)this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_BRANCH_NO]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_BRANCH_NO] = value; }
		}
		/// <summary>承認者オペレータID</summary>
		public string ApprOperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_APPR_OPERATOR_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_APPR_OPERATOR_ID] = value; }
		}
		/// <summary>結果ステータス</summary>
		public string ResultStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_RESULT_STATUS]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_RESULT_STATUS] = value; }
		}
		/// <summary>結果理由</summary>
		public string Comment
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_COMMENT]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_COMMENT] = value; }
		}
		/// <summary>ステータス変更日</summary>
		public DateTime? DateStatusChanged
		{
			get
			{
				if (this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_DATE_STATUS_CHANGED] == DBNull.Value) return null;
				return (DateTime?)this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_DATE_STATUS_CHANGED];
			}
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_DATE_STATUS_CHANGED] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUESTITEM_LAST_CHANGED] = value; }
		}
		#endregion

	}
}
