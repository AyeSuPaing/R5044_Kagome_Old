/*
=========================================================================================================
  Module      : DSK後払い請求書印字データ取得アダプタ(DskDeferredGetInvoiceAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Extensions.Currency;
using w2.Domain.InvoiceDskDeferred;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.DSKDeferred.GetInvoice
{
	/// <summary>
	/// DSK後払い請求書印字データ取得アダプタ
	/// </summary>
	public class DskDeferredGetInvoiceAdapter : BaseDskDeferredAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="apiSetting">API接続設定</param>
		public DskDeferredGetInvoiceAdapter(OrderModel order, DskDeferredApiSetting apiSetting = null)
			: base(apiSetting)
		{
			this.Order = order;
		}

		/// <summary>
		/// リクエスト生成
		/// </summary>
		/// <returns>リクエスト</returns>
		public DskDeferredGetInvoiceRequest CreateRequest()
		{
			var request = new DskDeferredGetInvoiceRequest();

			request.Transaction.TransactionId = this.Order.CardTranId;
			request.Transaction.ShopTransactionId = this.Order.PaymentOrderId;
			request.Transaction.BilledAmount = this.Order.LastBilledAmount.ToPriceString();

			return request;
		}

		/// <summary>
		/// 請求書印字データ取得実行
		/// </summary>
		/// <returns>レスポンス</returns>
		public DskDeferredGetInvoiceResponse Execute()
		{
			var facade = (this.ApiSetting == null) ? new DskDeferredApiFacade() : new DskDeferredApiFacade(this.ApiSetting);
			var response = facade.GetInvoiceData(CreateRequest());
			return response;
		}

		/// <summary>
		/// 請求書を登録
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="response">レスポンス</param>
		public void InsertInvoice(string orderId, DskDeferredGetInvoiceResponse response)
		{
			var model = response.CreateModel();
			model.OrderId = orderId;
			foreach (var detail in model.Details)
			{
				detail.OrderId = orderId;
			}

			new InvoiceDskDeferredService().Insert(model);
		}

		/// <summary>注文</summary>
		private OrderModel Order { get; set; }
	}
}
