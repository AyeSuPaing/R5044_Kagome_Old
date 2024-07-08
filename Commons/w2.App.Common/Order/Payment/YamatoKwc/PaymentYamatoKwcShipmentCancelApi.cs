/*
=========================================================================================================
  Module      : ヤマトKWC 出荷情報取消API(PaymentYamatoKwcShipmentCancelApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC 出荷情報取消API
	/// </summary>
	public class PaymentYamatoKwcShipmentCancelApi : PaymentYamatoKwcApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reserve1">予備項目設定値(テスト用）</param>
		public PaymentYamatoKwcShipmentCancelApi(string reserve1 = "")
			: base(PaymentYamatoKwcFunctionDiv.E02, reserve1)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="slipNo">送り状番号</param>
		/// <returns>結果</returns>
		public PaymentYamatoKwcShipmentCancelResponseData Exec(string paymentOrderId, string slipNo)
		{
			var param = CreateParam(paymentOrderId, slipNo);

			var resultString = PostHttpRequest(param);
			var responseData = new PaymentYamatoKwcShipmentCancelResponseData(resultString);
			WriteLog(
				Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT,
				PaymentFileLogger.PaymentProcessingType.CancelShippingInformation,
				responseData.Success,
				new KeyValuePair<string, string>("paymentOrderId", paymentOrderId),
				new KeyValuePair<string, string>("slipNo", slipNo));
			return responseData;
		}

		/// <summary>
		/// パラメタ作成
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="slipNo">送り状番号</param>
		/// <returns>パラメタ</returns>
		private string[][] CreateParam(string paymentOrderId, string slipNo)
		{
			var param = new[]
			{
				new[] {"function_div", this.FunctionDiv.ToString()},
				new[] {"trader_code", Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE},
				new[] {"order_no", paymentOrderId},
				new[] {"slip_no", slipNo},
				new[] {"reserve_1", this.Reserve1},
			};
			return param;
		}
	}
}
