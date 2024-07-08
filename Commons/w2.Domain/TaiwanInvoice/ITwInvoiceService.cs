/*
=========================================================================================================
  Module      : 電子発票情報サービスのインタフェース(ITwInvoiceService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Domain.TwInvoice
{
	/// <summary>
	/// 電子発票情報サービスのインタフェース
	/// </summary>
	public interface ITwInvoiceService : IService
	{
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void Insert(TwInvoiceModel model);

		/// <summary>
		/// Get Invoice For Order
		/// </summary>
		/// <param name="currentDate">Current date</param>
		/// <returns>Invoice for order</returns>
		string GetInvoiceNoForOrder(string currentDate);

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		void InsertAfterGetApi(TwInvoiceModel model);

		/// <summary>
		/// Get All
		/// </summary>
		/// <returns>モデル</returns>
		TwInvoiceModel[] GetAll();

		/// <summary>
		/// Get List Valid Invoice
		/// </summary>
		/// <param name="currentDate">Current date</param>
		/// <returns>Invoice for order</returns>
		TwInvoiceModel[] GetListValidInvoice(string currentDate);

		/// <summary>
		/// Update Invoice Alert Count For Valid Invoice
		/// </summary>
		/// <param name="invoiceAlertCount">Invoice Alert Count</param>
		/// <returns>Is Update</returns>
		int UpdateInvoiceAlertCountForValidInvoice(int invoiceAlertCount);

		/// <summary>
		/// Get Current Alert Count Invoice
		/// </summary>
		/// <param name="currentDate">Current date</param>
		/// <returns>Current Alert Count Invoice</returns>
		TwInvoiceModel GetCurrentAlertCountInvoice(string currentDate);

		/// <summary>
		/// Get List TwInvoice
		/// </summary>
		/// <param name="currentDate">Current Date</param>
		/// <returns>モデル</returns>
		TwInvoiceModel[] GetListTwInvoice(string currentDate);

		/// <summary>
		/// Get Invoice New
		/// </summary>
		/// <returns>Top 1 TwInvoice</returns>
		TwInvoiceModel GetInvoiceNew();
	}
}