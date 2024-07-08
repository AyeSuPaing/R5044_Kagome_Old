﻿/*
=========================================================================================================
  Module      : Gmo Request Billing Modification (GmoRequestBillingModification.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.GMO.BillingModification
{
	/// <summary>
	/// GMOリクエスト請求変更
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class GmoRequestBillingModification : BaseGmoRequest
	{
		/// <summary>コンストラクタ</summary>
		public GmoRequestBillingModification()
			: base()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="gmoTransactionId">GMO取引ID</param>
		/// <param name="fixRequestDate">請求書確認日</param>
		public GmoRequestBillingModification(string gmoTransactionId, string fixRequestDate)
			: base()
		{
			this.Transaction = new TransactionElement();
			this.Transaction.GmoTransactionId = gmoTransactionId;
			this.Transaction.GmoFixRequestDate = fixRequestDate;
			this.KindInfo = new KindInfoElement();
		}

		/// <summary>取引情報</summary>
		[XmlElement("transaction")]
		public TransactionElement Transaction;

		/// <summary>更新種別情報</summary>
		[XmlElement("kindInfo")]
		public KindInfoElement KindInfo;
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

	#region KindInfoElement 更新種別情報要素
	/// <summary>
	/// 更新種別情報要素
	/// </summary>
	public class KindInfoElement
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public KindInfoElement()
		{
			this.UpdateKind = UpdateKindType.OrderModify;
		}

		/// <summary>取引更新種別</summary>
		[XmlElement("updateKind")]
		public UpdateKindType UpdateKind;
	}
	#endregion
}