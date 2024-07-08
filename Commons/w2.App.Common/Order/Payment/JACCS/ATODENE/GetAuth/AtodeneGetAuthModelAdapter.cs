/*
=========================================================================================================
  Module      : Atodene与信取得モデルアダプタ(AtodeneGetAuthModelAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.GetAuth
{
	/// <summary>
	/// Atodene与信取得モデルアダプタ
	/// </summary>
	public class AtodeneGetAuthModelAdapter : BaseAtodeneGetAuthAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private AtodeneGetAuthModelAdapter()
			: base()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文モデル（注文者、注文配送先、注文明細のモデルを内包していること）</param>
		public AtodeneGetAuthModelAdapter(OrderModel order)
			: this(order, null)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文モデル（注文者、注文配送先、注文明細のモデルを内包していること）</param>
		/// <param name="apiSetting">Api設定</param>
		public AtodeneGetAuthModelAdapter(OrderModel order, AtodeneApiSetting apiSetting)
			: base(apiSetting)
		{
			this.Order = order;
		}

		/// <summary>
		/// トランザクションID取得
		/// </summary>
		/// <returns></returns>
		public override string GetTransactionId()
		{
			return this.Order.PaymentOrderId;
		}

		/// <summary>
		/// 注文
		/// </summary>
		public OrderModel Order { get; set; }
	}
}
