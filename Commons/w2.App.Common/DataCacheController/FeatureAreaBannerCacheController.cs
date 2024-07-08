/*
=========================================================================================================
  Module      : 特集エリアバナーキャッシュコントローラ(FeatureAreaBannerCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.FeatureArea;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 特集エリアバナーキャッシュコントローラ
	/// </summary>
	public class FeatureAreaBannerCacheController : DataCacheControllerBase<FeatureAreaModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public FeatureAreaBannerCacheController()
			: base(RefreshFileType.FeatureAreaBanner)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.FeatureAreaService.GetFeatureAreaAll();
		}
	}
}
