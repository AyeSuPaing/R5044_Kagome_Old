/*
=========================================================================================================
  Module      : Gooddeal Shipping Action (GooddealShippingAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using w2.App.Common;
using w2.App.Common.Gooddeal;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.GooddealShipping.Action
{
	/// <summary>
	/// Gooddeal shipping action
	/// </summary>
	public class GooddealShippingAction : IAction
	{
		#region +Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		public GooddealShippingAction()
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
			var orders = orderService.GetOrdersByOrderStatus(Constants.FLG_ORDER_ORDER_STATUS_ORDERED);

			foreach (var order in orders)
			{
				this.ExecuteCount++;

				var timestamp = GooddealUtility.GetTimestamp();
				var request = GooddealUtility.CreateRegisterShippingDeliveryRequest(order, timestamp);
				var response = GooddealApi.RegisterShippingDelivery(request, timestamp);
				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();
					try
					{
						orderService.UpdateLogiCooperationStatus(
							order.OrderId,
							response.IsRegisterSucceeded
								? Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED
								: order.OrderStatus,
							response.IsRegisterSucceeded
								? Constants.FLG_ORDER_LOGI_COOPERATION_STATUS_COMPLETE
								: Constants.FLG_ORDER_LOGI_COOPERATION_STATUS_ERROR,
							Constants.FLG_LASTCHANGED_BATCH,
							UpdateHistoryAction.DoNotInsert,
							accessor);

						accessor.CommitTransaction();

						var logMessage = string.Empty;
						if (response.IsRegisterSucceeded)
						{
							logMessage = string.Format(
								"注文ID：{0} -> {1}",
								order.OrderId,
								response.GetApiMessage());
							FileLogger.WriteInfo(logMessage);
							this.SuccessCount++;
							continue;
						}

						logMessage = string.Format(
							"注文ID：{0} -> {1}",
							order.OrderId,
							response.GetApiMessage());
						Console.WriteLine(logMessage);
						this.Messages.Add(logMessage);
						FileLogger.WriteError(logMessage);
						this.FailureCount++;
					}
					catch (Exception ex)
					{
						var message = string.Format(
							"注文ID：{0} -> {1}",
							order.OrderId,
							ex.ToString());
						Console.WriteLine(message);
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