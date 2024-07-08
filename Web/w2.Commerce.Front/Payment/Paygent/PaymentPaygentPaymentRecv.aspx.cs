/*
=========================================================================================================
  Module      : Payment Paygent Payment Recv (PaymentPaygentPaymentRecv.aspx.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2024 All Rights Reserved.
=========================================================================================================
*/
using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using w2.App.Common.Input.Order;
using w2.App.Common.Order;
using w2.App.Common.Order.Payment.Paygent;
using w2.App.Common.Order.Payment.Paygent.DifferenceNotification.Dto;
using w2.App.Common.Order.Payment.Paygent.Logger;
using w2.Common.Logger;
using w2.Domain;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// Payment paygent payment recv
/// </summary>
public partial class Payment_Paygent_PaymentPaygentPaymentRecv : System.Web.UI.Page
{
	/// <summary>応答情報</summary>
	private const string CONST_RESPONSE_OK = "result=0";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			try
			{
				var request = RetrieveRequest();
				PaygentApiLogger.WriteDifferenceNotificationRequestLog(request);
				this.RequestString = JsonConvert.SerializeObject(
					request,
					new JsonSerializerSettings
					{
						Formatting = Formatting.Indented,
						NullValueHandling = NullValueHandling.Ignore
					});

				if (string.IsNullOrEmpty(request.PaymentId))
				{
					WriteResponse();
					return;
				}

				OrderModel order;
				switch (request.PaymentType)
				{
					case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_PAIDY:
						order = DomainFacade.Instance.OrderService.GetOrderByPaymentOrderId(request.PaidyPaymentId);
						if ((order == null)
							|| ((string.IsNullOrEmpty(order.CardTranId) == false)
								&& (order.CardTranId != request.PaymentId))
							|| (request.PaymentStatus == PaygentConstants.PAYGENT_API_RESPONSE_PAYMENT_STATUS_DELETED))
						{
							WriteResponse();
							return;
						}
						break;

					case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_BANKNET:
						order = DomainFacade.Instance.OrderService.Get(request.TradingId);
						if ((order == null)
							|| ((string.IsNullOrEmpty(order.CardTranId) == false)
								&& (order.CardTranId != request.PaymentId)))
						{
							WriteResponse();
							return;
						}
						break;

					case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_CONVENIENCE_STORE:
					case PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_ATM:
						order = DomainFacade.Instance.OrderService.GetOrderByCardTranId(request.PaymentId);
						if (order == null)
						{
							WriteResponse();
							return;
						}
						break;

					default:
						WriteResponse();
						return;
				}

				var data = string.Concat(
					request.PaymentNoticeId,
					request.PaymentId,
					request.TradingId,
					request.PaymentType,
					request.PaymentAmount,
					Constants.PAYMENT_PAYGENT_NOTICE_HASHKEY);
				var hashData = CreateHashSha256(data);
				if (hashData != request.Hc)
				{
					PaygentApiExternalPaymentCooperationLog.AppendExternalPaymentCheckerLog(
						false,
						order.OrderId,
						request.PaymentType,
						PaygentConstants.PAYGENT_API_HASHCODE_ERROR,
						Constants.FLG_LASTCHANGED_CGI,
						UpdateHistoryAction.Insert);
					WriteResponse();
					return;
				}

				DomainFacade.Instance.OrderService.Modify(
					order.OrderId,
					updateOrder =>
					{
						updateOrder.CardTranId = request.PaymentId;
						updateOrder.PaymentOrderId = request.PaymentType == PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_PAIDY
							? updateOrder.PaymentOrderId
							: request.PaymentNoticeId;
						updateOrder.LastChanged = Constants.FLG_LASTCHANGED_CGI;
						updateOrder.DateChanged = DateTime.Now;
						updateOrder.AppendPaymentMemo(OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
							updateOrder.PaymentOrderId,
							updateOrder.OrderPaymentKbn,
							updateOrder.CardTranId,
							ValueText.GetValueText(
								Constants.TABLE_ORDER,
								Constants.VALUETEXT_PARAM_PAYMENT_MEMO,
								Constants.VALUETEXT_PARAM_AUTH),
							updateOrder.LastBilledAmount));
					},
					UpdateHistoryAction.DoNotInsert);

