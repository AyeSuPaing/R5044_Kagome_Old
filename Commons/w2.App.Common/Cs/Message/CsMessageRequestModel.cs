/*
=========================================================================================================
  Module      : メッセージ依頼モデル(CsMessageRequestModel.cs)
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
	/// メッセージ依頼モデル
	/// </summary>
	[Serializable]
	public partial class CsMessageRequestModel : ModelBase<CsMessageRequestModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMessageRequestModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CsMessageRequestModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CsMessageRequestModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_DEPT_ID] = value; }
		}
		/// <summary>インシデントID</summary>
		public string IncidentId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_INCIDENT_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_INCIDENT_ID] = value; }
		}
		/// <summary>メッセージ番号</summary>
		public int MessageNo
		{
			get { return (int)this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_MESSAGE_NO]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_MESSAGE_NO] = value; }
		}
		/// <summary>依頼番号</summary>
		public int RequestNo
		{
			get { return (int)this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_NO]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_NO] = value; }
		}
		/// <summary>依頼者オペレータID</summary>
		public string RequestOperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_OPERATOR_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_OPERATOR_ID] = value; }
		}
		/// <summary>依頼ステータス</summary>
		public string RequestStatus
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_STATUS]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_STATUS] = value; }
		}
		/// <summary>依頼種別</summary>
		public string RequestType
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_TYPE]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_REQUEST_TYPE] = value; }
		}
		/// <summary>緊急度</summary>
		public string UrgencyFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_URGENCY_FLG]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_URGENCY_FLG] = value; }
		}
		/// <summary>承認方法</summary>
		public string ApprovalType
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_APPROVAL_TYPE]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_APPROVAL_TYPE] = value; }
		}
		/// <summary>承認依頼コメント</summary>
		public string Comment
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_COMMENT]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_COMMENT] = value; }
		}
		/// <summary>作業中オペレータID</summary>
		public string WorkingOperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_WORKING_OPERATOR_ID]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_WORKING_OPERATOR_ID] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSMESSAGEREQUEST_LAST_CHANGED] = value; }
		}
		#endregion

	}
}
