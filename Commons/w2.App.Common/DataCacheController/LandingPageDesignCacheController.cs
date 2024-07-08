/*
=========================================================================================================
  Module      : ランディングページデザイン情報キャッシュコントローラ(LandingPageDesignCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.LandingPage;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// ランディングページデザイン情報キャッシュコントローラ
	/// </summary>
	public class LandingPageDesignCacheController : DataCacheControllerBase<LandingPageDesignModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LandingPageDesignCacheController()
			: base(RefreshFileType.LandingPageDesign)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.LandingPageService.GetAllPage();
		}
	}
}