/*
=========================================================================================================
  Module      : ＆mall在庫引当サービス (AndmallInventoryReserveService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Transactions;
using w2.Common.Sql;

namespace w2.Domain.AndmallInventoryReserve
{
	/// <summary>
	/// ＆mall在庫引当サービス
	/// </summary>
	public class AndmallInventoryReserveService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="identificationCode">識別コード</param>
		/// <param name="skuId">SKUコード</param>
		/// <param name="andmallBaseStoreCode">ショップコード</param>
		/// <param name="accessor">SQLアクセサー</param>
		/// <returns>モデル</returns>
		public AndmallInventoryReserveModel Get(string identificationCode, string skuId, string andmallBaseStoreCode, SqlAccessor accessor = null)
		{
			using (var repository = new AndmallInventoryReserveRepository(accessor))
			{
				var model = repository.Get(identificationCode, skuId, andmallBaseStoreCode);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(AndmallInventoryReserveModel model)
		{
			using (var repository = new AndmallInventoryReserveRepository())
			{
				model.DateChanged = DateTime.Now;
				repository.Insert(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(AndmallInventoryReserveModel model)
		{
			using (var repository = new AndmallInventoryReserveRepository())
			{
				model.DateChanged = DateTime.Now;
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion
	}
}
