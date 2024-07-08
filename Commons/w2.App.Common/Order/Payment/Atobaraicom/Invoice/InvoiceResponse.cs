/*
=========================================================================================================
  Module      : Invoice Response (InvoiceResponse.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Atobaraicom.Invoice
{
	/// <summary>
	/// Invoice response
	/// </summary>
	public static class InvoiceResponse
	{
		#region TransferPrintQueueResponse
		/// <summary>
		/// Transfer print queue response
		/// </summary>
		[XmlRoot(DataType = "string", ElementName = "Response", IsNullable = false, Namespace = "")]
		public class TransferPrintQueueResponse
		{
			/// <summary>Status</summary>
			[XmlElement("Status")]
			public string Status { get; set; }
			/// <summary>Messages object</summary>
			[XmlElement("Messages")]
			public Messages MessagesObject { get; set; }
			/// <summary>Results object</summary>
			[XmlElement("Results")]
			public Results ResultsObject { get; set; }

			/// <summary>
			/// Messages
			/// </summary>
			public class Messages
			{
				/// <summary>Message object</summary>
				[XmlElement("Message")]
				public Message MessageObject { get; set; }

				/// <summary>
				/// Message
				/// </summary>
				public class Message
				{
					/// <summary>Message body</summary>
					[XmlText]
					public string MessageBody { get; set; }
					/// <summary>Message code</summary>
					[XmlAttribute("cd")]
					public string MessageCode { get; set; }
				}
			}

			/// <summary>
			/// Results
			/// </summary>
			public class Results
			{
				/// <summary>Result object</summary>
				[XmlElement("Result")]
				public Result ResultObject { get; set; }

				/// <summary>
				/// Result
				/// </summary>
				public class Result
				{
					/// <summary>Order id</summary>
					[XmlElement("OrderId")]
					public string OrderId { get; set; }
					/// <summary>Exec result</summary>
					[XmlElement("ExecResult")]
					public string ExecResult { get; set; }
					/// <summary>Error object</summary>
					[XmlElement("Error")]
					public Error ErrorObject { get; set; }

					/// <summary>
					/// Error
					/// </summary>
					public class Error
					{
						/// <summary>Error body</summary>
						[XmlText]
						public string ErrorBody { get; set; }
						/// <summary>Error code</summary>
						[XmlAttribute("cd")]
						public string ErrorCode { get; set; }
					}
				}
			}
		}
		#endregion

		#region GetListTargetInvoiceResponse
		/// <summary>
		/// Get list target invoice response
		/// </summary>
		[XmlRoot(DataType = "string", ElementName = "Response", IsNullable = false, Namespace = "")]
		public class GetListTargetInvoiceResponse
		{
			/// <summary>Status</summary>
			[XmlElement("Status")]
			public string Status { get; set; }
			/// <summary>Messages object</summary>
			[XmlElement("messages")]
			public Messages MessagesObject { get; set; }
			/// <summary>Results object</summary>
			[XmlElement("Results")]
			public Results ResultsObject { get; set; }

			/// <summary>
			/// Messages
			/// </summary>
			public class Messages
			{
				/// <summary>Message object</summary>
				[XmlElement("Message")]
				public Message MessageObject { get; set; }

				/// <summary>
				/// Message
				/// </summary>
				public class Message
				{
					/// <summary>Message body</summary>
					[XmlText]
					public string MessageBody { get; set; }
					/// <summary>Message code</summary>
					[XmlAttribute("cd")]
					public string MessageCode { get; set; }
				}
			}

			/// <summary>
			/// Results
			/// </summary>
			public class Results
			{
				/// <summary>Result object</summary>
				[XmlElement("Result")]
				public Result[] ResultObject { get; set; }

				/// <summary>
				/// Result
				/// </summary>
				public class Result
				{
					/// <summary>Postal code</summary>
					[XmlElement("PostalCode")]
					public string PostalCode { get; set; }
					/// <summary>Uniting address</summary>
					[XmlElement("UnitingAddress")]
					public string UnitingAddress { get; set; }
					/// <summary>Name kj</summary>
					[XmlElement("NameKj")]
					public string NameKj { get; set; }
					/// <summary>Order id</summary>
					[XmlElement("OrderId")]
					public string OrderId { get; set; }
					/// <summary>Receipt order date</summary>
					[XmlElement("ReceiptOrderDate")]
					public string ReceiptOrderDate { get; set; }
					/// <summary>Site name kj</summary>
					[XmlElement("SiteNameKj")]
					public string SiteNameKj { get; set; }
					/// <summary>Url</summary>
					[XmlElement("Url")]
					public string Url { get; set; }
					/// <summary>Phone</summary>
					[XmlElement("Phone")]
					public string Phone { get; set; }
					/// <summary>Use amount</summary>
					[XmlElement("UseAmount")]
					public string UseAmount { get; set; }
					/// <summary>Sub total</summary>
					[XmlElement("SubTotal")]
					public string SubTotal { get; set; }
					/// <summary>Carriage fee</summary>
					[XmlElement("CarriageFee")]
					public string CarriageFee { get; set; }
					/// <summary>Charge fee</summary>
					[XmlElement("ChargeFee")]
					public string ChargeFee { get; set; }
					/// <summary>Re issue count</summary>
					[XmlElement("ReIssueCount")]
					public string ReIssueCount { get; set; }
					/// <summary>Limit date</summary>
					[XmlElement("LimitDate")]
					public string LimitDate { get; set; }
					/// <summary>Cv barcode data</summary>
					[XmlElement("Cv_BarcodeData")]
					public string Cv_BarcodeData { get; set; }
					/// <summary>Cv barcode string 1</summary>
					[XmlElement("Cv_BarcodeString1")]
					public string Cv_BarcodeString1 { get; set; }
					/// <summary>Cv barcode string 2</summary>
					[XmlElement("Cv_BarcodeString2")]
					public string Cv_BarcodeString2 { get; set; }
					/// <summary>Yu dt code</summary>
					[XmlElement("Yu_DtCode")]
					public string Yu_DtCode { get; set; }
					/// <summary>Order items obbject</summary>
					[XmlElement("OrderItems")]
					public OrderItems OrderItemsObbject { get; set; }
					/// <summary>Ent order id</summary>
					[XmlElement("Ent_OrderId")]
					public string Ent_OrderId { get; set; }
					/// <summary>Tax amount</summary>
					[XmlElement("TaxAmount")]
					public Decimal TaxAmount { get; set; }
					/// <summary>Sub use amount 1</summary>
					[XmlElement("SubUseAmount_1")]
					public string SubUseAmount_1 { get; set; }
					/// <summary>Sub tax amount 1</summary>
					[XmlElement("SubTaxAmount_1")]
					public string SubTaxAmount_1 { get; set; }
					/// <summary>Sub use amount 2</summary>
					[XmlElement("SubUseAmount_2")]
					public string SubUseAmount_2 { get; set; }
					/// <summary>Sub tax amount 2</summary>
					[XmlElement("SubTaxAmount_2")]
					public string SubTaxAmount_2 { get; set; }
					/// <summary>Cv receipt agent name</summary>
					[XmlElement("Cv_ReceiptAgentName")]
					public string Cv_ReceiptAgentName { get; set; }
					/// <summary>Cv subscriber name</summary>
					[XmlElement("Cv_SubscriberName")]
					public string Cv_SubscriberName { get; set; }
					/// <summary>Bk bank code</summary>
					[XmlElement("Bk_BankCode")]
					public string Bk_BankCode { get; set; }
					/// <summary>Bk branch code</summary>
					[XmlElement("Bk_BranchCode")]
					public string Bk_BranchCode { get; set; }
					/// <summary>Bk bank name</summary>
					[XmlElement("Bk_BankName")]
					public string Bk_BankName { get; set; }
					/// <summary>Bk branch name</summary>
					[XmlElement("Bk_BranchName")]
					public string Bk_BranchName { get; set; }
					/// <summary>Bk deposit class</summary>
					[XmlElement("Bk_DepositClass")]
					public string Bk_DepositClass { get; set; }
					/// <summary>Bk account number</summary>
					[XmlElement("Bk_AccountNumber")]
					public string Bk_AccountNumber { get; set; }
					/// <summary>Bk account holder</summary>
					[XmlElement("Bk_AccountHolder")]
					public string Bk_AccountHolder { get; set; }
					/// <summary>Bk account holder kn</summary>
					[XmlElement("Bk_AccountHolderKn")]
					public string Bk_AccountHolderKn { get; set; }
					/// <summary>Yu subscriber name</summary>
					[XmlElement("Yu_SubscriberName")]
					public string Yu_SubscriberName { get; set; }
					/// <summary>Yu account number</summary>
					[XmlElement("Yu_AccountNumber")]
					public string Yu_AccountNumber { get; set; }
					/// <summary>Yu charge class</summary>
					[XmlElement("Yu_ChargeClass")]
					public string Yu_ChargeClass { get; set; }
					/// <summary>Yu mt ocr code 1</summary>
					[XmlElement("Yu_MtOcrCode1")]
					public string Yu_MtOcrCode1 { get; set; }
					/// <summary>Yu mt ocr code 2</summary>
					[XmlElement("Yu_MtOcrCode2")]
					public string Yu_MtOcrCode2 { get; set; }
					/// <summary>マイページパスワード</summary>
					[XmlElement("MypagePassword")]
					public string MypagePassword { get; set; }
					/// <summary>マイページURL</summary>
					[XmlElement("MypageUrl")]
					public string MypageUrl { get; set; }
					/// <summary>クレジット利用期限日</summary>
					[XmlElement("CreditLimitDate")]
					public string CreditLimitDate { get; set; }
					/// <summary>Corporation number</summary>
					[XmlElement("CorporationNumber ")]
					public string CorporationNumber { get; set; }

					/// <summary>
					/// Order items
					/// </summary>
					public class OrderItems
					{
						/// <summary>Order item object</summary>
						[XmlElement("OrderItem")]
						public OrderItem OrderItemObject { get; set; }

						/// <summary>
						/// Order item
						/// </summary>
						public class OrderItem
						{
							/// <summary>Item name kj 1</summary>
							[XmlElement("ItemNameKj1")]
							public string ItemNameKj1 { get; set; }
							/// <summary>Item num 1</summary>
							[XmlElement("ItemNum1")]
							public string ItemNum1 { get; set; }
							/// <summary>Unit price 1</summary>
							[XmlElement("UnitPrice1")]
							public string UnitPrice1 { get; set; }
							/// <summary>Sum money 1</summary>
							[XmlElement("SumMoney1")]
							public string SumMoney1 { get; set; }
							/// <summary>Tax rate 1</summary>
							[XmlElement("TaxRate1")]
							public string TaxRate1 { get; set; }
						}
					}
				}
			}
		}
		#endregion

		#region InvoiceProcessExecuteResponse
		/// <summary>
		/// Invoice process execute response
		/// </summary>
		[XmlRoot(DataType = "string", ElementName = "Response", IsNullable = false, Namespace = "")]
		public class InvoiceProcessExecuteResponse
		{
			/// <summary>Status</summary>
			[XmlElement("Status")]
			public string Status { get; set; }
			/// <summary>Messages object</summary>
			[XmlElement("Messages")]
			public Messages MessagesObject { get; set; }
			/// <summary>Results object</summary>
			[XmlElement("Results")]
			public Results ResultsObject { get; set; }

			/// <summary>
			/// Messages
			/// </summary>
			public class Messages
			{
				/// <summary>Message object</summary>
				[XmlElement("Message")]
				public Message MessageObject { get; set; }

				/// <summary>
				/// Message
				/// </summary>
				public class Message
				{
					/// <summary>Message body</summary>
					[XmlText]
					public string MessageBody { get; set; }
					/// <summary>Message code</summary>
					[XmlAttribute("cd")]
					public string MessageCode { get; set; }
				}
			}

			/// <summary>
			/// Results
			/// </summary>
			public class Results
			{
				/// <summary>Result object</summary>
				[XmlElement("Result")]
				public Result ResultObject { get; set; }

				/// <summary>
				/// Result
				/// </summary>
				public class Result
				{
					/// <summary>Order id</summary>
					[XmlElement("OrderId")]
					public string OrderId { get; set; }
					/// <summary>Exec result</summary>
					[XmlElement("ExecResult")]
					public string ExecResult { get; set; }
					/// <summary>Error object</summary>
					[XmlElement("Error")]
					public Error ErrorObject { get; set; }

					/// <summary>
					/// Error
					/// </summary>
					public class Error
					{
						/// <summary>Error body</summary>
						[XmlText]
						public string ErrorBody { get; set; }
						/// <summary>Error code</summary>
						[XmlAttribute("cd")]
						public string ErrorCode { get; set; }
					}
				}
			}
		}
		#endregion
	}
}
