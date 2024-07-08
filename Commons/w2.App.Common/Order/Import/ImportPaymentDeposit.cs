/*
=========================================================================================================
  Module      : Import order payment deposit module(ImportPaymentDeposit.cs)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Import
{
	/// <summary>
	/// Import payment deposit
	/// </summary>
	public class ImportPaymentDeposit : ImportBase
	{
		/// <summary>Xml Setting</summary>
		private static ImportSetting m_xmlSetting;

		/// <summary>
		/// Import Payment Deposit
		/// </summary>
		/// <param name="orderFile">Order File</param>
		public ImportPaymentDeposit(string orderFile)
		{
			GetImportSetting(orderFile);
		}

		/// <summary>
		/// Get Import Setting
		/// </summary>
		/// <param name="orderFile">Order File</param>
		public void GetImportSetting(string orderFile)
		{
			m_xmlSetting = new ImportSetting();

			try
			{
				// Load configuration file
				var mainElement = XElement.Load(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + Constants.FILE_XML_ORDERFILEIMPORT_SETTING);
				var serviceNode =
					from serviceNodes in mainElement.Elements("OrderFile")
					where (serviceNodes.Element("Value").Value == orderFile)
					select new
					{
						importFileSetting = serviceNodes.Elements("ImportFileSetting")
							.ToDictionary(node => node.Attribute("key").Value, node => node.Attribute("value").Value)
					};

				// Import file basic settings
				var importFileSettings = serviceNode.First().importFileSetting;
				m_xmlSetting.ExtendStatusDoubleDepositError = importFileSettings["ExtendStatusDoubleDepositError"];
				m_xmlSetting.ExtendStatusDskDefPaymentConfirmed = importFileSettings["ExtendStatusDskDefPaymentConfirmed"];
				m_xmlSetting.ExtendStatusPostalTransfered = importFileSettings["ExtendStatusPostalTransfered"];
				m_xmlSetting.ExtendStatusShippedPending = importFileSettings["ExtendStatusShippedPending"];
				m_xmlSetting.PaymentCvsDefDskAllowPaymentPreliminaryType = importFileSettings["PaymentCvsDefDskAllowPaymentPreliminaryType"];
				m_xmlSetting.PaymentCvsDefDskAllowPaymentConfirmedType = importFileSettings["PaymentCvsDefDskAllowPaymentConfirmedType"];
				m_xmlSetting.PaymentCvsDefDskAllowPaymentCancelType = importFileSettings["PaymentCvsDefDskAllowPaymentCancelType"];
				m_xmlSetting.PaymentCvsDefDskAllowPaymentPostalTransferType = importFileSettings["PaymentCvsDefDskAllowPaymentPostalTransferType"];
				m_xmlSetting.PaymentCvsDefDskAllowPaymentPostalTransferCorrectedType = importFileSettings["PaymentCvsDefDskAllowPaymentPostalTransferCorrectedType"];
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// Import handle
		/// </summary>
		/// <param name="streamReader">CSV file Stream reader</param>
		/// <param name="loginOperatorName">Login operator name</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <returns>Import status</returns>
		public override bool Import(
			StreamReader streamReader,
			string loginOperatorName,
			UpdateHistoryAction updateHistoryAction)
		{
			var orderService = new OrderService();
			var fixedpurchaseService = new FixedPurchaseService();
			var errorMessage = new StringBuilder();

			// Current line
			var currentLine = 0;

			streamReader.ReadLine();

			// Read data
			while (streamReader.EndOfStream == false)
			{
				currentLine++;

				// Get data
				var buffer = StringUtility.SplitCsvLine(streamReader.ReadLine());

				// Check allow payment deposit type
				if ((m_xmlSetting.PaymentCvsDefDskAllowPaymentPreliminaryType != buffer[0])
					&& (m_xmlSetting.PaymentCvsDefDskAllowPaymentConfirmedType != buffer[0])
					&& (m_xmlSetting.PaymentCvsDefDskAllowPaymentCancelType != buffer[0])
					&& (m_xmlSetting.PaymentCvsDefDskAllowPaymentPostalTransferType != buffer[0])
					&& (m_xmlSetting.PaymentCvsDefDskAllowPaymentPostalTransferCorrectedType != buffer[0])) continue;

				// Get order info
				var paymentOrderId = string.Format("{0}{1}", buffer[4], buffer[5]);
				paymentOrderId = (paymentOrderId.Length > 10)
					? paymentOrderId.Substring(paymentOrderId.Length - 10, 10)
					: paymentOrderId;
				var order = orderService.GetOrderByPaymentOrderId(paymentOrderId);

				var messageExtendOrder = string.Empty;
				if (order == null)
				{
					// Not exists order
					errorMessage.AppendLine(string.Format(
						ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_PAYMENT_ORDER_ID_NOT_EXISTS),
						paymentOrderId,
						currentLine));
				}
				else if ((CheckExtendStatus(m_xmlSetting.ExtendStatusShippedPending, currentLine, order.OrderId, out messageExtendOrder) == false)
					|| (CheckExtendStatus(m_xmlSetting.ExtendStatusDoubleDepositError, currentLine, order.OrderId, out messageExtendOrder) == false)
					|| (CheckExtendStatus(m_xmlSetting.ExtendStatusDskDefPaymentConfirmed, currentLine, order.OrderId, out messageExtendOrder) == false)
					|| (CheckExtendStatus(m_xmlSetting.ExtendStatusPostalTransfered, currentLine, order.OrderId, out messageExtendOrder) == false))
				{
					errorMessage.AppendLine(messageExtendOrder);
				}
				else
				{
					var orderManagementMemo = string.Format("{0}[{1}]",
						(string.IsNullOrEmpty(order.ManagementMemo)
							? string.Empty
							: Environment.NewLine),
						DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

					var isOrderPaymentStatusComplete = (order.OrderPaymentStatus == Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE);

					// 速報キャンセル
					if (m_xmlSetting.PaymentCvsDefDskAllowPaymentCancelType == buffer[0])
					{
						var message = string.Format(
							ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_CVS_DEF_DSK_PRELIMINARY_PAYMENT_CANCEL),
							order.OrderId,
							currentLine);
						order.ManagementMemo += string.Format("{0} {1}", orderManagementMemo, message);
						orderService.UpdateOrderCvsDefDskPreliminaryPaymentCancel(
							order,
							int.Parse(m_xmlSetting.ExtendStatusDskDefPaymentConfirmed),
							loginOperatorName,
							updateHistoryAction);
						m_iUpdatedCount++;
						continue;
					}

					// 速報データ二重取込(スキップ)
					if (isOrderPaymentStatusComplete
						&& (m_xmlSetting.PaymentCvsDefDskAllowPaymentPreliminaryType == buffer[0])) continue;

					// 二重入金(確定・郵便振替のみ判定)
					if (isOrderPaymentStatusComplete
						&& ((m_xmlSetting.PaymentCvsDefDskAllowPaymentConfirmedType == buffer[0])
							|| (m_xmlSetting.PaymentCvsDefDskAllowPaymentPostalTransferType.ToString() == buffer[0])
							|| (m_xmlSetting.PaymentCvsDefDskAllowPaymentPostalTransferCorrectedType.ToString() == buffer[0]))
						&& (order.ExtendStatus[int.Parse(m_xmlSetting.ExtendStatusDskDefPaymentConfirmed) - 1].Value
							== Constants.FLG_ORDER_EXTEND_STATUS_ON))
					{
						var message = string.Format(
							ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_DOUBLE_DEPOSIT_ERROR),
							order.OrderId,
							buffer[7],
							currentLine);
						errorMessage.AppendLine(message);
						order.ManagementMemo += string.Format("{0} {1}", orderManagementMemo, message);
						orderService.UpdateDoubleDepositError(
							order,
							int.Parse(m_xmlSetting.ExtendStatusDoubleDepositError),
							loginOperatorName,
							updateHistoryAction);
						m_iUpdatedCount++;
						continue;
					}

					// 入金額不一致
					if (order.OrderPriceTotal != Convert.ToDecimal(buffer[7]))
					{
						var message = string.Format(
							ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_AMOUNT_AND_DEPOSIT_AMOUNT_NOT_MATCH),
							order.OrderId,
							Convert.ToInt32(buffer[7]),
							Convert.ToInt32(order.OrderPriceTotal),
							currentLine);
						errorMessage.AppendLine(message);
						order.ManagementMemo += string.Format("{0} {1}", orderManagementMemo, message);
						orderService.UpdateDepositAmountNotMatchError(order, loginOperatorName, updateHistoryAction);
						continue;
					}

					// 入金確定
					if (isOrderPaymentStatusComplete
						&& (m_xmlSetting.PaymentCvsDefDskAllowPaymentConfirmedType == buffer[0]))
					{
						var message = string.Format(
							ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_CVS_DEF_DSK_PAYMENT_CONFIRMED),
							order.OrderId,
							currentLine);
						order.ManagementMemo += string.Format("{0} {1}", orderManagementMemo, message);
						orderService.UpdateOrderCvsDefDskPaymentConfirmed(
							order,
							int.Parse(m_xmlSetting.ExtendStatusDskDefPaymentConfirmed),
							loginOperatorName,
							updateHistoryAction);
						m_iUpdatedCount++;
						continue;
					}

					// 速報時
					var updated = 0;
					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						var orderPaymentDate = DateTime.ParseExact(buffer[1], "yyyyMMdd", CultureInfo.InvariantCulture);
						updated = orderService.UpdatePaymentStatus(
							order.OrderId,
							Constants.FLG_ORDER_ORDER_PAYMENT_STATUS_COMPLETE,
							orderPaymentDate,
							loginOperatorName,
							updateHistoryAction,
							accessor);
					}

					if (updated > 0)
					{
						// Success count
						m_iUpdatedCount++;

						// 郵便振替or郵便振替金額訂正の場合はフラグON
						if ((m_xmlSetting.PaymentCvsDefDskAllowPaymentPostalTransferType.ToString() == buffer[0])
							|| (m_xmlSetting.PaymentCvsDefDskAllowPaymentPostalTransferCorrectedType.ToString() == buffer[0]))
						{
							var message = string.Format(
								ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_CVS_DEF_DSK_PAYMENT_CONFIRMED),
								order.OrderId,
								currentLine);
							order.ManagementMemo += string.Format("{0} {1}", orderManagementMemo, message);
							orderService.UpdateOrderExtendStatus(
								order.OrderId,
								int.Parse(m_xmlSetting.ExtendStatusDskDefPaymentConfirmed),
								Constants.FLG_ORDER_EXTEND_STATUS_ON,
								DateTime.Now,
								loginOperatorName,
								updateHistoryAction);
							orderService.UpdateOrderCvsDefDskPaymentConfirmed(
								order,
								int.Parse(m_xmlSetting.ExtendStatusPostalTransfered),
								loginOperatorName,
								updateHistoryAction);
						}

						if (fixedpurchaseService.GetCountOrderFixedPurchase(order.OrderId) > 0)
						{
							// Check fixedpurchase id
							if (order.IsFixedPurchaseOrder)
							{
								try
								{
									// Get fixed purchase
									var fixedpurchase = fixedpurchaseService.Get(order.FixedPurchaseId);

									// Update fixedpurchase extend status
									fixedpurchaseService.UpdateExtendStatus(
										fixedpurchase.FixedPurchaseId,
										int.Parse(m_xmlSetting.ExtendStatusShippedPending),
										Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_OFF,
										loginOperatorName,
										updateHistoryAction);
								}
								catch (Exception)
								{
									// Not exist fixed purchase
									errorMessage.AppendLine(string.Format(
										ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_PAYMENT_ORDER_FIXED_PURCHASE_NOT_EXISTS),
										order.PaymentOrderId,
										order.FixedPurchaseId,
										currentLine));
									m_iUpdatedCount--;
									continue;
								}
							}
							else
							{
								errorMessage.AppendLine(string.Format(
									ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_FIXED_PURCHASE_ID_NOT_EXISTS),
									order.PaymentOrderId,
									currentLine));
								m_iUpdatedCount--;
							}
						}
					}
				}
			}

			if (errorMessage.Length != 0)
			{
				// Write log and show error
				AppLogger.WriteError(errorMessage.ToString());
				m_strErrorMessage = errorMessage.ToString().Replace(Environment.NewLine, "<br/>");
			}

			var importSuccessMessage = string.Format("{0}{1}{2}",
				ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_IMPORT_SUCCESS)
					.Replace("@@ 1 @@", StringUtility.ToNumeric(m_iUpdatedCount))
					.Replace("@@ 2 @@", StringUtility.ToNumeric(currentLine)),
				Environment.NewLine,
				errorMessage);
			// 処理結果を表示
			AppLogger.WriteError(importSuccessMessage);
			m_strErrorMessage = errorMessage.ToString().Replace(Environment.NewLine, "<br/>");

			// Total line count
			m_iLinesCount = currentLine;
			return (m_iLinesCount > 0);
		}

		/// <summary>
		/// Check extend status
		/// </summary>
		/// <param name="extendStatus">Extend status</param>
		/// <param name="currentLine">Current line</param>
		/// <param name="orderId">Order id</param>
		/// <param name="message">Message</param>
		/// <returns>True if extend status not null, false with error message if extend status null</returns>
		public static bool CheckExtendStatus(
			string extendStatus,
			int currentLine,
			string orderId,
			out string message)
		{
			message = string.Empty;
			if (string.IsNullOrEmpty(extendStatus))
			{
				message = string.Format(
					ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_UPDATE_ORDER_EXTENDSTATUS_ERROR),
					currentLine,
					orderId,
					extendStatus);
				return false;
			}
			return true;
		}

		/// <summary>
		/// Import Setting
		/// </summary>
		private class ImportSetting
		{
			/// <summary>注文拡張ステータス：出荷済み保留</summary>
			public string ExtendStatusShippedPending { get; set; }
			/// <summary>注文拡張ステータス：二重入金発生</summary>
			public string ExtendStatusDoubleDepositError { get; set; }
			/// <summary>注文拡張ステータス：電算コンビニ後払い売上確定</summary>
			public string ExtendStatusDskDefPaymentConfirmed { get; set; }
			/// <summary>注文拡張ステータス：郵便振替入金</summary>
			public string ExtendStatusPostalTransfered { get; set; }
			/// <summary>決済：電算コンビニ後払い決済：入金データ取込対象種別ID(速報)</summary>
			public string PaymentCvsDefDskAllowPaymentPreliminaryType { get; set; }
			/// <summary>決済：電算コンビニ後払い決済：入金データ取込対象種別ID(確定)</summary>
			public string PaymentCvsDefDskAllowPaymentConfirmedType { get; set; }
			/// <summary>決済：電算コンビニ後払い決済：入金データ取込対象種別ID(速報キャンセル)</summary>
			public string PaymentCvsDefDskAllowPaymentCancelType { get; set; }
			/// <summary>決済：電算コンビニ後払い決済：入金データ取込対象種別ID(郵便振替)</summary>
			public string PaymentCvsDefDskAllowPaymentPostalTransferType { get; set; }
			/// <summary>決済：電算コンビニ後払い決済：入金データ取込対象種別ID(郵便振替金額訂正)</summary>
			public string PaymentCvsDefDskAllowPaymentPostalTransferCorrectedType { get; set; }
		}
	}
}
