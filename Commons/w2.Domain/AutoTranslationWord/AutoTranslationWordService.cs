/*
=========================================================================================================
  Module      : 自動翻訳ワード管理サービス (AutoTranslationWordService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.AutoTranslationWord.Helper;

namespace w2.Domain.AutoTranslationWord
{
	/// <summary>
	/// 自動翻訳ワード管理サービス
	/// </summary>
	public class AutoTranslationWordService : ServiceBase, IAutoTranslationWordService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(AutoTranslationWordListSearchCondition condition)
		{
			using (var repository = new AutoTranslationWordRepository())
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
		public AutoTranslationWordListSearchResult[] Search(AutoTranslationWordListSearchCondition condition)
		{
			using (var repository = new AutoTranslationWordRepository())
			{
				var results = repository.Search(condition);
				return results;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="wordHashKey">翻訳元ワード　ハッシュキー</param>
		/// <param name="languageCode">言語コード</param>
		/// <returns>モデル</returns>
		public AutoTranslationWordModel Get(string wordHashKey, string languageCode)
		{
			using (var repository = new AutoTranslationWordRepository())
			{
				var model = repository.Get(wordHashKey, languageCode);
				return model;
			}
		}
		#endregion

		#region +GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル配列</returns>
		public AutoTranslationWordModel[] GetAll()
		{
			using (var repository = new AutoTranslationWordRepository())
			{
				var model = repository.GetAll();
				return model;
			}
		}
		#endregion

		#region +Insert 登録(同一のword_hash_key,language_codeが存在しない場合のみ)
		/// <summary>
		/// 登録(同一のword_hash_key,language_codeが存在しない場合のみ) 
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(AutoTranslationWordModel model)
		{
			using (var repository = new AutoTranslationWordRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(AutoTranslationWordModel model)
		{
			using (var repository = new AutoTranslationWordRepository())
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +UsedUpdate 最終利用日の更新
		/// <summary>
		/// 最終利用日の更新
		/// </summary>
		/// <param name="models">モデル配列</param>
		public int UsedUpdate(AutoTranslationWordModel[] models)
		{
			using (var repository = new AutoTranslationWordRepository())
			{
				var result = repository.UsedUpdate(models);
				return result;
			}
		}
		#endregion

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="wordHashKey">翻訳前ワード ハッシュキー</param>
		/// <param name="languageCode">言語コード</param>
		public void Delete(string wordHashKey, string languageCode)
		{
			using (var repository = new AutoTranslationWordRepository())
			{
				var result = repository.Delete(wordHashKey, languageCode);
			}
		}

		#region +OldWordDelete 利用されていない古い翻訳ワードを削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="oldUsedDate">規定期間</param>
		public int OldWordDelete(DateTime oldUsedDate)
		{
			using (var repository = new AutoTranslationWordRepository())
			{
				var result = repository.OldWordDelete(oldUsedDate);
				return result;
			}
		}
		#endregion
	}
}
