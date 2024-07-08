/*
=========================================================================================================
  Module      : Data Exporter P0053_Haier (DataExporter_P0053_Haier_0.cs)
 ････････････････････････････････････････････････････････････････････････････････････････････････････････
  Application : w2.App.Common
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Resources;
using System.Text;
using System.Web;
using System.Xml;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Logger;
using w2.App.Common.DataExporters.SettingManager;

namespace w2.App.Common.DataExporters
{
	/// <summary>
	/// P0053 Haier data exporter
	/// </summary>
	public partial class DataExporter_P0053_Haier_0 : DataExporterBase
	{
		/// <summary> Formater </summary>
		private ExportDataFormatter formatter;
		/// <summary> Data exporter file name </summary>
		private const string DATA_EXPORTER_FILE_NAME = "WMSYHC";
		/// <summary> Data exporter link </summary>
		private const string DATA_EXPORTER_EXPORT_LINK = "YHC出荷指示CSV";
		/// <summary> Separate character </summary>
		private static string SEPARATECHARACTER = ",";
		/// <summary> Encoding </summary>
		private static string ENCODING = "Shift-JIS";
		/// <summary> Statement path </summary>
		private static string STATEMENT_PATH = "*/export/statement";
		/// <summary> Field shipper code </summary>
		private const string FIELD_SHIPPER_CODE = "ShipperCode";
		/// <summary> Field delivery code </summary>
		private const string FIELD_DELIVERY_CODE = "DeliveryCode";
		/// <summary> Field warehouse code </summary>
		private const string FIELD_WAREHOUSE_CODE = "WarehouseCode";
		/// <summary> Field check kbn </summary>
		private const string FIELD_CHECK_KBN = "CheckKbn";
		/// <summary> Field shop name </summary>
		private const string FIELD_SHOP_NAME = "ShopName";
		/// <summary> Field shop zip code </summary>
		private const string FIELD_SHOP_ZIP_CODE = "ShopZipCode";
		/// <summary> Field shop address </summary>
		private const string FIELD_SHOP_ADRESS = "ShopAdress";
		/// <summary> Field shop tel </summary>
		private const string FIELD_SHOP_TEL = "ShopTel";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DataExporter_P0053_Haier_0(SqlAccessor sqlAccessor)
			: base(sqlAccessor,
				DATA_EXPORTER_FILE_NAME + DateTime.Now.ToString("yyyyMMddhhmmss") + ".csv",
				SEPARATECHARACTER,
				string.Empty,
				Encoding.GetEncoding(ENCODING),
				Constants.ExportKbn.OrderList,
				STATEMENT_PATH,
				DataExporterSettingManager_P0053_Haier_0_Resource.GetResource(),
				DATA_EXPORTER_EXPORT_LINK)
		{
			formatter = new ExportDataFormatter_P0053_Haier_0(DataExporterSettingManager_P0053_Haier_0_Resource.GetResource());
		}

		/// <summary>
		/// Process for data exporter
		/// </summary>
		/// <param name="exporter">Exporter</param>
		public override void Process(HttpResponse exporter)
		{
			//------------------------------------------------------
			// Setting
			//------------------------------------------------------
			// Content encoding
			exporter.ContentEncoding = this.TextEncoding;
			// Set file name
			exporter.AppendHeader("Content-Disposition", "attachment; filename=" + this.FileName);

			//------------------------------------------------------
			// Export
			//------------------------------------------------------
			string dataLine;

			while (Read(out dataLine))
			{
				exporter.Write(dataLine);
			}

			exporter.End();
		}

		/// <summary>
		/// 基幹システム連携用単位出力データ取得
		/// </summary>
		/// <param name="dataReader">Data reader</param>
		/// <returns>Export unit data</returns>
		public override string GetExportUnitData(SqlStatementDataReader dataReader)
		{
			var rowData = formatter.FormatExportRowData(dataReader);
			rowData[2] = formatter.CheckingFormat(2, DataExporterSettingManager_P0053_Haier_0_ExternalSetting.GetExternalSetting(FIELD_SHIPPER_CODE));	// 荷主コード
			rowData[4] = formatter.CheckingFormat(4, DataExporterSettingManager_P0053_Haier_0_ExternalSetting.GetExternalSetting(FIELD_DELIVERY_CODE));	// 運送会社コード
			rowData[8] = formatter.CheckingFormat(8, DataExporterSettingManager_P0053_Haier_0_ExternalSetting.GetExternalSetting(FIELD_WAREHOUSE_CODE));	// 倉庫コード
			rowData[12] = formatter.CheckingFormat(12, DataExporterSettingManager_P0053_Haier_0_ExternalSetting.GetExternalSetting(FIELD_CHECK_KBN));	// 伝票区分
			rowData[32] = formatter.CheckingFormat(32, DataExporterSettingManager_P0053_Haier_0_ExternalSetting.GetExternalSetting(FIELD_SHOP_NAME));	// 取引先名称
			rowData[35] = formatter.CheckingFormat(35, DataExporterSettingManager_P0053_Haier_0_ExternalSetting.GetExternalSetting(FIELD_SHOP_ZIP_CODE));	// 取引先郵便番号
			rowData[36] = formatter.CheckingFormat(36, DataExporterSettingManager_P0053_Haier_0_ExternalSetting.GetExternalSetting(FIELD_SHOP_ADRESS));	// 取引先住所
			rowData[37] = formatter.CheckingFormat(37, DataExporterSettingManager_P0053_Haier_0_ExternalSetting.GetExternalSetting(FIELD_SHOP_TEL));	// 取引先電話番号

			// Shipping time
			if (DataExporterSettingManager_P0053_Haier_0_ZipCode.GetZipCodes().Any(zipCode => zipCode.Replace("-", string.Empty) == rowData[43])) rowData[11] = string.Empty;

			// Remove mall id
			rowData.RemoveAt(127);

			return CreateRowData(rowData);
		}
	}

	/// <summary>
	/// P0053 haier formatter
	/// </summary>
	public class ExportDataFormatter_P0053_Haier_0 : ExportDataFormatter
	{
		/// <summary>楽天用のモール連携ID</summary>
		private const string MALL_ID_RAKUTEN = "rakuten";

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="settingXML">Setting xml</param>
		public ExportDataFormatter_P0053_Haier_0(string settingXML) : base(settingXML) { }

		/// <summary>
		/// Format data
		/// </summary>
		/// <param name="convertSetting">Convert setting</param>
		/// <param name="value">Value</param>
		/// <returns>Format data</returns>
		protected override string FormatData(ConvertSettingField convertSetting, string value)
		{
			// Convert 注文書番号, 発生元番号
			if ((convertSetting.FieldName == "purchase_order_no") || (convertSetting.FieldName == "original_order_no"))
			{
				return base.FormatData(convertSetting, GetOrderId(value, this.CurrentRecord[Constants.FIELD_ORDER_MALL_ID].ToString()));
			}

			if (convertSetting.DataType == "date_haier")
			{
				DateTime dateForParse;
				if (DateTime.TryParse(value, out dateForParse))
				{
					value = dateForParse.ToString("yyyyMMdd");
				}
				else
				{
					throw new FormatException("型変換(date)にてパース失敗: " + convertSetting.FieldName + ": " + value);
				}
			}

			// Use base format
			return base.FormatData(convertSetting, value);
		}

		/// <summary>
		/// Get order id
		/// </summary>
		/// <param name="orderId">Order id</param>
		/// <param name="mallId">Mall id</param>
		/// <returns>Order id new</returns>
		private string GetOrderId(string orderId, string mallId)
		{
			// Mall id is own site
			if (mallId == Constants.FLG_USER_MALL_ID_OWN_SITE)
			{
				return orderId.Replace("-", string.Empty);
			}

			// Mall id is rakuten
			if ((mallId == MALL_ID_RAKUTEN) && (orderId.Length > 6))
			{
				string orderIdTmp = orderId.Substring(6, orderId.Length - 6).Replace("-", string.Empty);

				return (orderIdTmp.Length > 20) ? orderIdTmp.Substring(0, 20) : orderIdTmp;
			}

			return orderId;
		}
	}
}
