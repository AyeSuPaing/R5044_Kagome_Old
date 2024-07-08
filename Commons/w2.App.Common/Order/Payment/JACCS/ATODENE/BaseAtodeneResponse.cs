/*
=========================================================================================================
  Module      : Atodene規定レスポンス(BaseAtodeneResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;
using w2.Common.Helper;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE
{
	/// <summary>
	/// Atodene規定レスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public abstract class BaseAtodeneResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		protected BaseAtodeneResponse()
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

		#region ErrorsElement
		/// <summary>
		/// エラー詳細要素
		/// </summary>
		public class ErrorsElement
		{
			/// <summary>エラー情報</summary>
			[XmlElement("error")]
			public ErrorElement[] Error { get; set; }
		}
		#endregion

		#region ErrorElement
		/// <summary>
		/// エラー情報要素
		/// </summary>
		public class ErrorElement
		{
			/// <summary>エラー発生箇所</summary>
			[XmlElement("errorPoint")]
			public string ErrorPoint { get; set; }

			/// <summary>エラーコード</summary>
			[XmlElement("errorCode")]
			public string ErrorCode { get; set; }

			/// <summary>エラーメッセージ</summary>
			[XmlElement("errorMessage")]
			public string ErrorMessage { get; set; }
		}
		#endregion
	}
}
