/*
=========================================================================================================
  Module      : 端末識別情報要素(HttpInfoElement.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Score.ObjectsElement
{
	/// <summary>
	/// 端末識別情報要素
	/// </summary>
	public class HttpInfoElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public HttpInfoElement()
		{
			this.HttpHeader = string.Empty;
			this.DeviceInfo = string.Empty;
		}

		/// <summary>HTTPヘッダ情報</summary>
		[XmlElement("httpHeader")]
		public string HttpHeader { get; set; }
		/// <summary>デバイス情報</summary>
		[XmlElement("deviceInfo")]
		public string DeviceInfo { get; set; }
	}
}
