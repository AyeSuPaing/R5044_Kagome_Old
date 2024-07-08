/*
=========================================================================================================
  Module      : GMOレスポンス用データの基底クラス(BaseGmoResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;
using w2.Common.Helper;
using w2.Common.Web;

namespace w2.App.Common.Order.Payment.GMO
{
	/// <summary>
	/// レスポンス用データの基底クラス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public abstract class BaseGmoResponse : IHttpApiResponseData
	{
		/// <summary>処理結果</summary>
		[XmlElement("result")]
		public ResultCode Result;

		/// <summary>エラー情報</summary>
		[XmlElement("errors")]
		public ErrorsElement Errors;

		/// <summary>
		/// レスポンス文字列生成
		/// </summary>
		/// <returns>レスポンス文字列</returns>
		public virtual string CreateResponseString()
		{
			return SerializeHelper.Serialize(this);
		}
	}

	#region ErrorsElement
	/// <summary>
	/// エラー詳細要素
	/// </summary>
	public class ErrorsElement
	{
		/// <summary>エラー情報</summary>
		[XmlElement("error")]
		public ErrorElement[] Error;
	}
	#endregion

	#region ErrorElement
	/// <summary>
	/// エラー情報要素
	/// </summary>
	public class ErrorElement
	{
		/// <summary>GMO掛け払い: エラーコード</summary>
		[XmlElement("errCode")]
		public string ErrCode;

		/// <summary>エラーコード</summary>
		[XmlElement("errorCode")]
		public string ErrorCode;

		/// <summary>エラーメッセージ</summary>
		[XmlElement("errorMessage")]
		public string ErrorMessage;
	}
	#endregion
}
