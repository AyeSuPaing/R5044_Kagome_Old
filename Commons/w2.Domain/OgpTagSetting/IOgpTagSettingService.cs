/*
=========================================================================================================
  Module      : OGPタグ設定サービスのインターフェース (IOgpTagSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Sql;

namespace w2.Domain.OgpTagSetting
{
	/// <summary>
	/// OGPタグ設定サービスのインターフェース
	/// </summary>
	public interface IOgpTagSettingService : IService
	{
		/// <summary>
		/// １件取得
		/// </summary>
		/// <param name="dataKbn">データ区分</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		OgpTagSettingModel Get(string dataKbn, SqlAccessor accessor = null);
		
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		OgpTagSettingModel[] GetAll(SqlAccessor accessor = null);

		/// <summary>
		/// アップサート
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響件数</returns>
		int Upsert(OgpTagSettingModel model, SqlAccessor accessor = null);
	}
}