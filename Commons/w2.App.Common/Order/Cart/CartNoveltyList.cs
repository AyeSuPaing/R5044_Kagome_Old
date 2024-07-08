/*
=========================================================================================================
  Module      : カートノベルティリストクラス(CartNoveltyList.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.DataCacheController;
using w2.App.Common.Order;

namespace w2.App.Common.Order
{
	/// <summary>
	/// カートノベルティリストクラス
	/// </summary>
	[Serializable]
	public class CartNoveltyList
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CartNoveltyList()
		{
			this.Items = new Dictionary<string, CartNovelty[]>();
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CartNoveltyList(CartObjectList cartList)
			: this()
		{
			// カート数分繰り返す
			foreach (var cart in cartList.Items)
			{
				// 対象アイテム有り AND 付与条件有りの場合、
				// カートノベルティリストに追加
				var cartNoveltyList =
					DataCacheControllerFacade.GetNoveltyCacheController().GetApplicableNovelty()
					.Select(n => new CartNovelty(cart, n))
					.Where(cn => (cn.ExistsTargetItem && cn.ExistsGrantConditions))
					.ToArray();
				this.Items.Add(cart.CartId, cartNoveltyList);
			}
		}
		#endregion

		#region メソッド
		/// <summary>
		/// カートに追加された付与アイテムを含むカートノベルティを削除
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		public void RemoveCartNovelty(CartObjectList cartList)
		{
			// 0件の場合、処理を抜ける
			if (this.Items.Count == 0) return;

			// 全てのカートに追加された付与アイテムを取得
			var noveltyesAdded =
				cartList.Items.SelectMany(cart => cart.Items.Where(item => (item.NoveltyId != ""))).ToList();

			// カートに追加された付与アイテムを含むカートノベルティを削除
			noveltyesAdded.ForEach(item => 
				this.Items.ToList().ForEach(noveltyList => 
					{
						this.Items[noveltyList.Key] = 
							this.Items[noveltyList.Key]
							.Where(novelty => 
								novelty.GrantItemList.All(grantItem => 
									(((grantItem.ShopId == item.ShopId) && (grantItem.ProductId == item.ProductId)) == false))).ToArray();
					})
			);
		}

		/// <summary>
		/// 付与アイテムが存在する?
		/// </summary>
		public bool ExistsCartNoveltyGrantItem()
		{
			return this.Items.Any(noveltyList => this.Items[noveltyList.Key].Any(novelty => novelty.GrantItemList.Length != 0));
		}

		/// <summary>
		/// カートノベルティ取得
		/// </summary>
		/// <param name="cartId">カートID</param>
		/// <returns>カートノベルティ</returns>
		public CartNovelty[] GetCartNovelty(string cartId)
		{
			return this.Items[cartId];
		}
		#endregion

		#region プロパティ
		/// <summary>カートノベルティリスト（key：カートID、value：カートノベルティリスト）</summary>
		public Dictionary<string, CartNovelty[]> Items { get; private set; }
		#endregion
	}
}
