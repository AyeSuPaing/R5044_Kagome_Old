/*
=========================================================================================================
  Module      : ヤマトKWC コンビニ入金結果レシーバ(PaymentYamatoKwcCvsResultReceiver.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Web;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC コンビニ入金結果レシーバ
	/// </summary>
	public class PaymentYamatoKwcCvsResultReceiver
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentYamatoKwcCvsResultReceiver(HttpRequest req)
		{
			Receive(req);
		}

		/// <summary>
		/// 実行
		/// </summary>
		private void Receive(HttpRequest req)
		{
			this.TraderCode = req["trader_code"];
			this.OrderNo = req["order_no"];
			this.SettlePrice = decimal.Parse(req["settle_price"]);
			this.SettleDate = DateTime.ParseExact(req["settle_date"], "yyyyMMddHHmmss", null);
			this.SettleResult = req["settle_result"];
			this.SettleDetail = req["settle_detail"];
			this.SettleMethod = req["settle_method"];

			if (this.TraderCode != Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE) return;
			if (this.SettleResult == "2") // 決済結果：異常
			{
				PaymentYamatoKwcLogger.WriteLog(
					GetType().Name,
					PaymentFileLogger.PaymentProcessingType.Receive,
					false,
					new KeyValuePair<string, string>("OrderNo", this.OrderNo));
				return;
			}

			this.Settled = new PaymentYamatoKwcSettledChecker().Check(this.SettleMethod, this.SettleDetail);
			PaymentYamatoKwcLogger.WriteLog(
				GetType().Name,
				PaymentFileLogger.PaymentProcessingType.Receive,
				true,
				new KeyValuePair<string, string>("OrderNo", this.OrderNo));
		}

		/// <summary>加盟店コード</summary>
		public string TraderCode { get; private set; }
		/// <summary>お支払い受付番号</summary>
		public string OrderNo { get; private set; }
		/// <summary>決済金額</summary>
		public decimal SettlePrice { get; private set; }
		/// <summary>決済日時</summary>
		public DateTime SettleDate { get; private set; }
		/// <summary>決済結果</summary>
		public string SettleResult { get; private set; }
		/// <summary>決済詳細</summary>
		public string SettleDetail { get; private set; }
		/// <summary>決済手段</summary>
		public string SettleMethod { get; private set; }
		/// <summary>入金されたか</summary>
		public bool Settled { get; private set; }
	}
}
