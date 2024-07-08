/*
=========================================================================================================
  Module      : 税率毎の注文金額情報リポジトリ (OrderPriceByTaxRateRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.Domain.Order
{
	/// <summary>
	/// 税率毎の注文金額情報リポジトリ
	/// </summary>
	internal class OrderPriceByTaxRateRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "OrderPriceByTaxRate";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal OrderPriceByTaxRateRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal OrderPriceByTaxRateRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="taxRate">税率</param>
		/// <returns>モデル</returns>
		internal OrderPriceByTaxRateModel Get(string orderId, decimal taxRate)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERPRICEBYTAXRATE_ORDER_ID, orderId},
				{Constants.FIELD_ORDERPRICEBYTAXRATE_KEY_TAX_RATE, taxRate},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new OrderPriceByTaxRateModel(dv[0]);
		}
		#endregion

		#region GetReturnPriceCorrections 返品用金額補正の取得
		/// <summary>
		/// 返品用金額補正の取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>返品用金額補正金額</returns>
		internal decimal[] GetReturnPriceCorrections(string orderId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ORDERPRICEBYTAXRATE_ORDER_ID, orderId }
			};
			var dv = Get(XML_KEY_NAME, "GetReturnPriceCorrections", ht);
			var reslut = dv.Cast<DataRowView>().Select(drv => (decimal)drv[0]).ToArray();
			return reslut;
		}
		#endregion

		#region ~DeleteAllByOrderId 削除(注文に紐づいた全件削除)
		/// <summary>
		/// 削除(注文に紐づいた全件)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		internal void DeleteAllByOrderId(string orderId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ORDERPRICEBYTAXRATE_ORDER_ID, orderId},
			};
			Exec(XML_KEY_NAME, "DeleteAllByOrderId", ht);
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(OrderPriceByTaxRateModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~GetRelatedSummaryOrderPriceByTaxRate 関連する税率毎価格情報のサマリを取得
		/// <summary>
		/// 関連する税率毎価格情報のサマリを取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>税率毎価格情報リスト</returns>
		internal List<OrderPriceByTaxRateModel> GetRelatedSummaryOrderPriceByTaxRate(string orderId)
		{
			var param = new Hashtable
			{
				{ Constants.FIELD_ORDER_ORDER_ID, orderId },
			};

			var dv = Get(XML_KEY_NAME, "GetRelatedSummaryOrderPriceByTaxRate", param);
			var result = dv.Cast<DataRowView>().Select(drv => new OrderPriceByTaxRateModel(drv)).ToList();
			return result;
		}
		#endregion
	}
}
