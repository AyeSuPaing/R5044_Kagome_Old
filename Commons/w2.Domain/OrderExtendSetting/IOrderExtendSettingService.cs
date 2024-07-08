/*
=========================================================================================================
  Module      : 注文拡張項目設定サービス (IOrderExtendSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Common.Sql;

namespace w2.Domain.OrderExtendSetting
{
	/// <summary>
	/// 注文拡張項目設定サービス
	/// </summary>
	public interface IOrderExtendSettingService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル</returns>
		OrderExtendSettingModel[] GetAll();

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <returns>モデル</returns>
		OrderExtendSettingModel Get(string settingId);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="models">モデル配列</param>
		int Update(IEnumerable<OrderExtendSettingModel> models);
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="models">モデル配列</param>
		/// <param name="accessor"></param>
		int Update(IEnumerable<OrderExtendSettingModel> models, SqlAccessor accessor);
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		int Update(OrderExtendSettingModel model, SqlAccessor accessor = null);
	}
}
