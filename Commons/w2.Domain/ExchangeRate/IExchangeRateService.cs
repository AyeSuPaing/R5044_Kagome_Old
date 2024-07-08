/*
=========================================================================================================
  Module      : Exchange Rate Service Interface (IExchangeRateService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.ExchangeRate
{
	/// <summary>
	/// Exchange rate service interface
	/// </summary>
	public interface IExchangeRateService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <param name="dstCurrencyCode">通貨コード（先）</param>
		/// <returns>為替レートマスタモデル</returns>
		ExchangeRateModel Get(string srcCurrencyCode, string dstCurrencyCode);

		/// <summary>
		/// 全件取得
		/// </summary>
		/// <returns>為替レートマスタ配列</returns>
		ExchangeRateModel[] GetAll();

		/// <summary>
		/// 一括削除
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		void Truncate(SqlAccessor accessor = null);

		/// <summary>
		/// 一括登録
		/// </summary>
		/// <param name="reader">データリーダー</param>
		/// <param name="accessor">SQLアクセサ</param>
		void BulkInsert(IDataReader reader, SqlAccessor accessor = null);
	}
}