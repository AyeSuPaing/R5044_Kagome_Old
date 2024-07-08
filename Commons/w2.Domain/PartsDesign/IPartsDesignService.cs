/*
=========================================================================================================
  Module      : パーツデザイン パーツ管理サービスのインターフェース (IPartsDesignService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.Common.Sql;
using w2.Domain.PartsDesign.Helper;

namespace w2.Domain.PartsDesign
{
	/// <summary>
	/// パーツデザイン パーツ管理サービスのインターフェース
	/// </summary>
	public interface IPartsDesignService : IService
	{
		/// <summary>
		/// グループ削除
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		int DeleteGroup(long groupId, string lastChanged, SqlAccessor accessor = null);

		/// <summary>
		/// パーツ削除
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int DeleteParts(long partsId, SqlAccessor accessor = null);

		/// <summary>
		/// グループ全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		PartsDesignGroupModel[] GetAllGroup();
		/// <summary>
		/// パーツ 全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		PartsDesignModel[] GetAllParts();

		/// <summary>
		/// グループ取得
		/// </summary>
		/// <param name="groupId">グループID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		PartsDesignGroupModel GetGroup(long groupId, SqlAccessor accessor = null);

		/// <summary>
		/// パーツ取得
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		PartsDesignModel GetParts(long partsId, SqlAccessor accessor = null);

		/// <summary>
		/// パーツ取得
		/// </summary>
		/// <param name="areaId">特集エリアID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		PartsDesignModel GetPartsByAreaId(string areaId, SqlAccessor accessor = null);

		/// <summary>
		/// ファイル名でパーツ取得
		/// </summary>
		/// <param name="fileName">ファイル名</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		PartsDesignModel GetPartsByFileName(string fileName, SqlAccessor accessor = null);

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(PartsDesignListSearch condition);

		/// <summary>
		/// グループ登録
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>新規グループID</returns>
		int InsertGroup(PartsDesignGroupModel model, SqlAccessor accessor = null);

		/// <summary>
		/// パーツ登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>新規パーツID</returns>
		int InsertParts(PartsDesignModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 存在しないグループと紐づいているページのグループIDを抽出
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		long[] NotExistGroupIds(SqlAccessor accessor = null);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		PartsDesignListSearchResult[] Search(PartsDesignListSearch condition);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="otherPageGroupModel">その他 グループモデル</param>
		/// <returns>検索結果列</returns>
		PartsDesignListSearchGroupResult[] SearchGroup(PartsDesignListSearch condition, PartsDesignGroupModel otherPageGroupModel);

		/// <summary>
		/// グループ更新
		/// </summary>
		/// <param name="model">グループモデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int UpdateGroup(PartsDesignGroupModel model, SqlAccessor accessor = null);

		/// <summary>
		/// グループ順序 更新
		/// </summary>
		/// <param name="groupIds">グループ順序</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		void UpdateGroupSort(long[] groupIds, SqlAccessor accessor = null);

		/// <summary>
		///  管理用タイトル更新
		/// </summary>
		/// <param name="partsId">パーツID</param>
		/// <param name="title">管理用タイトル</param>
		void UpdateManagementTitle(long partsId, string title);

		/// <summary>
		/// パーツ更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int UpdateParts(PartsDesignModel model, SqlAccessor accessor = null);

		/// <summary>
		/// パーツをその他グループに移動
		/// </summary>
		/// <param name="groupId">変更対象のグループID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdatePartsMoveOtherGroup(long groupId, string lastChanged, SqlAccessor accessor = null);

		/// <summary>
		/// パーツ順序 更新
		/// </summary>
		/// <param name="models">モデル配列</param>
		/// <param name="accessor">SQLアクセサ</param>
		void UpdatePartsSort(PartsDesignModel[] models, SqlAccessor accessor = null);
	}
}