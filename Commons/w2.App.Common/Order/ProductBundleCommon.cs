/*
=========================================================================================================
  Module      : 商品同梱処理共通クラス (ProductBundleCommon.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common.Product;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.ProductBundle;
using w2.Domain.TargetList;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 商品同梱処理共通クラス
	/// </summary>
	public class ProductBundleCommon
	{
		/// <summary>
		/// 同梱商品をカートに追加
		/// </summary>
		/// <param name="cartList">カートオブジェクトリスト</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="advCodeFirst">初回広告コード</param>
		/// <param name="advCodeNew">最新広告コード</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <param name="hitTargetListIds">ログインユーザに有効なターゲットリスト群</param>
		/// <param name="isFront">フロント購入か</param>
		internal static void AddBundleItemsToCartList(
			List<CartObject> cartList,
			string userId,
			string advCodeFirst,
			string advCodeNew,
			string[] excludeOrderIds,
			string[] hitTargetListIds,
			bool isFront)
		{
			if (Constants.PRODUCTBUNDLE_OPTION_ENABLED == false) return;
			if (cartList.Any() == false) return;

			RemoveBundleItemsToCartList(cartList);
			cartList.ForEach(cart => AddBundleItemsToCart(
				cartList,
				cart,
				userId,
				advCodeFirst,
				advCodeNew,
				excludeOrderIds,
				hitTargetListIds,
				isFront));
		}

		/// <summary>
		/// 同梱商品を対象カートに追加
		/// </summary>
		/// <param name="cartList">カートオブジェクトリスト</param>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="advCodeFirst">初回広告コード</param>
		/// <param name="advCodeNew">最新広告コード</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <param name="hitTargetListIds">ログインユーザに有効なターゲットリスト群</param>
		/// <param name="isFront">フロント購入か</param>
		/// <returns>商品同梱設定</returns>
		private static void AddBundleItemsToCart(
			List<CartObject> cartList,
			CartObject cart,
			string userId,
			string advCodeFirst,
			string advCodeNew,
			string[] excludeOrderIds,
			string[] hitTargetListIds,
			bool isFront)
		{
			// 定期購入回数（出荷時点）を取得
			var fpShippedCount = 0;
			if ((cart.FixedPurchase != null) && (string.IsNullOrEmpty(cart.FixedPurchase.FixedPurchaseId) == false))
			{
				var fixedPurchase = new FixedPurchaseService().Get(cart.FixedPurchase.FixedPurchaseId);
				if (fixedPurchase != null) fpShippedCount = fixedPurchase.ShippedCount;
			}

			// 同梱前の商品リスト
			var rawCartProductsList = cart.Copy().Items;

			// カート毎の注文種別を取得
			var cartOrderType = GetCartOrderType(cart.Items);

			// 適用可能な商品同梱設定を取得
			var productBundles = GetProductBundleInfos(
				cart,
				userId,
				advCodeFirst,
				advCodeNew,
				fpShippedCount,
				excludeOrderIds,
				hitTargetListIds,
				isFront,
				cartOrderType);
			if (productBundles.Any() == false) return;

			foreach (var bundle in productBundles)
			{
				var bundledCount = cartList
					.SelectMany(cartObject => cartObject.Items)
					.Count(item => (item.ProductBundleId == bundle.ProductBundleId));

				// ユーザー利用可能回数を指定する場合は、指定回数以上に商品が同梱されていた場合はスキップする
				if ((bundledCount > 0)
					&& ((bundle.UsableTimesKbn == Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_ONCETIME)
						|| ((bundle.UsableTimesKbn == Constants.FLG_PRODUCTBUNDLE_USABLE_TIMES_KBN_NUMSPECIFY)
							&& (bundledCount + bundle.OrderedCount >= bundle.UsableTimes))))
				{
					continue;
				}
				AddBundleItemsToCart(cartList, cart, rawCartProductsList, bundle, fpShippedCount, cartOrderType);
			}
		}

		/// <summary>
		/// 同梱商品をカートから削除
		/// </summary>
		/// <param name="cartList">カートオブジェクトリスト</param>
		internal static void RemoveBundleItemsToCartList(List<CartObject> cartList)
		{
			if (Constants.PRODUCTBUNDLE_OPTION_ENABLED == false) return;

			cartList.ForEach(RemoveBundleItemsToCart);
		}

		/// <summary>
		/// 同梱商品をカートから削除
		/// </summary>
		/// <param name="cart">カートオブジェクト</param>
		private static void RemoveBundleItemsToCart(CartObject cart)
		{
			var removeItems = cart.Items
				.Where(item => string.IsNullOrEmpty(item.ProductBundleId) == false)
				.ToList();
			removeItems.ForEach(removeItem => cart.Items.Remove(removeItem));
		}

		/// <summary>
		/// 条件に合致する商品同梱設定を取得
		/// </summary>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="advCodeFirst">初回広告コード</param>
		/// <param name="advCodeNew">最新広告コード</param>
		/// <param name="fpShippedCount">定期購入回数（出荷時点）</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <param name="hitTargetListIds">ログインユーザに有効なターゲットリスト群</param>
		/// <param name="isFront">フロント購入か</param>
		/// <param name="cartOrderType">カート毎の注文種別</param>
		/// <returns>商品同梱設定</returns>
		private static ProductBundleModel[] GetProductBundleInfos(
			CartObject cart,
			string userId,
			string advCodeFirst,
			string advCodeNew,
			int fpShippedCount,
			string[] excludeOrderIds,
			string[] hitTargetListIds,
			bool isFront,
			string cartOrderType)
		{
			var bundleItems = GetProductBundleInfo(
				cart,
				userId,
				advCodeFirst,
				advCodeNew,
				fpShippedCount,
				excludeOrderIds,
				hitTargetListIds,
				isFront,
				cartOrderType);

			var multipleApplyInvalidBundleItems = bundleItems
				.Where(bundleItem => (bundleItem.MultipleApplyFlg == Constants.FLG_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG_INVALID))
				.ToArray();
			var highestPriorityBundle = multipleApplyInvalidBundleItems
				.FirstOrDefault(bundleItem => (bundleItem.ApplyOrder == multipleApplyInvalidBundleItems.Min(item => item.ApplyOrder)));

			var applicableBundle = bundleItems
				.Where(bundleItem => (bundleItem.MultipleApplyFlg == Constants.FLG_PRODUCTBUNDLE_MULTIPLE_APPLY_FLG_VALID))
				.ToList();
			if (highestPriorityBundle != null)
			{
				applicableBundle.Add(highestPriorityBundle);
			}
			// 決済種別設定が空か、決済種別が一致する商品同梱
			// 決済種別選択前に処理が通ったとき処理しない
			if ((cart.Payment != null) && (cart.Payment.PaymentId != null))
			{
				applicableBundle = applicableBundle
					.Where(bundle => string.IsNullOrEmpty(bundle.TargetPaymentIds)
						|| bundle.TargetPaymentIds.Contains(cart.Payment.PaymentId))
					.ToList();
			}
			// 同梱設定の同梱商品をセットする
			new ProductBundleService().SetProductBundleItems(userId, excludeOrderIds, applicableBundle);
			return applicableBundle.ToArray();
		}

		/// <summary>
		/// 商品同梱情報取得
		/// </summary>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="advCodeFirst">初回広告コード</param>
		/// <param name="advCodeNew">最新広告コード</param>
		/// <param name="fpShippedCount">定期購入回数（出荷時点）</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <param name="hitTargetListIds">ログインユーザに有効なターゲットリスト群</param>
		/// <param name="isFront">フロント購入か</param>
		/// <param name="cartOrderType">カート毎の注文種別</param>
		/// <returns>商品同梱情報</returns>
		private static ProductBundleModel[] GetProductBundleInfo(
			CartObject cart,
			string userId,
			string advCodeFirst,
			string advCodeNew,
			int fpShippedCount,
			string[] excludeOrderIds,
			string[] hitTargetListIds,
			bool isFront,
			string cartOrderType)
		{
			var targetCouponCode = Constants.W2MP_COUPON_OPTION_ENABLED && (cart.Coupon != null)
				? cart.Coupon.CouponCode
				: string.Empty;

			var bundleList = new ProductBundleService().GetProductBundleValidForCart(
				userId,
				excludeOrderIds,
				advCodeFirst,
				advCodeNew,
				cart.PriceSubtotal,
				cartOrderType,
				targetCouponCode);

			if (bundleList.Length == 0) return bundleList;

			// ターゲットリスト、対象購入商品、対象外購入商品などのチェック
			var applyProductBundleList = bundleList.Where(
				bundle => CheckTargetId(hitTargetListIds, cart.CartUserId, bundle, isFront)
					&& CheckApplyBundle(cart.Items, bundle, fpShippedCount, cartOrderType)).ToArray();
			return applyProductBundleList;
		}

		/// <summary>
		/// ターゲットリストチェック
		/// </summary>
		/// <param name="hitTargetListIds">ログインユーザに有効なターゲットリスト群</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="bundle">商品同梱情報</param>
		/// <param name="isFront">フロント購入か</param>
		/// <returns>適用可能か</returns>
		private static bool CheckTargetId(
			string[] hitTargetListIds,
			string userId,
			ProductBundleModel bundle,
			bool isFront)
		{
			if (string.IsNullOrEmpty(bundle.TargetId)) return true;

			var isTarget = isFront
				? hitTargetListIds.Any(targetListId => (bundle.TargetId == targetListId))
				: new TargetListService().CheckUserIdInTargetList(
					Constants.CONST_DEFAULT_DEPT_ID,
					bundle.TargetId,
					userId);
			return (isTarget == (bundle.TargetIdExceptFlg == Constants.FLG_PRODUCTBUNDLE_TARGET_ID_EXCEPT_FLG_TARGET));
		}

		/// <summary>
		/// 商品同梱設定が適用かの判定
		/// </summary>
		/// <param name="items">商品リスト</param>
		/// <param name="bundle">商品同梱設定</param>
		/// <param name="fpShippedCount">定期購入回数（出荷時点）</param>
		/// <param name="cartOrderType">カート毎の注文種別</param>
		/// <returns>適用されるか</returns>
		private static bool CheckApplyBundle(List<CartProduct> items, ProductBundleModel bundle, int fpShippedCount, string cartOrderType)
		{
			var canApply = false;
			foreach (var item in items)
			{
				canApply |= IsTargetProduct(item, bundle, fpShippedCount, cartOrderType);

				if (IsExceptProduct(item, bundle)) return false;
			}

			// 対象商品個数の判定
			if (canApply)
			{
				canApply &= ((bundle.TargetProductCountMin.HasValue == false)
					|| (bundle.TargetProductCountMin.Value <= GetTotalTargetProductInCart(items, bundle, fpShippedCount, cartOrderType)));
			}

			return canApply;
		}

		/// <summary>
		/// 商品条件についての判定
		/// </summary>
		/// <param name="item">商品情報</param>
		/// <param name="productIds">商品ID配列</param>
		/// <param name="variationIds">バリエーションID配列</param>
		/// <param name="categoryIds">カテゴリID配列</param>
		/// <returns>TRUE：条件を満たした；FALSE：条件を満たさない</returns>
		private static bool IsSatisfiedProductConditions(
			CartProduct item,
			string[] productIds,
			string[] variationIds,
			string[] categoryIds)
		{
			// 商品ID指定の判定
			var satisfiedProduct = ((productIds.Length > 0) && productIds.Contains(item.ProductId));

			// バリエーションID指定の判定
			var satisfiedVariation = ((variationIds.Length > 0)
				&& variationIds.Contains(item.ProductId + "," + item.VariationId));

			// カテゴリID指定の判定
			var satisfiedCategory = ((categoryIds.Length > 0)
				&& categoryIds.Any(
					cat =>
						(item.CategoryId1.StartsWith(cat)
						|| item.CategoryId2.StartsWith(cat)
						|| item.CategoryId3.StartsWith(cat)
						|| item.CategoryId4.StartsWith(cat)
						|| item.CategoryId5.StartsWith(cat))));

			return (satisfiedProduct || satisfiedVariation || satisfiedCategory);
		}

		/// <summary>
		/// 対象外商品の判定
		/// </summary>
		/// <param name="item">商品情報</param>
		/// <param name="bundle">商品同梱設定モデル</param>
		/// <returns>対象外商品：TRUE；そうでない場合：FALSE</returns>
		private static bool IsExceptProduct(CartProduct item, ProductBundleModel bundle)
		{
			return IsSatisfiedProductConditions(
				item,
				bundle.GetExceptProductIds(),
				bundle.GetExceptProductVariationIds(),
				bundle.GetExceptProductCategoryIds());
		}

		/// <summary>
		/// 対象商品の判定
		/// </summary>
		/// <param name="item">商品情報</param>
		/// <param name="bundle">商品同梱設定モデル</param>
		/// <param name="fpShippedCount">定期購入回数（出荷時点）</param>
		/// /// <param name="cartOrderType">カート全体の注文種別</param>
		/// <returns>対象商品：TRUE；そうでない場合：FALSE</returns>
		private static bool IsTargetProduct(CartProduct item, ProductBundleModel bundle, int fpShippedCount, string cartOrderType)
		{
			// 対象外の商品か？
			var isExceptProduct = IsExceptProduct(item, bundle);

			// 対象注文種別
			// カート内が 通常商品のみ（「通常注文」「全て」） 定期商品のみ（「定期注文」「全て」） 通常・定期商品が混在（「定期注文」「全て」）を適応
			var isMatchOrderType =
				(bundle.TargetOrderType == Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_ALL)
				|| ((cartOrderType == Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_ALL)
					? (bundle.TargetOrderType == Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_FIXED_PURCHASE)
					: (bundle.TargetOrderType == cartOrderType));

			// 定期購入回数の判定
			var withinFixedPurchaseCountRange = ((item.IsFixedPurchase == false)
					&& (cartOrderType == Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL))
				|| CheckWithinFixedPurchaseCountRange(bundle, fpShippedCount);

			// 全商品指定の場合、対象商品の条件を考慮しない
			if (bundle.TargetProductKbn == Constants.FLG_PRODUCTBUNDLE_TARGET_PRODUCT_KBN_ALL)
			{
				return (isMatchOrderType && withinFixedPurchaseCountRange && (isExceptProduct == false));
			}

			// 対象商品か？
			var canBeTargetProduct = IsSatisfiedProductConditions(
				item,
				bundle.GetTargetProductIds(),
				bundle.GetTargetProductVariationIds(),
				bundle.GetTargetProductCategoryIds());

			return (isMatchOrderType && withinFixedPurchaseCountRange && (isExceptProduct == false) && canBeTargetProduct);
		}

		/// <summary>
		/// 対象定期注文回数内か判定
		/// </summary>
		/// <param name="bundle">商品同梱設定モデル</param>
		/// <param name="fpShippedCount">定期購入回数（出荷時点）</param>
		/// <returns>対象定期注文回数内：TRUE；そうでない場合：FALSE</returns>
		private static bool CheckWithinFixedPurchaseCountRange(ProductBundleModel bundle, int fpShippedCount)
		{
			var checkFixedPurchaseCountFrom = ((bundle.TargetOrderFixedPurchaseCountFrom.HasValue == false)
				|| ((fpShippedCount + 1) >= bundle.TargetOrderFixedPurchaseCountFrom.Value));
			var checkFixedPurchaseCountTo = ((bundle.TargetOrderFixedPurchaseCountTo.HasValue == false)
				|| ((fpShippedCount + 1) <= bundle.TargetOrderFixedPurchaseCountTo.Value));
			
			return (checkFixedPurchaseCountFrom && checkFixedPurchaseCountTo);
		}

		/// <summary>
		/// 同梱商品をカートに追加
		/// </summary>
		/// <param name="cartList">カートオブジェクトリスト</param>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="rawCartProductsList">同梱前の商品リスト</param>
		/// <param name="productBundle">商品同梱設定</param>
		/// <param name="fpShippedCount">定期購入回数（出荷時点）</param>
		/// <param name="cartOrderType">カート毎の注文種別</param>
		/// <returns>追加したか</returns>
		private static bool AddBundleItemsToCart(
			List<CartObject> cartList,
			CartObject cart,
			List<CartProduct> rawCartProductsList,
			ProductBundleModel productBundle,
			int fpShippedCount,
			string cartOrderType)
		{
			var errorMessages = new List<string>();
			var isAdded = false;
			var productCountInCart = GetTotalTargetProductInCart(rawCartProductsList, productBundle, fpShippedCount, cartOrderType);
			foreach (var productBundleItem in productBundle.Items)
			{
				var grantItem = ProductCommon.GetProductVariationInfo(
					Constants.CONST_DEFAULT_SHOP_ID,
					productBundleItem.GrantProductId,
					productBundleItem.GrantProductVariationId,
					string.Empty);
				if (grantItem.Count == 0) continue;

				var grantCount = ((string)grantItem[0][Constants.FIELD_PRODUCT_PRODUCT_TYPE] == Constants.FLG_PRODUCT_PRODUCT_TYPE_FLYER)
					? 1
					: productBundleItem.GrantProductCount;

				var errorCode = ValidateProductBundleItem(cart, grantItem[0], grantCount);

				if (errorCode == OrderErrorcode.ProductShippingTypeChanged) continue;

				if (errorCode != OrderErrorcode.NoError)
				{
					errorMessages.Add(OrderCommon.GetErrorMessage(errorCode, ProductCommon.CreateProductJointName(grantItem[0])));
					continue;
				}

				// 初回のみ同梱フラグがONの場合は、既に商品が同梱されている場合はスキップする
				var hasBundled = cartList.SelectMany(cartObject => cartObject.Items).Any(
					item => ((item.ProductBundleId == productBundle.ProductBundleId)
						&& (item.ProductId == productBundleItem.GrantProductId)
						&& (item.VariationId == productBundleItem.GrantProductVariationId)));
				if (hasBundled && productBundleItem.IsOrderedProductExcept) continue;

				var hasBundledGrantItem = cart.Items.Any(item =>
					((item.ProductId == productBundleItem.GrantProductId)
					&& (item.VariationId == productBundleItem.GrantProductVariationId)
					&& (item.ProductBundleId == productBundle.ProductBundleId)));
				var hasBundledSameFlyer = cart.Items.Any(item =>
					((item.ProductId == productBundleItem.GrantProductId)
					&& (item.VariationId == productBundleItem.GrantProductVariationId)
					&& (item.ProductType == Constants.FLG_PRODUCT_PRODUCT_TYPE_FLYER)));
				if (hasBundledGrantItem || hasBundledSameFlyer) continue;   // 同梱済み

				// Check if bundled product can use store pickup
				if ((cart.Shippings[0].ShippingAddrKbn == CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_STORE_PICKUP)
					&& ((string)grantItem[0][Constants.FIELD_PRODUCT_STOREPICKUP_FLG] == Constants.FLG_OFF))
					continue;

				var bundleCartProduct = new CartProduct(
					grantItem[0],
					Constants.AddCartKbn.Normal,
					StringUtility.ToEmpty(grantItem[0][Constants.FIELD_PRODUCTSALE_PRODUCTSALE_ID]),
					grantCount,
					false,
					new ProductOptionSettingList())
				{
					ProductBundleId = productBundleItem.ProductBundleId,
					CountSingle = ((string)grantItem[0][Constants.FIELD_PRODUCT_PRODUCT_TYPE] == Constants.FLG_PRODUCT_PRODUCT_TYPE_FLYER)
						? 1
						: (productBundle.ApplyType == Constants.FLG_PRODUCTBUNDLE_APPLY_TYPE_FOR_ORDER)
							? productBundleItem.GrantProductCount
							: productCountInCart * productBundleItem.GrantProductCount,
					ApplyOrder = productBundle.ApplyOrder,
					MultipleApplyFlg = productBundle.MultipleApplyFlg
				};
				bundleCartProduct.SetPrice(0);

				cart.AddVirtural(bundleCartProduct, false);
				isAdded = true;
			}

			if (errorMessages.Any()) AppLogger.WriteError(string.Join(Environment.NewLine, errorMessages.ToArray()));

			return isAdded;
		}

		/// <summary>
		/// 有効な同梱商品か
		/// </summary>
		/// <param name="cart">カートオブジェクト</param>
		/// <param name="product">商品情報</param>
		/// <param name="productCount">同梱個数</param>
		/// <returns>エラーコード</returns>
		private static OrderErrorcode ValidateProductBundleItem(CartObject cart, DataRowView product, int productCount)
		{
			if ((string)product[Constants.FIELD_PRODUCT_VALID_FLG] == Constants.FLG_PRODUCT_VALID_FLG_INVALID)
			{
				return OrderErrorcode.ProductInvalid;
			}

			if (ProductCommon.IsSellTerm(product) == false)
			{
				return OrderErrorcode.ProductOutOfSellTerm;
			}

			if (product[Constants.FIELD_SHOPSHIPPING_SHIPPING_ID] == DBNull.Value)
			{
				return OrderErrorcode.ShopShippingUnfind;
			}

			if (StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_SHIPPING_TYPE]) != cart.ShippingType)
			{
				return OrderErrorcode.ProductShippingTypeChanged;
			}

			if (OrderCommon.CheckProductStockBuyable(product, productCount) == false)
			{
				return OrderErrorcode.ProductNoStockBeforeCart;
			}

			if (StringUtility.ToEmpty(product[Constants.FIELD_PRODUCT_FIXED_PURCHASE_FLG]) == Constants.FLG_PRODUCT_FIXED_PURCHASE_FLG_ONLY)
			{
				return OrderErrorcode.ProductBundleOnlyFixedPurchaseError;
			}

			return OrderErrorcode.NoError;
		}

		/// <summary>
		/// 対象商品がカート内にいくつあるか
		/// </summary>
		/// <param name="items">カート内の商品リスト</param>
		/// <param name="productBundle">商品同梱設定</param>
		/// <param name="fpShippedCount">定期購入回数（出荷時点）</param>
		/// /// <param name="cartOrderType">カート全体の注文種別</param>
		/// <returns>対象商品の個数</returns>
		private static int GetTotalTargetProductInCart(List<CartProduct> items, ProductBundleModel productBundle, int fpShippedCount, string cartOrderType)
		{
			// 対象商品を取得
			var targets = items.Where(item => IsTargetProduct(item, productBundle, fpShippedCount, cartOrderType)).ToList();

			// 対象商品の個数計算
			var total = (targets.Count > 0) ? targets.Sum(item => item.CountSingle) : 0;
			return total;
		}

		/// <summary>
		/// Cart全体の注文種別を取得
		/// </summary>
		/// <param name="items">カート商品情報</param>
		/// <returns>カート全体の注文種別「"ALL","FIXED_PURCHASE","NORMAL"」</returns>
		private static string GetCartOrderType(List<CartProduct> items)
		{
			var cartKbnArray = items.Select(item => item.AddCartKbn).Distinct().ToArray();
			var result = (cartKbnArray.FirstOrDefault(cartKbn => (cartKbn == Constants.AddCartKbn.FixedPurchase)
			|| (cartKbn == Constants.AddCartKbn.SubscriptionBox)) == Constants.AddCartKbn.Normal)
				? Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_NORMAL
				: Constants.FLG_PRODUCTBUNDLE_TARGET_ORDER_TYPE_FIXED_PURCHASE;
			return result;
		}
	}
}
