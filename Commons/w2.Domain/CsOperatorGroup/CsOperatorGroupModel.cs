/*
=========================================================================================================
  Module      : CSオペレータ所属グループ(CsOperatorGroupModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.Domain.CsOperatorGroup
{
	/// <summary>
	/// CSオペレータ所属グループ
	/// </summary>
	[Serializable]
	public class CsOperatorGroupModel : ModelBase<CsOperatorGroupModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsOperatorGroupModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">CSオペレータ所属グループ情報</param>
		public CsOperatorGroupModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">CSオペレータ所属グループ情報</param>
		public CsOperatorGroupModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORGROUP_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORGROUP_DEPT_ID] = value; }
		}
		/// <summary>CSグループID</summary>
		public string CsGroupId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORGROUP_CS_GROUP_ID]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORGROUP_CS_GROUP_ID] = value; }
		}
		/// <summary>オペレータID</summary>
		public string OperatorId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORGROUP_OPERATOR_ID]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORGROUP_OPERATOR_ID] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSOPERATORGROUP_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSOPERATORGROUP_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSOPERATORGROUP_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSOPERATORGROUP_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
