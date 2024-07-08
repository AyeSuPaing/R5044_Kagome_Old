/*
=========================================================================================================
  Module      : モール連携設定情報キャッシュコントローラ(MallCooperationSettingCacheController.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.RefreshFileManager;
using w2.Domain;
using w2.Domain.MallCooperationSetting;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// モール連携設定情報キャッシュコントローラ
	/// </summary>
	public class MallCooperationSettingCacheController : DataCacheControllerBase<MallCooperationSettingModel[]>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal MallCooperationSettingCacheController() : base(RefreshFileType.MallCooperationSetting)
		{
		}

		/// <summary>
		/// キャッシュデータリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = DomainFacade.Instance.MallCooperationSettingService.GetAll(Constants.CONST_DEFAULT_SHOP_ID).ToArray();
		}

		/// <summary>
		/// モール連携設定情報取得
		/// </summary>
		/// <param name="mallId">モールID</param>
		public MallCooperationSettingModel GetMallCooperationSetting(string mallId)
		{
			var mallCooperationSetting = this.CacheData.Where(m => m.MallId == mallId);
			return mallCooperationSetting.FirstOrDefault();
		}
	}
}