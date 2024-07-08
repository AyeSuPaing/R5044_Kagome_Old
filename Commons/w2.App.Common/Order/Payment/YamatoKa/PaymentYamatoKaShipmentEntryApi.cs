/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 出荷情報依頼APIクラス(PaymentYamatoKaShipmentEntryApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common.Order.Payment;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 出荷情報依頼APIクラス
	/// </summary>
	public class PaymentYamatoKaShipmentEntryApi : PaymentYamatoKaBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentYamatoKaShipmentEntryApi()
			: base(@"KAASL0010APIAction_execute")
		{
		}

		/// <summary>
		/// 実行 ヤマト決済(後払い) 出荷情報依頼
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="paymentNo">送り状番号</param>
		/// <param name="processDiv">処理区分</param>
		/// <param name="shipYmd">出荷予定日</param>
		/// <param name="beforePaymentNo">変更前送り状番号</param>
		/// <returns>実行結果</returns>
		public bool Exec(string paymentOrderId,
			string paymentNo,
			PaymentYamatoKaProcessDiv processDiv,
			string shipYmd,
			string beforePaymentNo)
		{
			// リクエストパラメータ作成
			var requestParam = CreateParam(paymentOrderId, paymentNo, processDiv, shipYmd, beforePaymentNo);

			// 接続・レスポンス取得
			var responseString = this.PostHttpRequest(requestParam);
			this.ResponseDataInner = new PaymentYamatoKaShipmentEntryResponseData(responseString);

			// 成功判定
			var result = this.ResponseDataInner.IsSuccess;

			WriteLog(
				result
					? LogCreator.CreateMessageWithRequestData(
						StringUtility.ToEmpty(this.ResponseDataInner.RequestDate),
						StringUtility.ToEmpty(this.ResponseDataInner.OrderNo))
					: LogCreator.CreateErrorMessageWithRequestData(
						StringUtility.ToEmpty(this.ResponseDataInner.RequestDate),
						StringUtility.ToEmpty(this.ResponseDataInner.ErrorCode),
						StringUtility.ToEmpty(this.ResponseDataInner.ErrorMessages)) + "\t" + LogCreator.CreateMessage(this.ResponseDataInner.OrderNo, ""),
				result,
				"",
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
				PaymentFileLogger.PaymentProcessingType.ShippingInformationRequest);

			return result;
		}

		/// <summary>
		/// パラメータ作成 ヤマト決済(後払い) 出荷情報依頼
		/// </summary>
		/// <param name="paymentOrderId">決済注文</param>
		/// <param name="paymentNo">送り状番号</param>
		/// <param name="processDiv">処理区分</param>
		/// <param name="shipYmd">出荷予定日</param>
		/// <param name="beforePaymentNo">変更前送り状番号</param>
		/// <returns>パラメータ</returns>
		private string[][] CreateParam(string paymentOrderId,
			string paymentNo,
			PaymentYamatoKaProcessDiv processDiv,
			string shipYmd,
			string beforePaymentNo)
		{
			var param = new[] 
				{
					new[] {"ycfStrCode", this.YcfStrCode},
					new[] {"orderNo", paymentOrderId},
					new[] {"paymentNo", paymentNo},
					new[] {"processDiv", ((int)processDiv).ToString()},
					new[] {"shipYmd", shipYmd},
					new[] {"beforePaymentNo", beforePaymentNo},
					new[] {"requestDate", DateTime.Now.ToString("yyyyMMddHHmmss")},
					new[] {"password", this.Password},
				};
			return param;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentYamatoKaShipmentEntryResponseData ResponseData { get { return (PaymentYamatoKaShipmentEntryResponseData)this.ResponseDataInner; } }
	}
}
