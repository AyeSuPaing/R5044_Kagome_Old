/*
=========================================================================================================
  Module      : 010_Crocs基幹システム連携用データ設定クラス(DataExporter_010_Crocs_0.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Resources;
using System.Text;
using System.Xml;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.App.Common.DataExporters
{
	///*********************************************************************************************
	/// <summary>
	/// 010_Crocs基幹システム連携用データ設定クラス
	/// </summary>
	///*********************************************************************************************
	public partial class DataExporter_010_Crocs_0 : DataExporterBase
	{
		// 一時保存用読み取り行データ
		private IDataRecord m_idrTempRowData = null;
		// 明細行No
		private int m_iLineNumber = 1;
		// 注文ID
		private string m_strOrderIdOld = null;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public DataExporter_010_Crocs_0(SqlAccessor sqlAccessor)
			: base(sqlAccessor,
				"E1ExportOrder" + DateTime.Today.ToString("yyyyMMdd") + ".txt",
				"|",
				"",
				Encoding.GetEncoding("Shift-JIS"),
				Constants.ExportKbn.OrderList,
				"ExportData/Statement",
				Properties.Resources.Db_ExportData_010_Crocs,
				"E1連携（受注データ）")
		{
			// 何もしない //
		}

		/// <summary>
		/// 基幹システム連携用単位出力データ取得
		/// </summary>
		/// <param name="ssdrDataReader">SQLデータリーダー</param>
		/// <returns>単位出力データ</returns>
		public override string GetExportUnitData(SqlStatementDataReader ssdrDataReader)
		{
			StringBuilder sbResult = new StringBuilder();

			if (m_strOrderIdOld == null)
			{
				sbResult.Append(GetShippingRow(ssdrDataReader));
			}

			if (m_iLineNumber == 1)
			{
				m_strOrderIdOld = (string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_ID];
			}

			if ((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_ID] != m_strOrderIdOld)
			{
				// 代引きの場合
				if ((string)m_idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
				{
					// 代引き
					sbResult.Append(GetCODRow(m_idrTempRowData, m_iLineNumber));
					// 明細行カウントアップ
					m_iLineNumber++;
				}
				// 配送料
				sbResult.Append(GetDeliveryRow(m_idrTempRowData, m_iLineNumber));

				// 送付先住所行作成
				sbResult.Append(GetShippingRow(ssdrDataReader));
				m_strOrderIdOld = (string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_ID];
				m_iLineNumber = 1;
			}

			// 商品明細行作成
			if ((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_ID] == (string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_ID])
			{
				List<string> lRowDataList = new List<string>();

				// 1.Record Type
				lRowDataList.Add("2");
				// 2.Order Number
				lRowDataList.Add((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_ID]);
				// 3.Line Number
				lRowDataList.Add(m_iLineNumber.ToString());
				// 4.Order Company
				lRowDataList.Add("00620");
				// 5.Branch Plant
				lRowDataList.Add("JAP003");
				// 6.Sold to/Bill to
				lRowDataList.Add("3000118");
				// 7.Ship to
				lRowDataList.Add("3000118");
				// 8.MCU Company
				lRowDataList.Add("00620");
				// 9.Transaction Data
				if (ssdrDataReader[Constants.FIELD_ORDER_ORDER_DATE] != System.DBNull.Value)
				{
					lRowDataList.Add(StringUtility.ToEmpty(((DateTime)ssdrDataReader[Constants.FIELD_ORDER_ORDER_DATE]).ToString("MM/dd/yyyy")));
				}
				else
				{
					lRowDataList.Add("");
				}
				// 10.Ship Date
				lRowDataList.Add("0");
				// 11.Item Number
				lRowDataList.Add(((string)ssdrDataReader[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID]));
				// 12.Item Transaction UOM
				lRowDataList.Add("");
				// 13.Ship Quantity
				lRowDataList.Add((ssdrDataReader[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]).ToString());
				// 14.Commitment Flag
				lRowDataList.Add("S");
				// 15.Extended price
				lRowDataList.Add(((int)ssdrDataReader[Constants.FIELD_ORDERITEM_ITEM_QUANTITY] * AwayFromZero(ssdrDataReader[Constants.FIELD_ORDERITEM_PRODUCT_PRICE].ToString())).ToString());
				// 16.Unit Price
				lRowDataList.Add(AwayFromZero(ssdrDataReader[Constants.FIELD_ORDERITEM_PRODUCT_PRICE].ToString()).ToString());
				// 17.Line Type
				lRowDataList.Add("S");
				// 18.Pricing UOM
				lRowDataList.Add("");
				// 19.Transaction currency
				lRowDataList.Add("JPY");
				// 20.Payment type
				if ((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
				{
					lRowDataList.Add(".");
				}
				else if ((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					lRowDataList.Add("");
				}
				// 21.Authorization Code
				lRowDataList.Add("");
				// 22.Credit Card Type
				lRowDataList.Add("");
				// 23.Authorized Amount
				if ((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
				{
					lRowDataList.Add("");
				}
				else if ((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					lRowDataList.Add(ssdrDataReader[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL].ToString());
				}
				// 24.Authorized Date
				if ((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
				{
					lRowDataList.Add("");
				}
				else if ((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					if (ssdrDataReader[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE] != System.DBNull.Value)
					{
						lRowDataList.Add(StringUtility.ToEmpty(((DateTime)ssdrDataReader[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE]).ToString("MM/dd/yyyy")));
					}
					else
					{
						lRowDataList.Add("");
					}
				}
				// 25.Payment Term
				if ((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
				{
					lRowDataList.Add("030");
				}
				else if ((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
				{
					lRowDataList.Add("010");
				}

				// 商品明細行データ追加
				sbResult.Append(CreateRowData(lRowDataList));

				// 明細行カウントアップ
				m_iLineNumber++;
				m_idrTempRowData = (IDataRecord)ssdrDataReader.SqlDataReader;
			}

			// 代引きの場合
			if ((string)m_idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
			{
				// 代引き
				sbResult.Append(GetCODRow(m_idrTempRowData, m_iLineNumber));
				// 明細行カウントアップ
				m_iLineNumber++;

			}
			// 配送料
			sbResult.Append(GetDeliveryRow(m_idrTempRowData, m_iLineNumber));

			return sbResult.ToString();
		}

		/// <summary>
		/// 送付先住所データ行作成
		/// </summary>
		/// <param name="ssdrDataReader">SQLデータリーダー</param>
		/// <returns>送付先住所行データ</returns>
		private string GetShippingRow(SqlStatementDataReader ssdrDataReader)
		{
			List<string> lRowDataList = new List<string>();

			// 1.Record Type
			lRowDataList.Add("6");
			// 2.Customer PO Number
			lRowDataList.Add((string)ssdrDataReader[Constants.FIELD_ORDER_ORDER_ID]);
			// 3.Order Company
			lRowDataList.Add("00620");
			// 4.Type of Address
			lRowDataList.Add("2");
			// 5.Address Number
			lRowDataList.Add("3000118");
			// 6.Name - Mailing
			lRowDataList.Add((string)ssdrDataReader[Constants.FIELD_ORDERSHIPPING_SHIPPING_NAME]);
			// 7.Address Line 1
			// 8.Address Line 2
			string strShippingAddr = null;
			string strShippingAddr2 = null;
			string strShippingAddrTemp = (string)ssdrDataReader[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR1] + (string)ssdrDataReader[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR2] + (string)ssdrDataReader[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR3] + (string)ssdrDataReader[Constants.FIELD_ORDERSHIPPING_SHIPPING_ADDR4];
			if (strShippingAddrTemp.Length <= 40)
			{
				if (strShippingAddrTemp.Length <= 20)
				{
					strShippingAddr = strShippingAddrTemp;
					strShippingAddr2 = "";
				}
				else
				{
					int iAddr2Sub = strShippingAddrTemp.Length - 20;
					strShippingAddr = strShippingAddrTemp.Substring(0, 20);
					strShippingAddr2 = strShippingAddrTemp.Substring(20, iAddr2Sub);
				}
			}
			else
			{
				strShippingAddr = strShippingAddrTemp.Substring(0, 20);
				strShippingAddr2 = strShippingAddrTemp.Substring(20, 20);
			}
			lRowDataList.Add(strShippingAddr);
			lRowDataList.Add(strShippingAddr2);
			// 9.Address Line 3
			lRowDataList.Add((string)ssdrDataReader[Constants.FIELD_ORDERSHIPPING_SHIPPING_TEL1]);
			// 10.Postal Code
			lRowDataList.Add((string)ssdrDataReader[Constants.FIELD_ORDERSHIPPING_SHIPPING_ZIP]);
			// 11.City
			lRowDataList.Add("Japan");
			// 12.Country
			lRowDataList.Add("JP");

			return CreateRowData(lRowDataList);
		}

		/// <summary>
		/// 代引きデータ行作成
		/// </summary>
		/// <param name="idrTempRowData">一時保存読み取り行データ</param>
		/// <param name="iLineNumber">明細行番号</param>
		/// <returns>代引き行データ</returns>
		private string GetCODRow(IDataRecord idrTempRowData, int iLineNumber)
		{
			List<string> lRowDataList = new List<string>();

			// 1.Record Type
			lRowDataList.Add("2");
			// 2.Order Number
			lRowDataList.Add((string)idrTempRowData[Constants.FIELD_ORDER_ORDER_ID]);
			// 3.Line Number
			lRowDataList.Add(iLineNumber.ToString());
			// 4.Order Company
			lRowDataList.Add("00620");
			// 5.Branch Plant
			lRowDataList.Add("JAP003");
			// 6.Sold to/Bill to
			lRowDataList.Add("3000118");
			// 7.Ship to
			lRowDataList.Add("3000118");
			// 8.MCU Company
			lRowDataList.Add("00620");
			// 9.Transaction Data
			if (idrTempRowData[Constants.FIELD_ORDER_ORDER_DATE] != System.DBNull.Value)
			{
				lRowDataList.Add(StringUtility.ToEmpty(((DateTime)idrTempRowData[Constants.FIELD_ORDER_ORDER_DATE]).ToString("MM/dd/yyyy")));
			}
			else
			{
				lRowDataList.Add("");
			}
			// 10.Ship Date
			lRowDataList.Add("0");
			// 11.Item Number
			lRowDataList.Add("NS031");
			// 12.Item Transaction UOM
			lRowDataList.Add("");
			// 13.Ship Quantity
			lRowDataList.Add("1");
			// 14.Commitment Flag
			lRowDataList.Add("S");
			// 15.Extended price
			lRowDataList.Add(AwayFromZero(idrTempRowData[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE].ToString()).ToString());
			// 16.Unit Price
			lRowDataList.Add(AwayFromZero(idrTempRowData[Constants.FIELD_ORDER_ORDER_PRICE_EXCHANGE].ToString()).ToString());
			// 17.Line Type
			lRowDataList.Add("NJ");
			// 18.Pricing UOM
			lRowDataList.Add("");
			// 19.Transaction currency
			lRowDataList.Add("JPY");
			// 20.Payment type
			lRowDataList.Add(".");
			// 21.Authorization Code
			lRowDataList.Add("");
			// 22.Credit Card Type
			lRowDataList.Add("");
			// 23.Authorized Amount
			lRowDataList.Add("");
			// 24.Authorized Date
			lRowDataList.Add("");
			// 25.Payment Term
			lRowDataList.Add("030");
			// 明細行カウントアップ
			iLineNumber++;

			return CreateRowData(lRowDataList);
		}

		/// <summary>
		/// 配送料データ行作成
		/// </summary>
		/// <param name="idrTempRowData">一時保存読み取り行データ</param>
		/// <param name="iLineNumber">明細行番号</param>
		/// <returns>配送料行データ</returns>
		private string GetDeliveryRow(IDataRecord idrTempRowData, int iLineNumber)
		{
			List<string> lRowDataList = new List<string>();

			// 1.Record Type
			lRowDataList.Add("2");
			// 2.Order Number
			lRowDataList.Add((string)idrTempRowData[Constants.FIELD_ORDER_ORDER_ID]);
			// 3.Line Number
			lRowDataList.Add(iLineNumber.ToString());
			// 4.Order Company
			lRowDataList.Add("00620");
			// 5.Branch Plant
			lRowDataList.Add("JAP003");
			// 6.Sold to/Bill to
			lRowDataList.Add("3000118");
			// 7.Ship to
			lRowDataList.Add("3000118");
			// 8.MCU Company
			lRowDataList.Add("00620");
			// 9.Transaction Data
			if (idrTempRowData[Constants.FIELD_ORDER_ORDER_DATE] != System.DBNull.Value)
			{
				lRowDataList.Add(StringUtility.ToEmpty(((DateTime)idrTempRowData[Constants.FIELD_ORDER_ORDER_DATE]).ToString("MM/dd/yyyy")));
			}
			else
			{
				lRowDataList.Add("");
			}
			// 10.Ship Date
			lRowDataList.Add("0");
			// 11.Item Number
			lRowDataList.Add("NS030");
			// 12.Item Transaction UOM
			lRowDataList.Add("");
			// 13.Ship Quantity
			lRowDataList.Add("1");
			// 14.Commitment Flag
			lRowDataList.Add("S");
			// 15.Extended price
			lRowDataList.Add(AwayFromZero(idrTempRowData[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING].ToString()).ToString());
			// 16.Unit Price
			lRowDataList.Add(AwayFromZero(idrTempRowData[Constants.FIELD_ORDER_ORDER_PRICE_SHIPPING].ToString()).ToString());
			// 17.Line Type
			lRowDataList.Add("NJ");
			// 18.Pricing UOM
			lRowDataList.Add("");
			// 19.Transaction currency
			lRowDataList.Add("JPY");
			// 20.Payment type
			if ((string)idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
			{
				lRowDataList.Add(".");
			}
			else if ((string)idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				lRowDataList.Add("");
			}
			// 21.Authorization Code
			lRowDataList.Add("");
			// 22.Credit Card Type
			lRowDataList.Add("");
			// 23.Authorized Amount
			if ((string)idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
			{
				lRowDataList.Add("");
			}
			else if ((string)idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				lRowDataList.Add(idrTempRowData[Constants.FIELD_ORDER_ORDER_PRICE_TOTAL].ToString());
			}
			// 24.Authorized Date
			if ((string)idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
			{
				lRowDataList.Add("");
			}
			else if ((string)idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				if (idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE] != System.DBNull.Value)
				{
					lRowDataList.Add(StringUtility.ToEmpty(((DateTime)idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE]).ToString("MM/dd/yyyy")));
				}
				else
				{
					lRowDataList.Add("");
				}
			}
			// 25.Payment Term
			if ((string)idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_COLLECT)
			{
				lRowDataList.Add("030");
			}
			else if ((string)idrTempRowData[Constants.FIELD_ORDER_ORDER_PAYMENT_KBN] == Constants.FLG_PAYMENT_PAYMENT_ID_CREDIT)
			{
				lRowDataList.Add("010");
			}

			return CreateRowData(lRowDataList);
		}

		/// <summary>
		/// 四捨五入
		/// </summary>
		/// <param name="strPrice">金額</param>
		/// <returns>金額</returns>
		private double AwayFromZero(string strPrice)
		{
			const MidpointRounding mrAway = MidpointRounding.AwayFromZero;

			return (Math.Round((double.Parse(strPrice) / 1.05), mrAway));
		}
	}
}
