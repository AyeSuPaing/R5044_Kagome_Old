/*
=========================================================================================================
  Module      : ターゲットリスト設定リポジトリ (TargetListRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
//using w2.Domain.TargetList.Helper;

namespace w2.Domain.TargetList
{
	/// <summary>
	/// ターゲットリスト設定リポジトリ
	/// </summary>
	internal class TargetListRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "TargetList";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal TargetListRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal TargetListRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion
		/*
		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(TargetListListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion
		*/
		/*
		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal TargetListListSearchResult[] Search(TargetListListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			return dv.Cast<DataRowView>().Select(drv => new TargetListListSearchResult(drv)).ToArray();
		}
		#endregion
		*/

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>モデル</returns>
		internal TargetListModel Get(string deptId, string targetId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TARGETLIST_DEPT_ID, deptId},
				{Constants.FIELD_TARGETLIST_TARGET_ID, targetId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new TargetListModel(dv[0]);
		}
		#endregion

		#region ~GetAll 取得(すべて)
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>モデル</returns>
		internal TargetListModel[] GetAll(string deptId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TARGETLIST_DEPT_ID, deptId},
			};
			var dv = Get(XML_KEY_NAME, "GetAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new TargetListModel(drv)).ToArray();
		}
		#endregion

		#region ~GetAllValidTargetList 有効なターゲットリストをすべて取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>モデル</returns>
		internal TargetListModel[] GetAllValidTargetList(string deptId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TARGETLIST_DEPT_ID, deptId},
			};
			var dv = Get(XML_KEY_NAME, "GetAllValidTargetList", ht);
			return dv.Cast<DataRowView>().Select(drv => new TargetListModel(drv)).ToArray();
		}
		#endregion

		/*
		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(TargetListModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion
		*/
		/*
		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(TargetListModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion
		*/
		/*
		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>
		internal int Delete(string deptId, string targetId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TARGETLIST_DEPT_ID, deptId},
				{Constants.FIELD_TARGETLIST_TARGET_ID, targetId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion
		*/

		#region +GetTargetListDataDeduplicated ターゲットリストデータ取得（重複除外）
		/// <summary>
		/// ターゲットリストデータ取得（重複除外）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetKbn">ターゲット区分</param>
		/// <param name="masterId">マスタID</param>
		/// <returns>ターゲットリスト</returns>
		internal TargetListDataModel[] GetTargetListDataDeduplicated(string deptId, string targetKbn, string masterId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TARGETLISTDATA_DEPT_ID, deptId},
				{Constants.FIELD_TARGETLISTDATA_TARGET_KBN, targetKbn},
				{Constants.FIELD_TARGETLISTDATA_MASTER_ID, masterId},
			};
			var dv = Get(XML_KEY_NAME, "GetTargetListDataDeduplicated", ht);
			return dv.Cast<DataRowView>().Select(drv => new TargetListDataModel(drv)).ToArray();
		}
		#endregion

		#region +GetHitTargetListId 対象ユーザの有効なターゲットリスト一覧を取得
		/// <summary>
		/// 対象ユーザの有効なターゲットリスト一覧を取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="targetKbn">ターゲット区分</param>
		/// <returns>ターゲットリストID配列</returns>
		internal string[] GetHitTargetListIds(string deptId, string userId, string targetKbn)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_TARGETLISTDATA_DEPT_ID, deptId},
				{Constants.FIELD_TARGETLISTDATA_TARGET_KBN, targetKbn},
				{Constants.FIELD_TARGETLISTDATA_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetHitTargetListIds", ht);
			return dv.Cast<DataRowView>().Select(drv => drv[Constants.FIELD_TARGETLIST_TARGET_ID].ToString()).ToArray();
		}
		#endregion

		#region ~GetTargetListForAction アクション実行ターゲットリスト取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>モデル</returns>
		internal TargetListModel[] GetTargetListForAction(string deptId, string targetListIds)
		{
			var dv = Get(XML_KEY_NAME, "GetTargetListForAction", replaces: new[]
			{
				new KeyValuePair<string, string>("@dept_id", deptId),
				new KeyValuePair<string, string>("@@ params @@", targetListIds),
			});
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new TargetListModel(drv)).ToArray();
		}
		#endregion

		#region ~CheckValidTargetList ターゲットリストID存在チェック
		/// <summary>
		/// ターゲットリストID存在チェック
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>ターゲットリストID存在するか</returns>
		internal bool CheckValidTargetList(string deptId, string targetId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_TARGETLIST_DEPT_ID, deptId },
				{ Constants.FIELD_TARGETLIST_TARGET_ID, targetId },
			};
			var dv = Get(XML_KEY_NAME, "CheckValidTargetList", ht);
			return ((int)dv[0][0] > 0);
		}
		#endregion

		#region ~CheckUserIdInTargetList ユーザーIDがターゲットリスト内にあるか
		/// <summary>
		/// ユーザーIDがターゲットリスト内にあるか
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザーIDがターゲットリスト内にあるか</returns>
		internal bool CheckUserIdInTargetList(string deptId, string masterId, string userId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_TARGETLISTDATA_DEPT_ID, deptId },
				{ Constants.FIELD_TARGETLISTDATA_MASTER_ID, masterId },
				{ Constants.FIELD_TARGETLISTDATA_USER_ID, userId },
			};
			var dv = Get(XML_KEY_NAME, "CheckUserIdInTargetList", ht);
			return ((int)dv[0][0] > 0);
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckTargetListDataFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckTargetListDataFields", input, replaces: replaces);
		}

		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, "GetTargetListDataMaster", input, replaces);
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
			var dv = Get(XML_KEY_NAME, "GetTargetListDataMaster", input, replaces: replaces);
			return dv;
		}
		#endregion
	}
}
