/*
=========================================================================================================
  Module      : Product Default Setting Service Interface (IProductDefaultSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ProductDefaultSetting
{
	/// <summary>
	/// Product default setting service interface
	/// </summary>
	public interface IProductDefaultSettingService : IService
	{
		/// <summary>
		/// Get by shop ID
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <returns>Product default setting model</returns>
		ProductDefaultSettingModel GetByShopId(string shopId);

		/// <summary>
		/// Insert or update
		/// </summary>
		/// <param name="model">Model</param>
		void Upsert(ProductDefaultSettingModel model);
	}
}
