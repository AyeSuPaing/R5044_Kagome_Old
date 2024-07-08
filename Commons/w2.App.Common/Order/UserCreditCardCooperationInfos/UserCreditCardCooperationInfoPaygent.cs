/*
=========================================================================================================
  Module      :トークン取得向け決済情報（ペイジェント）(UserCreditCardCooperationInfoPaygent.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Common.Sql;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.UserCreditCardCooperationInfos
{
	/// <summary>
	/// クレジットカード情報連携情報（ペイジェント）
	/// </summary>
	public class UserCreditCardCooperationInfoPaygent : UserCreditCardCooperationInfoBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public UserCreditCardCooperationInfoPaygent(string userId, SqlAccessor accessor = null)
			: base(CreatePaygentMemberId(userId, accessor), "")
		{
		}

		/// <summary>
		/// ペイジェント会員ID作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>連携ID1</returns>
		public static string CreatePaygentMemberId(string userId, SqlAccessor accessor = null)
		{
			var id = userId + new Random().Next(0, 99999).ToString("D5");
			// ペイジェント会員IDが登録済の場合再採番
			var userInfo = new UserCreditCardService().GetByCooperationId1(id, accessor);
			if (userInfo != null) id = CreatePaygentMemberId(userId, accessor);
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
