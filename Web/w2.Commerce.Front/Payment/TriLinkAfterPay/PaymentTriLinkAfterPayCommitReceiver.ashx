<%--
=========================================================================================================
  Module      : 後付款(TriLink後払い) 注文確定通知取得ハンドラ(PaymentTriLinkAfterPayCommitReceiver.ashx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="PaymentTriLinkAfterPayCommitReceiver" %>

using System.Web;
using System.IO;
using Newtonsoft.Json;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Order.Payment.TriLinkAfterPay.Receiver;

/// <summary>
/// 後付款(TriLink後払い) 注文確定通知取得ハンドラ
/// </summary>
public class PaymentTriLinkAfterPayCommitReceiver : IHttpHandler
{
	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context">HTTPコンテキスト</param>
	public void ProcessRequest(HttpContext context)
	{
		// リトライは30秒間隔で3回まで
		// 正常に処理ができた場合は「200」でレスポンスを返す
		context.Response.ContentType = "text/plain";

		// POST情報取得
		StreamReader streamReader = new StreamReader(context.Request.GetBufferedInputStream());
		string requestData = streamReader.ReadToEnd();

		var receiver = JsonConvert.DeserializeObject<TriLinkAfterPayCommitReceiver>(requestData);
		if (receiver == null)
		{
			PaymentFileLogger.WritePaymentLog(
				false,
				Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY,
				PaymentFileLogger.PaymentType.TriLink,
				PaymentFileLogger.PaymentProcessingType.GetOrderConfirmationNotice,
				WebMessages.GetMessages(
					WebMessages.ERRMSG_FRONT_IRREGULAR_PARAMETER_ERROR));
			return;
		}
		else
		{
			PaymentFileLogger.WritePaymentLog(
				true,
				Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY,
				PaymentFileLogger.PaymentType.TriLink,
				PaymentFileLogger.PaymentProcessingType.GetOrderConfirmationNotice,
				string.Format(
					"inquiry_number：{0},merchant_order_number：{1}",
					receiver.OrderCode,
					receiver.StoreOrderCode)); //inquiry_number:問い合わせ番号,merchant_order_number:加盟店注文番号
		}

		var paymentOrderId = receiver.StoreOrderCode;
		var cardTranId = receiver.OrderCode;

		// 決済取引ID更新
		using (var accessor = new SqlAccessor())
		{
			accessor.OpenConnection();
			var order = new OrderService().GetOrderByPaymentOrderId(paymentOrderId, accessor);
			new OrderService().UpdateCardTranId(order.OrderId, cardTranId, "receiver", UpdateHistoryAction.Insert, accessor);
			new OrderService().AddPaymentMemo(
				order.OrderId,
				OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
					paymentOrderId,
					Constants.FLG_PAYMENT_PAYMENT_ID_TRILINK_AFTERPAY,
					cardTranId,
					"決済注文ID取得",
					order.LastBilledAmount),
				Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert,
				accessor);
		}

		context.Response.StatusCode = 200;
	}

	/// <summary>再利用可能か</summary>
	public bool IsReusable
	{
		get { return false; }
	}
}