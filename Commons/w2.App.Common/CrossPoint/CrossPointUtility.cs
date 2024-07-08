/*
=========================================================================================================
  Module      : CrossPointユーティリティ (CrossPointUtility.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.User;

namespace w2.App.Common.CrossPoint
{
	public class CrossPointUtility
	{
		/// <summary>
		/// クロスポイントユーザーか？
		/// </summary>
		/// <param name="user">ユーザーモデル</param>
		/// <remarks>クロスポイントユーザーの場合、ユーザー拡張項目にショップカード番号とピンコードを保持している</remarks>
		/// <returns>クロスポイントユーザーか？</returns>
		public bool IsCrossPointUser(UserModel user)
		{
			var isCrossPointUser = IsCrossPointUser(user.UserExtend);
			return isCrossPointUser;
		}
		/// <summary>
		/// クロスポイントユーザーか？
		/// </summary>
		/// <param name="userExtend">ユーザー拡張項目モデル</param>
		/// <remarks>クロスポイントユーザーの場合、ユーザー拡張項目にショップカード番号とピンコードを保持している</remarks>
		/// <returns>クロスポイントユーザーか？</returns>
		public bool IsCrossPointUser(UserExtendModel userExtend)
		{
			var isCrossPointUser =
				(userExtend.UserExtendDataText.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_NO)
					&& userExtend.UserExtendDataText.ContainsKey(Constants.CROSS_POINT_USREX_SHOP_CARD_PIN)
					&& (string.IsNullOrEmpty(userExtend.UserExtendDataValue.CrossPointShopCardNo) == false)
					&& (string.IsNullOrEmpty(userExtend.UserExtendDataValue.CrossPointShopCardPin) == false));
			return isCrossPointUser;
		}
	}
}