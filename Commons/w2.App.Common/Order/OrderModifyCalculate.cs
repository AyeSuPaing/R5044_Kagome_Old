/*
=========================================================================================================
  Module      : マイページからの編集再計算処理(OrderModifyCalculate.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W２ Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Region.Currency;
using w2.App.Common.Input.Order;
using w2.App.Common.Option;
using w2.App.Common.Product;
using w2.Common.Util;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.DeliveryCompany;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Point;
using w2.Domain.Point.Helper;
using w2.Domain.SetPromotion;
using w2.Domain.ShopShipping;
using w2.Domain.User;
using w2.Domain.UserIntegration;

namespace w2.App.Common.Order
{
	/// <summary>
	/// マイページからの編集再計算処理クラス
	/// </summary>
	public class OrderModifyCalculate
	{
		/// <summary>
		/// Front受注編集用再計算
		/// </summary>
		/// <param name="orderOld">以前の受注情報</param>
		/// <param name="orderNew">新しい受注情報</param>
		/// <param name="addAutoNovelty">ノベルティを自動付与するか</param>
		public string ReCalculate(OrderInput orderOld, OrderInput orderNew, bool addAutoNovelty = true)
		{
			this.OrderInputOld = orderOld;
			this.OrderInput = new OrderInput(orderNew.CreateModel());
			SetUserPointProperty(orderOld, orderNew);

			// 自動計算適用
			ApplyAutoCalculation(orderNew, addAutoNovelty);

			// 注文情報更新
			if (this.HasError == false)
			{
				this.OrderInput.Update(orderNew);

				var orderModel = this.OrderInput.CreateModel();
				var isFirstOrder = DomainFacade.Instance.OrderService.CheckOrderFirstBuy(orderNew.UserId, orderNew.OrderId);
				var pointFirstBuy = isFirstOrder
					? PointOptionUtility.GetOrderPointAddForOrder(orderModel, Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY)
					: 0m;
				var pointOrder = PointOptionUtility.GetOrderPointAddForOrder(orderModel, Constants.FLG_POINTRULE_POINT_INC_KBN_BUY);
				this.OrderInput.OrderPointAdd = (pointFirstBuy + pointOrder).ToString();
			}

			var result = GetAllErrorMessage();
			return result;
		}

		/// <summary>
		/// 各プロパティセット ユーザポイント
		/// </summary>
		private void SetUserPointProperty(OrderInput orderInputOld, OrderInput orderInput)
		{
			this.IsNoPointPublished = Constants.FLG_USERPOINT_POINT_PUBLISHED;
			this.User = new UserService().Get(orderInput.UserId);

			// 統合前のユーザを取得
			this.BeforeIntegrationUser =
				new UserIntegrationService().GetBeforeIntegrationUserByOrderId(orderInput.OrderId);
			if (UserService.IsUser(this.User.UserKbn) == false) return;
			var userPointRelatedOrder = new PointService().GetUserPoint(this.User.UserId, string.Empty)
				.Where(x => (x.IsPointTypeComp && x.IsBasePoint)
					|| (x.IsLimitedTermPoint && x.OrderId == orderInputOld.OrderId)
					|| (x.IsPointTypeTemp && x.OrderId == orderInputOld.OrderId)).ToArray();

			this.UserPointList = new UserPointList(userPointRelatedOrder);

			// 注文関連ポイント情報(通常本ポイントだけ除く)
			this.UserPointRelatedThisOrder = this.UserPointList.GetOrderPoint();

			// 比較用に古いのも保存する
			this.UserPointOldRelatedThisOrder = this.UserPointList.GetOrderPoint();

			if (this.UserPointOldRelatedThisOrder.Items.Count == 0)
			{
				this.IsNoPointPublished = Constants.FLG_USERPOINT_POINT_NOT_PUBLISHED;
			}

			// 通常本ポイントの区分枝番を取得
			var basePointComp = userPointRelatedOrder.FirstOrDefault(up => (up.IsPointTypeComp && up.IsBasePoint));
			this.UserBasePointCompPointKbnNo = (basePointComp != null) ? basePointComp.PointKbnNo.ToString() : "";
		}

		#region 再計算処理
		/// <summary>
		/// 自動計算適用
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <param name="addAutoNovelty">ノベルティを自動付与するか</param>
		private void ApplyAutoCalculation(OrderInput order, bool addAutoNovelty)
		{
			// カート作成
			this.Cart = CreateCart(order, addAutoNovelty);

			var oldCart = CreateCart(this.OrderInputOld, addAutoNovelty);
			CalculateCart(this.OrderInputOld, oldCart);

			// カート再計算
			CalculateCart(order, this.Cart);

			// ノベルティ・商品同梱を設定
			SetNoveltyAndBundle(this.Cart, addAutoNovelty);

			// 配送サービスのエスカレーションを設定
			SetEscalationMailDeliveryCompany(this.Cart);

			// カート再計算
			CalculateCart(order, this.Cart);

			// ポイントとクーポンのチェック
			CheckPointAndCoupon(this.Cart);

			// カート情報を注文入力情報にセット
			SetCartInfoToOrder(order, this.Cart);
		}

		/// <summary>
		/// カート情報作成
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <param name="addAutoNovelty"></param>
		/// <returns>カート情報</returns>
		private CartObject CreateCart(OrderInput order, bool addAutoNovelty)
		{
			// カートオブジェクト作成
			var cart = new CartObject(order.UserId, order.OrderKbn, order.ShopId, order.ShippingId, isDigitalContentsOnly: false, updateCartDb: false)
			{
				MallId = order.MallId,
				ShippingTaxRate = decimal.Parse(order.ShippingTaxRate),
				PaymentTaxRate = decimal.Parse(order.PaymentTaxRate),
				Payment = new CartPayment()
			};

			cart.Payment.UpdateCartPayment(
				order.OrderPaymentKbn,
				order.OrderPaymentKbn,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				string.Empty,
				paymentObject: null,
				isSamePaymentAsCart1: true,
				rakutenCvvToken: string.Empty);
			decimal.TryParse(order.OrderPriceExchange, out var orderPriceExchange);
			cart.Payment.PriceExchange = orderPriceExchange;

			// 配送先を追加する前に、クリアする
			cart.Shippings.Clear();

			// 配送先追加
			for (var i = 0; i < order.Shippings.Length; i++)
			{
				var cartShipping = new CartShipping(cart);
				cartShipping.ShippingAddrKbn = CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW;
				cartShipping.ShippingNo = order.Shippings[i].OrderShippingNo;
				cartShipping.UpdateShippingAddr(
					order.Shippings[i].DataSource,
					blIsSameShippingAsCart1: true,
					strShippingAddrKbn: CartShipping.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_NEW);
				cartShipping.ShippingMethod = order.Shippings[i].ShippingMethod;
				cartShipping.DeliveryCompanyId = order.Shippings[i].DeliveryCompanyId;

				cart.Shippings.Add(cartShipping);
			}

			// 注文商品リスト取得
			var orderItems = order.Shippings.SelectMany(s => s.Items).ToArray();

			// カート商品セットリスト取得
			var cartProductSets = new Dictionary<string, CartProductSet>();
			foreach (var orderItem in orderItems.Where(i => i.IsProductSet).ToArray())
			{
				if (cartProductSets.ContainsKey(orderItem.ProductSetId)) continue;

				var productSetInfo = ProductCommon.GetProductSetInfo(orderItem.ShopId, orderItem.ProductSetId);
				if (productSetInfo.Count == 0) continue;

				var cartProductSet = new CartProductSet(
					productSetInfo[0],
					int.Parse(orderItem.ProductSetCount),
					int.Parse(orderItem.ProductSetNo),
					blUpdateCartDb: false);
				cartProductSets.Add(orderItem.ProductSetId, cartProductSet);
			}

			// カート商品追加
			foreach (var orderItem in orderItems)
			{
				// 返品商品の場合は、次の商品へ
				if (orderItem.IsReturnItem) continue;
				// 削除対象の場合は、次の商品へ
				if (orderItem.DeleteTarget) continue;
				// 編集時削除対象の場合は、次の商品へ
				if (orderItem.ModifyDeleteTarget) continue;

				// ノベルティ・商品同梱は一旦外す
				if ((addAutoNovelty && (string.IsNullOrEmpty(orderItem.NoveltyId) == false))
					|| (string.IsNullOrEmpty(orderItem.ProductBundleId) == false))
				{
					continue;
				}

				var itemQuantity = int.Parse(orderItem.ItemQuantity);
				var productPrice = decimal.Parse(orderItem.ProductPrice);

				var taxRate = orderItem.ProductTaxRate;
				var addCartKbn = order.IsGiftOrder
					? Constants.AddCartKbn.GiftOrder
					:
					orderItem.IsFixedPurchaseItem
						?
						(string.IsNullOrEmpty(order.SubscriptionBoxCourseId) == false)
							? Constants.AddCartKbn.SubscriptionBox
							: Constants.AddCartKbn.FixedPurchase
						: Constants.AddCartKbn.Normal;

				var cartProduct = cart.GetProduct(
					orderItem.ShopId,
					orderItem.ProductId,
					orderItem.VariationId,
					orderItem.IsProductSet,
					orderItem.IsFixedPurchaseItem,
					orderItem.ProductsaleId,
					orderItem.ProductOptionTexts,
					orderItem.ProductBundleId,
					productPrice: productPrice);

				// カート商品が存在する場合は、商品数セット
				if (cartProduct != null)
				{
					cartProduct.Count = cartProduct.Count + itemQuantity;
					cartProduct.CountSingle = cartProduct.CountSingle + itemQuantity;
				}
				// カート商品が存在しない場合は、カート商品追加
				else
				{
					var product = ProductCommon.GetProductVariationInfo(
						orderItem.ShopId,
						orderItem.ProductId,
						orderItem.VariationId,
						order.MemberRankId);
					if (product.Count == 0) continue;
					var cartProductTemp =
						new CartProduct(
							product[0],
							addCartKbn,
							orderItem.ProductsaleId,
							itemQuantity,
							blUpdateCartDb: false,
							strProductOptionTexts: orderItem.ProductOptionTexts,
							fixedPurchaseId: orderItem.IsFixedPurchaseItem ? order.FixedPurchaseId : string.Empty)
						{
							RecommendId = orderItem.RecommendId,
							FixedPurchaseItemOrderCountInput = orderItem.IsFixedPurchaseItem
								? orderItem.FixedPurchaseItemOrderCount
								: string.Empty,
						};

					// セット商品？
					if (cartProductSets.ContainsKey(orderItem.ProductSetId))
					{
						cartProductTemp = cartProductSets[orderItem.ProductSetId]
							.AddProductVirtual(product[0], cartProductTemp.CountSingle);
					}

					if (cartProductTemp == null) continue;
					// 商品税率セット
					cartProductTemp.TaxRate = decimal.Parse(taxRate);

					// 商品価格セット
					// ※入力内容を正とする
					cartProductTemp.SetPrice(productPrice);

					// カートに追加
					cart.AddVirtural(cartProductTemp, false);
				}
			}

			// ギフト注文？
			if (order.IsGiftOrder)
			{
				// 配送先に紐づける
				var cartShippingIndex = 0;
				foreach (var orderShipping in order.Shippings)
				{
					foreach (var orderItem in orderShipping.Items)
					{
						// 返品商品の場合は、次の商品へ
						if (orderItem.IsReturnItem) continue;
						// 削除対象の場合は、次の商品へ
						if (orderItem.DeleteTarget) continue;
						// Not calculate item empty
						if (string.IsNullOrEmpty(orderItem.ProductId)) continue;
						// 編集時削除対象の場合は、次の商品へ
						if (orderItem.ModifyDeleteTarget) continue;

						var productPrice = decimal.Parse(orderItem.ProductPrice);
						var cartProduct = cart.GetProduct(
							orderItem.ShopId,
							orderItem.ProductId,
							orderItem.VariationId,
							orderItem.IsProductSet,
							orderItem.IsFixedPurchaseItem,
							orderItem.ProductsaleId,
							orderItem.ProductOptionTexts,
							orderItem.ProductBundleId,
							productPrice: productPrice);

						cart.Shippings[cartShippingIndex].ProductCounts.Add(
							new CartShipping.ProductCount(cartProduct, int.Parse(orderItem.ItemQuantity)));
					}

					cartShippingIndex++;
				}

				// 商品数セット
				foreach (var cartProduct in cart.Items)
				{
					cartProduct.CountSingle = cart.Shippings
						.Select(s => s.ProductCounts.Where(p => p.Product == cartProduct))
						.Sum(s => s.Sum(p => p.Count));
					cartProduct.Calculate();
				}

				cart.CalculateWithCartShipping();
			}

			cart.PriceInfoByTaxRate.AddRange(
				order.OrderPriceByTaxRates.Select(
					orderPriceByTaxRate => new CartPriceInfoByTaxRate(orderPriceByTaxRate.CreateModel())).ToList());
			cart.PriceRegulation = decimal.Parse(order.OrderPriceRegulation);
			cart.EnteredShippingPrice = decimal.Parse(order.OrderPriceShipping);
			cart.EnteredPaymentPrice = decimal.Parse(order.OrderPriceExchange);
			cart.SubscriptionBoxCourseId = order.SubscriptionBoxCourseId;
			cart.SubscriptionBoxFixedAmount = order.IsSubscriptionBoxFixedAmount
				? (decimal?)decimal.Parse(order.SubscriptionBoxFixedAmount)
				: null;
			cart.OrderSubscriptionBoxOrderCount = int.TryParse(
				order.OrderSubscriptionBoxOrderCount,
				out var parsedOrderSubscriptionBoxOrderCount)
				? parsedOrderSubscriptionBoxOrderCount
				: 0;

			return cart;
		}

		/// <summary>
		/// カート再計算
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <param name="cart">カート情報</param>
		private void CalculateCart(OrderInput order, CartObject cart)
		{
			// 利用ポイントセット
			if (Constants.W2MP_POINT_OPTION_ENABLED && (UserService.IsUser(this.User.UserKbn)))
			{
				cart.SetUsePoint(decimal.Parse(order.OrderPointUse), decimal.Parse(order.OrderPointUseYen));
			}

			// 利用クーポンセット
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				if ((order.Coupon != null) && (string.IsNullOrEmpty(order.Coupon.CouponCode) == false))
				{
					// クーポン変更なし？
					UserCouponDetailInfo coupon = null;
					if (this.OrderInputOld.Coupon != null
						&& (order.Coupon.CouponCode == this.OrderInputOld.Coupon.CouponCode))
					{
						var coupons = new CouponService().GetAllUserCouponsFromCouponId(
							this.OrderInput.Coupon.DeptId,
							order.UserId,
							this.OrderInputOld.Coupon.CouponId,
							int.Parse(this.OrderInputOld.Coupon.CouponNo));
						if (coupons.Length != 0) coupon = coupons[0];
					}
					// クーポン指定あり or クーポン変更あり？
					else
					{
						var coupons = new CouponService().GetAllUserCouponsFromCouponCode(
							this.OrderInput.Coupon.DeptId,
							order.UserId,
							order.Coupon.CouponCode);
						if (coupons.Length != 0) coupon = coupons[0];
					}

					if (coupon != null)
					{
						cart.Coupon = new CartCoupon(coupon);
					}
				}
			}

			if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
			{
				// 定期会員判定セット
				cart.IsFixedPurchaseMember =
					(this.User.FixedPurchaseMemberFlg == Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON);

				// 定期購入情報セット
				if (order.IsFixedPurchaseOrder)
				{
					// 定期購入回数を注文情報の「定期購入回数(注文時点)-1」に変更
					// ※cart.Calculateで+1しているため、-1している
					var fixedPurchase = new FixedPurchaseService().Get(order.FixedPurchaseId);
					fixedPurchase.OrderCount = int.Parse(order.FixedPurchaseOrderCount) - 1;

					// 定期商品購入も同様に変更
					foreach (var item in fixedPurchase.Shippings[0].Items)
					{
						var orderItem = order.Shippings[0].Items.FirstOrDefault(
							i => (i.ProductId == item.ProductId) && (i.VariationId == item.VariationId));
						var orderItemCount =
							((orderItem == null) || (string.IsNullOrEmpty(orderItem.FixedPurchaseItemOrderCount)))
								? "1"
								: orderItem.FixedPurchaseItemOrderCount;
						item.ItemOrderCount = int.Parse(orderItemCount) - 1;
					}

					cart.FixedPurchase = fixedPurchase;
				}
			}

			if (int.TryParse(this.OrderInput.FixedPurchaseOrderCount, out var fixedPurchaseOrderCount) == false)
			{
				fixedPurchaseOrderCount = 0;
			}

			var cartSetpromotion = new List<SetPromotionModel>();
			if (this.OrderInputOld.SetPromotions.Any())
			{

				foreach (var setpromotion in this.OrderInputOld.SetPromotions)
				{
					var setPromotionModel = new SetPromotionService().Get(setpromotion.SetpromotionId);
					if (setPromotionModel == null) continue;
					if ((setPromotionModel.EndDate < DateTime.Now)
						|| (setPromotionModel.ValidFlg == Constants.FLG_SETPROMOTION_VALID_FLG_INVALID))
					{
						cartSetpromotion.Add(setPromotionModel);
					}
				}
			}

			cart.SetPromotionsOld = cartSetpromotion;

			// 再計算
			cart.Calculate(
				isDefaultShipping: false,
				fixedPurchaseOrderCount: fixedPurchaseOrderCount,
				isShippingChanged: true,
				isFrontOrderModify: true);

			// 初回購入ポイントセット
			// ※cart.Calculateでセットされないため
			if (Constants.W2MP_POINT_OPTION_ENABLED && UserService.IsUser(this.User.UserKbn)
				&& cart.IsOrderGrantablePoint)
			{
				var firstOrderPoint =
					PointOptionUtility.GetPointRulePriorityHigh(Constants.FLG_POINTRULE_POINT_INC_KBN_FIRST_BUY);
				var firstOrder = new OrderService().GetFirstOrder(this.User.UserId);
				if ((firstOrder != null) && (firstOrder.OrderId == order.OrderId) && firstOrderPoint.Any())
				{
					cart.SetFirstBuyPoint();
				}
			}
		}

		/// <summary>
		/// ノベルティ・商品同梱設定
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="addAutoNovelty">カート情報</param>
		private void SetNoveltyAndBundle(CartObject cart, bool addAutoNovelty)
		{
			if (addAutoNovelty)
			{
				SetCartNovelty(cart);
			}

			SetCartProductBundle(cart);
		}

		/// <summary>
		/// カートノベルティリストをセット
		/// </summary>
		/// <returns>カートに変更があったか</returns>
		private void SetCartNovelty(CartObject cart)
		{
			if (Constants.NOVELTY_OPTION_ENABLED)
			{
				var cartList = new CartObjectList(
					cart.OrderUserId,
					cart.OrderKbn,
					cart.UpdateCartDb,
					cart.FixedPurchase != null ? cart.FixedPurchase.FixedPurchaseId : string.Empty,
					cart.MemberRankId);
				cartList.AddCartVirtural(cart);
				// カートノベルティリスト作成
				var cartNoveltyList = new CartNoveltyList(cartList);

				// ノベルティ付与
				var cartItemCountBefore = cartList.Items.Count;
				AddProductGrantNovelty(cartNoveltyList, cartList);

				// ノベルティカート作成
				if (cartItemCountBefore != cartList.Items.Count)
				{
					cartNoveltyList = new CartNoveltyList(cartList);
				}

				// 付与条件外のカート内の付与アイテムを削除
				cartList.RemoveNoveltyGrantItem(cartNoveltyList);
				// カートに追加された付与アイテムを含むカートノベルティを削除
				cartNoveltyList.RemoveCartNovelty(cartList);
			}
		}

		/// <summary>
		/// ノベルティ付与
		/// </summary>
		/// <param name="cartNoveltyList">ノベルティリスト</param>
		/// <param name="cartObjectList">カートオブジェクトリスト</param>
		public void AddProductGrantNovelty(CartNoveltyList cartNoveltyList, CartObjectList cartObjectList)
		{
			var cartList = ObjectUtility.DeepCopy(cartObjectList);

			foreach (var cart in cartList.Items)
			{
				var noveltyForCart = cartNoveltyList.GetCartNovelty(cart.CartId);
				var cartNoveltyItem = noveltyForCart.Where(
					item => ((item.GrantItemList.Length > 0)
						&& (item.AutoAdditionalFlg == Constants.FLG_NOVELTY_AUTO_ADDITIONAL_FLG_VALID))).ToArray();

				foreach (var noveltyItem in cartNoveltyItem)
				{
					var noveltyGrantItem = noveltyItem.GrantItemList[0];

					// Exists NoveltyId in Any cart
					if (cartObjectList.Items.SelectMany(
								anyCart => anyCart.Items.Where(item => (item.NoveltyId == noveltyGrantItem.NoveltyId)))
							.Any() || ((cartList.NoveltyIdsDelete != null)
							&& cartList.NoveltyIdsDelete.Contains(noveltyGrantItem.NoveltyId))) continue;

					// Exists Product Novelty has added
					if (cartObjectList.Items.Any(
							cartItem => cartItem.Items.Any(
								cartProduct => (string.IsNullOrEmpty(cartProduct.NoveltyId) == false)
									&& (cartProduct.ProductId == noveltyGrantItem.ProductId)
									&& (cartProduct.VariationId == noveltyGrantItem.VariationId)))) continue;

					if (cart.IsProductNoveltyHasDelete(
							noveltyGrantItem.ProductId,
							noveltyGrantItem.VariationId,
							cartList.NoveltyIdsDelete,
							cartNoveltyItem.ToArray())) continue;

					var product = ProductCommon.GetProductVariationInfo(
						noveltyGrantItem.ShopId,
						noveltyGrantItem.ProductId,
						noveltyGrantItem.VariationId,
						this.OrderInput.MemberRankId);
					if (product.Count != 0)
					{
						// カート商品（ノベルティID含む）を作成し、カートに追加
						var cartProduct = new CartProduct(
							product[0],
							Constants.AddCartKbn.Normal,
							string.Empty,
							1,
							true,
							new ProductOptionSettingList());
						cartProduct.NoveltyId = noveltyGrantItem.NoveltyId;
						cartObjectList.AddProduct(cartProduct);
					}
				}
			}
		}

		/// <summary>
		/// 商品同梱設定
		/// </summary>
		/// <param name="cart">カート</param>
		private void SetCartProductBundle(CartObject cart)
		{
			ProductBundleCommon.AddBundleItemsToCartList(
				new List<CartObject> { cart },
				this.User.UserId,
				this.OrderInput.AdvcodeFirst,
				this.OrderInput.AdvcodeNew,
				excludeOrderIds: new[] { "" },
				hitTargetListIds: new[] { "" },
				isFront: false);
		}

		/// <summary>
		/// カート情報を注文入力情報にセット
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <param name="cart">カート情報</param>
		private void SetCartInfoToOrder(OrderInput order, CartObject cart)
		{
			// 注文商品セット
			var orderShippingNo = 1;
			var orderItems = CreateOrderItems(order, cart);

			order.ShippingTaxRate = cart.ShippingTaxRate.ToString();
			order.PaymentTaxRate = cart.PaymentTaxRate.ToString();
			order.OrderPriceShipping = cart.PriceShipping.ToString();
			order.OrderPriceExchange = cart.Payment.PriceExchange.ToString();

			foreach (var orderShipping in order.Shippings)
			{
				orderShipping.Items = orderItems.Where(i => i.OrderShippingNo == orderShippingNo.ToString()).ToArray();

				// orderItems[x].OrderShippingNoの実体はcart.Shippings[y].ShippingNoではなくcart.Shippingsの並び順
				orderShipping.ShippingMethod = cart.Shippings[orderShippingNo - 1].ShippingMethod;
				orderShipping.DeliveryCompanyId = cart.Shippings[orderShippingNo - 1].DeliveryCompanyId;

				orderShippingNo++;
			}

			var newOrderPriceByTaxRates = order.OrderPriceByTaxRates.ToList();

			var newCartPriceInfoByTaxRate = cart.PriceInfoByTaxRate.ToList();

			var returnItemInfoByTaxRate = order.Shippings.SelectMany(s => s.Items).Where(item => item.IsReturnItem)
				.GroupBy(item => item.ProductTaxRate);

			foreach (var returnItem in returnItemInfoByTaxRate)
			{
				var returnItemPriceTax = returnItem.Sum(item => decimal.Parse(item.ItemPriceTax));
				var returnItemPrice = returnItem.Sum(
					item => TaxCalculationUtility.GetPriceTaxIncluded(
						decimal.Parse(item.ItemPrice),
						decimal.Parse(item.ItemPriceTax)));

				var targetCartPriceInfoByTaxRate = newCartPriceInfoByTaxRate.FirstOrDefault(
					cartPriceInfoByTaxRate => cartPriceInfoByTaxRate.TaxRate == decimal.Parse(returnItem.Key));

				if (targetCartPriceInfoByTaxRate == null)
				{
					targetCartPriceInfoByTaxRate = new CartPriceInfoByTaxRate()
					{
						TaxRate = decimal.Parse(returnItem.Key)
					};
					newCartPriceInfoByTaxRate.Add(targetCartPriceInfoByTaxRate);
				}

				targetCartPriceInfoByTaxRate.PriceSubtotal += returnItemPrice;
				targetCartPriceInfoByTaxRate.PriceTotal += returnItemPrice;
				targetCartPriceInfoByTaxRate.TaxPrice = TaxCalculationUtility.GetTaxPrice(
					targetCartPriceInfoByTaxRate.PriceTotal,
					targetCartPriceInfoByTaxRate.TaxRate,
					Constants.TAX_EXCLUDED_FRACTION_ROUNDING);
				cart.PriceSubtotalTax += returnItemPriceTax;
			}

			order.OrderPriceTax = newCartPriceInfoByTaxRate.Sum(priceByTaxRate => priceByTaxRate.TaxPrice).ToString();
			order.OrderPriceSubtotalTax = cart.PriceSubtotalTax.ToString();
			newOrderPriceByTaxRates.Clear();
			newOrderPriceByTaxRates.AddRange(
				newCartPriceInfoByTaxRate.Select(
					priceInfoByTax => new OrderPriceByTaxRateInput(priceInfoByTax.CreateModel())));
			newOrderPriceByTaxRates.ForEach(priceInfoByTax => priceInfoByTax.OrderId = order.OrderId);
			order.OrderPriceByTaxRates = newOrderPriceByTaxRates.ToArray();
			// 注文セットプロモーションセット
			if (Constants.SETPROMOTION_OPTION_ENABLED)
			{
				order.SetPromotions = CreateOrderSetPromotions(order, cart);
			}

			// 会員ランク割引セット
			if (Constants.MEMBER_RANK_OPTION_ENABLED)
			{
				order.MemberRankDiscountPrice = cart.MemberRankDiscount.ToString();
			}

			// クーポン割引セット
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				order.OrderCouponUse = cart.UseCouponPrice.ToString();
			}

			// 定期会員割引＆定期購入割引セット
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
			{
				if (Constants.MEMBER_RANK_OPTION_ENABLED)
				{
					order.FixedPurchaseMemberDiscountAmount = cart.FixedPurchaseMemberDiscountAmount.ToString();
				}

				order.FixedPurchaseDiscountPrice = cart.FixedPurchaseDiscount.ToString();
			}

			var isConvenience =
				(order.Shippings.Any(
						shipping => (shipping.ShippingReceivingStoreFlg
							== Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
					&& (order.OrderPaymentKbn == Constants.FLG_PAYMENT_PAYMENT_ID_CONVENIENCE_STORE));

			//決済金額
			var settlementCurrency = CurrencyManager.GetSettlementCurrency(this.OrderInput.OrderPaymentKbn);
			var settlementRate = CurrencyManager.GetSettlementRate(settlementCurrency);
			order.SettlementAmount = CurrencyManager.GetSettlementAmount(
				this.OrderInput.OrderId,
				this.OrderInput.OrderPaymentKbn,
				decimal.Parse(this.OrderInput.LastBilledAmount),
				settlementRate,
				settlementCurrency).ToString();
		}

		/// <summary>
		/// ポイントとクーポンの設定
		/// </summary>
		/// <param name="cart">カート情報</param>
		private void CheckPointAndCoupon(CartObject cart)
		{
			var orderPointUsablePrice = OrderCommon.GetOrderPointUsable(
				cart.PriceSubtotal,
				cart.PriceRegulationTotal,
				cart.MemberRankDiscount,
				cart.UseCouponPrice,
				cart.SetPromotions.ProductDiscountAmount,
				cart.FixedPurchaseMemberDiscountAmount,
				cart.FixedPurchaseDiscount);

			if (cart.UsePointPrice > orderPointUsablePrice)
			{
				this.OrderPointErrorMessages.Append(
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_POINT_PRICE_SUBTOTAL_MINUS_ERROR)
						.Replace("@@ 1 @@", StringUtility.ToPrice(orderPointUsablePrice)));
			}

			CheckCouponValidity(cart);
		}

		/// <summary>
		/// クーポン情報有効性チェック
		/// </summary>
		/// <param name="cart">カート情報</param>
		private void CheckCouponValidity(CartObject cart)
		{
			if ((Constants.W2MP_COUPON_OPTION_ENABLED == false)
				|| (this.OrderInputOld.HasCoupon == false)) return;

			foreach (var coupon in this.OrderInputOld.Coupons)
			{
				if (string.IsNullOrEmpty(coupon.CouponCode)) continue;

				var coupons = new CouponService().GetAllUserCouponsFromCouponCode(
					coupon.DeptId,
					this.OrderInput.UserId,
					coupon.CouponCode);
				if (coupons.Length == 0)
				{
					this.OrderCouponErrorMessages.Append(
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_COUPON_INVALID_ERROR)
							.Replace("@@ 1 @@", coupon.CouponCode));
					return;
				}

				// クーポン有効チェック
				var errorMessage = CouponOptionUtility.CheckCouponValidWithCart(cart, coupons[0]);
				if (string.IsNullOrEmpty(errorMessage) == false) this.OrderCouponErrorMessages.Append(errorMessage.Replace("@@ 1 @@", coupon.CouponCode));
			}

			// クーポン割引額チェック
			if (cart.UseCouponPrice <= 0)
			{
				this.OrderCouponErrorMessages.Append(
					CommerceMessages.GetMessages(CommerceMessages.ERRMSG_COUPON_DISCOUNT_PRICE_ZERO));
			}
		}

		/// <summary>
		/// カート情報から注文商品入力情報リスト作成
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <param name="cart">カート情報</param>
		/// <returns>注文商品入力情報リスト</returns>
		private OrderItemInput[] CreateOrderItems(OrderInput order, CartObject cart)
		{
			// 返品商品追加
			var orderItems = new List<OrderItemInput>();
			orderItems.AddRange(order.Shippings.SelectMany(s => s.Items).Where(i => (i.IsReturnItem)).ToArray());

			// 通常注文？
			if (cart.IsGift == false)
			{
				var orderItemNo = orderItems.Count + 1;
				foreach (var cartProduct in cart.Items.Where(cp => cp.QuantitiyUnallocatedToSet != 0).ToArray())
				{
					var itemQuantity =
						cartProduct.IsSetItem ? cartProduct.Count : cartProduct.QuantitiyUnallocatedToSet;
					var itemQuantitySingle = cartProduct.IsSetItem
						? cartProduct.CountSingle
						: cartProduct.QuantitiyUnallocatedToSet;
					var orderItem = new OrderItemInput (
						order,
						cartProduct,
						itemQuantity,
						itemQuantitySingle,
						orderShippingNo: 1,
						orderItemNo,
						orderSetPromotionNo: null,
						orderSetpromotionItemNo: null,
						cart.Shippings[0].IsDutyFree);

					orderItem.IsOrderHistoryModifyCopyTarget = GetOrderHistoryModifyCopyTargetFlag(orderItem, order);
					orderItems.Add(orderItem);

					orderItemNo++;
				}

				// セットプロモーション商品登録
				foreach (CartSetPromotion setpromotion in cart.SetPromotions)
				{
					var orderSetpromotionItemNo = 1;
					foreach (var cartProduct in setpromotion.Items)
					{
						var itemQuantity = cartProduct.QuantityAllocatedToSet[setpromotion.CartSetPromotionNo];
						var orderItem = new OrderItemInput (
							order,
							cartProduct,
							itemQuantity,
							itemQuantity,
							1,
							orderItemNo,
							setpromotion.CartSetPromotionNo,
							orderSetpromotionItemNo,
							cart.Shippings[0].IsDutyFree);

						orderItem.IsOrderHistoryModifyCopyTarget = GetOrderHistoryModifyCopyTargetFlag(orderItem, order);
						orderItems.Add(orderItem);

						orderItemNo++;
						orderSetpromotionItemNo++;
					}
				}
			}
			// ギフト注文？
			else
			{
				var productsAllocatedToSetAndShipping = new List<Hashtable>();

				// セットプロモーションあり？
				if (cart.SetPromotions.Items.Count != 0)
				{
					// 配送先商品をばらす
					var orderShippingNo = 1;
					var cartProducts = new List<Hashtable>();
					foreach (var cartShipping in cart.Shippings)
					{
						foreach (var productCount in cartShipping.ProductCounts)
						{
							for (var i = 0; i < productCount.Count; i++)
							{
								cartProducts.Add(
									new Hashtable
									{
										{ "product", productCount.Product },
										{ Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, orderShippingNo },
										{ "isDutyFree", cartShipping.IsDutyFree }
									});
							}
						}

						orderShippingNo++;
					}

					foreach (var cartProduct in cart.Items)
					{
						// 対象商品を抽出
						var targetCartProducts = cartProducts.FindAll(cp => (CartProduct)cp["product"] == cartProduct);

						// セットプロモーション情報を追加
						var i = 0;
						for (var j = 0; j < cartProduct.QuantitiyUnallocatedToSet; j++)
						{
							targetCartProducts[i].Add(Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, "");
							i++;
						}

						foreach (KeyValuePair<int, int> setpromotionitem in cartProduct.QuantityAllocatedToSet)
						{
							for (var j = 0; j < setpromotionitem.Value; j++)
							{
								targetCartProducts[i].Add(
									Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO,
									setpromotionitem.Key.ToString());
								i++;
							}
						}

						// 配送先、セットプロモーションでグループ化
						var groupedTargetCartProducts = targetCartProducts.GroupBy(
							p => p[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO] + ","
								+ p[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]);

						productsAllocatedToSetAndShipping.AddRange(
							groupedTargetCartProducts.Select(
								groupedTargetCartProduct => new Hashtable
								{
									{ "product", cartProduct },
									{
										Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO,
										int.Parse(groupedTargetCartProduct.Key.Split(',')[0])
									},
									{
										Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO,
										groupedTargetCartProduct.Key.Split(',')[1]
									},
									{
										Constants.FIELD_ORDERITEM_ITEM_QUANTITY,
										groupedTargetCartProduct.ToList().Count
									},
									{ "isDutyFree", groupedTargetCartProduct.First()["isDutyFree"] }
								}));
					}
				}
				else
				{
					var orderShippingNo = 1;
					foreach (var cartShipping in cart.Shippings)
					{
						productsAllocatedToSetAndShipping.AddRange(
							cartShipping.ProductCounts.Select(
								productCount => new Hashtable
								{
									{ "product", productCount.Product },
									{ Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO, orderShippingNo },
									{ Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO, "" },
									{ Constants.FIELD_ORDERITEM_ITEM_QUANTITY, productCount.Count },
									{ "isDutyFree", cartShipping.IsDutyFree }
								}));
						orderShippingNo++;
					}
				}

				// 通常商品登録
				var orderItemNo = 1;
				foreach (var product in productsAllocatedToSetAndShipping.Where(
							product => (string)product[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO] == ""))
				{
					var cartProduct = (CartProduct)product["product"];
					var itemQuantity = (int)product[Constants.FIELD_ORDERITEM_ITEM_QUANTITY];
					var orderShippingNo = (int)product[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO];
					var orderItem = new OrderItemInput (
						order,
						cartProduct,
						itemQuantity,
						itemQuantity,
						orderShippingNo,
						orderItemNo,
						null,
						null,
						(bool)product["isDutyFree"]);

					orderItem.IsOrderHistoryModifyCopyTarget = GetOrderHistoryModifyCopyTargetFlag(orderItem, order);
					orderItems.Add(orderItem);
					orderItemNo++;
				}

				// セットプロモーション商品登録
				foreach (CartSetPromotion setpromotion in cart.SetPromotions)
				{
					var orderSetpromotionItemNo = 1;
					foreach (var product in productsAllocatedToSetAndShipping.Where(
								product => (string)product[Constants.FIELD_ORDERITEM_ORDER_SETPROMOTION_NO]
									== setpromotion.CartSetPromotionNo.ToString()))
					{
						var cartProduct = (CartProduct)product["product"];
						var itemQuantity = (int)product[Constants.FIELD_ORDERITEM_ITEM_QUANTITY];
						var orderShippingNo = (int)product[Constants.FIELD_ORDERITEM_ORDER_SHIPPING_NO];
						var orderItem = new OrderItemInput (
							order,
							cartProduct,
							itemQuantity,
							itemQuantity,
							orderShippingNo,
							orderItemNo,
							setpromotion.CartSetPromotionNo,
							orderSetpromotionItemNo,
							(bool)product["isDutyFree"]);

						orderItem.IsOrderHistoryModifyCopyTarget = GetOrderHistoryModifyCopyTargetFlag(orderItem, order);
						orderItems.Add(orderItem);
						orderItemNo++;
						orderSetpromotionItemNo++;
					}
				}
			}

			// 商品連番セット（配送先毎）
			foreach (var orderItemsGroupByOrderShippingNo in orderItems.GroupBy(orderitem => orderitem.OrderShippingNo)
						.ToArray())
			{
				var itemIndex = 0;
				foreach (var orderitem in orderItemsGroupByOrderShippingNo.ToArray())
				{
					orderitem.ItemIndex = itemIndex.ToString();
					itemIndex++;
				}
			}

			return orderItems.ToArray();
		}

		/// <summary>
		/// カート情報から注文セットプロモーション入力情報作成
		/// </summary>
		/// <param name="order">注文入力情報</param>
		/// <param name="cart">カート情報</param>
		/// <returns>注文セットプロモーション入力情報</returns>
		private OrderSetPromotionInput[] CreateOrderSetPromotions(OrderInput order, CartObject cart)
		{
			// 注文セットプロモーション入力情報作成
			var orderSetPromotions = cart.SetPromotions.Cast<CartSetPromotion>().Select(
				cartSetPromotion => new OrderSetPromotionInput
				{
					OrderId = order.OrderId,
					OrderSetpromotionNo = cartSetPromotion.CartSetPromotionNo.ToString(),
					SetpromotionId = cartSetPromotion.SetpromotionId,
					SetpromotionName = cartSetPromotion.SetpromotionName,
					SetpromotionDispName =
						((order.OrderKbn == Constants.FLG_ORDER_ORDER_KBN_MOBILE)
							&& (cartSetPromotion.SetpromotionDispNameMobile != ""))
							? cartSetPromotion.SetpromotionDispNameMobile
							: cartSetPromotion.SetpromotionDispName,
					UndiscountedProductSubtotal = cartSetPromotion.UndiscountedProductSubtotal.ToString(),
					ProductDiscountFlg = cartSetPromotion.ProductDiscountFlg,
					ProductDiscountAmount =
						cartSetPromotion.IsDiscountTypeProductDiscount
							? cartSetPromotion.ProductDiscountAmount.ToString()
							: "0",
					ShippingChargeFreeFlg = cartSetPromotion.ShippingChargeFreeFlg,
					PaymentChargeFreeFlg = cartSetPromotion.PaymentChargeFreeFlg,
				}).ToArray();

			// 配送料＆決済手数料は注文情報から取得＆セット
			var appliedShippingChargeFree = false;
			var appliedPaymentChargeFree = false;
			foreach (var orderSetPromotion in orderSetPromotions)
			{
				orderSetPromotion.ShippingChargeDiscountAmount = "0";
				if (orderSetPromotion.IsDiscountTypeShippingChargeFree && (appliedShippingChargeFree == false))
				{
					var couponType = (order.Coupon != null) ? order.Coupon.CouponType : string.Empty;
					var couponFreeShippingFlg = ((order.Coupon != null) && (order.Coupon.CouponCode != ""))
						? new CouponService().GetCoupon(Constants.CONST_DEFAULT_DEPT_ID, order.Coupon.CouponId)
							.FreeShippingFlg
						: Constants.FLG_COUPON_FREE_SHIPPING_INVALID;
					if ((CouponOptionUtility.IsFreeShipping(couponType) == false)
						&& (couponFreeShippingFlg != Constants.FLG_COUPON_FREE_SHIPPING_VALID))
					{
						orderSetPromotion.ShippingChargeDiscountAmount = order.OrderPriceShipping;
					}

					appliedShippingChargeFree = true;
				}

				orderSetPromotion.PaymentChargeDiscountAmount = "0";
				if (orderSetPromotion.IsDiscountTypePaymentChargeFree && (appliedPaymentChargeFree == false))
				{
					orderSetPromotion.PaymentChargeDiscountAmount = order.OrderPriceExchange;
					appliedPaymentChargeFree = true;
				}
			}

			return orderSetPromotions;
		}

		#endregion

		/// <summary>
		/// 全てのエラーメッセージ取得
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		private string GetAllErrorMessage()
		{
			var errorMessage = new List<string>();

			if (string.IsNullOrEmpty(this.OrderErrorMessages.ToString()) == false)
				errorMessage.Add(this.OrderErrorMessages.ToString());
			if (string.IsNullOrEmpty(this.OrderOwnerErrorMessages.ToString()) == false)
				errorMessage.Add(this.OrderOwnerErrorMessages.ToString());
			if (string.IsNullOrEmpty(this.OrderShippingErrorMessages.ToString()) == false)
				errorMessage.Add(this.OrderShippingErrorMessages.ToString());
			if (string.IsNullOrEmpty(this.OrderItemErrorMessages.ToString()) == false)
				errorMessage.Add(this.OrderItemErrorMessages.ToString());
			if (string.IsNullOrEmpty(this.OrderPriceErrorMessages.ToString()) == false)
				errorMessage.Add(this.OrderPriceErrorMessages.ToString());
			if (string.IsNullOrEmpty(this.OrderPointErrorMessages.ToString()) == false)
				errorMessage.Add(this.OrderPointErrorMessages.ToString());
			if (string.IsNullOrEmpty(this.OrderCouponErrorMessages.ToString()) == false)
				errorMessage.Add(this.OrderCouponErrorMessages.ToString());
			if (string.IsNullOrEmpty(this.OrderPaymentErrorMessages.ToString()) == false)
				errorMessage.Add(this.OrderPaymentErrorMessages.ToString());
			if (string.IsNullOrEmpty(this.OrderReceiptErrorMessages.ToString()) == false)
				errorMessage.Add(this.OrderReceiptErrorMessages.ToString());
			if (string.IsNullOrEmpty(this.OrderExtendErrorMessages.ToString()) == false)
				errorMessage.Add(this.OrderExtendErrorMessages.ToString());
			if (string.IsNullOrEmpty(this.EscalationErrorMessages.ToString()) == false)
				errorMessage.Add(this.EscalationErrorMessages.ToString());

			return string.Join("\n", errorMessage);
		}

		/// <summary>
		/// タグ置換
		/// </summary>
		/// <param name="targetString">置換対象文字列リスト</param>
		/// <param name="countyIsoCode">国ISOコード（配送先の国によって切り替わるときに利用）</param>
		/// <returns>置換後文字列</returns>
		public static string ReplaceTag(string targetString, string countyIsoCode = "")
		{
			var result = Constants.TAG_REPLACER_DATA_SCHEMA.GetValue(
				targetString,
				Constants.GLOBAL_OPTION_ENABLE
					? Constants.CONFIGURATION_SETTING.ReadKbnList.Contains(
						ConfigurationSetting.ReadKbn.C200_CommonFront)
						? RegionManager.GetInstance().Region.LanguageLocaleId
						: Constants.OPERATIONAL_LANGUAGE_LOCALE_CODE
					: "",
				Constants.GLOBAL_OPTION_ENABLE ? countyIsoCode : "");
			return result;
		}

		/// <summary>
		/// 配送サービスエスカレーションを設定
		/// </summary>
		/// <param name="cart">カート情報</param>
		private void SetEscalationMailDeliveryCompany(CartObject cart)
		{
			var shopShipping = new ShopShippingService().Get(cart.ShopId, cart.ShippingType);
			var expressDefaultShippongCompany = shopShipping.CompanyListExpress.FirstOrDefault(item => item.IsDefault);
			if (expressDefaultShippongCompany == null) return;

			if (cart.IsGift == false)
			{
				if (this.OrderInputOld.Shippings[0].IsExpress) return;

				// サイズ係数閾値の判定を優先
				if (SetShippingMethodByMailEscalationCount(
					cart.Shippings[0],
					cart.Items,
					expressDefaultShippongCompany))
				{
					return;
				}

				if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED == false) return;

				var deliveryCompanyId = OrderCommon.DeliveryCompanyMailEscalation(cart.Items, shopShipping.CompanyListMail);

				if (deliveryCompanyId != cart.Shippings[0].DeliveryCompanyId)
				{
					cart.Shippings[0].DeliveryCompanyId = string.IsNullOrEmpty(deliveryCompanyId)
						? expressDefaultShippongCompany.DeliveryCompanyId
						: deliveryCompanyId;
				}

				if (string.IsNullOrEmpty(deliveryCompanyId))
					cart.Shippings[0].ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;

				if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
				{
					var deliveryCompanyName = new DeliveryCompanyService().Get(cart.Shippings[0].DeliveryCompanyId).DeliveryCompanyName;
					this.EscalationErrorMessages.Append(
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDER_MODIFY_ESCALATION)
							.Replace("@@ 1 @@", deliveryCompanyName));
				}

				return;
			}

			// ギフトの場合
			foreach (var shipping in cart.Shippings)
			{
				var oldShipping =
					this.OrderInputOld.Shippings.FirstOrDefault(old => old.OrderShippingNo == shipping.ShippingNo);
				if ((oldShipping != null) && oldShipping.IsExpress) continue;

				// サイズ係数閾値の判定を優先
				if (SetShippingMethodByMailEscalationCount(
					shipping,
					shipping.ProductCounts.Select(productCount => productCount.Product).ToArray(),
					expressDefaultShippongCompany))
				{
					continue;
				}

				if (Constants.DELIVERYCOMPANY_MAIL_ESCALATION_ENBLED == false) continue;

				var deliveryCompanyId = OrderCommon.DeliveryCompanyMailEscalation(
					shipping.ProductCounts.Select(p => p.Product),
					shopShipping.CompanyListMail);

				if (deliveryCompanyId != shipping.DeliveryCompanyId)
				{
					shipping.DeliveryCompanyId = string.IsNullOrEmpty(deliveryCompanyId)
						? expressDefaultShippongCompany.DeliveryCompanyId
						: deliveryCompanyId;
				}

				if (string.IsNullOrEmpty(deliveryCompanyId))
					shipping.ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;

				if (cart.Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
				{
					var deliveryCompanyName = new DeliveryCompanyService().Get(shipping.DeliveryCompanyId).DeliveryCompanyName;
					this.EscalationErrorMessages.Append(
						CommerceMessages.GetMessages(CommerceMessages.ERRMSG_FRONT_ORDER_MODIFY_ESCALATION)
							.Replace("@@ 1 @@", deliveryCompanyName));
				}
			}
		}

		/// <summary>
		/// 商品のサイズ係数によるメール便から宅配便への更新
		/// </summary>
		/// <param name="shipping">配送先</param>
		/// <param name="products">配送商品</param>
		/// <param name="expressDefaultShippongCompany">宅配便のデフォルト配送会社</param>
		/// <returns>宅配便に更新したか</returns>
		private bool SetShippingMethodByMailEscalationCount(
			CartShipping shipping,
			IEnumerable<CartProduct> products,
			ShopShippingCompanyModel expressDefaultShippongCompany)
		{
			var newShippingMethod = OrderCommon.GetShippingMethod(products);
			if (newShippingMethod != Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS) return false;

			shipping.ShippingMethod = Constants.FLG_SHOPSHIPPINGSHIPPINGCOMPANY_SHIPPING_KBN_EXPRESS;
			shipping.DeliveryCompanyId = expressDefaultShippongCompany.DeliveryCompanyId;
			return true;
		}

		/// <summary>
		/// 複製商品フラグ取得
		/// </summary>
		/// <param name="orderItem">注文商品情報</param>
		/// <param name="order">注文入力情報</param>
		/// <returns>複製商品フラグ</returns>
		public bool GetOrderHistoryModifyCopyTargetFlag(OrderItemInput orderItem, OrderInput order)
		{
			var beforeProduct = order.Shippings[0].Items.FirstOrDefault(item => item.IsSameProductWithPrice(orderItem));
			var result = beforeProduct?.IsOrderHistoryModifyCopyTarget ?? false;
			return result;
		}

		#region プロパティ
		protected string IsNoPointPublished { get; set; }
		/// <summary>注文エラーメッセージ</summary>
		protected UserPointList UserPointOldRelatedThisOrder { get; set; }
		/// <summary>リクエスト：注文ID</summary>
		protected string RequestOrderId { get; set; }
		/// <summary>注文入力情報</summary>
		public OrderInput OrderInput { get; set; } = new OrderInput();
		/// <summary>注文入力情報情報（変更前）</summary>
		public OrderInput OrderInputOld { get; set; }
		/// <summary>元注文情報（元注文 or 最後の返品注文）</summary>
		protected OrderModel OrderOld { get; set; }
		/// <summary>ユーザ情報</summary>
		public new UserModel User { get; set; }
		/// <summary>カート情報</summary>
		public new CartObject Cart { get; set; }
		/// <summary>ユーザ統合前ユーザ情報</summary>
		protected UserModel BeforeIntegrationUser { get; set; }
		/// <summary>配送種別情報</summary>
		protected ShopShippingModel ShopShipping { get; set; }
		/// <summary>配送方法情報</summary>
		protected string[] ShippingMethod { get; set; }
		/// <summary>配送会社情報</summary>
		protected DeliveryCompanyModel[] DeliveryCompany { get; set; }
		/// <summary>ユーザーポイント</summary>
		protected UserPointList UserPointList { get; set; }
		/// <summary>ユーザー仮ポイント情報</summary>
		protected UserPointList UserPointTempList { get; set; }
		/// <summary>ユーザー本ポイント</summary>
		protected UserPointList UserPointCompList { get; set; }
		/// <summary>注文に関連するポイント</summary>
		protected UserPointList UserPointRelatedThisOrder { get; set; }
		/// <summary>ユーザーポイント通常本ポイント区分枝番</summary>
		protected string UserBasePointCompPointKbnNo { get; set; }
		/// <summary>注文エラーメッセージ</summary>
		protected StringBuilder OrderErrorMessages { get; } = new StringBuilder();
		/// <summary>注文者エラーメッセージ</summary>
		protected StringBuilder OrderOwnerErrorMessages { get; } = new StringBuilder();
		/// <summary>注文配送先エラーメッセージ</summary>
		protected StringBuilder OrderShippingErrorMessages { get; } = new StringBuilder();
		/// <summary>注文アイテムエラーメッセージ</summary>
		protected StringBuilder OrderItemErrorMessages { get; } = new StringBuilder();
		/// <summary>注文金額エラーメッセージ</summary>
		protected StringBuilder OrderPriceErrorMessages { get; } = new StringBuilder();
		/// <summary>注文ポイントエラーメッセージ</summary>
		protected StringBuilder OrderPointErrorMessages { get; } = new StringBuilder();
		/// <summary>注文クーポンエラーメッセージ</summary>
		protected StringBuilder OrderCouponErrorMessages { get; } = new StringBuilder();
		/// <summary>割引額上限エラーメッセージ</summary>
		protected StringBuilder DiscountLimitErrorMessages { get; } = new StringBuilder();
		/// <summary>決済種別メッセージ</summary>
		protected StringBuilder OrderPaymentErrorMessages { get; } = new StringBuilder();
		/// <summary>注文領収書エラーメッセージ</summary>
		private StringBuilder OrderReceiptErrorMessages { get; } = new StringBuilder();
		/// <summary>注文拡張項目エラーメッセージ</summary>
		protected StringBuilder OrderExtendErrorMessages { get; } = new StringBuilder();
		/// <summary>配送サービスエスカレーションエラーメッセージ</summary>
		protected StringBuilder EscalationErrorMessages { get; } = new StringBuilder();
		/// <summary>エラーがあるか判定</summary>
		protected bool HasError
		{
			get
			{
				return (this.OrderErrorMessages.Length
					+ this.OrderOwnerErrorMessages.Length
					+ this.OrderShippingErrorMessages.Length
					+ this.OrderItemErrorMessages.Length
					+ this.OrderPriceErrorMessages.Length
					+ this.OrderPointErrorMessages.Length
					+ this.OrderCouponErrorMessages.Length
					+ this.OrderPaymentErrorMessages.Length
					+ this.OrderReceiptErrorMessages.Length
					+ this.OrderExtendErrorMessages.Length
					+ this.EscalationErrorMessages.Length != 0);
			}
		}

		#endregion
	}
}
