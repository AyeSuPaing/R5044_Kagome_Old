/*
=========================================================================================================
  Module      : コーディネートカテゴリリポジトリ (CoordinateCategoryRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.CoordinateCategory.Helper;

namespace w2.Domain.CoordinateCategory
{
	/// <summary>
	/// コーディネートカテゴリリポジトリ
	/// </summary>
	internal class CoordinateCategoryRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "CoordinateCategory";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal CoordinateCategoryRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal CoordinateCategoryRepository(SqlAccessor accessor)
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
		internal int GetSearchHitCount(CoordinateCategoryListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal CoordinateCategoryListSearchResult[] Search(CoordinateCategoryListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new CoordinateCategoryListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~GetCategoryTree カテゴリツリーを取得
		/// <summary>
		/// カテゴリーツリーを取得
		/// </summary>
		/// <param name="categoryIds">カテゴリID</param>
		/// <returns>カテゴリーツリー</returns>
		internal CoordinateCategoryModel[] GetCategoryTree(Hashtable categoryIds)
		{
			var dv = Get(XML_KEY_NAME, "GetCategoryTree", categoryIds);
			return dv.Cast<DataRowView>().Select(drv => new CoordinateCategoryModel(drv)).ToArray();
		}
		#endregion

		#region ~GetParentCategories 指定カテゴリIDと、その上位のカテゴリ(TOPカテゴリまで)のリストを取得する
		/// <summary>
		/// 指定カテゴリIDと、その上位のカテゴリ(TOPカテゴリまで)のリストを取得する
		/// </summary>
		/// <param name="categoryId">コーディネートID</param>
		/// <returns>カテゴリーツリー</returns>
		internal CoordinateCategoryModel[] GetParentCategories(string categoryId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_ID, categoryId},
			};
			var dv = Get(XML_KEY_NAME, "GetParentCategories", ht);
			return dv.Cast<DataRowView>().Select(drv => new CoordinateCategoryModel(drv)).ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="coordinateCategoryId">カテゴリID</param>
		/// <returns>モデル</returns>
		internal CoordinateCategoryModel Get(string coordinateCategoryId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_ID, coordinateCategoryId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new CoordinateCategoryModel(dv[0]);
		}
		#endregion

		#region ~GetByName 名前で取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="coordinateCategoryName">カテゴリ名</param>
		/// <returns>モデル</returns>
		internal CoordinateCategoryModel GetByName(string coordinateCategoryName)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_NAME, coordinateCategoryName},
			};
			var dv = Get(XML_KEY_NAME, "GetByName", ht);
			if (dv.Count == 0) return null;
			return new CoordinateCategoryModel(dv[0]);
		}
		#endregion

		#region ~GetAll 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル</returns>
		internal CoordinateCategoryModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new CoordinateCategoryModel(drv)).ToArray();
		}
		#endregion

		#region ~GetTopCoordinateCategory 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル</returns>
		internal CoordinateCategoryModel[] GetTopCoordinateCategory()
		{
			var dv = Get(XML_KEY_NAME, "GetTopCoordinateCategory");
			return dv.Cast<DataRowView>().Select(drv => new CoordinateCategoryModel(drv)).ToArray();
		}
		#endregion

		#region ~GetTopCoordinateCategory 取得
		/// <summary>
		/// コーディネートカテゴリー情報取得(直下の子カテゴリー)
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>モデル</returns>
		internal CoordinateCategoryModel[] GetCoordinateCategoryFamily(string categoryId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_ID, categoryId}
			};
			var dv = Get(XML_KEY_NAME, "GetCoordinateCategoryFamily", ht);
			return dv.Cast<DataRowView>().Select(drv => new CoordinateCategoryModel(drv)).ToArray();
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(CoordinateCategoryModel model)
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
		internal int Update(CoordinateCategoryModel model)
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
		/// <param name="coordinateCategoryId">カテゴリID</param>
		internal int Delete(string coordinateCategoryId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_COORDINATECATEGORY_COORDINATE_CATEGORY_ID, coordinateCategoryId},
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
			var reader = GetWithReader(XML_KEY_NAME, "GetCoordinateCategoryMaster", input, replaces);
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
			var dv = Get(XML_KEY_NAME, "GetCoordinateCategoryMaster", input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckCoordinateCategoryFields", input, replaces: replaces);
		}
		#endregion
	}
}
