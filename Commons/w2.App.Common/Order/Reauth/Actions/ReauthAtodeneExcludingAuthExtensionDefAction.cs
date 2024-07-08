/*
=========================================================================================================
  Module      : 与信切れを防ぐための再与信を除くAtodeneの再与信(ReauthAtodeneExcludingAuthExtensionDefAction.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.JACCS.ATODENE;
using w2.App.Common.Order.Payment.JACCS.ATODENE.ModifyTransaction;
using w2.Domain.Order;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// 与信切れを防ぐための再与信を除くAtodeneの再与信アクション
	/// </summary>
	public class ReauthAtodeneExcludingAuthExtensionDefAction : BaseReauthAction<ReauthCvsDefAction.ReauthActionParams>
	{
		/// <summary>再与信の元の受注ID.同梱や新規カートによるアップセルの再与信の場合、再与信前と再与信後の受注IDが違うことに注意</summary>
		private readonly string m_originalOrderId;

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメータ</param>
		/// <param name="originalOrderId">再与信前のもとの受注ID</param>
		public ReauthAtodeneExcludingAuthExtensionDefAction(
			ReauthCvsDefAction.ReauthActionParams reauthActionParams,
			string originalOrderId)
			: base(ActionTypes.Reauth, "コンビニ後払い与信", reauthActionParams)
		{
			m_originalOrderId = originalOrderId;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// コンビニ後払い与信
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthCvsDefAction.ReauthActionParams reauthActionParams)
		{
			ReauthActionResult result;
			var order = reauthActionParams.Order;

			switch (Constants.PAYMENT_CVS_DEF_KBN)
			{
				case Constants.PaymentCvsDef.Atodene:
					result = ExecAuthAtodene(order);
					break;

				// その他
				default:
					result = new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						apiErrorMessage: "該当のコンビニ後払いは再与信をサポートしていません。");
					break;
			}

			// 取引IDをセット
			if (result.Result)
			{
				order.CardTranId = result.CardTranId;
				order.PaymentOrderId = result.PaymentOrderId;
			}
			return result;
		}

		/// <summary>
		/// Atodene後払い与信処理
		/// </summary>
		/// <param name="order">注文情報</param>
		/// <returns>与信結果情報</returns>
		private ReauthActionResult ExecAuthAtodene(OrderModel order)
		{
			// 注文IDは再与信を考慮して枝番を付与
			var paymentOrderId = OrderCommon.CreatePaymentOrderId(order.ShopId);
			var orderInput = order.Clone();
			orderInput.PaymentOrderId = paymentOrderId;

			if (string.IsNullOrEmpty(orderInput.OrderIdOrg) == false)
			{
				orderInput = CreateOrderForReturnExchange(
					orderInput,
					orderInput.OrderIdOrg);
			}

			var tranAdp = new AtodeneTransactionMpdofyModelAdapter(orderInput);
			var res = tranAdp.ExecuteModify();

			// すべての成功の条件を満たしたときのみ、成功とみなす(成功時はOK以外に空も来ることに注意)
			if ((res.Result == AtodeneConst.RESULT_OK)
				&& (res.TransactionInfo != null)
				&& (res.TransactionInfo.AutoAuthoriresult != AtodeneConst.AUTO_AUTH_RESULT_PROCESSING)
				&& (res.TransactionInfo.AutoAuthoriresult != AtodeneConst.AUTO_AUTH_RESULT_NG))
			{
				return new ReauthActionResult(
					true,
					orderInput.OrderId,
					CreatePaymentMemo(order.OrderPaymentKbn, paymentOrderId, res.TransactionInfo.TransactionId, orderInput.LastBilledAmount),
					cardTranId: res.TransactionInfo.TransactionId,
					paymentOrderId: paymentOrderId);
			}

			var authLostForError = false;
			// 自動審査保留・NGはキャンセルする
			if ((res.Result == AtodeneConst.RESULT_OK)
				&& (res.TransactionInfo != null)
				&& ((res.TransactionInfo.AutoAuthoriresult == AtodeneConst.AUTO_AUTH_RESULT_PROCESSING)
					|| (res.TransactionInfo.AutoAuthoriresult == AtodeneConst.AUTO_AUTH_RESULT_NG)))
			{
				var cancelAdp = new AtodeneCancelTransactionAdapter(res.TransactionInfo.TransactionId);
				cancelAdp.ExecuteCancel(); // キャンセル失敗時は与信が残るが、運用上の違いは多くないため、エラーが起こってもここで握りつぶす
				authLostForError = true;
			}

			var apiErrors = Enumerable.Empty<string>().ToArray();
			if ((res.Errors != null)
				&& (res.Errors.Error != null)
				&& res.Errors.Error.Any())
			{
				apiErrors = res.Errors.Error
					.Select(err => LogCreator.CreateErrorMessage(err.ErrorCode, err.ErrorMessage))
					.ToArray();
			}

			return new ReauthActionResult(
				false,
				m_originalOrderId,
				string.Empty,
				cardTranIdForLog: orderInput.CardTranId,
				apiErrorMessage: string.Format(
					"コンビニ決済の与信でエラーが発生しました。与信情報に問題が発生した可能性があるため、Atodeneの与信情報をご確認ください。 注文ID : {0}、取引ID : {1}、自動審査結果 : {2}{3}{4}",
					m_originalOrderId,
					(res.TransactionInfo != null) ? res.TransactionInfo.TransactionId : "",
						(res.TransactionInfo != null)
						? res.TransactionInfo.AutoAuthoriresult
						: "不明",
					apiErrors.Any()
						? Environment.NewLine
						: string.Empty,
					string.Join("/t", apiErrors)),
				authLostForError: authLostForError);
		}

		/// <summary>
		/// 返品交換用に新規注文を作成
		/// </summary>
		/// <param name="newOrder">新規注文</param>
		/// <param name="orderIdOrg">元注文</param>
		/// <returns>新規注文</returns>
		private OrderModel CreateOrderForReturnExchange(OrderModel newOrder, string orderIdOrg)
		{
			var orgOrder = new OrderService().Get(orderIdOrg);
			var returnExchangeOrderItems = new OrderService().GetReturnExchangeOrderItems(orderIdOrg);

			var tmpOrderItems = orgOrder.Items.ToList();

			returnExchangeOrderItems.Select(
				order =>
				{
					if (tmpOrderItems.Any(item => (item.ProductId == order.ProductId) && (item.VariationId == order.VariationId)) == false)
					{
						tmpOrderItems.Add(order);
					}
					else
					{
						tmpOrderItems
							.Where(item => (item.ProductId == order.ProductId) && (item.VariationId == order.VariationId))
							.Select(item => item.ItemQuantity += order.ItemQuantity).ToArray();
					}
					return orgOrder;
				}).ToArray();

			newOrder.Items.Select(
				order =>
				{
					if (tmpOrderItems.Any(item => (item.ProductId == order.ProductId) && (item.VariationId == order.VariationId)) == false)
					{
						tmpOrderItems.Add(order);
					}
					else
					{
						tmpOrderItems
							.Where(item => (item.ProductId == order.ProductId) && (item.VariationId == order.VariationId))
							.Select(item => item.ItemQuantity += order.ItemQuantity).ToArray();
					}
					return orgOrder;
				}).ToArray();

			newOrder.Items = tmpOrderItems.Where(item => item.ItemQuantity > 0).ToArray();
			newOrder.OrderPriceShipping = orgOrder.OrderPriceShipping;
			newOrder.OrderPriceExchange = orgOrder.OrderPriceExchange;
			newOrder.OrderPriceSubtotal = tmpOrderItems.Where(item => item.ItemQuantity > 0).Sum(item => item.ProductPrice * item.ItemQuantity);

			return newOrder;
		}
		#endregion

		/// <summary>
		/// 再与信アクションパラメタ
		/// </summary>
		public new class ReauthActionParams : IReauthActionParams
		{
			#region コンストラクタ
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="order">注文情報</param>
			public ReauthActionParams(OrderModel order)
			{
				this.Order = order;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報</summary>
			public OrderModel Order { get; private set; }
			#endregion
		}
	}
}
