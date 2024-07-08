/*
=========================================================================================================
  Module      :  File Download Cooperation Action (FileDownloadCooperationAction.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using w2.App.Common.Elogit;
using w2.App.Common.Flaps;
using w2.Commerce.Batch.WmsShippingBatch.Util;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.WmsShippingBatch.Action
{
	/// <summary>
	/// File download cooperation action
	/// </summary>
	public class FileDownloadCooperationAction : ActionBase
	{
		/// <summary>
		/// On execute
		/// </summary>
		public override void Execute()
		{
			try
			{
				CreateInstructionFile();

				// Delay a few minutes after the file upload API execution is completed, this process is performed
				Thread.Sleep(Constants.TIME_DELAY_MILLISECONDS);

				DownLoadFile();
			}
			catch (Exception ex)
			{
				// Set information to send mail to the operator
				CreateAndSendMailToOperator(ex.Message, string.Empty);

				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// Create instruction File
		/// </summary>
		private void CreateInstructionFile()
		{
			var result = new ElogitApiService().FileCreateInstructions();
			if (result.IsSuccess)
			{
				// Create folder「Waiting for download file creation」
				if (Directory.Exists(Constants.DIR_PATH_WAIT_DOWNLOAD_FILE) == false)
				{
					Directory.CreateDirectory(Constants.DIR_PATH_WAIT_DOWNLOAD_FILE);
				}

				// Create file path
				var filePath = Path.Combine(
					Constants.DIR_PATH_WAIT_DOWNLOAD_FILE,
					result.Response.IfHistoryKey + ".csv");

				// Delete file
				if (File.Exists(filePath)) File.Delete(filePath);

				// Create file
				File.Create(filePath).Close();

				WriteLog(StringUtility.ToEmpty(result.GetErrorMessage));
				return;
			}

			// Set information to send mail to the operator
			CreateAndSendMailToOperator(
				GetMessage(ElogitConstants.KEY_FILE_DOWNLOAD_LINKAGE_FAIL),
				string.Empty,
				statusCodeAndMessage : result.GetStatusCodeAndErrorMessage);

			// Write log error
			FileLogger.WriteError(result.GetErrorMessage);
		}

		/// <summary>
		/// DownLoad file
		/// </summary>
		private void DownLoadFile()
		{
			var files = GetFilesPath(Constants.DIR_PATH_WAIT_DOWNLOAD_FILE);
			foreach (var file in files)
			{
				var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(file);
				var result = new ElogitApiService().GetIfHistoryKeyDownload(fileNameWithoutExtension);

				if (result.IsSuccess)
				{
					// Excute download file
					DownLoadFile(fileNameWithoutExtension);
					continue;
				}
				else if (result.IsInProcess)
				{
					// Write log inprocess
					base.WriteLogInProcess(fileNameWithoutExtension);
					continue;
				}

				// Send mail to operator when has error
				CreateAndSendMailToOperator(
					GetMessage(ElogitConstants.KEY_IF_HISTORY_STATUS_FAILED),
					fileNameWithoutExtension,
					statusCodeAndMessage : result.GetStatusCodeAndErrorMessage);

				// Write log error
				WriteLogError(
					GetMessage(ElogitConstants.KEY_IF_HISTORY_STATUS_FAILED),
					fileNameWithoutExtension,
					result.GetStatusCodeAndErrorMessage);
			}
		}

		/// <summary>
		/// Download file
		/// </summary>
		/// <param name="ifHistoryKey">IF history key</param>
		private void DownLoadFile(string ifHistoryKey)
		{
			var fileName = string.Format("{0}.csv", ifHistoryKey);
			var result = new ElogitApiService().FileDownload(ifHistoryKey, fileName);

			// If there is no data in the file
			if (string.IsNullOrEmpty(result.DataFile))
			{
				// Set information to send mail to the operator
				CreateAndSendMailToOperator(
					GetMessage(ElogitConstants.KEY_DOWNLOAD_FAIL),
					ifHistoryKey,
					result.GetStatusCodeAndErrorMessage,
					null);

				// Write log error
				FileLogger.WriteError(result.GetStatusCodeAndErrorMessage);
				return;
			}

			// Write download data
			using (var streamWriter = new StreamWriter(
				File.Open(Path.Combine(Constants.DIR_PATH_WAIT_DOWNLOAD_FILE, fileName), FileMode.Open)))
			{
				streamWriter.WriteLine(result.DataFile);
				streamWriter.Close();
			}

			// Execute move file
			base.MoveFile(
				Path.Combine(Constants.DIR_PATH_WAIT_DOWNLOAD_FILE, fileName),
				Constants.DIR_PATH_DOWNLOADED_FILE);

			// Update shipping check no and shipping completion date
			base.ExecImport(Path.Combine(Constants.DIR_PATH_DOWNLOADED_FILE, fileName));

			var orderIdList = new List<string>();
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				foreach (var wmsOrder in this.WmsOrders)
				{
					// Get order information and check
					var order = new OrderService().Get(wmsOrder.OrderId, accessor);
					if (order == null) continue;

					// Update order status ship complete
					new OrderService().UpdateOrderStatus(
						wmsOrder.OrderId,
						Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP,
						DateTime.Now, Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.Insert,
						accessor);

					// FLAPS連携済みフラグをオンに更新
					if (Constants.FLAPS_OPTION_ENABLE)
					{
						new FlapsIntegrationFacade().TurnOnErpIntegrationFlg(
							wmsOrder.OrderId,
							Constants.FLG_LASTCHANGED_BATCH,
							accessor);
					}

					orderIdList.Add(wmsOrder.OrderId);
				}

				accessor.CommitTransaction();
			}

			foreach (var orderId in orderIdList)
			{
				// Set mail addr and send mail to user
				new MailSendUtility().SendMailToUser(orderId);
			}

			// Send mail to operator
			CreateAndSendMailToOperator(
				GetMessage(ElogitConstants.KEY_DOWNLOADED_SUCCESS),
				ifHistoryKey,
				result.GetStatusCodeAndErrorMessage,
				this.WmsOrders.Select(wmsOrder => wmsOrder.OrderId).ToArray());

			// Export log
			WriteLog(
				GetMessage(ElogitConstants.KEY_DOWNLOADED_SUCCESS),
				ifHistoryKey,
				this.WmsOrders.Select(wmsOrder => wmsOrder.OrderId).ToArray());
		}

		/// <summary>
		/// Create and send mail to operator
		/// </summary>
		/// <param name="messages">Messages</param>
		/// <param name="ifHistoryKey">IF history key</param>
		/// <param name="statusCodeAndMessage">Response message</param>
		/// <param name="orderIds">Order ids</param>
		/// <param name="logText">Log text</param>
		protected override void CreateAndSendMailToOperator(
			string messages,
			string ifHistoryKey,
			string statusCodeAndMessage = "",
			string[] orderIds = null,
			string logText = "")
		{
			base.CreateAndSendMailToOperator(messages, ifHistoryKey, statusCodeAndMessage, orderIds, logText);
		}

		/// <summary>
		/// Write log
		/// </summary>
		/// <param name="message">Message</param>
		private void WriteLog(string message)
		{
			FileLogger.Write("FileDownload", message);
		}

		/// <summary>
		/// Write log
		/// </summary>
		/// <param name="message">message</param>
		/// <param name="ifHistoryKey">IF history key</param>
		/// <param name="orderIds">Order ids</param>
		private void WriteLog(string message, string ifHistoryKey, string[] orderIds)
		{
			var result = new StringBuilder();
			result.AppendLine(message);
			result.AppendLine(GetMessage(ElogitConstants.KEY_IF_HISTORY_KEY).Replace("@@ 1 @@", ifHistoryKey));
			result.AppendLine(GetMessage(ElogitConstants.KEY_TARGET_ORDER_ID).Replace("@@ 1 @@", string.Join(Environment.NewLine, orderIds)));
			WriteLog(result.ToString());
		}

		/// <summary>
		/// Write log error
		/// </summary>
		/// <param name="message">Message</param>
		/// <param name="ifHistoryKey">IF history key</param>
		/// <param name="statusCodeAndMessage">Status code and message</param>
		private void WriteLogError(string message, string ifHistoryKey, string statusCodeAndMessage)
		{
			var result = new StringBuilder();
			result.AppendLine(message);
			result.AppendLine(GetMessage(ElogitConstants.KEY_IF_HISTORY_KEY).Replace("@@ 1 @@", ifHistoryKey));
			result.AppendLine(statusCodeAndMessage);
			FileLogger.WriteError(result.ToString());
		}
	}
}
