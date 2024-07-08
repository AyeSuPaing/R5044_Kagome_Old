﻿/*
=========================================================================================================
  Module      : Gmo Request Billing Confirmation (GmoRequestBillingConfirmation.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.BillingConfirmation
{
	/// <summary>
	/// 与信審査結果取得のリクエスト値
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestBillingConfirmation : BaseGmoRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public GmoRequestBillingConfirmation()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="gmoTransactionId">GMO 取引ID</param>
		/// <param name="fixRequestDate">請求書確認日</param>
		public GmoRequestBillingConfirmation(string gmoTransactionId, string fixRequestDate)
			: base()
		{
			this.Transaction = new TransactionElement();
			this.Transaction.GmoTransactionId = gmoTransactionId;
			this.Transaction.GmoFixRequestDate = fixRequestDate;
		}

		/// <summary>取引情報</summary>
		[XmlElement("transaction")]
		public TransactionElement Transaction;
	}

	#region Transaction Information
	/// <summary>
	/// 取引情報要素
	/// </summary>
	public class TransactionElement
	{
		/// <summary>コンストラクタ</summary>
		public TransactionElement()
		{
			this.GmoTransactionId = string.Empty;
			this.GmoFixRequestDate = string.Empty;
		}

		/// <summary>GMO取引ID</summary>
		[XmlElement("gmoTransactionId")]
		public string GmoTransactionId;

		/// <summary>GMO請求書確認日</summary>
		[XmlElement("fixRequestDate")]
		public string GmoFixRequestDate;
	}
	#endregion
}