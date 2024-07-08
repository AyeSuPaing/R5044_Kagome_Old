/*
=========================================================================================================
  Module      : DSK後払い取引登録レスポンス(DskDeferredOrderRegisterResponse.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.DSKDeferred.OrderRegister
{
	/// <summary>
	/// DSK後払い取引登録レスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false)]
	[XmlType(TypeName = "chmnAddResForm")]
	public class DskDeferredOrderRegisterResponse : BaseDskDeferredResponse
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

		/// <summary>自動審査結果</summary>
		[XmlElement("authorResult")]
		public string AuthorResult { get; set; }
	}
}
