/*
=========================================================================================================
  Module      : OGPタグ設定サービス (OgpTagSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Sql;

namespace w2.Domain.OgpTagSetting
{
	/// <summary>
	/// OGPタグ設定サービス
	/// </summary>
	public class OgpTagSettingService : ServiceBase, IOgpTagSettingService
	{
		#region +GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public OgpTagSettingModel[] GetAll(SqlAccessor accessor = null)
		{
			using (var repository = new OgpTagSettingRepository(accessor))
			{
				var models = repository.GetAll();
				return models;
			}
		}
		#endregion

		#region +Get 1件取得
		/// <summary>
		/// １件取得
		/// </summary>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public OgpTagSettingModel Get(string dataKbn, SqlAccessor accessor = null)
		{
			using (var repository = new OgpTagSettingRepository(accessor))
			{
				var model = repository.Get(dataKbn);
				return model;
			}
		}
		#endregion

		#region +Upsert アップサート
		/// <summary>
		/// アップサート
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響件数</returns>
		public int Upsert(OgpTagSettingModel model, SqlAccessor accessor = null)
		{
			using (var repository = new OgpTagSettingRepository(accessor))
			{
				var count = repository.Upsert(model);
				return count;
			}
		}
		#endregion
	}
}