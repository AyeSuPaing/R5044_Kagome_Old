/*
=========================================================================================================
  Module      : カートセットプロモーション計算処理クラス(CartSetPromotionCalculator.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2012 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.DataCacheController;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Domain.SetPromotion;

namespace w2.App.Common.Order
{
	/// <summary>
	/// カートセットプロモーション計算処理クラス
	/// </summary>
	class CartSetPromotionCalculator
	{
		/// <summary>Cart products</summary>
		private const string CONST_CART_PRODUCTS = "cartproducts";
		/// <summary>Setting quantity</summary>
		private const string CONST_SETTING_QUANTITY = "setting_quantity";
		/// <summary>Quantity more flag on</summary>
		private const string CONST_QUANTITY_MORE_FLG_ON = "quantity_more_flg_on";

		/// <summary>
		/// カートの状態から最安になるセットプロモーションを取得
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="isManagerModify">管理画面での注文編集時か</param>
		/// <returns>最安になるカートセットプロモーションリスト</returns>
		public static CartSetPromotionList GetLowestCartSetPromotionList(CartObject cart, bool isManagerModify = false, List<SetPromotionModel> beforeSetPromotions = null)
		{
			// 適用可能なセットプロモーションの設定を取得
			var setPromotionSettings = DataCacheControllerFacade.GetSetPromotionCacheController()
				.GetApplicableSetPromotions(cart.MemberRankId, cart.OrderKbn, cart.TargetLists);

			if (beforeSetPromotions != null)
			{
				var list = setPromotionSettings.ToList();
				list.AddRange(beforeSetPromotions);
				setPromotionSettings = list.ToArray();
			}

			var targetCartItems = cart.Items.Take(Constants.SETPROMOTION_MAXIMUM_NUMBER_OF_TARGET_SKUS).ToList();
			// 単体で成立するセットプロモーションを取得
			var applicableCartSetPromotions = GetApplicableSetPromotions(cart, targetCartItems, setPromotionSettings);

			// 成立するセットが1個もなければ終了
			if (applicableCartSetPromotions.Count == 0) return null;

			// 動的計画法による、最安となるセットプロモーションの組み合わせの算出
			return GetLowestSetPromotionsCombination(targetCartItems, applicableCartSetPromotions, (isManagerModify ? cart : null));
		}

		/// <summary>
		/// 適用優先順でセットプロモーション適用
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <returns>適用できるセットプロモーション</returns>
		public static CartSetPromotionList GetSetPromotionsByPriority(CartObject cart)
		{
			// 対象カートアイテム取得（ノベルティ商品以外、価格降順で並び替える）
			var targetCartItems = cart.Items
				.Where(item => (item.IsNovelty == false))
				.OrderByDescending(x => x.Price)
				.ToArray();

			var cacheController = DataCacheControllerFacade.GetSetPromotionCacheController();
			var setPromotionSettingsList = new List<SetPromotionModel[]>();

			// 適用したい順にセットプロモーションをまとめる
			// 商品割引
			if (cart.PriceSubtotal != 0)
			{
				setPromotionSettingsList.Add(
					cacheController.GetProductDiscountSetPromotion(cart.MemberRankId, cart.OrderKbn, cart.TargetLists)
						.OrderBy(sp => sp.ApplyOrder).ThenBy(sp => sp.SetpromotionId).ToArray());
			}
			// 配送料無料
			if (cart.PriceShipping != 0)
			{
				setPromotionSettingsList.Add(
					cacheController.GetShippingDiscountSetPromotion(cart.MemberRankId, cart.OrderKbn, cart.TargetLists)
						.OrderBy(sp => sp.SetpromotionId).ToArray());
			}
			// 決済手数料無料
			if ((cart.Payment != null) && (cart.Payment.PriceExchange != 0))
			{
				setPromotionSettingsList.Add(
					cacheController.GetPaymentDiscountSetPromotion(cart.MemberRankId, cart.OrderKbn, cart.TargetLists)
						.OrderBy(sp => sp.SetpromotionId).ToArray());
			}

			// セットプロモーション適用
			var result = new CartSetPromotionList();
			foreach (var setPromotionSettings in setPromotionSettingsList)
			{
				// 適用可能な設定がないなら次へ
				if (setPromotionSettings.Length == 0) continue;

				var list = ApplySetPromotions(cart, targetCartItems, setPromotionSettings, result);
				result.AddSetPromotions(list.Items);
			}
			return result;
		}

		/// <summary>
		/// セットプロモーション適用
		/// </summary>
		/// <param name="cart">カート情報</param>
		/// <param name="targetCartItems">商品情報（ノベルティ以外・金額降順に並んでる想定）</param>
		/// <param name="settings">セットプロモーション情報（優先度順に並んでる前提）</param>
		/// <param name="currentCartSetPromotionList">現在の適用するセットプロモーションリスト</param>
		/// <returns>適用できるセットプロモーション</returns>
		private static CartSetPromotionList ApplySetPromotions(
			CartObject cart,
			CartProduct[] targetCartItems,
			SetPromotionModel[] settings,
			CartSetPromotionList currentCartSetPromotionList)
		{
			var resultSetPromotionList = new CartSetPromotionList();

			// セットプロモーション未割当のみ対象とする
			var currentCartProducts = currentCartSetPromotionList.Items
				.SelectMany(cp => cp.tempItems.Select(y => y.Product))
				.ToArray();
			var cartItems = targetCartItems
				.Where(cp => (cp.IsSetItem == false) && (currentCartProducts.Contains(cp) == false))
				.Select(cp => new CartSetPromotion.Item(cp, cp.CountSingle))
				.ToArray();

			if (cartItems.Length == 0) return resultSetPromotionList;

			foreach (var setting in settings)
			{
				if(currentCartSetPromotionList.IsShippingChargeFree && setting.IsDiscountTypeShippingChargeFree) continue;
				if(currentCartSetPromotionList.IsPaymentChargeFree && setting.IsDiscountTypePaymentChargeFree) continue;

				var isApplyOnce = (setting.IsDiscountTypeProductDiscount == false);

				var necessaryItems = GetNecessaryToApplySetPromotionItems(setting, cartItems);
				if (necessaryItems.Length != setting.Items.Length) continue;

				var combinations = CreateCombinations(cart, necessaryItems, setting);
				resultSetPromotionList.AddSetPromotions(combinations);

				if (isApplyOnce && (resultSetPromotionList.Items.Count != 0)) break;
			}

			return resultSetPromotionList;
		}

		///<summary>
		/// セット成立にマストな条件を取得
		/// </summary>
		/// <param name="setting">セットプロモーション設定</param>
		/// <param name="cartSetPromotionItems">セットプロモーションアイテム</param>
		/// <returns>セット構築に必要な情報</returns>
		private static MatchItem[] GetNecessaryToApplySetPromotionItems(
			SetPromotionModel setting,
			CartSetPromotion.Item[] cartSetPromotionItems)
		{
			var setPromotionItems = cartSetPromotionItems.Select(
				wi =>
				{
					var item = new CartSetPromotion.Item(wi.Product, wi.Quantity);
					item.Allocate(wi.AllocatedQuantity);
					return item;
				}).ToArray();

			var necessaryItems = new List<MatchItem>();
			foreach (var settingItem in setting.Items)
			{
				// カート商品の中からこの設定に該当する商品をピックアップ
				var targetItems = GetSetPromotionItemFromCart(settingItem, cartSetPromotionItems);

				if (targetItems == null) break;

				var workItemList = GetSetPromotionItemFromCart(settingItem, setPromotionItems);

				// 該当商品があれば、その商品とセット成立に必要な数量を保持
				necessaryItems.Add(new MatchItem(settingItem, targetItems, workItemList));
			}

			return necessaryItems.ToArray();
		}

		/// <summary>
		/// カート商品の中からセットプロモーション対象商品を抽出
		/// </summary>
		/// <param name="itemSetting">セットプロモーション対象商品設定</param>
		/// <param name="cartSetPormotionItems">対象商品</param>
		/// <returns>セットプロモーション対象商品</returns>
		private static List<CartSetPromotion.Item> GetSetPromotionItemFromCart(
			SetPromotionItemModel itemSetting,
			CartSetPromotion.Item[] cartSetPormotionItems)
		{
			var resultItems = cartSetPormotionItems
				.Where(items => HasTargetProduct(itemSetting, items))
				.ToList();

			// セットに必要な商品の数量がなければ抜ける
			if (resultItems.Count == 0) return null;
			if (itemSetting.SetpromotionItemQuantity > resultItems.Sum(item => item.Product.CountSingle)) return null;

			return resultItems;
		}

		/// <summary>
		/// セット適用した組み合わせ作成
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="necessaryItems">セット作成にマストな条件</param>
		/// <param name="setting">セットプロモーション設定</param>
		/// <returns>セット適用した組み合わせ作成</returns>
		private static List<CartSetPromotion> CreateCombinations(
			CartObject cart,
			MatchItem[] necessaryItems,
			SetPromotionModel setting)
		{
			var applicableSetPromotions = new List<CartSetPromotion>();
			var itemCondition = false;

			// 同じカート内で複数適用できる可能性があるのでループ
			while (true)
			{
				var setPromotionItems = new List<CartSetPromotion.Item>();

				// セットプロモーションアイテム設定に関してループ
				foreach (var necessaryItem in necessaryItems)
				{
					var itemQuantity = necessaryItem.MatchItemSetting.SetpromotionItemQuantity;

					// 作業用アイテムに引き当てる商品,数量を割り当てる
					foreach (var item in necessaryItem.MatchItemsWork)
					{
						if (item.UnallocatedQuantity > 0)
						{
							// 引当可能な数量分引き当てる
							var allocateQuantity =
								((setting.IsDiscountTypeProductDiscount == false) || necessaryItem.MatchItemSetting.IsOverQuantityFlg)
									? item.UnallocatedQuantity : (itemQuantity < item.UnallocatedQuantity)
										? itemQuantity : item.UnallocatedQuantity;
							item.Allocate(allocateQuantity);

							var cartSetPromotionItem = new CartSetPromotion.Item(item.Product, allocateQuantity);
							cartSetPromotionItem.Allocate(allocateQuantity);
							setPromotionItems.Add(cartSetPromotionItem);

							itemQuantity -= allocateQuantity;
						}

						// 該当商品の引当可能な数量全てに引きあたったら抜ける
						if ((itemQuantity <= 0) && setting.IsDiscountTypeProductDiscount
							&& (necessaryItem.MatchItemSetting.IsOverQuantityFlg == false)) break;
					}

					// 引当条件を満たさなかったら抜ける
					if (itemQuantity > 0)
					{
						itemCondition = true;
						break;
					}
				}
				if (itemCondition) break;

				// 確定したセットプロモーションを結果に格納
				CartSetPromotion target = null;
				foreach (var applicableSetPromotion in applicableSetPromotions)
				{
					if (applicableSetPromotion.tempItems.Count != setPromotionItems.Count) continue;

					var sortedApplicableSetPromotionItems = applicableSetPromotion.tempItems
						.OrderBy(i => i.Product.VariationId).ThenBy(i => i.Quantity).ToArray();
					var sortedSetPromotionItems = setPromotionItems.OrderBy(i => i.Product.VariationId)
						.ThenBy(i => i.Quantity).ToArray();

					if (sortedApplicableSetPromotionItems.Where(
							(t, i) => (t.Product == sortedSetPromotionItems[i].Product)
								&& (t.Quantity == sortedSetPromotionItems[i].Quantity)).Any())
					{
						target = applicableSetPromotion;
						target.SetCount++;
					}
					if (target != null) break;
				}


				var cartSetPromotion = new CartSetPromotion(cart, setting, setPromotionItems)
				{
					SetCount = 1
				};
				if ((cartSetPromotion.ProductDiscountAmount > 0)
					|| (cartSetPromotion.ShippingChargeFreeFlg == Constants.FLG_ORDERSETPROMOTION_SHIPPING_CHARGE_FREE_FLG_ON)
					|| (cartSetPromotion.PaymentChargeFreeFlg == Constants.FLG_ORDERSETPROMOTION_PAYMENT_CHARGE_FREE_FLG_ON))
				{
					// 作業用で確定したものを正式に引き当てる
					foreach (var necessaryItem in necessaryItems)
					{
						for (var i = 0; i < necessaryItem.MatchItems.Count; i++)
						{
							var allocateQuantity = necessaryItem.MatchItemsWork[i].AllocatedQuantity
								- necessaryItem.MatchItems[i].AllocatedQuantity;
							necessaryItem.MatchItems[i].Allocate(allocateQuantity);
						}
					}

					if (target == null)
					{
						applicableSetPromotions.Add(cartSetPromotion);
					}
				}
			}
			return applicableSetPromotions;
		}

		/// <summary>
		/// カート商品がセットプロモーション対象商品に含まれるか
		/// </summary>
		/// <param name="itemSetting">セットプロモーション対象商品設定</param>
		/// <param name="item">対象商品</param>
		/// <returns>対象商品に含まれるか</returns>
		private static bool HasTargetProduct(SetPromotionItemModel itemSetting, CartSetPromotion.Item item)
		{
			switch (itemSetting.SetpromotionItemKbn)
			{
				// 商品ID指定
				case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_PRODUCT:
					var hasTargetProductByProductId = itemSetting.ItemsList.Contains(item.Product.ProductId);
					return hasTargetProductByProductId;

				// バリエーションID指定
				case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION:
					var hasTargetProductByVariationId = itemSetting.ItemsList.Any(
						listItem =>
						(listItem.Split(',')[0] == item.Product.ProductId) && (listItem.Split(',')[1] == item.Product.VariationId));
					return hasTargetProductByVariationId;

				// カテゴリID指定
				case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_CATEGORY:
					var hasTargetProductByCategoryId = ((itemSetting.ItemsList.Contains(item.Product.CategoryId1))
						|| (itemSetting.ItemsList.Contains(item.Product.CategoryId2))
						|| (itemSetting.ItemsList.Contains(item.Product.CategoryId3))
						|| (itemSetting.ItemsList.Contains(item.Product.CategoryId4))
						|| (itemSetting.ItemsList.Contains(item.Product.CategoryId5)));
					return hasTargetProductByCategoryId;

				// 全商品
				case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_ALL:
					return true;

				default:
					return false;
			}
		}

		#region 単体で成立するセットプロモーションを取得
		/// <summary>
		/// 単体で成立するセットプロモーションを取得
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="targetCartItems">対象カート商品</param>
		/// <param name="setPromotionSettings">セットプロモーション設定</param>
		/// <returns>単体で成立するセットプロモーション</returns>
		private static List<CartSetPromotion> GetApplicableSetPromotions(
			CartObject cart,
			List<CartProduct> targetCartItems,
			SetPromotionModel[] setPromotionSettings)
		{
			var applicableSetPromotions = new List<CartSetPromotion>();
			foreach (var model in setPromotionSettings)
			{
				var isApplicable = true;
				var setPromotionItems = new List<Hashtable>();

				foreach (var setPromotionItemSetting in model.Items)
				{
					// カート商品の中からこの設定に該当する商品をピックアップ
					var targetItems = GetSetPromotionItemFromCart(setPromotionItemSetting, targetCartItems);

					if (targetItems != null)
					{
						// 該当商品があれば、その商品とセット成立に必要な数量を保持
						var ht = new Hashtable
						{
							{ CONST_CART_PRODUCTS, targetItems },
							{ CONST_SETTING_QUANTITY, setPromotionItemSetting.SetpromotionItemQuantity },
							{ CONST_QUANTITY_MORE_FLG_ON, setPromotionItemSetting.IsOverQuantityFlg },
						};
						setPromotionItems.Add(ht);
					}
					else
					{
						// 該当商品がなければセット不成立。処理を抜ける
						isApplicable = false;
						break;
					}
				}

				if (isApplicable)
				{
					var unallocatedProducts = new List<CartSetPromotion.Item>();
					targetCartItems.ForEach(cp => unallocatedProducts.Add(new CartSetPromotion.Item(cp, cp.CountSingle)));

					// 該当商品が全部あったら、成立する組み合わせをすべて取得
					foreach (List<CartSetPromotion.Item> items in GetSetPromotionItemCombinations(setPromotionItems))
					{
						unallocatedProducts.ForEach(item => item.ResetAllocation());

						// 引当
						foreach (CartSetPromotion.Item setItem in items)
						{
							unallocatedProducts.Single(product => product.Product == setItem.Product).Allocate(setItem.Quantity);
						}

						if (unallocatedProducts.Count(product => product.HasRequiredQuantity == false) == 0)
						{
							applicableSetPromotions.Add(new CartSetPromotion(cart, model, items));
						}
					}
				}
			}

			return applicableSetPromotions;
		}

		/// <summary>
		/// カート商品の中からセットプロモーション対象商品を抽出
		/// </summary>
		/// <param name="itemModel">セットプロモーション対象商品(設定情報)</param>
		/// <param name="cartProducts">カート商品</param>
		/// <returns>セットプロモーション対象商品</returns>
		public static List<CartProduct> GetSetPromotionItemFromCart(SetPromotionItemModel itemModel, List<CartProduct> cartProducts)
		{
			List<CartProduct> targetProducts = new List<CartProduct>();
			switch (itemModel.SetpromotionItemKbn)
			{
				// 商品ID指定
				case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_PRODUCT:
					targetProducts = cartProducts.FindAll(cp =>
						((cp.IsSetItem == false)
						&& (cp.IsBundle == false)
						&& (itemModel.ItemsList.Contains(cp.ProductId))));
					break;

				// バリエーションID指定
				case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_VARIATION:
					targetProducts = cartProducts.FindAll(cp =>
						((cp.IsSetItem == false)
						&& (cp.IsBundle == false)
						&& (itemModel.ItemsList.Any(item => ((item.Split(',')[0] == cp.ProductId) && (item.Split(',')[1] == cp.VariationId))))));
					break;

				// カテゴリID指定
				case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_CATEGORY:
					targetProducts = cartProducts.FindAll(cp =>
						((cp.IsSetItem == false)
						&& (cp.IsBundle == false)
						&& (itemModel.ItemsList.Any(item =>
							(cp.CategoryId1.StartsWith(item)
							|| cp.CategoryId2.StartsWith(item)
							|| cp.CategoryId3.StartsWith(item)
							|| cp.CategoryId4.StartsWith(item)
							|| cp.CategoryId5.StartsWith(item))))));
					break;

				// 全商品
				case Constants.FLG_SETPROMOTIONITEM_SETPROMOTION_ITEM_KBN_ALL:
					targetProducts = cartProducts.FindAll(cp =>
						(cp.IsSetItem == false)
						&& (cp.IsBundle == false));
					break;

				default:
					break;
			}

			if ((targetProducts.Count != 0) && (itemModel.SetpromotionItemQuantity <= targetProducts.Sum(cp => cp.CountSingle)))
			{
				// 対象商品の数量がセットに必要な数量以上あればその商品を返す
				return targetProducts;
			}
			else
			{
				// 足りなければ null を返す
				return null;
			}
		}

		/// <summary>
		/// 対象のセットプロモーションが成立する、商品の全組み合わせを取得する
		/// </summary>
		/// <param name="setPromotionItems">セットプロモーション対象商品（セットプロモーションアイテムごとに、対象商品とセットに必要な数量を格納したリスト）</param>
		/// <returns>対象のセットプロモーションが成立する組み合わせ</returns>
		public static List<List<CartSetPromotion.Item>> GetSetPromotionItemCombinations(List<Hashtable> setPromotionItems)
		{
			// 各商品群から設定数分選択する組み合わせを取得
			var itemCombinations = new List<List<List<CartSetPromotion.Item>>>();
			foreach (var targetItems in setPromotionItems)
			{
				var tempItemCombinations = GetItemCombinations(targetItems);
				itemCombinations.Add(tempItemCombinations);
			}

			// セットプロモーションが成立する商品の組み合わせを返す
			// (各セットプロモーションアイテムの組み合わせから1つずつ選択した組み合わせを取得)
			return GetAllItemCombinations(itemCombinations);
		}

		/// <summary>
		/// 対象商品群から必要数量選択する組み合わせをすべて取得
		/// </summary>
		/// <param name="setPromotionItemAndSetting">対象商品と必要数量</param>
		/// <returns>対象商品群から必要数量選択する組み合わせ</returns>
		/// <remarks>
		/// 例：左のHashtableの情報から右のリストを作る
		///
		///  対象商品と数量           商品A x2、商品B ×1、商品C x1 から 2個選ぶ組み合わせ
		///   商品リスト----2個          +--+[商品A x2]
		///     +[商品A x2]             +--+[商品A x1]
		///     +[商品B x1]       =>    |  +[商品B x1]
		///     +[商品C x1]             +--+[商品A x1]
		///                             |  +[商品C x1]
		///                             +--+[商品B x1]
		///                                +[商品C x1]
		/// </remarks>
		private static List<List<CartSetPromotion.Item>> GetItemCombinations(Hashtable setPromotionItemAndSetting)
		{
			var resultList = new List<List<CartSetPromotion.Item>>();
			var stack = new Stack<CartSetPromotion.Item>();
			var cartProducts = (List<CartProduct>)setPromotionItemAndSetting[CONST_CART_PRODUCTS];
			var isQuantityMoreFlgOn = (bool)setPromotionItemAndSetting[CONST_QUANTITY_MORE_FLG_ON];
			var settingQuantity = (int)setPromotionItemAndSetting[CONST_SETTING_QUANTITY];
			if (isQuantityMoreFlgOn)
			{
				return GetItemCombinationsForSettingOverQuantityFlagOn(
					cartProducts,
					settingQuantity);
			}

			return GetItemCombinations(
				cartProducts,
				0,
				settingQuantity,
				stack,
				resultList);
		}
		/// <summary>
		/// 対象商品群から必要数量選択する組み合わせを取得(再帰処理)
		/// </summary>
		/// <param name="targetCartProducts">セットプロモーション対象商品</param>
		/// <param name="targetIndex">インデックス</param>
		/// <param name="quantity">セット成立に必要な残り数量</param>
		/// <param name="stack">対象商品格納用stack</param>
		/// <param name="resultList">結果リスト</param>
		/// <returns>対象商品群から必要数量選択する組み合わせ</returns>
		private static List<List<CartSetPromotion.Item>> GetItemCombinations(
			List<CartProduct> targetCartProducts,
			int targetIndex,
			int quantity,
			Stack<CartSetPromotion.Item> stack,
			List<List<CartSetPromotion.Item>> resultList)
		{
			// 必要な数量が0になったら結果リストに追加して終了
			if (quantity == 0)
			{
				resultList.Add(stack.ToList());
				return resultList;
			}

			// ターゲットインデックスが最後まで行ったら終了
			if (targetIndex == targetCartProducts.Count)
			{
				return resultList;
			}

			var cartProduct = targetCartProducts[targetIndex];
			for (int i = quantity; 0 < i; i--)
			{
				if (cartProduct.CountSingle >= i)
				{
					// 必要数分あれば、その商品と数量をstackに格納
					var tempCartSetPromotionItem = new CartSetPromotion.Item(cartProduct, i);
					stack.Push(tempCartSetPromotionItem);

					// 再帰呼び出し（抽出したい数量は減らして次の商品へ）
					GetItemCombinations(
						targetCartProducts,
						(targetIndex + 1),
						quantity - i,
						stack,
						resultList);

					stack.Pop();
				}
			}

			// 再帰呼び出し（現在の商品は選ばないパターン。数量は変えずに次の商品へ）
			return GetItemCombinations(
				targetCartProducts,
				(targetIndex + 1),
				quantity,
				stack,
				resultList);
		}

		/// <summary>
		/// 各セットプロモーションアイテムの組み合わせから1つずつ選択する組み合わせをすべて取得
		/// </summary>
		/// <param name="itemCombinations">各セットプロモーションアイテムの組み合わせ</param>
		/// <returns>各セットプロモーションアイテムの組み合わせから1つずつ選択する組み合わせ</returns>
		/// <remarks>
		/// 例：左のリストから右のリストを作る
		///  +--+-1:[商品A x2]                 1+5:[商品A x2 + 商品D x1]
		///  |  +-2:[商品A x1 + 商品B x1]      1+6:[商品A x2 + 商品E x1]
		///  |  +-3:[商品A x1 + 商品C x1]      2+5:[商品A x1 + 商品B x1 + 商品D x1]
		///  |  +-4:[商品B x1 + 商品C x1]  =>  2+6:[商品A x1 + 商品B x1 + 商品E x1]
		///  |                                 3+5:[商品A x1 + 商品C x1 + 商品D x1]
		///  +--+-5:[商品D x1]                 3+6:[商品A x1 + 商品C x1 + 商品E x1]
		///     +-6:[商品E x1]                 4+5:[商品B x1 + 商品C x1 + 商品D x1]
		///                                    4+6:[商品B x1 + 商品C x1 + 商品E x1]
		/// </remarks>
		private static List<List<CartSetPromotion.Item>> GetAllItemCombinations(List<List<List<CartSetPromotion.Item>>> itemCombinations)
		{
			List<List<CartSetPromotion.Item>> result = new List<List<CartSetPromotion.Item>>();
			Stack<List<CartSetPromotion.Item>> stack = new Stack<List<CartSetPromotion.Item>>();

			return GetAllItemCombinations(itemCombinations, stack, result);
		}
		/// <summary>
		/// 各セットプロモーションアイテムの組み合わせから1つずつ選択する組み合わせを取得(再帰処理)
		/// </summary>
		/// <param name="itemCombinations">各セットプロモーションアイテムの組み合わせ</param>
		/// <param name="stack">選択したセットプロモーションアイテムの組み合わせ格納用stack</param>
		/// <param name="result">結果リスト</param>
		/// <returns>各セットプロモーションアイテムの組み合わせから1つずつ選択する組み合わせ</returns>
		private static List<List<CartSetPromotion.Item>> GetAllItemCombinations(List<List<List<CartSetPromotion.Item>>> itemCombinations, Stack<List<CartSetPromotion.Item>> stack, List<List<CartSetPromotion.Item>> result)
		{
			// 各アイテムの組み合わせから1つずつ取り終わったら結果リストに情報格納して終了
			if (stack.Count == itemCombinations.Count)
			{
				// 1つのリストにする（同一商品があれば数量合算）
				List<CartSetPromotion.Item> items = GetDistinctItems(stack.ToList());

				// 全商品セットに必要な数量を満たしていればリストに追加
				if (items.Exists(i => i.Quantity > i.Product.CountSingle) == false)
				{
					result.Add(items);
				}

				return result;
			}

			// 再帰処理
			foreach (List<CartSetPromotion.Item> itemCombination in itemCombinations[stack.Count])
			{
				stack.Push(itemCombination);
				GetAllItemCombinations(itemCombinations, stack, result);
				stack.Pop();
			}

			return result;
		}

		/// <summary>
		/// セットプロモーションアイテムリスト取得
		/// </summary>
		/// <param name="items">対象商品とセットに必要な数量</param>
		/// <returns>セットプロモーションアイテムリスト</returns>
		private static List<CartSetPromotion.Item> GetDistinctItems(List<List<CartSetPromotion.Item>> items)
		{
			// 同一商品があればまとめる
			List<CartSetPromotion.Item> tempItems = new List<CartSetPromotion.Item>();
			foreach (List<CartSetPromotion.Item> cartSetPromotionItemList in items)
			{
				foreach (CartSetPromotion.Item cartSetPromotionItem in cartSetPromotionItemList)
				{
					CartSetPromotion.Item sameItem = tempItems.Find(item => item.Product == cartSetPromotionItem.Product);

					if (sameItem != null)
					{
						// 同一商品があれば合算
						tempItems.Add(new CartSetPromotion.Item(sameItem.Product, sameItem.Quantity + cartSetPromotionItem.Quantity));
						tempItems.Remove(sameItem);
					}
					else
					{
						// なければ追加
						tempItems.Add(cartSetPromotionItem);
					}
				}
			}

			return tempItems;
		}

		/// <summary>
		/// 対象商品群から必要数量選択する組み合わせを取得 (数量以上フラグが有効か)
		/// </summary>
		/// <param name="targetCartProducts">セットプロモーション対象商品</param>
		/// <param name="quantity">セット成立に必要な残り数量</param>
		/// <returns>対象商品群から必要数量選択する組み合わせ</returns>
		private static List<List<CartSetPromotion.Item>> GetItemCombinationsForSettingOverQuantityFlagOn(
			List<CartProduct> targetCartProducts,
			int quantity)
		{
			// Create work item combinations process
			var workItemCombinations = new List<List<CartSetPromotion.Item>>();
			foreach (var cartProduct in targetCartProducts)
			{
				var itemCombinations = GetAllItemCombinationsFromCartProduct(cartProduct);
				workItemCombinations.Add(itemCombinations);
			}

			// Create pattern item combinations process
			var stack = new Stack<CartSetPromotion.Item>();
			var resultList = new List<List<CartSetPromotion.Item>>();
			return GetItemCombinationsFromWorkItemCombinations(
				0,
				quantity,
				workItemCombinations,
				resultList,
				stack);
		}

		/// <summary>
		/// Get item combinations from work item combinations (recursive processing)
		/// </summary>
		/// <param name="targetIndex">The target index of item in the work item combinations</param>
		/// <param name="quantity">The required quantity</param>
		/// <param name="workItemCombinations">Work item combinations</param>
		/// <param name="resultItemCombinations">Result item combinations</param>
		/// <param name="stack">The stack of current item combinations</param>
		/// <returns>対象商品群から必要数量選択する組み合わせ</returns>
		private static List<List<CartSetPromotion.Item>> GetItemCombinationsFromWorkItemCombinations(
			int targetIndex,
			int quantity,
			List<List<CartSetPromotion.Item>> workItemCombinations,
			List<List<CartSetPromotion.Item>> resultItemCombinations,
			Stack<CartSetPromotion.Item> stack)
		{
			// When the target index is the last index of item in the work item combinations, add it to the result list and exit
			if (workItemCombinations.Count == targetIndex)
			{
				// Get item combinations and check valid quantity
				var tempItemCombinations = stack.ToList();
				if (tempItemCombinations.Sum(item => item.Quantity) >= quantity)
				{
					resultItemCombinations.Add(tempItemCombinations);
				}
				return resultItemCombinations;
			}

			foreach (var item in workItemCombinations[targetIndex])
			{
				// Store the item in stack if necessary
				stack.Push(item);

				// Recursive call (decrease the index you want to extract and go to the next item)
				GetItemCombinationsFromWorkItemCombinations(
					targetIndex + 1,
					quantity,
					workItemCombinations,
					resultItemCombinations,
					stack);

				stack.Pop();
			}

			// Recursive call (pattern that does not select the current work item combinations. To the next item in the work item combinations)
			return GetItemCombinationsFromWorkItemCombinations(
				targetIndex + 1,
				quantity,
				workItemCombinations,
				resultItemCombinations,
				stack);
		}

		/// <summary>
		/// Get all item combinations from cart product
		/// </summary>
		/// <param name="cartProduct">Cart product</param>
		/// <returns>All item combinations from cart product</returns>
		private static List<CartSetPromotion.Item> GetAllItemCombinationsFromCartProduct(CartProduct cartProduct)
		{
			var resultList = new List<CartSetPromotion.Item>();
			for (var quantity = 1; quantity <= cartProduct.CountSingle; quantity++)
			{
				var tempCartSetPromotionItem = new CartSetPromotion.Item(cartProduct, quantity);
				resultList.Add(tempCartSetPromotionItem);
			}
			return resultList;
		}
		#endregion

		/// <summary>
		/// 最安となるセットプロモーションの組み合わせを取得(動的計画法)
		/// </summary>
		/// <param name="cartItems">カート商品</param>
		/// <param name="applicableCartSetPromotion">セットプロモーションの適用パターン</param>
		/// <param name="cart">カート（管理画面からの注文編集時の制御用）</param>
		/// <returns>適用するセットプロモーションリスト</returns>
		/// <remarks>商品の割り当て状態をキーとし、動的計画法により解を求める</remarks>
		private static CartSetPromotionList GetLowestSetPromotionsCombination(
			List<CartProduct> cartItems,
			List<CartSetPromotion> applicableCartSetPromotion,
			CartObject cart = null)
		{
			// セットプロモーション組み合わせテーブルの宣言、初期化
			var setPromotionCombinationTable = new Dictionary<ItemAllocations, CartSetPromotionList>(new ItemListComparer());
			setPromotionCombinationTable.Add(new ItemAllocations(cartItems), new CartSetPromotionList());
			int numberOfPatterns = 0;

			// 縦軸(セットプロモーション)
			// 順番：割引金額が大きい（商品割引、配送料引き、決済手数料引きを含む）→商品のバリエーションID
			var orderByDesc = applicableCartSetPromotion
				.OrderByDescending(cartSetPromotion =>
					(cartSetPromotion.UnitProductDiscountAmount
						+ (cartSetPromotion.IsDiscountTypeShippingChargeFree ? cartSetPromotion.Cart.PriceShipping : 0)
						+ ((cartSetPromotion.IsDiscountTypePaymentChargeFree && cartSetPromotion.Cart.Payment != null) ? cartSetPromotion.Cart.Payment.PriceExchange : 0)
					) / cartSetPromotion.tempItems.Sum(item => item.Quantity))
				.ThenByDescending(cartSetPromotion => cartSetPromotion.tempItems.Select(item => item.Product.VariationId).Aggregate((i, j) => i + j))
				.ToArray();

			foreach (var cartSetPromotion in orderByDesc)
			{
				if (numberOfPatterns > Constants.SETPROMOTION_MAXIMUM_NUMBER_OF_COMBINATIONS) break;

				var setPromotionCombinationTableCopy = new Dictionary<ItemAllocations, CartSetPromotionList>(setPromotionCombinationTable);
				// 横軸(割当済商品数) シャローコピーで列挙
				foreach (ItemAllocations items in setPromotionCombinationTableCopy.Keys)
				{
					if (numberOfPatterns > Constants.SETPROMOTION_MAXIMUM_NUMBER_OF_COMBINATIONS) break;

					int numberOfSets = 0;
					ItemAllocations newItems = items;
					// セット個数ループ
					while (newItems.HasRequiredQuantity)
					{
						newItems = newItems.Allocate(cartSetPromotion);
						numberOfSets++;
						numberOfPatterns++;
						if (newItems.HasRequiredQuantity)
						{
							CartSetPromotionList currentSetPromotion = setPromotionCombinationTableCopy[items].Clone();
							CartSetPromotion cartSetPromotionCopy = cartSetPromotion.Clone();
							cartSetPromotionCopy.SetCount = numberOfSets;

							currentSetPromotion.AddSetPromotion(cartSetPromotionCopy);

							if (setPromotionCombinationTable.ContainsKey(newItems))
							{
								// 比較元のほうが優先度が高ければ更新
								if (IsPriorityHigh(currentSetPromotion, setPromotionCombinationTable[newItems], cartItems))
								{
									setPromotionCombinationTable[newItems] = currentSetPromotion;
								}
							}
							else
							{
								setPromotionCombinationTable.Add(newItems, currentSetPromotion);
							}
						}
					}
				}
			}

			// 割引金額が最大のものを取得
			var totalDiscountAmountMax = setPromotionCombinationTable.Max(combination => combination.Value.TotalDiscountAmount);

			// 割引金額がゼロなら処理を抜ける
			if (totalDiscountAmountMax == 0)
			{
				if (cart == null) return null;

				var setPromotion = setPromotionCombinationTable
					.First(item =>
						((item.Value.TotalDiscountAmount == setPromotionCombinationTable.Max(combination => combination.Value.TotalDiscountAmount))
							&& (item.Value.Items.Count > 0)))
					.Value.Items[0];

				if ((setPromotion.IsDiscountTypeShippingChargeFree && (cart.EnteredShippingPrice == 0))
					|| (setPromotion.IsDiscountTypePaymentChargeFree
						&& ((setPromotion.Cart.Payment != null) && (cart.EnteredPaymentPrice == 0))))
				{
					return null;
				}
			}

			var result = setPromotionCombinationTable
				.Where(combination => combination.Value.TotalDiscountAmount == totalDiscountAmountMax)
				.Select(combination => combination.Value)
				.ToArray();

			// セットプロモーション割当商品数が最大のものを取得
			if (result.Length > 1)
			{
				var summaryArray = result
					.Select(cartSetPromotions => new
					{
						cartSetPromotions = cartSetPromotions,
						sumValue = cartSetPromotions.Items
							.Sum(cartSetPromotion => cartSetPromotion.tempItems.Sum(item => item.Quantity) * cartSetPromotion.SetCount)
					})
					.ToArray();
				int maximumNumberOfAllocatedProducts = summaryArray.Max(x => x.sumValue);
				result = summaryArray
					.Where(summary => summary.sumValue == maximumNumberOfAllocatedProducts)
					.Select(x => x.cartSetPromotions)
					.ToArray();
			}
			// 適用するセットプロモーションの種類数が最小のものを取得
			if (result.Length > 1)
			{
				var minimumArray = result
					.Select(cartSetPromotions => new
					{
						cartSetPromotions = cartSetPromotions,
						minimumValue = cartSetPromotions.Items.Select(setPromotion => setPromotion.SetpromotionId).Distinct().Count()
					})
					.ToArray();

				int minimumSetPromotionCount = minimumArray.Min(x => x.minimumValue);
				result = minimumArray
					.Where(minimum => minimum.minimumValue == minimumSetPromotionCount)
					.Select(x => x.cartSetPromotions)
					.ToArray();
			}
			// セットプロモーションが適用される商品パターン数が最小のものを取得
			if (result.Length > 1)
			{
				var minimumPatternArray = result
					.Select(cartSetPromotions => new
					{
						cartSetPromotions = cartSetPromotions,
						minimumCount = cartSetPromotions.Items.Sum(cartSetPromotion => cartSetPromotion.tempItems.Count)
					})
					.ToArray();
				int minimumNumberOfItemPatterns = minimumPatternArray.Min(x => x.minimumCount);
				result = minimumPatternArray
					.Where(minimumPattern => minimumPattern.minimumCount == minimumNumberOfItemPatterns)
					.Select(x => x.cartSetPromotions)
					.ToArray();
			}
			return result.First();
		}

		/// <summary>
		/// 比較元がプライオリティが高いか判定
		/// </summary>
		/// <param name="src">比較元</param>
		/// <param name="dst">比較先</param>
		/// <param name="cartItems">カートアイテム</param>
		/// <returns>プライオリティが高いか</returns>
		private static bool IsPriorityHigh(CartSetPromotionList src, CartSetPromotionList dst, List<CartProduct> cartItems)
		{
			// 割引額が違う場合は高い方を優先
			if (src.TotalDiscountAmount != dst.TotalDiscountAmount) return (src.TotalDiscountAmount > dst.TotalDiscountAmount);

			// 明細数が異なる場合少ない方を優先
			var srcItemCount = src.Items.Sum(i => i.tempItems.Count);
			var dstItemCount = dst.Items.Sum(i => i.tempItems.Count);
			if (srcItemCount != dstItemCount) return (srcItemCount < dstItemCount);

			// 先にカート投入された方がよりトータル多く割り当てられているものを優先したい
			foreach (var item in cartItems)
			{
				// 割当数合計で判定
				var srcCountTotal = src.Items.SelectMany(i => i.tempItems.Where(i2 => i2.Product == item).Select(i2 => i2.Quantity)).Sum();
				var dstCountTotal = dst.Items.SelectMany(i => i.tempItems.Where(i2 => i2.Product == item).Select(i2 => i2.Quantity)).Sum();
				if (srcItemCount > dstItemCount)
				{
					return true;
				}

				// 割当数MAXで判定
				var srcCountMax = (srcCountTotal == 0) ? 0 : src.Items.SelectMany(i => i.tempItems.Where(i2 => i2.Product == item).Select(i2 => i2.Quantity)).Max();
				var dstCountMax = (dstCountTotal == 0) ? 0 : dst.Items.SelectMany(i => i.tempItems.Where(i2 => i2.Product == item).Select(i2 => i2.Quantity)).Max();
				if (srcCountMax > dstCountMax)
				{
					return true;
				}

				// より高い割引額の方に割り当てられているかで判定
				var srcDiscPriceRate = src.Items.SelectMany(i => i.tempItems.Where(i2 => i2.Product == item).Select(i2 => i2.Quantity * i.ProductDiscountAmount)).Sum();
				var dstDiscPriceRate = dst.Items.SelectMany(i => i.tempItems.Where(i2 => i2.Product == item).Select(i2 => i2.Quantity * i.ProductDiscountAmount)).Sum();
				if (srcDiscPriceRate > dstDiscPriceRate)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>
		/// 按分した割引額を取得
		/// </summary>
		/// <param name="setPromotion">カートセットプロモーション情報</param>
		/// <param name="targetItem">対象商品</param>
		/// <param name="itemQuantity">数量</param>
		/// <param name="isDutyFree">免税フラグ</param>
		/// <returns>按分した割引額</returns>
		public static decimal GetDistributedDiscountAmount(CartSetPromotion setPromotion, CartProduct targetItem, int itemQuantity, bool isDutyFree)
		{
			// 対象商品の小計
			decimal targetProductPrice = (isDutyFree
				? targetItem.Price
				: targetItem.PricePretax) * itemQuantity;

			var discountPrice = 0m;
			switch (setPromotion.ProductDiscountKbn)
			{
				case Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNTED_PRICE:
				case Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_PRICE:
					if (setPromotion.UndiscountedProductSubtotal == 0) return 0;

					// 計算方法：割引額 * 商品小計 / 商品合計　※端数切捨て
					discountPrice = (setPromotion.ProductDiscountAmount * targetProductPrice / setPromotion.UndiscountedProductSubtotal).ToPriceDecimal(DecimalUtility.Format.RoundDown).Value;
					break;

				case Constants.FLG_SETPROMOTION_PRODUCT_DISCOUNT_KBN_DISCOUNT_RATE:
					// 計算方法：商品小計 * 割引率
					discountPrice = RoundingCalculationUtility.GetRoundPercentDiscountFraction(targetProductPrice, (decimal)setPromotion.ProductDiscountSetting);
					break;
			}
			return discountPrice;
		}
	}

	#region 商品引当状態コレクション
	/// <summary>
	/// 商品引当状態コレクション
	/// </summary>
	[Serializable]
	struct ItemAllocations
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cartProducts">カート商品</param>
		public ItemAllocations(List<CartProduct> cartProducts)
		{
			this.Allocations = new Dictionary<CartProduct, int>();
			foreach (CartProduct product in cartProducts)
			{
				this.Allocations.Add(product, 0);
			}
		}

		/// <summary>
		/// 引当
		/// </summary>
		/// <param name="cartSetPromotion">カートセットプロモーション</param>
		/// <returns>商品引当状態コレクション</returns>
		public ItemAllocations Allocate(CartSetPromotion cartSetPromotion)
		{
			Dictionary<CartProduct, int> newAllocatedProducts = new Dictionary<CartProduct, int>(this.Allocations);
			foreach (CartSetPromotion.Item item in cartSetPromotion.tempItems)
			{
				newAllocatedProducts[item.Product] += item.Quantity;
			}
			this.Allocations = newAllocatedProducts;
			return this;
		}

		/// <summary>引当状態(商品クラス,引当数)</summary>
		public Dictionary<CartProduct, int> Allocations;
		/// <summary>引当可能な数量があるか</summary>
		public bool HasRequiredQuantity { get { return this.Allocations.Count(item => item.Key.CountSingle < item.Value) == 0; } }
	}
	#endregion

	#region ItemAllocations比較クラス
	/// <summary>
	/// ItemAllocations比較クラス
	/// </summary>
	[Serializable]
	class ItemListComparer : IEqualityComparer<ItemAllocations>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ItemListComparer()
		{
		}

		/// <summary>
		/// 指定したオブジェクトが等しいかどうかを返す
		/// </summary>
		/// <param name="items1"></param>
		/// <param name="items2"></param>
		/// <returns>指定したオブジェクトが等しいかどうか</returns>
		public bool Equals(ItemAllocations items1, ItemAllocations items2)
		{
			return items1.Allocations.Values.SequenceEqual(items2.Allocations.Values);
		}

		/// <summary>
		/// ハッシュコードを取得
		/// </summary>
		/// <param name="items"></param>
		/// <returns></returns>
		public int GetHashCode(ItemAllocations items)
		{
			return string.Join(",", items.Allocations.Values).GetHashCode();
		}
	}
	#endregion

	#region マッチアイテムクラス
	/// <summary>
	/// マッチアイテムクラス
	/// </summary>
	public class MatchItem
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="matchSetting">マッチセットプロモーションアイテム設定</param>
		/// <param name="matchItems">マッチカートセットプロモーションアイテムリスト</param>
		/// <param name="matchItemsWork">マッチカートセットプロモーションアイテムリスト(作業用)</param>
		public MatchItem(
			SetPromotionItemModel matchSetting,
			List<CartSetPromotion.Item> matchItems,
			List<CartSetPromotion.Item> matchItemsWork)
		{
			this.MatchItemSetting = matchSetting;
			this.MatchItems = matchItems;
			this.MatchItemsWork = matchItemsWork;
		}

		/// <summary>マッチセットプロモーションアイテム設定</summary>
		public SetPromotionItemModel MatchItemSetting { get; private set; }

		/// <summary>マッチセットプロモーションアイテムリスト</summary>
		public List<CartSetPromotion.Item> MatchItems { get; private set; }

		/// <summary>マッチセットプロモーションアイテムリスト(作業用)</summary>
		public List<CartSetPromotion.Item> MatchItemsWork { get; set; }
	}
	#endregion
}
