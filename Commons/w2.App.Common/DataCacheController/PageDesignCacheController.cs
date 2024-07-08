/*
=========================================================================================================
  Module      : ページ管理キャッシュコントローラ(PageDesignCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.PageDesign;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// ページ管理キャッシュコントローラ
	/// </summary>
	public class PageDesignCacheController : DataCacheControllerBase<PageDesignModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PageDesignCacheController() 
			: base(RefreshFileType.PageDesign)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.PageDesignService.GetAllPage();
		}
	}
}
