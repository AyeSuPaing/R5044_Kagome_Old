/*
=========================================================================================================
  Module      : 注文拡張ステータス設定サービス (OrderExtendStatusSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
namespace w2.Domain.OrderExtendStatusSetting
{
	/// <summary>
	/// 注文拡張ステータス設定サービス
	/// </summary>
	public class OrderExtendStatusSettingService : ServiceBase, IOrderExtendStatusSettingService
	{
		#region +GetOrderExtendStatusSetting 取得
		/// <summary>
		/// 注文ステータス名称が登録されているデータを取得
		/// </summary>
		/// <returns>モデル配列</returns>
		public OrderExtendStatusSettingModel[] GetOrderExtendStatusSetting()
		{
			using (var repository = new OrderExtendStatusSettingRepository())
			{
				var model = repository.GetOrderExtendStatusSetting();
				return model;
			}
		}
		#endregion
	}
}