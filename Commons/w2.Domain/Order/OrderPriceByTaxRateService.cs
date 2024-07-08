/*
=========================================================================================================
  Module      : 税率毎の注文金額情報サービス (OrderPriceByTaxRateService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Common.Sql;

namespace w2.Domain.Order
{
	/// <summary>
	/// 税率毎の注文金額情報サービス
	/// </summary>
	public class OrderPriceByTaxRateService : ServiceBase, IOrderPriceByTaxRateService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="taxRate">税率</param>
		/// <returns>モデル</returns>
		public OrderPriceByTaxRateModel Get(string orderId, decimal taxRate)
		{
			using (var repository = new OrderPriceByTaxRateRepository())
			{
				var model = repository.Get(orderId, taxRate);
				return model;
			}
		}
		#endregion

		#region GetReturnPriceCorrections 返品用金額補正金額
		/// <summary>
		/// 返品用金額補正金額の取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>返品用金額補正金額</returns>
		public decimal[] GetReturnPriceCorrections(string orderId, SqlAccessor accessor)
		{
			using (var repository = new OrderPriceByTaxRateRepository(accessor))
			{
				var returnPriceCorrections = repository.GetReturnPriceCorrections(orderId);
				return returnPriceCorrections;
			}
		}
		#endregion

		#region +DeleteAllByOrderId 削除(注文に紐づいた全件削除)
		/// <summary>
		/// 削除(注文に紐づいた全件削除)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="sqlAccessor">Sqlアクセサ</param>
		public void DeleteAllByOrderId(string orderId, SqlAccessor sqlAccessor = null)
		{
			using (var repository = new OrderPriceByTaxRateRepository(sqlAccessor))
			{
				repository.DeleteAllByOrderId(orderId);
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="sqlAccessor">Sqlアクセサ</param>
		public void Insert(OrderPriceByTaxRateModel model, SqlAccessor sqlAccessor = null)
		{
			using (var repository = new OrderPriceByTaxRateRepository(sqlAccessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +GetRelatedSummaryOrderPriceByTaxRate 返品交換含めた関連注文の税率毎価格情報のサマリを取得
		/// <summary>
		/// 返品交換含めた関連注文の税率毎価格情報のサマリを取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>関連注文の税率毎価格情報のサマリ</returns>
		public List<OrderPriceByTaxRateModel> GetRelatedSummaryOrderPriceByTaxRate(string orderId)
		{
			using (var repository = new OrderPriceByTaxRateRepository())
			{
				var models = repository.GetRelatedSummaryOrderPriceByTaxRate(orderId);
				return models;
			}
		}
		#endregion

	}
}
