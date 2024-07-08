/*
=========================================================================================================
  Module      : ゼウスセキュアリンク決済API(ZeusSecureLinkApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Extensions.Currency;
using w2.Common.Logger;

namespace w2.App.Common.Order.Payment.Zeus
{
	/// <summary>
	/// ゼウスセキュアリンク決済
	/// </summary>
	public class ZeusSecureLinkApi : ZeusApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZeusSecureLinkApi()
			: base(Constants.PAYMENT_SETTING_ZEUS_SECURE_LINK_SERVER_URL)
		{
		}

		/// <summary>
		/// クレジットカード登録向け実行
		/// </summary>
		/// <param name="tokenKey">トークンキー（既存カード利用の場合はnull）</param>
		/// <param name="telNo">ユーザーの電話番号</param>
		/// <param name="emal">ユーザーのe-mailアドレス</param>
		/// <param name="sendId">フリーパラメータ</param>
		/// <returns>決済結果</returns>
		public Result ExecForRegisterCreditCard(
			string tokenKey,
			string telNo,
			string emal,
			string sendId)
		{
			var result = Exec(tokenKey, 0, telNo, emal, sendId, "01");
			return result;
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="tokenKey">トークンキー（既存カード利用の場合はnull）</param>
		/// <param name="money">決済金額（与信だけ取る場合は"0"）</param>
		/// <param name="telNo">ユーザーの電話番号</param>
		/// <param name="emal">ユーザーのe-mailアドレス</param>
		/// <param name="sendId">フリーパラメータ</param>
		/// <param name="div">支払回数</param>
		/// <returns>決済結果</returns>
		public Result Exec(string tokenKey, decimal money, string telNo, string emal, string sendId, string div)
		{
			// URL作成
			var url = CreateUrl(tokenKey, money, telNo, emal, sendId, div);

			// 決済実行
			Result result;
			var response = GetResponse(url);
			var isSuccess = response.Contains("Success_order");

			var responses = response
				.Replace("\r\n", "\n")
				.Split('\n')
				.Where(res => res.Contains("="))
				.ToDictionary(str => str.Split('=')[0], str => str.Split('=')[1]);

			if (isSuccess)
			{
				// 成功時：ゼウス決済種別ID取得
				result = new Result(true)
				{
					ZeusOrderId = responses.First(kvp => (kvp.Key == "ordd")).Value,
				};
			}
			else
			{
				// 失敗時
				result = new Result(false)
				{
					ErrorMessage = response,
					ErrorCode = responses.FirstOrDefault(kvp => (kvp.Key == "err_code")).Value,
				};
			}

			// ログ格納処理
			WriteLog(
				PaymentFileLogger.PaymentProcessingType.OthoriProcessing,
				result.Success,
				new KeyValuePair<string, string>("tokenKey", tokenKey),
				new KeyValuePair<string, string>("sendId", sendId),
				new KeyValuePair<string, string>("money", money.ToPriceString()),
				new KeyValuePair<string, string>("div", div),
				new KeyValuePair<string, string>("response", response));
			return result;
		}

		/// <summary>
		/// URL作成
		/// </summary>
		/// <param name="tokenKey">トークンキー（既存カード利用の場合はnull）</param>
		/// <param name="money">決済金額（与信だけ取る場合は"0"）</param>
		/// <param name="telNo">ユーザーの電話番号</param>
		/// <param name="emal">ユーザーのe-mailアドレス</param>
		/// <param name="sendId">フリーパラメータ</param>
		/// <param name="div">支払回数</param>
		/// <returns>URL</returns>
		private string CreateUrl(string tokenKey, decimal money, string telNo, string emal, string sendId, string div)
		{
			var url = new StringBuilder(this.ServerUrl);
			url.Append("?").Append("clientip").Append("=").Append(this.ClientIP); // 加盟店IPコード
			if (string.IsNullOrEmpty(tokenKey) == false)
			{
				url.Append("&").Append("token_key").Append("=").Append(tokenKey);
			}
			else
			{
				url.Append("&").Append("cardnumber").Append("=").Append("9999999999999999");
			}
			url.Append("&").Append("money").Append("=").Append(money.ToPriceString());
			url.Append("&").Append("send").Append("=").Append(KBN_SEND_YEN);
			url.Append("&").Append("telno").Append("=").Append(telNo.Replace("-", ""));
			url.Append("&").Append("email").Append("=").Append(ReduceString(System.Web.HttpUtility.UrlEncode(emal), 50));
			url.Append("&").Append("sendid").Append("=").Append(sendId); // 一意のID(25文字以内)
			url.Append("&").Append("endpoint").Append("=").Append("");
			url.Append("&").Append("pubsec").Append("=").Append(KBN_PUBSEC_YES);
			url.Append("&").Append("printord").Append("=").Append(KBN_PRINTORD_YES);
			if (string.IsNullOrEmpty(div) == false) url.Append("&").Append("div").Append("=").Append(div);
			url.Append("&").Append("return_value").Append("=").Append(KBN_RPERRCODE_YED);
			return url.ToString();
		}

		/// <summary>
		/// セキュアリンク結果
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
			/// <summary>エラーコード</summary>
			public string ErrorCode { get; set; }
			/// <summary>エラー種別コード（エラーコードの末尾3桁）</summary>
			public string ErrorTypeCode
			{
				get
				{
					if (string.IsNullOrEmpty(this.ErrorCode)) return "";

					var errorTypeCode = this.ErrorCode.Substring(this.ErrorCode.Length - 3);
					return errorTypeCode;
				}
			}
		}
	}
}
