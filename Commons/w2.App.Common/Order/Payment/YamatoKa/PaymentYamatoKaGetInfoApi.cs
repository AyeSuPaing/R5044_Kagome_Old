/*
=========================================================================================================
  Module      : ヤマト決済(後払い) 請求書印字情報取得APIクラス(PaymentYamatoKaGetInfoApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Order.Payment;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) 請求書印字情報取得APIクラス
	/// </summary>
	public class PaymentYamatoKaGetInfoApi : PaymentYamatoKaBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentYamatoKaGetInfoApi()
			: base(@"KAASD0010APIAction_execute")
		{
		}

		/// <summary>
		/// 実行 ヤマト決済(後払い) 請求書印字情報取得
		/// </summary>
		/// <param name="orderNo">受注番号</param>
		/// <returns>実行結果</returns>
		public bool Exec(string orderNo)
		{
			// リクエストパラメータ作成
			var requestParam = CreateParam(orderNo);

			// 接続・レスポンス取得
			var responseString = this.PostHttpRequest(requestParam);
			this.ResponseDataInner = new PaymentYamatoKaGetInfoResponseData(responseString);

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
						StringUtility.ToEmpty(this.ResponseDataInner.ErrorMessages)) + "\t" + LogCreator.CreateMessage(orderNo, ""),
				result,
				"",
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
				PaymentFileLogger.PaymentProcessingType.AcquisitionOfInvoicePrintInformation);

			return result;
		}

		/// <summary>
		/// パラメータ作成 ヤマト決済(後払い) 請求書印字情報取得
		/// </summary>
		/// <param name="orderNo">受注番号</param>
		/// <returns>パラメータ</returns>
		private string[][] CreateParam(string orderNo)
		{
			var param = new[]
				{
					new[] {"ycfStrCode", this.YcfStrCode},
					new[] {"orderNo", orderNo},
					new[] {"requestDate", DateTime.Now.ToString("yyyyMMddHHmmss")},
					new[] {"password", this.Password},
				};
			return param.ToArray();
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentYamatoKaGetInfoResponseData ResponseData { get { return (PaymentYamatoKaGetInfoResponseData)this.ResponseDataInner; } }
	}
}
