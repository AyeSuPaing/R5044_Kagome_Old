﻿﻿/*
=========================================================================================================
  Module      : Gmo Response Billing Confirmation (GmoResponseBillingConfirmation.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.BillingConfirmation
{
	/// <summary>
	/// APIレスポンス
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "response", IsNullable = false, Namespace = "")]
	public class GmoResponseBillingConfirmation : BaseGmoResponse
	{
		/// <summary>
		/// 取得結果
		/// </summary>
		[XmlElement("transactionInfo")]
		public TransactionResultElement TransactionInfo;
	}

	#region Transaction Results
	/// <summary>
	/// 取引情報要素
	/// </summary>
	public class TransactionResultElement
	{
		/// <summary>コンストラクタ</summary>
		public TransactionResultElement()
		{
			this.GmoTransactionId = string.Empty;
		}

		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;
	}
	#endregion
}