/*
=========================================================================================================
  Module      : 注文商品情報取得／注文メール／ヤフー (YahooOrderItem.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using w2.App.Common.Option;
using w2.Commerce.MallBatch.MailOrderGetter.Process.Base;
using w2.Domain.Product;
using w2.Domain.ProductTaxCategory;

namespace w2.Commerce.MallBatch.MailOrderGetter.Process.Yahoo
{
	///**************************************************************************************
	/// <summary>
	/// ヤフー注文商品解析クラス
	/// </summary>
	///**************************************************************************************
	public class YahooOrderItem : BaseProcess
	{
		const string PRICE_YEN = "円×";
		const string EQUAL = "＝";
		const string SUBTOTAL_YEN = "円";
		
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strOrderItem">注文商品</param>
		/// <param name="shopId">注文店舗ID</param>
		public YahooOrderItem(string strOrderItem, string shopId)
		{
			InitYahooOrderItem(strOrderItem, shopId);
		}
		/// <summary>
		/// 注文レコードを解析し、注文明細を構築する
		/// </summary>
		/// <param name="strOrderItem">注文商品</param>
		/// <param name="shopId">注文店舗ID</param>
		private void InitYahooOrderItem(string strOrderItem, string shopId)
		{
			// 行単位で分割
			string[] splitters = new string[] { "\r\n" };
			string[] itemInfo = strOrderItem.Split(splitters, StringSplitOptions.RemoveEmptyEntries);

			// 3行未満だったらデータがおかしい
			if (itemInfo.Length < 3) throw new Exception();

			// 1行目：商品名
			string productName = itemInfo[0].Replace(Regex.Match(itemInfo[0], "（[0-9]+）").Value, "");
			// 3行目～最後から2行目まで：オプション情報（バリエーション情報）⇒商品名に結合
			if (itemInfo.Length > 3)
			{
				// オプション情報の行だけ抽出
				int optionCount = itemInfo.Length - 3;
				string[] option = new string[optionCount];
				Array.Copy(itemInfo, 2, option, 0, optionCount);
				
				// 商品情報の後ろに追加
				productName += "(" + string.Join("、", option) + ")";
			}
			this.ProductName = productName.Length > 200 ? productName.Substring(0, 200) : productName;

			// 2行目：商品ID、バリエーションID
			string[] ids = itemInfo[1].Split('/');
			this.ProductId = ids[0];
			this.VariationId = ids.Length > 1 ? ids[1] : ids[0];
			var product = new ProductService().GetProductVariation(shopId, this.ProductId, this.VariationId, string.Empty);
			var productTaxCategory = new ProductTaxCategoryService().Get(product.TaxCategoryId);

			// 最終行：金額、数量
			string temp = itemInfo[itemInfo.Length - 1].Replace(",", "");
			var productPrice = int.Parse(Regex.Match(temp, @"[0-9]+" + PRICE_YEN).ToString().Replace(PRICE_YEN, ""));
			var productPriceTax = TaxCalculationUtility.GetTaxPrice(
				productPrice,
				productTaxCategory.TaxRate,
				Constants.TAX_ROUNDTYPE,
				true);

			this.TaxRate = productTaxCategory.TaxRate;
			this.Price = TaxCalculationUtility.GetPrescribedPrice(productPrice, productPriceTax, true);
			this.PricePreTax = productPrice;
			this.PriceTax = productPriceTax;
			this.Quantity = int.Parse(Regex.Match(temp, @"[0-9]+" + EQUAL).ToString().Replace(EQUAL, ""));
			this.SubTotal = this.Price * this.Quantity;
		}

		/// <summary>商品名</summary>
		public string ProductName { get; private set; }
		/// <summary>商品ID</summary>
		public string ProductId { get; private set; }
		/// <summary>バリエーションID</summary>
		public string VariationId { get; private set; }
		/// <summary>単価</summary>
		public decimal Price { get; private set; }
		/// <summary>税込単価</summary>
		public decimal PricePreTax { get; private set; }
		/// <summary>単価消費税</summary>
		public decimal PriceTax { get; private set; }
		/// <summary>税率</summary>
		public decimal TaxRate { get; private set; }
		/// <summary>個数</summary>
		public decimal Quantity { get; private set; }
		/// <summary>小計</summary>
		public decimal SubTotal { get; private set; }
		/// <summary>小計消費税額</summary>
		public decimal TaxSubTotal { get { return this.PriceTax * this.Quantity; } }
		/// <summary>調整金額(小計価格で按分適用後)</summary>
		public decimal ItemPriceRegulation { get; set; }
	}
}
