/*
=========================================================================================================
  Module      : 注文電子発票情報サービス (TwOrderInvoiceService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Domain.TwOrderInvoice
{
	/// <summary>
	/// 注文電子発票情報サービス
	/// </summary>
	public class TwOrderInvoiceService : ServiceBase, ITwOrderInvoiceService
	{
		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">Sql Accessor</param>
		public void Insert(TwOrderInvoiceModel model, SqlAccessor accessor = null)
		{
			using (var repository = new TwOrderInvoiceRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Get Order Invoice
		/// <summary>
		/// Get Order Invoice
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="orderShippingNo">Order Shipping No</param>
		/// <param name="accessor">Sql Accessor</param>
		/// <returns>モデル</returns>
		public TwOrderInvoiceModel GetOrderInvoice(string orderId, int orderShippingNo, SqlAccessor accessor = null)
		{
			using (var repository = new TwOrderInvoiceRepository(accessor))
			{
				var model = repository.GetOrderInvoice(orderId, orderShippingNo);

				return model;
			}
		}
		#endregion

		#region +UpdateTwOrderInvoiceForModify
		/// <summary>
		/// Update Taiwan Order Invoice For Modify
		/// </summary>
		/// <param name="model">Order Invoice Model</param>
		/// <param name="lastChanged">Last Changed</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <param name="accessor">Accessor</param>
		/// <returns>Update Count</returns>
		public int UpdateTwOrderInvoiceForModify(
			TwOrderInvoiceModel model,
			string lastChanged,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor accessor)
		{
			// 更新
			var updated = UpdateTwOrderInvoice(model, accessor);

			// 更新履歴登録
			if (updateHistoryAction == UpdateHistoryAction.Insert)
			{
				new UpdateHistoryService().InsertForOrder(model.OrderId, lastChanged, accessor);
			}

			return updated;
		}
		#endregion

		#region ~Update Taiwan Order Invoice
		/// <summary>
		/// Update Taiwan Order Invoice
		/// </summary>
		/// <param name="model">model</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>Update Count</returns>
		private int UpdateTwOrderInvoice(TwOrderInvoiceModel model, SqlAccessor accessor)
		{
			using (var repository = new TwOrderInvoiceRepository(accessor))
			{
				var result = repository.UpdateTwOrderInvoice(model);

				return result;
			}
		}
		#endregion

		#region ~UpdateTwOrderInvoiceStatus
		/// <summary>
		/// Update Taiwan Order Invoice Status
		/// </summary>
		/// <param name="orderId">Order Id</param>
		/// <param name="orderShippingNo">Order Shipping No</param>
		/// <param name="invoiceStatus">Invoice Status</param>
		/// <param name="invoiceNo">Invoice No</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>result</returns>
		public int UpdateTwOrderInvoiceStatus(
			string orderId,
			int orderShippingNo,
			string invoiceStatus,
			string invoiceNo,
			SqlAccessor accessor = null)
		{
			using (var repository = new TwOrderInvoiceRepository(accessor))
			{
				var result = repository.UpdateTwOrderInvoiceStatus(orderId, orderShippingNo, invoiceStatus, invoiceNo);

				return result;
			}
		}
		#endregion

		#region~UpdateInvoiceNoByOrderId
		/// <summary>
		/// Update InvoiceNo By Order Id
		/// </summary>
		/// <param name="invoiceNo">Invoice No</param>
		/// <param name="orderId">Order Id</param>
		public void UpdateInvoiceNoByOrderId(string invoiceNo, string orderId)
		{
			using (var repository = new TwOrderInvoiceRepository())
			{
				repository.UpdateInvoiceNoByOrderId(invoiceNo, orderId);
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="orderId">ユーザID</param>
		/// <param name="accessor">Sql Accessor</param>
		public bool Delete(string orderId, SqlAccessor accessor = null)
		{
			using (var repository = new TwOrderInvoiceRepository(accessor))
			{
				var result = repository.Delete(orderId);
				return (result > 0);
			}
		}
		#endregion
	}
}
