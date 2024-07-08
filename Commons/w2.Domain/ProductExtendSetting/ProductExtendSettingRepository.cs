/*
=========================================================================================================
  Module      : Product Extend Setting Repository (ProductExtendSettingRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductExtendSetting
{
	/// <summary>
	/// Product Extend Setting Repository
	/// </summary>
	internal class ProductExtendSettingRepository : RepositoryBase
	{
		/// <returns>XML key name</returns>
		private const string XML_KEY_NAME = "ProductExtendSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductExtendSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal ProductExtendSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>Models</returns>
		internal ProductExtendSettingModel[] GetAll(string shopId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTEXTENDSETTING_SHOP_ID, shopId },
			};
			var dv = Get(XML_KEY_NAME, "GetAll", input);
			return dv.Cast<DataRowView>()
				.Select(drv => new ProductExtendSettingModel(drv))
				.ToArray();
		}
		#endregion
	}
}
