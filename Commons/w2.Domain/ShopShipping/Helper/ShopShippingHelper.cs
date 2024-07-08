/*
=========================================================================================================
  Module      : 店舗配送種別ヘルパー (ShopShippingHelper.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Data;

namespace w2.Domain.ShopShipping.Helper
{
	/// <summary>
	/// 店舗配送種別ヘルパー
	/// </summary>
	internal class ShopShippingHelper
	{
		/// <summary>
		/// 配送料情報含む店舗配送種別情報DataRowViewからモデルを返す
		/// </summary>
		/// <param name="drv">店舗配送種別情報</param>
		/// <returns>モデル</returns>
		internal ShopShippingModel GetShopShipping(DataRowView drv)
		{
			var shopShipping = new ShopShippingModel(drv);
			shopShipping.ZoneList = new [] { new ShopShippingZoneModel(drv) };
			shopShipping.CompanyPostageSettings = new[] { new ShippingDeliveryPostageModel(drv) };
			return shopShipping;
		}
	}
}
