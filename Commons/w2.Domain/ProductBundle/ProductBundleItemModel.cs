/*
=========================================================================================================
  Module      : 商品同梱 同梱商品テーブルモデル (ProductBundleItemModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductBundle
{
	/// <summary>
	/// 商品同梱 同梱商品テーブルモデル
	/// </summary>
	[Serializable]
	public partial class ProductBundleItemModel : ModelBase<ProductBundleItemModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductBundleItemModel()
		{
			this.GrantProductCount = Constants.FLG_PRODUCTBUNDLEITEM_GRANT_PRODUCT_COUNT_DEFAULT;
			this.OrderedProductExceptFlg =
				Constants.FLG_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG_BUNDLED_TARGET;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductBundleItemModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductBundleItemModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>商品同梱ID</summary>
		public string ProductBundleId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_PRODUCT_BUNDLE_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_PRODUCT_BUNDLE_ID] = value; }
		}
		/// <summary>同梱商品枝番</summary>
		public int ProductBundleItemNo
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_PRODUCT_BUNDLE_ITEM_NO]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_PRODUCT_BUNDLE_ITEM_NO] = value; }
		}
		/// <summary>同梱商品ID</summary>
		public string GrantProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_ID] = value; }
		}
		/// <summary>同梱商品バリエーションID</summary>
		public string GrantProductVariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_VARIATION_ID] = value; }
		}
		/// <summary>同梱個数</summary>
		public int GrantProductCount
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_COUNT]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_GRANT_PRODUCT_COUNT] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_DATE_CREATED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_LAST_CHANGED] = value; }
		}
		/// <summary>初回のみ同梱フラグ</summary>
		public string OrderedProductExceptFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBUNDLEITEM_ORDERED_PRODUCT_EXCEPT_FLG] = value; }
		}
		#endregion
	}
}
