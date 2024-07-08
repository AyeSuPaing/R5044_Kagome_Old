/*
=========================================================================================================
  Module      : カート商品毎価格情報クラス(CartPriceInfoByTaxRate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.Order;

namespace w2.App.Common.Order
{
	/// <summary>
	/// カート商品毎価格情報クラス
	/// </summary>
	[Serializable]
	public class CartPriceInfoByTaxRate
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CartPriceInfoByTaxRate()
		{
			this.OrderId = "";
			this.TaxRate = 0;
			this.PriceSubtotal = 0;
			this.PriceShipping = 0;
			this.PricePayment = 0;
			this.PriceTotal = 0;
			this.TaxPrice = 0;
			this.ReturnPriceCorrection = 0;
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="model">税率毎価格情報モデル</param>
		public CartPriceInfoByTaxRate(OrderPriceByTaxRateModel model)
		{
			this.TaxRate = model.KeyTaxRate;
			this.PriceSubtotal = model.PriceSubtotalByRate;
			this.PriceShipping = model.PriceShippingByRate;
			this.PricePayment = model.PricePaymentByRate;
			this.PriceTotal = model.PriceTotalByRate;
			this.TaxPrice = model.TaxPriceByRate;
			this.ReturnPriceCorrection = model.ReturnPriceCorrectionByRate;
		}
		#endregion

		/// <summary>
		/// カート商品毎価格情報から商品毎価格情報モデルを作成
		/// </summary>
		/// <returns>注文商品情報</returns>
		public OrderPriceByTaxRateModel CreateModel()
		{
			var model = new OrderPriceByTaxRateModel
			{
				OrderId = this.OrderId,
				KeyTaxRate = this.TaxRate,
				PriceSubtotalByRate = this.PriceSubtotal,
				PriceShippingByRate = this.PriceShipping,
				PricePaymentByRate = this.PricePayment,
				PriceTotalByRate = this.PriceTotal,
				TaxPriceByRate = this.TaxPrice,
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				ReturnPriceCorrectionByRate = this.ReturnPriceCorrection,
			};
			return model;
		}

		/// <summary>
		/// シャローコピー
		/// </summary>
		public CartPriceInfoByTaxRate Clone()
		{
			var newCartPriceInfoByTaxRate = (CartPriceInfoByTaxRate)this.MemberwiseClone();
			return newCartPriceInfoByTaxRate;
		}

		/// <summary>注文ID</summary>
		public string OrderId { get; set; }
		/// <summary>税率</summary>
		public decimal TaxRate { get; set; }
		/// <summary>税込商品合計額</summary>
		public decimal PriceSubtotal { get; set; }
		/// <summary>税込配送料合計額</summary>
		public decimal PriceShipping { get; set; }
		/// <summary>税込決済手数料合計額</summary>
		public decimal PricePayment { get; set; }
		/// <summary>税込合計額</summary>
		public decimal PriceTotal { get; set; }
		/// <summary>税額</summary>
		public decimal TaxPrice { get; set; }
		/// <summary>返品用金額補正</summary>
		public decimal ReturnPriceCorrection { get; set; }
	}
}
