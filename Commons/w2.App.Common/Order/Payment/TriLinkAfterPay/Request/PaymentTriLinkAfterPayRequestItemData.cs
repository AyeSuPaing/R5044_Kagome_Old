/*
=========================================================================================================
  Module      : 後付款(TriLink後払い) リクエストデータ(Item)生成クラス(PaymentTriLinkAfterPayRequestItemData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using w2.Common.Util;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.TriLinkAfterPay.Request
{
	/// <summary>
	/// 後付款(TriLink後払い) リクエストデータ(Item)生成クラス
	/// </summary>
	[JsonObject]
	public class PaymentTriLinkAfterPayRequestItemData
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="product">購入商品</param>
		/// <param name="cart">カート</param>
		public PaymentTriLinkAfterPayRequestItemData(CartProduct product, CartObject cart)
			: this(product.ProductJointName, Decimal.ToInt32(product.Price), product.Count , cart.SettlementRate)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="item">購入商品</param>
		/// <param name="order">注文</param>
		public PaymentTriLinkAfterPayRequestItemData(OrderItemModel item, OrderModel order)
			: this(item.ProductName, Decimal.ToInt32(item.ProductPrice), item.ItemQuantity, order.SettlementRate)
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="name">商品名</param>
		/// <param name="price">商品単価</param>
		/// <param name="quantity">個数</param>
		/// <param name="settlementRate">決済レート</param>
		private PaymentTriLinkAfterPayRequestItemData(string name, int price, int quantity, decimal settlementRate)
		{
			this.Name = name;
			this.Price = Decimal.ToInt32(DecimalUtility.DecimalRound(price * settlementRate, DecimalUtility.Format.Round, 0));
			this.Quantity = quantity;
		}
		#endregion

		#region プロパティ
		/// <summary>商品名</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_NAME)]
		public string Name { get; set; }
		/// <summary>商品単価</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ITEMS_PRICE)]
		public int Price { get; set; }
		/// <summary>個数</summary>
		[JsonProperty(TriLinkAfterPayConstants.TW_AFTERPAY_FIELD_ITEMS_QUANTITY)]
		public int Quantity { get; set; }
		#endregion
	}
}
