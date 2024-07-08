/*
=========================================================================================================
  Module      : Atodene出荷報告リクエスト(AtodeneShippingRequest.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.Shipping
{
	/// <summary>
	/// Atodene出荷報告リクエスト
	/// </summary>
	[XmlRoot(DataType = "string", ElementName = "request", IsNullable = false, Namespace = "")]
	public class AtodeneShippingRequest : BaseAtodeneRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public AtodeneShippingRequest()
			: base()
		{
			this.TransactionInfo = new TransactionInfoElement();
		}

		/// <summary>取引情報</summary>
		[XmlElement("transactionInfo")]
		public TransactionInfoElement TransactionInfo;

		/// <summary>
		/// 取引情報要素
		/// </summary>
		public class TransactionInfoElement
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			public TransactionInfoElement()
			{
				this.TransactionId = "";
				this.DeliveryCompanyCode = Constants.PAYMENT_SETTING_ATODENE_DELIVERY_COMPANY_CODE;
			}

			/// <summary>出荷報告種別</summary>
			[XmlElement("deliveryType")]
			public string DeliveryType { get; set; }

			/// <summary>お問合せ番号</summary>
			[XmlElement("transactionId")]
			public string TransactionId { get; set; }

			/// <summary>運送会社コード</summary>
			[XmlElement("deliveryCompanyCode")]
			public string DeliveryCompanyCode { get; set; }

			/// <summary>配送伝票番号</summary>
			[XmlElement("deliverySlipNo")]
			public string DeliverySlipNo { get; set; }

			/// <summary>請求書発行日</summary>
			[XmlElement("InvoiceDate")]
			public string InvoiceDate { get; set; }
		}
	}
}
