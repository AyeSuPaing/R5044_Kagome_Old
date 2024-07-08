/*
=========================================================================================================
  Module      : Interface Default Setting Service (IDefaultSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.DefaultSetting
{
	/// <summary>
	/// Interface Default Setting Service
	/// </summary>
	public interface IDefaultSettingService : IService
	{
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="classification">Classification</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Default setting model</returns>
		DefaultSettingModel Get(string shopId, string classification, SqlAccessor accessor = null);

		/// <summary>
		/// Upsert default setting
		/// </summary>
		/// <param name="model">Default setting model</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Number of affected cases</returns>
		int UpsertDefaultSetting(DefaultSettingModel model, SqlAccessor accessor = null);
	}
}
