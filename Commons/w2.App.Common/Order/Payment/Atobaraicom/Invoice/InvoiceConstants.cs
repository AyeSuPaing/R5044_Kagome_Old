/*
=========================================================================================================
  Module      : Invoice Constants (InvoiceConstants.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.Atobaraicom.Invoice
{
	/// <summary>
	/// Invoice constants
	/// </summary>
	public static class InvoiceConstants
	{
		/// <summary>Invoice api enter prise id</summary>
		private const string INVOICE_API_ENTER_PRISE_ID = "EnterpriseId";
		/// <summary>Invoice api user id</summary>
		private const string INVOICE_API_API_USER_ID = "ApiUserId";
		/// <summary>Invoice api order id</summary>
		private const string INVOICE_API_ORDER_ID = "OrderId";
		/// <summary>Invoice api deliv id</summary>
		private const string INVOICE_API_DELIV_ID = "DelivId";
		/// <summary>Invoice api journal num</summary>
		private const string INVOICE_API_JOURNAL_NUM = "JournalNum";
		/// <summary>Transfer print queue api action</summary>
		public const string TRANSFER_PRINT_QUEUE_API_ACTION = "Enqueue";
		/// <summary>Api result ok</summary>
		public const string API_RESULT_OK = "1";
		/// <summary>Api result error</summary>
		public const string API_RESULT_ERROR = "-1";
		/// <summary>Api result status ok</summary>
		public const string API_RESULT_STATUS_OK = "success";
		/// <summary>Invoice process execute api action</summary>
		public const string INVOICE_PROCESS_EXECUTE_API_ACTION = "Processed";
		/// <summary>HTTP header: Content-Type: application/json</summary>
		private const string HTTP_HEADER_CONTENT_TYPE_APPLICATION_JSON = "application/json";
		/// <summary>Get list target invoice api action</summary>
		public const string GET_LIST_TARGET_INVOICE_API_ACTION = "FetchTargets";
		/// <summary>請求書を同梱する</summary>
		public const string INVOICE_BUNDLE_FLG_ON = "0";
		/// <summary>請求書を同梱しない</summary>
		public const string INVOICE_BUNDLE_FLG_OFF = "1";
	}
}
