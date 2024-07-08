/*
=========================================================================================================
  Module      : メッセージリポジトリ(CsMessageRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.CsMessage
{
	/// <summary>
	/// メッセージリポジトリ
	/// </summary>
	public class CsMessageRepository : RepositoryBase
	{
		private const string XML_KEY_NAME = "CsMessage";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public CsMessageRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CsMessageRepository(SqlAccessor accessor)
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
			var reader = GetWithReader(XML_KEY_NAME, "GetCsMessageMaster", input, replaces);
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
			var dv = Get(XML_KEY_NAME, "GetCsMessageMaster", input, replaces: replaces);
			return dv;
		}
		#endregion
	}
}