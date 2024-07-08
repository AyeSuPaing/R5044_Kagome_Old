/*
=========================================================================================================
  Module      : 注文拡張項目設定サービス (OrderExtendSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.OrderExtendSetting
{
	/// <summary>
	/// 注文拡張項目設定サービス
	/// </summary>
	public class OrderExtendSettingService : ServiceBase, IOrderExtendSettingService
	{
		#region +GetAll 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル</returns>
		public OrderExtendSettingModel[] GetAll()
		{
			using (var repository = new OrderExtendSettingRepository())
			{
				var model = repository.GetAll();
				return model;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="settingId">ユーザ拡張項目ID</param>
		/// <returns>モデル</returns>
		public OrderExtendSettingModel Get(string settingId)
		{
			using (var repository = new OrderExtendSettingRepository())
			{
				var model = repository.Get(settingId);
				return model;
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="models">モデル配列</param>
		public int Update(IEnumerable<OrderExtendSettingModel> models)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();
				var updated = Update(models, accessor);
				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="models">モデル配列</param>
		/// <param name="accessor"></param>
		public int Update(IEnumerable<OrderExtendSettingModel> models, SqlAccessor accessor)
		{
			var updated = models.Sum(model => Update(model, accessor));
			return updated;
		}
		#endregion
		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(OrderExtendSettingModel model, SqlAccessor accessor = null)
		{
			using (var repository = new OrderExtendSettingRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion
	}
}
