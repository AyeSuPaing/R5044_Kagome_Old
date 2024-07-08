/*
=========================================================================================================
  Module      : Order status(OrderStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common.Order;
using w2.Database.Common;
using w2.Domain.Order;

namespace w2.ExternalAPI.Common.Command.ApiCommand.Order
{
	/// <summary>
	/// Order status class
	/// </summary>
	public class OrderStatus : ApiCommandBase
	{
		#region Constants
		private const string FTP_ORDER_SHP_COMP = "800";
		private const string FTP_ORDER_DLV_COMP = "950";
		private const string FTP_ORDER_ORR_CNSL = "999";
		#endregion

		#region #Execute
		/// <summary>
		/// Excute
		/// </summary>
		/// <param name="apiCommandArg">Api command argument</param>
		/// <returns>Result</returns>
		protected override ApiCommandResult Execute(ApiCommandArgBase apiCommandArg = null)
		{
			var ordersStatusArg = (OrderStatusArg)apiCommandArg;
			var updated = 0;

			var orderId = ordersStatusArg.OrderId.Split('-')[1];
			var order = new OrderService().Get(orderId);

			DateTime dateTime;
			var isUpdateOrderPaymentInfo = (DateTime.TryParse(ordersStatusArg.OrderPaymentDate.ToString(), out dateTime)
				&& (order.OrderPaymentKbn != Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT));

			// Update order status
			updated = (isUpdateOrderPaymentInfo)
				? OrderCommon.UpdateOrderStatus(orderId, ConvertOrderStatus(ordersStatusArg.OrderStatus), Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE, ordersStatusArg.OrderPaymentDate)
				: OrderCommon.UpdateOrderStatus(orderId, ConvertOrderStatus(ordersStatusArg.OrderStatus), order.OrderPaymentStatus, order.OrderPaymentDate);

			return (updated > 0) ? new OrdersStatusResult(EnumResultStatus.Complete) : new OrdersStatusResult(EnumResultStatus.Faile);
		}
		#endregion

		#region ConvertOrderStatus
		/// <summary>
		/// Convert order status
		/// </summary>
		/// <param name="ftpOrderStatus">Ftp order status</param>
		/// <returns>Order status</returns>
		private string ConvertOrderStatus(string ftpOrderStatus)
		{
			switch (ftpOrderStatus)
			{
				case FTP_ORDER_SHP_COMP:
					return Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP;

				case FTP_ORDER_DLV_COMP:
					return Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP;

				default:
					return Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED;
			}
		}
		#endregion
	}

	#region Argument
	/// <summary>
	/// Order Status Arg
	/// </summary>
	public class OrderStatusArg : ApiCommandArgBase
	{
		#region Constructor
		/// <summary>
		/// Orders Status Arg
		/// </summary>
		public OrderStatusArg()
		{
			this.OrderId = string.Empty;
			this.OrderStatus = string.Empty;
			this.OrderPaymentDate = DateTime.Now;
		}
		#endregion

		#region Properties
		/// <summary>Order id</summary>
		public string OrderId { get; set; }
		/// <summary>Order status<summary>
		public string OrderStatus { get; set; }
		/// <summary>Payment date</summary>
		public DateTime? OrderPaymentDate { get; set; }
		#endregion
	}
	#endregion

	#region Result
	/// <summary>
	/// Order status result
	/// </summary>
	public class OrdersStatusResult : ApiCommandResult
	{
		#region Constructor
		/// <summary>
		/// Order status result
		/// </summary>
		public OrdersStatusResult(EnumResultStatus enumResultStatus)
			: base(enumResultStatus)
		{
		}
		#endregion
	}
	#endregion
}