/*
=========================================================================================================
  Module      : 特集ページカテゴリモデル (FeaturePageCategoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FeaturePageCategory
{
	/// <summary>
	/// 特集ページアイテム設定マスタモデル
	/// </summary>
	[Serializable]
	public partial class FeaturePageCategoryModel : ModelBase<FeaturePageCategoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FeaturePageCategoryModel()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeaturePageCategoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FeaturePageCategoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_SHOP_ID] = value; }
		}
		/// <summary>カテゴリID</summary>
		public string CategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_ID] = value; }
		}
		/// <summary>親カテゴリID</summary>
		public string ParentCategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_PARENT_CATEGORY_ID] = value; }
		}
		/// <summary>カテゴリ名</summary>
		public string CategoryName
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_NAME]; }
			set { this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_NAME] = value; }
		}
		/// <summary>カテゴリ説明文</summary>
		public string CategoryOutline
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_OUTLINE]; }
			set { this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_CATEGORY_OUTLINE] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_DISPLAY_ORDER] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_FEATURE_PAGE_CATEGORY_VALID_FLG] = value; }
		}
		#endregion
	}
}