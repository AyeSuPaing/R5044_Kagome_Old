/*
=========================================================================================================
  Module      :  Action Base (ActionBase.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using w2.App.Common.Elogit;
using w2.App.Common.Order;
using w2.Commerce.Batch.WmsShippingBatch.Util;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.DeliveryCompany.Helper;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.WmsShippingBatch.Action
{
	/// <summary>
	/// Action base
	/// </summary>
	public abstract class ActionBase
	{
		/// <summary>
		/// Constructor
		/// </summary>
		protected ActionBase()
		{
		}

		#region Method
		/// <summary>
		/// Execute action
		/// </summary>
		public virtual void Execute()
		{
		}

		/// <summary>
		/// Execute import
		/// </summary>
		/// <param name="filePath">File path</param>
		protected void ExecImport(string filePath)
		{
			this.WmsOrders = new List<WmsOrder>();
			using (var streamReader = new StreamReader(filePath))
			{
				// Convert data
				this.WmsOrders = ConvertDataForImport(streamReader);

				foreach (var wmsOrder in this.WmsOrders)
				{
					// Check and execute import orders
					var order = new OrderService().Get(wmsOrder.OrderId);
					if (order == null) continue;

					Import(order, wmsOrder);
				}
			}
		}

		/// <summary>
		/// Convert data for import
		/// </summary>
		/// <param name="streamData">Stream data</param>
		/// <returns>List of wms order</returns>
		private List<WmsOrder> ConvertDataForImport(StreamReader streamData)
		{
			var lineBuffer = string.Empty;
			var lineCount = 0;
			var wmsOrders = new List<WmsOrder>();

			while (streamData.Peek() >= 0)
			{
				lineCount++;
				lineBuffer = streamData.ReadLine();

				// Combine lines in case the field contains line breaks
				// (if there are line breaks in the CSV line,「"」should be an odd number)
				while (((lineBuffer.Length - lineBuffer.Replace("\"", string.Empty).Length) % 2) != 0)
				{
					lineBuffer += Environment.NewLine + streamData.ReadLine();
				}

				var csvData = StringUtility.SplitCsvLine(lineBuffer);
				if ((lineCount == 1) || (csvData.Length < 4))
				{
					continue;
				}

				// Create Wms order
				var wmsOrder = new WmsOrder
				{
					WmsCode = csvData[0],
					OrderId = csvData[1],
					OrderShippingDate = Validator.IsDate(csvData[2])
						? (DateTime?)DateTime.Parse(csvData[2])
						: null,
					ShippingCheckNo = csvData[3],
				};
				wmsOrders.Add(wmsOrder);
			}
			return wmsOrders;
		}

		/// <summary>
		/// Import
		/// </summary>
		/// <param name="order">Order model</param>
		/// <param name="wmsOrder">Wms order</param>
		private void Import(OrderModel order, WmsOrder wmsOrder)
		{
			var service = new OrderService();
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				// Update order for Wms shipping
				service.UpdateOrderForWmsShipping(
					order.OrderId,
					wmsOrder.ShippingCheckNo,
					wmsOrder.OrderShippingDate,
					Constants.FLG_LASTCHANGED_BATCH,
					UpdateHistoryAction.Insert,
					accessor);

				// Is it possible to register shipping information
				if (OrderCommon.CanShipmentEntry())
				{
					var errorMessage = ExternalShipmentEntry(
						order.OrderId,
						order.PaymentOrderId,
						order.OrderPaymentKbn,
						order.Shippings[0].ShippingCheckNo,
						wmsOrder.ShippingCheckNo,
						order.CardTranId,
						DeliveryCompanyUtil.GetDeliveryCompanyType(
							order.Shippings[0].DeliveryCompanyId,
							order.OrderPaymentKbn),
						UpdateHistoryAction.DoNotInsert,
						accessor);

					if (string.IsNullOrEmpty(errorMessage) == false)
					{
						FileLogger.WriteError(errorMessage);
						accessor.RollbackTransaction();
						return;
					}

					// Add payment memo
					service.AddPaymentMemo(
						order.OrderId,
						OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
							(string.IsNullOrEmpty(order.PaymentOrderId) == false) ? order.PaymentOrderId : order.OrderId,
							order.OrderPaymentKbn,
							order.CardTranId,
							Constants.ACTION_NAME_SHIPPING_REPORT,
							order.LastBilledAmount),
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.DoNotInsert,
						accessor);

					accessor.CommitTransaction();
				}
			}
		}

		/// <summary>
		/// External shipment entry
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="paymentOrderId">Payment order id</param>
		/// <param name="orderPaymentKbn">Order payment kbn</param>
		/// <param name="shippingCheckNoOld">Shipping check no old</param>
		/// <param name="shippingCheckNoNew">Shipping check no new</param>
		/// <param name="cardTranId">Card tran id</param>
		/// <param name="deliveryCompanyType">Delivery company type</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <param name="sqlAccessor">Sql accessor</param>
		/// <returns>Error message</returns>
		protected string ExternalShipmentEntry(
			string orderId,
			string paymentOrderId,
			string orderPaymentKbn,
			string shippingCheckNoOld,
			string shippingCheckNoNew,
			string cardTranId,
			string deliveryCompanyType,
			UpdateHistoryAction updateHistoryAction,
			SqlAccessor sqlAccessor)
		{
			if (OrderCommon.CanShipmentEntry(orderPaymentKbn) == false) return string.Empty;

			var errorMessage = OrderCommon.ShipmentEntry(
				orderId,
				paymentOrderId,
				orderPaymentKbn,
				shippingCheckNoNew,
				shippingCheckNoOld,
				cardTranId,
				deliveryCompanyType,
				updateHistoryAction,
				sqlAccessor);

			// There is an error message
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				return string.Format("{0} 注文ID：{1}", errorMessage, orderId);
			}
			return errorMessage;
		}

		/// <summary>
		/// Get order from file csv
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <returns>List of order information</returns>
		protected List<Hashtable> GetOrderFromFileCSV(string filePath)
		{
			var datas = new List<Hashtable>();
			using (var streamReader = new StreamReader(filePath, Encoding.GetEncoding("Shift_JIS")))
			{
				// Initialization
				var lineBuffer = string.Empty;
				var lineCount = 0;
				var headers = new string[0];

				while (streamReader.Peek() >= 0)
				{
					lineCount++;
					lineBuffer = streamReader.ReadLine();

					// Combine lines in case the field contains line breaks
					// (if there are line breaks in the CSV line,「"」should be an odd number)
					while (((lineBuffer.Length - lineBuffer.Replace("\"", string.Empty).Length) % 2) != 0)
					{
						lineBuffer += Environment.NewLine + streamReader.ReadLine();
					}

					// Csv row split
					var csvData = StringUtility.SplitCsvLine(lineBuffer);
					if (lineCount == 1)
					{
						headers = csvData;
						continue;
					}

					// Create data
					var data = new Hashtable();
					for (var index = 0; index < headers.Length; index++)
					{
						data[StringUtility.ToEmpty(headers[index])] = StringUtility.ToEmpty(csvData[index]);
					}
					datas.Add(data);
				}
			}
			return datas;
		}

		/// <summary>
		/// Create and send mail to operator
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="ifHistoryKey">IF history key</param>
		/// <param name="statusCodeAndMessage">Response message</param>
		/// <param name="orderIds">Order ids</param>
		/// <param name="logText">Log text</param>
		protected virtual void CreateAndSendMailToOperator(
			string message,
			string ifHistoryKey,
			string statusCodeAndMessage = "",
			string[] orderIds = null,
			string logText = "")
		{
			// Create data to send mail
			var input = new Hashtable
			{
				{ MailSendUtility.CONST_KEY_MESSAGE, message },
			};

			if (string.IsNullOrEmpty(statusCodeAndMessage) == false)
			{
				input[ElogitConstants.KEY_RESPONSE_MESSAGE] = statusCodeAndMessage;
			}
			if (string.IsNullOrEmpty(ifHistoryKey) == false)
			{
				input[ElogitConstants.KEY_IF_HISTORY_KEY] = GetMessage(ElogitConstants.KEY_IF_HISTORY_KEY)
					.Replace("@@ 1 @@", ifHistoryKey);
			}
			if (string.IsNullOrEmpty(logText) == false)
			{
				input[ElogitConstants.KEY_LOG_TEXT] = GetMessage(ElogitConstants.KEY_LOG_TEXT)
					.Replace("@@ 1 @@", logText);
			}
			if (orderIds != null)
			{
				input[Constants.FIELD_ORDER_ORDER_ID] = GetMessage(ElogitConstants.KEY_TARGET_ORDER_ID)
					.Replace("@@ 1 @@", string.Join(Environment.NewLine, orderIds));
			}

			// Send mail
			new MailSendUtility().SendMailToOperator(input);
		}

		/// <summary>
		/// Move file
		/// </summary>
		/// <param name="oldFilePath">Old file path</param>
		/// <param name="newDirPath">New directory path</param>
		/// <param name="newPath">New file name</param>
		protected void MoveFile(string oldFilePath, string newDirPath, string newFileName = "")
		{
			// Directory existence check (create if it does not exist)
			if (Directory.Exists(newDirPath) == false) Directory.CreateDirectory(newDirPath);

			// If file name is empty, reset file name
			if (string.IsNullOrEmpty(newFileName)) newFileName = new FileInfo(oldFilePath).Name;

			// Move file
			var newFilePath = Path.Combine(newDirPath, newFileName);
			File.Move(oldFilePath, newFilePath);
		}

		/// <summary>
		/// Get files path
		/// </summary>
		/// <param name="directoryPath">Directory path</param>
		/// <returns>List files path</returns>
		protected string[] GetFilesPath(string directoryPath)
		{
			// Directory existence check (empty array if files not stored)
			if (Directory.Exists(directoryPath) == false) return new string[0];

			return Directory.GetFiles(directoryPath);
		}

		/// <summary>
		/// Get message
		/// </summary>
		/// <param name="key">Key</param>
		/// <returns>Message</returns>
		public string GetMessage(string key)
		{
			if (string.IsNullOrEmpty(key)) return string.Empty;

			var document = XDocument.Load(Constants.PATH_XML_WMS_MESSAGES);
			var message = document.Root.Elements("Message")
				.FirstOrDefault(element => (element.Attributes("key").First().Value == key))
				.Value;
			return message;
		}

		/// <summary>
		/// Write login process
		/// </summary>
		/// <param name="ifHistoryKey">Specify IF history key</param>
		public virtual void WriteLogInProcess(string ifHistoryKey)
		{
			FileLogger.WriteWarn(ifHistoryKey);
		}
		#endregion

		#region Properties
		/// <summary>List of Wms order</summary>
		protected List<WmsOrder> WmsOrders { get; private set; }
		#endregion
	}

	/// <summary>
	/// Wms order
	/// </summary>
	public class WmsOrder
	{
		/// <summary>Order id</summary>
		public string OrderId { set; get; }
		/// <summary>Shipping check no</summary>
		public string ShippingCheckNo { get; set; }
		/// <summary>Order shipping date</summary>
		public DateTime? OrderShippingDate { get; set; }
		/// <summary>Wms code</summary>
		public string WmsCode { set; get; }
	}
}
