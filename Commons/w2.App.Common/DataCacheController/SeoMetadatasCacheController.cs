/*
=========================================================================================================
  Module      : SEOメタデータキャッシュコントローラ(SeoMetadatasCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.SeoMetadatas;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// SEOメタデータキャッシュコントローラ
	/// </summary>
	public class SeoMetadatasCacheController : DataCacheControllerBase<SeoMetadatasModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal SeoMetadatasCacheController()
			: base(RefreshFileType.SeoMetadatas)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.SeoMetadatasService.GetAll();
		}
	}
}
