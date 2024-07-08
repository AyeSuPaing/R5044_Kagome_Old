/*
=========================================================================================================
  Module      : Import Order Extend(ImportOrderExtend.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
	/// Import Order Extend
	/// </summary>
	public class ImportOrderExtend : ImportBase
	{
		/// <summary>Xml Setting</summary>
		private static ImportSetting m_xmlSetting;
		/// <summary>Minimum number of column</summary>
		private const int COLUMNS_COUNT_MIN = 2;

		/// <summary>
		/// Import
		/// </summary>
		/// <param name="fileStream">File Stream</param>
		/// <param name="loginOperatorName">Login Operator Name</param>
		/// <param name="updateHistoryAction">Update History Action</param>
		/// <returns>True: If import success</returns>
		public override bool Import(StreamReader fileStream, string loginOperatorName, UpdateHistoryAction updateHistoryAction)
		{
			// Get csv file data
			var csvData = ReadCsvData(fileStream);

			// Check error message
			if (string.IsNullOrEmpty(m_strErrorMessage) == false) return false;

			// Update Order Extend Status
			if (csvData.Count > 0)
			{
				UpdateOrderExtendStatus(csvData, loginOperatorName);
			}

			return true;
		}

		/// <summary>
		/// Import Order Extend
		/// </summary>
		/// <param name="orderFile">Order File</param>
		public ImportOrderExtend(string orderFile)
		{
			// Get Import Setting
			GetImportSetting(orderFile);
		}

		/// <summary>
		/// Get Import Setting
		/// </summary>
		/// <param name="orderFile">Order File</param>
		public void GetImportSetting(string orderFile)
		{
			// Xml Setting
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
				m_xmlSetting.ColumnCount = int.Parse(importFileSettings["ColumnCount"]);
				m_xmlSetting.HeaderRowCount = int.Parse(importFileSettings["HeaderRowCount"]);
				m_xmlSetting.FooterRowCount = int.Parse(importFileSettings["FooterRowCount"]);
				m_xmlSetting.ExtendStatusNosOnOff = importFileSettings["ExtendStatusNosOnOff"];
				m_xmlSetting.AlsoUpdateFixedPurchaseExtendNos = int.Parse(importFileSettings["AlsoUpdateFixedPurchaseExtendNos"]);
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// Update Order Extend Status
		/// </summary>
		/// <param name="csvData">Csv Data</param>
		/// <param name="loginOperatorName">Login Operator Name</param>
		/// <returns>True: If update success</returns>
		public void UpdateOrderExtendStatus(List<string[]> csvData, string loginOperatorName)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();

				// Get extend status no
				var extendStatusNo = m_xmlSetting.ExtendStatusNosOnOff.Split(',');
				foreach (var data in csvData)
				{
					var orderInfo = new OrderService().Get(data[0]);
					if (orderInfo == null) continue;

					var updated = 0;
					for (var index = 0; index < extendStatusNo.Length; index++)
					{
						// Check max lengh data
						if (data.Length <= (index + 1)) break;

						int valueExtendStatusNo;
						if (int.TryParse(extendStatusNo[index], out valueExtendStatusNo) == false) continue;

						// Get extend status value
						var extendStatusValue = data[index + 1];
						if ((extendStatusValue == Constants.FLG_ORDER_EXTEND_STATUS_OFF)
							|| (extendStatusValue == Constants.FLG_ORDER_EXTEND_STATUS_ON))
						{
							if (m_xmlSetting.AlsoUpdateFixedPurchaseExtendNos == 0)
							{
								// Update Order Extend Status
								updated =
									new OrderService().UpdateOrderExtendStatus(
										orderInfo.OrderId,
										valueExtendStatusNo,
										extendStatusValue,
										DateTime.Today,
										loginOperatorName,
										UpdateHistoryAction.Insert,
										accessor);
							}

							if ((m_xmlSetting.AlsoUpdateFixedPurchaseExtendNos == 1)
								&& (string.IsNullOrEmpty(orderInfo.FixedPurchaseId) == false))
							{
								// Update Order Fixed Purchase Extend Status
								updated =
									new FixedPurchaseService().UpdateExtendStatus(
									orderInfo.FixedPurchaseId,
									valueExtendStatusNo,
									extendStatusValue,
									DateTime.Today,
									loginOperatorName,
									UpdateHistoryAction.Insert,
									accessor);
							}
						}
					}
					m_iUpdatedCount += updated;
				}

				// Number of processing lines
				m_iLinesCount = csvData.Count;
			}
		}

		/// <summary>
		/// Read Csv Data
		/// </summary>
		/// <param name="fileStream">File stream</param>
		/// <returns>Csv Data</returns>
		private List<string[]> ReadCsvData(StreamReader fileStream)
		{
			var data = new List<string[]>();
			var currentLine = 0;
			while (fileStream.Peek() != -1)
			{
				currentLine++;
				var lineBuffer = fileStream.ReadLine();
				var buffer = StringUtility.SplitCsvLine(lineBuffer);

				// Check number of columns of import file
				if ((COLUMNS_COUNT_MIN > buffer.Length)
					|| (m_xmlSetting.ColumnCount != buffer.Length))
				{
					m_strErrorMessage = MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DEFINITION_MISMATCH)
						.Replace("@@ 1 @@", currentLine.ToString());
					m_strErrorMessage += MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DEFINITION_MISMATCH_EXPLANATION)
						.Replace("@@ 1 @@", m_xmlSetting.ColumnCount.ToString())
						.Replace("@@ 2 @@", buffer.Length.ToString());
					this.m_successOrderInfos.Clear();

					return data;
				}

				data.Add(buffer);
			}

			// Check Data Lines Excluded
			CheckDataLinesExcluded(data);

			return data;
		}

		/// <summary>
		/// Check Data Lines Excluded
		/// </summary>
		/// <param name="data"></param>
		private void CheckDataLinesExcluded(List<string[]> data)
		{
			// Check data lines excluded
			if ((data.Count < m_xmlSetting.HeaderRowCount) || (data.Count < m_xmlSetting.FooterRowCount))
			{
				data.RemoveRange(0, data.Count);
			}

			// Check header lines excluded
			if (data.Count > m_xmlSetting.HeaderRowCount)
			{
				data.RemoveRange(0, m_xmlSetting.HeaderRowCount);
			}

			// Check footer lines excluded
			if (data.Count > m_xmlSetting.FooterRowCount)
			{
				data.RemoveRange((data.Count - m_xmlSetting.FooterRowCount), m_xmlSetting.FooterRowCount);
			}
		}

		/// <summary>
		/// Import Setting
		/// </summary>
		private class ImportSetting
		{
			/// <summary>Number of columns of import file</summary>
			public int ColumnCount { get; set; }
			/// <summary>Number of header lines excluded</summary>
			public int HeaderRowCount { get; set; }
			/// <summary>Number of footer lines excluded</summary>
			public int FooterRowCount { get; set; }
			/// <summary>Order extension status number</summary>
			public string ExtendStatusNosOnOff { get; set; }
			/// <summary>Order Fixed Purchase extension status flag</summary>
			public int AlsoUpdateFixedPurchaseExtendNos { get; set; }
		}
	}
}