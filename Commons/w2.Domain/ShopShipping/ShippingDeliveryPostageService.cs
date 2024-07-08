/*
=========================================================================================================
  Module      : 配送料サービス (ShippingDeliveryPostageService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 配送料サービス
	/// </summary>
	public class ShippingDeliveryPostageService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>モデル</returns>
		public ShippingDeliveryPostageModel Get(string shopId, string shippingId, string deliveryCompanyId)
		{
			using (var repository = new ShippingDeliveryPostageRepository())
			{
				var model = repository.Get(shopId, shippingId, deliveryCompanyId);
				return model;
			}
		}
		#endregion

		#region +GetByShippingId 配送種別IDに紐づく全ての情報取得
		/// <summary>
		/// 配送種別IDに紐づく全ての情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>モデル配列</returns>
		public ShippingDeliveryPostageModel[] GetByShippingId(string shopId, string shippingId)
		{
			using (var repository = new ShippingDeliveryPostageRepository())
			{
				var model = repository.GetByShippingId(shopId, shippingId);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Insert(ShippingDeliveryPostageModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ShippingDeliveryPostageRepository(accessor))
			{
				var result = repository.Insert(model);
				return result;
			}
		}
		#endregion

		#region +Inserts 登録（配送種別に紐づく全ての情報）
		/// <summary>
		/// 登録（配送種別に紐づく全ての情報）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="models">モデル配列</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Inserts(
			string shopId,
			string shippingId,
			ShippingDeliveryPostageModel[] models,
			SqlAccessor accessor = null)
		{
			using (var repository = new ShippingDeliveryPostageRepository(accessor))
			{
				// 一旦全部消す
				repository.DeleteByShippingId(shopId, shippingId);

				// 登録
				var result = 0;
				foreach (var model in models)
				{
					result += repository.Insert(model);
				}
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(string shopId, string shippingId, string deliveryCompanyId, SqlAccessor accessor = null)
		{
			using (var repository = new ShippingDeliveryPostageRepository(accessor))
			{
				var result = repository.Delete(shopId, shippingId, deliveryCompanyId);
				return result;
			}
		}
		#endregion

		#region +DeleteByShippingId 配送種別IDに紐づく全ての情報削除
		/// <summary>
		/// 配送種別IDに紐づく全ての情報削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int DeleteByShippingId(string shopId, string shippingId, SqlAccessor accessor = null)
		{
			using (var repository = new ShippingDeliveryPostageRepository(accessor))
			{
				var result = repository.DeleteByShippingId(shopId, shippingId);
				return result;
			}
		}
		#endregion
	}
}
