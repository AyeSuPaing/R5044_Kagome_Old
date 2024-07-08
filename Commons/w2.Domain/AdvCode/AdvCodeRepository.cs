/*
=========================================================================================================
  Module      : 広告コードリポジトリ (AdvCodeRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using w2.Common.Extensions;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.AdvCode.Helper;

namespace w2.Domain.AdvCode
{
	/// <summary>
	/// 広告コードリポジトリ
	/// </summary>
	internal class AdvCodeRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "AdvCode";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal AdvCodeRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal AdvCodeRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Advertisement Code

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// <param name="replaces">置換パラメタ</param>
		/// <returns>件数</returns>
		internal int GetAdvCodeSearchHitCount(AdvCodeListSearchCondition condition, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, "GetAdvCodeSearchHitCount", condition.CreateHashtableParams(), null, replaces);
			return (int)dv[0][0];
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// <param name="replaces">置換パラメタ</param>
		/// <returns>モデル列</returns>
		internal AdvCodeListSearchResult[] SearchAdvCode(AdvCodeListSearchCondition condition, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, "SearchAdvCode", condition.CreateHashtableParams(), null, replaces);
			return dv.Cast<DataRowView>().Select(drv => new AdvCodeListSearchResult(drv)).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="advcodeNo">広告コードNO</param>
		/// <returns>モデル</returns>
		internal AdvCodeModel GetAdvCode(long advcodeNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ADVCODE_ADVCODE_NO, advcodeNo},
			};
			var dv = Get(XML_KEY_NAME, "GetAdvCode", ht);
			if (dv.Count == 0) return null;
			return new AdvCodeModel(dv[0]);
		}

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="advertisementCode">広告コード</param>
		/// <returns>モデル</returns>
		internal AdvCodeModel GetAdvCodeFromAdvertisementCode(string advertisementCode)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE, advertisementCode},
			};
			var resultData = Get(XML_KEY_NAME, "GetAdvCodeFromAdvertisementCode", input);
			if (resultData.Count == 0) return null;
			return new AdvCodeModel(resultData[0]);
		}

		/// <summary>
		/// Get advertisement code from advertisement code media type id
		/// </summary>
		/// <param name="advCodeMediaType">AdvCode Media Type</param>
		/// <returns>advcode no</returns>
		internal int GetAdvCodeFromAdvcodeMediaTypeId(string advCodeMediaType)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID, advCodeMediaType},
			};
			var dataMediaType = Get(XML_KEY_NAME, "GetAdvCodeFromAdvcodeMediaTypeId", input);

			if (dataMediaType.Count != 0) return Convert.ToInt32(dataMediaType[0][0]);

			return 0;
		}

		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデルリスト</returns>
		internal AdvCodeModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			return dv.Cast<DataRowView>().Select(item => new AdvCodeModel(item)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertAdvCode(AdvCodeModel model)
		{
			Exec(XML_KEY_NAME, "InsertAdvCode", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateAdvCode(AdvCodeModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateAdvCode", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="advcodeNo">広告コードNO</param>
		internal int DeleteAdvCode(long advcodeNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ADVCODE_ADVCODE_NO, advcodeNo},
			};
			var result = Exec(XML_KEY_NAME, "DeleteAdvCode", ht);
			return result;
		}
		#endregion

		#endregion

		#region +Advertisement Code Media Type

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// <returns>件数</returns>
		internal int GetAdvCodeMediaTypeSearchHitCount(AdvCodeMediaTypeListSearchCondition condition)
		{
			var replaces = new KeyValuePair<string, string>(
				"@@ where_usable_media_type_ids @@",
				CreateWhereInStatementReplaces(Constants.FIELD_ADVCODE_ADVCODE_MEDIA_TYPE_ID, condition.GetUsableMediaTypeIdsArray()));
			var dv = Get(XML_KEY_NAME, "GetAdvCodeMediaTypeSearchHitCount", condition.CreateHashtableParams(), replaces: replaces);
			return (int)dv[0][0];
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// <returns>モデル列</returns>
		internal AdvCodeMediaTypeListSearchResult[] SearchAdvCodeMediaType(AdvCodeMediaTypeListSearchCondition condition)
		{
			var replaces = new KeyValuePair<string, string>(
				"@@ where_usable_media_type_ids @@",
				CreateWhereInStatementReplaces(Constants.FIELD_ADVCODE_ADVCODE_MEDIA_TYPE_ID, condition.GetUsableMediaTypeIdsArray()));
			var dv = Get(XML_KEY_NAME, "SearchAdvCodeMediaType", condition.CreateHashtableParams(), replaces: replaces);
			return dv.Cast<DataRowView>().Select(drv => new AdvCodeMediaTypeListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~SearchMediaTypeByKeyword キーワードから広告媒体区分を検索
		/// <summary>
		/// キーワードから広告媒体区分を検索
		/// </summary>
		/// <param name="searchWord">検索キーワード</param>
		/// <returns>検索結果列</returns>
		internal AdvCodeMediaTypeModel[] SearchMediaTypeByKeyword(string searchWord)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_ADVCODEMEDIATYPE_SEARCH_WORD, searchWord },
			};
			var dv = Get(XML_KEY_NAME, "SearchMediaTypeByKeyword", ht);
			return dv.Cast<DataRowView>().Select(drv => new AdvCodeMediaTypeModel(drv)).ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="advcodeMediaTypeId">区分ID</param>
		/// <returns>モデル</returns>
		internal AdvCodeMediaTypeModel GetAdvCodeMediaType(string advcodeMediaTypeId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID, advcodeMediaTypeId},
			};
			var dv = Get(XML_KEY_NAME, "GetAdvCodeMediaType", ht);
			if (dv.Count == 0) return null;
			return new AdvCodeMediaTypeModel(dv[0]);
		}

		/// <summary>
		/// Get AdvCode List All
		/// </summary>
		/// <returns>List AdvCodeModel</returns>
		internal AdvCodeMediaTypeModel[] GetAdvCodeMediaTypeListAll()
		{
			var advCodeList = Get(XML_KEY_NAME, "GetAdvCodeMediaTypeListAll");

			return advCodeList.Cast<DataRowView>().Select(item => new AdvCodeMediaTypeModel(item)).ToArray();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertAdvCodeMediaType(AdvCodeMediaTypeModel model)
		{
			Exec(XML_KEY_NAME, "InsertAdvCodeMediaType", model.DataSource);
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateAdvCodeMediaType(AdvCodeMediaTypeModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateAdvCodeMediaType", model.DataSource);
			return result;
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="advcodeMediaTypeId">区分ID</param>
		internal int DeleteAdvCodeMediaType(string advcodeMediaTypeId)
		{
			var input = new Hashtable
			{
				{Constants.FIELD_ADVCODEMEDIATYPE_ADVCODE_MEDIA_TYPE_ID, advcodeMediaTypeId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteAdvCodeMediaType", input);
			return result;
		}
		#endregion

		#endregion

		#region マスタ出力
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
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE:				// 広告コード
					statement = "CheckAdvCodeFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE_MEDIA_TYPE:	// 広告媒体区分マスタ
					statement = "CheckAdvCodeMediaTypeFields";
					break;
			}
			Get(XML_KEY_NAME, statement, input, replaces: replaces);
		}

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
		#endregion

		#region +SearchAdvCodesForAutosuggest
		/// <summary>
		/// Search advertisement codes for autosuggest
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>Advertisement codes</returns>
		internal AdvCodeModel[] SearchAdvCodesForAutosuggest(string searchWord)
		{
			var dv = Get(
				XML_KEY_NAME,
				"SearchAdvCodesForAutosuggest",
				new Hashtable
				{
					{ Constants.FIELD_ADVCODE_DEPT_ID, Constants.CONST_DEFAULT_DEPT_ID },
					{ "search_word_like_escaped", StringUtility.SqlLikeStringSharpEscape(searchWord) },
				});
			var advCodes = dv.Cast<DataRowView>()
				.Select(drv => new AdvCodeModel(drv))
				.ToArray();
			return advCodes;
		}
		#endregion
	}
}
