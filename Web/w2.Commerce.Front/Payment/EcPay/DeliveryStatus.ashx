<%--
=========================================================================================================
  Module      : DeliveryStatus(DeliveryStatus.ashx)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2020 All Rights Reserved.
=========================================================================================================
--%>
<%@ WebHandler Language="C#" Class="DeliveryStatus" %>
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using w2.App.Common.Order.Payment;
using w2.App.Common.Order.Payment.ECPay;
using w2.App.Common.Pdf;
using w2.Common.Logger;
using w2.Domain.Order;
using w2.Common.Util;
using w2.Common.Sql;
using w2.Domain.UpdateHistory.Helper;
using System.Text;

/// <summary>
/// Delivery Status
/// </summary>
public class DeliveryStatus : IHttpHandler
{
	/// <summary>Message Update Success</summary>
	private const string MESSAGE_UPDATE_SUCCESS = "1|OK";

	/// <summary>
	/// プロセスリクエスト
	/// </summary>
	/// <param name="context">コンテキスト</param>
	public void ProcessRequest(HttpContext context)
	{
		this.Parameters = context.Request.Form.AllKeys.Any()
			? context.Request.Form
			: HttpUtility.ParseQueryString(context.Request.Url.Query);
		this.OrderId = context.Request[ECPayConstants.PARAM_ORDER_ID];
		var isCVSChangeNotification = (context.Request["no"] == "2");

		try
		{
			var orderService = new OrderService();
			var order = orderService.GetOrderInfoByOrderId(this.OrderId);
			if (isCVSChangeNotification)
			{
				var message = new StringBuilder();
				message.Append("注文ID:").Append(order.OrderId).Append("\r\n");
				message.Append("物流取引ID:").Append(this.AllPayLogisticsId).Append("\r\n");
				message.Append("コンビニ種別:").Append(this.StoreType).Append("\r\n");
				message.Append("状態:").Append(GetNumberAndContentOfStatus());

				// Send mail error for manager
				ECPayUtility.SendMailError(message.ToString().Trim());

				var dictionaryWriteLog = new Dictionary<string, string>
				{
					{ "注文ID", order.OrderId },
					{ "物流取引ID", this.AllPayLogisticsId },
					{ "コンビニ種別", this.StoreType },
					{ "状態", GetNumberAndContentOfStatus() }
				};

				PaymentFileLogger.WritePaymentLog(
					true,
					order.PaymentName ?? string.Empty,
					PaymentFileLogger.PaymentType.Unknown,
					PaymentFileLogger.PaymentProcessingType.OrderInfoUpdate,
					string.Empty,
					dictionaryWriteLog);
			}
			else
			{
				// Create Url Parameters And Check Sum
				var urlParameters = CreateUrlParameters();
				var checkMacValue = ECPayUtility.CreateCheckSumValue(urlParameters);
				if ((checkMacValue != this.CheckMacValue) || string.IsNullOrEmpty(this.OrderId)) return;
				if ((order == null) || (order.GiftFlg == Constants.FLG_ORDER_GIFT_FLG_ON)) return;

				// Set data for update
				order.DeliveryTranId = order.IsReturnOrder
					? this.RtnMerchantTradeNo
					: this.AllPayLogisticsId;
				order.RelationMemo = ECPayUtility.CreateRelationMemo(this.RtnCode, this.RtnMsg, string.Empty);

				var logisticsSubType = ECPayUtility.GetLogisticsSubType(order);
				var orderShipping = order.Shippings[0];
				orderShipping.ShippingExternalDelivertyStatus = this.RtnCode;
				orderShipping.ShippingStatus = ECPayUtility.ConvertShippingStatus(logisticsSubType, this.RtnCode);
				orderShipping.ShippingStatusUpdateDate = (string.IsNullOrEmpty(this.UpdateStatusDate) == false)
					? DateTime.Parse(this.UpdateStatusDate)
					: (DateTime?)null;
				orderShipping.ShippingCheckNo = (string.IsNullOrEmpty(this.CvsPaymentNo) == false)
					? this.CvsPaymentNo
					: string.Empty;

				using (var accessor = new SqlAccessor())
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					orderService.UpdateDeliveryTransactionIdAndRelationMemo(
						order.OrderId,
						order.DeliveryTranId,
						order.RelationMemo,
						Constants.FLG_LASTCHANGED_USER,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					orderService.UpdateOrderShipping(orderShipping, accessor);
					accessor.CommitTransaction();
				}

				var dictionaryWriteLog = new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId },
					{ ECPayConstants.PARAM_RTN_CODE, this.RtnCode },
					{ ECPayConstants.PARAM_RTN_MSG, this.RtnMsg }
				};

				if (order.IsReturnOrder)
				{
					dictionaryWriteLog.Add(ECPayConstants.PARAM_RTN_MERCHANT_TRADE_NO, this.RtnMerchantTradeNo);
				}
				else
				{
					dictionaryWriteLog.Add(ECPayConstants.PARAM_ALL_PAY_LOGISTICS_ID, this.AllPayLogisticsId);
				}

				PaymentFileLogger.WritePaymentLog(
					true,
					order.PaymentName ?? string.Empty,
					PaymentFileLogger.PaymentType.Unknown,
					PaymentFileLogger.PaymentProcessingType.OrderInfoUpdate,
					string.Empty,
					dictionaryWriteLog);
			}

