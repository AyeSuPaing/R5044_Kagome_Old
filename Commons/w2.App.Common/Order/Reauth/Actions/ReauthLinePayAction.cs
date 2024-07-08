/*
=========================================================================================================
  Module      : Reauth Line Pay Action(ReauthLinePayAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.LinePay;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.App.Common.Order.Reauth.Actions
{
	/// <summary>
	/// Reauth Line Pay Action
	/// </summary>
	public class ReauthLinePayAction : BaseReauthAction<ReauthLinePayAction.ReauthActionParams>
	{
		/// <summary>配送料</summary>
		protected const string CONST_SHIPPING_FEE = "配送料";
		/// <summary>決済手数料</summary>
		protected const string CONST_SETTLEMENT_FEE = "決済手数料";
		/// <summary>調整金額</summary>
		protected const string CONST_REGULATION = "調整金額";

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="reauthActionParams">Reauth action params</param>
		public ReauthLinePayAction(ReauthActionParams reauthActionParams)
			: base(ActionTypes.Reauth, "LINEPay決済与信", reauthActionParams)
		{
		}
		#endregion

		#region メソッド
		/// <summary>
		/// LINE Pay 与信
		/// </summary>
		/// <param name="reauthActionParams">再与信アクションパラメタ</param>
		/// <returns>再与信アクション結果</returns>
		protected override ReauthActionResult OnExecute(ReauthActionParams reauthActionParams)
		{
			var orderOld = reauthActionParams.OrderOld;
			var orderNew = reauthActionParams.OrderNew;
			var paymentOrderId = orderNew.PaymentOrderId;
			var userService = new UserService();
			var userExtend = userService.GetUserExtend(orderNew.UserId);
			var regKey = ((userExtend != null)
				? StringUtility.ToEmpty(userExtend.UserExtendDataValue[Constants.LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY])
				: string.Empty);

			var order = CreateOrder(orderNew);
			var productNameJoins = (string.IsNullOrEmpty(orderNew.OrderIdOrg)
				&& (orderOld.OrderId == orderNew.OrderId))
					? string.Join(",", GetProductNames(orderNew.Shippings[0].Items))
					: string.Join(",", GetProductNames(order.Shippings[0].Items));

			if (string.IsNullOrEmpty(productNameJoins))
			{
				productNameJoins = GetNameOfFee(order, orderNew);
			}

			if (orderNew.IsNeedConfirmLinePayPayment)
			{
				var dataConfirm = new LinePayConfirmPaymentRequest()
				{
					Amount = decimal.ToInt32(orderNew.SettlementAmount),
					Currency = orderNew.SettlementCurrency
				};
				var responseConfirm = LinePayApiFacade.ConfirmPayment(
					orderNew.CardTranId,
					dataConfirm,
					new LinePayApiFacade.LinePayLogInfo(orderNew));

				if (responseConfirm.IsSuccess == false)
				{
					return new ReauthActionResult(
						false,
						orderNew.OrderId,
						string.Empty,
						cardTranIdForLog: orderNew.CardTranId,
						apiErrorMessage: LogCreator.CreateErrorMessage(responseConfirm.ReturnCode, responseConfirm.ReturnMessage));
				}

				// Update User Extend
				userExtend.UserExtendDataValue[Constants.LINEPAY_USEREXRTEND_COLUMNNAME_REGKEY] = responseConfirm.Info.RegKey;
				userService.UpdateUserExtend(
					userExtend,
					orderNew.UserId,
					Constants.FLG_LASTCHANGED_USER,
					UpdateHistoryAction.DoNotInsert);
			}
			else
			{
				paymentOrderId = OrderCommon.CreatePaymentOrderId(orderNew.ShopId);
				var response = LinePayApiFacade.PreapprovedPayment(
					regKey,
					productNameJoins,
					orderNew.SettlementAmount,
					orderNew.SettlementCurrency,
					paymentOrderId,
					Constants.PAYMENT_LINEPAY_PAYMENTCAPTURENOW,
					new LinePayApiFacade.LinePayLogInfo(orderNew));

				if (response.IsSuccess == false)
				{
					return new ReauthActionResult(
						false,
						orderNew.OrderId,
						string.Empty,
						cardTranIdForLog: orderNew.CardTranId,
						apiErrorMessage: LogCreator.CreateErrorMessage(response.ReturnCode, response.ReturnMessage));
				}
				orderNew.CardTranId = response.Info.TransactionId;
			}

			return new ReauthActionResult(
				true,
				orderNew.OrderId,
				CreatePaymentMemo(
					orderNew.OrderPaymentKbn,
					paymentOrderId,
					orderNew.CardTranId,
					orderNew.LastBilledAmount),
				cardTranId: orderNew.CardTranId,
				paymentOrderId: paymentOrderId,
				cardTranIdForLog: orderNew.CardTranId);
		}

		/// <summary>
		/// Create Order
		/// </summary>
		/// <param name="orderNew">Order New</param>
		/// <returns>Order</returns>
		private OrderModel CreateOrder(OrderModel orderNew)
		{
			var orderIdOrg = string.IsNullOrEmpty(orderNew.OrderIdOrg)
				? orderNew.OrderId
				: orderNew.OrderIdOrg;
			var orderService = new OrderService();
			var orderItems = orderService.GetRelatedOrders(orderIdOrg, this.Accessor);
			var newOrderItems = new List<OrderItemModel>();

			foreach (var items in orderItems)
			{
				foreach (var item in items.Shippings[0].Items)
				{
					if (ExistProductInOrderItems(newOrderItems, item) == false)
					{
						newOrderItems.Add(item);
						continue;
					}

					foreach (var orderItem in newOrderItems.Where(data => IsSameProduct(data, item)))
					{
						orderItem.ItemQuantity += item.ItemQuantity;
					}
				}
			}

			foreach (var item in orderNew.Shippings[0].Items)
			{
				if (ExistProductInOrderItems(newOrderItems, item) == false)
				{
					newOrderItems.Add(item);
					continue;
				}

				foreach (var orderItem in newOrderItems.Where(data => IsSameProduct(data, item)))
				{
					orderItem.ItemQuantity += item.ItemQuantity;
				}
			}

			var order = orderService.Get(orderIdOrg, this.Accessor);
			order.Shippings[0].Items = newOrderItems.Where(data => (data.ItemQuantity > 0)).ToArray();

			return order;
		}

		/// <summary>
		/// Get list of product names
		/// </summary>
		/// <param name="orderItems">Order items</param>
		/// <returns>List of product names</returns>
		private List<string> GetProductNames(OrderItemModel[] orderItems)
		{
			var result = orderItems.Where(product => (product.ItemQuantity > 0))
				.Select(product => product.ProductName)
				.ToList();
			return result;
		}

		/// <summary>
		/// Check two orders has the same ID
		/// </summary>
		/// <param name="firstOrderItem">First order item to compare</param>
		/// <param name="secondOrderItem">Second order item to compare</param>
		/// <returns>True two order has the same ID, otherwise false</returns>
		private bool IsSameProduct(OrderItemModel firstOrderItem, OrderItemModel secondOrderItem)
		{
			var result = ((firstOrderItem.ProductId == secondOrderItem.ProductId)
				&& (firstOrderItem.VariationId == secondOrderItem.VariationId));
			return result;
		}

		/// <summary>
		/// Exist product in order items
		/// </summary>
		/// <param name="orderItems">List of order items</param>
		/// <param name="orderItem">Order item to compare</param>
		/// <returns>True if exist product in order items, otherwise false</returns>
		private bool ExistProductInOrderItems(List<OrderItemModel> orderItems, OrderItemModel orderItem)
		{
			var result = orderItems.Any(item => IsSameProduct(item, orderItem));
			return result;
		}

		/// <summary>
		/// Get Name Of Fee
		/// </summary>
		/// <param name="orderOld">Order Old</param>
		/// <param name="orderNew">Order New</param>
		/// <returns>Name Of Fee</returns>
		protected string GetNameOfFee(OrderModel orderOld, OrderModel orderNew)
		{
			var nameFee = string.Empty;
			if (orderOld.OrderPriceShipping > 0)
			{
				nameFee = CONST_SHIPPING_FEE;
			}
			if (orderOld.OrderPriceExchange > 0)
			{
				nameFee += (string.IsNullOrEmpty(nameFee) == false)
					? "," + CONST_SETTLEMENT_FEE
					: CONST_SETTLEMENT_FEE;
			}
			if (orderNew.OrderPriceRegulationTotal > 0)
			{
				nameFee += (string.IsNullOrEmpty(nameFee) == false)
					? "," + CONST_REGULATION
					: CONST_REGULATION;
			}
			return nameFee;
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
			/// <param name="orderOld">注文情報(変更後)</param>
			/// <param name="orderNew">注文情報（変更後）</param>
			public ReauthActionParams(OrderModel orderOld, OrderModel orderNew)
			{
				this.OrderOld = orderOld;
				this.OrderNew = orderNew;
			}
			#endregion

			#region プロパティ
			/// <summary>注文情報(変更後)</summary>
			public OrderModel OrderOld { get; private set; }
			/// <summary>注文情報（変更後）</summary>
			public OrderModel OrderNew { get; private set; }
			#endregion
		}
	}
}
