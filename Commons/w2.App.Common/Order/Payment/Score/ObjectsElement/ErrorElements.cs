/*
=========================================================================================================
  Module      : エラー情報要素(ErrorElements.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Score.ObjectsElement
{
	/// <summary>
	/// エラー情報要素
	/// </summary>
	public class ErrorElements
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ErrorElements()
		{
			this.ErrorList = new[] { new ErrorElement() };
		}

		/// <summary>エラー情報</summary>
		[XmlElement("error")]
		public ErrorElement[] ErrorList { get; set; }
	}

	#region ErrorElement
	/// <summary>
	/// エラー情報要素
	/// </summary>
	public class ErrorElement
	{
		/// <summary>エラーコード</summary>
		[XmlElement("errorCode")]
		public string ErrorCode { get; set; }
		/// <summary>エラーメッセージ</summary>
		[XmlElement("errorMessage")]
		public string ErrorMessage { get; set; }
	}
	#endregion
}
