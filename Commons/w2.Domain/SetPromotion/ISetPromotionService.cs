/*
=========================================================================================================
  Module      : セットプロモーションサービスのインターフェース (ISetPromotionService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.SetPromotion.Helper;

namespace w2.Domain.SetPromotion
{
	/// <summary>
	/// セットプロモーションサービスのインターフェース
	/// </summary>
	public interface ISetPromotionService : IService
	{
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="setpromotionId">セットプロモーションID</param>
		void Delete(string setpromotionId);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="setpromotionId">セットプロモーションID</param>
		/// <returns>モデル</returns>
		SetPromotionModel Get(string setpromotionId);

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(SetPromotionListSearchCondition condition);

		/// <summary>
		/// 利用可能なものを取得
		/// </summary>
		/// <returns>モデル配列</returns>
		SetPromotionModel[] GetUsable();

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(SetPromotionModel model);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		SetPromotionListSearchResult[] Search(SetPromotionListSearchCondition condition);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		void Update(SetPromotionModel model);
	}
}