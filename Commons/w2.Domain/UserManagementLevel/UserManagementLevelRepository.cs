/*
=========================================================================================================
  Module      : ユーザー管理レベルリポジトリ (UserManagementLevelRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.UserManagementLevel
{
	/// <summary>
	/// ユーザー管理レベルリポジトリ
	/// </summary>
	internal class UserManagementLevelRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "UserManagementLevel";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal UserManagementLevelRepository()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		internal UserManagementLevelRepository(SqlAccessor accessor) : base(accessor)
		{
		}
		#endregion

		#region ~GetAllList 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>全モデル</returns>
		internal UserManagementLevelModel[] GetAllList()
		{
			var dv = Get(XML_KEY_NAME, "GetAllList");
			var result = dv.Cast<DataRowView>().Select(drv => new UserManagementLevelModel(drv)).ToArray();
			return result;
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
		/// <returns>モデル</returns>
		internal UserManagementLevelModel Get(string userManagementLevelId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_ID, userManagementLevelId },
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			var result = (dv.Count == 0) ? null : new UserManagementLevelModel(dv[0]);
			return result;
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(UserManagementLevelModel model)
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
		internal int Update(UserManagementLevelModel model)
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
		/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
		/// <returns>影響を受けた件数</returns>
		internal int Delete(string userManagementLevelId)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_ID, userManagementLevelId },
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region +GetUserManagementLevelNamesByUserManagementLevelIds
		/// <summary>
		/// Get user management level names by user management level ids
		/// </summary>
		/// <param name="userManagementLevelIds">User management level ids</param>
		/// <returns>User management level names</returns>
		internal string[] GetUserManagementLevelNamesByUserManagementLevelIds(string[] userManagementLevelIds)
		{
			var replaceKeyValues = new[]
			{
				new KeyValuePair<string, string>(
					"@@ user_management_level_ids @@",
					string.Join(",", userManagementLevelIds.Select(userManagementLevelId => string.Format("'{0}'", userManagementLevelId.Replace("'", "''"))))),
			};
			var dv = Get(XML_KEY_NAME, "GetUserManagementLevelNamesByUserManagementLevelIds", replaces: replaceKeyValues);
			var userManagementLevelNames = dv.Cast<DataRowView>()
				.Select(drv => (string)drv[Constants.FIELD_USERMANAGEMENTLEVEL_USER_MANAGEMENT_LEVEL_NAME])
				.ToArray();
			return userManagementLevelNames;
		}
		#endregion
	}
}