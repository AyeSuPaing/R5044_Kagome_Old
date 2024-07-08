/*
=========================================================================================================
  Module      : 自動翻訳ワード管理サービスのインターフェース (IAutoTranslationWordService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.AutoTranslationWord.Helper;

namespace w2.Domain.AutoTranslationWord
{
	/// <summary>
	/// 自動翻訳ワード管理サービスのインターフェース
	/// </summary>
	public interface IAutoTranslationWordService : IService
	{
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="wordHashKey">翻訳前ワード ハッシュキー</param>
		/// <param name="languageCode">言語コード</param>
		void Delete(string wordHashKey, string languageCode);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="wordHashKey">翻訳元ワード　ハッシュキー</param>
		/// <param name="languageCode">言語コード</param>
		/// <returns>モデル</returns>
		AutoTranslationWordModel Get(string wordHashKey, string languageCode);

		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		AutoTranslationWordModel[] GetAll();

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		int GetSearchHitCount(AutoTranslationWordListSearchCondition condition);

		/// <summary>
		/// 登録(同一のword_hash_key,language_codeが存在しない場合のみ) 
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(AutoTranslationWordModel model);

		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="oldUsedDate">規定期間</param>
		int OldWordDelete(DateTime oldUsedDate);

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		AutoTranslationWordListSearchResult[] Search(AutoTranslationWordListSearchCondition condition);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		int Update(AutoTranslationWordModel model);

		/// <summary>
		/// 最終利用日の更新
		/// </summary>
		/// <param name="models">モデル配列</param>
		int UsedUpdate(AutoTranslationWordModel[] models);
	}
}