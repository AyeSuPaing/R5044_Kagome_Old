/*
=========================================================================================================
  Module      : 与信審査結果取得のレスポンス値(GmoResponseGetCreditStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.GetCreditStatus
{
	/// <summary>
	/// APIレスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseGetCreditStatus : BaseGmoResponse
	{
		/// <summary>
		/// 取得結果
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
		/// <summary>Status: OK?</summary>
		public bool IsOk
		{
			get { return ((this.Result == ResultCode.OK) && (this.TransactionResult.AutoAuthorResult == "OK")); }
		}
		/// <summary>Status: NG?</summary>
		public bool IsNG
		{
			get { return ((this.Result == ResultCode.OK) && (this.TransactionResult.AutoAuthorResult == "NG")); }
		}
		/// <summary>Status: 審査中？</summary>
		public bool IsInReview
		{
			get { return ((this.Result == ResultCode.OK) && (this.TransactionResult.AutoAuthorResult == Constants.CONST_RESPONSE_AUTHOR_RESULT_INREVIEW)); }
		}
		/// <summary>Status: 入金待機？</summary>
		public bool IsDepositWaiting
		{
			get { return ((this.Result == ResultCode.OK) && (this.TransactionResult.AutoAuthorResult == Constants.CONST_RESPONSE_AUTHOR_RESULT_DEPOSIT_WAITING)); }
		}
		/// <summary>Status: Alert？</summary>
		public bool IsAlert
		{
			get { return ((this.Result == ResultCode.OK) && (this.TransactionResult.AutoAuthorResult == Constants.CONST_RESPONSE_AUTHOR_RESULT_ALERT)); }
		}
	}

	#region TransactionResultElement
	/// <summary>
	/// 取得結果要素
	/// </summary>
	public class TransactionResultElement
	{
		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;

		/// <summary>自動審査結果</summary>
		[XmlElement("autoAuthorResult")]
		public string AutoAuthorResult;

		/// <summary>目視審査結果</summary>
		[XmlElement("maulAuthorResult")]
		public string MaulAuthorResult;

		/// <summary>目視審査結果理由情報</summary>
		[XmlElement("reasons")]
		public ReasonsElement Reasons;
	}
	#endregion

	#region ReasonsElement
	/// <summary>
	/// 目視審査結果理由情報要素
	/// </summary>
	public class ReasonsElement
	{
		/// <summary>目視審査結果理由</summary>
		[XmlElement("reason")]
		public string Reason;
	}
	#endregion
}

