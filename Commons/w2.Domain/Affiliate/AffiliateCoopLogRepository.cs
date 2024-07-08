/*
=========================================================================================================
  Module      : アフィリエイト連携ログリポジトリ (AffiliateCoopLogRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using System.Data;
using w2.Common.Sql;

namespace w2.Domain.Affiliate
{
	/// <summary>
	/// アフィリエイトリポ連携ログリポジトリ
	/// </summary>
	internal class AffiliateCoopLogRepository : RepositoryBase
	{
		/// <returns>アフィリエイト連携ログSQLファイル</returns>
		private const string XML_KEY_NAME = "AffiliateCoopLog";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal AffiliateCoopLogRepository()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal AffiliateCoopLogRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~DeleteByTagId 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>削除実行数</returns>
		/// <param name="tagId">タグID</param>
		internal int DeleteByTagId(int tagId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA11, tagId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteByCoopData11", ht);
			return result;
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckAffiliateCoopLogFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckAffiliateCoopLogFields", input, replaces: replaces);
		}

		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, "GetAffiliateCoopLogMaster", input, replaces);
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
			var dv = Get(XML_KEY_NAME, "GetAffiliateCoopLogMaster", input, replaces: replaces);
			return dv;
		}
		#endregion
	}
}
