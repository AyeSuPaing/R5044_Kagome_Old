/*
=========================================================================================================
  Module      : 決済受信結果 (ReceivedPaymentResult.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using w2.Common.Extensions;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment.Paypay
{
	/// <summary>
	/// 決済受信結果
	/// </summary>
	public class ReceivedPaymentResult
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="httpContext">HTTPコンテキスト</param>
		public ReceivedPaymentResult(HttpContext httpContext)
		{
			var param = (httpContext.Request.HttpMethod == "POST")
				? httpContext.Request.Form.ToPairs().ToDictionary(kvp => kvp.Key, kvp => kvp.Value)
				: httpContext.Request.Params.ToPairs().ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

			this.ShopId = param["ShopID"];
			this.PaymentOrderId = param["OrderID"];
			this.Status = param["Status"];
			this.TranDate = DateTime.ParseExact(param["TranDate"], "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
			this.PaypayAcceptCode = param.ContainsKey("PaypayAcceptCode")
				? param["PaypayAcceptCode"]
				: string.Empty;
			this.Errors = param.ContainsKey("ErrCode")
				? param["ErrCode"]
					.Split('|')
					.Zip(
						param["ErrInfo"].Split('|'),
						(errCode, errInfo) => new PaypayGmoResponse.Error
						{
							ErrorCode = errCode,
							ErrorInfo = errInfo,
						})
					.Where(
						err =>
							((string.IsNullOrEmpty(err.ErrorCode) == false)
								&& (string.IsNullOrEmpty(err.ErrorInfo) == false)))
					.ToArray()
				: Enumerable.Empty<PaypayGmoResponse.Error>().ToArray();

			FileLogger.Write(
				"Paypay",
				string.Format(
					"ShopId:{0} PaymentOrderId:{1} Status:{2} TranDate:{3} Errors:{4}",
					this.ShopId,
					this.PaymentOrderId,
					this.Status,
					this.TranDate,
					GetErrorMessages()));
		}

		/// <summary>
		/// エラーをフォーマット
		/// </summary>
		/// <returns>フォーマットされたエラー文字列</returns>
		public string GetErrorMessages()
		{
			var errorMessage = new StringBuilder();
			foreach (var error in this.Errors)
			{
				var message = ErrorCodes.GetInstance().GetErrorDefinition(error.ErrorCode, error.ErrorInfo).Message;
				var errorCode = string.Format("{0} | {1}: ", error.ErrorCode, error.ErrorInfo);
				errorMessage.Append(errorCode)
					.Append(message)
					.Append(Environment.NewLine);
			}
			return errorMessage.ToString();
		}

		/// <summary>店舗ID</summary>
		public string ShopId { get; set; }
		/// <summary>決済注文ID</summary>
		public string PaymentOrderId { get; set; }
		/// <summary>ステータス</summary>
		public string Status { get; set; }
		/// <summary>処理日付</summary>
		public DateTime TranDate { get; set; }
		/// <summary>PayPay承諾番号</summary>
		public string PaypayAcceptCode { get; set; }
		/// <summary>検証文字列（MD5）</summary>
		public string CheckString { get; set; }
		/// <summary>エラー</summary>
		public PaypayGmoResponse.Error[] Errors { get; set; }
	}
}
