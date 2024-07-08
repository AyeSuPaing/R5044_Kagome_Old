/*
=========================================================================================================
  Module      :決済連携向け決済情報（Zcom）(UserCreditCardCooperationInfoZcom.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.UserCreditCardCooperationInfos
{
	/// <summary>
	/// クレジットカード情報連携情報（Zcom）
	/// </summary>
	public class UserCreditCardCooperationInfoZcom : UserCreditCardCooperationInfoBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		public UserCreditCardCooperationInfoZcom(string userId)
			: base(CreateCooperationId(userId), "")
		{
		}

		/// <summary>
		/// GMO会員ID作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>連携ID1</returns>
		public static string CreateCooperationId(string userId)
		{
			var id = userId + "." + DateTime.Now.ToString("yyyyMMddHHmmssfff");
			return id;
		}

		/// <summary>
		/// パラメタ作成
		/// </summary>
		/// <returns>パラメタ</returns>
		public override string[] CreateParams()
		{
			return new[] { this.CooperationId1, this.CooperationId2 };
		}
	}
}
