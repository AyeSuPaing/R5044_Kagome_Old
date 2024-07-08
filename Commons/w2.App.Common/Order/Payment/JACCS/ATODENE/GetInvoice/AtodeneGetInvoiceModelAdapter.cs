/*
=========================================================================================================
  Module      : Atodene請求書印字データ取得モデルアダプタ(AtodeneGetInvoiceModelAdapter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using w2.Domain.Order;

namespace w2.App.Common.Order.Payment.JACCS.ATODENE.GetInvoice
{
	/// <summary>
	/// Atodene請求書印字データ取得モデルアダプタ
	/// </summary>
	public class AtodeneGetInvoiceModelAdapter : BaseAtodeneGetInvoiceAdapter
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		private AtodeneGetInvoiceModelAdapter()
			: base()
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文モデル（注文者、注文配送先、注文明細のモデルを内包していること）</param>
		public AtodeneGetInvoiceModelAdapter(OrderModel order)
			: this(order, null)
		{

		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="order">注文モデル（注文者、注文配送先、注文明細のモデルを内包していること）</param>
		/// <param name="apiSetting">API設定</param>
		public AtodeneGetInvoiceModelAdapter(OrderModel order, AtodeneApiSetting apiSetting)
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
			return this.Order.CardTranId;
		}

		/// <summary>
		/// 注文
		/// </summary>
		public OrderModel Order { get; set; }
	}
}
