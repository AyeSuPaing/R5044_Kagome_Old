/*
=========================================================================================================
  Module      : 入荷通知メールリポジトリ (UserProductArrivalMailRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.UserProductArrivalMail
{
	/// <summary>
	/// 入荷通知メールリポジトリ
	/// </summary>
	public class UserProductArrivalMailRepository : RepositoryBase
	{
		/// <returns>ユーザーSQLファイル</returns>
		private const string XML_KEY_NAME = "UserProductArrivalMail";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserProductArrivalMailRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserProductArrivalMailRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, statementName, input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="statementName">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, string statementName, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, statementName, input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <param name="replaces">置換値</param>
		public void CheckFieldsForGetMaster(Hashtable input, string masterKbn, params KeyValuePair<string, string>[] replaces)
		{
			var statement = string.Empty;

			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL: // 入荷通知メールマスタ表示（サマリー）
					statement = "CheckUserProductArrivalMailFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_USERPRODUCTARRIVALMAIL_DETAIL: // 入荷通知メールマスタ表示（明細）
					statement = "CheckUserProductArrivalMailDetailFields";
					break;
			}
			Get(XML_KEY_NAME, statement, input, replaces: replaces);
		}
		#endregion
	}
}