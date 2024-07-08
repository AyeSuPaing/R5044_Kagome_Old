/*
=========================================================================================================
  Module      : パーツ管理キャッシュコントローラ(PartsDesignCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.PartsDesign;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// パーツ管理キャッシュコントローラ
	/// </summary>
	public class PartsDesignCacheController : DataCacheControllerBase<PartsDesignModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PartsDesignCacheController() 
			: base(RefreshFileType.PartsDesign)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.PartsDesignService.GetAllParts();
		}
	}
}
