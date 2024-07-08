/*
=========================================================================================================
  Module      : GMOアトカラ 出荷報告キャンセル APIクラス(PaymentGmoAtokaraShipmentCancelApi.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Linq;
using w2.App.Common.Order.Payment.GMOAtokara.Utils;

namespace w2.App.Common.Order.Payment.GMOAtokara
{
	/// <summary>
	/// GMOアトカラ 出荷報告キャンセル APIクラス
	/// </summary>
	public class PaymentGmoAtokaraShipmentCancelApi : PaymentGmoAtokaraBaseApi
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PaymentGmoAtokaraShipmentCancelApi()
			: this(PaymentGmoAtokaraSetting.GetDefaultSetting())
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="settings">設定</param>
		public PaymentGmoAtokaraShipmentCancelApi(
			PaymentGmoAtokaraSetting settings)
			: base(Constants.PAYMENT_GMOATOKARA_DEFERRED_URL_SHIPMENTMODIFYCANCEL, settings, new PaymentGmoAtokaraShipmentResponseData())
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="updateKind">取引更新種別</param>
		/// <param name="gmoTransactionId">GMO取引ID</param>
		/// <param name="pdcompanycode">運送会社コード</param>
		/// <param name="slipno">発送伝票番号</param>
		/// <param name="invoiceIssueDate">請求書発行日</param>
		/// <returns>実行結果</returns>
		public bool Exec(
			PaymentGmoAtokaraTypes.UpdateKind updateKind,
			string gmoTransactionId,
			string pdcompanycode,
			string slipno,
			string invoiceIssueDate)
		{
			return Exec(
				updateKind,
				gmoTransactionId,
				pdcompanycode,
				slipno,
				invoiceIssueDate,
				PaymentGmoAtokaraConstants.TRANSACTIONTYPE_CLOSING);
		}
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="updateKind">取引更新種別</param>
		/// <param name="gmoTransactionId">GMO取引ID</param>
		/// <param name="pdcompanycode">運送会社コード</param>
		/// <param name="slipno">発送伝票番号</param>
		/// <param name="invoiceIssueDate">請求書発行日</param>
		/// <param name="transactionType">取引種別</param>
		/// <returns>実行結果</returns>
		private bool Exec(
			PaymentGmoAtokaraTypes.UpdateKind updateKind,
			string gmoTransactionId,
			string pdcompanycode,
			string slipno,
			string invoiceIssueDate,
			string transactionType)
		{
			var requestXml = CreateRequestXml(
				updateKind,
				gmoTransactionId,
				pdcompanycode,
				slipno,
				invoiceIssueDate,
				transactionType);

			return Post(requestXml);
		}

		/// <summary>
		/// リクエストXML作成
		/// </summary>
		/// <param name="updateKind">取引更新種別</param>
		/// <param name="gmoTransactionId">GMO取引ID</param>
		/// <param name="pdcompanycode">運送会社コード</param>
		/// <param name="slipno">発送伝票番号</param>
		/// <param name="invoiceIssueDate">請求書発行日</param>
		/// <param name="transactionType">取引種別</param>
		/// <returns>リクエストXML</returns>
		private XDocument CreateRequestXml(
			PaymentGmoAtokaraTypes.UpdateKind updateKind,
			string gmoTransactionId,
			string pdcompanycode,
			string slipno,
			string invoiceIssueDate,
			string transactionType)
		{
			var document = new XDocument(new XDeclaration("1.0", "UTF-8", ""));
			document.Add(
				new XElement("request",
					CreateShopInfoNode(),
					new XElement("kindInfo",
						new XElement("updateKind", updateKind)
					),
					new XElement("transaction",
						new XElement("gmoTransactionId", gmoTransactionId),
						new XElement("pdcompanycode", pdcompanycode),
						new XElement("slipno", slipno),
						new XElement("invoiceIssueDate", invoiceIssueDate),
						new XElement("transactionType", transactionType)
					)
				));

			return document;
		}

		/// <summary>レスポンスデータ</summary>
		public PaymentGmoAtokaraShipmentCancelResponseData ResponseData { get { return (PaymentGmoAtokaraShipmentCancelResponseData)this.ResponseDataInner; } }
	}
}
