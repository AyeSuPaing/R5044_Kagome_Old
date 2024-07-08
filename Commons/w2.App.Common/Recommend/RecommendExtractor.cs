/*
=========================================================================================================
  Module      : レコメンド設定抽出クラス(RecommendExtractor.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.Recommend;
using w2.App.Common.DataCacheController;
using w2.App.Common.Order;
using w2.App.Common.Option;

namespace w2.App.Common.Recommend
{
	/// <summary>
	/// レコメンド設定抽出クラス
	/// </summary>
	[Serializable]
	public class RecommendExtractor
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="memberRankId">会員ランクID</param>
		/// <param name="currentCartList">現在のカートリスト</param>
		public RecommendExtractor(string userId, string memberRankId, CartObject[] currentCartList)
		{
			// ユーザID＆会員ランクID＆現在のカートリストセット
			this.UserId = userId;
			this.MemberRankId = memberRankId;
			this.CurrentCartList = currentCartList;
			// 有効なレコメンド設定リストセット
			// ※キャッシュから取得している
			this.ApplicableRecommends
				= DataCacheControllerFacade.GetRecommendCacheController().GetApplicableRecommend();
		}
		#endregion

		#region メソッド
		/// <summary>
		/// レコメンド設定抽出（優先度の高いものを１つ）
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		/// <param name="displayPage">表示ページ</param>
		/// <returns>レコメンド設定</returns>
		public RecommendModel Exec(CartObject[] cartList, string displayPage = Constants.FLG_RECOMMEND_RECOMMEND_DISPLAY_PAGE_ORDER_CONFIRM)
		{
			// 既にレコメンドが適用されているカートを除外する
			var targetCartList
				= cartList.Where(cart =>
					cart.Items.Any(i =>
						string.IsNullOrEmpty(i.RecommendId) == false) == false).ToArray();
			if (targetCartList.Length == 0) return null;

			// 適用条件アイテムからレコメンド設定抽出
			var recommends = ExtractApplyConditionItem(targetCartList, displayPage);
			if (recommends.Length == 0) return null;

			// 既にカートに適用されているレコメンドを除外する
			var applyRecommendIds =
				this.CurrentCartList.SelectMany(cart =>
					cart.Items
					.Where(i => string.IsNullOrEmpty(i.RecommendId) == false)
					.Select(i => i.RecommendId)).Distinct().ToArray();
			if (recommends.Any(r => applyRecommendIds.Contains(r.RecommendId))) return null;

			// ワンタイム表示有効で過去に適用済みのレコメンドを除外
			var service = new RecommendService();
			var histories = service.GetRecommendHistoryByUserId(targetCartList.First().ShopId, this.UserId);
			var canApplyRecommends = recommends
				.Where(recommend =>
					((recommend.IsOnetime == false)
						|| histories.All(history => history.RecommendId != recommend.RecommendId))
					&& ((recommend.IsUpsell == false)
						|| recommend.Items.Length > 0))
				.ToArray();
			if (canApplyRecommends.Any() == false) return null;
			
			// レコメンドアイテムからレコメンド設定抽出
			return ExtractRecommendItem(targetCartList, canApplyRecommends);
		}

		/// <summary>
		/// 適用条件アイテムからレコメンド設定抽出
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		/// <param name="displayPage">表示ページ</param>
		/// <returns>レコメンド設定リスト</returns>
		private RecommendModel[] ExtractApplyConditionItem(CartObject[] cartList, string displayPage)
		{
			// 注文＆カート商品から適用条件アイテムリストを取得
			var applyConditionItemsForOrderAndCart = 
				GetApplyConditionItemsForOrderAndCart(cartList);
			
			var recommends = new List<RecommendModel>();
			foreach (var recommend in this.ApplicableRecommends.Where(r => r.RecommendDisplayPage == displayPage))
			{
				// 購入している？（過去注文もしくはカート）
				var buyApplyConditionItems =
					recommend.ApplyConditionItems.Where(i => i.IsRecommendApplyConditionTypeBuy).ToArray();
				if (buyApplyConditionItems.Length != 0)
				{
					// 通常/定期商品 AND 商品ID OR 商品バリエーションIDが一致している？
					if (buyApplyConditionItems.Any(i =>
						applyConditionItemsForOrderAndCart.Any(ioc => MatchApplyConditionItem(i, ioc))) == false) continue;
				}

				// 購入していない？（過去注文もしくはカート）
				var notBuyApplyConditionItems =
					recommend.ApplyConditionItems.Where(i => i.IsRecommendApplyConditionTypeNotBuy).ToArray();
				if (notBuyApplyConditionItems.Length != 0)
				{
					// 通常/定期商品 AND 商品ID OR 商品バリエーションIDが一致している？
					if (notBuyApplyConditionItems.Any(i =>
						applyConditionItemsForOrderAndCart.Any(ioc => MatchApplyConditionItem(i, ioc)))) continue;
				}

				if (IsShippingStorePickup(cartList))
				{
					var validRecommend = RemoveInvalidStorePickupRecommendProducts(recommend);
					if (validRecommend != null) recommends.Add(validRecommend);
				}
				else
				{
					recommends.Add(recommend);
				}
			}

			return recommends.ToArray();
		}

		/// <summary>
		/// 適用条件アイテムが一致しているか？
		/// </summary>
		/// <param name="applyConditionItem">適用条件アイテム</param>
		/// <param name="applyConditionItemForOrderAndCart">注文＆カート商品用適用条件アイテム</param>
		/// <returns>一致している：true、一致していない：false</returns>
		private bool MatchApplyConditionItem(RecommendApplyConditionItemModel applyConditionItem, ApplyConditionItemForOrderAndCart applyConditionItemForOrderAndCart)
		{
			// 通常/定期商品/頒布会商品が一致している？
			if ((applyConditionItem.IsFixedPurchase != applyConditionItemForOrderAndCart.IsFixedPurchase)
				|| (applyConditionItem.IsSubscriptionBox != applyConditionItemForOrderAndCart.IsSubscriptionBox)) return false;

			// 商品指定？
			if (applyConditionItem.IsRecommendApplyConditionItemTypeProduct)
			{
				// 商品IDが一致している？
				return (applyConditionItem.RecommendApplyConditionItemProductId == applyConditionItemForOrderAndCart.ProductId);
			}
			// 商品バリエーション指定？
			else
			{
				// 商品ID＆商品バリエーションIDが一致している？
				return (applyConditionItem.RecommendApplyConditionItemProductId == applyConditionItemForOrderAndCart.ProductId)
					&& (applyConditionItem.RecommendApplyConditionItemVariationId == applyConditionItemForOrderAndCart.VariationId);
			}
		}

		/// <summary>
		/// 注文＆カート商品用適用条件アイテムリストを取得
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		/// <returns>注文＆カート商品用適用条件アイテムリスト</returns>
		private ApplyConditionItemForOrderAndCart[] GetApplyConditionItemsForOrderAndCart(CartObject[] cartList)
		{
			// ユーザーIDが存在する場合
			// 注文商品から適用条件アイテムを追加
			var applyConditionItems = new List<ApplyConditionItemForOrderAndCart>();
			if (string.IsNullOrEmpty(this.UserId) == false)
			{
				var orders = new OrderService().GetUncancelledOrderInfosByUserId(this.UserId);
				applyConditionItems
					.AddRange(orders.SelectMany(o => 
						o.Shippings.SelectMany(s => 
							s.Items.Select(i =>
								new ApplyConditionItemForOrderAndCart(
									i.ProductId,
									i.VariationId,
									// 定期購入IDあり＆定期商品文字列あり = 定期商品とみなす
									((o.FixedPurchaseId != string.Empty) 
									&& i.ProductName.StartsWith(Constants.PRODUCT_FIXED_PURCHASE_STRING)),
									(string.IsNullOrEmpty(o.SubscriptionBoxCourseId) == false)))))
						.ToArray()
				);
			}
			// カート商品から適用条件アイテムを追加
			applyConditionItems
				.AddRange(cartList.SelectMany(c => 
					c.Items.Select(i =>
						new ApplyConditionItemForOrderAndCart(
							i.ProductId,
							i.VariationId,
							i.IsFixedPurchase,
							i.IsSubscriptionBox))).ToArray()
			);

			return applyConditionItems.ToArray();
		}

		/// <summary>
		/// レコメンドアイテムからレコメンド設定抽出
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		/// <param name="recommends">レコメンド設定リスト</param>
		/// <returns>レコメンド設定</returns>
		private RecommendModel ExtractRecommendItem(CartObject[] cartList, RecommendModel[] recommends)
		{
			foreach (var recommend in recommends)
			{
				// アップセルかつ、カート内にアップセル対象アイテムが存在しない場合は次のレコメンド設定へ
				var itemAddQuantity = 0;
				if (recommend.IsUpsell)
				{
					if (ExistsUpsellTargetItem(cartList, recommend, out itemAddQuantity) == false) continue;
				}

				// アップセル or クロスセル？
				if (recommend.IsUpsell || recommend.IsCrosssell)
				{
					// レコメンドアイテムが投入不可の場合は次のレコメンド設定へ
					if (CanAddRecomendItem(recommend, itemAddQuantity) == false) continue;
				}

				// 注文完了ページ向けレコメンド？
				if (recommend.CanDisplayOrderCompletePage)
				{
					// 決済方法が適正？
					if (IsValidPayment(cartList, recommend) == false) continue;
					// 利用クーポンが適正？
					if (IsValidCoupon(cartList, recommend) == false) continue;
				}

				return recommend;
			}

			return null;
		}

		/// <summary>
		/// カート内にアップセル対象アイテムが存在するか？
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		/// <param name="recommend">レコメンド設定リスト</param>
		/// <param name="itemAddQuantity">アップセル対象アイテム数量</param>
		/// <returns>存在する：true、存在しない：false</returns>
		private bool ExistsUpsellTargetItem(CartObject[] cartList, RecommendModel recommend, out int itemAddQuantity)
		{
			itemAddQuantity = 0;

			// アップセル対象アイテム取得
			// ※通常・定期＆商品ID＆商品バリエーションIDが同じ
			var upsellTargetItem = recommend.UpsellTargetItem;
			var cartItems =
				cartList.SelectMany(c => c.Items)
					.Where(i =>
						((upsellTargetItem.IsNormal && (i.IsFixedPurchase == false) && (i.IsSubscriptionBox == false))
							|| (upsellTargetItem.IsFixedPurchase && i.IsFixedPurchase && (i.IsSubscriptionBox == false))
							|| (upsellTargetItem.IsSubscriptionBox && i.IsSubscriptionBox))
						&& (i.ProductId == upsellTargetItem.RecommendUpsellTargetItemProductId)
						&& (i.VariationId == upsellTargetItem.RecommendUpsellTargetItemVariationId)
					).ToArray();

			if (cartItems.Length == 0) return false;

			// アップセル対象アイテム数量をセット
			itemAddQuantity = cartItems.Sum(i => i.Count);
			return true;
		}

		/// <summary>
		/// レコメンドアイテムが投入可能か？
		/// </summary>
		/// <param name="recommend">レコメンド設定</param>
		/// <param name="itemAddQuantity">アップセル対象アイテム数量</param>
		/// <returns>投入可：true、投入不可：false</returns>
		private bool CanAddRecomendItem(RecommendModel recommend, int itemAddQuantity)
		{
			foreach (var item in recommend.Items)
			{
				// 商品情報（バリエーション含む）取得
				var product =
					ProductCommon.GetProductInfo(item.ShopId, item.RecommendItemProductId, this.MemberRankId, string.Empty);
				// バリエーションIDで絞込
				product.RowFilter = 
					string.Format("{0}='{1}'",
					Constants.FIELD_PRODUCTVARIATION_VARIATION_ID,
					item.RecommendItemVariationId);
				// レコメンドアイテムが存在しない場合はレコメンドは有効としない
				if (product.Count == 0) return false;

				// 商品状態チェックエラーの場合はレコメンドは有効としない
				var buyQuantity =
					item.IsRecommendItemAddQuantityTypeSameQuantity
					? itemAddQuantity
					: item.RecommendItemAddQuantity;

				var addCartKbn = new Constants.AddCartKbn();
				if (item.IsNormal) addCartKbn = Constants.AddCartKbn.Normal;
				if (item.IsFixedPurchase) addCartKbn = Constants.AddCartKbn.FixedPurchase;
				if (item.IsSubscriptionBox) addCartKbn = Constants.AddCartKbn.SubscriptionBox;

				if (OrderCommon.CheckProductStatus(product[0], buyQuantity, addCartKbn, this.UserId) != OrderErrorcode.NoError)
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// 有効な決済方法か
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		/// <param name="recommend">レコメンド設定</param>
		/// <returns>有効か</returns>
		private bool IsValidPayment(CartObject[] cartList, RecommendModel recommend)
		{
			var products = recommend.Items.Select(item =>
				new ProductService().GetProductVariation(item.ShopId, item.RecommendItemProductId, item.RecommendItemVariationId, string.Empty)).ToArray();
			var shippings = products.Select(product => DataCacheControllerFacade.GetShopShippingCacheController().Get(product.ShippingId)).Distinct().ToArray();
			var paymentIds = cartList.Select(cart => cart.Payment.PaymentId).Distinct().ToArray();

			// 下記を満たせばOK
			// 1.レコメンド商品に紐づいている全ての配送種別で使用可能
			// 2.レコメンド商品で使用不可設定されていない
			// 3.完了ページ向けの場合、決済方法がレコメンドで使用可能
			var isValid = paymentIds.All(payment => 
				(products.All(product => product.IsLimitedPaymentId(payment) == false)
					&& shippings.All(shipping => shipping.IsPermittedPayment(payment))
					&& (recommend.CanDisplayOrderConfirmPage 
						|| Constants.RECOMMENDOPTION_APPLICABLE_PAYMENTIDS_FOR_ORDER_COMPLETE.Contains(payment))));
			return isValid;
		}

		/// <summary>
		/// 利用クーポンがレコメンド適用後も適正か
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		/// <param name="recommend">レコメンド設定</param>
		/// <returns>利用クーポンが適正か</returns>
		private bool IsValidCoupon(CartObject[] cartList, RecommendModel recommend)
		{
			var couponUseCart = cartList.Where(cart => cart.IsCouponUse()).ToArray();
			// クーポン利用している場合のみ判定
			if (couponUseCart.Any() == false) return true;

			var couponService = new CouponService();
			var productService = new ProductService();
			var useCoupon = couponService.GetCoupon(couponUseCart.First().Coupon.DeptId, couponUseCart.First().Coupon.CouponId);
			// 下記を満たせばOK
			// 1.アップセルかつアップセル対象商品以外のカート内商品がクーポン利用対象商品
			// 2.レコメンド商品がクーポン利用除外対象商品でない
			var isValid = (
				(useCoupon != null)
				&& ((recommend.IsUpsell
						&& couponUseCart.All(cart => cart.Items
							.Where(item => (item.IsFixedPurchase != recommend.UpsellTargetItem.IsFixedPurchase)
								|| (item.ProductId != recommend.UpsellTargetItem.RecommendUpsellTargetItemProductId)
								|| (item.VariationId != recommend.UpsellTargetItem.RecommendUpsellTargetItemVariationId))
							.Any(item => CouponOptionUtility.IsCouponApplyCartProduct(useCoupon, item))))
					|| (couponUseCart.All(cart => recommend.Items.All(item =>
							CouponOptionUtility.IsCouponApplyProduct(
								useCoupon,
								productService.GetProductVariation(item.ShopId, item.RecommendItemProductId, item.RecommendItemVariationId, string.Empty)))))));
			return isValid;
		}

		/// <summary>
		/// Is shipping store pickup
		/// </summary>
		/// <param name="cartList">Cart list</param>
		/// <returns>True if cart list has the cart is shipping store pickup, otherwise false</returns>
		private bool IsShippingStorePickup(CartObject[] cartList)
		{
			if (cartList.Any(cart => string.IsNullOrEmpty(cart.OrderId) == false))
			{
				var orderShippings = DomainFacade.Instance.OrderService.GetShippingAll(cartList[0].OrderId);
				var isShippingStorePickup = orderShippings.Any(item => string.IsNullOrEmpty(item.StorePickupRealShopId) == false)
					|| cartList.Any(cart => cart.IsShippingStorePickup);
				return isShippingStorePickup;
			}

			if (cartList.Any(cart => cart.Shippings[0].IsShippingStorePickup)) return true;

			return false;
		}

		/// <summary>
		/// Remove invalid storepickup recommend products
		/// </summary>
		/// <param name="recommend">Recommend model</param>
		/// <returns>Valid recommend</returns>
		private RecommendModel RemoveInvalidStorePickupRecommendProducts(RecommendModel recommend)
		{
			// Ignore change input
			var result = recommend.Clone();
			var validRecommendProducts = new List<RecommendItemModel>();
			foreach (var item in result.Items)
			{
				var product = DomainFacade.Instance.ProductService.Get(item.ShopId, item.RecommendItemProductId);
				if ((product == null)
					|| (product.StorePickupFlg == Constants.FLG_PRODUCT_STOREPICKUP_FLG_INVALID)) continue;

				validRecommendProducts.Add(item);
			}

			if (recommend.RecommendKbn == Constants.FLG_RECOMMEND_RECOMMEND_KBN_CROSS_SELL && validRecommendProducts.Count == 0)
			{
				result = null;
			}
			else
			{
				result.Items = validRecommendProducts.ToArray();
			}
			return result;
		}
		#endregion

		#region プロパティ
		/// <summary>ユーザID</summary>
		private string UserId { get; set; }
		/// <summary>会員ランクID</summary>
		private string MemberRankId { get; set; }
		/// <summary>現在のカートリスト</summary>
		private CartObject[] CurrentCartList { get; set; }
		/// <summary>有効なレコメンド設定リスト</summary>
		private RecommendModel[] ApplicableRecommends { get; set; }
		#endregion
	}

	/// <summary>
	/// 注文＆カート商品用適用条件アイテムクラス
	/// </summary>
	[Serializable]
	public class ApplyConditionItemForOrderAndCart
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="isFixedPurchase">定期商品か？</param>
		/// /// <param name="isSubscriptionBox">頒布会商品か？</param>
		public ApplyConditionItemForOrderAndCart(string productId, string variationId, bool isFixedPurchase, bool isSubscriptionBox)
		{
			this.ProductId = productId;
			this.VariationId = variationId;
			this.IsFixedPurchase = isFixedPurchase;
			this.IsSubscriptionBox = isSubscriptionBox;
		}
		#endregion

		#region プロパティ
		/// <summary>商品ID</summary>
		public string ProductId { get; private set; }
		/// <summary>商品バリエーションID</summary>
		public string VariationId { get; private set; }
	/// <summary>定期商品か？</summary>
		public bool IsFixedPurchase { get; private set; }
		/// <summary>頒布会商品か？</summary>
		public bool IsSubscriptionBox { get; private set; }
		#endregion
	}
}
