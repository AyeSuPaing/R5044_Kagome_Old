/*
=========================================================================================================
  Module      : ApiExportCommandBuilder Egoist ExportOrderItems(ApiExportCommandBuilder_P0008_Egoist_ExportOrderItems.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using w2.App.Common;
using w2.App.Common.Option;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.ExternalAPI.Common.Command.ApiCommand.EntireOrder;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;
using w2.ExternalAPI.Common.Ftp;

namespace P0008_Egoist.w2.Commerce.ExternalAPI
{
	public class ApiExportCommandBuilder_P0008_Egoist_ExportOrderItems : ApiExportCommandBuilder
	{
		private const string CONST_API_EXPORT_PROCESS_KBN = "51";
		private const string CONST_API_EXPORT_TRANSACTION_KBN = "10";
		private const string CONST_API_EXPORT_STOCK_CD = "200001";
		private const string CONST_API_EXPORT_CUSTOMER_CD = "200001";
		private const string CONST_API_EXPORT_PRICE_FLG = "0";
		private const string CONST_API_EXPORT_SLIP_NO = "0";
		private const string CONST_API_EXPORT_SLIP_NO2 = "0";
		private const string CONST_API_EXPORT_RATE_KBN = "00";
		private const string CONST_API_EXPORT_RATE_OVER = "0";
		private const string CONST_API_EXPORT_CURRENCY_RATE = "0";
		private const int CONST_API_EXPORT_EXTEND_STATUS19 = 19;
		private const int CONST_API_EXPORT_EXTEND_STATUS20 = 20;

		#region #Init 初期化処理
		/// <summary>初期化処理</summary>
		public override DataTable GetDataTable()
		{
			this.IndexRecord = 0;
			GetOrderItemsResult result = null;

			foreach (string orderStatus in Properties[FIELD_API_EXPORT_ORDER_STATUS].Split(','))
			{
				// コマンド生成
				GetOrderItems orderItems = new GetOrderItems();

				GetOrderItemsArg orderItemsArg = new GetOrderItemsArg()
				{
					CreatedTimeSpan = new PastAbsoluteTimeSpan(0,
						DateTime.Now.AddDays(-int.Parse(Properties[FIELD_API_EXPORT_TIMESPAN])),
						DateTime.Now),
					OrderStatus = orderStatus,	// 注文ステータス
					OrderExtendedStatusSpecification = OrderExtendedStatusSpecification.GenByString(string.Format("({0}F)",
						(orderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDERED) ? CONST_API_EXPORT_EXTEND_STATUS19 : CONST_API_EXPORT_EXTEND_STATUS20)),	// 連携フラグがOFF
					ReturnExchangeKbn = Properties[FIELD_API_EXPORT_RETURN_EXCHANGE_KBN]	// Order return exchange kbn
				};

				if (result == null)
				{
					result = (GetOrderItemsResult)orderItems.Do(orderItemsArg);
				}
				else
				{
					result.ResultTable.Merge(((GetOrderItemsResult)orderItems.Do(orderItemsArg)).ResultTable);
				}
			}

			return result.ResultTable;
		}
		#endregion

		#region #Export 出力処理
		/// <summary>出力処理</summary>
		protected override object[] Export(IDataRecord record)
		{
			// 注文情報
			DataTable orderData = GetOrder(record[FIELD_API_EXPORT_ORDER_ID].ToString());
			DataRow order = orderData.Rows[0];

			this.IndexRecord++;
			this.OrderStatus = order[Constants.FIELD_ORDER_ORDER_STATUS].ToString();
			string productId = record[FIELD_API_EXPORT_PRODUCT_ID].ToString();
			string variationId = record[FIELD_API_EXPORT_VARIATION_ID].ToString();
			string color = string.Empty;
			string size = string.Empty;

			if ((productId.Length == 8) && (variationId.Length > 10))
			{
				size = string.Format("0{0}", variationId.Substring(8, 1));
				color = variationId.Substring(9, 2);
			}
			else if ((productId.Length == 12) && (variationId.Length > 14))
			{
				size = string.Format("0{0}", variationId.Substring(14, 1));
				color = variationId.Substring(12, 2);
			}

			// Order date
			var orderDate = Convert.ToDateTime(order[Constants.FIELD_ORDER_ORDER_DATE]).ToString("yyyyMMdd");

			// Excluded product price
			var excludedProductPrice = TaxCalculationUtility.GetPriceTaxExcluded(
				(decimal)record[FIELD_API_EXPORT_PRODUCT_PRICE],
				TaxCalculationUtility.GetTaxPrice(
						(decimal)record[FIELD_API_EXPORT_PRODUCT_PRICE],
						(decimal)record[FIELD_API_EXPORT_PRODUCT_TAX_RATE],
						(string)record[FIELD_API_EXPORT_PRODUCT_TAX_ROUND_TYPE]));

			// Excluded product price org
			var excludedProductPriceOrg = TaxCalculationUtility.GetPriceTaxExcluded(
				(decimal)record[FIELD_API_EXPORT_PRODUCT_PRICE_ORG],
				TaxCalculationUtility.GetTaxPrice(
						(decimal)record[FIELD_API_EXPORT_PRODUCT_PRICE],
						(decimal)record[FIELD_API_EXPORT_PRODUCT_TAX_RATE],
						(string)record[FIELD_API_EXPORT_PRODUCT_TAX_ROUND_TYPE]));

			var result = new List<object>
			{
				this.IndexRecord.ToString(),		// 1.行NO
				CONST_API_EXPORT_PROCESS_KBN,		// 2.処理区分
				CONST_API_EXPORT_TRANSACTION_KBN,	// 3.取引区分
				orderDate,							// 4.出荷日
				orderDate,							// 5.納品日
				CONST_API_EXPORT_STOCK_CD,			// 6.倉庫CD
				CONST_API_EXPORT_CUSTOMER_CD,		// 7.得意先CD
				string.Empty,						// 8.JANCODE
				productId,							// 9.商品CD
				CONST_API_EXPORT_PRICE_FLG,			// 10.原価FLG
				color,								// 11.色CD
				size,								// 12.サイズCD
				record[FIELD_API_EXPORT_ORDER_ID].ToString(),	// 13.商品シリアル
				Convert.ToDecimal(record[FIELD_API_EXPORT_ITEM_QUANTITY]) * (decimal)(100 * ((order[Constants.FIELD_ORDER_ORDER_STATUS].ToString() == Constants.FLG_ORDER_ORDER_STATUS_ORDERED) ? 1 : (-1))),	// 14.数量
				excludedProductPrice,				// 15.単価
				excludedProductPriceOrg,											// 16.上代単価
				excludedProductPrice,				// 17.下代単価
				CONST_API_EXPORT_SLIP_NO,			// 18.関連伝票NO
				CONST_API_EXPORT_SLIP_NO2,			// 19.関連伝票NO2
				CONST_API_EXPORT_RATE_KBN,			// 20.レート区分
				string.Empty,						// 21.ブランドCD
				string.Empty,						// 22.メモ
				CONST_API_EXPORT_RATE_OVER,			// 23.掛け率
				CONST_API_EXPORT_CURRENCY_RATE		// 24.外貨単価
			};

			return result.ToArray();
		}
		#endregion

		#region #Get Order
		/// <summary>
		/// 注文情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文情報</returns>
		private DataTable GetOrder(string orderId)
		{
			// コマンド生成
			GetEntireOrder entireOrder = new GetEntireOrder();

			// 引数生成
			GetEntireOrderArg entireOrderArg = new GetEntireOrderArg()
			{
				DataType = OrderDataType.Order,
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				OrderId = orderId
			};

			// コマンド実行
			return ((GetEntireOrderResult)entireOrder.Do(entireOrderArg)).ResultTable;
		}
		#endregion

		#region #Switch flags
		/// <summary>
		/// 書き込み完了したらフラグをたてる
		/// </summary>
		/// <param name="objects">objects</param>
		public override void PostExport(object[] objects)
		{
			// 連携フラグをONに
			var orderExtendedStatus = new UpdateOrderExtendedStatus();
			var orderExtendedStatusArg = new UpdateOrderExtendedStatusArg(objects[12].ToString(),
				(this.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDERED) ? CONST_API_EXPORT_EXTEND_STATUS19 : CONST_API_EXPORT_EXTEND_STATUS20,
				true,
				Constants.FLG_ORDER_ORDER_EXTEND_STATUS_ON);
			orderExtendedStatus.Do(orderExtendedStatusArg);
		}
		#endregion

		#region
		/// <summary>
		/// CSVファイルの生成後に実行
		/// </summary>
		/// <param name="filePath">生成したファイルのフルパス</param>
		public override void EndExport(string filePath)
		{
			if (File.Exists(filePath) == false)
			{
				FileLogger.WriteError("FTPアップロード予定のファイルが存在しません ファイル名: " + filePath);
				throw new Exception("FTPアップロード予定のファイルが存在しません ファイル名: " + filePath);
			}

			AddFileHeader(filePath);

			if (FtpFileUpload(filePath))
			{
				FileLogger.WriteInfo("ファイルのFTPアップロードが完了しました ファイル名 : " + filePath);
			}
			else
			{
				FileLogger.WriteError("ファイルのFTPアップロードに失敗しました ファイル名 : " + filePath);
				throw new Exception("ファイルのFTPアップロードに失敗しました ファイル名 : " + filePath);
			}
		}
		#endregion

		#region
		/// <summary>
		/// 指定したファイルの先頭にヘッダーを追加
		/// </summary>
		/// <param name="filePath">ヘッダーを追加するファイルのフルパス</param>
		private void AddFileHeader(string filePath)
		{
			var enc = Encoding.GetEncoding(ExternalAPI.Properties.Settings.Default.ftpUploadFileEncode);
			File.WriteAllText(
				filePath,
				  ((string.IsNullOrEmpty(ExternalAPI.Properties.Settings.Default.header)) ? "" : ExternalAPI.Properties.Settings.Default.header + "\r\n")
					+ File.ReadAllText(filePath, enc), enc);
		}
		#endregion

		#region
		/// <summary>
		/// 指定したファイルをFTPアップロード
		/// </summary>
		/// <param name="filePath">アップロードするファイルのフルパス</param>
		/// <returns>成功時はtrue,失敗時はfalse</returns>
		private bool FtpFileUpload(string filePath)
		{
			var fluentFtpUtill = new FluentFtpUtility(
				ExternalAPI.Properties.Settings.Default.ftpHost,
				ExternalAPI.Properties.Settings.Default.ftpUserName,
				ExternalAPI.Properties.Settings.Default.ftpUserPassword,
				ExternalAPI.Properties.Settings.Default.ftpUseActive,
				ExternalAPI.Properties.Settings.Default.ftpEnableSsl);
			var result = fluentFtpUtill.Upload(
				filePath,
					ExternalAPI.Properties.Settings.Default.ftpUploadDirectory
					+ Path.GetFileNameWithoutExtension((new FileInfo(filePath).Name))
					+ ExternalAPI.Properties.Settings.Default.ftpUploadFileExtension);
			return result;
		}
		#endregion

		/// <summary>Index record</summary>
		private int IndexRecord { get; set; }

		/// <summary>Order status</summary>
		private string OrderStatus { get; set; }
	}
}