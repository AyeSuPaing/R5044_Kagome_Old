/*
=========================================================================================================
  Module      : Interface User Management Level Service (IUserManagementLevelService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.UserManagementLevel
{
	/// <summary>
	/// Interface User Management Level Service
	/// </summary>
	public interface IUserManagementLevelService : IService
	{
		/// <summary>
		/// 全取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		UserManagementLevelModel[] GetAllList(SqlAccessor accessor = null);

		/// <summary>
		/// 全取得
		/// </summary>
		/// <param name="sortType">Seqソート</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		UserManagementLevelModel[] GetAllListSeqSort(UserManagementLevelService.SortType sortType, SqlAccessor accessor = null);

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		UserManagementLevelModel Get(string userManagementLevelId, SqlAccessor accessor = null);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		void Insert(UserManagementLevelModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Update(UserManagementLevelModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Delete(string userManagementLevelId, SqlAccessor accessor = null);

		/// <summary>
		/// Get user management level names by user management level ids
		/// </summary>
		/// <param name="userManagementLevelIds">User management level ids</param>
		/// <returns>User management level names</returns>
		string[] GetUserManagementLevelNamesByUserManagementLevelIds(string[] userManagementLevelIds);
	}
}
