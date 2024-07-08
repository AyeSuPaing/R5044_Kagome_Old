/*
=========================================================================================================
  Module      : ユーザークレジットカード連携情報ダミークラス(UserCreditCardCooperationInfoBase.cs)
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
	/// ユーザークレジットカード連携情報ダミークラス
	/// </summary>
	public class UserCreditCardCooperationInfoDummy : UserCreditCardCooperationInfoBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public UserCreditCardCooperationInfoDummy()
			: base("", "")	// ここでは作成しない
		{
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
