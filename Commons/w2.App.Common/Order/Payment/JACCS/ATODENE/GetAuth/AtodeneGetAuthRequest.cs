﻿/*
=========================================================================================================
  Module      : Atodene与信取得リクエスト(AtodeneGetAuthRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.GetAuth
{
	/// <summary>
	/// Atodene与信取得リクエスト
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class AtodeneGetAuthRequest : BaseAtodeneRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AtodeneGetAuthRequest()
			: base()
		{
			this.TransactionInfo = new TransactionInfoElement();
		}

		/// <summary>取引登録情報項目</summary>
		[XmlElement("transactionInfo")]
		public TransactionInfoElement TransactionInfo { get; set; }

		/// <summary>
		/// 取引登録情報項目要素
		/// </summary>
		public class TransactionInfoElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public TransactionInfoElement()
			{
				this.TransactionId = "";
			}

			/// <summary>お問合せ番号</summary>
			[XmlElement("transactionId")]
			public string TransactionId { get; set; }
		}
	}
}