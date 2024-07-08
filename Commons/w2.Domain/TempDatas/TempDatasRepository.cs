/*
=========================================================================================================
  Module      : テンポラリデータリポジトリ (TempDatasRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.TempDatas
{
	/// <summary>
	/// テンポラリデータリポジトリ
	/// </summary>
	public class TempDatasRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "TempDatas";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public TempDatasRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public TempDatasRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="tempType">テンポラリタイプ</param>
		/// <param name="tempKey">テンポラリキー</param>
		/// <returns>モデル</returns>
		public TempDatasModel Get(string tempType, string tempKey)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TEMPDATAS_TEMP_TYPE, tempType},
				{Constants.FIELD_TEMPDATAS_TEMP_KEY, tempKey},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new TempDatasModel(dv[0]);
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(TempDatasModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="tempType">テンポラリタイプ</param>
		/// <param name="tempKey">テンポラリキー</param>
		public int Delete(string tempType, string tempKey)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TEMPDATAS_TEMP_TYPE, tempType},
				{Constants.FIELD_TEMPDATAS_TEMP_KEY, tempKey},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
	}
}
