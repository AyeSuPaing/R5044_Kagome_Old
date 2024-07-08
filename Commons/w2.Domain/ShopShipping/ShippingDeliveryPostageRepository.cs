/*
=========================================================================================================
  Module      : 配送料リポジトリ (ShippingDeliveryPostageRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ShopShipping
{
	/// <summary>
	/// 配送料リポジトリ
	/// </summary>
	internal class ShippingDeliveryPostageRepository : RepositoryBase
	{
		/// <summary>XMLファイル名</summary>
		private const string XML_KEY_NAME = "ShippingDeliveryPostage";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ShippingDeliveryPostageRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ShippingDeliveryPostageRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>モデル</returns>
		internal ShippingDeliveryPostageModel Get(string shopId, string shippingId, string deliveryCompanyId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHOP_ID, shopId},
				{Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_ID, shippingId},
				{Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DELIVERY_COMPANY_ID, deliveryCompanyId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			var result = (dv.Count > 0) ? new ShippingDeliveryPostageModel(dv[0]) : null;
			return result;
		}
		#endregion

		#region ~GetByShippingId 配送種別IDに紐づく全ての情報取得
		/// <summary>
		/// 配送種別IDに紐づく全ての情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>モデル配列</returns>
		internal ShippingDeliveryPostageModel[] GetByShippingId(string shopId, string shippingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHOP_ID, shopId},
				{Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_ID, shippingId},
			};
			var dv = Get(XML_KEY_NAME, "GetByShippingId", ht);
			var result = dv.Cast<DataRowView>().Select(drv => new ShippingDeliveryPostageModel(drv)).ToArray();
			return result;
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Insert(ShippingDeliveryPostageModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <returns>影響を受けた件数</returns>
		internal int Delete(string shopId, string shippingId, string deliveryCompanyId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHOP_ID, shopId},
				{Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_ID, shippingId},
				{Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_DELIVERY_COMPANY_ID, deliveryCompanyId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region ~DeleteByShippingId 配送種別IDに紐づく全ての情報削除
		/// <summary>
		/// 配送種別IDに紐づく全ての情報削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="shippingId">配送種別ID</param>
		/// <returns>影響を受けた件数</returns>
		internal int DeleteByShippingId(string shopId, string shippingId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHOP_ID, shopId},
				{Constants.FIELD_SHIPPINGDELIVERYPOSTAGE_SHIPPING_ID, shippingId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteByShippingId", ht);
			return result;
		}
		#endregion
	}
}
