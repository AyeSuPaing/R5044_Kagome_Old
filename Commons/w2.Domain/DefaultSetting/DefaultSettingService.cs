/*
=========================================================================================================
  Module      : Default Setting Service (DefaultSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.DefaultSetting
{
	/// <summary>
	/// Default Setting Service
	/// </summary>
	public class DefaultSettingService : ServiceBase, IDefaultSettingService
	{
		#region +Get
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="classification">Classification</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Default setting model</returns>
		public DefaultSettingModel Get(string shopId, string classification, SqlAccessor accessor = null)
		{
			using (var repository = new DefaultSettingRepository(accessor))
			{
				var result = repository.Get(shopId, classification);
				return result;
			}
		}
		#endregion

		#region +UpsertDefaultSetting
		/// <summary>
		/// Upsert default setting
		/// </summary>
		/// <param name="model">Default setting model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		public int UpsertDefaultSetting(DefaultSettingModel model, SqlAccessor accessor = null)
		{
			using (var repository = new DefaultSettingRepository(accessor))
			{
				var result = repository.UpsertDefaultSetting(model);
				return result;
			}
		}
		#endregion
	}
}
