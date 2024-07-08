/*
=========================================================================================================
  Module      : 商品同梱処理実行クラス (ProductBundler.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 商品同梱処理実行クラス
	/// </summary>
	public class ProductBundler : IDisposable
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cartList">カートリスト</param>
		/// <param name="userId">ユーザーID</param>
		/// <param name="advCodeFirst">初回広告コード</param>
		/// <param name="advCodeNew">最新広告コード</param>
		/// <param name="excludeOrderIds">利用回数にカウントしない注文ID</param>
		/// <param name="hitTargetListIds">ログインユーザに有効なターゲットリスト群</param>
		/// <param name="isFront">フロント購入か</param>
		public ProductBundler(
			List<CartObject> cartList,
			string userId,
			string advCodeFirst,
			string advCodeNew,
			string[] excludeOrderIds = null,
			string[] hitTargetListIds = null,
			bool isFront = false)
		{
			this.CartList = cartList;

			var orderIds = excludeOrderIds ?? new[] { "" };
			var targetListIds = hitTargetListIds ?? new string[0];
			ProductBundleCommon.AddBundleItemsToCartList(
				this.CartList,
				userId,
				advCodeFirst,
				advCodeNew,
				orderIds,
				targetListIds,
				isFront);
		}
		#endregion

		#region +Dispose
		/// <summary>
		/// 解放処理
		/// </summary>
		public void Dispose()
		{
			ProductBundleCommon.RemoveBundleItemsToCartList(this.CartList);
		}
		#endregion

		#region プロパティ
		/// <summary>カートリスト</summary>
		public List<CartObject> CartList { get; private set; }
		#endregion
	}
}
