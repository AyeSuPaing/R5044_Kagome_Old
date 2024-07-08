/*
=========================================================================================================
  Module      : CSインシデント警告アイコンモデル (CsIncidentWarningIconModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.CsIncidentWarningIcon
{
	/// <summary>
	/// CSインシデント警告アイコンモデル
	/// </summary>
	[Serializable]
	public class CsIncidentWarningIconModel : ModelBase<CsIncidentWarningIconModel>
	{
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsIncidentWarningIconModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CsIncidentWarningIconModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CsIncidentWarningIconModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}

		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_DEPT_ID]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_DEPT_ID] = value; }
		}
		/// <summary>オペレータID</summary>
		public string OperatorId
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_OPERATOR_ID]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_OPERATOR_ID] = value; }
		}
		/// <summary>インシデントステータス</summary>
		public string IncidentStatus
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_INCIDENT_STATUS]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_INCIDENT_STATUS] = value; }
		}
		/// <summary>警告レベル</summary>
		public string WarningLevel
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_WARNING_LEVEL]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_WARNING_LEVEL] = value; }
		}
		/// <summary>アイコン切替時間</summary>
		public int? Term
		{
			get
			{
				if (this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_TERM] == DBNull.Value) return null;
				return (int?)this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_TERM];
			}
			set { this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_TERM] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSINCIDENTWARNINGICON_LAST_CHANGED] = value; }
		}
	}
}
