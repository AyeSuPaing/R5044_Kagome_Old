/*
=========================================================================================================
  Module      : ユーザー管理レベル系ユーティリティクラス(UserManagementLevelUtility.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2011 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.Domain.Payment;
using w2.Domain.UserManagementLevel;

namespace w2.App.Common.User
{
	public class UserManagementLevelUtility
	{
		/// <summary>
		/// ユーザー管理レベル名取得
		/// </summary>
		/// <param name="userManagementLevelId">ユーザー管理レベルID</param>
		/// <returns>ユーザー管理レベル名</returns>
		public static string GetUserManagementLevelName(string userManagementLevelId)
		{
			var model = new UserManagementLevelService().Get(userManagementLevelId);
			var result =  (model != null) 
				? model.UserManagementLevelName 
				: "";

			return result;
		}

		/// <summary>
		/// Get Payment list that user Level Not Use
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="userManagementLevelId">User level ID</param>
		/// <returns>List of payment name that user level not use</returns>
		public static PaymentModel[] GetPaymentsUserManagementLevelNotUse(string shopId, string userManagementLevelId)
		{
			var paymentModels = new PaymentService().GetValidAll(shopId);

			var result = paymentModels
				.Where(m => m.UserManagementLevelNotUse.Split(',')
					.Where(payment => payment != string.Empty)
					.Contains(userManagementLevelId))
				.ToArray();
			return result;
		}
	}
}
