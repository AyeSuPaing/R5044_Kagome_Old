/*
=========================================================================================================
  Module      : 取引修正・キャンセルのレスポンス値(GmoResponseOrderModifyCancel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.OrderModifyCancel
{
	/// <summary>
	/// 取引修正・キャンセルのレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseOrderModifyCancel : BaseGmoResponse
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
		[XmlElement("GmoTransactionId")]
		public string GmoTransactionId;

		/// <summary>自動審査結果</summary>
		[XmlElement("authorResult")]
		public string AuthorResult;
	}
	#endregion
}
