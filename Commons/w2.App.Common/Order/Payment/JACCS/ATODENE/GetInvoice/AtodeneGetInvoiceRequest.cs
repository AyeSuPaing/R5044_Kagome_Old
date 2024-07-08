/*
=========================================================================================================
  Module      : Atodene請求書印字データ取得リクエスト(AtodeneGetInvoiceRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.GetInvoice
{
	/// <summary>
	/// Atodene請求書印字データ取得リクエスト
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class AtodeneGetInvoiceRequest : BaseAtodeneRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AtodeneGetInvoiceRequest()
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
