/*
=========================================================================================================
  Module      : 特集エリアサービスのインターフェース (IFeatureAreaService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.FeatureArea.Helper;

namespace w2.Domain.FeatureArea
{
	/// <summary>
	/// 特集エリアサービスのインターフェース
	/// </summary>
	public interface IFeatureAreaService : IService
	{
		/// <summary>
		/// 特集エリア削除
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <param name="accessor">SQLアクセサ</param>
		void DeleteFeatureArea(string areaId, SqlAccessor accessor = null);

		/// <summary>
		/// 特集エリアタイプ削除
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <param name="accessor">SQLアクセサ</param>
		void DeleteFeatureAreaType(string areaId, SqlAccessor accessor = null);

		/// <summary>
		/// 特集エリア取得
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <returns>モデル</returns>
		FeatureAreaModel GetFeatureArea(string areaId);

		/// <summary>
		/// 特集エリア取得
		/// </summary>
		/// <returns>モデル</returns>
		FeatureAreaModel[] GetFeatureAreaAll();

		/// <summary>
		/// 特集エリアバナー取得
		/// </summary>
		/// <returns>取得結果</returns>
		FeatureAreaBannerModel[] GetFeatureAreaBanner(string areaId);

		/// <summary>
		/// 全件取得
		/// </summary>
		/// <returns>取得結果</returns>
		FeatureAreaListSearchResult[] GetFeatureAreaSearchAll();

		/// <summary>
		/// 特集エリアタイプ取得
		/// </summary>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		/// <returns>特集エリアタイプ</returns>
		FeatureAreaTypeModel GetFeatureAreaType(string areaTypeId);

		/// <summary>
		/// 特集エリアタイプ取得
		/// </summary>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		/// <returns>特集エリアタイプ</returns>
		int GetFeatureAreaTypeCount(string areaTypeId);

		/// <summary>
		/// 特集エリアタイプ配列取得
		/// </summary>
		/// <returns>特集エリアタイプ配列</returns>
		FeatureAreaTypeListSearchResult[] GetFeatureAreaTypeList();

		/// <summary>
		/// 特集エリア登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertFeatureArea(FeatureAreaModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 特集エリアタイプ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void InsertFeatureAreaType(FeatureAreaTypeModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 特集エリア更新
		/// </summary>
		/// <param name="model">モデル</param>
		void UpdateFeatureArea(FeatureAreaModel model);

		/// <summary>
		/// 特集エリアタイプ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdateFeatureAreaType(FeatureAreaTypeModel model, SqlAccessor accessor = null);
	}
}