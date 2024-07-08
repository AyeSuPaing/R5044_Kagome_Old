/*
=========================================================================================================
  Module      : リアル店舗商品在庫情報モデル (RealShopProductStockModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.RealShop
{
	/// <summary>
	/// リアル店舗商品在庫情報モデル
	/// </summary>
	[Serializable]
	public partial class RealShopProductStockModel : ModelBase<RealShopProductStockModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public RealShopProductStockModel()
		{
			this.RealShopId = "";
			this.ProductId = "";
			this.VariationId = "";
			this.RealShopStock = 0;
			this.LastChanged = "";
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RealShopProductStockModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public RealShopProductStockModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>リアル店舗ID</summary>
		public string RealShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_VARIATION_ID] = value; }
		}
		/// <summary>リアル店舗商品在庫数</summary>
		public int RealShopStock
		{
			get { return (int)this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK]; }
			set { this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_REAL_SHOP_STOCK] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_REALSHOPPRODUCTSTOCK_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
