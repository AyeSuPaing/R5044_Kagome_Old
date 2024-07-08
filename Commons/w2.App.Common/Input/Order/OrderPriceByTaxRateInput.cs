/*
=========================================================================================================
  Module      : 税率毎の注文金額情報入力クラス (OrderPriceByTaxRateInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using w2.Domain.Order;

namespace w2.App.Common.Input.Order
{
	/// <summary>
	/// 税率毎の注文金額情報入力クラス（登録、編集で利用）
	/// </summary>
	[Serializable]
	public class OrderPriceByTaxRateInput : InputBase<OrderPriceByTaxRateModel>
	{
		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public OrderPriceByTaxRateInput()
		{
			this.OrderId = string.Empty;
			this.KeyTaxRate = "0";
			this.PriceSubtotalByRate = "0";
			this.PriceShippingByRate = "0";
			this.PricePaymentByRate = "0";
			this.PriceTotalByRate = "0";
			this.TaxPriceByRate = "0";
			this.PriceCorrectionByRate = "0";
			this.DateCreated = DateTime.Now.ToString();
			this.DateChanged = DateTime.Now.ToString();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">モデル</param>
		public OrderPriceByTaxRateInput(OrderPriceByTaxRateModel model)
			: this()
		{
			this.OrderId = model.OrderId;
			this.KeyTaxRate = model.KeyTaxRate.ToString();
			this.PriceSubtotalByRate = model.PriceSubtotalByRate.ToString();
			this.PriceShippingByRate = model.PriceShippingByRate.ToString();
			this.PricePaymentByRate = model.PricePaymentByRate.ToString();
			this.PriceTotalByRate = model.PriceTotalByRate.ToString();
			this.TaxPriceByRate = model.TaxPriceByRate.ToString();
			this.DateCreated = model.DateCreated.ToString();
			this.DateChanged = model.DateChanged.ToString();
			this.PriceCorrectionByRate = model.ReturnPriceCorrectionByRate.ToString();
		}
		#endregion

		#region メソッド
		/// <summary>
		/// モデル作成
		/// </summary>
		/// <returns>モデル</returns>
		public override OrderPriceByTaxRateModel CreateModel()
		{
			var model = new OrderPriceByTaxRateModel
			{
				OrderId = this.OrderId,
				KeyTaxRate = decimal.Parse(this.KeyTaxRate),
				PriceSubtotalByRate = decimal.Parse(this.PriceSubtotalByRate),
				PriceShippingByRate = decimal.Parse(this.PriceShippingByRate),
				PricePaymentByRate = decimal.Parse(this.PricePaymentByRate),
				PriceTotalByRate = decimal.Parse(this.PriceTotalByRate),
				TaxPriceByRate = decimal.Parse(this.TaxPriceByRate),
				ReturnPriceCorrectionByRate = decimal.Parse(this.PriceCorrectionByRate),
				DateCreated = DateTime.Parse(this.DateCreated),
				DateChanged = DateTime.Parse(this.DateChanged),
			};
			return model;
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
		public string KeyTaxRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE] = value; }
		}
		/// <summary>税込商品合計額</summary>
		public string PriceSubtotalByRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SUBTOTAL_BY_RATE] = value; }
		}
		/// <summary>税込配送料合計額</summary>
		public string PriceShippingByRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_SHIPPING_BY_RATE] = value; }
		}
		/// <summary>税込決済手数料合計額</summary>
		public string PricePaymentByRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_PAYMENT_BY_RATE] = value; }
		}
		/// <summary>税込合計額</summary>
		public string PriceTotalByRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_TOTAL_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_PRICE_TOTAL_BY_RATE] = value; }
		}
		/// <summary>税額</summary>
		public string TaxPriceByRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_TAX_PRICE_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_TAX_PRICE_BY_RATE] = value; }
		}
		/// <summary>作成日</summary>
		public string DateCreated
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_DATE_CREATED]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_DATE_CREATED] = value; }
		}
		/// <summary>更新日</summary>
		public string DateChanged
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_DATE_CHANGED]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_DATE_CHANGED] = value; }
		}
		/// <summary>返品用金額補正</summary>
		public string PriceCorrectionByRate
		{
			get { return (string)this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE]; }
			set { this.DataSource[Constants.FIELD_ORDERPRICEBYTAXRATE_RETURN_PRICE_CORRECTION_BY_RATE] = value; }
		}
		#endregion
	}
}
