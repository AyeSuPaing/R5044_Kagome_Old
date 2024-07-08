/*
=========================================================================================================
  Module      : DSK後払い規定レスポンス(BaseDskDeferredResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.Common.Helper;

namespace w2.App.Common.Order.Payment.DSKDeferred
{
	/// <summary>
	/// DSK後払い規定レスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false)]
	public abstract class BaseDskDeferredResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseDskDeferredResponse()
		{
			this.Result = "";
		}

		/// <summary>処理結果</summary>
		[XmlElement("result")]
		public string Result { get; set; }

		/// <summary>エラー情報</summary>
		[XmlElement("errors")]
		public ErrorsElement Errors { get; set; }

		/// <summary>
		/// レスポンス文字列生成
		/// </summary>
		/// <returns>レスポンス文字列</returns>
		public virtual string CreateResponseString()
		{
			return SerializeHelper.Serialize(this);
		}

		/// <summary>
		/// エラー詳細要素
		/// </summary>
		public class ErrorsElement
		{
			/// <summary>エラー情報</summary>
			[XmlElement("error")]
			public ErrorElement[] Error { get; set; }
		}

		/// <summary>
		/// エラー情報要素
		/// </summary>
		public class ErrorElement
		{
			/// <summary>エラーメッセージ</summary>
			[XmlElement("errorMessage")]
			public string ErrorMessage { get; set; }

			/// <summary>エラーコード</summary>
			[XmlElement("errorCode")]
			public string ErrorCode { get; set; }
		}

		/// <summary>処理結果がOKか</summary>
		public bool IsResultOk
		{
			get { return (this.Result == DskDeferredConst.RESULT_OK); }
		}
	}
}
