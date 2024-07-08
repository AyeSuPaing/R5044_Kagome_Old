/*
=========================================================================================================
  Module      : ユーザー統合情報リポジトリ (UserIntegrationRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Util;
using w2.Common.Sql;
using w2.Domain.User;
using w2.Domain.UserIntegration.Helper;

namespace w2.Domain.UserIntegration
{
	/// <summary>
	/// ユーザー統合情報リポジトリ
	/// </summary>
	public class UserIntegrationRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "UserIntegration";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public UserIntegrationRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserIntegrationRepository(SqlAccessor accessor)
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
		public int GetSearchHitCount(UserIntegrationListSearchCondition condition)
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
		public UserIntegrationListSearchResult[] Search(UserIntegrationListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "Search", condition.CreateHashtableParams());
			if (dv.Count == 0) return new UserIntegrationListSearchResult[0];

			// モデルから検索結果に変換する
			var results = new List<UserIntegrationListSearchResult>();
			foreach(var model in CreateUserIntegrationList(dv))
			{
				var result = new UserIntegrationListSearchResult(model.DataSource);
				result.Users = model.Users.Select(u => new UserIntegrationUserListSearchResult(u.DataSource)).ToArray();
				foreach(var user in result.Users)
				{
					user.Histories = 
						model.Users.First(mu => (user.UserIntegrationNo == mu.UserIntegrationNo) && (user.UserId == mu.UserId)).Histories.Select(h => new UserIntegrationHistoryListSearchResult(h.DataSource)).ToArray();
				}
				results.Add(result);
			}

			return results.ToArray();
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		/// <returns>モデル</returns>
		public UserIntegrationModel Get(long userIntegrationNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERINTEGRATION_USER_INTEGRATION_NO, userIntegrationNo},
			};
			var dv = Get(XML_KEY_NAME, "Get" , ht);
			return (dv.Count != 0) ? new UserIntegrationModel(dv[0]) : null;
		}
		#endregion

		#region +GetContainer 取得（表示用）
		/// <summary>
		/// 取得（表示用）
		/// </summary>
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		/// <returns>モデル</returns>
		public UserIntegrationContainer GetContainer(long userIntegrationNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERINTEGRATION_USER_INTEGRATION_NO, userIntegrationNo},
			};
			var dv = Get(XML_KEY_NAME, "GetContainer", ht);
			if (dv.Count == 0) return null;

			// モデルから検索結果に変換する
			UserIntegrationContainer result = null;
			foreach (var model in CreateUserIntegrationList(dv))
			{
				result = new UserIntegrationContainer(model.DataSource);
				result.Users = model.Users.Select(u => new UserIntegrationUserContainer(u.DataSource)).ToArray();
				foreach (var user in result.Users)
				{
					user.Histories =
						model.Users.First(mu => (user.UserIntegrationNo == mu.UserIntegrationNo) && (user.UserId == mu.UserId)).Histories.Select(h => new UserIntegrationHistoryContainer(h.DataSource)).ToArray();
				}
				break;
			}

			return result;
		}
		#endregion

		#region +GetAll 全件取得
		/// <summary>
		/// 全件取得
		/// </summary>
		/// <returns>モデル列</returns>
		public UserIntegrationModel[] GetAll()
		{
			var dv = Get(XML_KEY_NAME, "GetAll");
			if (dv.Count == 0) return new UserIntegrationModel[0];

			return CreateUserIntegrationList(dv).OrderBy(ui => ui.UserIntegrationNo).ToArray();
		}
		#endregion

		#region +GetUnintegratedUsers ユーザー統合解除情報取得
		/// <summary>
		/// ユーザー統合解除情報取得
		/// </summary>
		/// <returns>モデル列</returns>
		public UserIntegrationModel[] GetUnintegratedUsers(long userIntegrationNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERINTEGRATION_USER_INTEGRATION_NO, userIntegrationNo},
			};
			var dv = Get(XML_KEY_NAME, "GetUnintegratedUsers", ht);
			if (dv.Count == 0) return new UserIntegrationModel[0];

			return CreateUnIntegrationUsersList(dv).OrderBy(ui => ui.UserIntegrationNo).ToArray();
		}
		#endregion

		#region +GetUserIntegrationByUserId ユーザーIDからユーザー統合情報取得
		/// <summary>
		/// ユーザーIDからユーザー統合情報取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル列</returns>
		public UserIntegrationModel[] GetUserIntegrationByUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERINTEGRATIONUSER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetUserIntegrationByUserId", ht);
			if (dv.Count == 0) return new UserIntegrationModel[0];

			return CreateUserIntegrationList(dv).OrderBy(ui => ui.UserIntegrationNo).ToArray();
		}
		#endregion

		#region +GetIntegratedUserId 統合先ユーザーID取得
		/// <summary>
		/// 統合先ユーザーID取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>統合先ユーザーID</returns>
		public string GetIntegratedUserId(string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERINTEGRATIONUSER_USER_ID, userId},
			};
			var dv = Get(XML_KEY_NAME, "GetIntegratedUserId", ht);
			if (dv.Count == 0) return string.Empty;
			
			return dv[0][Constants.FIELD_USERINTEGRATIONUSER_USER_ID].ToString();
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public long Insert(UserIntegrationModel model)
		{
			return (long)(decimal)Get(XML_KEY_NAME, "Insert", model.DataSource)[0][Constants.FIELD_USERINTEGRATION_USER_INTEGRATION_NO];
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(UserIntegrationModel model)
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
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		public int Delete(long userIntegrationNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERINTEGRATION_USER_INTEGRATION_NO, userIntegrationNo},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region +GetUserAll 取得（ユーザ）
		/// <summary>
		/// 取得（ユーザ）
		/// </summary>
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		/// <returns>モデル列</returns>
		public UserIntegrationUserModel[] GetUserAll(long userIntegrationNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERINTEGRATIONUSER_USER_INTEGRATION_NO, userIntegrationNo}
			};
			var dv = Get(XML_KEY_NAME, "GetUserAll", ht);
			if (dv.Count == 0) return new UserIntegrationUserModel[0];

			return dv.Cast<DataRowView>().Select(drv => new UserIntegrationUserModel(drv)).ToArray();
		}
		#endregion

		#region +InsertUser 登録（ユーザ）
		/// <summary>
		/// 登録（ユーザ）
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertUser(UserIntegrationUserModel model)
		{
			Exec(XML_KEY_NAME, "InsertUser", model.DataSource);
		}
		#endregion

		#region +UpdateUser 更新（ユーザ）
		/// <summary>
		/// 更新（ユーザ）
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateUser(UserIntegrationUserModel model)
		{
			var result = Exec(XML_KEY_NAME, "UpdateUser", model.DataSource);
			return result;
		}
		#endregion

		#region +DeleteUser 削除（ユーザ）
		/// <summary>
		/// 削除（ユーザ）
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		public int DeleteUser(long userIntegrationNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERINTEGRATIONUSER_USER_INTEGRATION_NO, userIntegrationNo},
			};
			var result = Exec(XML_KEY_NAME, "DeleteUser", ht);
			return result;
		}
		#endregion

		#region +GetHistoryAll 取得（履歴）
		/// <summary>
		/// 取得（履歴）
		/// </summary>
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル列</returns>
		public UserIntegrationHistoryModel[] GetHistoryAll(long userIntegrationNo, string userId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERINTEGRATIONHISTORY_USER_INTEGRATION_NO, userIntegrationNo},
				{Constants.FIELD_USERINTEGRATIONHISTORY_USER_ID, userId}
			};
			var dv = Get(XML_KEY_NAME, "GetHistoryAll", ht);
			if (dv.Count == 0) return new UserIntegrationHistoryModel[0];
			return dv.Cast<DataRowView>().Select(drv => new UserIntegrationHistoryModel(drv)).ToArray();
		}
		#endregion

		#region +InsertHistory 登録（履歴）
		/// <summary>
		/// 登録（履歴）
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertHistory(UserIntegrationHistoryModel model)
		{
			Exec(XML_KEY_NAME, "InsertHistory", model.DataSource);
		}
		#endregion

		#region +DeleteHistory 削除（履歴）
		/// <summary>
		/// 削除（履歴）
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="userIntegrationNo">ユーザー統合No</param>
		public int DeleteHistory(long userIntegrationNo)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERINTEGRATIONHISTORY_USER_INTEGRATION_NO, userIntegrationNo},
			};
			var result = Exec(XML_KEY_NAME, "DeleteHistory", ht);
			return result;
		}
		#endregion

		#region +GetStatusCount ユーザー統合ステータス件数取得
		/// <summary>
		/// ユーザー統合ステータス件数取得
		/// </summary>
		/// <returns>検索結果列</returns>
		public UserIntegrationStatusCount[] GetStatusCount()
		{
			var dv = Get(XML_KEY_NAME, "GetStatusCount");
			if (dv.Count == 0) return new UserIntegrationStatusCount[0];
			return dv.Cast<DataRowView>().Select(drv => new UserIntegrationStatusCount(drv)).ToArray();
		}
		#endregion

		#region -ユーザー統合情報リスト作成
		/// <summary>
		/// ユーザー統合情報リスト作成
		/// </summary>
		/// <param name="dv">ユーザー統合情報（ユーザー・履歴を含む）</param>
		/// <returns>ユーザー統合情報リスト</returns>
		private UserIntegrationModel[] CreateUserIntegrationList(DataView dv)
		{
			var userIntegrationList = new List<UserIntegrationModel>();
			var userList = new List<UserIntegrationUserModel>();
			var historyList = new List<UserIntegrationHistoryModel>();
			foreach (DataRowView drv in dv)
			{
				var userIntegrationNo = (long)drv[Constants.FIELD_USERINTEGRATION_USER_INTEGRATION_NO];
				var userId = (string)drv[Constants.FIELD_USERINTEGRATIONUSER_USER_ID];
				if (userIntegrationList.Exists(ui => ui.UserIntegrationNo == userIntegrationNo) == false)
				{
					// ユーザー統合情報
					var userIntegration = new UserIntegrationModel(drv);
					userIntegrationList.Add(userIntegration);
				}
				if (userList.Exists(u => (u.UserIntegrationNo == userIntegrationNo) && (u.UserId == userId)) == false)
				{
					// ユーザー統合ユーザ情報
					var user = new UserIntegrationUserModel(drv)
					{
						DateCreated = (DateTime)drv[Constants.FIELD_USERINTEGRATIONUSER_DATE_CREATED + "_" + Constants.TABLE_USERINTEGRATIONUSER],
						DateChanged = (DateTime)drv[Constants.FIELD_USERINTEGRATIONUSER_DATE_CHANGED + "_" + Constants.TABLE_USERINTEGRATIONUSER],
						LastChanged = (string)drv[Constants.FIELD_USERINTEGRATIONUSER_LAST_CHANGED + "_" + Constants.TABLE_USERINTEGRATIONUSER]
					};
					userList.Add(user);
				}
				if (drv[Constants.FIELD_USERINTEGRATIONHISTORY_BRANCH_NO] != DBNull.Value)
				{
					var branchNo = (int)drv[Constants.FIELD_USERINTEGRATIONHISTORY_BRANCH_NO];
					if (historyList.Exists(u => (u.UserIntegrationNo == userIntegrationNo) && (u.UserId == userId) && (u.BranchNo == branchNo)) == false)
					{
						// ユーザー統合履歴情報
						var history = new UserIntegrationHistoryModel(drv)
						{
							DateCreated = (DateTime)drv[Constants.FIELD_USERINTEGRATIONHISTORY_DATE_CREATED + "_" + Constants.TABLE_USERINTEGRATIONHISTORY],
							DateChanged = (DateTime)drv[Constants.FIELD_USERINTEGRATIONHISTORY_DATE_CHANGED + "_" + Constants.TABLE_USERINTEGRATIONHISTORY],
							LastChanged = (string)drv[Constants.FIELD_USERINTEGRATIONHISTORY_LAST_CHANGED + "_" + Constants.TABLE_USERINTEGRATIONHISTORY]
						};
						historyList.Add(history);
					}
				}
			}
			foreach (var userIntegration in userIntegrationList)
			{
				userIntegration.Users =
					userList.Where(u => u.UserIntegrationNo == userIntegration.UserIntegrationNo).ToArray();
				foreach (var user in userIntegration.Users)
				{
					user.Histories = historyList.Where(h => (h.UserIntegrationNo == user.UserIntegrationNo) && (h.UserId == user.UserId)).ToArray();
				}
			}

			return userIntegrationList.ToArray();
		}

		#endregion

		#region -ユーザー統合解除情報リスト作成
		/// <summary>
		/// ユーザー統合解除情報リスト作成
		/// </summary>
		/// <param name="dv">ユーザー統合情報</param>
		/// <returns>ユーザー統合解除情報リスト</returns>
		private UserIntegrationModel[] CreateUnIntegrationUsersList(DataView dv)
		{
			var userIntegrationList = new List<UserIntegrationModel>();
			var userList = new List<UserIntegrationUserModel>();
			foreach (DataRowView drv in dv)
			{
				var userIntegration = new UserIntegrationModel(drv);
				var user = new UserIntegrationUserModel(drv)
				{
					DateCreated = (DateTime)drv[Constants.FIELD_USERINTEGRATIONUSER_DATE_CREATED + "_" + Constants.TABLE_USERINTEGRATIONUSER],
					DateChanged = (DateTime)drv[Constants.FIELD_USERINTEGRATIONUSER_DATE_CHANGED + "_" + Constants.TABLE_USERINTEGRATIONUSER],
					LastChanged = (string)drv[Constants.FIELD_USERINTEGRATIONUSER_LAST_CHANGED + "_" + Constants.TABLE_USERINTEGRATIONUSER],
				};

				if (userIntegrationList.Exists(ui => ui.UserIntegrationNo == userIntegration.UserIntegrationNo) == false) userIntegrationList.Add(userIntegration);
				if (userList.Exists(u => (u.UserIntegrationNo == userIntegration.UserIntegrationNo) && (u.UserId == user.UserId)) == false) userList.Add(user);
			}
			foreach (var userIntegration in userIntegrationList)
			{
				userIntegration.Users =
					userList.Where(u => u.UserIntegrationNo == userIntegration.UserIntegrationNo).ToArray();
			}

			return userIntegrationList.ToArray();
		}

		#endregion

		#region +GetBeforeIntegrationUserByOrderId 注文IDより統合前のユーザ情報を取得
		/// <summary>
		/// 注文IDより統合前のユーザ情報を取得
		/// </summary>
		/// <param name="primaryKey1">注文ID</param>
		/// <returns>ユーザモデル</returns>
		public UserModel GetBeforeIntegrationUserByOrderId(string primaryKey1)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_USERINTEGRATIONHISTORY_PRIMARY_KEY1, primaryKey1},
			};
			var dv = Get(XML_KEY_NAME, "GetBeforeIntegrationUserByOrderId", ht);
			return (dv.Count != 0) ? new UserModel(dv[0]) : null;
		}
		#endregion
	}
}
