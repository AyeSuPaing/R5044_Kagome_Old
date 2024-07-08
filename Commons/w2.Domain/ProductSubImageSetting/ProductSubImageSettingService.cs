/*
=========================================================================================================
  Module      : 商品サブ画像設定サービス (ProductSubImageSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ProductSubImageSetting
{
	/// <summary>
	/// 商品サブ画像設定サービス
	/// </summary>
	public class ProductSubImageSettingService : ServiceBase, IProductSubImageSettingService
	{
		#region +GetProductSubImageSettings
		/// <summary>
		/// Get product sub image settings
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="imageMaxCount">Image max count</param>
		/// <returns>Product sub image settings</returns>
		public ProductSubImageSettingModel[] GetProductSubImageSettings(string shopId, int imageMaxCount)
		{
			using (var repository = new ProductSubImageSettingRepository())
			{
				var models = repository.GetProductSubImageSettings(shopId, imageMaxCount);
				return models;
			}
		}
		#endregion
	}
}
