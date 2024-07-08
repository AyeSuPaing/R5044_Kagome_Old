/*
=========================================================================================================
  Module      : Invoice Request (InvoiceRequest.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System.Xml.Serialization;

namespace w2.App.Common.Order.Payment.Atobaraicom.Invoice
{
	/// <summary>
	/// Invoice request
	/// </summary>
	public static class InvoiceRequest
	{
		#region TransferPrintQueueRequest
		/// <summary>
		/// Transfer print queue request
		/// </summary>
		public class TransferPrintQueueRequest
		{
			/// <summary>
			/// Transfer print queue request
			/// </summary>
			/// <param name="orderId">Order id</param>
			/// <param name="invoiceFlg">Invoice flag</param>
			public TransferPrintQueueRequest(string orderId, string invoiceFlg)
			{
				this.BillingObject = new Billing
				{
					AuthObject = new Billing.Auth
					{
						EnterpriseId = Constants.PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED,
						ApiUserId = Constants.PAYMENT_SETTING_ATOBARAICOM_API_USER_ID,
					},
					Action = InvoiceConstants.TRANSFER_PRINT_QUEUE_API_ACTION,
					ParametersObject = new Billing.Parameters
					{
						ParameterObject = new Billing.Parameters.Parameter
						{
							OrderId = orderId,
							Mode = (invoiceFlg == Constants.FLG_ORDER_INVOICE_BUNDLE_FLG_ON)
								? InvoiceConstants.INVOICE_BUNDLE_FLG_ON 
								: InvoiceConstants.INVOICE_BUNDLE_FLG_OFF,
						}
					}
				};
			}

			/// <summary>Billing object</summary>
			[XmlElement("Billing")]
			public Billing BillingObject { get; set; }

			/// <summary>
			/// Billing
			/// </summary>
			public class Billing
			{
				/// <summary>Auth object</summary>
				[XmlElement("Auth")]
				public Auth AuthObject { get; set; }
				/// <summary>Action</summary>
				[XmlElement("Action")]
				public string Action { get; set; }
				/// <summary>Parameters</summary>
				[XmlElement("Parameters")]
				public Parameters ParametersObject { get; set; }

				/// <summary>
				/// Auth
				/// </summary>
				public class Auth
				{
					/// <summary>Enterprise id</summary>
					[XmlElement("EnterpriseId")]
					public string EnterpriseId { get; set; }
					/// <summary>Api user id</summary>
					[XmlElement("ApiUserId")]
					public string ApiUserId { get; set; }
				}

				/// <summary>
				/// Parameters
				/// </summary>
				public class Parameters
				{
					/// <summary>Parameter object</summary>
					[XmlElement("Parameter")]
					public Parameter ParameterObject { get; set; }

					/// <summary>
					/// Parameter
					/// </summary>
					public class Parameter
					{
						/// <summary>Order id</summary>
						[XmlElement("OrderId")]
						public string OrderId { get; set; }
						/// <summary>Mode</summary>
						[XmlElement("Mode")]
						public string Mode { get; set; }
					}
				}
			}
		}
		#endregion

		#region GetListTargetInvoiceRequest
		/// <summary>
		/// Get list target invoice request
		/// </summary>
		public class GetListTargetInvoiceRequest
		{
			/// <summary>
			/// Get list target invoice request
			/// </summary>
			public GetListTargetInvoiceRequest()
			{
				this.BillingObject = new Billing
				{
					AuthObject = new Billing.Auth
					{
						EnterpriseId = Constants.PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED,
						ApiUserId = Constants.PAYMENT_SETTING_ATOBARAICOM_API_USER_ID,
					},
					Action = InvoiceConstants.GET_LIST_TARGET_INVOICE_API_ACTION,
					Parameters = string.Empty,
				};
			}

			/// <summary>Billing object</summary>
			[XmlElement("Billing")]
			public Billing BillingObject { get; set; }

			/// <summary>
			/// Billing
			/// </summary>
			public class Billing
			{
				/// <summary>Auth object</summary>
				[XmlElement("Auth")]
				public Auth AuthObject { get; set; }
				/// <summary>Action</summary>
				[XmlElement("Action")]
				public string Action { get; set; }
				/// <summary>Parameters</summary>
				[XmlElement("Parameters")]
				public string Parameters { get; set; }

				/// <summary>
				/// Auth
				/// </summary>
				public class Auth
				{
					/// <summary>Enterprise id</summary>
					[XmlElement("EnterpriseId")]
					public string EnterpriseId { get; set; }
					/// <summary>Api user id</summary>
					[XmlElement("ApiUserId")]
					public string ApiUserId { get; set; }
				}
			}
		}
		#endregion

		#region InvoiceProcessExecuteRequest
		/// <summary>
		/// Invoice process execute request
		/// </summary>
		public class InvoiceProcessExecuteRequest
		{
			/// <summary>
			/// Invoice process execute request
			/// </summary>
			/// <param name="orderId">Order id</param>
			public InvoiceProcessExecuteRequest(string orderId)
			{
				this.BillingObject = new Billing
				{
					AuthObject = new InvoiceRequest.InvoiceProcessExecuteRequest.Billing.Auth
					{
						EnterpriseId = Constants.PAYMENT_SETTING_ATOBARAICOM_ENTERPRISED,
						ApiUserId = Constants.PAYMENT_SETTING_ATOBARAICOM_API_USER_ID,
					},
					Action = InvoiceConstants.INVOICE_PROCESS_EXECUTE_API_ACTION,
					ParametersObject = new InvoiceRequest.InvoiceProcessExecuteRequest.Billing.Parameters
					{
						ParameterObject = new InvoiceRequest.InvoiceProcessExecuteRequest.Billing.Parameters.Parameter
						{
							OrderId = orderId,
						},
					}
				};
			}

			/// <summary>Billing object</summary>
			[XmlElement("Billing")]
			public Billing BillingObject { get; set; }

			/// <summary>
			/// Billing
			/// </summary>
			public class Billing
			{
				/// <summary>Auth object</summary>
				[XmlElement("Auth")]
				public Auth AuthObject { get; set; }
				/// <summary>Action</summary>
				[XmlElement("Action")]
				public string Action { get; set; }
				/// <summary>Parameters</summary>
				[XmlElement("Parameters")]
				public Parameters ParametersObject { get; set; }

				/// <summary>
				/// Auth
				/// </summary>
				public class Auth
				{
					/// <summary>Enterprise id</summary>
					[XmlElement("EnterpriseId")]
					public string EnterpriseId { get; set; }
					/// <summary>Api user id</summary>
					[XmlElement("ApiUserId")]
					public string ApiUserId { get; set; }
				}

				/// <summary>
				/// Parameters
				/// </summary>
				public class Parameters
				{
					/// <summary>Parameter object</summary>
					[XmlElement("Parameter")]
					public Parameter ParameterObject { get; set; }

					/// <summary>
					/// Parameter
					/// </summary>
					public class Parameter
					{
						/// <summary>Order id</summary>
						[XmlElement("OrderId")]
						public string OrderId { get; set; }
					}
				}
			}
		}
		#endregion
	}
}
