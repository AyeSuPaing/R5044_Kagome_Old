/*
=========================================================================================================
  Module      : モール出品設定リポジトリ (MallExhibitsConfigRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.MallExhibitsConfig
{
	/// <summary>
	/// モール出品設定リポジトリ
	/// </summary>
	public class MallExhibitsConfigRepository : RepositoryBase
	{
		/// <returns>ユーザーSQLファイル</returns>
		private const string XML_KEY_NAME = "MallExhibitsConfig";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MallExhibitsConfigRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MallExhibitsConfigRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, "GetMallExhibitsConfigMaster", input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, "GetMallExhibitsConfigMaster", input, replaces: replaces);
			return dv;
		}
		#endregion

		#region +IsExist
		/// <summary>
		/// Is exist
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <returns>True if mall exhibits config is existed, otherwise false</returns>
		internal bool IsExist(string shopId, string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_PRODUCTVARIATION_SHOP_ID, shopId },
				{ Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID, productId },
			};

			var result = Get(XML_KEY_NAME, "IsExist", input);
			return ((int)result[0][0] > 0);
		}
		#endregion

		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <returns>Number of rows affected</returns>
		internal int Insert(MallExhibitsConfigModel model)
		{
			var result = Exec(XML_KEY_NAME, "Insert", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete
		/// <summary>
		/// Delete
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <returns>Number of rows affected</returns>
		internal int Delete(string shopId, string productId)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_MALLEXHIBITSCONFIG_SHOP_ID, shopId },
				{ Constants.FIELD_MALLEXHIBITSCONFIG_PRODUCT_ID, productId },
			};

			var result = Exec(XML_KEY_NAME, "Delete", input);
			return result;
		}
		#endregion
	}
}