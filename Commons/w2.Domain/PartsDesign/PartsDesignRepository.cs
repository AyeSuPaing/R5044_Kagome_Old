/*
=========================================================================================================
  Module      : パーツデザイン パーツ管理リポジトリ (PartsDesignRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.PartsDesign.Helper;

namespace w2.Domain.PartsDesign
{
	/// <summary>
	/// パーツデザイン パーツ管理リポジトリ
	/// </summary>
	internal class PartsDesignRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "PartsDesign";

		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(PartsDesignListSearch condition)
		{
			var input = condition.CreateHashtableParams();
			var dv = Get(
				XML_KEY_NAME,
				"GetSearchHitCount",
				input,
				replaces: condition.ReplaceList());

			return (int)dv[0][0];
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal PartsDesignListSearchResult[] Search(PartsDesignListSearch condition)
		{
			var input = condition.CreateHashtableParams();
			var dv = Get(
				XML_KEY_NAME,
				"Search",
				input,
				replaces: condition.ReplaceList());

			return dv.Cast<DataRowView>().Select(drv => new PartsDesignListSearchResult(drv)).ToArray();
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal PartsDesignRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal PartsDesignRepository(SqlAccessor accessor) : base(accessor)
		{
		}
		#endregion

		#region グループ系
		#region +GetGroup グループ取得
		/// <summary>
		/// グループ取得
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <returns>モデル</returns>
		internal PartsDesignGroupModel GetGroup(long groupId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PAGEDESIGNGROUP_GROUP_ID, groupId },
			};
			var dv = Get(XML_KEY_NAME, "GetGroup", ht);
			if (dv.Count == 0) return null;
			return new PartsDesignGroupModel(dv[0]);
		}
		#endregion

		#region +GetAllGroup グループ全取得
		/// <summary>
		/// グループ全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		internal PartsDesignGroupModel[] GetAllGroup()
		{
			var dv = Get(XML_KEY_NAME, "GetAllGroup", new Hashtable());
			return dv.Cast<DataRowView>().Select(item => new PartsDesignGroupModel(item)).ToArray();
		}
		#endregion

		#region +InsertGroup グループ登録
		/// <summary>
		/// グループ登録
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <returns>新規グループID</returns>
		internal int InsertGroup(PartsDesignGroupModel model)
		{
			var result = Get(XML_KEY_NAME, "InsertGroup", model.DataSource);
			return int.Parse(result[0][0].ToString());
		}
		#endregion


		#region +UpdateGroup グループ更新
		/// <summary>
		/// グループ更新
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateGroup(PartsDesignGroupModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateGroup", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete グループ削除
		/// <summary>
		/// グループ削除
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteGroup(long groupId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PAGEDESIGNGROUP_GROUP_ID, groupId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteGroup", ht);
			return result;
		}
		#endregion
		#endregion

		#region パーツ系
		#region +GetParts パーツ取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <returns>モデル</returns>
		internal PartsDesignModel GetParts(long partsId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PARTSDESIGN_PARTS_ID, partsId },
			};
			var dv = Get(XML_KEY_NAME, "GetParts", ht);
			if (dv.Count == 0) return null;
			return new PartsDesignModel(dv[0]);
		}
		#endregion

		#region +GetPartsByFileName ファイル名でパーツ取得
		/// <summary>
		/// ファイル名で取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <returns>モデル</returns>
		internal PartsDesignModel GetPartsByFileName(string fileName)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PARTSDESIGN_FILE_NAME, fileName },
			};
			var dv = Get(XML_KEY_NAME, "GetPartsByFileName", ht);
			if (dv.Count == 0) return null;
			return new PartsDesignModel(dv[0]);
		}
		#endregion

		#region +GetPartsByAreaId パーツ取得 条件:特集エリアID
		/// <summary>
		/// パーツ取得 条件:特集エリアID
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <returns>モデル</returns>
		internal PartsDesignModel GetPartsByAreaId(string areaId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PARTSDESIGN_AREA_ID, areaId },
			};
			var dv = Get(XML_KEY_NAME, "GetPartsByAreaId", ht);
			if (dv.Count == 0) return null;
			return new PartsDesignModel(dv[0]);
		}
		#endregion

		#region +GetAllParts パーツ全取得
		/// <summary>
		/// パーツ 全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		internal PartsDesignModel[] GetAllParts()
		{
			var dv = Get(XML_KEY_NAME, "GetAllParts");
			return dv.Cast<DataRowView>().Select(item => new PartsDesignModel(item)).ToArray();
		}
		#endregion

		#region +InsertParts パーツ登録
		/// <summary>
		/// パーツ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>新規パーツID</returns>
		internal int InsertParts(PartsDesignModel model)
		{
			var result = Get(XML_KEY_NAME, "InsertParts", model.DataSource);
			return int.Parse(result[0][0].ToString());
		}
		#endregion

		#region +UpdateCustomParts カスタムパーツ更新
		/// <summary>
		/// カスタムパーツ更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateCustomParts(PartsDesignModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateCustomParts", model.DataSource);
			return result;
		}
		#endregion

		#region +UpdateNormalParts 標準パーツ更新
		/// <summary>
		/// 標準パーツ更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateNormalParts(PartsDesignModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateNormalParts", model.DataSource);
			return result;
		}
		#endregion

		#region +UpdatePartsSort パーツ順序 更新
		/// <summary>
		/// パーツ順序 更新
		/// </summary>
		/// <param name="model">モデル配列</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdatePartsGroupSort(PartsDesignModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdatePartsGroupSort", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteParts パーツ削除
		/// <summary>
		/// パーツ削除
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteParts(long partsId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_PARTSDESIGN_PARTS_ID, partsId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteParts", ht);
			return result;
		}
		#endregion

		#region +UpdatePartsMoveOtherGroup パーツをその他グループに移動
		/// <summary>
		/// パーツをその他グループに移動
		/// </summary>
		/// <param name="groupId">変更対象のグループID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdatePartsMoveOtherGroup(long groupId, string lastChanged)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PARTSDESIGNGROUP_GROUP_ID, groupId},
				{Constants.FIELD_PARTSDESIGNGROUP_LAST_CHANGED, lastChanged},
			};
			var result = Exec(XML_KEY_NAME, "UpdatePartsMoveOtherGroup", ht);
			return result;
		}
		#endregion

		#region +NotExistGroupIds 存在しないグループと紐づいているページのグループIDを抽出
		/// <summary>
		/// 存在しないグループと紐づいているページのグループIDを抽出
		/// </summary>
		/// <returns>モデル</returns>
		internal long[] NotExistGroupIds()
		{
			var dv = Get(XML_KEY_NAME, "NotExistGroupIds");
			if (dv.Count == 0) return new long[] { };
			return dv.Cast<DataRowView>().Select(drv => (long)drv[0]).ToArray();
		}
		#endregion
		#endregion
	}
}