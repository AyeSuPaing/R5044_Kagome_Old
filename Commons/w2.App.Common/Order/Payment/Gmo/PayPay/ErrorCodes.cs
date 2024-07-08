/*
=========================================================================================================
  Module      : Error Codes (ErrorCodes.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Xml.Serialization;
using w2.Common.Helper;

namespace w2.App.Common.Order.Payment.Paypay
{
	/// <summary>
	/// Error Codes
	/// </summary>
	[Serializable]
	[XmlRoot(DataType = "string", ElementName = "ErrorCodes", IsNullable = false, Namespace = "")]
	public class ErrorCodes
	{
		/// <summary>シングルトンインスタンス</summary>
		private static ErrorCodes _singletonInstance;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		private ErrorCodes()
		{
		}

		/// <summary>
		/// インスタンスを取得
		/// </summary>
		/// <returns>インスタンス</returns>
		public static ErrorCodes GetInstance()
		{
			return _singletonInstance = _singletonInstance ?? LoadXml();
		}

		/// <summary>
		/// XML読み込み
		/// </summary>
		/// <returns>インスタンス</returns>
		private static ErrorCodes LoadXml()
		{
			var result = SerializeHelper.Deserialize<ErrorCodes>(Properties.Resources.PaypayErrorCodes);
			return result;
		}

		/// <summary>
		/// エラー定義取得
		/// </summary>
		/// <param name="errorCode">エラーコード</param>
		/// <param name="errorInfo">エラー情報</param>
		/// <returns>エラー定義</returns>
		public ErrorDefinition GetErrorDefinition(string errorCode, string errorInfo)
		{
			var result = this.ErrorDefinitions.FirstOrDefault(ed => ((ed.ErrorCode == errorCode) && (ed.ErrorInfo == errorInfo)))
				?? new ErrorDefinition
				{
					ErrorCode = errorCode,
					ErrorInfo = errorInfo,
					DisplayForClient = false,
					Message = "不明なエラーが発生しました。",
				};
			return result;
		}

		/// <summary>エラー定義</summary>
		[XmlElement("Error")]
		public ErrorDefinition[] ErrorDefinitions { get; set; }

		/// <summary>
		/// エラー定義
		/// </summary>
		[Serializable]
		public class ErrorDefinition
		{
			/// <summary>エラーコード</summary>
			[XmlAttribute("ErrorCode")]
			public string ErrorCode { get; set; }
			/// <summary>エラー情報</summary>
			[XmlAttribute("ErrorInfo")]
			public string ErrorInfo { get; set; }
			/// <summary>メッセージ</summary>
			[XmlAttribute("Message")]
			public string Message { get; set; }
			/// <summary>クライアント表示するか</summary>
			[XmlAttribute("DisplayForClient")]
			public bool DisplayForClient { get; set; }
		}
	}
}
