/*
=========================================================================================================
Module      : OGPタグ設定キャッシュコントローラー(OgpTagSettingCacheController.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.OgpTagSetting;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// OGPタグ設定キャッシュコントローラー
	/// </summary>
	public class OgpTagSettingCacheController : DataCacheControllerBase<OgpTagSettingModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OgpTagSettingCacheController()
			: base(RefreshFileType.OgpTagSetting)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.OgpTagSettingService.GetAll();
		}
	}
}
