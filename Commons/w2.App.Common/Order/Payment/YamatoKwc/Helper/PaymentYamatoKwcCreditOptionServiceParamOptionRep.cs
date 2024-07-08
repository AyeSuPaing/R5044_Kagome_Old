/*
=========================================================================================================
  Module      : ヤマトKWC クレジットオプションサービスパラメタ リピート(PaymentYamatoKwcCreditOptionServiceParamOptionRep.cs)
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
	/// ヤマトKWCクレジットオプションサービスパラメタ リピート
	/// </summary>
	public class PaymentYamatoKwcCreditOptionServiceParamOptionRep : PaymentYamatoKwcCreditOptionServiceParamBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="membderId">カード保有者を特定するＩＤ</param>
		/// <param name="authenticationKey">認証キー</param>
		/// <param name="cardData">カードデータ</param>
		/// <param name="secutiryCode">セキュリティコード</param>
		public PaymentYamatoKwcCreditOptionServiceParamOptionRep(
			string membderId,
			string authenticationKey,
			PaymentYamatoKwcCreditInfoGetResponseData.CardData cardData,
			string secutiryCode)
		{
			this.OptionServiceDiv = "01";	// オプションサービス受注
			this.MembderId = membderId;
			this.AuthenticationKey = authenticationKey;
			this.CardKey = cardData.CardKey.ToString();
			this.LastCreditDate = cardData.LastCreditDate.ToString("yyyyMMddHHmmss");
			this.CheckSum = PaymentYamatoKwcCheckSumCreater.CreateForAuth(membderId, authenticationKey);
			this.SecurityCode = secutiryCode;
		}
	}
}
