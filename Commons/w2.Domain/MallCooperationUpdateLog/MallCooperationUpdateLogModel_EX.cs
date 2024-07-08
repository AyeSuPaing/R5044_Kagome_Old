/*
=========================================================================================================
  Module      : モール連携更新ログモデル (MallCooperationUpdateLogModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.MallCooperationUpdateLog
{
	/// <summary>
	/// モール連携更新ログモデル
	/// </summary>
	public partial class MallCooperationUpdateLogModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>商品バリエーション連携ID1</summary>
		public string VariationCooperationId1
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID1] = value; }
		}
		/// <summary>商品バリエーション連携ID2</summary>
		public string VariationCooperationId2
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID2] = value; }
		}
		/// <summary>商品バリエーション連携ID3</summary>
		public string VariationCooperationId3
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID3] = value; }
		}
		/// <summary>商品バリエーション連携ID4</summary>
		public string VariationCooperationId4
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID4] = value; }
		}
		/// <summary>商品バリエーション連携ID5</summary>
		public string VariationCooperationId5
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID5] = value; }
		}
		/// <summary>商品バリエーション連携ID6</summary>
		public string VariationCooperationId6
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID6] = value; }
		}
		/// <summary>商品バリエーション連携ID7</summary>
		public string VariationCooperationId7
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID7] = value; }
		}
		/// <summary>商品バリエーション連携ID8</summary>
		public string VariationCooperationId8
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID8] = value; }
		}
		/// <summary>商品バリエーション連携ID9</summary>
		public string VariationCooperationId9
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID9] = value; }
		}
		/// <summary>商品バリエーション連携ID10</summary>
		public string VariationCooperationId10
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10]; }
			set { this.DataSource[Constants.FIELD_PRODUCTVARIATION_VARIATION_COOPERATION_ID10] = value; }
		}
		/// <summary>商品在庫数</summary>
		public int Stock
		{
			get { return (int)this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK]; }
			set { this.DataSource[Constants.FIELD_PRODUCTSTOCK_STOCK] = value; }
		}
		/// <summary>複数バリエーション使用フラグ</summary>
		public string UseVariationFlg
		{
			get { return (string)this.DataSource[Constants.FIELD_PRODUCT_USE_VARIATION_FLG]; }
			set { this.DataSource[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] = value; }
		}
		/// <summary>AmazonSKU（バリエーションあり）</summary>
		public string AmazonSkuUseVariation
		{
			get { return (string)this.DataSource["amazon_sku_use_variation"]; }
			set { this.DataSource["amazon_sku_use_variation"] = value; }
		}
		/// <summary>AmazonSKU（バリエーションなし）</summary>
		public string AmazonSkuNoVariation
		{
			get { return (string)this.DataSource["amazon_sku_no_variation"]; }
			set { this.DataSource["amazon_sku_no_variation"] = value; }
		}
		/// <summary>出荷作業日数（バリエーションあり）※Amazon在庫連携に使用</summary>
		public string FulfillmentLatencyUseVariation
		{
			get { return (string)this.DataSource["fulfillmentlatency_use_variation"]; }
			set { this.DataSource["fulfillmentlatency_use_variation"] = value; }
		}
		/// <summary>出荷作業日数（バリエーションなし）※Amazon在庫連携に使用</summary>
		public string FulfillmentLatencyNoVariation
		{
			get { return (string)this.DataSource["fulfillmentlatency_no_variation"]; }
			set { this.DataSource["fulfillmentlatency_no_variation"] = value; }
		}
		#endregion
	}
}
