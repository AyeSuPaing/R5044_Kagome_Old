/*
=========================================================================================================
  Module      : Criteo連携設定(CriteoIntegrationSetting.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Commerce.Batch.ExportCriteoDatas.Proc
{
	/// <summary>
	/// Criteo連携設定
	/// </summary>
	public class CriteoIntegrationSetting
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="setting">連携用サイト設定</param>
		public CriteoIntegrationSetting(CriteoSiteSetting setting)
		{
			this.setting = setting;
		}

		/// <summary>設定情報</summary>
		public CriteoSiteSetting setting { get; private set; }
	}
}
