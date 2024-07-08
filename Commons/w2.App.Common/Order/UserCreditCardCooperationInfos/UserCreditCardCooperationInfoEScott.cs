/*
=========================================================================================================
  Module      :決済連携向け決済情報（e-SCOTT）(UserCreditCardCooperationInfoEScott.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.Order.UserCreditCardCooperationInfos
{
	/// <summary>
	/// クレジットカード情報連携情報（e-SCOTT）
	/// </summary>
	public class UserCreditCardCooperationInfoEScott : UserCreditCardCooperationInfoBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="branchNo">枝番</param>
		public UserCreditCardCooperationInfoEScott(string userId, string branchNo)
			: base(CreateCooperationId(userId, branchNo), string.Empty)
		{
		}

		/// <summary>
		/// 会員ID作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="branchNo">枝番</param>
		/// <returns>連携ID1</returns>
		public static string CreateCooperationId(string userId, string branchNo)
		{
			var id = userId + "" + branchNo.PadLeft(5, '0');
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
