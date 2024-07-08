/*
=========================================================================================================
  Module      : Atodene取引登録レスポンス(AtodeneTransactionResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.Transaction
{
	/// <summary>
	/// Atodene取引登録レスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class AtodeneTransactionResponse : BaseAtodeneResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AtodeneTransactionResponse()
			: base()
		{
		}

		/// <summary>取引登録情報項目</summary>
		[XmlElement("transactionInfo")]
		public TransactionInfoElement TransactionInfo { get; set; }

		/// <summary>
		/// 取引登録情報項目
		/// </summary>
		public class TransactionInfoElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public TransactionInfoElement()
			{

			}

			/// <summary>ご購入店受注番号</summary>
			[XmlElement("shopOrderId")]
			public string ShopOrderId { get; set; }

			/// <summary>お問合せ番号</summary>
			[XmlElement("transactionId")]
			public string TransactionId { get; set; }

			/// <summary>自動審査結果</summary>
			[XmlElement("autoAuthoriresult")]
			public string AutoAuthoriresult { get; set; }
		}
	}
}
