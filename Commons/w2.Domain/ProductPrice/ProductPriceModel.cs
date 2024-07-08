/*
=========================================================================================================
  Module      : 商品価格マスタモデル (ProductPriceModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;
using w2.Common.Util;

namespace w2.Domain.ProductPrice
{
	/// <summary>
	/// 商品価格マスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductPriceModel : ModelBase<ProductPriceModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductPriceModel()
		{
			this.ShopId = string.Empty;
			this.ProductId = string.Empty;
			this.VariationId = string.Empty;
			this.MemberRankId = string.Empty;
			this.MemberRankPrice = 0m;
			this.LastChanged = string.Empty;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductPriceModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductPriceModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTPRICE_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTPRICE_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTPRICE_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTPRICE_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return StringUtility.ToEmpty(this.DataSource[Constants.FIELD_PRODUCTPRICE_VARIATION_ID]); }
			set { this.DataSource[Constants.FIELD_PRODUCTPRICE_VARIATION_ID] = value; }
		}
		/// <summary>会員ランクID</summary>
		public string MemberRankId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_ID] = value; }
		}
		/// <summary>会員価格</summary>
		public decimal? MemberRankPrice
		{
			get
			{
				if (this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE] == DBNull.Value) return null;
				return (decimal)this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE];
			}
			set { this.DataSource[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTPRICE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTPRICE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTPRICE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTPRICE_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTPRICE_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTPRICE_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
