/*
=========================================================================================================
  Module      : リアル店舗商品在庫リポジトリ (RealShopProductStockRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.RealShopProductStock
{
	/// <summary>
	/// リアル店舗商品在庫リポジトリ
	/// </summary>
	public class RealShopProductStockRepository : RepositoryBase
	{
		/// <returns>ユーザーSQLファイル</returns>
		private const string XML_KEY_NAME = "RealShopProductStock";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public RealShopProductStockRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public RealShopProductStockRepository(SqlAccessor accessor)
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
			var reader = GetWithReader(XML_KEY_NAME, "GetRealShopProductStockMaster", input, replaces);
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
			var dv = Get(XML_KEY_NAME, "GetRealShopProductStockMaster", input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckRealShopProductStockFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckRealShopProductStockFields", input, replaces: replaces);
		}
		#endregion
	}
}