<%--
=========================================================================================================
  Module      : Receive(Receive.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="Receive" %>
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Web;
using w2.App.Common;
using w2.Common.Logger;
using w2.Domain.Order;

public class Receive : IHttpHandler
{
	/// <summary>
	/// Process Request
	/// </summary>
	/// <param name="context">Context</param>
	public void ProcessRequest(HttpContext context)
	{
		try
		{
			var json = string.Empty;
			using (var reader = new StreamReader(context.Request.GetBufferedInputStream()))
			{
				json = reader.ReadToEnd();
				FileLogger.Write("AfteeApi", "Json Input: " + json);
			}

			// Check Method
			if (context.Request.HttpMethod != "POST")
			{
				context.Response.StatusCode = 405;
				FileLogger.Write("AfteeApi", "Method Not Allowed");
				return;
			}
			
			// Check Header
			var header = context.Request.Headers["Aftee-Confirmation-Checksum"];
			if (string.IsNullOrEmpty(header))
			{
				context.Response.StatusCode = 404;
				FileLogger.Write("AfteeApi", "No Header");
				return;
			}

			var body = JsonConvert.DeserializeObject<RequestBody>(json);
			if (body != null)
			{
				if (string.IsNullOrEmpty(body.CardTranId)
					|| string.IsNullOrEmpty(body.Amount))
				{
					FileLogger.Write("AfteeApi", "Object Id Invalid");
					return;
				}

				var order = new OrderService().GetOrderByCardTranId(body.CardTranId);
				var amount = decimal.Parse(body.Amount);
				if ((order != null)
					&& (order.SettlementAmount == amount))
				{
					FileLogger.Write("AfteeApi", "Object Id Valid");
					return;
				}

				SendMailToOperator(order, body.CardTranId);
				FileLogger.Write("AfteeApi", "OK");
				context.Response.StatusCode = 200;
			}
			else
			{
				FileLogger.Write("AfteeApi", "No Body");
				context.Response.StatusCode = 400;
			}
		}
		catch (Exception exception)
		{
			FileLogger.WriteError(exception);
			context.Response.StatusCode = 400;
		}
	}

	/// <summary>
	/// Send Mail To Operator
	/// </summary>
	/// <param name="order">Order</param>
	/// <param name="cardTranId">Card Tran Id</param>
	private static void SendMailToOperator(OrderModel order, string cardTranId)
	{
		var body = new StringBuilder();
		body.AppendLine("Aftee決済の不整合性が発生しました。");
		var message = ((order == null)
			? string.Format("取引オブジェクトID：{0}の注文データがありません。", cardTranId)
			: string.Format("取引オブジェクトID：{0}の注文ID:{1}、決済金額が合わないです。", cardTranId, order.OrderId));

		body.AppendLine(message);
		var mailSender = new MailSendUtility(
			Constants.CONST_DEFAULT_SHOP_ID,
			Constants.CONST_MAIL_ID_NP_PAYMENT_SYSTEM_NOTIFICATION_FOR_OPERATOR,
			string.Empty,
			new Hashtable(),
			true,
			Constants.MailSendMethod.Auto);
		mailSender.SetBody(body.ToString());
		if (mailSender.SendMail() == false)
		{
			FileLogger.WriteError(body.ToString(), mailSender.MailSendException);
		}
	}

	/// <summary>
	/// Is Reusable
	/// </summary>
	public bool IsReusable
	{
		get
		{
			return false;
		}
	}

	/// <summary>
	/// Request Body
	/// </summary>
	public class RequestBody
	{
		/// <summary>Card Tran Id</summary>
		[JsonProperty("id")]
		public string CardTranId { get; set; }
		/// <summary>User Id</summary>
		[JsonProperty("user_no")]
		public string UserId { get; set; }
		/// <summary>Amount</summary>
		[JsonProperty("amount")]
		public string Amount { get; set; }
	}
}