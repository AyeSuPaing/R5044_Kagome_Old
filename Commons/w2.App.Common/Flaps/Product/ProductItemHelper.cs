/*
=========================================================================================================
  Module      : FLAPS商品情報ヘルパークラス (ProductItemHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.Common.Logger;
using w2.Domain.Product;
using w2.Domain.ProductVariation;

namespace w2.App.Common.Flaps.Product
{
	/// <summary>
	/// FLAPS商品情報ヘルパー
	/// </summary>
	public partial class ProductItem
	{
		/// <summary>
		/// 商品モデルへ変換
		/// </summary>
		private void ConvertIntoProductModel()
		{
			var taxedStdPrice = 0m;
			if (decimal.TryParse(this.TaxedStdPrice, out taxedStdPrice) == false)
			{
				var msg = string.Format(
					"TaxedStdPriceの値が正しいか確認してください。Code: {0},TaxedStdPrice: {1}",
					this.Code,
					this.TaxedStdPrice);
				Console.WriteLine(msg);
				FileLogger.WriteWarn(msg);
			}
			var weight = 0;
			if (int.TryParse(this.Weight, out weight) == false)
			{
				var msg = string.Format(
					"Weightの値が正しいか確認してください。Code: {0},Weight: {1}",
					this.Code,
					this.Weight);
				Console.WriteLine(msg);
				FileLogger.WriteWarn(msg);
			}

			// 存在する配送種別設定を取得し、その要素番号1番の設定を設定しておく
			// 配送種別設定が一つも存在しない場合は仮の値を格納する
			var shippings = DataCacheControllerFacade.GetShopShippingCacheController().CacheData;
			var defaultShippingId = "0";
			if (shippings.Any() == false)
			{
				var warning = string.Format(
					"EC管理画面の商品情報詳細を開けない可能性があります。商品情報詳細を開く一時的に配送種別IDが'{0}'の配送種別設定を新規作成してください。",
					defaultShippingId);
				FileLogger.WriteWarn(warning);
			}
			var shippingId = shippings.Any() ? shippings[0].ShippingId : defaultShippingId;
			var product = new ProductModel
			{
				ProductId = this.StyleCode,
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				Name = this.Name,
				CategoryId1 = this.BrandCode,
				CategoryId2 = this.LargeCategoryCode,
				CategoryId3 = this.MiddleCategoryCode,
				CategoryId4 = this.SmallCategoryCode,
				StockManagementKbn = Constants.FLG_PRODUCT_STOCK_MANAGEMENT_KBN_DISPOK_BUYOK,
				DisplayPrice = taxedStdPrice,
				ShippingType = shippingId,
				ProductWeightGram = weight,
				UseVariationFlg = this.UseVariationFlg,
				LastChanged = Constants.FLG_LASTCHANGED_FLAPS,
			};
			product.DataSource[Constants.FLAPS_PRODUCT_COOPIDS_FOR_SERNO_AND_BARCODE[0]] = this.SerNo;
			product.DataSource[Constants.FLAPS_PRODUCT_COOPIDS_FOR_SERNO_AND_BARCODE[1]] = this.Barcode;
			this.ProductModel = product;
		}

		/// <summary>
		/// 商品バリエーションモデルへ変換
		/// </summary>
		private void ConvertIntoProductVariationModel()
		{
			var taxedStdPrice = 0m;
			if (decimal.TryParse(this.TaxedStdPrice, out taxedStdPrice) == false)
			{
				var msg = string.Format(
					"TaxedStdPriceの値が正しいか確認してください。Code: {0},TaxedStdPrice: {1}",
					this.Code,
					this.TaxedStdPrice);
				Console.WriteLine(msg);
				FileLogger.WriteWarn(msg);
			}
			var weight = 0;
			if (int.TryParse(this.Weight, out weight) == false)
			{
				var msg = string.Format(
					"Weightの値が正しいか確認してください。Code: {0},Weight: {1}",
					this.Code,
					this.Weight);
				Console.WriteLine(msg);
				FileLogger.WriteWarn(msg);
			}

			var pv = new ProductVariationModel
			{
				ProductId = this.StyleCode,
				VariationId = this.Code,
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				VariationColorId = this.ColorCode,
				VariationName1 = this.ColorName,
				VariationName2 = this.SizeName,
				Price = taxedStdPrice,
				VariationWeightGram = weight,
				LastChanged = Constants.FLG_LASTCHANGED_FLAPS,
			};
			pv.DataSource[Constants.FLAPS_PRODUCTVARIATION_COOPIDS_FOR_SERNO_AND_BARCODE[0]] = this.SerNo;
			pv.DataSource[Constants.FLAPS_PRODUCTVARIATION_COOPIDS_FOR_SERNO_AND_BARCODE[1]] = this.Barcode;
			this.ProductVariationModel = pv;
		}

		/// <summary>
		/// 更新
		/// </summary>
		/// <returns>更新結果</returns>
		public bool Update()
		{
			var msg = string.Format("FLAPSから商品取込 variation_id: {0}", this.Code);
			Console.WriteLine(msg);
			FileLogger.WriteDebug(msg);

			ConvertIntoProductModel();
			ConvertIntoProductVariationModel();

			var result = new ProductService().UpsertForFlapsIntegration(
				this.ProductModel,
				this.ProductVariationModel);
			return result;
		}
		
	}
}
