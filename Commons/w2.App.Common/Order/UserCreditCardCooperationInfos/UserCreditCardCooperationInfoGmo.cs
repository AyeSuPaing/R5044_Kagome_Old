﻿/*
=========================================================================================================
  Module      :トークン取得向け決済情報（GMO）(UserCreditCardCooperationInfoGmo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.Common.Sql;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.UserCreditCardCooperationInfos
{
	/// <summary>
	/// クレジットカード情報連携情報（GMO）
	/// </summary>
	public class UserCreditCardCooperationInfoGmo : UserCreditCardCooperationInfoBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public UserCreditCardCooperationInfoGmo(string userId, SqlAccessor accessor = null)
			: base(CreateGmoMemberId(userId, accessor), "")
		{
		}

		/// <summary>
		/// GMO会員ID作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>連携ID1</returns>
		public static string CreateGmoMemberId(string userId, SqlAccessor accessor = null)
		{
			var id = userId + new Random().Next(0, 99999).ToString("D5");
			// GMO会員IDが登録済の場合再採番
			var userInfo = new UserCreditCardService().GetByCooperationId1(id, accessor);
			if (userInfo != null) id = CreateGmoMemberId(userId, accessor);
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