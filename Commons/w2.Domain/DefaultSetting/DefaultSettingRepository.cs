/*
=========================================================================================================
  Module      : Default Setting Repository (DefaultSettingRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.DefaultSetting
{
	/// <summary>
	/// Default setting repository
	/// </summary>
	public class DefaultSettingRepository : RepositoryBase
	{
		/// <summary>Xml key name</summary>
		private const string XML_KEY_NAME = "DefaultSetting";

		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public DefaultSettingRepository()
		{
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="accessor">Sql accessor</param>
		public DefaultSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get
		/// <summary>
		/// Get
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="classification">Classification</param>
		/// <returns>Default setting model</returns>
		internal DefaultSettingModel Get(string shopId, string classification)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_DEFAULTSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_DEFAULTSETTING_CLASSIFICATION, classification },
			};
			var result = Get(XML_KEY_NAME, "Get", input);

			if (result.Count == 0) return null;

			return new DefaultSettingModel(result[0]);
		}
		#endregion

		#region +UpsertDefaultSetting
		/// <summary>
		/// Upsert default setting
		/// </summary>
		/// <param name="model">Default setting model</param>
		/// <returns>Number of affected cases</returns>
		internal int UpsertDefaultSetting(DefaultSettingModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpsertDefaultSetting", model.DataSource);
			return result;
		}
		#endregion
	}
}
