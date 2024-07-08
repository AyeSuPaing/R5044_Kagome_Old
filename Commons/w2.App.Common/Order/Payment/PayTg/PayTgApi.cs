/*
=========================================================================================================
  Module      : PayTgApiクラス(PayTgApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web.UI;
using Newtonsoft.Json;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.PayTg
{
	/// <summary>
	/// PayTgApiクラス
	/// </summary>
	public class PayTgApi
	{
		/// <summary>
		///デフォルト コンストラクタ
		/// </summary>
		public PayTgApi()
		{
			this.Data = new PostDataPayTg();
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="amount">金額</param>
		/// <param name="jpo">支払区分</param>
		/// <param name="count">支払回数</param>
		public PayTgApi(string paymentOrderId, string amount = null, string jpo = null, string count = null)
		{
			this.Data = new PostDataPayTg
			{
				OrderId = paymentOrderId,
				Amount = amount,
				Jpo = jpo,
				Count = count
			};
		}

		/// <summary>
		/// ポスト情報作成
		/// </summary>
		/// <returns>ポスト情報</returns>
		public string CreatePostData()
		{
			var json = JsonConvert.SerializeObject(this.Data);
			return json;
		}

		/// <summary>
		/// レスポンス処理
		/// </summary>
		/// <param name="response">レスポンス</param>
		public void ParseResponse(string response)
		{
			// レスポンス文字列から結果取得
			this.Result = new ResponseResult(response);
			this.ErrorMessages = this.Result.ErrorMessages;

			// POST送信＆レスポンスのログ出力l
			var postInfo = string.Join(",",
				this.Data.GetType().GetProperties()
					.Where(p => (p.GetValue(this.Data) != null) && !string.IsNullOrEmpty(p.GetValue(this.Data).ToString()))
					.Select(p => string.Concat(p.Name, "=", ((string)p.GetValue(this.Data)).Replace("\n", "")))
					.ToArray());

			var logContent = string.Format(
				"\tPostData=[{0}]\nResult=[{1}{2}]",
				postInfo,
				response,
				(string.IsNullOrEmpty(this.ErrorMessages) == false) ? (",ErrorMessage=" + this.ErrorMessages) : string.Empty);

			PaymentFileLogger.WritePaymentLog(
				this.Result.IsSuccess,
				"",
				PaymentFileLogger.PaymentType.PayTg,
				PaymentFileLogger.PaymentProcessingType.Unknown,
				logContent,
				new Dictionary<string, string>
				{
					{ PayTgConstants.PARAM_ORDERID, this.Data.OrderId },
				});
		}

		#region プロパティ
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessages { get; set; }
		/// <summary>結果</summary>
		public ResponseResult Result { get; set; }
		/// <summary>ポストデータ</summary>
		public PostDataPayTg Data { get; set; }
		#endregion

		/// <summary>
		/// レスポンス結果クラス
		/// </summary>
		public class ResponseResult
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="responseString">レスポンス文字列</param>
			public ResponseResult(string responseString)
			{
				// シリアライザ
				var serializer = new DataContractJsonSerializer(typeof(ResponseData));
				using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(responseString)))
				{
					// JSONデシリアライズ
					this.Response = (ResponseData)serializer.ReadObject(ms);
				}
			}

			/// <summary>成功？（true:成功、false:失敗）</summary>
			public bool IsSuccess
			{
				get
				{
					var mStatus = this.Response.Mstatus;
					var result = false;
					if (string.IsNullOrEmpty(mStatus) == false) result = (mStatus == PayTgConstants.PAYTG_STATUS_SUCCESS);
					return result;
				}
			}
			/// <summary>エラーメッセージ</summary>
			public string ErrorMessages
			{
				get
				{
					if (this.IsSuccess) return string.Empty;
					var comResultMsg = ValueText.GetValueText(
						PayTgConstants.VALUETEXT_PARAM_ORDER,
						PayTgConstants.VALUETEXT_PARAM_VERITRANS_COM_RESULT,
						this.Response.ComResult);
					var message = string.Format("{0} {1}", comResultMsg, this.Response.ErrorMsg).Trim();
					return message;
				}
			}
			/// <summary>レスポンス</summary>
			public ResponseData Response { get; set; }
		}
	}
}
