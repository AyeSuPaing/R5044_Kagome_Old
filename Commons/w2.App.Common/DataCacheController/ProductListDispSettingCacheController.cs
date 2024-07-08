/*
=========================================================================================================
  Module      : 商品一覧設定キャッシュコントローラ(ProductListDispSettingCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.ProductListDispSetting;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 商品一覧設定キャッシュコントローラ
	/// </summary>
	public class ProductListDispSettingCacheController : DataCacheControllerBase<ProductListDispSettingModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ProductListDispSettingCacheController()
			: base(RefreshFileType.ProductListDispSetting)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.ProductListDispSettingService.GetUsable();
		}
	}
}
