/*
=========================================================================================================
  Module      : ゼウスセキュアバッチ取消API(ZeusSecureBatchCancelApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Text;

namespace w2.App.Common.Order.Payment.Zeus
{
	/// <summary>
	/// ゼウスセキュアバッチ取消API
	/// </summary>
	public class ZeusSecureBatchCancelApi : ZeusApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZeusSecureBatchCancelApi()
			: base(Constants.PAYMENT_SETTING_ZEUS_SECURE_LINK_SERVER_URL)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="ordd">決済時のオーダー番号</param>
		/// <returns>決済結果</returns>
		public Result Exec(string ordd)
		{
			// URL作成
			var url = CreateUrl(ordd);

			// 決済実行
			Result result;
			var response = GetResponse(url);
			if (response.Contains("SuccessOK"))
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
			}
			WriteLog(
				PaymentFileLogger.PaymentProcessingType.Cancel,
				result.Success,
				new KeyValuePair<string, string>("ordd", ordd));
			// WriteLogに　new KeyValuePair<string, string>("responses", response)を渡してあげることもできる。
			return result;
		}

		/// <summary>
		/// URL作成
		/// </summary>
		/// <param name="ordd">決済時のオーダー番号</param>
		/// <returns>URL</returns>
		private string CreateUrl(string ordd)
		{
			var url = new StringBuilder(this.ServerUrl);
			url.Append("?").Append("clientip").Append("=").Append(this.ClientIP);
			url.Append("&").Append("return").Append("=").Append("yes");
			url.Append("&").Append("ordd").Append("=").Append(ordd);
			return url.ToString();
		}

		/// <summary>
		/// セキュアバッチ取消結果
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
