/*
=========================================================================================================
  Module      : ショートURLサービスのインターフェース (IShortUrlService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using w2.Common.Sql;
using w2.Domain.ShortUrl.Helper;

namespace w2.Domain.ShortUrl
{
	/// <summary>
	/// ショートURLサービスのインターフェース
	/// </summary>
	public interface IShortUrlService : IService
	{
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="surlNo">ショートURL NO</param>
		void Delete(long surlNo);

		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		ShortUrlModel[] GetAll(string shopId);

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(ShortUrlListSearchCondition condition);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(ShortUrlModel model);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		ShortUrlListSearchResult[] Search(ShortUrlListSearchCondition condition);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="models">モデル</param>
		/// <returns>影響を受けた件数</returns>
		int Update(IEnumerable<ShortUrlModel> models);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Update(ShortUrlModel model, SqlAccessor accessor);
	}
}