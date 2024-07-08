/*
=========================================================================================================
  Module      : 税率毎の注文金額情報モデル (OrderPriceByTaxRateModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using w2.Common.Extensions;

namespace w2.Domain.Order
{
	/// <summary>
	/// 税率毎の注文金額情報モデル
	/// </summary>
	[Serializable]
	public partial class OrderPriceByTaxRateModel : ModelBase<OrderPriceByTaxRateModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderPriceByTaxRateModel()
		{
			this.OrderId = string.Empty;
			this.KeyTaxRate = 0;
			this.PriceSubtotalByRate = 0;
			this.PriceShippingByRate = 0;
			this.PricePaymentByRate = 0;
			this.PriceTotalByRate = 0;
			this.TaxPriceByRate = 0;
			this.ReturnPriceCorrectionByRate = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderPriceByTaxRateModel(DataRowView source)
			: this(source.ToHashtable())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="source">ソース</param>
		public OrderPriceByTaxRateModel(Hashtable source)
			: this()
		{
			this.DataSource = source;
		}
		#endregion

		#region プロパティ
		/// <summary>注文ID</summary>
		public string OrderId
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_ORDER_ID]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_ORDER_ID] = value; }
		}
		/// <summary>税率</summary>
		public decimal KeyTaxRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE] = value; }
		}
		/// <summary>税込商品合計額</summary>
		public decimal PriceSubtotalByRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE] = value; }
		}
		/// <summary>税込配送料合計額</summary>
		public decimal PriceShippingByRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE] = value; }
		}
		/// <summary>税込決済手数料合計額</summary>
		public decimal PricePaymentByRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE] = value; }
		}
		/// <summary>税込合計額</summary>
		public decimal PriceTotalByRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_TOTAL_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_TOTAL_BY_RATE] = value; }
		}
		/// <summary>税額</summary>
		public decimal TaxPriceByRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_TAX_PRICE_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_TAX_PRICE_BY_RATE] = value; }
		}
		/// <summary>作成日</summary>
		public DateTime DateCreated
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public DateTime DateChanged
		{
			get { return (DateTime)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_DATE_CHANGED] = value; }
		}
		/// <summary>商品価格調整金額</summary>
		public decimal ReturnPriceCorrectionByRate
		{
			get { return (decimal)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE] = value; }
		}
		#endregion
	}
}
