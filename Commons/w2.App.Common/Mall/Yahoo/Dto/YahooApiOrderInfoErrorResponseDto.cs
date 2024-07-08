/*
=========================================================================================================
  Module      : YAHOO API 注文詳細API エラーレスポンスDTO クラス(YahooApiOrderInfoErrorResponseDto.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Serialization;

namespace w2.App.Common.Mall.Yahoo.Dto
{
	/// <summary>
	/// 注文詳細API エラーレスポンスDTO
	/// </summary>
	[Serializable]
	[XmlRoot("Erorr")]
	public class YahooApiOrderInfoErrorResponseDto
	{
		/// <summary>エラーメッセージ</summary>
		[XmlElement("Message")]
		public string Message { get; set; }
		/// <summary>エラーコード</summary>
		[XmlElement("Code")]
		public string Code { get; set; }
	}
}
