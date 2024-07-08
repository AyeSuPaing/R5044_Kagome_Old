/*
=========================================================================================================
  Module      : ＆mall在庫引当リポジトリ (AndmallInventoryReserveRepository.cs)
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

namespace w2.Domain.AndmallInventoryReserve
{
	/// <summary>
	/// ＆mall在庫引当リポジトリ
	/// </summary>
	internal class AndmallInventoryReserveRepository : RepositoryBase
	{
		/// <summary>影響を受けた件数</summary>
		private const string XML_KEY_NAME = "AndmallInventoryReserve";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal AndmallInventoryReserveRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal AndmallInventoryReserveRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="identificationCode">識別コード</param>
		/// <param name="skuId">SKUコード</param>
		/// <param name="andmallBaseStoreCode">ショップコード</param>
		/// <returns>モデル</returns>
		internal AndmallInventoryReserveModel Get(string identificationCode, string skuId, string andmallBaseStoreCode)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ANDMALLINVENTORYRESERVE_IDENTIFICATION_CODE, identificationCode},
				{Constants.FIELD_ANDMALLINVENTORYRESERVE_SKU_ID, skuId},
				{Constants.FIELD_ANDMALLINVENTORYRESERVE_ANDMALL_BASE_STORE_CODE, andmallBaseStoreCode},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new AndmallInventoryReserveModel(dv[0]);
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(AndmallInventoryReserveModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(AndmallInventoryReserveModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion
	}
}
