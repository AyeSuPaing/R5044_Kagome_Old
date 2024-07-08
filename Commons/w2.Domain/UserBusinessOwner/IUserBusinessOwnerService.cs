/*
=========================================================================================================
  Module      : User Business Owner Service(IUserBusinessOwnerService.cs)
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
	public interface IUserBusinessOwnerService : IService
	{
		/// <summary>
		/// ユーザー情報取得 (SqlAccessor指定)
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="accessor">
		/// トランザクションを内包するアクセサ
		/// 指定しない場合はメソッド内でトランザクションが完結
		/// </param>
		/// <returns>モデル</returns>
		UserBusinessOwnerModel GetByUserId(string userId, SqlAccessor accessor = null);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Insert(UserBusinessOwnerModel user,  SqlAccessor accessor = null);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Update(UserBusinessOwnerModel user,  SqlAccessor accessor);

		/// <summary>
		/// 与信ステータスによるデータ取得
		/// </summary>
		/// <param name="CreditStatus">与信状況</param>
		/// <returns>与信ステータス</returns>
		UserBusinessOwnerModel[] GetByCreditStatus(string CreditStatus);

		/// <summary>
		/// 審査中のGMOユーザーリスト取得
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>ユーザーリスト</returns>
		UserBusinessOwnerModel[] GetFrameGuaranteeNeedUpdate(SqlAccessor accessor = null);

		/// <summary>
		/// クレジットステータス更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="status">ステータス</param>
		/// <param name="accessor">SQLアクセサ</param>
		int UpdateCreditStatus(string userId, string status, SqlAccessor accessor = null);
	}
}
