/*
=========================================================================================================
  Module      : User Business Owner Service(UserBusinessOwnerService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Sql;

namespace w2.Domain.UserBusinessOwner
{
	public class UserBusinessOwnerService : ServiceBase, IUserBusinessOwnerService
	{
		#region +Get By User Id
		/// <summary>
		/// ユーザー情報取得 (SqlAccessor指定)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// </param>
		/// <returns>モデル</returns>
		public UserBusinessOwnerModel GetByUserId(string userId, SqlAccessor accessor = null)
		{
			using (var repository = new UserBusinessOwnerRepository(accessor))
			{
				// ユーザー情報
				var user = repository.GetByUserId(userId);
				return user;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Insert(UserBusinessOwnerModel user, SqlAccessor accessor = null)
		{
			using (var repository = new UserBusinessOwnerRepository(accessor))
			{
				var result = repository.Insert(user);
				return result;
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(UserBusinessOwnerModel user, SqlAccessor accessor = null)
		{
			using (var repository = new UserBusinessOwnerRepository(accessor))
			{
				var result = repository.Update(user);
				return result;
			}
		}
		#endregion

		#region +Get By Credit Status
		/// <summary>
		/// 与信ステータスによるデータ取得
		/// </summary>
		/// <param name="CreditStatus">与信状況</param>
		/// <returns>与信ステータス</returns>
		public UserBusinessOwnerModel[] GetByCreditStatus(string CreditStatus)
		{
			using (var repository = new UserBusinessOwnerRepository())
			{
				return repository.GetByCreditStatus(CreditStatus);
			}
		}
		#endregion

		#region +Get Frame Guarantee Need Update 審査中のGMOユーザーリスト取得
		/// <summary>
		/// 審査中のGMOユーザーリスト取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザーリスト</returns>
		public UserBusinessOwnerModel[] GetFrameGuaranteeNeedUpdate(SqlAccessor accessor = null)
		{
			using (var repository = new UserBusinessOwnerRepository(accessor))
			{
				var userBusinessOwner = repository.GetFrameGuaranteeNeedUpdate();
				return userBusinessOwner;
			}
		}
		#endregion

		#region +Update Credit Status クレジットステータス更新
		/// <summary>
		/// クレジットステータス更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="status">ステータス</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int UpdateCreditStatus(string userId, string status, SqlAccessor accessor = null)
		{
			using (var repository = new UserBusinessOwnerRepository(accessor))
			{
				if (status == Constants.FLG_BUSINESS_OWNER_CREDIT_STATUS_UNDER_REVIEW)
				{
					status = Constants.FLG_BUSINESS_OWNER_CREDIT_STATUS_UNDER_REVIEW_EN;
				}
				var result = repository.UpdateCreditStatus(userId, status);
				return result;
			}
		}
		#endregion
	}
}