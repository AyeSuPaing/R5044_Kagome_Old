/*
=========================================================================================================
  Module      : 特集エリアサービス (FeatureAreaService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Common.Sql;
using w2.Domain.FeatureArea.Helper;

namespace w2.Domain.FeatureArea
{
	/// <summary>
	/// 特集エリアサービス
	/// </summary>
	public class FeatureAreaService : ServiceBase, IFeatureAreaService
	{
		#region +GetFeatureArea 特集エリア取得
		/// <summary>
		/// 特集エリア取得
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <returns>モデル</returns>
		public FeatureAreaModel GetFeatureArea(string areaId)
		{
			using (var repository = new FeatureAreaRepository())
			{
				var model = repository.GetFeatureArea(areaId);
				return model;
			}
		}
		#endregion

		#region +GetFeatureAreaAll 特集エリアすべて取得
		/// <summary>
		/// 特集エリア取得
		/// </summary>
		/// <returns>モデル</returns>
		public FeatureAreaModel[] GetFeatureAreaAll()
		{
			using (var repository = new FeatureAreaRepository())
			{
				var model = repository.GetFeatureAreaAll();
				return model;
			}
		}
		#endregion


		#region +GetFeatureAreaSearchAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <returns>取得結果</returns>
		public FeatureAreaListSearchResult[] GetFeatureAreaSearchAll()
		{
			using (var repository = new FeatureAreaRepository())
			{
				var models = repository.GetFeatureAreaSearchAll();
				return models;
			}
		}
		#endregion

		#region +GetFeatureAreaBanner 特集エリアバナー取得
		/// <summary>
		/// 特集エリアバナー取得
		/// </summary>
		/// <returns>取得結果</returns>
		public FeatureAreaBannerModel[] GetFeatureAreaBanner(string areaId)
		{
			using (var repository = new FeatureAreaRepository())
			{
				var models = repository.GetFeatureAreaBanner(areaId);
				return models;
			}
		}
		#endregion

		#region +GetFeatureAreaBanner 特集エリアバナー全取得
		/// <summary>
		/// 特集エリアバナー全取得
		/// </summary>
		/// <returns>取得結果</returns>
		public FeatureAreaBannerModel[] GetAllFeatureAreaBanner()
		{
			using (var repository = new FeatureAreaRepository())
			{
				var models = repository.GetAllFeatureAreaBanner();
				return models;
			}
		}
		#endregion

		#region +InsertFeatureArea 特集エリア登録
		/// <summary>
		/// 特集エリア登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertFeatureArea(FeatureAreaModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FeatureAreaRepository(accessor))
			{
				repository.InsertFeatureArea(model);
				foreach (var banner in model.Banners)
				{
					repository.InsertFeatureAreaBanner(banner);
				}
			}
		}
		#endregion

		#region +UpdateFeatureArea 特集エリア更新
		/// <summary>
		/// 特集エリア更新
		/// </summary>
		/// <param name="model">モデル</param>
		public void UpdateFeatureArea(FeatureAreaModel model)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new FeatureAreaRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				repository.UpdateFeatureArea(model);
				repository.UpdateFeatureAreaBanner(model);

				accessor.CommitTransaction();
			}
		}
		#endregion

		#region +DeleteFeatureArea 特集エリア削除
		/// <summary>
		/// 特集エリア削除
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteFeatureArea(string areaId, SqlAccessor accessor = null)
		{
			using (var repository = new FeatureAreaRepository(accessor))
			{
				repository.DeleteFeatureArea(areaId);
				repository.DeleteFeatureAreaBanner(areaId);
			}
		}
		#endregion

		#region +InsertFeatureAreaType 特集エリアタイプ登録
		/// <summary>
		/// 特集エリアタイプ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertFeatureAreaType(FeatureAreaTypeModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FeatureAreaRepository(accessor))
			{
				repository.InsertFeatureAreaType(model);
			}
		}
		#endregion

		#region +UpdateFeatureAreaType 特集エリアタイプ登録
		/// <summary>
		/// 特集エリアタイプ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdateFeatureAreaType(FeatureAreaTypeModel model, SqlAccessor accessor = null)
		{
			using (var repository = new FeatureAreaRepository(accessor))
			{
				repository.UpdateFeatureAreaType(model);
			}
		}
		#endregion

		#region +DeleteFeatureAreaType 特集エリアタイプ削除
		/// <summary>
		/// 特集エリアタイプ削除
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteFeatureAreaType(string areaId, SqlAccessor accessor = null)
		{
			using (var repository = new FeatureAreaRepository(accessor))
			{
				repository.DeleteFeatureAreaType(areaId);
			}
		}
		#endregion

		#region +GetFeatureAreaTypeList 特集エリアタイプ配列取得
		/// <summary>
		/// 特集エリアタイプ配列取得
		/// </summary>
		/// <returns>特集エリアタイプ配列</returns>
		public FeatureAreaTypeListSearchResult[] GetFeatureAreaTypeList()
		{
			using (var repository = new FeatureAreaRepository())
			{
				var results = repository.GetFeatureAreaTypeList();
				var counts = repository.GetFeatureAreaTypeCount();

				foreach (var result in results)
				{
					var count = counts.FirstOrDefault(c => (c.Key == result.AreaTypeId));
					result.ReferenceCount = count.Value;
				}
				return results;
			}
		}
		#endregion

		#region +GetFeatureAreaType 特集エリアタイプ取得
		/// <summary>
		/// 特集エリアタイプ取得
		/// </summary>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		/// <returns>特集エリアタイプ</returns>
		public FeatureAreaTypeModel GetFeatureAreaType(string areaTypeId)
		{
			using (var repository = new FeatureAreaRepository())
			{
				var results = repository.GetFeatureAreaType(areaTypeId);
				return results;
			}
		}
		#endregion

		#region +GetFeatureAreaTypeCount 特集エリアタイプ利用数取得
		/// <summary>
		/// 特集エリアタイプ取得
		/// </summary>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		/// <returns>特集エリアタイプ</returns>
		public int GetFeatureAreaTypeCount(string areaTypeId)
		{
			using (var repository = new FeatureAreaRepository())
			{
				var results = repository.GetFeatureAreaTypeCount();
				return results.FirstOrDefault(data => data.Key == areaTypeId).Value;
			}
		}
		#endregion
	}
}
