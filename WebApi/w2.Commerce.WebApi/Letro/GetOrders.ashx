<%--
=========================================================================================================
  Module      : Get Orders (GetOrders.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="Letro.Order.GetOrders" %>

using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Web;
using w2.App.Common;
using w2.Common.Util;
using w2.Domain;

namespace Letro.Order
{
	/// <summary>
	/// Get orders
	/// </summary>
	public class GetOrders : LetroBase, IHttpHandler
	{
		/// <summary>Request Key order from</summary>
		public const string REQUEST_KEY_ORDER_FROM = "order_from";
		/// <summary>Request Key order to</summary>
		public const string REQUEST_KEY_ORDER_TO = "order_to";
		/// <summary>Request Key shipped from</summary>
		public const string REQUEST_KEY_ORDER_SHIPPED_FROM = "order_shipped_from";
		/// <summary>Request Key shipped to</summary>
		public const string REQUEST_KEY_ORDER_SHIPPED_TO = "order_shipped_to";

		/// <summary>
		/// プロセスリクエスト
		/// </summary>
		/// <param name="context">Context</param>
		public void ProcessRequest(HttpContext context)
		{
			this.CurrentContext = context;
			GetRequest();
			WriteResponse();
		}

		/// <summary>
		/// リクエスト取得
		/// </summary>
		/// <returns>リクエスト文字列</returns>
		protected override void GetRequest()
		{
			this.OrderFrom = this.CurrentContext.Request[REQUEST_KEY_ORDER_FROM];
			this.OrderTo = this.CurrentContext.Request[REQUEST_KEY_ORDER_TO];
			this.OrderShippedFrom = this.CurrentContext.Request[REQUEST_KEY_ORDER_SHIPPED_FROM];
			this.OrderShippedTo = this.CurrentContext.Request[REQUEST_KEY_ORDER_SHIPPED_TO];
		}

		/// <summary>
		/// Is valid authorization
		/// </summary>
		/// <returns>True if parameter is valid, otherwise false</returns>
		protected override bool IsValidParameters()
		{
			var requestDates = new string[]
			{
				this.OrderFrom,
				this.OrderTo,
				this.OrderShippedFrom,
				this.OrderShippedTo
			};

			foreach (var requestDate in requestDates)
			{
				if (string.IsNullOrEmpty(requestDate)) continue;
				if (Validator.IsDateExact(requestDate, Constants.DATE_FORMAT_SHORT)) continue;

				WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_API_DATE_FORMAT_ERROR, new string[] { requestDate });

				return false;
			}

			if (IsValidDateRange(this.OrderFrom, this.OrderTo) == false)
			{
				var replace = string.Join(" - ", new string[] { this.OrderFrom, this.OrderTo });
				WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_API_DATE_FORMAT_ERROR, new string[] { replace });

				return false;
			}

			if (IsValidDateRange(this.OrderShippedFrom, this.OrderShippedTo) == false)
			{
				var replace = string.Join(" - ", new string[] { this.OrderShippedFrom, this.OrderShippedTo });
				WriteErrorResponse(CommerceMessages.ERRMSG_LETRO_API_DATE_FORMAT_ERROR, new string[] { replace });

				return false;
			}

			return true;
		}

		/// <summary>
		/// Get response data
		/// </summary>
		/// <returns>A response object</returns>
		protected override object GetResponseData()
		{
			var orderTo = ConvertDateTime(this.OrderTo) != null ? ConvertDateTime(this.OrderTo).Value.AddDays(1) : (DateTime?)null;
			var orderShippedTo = ConvertDateTime(this.OrderShippedTo) != null ? ConvertDateTime(this.OrderShippedTo).Value.AddDays(1) : (DateTime?)null;
			var input = new Hashtable()
			{
				{ REQUEST_KEY_ORDER_FROM, ConvertDateTime(this.OrderFrom) },
				{ REQUEST_KEY_ORDER_TO, orderTo },
				{ REQUEST_KEY_ORDER_SHIPPED_FROM, ConvertDateTime(this.OrderShippedFrom) },
				{ REQUEST_KEY_ORDER_SHIPPED_TO, orderShippedTo },
			};
			var orders = DomainFacade.Instance.OrderService.GetOrdersForLetro(input);
			var orderDatas = orders
				.Select(data => new OrderDetail(data))
				.ToArray();
			foreach(var orderData in orderDatas)
			{
				var orderItems = DomainFacade.Instance.OrderService.GetOrderItemsForLetro(orderData.OrderId);
				orderData.OrderProducts = orderItems
					.Select(data => new OrderProduct(data, orderData.IsFixedPurchase))
					.ToArray();
			}

			var responseData = new LetroOrdersGetResponse
			{
				Orders = orderDatas
			};

			return responseData;
		}

		/// <summary>
		/// Convert date time
		/// </summary>
		/// <param name="dateString">Date string</param>
		/// <returns>Date time</returns>
		private DateTime? ConvertDateTime(string dateString)
		{
			DateTime tempDate;
			var isSuccessConvert = DateTime.TryParseExact(
				dateString,
				Constants.DATE_FORMAT_SHORT,
				null,
				DateTimeStyles.None,
				out tempDate);
			if (isSuccessConvert) return tempDate;

			return null;
		}

		/// <summary>
		/// Is valid date range
		/// </summary>
		/// <param name="dateFrom">Date from</param>
		/// <param name="dateTo">Date to</param>
		/// <returns>True if is valid date range, otherwise false</returns>
		private bool IsValidDateRange(string dateFrom, string dateTo)
		{
			if (string.IsNullOrEmpty(dateFrom)
				|| string.IsNullOrEmpty(dateTo))
			{
				return true;
			}
			var invalidDateRange = (Validator.CheckDateRange(
				ConvertDateTime(dateFrom),
				ConvertDateTime(dateTo)) == false);

			return invalidDateRange == false;
		}

		/// <summary>Order from</summary>
		public string OrderFrom { get; set; }
		/// <summary>Order to</summary>
		public string OrderTo { get; set; }
		/// <summary>Order shipped from</summary>
		public string OrderShippedFrom { get; set; }
		/// <summary>Order shipped to</summary>
		public string OrderShippedTo { get; set; }
		/// <summary>別の要求は使用できない</summary>
		public bool IsReusable { get { return false; } }
	}
}
