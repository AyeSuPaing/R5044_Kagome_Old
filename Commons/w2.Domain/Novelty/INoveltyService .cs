/*
=========================================================================================================
  Module      : ノベルティ設定サービスのインターフェース (INoveltyService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;

namespace w2.Domain.Novelty
{
	/// <summary>
	/// ノベルティ設定サービスのインターフェース
	/// </summary>
	public interface INoveltyService : IService
	{
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(Hashtable param);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="param">検索パラメタ</param>
		/// <returns>モデル列</returns>
		NoveltyModel[] Search(Hashtable param);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="noveltyId">ノベルティID</param>
		/// <returns>モデル</returns>
		NoveltyModel Get(string shopId, string noveltyId);

		/// <summary>
		/// 全て取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル列</returns>
		NoveltyModel[] GetAll(string shopId);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(NoveltyModel model);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		void Update(NoveltyModel model);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="noveltyId">ノベルティID</param>
		void Delete(string shopId, string noveltyId);
	}
}
