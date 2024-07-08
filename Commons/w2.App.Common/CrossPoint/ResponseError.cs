/*
=========================================================================================================
  Module      : CrossPoint API レスポンスのエラーモデル (ResponseError.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.CrossPoint
{
	/// <summary>
	/// エラーモデル
	/// </summary>
	public class Error
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Error()
		{
			this.No = 0;
			this.Code = string.Empty;
			this.Message = string.Empty;
		}

		/// <summary>エラーコード</summary>
		private enum ErrorCode
		{
			/// <summary>テナントコード、ショップコード不正</summary>
			AuthError = 1,
			/// <summary>認証署名不正</summary>
			AuthSignatureError = 2,
			/// <summary>リクエストパラメータ不正</summary>
			RequestParamError = 3,
			/// <summary>業務エラー</summary>
			WorkError = 4,
			/// <summary>異常系エラー</summary>
			AbnormalError = 9,
		}

		/// <summary>取得件数</summary>
		[XmlAttribute("No")]
		public int No { get; set; }
		/// <summary>エラーコード</summary>
		[XmlElement("Code")]
		public string Code { get; set; }
		/// <summary>エラー内容</summary>
		[XmlElement("Message")]
		public string Message { get; set; }
		/// <summary>異常系エラー発生</summary>
		public bool IsAbnormalError
		{
			get { return (this.Code.Substring(0, 1) == ErrorCode.AbnormalError.ToString()); }
		}
	}
}
