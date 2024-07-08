/*
=========================================================================================================
  Module      : 税率毎の注文金額情報サービスのインタフェース (IOrderPriceByTaxRateService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.Common.Sql;

namespace w2.Domain.Order
{
	/// <summary>
	/// 税率毎の注文金額情報サービスのインタフェース
	/// </summary>
	public interface IOrderPriceByTaxRateService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="taxRate">税率</param>
		/// <returns>モデル</returns>
		OrderPriceByTaxRateModel Get(string orderId, decimal taxRate);

		/// <summary>
		/// 削除(注文に紐づいた全件削除)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="sqlAccessor">Sqlアクセサ</param>
		void DeleteAllByOrderId(string orderId, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="sqlAccessor">Sqlアクセサ</param>
		void Insert(OrderPriceByTaxRateModel model, SqlAccessor sqlAccessor = null);

		/// <summary>
		/// 返品交換含めた関連注文の税率毎価格情報のサマリを取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>関連注文の税率毎価格情報のサマリ</returns>
		List<OrderPriceByTaxRateModel> GetRelatedSummaryOrderPriceByTaxRate(string orderId);
	}
}