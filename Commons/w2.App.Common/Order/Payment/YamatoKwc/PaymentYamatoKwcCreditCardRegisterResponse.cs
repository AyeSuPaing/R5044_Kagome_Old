/*
=========================================================================================================
  Module      : ヤマトKWC クレジット登録レスポンスクラス(PaymentYamatoKwcCreditCardRegisterResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWCクレジットカード登録レスポンスクラス
	/// </summary>
	public class PaymentYamatoKwcCreditCardRegisterResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="context">コンテキスト</param>
		public PaymentYamatoKwcCreditCardRegisterResponse(HttpContext context)
		{
			this.Post = context.Request.Form;
		}

		/// <summary>加盟店コード</summary>
		public string TraderCode { get { return this.Post["trader_code"]; } }
		/// <summary>受付番号</summary>
		public string OrderNo { get { return this.Post["order_no"]; } }
		/// <summary>決済金額</summary>
		public string SettlePrice { get { return this.Post["settle_price"]; } }
		/// <summary>決済日時</summary>
		public string SettleDate { get { return this.Post["ettle_date"]; } }
		/// <summary>決済結果</summary>
		public string SettleResult { get { return this.Post["settle_result"]; } }
		/// <summary>決済結果詳細</summary>
		public string SettleDetail { get { return this.Post["settle_detail"]; } }
		/// <summary>決済手段</summary>
		public string SettleMethod { get { return this.Post["settle_method"]; } }
		/// <summary>POST</summary>
		public NameValueCollection Post { get; private set; }
	}
}
