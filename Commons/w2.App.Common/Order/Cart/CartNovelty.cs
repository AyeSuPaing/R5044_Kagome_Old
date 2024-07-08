/*
=========================================================================================================
  Module      : カートノベルティクラス(CartNovelty.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.App.Common.Global.Region;
using w2.App.Common.Global.Translation;
using w2.App.Common.Option;
using w2.Domain.Novelty;

namespace w2.App.Common.Order
{
	/// <summary>
	/// カートノベルティクラス
	/// </summary>
	[Serializable]
	public class CartNovelty : NoveltyModel
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cart">カート</param>
		/// <param name="model">ノベルティ設定モデル</param>
		public CartNovelty(CartObject cart, NoveltyModel model)
		{
			// 各プロパティにセット
			this.Cart = cart;
			this.DataSource = model.DataSource;

			// 付与アイテムリストをセット
			SetGrantItemList();
		}
		#endregion

		#region メソッド
		/// <summary>
		/// 付与アイテムリストをセット
		/// </summary>
		private void SetGrantItemList()
		{
			// 対象アイテムリストを対象アイテム枝番でグループ化
			decimal priceSubtotalAfterDistribution = 0;
			var checkedProductVariationIds = new List<string>();
			foreach (var targetItemList in this.TargetItemList.GroupBy(g => g.NoveltyTargetItemNo))
			{
				var checkTargetItem = false;
				foreach (CartProduct item in this.Cart)
				{
					// 対象アイテムに該当するか?
					var isTargetItem = IsTargetItem(item, targetItemList.ToArray());
					// 比較対象金額用にキャンペーン（ノベルティ、クーポンなど）用商品金額（按分処理後）を加算
					if (isTargetItem && (checkedProductVariationIds.Contains(item.VariationId) == false))
					{
						priceSubtotalAfterDistribution +=
							this.Cart.Items
								.Where(x => (x.VariationId == item.VariationId))
								.Sum(x => x.PriceSubtotalAfterDistributionForCampaign);
						checkedProductVariationIds.Add(item.VariationId);
					}

					checkTargetItem = (checkTargetItem || isTargetItem);
				}
				// 対象アイテムに該当する商品が1つも存在しない場合、処理を抜ける
				if (checkTargetItem == false) return;
			}
			this.ExistsTargetItem = true;

			// 条件に該当する付与条件取得
			var noveltyGranConditionsList = GetNoveltyGranConditions(priceSubtotalAfterDistribution);
			this.ExistsGrantConditions = (noveltyGranConditionsList.Length != 0);

			// 付与アイテムリストセット
			this.GrantItemList = GetCartNoveltyGrantItemList(noveltyGranConditionsList);
		}

		/// <summary>
		/// 対象アイテムに該当するか?
		/// </summary>
		/// <param name="cartProduct">カート商品</param>
		/// <param name="targetItemList">対象アイテムリスト</param>
		/// <returns>該当する：true、該当しない：false</returns>
		private bool IsTargetItem(CartProduct cartProduct, NoveltyTargetItemModel[] targetItemList)
		{
			// 全商品
			var matchAll = targetItemList.Any(targetItem =>
				(targetItem.NoveltyTargetItemType == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_ALL));
			// 全商品の場合、該当する（true）を返す
			if (matchAll) return true;

			// ブランドID
			var matchBrand = targetItemList.Where(targetItem =>
				(targetItem.NoveltyTargetItemType == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_BRAND)).ToArray();
			if (matchBrand.Length != 0)
			{
				// ブランドIDに該当しない場合、該当しない（false）を返す
				if (matchBrand.Any(targetItem =>
					(
						(targetItem.NoveltyTargetItemValue == cartProduct.BrandId)
						||
						(targetItem.NoveltyTargetItemValue == cartProduct.BrandId2)
						||
						(targetItem.NoveltyTargetItemValue == cartProduct.BrandId3)
						||
						(targetItem.NoveltyTargetItemValue == cartProduct.BrandId4)
						||
						(targetItem.NoveltyTargetItemValue == cartProduct.BrandId5))) == false) return false;
			}

			// カテゴリID
			var matchCategory = targetItemList.Where(targetItem =>
				(targetItem.NoveltyTargetItemType == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_CATEGORY)).ToArray();
			if (matchCategory.Length != 0)
			{
				// カテゴリIDに該当しない場合、該当しない（false）を返す
				if (matchCategory.Any(targetItem =>
					(
						IsTargetCategoryItem(targetItem.NoveltyTargetItemValue, cartProduct.CategoryId1)
						||
						IsTargetCategoryItem(targetItem.NoveltyTargetItemValue, cartProduct.CategoryId2)
						||
						IsTargetCategoryItem(targetItem.NoveltyTargetItemValue, cartProduct.CategoryId3)
						||
						IsTargetCategoryItem(targetItem.NoveltyTargetItemValue, cartProduct.CategoryId4)
						||
						IsTargetCategoryItem(targetItem.NoveltyTargetItemValue, cartProduct.CategoryId5)
					)) == false) return false;
			}

			// 商品ID
			var matchProduct = targetItemList.Where(targetItem =>
				(targetItem.NoveltyTargetItemType == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_PRODUCT)).ToArray();
			if (matchProduct.Length != 0)
			{
				// 商品IDに該当しない場合、該当しない（false）を返す
				if (matchProduct.Any(targetItem =>
					(
						(targetItem.ShopId == cartProduct.ShopId)
						&& (targetItem.NoveltyTargetItemValue == cartProduct.ProductId)
					)) == false) return false;
			}

			// バリエーションID
			var matchVariation = targetItemList.Where(targetItem =>
				(targetItem.NoveltyTargetItemType == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_VARIATION)).ToArray();
			if (matchVariation.Length != 0)
			{
				// バリエーションIDに該当しない場合、該当しない（false）を返す
				if (matchVariation.Any(targetItem =>
					(
						(targetItem.ShopId == cartProduct.ShopId)
						&& (targetItem.NoveltyTargetItemValue.Split(',')[0] == cartProduct.ProductId)
						&& (targetItem.NoveltyTargetItemValue.Split(',')[1] == cartProduct.VariationId)
					)) == false) return false;
			}

			return true;
		}

		/// <summary>
		/// 対象アイテムがカテゴリIDに該当するか？
		/// </summary>
		/// <param name="targetItemCategoryId">対象アイテムのカテゴリID</param>
		/// <param name="cartProductCategoryId">カート商品のカテゴリID</param>
		/// <returns>該当する：true、該当しない：false</returns>
		private bool IsTargetCategoryItem(string targetItemCategoryId, string cartProductCategoryId)
		{
			// カート商品にカテゴリID指定がない？
			if (string.IsNullOrEmpty(cartProductCategoryId)) return false;

			// カテゴリIDが同じ？
			if (targetItemCategoryId == cartProductCategoryId) return true;

			// カート商品のカテゴリIDが子カテゴリである？
			if (cartProductCategoryId.Length > targetItemCategoryId.Length)
			{
				return cartProductCategoryId.StartsWith(targetItemCategoryId);
			}

			return false;
		}

		/// <summary>
		/// 条件に該当する付与条件リスト取得
		/// </summary>
		/// <param name="priceSubtotalAfterDistribution">比較対象金額</param>
		/// <remarks>比較対象金額 = ( 商品合計( 商品価格（会員ランク価格 ＞ セール価格 ＞ 特別価格 ＞ 通常価格）* 数量 ) - セットプロモーション割引金額(按分) - 会員ランク割引金額(按分) ）</remarks>
		/// <returns>付与条件リスト</returns>
		private NoveltyGrantConditionsModel[] GetNoveltyGranConditions(decimal priceSubtotalAfterDistribution)
		{
			// ランクNo取得
			var memberRankNo = MemberRankOptionUtility.GetMemberRankNo(this.Cart.MemberRankId);

			// 適用対象ランクに該当する付与条件取得
			var result =
				this.GrantConditionsList
				.Where(grantConditions =>
				{
					switch (grantConditions.UserRankId)
					{
						// 全ユーザ
						case Constants.FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBERRANK_ALL:
							return true;
						// 会員のみ
						case Constants.FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBER_ONLY:
							return (string.IsNullOrEmpty(this.Cart.CartUserId) == false);
						// 会員ランク
						default:
							// 会員ランクOP有効ではない?
							if (Constants.MEMBER_RANK_OPTION_ENABLED == false) return false;
							// 会員ではない?
							if (string.IsNullOrEmpty(this.Cart.MemberRankId)) return false;
							// ランク順位以上?
							return (memberRankNo <= MemberRankOptionUtility.GetMemberRankNo(grantConditions.UserRankId));
					}
				});

			// 付与金額に該当する付与条件取得
			return result
				.Where(grantConditions => 
				{
					// 対象金額範囲内?
					var amountEnd = (grantConditions.AmountEnd != null) ? grantConditions.AmountEnd : decimal.MaxValue;
					return ((grantConditions.AmountBegin <= priceSubtotalAfterDistribution) && (priceSubtotalAfterDistribution <= amountEnd));
				}).ToArray();
		}

		/// <summary>
		/// 付与アイテムリスト取得
		/// </summary>
		/// <param name="noveltyGranConditionsList">付与条件リスト</param>
		/// <returns>付与アイテムリスト</returns>
		private CartNoveltyGrantItem[] GetCartNoveltyGrantItemList(NoveltyGrantConditionsModel[] noveltyGranConditionsList)
		{
			var memberAll = new List<CartNoveltyGrantItem>();
			var memberOnly = new List<CartNoveltyGrantItem>();
			var memberRank = new List<CartNoveltyGrantItem>();
			var maxPriorityMemberRankNo = 0;

			// 付与条件ループ
			foreach (var grantConditions in noveltyGranConditionsList)
			{
				// 付与アイテムループ
				var cartNoveltyGrantItemList = new List<CartNoveltyGrantItem>();
				foreach (var item in grantConditions.GrantItemList)
				{
					// 付与アイテムを追加
					var products = ProductCommon.GetProductInfo(item.ShopId,
						item.ProductId,
						this.Cart.MemberRankId,
						this.Cart.IsFixedPurchaseMember
							? Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_ON
							: Constants.FLG_USER_FIXED_PURCHASE_MEMBER_FLG_OFF);
					foreach (DataRowView product in products)
					{
						// 商品状態チェック
						if (OrderCommon.CheckProductStatus(product, 1, Constants.AddCartKbn.Normal, this.Cart.CartUserId) == OrderErrorcode.NoError)
						{
							var productVariationInfo = ProductCommon.GetProductVariationInfo(
								item.ShopId,
								item.ProductId,
								product[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID].ToString(),
								this.Cart.MemberRankId)[0];

							cartNoveltyGrantItemList.Add(new CartNoveltyGrantItem(item, product));

							if ((productVariationInfo.Row.Table.Columns.Contains(
									Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION))
								&& (productVariationInfo[Constants.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION]
									!= DBNull.Value))
							{
								cartNoveltyGrantItemList[0].Price =
									((decimal)productVariationInfo[Constants
										.FIELD_PRODUCTPRICE_MEMBER_RANK_PRICE_VARIATION]).ToString();
							}
						}
					}
				}
				// 付与アイテムが存在する?
				if (cartNoveltyGrantItemList.Count != 0)
				{
					switch (grantConditions.UserRankId)
					{
						// 全ユーザ
						case Constants.FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBERRANK_ALL:
							memberAll = cartNoveltyGrantItemList;
							break;
						// 会員のみ
						case Constants.FLG_NOVELTYGRANTCONDITIONS_USER_RANK_ID_MEMBER_ONLY:
							memberOnly = cartNoveltyGrantItemList;
							break;
						// 会員ランク
						default:
							var memberRankNo = MemberRankOptionUtility.GetMemberRankNo(grantConditions.UserRankId);
							if (memberRank.Count == 0) maxPriorityMemberRankNo = memberRankNo;
							if (memberRankNo <= maxPriorityMemberRankNo)
							{
								memberRank = cartNoveltyGrantItemList;
								maxPriorityMemberRankNo = memberRankNo;
							}
							break;
					}
				}
			}

			// 適用対象会員の内、最も優先度の高い（会員ランク > 会員のみ > 全ユーザ）付与アイテム取得
			if (memberRank.Count != 0) return memberRank.ToArray();
			if (memberOnly.Count != 0) return memberOnly.ToArray();
			if (memberAll.Count != 0) return memberAll.ToArray();

			return new CartNoveltyGrantItem[0];
		}

		/// <summary>
		/// ノベルティ名(表示用)翻訳名取得
		/// </summary>
		/// <returns></returns>
		private string GetNoveltyDispNameTranslationName()
		{
			var noveltyDispNameTranslationName = NameTranslationCommon.GetTranslationName(
				this.NoveltyId,
				Constants.FLG_NAMETRANSLATIONSETTING_DATA_KBN_NOVELTY,
				Constants.FLG_NAMETRANSLATIONSETTING_TRANSLATION_TARGET_COLUMN_NOVELTY_NOVELTY_DISP_NAME,
				base.NoveltyDispName);

			return noveltyDispNameTranslationName;
		}

		#endregion

		#region プロパティ
		/// <summary>カート</summary>
		public CartObject Cart { get; private set; }
		/// <summary>付与アイテムリスト</summary>
		public CartNoveltyGrantItem[] GrantItemList { get; private set; }
		/// <summary>対象アイテムが存在する?</summary>
		public bool ExistsTargetItem { get; private set; }
		/// <summary>付与条件が存在する?</summary>
		public bool ExistsGrantConditions { get; private set; }
		/// <summary>ノベルティ名(表示用)</summary>
		public new string NoveltyDispName
		{
			get
			{
				if (Constants.GLOBAL_OPTION_ENABLE == false) return base.NoveltyDispName;
				return GetNoveltyDispNameTranslationName();
			}
		}
		#endregion
	}
}