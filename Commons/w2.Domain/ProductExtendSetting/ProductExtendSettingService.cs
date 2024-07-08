/*
=========================================================================================================
  Module      : Product Extend Setting Service (ProductExtendSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ProductExtendSetting
{
	/// <summary>
	/// Product Extend Setting Service
	/// </summary>
	public class ProductExtendSettingService : ServiceBase, IProductExtendSettingService
	{
		#region ~GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>Models</returns>
		public ProductExtendSettingModel[] GetAll(string shopId)
		{
			using (var repository = new ProductExtendSettingRepository())
			{
				var models = repository.GetAll(shopId);
				return models;
			}
		}
		#endregion
	}
}
