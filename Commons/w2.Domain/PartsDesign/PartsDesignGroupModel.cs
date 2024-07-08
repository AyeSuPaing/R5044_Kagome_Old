/*
=========================================================================================================
  Module      : パーツデザイン グループマスタモデル (PartsDesignGroupModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.PartsDesign
{
	/// <summary>
	/// パーツデザイン グループマスタモデル
	/// </summary>
	[Serializable]
	public partial class PartsDesignGroupModel : ModelBase<PartsDesignGroupModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PartsDesignGroupModel()
		{
			this.GroupName = "";
			this.GroupSortNumber = 0;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PartsDesignGroupModel(DataRowView source) : this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public PartsDesignGroupModel(Hashtable source) : this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>識別ID</summary>
		public long GroupId
		{
			get { return (long)this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_GROUP_ID]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_GROUP_ID] = value; }
		}
		/// <summary>グループ名</summary>
		public string GroupName
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_GROUP_NAME]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_GROUP_NAME] = value; }
		}
		/// <summary>グループ順序</summary>
		public int GroupSortNumber
		{
			get { return (int)this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_GROUP_SORT_NUMBER]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_GROUP_SORT_NUMBER] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PARTSDESIGNGROUP_LAST_CHANGED] = value; }
		}
		#endregion
	}
}