/*
=========================================================================================================
  Module      : CSグループモデル(CsGroupModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.App.Common.Cs.CsOperator
{
	/// <summary>
	/// CSグループモデル
	/// </summary>
	[Serializable]
	public partial class CsGroupModel : ModelBase<CsGroupModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsGroupModel()
			: base()
		{ }
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">CSグループ情報</param>
		public CsGroupModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">CSグループ情報</param>
		public CsGroupModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public string DeptId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSGROUP_DEPT_ID]); }
			set { this.DataSource[Constants.FIELD_CSGROUP_DEPT_ID] = value; }
		}
		/// <summary>CSグループID</summary>
		public string CsGroupId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSGROUP_CS_GROUP_ID]); }
			set { this.DataSource[Constants.FIELD_CSGROUP_CS_GROUP_ID] = value; }
		}
		/// <summary>CSグループ名</summary>
		public string CsGroupName
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSGROUP_CS_GROUP_NAME]); }
			set { this.DataSource[Constants.FIELD_CSGROUP_CS_GROUP_NAME] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_CSGROUP_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_CSGROUP_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSGROUP_VALID_FLG]); }
			set { this.DataSource[Constants.FIELD_CSGROUP_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSGROUP_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_CSGROUP_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_CSGROUP_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_CSGROUP_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_CSGROUP_LAST_CHANGED]); }
			set { this.DataSource[Constants.FIELD_CSGROUP_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
