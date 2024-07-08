/*
=========================================================================================================
  Module      : Paypay Response (PaypayGmoResponse.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.Paypay
{
	/// <summary>
	/// Paypay Response
	/// </summary>
	public abstract class PaypayGmoResponse : IHttpApiResponseData
	{
		/// <summary>
		/// エラーをフォーマット
		/// </summary>
		/// <returns>フォーマットされたエラー文字列</returns>
		public string GetErrorMessages()
		{
			var errorMessage = new StringBuilder();
			foreach (var error in Errors)
			{
				var message = ErrorCodes.GetInstance().GetErrorDefinition(error.ErrorCode, error.ErrorInfo).Message;
				var errorCode = string.Format("{0} | {1}: ", error.ErrorCode, error.ErrorInfo);
				errorMessage.Append(errorCode)
					.Append(message)
					.Append(Environment.NewLine);
			}
			return errorMessage.ToString();
		}

		/// <summary>
		/// かならず例外をスローします
		/// </summary>
		public string CreateResponseString()
		{
			// 使わない
			throw new NotSupportedException();
		}

		/// <summary>エラー情報</summary>
		[JsonProperty("errors")]
		public virtual Error[] Errors { get; set; }
		/// <summary>エラーがあるか？</summary>
		[JsonIgnore]
		public bool IsError
		{
			get { return ((this.Errors != null) && this.Errors.Any()); }
		}

		/// <summary>
		/// エラー
		/// </summary>
		[Serializable]
		public class Error
		{
			/// <summary>
			/// エンドユーザーによりキャンセルか
			/// </summary>
			/// <returns>エンドユーザーによるキャンセル: true</returns>
			public bool IsCanceledByUser()
			{
				return (this.ErrorInfo == PaypayConstants.FLG_ERROR_INFORMATION_CANCELED_BY_USER);
			}

			/// <summary>エラーコード</summary>
			[JsonProperty("errCode")]
			public string ErrorCode { get; set; }
			/// <summary>エラー情報</summary>
			[JsonProperty("errInfo")]
			public string ErrorInfo { get; set; }
		}
	}
}
