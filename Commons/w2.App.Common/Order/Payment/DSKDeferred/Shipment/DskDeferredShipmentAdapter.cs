/*
=========================================================================================================
  Module      : DSK後払い出荷報告アダプタ(DskDeferredShipmentAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using w2.App.Common.Extensions.Currency;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.DSKDeferred.Shipment
{
	/// <summary>
	/// DSK後払い出荷報告アダプタ
	/// </summary>
	public class DskDeferredShipmentAdapter : BaseDskDeferredAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文</param>
		/// <param name="transactionId">取引ID</param>
		/// <param name="ShopTransactionId">加盟店取引ID</param>
		/// <param name="deliverySlipNo">配送伝票番号</param>
		/// <param name="deliveryServiceCode">出荷連携配送会社コード</param>
		/// <param name="apiSetting">API接続設定</param>
		public DskDeferredShipmentAdapter(OrderModel order, string transactionId, string deliverySlipNo, string deliveryServiceCode, DskDeferredApiSetting apiSetting = null)
			: base(apiSetting)
		{
			this.Order = order;
			this.TransactionId = transactionId;
			this.DeliverySlipNo = deliverySlipNo;
			this.DeliveryServiceCode = deliveryServiceCode;
		}

		/// <summary>
		/// リクエスト生成
		/// </summary>
		/// <returns>リクエストデータ</returns>
		public DskDeferredShipmentRequest CreateRequest()
		{
			var request = new DskDeferredShipmentRequest();

			request.Transaction.TransactionId = this.TransactionId;
			request.Transaction.ShopTransactionId = this.Order.PaymentOrderId;
			request.Transaction.BilledAmount = this.Order.LastBilledAmount.ToPriceString();
			request.PdRequest.PdCompanyCode = this.DeliveryServiceCode;
			request.PdRequest.SlipNo = this.DeliverySlipNo;
			request.PdRequest.Address1 = this.Order.Shippings[0].ShippingAddr1;
			request.PdRequest.Address2 = this.Order.Shippings[0].ShippingAddr2;
			request.PdRequest.Address3 = this.Order.Shippings[0].ShippingAddr3 + "　" + this.Order.Shippings[0].ShippingAddr4;

			return request;
		}

		/// <summary>
		/// 取引登録実行
		/// </summary>
		/// <returns>レスポンスデータ</returns>
		public DskDeferredShipmentResponse Execute()
		{
			var facade = (this.ApiSetting == null) ? new DskDeferredApiFacade() : new DskDeferredApiFacade(this.ApiSetting);
			var response = facade.Shipment(CreateRequest());
			return response;
		}

		/// <summary>注文</summary>
		private OrderModel Order { get; set; }
		/// <summary>お問い合わせ番号</summary>
		private string TransactionId { get; set; }
		/// <summary>配送伝票番号</summary>
		private string DeliverySlipNo { get; set; }
		/// <summary>出荷連携配送会社コード</summary>
		private string DeliveryServiceCode { get; set; }
	}
}
