/*
=========================================================================================================
  Module      : スタッフリポジトリ (StaffRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.Staff.Helper;

namespace w2.Domain.Staff
{
	/// <summary>
	/// スタッフリポジトリ
	/// </summary>
	internal class StaffRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "Staff";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal StaffRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal StaffRepository(SqlAccessor accessor)
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
		internal int GetSearchHitCount(StaffListSearchCondition condition)
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
		internal StaffListSearchResult[] Search(StaffListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new StaffListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="staffId">スタッフID</param>
		/// <returns>モデル</returns>
		internal StaffModel Get(string staffId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_STAFF_STAFF_ID, staffId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new StaffModel(dv[0]);
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// オペレータIDで取得
		/// </summary>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>モデル</returns>
		internal StaffModel[] GetByOperatorId(string operatorId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_STAFF_OPERATOR_ID, operatorId},
			};
			var dv = Get(XML_KEY_NAME, "GetByOperatorId", ht);
			return dv.Cast<DataRowView>().Select(drv => new StaffModel(drv)).ToArray();
		}
		#endregion

		#region ~GetAllForCoordinate 全取得（コーディネート用）
		/// <summary>
		/// 全取得（コーディネート用）
		/// </summary>
		/// <returns>モデル</returns>
		internal StaffModel[] GetAllForCoordinate()
		{
			var dv = Get(XML_KEY_NAME, "GetAllForCoordinate");
			return dv.Cast<DataRowView>().Select(drv => new StaffModel(drv)).ToArray();
		}
		#endregion

		#region ~GetFollowList フォローリストを取得
		/// <summary>
		/// フォローリストを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pageNumber">ページNo</param>
		/// <param name="dispContents">表示数</param>
		/// <returns>検索結果列</returns>
		internal StaffModel[] GetFollowList(string userId, int pageNumber, int dispContents)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERACTIVITY_USER_ID, userId},
				{"bgn_row_num", dispContents * (pageNumber - 1) + 1},
				{"end_row_num", dispContents * pageNumber}
			};
			var dv = Get(XML_KEY_NAME, "GetFollowList", ht);
			return dv.Cast<DataRowView>().Select(drv => new StaffModel(drv)).ToArray();
		}
		#endregion

		#region ~GetFollowCount フォローリスト件数を取得
		/// <summary>
		/// フォローリスト件数を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>検索結果列</returns>
		internal int GetFollowCount(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERACTIVITY_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetFollowCount", ht);
			return (int)dv[0][0];
		}
		#endregion

		#region ~GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル</returns>
		internal StaffModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new StaffModel(drv)).ToArray();
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(StaffModel model)
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
		internal int Update(StaffModel model)
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
		/// <param name="staffId">スタッフID</param>
		internal int Delete(string staffId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_STAFF_STAFF_ID, staffId},
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
			var reader = GetWithReader(XML_KEY_NAME, "GetStaffMaster", input, replaces);
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
			var dv = Get(XML_KEY_NAME, "GetStaffMaster", input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckStaffFields", input, replaces: replaces);
		}
		#endregion
	}
}
