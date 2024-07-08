/*
=========================================================================================================
  Module      : ショートURLリポジトリ (ShortUrlRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.ShortUrl.Helper;

namespace w2.Domain.ShortUrl
{
	/// <summary>
	/// ショートURLリポジトリ
	/// </summary>
	public class ShortUrlRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ShortUrl";

		#region コンストラクタ

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public ShortUrlRepository()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ShortUrlRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}

		#endregion

		#region +GetSearchHitCount 検索ヒット件数取得

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(ShortUrlListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}

		#endregion

		#region +Search 検索

		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public ShortUrlListSearchResult[] Search(ShortUrlListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new ShortUrlListSearchResult(drv)).ToArray();
		}

		#endregion

		#region +GetAll 取得（全て）

		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public ShortUrlModel[] GetAll(string shopId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHORTURL_SHOP_ID, shopId},
			};
			var dv = Get(XML_KEY_NAME, "GetAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new ShortUrlModel(drv)).ToArray();
		}

		#endregion

		#region ~GetShortUrlForDuplicationShortUrl ショートURL重複ショートURL情報取得（※ワークテーブル参照）
		/// <summary>
		/// 重複ショートURL情報取得（※ワークテーブル参照）
		/// </summary>
		/// <returns>重複ショートURL情報</returns>
		internal Dictionary<string, int> GetShortUrlForDuplicationShortUrl()
		{
			var dv = Get(XML_KEY_NAME, "GetShortUrlForDuplicationShortUrl");
			return dv.Cast<DataRowView>().ToDictionary(drv => (string)drv[Constants.FIELD_SHORTURL_SHORT_URL], drv => (int)drv["count"]);
		}
		#endregion

		#region +Insert 登録

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(ShortUrlModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}

		#endregion

		#region +Update 更新

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(ShortUrlModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}

		#endregion

		#region +Delete 削除

		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="surlNo">ショートURL NO</param>
		public int Delete(long surlNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHORTURL_SURL_NO, surlNo},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
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
			var reader = GetWithReader(XML_KEY_NAME, "GetShortUrlMaster", input, replaces);
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
			var dv = Get(XML_KEY_NAME, "GetShortUrlMaster", input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckShortUrlFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckShortUrlFields", input, replaces: replaces);
		}
		#endregion
	}
}