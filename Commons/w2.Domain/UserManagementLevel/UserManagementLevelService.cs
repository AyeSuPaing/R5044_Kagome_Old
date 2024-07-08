/*
=========================================================================================================
  Module      : ユーザー管理レベルサービス (UserManagementLevelService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/

using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.UserManagementLevel
{
	/// <summary>
	/// ユーザー管理レベルサービス
	/// </summary>
	public class UserManagementLevelService : ServiceBase, IUserManagementLevelService
	{
		/// <summary>ソートタイプ</summary>
		public enum SortType
		{
			/// <summary>昇順</summary>
			Desc,
			/// <summary>降順</summary>
			Asc
		}

		#region +GetAllList 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public UserManagementLevelModel[] GetAllList(SqlAccessor accessor = null)
		{
			using (var repository = new UserManagementLevelRepository(accessor))
			{
				var models = repository.GetAllList();
				return models;
			}
		}
		#endregion

		#region +GetAllList 全取得 Seqソート
		/// <summary>
		/// 全取得
		/// </summary>
		/// <param name="sortType">Seqソート</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public UserManagementLevelModel[] GetAllListSeqSort(SortType sortType, SqlAccessor accessor = null)
		{
			var result = (sortType == SortType.Desc)
				? GetAllList().ToList().OrderBy(m => m.SeqNo).ToArray()
				: GetAllList().ToList().OrderByDescending(m => m.SeqNo).ToArray();

			return result;
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public UserManagementLevelModel Get(string userManagementLevelId, SqlAccessor accessor = null)
		{
			using (var repository = new UserManagementLevelRepository(accessor))
			{
				var model = repository.Get(userManagementLevelId);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(UserManagementLevelModel model, SqlAccessor accessor = null)
		{
			using (var repository = new UserManagementLevelRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(UserManagementLevelModel model, SqlAccessor accessor = null)
		{
			using (var repository = new UserManagementLevelRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Delete(string userManagementLevelId, SqlAccessor accessor = null)
		{
			using (var repository = new UserManagementLevelRepository(accessor))
			{
				var result = repository.Delete(userManagementLevelId);
				return result;
			}
		}
		#endregion

		#region +GetUserManagementLevelNamesByUserManagementLevelIds
		/// <summary>
		/// Get user management level names by user management level ids
		/// </summary>
		/// <param name="userManagementLevelIds">User management level ids</param>
		/// <returns>User management level names</returns>
		public string[] GetUserManagementLevelNamesByUserManagementLevelIds(string [] userManagementLevelIds)
		{
			using (var repository = new UserManagementLevelRepository())
			{
				var userManagementLevelNames = repository.GetUserManagementLevelNamesByUserManagementLevelIds(userManagementLevelIds);
				return userManagementLevelNames;
			}
		}
		#endregion
	}
}