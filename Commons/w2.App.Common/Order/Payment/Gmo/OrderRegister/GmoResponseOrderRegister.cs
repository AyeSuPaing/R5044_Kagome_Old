/*
=========================================================================================================
  Module      : 取引登録のレスポンス値(GmoResponseOrderRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.OrderRegister
{
	/// <summary>
	/// 取引登録のレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseOrderRegister : BaseGmoResponse
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
		[XmlElement("shopTransactionId")]
		public string ShopTransactionId;

		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;

		/// <summary>自動審査結果</summary>
		[XmlElement("authorResult")]
		public string AuthorResult;

		/// <summary>結果：OK？</summary>
		public bool IsResultOk
		{
			get { return (this.AuthorResult == "OK"); }
		}
		/// <summary>結果：NG？</summary>
		public bool IsResultNg
		{
			get { return (this.AuthorResult == "NG"); }
		}
		/// <summary>結果：審査中？</summary>
		public bool IsResultExamination
		{
			get { return (this.AuthorResult == "審査中"); }
		}
	}
	#endregion
}
