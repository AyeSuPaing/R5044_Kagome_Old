/*
=========================================================================================================
  Module      : 商品サブ画像設定リポジトリ (ProductSubImageSettingRepository.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ProductSubImageSetting
{
	/// <summary>
	/// 商品サブ画像設定リポジトリ
	/// </summary>
	internal class ProductSubImageSettingRepository : RepositoryBase
	{
		/// <summary>SQLファイル</summary>
		private const string XML_KEY_NAME = "ProductSubImageSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ProductSubImageSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal ProductSubImageSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +GetProductSubImageSettings
		/// <summary>
		/// Get product sub image settings
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="imageMaxCount">Image max count</param>
		/// <returns>Product sub image settings</returns>
		internal ProductSubImageSettingModel[] GetProductSubImageSettings(string shopId, int imageMaxCount)
		{
			var param = new Hashtable
			{
				{ Constants.FIELD_PRODUCTSUBIMAGESETTING_SHOP_ID, shopId },
				{ "image_max_count", imageMaxCount }
			};
			var result = Get(XML_KEY_NAME, "GetProductSubImageSettings", param);
			return result.Cast<DataRowView>()
				.Select(row => new ProductSubImageSettingModel(row))
				.ToArray();
		}
		#endregion
	}
}
