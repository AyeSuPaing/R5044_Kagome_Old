/*
=========================================================================================================
  Module      : ヤマト決済(後払い) SMS認証番号判定APIクラス(PaymentYamatoKaSmsAuthApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Order.Payment;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// ヤマト決済(後払い) SMS認証番号判定APIクラス
	/// </summary>
	public class PaymentYamatoKaSmsAuthApi : PaymentYamatoKaBaseApi
	{
		/// <summary>
		/// 実行結果
		/// </summary>
		public enum ExecResult
		{
			/// <summary>認証結果OK</summary>
			Ok,
			/// <summary>認証コード再入力</summary>
			Reenter,
			/// <summary>認証結果NG</summary>
			Ng,
		}

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public PaymentYamatoKaSmsAuthApi()
			: base(@"KAASA0020APIAction_execute")
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="orderNo">受注番号</param>
		/// <param name="ninCode">認証コード</param>
		/// <returns>実行結果</returns>
		public ExecResult Exec(
			string orderNo,
			string ninCode)
		{
			var requestParam = CreateParam(orderNo, ninCode);

			// 接続・レスポンス取得
			var responseString = this.PostHttpRequest(requestParam);
			this.ResponseDataInner = this.ResponseData = new PaymentYamatoKaSmsAuthResponseData(responseString);

			// 成功判定
			var result = EvalResult();

			WriteLog(
				(result != ExecResult.Ng)
					? LogCreator.CreateMessageWithRequestData(
						StringUtility.ToEmpty(this.ResponseDataInner.RequestDate),
						StringUtility.ToEmpty(this.ResponseDataInner.OrderNo))
					: LogCreator.CreateErrorMessageWithRequestData(
						StringUtility.ToEmpty(this.ResponseDataInner.RequestDate),
						StringUtility.ToEmpty(this.ResponseDataInner.ErrorCode),
						StringUtility.ToEmpty(this.ResponseDataInner.ErrorMessages)) + "\t"
					+ LogCreator.CreateMessage(this.ResponseDataInner.OrderNo, ""),
				(result != ExecResult.Ng),
				"",
				Constants.FLG_PAYMENT_PAYMENT_ID_CVS_DEF,
				PaymentFileLogger.PaymentProcessingType.SmsAuth);

			return result;
		}

		/// <summary>
		/// パラメータ作成 ヤマト決済(後払い) SMS認証番号判定
		/// </summary>
		/// <param name="orderNo">受注番号</param>
		/// <param name="ninCode">認証コード</param>
		/// <returns>パラメータ</returns>
		private string[][] CreateParam(
			string orderNo,
			string ninCode)
		{
			var param = new[]
			{
				new[] { "ycfStrCode", this.YcfStrCode }, // 基本情報
				new[] { "orderNo", orderNo },
				new[] { "ninCode", ninCode },
				new[] { "requestDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
				new[] { "password", this.Password },
			};
			return param.ToArray();
		}

		/// <summary>
		/// 判定
		/// </summary>
		/// <returns>判定結果</returns>
		private ExecResult EvalResult()
		{
			PaymentYamatoKaSmsAuthResponseData.ResultValue value;
			var isConvertionSuccess = Enum.TryParse(this.ResponseData.Result, out value);

			var result = ((isConvertionSuccess && this.ResponseData.IsSuccess)
				? ((value == PaymentYamatoKaSmsAuthResponseData.ResultValue.Ok)
					? ExecResult.Ok
					: ((value == PaymentYamatoKaSmsAuthResponseData.ResultValue.Mismatch)
						? ExecResult.Reenter
						: ExecResult.Ng))
				: ExecResult.Ng);

			return result;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentYamatoKaSmsAuthResponseData ResponseData { get; set; }
	}
}
