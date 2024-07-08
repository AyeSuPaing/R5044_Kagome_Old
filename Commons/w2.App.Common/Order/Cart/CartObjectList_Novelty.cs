/*
=========================================================================================================
  Module      : カートオブジェクトリストクラスのパーシャルクラス(CartObjectList_Novelty.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Order
{
	///*********************************************************************************************
	/// <summary>
	/// カートオブジェクトリストクラス
	/// </summary>
	///*********************************************************************************************
	public partial class CartObjectList : IEnumerable
	{
		/// <summary>
		/// 付与条件外のカート内の付与アイテムを削除
		/// </summary>
		/// <param name="cartNoveltyList">カートノベルティリスト</param>
		public void RemoveNoveltyGrantItem(CartNoveltyList cartNoveltyList)
		{
			// 付与条件外のカート内の付与アイテムを抽出し、カートから付与アイテムを削除
			this.Items
				.ForEach(cart => cart.Items
					.Where(p => (p.NoveltyId != "") && (IsApplicableNoveltyGrantItem(p, cartNoveltyList) == false))
					.ToList()
					.ForEach(p => cart.RemoveProduct(p.ShopId, p.ProductId, p.VariationId, Constants.AddCartKbn.Normal, p.ProductSaleId, "")));

			// LandingCartの場合はカート削除は行わない
			if (this.IsLandingCart) return;
			
			// 付与アイテム削除後、カート内の商品件数が0件の場合、カート削除
			this.Items
				.Where(cart => cart.Items.Count == 0)
				.ToList()
				.ForEach(DeleteCartVurtual);
		}

		/// <summary>
		/// 有効な付与アイテム?
		/// </summary>
		/// <param name="cartProduct">カート商品</param>
		/// <param name="cartNoveltyList">カートノベルティリスト</param>
		/// <returns>有効：true、無効：false</returns>
		private bool IsApplicableNoveltyGrantItem(CartProduct cartProduct, CartNoveltyList cartNoveltyList)
		{
			// 各カートのノベルティ情報を取得
			var cartNovelty = cartNoveltyList.Items.SelectMany(noveltyList => cartNoveltyList.Items[noveltyList.Key]);
			// 有効なノベルティ商品かチェック
			var result = CheckNoveltyGrantItem(cartProduct,
				cartNovelty.SelectMany(n => n.GrantItemList),
				cartNovelty.Where(i => i.TargetItemList.Any(t => (t.NoveltyTargetItemType == Constants.FLG_NOVELTY_TARGET_ITEM_TYPE_ALL))));
			return result;
		}

		/// <summary>
		/// 有効なノベルティ商品かチェックする
		/// </summary>
		/// <param name="cartProduct">カート商品</param>
		/// <param name="allGrantItemList">カートノベルティ商品リスト</param>
		/// <param name="targetTypeAllNovelties">全商品対象ノベルティ情報</param>
		/// <returns>ノベルティ商品削除するか(True：削除 False：残す)</returns>
		private bool CheckNoveltyGrantItem(CartProduct cartProduct, IEnumerable<CartNoveltyGrantItem> allGrantItemList, IEnumerable<CartNovelty> targetTypeAllNovelties)
		{
			// 商品の有効性チェック
			var result = allGrantItemList.Any(grantItem =>
				((grantItem.NoveltyId == cartProduct.NoveltyId)
					&& (grantItem.ShopId == cartProduct.ShopId)
					&& (grantItem.ProductId == cartProduct.ProductId)
					&& (grantItem.VariationId == cartProduct.VariationId)));

			// 全商品対象の場合自身を対象ノベルティさせない
			if (targetTypeAllNovelties.Any(i =>
				((i.NoveltyId == cartProduct.NoveltyId)
					&& (i.GrantItemList.Any(p => p.ProductId == cartProduct.ProductId)))))
			{
				// 各カートにノベルティ以外の商品が存在する場合はノベルティ商品を残す
				// 存在しない場合は削除する
				result = this.Items.Any(i => i.Items.Any(p =>
					(string.IsNullOrEmpty(p.NoveltyId)
						&& (p.ProductId != cartProduct.ProductId)
						&& (p.VariationId != cartProduct.VariationId))));
			}

			return result;
		}
	}
}
