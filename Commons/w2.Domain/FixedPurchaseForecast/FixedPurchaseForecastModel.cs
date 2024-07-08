/*
=========================================================================================================
  Module      : 定期出荷予測モデル (FixedPurchaseForecastModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.FixedPurchaseForecast
{
	/// <summary>
	/// 定期出荷予測モデル
	/// </summary>
	[Serializable]
	public class FixedPurchaseForecastModel : ModelBase<FixedPurchaseForecastModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public FixedPurchaseForecastModel()
		{
			this.FixedPurchaseId = "";
			this.ShopId = "";
			this.ProductId = "";
			this.VariationId = "";
			this.ProductPrice = 0;
			this.ItemQuantity = 0;
			this.DeliveryFrequency = 0;
			this.DeliveryFrequencyByScheduledShippingDate = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseForecastModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public FixedPurchaseForecastModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>対象年月</summary>
		public DateTime TargetMonth
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_TARGET_MONTH]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_TARGET_MONTH] = value; }
		}
		/// <summary>定期購入ID</summary>
		public string FixedPurchaseId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_FIXED_PURCHASE_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_FIXED_PURCHASE_ID] = value; }
		}
		/// <summary>店舗ID</summary>
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
		/// <summary>商品バリエーションID</summary>
		public string VariationId
		{
			get { return (string)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_VARIATION_ID]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_VARIATION_ID] = value; }
		}
		/// <summary>商品金額</summary>
		public decimal ProductPrice
		{
			get { return (decimal)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_PRODUCT_PRICE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_PRODUCT_PRICE] = value; }
		}
		/// <summary>商品数</summary>
		public int ItemQuantity
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_ITEM_QUANTITY]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_ITEM_QUANTITY] = value; }
		}
		/// <summary>配送頻度(配送日基準)</summary>
		public int DeliveryFrequency
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_DATE_CREATED] = value; }
		}
		/// <summary>配送頻度(出荷予定日基準)</summary>
		public int DeliveryFrequencyByScheduledShippingDate
		{
			get { return (int)this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY_BY_SCHEDULED_SHIPPING_DATE]; }
			set { this.DataSource[Constants.FIELD_FIXEDPURCHASEFORECAST_DELIVERY_FREQUENCY_BY_SCHEDULED_SHIPPING_DATE] = value; }
		}
		#endregion
	}
}
