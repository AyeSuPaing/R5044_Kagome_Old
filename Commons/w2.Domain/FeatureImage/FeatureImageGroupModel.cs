/*
=========================================================================================================
  Module      : 特集画像グループマスタモデル (FeatureImageGroupModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FeatureImage
{
	/// <summary>
	/// 特集画像グループマスタモデル
	/// </summary>
	[Serializable]
	public partial class FeatureImageGroupModel : ModelBase<FeatureImageGroupModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FeatureImageGroupModel()
		{
			this.GroupName = string.Empty;
			this.GroupSortNumber = 0;
			this.LastChanged = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureImageGroupModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeatureImageGroupModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>グループID</summary>
		public long GroupId
		{
			get { return (long)this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_FEATURE_IMAGE_GROUP_ID]; }
		}
		/// <summary>グループ名</summary>
		public string GroupName
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_GROUP_NAME]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_GROUP_NAME] = value; }
		}
		/// <summary>グループ順序</summary>
		public int GroupSortNumber
		{
			get { return (int)this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_GROUP_SORT_NUMBER]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_GROUP_SORT_NUMBER] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_FEATUREIMAGEGROUP_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