				if ((request.PaymentType == PaygentConstants.FLG_PAYGENT_API_RESPONSE_PAYMENT_TYPE_PAIDY)
					&& (order.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP))
				{
					var orderStatus = Constants.FLG_ORDER_ORDER_STATUS_ORDERED;
					var statement = OrderCommon.GetUpdateOrderStatusStatement(orderStatus);
					var process = new ProcessAfterUpdateOrderStatus();
					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						accessor.BeginTransaction();
						var input = new Hashtable
						{
							{ Constants.FIELD_ORDER_ORDER_ID, order.OrderId },
							{ "update_date", DateTime.Now },
							{ Constants.FIELD_ORDER_USER_ID, order.UserId },
							{ Constants.FIELD_ORDER_LAST_CHANGED, Constants.FLG_LASTCHANGED_CGI }
						};
						var orderInput = new OrderInput(order);
						var errorMessage = process.UpdatedInvoiceByOrderStatus(
							orderInput,
							Constants.StatusType.Order,
							orderStatus,
							accessor,
							Constants.FLG_LASTCHANGED_CGI);

						var orderStatusUpdated = process.ModifyOrderStatus(statement, accessor, input);

						// ステータス更新による定期台帳処理
						process.UpdateFixedPurchaseByOrderStatus(
							orderStatusUpdated,
							Constants.StatusType.Order,
							orderStatus,
							orderInput,
							string.Empty,
							accessor);

						// ステータス更新による仮ポイント→本ポイントへ変更
						process.UpdateTempPointToRealPointByOrderStatus(
							orderInput,
							Constants.StatusType.Order,
							orderStatus,
							accessor,
							orderInput.UserId,
							orderInput.OrderId,
							Constants.FLG_LASTCHANGED_CGI);

						process.DeliverSerialKeyByUpdateStatus(
							true,
							orderStatusUpdated,
							orderInput.OrderId,
							orderInput.DigitalContentsFlg,
							orderStatus,
							orderInput.OrderPaymentStatus,
							Constants.FLG_LASTCHANGED_CGI,
							accessor);

						process.UpdateFixPurChaseMemberFlgByPaymentStatus(
							Constants.StatusType.Order,
							orderStatus,
							orderInput,
							Constants.FLG_LASTCHANGED_CGI,
							UpdateHistoryAction.DoNotInsert,
							accessor);
						accessor.CommitTransaction();
					}
				}

				PaygentUtility.UpdateOrderForPaygent(
					order.OrderId,
					request.PaymentStatus,
					request.PaymentType,
					request.PaymentInitDate,
					Constants.FLG_LASTCHANGED_CGI);

				DomainFacade.Instance.UpdateHistoryService.InsertForOrder(order.OrderId, Constants.FLG_LASTCHANGED_CGI);
				WriteResponse();
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}
	}

	/// <summary>
	/// Paygentからのリクエストデータ抽出
	/// </summary>
	/// <returns>リクエストデータ</returns>
	private PaygentDifferenceNotificationRequestDataset RetrieveRequest()
	{
		using (var reader = new StreamReader(this.Request.InputStream))
		{
			var content = reader.ReadToEnd();
			var values = HttpUtility.ParseQueryString(content);
			var responseResult = new PaygentDifferenceNotificationRequestDataset
			{
				PaymentNoticeId = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_NOTICE_ID]),
				ChangeDate = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_CHANGE_DATE]),
				PaymentId = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_ID]),
				TradingId = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_TRADING_ID]),
				PaymentType = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_TYPE]),
				SiteId = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_SITE_ID]),
				PaymentStatus = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_STATUS]),
				PaymentAmount = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_AMOUNT]),
				PaidyPaymentId = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_PAIDY_PAYMENT_ID]),
				PaymentInitDate = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_INIT_DATE]),
				AuthorizedDate = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_AUTHORIZED_DATE]),
				CancelDate = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_CANCEL_DATE]),
				PaymentDate = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_PAYMENT_DATE]),
				Event = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_EVENT]),
				EventResult = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_EVENT_RESULT]),
				Result = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_RESPONSE_RESULT]),
				Hc = StringUtility.ToEmpty(values[PaygentConstants.PAYGENT_API_REQUEST_HC]),
			};
			return responseResult;
		}
	}

	/// <summary>
	/// SHA-256でハッシュ化する
	/// </summary>
	/// <param name="data">データ</param>
	/// <returns>ハッシュ化されたデータ</returns>
	private string CreateHashSha256(string data)
	{
		var keyBytesForHash = Encoding.UTF8.GetBytes(data.Trim());
		using (var sha256 = new SHA256CryptoServiceProvider())
		{
			var hashBytes = sha256.ComputeHash(keyBytesForHash);
			var result = string.Join(string.Empty, hashBytes.Select(item => item.ToString("x2")));
			return result;
		}
	}

	/// <summary>
	/// Paygentへのレスポンス生成
	/// </summary>
	private void WriteResponse()
	{
		PaygentApiLogger.WriteDifferenceNotificationResponseLog(this.RequestString, CONST_RESPONSE_OK);
		this.Response.StatusCode = HttpStatusCode.OK.GetHashCode();
		this.Response.Write(CONST_RESPONSE_OK);
	}

	/// <summary>Request string</summary>
	private string RequestString { get; set; }
}
