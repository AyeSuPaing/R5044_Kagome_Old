/*
=========================================================================================================
  Module      : 注文電子発票情報サービスのインタフェース(ITwOrderInvoiceService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.TwOrderInvoice;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.TwOrderInvoice
{
	/// <summary>
	/// 注文電子発票情報サービスのインタフェース
	/// </summary>
	public interface ITwOrderInvoiceService : IService
	{
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">Sql Accessor</param>
		void Insert(TwOrderInvoiceModel model, SqlAccessor accessor = null);

		/// <summary>
		/// Get Order Invoice
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderShippingNo">Order Shipping No</param>
		/// <param name="accessor">Sql Accessor</param>
		/// <returns>モデル</returns>
		TwOrderInvoiceModel GetOrderInvoice(string orderId, int orderShippingNo, SqlAccessor accessor = null);

		/// <summary>
		/// Update Taiwan Order Invoice For Modify
		/// </summary>
		/// <param name="model">Order Invoice Model</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Update Count</returns>
		int UpdateTwOrderInvoiceForModify(
			TwOrderInvoiceModel model,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor);

		/// <summary>
		/// Update Taiwan Order Invoice Status
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="orderShippingNo">Order Shipping No</param>
		/// <param name="invoiceStatus">Invoice Status</param>
		/// <param name="invoiceNo">Invoice No</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>result</returns>
		int UpdateTwOrderInvoiceStatus(
			string orderId,
			int orderShippingNo,
			string invoiceStatus,
			string invoiceNo,
			SqlAccessor accessor = null);

		/// <summary>
		/// Update InvoiceNo By Order Id
		/// </summary>
		/// <param name="invoiceNo">Invoice No</param>
		/// <param name="orderId">Order Id</param>
		void UpdateInvoiceNoByOrderId(string invoiceNo, string orderId);

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="orderId">ユーザID</param>
		/// <param name="accessor">Sql Accessor</param>
		bool Delete(string orderId, SqlAccessor accessor = null);
	}
}