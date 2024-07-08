/*
=========================================================================================================
  Module      :決済連携向け決済情報（ベリトランス連携）(UserCreditCardCooperationInfoVeritrans.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.UserCreditCardCooperationInfos
{
	/// <summary>
	/// クレジットカード情報連携情報（ベリトランス）
	/// </summary>
	public class UserCreditCardCooperationInfoVeritrans : UserCreditCardCooperationInfoBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		public UserCreditCardCooperationInfoVeritrans(string userId)
			: base(CreateCooperationId(userId), "")
		{
		}

		/// <summary>
		/// ベリトランス会員ID作成
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
