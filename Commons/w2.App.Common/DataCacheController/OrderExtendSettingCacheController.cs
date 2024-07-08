/*
=========================================================================================================
Module      : 注文拡張項目設定キャッシュコントローラー(OrderExtendSettingCacheController.cs)
･･･････････････････････････････････････････････････････････････････････････････････････････････････････
Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.App.Common.DataCacheController.CacheData;
using w2.App.Common.RefreshFileManager;

namespace w2.App.Common.DataCacheController
{
	/// <summary>
	/// 注文拡張項目設定キャッシュコントローラー
	/// </summary>
	public class OrderExtendSettingCacheController : DataCacheControllerBase<OrderExtendSettingCacheData>
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public OrderExtendSettingCacheController()
			: base(RefreshFileType.OrderExtendSetting)
		{
		}

		/// <summary>
		/// データリフレッシュ
		/// </summary>
		internal override void RefreshCacheData()
		{
			this.CacheData = new OrderExtendSettingCacheData();
			this.CacheData.SetProperty();
		}
	}
}