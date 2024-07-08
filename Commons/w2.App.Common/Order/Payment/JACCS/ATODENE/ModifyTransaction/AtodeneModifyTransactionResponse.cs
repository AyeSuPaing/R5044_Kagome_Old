/*
=========================================================================================================
  Module      : Atodene取引変更・キャンセルレスポンス(AtodeneModifyTransactionResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.ModifyTransaction
{
	/// <summary>
	/// Atodene取引変更・キャンセルレスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class AtodeneModifyTransactionResponse : BaseAtodeneResponse
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AtodeneModifyTransactionResponse()
			: base()
		{

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
				this.AutoAuthoriresult = "";
			}

			/// <summary>お問合せ番号</summary>
			[XmlElement("transactionId")]
			public string TransactionId { get; set; }

			/// <summary>自動審査結果</summary>
			[XmlElement("autoAuthoriresult")]
			public string AutoAuthoriresult { get; set; }
		}
	}
}
