/*
=========================================================================================================
  Module      : 商品情報(ProductData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;
using w2.Domain;

namespace w2.Commerce.Batch.AwooProductSync
{
	/// <summary>
	/// 商品情報
	/// </summary>
	class ProductData : ModelBase<ProductData>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductData(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductData(Hashtable source)
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCT_SHOP_ID]); }
			set { this.DataSource[Constants.FIELD_PRODUCT_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_ID]); }
			set { this.DataSource[Constants.FIELD_PRODUCT_PRODUCT_ID] = value; }
		}
		/// <summary>商品名</summary>
		public string Name
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCT_NAME]); }
			set { this.DataSource[Constants.FIELD_PRODUCT_NAME] = value; }
		}
		/// <summary>商品概要</summary>
		public string Outline
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCT_OUTLINE]); }
			set { this.DataSource[Constants.FIELD_PRODUCT_OUTLINE] = value; }
		}
		/// <summary>商品詳細１</summary>
		public string DescDetail1
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL1]); }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL1] = value; }
		}
		/// <summary>商品詳細2</summary>
		public string DescDetail2
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL2]); }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL2] = value; }
		}
		/// <summary>商品詳細3</summary>
		public string DescDetail3
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL3]); }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL3] = value; }
		}
		/// <summary>商品詳細4</summary>
		public string DescDetail4
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL4]); }
			set { this.DataSource[Constants.FIELD_PRODUCT_DESC_DETAIL4] = value; }
		}
		/// <summary>商品画像名ヘッダ</summary>
		public string ImageHead
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD]); }
			set { this.DataSource[Constants.FIELD_PRODUCT_IMAGE_HEAD] = value; }
		}
		/// <summary>商品表示価格</summary>
		public decimal DisplayPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_PRICE] = value; }
		}
		/// <summary>商品表示特別価格</summary>
		public decimal? DisplaySpecialPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE] = value; }
		}
		/// <summary>販売期間(FROM)</summary>
		public DateTime SellFrom
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCT_SELL_FROM]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_SELL_FROM] = value; }
		}
		/// <summary>カテゴリID1</summary>
		public string CategoryId1
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID1]); }
			set { this.DataSource[Constants.FIELD_PRODUCT_CATEGORY_ID1] = value; }
		}
		/// <summary>商品表示特別価格</summary>
		public decimal? SalePrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE] == DBNull.Value) return null;
				return (decimal?)this.DataSource[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE] = value; }
		}
		/// <summary>ブランドID</summary>
		public string BrandId
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCTBRAND_BRAND_ID]); }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_BRAND_ID] = value; }
		}
		/// <summary>ブランド名</summary>
		public string BrandName
		{
			get { return StringUtility.ToNull(this.DataSource[Constants.FIELD_PRODUCTBRAND_BRAND_NAME]); }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_BRAND_NAME] = value; }
		}
		/// <summary>カテゴリ名1</summary>
		public string CategoryName1
		{
			get { return StringUtility.ToNull(this.DataSource["category_name1"]); }
			set { this.DataSource["category_name1"] = value; }
		}
		/// <summary>カテゴリ名2</summary>
		public string CategoryName2
		{
			get { return StringUtility.ToNull(this.DataSource["category_name2"]); }
			set { this.DataSource["category_name2"] = value; }
		}
		/// <summary>カテゴリ名3</summary>
		public string CategoryName3
		{
			get { return StringUtility.ToNull(this.DataSource["category_name3"]); }
			set { this.DataSource["category_name3"] = value; }
		}
		/// <summary>カテゴリ名4</summary>
		public string CategoryName4
		{
			get { return StringUtility.ToNull(this.DataSource["category_name4"]); }
			set { this.DataSource["category_name4"] = value; }
		}
		/// <summary>カテゴリ名5</summary>
		public string CategoryName5
		{
			get { return StringUtility.ToNull(this.DataSource["category_name5"]); }
			set { this.DataSource["category_name5"] = value; }
		}
		/// <summary>お気に入り登録数</summary>
		public int FavoriteCount
		{
			get { return (int)this.DataSource["favorite_count"]; }
			set { this.DataSource["favorite_count"] = value; }
		}
		#endregion
	}
}
