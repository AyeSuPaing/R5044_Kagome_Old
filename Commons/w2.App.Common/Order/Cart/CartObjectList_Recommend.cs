/*
=========================================================================================================
  Module      : レコメンド処理系カートオブジェクトリストクラスのパーシャルクラス(CartObjectList_Recommend.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using Microsoft.ReportingServices.Interfaces;
using System;
using System.Collections;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Product;
using w2.App.Common.Web.WrappedContols;
using w2.Domain;
using w2.Domain.Holiday.Helper;
using w2.Domain.Payment;
using w2.Domain.Product;
using w2.Domain.Recommend;
using w2.Domain.ShopShipping;
using w2.Domain.User;

namespace w2.App.Common.Order
{
	/// <summary>
	/// レコメンド処理系カートオブジェクトリストクラスのパーシャルクラス
	/// </summary>
	public partial class CartObjectList : IEnumerable
	{
		/// <summary>
		/// レコメンドアイテム投入
		/// </summary>
		/// <param name="recommend">レコメンド設定</param>
		public void AddRecommendItem(RecommendModel recommend)
		{
			if (recommend == null) return;

			// カート商品情報からアップセル対象アイテム取得
			var itemAddQuantity = 0;
			var cartItems = GetUpsellTargetCartItems(recommend);
			if (cartItems.Length != 0)
			{
				// アップセル対象アイテム数量を取得
				itemAddQuantity = cartItems.Sum(i => i.Count);
			}

			// レコメンドアイテム投入
			AddRecommendItemInner(recommend, itemAddQuantity, cartItems);

			// アップセル対象アイテム削除
			DeleteUpsellTargetCartItem(cartItems);

			// 配送方法更新
			foreach (var cartObject in this.Items)
			{
				foreach (var shipping in cartObject.Shippings)
				{
					shipping.UpdateShippingMethod(
						DataCacheControllerFacade
							.GetShopShippingCacheController()
							.Get(cartObject.ShippingType));
				}
			}

			// Update first shipping date, next shipping date and next next shipping for fixed purchase
			UpdateFirstShippingDateForFixedPurchase();
		}

		/// <summary>
		/// カート商品情報からアップセル対象アイテム取得
		/// </summary>
		/// <param name="recommend">レコメンド設定</param>
		private CartProduct[] GetUpsellTargetCartItems(RecommendModel recommend)
		{
			// アップセルではない場合は処理を抜ける
			if (recommend.IsUpsell == false) return new CartProduct[0];

			// カート商品情報からアップセル対象アイテムを取得
			// ※通常・定期＆商品ID＆商品バリエーションIDが同じ
			var upsellTargetItem = recommend.UpsellTargetItem;
			var cartList = this.Items.SelectMany(c => c.Items).Where(
				i => (i.IsFixedPurchase == (upsellTargetItem.IsFixedPurchase || (upsellTargetItem.IsSubscriptionBox
						&& (string.IsNullOrEmpty(i.SubscriptionBoxCourseId) == false))))
					&& (i.ProductId == upsellTargetItem.RecommendUpsellTargetItemProductId)
					&& (i.VariationId == upsellTargetItem.RecommendUpsellTargetItemVariationId))
				.ToArray();
			return cartList;
		}

		/// <summary>
		/// レコメンドアイテム投入
		/// </summary>
		/// <param name="recommend">レコメンド設定</param>
		/// <param name="cartItems">アップセル対象アイテム</param>
		/// <param name="itemAddQuantity">アップセル対象アイテム数量</param>
		private void AddRecommendItemInner(RecommendModel recommend, int itemAddQuantity, CartProduct[] cartItems)
		{
			var cartItemsProductIds = cartItems.Select(product => product.ProductId).ToArray();
			foreach (var item in recommend.Items)
			{
				// 商品情報取得
				var recommendItem = ProductCommon.GetProductVariationInfo(
					item.ShopId,
					item.RecommendItemProductId,
					item.RecommendItemVariationId,
					this.MemberRankId);
				// データが無ければ次のレコメンド商品へ
				if (recommendItem.Count == 0) continue;

				// 頒布会商品の場合、頒布会コースIDを設定
				if (item.IsSubscriptionBox)
				{
					var subsctiptionBoxCourse = DataCacheControllerFacade
						.GetSubscriptionBoxCacheController()
						.GetSubscriptionBoxesByProductId(
							item.ShopId,
							item.RecommendItemProductId,
							item.RecommendItemVariationId).First();
					item.SubscriptionBoxCourseId = subsctiptionBoxCourse.SelectableProducts
						.FirstOrDefault(
							x => (x.ProductId == item.RecommendItemProductId)
								&& (x.VariationId == item.RecommendItemVariationId)).SubscriptionBoxCourseId;
				}

				// 追加数量取得
				var buyQuantity = item.IsRecommendItemAddQuantityTypeSameQuantity
					? itemAddQuantity
					: item.RecommendItemAddQuantity;
				var addCartKbn = new Constants.AddCartKbn();

				if (item.IsNormal) addCartKbn = Constants.AddCartKbn.Normal;
				if (item.IsFixedPurchase) addCartKbn = Constants.AddCartKbn.FixedPurchase;
				if (item.IsSubscriptionBox) addCartKbn = Constants.AddCartKbn.SubscriptionBox;

				// カートにレコメンドアイテム追加
				// ※レコメンドIDをセットする
				var cartProduct = new CartProduct(
					recommendItem[0],
					addCartKbn,
					string.Empty,
					buyQuantity,
					true,
					new ProductOptionSettingList(),
					string.Empty,
					null,
					item.SubscriptionBoxCourseId)
				{
					RecommendId = recommend.RecommendId
				};

				// 完了画面向けの場合はカート分割されないようにする
				AddProduct(
					cartProduct,
					((item.IsSubscriptionBox == false) && recommend.CanDisplayOrderCompletePage),
					false);

				// 頒布会アップセル かつ 必須商品がカート未投入の場合追加
				if (item.IsSubscriptionBox)
				{
					var subscriptionBox = DomainFacade.Instance.SubscriptionBoxService.GetByCourseId(item.SubscriptionBoxCourseId);
					if (subscriptionBox != null)
					{
						var necessaryItems = subscriptionBox.DefaultOrderProducts
							.Where(defaultItem => defaultItem.IsNecessary)
							.Select(
								defaultItem => ProductCommon.GetProductVariationInfo(
									defaultItem.ShopId,
									defaultItem.ProductId,
									defaultItem.VariationId,
									this.MemberRankId))
							.Where(
								productInfo => ((productInfo.Count > 0)
									&& (this.Items.First().Items.Any(
										cartItem => (cartItem.IsSubscriptionBox
											&& (cartItem.ProductId == (string)productInfo[0][Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID])
											&& (cartItem.VariationId == (string)productInfo[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]))) == false)))
							.Select(
								subscriptionItem => new CartProduct(
									subscriptionItem[0],
									addCartKbn,
									string.Empty,
									buyQuantity,
									true,
									new ProductOptionSettingList(),
									string.Empty,
									subscriptionBoxCourseId: item.SubscriptionBoxCourseId)
								{
									RecommendId = recommend.RecommendId,
								})
							.ToArray();

						foreach (var necessaryItem in necessaryItems)
						{
							AddProduct(necessaryItem, false, false);
						}
					}
				}

				var addedCart = this.Items.First(
					cart => cart.Items.Any(
						cp => (cp.ProductId == cartProduct.ProductId)
							&& (cp.VariationId == cartProduct.VariationId)
							&& (cp.IsFixedPurchase == cartProduct.IsFixedPurchase)));
				var product = new ProductModel(recommendItem[0]);
				var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(product.ShippingId);

				// 注文完了後のレコメンドで新規カートが作られる場合は情報が欠落するのでコピー
				if (recommend.CanDisplayOrderCompletePage
					&& (addedCart.Payment == null))
				{
					var cart1 = this.Items.First();
					var cart1Shipping = cart1.GetShipping();
					var deliveryCompanyId = shopShipping.GetDefaultDeliveryCompany(cart1Shipping.IsExpress).DeliveryCompanyId;
					addedCart.Owner = cart1.Owner;
					addedCart.Payment = cart1.Payment;
					addedCart.GetShipping().UpdateSenderAddr(cart1.Owner, true);
					addedCart.GetShipping().UpdateShippingAddr(cart1.Owner, true);
					addedCart.GetShipping().UpdateShippingMethod(
						cart1Shipping.ShippingMethod,
						deliveryCompanyId);
					addedCart.Payment = cart1.Payment;
					addedCart.OrderMemos = cart1.OrderMemos;
					addedCart.OrderExtend = cart1.OrderExtend;

					addedCart.AdvCodeNew = cart1.AdvCodeNew;

					if (OrderCommon.DisplayTwInvoiceInfo()
						&& cart1Shipping.IsShippingAddrTw)
					{
						addedCart.GetShipping().UpdateInvoice(cart1Shipping.UniformInvoiceType,
							cart1Shipping.UniformInvoiceOption1,
							cart1Shipping.UniformInvoiceOption2,
							cart1Shipping.CarryType,
							cart1Shipping.CarryTypeOptionValue);
					}

					// Update shipping address for EcPay
					if (Constants.RECEIVINGSTORE_TWECPAY_CVSOPTION_ENABLED
						&& (cart1Shipping.ConvenienceStoreFlg == Constants.FLG_ORDERSHIPPING_SHIPPING_ADDR_KBN_CONVENIENCE_STORE_ON))
					{
						addedCart.GetShipping().UpdateConvenienceStoreShippingAddressForEcPay(
							cart1Shipping.Name,
							cart1Shipping.Addr4,
							cart1Shipping.Tel1,
							cart1Shipping.DeliveryCompanyId,
							cart1Shipping.ConvenienceStoreId,
							cart1Shipping.ShippingReceivingStoreType);
					}

					var isValidShippingTime = (cart1Shipping.DeliveryCompanyId == deliveryCompanyId);
					addedCart.GetShipping().UpdateShippingDateTime(
						cart1Shipping.ShippingDate.HasValue,
						(string.IsNullOrEmpty(cart1Shipping.ShippingTime) == false),
						shopShipping.IsValidShippingDate(DateTime.Today, cart1Shipping.ShippingDate)
							? cart1Shipping.ShippingDate
							: null,
						isValidShippingTime ? cart1Shipping.ShippingTime : string.Empty,
						isValidShippingTime ? cart1Shipping.ShippingTimeMessage : string.Empty);
					addedCart.BeforeRecommendOrderId = cart1.OrderId;

					// クーポン情報は新規カートが1カートにのみ引き継ぐ(多重適用しない)
					if ((cart1.Coupon != null)
						&& (this.Items.Any(cart => ((cart.Coupon != null) && (cart.CartId != cart1.CartId))) == false))
					{
						addedCart.Coupon = cart1.Coupon;
					}
				}

				// Amazon Pay Cv2でレコメンドをクリックした場合、決済方法と注文メモを元のカートと同じにします
				if ((this.Items[0].Payment.PaymentId == Constants.FLG_PAYMENT_PAYMENT_ID_AMAZON_PAYMENT_CV2)
					&& (addedCart.Payment == null))
				{
					// Amazon Pay Cv2決済が利用できる場合決済方法をAmazon Pay Cv2にする
					var amazonPay = new PaymentService().Get(item.ShopId, OrderCommon.GetAmazonPayPaymentId());
					if ((amazonPay.ValidFlg == Constants.FLG_PAYMENT_VALID_FLG_VALID)
						&& (amazonPay.UserManagementLevelNotUse
							!= new UserService().Get(this.UserId).UserManagementLevelId)
						&& addedCart.CanUseAmazonPayment())
					{
						addedCart.Payment = this.Items[0].Payment;
						addedCart.OrderMemos = this.Items[0].OrderMemos;
					}
				}

				// レコメンド商品が定期の場合、配送パターンをチェック
				if (cartProduct.IsFixedPurchase)
				{
					CartShipping shipping;
					var settingTmp = string.Empty;
					var fixedPurchaseKbn = string.Empty;

					// 定期->定期のアップセルと定期専用カートの定期->定期のクロスセルは注文時の配送パターンを適用する
					if ((Constants.RECOMMENDOPTION_IS_FORCED_FIXEDPURCHASE_SETTING_BY_RECOMMEND == false)
						&& (((recommend.UpsellTargetItem != null) 
							&& (recommend.UpsellTargetItem.IsFixedPurchase 
								|| (recommend.UpsellTargetItem.IsSubscriptionBox && (string.IsNullOrEmpty(cartProduct.SubscriptionBoxCourseId) == false))))
							|| (Constants.FIXEDPURCHASE_OPTION_CART_SEPARATION && recommend.IsCrosssell))
						&& HasApplicableFixedPurchaseSetting(product, shopShipping, out shipping))
					{
						// レコメンド商品に適用できる配送パターンならそれを使う
						addedCart.GetShipping().UpdateFixedPurchaseSetting(
							shipping.FixedPurchaseKbn,
							shipping.FixedPurchaseSetting,
							shipping.FixedPurchaseDaysRequired,
							shipping.FixedPurchaseMinSpan);
						addedCart.GetShipping().UpdateNextShippingDates(
							shipping.NextShippingDate,
							shipping.NextNextShippingDate);
						settingTmp = shipping.FixedPurchaseSetting;
						fixedPurchaseKbn = shipping.FixedPurchaseKbn;
					}
					else
					{
						addedCart.GetShipping().UpdateFixedPurchaseSetting(
							item.FixedPurchaseKbn,
							item.FixedPurchaseSetting1,
							shopShipping.FixedPurchaseShippingDaysRequired,
							shopShipping.FixedPurchaseMinimumShippingSpan);
						var useFirstShippingDate = recommend.CanDisplayOrderCompletePage
							|| (recommend.CanDisplayOrderConfirmPage && cartItems.Any(cartItem => cartItem.IsFixedPurchase));
						addedCart.GetShipping().CalculateNextShippingDates(useFirstShippingDate);
						settingTmp = item.FixedPurchaseSetting1;
						fixedPurchaseKbn = item.FixedPurchaseKbn;
					}

					if (Constants.FIXEDPURCHASE_NEXTSHIPPING_OPTION_ENABLED)
					{
						var cartFixedPurchaseNextShippingProduct = addedCart.Items.OrderBy(cp => cp.ProductId)
							.FirstOrDefault(cartProductInfo => (cartProductInfo.CanSwitchProductFixedPurchaseNextShippingSecondTime()
								&& (cartItemsProductIds.Contains(cartProductInfo.ProductId) == false)));

						if (cartFixedPurchaseNextShippingProduct != null)
						{
							addedCart.GetShipping().CanSwitchProductFixedPurchaseNextShippingSecondTime = true;
							addedCart.GetShipping().UpdateNextShippingItemFixedPurchaseInfos(
								cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseKbn,
								cartFixedPurchaseNextShippingProduct.NextShippingItemFixedPurchaseSetting);
							addedCart.GetShipping().CalculateNextShippingItemNextNextShippingDate();
						}
					}
				}
			}
		}

		/// <summary>
		/// 頒布会必須商品追加
		/// </summary>
		/// <param name="recommend">レコメンド設定</param>
		/// <param name="subscriptionBoxCourseId">頒布会コースID</param>
		public void AddSubscriptionNecessaryItems(RecommendModel recommend, string subscriptionBoxCourseId)
		{
			var subscriptionBox = DomainFacade.Instance.SubscriptionBoxService.GetByCourseId(subscriptionBoxCourseId);
			if (subscriptionBox == null) return;

			var targetCart = this.Items.FirstOrDefault();
			if (targetCart == null) return;

			var shipping = targetCart.GetShipping().Clone();

			// 頒布会コースIDを補完
			targetCart.SubscriptionBoxCourseId = subscriptionBoxCourseId;
			foreach (var item in targetCart.Items)
			{
				item.SubscriptionBoxCourseId = subscriptionBoxCourseId;
			}

			// カート商品情報からアップセル対象アイテム取得
			var itemAddQuantity = 0;
			var cartItems = GetUpsellTargetCartItems(recommend);
			if (cartItems.Length != 0)
			{
				// アップセル対象アイテム数量を取得
				itemAddQuantity = cartItems.Sum(i => i.Count);
			}

			var necessaryItems = subscriptionBox.DefaultOrderProducts
				.Where(defaultItem => defaultItem.IsNecessary)
				.Select(
					defaultItem => ProductCommon.GetProductVariationInfo(
						defaultItem.ShopId,
						defaultItem.ProductId,
						defaultItem.VariationId,
						this.MemberRankId))
				.Where(
					productInfo => ((productInfo.Count > 0)
						&& (targetCart.Items.Any(
							cartItem => (cartItem.IsSubscriptionBox
								&& (cartItem.ProductId == (string)productInfo[0][Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID])
								&& (cartItem.VariationId == (string)productInfo[0][Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]))) == false)))
				.Select(
					subscriptionItem => new CartProduct(
						subscriptionItem[0],
						Constants.AddCartKbn.SubscriptionBox,
						string.Empty,
						itemAddQuantity,
						true,
						new ProductOptionSettingList(),
						string.Empty,
						subscriptionBoxCourseId: subscriptionBoxCourseId)
					{
						RecommendId = recommend.RecommendId,
					})
				.ToArray();

			foreach (var necessaryItem in necessaryItems)
			{
				AddProduct(necessaryItem, false, false);
			}

			// AddProductで配送パターンが飛ぶため補完
			targetCart.SetShippingAddressAndShippingDateTime(shipping);
		}

		/// <summary>
		/// いずれかのカートがレコメンド商品に適用可能な配送パターンがあるか
		/// </summary>
		/// <param name="product">商品情報</param>
		/// <param name="shopShipping">レコメンド商品配送種別情報</param>
		/// <param name="shipping">配送先情報</param>
		/// <returns>適用可能な配送パターンがあるか</returns>
		private bool HasApplicableFixedPurchaseSetting(ProductModel product, ShopShippingModel shopShipping, out CartShipping shipping)
		{
			shipping = null;
			if (this.Items.Any(cart => cart.HasFixedPurchase) == false) return false;

			foreach (var cart in this.Items.Where(i => i.HasFixedPurchase))
			{
				if (shopShipping.IsValidFixedPurchaseKbn1Flg
					&& (cart.GetShipping().FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_DATE))
				{
					var monthAndDay = cart.GetShipping().FixedPurchaseSetting.Split(',');
					var month = (monthAndDay.Length > 1) ? monthAndDay[0] : "1";
					if (shopShipping.IsValidFixedPurchaseKbn1Setting(month)
						&& (product.IsLimitedFixedPurchaseSetting1(month) == false))
					{
						shipping = cart.GetShipping();
						return true;
					}
				}
				if (shopShipping.IsValidFixedPurchaseKbn2Flg
					&& (cart.GetShipping().FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_MONTHLY_WEEKANDDAY))
				{
					shipping = cart.GetShipping();
					return true;
				}
				if (shopShipping.IsValidFixedPurchaseKbn3Flg
					&& (cart.GetShipping().FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_INTERVAL_BY_DAYS))
				{
					if (shopShipping.IsValidFixedPurchaseKbn3Setting(cart.GetShipping().FixedPurchaseSetting)
						&& (product.IsLimitedFixedPurchaseSetting3(cart.GetShipping().FixedPurchaseSetting) == false))
					{
						shipping = cart.GetShipping();
						return true;
					}
				}
				if (shopShipping.IsValidFixedPurchaseKbn4Flg
					&& (cart.GetShipping().FixedPurchaseKbn == Constants.FLG_FIXEDPURCHASE_FIXED_PURCHASE_KBN_WEEK_AND_DAY))
				{
					if (shopShipping.IsValidFixedPurchaseKbn4Setting(cart.GetShipping().FixedPurchaseSetting)
						&& (product.IsLimitedFixedPurchaseSetting4(cart.GetShipping().FixedPurchaseSetting) == false))
					{
						shipping = cart.GetShipping();
						return true;
					}
				}
			}

			return false;
		}

		/// <summary>
		/// アップセル対象アイテム削除
		/// </summary>
		/// <param name="cartItems">アップセル対象アイテム</param>
		private void DeleteUpsellTargetCartItem(CartProduct[] cartItems)
		{
			// アップセル対象アイテム削除
			foreach (var cartItem in cartItems)
			{
				this.DeleteProduct(
					cartItem.ShopId,
					cartItem.ProductId,
					cartItem.VariationId,
					cartItem.AddCartKbn,
					cartItem.ProductSaleId,
					cartItem.ProductOptionSettingList.GetDisplayProductOptionSettingSelectValues()
				);
			}
		}

		/// <summary>
		/// Update first shipping date for fixed purchase
		/// </summary>
		private void UpdateFirstShippingDateForFixedPurchase()
		{
			var fixedPurchaseService = DomainFacade.Instance.FixedPurchaseService;
			foreach (var cart in this.Items)
			{
				if (cart.HasFixedPurchase == false) continue;

				var recommendProductFixedPurchase = cart.Items.FirstOrDefault(cp =>
					(cp.IsFixedPurchase && (string.IsNullOrEmpty(cp.RecommendId) == false)));
				if (recommendProductFixedPurchase == null) continue;

				// Update fixed purchase if first shipping date next month flag is true and shipping method is not mail
				var shopShipping = DataCacheControllerFacade.GetShopShippingCacheController().Get(recommendProductFixedPurchase.ShippingType);
				var cartShipping = cart.GetShipping();
				// Get next 1 month for first shipping date dropdown list option
				var calculateMode = fixedPurchaseService.GetCalculationMode(
					cartShipping.FixedPurchaseKbn,
					Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);
				if ((shopShipping.FixedPurchaseFirstShippingNextMonthFlg
						== Constants.FLG_SHOPSHIPPING_FIXED_PURCHASE_FIRST_SHIPPING_NEXT_MONTH_FLG_VALID)
					&& (cartShipping.IsMail == false))
				{
					// Calculate first shipping date
					var deliveryCompanyId = shopShipping.GetDefaultDeliveryCompany(cartShipping.IsExpress).DeliveryCompanyId;
					var addressOfJp = string.IsNullOrEmpty(cartShipping.Addr1)
						? Constants.CONST_DEFAULT_SHIPPING_ADDR1
						: cartShipping.Addr1;
					var addressOfTw = Constants.TW_COUNTRY_SHIPPING_ENABLE
						? string.IsNullOrEmpty(cartShipping.Addr2)
							? Constants.CONST_DEFAULT_SHIPPING_ADDRESS2_TW
							: cartShipping.Addr2
						: string.Empty;

					var zip = string.IsNullOrEmpty(cartShipping.Zip)
						? "dummy"
						: cartShipping.Zip;

					// Calculate earliest, next and next next shipping date
					var earliesShippingDate = OrderCommon.GetFirstShippingDateBasedOnToday(
						recommendProductFixedPurchase.ShopId,
						(shopShipping.FixedPurchaseShippingDaysRequired + shopShipping.BusinessDaysForShipping),
						null,
						cartShipping.ShippingMethod,
						deliveryCompanyId,
						cartShipping.ShippingCountryIsoCode,
						cartShipping.IsTaiwanCountryShippingEnable
							? addressOfTw
							: addressOfJp,
						zip);

					var firstShippingDate = fixedPurchaseService.CalculateFirstShippingDate(
						cartShipping.FixedPurchaseKbn,
						cartShipping.FixedPurchaseSetting.Remove(0, 1).Insert(0, "1"),
						earliesShippingDate.AddMonths(-1),
						shopShipping.FixedPurchaseMinimumShippingSpan,
						calculateMode);

					if (firstShippingDate < earliesShippingDate)
					{
						firstShippingDate = fixedPurchaseService.CalculateFirstShippingDate(
							cartShipping.FixedPurchaseKbn,
							cartShipping.FixedPurchaseSetting,
							earliesShippingDate,
							shopShipping.FixedPurchaseMinimumShippingSpan,
							calculateMode);
					}
					else if (firstShippingDate.Month == DateTime.Now.Month)
					{
						firstShippingDate = fixedPurchaseService.CalculateFirstShippingDate(
							cartShipping.FixedPurchaseKbn,
							cartShipping.FixedPurchaseSetting,
							firstShippingDate,
							shopShipping.FixedPurchaseMinimumShippingSpan,
							calculateMode);
					}

					// Update next and next next shipping date
					cartShipping.ShippingDate = firstShippingDate;
					var nextShippingDate = fixedPurchaseService.CalculateFollowingShippingDate(
						cartShipping.FixedPurchaseKbn,
						cartShipping.FixedPurchaseSetting,
						firstShippingDate,
						shopShipping.FixedPurchaseMinimumShippingSpan,
						calculateMode);
					var nextNextShippingDate = fixedPurchaseService.CalculateFollowingShippingDate(
						cartShipping.FixedPurchaseKbn,
						cartShipping.FixedPurchaseSetting,
						nextShippingDate,
						shopShipping.FixedPurchaseMinimumShippingSpan,
						calculateMode);
					cartShipping.UpdateNextShippingDates(nextShippingDate, nextNextShippingDate);
					cartShipping.SpecifyShippingDateFlg = true;
				}
			}
		}

		#region 注文確認ページレコメンド プロパティ
		/// <summary>注文確認ページレコメンド設定</summary>
		public RecommendModel RecommendOrderConfirm { get; set; }
		/// <summary>注文確認ページレコメンド表示履歴枝番</summary>
		public int RecommendHistoryNoOrderConfirm { get; set; }
		#endregion
	}
}
