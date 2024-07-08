/*
=========================================================================================================
  Module      : ヤマトKWC クレジット金額変更API(PaymentYamatoKwcCreditChangePriceApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC クレジット金額変更API
	/// </summary>
	public class PaymentYamatoKwcCreditChangePriceApi : PaymentYamatoKwcApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reserve1">予備項目設定値(テスト用）</param>
		public PaymentYamatoKwcCreditChangePriceApi(string reserve1 = "")
			: base(PaymentYamatoKwcFunctionDiv.A07, reserve1)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="newPrice">変更後決済金額</param>
		/// <returns>結果</returns>
		public PaymentYamatoKwcCreditChangePriceResponseData Exec(string paymentOrderId, decimal newPrice)
		{
			var param = CreateParam(paymentOrderId, newPrice);

			var resultString = PostHttpRequest(param);
			var responseData = new PaymentYamatoKwcCreditChangePriceResponseData(resultString);
			WriteLog(
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentProcessingType.ChangeCreditAmount,
				responseData.Success,
				new KeyValuePair<string, string>("paymentOrderId", paymentOrderId),
				new KeyValuePair<string, string>("newPrice", newPrice.ToPriceString()));
			return responseData;
		}

		/// <summary>
		/// パラメタ作成
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="newPrice">変更後決済金額</param>
		/// <returns>パラメタ</returns>
		private string[][] CreateParam(string paymentOrderId, decimal newPrice)
		{
			var param = new[]
			{
				new[] {"function_div", this.FunctionDiv.ToString()},
				new[] {"trader_code", Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE},
				new[] {"order_no", paymentOrderId},
				new[] {"new_price", newPrice.ToPriceString()},
				new[] {"reserve_1", this.Reserve1},
			};
			return param;
		}
	}
}
