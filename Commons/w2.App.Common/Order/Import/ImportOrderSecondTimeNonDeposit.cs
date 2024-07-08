/*
=========================================================================================================
  Module      : Import order second time non deposit module(ImportOrderSecondTimeNonDeposit.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Import
{
	/// <summary>
	/// Import order second time non deposit
	/// </summary>
	public class ImportOrderSecondTimeNonDeposit : ImportBase
	{
		/// <summary>Xml Setting</summary>
		private static ImportSetting m_xmlSetting;

		/// <summary>
		/// Import Order Second Time Non Deposit
		/// </summary>
		/// <param name="orderFile">Order File</param>
		public ImportOrderSecondTimeNonDeposit(string orderFile)
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
				m_xmlSetting.ExtendStatusShippedPending = importFileSettings["ExtendStatusShippedPending"];
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
		/// <returns>True: import success otherwise false</returns>
		public override bool Import(
			StreamReader streamReader,
			string loginOperatorName,
			UpdateHistoryAction updateHistoryAction)
		{
			var currentLine = 0;
			var orderService = new OrderService();
			var fixedpurchaseService = new FixedPurchaseService();
			var errorMessage = new StringBuilder();

			streamReader.ReadLine();

			// Read data
			while (streamReader.EndOfStream == false)
			{
				currentLine++;

				// Get line data
				var buffer = StringUtility.SplitCsvLine(streamReader.ReadLine());

				// Get order info
				var paymentOrderId = (buffer[2].Length > 10)
					? buffer[2].Substring(buffer[2].Length - 10, 10)
					: buffer[2];
				var order = orderService.GetOrderByPaymentOrderId(paymentOrderId);

				if (order == null)
				{
					// Not exists order
					errorMessage.AppendLine(string.Format(
						ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_ID_NOT_EXISTS),
						paymentOrderId,
						currentLine));
				}
				else if (string.IsNullOrEmpty(m_xmlSetting.ExtendStatusShippedPending))
				{
					// WriteLog update order extend status failed
					errorMessage.AppendLine(string.Format(
						ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_UPDATE_ORDER_EXTENDSTATUS_ERROR),
						currentLine,
						order.OrderId,
						m_xmlSetting.ExtendStatusShippedPending));
				}
				else
				{
					// Update order extend status
					var update = orderService.UpdateOrderExtendStatus(
						order.OrderId,
						int.Parse(m_xmlSetting.ExtendStatusShippedPending),
						Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON,
						DateTime.Now,
						loginOperatorName,
						updateHistoryAction);

					if (update == 0)
					{
						// WriteLog update order extend status failed
						errorMessage.AppendLine(string.Format(
							ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_UPDATE_ORDER_EXTENDSTATUS_ERROR),
							currentLine,
							order.OrderId,
							m_xmlSetting.ExtendStatusShippedPending));
					}
					else
					{
						// Success count
						m_iUpdatedCount++;

						if (fixedpurchaseService.GetCountOrderFixedPurchase(order.OrderId) > 0)
						{
							// Check fixed purchase id exists
							if (order.IsFixedPurchaseOrder)
							{
								try
								{
									// Get fixed purchase
									var fixedpurchase = fixedpurchaseService.Get(StringUtility.ToEmpty(order.FixedPurchaseId));

									// Update fixed purchase extend status
									fixedpurchaseService.UpdateExtendStatus(
										fixedpurchase.FixedPurchaseId,
										int.Parse(m_xmlSetting.ExtendStatusShippedPending),
										Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_ON,
										DateTime.Now,
										loginOperatorName,
										updateHistoryAction);
								}
								catch (Exception)
								{
									// Not exist fixed purchase
									errorMessage.AppendLine(string.Format(
										ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_FIXED_PURCHASE_NOT_EXISTS),
										order.PaymentOrderId,
										order.FixedPurchaseId,
										currentLine));
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

			// Total line count
			m_iLinesCount = currentLine;
			return (m_iLinesCount > 0);
		}

		/// <summary>
		/// Import Setting
		/// </summary>
		private class ImportSetting
		{
			/// <summary>注文拡張ステータス：出荷済み保留</summary>
			public string ExtendStatusShippedPending { get; set; }
		}
	}
}
