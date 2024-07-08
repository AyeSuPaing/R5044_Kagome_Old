/*
=========================================================================================================
  Module      : 為替レートサービス (ExchangeRateService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;

namespace w2.Domain.ExchangeRate
{
	/// <summary>
	/// 為替レートサービス
	/// </summary>
	public class ExchangeRateService : ServiceBase, IExchangeRateService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <param name="dstCurrencyCode">通貨コード（先）</param>
		/// <returns>為替レートマスタモデル</returns>
		public ExchangeRateModel Get(string srcCurrencyCode, string dstCurrencyCode)
		{
			using (var repository = new ExchangeRateRepository())
			{
				var model = repository.Get(srcCurrencyCode, dstCurrencyCode);
				return model;
			}
		}
		#endregion

		#region +GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <returns>為替レートマスタ配列</returns>
		public ExchangeRateModel[] GetAll()
		{
			using (var repository = new ExchangeRateRepository())
			{
				var models = repository.GetAll();
				return models;
			}
		}
		#endregion

		#region +Truncate 一括削除
		/// <summary>
		/// 一括削除
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		public void Truncate(SqlAccessor accessor = null)
		{
			using (var repository = new ExchangeRateRepository(accessor))
			{
				repository.Truncate();
			}
		}
		#endregion

		#region +BulkInsert 一括登録
		/// <summary>
		/// 一括登録
		/// </summary>
		/// <param name="reader">データリーダー</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void BulkInsert(IDataReader reader, SqlAccessor accessor = null)
		{
			var bulkCopy = (accessor != null)
				? new SqlBulkCopy(accessor.Connection.GetRealConnection(), SqlBulkCopyOptions.Default, accessor.Transaction?.GetRealTransaction())
				: new SqlBulkCopy(Constants.STRING_SQL_CONNECTION);

			using (bulkCopy)
			{
				bulkCopy.DestinationTableName = Constants.TABLE_EXCHANGERATE;
				bulkCopy.WriteToServer(reader);
			}
		}
		#endregion
	}
}
