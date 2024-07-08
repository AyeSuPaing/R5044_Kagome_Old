/*
=========================================================================================================
  Module      : LINE仮会員テーブルサービス (LineTemporaryUserService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Sql;

namespace w2.Domain.LineTemporaryUser
{
	/// <summary>
	/// LINE仮会員テーブルサービス
	/// </summary>
	public class LineTemporaryUserService : ServiceBase
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="lineUserId">LINEユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public LineTemporaryUserModel Get(string lineUserId, SqlAccessor accessor = null)
		{
			using (var repository = new LineTemporaryUserRepository(accessor))
			{
				var model = repository.Get(lineUserId);
				return model;
			}
		}
		#endregion

		#region +GetByUserId ユーザーIDから取得
		/// <summary>
		/// ユーザーIDから取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル</returns>
		public LineTemporaryUserModel GetByUserId(string userId)
		{
			using (var repository = new LineTemporaryUserRepository())
			{
				var model = repository.GetByUserId(userId);
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
		public void Insert(LineTemporaryUserModel model, SqlAccessor accessor = null)
		{
			using (var repository = new LineTemporaryUserRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		/// <summary>
		/// 本会員移行
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響件数</returns>
		/// <remarks>本会員登録時の規定データ設定を含む更新処理</remarks>
		public int UpdateToRegularAccount(LineTemporaryUserModel model, string lastChanged, SqlAccessor accessor = null)
		{
			model.RegularUserRegistrationFlag = Constants.FLG_LINETEMPORARYUSER_REGISTRATION_FLAG_VALID;
			model.RegularUserRegistrationDate = DateTime.Now;
			model.LastChanged = lastChanged;

			return Update(model, accessor);
		}

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響件数</returns>
		public int Update(LineTemporaryUserModel model, SqlAccessor accessor = null)
		{
			using (var repository = new LineTemporaryUserRepository(accessor))
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
		/// <param name="lineUserId">LINEユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響件数</returns>
		public int Delete(string lineUserId, SqlAccessor accessor = null)
		{
			using (var repository = new LineTemporaryUserRepository(accessor))
			{
				var result = repository.Delete(lineUserId);
				return result;
			}
		}
		#endregion
	}
}
