/*
=========================================================================================================
  Module      : Gmo Response Transaction Register(GmoResponseTransactionRegister.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.TransactionRegister
{
	/// <summary>
	/// 取引登録のレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseTransactionRegister : BaseGmoResponse
	{
		/// <summary>
		/// 取引登録結果
		/// </summary>
		[XmlElement("transactionResult")]
		public TransactionResultElement TransactionResult;

		/// <summary>結果：OK？</summary>
		public bool IsResultOk
		{
			get { return (this.Result == ResultCode.OK); }
		}
		/// <summary>結果：NG？</summary>
		public bool IsResultNg
		{
			get { return (this.Result == ResultCode.NG); }
		}
		/// <summary>承認結果: OK?</summary>
		public bool IsOk
		{
			get { return ((this.Result == ResultCode.OK) && (this.TransactionResult.AuthorResult == "OK")); }
		}
		/// <summary>承認結果: NG?</summary>
		public bool IsNG
		{
			get { return ((this.Result == ResultCode.OK) && (this.TransactionResult.AuthorResult == "NG")); }
		}
		/// <summary>承認結果: 審査中？</summary>
		public bool IsInReview
		{
			get { return ((this.Result == ResultCode.OK) && (this.TransactionResult.AuthorResult == Constants.CONST_RESPONSE_AUTHOR_RESULT_INREVIEW)); }
		}
		/// <summary>承認結果: 入金待機？</summary>
		public bool IsDepositWaiting
		{
			get { return ((this.Result == ResultCode.OK) && (this.TransactionResult.AuthorResult == Constants.CONST_RESPONSE_AUTHOR_RESULT_DEPOSIT_WAITING)); }
		}
		/// <summary>承認結果: ALERT？</summary>
		public bool IsAlert
		{
			get { return ((this.Result == ResultCode.OK) && (this.TransactionResult.AuthorResult == Constants.CONST_RESPONSE_AUTHOR_RESULT_ALERT)); }
		}
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
	}
	#endregion
}