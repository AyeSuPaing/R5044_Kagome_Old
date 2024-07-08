/*
=========================================================================================================
  Module      : 為替レートキャッシュコントローラ (ExchangeRateCacheController.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.ExchangeRate;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 為替レートキャッシュコントローラ
	/// </summary>
	public class ExchangeRateCacheController : DataCacheControllerBase<ExchangeRateModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ExchangeRateCacheController()
			: base(RefreshFileType.ExchangeRate)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.ExchangeRateService.GetAll();
		}
	}
}