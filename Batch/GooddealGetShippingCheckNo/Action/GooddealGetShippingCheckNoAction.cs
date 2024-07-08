/*
=========================================================================================================
  Module      : Gooddeal get shipping check no action (GooddealGetShippingCheckNoAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Gooddeal;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.GooddealGetShippingCheckNo.Action
{
	/// <summary>
	/// Gooddeal get shipping check no action
	/// </summary>
	public class GooddealGetShippingCheckNoAction : IAction
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public GooddealGetShippingCheckNoAction()
		{
			this.BeginTime = DateTime.Now;
			this.EndTime = DateTime.Now;
			this.ExecuteCount = 0;
			this.SuccessCount = 0;
			this.FailureCount = 0;
			this.Messages = new List<string>();
		}
		#endregion

		#region +Methods
		/// <summary>
		/// Execute
		/// </summary>
		public void Execute()
		{
			// Get orders
			var orderService = new OrderService();
			var orders = orderService.GetOrdersByOrderStatus(Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED);

			foreach (var order in orders)
			{
				this.ExecuteCount++;

				var timestamp = GooddealUtility.GetTimestamp();
				var request = GooddealUtility.CreateGetShippingDeliverySlipRequest(order.OrderId, timestamp);
				var response = GooddealApi.GetShippingDeliverySlip(request, timestamp);
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();
					try
					{
						if (response.IsGetDeliverySlipSucceeded == false)
						{
							var message = string.Format(
								"注文ID：{0} -> {1}",
								order.OrderId,
								response.GetApiMessage());
							this.Messages.Add(message);
							FileLogger.WriteError(message);
							this.FailureCount++;
							continue;
						}

						// Update order status
						orderService.UpdateOrderStatus(
							order.OrderId,
							Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED,
							DateTime.Now,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.DoNotInsert,
							accessor);

						// Update shipping check no
						var orderShipping = order.Shippings.FirstOrDefault();
						orderShipping.ShippingDate = (string.IsNullOrEmpty(response.Response.DateTime) == false)
							? DateTime.Parse(response.Response.DateTime)
							: (DateTime?)null;
						orderShipping.ShippingCheckNo = response.Response.DeliverNo;
						orderService.UpdateOrderShipping(orderShipping, accessor);

						accessor.CommitTransaction();
						this.SuccessCount++;
					}
					catch (Exception ex)
					{
						var message = string.Format(
							"注文ID：{0} -> {1}",
							order.OrderId,
							ex.ToString());
						FileLogger.WriteError(message);
						this.Messages.Add(message);
						this.FailureCount++;
					}
				}
			}

			this.EndTime = DateTime.Now;

			// Send mail for operator
			SendMailForOperator();
		}

		/// <summary>
		/// Send mail for operator
		/// </summary>
		private void SendMailForOperator()
		{
			GooddealUtility.SendMailForOperator(
				this.BeginTime,
				this.EndTime,
				this.ExecuteCount,
				this.SuccessCount,
				this.FailureCount,
				this.Messages.ToArray());
		}
		#endregion

		#region +Properties
		/// <summary>Begin time</summary>
		private DateTime BeginTime { get; set; }
		/// <summary>End time</summary>
		private DateTime EndTime { get; set; }
		/// <summary>Execute count</summary>
		private int ExecuteCount { get; set; }
		/// <summary>Success count</summary>
		private int SuccessCount { get; set; }
		/// <summary>Failure count</summary>
		private int FailureCount { get; set; }
		/// <summary>Message</summary>
		private List<string> Messages { get; set; }
		#endregion
	}
}