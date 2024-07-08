/*
=========================================================================================================
  Module      : ヤマトKWC クレジット情報削除API(PaymentYamatoKwcCreditInfoDeleteApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC クレジット情報削除API
	/// </summary>
	public class PaymentYamatoKwcCreditInfoDeleteApi : PaymentYamatoKwcApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reserve1">予備項目設定値(テスト用）</param>
		public PaymentYamatoKwcCreditInfoDeleteApi(string reserve1 = "")
			: base(PaymentYamatoKwcFunctionDiv.A03, reserve1)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="memberId">会員ID</param>
		/// <param name="authenticationKey">認証キー</param>
		/// <param name="cardKey">カード識別キー</param>
		/// <param name="lastCreditDate">最終利用日時</param>
		/// <returns>結果</returns>
		public PaymentYamatoKwcCreditInfoDeleteResponseData Exec(string memberId, string authenticationKey, string cardKey, DateTime lastCreditDate)
		{
			var param = CreateParam(memberId, authenticationKey, cardKey, lastCreditDate);

			var resultString = PostHttpRequest(param);
			var responseData = new PaymentYamatoKwcCreditInfoDeleteResponseData(resultString);
			WriteLog(
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentProcessingType.Cancel,
				responseData.Success,
				new KeyValuePair<string, string>("memberId", memberId),
				new KeyValuePair<string, string>("authenticationKey", authenticationKey),
				new KeyValuePair<string, string>("cardKey", cardKey),
				new KeyValuePair<string, string>("lastCreditDate", lastCreditDate.ToString("yyyyMMddHHmmss")));
			return responseData;
		}

		/// <summary>
		/// パラメタ作成
		/// </summary>
		/// <param name="memberId">会員ID</param>
		/// <param name="authenticationKey">認証キー</param>
		/// <param name="cardKey">カード識別キー</param>
		/// <param name="lastCreditDate">最終利用日時</param>
		/// <returns>パラメタ</returns>
		private string[][] CreateParam(string memberId, string authenticationKey, string cardKey, DateTime lastCreditDate)
		{
			var param = new[]
			{
				new[] {"function_div", this.FunctionDiv.ToString()},
				new[] {"trader_code", Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE},
				new[] {"member_id", memberId},
				new[] {"authentication_key", authenticationKey},
				new[] {"check_sum", PaymentYamatoKwcCheckSumCreater.CreateForAuth(memberId, authenticationKey) },
				new[] {"card_key", cardKey},
				new[] {"last_credit_date", lastCreditDate.ToString("yyyyMMddHHmmss")},
				new[] {"reserve_1", this.Reserve1},
			};
			return param;
		}
	}
}
