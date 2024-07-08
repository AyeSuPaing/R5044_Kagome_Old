/*
=========================================================================================================
  Module      : 与信審査結果取得のレスポンス値(GmoResponseGetAuthResult.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.GetAuthResult
{
	/// <summary>
	/// 与信審査結果取得のレスポンス値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseGetAuthResult : BaseGmoResponse
	{
		/// <summary>
		/// 取得結果
		/// </summary>
		[XmlElement("transactionResult")]
		public TransactionResultElement TransactionResult;
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
