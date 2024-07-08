/*
=========================================================================================================
  Module      : パーツデザイン パーツ管理サービス (PartsDesignService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.PartsDesign.Helper;

namespace w2.Domain.PartsDesign
{
	/// <summary>
	/// パーツデザイン パーツ管理サービス
	/// </summary>
	public class PartsDesignService : ServiceBase, IPartsDesignService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(PartsDesignListSearch condition)
		{
			using (var repository = new PartsDesignRepository())
			{
				var count = repository.GetSearchHitCount(condition);
				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public PartsDesignListSearchResult[] Search(PartsDesignListSearch condition)
		{
			using (var repository = new PartsDesignRepository())
			{
				var results = repository.Search(condition);
				return results;
			}
		}

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="otherPageGroupModel">その他 グループモデル</param>
		/// <returns>検索結果列</returns>
		public PartsDesignListSearchGroupResult[] SearchGroup(
			PartsDesignListSearch condition,
			PartsDesignGroupModel otherPageGroupModel)
		{
			var searchResult = Search(condition);

			var groupModelList = GetAllGroup();
			var result = groupModelList.Where(g => (condition.GroupId == null) || (g.GroupId == condition.GroupId.Value))
				.Select(
					g => new PartsDesignListSearchGroupResult(g.DataSource)
					{
						PartsList = searchResult.Where(s => s.GroupId == g.GroupId).ToArray()
					}).ToArray();

			if ((condition.GroupId == null)
				|| (Constants.FLG_PARTSDESIGNGROUP_GROUP_ID_OTHER_ID == condition.GroupId.Value))
			{
				var otherGroupModel = new PartsDesignListSearchGroupResult(otherPageGroupModel.DataSource)
				{
					PartsList = searchResult
						.Where(s => (s.GroupId == Constants.FLG_PARTSDESIGNGROUP_GROUP_ID_OTHER_ID))
						.ToArray()
				};
				Array.Resize(ref result, result.Length + 1);
				result[result.Length - 1] = otherGroupModel;
			}
			return result;
		}
		#endregion

		#region グループ系
		#region +GetGroup グループ取得
		/// <summary>
		/// グループ取得
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public PartsDesignGroupModel GetGroup(long groupId, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				var model = repository.GetGroup(groupId);
				return model;
			}
		}
		#endregion

		#region +GetAllGroup グループ全取得
		/// <summary>
		/// グループ全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		public PartsDesignGroupModel[] GetAllGroup()
		{
			using (var repository = new PartsDesignRepository())
			{
				var model = repository.GetAllGroup();
				return model;
			}
		}
		#endregion

		#region +InsertGroup グループ登録
		/// <summary>
		/// グループ登録
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>新規グループID</returns>
		public int InsertGroup(PartsDesignGroupModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				var id = repository.InsertGroup(model);
				return id;
			}
		}
		#endregion

		#region +UpdateGroup グループ更新
		/// <summary>
		/// グループ更新
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateGroup(PartsDesignGroupModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				var result = repository.UpdateGroup(model);
				return result;
			}
		}
		#endregion

		#region +UpdateGroupSort グループ順序 更新
		/// <summary>
		/// グループ順序 更新
		/// </summary>
		/// <param name="groupIds">グループ順序</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public void UpdateGroupSort(long[] groupIds, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				for (var i = 0; i < groupIds.Length; i++)
				{
					var model = repository.GetGroup(groupIds[i]);
					if (model == null)
					{
						continue;
					}

					model.GroupSortNumber = i + 1;
					repository.UpdateGroup(model);
				}
			}
		}
		#endregion

		#region +Delete グループ削除
		/// <summary>
		/// グループ削除
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int DeleteGroup(long groupId, string lastChanged, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				var result = repository.DeleteGroup(groupId);
				repository.UpdatePartsMoveOtherGroup(groupId, lastChanged);
				return result;
			}
		}
		#endregion
		#endregion

		#region パーツ系
		#region +GetParts パーツ取得
		/// <summary>
		/// パーツ取得
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public PartsDesignModel GetParts(long partsId, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				var model = repository.GetParts(partsId);
				return model;
			}
		}
		#endregion

		#region +GetPartsByFileName ファイル名でパーツ取得
		/// <summary>
		/// ファイル名でパーツ取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public PartsDesignModel GetPartsByFileName(string fileName, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				var model = repository.GetPartsByFileName(fileName);
				return model;
			}
		}
		#endregion

		#region +GetPartsByAreaId パーツ取得 条件:特集エリアID
		/// <summary>
		/// パーツ取得
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public PartsDesignModel GetPartsByAreaId(string areaId, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				var model = repository.GetPartsByAreaId(areaId);
				return model;
			}
		}
		#endregion

		#region +GetAllParts パーツ 全取得
		/// <summary>
		/// パーツ 全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		public PartsDesignModel[] GetAllParts()
		{
			using (var repository = new PartsDesignRepository())
			{
				var model = repository.GetAllParts();
				return model;
			}
		}
		#endregion

		#region +InsertParts パーツ登録
		/// <summary>
		/// パーツ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>新規パーツID</returns>
		public int InsertParts(PartsDesignModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				var id = repository.InsertParts(model);
				return id;
			}
		}
		#endregion

		#region +UpdateParts パーツ更新
		/// <summary>
		/// パーツ更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateParts(PartsDesignModel model, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				var result = (model.PartsType == Constants.FLG_PAGEDESIGN_PAGE_TYPE_CUSTOM)
					? repository.UpdateCustomParts(model)
					: repository.UpdateNormalParts(model);
				return result;
			}
		}
		#endregion

		#region +UpdateManagementTitle 管理用タイトル更新
		/// <summary>
		///  管理用タイトル更新
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <param name="title">管理用タイトル</param>
		public void UpdateManagementTitle(long partsId, string title)
		{
			var model = GetParts(partsId);
			if (model == null) return;
			model.ManagementTitle = title;
			UpdateParts(model);
		}
		#endregion

		#region +UpdatePartsSort パーツ順序 更新
		/// <summary>
		/// パーツ順序 更新
		/// </summary>
		/// <param name="models">モデル配列</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdatePartsSort(PartsDesignModel[] models, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				foreach (var model in models)
				{
					repository.UpdatePartsGroupSort(model);
				}
			}
		}
		#endregion

		#region +DeleteParts パーツ削除
		/// <summary>
		/// パーツ削除
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteParts(long partsId, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				var result = repository.DeleteParts(partsId);
				return result;
			}
		}
		#endregion

		#region +UpdatePartsMoveOtherGroup パーツをその他グループに移動
		/// <summary>
		/// パーツをその他グループに移動
		/// </summary>
		/// <param name="groupId">変更対象のグループID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void UpdatePartsMoveOtherGroup(long groupId, string lastChanged, SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				repository.UpdatePartsMoveOtherGroup(groupId, lastChanged);
			}
		}
		#endregion

		#region +NotExistGroupIds 存在しないグループと紐づいているページのグループIDを抽出
		/// <summary>
		/// 存在しないグループと紐づいているページのグループIDを抽出
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public long[] NotExistGroupIds(SqlAccessor accessor = null)
		{
			using (var repository = new PartsDesignRepository(accessor))
			{
				var result = repository.NotExistGroupIds();
				return result;
			}
		}
		#endregion
		#endregion
	}
}