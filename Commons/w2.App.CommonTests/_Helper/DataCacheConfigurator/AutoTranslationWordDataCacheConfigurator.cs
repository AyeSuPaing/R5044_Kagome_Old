using System;
using Moq;
using w2.App.Common.DataCacheController;
using w2.Domain.AutoTranslationWord;
using w2.Domain.MemberRank;

namespace w2.App.CommonTests._Helper.DataCacheConfigurator
{
	/// <summary>
	/// 自動翻訳分設定データキャッシュ設定クラス
	/// </summary>
	internal class AutoTranslationWordDataCacheConfigurator : IDisposable
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal AutoTranslationWordDataCacheConfigurator(AutoTranslationWordModel[] data)
		{
			RefreshDataCache(data);
		}

		/// <summary>
		/// 自動翻訳分設定データキャッシュリフレッシュ
		/// </summary>
		/// <param name="data">自動翻訳分設定データ</param>
		private void RefreshDataCache(AutoTranslationWordModel[] data)
		{
			var autoTranslationWordServiceMock = new Mock<IAutoTranslationWordService>();
			autoTranslationWordServiceMock.Setup(s => s.GetAll()).Returns(data);
			Domain.DomainFacade.Instance.AutoTranslationWordService = autoTranslationWordServiceMock.Object;

			DataCacheControllerFacade.GetAutoTranslationWordCacheController().RefreshCacheData();
		}

		/// <summary>
		/// Dispose(キャッシュデータを空でリフレッシュ)
		/// </summary>
		public void Dispose()
		{
			RefreshDataCache(new AutoTranslationWordModel[0]);
		}
	}
}