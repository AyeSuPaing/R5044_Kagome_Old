/*
=========================================================================================================
  Module      : DSK後払い注文情報変更リスポンス(DskDeferredOrderModifyResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.DSKDeferred.OrderModify
{
	/// <summary>
	/// DSK後払い注文情報変更リスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false)]
	[XmlType(TypeName = "chmnShusResForm")]
	public class DskDeferredOrderModifyResponse : BaseDskDeferredResponse
	{
		/// <summary>注文登録結果</summary>
		[XmlElement("transactionResult")]
		public TransactionResultElement TransactionResult { get; set; }
	}

	/// <summary>
	/// 注文登録結果要素
	/// </summary>
	public class TransactionResultElement
	{
		/// <summary>注文ID</summary>
		[XmlElement("transactionId")]
		public string TransactionId { get; set; }

		/// <summary>加盟店注文ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId { get; set; }

		/// <summary>自動審査結果</summary>
		[XmlElement("authorResult")]
		public string AuthorResult { get; set; }
	}
}