			context.Response.Write(MESSAGE_UPDATE_SUCCESS);
			context.Response.End();
		}
		catch (Exception exception)
		{
			PaymentFileLogger.WritePaymentLog(
				false,
				string.Empty,
				PaymentFileLogger.PaymentType.Unknown,
				PaymentFileLogger.PaymentProcessingType.OrderInfoUpdate,
				BaseLogger.CreateExceptionMessage(exception),
				new Dictionary<string, string>
				{
					{ Constants.FIELD_ORDER_ORDER_ID, this.OrderId }
				});
		}
	}

	/// <summary>
	/// Create Url Parameters
	/// </summary>
	/// <returns>Url Parameters</returns>
	private string CreateUrlParameters()
	{
		var parameters = this.Parameters.AllKeys.ToDictionary(item => item, item => this.Parameters[item]);
		parameters.Remove(ECPayConstants.PARAM_CHECK_MAC_VALUE);
		var urlParameters = string.Join(
			"&",
			parameters
				.OrderBy(item => item.Key)
				.Select(item => string.Format("{0}={1}", item.Key, item.Value))
				.ToArray());
		return urlParameters;
	}

	/// <summary>
	/// Get Number And Content Of Status
	/// </summary>
	/// <returns>Number And Content Of Status</returns>
	private string GetNumberAndContentOfStatus()
	{
		var numberAndContent = string.Empty;
		switch (this.Status)
		{
			case "01":
				numberAndContent = "コンビニ閉店";
				break;

			case "02":
				numberAndContent = "コンビニ番号変更";
				break;

			case "03":
				numberAndContent = "返品コンビニが元の商品受取コンビニ、但し元の商品受取コンビニが存在しない";
				break;

			case "04":
				numberAndContent = "受取(返品)コンビニ臨時閉店";
				break;
		}
		return numberAndContent;
	}

	#region Properties
	/// <summary>Is Reusable</summary>
	public bool IsReusable
	{
		get { return false; }
	}
	/// <summary>Order Id</summary>
	private string OrderId { get; set; }
	/// <summary>Return Code</summary>
	private string RtnCode
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_RTN_CODE]); }
	}
	/// <summary>Return Message</summary>
	private string RtnMsg
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_RTN_MSG]); }
	}
	/// <summary>Return Merchant Trade No</summary>
	private string RtnMerchantTradeNo
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_RTN_MERCHANT_TRADE_NO]); }
	}
	/// <summary>All Pay Logistics ID</summary>
	private string AllPayLogisticsId
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_ALL_PAY_LOGISTICS_ID]); }
	}
	/// <summary>Cvs Payment No</summary>
	private string CvsPaymentNo
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_CVS_PAYMENT_NO]); }
	}
	/// <summary>Update Status Date</summary>
	private string UpdateStatusDate
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_UPDATE_STATUS_DATE]); }
	}
	/// <summary>Check Mac Value</summary>
	private string CheckMacValue
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_CHECK_MAC_VALUE]); }
	}
	/// <summary>Merchant ID</summary>
	private string MerchantId
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_MERCHANT_ID]); }
	}
	/// <summary>Goods Name</summary>
	private string GoodsName
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_GOODS_NAME]); }
	}
	/// <summary>Goods Amount</summary>
	private string GoodsAmount
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_GOODS_AMOUNT]); }
	}
	/// <summary>Store Type</summary>
	private string StoreType
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_CVS_STORE_TYPE]); }
	}
	/// <summary>Status</summary>
	private string Status
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_CVS_STATUS]); }
	}
	/// <summary>Store ID</summary>
	private string StoreId
	{
		get { return StringUtility.ToEmpty(this.Parameters[ECPayConstants.PARAM_CVS_STORE_ID]); }
	}
	/// <summary>Parameters</summary>
	private NameValueCollection Parameters { get; set; }
	#endregion
}