/*
=========================================================================================================
  Module      : Product Default Setting Repository (ProductDefaultSettingRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using w2.Common.Sql;

namespace w2.Domain.ProductDefaultSetting
{
	/// <summary>
	/// Product default setting repository
	/// </summary>
	internal class ProductDefaultSettingRepository : RepositoryBase
	{
		/// <summary>Product default setting SQL file</summary>
		private const string XML_KEY_NAME = "ProductDefaultSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ProductDefaultSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		public ProductDefaultSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetByShopId
		/// <summary>
		/// Get by shop ID
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <returns>Product default setting model</returns>
		public ProductDefaultSettingModel GetByShopId(string shopId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTDEFAULTSETTING_SHOP_ID, shopId }
			};
			var result = Get(XML_KEY_NAME, "GetByShopId", input);
			return (result.Count != 0) ? new ProductDefaultSettingModel(result[0]) : null;
		}
		#endregion

		#region +Upsert
		/// <summary>
		/// Insert or update
		/// </summary>
		/// <param name="model">Model</param>
		public void Upsert(ProductDefaultSettingModel model)
		{
			Exec(XML_KEY_NAME, "Upsert", model.DataSource);
		}
		#endregion
	}
}
