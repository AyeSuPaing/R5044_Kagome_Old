/*
=========================================================================================================
  Module      : 2回目以降定期商品入力情報チェッククラス(ProductFixedPurchaseNextShippingSettingChecker.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Order;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.Product;

namespace w2.App.Common.Product
{
	/// <summary>
	/// 2回目以降定期商品入力情報チェッククラス
	/// </summary>
	public class ProductFixedPurchaseNextShippingSettingChecker
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="shippingType">配送種別</param>
		/// <param name="nextShippingProductId">定期購入2回目以降配送商品ID</param>
		/// <param name="nextShippingVariationId">定期購入2回目以降配送商品バリエーションID</param>
		/// <param name="nextShippingItemQuantity">定期購入2回目以降配送商品注文個数</param>
		/// <param name="nextShippingItemKbn">定期購入2回目以降配送商品定期購入区分</param>
		/// <param name="nextShippingItemSetting">定期購入2回目以降配送商品定期購入設定</param>
		public ProductFixedPurchaseNextShippingSettingChecker(
			string shopId,
			string productId,
			string shippingType,
			string nextShippingProductId,
			string nextShippingVariationId,
			string nextShippingItemQuantity,
			string nextShippingItemKbn,
			string nextShippingItemSetting)
		{
			this.ShopId = shopId;
			this.ProductId = productId;
			this.ShippingType = shippingType;
			this.NextShippingProductId = nextShippingProductId;
			this.NextShippingVariationId = nextShippingVariationId;
			this.NextShippingItemQuantity = nextShippingItemQuantity;
			this.NextShippingItemFixedPurchaseKbn = nextShippingItemKbn;
			this.NextShippingItemFixedPurchaseSetting = nextShippingItemSetting;
		}

		/// <summary>
		/// 定期購入2回目以降定期商品情報入力チェック
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public Validator.ErrorMessageList Check()
		{
			var errorMessages = new Validator.ErrorMessageList();

			if (string.IsNullOrEmpty(this.NextShippingProductId)
				&& string.IsNullOrEmpty(this.NextShippingVariationId))
			{
				return errorMessages;
			}

			if (string.IsNullOrEmpty(this.NextShippingProductId)
				&& (string.IsNullOrEmpty(this.NextShippingVariationId) == false))
			{
				errorMessages.Add(
					CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_PRODUCT_INVALID,
					MessageManager.GetMessages(CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_PRODUCT_INVALID));
				return errorMessages;
			}

			// 定期購入2回目以降配送商品が存在するかを確認
			var nextShippingProductInfo = ProductCommon.GetProductVariationInfo(
				this.ShopId,
				this.NextShippingProductId,
				this.NextShippingVariationId,
				string.Empty);
			if (nextShippingProductInfo.Count == 0)
			{
				errorMessages.Add(
					CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_PRODUCT_INVALID,
					MessageManager.GetMessages(CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_PRODUCT_INVALID));
				return errorMessages;
			}

			// 定期購入2回目以降配送商品の定期購入フラグ確認
			var nextShippingProductFixedPurchaseFlg = StringUtility.ToEmpty(ProductCommon.GetKeyValue(nextShippingProductInfo[0], Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG));
			if (nextShippingProductFixedPurchaseFlg == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
			{
				errorMessages.Add(
					CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_FIXED_PURCHASE_DISABLE,
					MessageManager.GetMessages(
						CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_FIXED_PURCHASE_DISABLE));
			}

			// 定期購入2回目以降配送商品注文個数確認
			if (string.IsNullOrEmpty(this.NextShippingItemQuantity))
			{
				errorMessages.Add(
					CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY_NECESSARY,
					MessageManager.GetMessages(
						CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY_NECESSARY));
			}
			else
			{
				int itemQuantityNumber;
				if (int.TryParse(this.NextShippingItemQuantity, out itemQuantityNumber))
				{
					if (itemQuantityNumber == 0)
					{
						errorMessages.Add(
							CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY_NUMBER_MIN,
							MessageManager.GetMessages(
								CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_ITEM_QUANTITY_NUMBER_MIN));
					}
				}
			}

			// 定期購入2回目以降配送商品に異なる配送種別の商品が設定されているか確認
			var shippingTypeOfProductMaster = string.IsNullOrEmpty(this.ShippingType)
				? new ProductService().Get(this.ShopId, this.ProductId).ShippingType
				: this.ShippingType;
			var shippingTypeOfProductFixedPurchaseNextShippingSetting =
				StringUtility.ToEmpty(ProductCommon.GetKeyValue(nextShippingProductInfo[0], Constants.FIELD_PRODUCT_SHIPPING_TYPE));
			if (shippingTypeOfProductMaster != shippingTypeOfProductFixedPurchaseNextShippingSetting)
			{
				errorMessages.Add(
					CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SHIPPING_KBN_DIFF,
					MessageManager.GetMessages(
						CommerceMessages.ERRMSG_FIXED_PURCHASE_NEXT_SHIPPING_PRODUCT_SHIPPING_KBN_DIFF));
			}

			// 定期購入2回目以降配送商品の定期購入区分と定期購入設定が片方だけ設定された場合はエラー
			if (string.IsNullOrEmpty(this.NextShippingItemFixedPurchaseKbn) != string.IsNullOrEmpty(this.NextShippingItemFixedPurchaseSetting))
			{
				errorMessages.Add(
					CommerceMessages.ERRMSG_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING_NOT_SET,
					MessageManager.GetMessages(
						CommerceMessages.ERRMSG_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING_NOT_SET));
			}

			// 定期購入２回目配送パターン入力チェック
			if ((string.IsNullOrEmpty(this.NextShippingItemFixedPurchaseKbn) == false)
				&& (string.IsNullOrEmpty(this.NextShippingItemFixedPurchaseSetting) == false))
			{
				var fixedPurchaseSettingValues = new Dictionary<string, int>
				{
					{ Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE, FixedPurchaseModel.FLG_FIXED_PURCHASE_SETTING_MONTHLY_DATE_COUNT },
					{ Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY,FixedPurchaseModel.FLG_FIXED_PURCHASE_SETTING_MONTHLY_WEEKANDDAY_COUNT },
					{ Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS, FixedPurchaseModel.FLG_FIXED_PURCHASE_SETTING_INTERVAL_BY_DAYS_COUNT },
					{ Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY, FixedPurchaseModel.FLG_FIXED_PURCHASE_SETTING_WEEK_AND_DAY_COUNT }
				};
				if ((fixedPurchaseSettingValues.ContainsKey(this.NextShippingItemFixedPurchaseKbn) == false)
					|| (fixedPurchaseSettingValues[this.NextShippingItemFixedPurchaseKbn] != this.NextShippingItemFixedPurchaseSetting.Split(',').Length))
				{
					errorMessages.Add(
						CommerceMessages.ERRMSG_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING_NOT_SET,
						MessageManager.GetMessages(
							CommerceMessages.ERRMSG_NEXT_SHIPPING_ITEM_FIXED_PURCHASE_SETTING_NOT_SET));
				}
			}

			return errorMessages;
		}

		/// <summary>店舗ID</summary>
		private string ShopId { get; set; }
		/// <summary>商品ID</summary>
		private string ProductId { get; set; }
		/// <summary>配送種別</summary>
		private string ShippingType { get; set; }
		/// <summary>定期購入2回目以降配送商品ID</summary>
		private string NextShippingProductId { get; set; }
		/// <summary>定期購入2回目以降配送商品バリエーションID</summary>
		private string NextShippingVariationId { get; set; }
		/// <summary>定期購入2回目以降配送商品注文個数</summary>
		private string NextShippingItemQuantity { get; set; }
		/// <summary>定期購入2回目以降配送商品定期購入区分</summary>
		private string NextShippingItemFixedPurchaseKbn { get; set; }
		/// <summary>定期購入2回目以降配送商品定期購入設定</summary>
		private string NextShippingItemFixedPurchaseSetting { get; set; }
	}
}
