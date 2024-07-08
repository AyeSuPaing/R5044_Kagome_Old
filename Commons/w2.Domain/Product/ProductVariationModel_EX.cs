/*
=========================================================================================================
  Module      : 商品バリエーションマスタモデル (ProductVariationModel.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Util;
using w2.Domain.ProductPrice;
using w2.Domain.ProductStock;

namespace w2.Domain.ProductVariation
{
	/// <summary>
	/// 商品バリエーションマスタモデル
	/// </summary>
	public partial class ProductVariationModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>Variation Id (Not include product id)</summary>
		public string VId
		{
			get { return StringUtility.ToEmpty(this.DataSource["v_id"]); }
		}
		/// <summary>Product prices</summary>
		public ProductPriceModel[] ProductPrices
		{
			get { return (ProductPriceModel[])this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRODUCTPRICE_EXTEND]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_PRODUCTPRICE_EXTEND] = value; }
		}
		/// <summary>Has product prices</summary>
		public bool HasProductPrices
		{
			get { return ((this.ProductPrices != null) && (this.ProductPrices.Length != 0)); }
		}
		/// <summary>Product stock</summary>
		public ProductStockModel ProductStock
		{
			get { return (ProductStockModel)this.DataSource["EX_ProductVariationStock"]; }
			set { this.DataSource["EX_ProductVariationStock"] = value; }
		}
		#endregion
	}
}
