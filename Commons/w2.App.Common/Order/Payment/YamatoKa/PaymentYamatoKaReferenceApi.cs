/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 取引状況照会APIクラス(PaymentYamatoKaReferenceApi.cs)
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
	/// ヤマト決済(後払い) 取引状況照会APIクラス
	/// </summary>
	public class PaymentYamatoKaReferenceApi : PaymentYamatoKaBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentYamatoKaReferenceApi()
			: base(@"KAAST0010APIAction_execute")
		{
		}

		/// <summary>
		/// 実行 ヤマト決済(後払い) 取引状況照会
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>実行結果</returns>
		public bool Exec(string paymentOrderId)
		{
			// リクエストパラメータ作成
			var requestParam = CreateParam(paymentOrderId);

			// 接続・レスポンス取得
			var responseString = this.PostHttpRequest(requestParam);
			this.ResponseDataInner = new PaymentYamatoKaReferenceResponseData(responseString);

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
				PaymentFileLogger.PaymentProcessingType.ChargeChangeForReductionRequest);

			return result;
		}

		/// <summary>
		/// パラメータ作成 ヤマト決済(後払い) 取引状況照会
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <returns>パラメータ</returns>
		private string[][] CreateParam(string paymentOrderId)
		{
			var param = new[]
				{
					new[] {"ycfStrCode", this.YcfStrCode},
					new[] {"orderNo", paymentOrderId},
					new[] {"requestDate", DateTime.Now.ToString("yyyyMMddHHmmss")},
					new[] {"password", this.Password},
				};
			return param;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentYamatoKaReferenceResponseData ResponseData { get { return (PaymentYamatoKaReferenceResponseData)this.ResponseDataInner; } }
	}
}
