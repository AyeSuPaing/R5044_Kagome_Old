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
	public interface IProductExtendSettingService : IService
	{
		/// <summary>
		/// 全取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>Models</returns>
		ProductExtendSettingModel[] GetAll(string shopId);
	}
}