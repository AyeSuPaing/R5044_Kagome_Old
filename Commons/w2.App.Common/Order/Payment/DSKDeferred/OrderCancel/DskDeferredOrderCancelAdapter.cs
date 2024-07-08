/*
=========================================================================================================
  Module      : DSK後払い注文情報キャンセルアダプタ(DskDeferredOrderCancelAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

namespace w2.App.Common.Order.Payment.DSKDeferred.OrderCancel
{
	/// <summary>
	/// DSK後払い注文情報キャンセルアダプタ
	/// </summary>
	public class DskDeferredOrderCancelAdapter : BaseDskDeferredAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="cardTranId">決済取引ID</param>
		/// <param name="paymentOrderId">決済注文ID</param>
		/// <param name="billedAmount">請求金額</param>
		/// <param name="apiSetting">API接続設定</param>
		public DskDeferredOrderCancelAdapter(string cardTranId, string paymentOrderId, string billedAmount, DskDeferredApiSetting apiSetting = null)
			: base(apiSetting)
		{
			this.CardTranId = cardTranId;
			this.PaymentOrderId = paymentOrderId;
			this.BilledAmount = billedAmount;
		}

		/// <summary>
		/// リクエスト生成
		/// </summary>
		/// <returns>リクエスト</returns>
		public DskDeferredOrderCancelRequest CreateRequest()
		{
			var request = new DskDeferredOrderCancelRequest();

			request.Transaction.TransactionId = this.CardTranId;
			request.Transaction.ShopTransactionId = this.PaymentOrderId;
			request.Transaction.BilledAmount = this.BilledAmount;
			return request;
		}

		/// <summary>
		/// 取引登録実行
		/// </summary>
		/// <returns>レスポンス</returns>
		public DskDeferredOrderCancelResponse Execute()
		{
			var facade = (this.ApiSetting == null) ? new DskDeferredApiFacade() : new DskDeferredApiFacade(this.ApiSetting);
			var response = facade.OrderCancel(CreateRequest());
			return response;
		}

		/// <summary>決済取引ID</summary>
		private string CardTranId { get; set; }
		/// <summary>決済注文ID</summary>
		private string PaymentOrderId { get; set; }
		/// <summary>請求金額</summary>
		private string BilledAmount { get; set; }
	}
}
