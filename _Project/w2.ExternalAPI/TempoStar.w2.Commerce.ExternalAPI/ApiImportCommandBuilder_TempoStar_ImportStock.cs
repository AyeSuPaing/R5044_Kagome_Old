/*
=========================================================================================================
  Module      : Tempostar import stock(ApiImportCommandBuilder_TempoStar_ImportStock.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using w2.ExternalAPI.Common;
using w2.ExternalAPI.Common.Command;
using w2.ExternalAPI.Common.Command.ApiCommand;
using w2.ExternalAPI.Common.Command.ApiCommand.Stock;
using w2.ExternalAPI.Common.Entry;
using w2.ExternalAPI.Common.Ftp;
using w2.ExternalAPI.Common.Import;

namespace TempoStar.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// Tempostar import stock
	/// </summary>
	public class ApiImportCommandBuilder_TempoStar_ImportStock : ApiImportCommandBuilder
	{
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
			int stock;
			if (Int32.TryParse((string)apiEntry.Data[1], out stock) == false) return;

			var argument = new ImportStockQuantityArg
			{
				ProductID = GetProductId((string)apiEntry.Data[0]),
				VariationID = (string)apiEntry.Data[0],
				Stock = stock
			};

			if (string.IsNullOrEmpty(argument.ProductID)) return;

			// Command excute
			try
			{
				var command = new ImportStockQuantity();
				var result = (ImportStockQuantityResult)command.Do(argument);

				if (result.ResultStatus != EnumResultStatus.Complete)
				{
					ExternalApiUtility.SendMailToOperator(string.Empty, Constants_Setting.ERRMSG_IMPORT_STOCK_UPDATE_FAIL, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);
				}
			}
			catch (Exception exception)
			{
				ExternalApiUtility.SendMailToOperator(exception.ToString(), Constants_Setting.ERRMSG_IMPORT_STOCK_UPDATE_FAIL, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);

				throw exception;
			}

			Console.WriteLine("\t・商品ID: " + argument.ProductID);
			Console.WriteLine("\t・バリエーションID: " + argument.VariationID);
			Console.WriteLine("\t・在庫数: " + argument.Stock);
		}
		#endregion

		#region GetProductId
		/// <summary>
		/// Get product id
		/// </summary>
		/// <param name="variationId">Variation id</param>
		/// <returns>Product id</returns>
		private string GetProductId(string variationId)
		{
			if (Stocks == null)
			{
				// Command
				var command = new GetStockQuantitiesNow();

				// Argument
				var argument = new GetStockQuantitiesNowArg();

				// Command excute
				Stocks = ((GetStockQuantitiesNowResult)command.Do(argument)).ResultTable;
			}

			var products = Stocks.AsEnumerable().Where(stock => (string)stock["VariationID"] == variationId);

			if (products.Any())
			{
				return (string)products.First()["ProductID"];
			}

			return string.Empty;
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
			message.Append("・開始日時: ").AppendLine(DateTime.Now.ToString()).Append("・ファイル: ").Append(fileName);
			Console.WriteLine(message.ToString());

			var downloadSource = Path.Combine(Constants_Setting.SETTING_FTP_STOCK_DOWNLOAD_DIRECTORY, fileName);

			// File exists
			if (File.Exists(Path.Combine(importDir, Constants_Setting.SETTING_LOCAL_SUCCESS_DIRECTORY, fileName)))
			{
				ExternalApiUtility.SendMailToOperator(fileName, Constants_Setting.ERRMSG_IMPORT_STOCK_LOCAL_FILE_ALREADY_EXISTS, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);

				return;
			}

			// Down load file from ftp
			if (m_fluentFtpUtill.Download(downloadSource, importFilepath) == false)
			{
				ExternalApiUtility.SendMailToOperator(string.Empty, Constants_Setting.ERRMSG_IMPORT_STOCK_GET_OR_CONNECT_FAIL, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);

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
			Console.WriteLine("・終了日時: " + DateTime.Now + "\n");
		}
		#endregion

		#region Properties
		/// <summary>
		/// Stocks
		/// </summary>
		private static DataTable Stocks { get; set; }
		#endregion
	}
}