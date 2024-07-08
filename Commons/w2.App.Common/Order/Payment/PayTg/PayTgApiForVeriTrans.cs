/*
=========================================================================================================
  Module      : PayTgApiクラス(PayTgApiForVeriTrans.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Text;
using Newtonsoft.Json;
using w2.Common.Util;

namespace w2.App.Common.Order.Payment.PayTg
{
	/// <summary>
	/// PayTgApiForVeriTransクラス
	/// </summary>
	public class PayTgApiForVeriTrans
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="customerId">連携用のユニークID</param>
		public PayTgApiForVeriTrans(string customerId)
		{
			this.Data = new PostDataVeriTrans
			{
				CustomerId = customerId,
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

			// POST送信＆レスポンスのログ出力
			var postInfo = string.Join(
				",",
				this.Data.GetType().GetProperties()
					.Select(p => string.Concat(p.Name, "=", (string)p.GetValue(this.Data))).ToArray());

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
					{ PayTgConstants.PARAM_CUSTOMERID, this.Data.CustomerId },
				});
		}

		#region プロパティ
		/// <summary>エラーメッセージ</summary>
		public string ErrorMessages { get; set; }
		/// <summary>結果</summary>
		public ResponseResult Result { get; set; }
		/// <summary>ポストデータ</summary>
		public PostDataVeriTrans Data { get; set; }
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
				var serializer = new DataContractJsonSerializer(typeof(ResponseDataVeriTrans));
				using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(responseString)))
				{
					// JSONデシリアライズ
					this.Response = (ResponseDataVeriTrans)serializer.ReadObject(ms);
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
			public ResponseDataVeriTrans Response { get; set; }
		}
	}
}
