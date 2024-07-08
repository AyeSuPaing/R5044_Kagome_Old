/*
=========================================================================================================
  Module      : 商品税率カテゴリマスタモデル (ProductTaxCategoryModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductTaxCategory
{
	/// <summary>
	/// 商品税率カテゴリマスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductTaxCategoryModel : ModelBase<ProductTaxCategoryModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductTaxCategoryModel()
		{
			this.TaxRate = 0;
			this.DisplayOrder = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductTaxCategoryModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductTaxCategoryModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>税率カテゴリID</summary>
		public string TaxCategoryId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_ID] = value; }
		}
		/// <summary>税率カテゴリ名</summary>
		public string TaxCategoryName
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_NAME]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_CATEGORY_NAME] = value; }
		}
		/// <summary>税率</summary>
		public decimal TaxRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_TAX_RATE] = value; }
		}
		/// <summary>表示順</summary>
		public int DisplayOrder
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DISPLAY_ORDER]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DISPLAY_ORDER] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTTAXCATEGORY_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
