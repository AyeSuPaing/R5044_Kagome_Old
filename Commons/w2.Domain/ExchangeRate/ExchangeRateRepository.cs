/*
=========================================================================================================
  Module      : 為替レートリポジトリ (ExchangeRateRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.ExchangeRate
{
	/// <summary>
	/// 為替レートリポジトリ
	/// </summary>
	internal class ExchangeRateRepository : RepositoryBase
	{
		/// <summary>XMLキー名</summary>
		private const string XML_KEY_NAME = "ExchangeRate";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ExchangeRateRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ExchangeRateRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="srcCurrencyCode">通貨コード（元）</param>
		/// <param name="dstCurrencyCode">通貨コード（先）</param>
		/// <returns>為替レートマスタモデ</returns>
		internal ExchangeRateModel Get(string srcCurrencyCode, string dstCurrencyCode)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_EXCHANGERATE_SRC_CURRENCY_CODE, srcCurrencyCode},
				{Constants.FIELD_EXCHANGERATE_DST_CURRENCY_CODE, dstCurrencyCode},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new ExchangeRateModel(dv[0]);
		}
		#endregion

		#region ~GetAll 全取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <returns>為替レートマスタモデル配列</returns>
		internal ExchangeRateModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll", null);
			return dv.Cast<DataRowView>().Select(drv => new ExchangeRateModel(drv)).ToArray();
		}
		#endregion

		#region ~Truncate 一括削除
		/// <summary>
		/// 一括削除
		/// </summary>
		internal void Truncate()
		{
			Exec(XML_KEY_NAME, "Truncate");
		}
		#endregion
	}
}
