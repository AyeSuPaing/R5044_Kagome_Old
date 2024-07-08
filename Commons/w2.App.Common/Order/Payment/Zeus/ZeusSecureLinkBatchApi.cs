/*
=========================================================================================================
  Module      : ゼウスセキュアリンクバッチ決済API(ZeusSecureLinkBatchApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;
using w2.App.Common.Extensions.Currency;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment.Zeus
{
	/// <summary>
	/// ゼウスセキュアリンクバッチ決済API
	/// </summary>
	public class ZeusSecureLinkBatchApi : ZeusApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZeusSecureLinkBatchApi()
			: base(Constants.PAYMENT_SETTING_ZEUS_SECURE_LINK_SERVER_URL)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="money">決済金額（与信だけ取る場合は"0"）</param>
		/// <param name="telNo">ユーザーの電話番号</param>
		/// <param name="emal">ユーザーのe-mailアドレス</param>
		/// <param name="sendId">フリーパラメータ</param>
		/// <param name="div">支払回数</param>
		/// <returns>決済結果</returns>
		public Result Exec(
			decimal money,
			string telNo,
			string emal,
			string sendId,
			string div)
		{
			// URL作成
			var url = CreateUrl(money, telNo, emal, sendId, div);

			// 決済実行
			Result result;
			var response = GetResponse(url);
			var isSuccess = response.Contains("Success_order");

			// ログ格納処理
			PaymentFileLogger.WritePaymentLog(
				isSuccess,
				Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT,
				PaymentFileLogger.PaymentType.Zeus,
				PaymentFileLogger.PaymentProcessingType.ZeusSecureBatchPaymentExec,
				isSuccess ? "" : response,
				new Dictionary<string, string>
				{
					{"zeus_order_id", response[1].ToString()} // ゼウス注文ID
				});

			if (response.Contains("Success_order"))
			{
				// 成功時：ゼウス決済種別ID取得
				var responses = response.Split('\n');
				result = new Result(true)
				{
					ZeusOrderId = responses[1],
				};
			}
			else
			{
				// 失敗時
				result = new Result(false)
				{
					ErrorMessage = response,
				};
				AppLogger.WriteError(result.ErrorMessage);
			}

			WriteLog(
				PaymentFileLogger.PaymentProcessingType.OthoriProcessing,
				result.Success,
				new KeyValuePair<string, string>("sendId", sendId),
				new KeyValuePair<string, string>("money", money.ToPriceString()),
				new KeyValuePair<string, string>("response", response));
			return result;
		}

		/// <summary>
		/// URL作成
		/// </summary>
		/// <param name="money">決済金額（与信だけ取る場合は"0"）</param>
		/// <param name="telNo">ユーザーの電話番号</param>
		/// <param name="emal">ユーザーのe-mailアドレス</param>
		/// <param name="sendId">フリーパラメータ</param>
		/// <param name="div">支払回数</param>
		/// <returns>URL</returns>
		private string CreateUrl(decimal money, string telNo, string emal, string sendId, string div)
		{
			var url = new StringBuilder(this.ServerUrl);
			url.Append("?").Append("clientip").Append("=").Append(this.ClientIP);
			url.Append("&").Append("cardnumber").Append("=").Append("9999999999999999");	//clientip + telno + sendidでカード情報を検索
			url.Append("&").Append("expyy").Append("=").Append("00");
			url.Append("&").Append("expmm").Append("=").Append("00");
			url.Append("&").Append("telno").Append("=").Append(telNo.Replace("-", ""));
			url.Append("&").Append("email").Append("=").Append(ReduceString(System.Web.HttpUtility.UrlEncode(emal), 50));
			url.Append("&").Append("sendid").Append("=").Append(sendId); // 一意のID(25文字以内)
			url.Append("&").Append("money").Append("=").Append(money.ToPriceString());
			url.Append("&").Append("send").Append("=").Append(KBN_SEND_YEN);
			url.Append("&").Append("printord").Append("=").Append(KBN_PRINTORD_YES);
			url.Append("&").Append("pubsec").Append("=").Append(KBN_PUBSEC_YES);
			if (string.IsNullOrEmpty(div) == false) url.Append("&").Append("div").Append("=").Append(div);
			return url.ToString();
		}

		/// <summary>
		/// セキュアリンクバッチ結果
		/// </summary>
		public class Result
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="success">成功したか</param>
			public Result(bool success)
			{
				this.Success = success;
			}

			/// <summary>成功したか</summary>
			public bool Success { get; private set; }
			/// <summary>ゼウス注文ID</summary>
			public string ZeusOrderId { get; set; }
			/// <summary>エラーメッセージ</summary>
			public string ErrorMessage { get; set; }
		}
	}
}
