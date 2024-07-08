/*
=========================================================================================================
  Module      : ヤマトKWC クレジットオプションサービスパラメタ 登録(PaymentYamatoKwcCreditOptionServiceParamOptionReg.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.Order.Payment.YamatoKwc.Helper
{
	/// <summary>
	/// ヤマトKWCクレジットオプションサービスパラメタ 登録
	/// </summary>
	public class PaymentYamatoKwcCreditOptionServiceParamOptionReg : PaymentYamatoKwcCreditOptionServiceParamBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cardCodeApi">カード会社コード(API用)</param>
		/// <param name="token">クレジットトークン</param>
		public PaymentYamatoKwcCreditOptionServiceParamOptionReg(
			string cardCodeApi,
			string token)
		{
			this.OptionServiceDiv = "01";	// オプションサービス受注
			this.CardCodeApi = cardCodeApi;
			this.Token = token;
		}
	}
}
