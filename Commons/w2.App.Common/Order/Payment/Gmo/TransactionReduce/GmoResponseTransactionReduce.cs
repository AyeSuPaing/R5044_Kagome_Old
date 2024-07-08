/*
=========================================================================================================
  Module      : Gmo Response Transaction Reduce (GmoResponseTransactionReduce.cs)
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

namespace w2.App.Common.Order.Payment.GMO.TransactionReduce
{
	/// <summary>
	/// APIレスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseTransactionReduce : BaseGmoResponse
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
		/// <summary>加盟店取引ID</summary>
		[XmlElement("oldGmopsTranId")]
		public string OldGmoTransactionId;

		/// <summary>GMO取引ID</summary>
		[XmlElement("newGmopsTranId")]
		public string NewGmoTransactionId;

		/// <summary>自動審査結果</summary>
		[XmlElement("authorResult")]
		public string AuthorResult;
	}
	#endregion
}