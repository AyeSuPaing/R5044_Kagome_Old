/*
=========================================================================================================
  Module      : ヤマトKWC 出荷情報登録API(PaymentYamatoKwcShipmentEntryApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using w2.App.Common.Order.Payment.YamatoKwc.Helper;

namespace w2.App.Common.Order.Payment.YamatoKwc
{
	/// <summary>
	/// ヤマトKWC 出荷情報登録API
	/// </summary>
	public class PaymentYamatoKwcShipmentEntryApi : PaymentYamatoKwcApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reserve1">予備項目設定値(テスト用）</param>
		public PaymentYamatoKwcShipmentEntryApi(string reserve1 = "")
			: base(PaymentYamatoKwcFunctionDiv.E01, reserve1)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="slipNo">送り状番号</param>
		/// <param name="deliveryServiceCode">配送サービスコード</param>
		/// <returns>結果</returns>
		public PaymentYamatoKwcShipmentEntryResponseData Exec(string paymentOrderId, string slipNo, string deliveryServiceCode)
		{
			var param = CreateParam(paymentOrderId, slipNo, deliveryServiceCode);

			var resultString = PostHttpRequest(param);
			var responseData = new PaymentYamatoKwcShipmentEntryResponseData(resultString);
			WriteLog(
				Constants.FLG_PAYMENT_PAYMENT_ID_NOPAYMENT,
				PaymentFileLogger.PaymentProcessingType.ShippingInformationRegistratio,
				responseData.Success,
				new KeyValuePair<string, string>("paymentOrderId", paymentOrderId),
				new KeyValuePair<string, string>("slipNo", slipNo),
				new KeyValuePair<string, string>("deliveryServiceCode", deliveryServiceCode));
			return responseData;
		}

		/// <summary>
		/// パラメタ作成
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="slipNo">送り状番号</param>
		/// <param name="deliveryServiceCode">配送サービスコード</param>
		/// <returns>パラメタ</returns>
		private string[][] CreateParam(string paymentOrderId, string slipNo, string deliveryServiceCode)
		{
			var param = new[]
			{
				new[] {"function_div", this.FunctionDiv.ToString()},
				new[] {"trader_code", Constants.PAYMENT_SETTING_YAMATO_KWC_TRADER_CODE},
				new[] {"order_no", paymentOrderId},
				new[] {"slip_no", slipNo},
				new[] {"delivery_service_code", deliveryServiceCode},
				new[] {"reserve_1", this.Reserve1},
			};
			return param;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentYamatoKwcShipmentEntryResponseData ResponseData { get; private set; }
	}
}
