/*
=========================================================================================================
  Module      : 商品カテゴリマスタモデル (ProductCategoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductCategory
{
	/// <summary>
	/// 商品カテゴリマスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductCategoryModel : ModelBase<ProductCategoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductCategoryModel()
		{
			this.MobileDispFlg = Constants.FLG_PRODUCTCATEGORY_VALID_FLG_VALID;
			this.ValidFlg = Constants.FLG_PRODUCTCATEGORY_VALID_FLG_VALID;
			this.UseRecommendFlg = Constants.FLG_PRODUCTCATEGORY_USE_RECOMMEND_FLG_VALID;
			this.LowerMemberCanDisplayTreeFlg = Constants.FLG_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG_INVALID;
			this.ChildCategorySortKbn = Constants.FLG_CATEGORY_SORT_BY_CATEGORY_ID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductCategoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductCategoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_SHOP_ID] = value; }
		}
		/// <summary>カテゴリID</summary>
		public string CategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_CATEGORY_ID] = value; }
		}
		/// <summary>親カテゴリID</summary>
		public string ParentCategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_PARENT_CATEGORY_ID] = value; }
		}
		/// <summary>カテゴリ名</summary>
		public string Name
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_NAME] = value; }
		}
		/// <summary>モバイル用カテゴリ名</summary>
		public string NameMobile
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_NAME_MOBILE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_NAME_MOBILE] = value; }
		}
		/// <summary>SEOキーワード</summary>
		public string SeoKeywords
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_SEO_KEYWORDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_SEO_KEYWORDS] = value; }
		}
		/// <summary>カノニカルタグ用テキスト</summary>
		public string CanonicalText
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_CANONICAL_TEXT]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_CANONICAL_TEXT] = value; }
		}
		/// <summary>カテゴリページURL</summary>
		public string Url
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_URL]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_URL] = value; }
		}
		/// <summary>モバイル表示フラグ</summary>
		public string MobileDispFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_MOBILE_DISP_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_MOBILE_DISP_FLG] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_VALID_FLG] = value; }
		}
		/// <summary>削除フラグ</summary>
		public string DelFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_DEL_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_DEL_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_LAST_CHANGED] = value; }
		}
		/// <summary>許可ブランドID</summary>
		public string PermittedBrandIds
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_PERMITTED_BRAND_IDS]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_PERMITTED_BRAND_IDS] = value; }
		}
		/// <summary>外部レコメンド利用フラグ</summary>
		public string UseRecommendFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_USE_RECOMMEND_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_USE_RECOMMEND_FLG] = value; }
		}
		/// <summary>閲覧可能会員ランクID</summary>
		public string MemberRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_MEMBER_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_MEMBER_RANK_ID] = value; }
		}
		/// <summary>会員ランク表示制御：カテゴリツリー表示フラグ</summary>
		public string LowerMemberCanDisplayTreeFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_LOWER_MEMBER_CAN_DISPLAY_TREE_FLG] = value; }
		}
		/// <summary>カテゴリカナ名</summary>
		public string NameKana
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_NAME_KANA]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_NAME_KANA] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_DISPLAY_ORDER] = value; }
		}
		/// <summary>子カテゴリの並び順</summary>
		public string ChildCategorySortKbn
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_CHILD_CATEGORY_SORT_KBN]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_CHILD_CATEGORY_SORT_KBN] = value; }
		}
		/// <summary>定期会員限定フラグ</summary>
		public string OnlyFixedPurchaseMemberFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTCATEGORY_ONLY_FIXED_PURCHASE_MEMBER_FLG] = value; }
		}
		#endregion
	}
}
