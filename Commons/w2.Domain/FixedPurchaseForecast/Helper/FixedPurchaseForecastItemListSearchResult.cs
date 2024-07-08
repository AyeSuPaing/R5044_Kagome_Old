/*
=========================================================================================================
  Module      : 定期出荷予測アイテム検索結果クラス(FixedPurchaseForecastItemSearchResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FixedPurchaseForecast.Helper
{
	/// <summary>
	/// 定期出荷予測アイテム検索結果クラス
	/// </summary>
	[Serializable]
	public class FixedPurchaseForecastItemSearchResult : ModelBase<FixedPurchaseForecastItemSearchResult>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseForecastItemSearchResult(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseForecastItemSearchResult(Hashtable source)
		{
			this.DataSource = source;

		}
		#endregion

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
		/// <summary>商品名</summary>
		public string ProductName
		{
			get { return (string)this.DataSource["product_name"]; }
			set { this.DataSource["product_name"] = value; }
		}
		/// <summary>対象年月</summary>
		public DateTime TargetMonth
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_TARGET_MONTH]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_TARGET_MONTH] = value; }
		}
		/// <summary>合計金額</summary>
		public decimal Price
		{
			get { return (decimal)this.DataSource["target_month_later_sales"]; }
			set { this.DataSource["target_month_later_sales"] = value; }
		}
		/// <summary>必要在庫</summary>
		public int Stock
		{
			get { return (int)this.DataSource["target_month_later_stock"]; }
			set { this.DataSource["target_month_later_stock"] = value; }
		}
		/// <summary>対象年月</summary>
		public int DeliveryFrequencyByScheduledShippingDate
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY_BY_SCHEDULED_SHIPPING_DATE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY_BY_SCHEDULED_SHIPPING_DATE] = value; }
		}
	}
}
