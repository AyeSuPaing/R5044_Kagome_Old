/*
=========================================================================================================
  Module      : 特集エリアリポジトリ (FeatureAreaRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.FeatureArea.Helper;

namespace w2.Domain.FeatureArea
{
	/// <summary>
	/// 特集エリアリポジトリ
	/// </summary>
	internal class FeatureAreaRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "FeatureArea";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal FeatureAreaRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal FeatureAreaRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetFeatureArea 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <returns>モデル</returns>
		internal FeatureAreaModel GetFeatureArea(string areaId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FEATUREAREA_AREA_ID, areaId},
			};
			var dv = Get(XML_KEY_NAME, "GetFeatureArea", ht);
			if (dv.Count == 0) return null;
			var model = new FeatureAreaModel(dv[0])
			{
				Banners = dv.Cast<DataRowView>().Select(drv => new FeatureAreaBannerModel(drv)).ToArray()
			};
			return model;
		}
		#endregion

		#region ~GetFeatureAreaAll 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル</returns>
		internal FeatureAreaModel[] GetFeatureAreaAll()
		{
			var dv = Get(XML_KEY_NAME, "GetFeatureAreaAll", new Hashtable());
			if (dv.Count == 0) return new FeatureAreaModel[0];

			var idList = dv.Cast<DataRowView>()
				.Select(drv => (string)drv[Constants.FIELD_FEATUREAREA_AREA_ID])
				.Distinct()
				.ToArray();
			var areaModels = idList.Select(
				id =>
				{
					var areaModel = new FeatureAreaModel(
						dv.Cast<DataRowView>().First(data => (string)data[Constants.FIELD_FEATUREAREA_AREA_ID] == id));
					var bannerModels = dv.Cast<DataRowView>()
						.Where(drv => (string)drv[Constants.FIELD_FEATUREAREA_AREA_ID] == id)
						.Select(drv => new FeatureAreaBannerModel(drv)).ToArray();
					areaModel.Banners = bannerModels;
					return areaModel;
				}).ToArray();

			return areaModels;
		}
		#endregion

		#region ~GetFeatureAreaSearchAll 取得
		/// <summary>
		/// 特集エリアすべて取得
		/// </summary>
		/// <returns>モデル</returns>
		internal FeatureAreaListSearchResult[] GetFeatureAreaSearchAll()
		{
			var dv = Get(XML_KEY_NAME, "GetFeatureAreaSearchAll", new Hashtable());
			return dv.Cast<DataRowView>().Select(drv => new FeatureAreaListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetFeatureAreaBanner 特集エリアバナー取得
		/// <summary>
		/// 特集エリアバナー取得
		/// </summary>
		/// <returns>取得結果</returns>
		public FeatureAreaBannerModel[] GetFeatureAreaBanner(string areaId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_FEATUREAREA_AREA_ID, areaId},
			};
			var dv = Get(XML_KEY_NAME, "GetFeatureAreaBanner", ht);
			return dv.Cast<DataRowView>().Select(drv => new FeatureAreaBannerModel(drv)).ToArray();
		}
		#endregion

		#region ~GetAllFeatureAreaBanner 特集エリアバナー全取得
		/// <summary>
		/// 特集エリアバナー全取得
		/// </summary>
		/// <returns>取得結果</returns>
		public FeatureAreaBannerModel[] GetAllFeatureAreaBanner()
		{
			var dv = Get(XML_KEY_NAME, "GetAllFeatureAreaBanner");
			return dv.Cast<DataRowView>().Select(drv => new FeatureAreaBannerModel(drv)).ToArray();
		}
		#endregion

		#region ~InsertFeatureArea 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertFeatureArea(FeatureAreaModel model)
		{
			Exec(XML_KEY_NAME, "InsertFeatureArea", model.DataSource);
		}
		#endregion

		#region ~UpdateFeatureArea 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		internal void UpdateFeatureArea(FeatureAreaModel model)
		{
			Exec(XML_KEY_NAME, "UpdateFeatureArea", model.DataSource);
		}
		#endregion

		#region ~DeleteFeatureArea 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		internal void DeleteFeatureArea(string areaId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FEATUREAREA_AREA_ID, areaId }
			};
			Exec(XML_KEY_NAME, "DeleteFeatureArea", ht);
		}
		#endregion

		#region ~InsertFeatureAreaBanner 特集エリアバナー登録
		/// <summary>
		/// 特集エリアバナー登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertFeatureAreaBanner(FeatureAreaBannerModel model)
		{
			Exec(XML_KEY_NAME, "InsertFeatureAreaBanner", model.DataSource);
		}
		#endregion

		#region ~UpdateFeatureAreaBanner 特集エリアバナー更新
		/// <summary>
		/// 特集エリアバナー更新(DELETE/INSERT)
		/// </summary>
		/// <param name="model">モデル</param>
		internal void UpdateFeatureAreaBanner(FeatureAreaModel model)
		{
			DeleteFeatureAreaBanner(model.AreaId);
			foreach (var banner in model.Banners)
			{
				InsertFeatureAreaBanner(banner);
			}
		}
		#endregion

		#region ~DeleteFeatureAreaBanner 特集エリアバナー削除
		/// <summary>
		/// 特集エリアバナー削除
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		internal void DeleteFeatureAreaBanner(string areaId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FEATUREAREA_AREA_ID, areaId }
			};
			Exec(XML_KEY_NAME, "DeleteFeatureAreaBanner", ht);
		}
		#endregion

		#region ~GetFeatureAreaList 特集エリア配列取得
		/// <summary>
		/// 特集エリア配列取得
		/// </summary>
		/// <returns>特集エリア配列</returns>
		internal FeatureAreaListSearchResult[] GetFeatureAreaList()
		{
			var dv = Get(XML_KEY_NAME, "GetFeatureAreaList", new Hashtable());
			return dv.Cast<DataRowView>().Select(drv => new FeatureAreaListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetFeatureAreaCount 特集エリアタイプ参照数取得
		/// <summary>
		/// 特集エリアタイプ参照数取得
		/// </summary>
		/// <returns>特集エリアタイプと参照数</returns>
		internal KeyValuePair<string, int>[] GetFeatureAreaCount()
		{
			var dv = Get(XML_KEY_NAME, "GetFeatureAreaCount", new Hashtable());
			return dv.Cast<DataRowView>()
				.Select(drv => new KeyValuePair<string, int>((string)drv[0], (int)drv[1]))
				.ToArray();
		}
		#endregion
		
		#region ~InsertFeatureAreaType 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertFeatureAreaType(FeatureAreaTypeModel model)
		{
			Exec(XML_KEY_NAME, "InsertFeatureAreaType", model.DataSource);
		}
		#endregion

		#region ~UpdateFeatureAreaType 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		internal void UpdateFeatureAreaType(FeatureAreaTypeModel model)
		{
			Exec(XML_KEY_NAME, "UpdateFeatureAreaType", model.DataSource);
		}
		#endregion

		#region ~DeleteFeatureAreaType 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		internal void DeleteFeatureAreaType(string areaTypeId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FEATUREAREATYPE_AREA_TYPE_ID, areaTypeId }
			};
			Exec(XML_KEY_NAME, "DeleteFeatureAreaType", ht);
		}
		#endregion

		#region ~GetFeatureAreaType 特集エリアタイプ取得
		/// <summary>
		/// 特集エリアタイプ取得
		/// </summary>
		/// <param name="areaTypeId">特集エリアタイプID</param>
		/// <returns>特集エリアタイプ</returns>
		internal FeatureAreaTypeModel GetFeatureAreaType(string areaTypeId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_FEATUREAREATYPE_AREA_TYPE_ID, areaTypeId }
			};
			var dv = Get(XML_KEY_NAME, "GetFeatureAreaType", ht);
			return dv.Cast<DataRowView>().Select(drv => new FeatureAreaTypeModel(drv)).FirstOrDefault();
		}
		#endregion

		#region ~GetFeatureAreaTypeList 特集エリアタイプ配列取得
		/// <summary>
		/// 特集エリアタイプ配列取得
		/// </summary>
		/// <returns>特集エリアタイプ配列</returns>
		internal FeatureAreaTypeListSearchResult[] GetFeatureAreaTypeList()
		{
			var dv = Get(XML_KEY_NAME, "GetFeatureAreaTypeList", new Hashtable());
			return dv.Cast<DataRowView>().Select(drv => new FeatureAreaTypeListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetFeatureAreaTypeCount 特集エリアタイプ利用数取得
		/// <summary>
		/// 特集エリアタイプ利用数取得
		/// </summary>
		/// <returns>特集エリアタイプと利用数</returns>
		internal KeyValuePair<string, int>[] GetFeatureAreaTypeCount()
		{
			var dv = Get(XML_KEY_NAME, "GetFeatureAreaTypeCount", new Hashtable());
			return dv.Cast<DataRowView>()
				.Select(drv => new KeyValuePair<string, int>((string)drv[0], (int)drv[1]))
				.ToArray();
		}
		#endregion

	}
}
