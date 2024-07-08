/*
=========================================================================================================
  Module      : 注文拡張ステータス設定サービスのインターフェース (IOrderExtendStatusSettingService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.OrderExtendStatusSetting
{
	/// <summary>
	/// 注文拡張ステータス設定サービスのインターフェース
	/// </summary>
	public interface IOrderExtendStatusSettingService : IService
	{
		/// <summary>
		/// 注文ステータス名称が登録されているデータを取得
		/// </summary>
		/// <returns>モデル配列</returns>
		OrderExtendStatusSettingModel[] GetOrderExtendStatusSetting();
	}
}
