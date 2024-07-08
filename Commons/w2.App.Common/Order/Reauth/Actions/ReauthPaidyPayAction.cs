/*
=========================================================================================================
  Module      : Reauth Paidy Pay Action (ReauthPaidyPayAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Order.Payment.Paidy;
using w2.Domain.Order;
using w2.Domain.UserCreditCard;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Reauth Paidy Pay Action Class
	/// </summary>
	public class ReauthPaidyPayAction : BaseReauthAction<ReauthPaidyPayAction.ReauthActionParams>
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		public ReauthPaidyPayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "Paidy決済", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// Paidy決済
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderNew = reauthActionParams.OrderNew;
			var orderOld = reauthActionParams.OrderOld;
			var order = orderNew.Clone();
			var userCreditCard = new UserCreditCard(new UserCreditCardService().Get(
				order.UserId,
				order.CreditBranchNo.Value));

			// For modify order
			if (string.IsNullOrEmpty(orderNew.OrderIdOrg)
				&& (orderNew.OrderId == orderOld.OrderId))
			{
				// 最終請求金額が0円の場合、エラーとする
				if (order.LastBilledAmount == 0)
				{
					return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						apiErrorMessage: "最終請求金額が0円のため、与信できません。");
				}

				// Create cart and recalculate cart
				var cartNew = CartObject.CreateCartByOrder(order);
				cartNew.SetPriceShipping(order.OrderPriceShipping);
				cartNew.Payment.PriceExchange = order.OrderPriceExchange;
				cartNew.Calculate(false);

				cartNew.Payment.UserCreditCard = userCreditCard;
				var requestDataPayment = PaidyUtility.CreatePaidyPaymentObject(
					cartNew,
					order.DataSource,
					this.Accessor,
					true);
				var result = PaidyPaymentApiFacade.CreateTokenPayment(requestDataPayment);
				if (result.HasError)
				{
					return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						apiErrorMessage: result.GetApiErrorMessages());
				}

				// Update payment order id for order new
				orderNew.PaymentOrderId = result.Payment.Id;

				return new ReauthActionResult(
					true,
					order.OrderId,
					CreatePaymentMemo(
						order.OrderPaymentKbn,
						result.Payment.Id,
						string.Empty,
						order.LastBilledAmount),
					paymentOrderId: result.Payment.Id);
			}
			// For return exchange, reauth order
			else
			{
				var fixedPurchaseId = string.Empty;
				var orderIdOrg = string.IsNullOrEmpty(orderOld.OrderIdOrg)
					? orderOld.OrderId
					: orderOld.OrderIdOrg;
				var cartNew = CreatedCart(orderOld, orderNew, orderIdOrg, out fixedPurchaseId);
				order.OrderId = orderIdOrg;
				order.FixedPurchaseId = fixedPurchaseId;
				cartNew.Payment.UserCreditCard = userCreditCard;

				var requestDataPayment = PaidyUtility.CreatePaidyPaymentObject(
					cartNew,
					order.DataSource,
					this.Accessor,
					true);
				var result = PaidyPaymentApiFacade.CreateTokenPayment(requestDataPayment);
				if (result.HasError)
				{
					return new ReauthActionResult(
						false,
						order.OrderId,
						string.Empty,
						apiErrorMessage: result.GetApiErrorMessages());
				}

				// Update payment order id for order new
				orderNew.PaymentOrderId = result.Payment.Id;

				return new ReauthActionResult(
					true,
					order.OrderId,
					CreatePaymentMemo(
						order.OrderPaymentKbn,
						result.Payment.Id,
						string.Empty,
						order.LastBilledAmount),
					paymentOrderId: result.Payment.Id);
			}
		}

		/// <summary>
		/// Created Cart
		/// </summary>
		/// <param name="orderOld">Order Old</param>
		/// <param name="orderNew">Order New</param>
		/// <param name="orderIdOrg">Order Id Org</param>
		/// <param name="fixedPurchaseId">Fixed Purchase Id</param>
		/// <returns>Cart Object</returns>
		private CartObject CreatedCart(
			OrderModel orderOld,
			OrderModel orderNew,
			string orderIdOrg,
			out string fixedPurchaseId)
		{
			var orderService = new OrderService();
			var order = orderService.Get(orderIdOrg, this.Accessor);
			fixedPurchaseId = order.FixedPurchaseId;
			var orderItems = orderService.GetRelatedOrderItems(
				orderIdOrg,
				this.Accessor);

			var modelList = new List<OrderItemModel>();
			foreach (var item in orderItems)
			{
				if (modelList.Any(data => (data.ProductId == item.ProductId) && (data.VariationId == item.VariationId)) == false)
				{
					modelList.Add(item);
				}
				else
				{
					modelList.Where(data => (data.ProductId == item.ProductId) && (data.VariationId == item.VariationId))
						.Select(data => data.ItemQuantity += item.ItemQuantity).ToArray();
				}
			}

			// For return exchange order
			if (orderOld.OrderId != orderNew.OrderId)
			{
				foreach (var item in orderNew.Shippings[0].Items)
				{
					if (modelList.Any(data => (data.ProductId == item.ProductId) && (data.VariationId == item.VariationId)) == false)
					{
						modelList.Add(item);
					}
					else
					{
						modelList.Where(data => (data.ProductId == item.ProductId) && (data.VariationId == item.VariationId))
							.Select(data => data.ItemQuantity += item.ItemQuantity).ToArray();
					}
				}

				// Set order point use for return exchange order
				order.OrderPointUse = order.LastOrderPointUse + orderNew.OrderPointUse;
			}
			else
			{
				// Set order point use for reauth order exchange order
				order.OrderPointUse = order.LastOrderPointUse;
			}

			// Set items
			order.Shippings[0].Items = modelList.Where(data => (data.ItemQuantity > 0)).ToArray();

			// Create cart and recalculate cart because this cart has item changed
			var cart = CartObject.CreateCartByOrder(order);
			cart.SetPriceShipping(order.OrderPriceShipping);
			cart.Payment.PriceExchange = order.OrderPriceExchange;
			cart.PriceRegulation = orderNew.LastBilledAmount - cart.PriceTotal;
			cart.Calculate(false, true);

			return cart;
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
			/// <param name="orderOld">注文情報（変更前）</param>
			/// <param name="orderNew">注文情報（変更後）</param>
			public ReauthActionParams(OrderModel orderOld, OrderModel orderNew)
			{
				this.OrderOld = orderOld;
				this.OrderNew = orderNew;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報（変更前）</summary>
			public OrderModel OrderOld { get; private set; }
			/// <summary>注文情報（変更後）</summary>
			public OrderModel OrderNew { get; private set; }
			#endregion
		}
	}
}
