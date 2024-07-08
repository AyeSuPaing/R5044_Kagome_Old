/*
=========================================================================================================
  Module      : アフィリエイトタグ設定サービスのインターフェース (IAffiliateTagSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.Domain.Affiliate.Helper;

namespace w2.Domain.Affiliate
{
	/// <summary>
	/// アフィリエイトタグ設定サービスのインターフェース
	/// </summary>
	public interface IAffiliateTagSettingService : IService
	{
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="affiliateProductTagId">アフィリエイト商品タグID</param>
		int AffiliateProductTagSettingDelete(int affiliateProductTagId);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="affiliateProductTagId">アフィリエイト商品タグID</param>
		/// <returns>モデル</returns>
		AffiliateProductTagSettingModel AffiliateProductTagSettingGet(int affiliateProductTagId);

		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>検索結果列</returns>
		AffiliateProductTagSettingModel[] AffiliateProductTagSettingGetAll();

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int AffiliateProductTagSettingGetSearchHitCount(AffiliateProductTagSettingListSearchCondition condition);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void AffiliateProductTagSettingInsert(AffiliateProductTagSettingModel model);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		AffiliateProductTagSettingListSearchResult[] AffiliateProductTagSettingSearch(AffiliateProductTagSettingListSearchCondition condition);

		/// <summary>
		/// キーワード検索
		/// </summary>
		/// <param name="searchWord">検索キーワード</param>
		/// <returns>検索結果列</returns>
		AffiliateTagSettingModel[] SearchByKeyword(string searchWord);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		int AffiliateProductTagSettingUpdate(AffiliateProductTagSettingModel model);

		/// <summary>
		/// 対象アフィリエイトタグIDに紐づく条件の全削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="affiliateId">アフィリエイトID</param>
		int AffiliateTagConditionDeleteAll(int affiliateId);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		/// <returns>モデル</returns>
		AffiliateTagConditionModel[] AffiliateTagConditionGetAllByAffiliateId(int affiliateId);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="models">モデル配列</param>
		void AffiliateTagConditionInsertAll(AffiliateTagConditionModel[] models);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		/// <param name="models">モデル配列</param>
		void AffiliateTagConditionUpdate(int affiliateId, AffiliateTagConditionModel[] models);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		int Delete(int affiliateId);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		/// <returns>モデル</returns>
		AffiliateTagSettingModel Get(int affiliateId);

		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル</returns>
		AffiliateTagSettingModel[] GetAllIncludeConditionModels();

		/// <summary>
		/// 出力条件管理を考慮して取得
		/// </summary>
		/// <param name="affiliateId">アフィリエイトID</param>
		/// <returns>タグ情報</returns>
		AffiliateTagSettingModel[] GetConsiderationCondition(int affiliateId);

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="tagIds">閲覧可能なタグID(配列)</param>
		/// <param name="mediaTypeIds">閲覧可能な広告媒体区分(配列)</param>
		/// <param name="locations">閲覧可能な設置個所(配列)</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(AffiliateTagSettingListSearchCondition condition, string[] tagIds, string[] mediaTypeIds, string[] locations);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		int Insert(AffiliateTagSettingModel model);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <param name="tagIds">閲覧可能なタグID(配列)</param>
		/// <param name="mediaTypeIds">閲覧可能な広告媒体区分(配列)</param>
		/// <param name="locations">閲覧可能な設置個所(配列)</param>
		/// <returns>検索結果列</returns>
		AffiliateTagSettingListSearchResult[] Search(AffiliateTagSettingListSearchCondition condition, string[] tagIds, string[] mediaTypeIds, string[] locations);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		int Update(AffiliateTagSettingModel model);
	}
}