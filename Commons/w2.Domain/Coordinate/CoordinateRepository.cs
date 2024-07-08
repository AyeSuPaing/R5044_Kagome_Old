/*
=========================================================================================================
  Module      : コーディネートリポジトリ (CoordinateRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.Coordinate.Helper;

namespace w2.Domain.Coordinate
{
	/// <summary>
	/// コーディネートリポジトリ
	/// </summary>
	internal class CoordinateRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "Coordinate";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal CoordinateRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal CoordinateRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(CoordinateListSearchCondition condition)
		{
			var input = condition.CreateHashtableParams();
			var dv = Get(
				XML_KEY_NAME,
				"GetSearchHitCount",
				input,
				new Tuple<object[], SqlDbType, int?>(condition.StaffIds.Cast<object>().ToArray(), SqlDbType.NVarChar, null));

			return (int)dv[0][0];
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal CoordinateListSearchResult[] Search(CoordinateListSearchCondition condition)
		{
			var input = condition.CreateHashtableParams();
			var dv = Get(
				XML_KEY_NAME,
				"Search",
				input,
				new Tuple<object[], SqlDbType, int?>(condition.StaffIds.Cast<object>().ToArray(), SqlDbType.NVarChar, null));

			return dv.Cast<DataRowView>().Select(drv => new CoordinateListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>モデル</returns>
		internal CoordinateModel Get(string coordinateId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COORDINATE_COORDINATE_ID, coordinateId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new CoordinateModel(dv[0]);
		}
		#endregion

		#region ~GetForPreview プレビュー取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>モデル</returns>
		internal DataView GetForPreview(string coordinateId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COORDINATE_COORDINATE_ID, coordinateId},
			};
			var dv = Get(XML_KEY_NAME, "GetForPreview", ht);
			if (dv.Count == 0) return null;
			return dv;
		}
		#endregion

		#region ~GetItems 項目取得
		/// <summary>
		/// 項目取得
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <returns>モデル</returns>
		internal CoordinateItemModel[] GetItems(string coordinateId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COORDINATE_COORDINATE_ID, coordinateId},
			};
			var dv = Get(XML_KEY_NAME, "GetItems", ht);
			return dv.Cast<DataRowView>().Select(drv => new CoordinateItemModel(drv)).ToArray();
		}
		#endregion

		#region ~GetItemsByProductId 項目取得
		/// <summary>
		/// 項目取得
		/// </summary>
		/// <param name="productId"></param>
		/// <returns>モデル</returns>
		internal CoordinateItemModel[] GetItemsByProductId(string productId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_PRODUCT_PRODUCT_ID, productId},
			};
			var dv = Get(XML_KEY_NAME, "GetItemsByProductId", ht);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new CoordinateItemModel(drv)).ToArray();
		}
		#endregion

		#region ~GetAll 全取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル</returns>
		internal CoordinateModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new CoordinateModel(drv)).ToArray();
		}
		#endregion

		#region ~GetLikeList いいねリストを取得
		/// <summary>
		/// いいねリストを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pageNumber">ページNo</param>
		/// <param name="dispContents">表示数</param>
		/// <returns>検索結果列</returns>
		internal CoordinateModel[] GetLikeList(string userId, int pageNumber, int dispContents)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERACTIVITY_USER_ID, userId},
				{"bgn_row_num", dispContents * (pageNumber - 1) + 1},
				{"end_row_num", dispContents * pageNumber}
			};
			var dv = Get(XML_KEY_NAME, "GetLikeList", ht);
			return dv.Cast<DataRowView>().Select(drv => new CoordinateModel(drv)).ToArray();
		}
		#endregion

		#region ~GetLikeListCount いいねリスト件数を取得
		/// <summary>
		/// いいねリスト件数を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>検索結果列</returns>
		internal int GetLikeListCount(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERACTIVITY_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetLikeListCount", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetLikeRankingList いいねランキングを取得
		/// <summary>
		/// いいねランキングを取得
		/// </summary>
		/// <param name="countDays">集計日数</param>
		/// <returns>モデル</returns>
		internal CoordinateModel[] GetLikeRankingList(int countDays)
		{
			var ht = new Hashtable
			{
				{"count_days", countDays},
			};
			var dv = Get(XML_KEY_NAME, "GetLikeRankingList", ht);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new CoordinateModel(drv)).ToArray();
		}
		#endregion

		#region ~GetContentsSummaryRankingList 日時概要ランキングを取得
		/// <summary>
		/// 日時概要ランキングを取得
		/// </summary>
		/// <param name="reportType">レポートタイプ</param>
		/// <param name="countDays">集計日数</param>
		/// <returns>モデル</returns>
		internal CoordinateModel[] GetContentsSummaryRankingList(string reportType, int countDays)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_CONTENTSLOG_REPORT_TYPE , reportType},
				{"count_days", countDays},
			};
			var dv = Get(XML_KEY_NAME, "GetContentsSummaryRankingList", ht);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new CoordinateModel(drv)).ToArray();
		}
		#endregion

		#region ~GetCoordinateTopForPreview 先頭のコーディネート取得 (プレビュー用)
		/// <summary>
		/// 先頭のコーディネート取得 (プレビュー用)
		/// </summary>
		/// <returns>モデル</returns>
		internal CoordinateModel GetCoordinateTopForPreview()
		{
			var dv = Get(XML_KEY_NAME, "GetCoordinateTopForPreview");
			if (dv.Count == 0) return null;
			return new CoordinateModel(dv[0]);
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(CoordinateModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~InsertCoordinateItem 項目登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void InsertCoordinateItem(CoordinateItemModel model)
		{
			Exec(XML_KEY_NAME, "InsertCoordinateItem", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(CoordinateModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="coordinateId">コーディネートID</param>
		internal void Delete(string coordinateId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COORDINATE_COORDINATE_ID, coordinateId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
		}
		#endregion

		#region ~DeleteItem 項目削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="coordinateId">コーディネートID</param>
		internal void DeleteItem(string coordinateId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COORDINATE_COORDINATE_ID, coordinateId},
			};
			var result = Exec(XML_KEY_NAME, "DeleteItem", ht);
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
		/// <param name="masterKbn"></param>
		/// <param name="replaces">置換値</param>
		public void CheckFieldsForGetMaster(Hashtable input, string masterKbn, params KeyValuePair<string, string>[] replaces)
		{
			var statement = string.Empty;

			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE: // コーディネートマスタ表示
					statement = "CheckCoordinateFields";
					break;

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_ITEM: // コーディネートアイテムマスタ表示
					statement = "CheckCoordinateItemFields";
					break;
			}
			Get(XML_KEY_NAME, statement, input, replaces: replaces);
		}
		#endregion
	}
}
