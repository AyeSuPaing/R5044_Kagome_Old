/*
=========================================================================================================
  Module      : 請求減額のレスポンス値(GmoResponseReduceSales.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.ReduceSales
{
	/// <summary>
	/// 請求減額のレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseReduceSales : BaseGmoResponse
	{
		/// <summary>
		/// 取引登録結果
		/// </summary>
		[XmlElement("transactionResult")]
		public TransactionResultElement TransactionResult;
	}

	#region TransactionResultElement
	/// <summary>
	/// 取引登録結果要素
	/// </summary>
	public class TransactionResultElement
	{
		/// <summary>旧GMO取引ID</summary>
		[XmlElement("oldGmoTransactionId")]
		public string OldGmoTransactionId;

		/// <summary>新GMO取引ID</summary>
		[XmlElement("newGmoTransactionId")]
		public string NewGmoTransactionId;

		/// <summary>自動審査結果</summary>
		[XmlElement("authorResult")]
		public string AuthorResult;
	}
	#endregion
}
