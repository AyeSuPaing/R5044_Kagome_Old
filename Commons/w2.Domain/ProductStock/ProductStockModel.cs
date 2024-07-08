/*
=========================================================================================================
  Module      : 商品在庫マスタモデル (ProductStockModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.ProductStock
{
	/// <summary>
	/// 商品在庫マスタモデル
	/// </summary>
	[Serializable]
	public partial class ProductStockModel : ModelBase<ProductStockModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductStockModel()
		{
			this.Stock = 0;
			this.StockAlert = 0;
			this.Realstock = 0;
			this.RealstockB = 0;
			this.RealstockC = 0;
			this.RealstockReserved = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductStockModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public ProductStockModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>店舗ID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID] = value; }
		}
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_VARIATION_ID] = value; }
		}
		/// <summary>商品在庫数</summary>
		public int Stock
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK] = value; }
		}
		/// <summary>商品在庫安全基準</summary>
		public int StockAlert
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK_ALERT] = value; }
		}
		/// <summary>実在庫数</summary>
		public int Realstock
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK] = value; }
		}
		/// <summary>実在庫数B</summary>
		public int RealstockB
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_B] = value; }
		}
		/// <summary>実在庫数C</summary>
		public int RealstockC
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_C] = value; }
		}
		/// <summary>引当済実在庫数</summary>
		public int RealstockReserved
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_REALSTOCK_RESERVED] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSTOCK_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_PRODUCTSTOCK_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_DATE_CHANGED] = value; }
		}
		/// <summary>最終更新者</summary>
		public string LastChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_LAST_CHANGED] = value; }
		}
		#endregion
	}
}
