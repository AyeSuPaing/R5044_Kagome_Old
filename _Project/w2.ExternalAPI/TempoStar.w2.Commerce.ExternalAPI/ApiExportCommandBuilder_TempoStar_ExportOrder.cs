/*
=========================================================================================================
  Module      : Tempostar export order(ApiExportCommandBuilder_TempoStar_ExportOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using w2.App.Common;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Option;
using w2.App.Common.Order;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.DeliveryCompany;
using w2.Domain.UpdateHistory.Helper;
using w2.ExternalAPI.Common;
using w2.ExternalAPI.Common.Command.ApiCommand.EntireOrder;
using w2.ExternalAPI.Common.Command.ApiCommand.Order;
using w2.ExternalAPI.Common.Export;
using w2.ExternalAPI.Common.Ftp;

namespace TempoStar.w2.Commerce.ExternalAPI
{
	/// <summary>
	/// Tempostar export order
	/// </summary>
	public class ApiExportCommandBuilder_TempoStar_ExportOrder : ApiExportCommandBuilder
	{
		#region Variables
		// Order ids
		private List<string> m_orderIds = new List<string>();

		// Fluent ftp utility
		private FluentFtpUtility m_fluentFtpUtill = new FluentFtpUtility(
			Constants_Setting.SETTING_FTP_HOST,
			Constants_Setting.SETTING_FTP_USER_NAME,
			Constants_Setting.SETTING_FTP_USER_PASSWORD,
			Constants_Setting.SETTING_FTP_USER_ACTIVE,
			Constants_Setting.SETTING_FTP_ENABLE_SSL);
		#endregion

		#region Init Initialization processing
		/// <summary>
		/// Initialization processing
		/// </summary>
		/// <returns>Result data</returns>
		public override DataTable GetDataTable()
		{
			// Command generation
			var command = new GetOrdersTempostar();

			// Command execution
			var result = (GetOrdersTempostarResult)command.Do(null);

			return result.ResultTable;
		}
		#endregion

		#region Export processing
		/// <summary>
		/// Export
		/// </summary>
		/// <param name="record">Record</param>
		/// <returns>Result data array</returns>
		protected override object[] Export(IDataRecord record)
		{
			// Get order items
			// 注文商品情報
			var orderPriceSubTotalTax = (decimal)record["order_price_subtotal_tax"];
			var orderItemProductPrice = (decimal)record["product_price"];
			var orderItemPoductPriceTax = TaxCalculationUtility.GetTaxPrice(
				orderItemProductPrice,
				(decimal)record["product_tax_rate"],
				(string)record["product_tax_round_type"]);
			var orderItemtPriceTaxExclude = TaxCalculationUtility.GetPriceTaxExcluded(orderItemProductPrice, orderItemPoductPriceTax);
			var orderItemtPriceTaxInclude = TaxCalculationUtility.GetPriceTaxIncluded(orderItemProductPrice, orderItemPoductPriceTax);
			var orderPriceSubTotal = (decimal)record["order_price_subtotal"];
			var orderPriceSubtotalTaxExclude = TaxCalculationUtility.GetPriceTaxExcluded(orderPriceSubTotal, orderPriceSubTotalTax);
			var orderPriceSubtotalTaxInclude = TaxCalculationUtility.GetPriceTaxIncluded(orderPriceSubTotal, orderPriceSubTotalTax);
			var orderPriceShippingTaxInclude = TaxCalculationUtility.GetPriceTaxIncluded(
				(decimal)record["order_price_shipping"],
				(decimal)record["order_price_shipping_tax"]);
			var orderPriceExchangeTaxInclude = TaxCalculationUtility.GetPriceTaxIncluded(
				(decimal)record["order_price_exchange"],
				(decimal)record["order_price_exchange_tax"]);
			var orderPriceTotal = (decimal)record["order_price_total"];
			var totalOrderDiscountPrice = CalculateOrderTotalDiscount(record);
			var orderDate = Convert.ToDateTime(record["order_date"]).ToString("yyyy/MM/dd HH:mm");
			var orderPaymentDate = string.IsNullOrEmpty(record["order_payment_date"].ToString())
				? record["order_payment_date"]
				: Convert.ToDateTime(record["order_payment_date"]).ToString("yyyy/MM/dd HH:mm");
			var shippingDate = string.IsNullOrEmpty(record[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE].ToString())
				? string.Empty
				: Convert.ToDateTime(record[Constants.FIELD_ORDERSHIPPING_SHIPPING_DATE]).ToString("yyyy/MM/dd");

			var result = new List<object>();

			// 1. ショップ受注番号
			result.Add(string.Format("{0}-{1}", Constants_Setting.SETTING_FTP_SHOP_CODE_DIRECTORY, record["order_id"].ToString()));
			// 2. 受注日時
			result.Add(orderDate);
			// 3.注文者名
			result.Add(StringUtility.ToZenkaku(record["owner_name"].ToString()));
			// 4.注文者名カナ
			result.Add(StringUtility.ToZenkakuKatakana(record["owner_name_kana"]));
			// 5.注文者郵便番号
			result.Add(record["owner_zip"].ToString());
			// 6.注文者住所
			result.Add(StringUtility.ToZenkaku(record["owner_addr"]));
			// 7.注文者電話番号
			result.Add(record["owner_tel1"].ToString());
			// 8.注文者FAX番号
			result.Add(string.Empty);
			// 9.注文者メールアドレス
			result.Add(GetValidMailAddressLength(record["owner_mail_addr"].ToString()));
			// 10. 支払方法名
			result.Add(StringUtility.ToEmpty(record["payment_name"]));
			// 11 支払回数名
			result.Add(record["card_instruments"].ToString());
			// 12 デバイスタイプ
			result.Add(GetOrderKbn(record["order_kbn"].ToString()));
			// 13 ポイント
			result.Add(record["last_order_point_use"].ToString());
			// 14 クーポン
			result.Add(totalOrderDiscountPrice.ToPriceString());
			// 15 商品計税抜
			result.Add(orderPriceSubtotalTaxExclude.ToPriceString());
			// 16 消費税計
			result.Add(orderPriceSubTotalTax.ToPriceString());
			// 17 商品計税込 
			result.Add(orderPriceSubtotalTaxInclude.ToPriceString());
			// 18 配送料計
			result.Add(orderPriceShippingTaxInclude.ToPriceString());
			// 19 決済手数料計
			result.Add(orderPriceExchangeTaxInclude.ToPriceString());
			// 20 請求金額
			result.Add(orderPriceTotal.ToPriceString());
			// 21 入金日付
			result.Add(orderPaymentDate);
			// 22 受注備考
			result.Add(record["memo"].ToString());
			// 23 社内備考
			result.Add(record["management_memo"].ToString());
			// 24 倉庫指示
			result.Add(string.Empty);
			// 25 配送先名
			result.Add(StringUtility.ToZenkaku(record["shipping_name"]));
			// 26 配送先名カナ
			result.Add(StringUtility.ToZenkakuKatakana(record["shipping_name_kana"]));
			// 27 配送先郵便番号
			result.Add(record["shipping_zip"]);
			// 28 配送先住所
			result.Add(StringUtility.ToZenkaku(record["shipping_addr"]));
			// 29 配送先電話番号
			result.Add(record["shipping_tel1"]);
			// 30 配送先FAX番号
			result.Add(string.Empty);
			// 31 配送方法名
			result.Add(StringUtility.ToEmpty(record["delivery_company_name"]));
			// 32 配送時間名
			result.Add(GetShippingTimeText(StringUtility.ToEmpty(record["delivery_company_id"]), StringUtility.ToEmpty(record["shipping_time"])));
			// 33 配送毎配送料
			result.Add(orderPriceShippingTaxInclude.ToPriceString());
			// 34 配送毎決済手数料
			result.Add(orderPriceExchangeTaxInclude.ToPriceString());
			// 35 配送毎ポイント
			result.Add(record["last_order_point_use"]);
			// 36 配送毎クーポン
			result.Add(totalOrderDiscountPrice.ToPriceString());
			// 37 配送毎請求金額
			result.Add(orderPriceTotal.ToPriceString());
			// 38 ギフトフラグ
			result.Add(string.Empty);
			// 39 出荷日
			result.Add(string.Empty);
			// 40 配送日
			result.Add(shippingDate);
			// 41 荷物番号
			result.Add(string.Empty);
			// 42 配送毎連絡
			result.Add(string.Empty);
			// 43 商品コード
			result.Add(record["variation_id"]);
			// 44 商品名
			result.Add(record["product_name"]);
			// 45 項目選択肢
			result.Add(string.Empty);
			// 46 数量
			result.Add((int)record["item_quantity_single"]);
			// 47 税抜単価
			result.Add(orderItemtPriceTaxExclude.ToPriceString());
			// 48 単品消費税
			result.Add(orderItemPoductPriceTax.ToPriceString());
			// 49 税込単価
			result.Add(orderItemtPriceTaxInclude.ToPriceString());
			// 50 商品連絡
			result.Add(string.Empty);

			return result.ToArray();
		}
		#endregion

		#region GetOrderKbn
		/// <summary>
		/// Get order kbn
		/// </summary>
		/// <param name="orderKbnInput">Order kbn input</param>
		/// <returns>Order kbn</returns>
		private string GetOrderKbn(string orderKbnInput)
		{
			switch (orderKbnInput)
			{
				case Constants.FLG_ORDER_ORDER_KBN_SMARTPHONE:
					return "S";

				case Constants.FLG_ORDER_ORDER_KBN_MOBILE:
					return "M";

				default:
					return "P";
			}
		}
		#endregion

		#region GetValidMailAddressLength
		/// <summary>
		/// Get valid mail address length
		/// </summary>
		/// <param name="mailAddr">Mail address</param>
		/// <returns>Mail address</returns>
		private string GetValidMailAddressLength(string mailAddr)
		{
			return (mailAddr.Length > Constants_Setting.SETTING_FTP_MAIL_ADDRESS_LENGTH_MAX) ? mailAddr.Substring(mailAddr.Length - 1, 1) : mailAddr;
		}
		#endregion

		#region CalculateOrderTotalDiscount
		/// <summary>
		/// Calculate order total discount
		/// </summary>
		/// <param name="record">Record</param>
		/// <returns>Order total discount</returns>
		private decimal CalculateOrderTotalDiscount(IDataRecord record)
		{
			return TaxCalculationUtility.GetPriceTaxIncluded(
				(decimal)record["order_price_subtotal"]
					+ (decimal)record["order_price_shipping"]
					+ (decimal)record["order_price_exchange"],
				(decimal)record["order_price_subtotal_tax"]
					+ (decimal)record["order_price_shipping_tax"]
					+ (decimal)record["order_price_exchange_tax"])
				- (decimal)record["last_order_point_use"]
				- (decimal)record["order_price_total"];
		}
		#endregion

		#region GetShippingTimeText
		/// <summary>
		/// 配送希望時間帯表示文言取得
		/// </summary>
		/// <param name="deliveryCompanyId">配送会社ID</param>
		/// <param name="shippingTimeId">配送希望時間帯</param>
		/// <returns>配送希望時間帯表示文言</returns>
		private string GetShippingTimeText(string deliveryCompanyId, string shippingTimeId)
		{
			var shippingTimeText = string.Empty;

			var deliveryCompany = new DeliveryCompanyService().Get(deliveryCompanyId);
			if (deliveryCompany != null)
			{
				shippingTimeText = deliveryCompany.GetShippingTimeMessage(shippingTimeId);
			}

			return ((string.IsNullOrEmpty(shippingTimeText) == false) ? shippingTimeText : "指定無し");
		}
		#endregion

		#region Post export
		/// <summary>
		/// Post export
		/// </summary>
		/// <param name="objects">Object data</param>
		public override void PostExport(object[] objects)
		{
			if (objects.Length > 0)
			{
				m_orderIds.Add(objects[0].ToString());
			}
			else
			{
				ExternalApiUtility.SendMailToOperator(string.Empty, Constants_Setting.ERRMSG_EXPORT_ORDER_EXPORT_FAIL, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);
			}
		}
		#endregion

		#region EndExport
		/// <summary>
		/// Execute after generating CSV file
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <param name="orderIds">Order ids</param>
		public override void EndExport(string filePath)
		{
			// Add file header
			AddFileHeader(filePath);

			// Upload file to ftp
			FtpFileUpload(filePath);

			// Clear log
			ClearLog();
		}
		#endregion

		#region AddFileHeader
		/// <summary>
		/// 指定したファイルの先頭にヘッダーを追加
		/// </summary>
		/// <param name="filePath">ヘッダーを追加するファイルのフルパス</param>
		private void AddFileHeader(string filePath)
		{
			var header = string.Format("'{0}'", string.Join("','", Constants_Setting.SETTING_FTP_FILE_HEADER.Split(','))).Replace("'", "\"");
			var encoding = Encoding.GetEncoding(Constants_Setting.SETTING_FTP_FILE_ENCODE);
			File.WriteAllText(filePath, (header + "\r\n" + File.ReadAllText(filePath, encoding)), encoding);
		}
		#endregion

		#region FtpFileUpload
		/// <summary>
		/// Ftp file upload
		/// </summary>
		/// <param name="filePath">File path</param>
		private void FtpFileUpload(string filePath)
		{
			var isCheckFtp = true;
			var fileName = new FileInfo(filePath).Name;
			var ftpTargetFilePath = Path.Combine(Constants_Setting.SETTING_FTP_ORDER_UPLOAD_DIRECTORY, fileName);

			var exportDirectory = Path.GetDirectoryName(filePath);

			var localActiveFilePath = Path.Combine(exportDirectory, Constants_Setting.SETTING_LOCAL_ACTIVE_DIRECTORY, fileName);
			if (File.Exists(localActiveFilePath))
			{
				AppLogger.WriteError(Constants_Setting.ERRMSG_EXPORT_ORDER_LOCAL_FILE_ALREADY_EXISTS);

				return;
			}

			// Move file to active directory
			ExternalApiUtility.MoveFileToDirectory(filePath, localActiveFilePath);

			// Check connect ftp
			if (m_fluentFtpUtill.IsConnected() == false)
			{
				isCheckFtp = false;

				// Send mail to operator
				ExternalApiUtility.SendMailToOperator(fileName, Constants_Setting.ERRMSG_EXPORT_ORDER_CONNECT_OR_UPLOAD_FAIL, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);
			}

			if (isCheckFtp && m_fluentFtpUtill.CheckFileExist(ftpTargetFilePath))
			{
				isCheckFtp = false;

				// Send mail to operator
				ExternalApiUtility.SendMailToOperator(fileName, Constants_Setting.ERRMSG_EXPORT_ORDER_FTP_FILE_ALREADY_EXISTS, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);
			}

			if (m_orderIds.Count > 0)
			{
				var message = new StringBuilder();
				message.Append("・開始日時: ").AppendLine(DateTime.Now.ToString()).Append("・ファイル: ").Append(fileName);
				Console.WriteLine(message.ToString());

				if (isCheckFtp && m_fluentFtpUtill.Upload(localActiveFilePath, ftpTargetFilePath))
				{
					// Update order extend status
					foreach (var orderIdTemp in m_orderIds)
					{
						var orderId = orderIdTemp.Split('-')[1];
						OrderCommon.UpdateOrderExtendStatus(orderId, 37, true, string.Empty, DateTime.Now, UpdateHistoryAction.Insert);

						Console.WriteLine("\t -> 注文ID: " + orderId);
					}

					// Move file to success directory
					ExternalApiUtility.MoveFileToDirectory(localActiveFilePath, Path.Combine(exportDirectory, Constants_Setting.SETTING_LOCAL_SUCCESS_DIRECTORY, fileName));

					// Check capacity Ftp server
					if (IsFullCapacity(Path.GetDirectoryName(ftpTargetFilePath)))
					{
						ExternalApiUtility.SendMailToOperator(string.Empty, Constants_Setting.ERRMSG_EXPORT_ORDER_FTP_FULL_CAPACITY, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);
					}
				}
				else
				{
					if (isCheckFtp)
					{
						// Send mail to operator
						ExternalApiUtility.SendMailToOperator(fileName, Constants_Setting.ERRMSG_EXPORT_ORDER_CONNECT_OR_UPLOAD_FAIL, Constants_Setting.SHOP_ID, Constants_Setting.SETTING_LOCAL_MAIL_TEMPLATE);
					}

					// Move file to error directory
					ExternalApiUtility.MoveFileToDirectory(localActiveFilePath, Path.Combine(exportDirectory, Constants_Setting.SETTING_LOCAL_ERROR_DIRECTORY, fileName));
				}

				Console.WriteLine("・終了日時: " + DateTime.Now);
			}
		}
		#endregion

		#region CheckFtpFreeCapacity
		/// <summary>
		/// Check capacity memory
		/// </summary>
		/// <param name="filePath">File path</param>
		/// <returns>True if full capacity, else false</returns>
		private bool IsFullCapacity(string filePath)
		{
			long value = 0;
			if (long.TryParse(Constants_Setting.SETTING_FTP_CAPACITY_LIMIT, out value) == false) return false;

			var currentSize = m_fluentFtpUtill.GetFileSize(filePath) / (1024 * 1024);

			return currentSize >= value;
		}
		#endregion

		#region ClearLog
		/// <summary>
		/// Clear log on FTP server
		/// </summary>
		private void ClearLog()
		{
			var path = Path.Combine(Constants_Setting.SETTING_FTP_ORDER_UPLOAD_DIRECTORY, "log");
			m_fluentFtpUtill.ClearLog(path, Constants_Setting.SETTING_FTP_LOG_TIME_LIMIT);
		}
		#endregion

		#region
		/// <summary>
		/// 注文商品情報取得
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <returns>注文情報</returns>
		private DataTable GetOrderItem(string orderId)
		{
			// コマンド生成
			var cmd = new GetEntireOrder();

			// 引数生成
			var arg = new GetEntireOrderArg
			{
				DataType = OrderDataType.OrderItem,
				ShopId = "0",
				OrderId = orderId
			};

			// コマンド実行
			return ((GetEntireOrderResult)cmd.Do(arg)).ResultTable;
		}
		#endregion
	}
}