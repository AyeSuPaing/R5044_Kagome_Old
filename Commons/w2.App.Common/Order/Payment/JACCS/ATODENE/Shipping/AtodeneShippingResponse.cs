/*
=========================================================================================================
  Module      : Atodene出荷報告レスポンス(AtodeneShippingResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.Shipping
{
	/// <summary>
	/// Atodene出荷報告レスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class AtodeneShippingResponse : BaseAtodeneResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AtodeneShippingResponse()
			: base()
		{
		}

		/// <summary>取引情報</summary>
		[XmlElement("transactionInfo")]
		public TransactionInfoElement TransactionInfo { get; set; }

		/// <summary>
		/// 取引情報要素
		/// </summary>
		public class TransactionInfoElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public TransactionInfoElement()
			{
			}

			/// <summary>お問合せ番号</summary>
			[XmlElement("transactionId")]
			public string TransactionId { get; set; }
		}
	}
}
