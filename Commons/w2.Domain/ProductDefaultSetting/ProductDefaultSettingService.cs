/*
=========================================================================================================
  Module      : Product Default Setting Service (ProductDefaultSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.ProductDefaultSetting
{
	/// <summary>
	/// Product default setting service
	/// </summary>
	public class ProductDefaultSettingService : ServiceBase, IProductDefaultSettingService
	{
		#region +GetByShopId
		/// <summary>
		/// Get by shop ID
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <returns>Product default setting model</returns>
		public ProductDefaultSettingModel GetByShopId(string shopId)
		{
			using (var repository = new ProductDefaultSettingRepository())
			{
				var model = repository.GetByShopId(shopId);
				return model;
			}
		}
		#endregion

		#region +Upsert
		/// <summary>
		/// Insert or update
		/// </summary>
		/// <param name="model">Model</param>
		public void Upsert(ProductDefaultSettingModel model)
		{
			using (var repository = new ProductDefaultSettingRepository())
			{
				repository.Upsert(model);
			}
		}
		#endregion
	}
}
