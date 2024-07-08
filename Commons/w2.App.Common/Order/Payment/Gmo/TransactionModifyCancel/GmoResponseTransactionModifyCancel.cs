/*
=========================================================================================================
  Module      : GMO response transaction modify cancel(GmoResponseTransactionModifyCancel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.TransactionModifyCancel
{
	/// <summary>
	/// 取引修正・キャンセルのレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseTransactionModifyCancel : BaseGmoResponse
	{
		/// <summary>
		/// 取引修正・キャンセル結果
		/// </summary>
		[XmlElement("transactionResult")]
		public TransactionResultElement TransactionResult;
	}

	#region TransactionResultElement
	/// <summary>
	/// 取引修正・キャンセル結果要素
	/// </summary>
	public class TransactionResultElement
	{
		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;

		/// <summary>加盟店取引ID</summary>
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId;

		/// <summary>自動審査結果</summary>
		[XmlElement("authorResult")]
		public string AuthorResult;
	}
	#endregion
}