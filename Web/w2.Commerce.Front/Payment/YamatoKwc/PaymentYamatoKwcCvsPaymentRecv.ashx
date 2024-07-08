<%@ WebHandler Language="C#" Class="YamatoKwcCvsPaymentRecv" %>

using System;
using System.Collections.Generic;
using System.Web;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.YamatoKwc;

public class YamatoKwcCvsPaymentRecv : IHttpHandler
{

	/// <summary>
	/// リクエスト処理
	/// </summary>
	/// <param name="context"></param>
	public void ProcessRequest(HttpContext context)
	{
		context.Response.ContentType = "text/plain";

		if (string.IsNullOrEmpty(context.Request["trader_code"])) return;

		// POSTデータ取得
		var receiver = new PaymentYamatoKwcCvsResultReceiver(context.Request);
		if (receiver.Settled == false) return;

		// 注文情報取得
		var order = OrderCommon.GetOrder(receiver.OrderNo);
		if (order.Count == 0)
		{
			// ログファイル格納処理
			PaymentFileLogger.WritePaymentLog(
				false,
				"",
				PaymentFileLogger.PaymentType.YamatoKwcCvsPaymentRecv,
				PaymentFileLogger.PaymentProcessingType.Receive,
				string.Format("payment_detail：{0}", receiver.SettleDetail),
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, receiver.OrderNo}
				});
			return;
		}
		else
		{
			PaymentFileLogger.WritePaymentLog(
				true,
				"",
				PaymentFileLogger.PaymentType.YamatoKwcCvsPaymentRecv,
				PaymentFileLogger.PaymentProcessingType.Receive,
				string.Format("payment_detail：{0}", receiver.SettleDetail),
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, receiver.OrderNo}
				});
		}

		// ステータス確定
		var orderPriceTotal = (decimal)order[0][Constants.FIELD_ORDER_ORDER_PRICE_TOTAL];
		var paymentStatus = Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE;
		if (receiver.SettlePrice < orderPriceTotal)
		{
			// ログファイル格納処理
			PaymentFileLogger.WritePaymentLog(
				null,
				"",
				PaymentFileLogger.PaymentType.YamatoKwcCvsPaymentRecv,
				PaymentFileLogger.PaymentProcessingType.Receive,
				string.Format(
					"一部入金:OrderNo={0} {1}<{2}",
					receiver.SettleDetail,
					receiver.SettlePrice,
					orderPriceTotal),
				new Dictionary<string, string>
				{
					{Constants.FIELD_ORDER_ORDER_ID, receiver.OrderNo}
				});
			return;
		}

		// 入金ステータス更新
		new OrderService().UpdatePaymentStatusForCvs(
			receiver.OrderNo,
			paymentStatus,
			receiver.SettleDate,
			(string)order[0][Constants.FIELD_ORDER_CARD_TRAN_ID],
			Constants.FLG_LASTCHANGED_CGI,
			UpdateHistoryAction.Insert);
	}

	public bool IsReusable
	{
		get { return true; }
	}

}