/*
=========================================================================================================
  Module      :トークン取得向け決済情報（SBPS）(UserCreditCardCooperationInfoSbps.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.UserCreditCardCooperationInfos
{
	/// <summary>
	/// トークン取得向け決済情報（SBPS）
	/// </summary>
	public class UserCreditCardCooperationInfoSbps : UserCreditCardCooperationInfoBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		public UserCreditCardCooperationInfoSbps(string userId)
			: base(CreateCooperationId1(userId), CreateCooperationId2())
		{
		}

		/// <summary>
		/// 連携ID1作成
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>連携ID1</returns>
		private static string CreateCooperationId1(string userId)
		{
			// 顧客IDは一意な半角英数64文字
			var id1 = userId + "-" + Guid.NewGuid();
			return id1;
		}

		/// <summary>
		/// 連携ID2作成
		/// </summary>
		/// <returns>連携ID1</returns>
		private static string CreateCooperationId2()
		{
			// 利用なし
			return "";
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
