/*
=========================================================================================================
  Module      :  DSK後払い基底アダプタ(BaseDskDeferredAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.DSKDeferred
{
	/// <summary>
	/// DSK後払い基底アダプタ
	/// </summary>
	public class BaseDskDeferredAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="apiSetting">API接続設定</param>
		protected BaseDskDeferredAdapter(DskDeferredApiSetting apiSetting = null)
		{
			this.ApiSetting = apiSetting;
		}

		/// <summary>API接続設定</summary>
		protected DskDeferredApiSetting ApiSetting { get; set; }
	}
}
