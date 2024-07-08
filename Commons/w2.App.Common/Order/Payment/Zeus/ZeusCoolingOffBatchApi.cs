/*
=========================================================================================================
  Module      : クーリングオフ（売上確定）バッチAPI(ZeusCoolingOffBatchApi.cs)
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
	/// クーリングオフ（売上確定）バッチAPI
	/// </summary>
	public class ZeusCoolingOffBatchApi : ZeusApiBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ZeusCoolingOffBatchApi()
			: base(Constants.PAYMENT_SETTING_ZEUS_SECURE_LINK_SERVER_URL)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="king">実売りにするときの決済金額</param>
		/// <param name="date">売上処理日(yyyymmdd)</param>
		/// <param name="ordd">仮売り時のオーダー番号</param>
		/// <returns>実行結果</returns>
		public Result Exec(decimal king, string date, string ordd)
		{
			// URL作成
			var url = CreateUrl(king, date, ordd);

			// 実行
			Result result;
			var response = GetResponse(url);
			if (response.Contains("Success_order"))
			{
				result = new Result(true);
			}
			else
			{
				AppLogger.WriteError(response);
				result = new Result(false)
				{
					ErrorMessage = response
				};
			}
			WriteLog(
				PaymentFileLogger.PaymentProcessingType.Cancel,
				result.Success,
				new KeyValuePair<string, string>("ordd", ordd),
				new KeyValuePair<string, string>("king", king.ToPriceString()));
			return result;
		}

		/// <summary>
		/// URL作成
		/// </summary>
		/// <param name="king">実売りにするときの決済金額</param>
		/// <param name="date">売上処理日(yyyymmdd)</param>
		/// <param name="ordd">仮売り時のオーダー番号</param>
		/// <returns>URL</returns>
		private string CreateUrl(decimal king, string date, string ordd)
		{
			var url = new StringBuilder(this.ServerUrl);
			url.Append("?").Append("clientip").Append("=").Append(this.ClientIP);
			url.Append("&").Append("king").Append("=").Append(king.ToPriceString());
			url.Append("&").Append("date").Append("=").Append(date);
			url.Append("&").Append("ordd").Append("=").Append(ordd);
			url.Append("&").Append("autype").Append("=").Append("sale"); // 基本実売りにする
			return url.ToString();
		}

		/// <summary>
		/// クーリングオフバッチ実行結果
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
			/// <summary>エラーメッセージ</summary>
			public string ErrorMessage { get; set; }
		}
	}
}
