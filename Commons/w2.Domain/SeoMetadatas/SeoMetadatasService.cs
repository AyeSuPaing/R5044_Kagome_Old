/*
=========================================================================================================
  Module      : SEOメタデータサービス (SeoMetadatasService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.SeoMetadatas
{
	/// <summary>
	/// SEOメタデータサービス
	/// </summary>
	public class SeoMetadatasService : ServiceBase, ISeoMetadatasService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデルリスト</returns>
		public SeoMetadatasModel Get(string shopId, string dataKbn)
		{
			using (var repository = new SeoMetadatasRepository())
			{
				var model = repository.GetSeoMetadata(shopId, dataKbn);
				return model;
			}
		}
		#endregion

		#region +GetAll すべて取得
		/// <summary>
		/// すべて取得
		/// </summary>
		/// <returns>モデルリスト</returns>
		public SeoMetadatasModel[] GetAll()
		{
			using (var repository = new SeoMetadatasRepository())
			{
				var models = repository.GetAll();
				return models;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(SeoMetadatasModel model)
		{
			using (var repository = new SeoMetadatasRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(SeoMetadatasModel model)
		{
			using (var repository = new SeoMetadatasRepository())
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +UpdateForDefaultText 更新
		/// <summary>
		/// デフォルト文言更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateForDefaultText(SeoMetadatasModel model)
		{
			using (var repository = new SeoMetadatasRepository())
			{
				var result = repository.UpdateForDefaultText(model);
				return result;
			}
		}
		#endregion
	}
}
