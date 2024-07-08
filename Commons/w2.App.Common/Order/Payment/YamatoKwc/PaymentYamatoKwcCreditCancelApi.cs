/*
=========================================================================================================
  Module      : ヤマトKWC クレジットキャンセルAPI(PaymentYamatoKwcCreditCancelApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC クレジットキャンセルAPI
	/// </summary>
	public class PaymentYamatoKwcCreditCancelApi : PaymentYamatoKwcApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reserve1">予備項目設定値(テスト用）</param>
		public PaymentYamatoKwcCreditCancelApi(string reserve1 = "")
			: base(PaymentYamatoKwcFunctionDiv.A06, reserve1)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>結果</returns>
		public PaymentYamatoKwcCreditCancelResponseData Exec(string paymentOrderId)
		{
			var param = CreateParam(paymentOrderId);

			var resultString = PostHttpRequest(param);
			var responseData = new PaymentYamatoKwcCreditCancelResponseData(resultString);

			WriteLog(
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentProcessingType.CreditChancel,
				responseData.Success,
				new KeyValuePair<string, string>("paymentOrderId", paymentOrderId));

			return responseData;
		}

		/// <summary>
		/// パラメタ作成
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>パラメタ</returns>
		private string[][] CreateParam(string paymentOrderId)
		{
			var param = new[]
			{
				new[] {"function_div", this.FunctionDiv.ToString()},
				new[] {"trader_code", Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE},
				new[] {"order_no", paymentOrderId},
				new[] {"reserve_1", this.Reserve1},
			};
			return param;
		}
	}
}
