/*
=========================================================================================================
  Module      : 取引実行リクエスト (ExecTranRequest.cs)
･････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System.Linq;
using w2.Common.Web;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.Paypay.Request
{
	/// <summary>
	/// 取引登録リクエスト
	/// </summary>
	public class ExecTranRequest : PaypayGmoRequest
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public ExecTranRequest()
		{
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <param name="accessId">取引ID</param>
		/// <param name="accessPassword">取引パスワード</param>
		public ExecTranRequest(OrderModel order, string accessId, string accessPassword)
		{
			var urlPath = string.Format("{0}{1}{2}{3}",
				Constants.PROTOCOL_HTTPS,
				Constants.SITE_DOMAIN,
				Constants.PATH_ROOT_FRONT_PC,
				Constants.PAGE_FRONT_PAYMENT_PAYPAY_RECEIVE);
			this.ReturnUrl = new UrlCreator(urlPath)
				.AddParam(PaypayConstants.REQUEST_KEY_ORDERID, order.OrderId)
				.CreateUrl();
			this.PaymentOrderId = order.IsFixedPurchaseOrder
				? order.FixedPurchaseId
				: order.PaymentOrderId;
			this.AccessId = accessId;
			this.AccessPassword = accessPassword;
			this.Items = order.Shippings
				.SelectMany(os => os.Items)
				.Where(oi => (oi.ItemPrice > 0))
				.Select(
					oi => new Item
					{
						CategoryId = Constants.PAYMENT_PAYPAY_CATEGORY_ID,
					})
				.ToArray();
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="order">Order model</param>
		/// <param name="fixedPurchaseModel">Fixed purchase model</param>
		/// <param name="accessId">Access id</param>
		/// <param name="accessPassword">Access password</param>
		public ExecTranRequest(
			OrderModel order,
			FixedPurchaseModel fixedPurchaseModel,
			string accessId,
			string accessPassword)
		{
			this.AccessId = accessId;
			this.AccessPassword = accessPassword;
			this.PaymentOrderId = order.PaymentOrderId;
			this.PaypayAcceptCode = fixedPurchaseModel.ExternalPaymentAgreementId;
		}

		/// <summary>店舗ID</summary>
		[JsonProperty("shopID")]
		public string ShopId
		{
			get { return Constants.PAYMENT_PAYPAY_SHOP_ID; }
		}
		/// <summary>店舗パスワード</summary>
		[JsonProperty("shopPass")]
		public string ShopPassword
		{
			get { return Constants.PAYMENT_PAYPAY_SHOP_PASSWORD; }
		}
		/// <summary>取引ID</summary>
		[JsonProperty("accessID")]
		public string AccessId { get; set; }
		/// <summary>取引パスワード</summary>
		[JsonProperty("accessPass")]
		public string AccessPassword { get; set; }
		/// <summary>注文ID（ｗ２側の決済注文ID）</summary>
		[JsonProperty("orderID")]
		public string PaymentOrderId { get; set; }
		/// <summary>注文商品</summary>
		[JsonProperty("items")]
		public Item[] Items { get; set; }
		/// <summary>カテゴリID</summary>
		[JsonProperty("categoryId")]
		public string CategoryId
		{
			get { return Constants.PAYMENT_PAYPAY_CATEGORY_ID; }
		}
		/// <summary>決済結果戻しURL</summary>
		[JsonProperty("retURL")]
		public string ReturnUrl { get; set; }
		/// <summary>User Info</summary>
		[JsonProperty("userInfo")]
		public string UserInfo { get; set; }
		/// <summary>PayPay承諾番号</summary>
		[JsonProperty("paypayAcceptCode")]
		public string PaypayAcceptCode { get; set; }

		/// <summary>
		/// 取引商品
		/// </summary>
		public class Item
		{
			/// <summary>カテゴリID</summary>
			[JsonProperty("categoryId")]
			public string CategoryId { get; set; }
		}
	}
}
