/*
=========================================================================================================
  Module      : 配送種別ユーティリティクラス(ShopShippingUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Order;

namespace w2.App.Common.Util
{
	/// <summary>
	/// 配送種別ユーティリティクラス
	/// </summary>
	public static class ShopShippingUtility
	{
		/// <summary>
		/// 異なる配送種別IDでも同一カートに入るIDかの判定
		/// </summary>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>判定結果</returns>
		public static bool CanSameCartByDifferentShippingId(string shippingId)
		{
			// 配送種別が通常、冷蔵、冷凍の場合にカート分割しないための判定
			var result = ((shippingId == Constants.SHIPPING_ID_FREEZER)
				|| (shippingId == Constants.SHIPPING_ID_REFRIGERATOR)
				|| (shippingId == Constants.SHIPPING_ID_NORMAL)
				|| (shippingId == Constants.SHIPPING_ID_PACKET));
			return result;
		}

		/// <summary>
		/// 配送種別IDを選択
		/// </summary>
		/// <param name="products">カート商品配列</param>
		/// <remarks>異なる配送種別の場合、冷凍＞冷蔵＞通常の優先順で選択する</remarks>
		/// <returns>適当な配送種別ID</returns>
		public static string ChooseShippingType(CartProduct[] products)
		{
			if (products.Any() == false) return string.Empty;

			var shippingTypes = products.Select(p => p.ShippingType).ToArray();
			var result = ChooseShippingType(shippingTypes);
			return result;
		}
		/// <summary>
		/// 配送種別IDを選択
		/// </summary>
		/// <param name="shippingTypes">配送種別ID配列</param>
		/// <remarks>異なる配送種別の場合、冷凍＞冷蔵＞通常の優先順で選択する</remarks>
		/// <returns>適当な配送種別ID</returns>
		public static string ChooseShippingType(string[] shippingTypes)
		{
			if (shippingTypes.Any() == false) return string.Empty;

			var distinctShippingTypes = shippingTypes.Distinct().ToArray();
			if (distinctShippingTypes.Length == 1) return distinctShippingTypes[0];

			// 冷凍配送種別がある場合、冷凍を選択
			if (distinctShippingTypes.Any(id => (id == Constants.SHIPPING_ID_FREEZER)))
				return Constants.SHIPPING_ID_FREEZER;

			// 冷蔵配送種別がある場合、冷蔵を選択
			if (distinctShippingTypes.Any(id => (id == Constants.SHIPPING_ID_REFRIGERATOR)))
				return Constants.SHIPPING_ID_REFRIGERATOR;

			// 通常配送種別がある場合、通常を選択
			if (distinctShippingTypes.Any(id => (id == Constants.SHIPPING_ID_NORMAL)))
				return Constants.SHIPPING_ID_NORMAL;

			// 配送種別が全てゆうパケットの場合、ゆうパケットを選択し、一旦他は先頭のものを選択
			return distinctShippingTypes.All(id => (id == Constants.SHIPPING_ID_PACKET))
				? Constants.SHIPPING_ID_PACKET
				: distinctShippingTypes[0];
		}
	}
}
