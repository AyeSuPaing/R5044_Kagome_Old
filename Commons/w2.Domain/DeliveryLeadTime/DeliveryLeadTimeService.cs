/*
=========================================================================================================
  Module      : 配送リードタイムサービス (DeliveryLeadTimeService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Common.Sql;

namespace w2.Domain.DeliveryLeadTime
{
	/// <summary>
	/// 配送リードタイムサービス
	/// </summary>
	public class DeliveryLeadTimeService : ServiceBase
	{
		#region +Get All
		/// <summary>
		/// Get All
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>モデル</returns>
		public DeliveryLeadTimeModel[] GetAll(string shopId, string deliveryCompanyId)
		{
			using (var repository = new DeliveryLeadTimeRepository())
			{
				return repository.GetAll(shopId, deliveryCompanyId);
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">Sql accessor</param>
		public void Insert(DeliveryLeadTimeModel model, SqlAccessor accessor = null)
		{
			using (var repository = new DeliveryLeadTimeRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="accessor">Sql accessor</param>
		public void Delete(string shopId, string deliveryCompanyId, SqlAccessor accessor = null)
		{
			using (var repository = new DeliveryLeadTimeRepository(accessor))
			{
				repository.Delete(shopId, deliveryCompanyId);
			}
		}
		#endregion

		#region Update Delivery Lead Time Zone
		/// <summary>
		/// Update Delivery Lead Time Zone
		/// </summary>
		/// <param name="deliveryLeadTimes">List model delivery lead times</param>
		/// <param name="accessor">Sql accessor</param>
		public void UpdateDeliveryLeadTimeZone(List<DeliveryLeadTimeModel> deliveryLeadTimes, SqlAccessor accessor = null)
		{
			using (var repository = new DeliveryLeadTimeRepository(accessor))
			{
				// DELETE>INSERT
				repository.Delete(deliveryLeadTimes[0].ShopId, deliveryLeadTimes[0].DeliveryCompanyId);

				foreach (var deliveryLeadTime in deliveryLeadTimes)
				{
					repository.Insert(deliveryLeadTime);
				}
			}
		}
		#endregion
	}
}