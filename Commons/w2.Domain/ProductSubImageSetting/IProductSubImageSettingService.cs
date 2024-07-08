/*
=========================================================================================================
  Module      : 商品サブ画像設定サービスのインターフェース (IProductSubImageSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ProductSubImageSetting
{
	/// <summary>
	/// 商品サブ画像設定サービスのインターフェース
	/// </summary>
	public interface IProductSubImageSettingService : IService
	{
		/// <summary>
		/// Get product sub image settings
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="imageMaxCount">Image max count</param>
		/// <returns>Product sub image settings</returns>
		ProductSubImageSettingModel[] GetProductSubImageSettings(string shopId, int imageMaxCount);
	}
}
