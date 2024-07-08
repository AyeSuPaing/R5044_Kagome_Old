/*
=========================================================================================================
  Module      : 定期商品変更設定インターフェース (IFixedPurchaseProductChangeSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.FixedPurchaseProductChangeSetting.Helper;

namespace w2.Domain.FixedPurchaseProductChangeSetting
{
	/// <summary>
	/// 定期商品変更設定インターフェース
	/// </summary>
	public interface IFixedPurchaseProductChangeSettingService : IService
	{
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(FixedPurchaseProductChangeSettingListSearchCondition condition);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		FixedPurchaseProductChangeSettingListSearchResult[] Search(FixedPurchaseProductChangeSettingListSearchCondition condition);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>定期商品変更設定モデル</returns>
		FixedPurchaseProductChangeSettingModel Get(string fixedPurchaseProductChangeId, SqlAccessor accessor = null);

		/// <summary>
		/// 取得：商品ID
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>定期商品変更設定モデル</returns>
		FixedPurchaseProductChangeSettingModel GetByProductId(string productId, string variationId, string shopId, SqlAccessor accessor = null);

		/// <summary>
		/// 表示用定期商品変更設定取得
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更ID</param>
		/// <returns>定期商品変更設定モデル</returns>
		FixedPurchaseProductChangeSettingContainer GetContainer(string fixedPurchaseProductChangeId);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">定期商品変更設定モデル</param>
		void Insert(FixedPurchaseProductChangeSettingModel model);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">定期商品変更設定モデル</param>
		void Update(FixedPurchaseProductChangeSettingModel model);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="fixedPurchaseProductChangeSettingId">定期商品変更設定ID</param>
		void Delete(string fixedPurchaseProductChangeSettingId);

		/// <summary>
		/// 変更後商品取得
		/// </summary>
		/// <param name="fixedPurchaseProductChangeId">定期商品変更設定ID</param>
		/// <returns></returns>
		FixedPurchaseAfterChangeItemModel[] GetAfterChangeItems(string fixedPurchaseProductChangeId);
	}
}
