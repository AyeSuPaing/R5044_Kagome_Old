/*
=========================================================================================================
  Module      : SEOメタデータサービスのインターフェース (ISeoMetadatasService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Domain.SeoMetadatas
{
	/// <summary>
	/// SEOメタデータサービスのインターフェース
	/// </summary>
	public interface ISeoMetadatasService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデルリスト</returns>
		SeoMetadatasModel Get(string shopId, string dataKbn);

		/// <summary>
		/// すべて取得
		/// </summary>
		/// <returns>モデルリスト</returns>
		SeoMetadatasModel[] GetAll();

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(SeoMetadatasModel model);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		int Update(SeoMetadatasModel model);

		/// <summary>
		/// デフォルト文言更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		int UpdateForDefaultText(SeoMetadatasModel model);
	}
}