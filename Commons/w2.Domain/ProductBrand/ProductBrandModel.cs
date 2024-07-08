/*
=========================================================================================================
  Module      : 商品ブランドマスタモデル (ProductBrandModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductBrand
{
	/// <summary>
	/// 商品ブランドマスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductBrandModel : ModelBase<ProductBrandModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductBrandModel()
		{
			this.DefaultFlg = Constants.FLG_PRODUCTBRAND_DEFAULT_FLG_OFF;
			this.ValidFlg = Constants.FLG_PRODUCTBRAND_VALID_FLG_VALID;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductBrandModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductBrandModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>ブランドID</summary>
		public string BrandId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBRAND_BRAND_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_BRAND_ID] = value; }
		}
		/// <summary>ブランド名</summary>
		public string BrandName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBRAND_BRAND_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_BRAND_NAME] = value; }
		}
		/// <summary>ブランドタイトル</summary>
		public string BrandTitle
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBRAND_BRAND_TITLE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_BRAND_TITLE] = value; }
		}
		/// <summary>追加デザインタグ</summary>
		public string AdditionalDesignTag
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBRAND_ADDITIONAL_DESIGN_TAG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_ADDITIONAL_DESIGN_TAG] = value; }
		}
		/// <summary>SEOキーワード</summary>
		public string SeoKeyword
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBRAND_SEO_KEYWORD]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_SEO_KEYWORD] = value; }
		}
		/// <summary>デフォルト設定フラグ</summary>
		public string DefaultFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBRAND_DEFAULT_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_DEFAULT_FLG] = value; }
		}
		/// <summary>有効フラグ</summary>
		public string ValidFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBRAND_VALID_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_VALID_FLG] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTBRAND_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTBRAND_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTBRAND_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTBRAND_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
