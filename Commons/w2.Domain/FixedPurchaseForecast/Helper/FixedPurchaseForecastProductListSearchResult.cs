/*
=========================================================================================================
  Module      : 定期レポート情報検索結果情報(FixedPurchaseForecastProductListSearchResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FixedPurchaseForecast.Helper
{
	/// <summary>
	/// 定期レポート情報検索結果情報
	/// </summary>
	[Serializable]
	public class FixedPurchaseForecastProductListSearchResult : ModelBase<FixedPurchaseForecastProductListSearchResult>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseForecastProductListSearchResult()
		{
			this.ProductId = "";
			this.ProductName = "";
			this.RowCount = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="drv">クーポン検索結果情報</param>
		public FixedPurchaseForecastProductListSearchResult(DataRowView drv)
			: this(drv.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseForecastProductListSearchResult(Hashtable source)
			: this()
		{
			this.DataSource = source;
			Item = new List<FixedPurchaseForecastItemSearchResult>();
			Item.Add(new FixedPurchaseForecastItemSearchResult(this.DataSource));
		}
		#endregion

		/// <summary>
		/// 追加
		/// </summary>
		/// <param name="source">ソース</param>
		public void AddItem(Hashtable source)
		{
			this.DataSource = source;
			Item.Add(new FixedPurchaseForecastItemSearchResult(this.DataSource));
		}

		/// <summary>ショップID</summary>
		public string ShopId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_SHOP_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_SHOP_ID] = value; }
		}
		/// <summary>商品ID</summary>
		public string ProductId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_PRODUCT_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_PRODUCT_ID] = value; }
		}
		/// <summary>バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_VARIATION_ID] = value; }
		}
		/// <summary>各月の売上予測金額と個数</summary>
		public List<FixedPurchaseForecastItemSearchResult> Item { get; set; }
		/// <summary>商品名</summary>
		public string ProductName
		{
			get { return (string)this.DataSource["product_name"]; }
			set { this.DataSource["product_name"] = value; }
		}
		/// <summary>該当件数</summary>
		public int RowCount
		{
			get { return (int)this.DataSource["row_count"]; }
			set { this.DataSource["row_count"] = value; }
		}
	}
}
