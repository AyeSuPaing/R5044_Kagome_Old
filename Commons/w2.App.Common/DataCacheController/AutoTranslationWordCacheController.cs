/*
=========================================================================================================
  Module      : 自動翻訳設定情報キャッシュコントローラ(AutoTranslationWordCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.AutoTranslationWord;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 自動翻訳分設定情報キャッシュコントローラ
	/// </summary>
	public class AutoTranslationWordCacheController : DataCacheControllerBase<AutoTranslationWordModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal AutoTranslationWordCacheController()
			: base(RefreshFileType.AutoTranslationWord)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.AutoTranslationWordService.GetAll();
		}

		/// <summary>
		/// 自動翻訳設定情報取得
		/// </summary>
		/// <param name="wordHashKey">ワードハッシュキー</param>
		/// <param name="languageCode">言語コード</param>
		public AutoTranslationWordModel GetAutoTranslationWordModel(string wordHashKey, string languageCode)
		{
			var model = this.CacheData.FirstOrDefault(m => (m.WordHashKey == wordHashKey) && (m.LanguageCode == languageCode));
			return model;
		}
	}
}
