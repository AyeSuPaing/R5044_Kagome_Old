/*
=========================================================================================================
  Module      : Atodene出荷報告モデルアダプタ(AtodeneShippingModelAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using w2.App.Common.Order.Payment.JACCS.ATODENE.Helper;
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.Shipping
{
	/// <summary>
	/// Atodene出荷報告モデルアダプタ
	/// </summary>
	public class AtodeneShippingModelAdapter : BaseAtodeneShippingAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private AtodeneShippingModelAdapter()
			: base()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文モデル</param>
		public AtodeneShippingModelAdapter(OrderModel order)
			: this(order, null)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文モデル</param>
		/// <param name="apiSetting">API設定</param>
		public AtodeneShippingModelAdapter(OrderModel order, AtodeneApiSetting apiSetting)
			: base(apiSetting)
		{
			this.Order = order;
		}

		/// <summary>
		/// お問い合わせ番号取得
		/// </summary>
		/// <returns>お問い合わせ番号</returns>
		public override string GetTransactionId()
		{
			return this.Order.PaymentOrderId;
		}

		/// <summary>
		/// 配送伝票番号取得
		/// </summary>
		/// <returns>配送伝票番号</returns>
		public override string GetDeliverySlipNo()
		{
			return this.Order.Shippings.First().ShippingCheckNo;
		}

		/// <summary>
		/// 運送会社コード取得
		/// </summary>
		/// <returns>運送会社コード</returns>
		public override string GetDeliveryCompanyCode()
		{
			return PaymentAtodeneDeliveryServiceCode.GetDeliveryServiceCode(this.Order.Shippings.First().DeliveryCompanyId);
		}

		/// <summary>
		/// 注文モデル
		/// </summary>
		public OrderModel Order { get; set; }
	}
}
