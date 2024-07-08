/*
=========================================================================================================
  Module      : コーディネートカテゴリモデル (CoordinateCategoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.CoordinateCategory
{
	/// <summary>
	/// コーディネートカテゴリモデル
	/// </summary>
	[Serializable]
	public partial class CoordinateCategoryModel : ModelBase<CoordinateCategoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CoordinateCategoryModel()
		{
			this.CoordinateCategoryId = "";
			this.CoordinateParentCategoryId = "";
			this.CoordinateCategoryName = "";
			this.SeoKeywords = "";
			this.DisplayOrder = 0;
			this.ValidFlg = Constants.FLG_COORDINATECATEGORY_VALID_FLG_VALID;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CoordinateCategoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public CoordinateCategoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>カテゴリID</summary>
		public string CoordinateCategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_ID] = value; }
		}
		/// <summary>親カテゴリID</summary>
		public string CoordinateParentCategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_PARENT_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_PARENT_CATEGORY_ID] = value; }
		}
		/// <summary>カテゴリ名</summary>
		public string CoordinateCategoryName
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_NAME]; }
			set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_NAME] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_COORDINATECATEGORY_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_DISPLAY_ORDER] = value; }
		}
		/// <summary>SEOキーワード</summary>
		public string SeoKeywords
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_SEO_KEYWORDS]; }
			set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_SEO_KEYWORDS] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COORDINATECATEGORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_COORDINATECATEGORY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_COORDINATECATEGORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_COORDINATECATEGORY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
