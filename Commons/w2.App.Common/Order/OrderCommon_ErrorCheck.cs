/*
=========================================================================================================
  Module      : 注文共通処理クラスエラーチェック関連部分(OrderCommon_ErrorCheck.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.Global;
using w2.App.Common.Global.Config;
using w2.App.Common.Option;
using w2.App.Common.Order.Botchan;
using w2.Common.Util;
using w2.Domain.DeliveryCompany;
using w2.Domain.DeliveryLeadTime;
using w2.Domain.Holiday.Helper;
using w2.Domain.Order;
using w2.Domain.ShopShipping;
using w2.Domain.User;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>注文エラーコード</summary>
	///*********************************************************************************************
	public enum OrderErrorcode
	{
		/// <summary>エラーなし</summary>
		NoError,
		/// <summary> 商品無効エラー</summary>
		ProductInvalid,
		/// <summary>商品販売前の期間エラー</summary>
		ProductBeforeSellTerm,
		/// <summary>商品販売期間後エラー</summary>
		ProductOutOfSellTerm,
		/// <summary>商品在庫エラー（カート投入前）</summary>
		ProductNoStockBeforeCart,
		/// <summary>商品在庫エラー</summary>
		ProductNoStock,
		/// <summary>商品マスタ削除済エラー</summary>
		ProductDeleted,
		/// <summary> 商品販売可能数量エラー</summary>
		MaxSellQuantityError,
		/// <summary>配送日設定可能範囲エラー</summary>
		ShippingDateError,
		/// <summary>販売可能会員ランクエラー</summary>
		SellMemberRankError,
		/// <summary>商品セール無効</summary>
		ProductSalesInvalid,
		/// <summary>商品セール変更</summary>
		ProductSalesChanged,
		/// <summary>商品価格変更</summary>
		ProductPriceChanged,
		/// <summary>商品付帯情報価格変更</summary>
		ProductOptionPriceChanged,
		/// <summary>配送種別変更</summary>
		ProductShippingTypeChanged,
		/// <summary>決済種別情報なしエラー</summary>
		PaymentUnfind,
		/// <summary>配送種別情報なしエラー</summary>
		ShopShippingUnfind,
		/// <summary>決済金額範囲外エラー</summary>
		PaymentUsablePriceOutOfRangeError,
		/// <summary>決済金額オーバーエラー（選択不能）</summary>
		PaymentUsablePriceOverUnselectableError,
		/// <summary>定期購入のみ可の商品を同梱しようとした</summary>
		ProductBundleOnlyFixedPurchaseError,
		/// <summary>商品定期購入無効エラー</summary>
		ProducFixedPurchaseInvalidError,
		/// <summary>商品販売可能上限値と下限値が同値の場合に表示するエラー</summary>
		MaxSellQuantitySameAsMinSellQuantity,
		/// <summary>商品販売可能数量エラー時に表示する商品販売可能数</summary>
		DisplayOnMaxSellQuantityCaseOfMaxSellQuantityError,
		/// <summary>配送不可エリアエラー</summary>
		UnavailableShippingAreaError,
	}

	///*********************************************************************************************
	/// <summary>
	/// 注文共通処理クラス
	/// </summary>
	///*********************************************************************************************
	public partial class OrderCommon
	{
		/// <summary>
		/// カート商品整合性チェック
		/// </summary>
		/// <param name="cpTarget">チェック対象カート商品</param>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="strLoginUserId">ログインユーザーID</param>
		/// <returns>エラーメッセージ</returns>
		/// <remarks>
		/// 配送種別変更チェック → 商品状態チェック → 販売価格変更チェックの順にチェック
		/// </remarks>
		public static OrderErrorcode CheckCartProduct(CartProduct cpTarget, DataRowView drvProduct, string strLoginUserId)
		{
			OrderErrorcode oeErrorCode = OrderErrorcode.NoError;

			//------------------------------------------------------
			// 配送種別変更チェック
			//------------------------------------------------------
			if (oeErrorCode == OrderErrorcode.NoError)
			{
				oeErrorCode = CheckProductShippingType(cpTarget, drvProduct);
			}

			//------------------------------------------------------
			// 商品状態チェック（有効フラグ、販売期間、データ整合性、最大購入可能数、商品在庫、会員ランク、定期購入フラグ）
			//------------------------------------------------------
			if (oeErrorCode == OrderErrorcode.NoError)
			{
				oeErrorCode = CheckProductStatus(drvProduct, cpTarget.Count, cpTarget.AddCartKbn, strLoginUserId);
			}

			// 商品セール期間チェック
			if (oeErrorCode == OrderErrorcode.NoError)
			{
				oeErrorCode = CheckProductSalesValid(cpTarget);
			}

			// 商品セールID変更チェック
			if (oeErrorCode == OrderErrorcode.NoError)
			{
				oeErrorCode = CheckProductSalesIdChange(cpTarget, drvProduct);
			}

			//------------------------------------------------------
			// 販売価格変更チェック
			//------------------------------------------------------
			if (oeErrorCode == OrderErrorcode.NoError)
			{
				oeErrorCode = CheckProductPrice(cpTarget, drvProduct, false);	// チェックのみ
			}

			return oeErrorCode;
		}

		/// <summary>
		/// 配送種別変更チェック
		/// </summary>
		/// <param name="cpTarget">チェック対象カート商品</param>
		/// <param name="drvProduct">商品情報</param>
		/// <returns>エラーコード</returns>
		/// <remarks>
		/// カート表示、注文確認時にカート商品情報の配送種別変更チェックを行う
		/// </remarks>
		public static OrderErrorcode CheckProductShippingType(CartProduct cpTarget, DataRowView drvProduct)
		{
			if (cpTarget.ShippingType != (string)drvProduct[Constants.FIELD_PRODUCT_SHIPPING_TYPE])
			{
				return OrderErrorcode.ProductShippingTypeChanged;
			}

			return OrderErrorcode.NoError;
		}

		/// <summary>
		/// 商品状態チェック
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		/// <param name="iProductCount">商品注文数</param>
		/// <param name="addCartKbn">カーと投入区分</param>
		/// <param name="strLoginUserId">ログインユーザーID</param>
		/// <param name="addCartUrlFlg">カート投入URLフラグ</param>
		/// <returns>商品状態</returns>
		/// <remarks>
		/// カート投入、カート表示、再計算、注文確認時に商品情報のチェックを行う。
		/// </remarks>
		public static OrderErrorcode CheckProductStatus(DataRowView drvProduct, int iProductCount, Constants.AddCartKbn addCartKbn, string strLoginUserId, bool addCartUrlFlg = false)
		{
			var oe = CheckProduct(drvProduct, iProductCount, addCartKbn, strLoginUserId, addCartUrlFlg);
			return oe[0];
		}

		/// <summary>
		/// 商品状態チェック
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="productCount">商品注文数</param>
		/// <param name="addCartKbn">カーと投入区分</param>
		/// <param name="loginUserId">ログインユーザーID</param>
		/// <param name="addCartUrlFlg">カート投入URLフラグ</param>
		/// <returns>商品状態</returns>
		/// <remarks>
		/// カート投入、カート表示、再計算、注文確認時に商品情報のチェックを行う。
		/// </remarks>
		public static OrderErrorcode[] CheckProduct(DataRowView product, int productCount, Constants.AddCartKbn addCartKbn, string loginUserId, bool addCartUrlFlg = false)
		{
			var result = new OrderErrorcode[0];

			// 有効フラグチェック
			if ((string)product[Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
			{
				result = result.Concat(new[] { OrderErrorcode.ProductInvalid }).ToArray();
			}

			// 販売期間チェック
			if ((ProductCommon.IsSellTerm(product) == false))
			{
				result = result.Concat(new[] { OrderErrorcode.ProductOutOfSellTerm }).ToArray();
			}

			// データ整合性チェック
			// 紐づく配送種別がない？
			if (product[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID] == System.DBNull.Value)
			{
				result = result.Concat(new[] { OrderErrorcode.ShopShippingUnfind }).ToArray();
			}

			// 最大購入可能数量チェック
			if (productCount > (int)product[Constants.FIELD_PRODUCT_MAX_SELL_QUANTITY])
			{
				result = result.Concat(new[] { OrderErrorcode.MaxSellQuantityError }).ToArray();
			}

			// 商品在庫チェック
			if (OrderCommon.CheckProductStockBuyable(product, productCount) == false)
			{
				result = result.Concat(new[] { OrderErrorcode.ProductNoStockBeforeCart }).ToArray();
			}

			// カート投入URLからの購入可否チェック
			if (addCartUrlFlg)
			{
				if ((string)product[Constants.FIELD_PRODUCT_USE_VARIATION_FLG]
					== Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_UNUSE)
				{
					if ((string)product[Constants.FIELD_PRODUCT_ADD_CART_URL_LIMIT_FLG]
						== Constants.FLG_PRODUCT_ADD_CART_URL_LIMIT_FLG_VALID)
					{
						result = result.Concat(new[] { OrderErrorcode.ProductInvalid }).ToArray();
					}
				}
				else
				{
					// 商品バリエーション優先
					if ((string)product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG]
						== Constants.FLG_PRODUCTVARIATION_VARIATION_ADD_CART_URL_LIMIT_FLG_VALID)
					{
						result = result.Concat(new[] { OrderErrorcode.ProductInvalid }).ToArray();
					}
				}
			}

			// 購入可能会員ランクチェック
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				var orderMemberRankdId = MemberRankOptionUtility.GetMemberRankId(loginUserId);
				var buyableMemberRank = (string)product[Constants.FIELD_PRODUCT_BUYABLE_MEMBER_RANK];

				// 購入不可会員ランクの商品があれば
				if (MemberRankOptionUtility.CheckMemberRankPermission(orderMemberRankdId, buyableMemberRank) == false)
				{
					result = result.Concat(new[] { OrderErrorcode.SellMemberRankError }).ToArray();
				}
			}

			// 定期購入チェック
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
			{
				var isFixedPurchase = (addCartKbn == Constants.AddCartKbn.FixedPurchase);
				var isSubscriptionBox = (addCartKbn == Constants.AddCartKbn.SubscriptionBox);

				// 定期・頒布会商品の場合、「定期不可」 or 定期購入不可の配送種別でエラー
				var isProductInvalid = (isFixedPurchase || isSubscriptionBox)
					&& (((string)product[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_INVALID)
						|| ((string)product["shipping_" + Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FLG_INVALID));

				// 頒布会商品以外の場合、「頒布会のみ」でエラー
				isProductInvalid |= ((isSubscriptionBox == false)
					&& ((string)product[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG] == Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_ONLY));

				// 頒布会商品の場合、「頒布会不可」でエラー
				isProductInvalid |= (isSubscriptionBox
					&& ((string)product[Constants.FIELD_PRODUCT_SUBSCRIPTION_BOX_FLG] == Constants.FLG_PRODUCT_SUBSCRIPTION_BOX_FLG_INVALID));

				// 通常 or ギフト商品の場合、「定期のみ」の場合エラー
				isProductInvalid |= ((addCartKbn == Constants.AddCartKbn.Normal || addCartKbn == Constants.AddCartKbn.GiftOrder)
					&& ((string)product[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG] == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY));

				// （定期購入のカート内商品＆定期購入不可のマスタ商品）又は（通常購入のカート内商品＆通常購入不可のマスタ商品）又は（定期購入のカート内商品＆定期購入不可の配送種別）
				if (isProductInvalid)
				{
					// 定期購入不可メッセージを作成するべきだが、イレギュラーエラーのため「商品無効エラー」メッセージを使用している。
					result = result.Concat(new[] { OrderErrorcode.ProductInvalid }).ToArray();
				}
				// 該当ユーザーは該当商品に制限されているかどうか
				else if ((isFixedPurchase || isSubscriptionBox)
					&& CheckFixedPurchaseLimitedUserLevel((string)product[Constants.FIELD_PRODUCT_SHOP_ID], (string)product[Constants.FIELD_PRODUCT_PRODUCT_ID], loginUserId))
				{
					result = result.Concat(new[] { OrderErrorcode.ProducFixedPurchaseInvalidError }).ToArray();
				}
			}

			// ギフト購入チェック
			if (Constants.GIFTORDER_OPTION_ENABLED)
			{
				var isGiftOrder = (addCartKbn == Constants.AddCartKbn.GiftOrder);

				// （ギフトのカート内商品＆ギフト不可のマスタ商品）又は（通常購入のカート内商品＆通常購入不可のマスタ商品）
				if ((isGiftOrder && ((string)product[Constants.FIELD_PRODUCT_GIFT_FLG] == Constants.FLG_PRODUCT_GIFT_FLG_INVALID))
					|| ((isGiftOrder == false) && ((string)product[Constants.FIELD_PRODUCT_GIFT_FLG] == Constants.FLG_PRODUCT_GIFT_FLG_ONLY)))
				{
					// ギフト購入不可メッセージを作成するべきだが、イレギュラーエラーのため「商品無効エラー」メッセージを使用している。
					result = result.Concat(new[] { OrderErrorcode.ProductInvalid }).ToArray();
				}
			}

			// ここまでエラーなしなら、エラーなしを返す
			if (result.Length == 0)
			{
				result = result.Concat(new[] { OrderErrorcode.NoError }).ToArray();
			}

			return result;
		}

		/// <summary>
		/// 商品セール期間チェック
		/// </summary>
		/// <param name="cartProduct">チェック対象カート商品</param>
		/// <returns>エラーコード</returns>
		/// <remarks>カート表示、注文確認時にカート商品情報のセール期間チェックを行う。</remarks>
		public static OrderErrorcode CheckProductSalesValid(CartProduct cartProduct)
		{
			// セールIDが無い場合はチェックしない
			if (cartProduct.ProductSaleId == "")
			{
				return OrderErrorcode.NoError;
			}

			decimal? productSalePrice = OrderCommon.GetProductSalePrice(cartProduct.ShopId, cartProduct.ProductSaleId, cartProduct.ProductId, cartProduct.VariationId);
			// 価格がnullの場合セール期間が無効ですので、エラーを返す
			if (productSalePrice == null)
			{
				return OrderErrorcode.ProductSalesInvalid;
			}

			return OrderErrorcode.NoError;
		}

		/// <summary>
		/// 商品セールID変更チェック
		/// </summary>
		/// <param name="cartProduct">チェック対象カート商品</param>
		/// <param name="product">商品情報</param>
		/// <returns>エラーコード</returns>
		/// <remarks>カート表示、注文確認時にカート商品情報のセールID変更チェックを行う。</remarks>
		public static OrderErrorcode CheckProductSalesIdChange(CartProduct cartProduct, DataRowView product)
		{
			string validity = StringUtility.ToEmpty(product["validity"]);
			string productSaleId = (validity == "1") ? StringUtility.ToEmpty(product[Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]) : "";

			// 商品セールIDが変わった場合、エラーを返す (HACK：カートに闇市のセールIDを持つ可能性があるので、値がある場合更新しない)
			if ((cartProduct.ProductSaleId == "") && (cartProduct.ProductSaleId != productSaleId))
			{
				return OrderErrorcode.ProductSalesChanged;
			}

			return OrderErrorcode.NoError;
		}

		/// <summary>
		/// 販売価格変更チェック
		/// </summary>
		/// <param name="cartProduct">チェック対象カート商品</param>
		/// <param name="product">商品情報</param>
		/// <param name="blUpdate">金額更新</param>
		/// <returns>エラーコード</returns>
		/// <remarks>カート表示、注文確認時にカート商品情報の販売価格変更チェックを行う。</remarks>
		public static OrderErrorcode CheckProductPrice(CartProduct cartProduct, DataRowView product, bool blUpdate)
		{
			//------------------------------------------------------
			// 同梱商品またはセット商品はチェックしない
			//------------------------------------------------------
			if ((string.IsNullOrEmpty(cartProduct.ProductBundleId) == false)
				|| (cartProduct.IsSetItem))
			{
				return OrderErrorcode.NoError;
			}

			//------------------------------------------------------
			// 商品価格取得＆カート商品価格と比較
			//------------------------------------------------------
			var productTmp = new CartProduct(
				product,
				cartProduct.AddCartKbn,
				cartProduct.ProductSaleId,
				1,
				false,
				string.Empty,
				cartProduct.FixedPurchaseId,
				cartProduct.CartId,
				null,
				cartProduct.SubscriptionBoxCourseId);
			if (cartProduct.Price != productTmp.Price)
			{
				if (blUpdate)
				{
					cartProduct.SetPrice(product);
				}

				if (cartProduct.IsSubscriptionBox)
				{
					// 商品無効エラー
					return OrderErrorcode.ProductInvalid;
				}

				// 商品価格が変更された
				return OrderErrorcode.ProductPriceChanged;
			}

			//------------------------------------------------------
			// 商品オプション価格取得＆カート商品オプション価格と比較
			//------------------------------------------------------
			var cartProductOptionSetting = cartProduct.ProductOptionSettingList.Items
				.Aggregate(
					string.Empty,
					(current, productOptionSetting) =>
						current + string.Join(null, productOptionSetting.OptionPriceSettingValues));
			var tempCartProductOptionSetting = productTmp.ProductOptionSettingList.Items
				.Aggregate(
					string.Empty,
					(current, productOptionSetting) =>
						current + string.Join(null, productOptionSetting.OptionPriceSettingValues));
			if (cartProductOptionSetting != tempCartProductOptionSetting)
			{
				if (blUpdate)
				{
					SetOptionPrice(productTmp, cartProduct);
				}

				// 商品オプション価格が変更された
				return OrderErrorcode.ProductOptionPriceChanged;
			}

			return OrderErrorcode.NoError;
		}

		/// <summary>
		/// 付帯情報最新情報へ更新
		/// </summary>
		/// <param name="tempCartProduct">最新プロダクト</param>
		/// <param name="cartProduct">カート投入プロダクト</param>
		private static void SetOptionPrice(CartProduct tempCartProduct, CartProduct cartProduct)
		{
			if (Constants.PRODUCT_OPTION_SETTINGS_PRICE_GRANT_ENABLED == false) return;

			for (var i = 0; i < cartProduct.ProductOptionSettingList.Items.Count; i++)
			{
				var optionSetting = tempCartProduct.ProductOptionSettingList.Items[i];
				if (cartProduct.ProductOptionSettingList.Items[i].SelectedSettingValue == null) continue;

				var selectedValues = cartProduct.ProductOptionSettingList.Items[i].SelectedSettingValue
					.Replace(Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE, "\n")
					.Split(new char[] { '\n' }, StringSplitOptions.RemoveEmptyEntries);
				var tempSettingValues = optionSetting.SettingValues;
				foreach (var valueStr in selectedValues)
				{
					foreach (var lstr in tempSettingValues.Where(lstr => valueStr.Contains("(") != false).Where(lstr => valueStr.Substring(0, valueStr.IndexOf("(")) == lstr.Substring(0, lstr.IndexOf("("))))
					{
						tempCartProduct.ProductOptionSettingList.Items[i].SelectedSettingValue += lstr + Constants.PRODUCTOPTIONVALUES_SEPARATING_CHAR_SELECT_SETTING_VALUE;
					}
				}
			}
			cartProduct.ProductOptionSettingList = tempCartProduct.ProductOptionSettingList;
		}

		/// <summary>
		/// 決済手数料設定チェック
		/// </summary>
		/// <param name="coCart">カート情報</param>
		/// <returns>エラーコード</returns>
		public static OrderErrorcode CheckSetUpPaymentPrice(CartObject coCart)
		{
			if (coCart.Payment == null)
			{
				return OrderErrorcode.NoError;
			}

			return CheckSetUpPaymentPrice(
				coCart.ShopId,
				coCart.Payment.PaymentId,
				coCart.PriceSubtotal,
				coCart.PriceCartTotalWithoutPaymentPrice);
		}
		/// <summary>
		/// 決済手数料設定チェック
		/// </summary>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strPaymentId">決済種別ID</param>
		/// <param name="dPriceSubtotal">商品合計金額</param>
		/// <param name="priceTotalWithoutPayment">決済手数料を除いた合計金額</param>
		/// <returns>エラーコード</returns>
		public static OrderErrorcode CheckSetUpPaymentPrice(
			string strShopId,
			string strPaymentId,
			decimal dPriceSubtotal,
			decimal priceTotalWithoutPayment)
		{
			var paymentPrice = GetPaymentPriceInfo(
				strShopId,
				strPaymentId,
				dPriceSubtotal,
				priceTotalWithoutPayment);
			if (paymentPrice == null)
			{
				return OrderErrorcode.PaymentUnfind;
			}

			return OrderErrorcode.NoError;
		}

		/// <summary>
		/// 注文エラーコードからエラーメッセージ取得
		/// </summary>
		/// <param name="oeErrorCode">エラーコード</param>
		/// <param name="strReplaceStrings">置換文字列配列</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetErrorMessage(OrderErrorcode oeErrorCode, params string[] strReplaceStrings)
		{
			// エラーメッセージ取得
			StringBuilder sbErrorMessage = new StringBuilder(GetErrorMessage(oeErrorCode));

			// 置換
			for (int iLoop = 0; iLoop < strReplaceStrings.Length; iLoop++)
			{
				sbErrorMessage.Replace("@@ " + (iLoop + 1).ToString() + " @@", (string)strReplaceStrings[iLoop]);
			}

			return sbErrorMessage.ToString();
		}
		/// <summary>
		/// 注文エラーコードからエラーメッセージ取得
		/// </summary>
		/// <param name="oeErrorCode">エラーコード</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetErrorMessage(OrderErrorcode oeErrorCode)
		{
			string strMessageKey = null;
			switch (oeErrorCode)
			{
				// 商品無効エラー
				case OrderErrorcode.ProductInvalid:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_INVALID;
					break;

				// 商品販売前の期間エラー
				case OrderErrorcode.ProductBeforeSellTerm:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_BEFORE_SELL;
					break;

				// 商品販売期間後エラー
				case OrderErrorcode.ProductOutOfSellTerm:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_NO_SELL;
					break;

				// 商品在庫エラー（カート投入前）
				case OrderErrorcode.ProductNoStockBeforeCart:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_NO_STOCK_BEFORE_CART;
					break;

				// 商品在庫エラー
				case OrderErrorcode.ProductNoStock:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_NO_STOCK;
					break;

				// 商品マスタ削除エラー
				case OrderErrorcode.ProductDeleted:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_DELETE;
					break;

				// 商品販売可能数量エラー
				case OrderErrorcode.MaxSellQuantityError:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_OVER_MAXSELLQUANTITY;
					break;

				// 商品セール無効
				case OrderErrorcode.ProductSalesInvalid:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_SALES_INVALID;
					break;

				// 商品セール変更
				case OrderErrorcode.ProductSalesChanged:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_SALES_CHANGE;
					break;

				// 商品価格変更
				case OrderErrorcode.ProductPriceChanged:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_PRICE_CHANGE;
					break;

				// 付帯情報価格変更
				case OrderErrorcode.ProductOptionPriceChanged:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_OPTION_PRICE_CHANGE;
					break;

				// 配送種別変更
				case OrderErrorcode.ProductShippingTypeChanged:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_SHIPPING_TYPE_CHANGE;
					break;

				// 決済種別未設定エラー
				case OrderErrorcode.PaymentUnfind:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PAYMENT_PRICE_UNFIND;
					break;

				// 配送種別未設定エラー
				case OrderErrorcode.ShopShippingUnfind:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_SHOP_SHIPPING_UNFIND;
					break;

				// 配送日設定可能範囲エラー
				case OrderErrorcode.ShippingDateError:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_ORDERSHIPPING_NO_TERM;
					break;

				// 決済金額範囲外エラー
				case OrderErrorcode.PaymentUsablePriceOutOfRangeError:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PAYMENT_USEABLE_PRICE_OUT_OF_RANGE_ERROR;
					break;

				// 決済金額オーバーエラー（選択不能）
				case OrderErrorcode.PaymentUsablePriceOverUnselectableError:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PAYMENT_USEABLE_PRICE_OVER_UNSELECTABLE_ERROR;
					break;

				// 販売可能会員ランク外エラー
				case OrderErrorcode.SellMemberRankError:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_BUYABLE_MEMBER_RANK;
					break;

				///定期購入のみ可の商品を同梱しようとした
				case OrderErrorcode.ProductBundleOnlyFixedPurchaseError:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_BUNDLE_ONLY_FIXED_PURCHASE;
					break;

				//定期購入制限されるユーザーエラー
				case OrderErrorcode.ProducFixedPurchaseInvalidError:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_FIXED_PURCHASE_INVALID;
					break;

				// 商品販売可能上限値と下限値が同値
				case OrderErrorcode.MaxSellQuantitySameAsMinSellQuantity:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_MAXSELLQUANTITY_SAME_AS_MINSELLQUANTITY;
					break;

				// 商品販売可能数量超過エラー時に表示する商品販売可能数上限値
				case OrderErrorcode.DisplayOnMaxSellQuantityCaseOfMaxSellQuantityError:
					strMessageKey = CommerceMessages.ERRMSG_FRONT_PRODUCT_DISPLAY_ON_MAXSELLQUANTITY;
					break;

				// 配送不可エリアエラー
				case OrderErrorcode.UnavailableShippingAreaError:
					strMessageKey = CommerceMessages.ERRMSG_MANAGER_UNAVAILABLE_SHIPPING_AREA_ERROR;
					break;
			}

			return CommerceMessages.GetMessages(strMessageKey);
		}

		/// <summary>
		/// Can calculate scheduled shipping date
		/// </summary>
		/// <param name="shopId">The shop ID</param>
		/// <param name="model">The order shipping model</param>
		/// <returns>Return true if can calculate scheduled shipping date, otherwise return false</returns>
		public static bool CanCalculateScheduledShippingDate(string shopId, OrderShippingModel model)
		{
			var isTaiwanCountryShippingEnable = (Constants.TW_COUNTRY_SHIPPING_ENABLE
				&& GlobalAddressUtil.IsCountryTw(model.ShippingCountryIsoCode));
			var result = CanCalculateScheduledShippingDate(
				shopId,
				model.ShippingDate,
				model.ShippingMethod,
				model.DeliveryCompanyId,
				model.ShippingCountryIsoCode,
				isTaiwanCountryShippingEnable
					? model.ShippingAddr2
					: model.ShippingAddr1,
				model.ShippingZip.Replace("-", string.Empty));
			return result;
		}
		/// <summary>
		/// Can calculate scheduled shipping date
		/// </summary>
		/// <param name="shopId">The shop ID</param>
		/// <param name="shipping">The cart shipping object</param>
		/// <returns>Return true if can calculate scheduled shipping date, otherwise return false</returns>
		public static bool CanCalculateScheduledShippingDate(string shopId, CartShipping shipping)
		{
			var result = CanCalculateScheduledShippingDate(
				shopId,
				shipping.ShippingDate,
				shipping.ShippingMethod,
				shipping.DeliveryCompanyId,
				shipping.ShippingCountryIsoCode,
				shipping.IsTaiwanCountryShippingEnable
					? shipping.Addr2
					: shipping.Addr1,
				shipping.Zip.Replace("-", string.Empty));
			return result;
		}
		/// <summary>
		/// Can calculate scheduled shipping date
		/// </summary>
		/// <param name="shopId">A shop ID</param>
		/// <param name="shippingDate">A shipping date</param>
		/// <param name="shippingMethod">A shipping method</param>
		/// <param name="deliveryCompanyId">A delivery company ID</param>
		/// <param name="countryIsoCode">国ISOコード</param>
		/// <param name="prefecture">The prefecture</param>
		/// <param name="zip">A zip code</param>
		/// <returns>Return true if can calculate scheduled shipping date, otherwise return false</returns>
		public static bool CanCalculateScheduledShippingDate(
			string shopId,
			DateTime? shippingDate,
			string shippingMethod,
			string deliveryCompanyId,
			string countryIsoCode,
			string prefecture,
			string zip)
		{
			if ((GlobalConfigUtil.UseLeadTime() == false)
				|| ((GlobalAddressUtil.IsCountryJp(countryIsoCode) == false)
					&& (GlobalAddressUtil.IsCountryTw(countryIsoCode) == false))
				|| IsLeadTimeFlgOff(deliveryCompanyId)
				|| (shippingDate == null))
			{
				return true;
			}

			var totalLeadTime = HolidayUtil.GetTotalLeadTime(
				shopId,
				deliveryCompanyId,
				prefecture,
				zip);

			// Calculate scheduled shipping date
			var scheduledShippingDate = shippingDate.Value.AddDays(-totalLeadTime);
			var shipableDate = HolidayUtil.GetShortestShippingDateBasedOnToday(deliveryCompanyId);

			return (scheduledShippingDate >= shipableDate);
		}

		/// <summary>
		/// 該当ユーザーは該当商品に定期購入制限されるかをチェック
		/// </summary>
		/// <param name="shopId">ショップId</param>
		/// <param name="productId">商品Id</param>
		/// <param name="userId">ユーザId</param>
		/// <returns>制限されているかどうか</returns>
		public static bool CheckFixedPurchaseLimitedUserLevel(string shopId, string productId, string userId)
		{
			if (string.IsNullOrEmpty(userId)) return false;

			var fixedPurchaseLimitedUserLevels = ProductCommon.GetProductInfoUnuseMemberRankPrice(shopId, productId);
			var userManagementLevelId = new UserService().Get(userId).UserManagementLevelId;
			var fixedPurchaseAbleUserLevel = fixedPurchaseLimitedUserLevels
				.Cast<DataRowView>()
				.Select(drv => drv[Constants.FIELD_PRODUCT_FIXED_PURCHASE_LIMITED_USER_LEVEL_IDS].ToString().Split(','))
				.FirstOrDefault();

			return (fixedPurchaseAbleUserLevel == null) ? false : fixedPurchaseAbleUserLevel.Contains(userManagementLevelId);
		}
	}
}
