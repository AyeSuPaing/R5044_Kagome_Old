/*
=========================================================================================================
  Module      : 後払い応答データ (AtobaraicomResponseData.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Xml.Serialization;

namespace w2.App.Common.Order
{
	/// <summary>
	/// 後払い応答データ
	/// </summary>
	[XmlRoot("response")]
	public class AtobaraicomResponseData
	{
		/// <summary>状態</summary>
		public string status { get; set; }
		/// <summary>注文ID</summary>
		public string orderId { get; set; }
		/// <summary>システム注文ID</summary>
		public string systemOrderId { get; set; }
		/// <summary>メッセージ</summary>
		public List<Messages> messages { get; set; }
		/// <summary>注文の状況</summary>
		public string orderStatus { get; set; }
	}

	/// <summary>
	/// メッセージ
	/// </summary>
	public class Messages
	{
		/// <summary>メッセージ</summary>
		public string message { get; set; }
		/// <summary>メッセージコード</summary>
		[XmlAttribute]
		public string cd { get; set; }
	}
}
