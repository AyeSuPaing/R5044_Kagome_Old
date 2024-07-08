/*
=========================================================================================================
  Module      : ユーザークレジットカード連携情報基底クラス(UserCreditCardCooperationInfoBase.cs)
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
	/// ユーザークレジットカード連携情報基底クラス
	/// </summary>
	public abstract class UserCreditCardCooperationInfoBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cooperationId1">連携ID1</param>
		/// <param name="cooperationId2">連携ID2</param>
		protected UserCreditCardCooperationInfoBase(string cooperationId1, string cooperationId2)
		{
			this.CooperationId1 = cooperationId1;
			this.CooperationId2 = cooperationId2;
		}

		/// <summary>
		/// パラメタリスト作成
		/// </summary>
		/// <returns>パラメタ</returns>
		public abstract string[] CreateParams();

		/// <summary>連携ID1</summary>
		public string CooperationId1 { get; set; }
		/// <summary>連携ID2</summary>
		public string CooperationId2 { get; set; }
	}
}
