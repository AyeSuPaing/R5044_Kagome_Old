/*
=========================================================================================================
  Module      : 店舗配送料地帯マスタモデル (ShopShippingZoneModel_EX.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 店舗配送料地帯マスタモデル
	/// </summary>
	public partial class ShopShippingZoneModel
	{
		#region メソッド
		#endregion

		#region プロパティ
		/// <summary>条件付き配送料が無料設定か</summary>
		public bool IsConditionalShippingPriceFree
		{
			get
			{
				var result = ((this.ConditionalShippingPriceThreshold != null)
					&& (this.ConditionalShippingPrice == 0m));
				return result;
			}
		}
		/// <summary>配送不可エリアかどうか</summary>
		public bool IsUnavailableShippingAreaFlg
		{
			get
			{
				return (this.UnavailableShippingAreaFlg == Constants.FLG_SHOPSHIPPINGZONE_UNAVAILABLE_SHIPPING_AREA_VALID);
			}
		}
		#endregion
	}
}
