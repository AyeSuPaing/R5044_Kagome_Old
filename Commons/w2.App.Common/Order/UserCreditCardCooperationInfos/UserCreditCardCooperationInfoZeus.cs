/*
=========================================================================================================
  Module      :トークン取得向け決済情報（ZEUS）(UserCreditCardCooperationInfoZeus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.App.Common.Order.UserCreditCardCooperationInfos
{
	/// <summary>
	/// トークン取得向け決済情報（ZEUS）
	/// </summary>
	public class UserCreditCardCooperationInfoZeus : UserCreditCardCooperationInfoBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="telNo">電話番号</param>
		public UserCreditCardCooperationInfoZeus(string telNo)
			: base(CreateCooperationId1ForZeus(telNo), "")
		{
		}

		/// <summary>
		/// 連携ID1作成
		/// </summary>
		/// <param name="telNo">電話番号</param>
		/// <returns>連携ID1</returns>
		public static string CreateCooperationId1ForZeus(string telNo)
		{
			// ZEUS決済の「telno」
			var id1 = telNo;
			return id1;
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
