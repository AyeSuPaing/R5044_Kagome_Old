/*
=========================================================================================================
  Module      : Tempostar import order status(ApiImportCommandBuilder_TempoStar_ImportOrderStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using w2.ExternalAPI.Common;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Ftp;
using w2.ExternalAPI.Common.Import;
using System.Text;
using w2.ExternalAPI.Common.Command;

namespace TempoStar.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// Tempostar import order status
	/// </summary>
	public class ApiImportCommandBuilder_TempoStar_ImportOrderStatus : ApiImportCommandBuilder
	{
		#region Constants
		private List<string> m_orderStatusValid = new List<string> { "800", "950", "999" };
		#endregion

		#region Variables
		// Fluent ftp utility
		private FluentFtpUtility m_fluentFtpUtill = new FluentFtpUtility(
			Constants_Setting.SETTING_FTP_HOST,
			Constants_Setting.SETTING_FTP_USER_NAME,
			Constants_Setting.SETTING_FTP_USER_PASSWORD,
			Constants_Setting.SETTING_FTP_USER_ACTIVE,
			Constants_Setting.SETTING_FTP_ENABLE_SSL);
		#endregion

		#region Import
		/// <summary>
		/// Import
		/// </summary>
		/// <param name="apiEntry">Api entry</param>
		protected override void Import(ApiEntry apiEntry)
		{
			var orderStatusArg = GetArgument(apiEntry);
			if ((string.IsNullOrEmpty(orderStatusArg.OrderId)) || (orderStatusArg.OrderId.Contains("-") == false)) return;

			var orderStatus = new OrderStatus();
			var orderStatusValid = m_orderStatusValid.Where(item => item.Contains(orderStatusArg.OrderStatus)).FirstOrDefault();
			if (string.IsNullOrEmpty(orderStatusValid)) return;

			try
			{
				var result = orderStatus.Do(orderStatusArg);
				if (result.ResultStatus != EnumResultStatus.Complete)
				{
					ExternalApiUtility.SendMailToOperator(string.Empty, Constants_Setting.ERRMSG_IMPORT_ORDER_STATUS_UPDATE_FAIL, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);
				}
			}
			catch (Exception exception)
			{
				ExternalApiUtility.SendMailToOperator(exception.ToString(), Constants_Setting.ERRMSG_IMPORT_ORDER_STATUS_UPDATE_FAIL, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);

				throw exception;
			}

			Console.WriteLine("\t -> 注文ID: " + orderStatusArg.OrderId.Split('-')[1]);
		}
		#endregion

		#region GetArgument
		/// <summary>
		/// Get argument
		/// </summary>
		/// <param name="apiEntry">Api entry</param>
		/// <returns>Order status arg</returns>
		private static OrderStatusArg GetArgument(ApiEntry apiEntry)
		{
			DateTime dateTime;
			DateTime? orderPaymentDate = null;
			if (apiEntry.Data.ItemArray.Length > 2)
			{
				orderPaymentDate = DateTime.TryParse(apiEntry.Data[2].ToString(), out dateTime) ? dateTime : (DateTime?)null;
			}

			var orderStatusArg = new OrderStatusArg
			{
				OrderId = apiEntry.Data[0].ToString(),
				OrderStatus = apiEntry.Data[1].ToString(),
				OrderPaymentDate = orderPaymentDate
			};

			return orderStatusArg;
		}
		#endregion

		#region ParepareImportFile
		/// <summary>
		/// Parepare import file
		/// </summary>
		/// <param name="importFilepath">Import file path</param>
		public override void ParepareImportFile(string importFilepath)
		{
			var fileName = Path.GetFileName(importFilepath);

			var importDir = Path.GetDirectoryName(importFilepath);

			var message = new StringBuilder();
			message.Append("・開始日時: ").AppendLine(DateTime.Now.ToString()).Append("・ファイル: ").AppendLine(fileName);
			Console.WriteLine(message.ToString());

			var downloadSource = Path.Combine(Constants_Setting.SETTING_FTP_ORDER_STATUS_DOWNLOAD_DIRECTORY, fileName);

			// File exists
			if (File.Exists(Path.Combine(importDir, Constants_Setting.SETTING_LOCAL_SUCCESS_DIRECTORY, fileName)))
			{
				ExternalApiUtility.SendMailToOperator(fileName, Constants_Setting.ERRMSG_IMPORT_ORDER_STATUS_LOCAL_FILE_ALREADY_EXISTS, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);

				return;
			}

			// Down load file from ftp
			if (m_fluentFtpUtill.Download(downloadSource, importFilepath) == false)
			{
				ExternalApiUtility.SendMailToOperator(string.Empty, Constants_Setting.ERRMSG_IMPORT_ORDER_STATUS_GET_OR_CONNECT_FAIL, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);

				return;
			}

			m_fluentFtpUtill.Delete(downloadSource);
		}
		#endregion

		#region PostDo
		/// <summary>
		/// Post do
		/// </summary>
		public override void PostDo()
		{
			Console.WriteLine("\n・終了日時: " + DateTime.Now);
		}
		#endregion
	}
}