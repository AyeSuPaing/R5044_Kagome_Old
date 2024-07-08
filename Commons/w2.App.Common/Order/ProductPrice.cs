/*
=========================================================================================================
  Module      : 商品価格制御クラス(ProductPrice.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.Common.Util;
using w2.Domain;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 商品価格制御クラス
	/// </summary>
	public class ProductPrice
	{
		/// <summary>
		/// 商品価格タイプ
		/// </summary>
		public enum PriceTypes
		{
			/// <summary>通常価格</summary>
			Normal,
			/// <summary>特別価格</summary>
			Special,
			/// <summary>タイムセール</summary>
			TimeSale,
			/// <summary>会員ランク価格</summary>
			MemberRankPrice,
			/// <summary>闇市価格</summary>
			ClosedMarketPrice,
			/// <summary>定期購入通常価格</summary>
			FixedPurchasePrice,
			/// <summary>定期購入初回価格</summary>
			FixedPurchaseFirsttimePrice
		}

		/// <summary>
		/// 定期購入 商品価格タイプ取得
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="targetVariation">バリエーション対象</param>
		/// <param name="firstTime">定期初回購入有無</param>
		/// <returns>商品価格タイプ</returns>
		public static PriceTypes GetFixedPurchaseProductPriceType(object product, bool targetVariation, bool firstTime = false)
		{
			if ((firstTime)
				&& (((targetVariation == false) && (HasValue(product, Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE)))
					|| ((targetVariation) && (HasValue(product, Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE)))))
			{
				return PriceTypes.FixedPurchaseFirsttimePrice;
			}
			else if (((targetVariation == false) && HasValue(product, Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE))
				|| ((targetVariation) && (HasValue(product, Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE))))
			{
				return PriceTypes.FixedPurchasePrice;
			}
			else
			{
				// 定期初回価格もしくは定期通常価格が設定されていない場合、通常購入の商品価格を定期購入価格とする
				return (GetProductPriceType(product, targetVariation));
			}
		}

		/// <summary>
		/// 定期購入 商品価格取得
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="targetVariation">バリエーション対象</param>
		/// <param name="firstTime">定期初回購入有無</param>
		/// <returns>定期購入価格</returns>
		public static string GetFixedPurchasePrice(object product, bool targetVariation, bool firstTime = false)
		{
			string fixedPurchasePrice;

			var applyPriceType = GetFixedPurchaseProductPriceType(product, targetVariation, firstTime);
			switch (applyPriceType)
			{
				case PriceTypes.FixedPurchaseFirsttimePrice:
					fixedPurchasePrice = StringUtility.ToEmpty(ProductCommon.GetKeyValue(
						product,
						(targetVariation) ? Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_FIRSTTIME_PRICE : Constants.FIELD_PRODUCT_FIXED_PURCHASE_FIRSTTIME_PRICE));
					break;

				case PriceTypes.FixedPurchasePrice:
					fixedPurchasePrice = StringUtility.ToEmpty(ProductCommon.GetKeyValue(
						product,
						(targetVariation) ? Constants.FIELD_PRODUCTVARIATION_VARIATION_FIXED_PURCHASE_PRICE : Constants.FIELD_PRODUCT_FIXED_PURCHASE_PRICE));
					break;

				case PriceTypes.MemberRankPrice:
					fixedPurchasePrice = StringUtility.ToEmpty(ProductCommon.GetKeyValue(
						product,
						(targetVariation) ? Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION : Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE));
					break;

				case PriceTypes.TimeSale:
					fixedPurchasePrice = StringUtility.ToEmpty(ProductCommon.GetKeyValue(
						product,
						Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE));
					break;

				case PriceTypes.ClosedMarketPrice:
					fixedPurchasePrice = StringUtility.ToEmpty(ProductCommon.GetKeyValue(
						product,
						"closed_market_price"));
					break;

				case PriceTypes.Special:
					fixedPurchasePrice = StringUtility.ToEmpty(ProductCommon.GetKeyValue(
						product,
						(targetVariation) ? Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE : Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE));
					break;

				default:
					fixedPurchasePrice = ProductCommon.GetProductPriceNumeric(product, targetVariation);
					break;
			}

			// バリエーションの場合で定期購入価格の取得ができなかった場合バリエーションではない通常購入の価格を定期購入の価格とする
			return ((targetVariation == false) || (fixedPurchasePrice != null))
				? fixedPurchasePrice
				: GetFixedPurchasePrice(product, false);
		}

		/// <summary>
		/// 商品価格タイプ取得
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="targetVariation">バリエーション対象</param>
		/// <returns>商品価格タイプ</returns>
		public static PriceTypes GetProductPriceType(object product, bool targetVariation)
		{
			if (HasValue(product, "closed_market_price"))
			{
				return PriceTypes.ClosedMarketPrice;
			}
			else if (((targetVariation == false) && HasValue(product, Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE))
				|| (targetVariation && HasValue(product, Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION)))
			{
				return PriceTypes.MemberRankPrice;
			}
			else if ((HasValue(product, Constants.FIELD_PRODUCTSALEPRICE_SALE_PRICE))
				&& ((HasValue(product, "validity") && ((string)GetValue(product, "validity") == "1"))
					|| ((HasValue(product, "validity") == false))))
			{
				return PriceTypes.TimeSale;
			}
			else if (((targetVariation == false) && HasValue(product, Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE))
				|| (targetVariation && HasValue(product, Constants.FIELD_PRODUCTVARIATION_SPECIAL_PRICE)))
			{
				return PriceTypes.Special;
			}
			else
			{
				return PriceTypes.Normal;
			}
		}

		/// <summary>
		/// 値取得(あることがの前提）
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>値</returns>
		private static object GetValue(object product, string fieldName)
		{
			if (product is DataRowView)
			{
				return ((DataRowView)product)[fieldName];
			}
			else if (product is Hashtable)
			{
				return ((Hashtable)product)[fieldName];
			}
			else if (product is Dictionary<string, object>)
			{
				return ((Dictionary<string, object>)product)[fieldName];
			}
			if (product is IModel)
			{
				return ((IModel)product).DataSource[fieldName];
			}
			throw new ArgumentException("パラメタエラー: objProduct is [" + product.GetType().ToString() + "]");
		}

		/// <summary>
		/// 値を持っているか
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>値があるか無いか</returns>
		private static bool HasValue(object product, string fieldName)
		{
			if (product is DataRowView)
			{
				return HasValue((DataRowView)product, fieldName);
			}
			else if (product is Hashtable)
			{
				return HasValue((Hashtable)product, fieldName);
			}
			else if (product is Dictionary<string, object>)
			{
				return HasValue((Dictionary<string, object>)product, fieldName);
			}
			else if (product is IModel)
			{
				return HasValue(((IModel)product).DataSource, fieldName);
			}

			throw new ArgumentException("パラメタエラー: objProduct is [" + product.GetType().ToString() + "]");
		}
		/// <summary>
		/// 値を持っているか
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>値があるか無いか</returns>
		private static bool HasValue(DataRowView product, string fieldName)
		{
			return (product.Row.Table.Columns.Contains(fieldName)
				&& (product[fieldName] != DBNull.Value));
		}
		/// <summary>
		/// 値を持っているか
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="fieldName">フィールド名</param>
		/// <returns>値があるか無いか</returns>
		private static bool HasValue(IDictionary product, string fieldName)
		{
			return (product.Contains(fieldName)
				&& (product[fieldName] != DBNull.Value));
		}
	}
}
