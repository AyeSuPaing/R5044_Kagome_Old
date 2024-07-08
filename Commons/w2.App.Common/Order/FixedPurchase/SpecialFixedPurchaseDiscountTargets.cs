/*
=========================================================================================================
  Module      : 定期特典割引対象クラス(SpecialFixedPurchaseDiscountTargets.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;

namespace w2.App.Common.Order.FixedPurchase
{
	/// <summary>
	/// 定期特典割引対象クラス
	/// </summary>
	[Serializable]
	public class SpecialFixedPurchaseDiscountTargets
	{
		/// <summary>
		/// コントラクター
		/// </summary>
		public SpecialFixedPurchaseDiscountTargets()
		{
			this.IsCombinableWithOther = true;
			this.ShippingFeeDiscountType = Constants.SHIPPING_FEE_DISCOUNT_TYPE_NONE;
			this.DiscountableRateAndProductIds = new List<KeyValuePair<string, decimal>>();
		}

		/// <summary>
		/// 適用対象があるか
		/// </summary>
		/// <returns>TRUE:ある；FALSE:ない</returns>
		public bool HasApplication()
		{
			return ((this.ShippingFeeDiscountType != Constants.SHIPPING_FEE_DISCOUNT_TYPE_NONE)
				|| this.DiscountableRateAndProductIds.Any());
		}

		/// <summary>セットプロモーションまたは他の定期特典との併用可能か</summary>
		public bool IsCombinableWithOther { get; set; }
		/// <summary>配送料割引区分</summary>
		public string ShippingFeeDiscountType { get; set; }
		/// <summary>配送料割引後の額</summary>
		public decimal? ShippingFeeAfterDiscount { get; set; }
		/// <summary>割引率と割引可能の商品IDリスト（＜商品ID,割引率＞の形）</summary>
		public List<KeyValuePair<string, decimal>> DiscountableRateAndProductIds { get; set; }
	}
}
