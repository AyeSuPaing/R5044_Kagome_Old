/*
=========================================================================================================
  Module      : 自動翻訳ワード管理リポジトリ (AutoTranslationWordRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.AutoTranslationWord.Helper;

namespace w2.Domain.AutoTranslationWord
{
	/// <summary>
	/// 自動翻訳ワード管理リポジトリ
	/// </summary>
	internal class AutoTranslationWordRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "AutoTranslationWord";
		/// <summary>利用時間更新対象の検索条件</summary>
		private const string USED_WORDS = "@@ used_words @@";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal AutoTranslationWordRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal AutoTranslationWordRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(AutoTranslationWordListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal AutoTranslationWordListSearchResult[] Search(AutoTranslationWordListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new AutoTranslationWordListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetAll 全取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル配列</returns>
		internal AutoTranslationWordModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll", null);
			return dv.Cast<DataRowView>().Select(drv => new AutoTranslationWordModel(drv)).ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="wordHashKey">翻訳元ワード　ハッシュキー</param>
		/// <param name="languageCode">言語コード</param>
		/// <returns>モデル</returns>
		internal AutoTranslationWordModel Get(string wordHashKey, string languageCode)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_AUTOTRANSLATIONWORD_WORD_HASH_KEY, wordHashKey},
				{Constants.FIELD_AUTOTRANSLATIONWORD_LANGUAGE_CODE, languageCode},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new AutoTranslationWordModel(dv[0]);
		}
		#endregion

		#region ~Insert 登録(同一のword_hash_key,language_codeが存在しない場合のみ)
		/// <summary>
		/// 登録(同一のword_hash_key,language_codeが存在しない場合のみ) 
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(AutoTranslationWordModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(AutoTranslationWordModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~UsedUpdate 最終利用日の更新
		/// <summary>
		/// 最終利用日の更新
		/// </summary>
		/// <param name="models">モデル配列</param>
		/// <returns>影響を受けた件数</returns>
		internal int UsedUpdate(AutoTranslationWordModel[] models)
		{
			var words = (models.Count() == 0) ? "''" : string.Join(",", models.Select(m => string.Format("'{0}'", (m.WordHashKey + m.LanguageCode))));
			var result = Exec(XML_KEY_NAME, "UsedUpdate", null, new KeyValuePair<string, string>(USED_WORDS, words));
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="wordHashKey">翻訳前ワード ハッシュキー</param>
		/// <param name="languageCode">言語コード</param>
		internal int Delete(string wordHashKey, string languageCode)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_AUTOTRANSLATIONWORD_WORD_HASH_KEY, wordHashKey},
				{Constants.FIELD_AUTOTRANSLATIONWORD_LANGUAGE_CODE, languageCode},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region ~OldWordDelete 利用されていない古い翻訳ワードを削除
		/// <summary>
		/// 利用されていない古い翻訳ワードを削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="oldUsedDate">規定期間</param>
		internal int OldWordDelete(DateTime oldUsedDate)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_AUTOTRANSLATIONWORD_DATE_USED + "_line", oldUsedDate},
				{Constants.FIELD_AUTOTRANSLATIONWORD_CLEAR_FLG, Constants.FLG_AUTOTRANSLATIONWORD_CLEAR_FLG_ON},
			};
			var result = Exec(XML_KEY_NAME, "OldWordDelete", ht);
			return result;
		}
		#endregion
	}
}
