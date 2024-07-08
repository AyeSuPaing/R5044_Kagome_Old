/*
=========================================================================================================
  Module      : OGPタグ設定リポジトリ (OgpTagSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.OgpTagSetting
{
	/// <summary>
	/// OGPタグ設定リポジトリ
	/// </summary>
	internal class OgpTagSettingRepository : RepositoryBase
	{
		/// <returns>XMLページ名</returns>
		private const string XML_KEY_NAME = "OgpTagSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal OgpTagSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal OgpTagSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <returns>モデル</returns>
		internal OgpTagSettingModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			return dv.Cast<DataRowView>()
				.Select(drv => new OgpTagSettingModel(drv))
				.ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="dataKbn">データ区分</param>
		/// <returns>モデル</returns>
		internal OgpTagSettingModel Get(string dataKbn)
		{
			var dv = Get(
				XML_KEY_NAME,
				"Get",
				new Hashtable
				{
					{ Constants.FIELD_OGPTAGSETTING_DATA_KBN, dataKbn }
				});

			return dv.Cast<DataRowView>()
				.Select(drv => new OgpTagSettingModel(drv))
				.FirstOrDefault();
		}
		#endregion

		#region ~Upsert アップサート
		/// <summary>
		/// アップサート
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響件数</returns>
		internal int Upsert(OgpTagSettingModel model)
		{
			var count = Exec(XML_KEY_NAME, "Upsert", model.DataSource);
			return count;
		}
		#endregion
	}
}
