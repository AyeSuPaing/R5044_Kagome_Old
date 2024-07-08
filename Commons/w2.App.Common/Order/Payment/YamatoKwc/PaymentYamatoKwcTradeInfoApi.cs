/*
=========================================================================================================
  Module      : ヤマトKWC 取引情報照会API(PaymentYamatoKwcTradeInfoApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC 取引情報照会API
	/// </summary>
	public class PaymentYamatoKwcTradeInfoApi : PaymentYamatoKwcApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentYamatoKwcTradeInfoApi()
			: base(PaymentYamatoKwcFunctionDiv.E04, "")
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>結果</returns>
		public PaymentYamatoKwcTradeInfoResponseData Exec(string paymentOrderId)
		{
			var param = CreateParam(paymentOrderId);

			var resultString = PostHttpRequest(param);
			var responseData = new PaymentYamatoKwcTradeInfoResponseData(resultString);
			WriteLog(
				"",
				PaymentFileLogger.PaymentProcessingType.TransactionInformationInquiry,
				responseData.Success,
				new KeyValuePair<string, string>("paymentOrderId", paymentOrderId),
				new KeyValuePair<string, string>("ErrorInfoForLog", responseData.ErrorInfoForLog));
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
			};
			return param;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentYamatoKwcTradeInfoResponseData ResponseData { get; private set; }
	}
}
