/*
=========================================================================================================
  Module      : Import Shipping (ImportShipping.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.DeliveryCompany.Helper;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Import
{
	/// <summary>
	/// Import shipping
	/// </summary>
	public class ImportShipping : ImportBase
	{
		#region Constants
		/// <summary>Data type text</summary>
		private const string TYPE_TEXT = "text";
		/// <summary>Data type number</summary>
		private const string TYPE_NUMBER = "number";
		/// <summary>Data type date</summary>
		private const string TYPE_DATE = "date";
		/// <summary>Xml setting</summary>
		private static ImportSetting m_xmlSetting;
		/// <summary>Error display count</summary>
		private const int ERROR_DISPLAY_COUNT = 100;

		/// <summary>Import status</summary>
		private enum ImportStatus
		{
			/// <summary>Read complete</summary>
			ReadComplete,
			/// <summary>Column count error</summary>
			ColumnCountError,
			/// <summary>Value type error</summary>
			ValueTypeError,
			/// <summary>Format checked</summary>
			FormatChecked,
			/// <summary>No record error</summary>
			NoRecordError,
			/// <summary>Multi record error</summary>
			MultiRecordError,
			/// <summary>Matching error</summary>
			MatchingError,
			/// <summary>Exclude record</summary>
			ExcludeRecord,
			/// <summary>Header record</summary>
			HeaderRecord,
			/// <summary>Footer record</summary>
			FooterRecord,
			/// <summary>Data checked</summary>
			DataChecked,
		}
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="importFileType">Import file type</param>
		/// <param name="fileName">File name</param>
		public ImportShipping(string importFileType, string fileName)
		{
			GetImportSetting(importFileType);
			this.FileName = fileName;
		}

		#region Methods
		/// <summary>
		/// Get import setting
		/// </summary>
		/// <param name="importFileType">Import file type</param>
		private void GetImportSetting(string importFileType)
		{
			// Read configuration file
			var mainElement = XElement.Load(Path.Combine(
				Constants.PHYSICALDIRPATH_COMMERCE_MANAGER,
				Constants.FILE_XML_ORDERFILEIMPORT_SETTING));
			var serviceNodeList =
				from serviceNode in mainElement.Elements("OrderFile")
				where (serviceNode.Element("Value").Value == importFileType)
				select new
				{
					ImportFileSetting = serviceNode.Elements("ImportFileSetting")
						.ToDictionary(node => node.Attribute("key").Value, node => node.Attribute("value").Value),
					WhereConditions = serviceNode.Element("WhereConditions"),
					MatchConditions = serviceNode.Element("MatchConditions"),
					ExcludeConditions = serviceNode.Element("ExcludeConditions"),
				};

			// Basic setting information for import file
			var importFileSettings = serviceNodeList.First().ImportFileSetting;

			// Where condition information
			var whereElements = serviceNodeList.First().WhereConditions.Elements("Item");
			var whereConditions = GetImportSettingConditionItems(whereElements);

			// Match condition information
			var matchElements = serviceNodeList.First().MatchConditions.Elements("Item");
			var matchConditions = GetImportSettingConditionItems(matchElements);

			// Exclude condition information
			var excludeElements = serviceNodeList.First().ExcludeConditions.Elements("Item");
			var excludeConditions = GetImportSettingConditionItems(excludeElements, true);

			var xmlSetting = new ImportSetting
			{
				OrderIdColumnNo = int.Parse(importFileSettings["OrderIdColumnNo"]),
				ShippingCheckNoColumnNo = int.Parse(importFileSettings["ShippingCheckNoColumnNo"]),
				HeaderRowCount = int.Parse(importFileSettings["HeaderRowCount"]),
				FooterRowCount = int.Parse(importFileSettings["FooterRowCount"]),
				ColumnCount = int.Parse(importFileSettings["ColumnCount"]),
				PastMonths = int.Parse(importFileSettings["PastMonths"]),
				AllowMultiRecordUpdate = bool.Parse(importFileSettings["AllowMultiRecordUpdate"]),
				NoRecordExclude = bool.Parse(importFileSettings["NoRecordExclude"]),
				WhereCondition = whereConditions,
				MatchCondition = matchConditions,
				ExcludeCondition = excludeConditions,
			};

			m_xmlSetting = xmlSetting;
		}

		/// <summary>
		/// Get import setting condition items
		/// </summary>
		/// <param name="elements">Elements</param>
		/// <param name="isExclude">Is exclude</param>
		/// <returns>List of condition item</returns>
		private List<ImportSetting.ConditionItem> GetImportSettingConditionItems(IEnumerable<XElement> elements, bool isExclude = false)
		{
			var conditionItems = new List<ImportSetting.ConditionItem>();

			// Set exclude condition
			if (isExclude)
			{
				foreach (var element in elements)
				{
					var conditionItem = new ImportSetting.ConditionItem
					{
						Mode = element.Attribute("mode").Value,
						ColumnNo = int.Parse(element.Attribute("columnNo").Value),
						ColumnType = element.Attribute("columnType").Value,
						Value = element.Attribute("value").Value,
					};
					conditionItems.Add(conditionItem);
				}
				return conditionItems;
			}

			// Set match and where condition
			foreach (var element in elements)
			{
				var conditionItem = new ImportSetting.ConditionItem
				{
					Name = element.Attribute("name").Value,
					ColumnNo = int.Parse(element.Attribute("columnNo").Value),
					ColumnType = element.Attribute("columnType").Value,
					FieldName = element.Attribute("field").Value,
				};
				conditionItems.Add(conditionItem);
			}
			return conditionItems;
		}

		/// <summary>
		/// Import
		/// </summary>
		/// <param name="fileStream">Contents of csv file</param>
		/// <param name="operatorName">Operator name</param>
		/// <param name="fileName">Import file name</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <returns>Execution result</returns>
		public override bool Import(StreamReader fileStream, string operatorName, UpdateHistoryAction updateHistoryAction)
		{
			this.ErrorList = new List<Dictionary<string, string>>();

			// Get data from csv file
			var shippingData = ReadCsvData(fileStream);

			// Check if there is a line read as shipping data
			var hasShippingData = shippingData.Exists(item => (item.Status == ImportStatus.ReadComplete));
			if (hasShippingData == false)
			{
				m_strErrorMessage = ImportMessage.GetMessages(
					ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_SHIPPING_DATA_EMPTY);
				return hasShippingData;
			}

			// Check the imported file data
			CheckFormat(shippingData);

			// Check exclusion data
			CheckExcludeData(shippingData);

			// Order data acquisition & data check
			CheckData(shippingData);

			// Configure error display data
			CreateErrorDisplayData(shippingData);

			// Output the excluded records to a file
			CreateExcludeRecordsFile(shippingData, this.FileName);

			// Update shipping check no
			var hasNotError = (this.ErrorList.Count == 0);
			if (hasNotError) m_iUpdatedCount = UpdateShippingCheckNo(shippingData, operatorName, updateHistoryAction);

			m_iLinesCount = shippingData
				.Count(item => ((item.Status != ImportStatus.HeaderRecord) && (item.Status != ImportStatus.FooterRecord)));

			return hasNotError;
		}

		/// <summary>
		/// Read csv data
		/// </summary>
		/// <param name="fileStream">The contents of the file</param>
		/// <returns>Import file data</returns>
		private List<ShippingItem> ReadCsvData(StreamReader fileStream)
		{
			var shippingData = new List<ShippingItem>();
			var readLineCount = 0;
			while (fileStream.Peek() > -1)
			{
				readLineCount++;
				var csvData = StringUtility.SplitCsvLine(fileStream.ReadLine());
				var shippingItem = new ShippingItem
				{
					RowNo = readLineCount,
					CsvData = csvData,
					Status = (readLineCount > m_xmlSetting.HeaderRowCount)
						? ImportStatus.ReadComplete
						: ImportStatus.HeaderRecord,
				};
				shippingData.Add(shippingItem);
			}

			var countShippingData = shippingData.Count;
			if (countShippingData == 0) return shippingData;

			var lastDataRow = Math.Max(0, (countShippingData - m_xmlSetting.FooterRowCount));
			for (var index = lastDataRow; index < countShippingData; index++)
			{
				shippingData[index].Status = ImportStatus.FooterRecord;
			}
			return shippingData;
		}

		/// <summary>
		/// Check format
		/// </summary>
		/// <param name="shippingData">Import file data</param>
		private void CheckFormat(List<ShippingItem> shippingData)
		{
			var shippingItems = shippingData.Where(item => (item.Status == ImportStatus.ReadComplete));
			foreach (var shippingItem in shippingItems)
			{
				// Check for excess or deficiency of columns
				if (shippingItem.CsvData.Length != m_xmlSetting.ColumnCount)
				{
					shippingItem.Status = ImportStatus.ColumnCountError;
					continue;
				}

				// Check if a column number larger than the number of imported file columns is specified
				var isColumnNoWhereLarge = (m_xmlSetting.WhereCondition.Where(item => (item.ColumnNo > shippingItem.CsvData.Length)).Count() > 0);
				var isColumnNoMatchLarge = (m_xmlSetting.MatchCondition.Where(item => (item.ColumnNo > shippingItem.CsvData.Length)).Count() > 0);
				if (isColumnNoWhereLarge || isColumnNoMatchLarge)
				{
					shippingItem.Status = ImportStatus.ColumnCountError;
					continue;
				}

				// Check the type of where part and match part
				var isInvalidWhereType = (m_xmlSetting.WhereCondition.Where(item => (TryCast(shippingItem.CsvData[item.ColumnNo - 1], item.ColumnType) == false)).Count() > 0);
				var isInvalidMatchType = (m_xmlSetting.MatchCondition.Where(item => (TryCast(shippingItem.CsvData[item.ColumnNo - 1], item.ColumnType) == false)).Count() > 0);
				if (isInvalidWhereType || isInvalidMatchType)
				{
					shippingItem.Status = ImportStatus.ValueTypeError;
					continue;
				}

				// Check location shipping check no column
				if (m_xmlSetting.ShippingCheckNoColumnNo > shippingItem.CsvData.Length)
				{
					shippingItem.Status = ImportStatus.ColumnCountError;
					continue;
				}

				// Set value for shipping check no
				shippingItem.ShippingCheckNo = shippingItem.CsvData[m_xmlSetting.ShippingCheckNoColumnNo - 1];
				shippingItem.Status = ImportStatus.FormatChecked;
			}
		}

		/// <summary>
		/// Check exclude data
		/// </summary>
		/// <param name="shippingData">Import file contents</param>
		private void CheckExcludeData(List<ShippingItem> shippingData)
		{
			var shippingItems = shippingData.Where(item => (item.Status == ImportStatus.FormatChecked));
			foreach (var shippingItem in shippingItems)
			{
				foreach (var excludeCondition in m_xmlSetting.ExcludeCondition)
				{
					switch (excludeCondition.Mode)
					{
						case "equal":
							if (shippingItem.CsvData[excludeCondition.ColumnNo - 1].Trim() == excludeCondition.Value.Trim())
							{
								shippingItem.Status = ImportStatus.ExcludeRecord;
							}
							break;

						case "multi":
							if (shippingData.FindAll(item => (item.CsvData[excludeCondition.ColumnNo - 1]
								== shippingItem.CsvData[excludeCondition.ColumnNo - 1])).Count() > 1)
							{
								shippingItem.Status = ImportStatus.ExcludeRecord;
							}
							break;

						default:
							break;
					}
				}
			}
		}

		/// <summary>
		/// Check data
		/// </summary>
		/// <param name="shippingData">Import file contents</param>
		private void CheckData(List<ShippingItem> shippingData)
		{
			// Check list format
			var shippingItems = shippingData.Where(item => (item.Status == ImportStatus.FormatChecked));

			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("Order", "GetOrderForImportShipping"))
			{
				var selectStatement = new StringBuilder();
				var whereStatement = new StringBuilder();

				foreach (var matchCondition in m_xmlSetting.MatchCondition)
				{
					selectStatement.Append(", ").Append(matchCondition.FieldName);
				}

				foreach (var conditionItem in m_xmlSetting.WhereCondition)
				{
					var paramName = conditionItem.FieldName.Replace('.', '_');
					whereStatement.Append(" AND ").Append(conditionItem.FieldName).Append(" = @").Append(paramName);
					sqlStatement.AddInputParameters(paramName, GetDbFieldType(conditionItem.ColumnType));
				}

				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ select @@", selectStatement.ToString());
				sqlStatement.Statement = sqlStatement.Statement.Replace("@@ where @@", whereStatement.ToString());

				// Loop with data excluding errors
				foreach (var shippingItem in shippingItems)
				{
					var input = new Hashtable();
					foreach (var conditionItem in m_xmlSetting.WhereCondition)
					{
						input.Add(
							conditionItem.FieldName.Replace('.', '_'),
							CastFieldData(shippingItem.CsvData[conditionItem.ColumnNo - 1], conditionItem.ColumnType));
					}
					input.Add("order_date_from", DateTime.Now.Date.AddMonths(-1 * m_xmlSetting.PastMonths));

					var orderData = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
					// Check the number of order data
					if (orderData.Count > 0)
					{
						shippingItem.OrderData = orderData;

						// Store order ids separated by commas
						foreach (DataRowView item in orderData)
						{
							if (shippingItem.OrderIdList == null) shippingItem.OrderIdList = new List<string>();

							shippingItem.OrderIdList.Add((string)item[Constants.FIELD_ORDER_ORDER_ID]);
						}

						// Multiple record error
						if ((m_xmlSetting.AllowMultiRecordUpdate == false)
							&& (orderData.Count > 1))
						{
							shippingItem.Status = ImportStatus.MultiRecordError;
						}
						continue;
					}

					// If the exclusion setting for 0 cases is true, it is judged as exclusion, otherwise it is set as no record error.
					shippingItem.Status = m_xmlSetting.NoRecordExclude
						? ImportStatus.ExcludeRecord
						: ImportStatus.NoRecordError;
				}
			}

			// Loop with data excluding errors
			foreach (var shippingItem in shippingItems)
			{
				// Data matching check
				var matchCondition = m_xmlSetting.MatchCondition.Where(item => CompareDataMatchCondition(shippingItem, item) == false);
				if (matchCondition.Count() > 0)
				{
					shippingItem.Status = ImportStatus.MatchingError;
					continue;
				}

				// If there are no errors
				shippingItem.Status = ImportStatus.DataChecked;
			}
		}

		/// <summary>
		/// Create error display data
		/// </summary>
		/// <param name="shippingData">Shipping data</param>
		private void CreateErrorDisplayData(List<ShippingItem> shippingData)
		{
			var shippingItems = shippingData.Where(item =>
				((item.Status != ImportStatus.DataChecked)
					&& (item.Status != ImportStatus.ExcludeRecord)
					&& (item.Status != ImportStatus.HeaderRecord)
					&& (item.Status != ImportStatus.FooterRecord)));
			foreach (var shippingItem in shippingItems)
			{
				// Error data is not configured beyond the limit of the number of error displays
				if (this.ErrorList.Count >= ERROR_DISPLAY_COUNT) break;

				var errorData = new Dictionary<string, string>();
				errorData.Add("行", shippingItem.RowNo.ToString());

				// Configure db search condition part
				foreach (var item in m_xmlSetting.WhereCondition)
				{
					errorData.Add(
						item.Name,
						(shippingItem.Status != ImportStatus.ColumnCountError)
							? shippingItem.CsvData[item.ColumnNo - 1]
							: string.Empty);
				}

				// Error contents
				errorData.Add("エラー内容", GetErrorMessage(shippingItem));
				this.ErrorList.Add(errorData);
			}

			// If there is error data, create an error count message
			if (this.ErrorList.Count > 0)
			{
				var errorMessage = new StringBuilder();
				errorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ERROR_EXIST)
					.Replace("@@ 1 @@", this.ErrorList.Count.ToString())
					.Replace("@@ 2 @@", (this.ErrorList.Count == ERROR_DISPLAY_COUNT) ? "以上" : string.Empty));

				// Message of over display number
				if (this.ErrorList.Count == ERROR_DISPLAY_COUNT)
				{
					errorMessage.Append("<br />");
					errorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ERROR_RECORD_OVERLIMIT)
						.Replace("@@ 1 @@", ERROR_DISPLAY_COUNT.ToString()));
				}
				m_strErrorMessage = errorMessage.ToString();
			}
		}

		/// <summary>
		/// Update shipping check no
		/// </summary>
		/// <param name="shippingData">Shipping data</param>
		/// <param name="operatorName">Operator name</param>
		/// <param name="updateHistoryAction">Update history action</param>
		/// <returns>Number of updates (number of processed lines)</returns>
		private int UpdateShippingCheckNo(List<ShippingItem> shippingData, string operatorName, UpdateHistoryAction updateHistoryAction)
		{
			var resultCount = 0;
			var service = new OrderService();

			// Shipping check no update sql execution
			using (var sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				var shippingItems = shippingData.Where(item => (item.Status == ImportStatus.DataChecked));
				foreach (var shippingItem in shippingItems)
				{
					var result = 0;
					// Consider batch renewal of multiple orders
					foreach (DataRowView item in shippingItem.OrderData)
					{
						try
						{
							var orderId = StringUtility.ToEmpty(item[Constants.FIELD_ORDER_ORDER_ID]);
							var order = OrderCommon.GetOrder(orderId, sqlAccessor);

							if (order.Count != 0)
							{
								var shippingCheckNoOld = StringUtility.ToEmpty(order[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO]);
								var paymentOrderId = StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_PAYMENT_ORDER_ID]);
								var orderPaymentKbn = StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]);
								var cardTranId = StringUtility.ToEmpty(order[0][Constants.FIELD_ORDER_CARD_TRAN_ID]);

								// Shipping information registration (external cooperation)
								if (this.ExecExternalShipmentEntry)
								{
									var canShipmentEntry = false;
									var deliveryCompanyType = DeliveryCompanyUtil.GetDeliveryCompanyType(
										StringUtility.ToEmpty(order[0][Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID]),
										orderPaymentKbn);
									var errorMessage = ExternalShipmentEntry(
										orderId,
										paymentOrderId,
										orderPaymentKbn,
										shippingCheckNoOld,
										shippingItem.ShippingCheckNo,
										cardTranId,
										deliveryCompanyType,
										UpdateHistoryAction.DoNotInsert,
										out canShipmentEntry);
									if (string.IsNullOrEmpty(errorMessage) == false)
									{
										m_strErrorMessage += errorMessage + "<br/>";
										continue;
									}

									if (canShipmentEntry)
									{
										// Addition of payment cooperation memo
										var orderPaymentMemo = OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
											string.IsNullOrEmpty(paymentOrderId) ? orderId : paymentOrderId,
											orderPaymentKbn,
											cardTranId,
											Constants.ACTION_NAME_SHIPPING_REPORT,
											(decimal?)order[0][Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]);
										service.AddPaymentMemo(
											orderId,
											orderPaymentMemo,
											operatorName,
											UpdateHistoryAction.DoNotInsert,
											sqlAccessor);
									}
								}

								result += service.UpdateOrderShippingForImportShipping(
									orderId,
									(int)item[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO],
									shippingItem.ShippingCheckNo,
									operatorName,
									updateHistoryAction,
									sqlAccessor);

								if (result == 0)
								{
									AppLogger.WriteError(string.Format(
										ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_UPDATE_SHIPPING_CHECK_NO_ERROR),
										shippingItem.RowNo,
										orderId));
								}
							}
						}
						catch (Exception ex)
						{
							sqlAccessor.RollbackTransaction();
							throw ex;
						}
					}

					if (result > 0) resultCount++;
				}
			}
			return resultCount;
		}

		/// <summary>
		/// Create a filename format string
		/// </summary>
		/// <param name="formatString">Format string</param>
		/// <returns>Formal performance</returns>
		private static string GetRegexString(string formatString)
		{
			var regexPattern = new StringBuilder();
			regexPattern.Append("^");

			foreach (char formatChar in formatString)
			{
				// Convert to regular expression characters
				switch (formatChar)
				{
					// Wildcard
					case '*':
						regexPattern.Append(".*");
						break;

					// Wildcard (1 character)
					case '?':
						regexPattern.Append(".");
						break;

					default:
						regexPattern.Append("[").Append(formatChar).Append("]");
						break;
				}
			}
			return regexPattern.Append("$").ToString();
		}

		/// <summary>
		/// Get error message
		/// </summary>
		/// <param name="shippingItem">ShippingItem</param>
		/// <returns>Return Error Message</returns>
		private string GetErrorMessage(ShippingItem shippingItem)
		{
			var displayErrorMessage = new StringBuilder();
			var errorValuesList = new List<List<string>>();

			// Create a display error message according to the status
			switch (shippingItem.Status)
			{
				// Excess or deficiency of columns in the import file
				case ImportStatus.ColumnCountError:
					displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_COLUMN_COUNT_ERROR)
						.Replace("@@ 1 @@", m_xmlSetting.ColumnCount.ToString())
						.Replace("@@ 2 @@", shippingItem.CsvData.Length.ToString()));
					return displayErrorMessage.ToString();

				// The type of the item in the import file is invalid
				case ImportStatus.ValueTypeError:
					// Detect all values that are subject to error
					errorValuesList = GetTypeErrorValues(shippingItem);
					foreach (List<string> errorValue in errorValuesList)
					{
						if (displayErrorMessage.Length != 0) displayErrorMessage.Append("<br />");
						displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_VALUE_TYPE_ERROR)
							.Replace("@@ 1 @@", errorValue[0])
							.Replace("@@ 2 @@", errorValue[1])
							.Replace("@@ 3 @@", errorValue[2]));
					}
					return displayErrorMessage.ToString();

				// Error recording has no data
				case ImportStatus.NoRecordError:
					displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NO_TARGET_ERROR));
					return displayErrorMessage.ToString();

				// Multi Record Error
				case ImportStatus.MultiRecordError:
					displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_MULTI_TARGET_ERROR));

					displayErrorMessage.Append("<br /><br />注文ID : ");
					displayErrorMessage.Append(CreatePopupUrl(shippingItem.OrderIdList));
					return displayErrorMessage.ToString();

				// Error when matching file and db
				case ImportStatus.MatchingError:
					// Detect all values that are subject to error
					errorValuesList = GetMatchingErrorValues(shippingItem);
					foreach (List<string> errorValues in errorValuesList)
					{
						if (displayErrorMessage.Length != 0) displayErrorMessage.Append("<br />");
						displayErrorMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_MATCHING_ERROR)
							.Replace("@@ 1 @@", errorValues[0])
							.Replace("@@ 2 @@", errorValues[1])
							.Replace("@@ 3 @@", errorValues[2]));
					}

					displayErrorMessage.Append("<br /><br />注文ID : ");
					displayErrorMessage.Append(CreatePopupUrl(shippingItem.OrderIdList));
					return displayErrorMessage.ToString();
			}

			return displayErrorMessage.ToString();
		}

		/// <summary>
		/// Get the error content of the type check used for extraction
		/// </summary>
		/// <param name="shippingItem">Shipping item</param>
		/// <returns>A list of display names and values for the data in which the error was detected</returns>
		private List<List<string>> GetTypeErrorValues(ShippingItem shippingItem)
		{
			var errorValues = new List<List<string>>();

			// Check the type of the search part
			foreach (var item in m_xmlSetting.MatchCondition)
			{
				if (TryCast(shippingItem.CsvData[item.ColumnNo - 1], item.ColumnType)) continue;

				var itemError = GetItemErrorInfo(
					item.Name,
					GetTypeName(item.ColumnType),
					StringUtility.ToEmpty(shippingItem.CsvData[item.ColumnNo - 1]));
				errorValues.Add(itemError);
			}

			// Perform type check of matching part
			foreach (var item in m_xmlSetting.WhereCondition)
			{
				if (TryCast(shippingItem.CsvData[item.ColumnNo - 1], item.ColumnType)) continue;

				var itemError = GetItemErrorInfo(
					item.Name,
					GetTypeName(item.ColumnType),
					StringUtility.ToEmpty(shippingItem.CsvData[item.ColumnNo - 1]));
				errorValues.Add(itemError);
			}
			return errorValues;
		}

		/// <summary>
		/// Get the error content of the type check used for matching
		/// </summary>
		/// <param name="shippingItem">Payment Item</param>
		/// <returns>A list of display names and values for the data in which the error was detected</returns>
		private List<List<string>> GetMatchingErrorValues(ShippingItem shippingItem)
		{
			var errorValuesList = new List<List<string>>();
			// Match file and db data
			foreach (var conditionItem in m_xmlSetting.MatchCondition)
			{
				foreach (DataRowView item in shippingItem.OrderData)
				{
					if (CompareData(shippingItem.CsvData, item, conditionItem)) continue;

					var fieldName = GetFiledNameFromItemSetting(conditionItem.FieldName);
					var itemError = GetItemErrorInfo(
						conditionItem.Name,
						StringUtility.ToEmpty(item[fieldName]),
						shippingItem.CsvData[conditionItem.ColumnNo - 1]);
					errorValuesList.Add(itemError);
				}
			}
			return errorValuesList;
		}

		/// <summary>
		/// Get item error information
		/// </summary>
		/// <param name="name">Name</param>
		/// <param name="typeName">Type name</param>
		/// <param name="columnNo">Column no</param>
		/// <returns>Item error information</returns>
		private List<string> GetItemErrorInfo(string name, string typeName, string columnNo)
		{
			var itemError = new List<string>();
			itemError.Add(name);
			itemError.Add(typeName);
			itemError.Add(columnNo);
			return itemError;
		}

		/// <summary>
		/// Compare data match condition
		/// </summary>
		/// <param name="shippingItem">Shipping item</param>
		/// <param name="conditionItem">Condition item</param>
		/// <returns>Comparison result</returns>
		private bool CompareDataMatchCondition(ShippingItem shippingItem, ImportSetting.ConditionItem conditionItem)
		{
			foreach (DataRowView order in shippingItem.OrderData)
			{
				if (CompareData(shippingItem.CsvData, order, conditionItem) == false) return false;
			}
			return true;
		}

		/// <summary>
		/// Compare data
		/// </summary>
		/// <param name="csvData">Csv data</param>
		/// <param name="orderData">Order data</param>
		/// <param name="conditionItem">Xml configuration file: db collation definition for 1 field</param>
		/// <returns>Comparison result</returns>
		private bool CompareData(string[] csvData, DataRowView orderData, ImportSetting.ConditionItem conditionItem)
		{
			var fieldName = GetFiledNameFromItemSetting(conditionItem.FieldName);
			// Compare values between csv and data
			var result = (CastFieldData(csvData[conditionItem.ColumnNo - 1], conditionItem.ColumnType).ToString()
				== CastFieldData(orderData[fieldName].ToString(), conditionItem.ColumnType).ToString());
			return result;
		}

		/// <summary>
		/// Get filed name from item setting
		/// </summary>
		/// <param name="conditionItemFieldName">Condition item field name</param>
		/// <returns>Filed name</returns>
		private string GetFiledNameFromItemSetting(string conditionItemFieldName)
		{
			if (string.IsNullOrEmpty(conditionItemFieldName)) return string.Empty;

			// Only column of tables (w2_Order, w2_OrderShipping, w2_User) are validated
			var isValidTable = (conditionItemFieldName.Contains(Constants.TABLE_ORDER)
				|| conditionItemFieldName.Contains(Constants.TABLE_ORDERSHIPPING)
				|| conditionItemFieldName.Contains(Constants.TABLE_USER));
			var fieldName = isValidTable
				? StringUtility.ToEmpty(conditionItemFieldName.Split('.')[1])
				: StringUtility.ToEmpty(conditionItemFieldName);
			return fieldName;
		}

		/// <summary>
		/// Make the wording of the type sql type data
		/// </summary>
		/// <param name="columnType">Column type</param>
		/// <returns>Sql server type</returns>
		private SqlDbType GetDbFieldType(string columnType)
		{
			switch (columnType)
			{
				case TYPE_NUMBER:
					return SqlDbType.Int;

				case TYPE_DATE:
					return SqlDbType.DateTime;

				case TYPE_TEXT:
					return SqlDbType.NVarChar;

				default:
					return SqlDbType.NVarChar;
			}
		}

		/// <summary>
		/// Get type name
		/// </summary>
		/// <param name="columnType">Column type</param>
		/// <returns>Model name in Japanese</returns>
		private string GetTypeName(string columnType)
		{
			switch (columnType)
			{
				case TYPE_NUMBER:
					return "数値";

				case TYPE_DATE:
					return "日付";

				case TYPE_TEXT:
					return "文字列";

				default:
					return string.Empty;
			}
		}

		/// <summary>
		/// Cast field data
		/// </summary>
		/// <param name="data">Data</param>
		/// <param name="columnType">Column type</param>
		/// <returns>Data after type conversion</returns>
		private object CastFieldData(string data, string columnType)
		{
			switch (columnType)
			{
				case TYPE_NUMBER:
					return Decimal.Parse(data);

				case TYPE_DATE:
					return CastShortDatetime(data);

				case TYPE_TEXT:
					return StringUtility.ToEmpty(data);

				default:
					return null;
			}
		}

		/// <summary>
		/// Check if it can be converted to the specified type
		/// </summary>
		/// <param name="data">Data</param>
		/// <param name="columnType">Column type</param>
		/// <returns>Check result</returns>
		private bool TryCast(string data, string columnType)
		{
			switch (columnType)
			{
				case TYPE_TEXT:
					return true;

				case TYPE_NUMBER:
					decimal decParseDummy;
					return decimal.TryParse(data, out decParseDummy);

				case TYPE_DATE:
					DateTime dummy;
					var result = (DateTime.TryParse(data, out dummy)
						|| DateTime.TryParseExact(data, "yyyyMMdd", null, DateTimeStyles.AllowLeadingWhite, out dummy));
					return result;

				default:
					return false;
			}
		}

		/// <summary>
		/// Write header and exclusion lines to a file
		/// </summary>
		/// <param name="shippingData">Shipping data</param>
		/// <param name="originalFileName">Original file name</param>
		private void CreateExcludeRecordsFile(List<ShippingItem> shippingData, string originalFileName)
		{
			var existExcludeRecord = shippingData.Exists(item => (item.Status == ImportStatus.ExcludeRecord));
			if (existExcludeRecord == false) return;

			// File name to create
			var fileName = originalFileName + "_exc.csv";

			// Create if there is no output directory
			Directory.CreateDirectory(this.PhysicalOutputDirPath);

			// File path preparation
			var filePath = this.PhysicalOutputDirPath + fileName;

			// Export csv file
			using (var streamWriter = new StreamWriter(filePath, false, Encoding.GetEncoding("Shift_JIS")))
			{
				// Write out header/footer lines and exclusion lines
				var shippingItems = shippingData.FindAll(item =>
					(item.Status == ImportStatus.HeaderRecord)
						|| (item.Status == ImportStatus.FooterRecord)
						|| (item.Status == ImportStatus.ExcludeRecord));
				foreach (var shippingItem in shippingItems)
				{
					streamWriter.Write(StringUtility.CreateEscapedCsvString(shippingItem.CsvData) + "\r\n");
				}
			}
			this.FilePathExcludeRecord = this.OutputDirPath + fileName;
		}

		/// <summary>
		/// Cast short datetime
		/// </summary>
		/// <param name="uncastedDate">Date data (character string type/date type)</param>
		/// <remark>Set the hour, minute, and second to the default (00:00:00)</remark>
		private DateTime CastShortDatetime(string dateString)
		{
			DateTime result;
			if (DateTime.TryParse(dateString, out result))
			{
				return result.Date;
			}

			// If TryParse cannot be performed, specify the format and perform type conversion again
			DateTime.TryParseExact(
				dateString,
				"yyyyMMdd",
				null,
				DateTimeStyles.AllowLeadingWhite,
				out result);
			return result.Date;
		}

		/// <summary>
		/// Create a popup link
		/// </summary>
		/// <param name="orderIdList">List of order id</param>
		/// <returns>Order information detailed url</returns>
		private string CreatePopupUrl(List<string> orderIdList)
		{
			if (m_CreatePopupMessageProc == null) return string.Empty;
			return m_CreatePopupMessageProc(orderIdList);
		}
		#endregion

		/// <summary>
		/// Import Setting
		/// </summary>
		private class ImportSetting
		{
			/// <summary>Order id column no</summary>
			public int OrderIdColumnNo;
			/// <summary>Shipping check no column no</summary>
			public int ShippingCheckNoColumnNo;
			/// <summary>Number of header lines to exclude</summary>
			public int HeaderRowCount;
			/// <summary>Number of footer lines to exclude</summary>
			public int FooterRowCount;
			/// <summary>Number of columns in the import file</summary>
			public int ColumnCount;
			/// <summary>Past months</summary>
			public int PastMonths;
			/// <summary>Multi-line update permission setting</summary>
			public bool AllowMultiRecordUpdate;
			/// <summary>Exclusion permission setting when the record does not exist</summary>
			public bool NoRecordExclude;
			/// <summary>Extraction conditions</summary>
			public List<ConditionItem> WhereCondition;
			/// <summary>Matching conditions</summary>
			public List<ConditionItem> MatchCondition;
			/// <summary>Conditions to be excluded</summary>
			public List<ConditionItem> ExcludeCondition;

			/// <summary>
			/// Detailed conditions for various settings
			/// </summary>
			public class ConditionItem
			{
				/// <summary>Project name</summary>
				public string Name;
				/// <summary>Target column number</summary>
				public int ColumnNo;
				/// <summary>Column type</summary>
				public string ColumnType;
				/// <summary>Db field name to be associated</summary>
				public string FieldName;
				/// <summary>Specify the judgment type</summary>
				public string Mode;
				/// <summary>Specify the comparison character string</summary>
				public string Value;
			}
		}

		/// <summary>
		/// Shipping item
		/// </summary>
		private class ShippingItem
		{
			/// <summary>Csv data</summary>
			public string[] CsvData;
			/// <summary>Order data</summary>
			public DataView OrderData;
			/// <summary>Shipping check no</summary>
			public string ShippingCheckNo;
			/// <summary>Shipping date</summary>
			public DateTime? ShippingDate;
			/// <summary>List of order id</summary>
			public List<string> OrderIdList;
			/// <summary>Status</summary>
			public ImportStatus Status;
			/// <summary>Row no</summary>
			public int RowNo;
		}

		#region +CreatePopupMessageProc
		/// <summary>Create popup message</summary>
		private Func<List<string>, string> m_CreatePopupMessageProc;
		/// <summary>
		/// Popup message creation process
		/// </summary>
		/// <param name="createPopupMessageProc">Popup message creation process</param>
		/// <returns>Instance</returns>
		public ImportShipping SetCreatePopupMessageProc(Func<List<string>, string> createPopupMessageProc)
		{
			m_CreatePopupMessageProc = createPopupMessageProc;
			return this;
		}
		#endregion

		#region Properties
		/// <summary>Error list</summary>
		public List<Dictionary<string, string>> ErrorList { get; private set; }
		/// <summary>Path contents csv</summary>
		private string PathContentsCsv
		{
			get { return Constants.PATH_CONTENTS + "Csv/"; }
		}
		/// <summary>Physical output dirPath</summary>
		private string PhysicalOutputDirPath
		{
			get { return Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + this.PathContentsCsv.Replace("/", @"\"); }
		}
		/// <summary>Output directory path</summary>
		private string OutputDirPath
		{
			get { return Constants.PATH_ROOT_EC + this.PathContentsCsv; }
		}
		/// <summary>File path exclude record</summary>
		public string FilePathExcludeRecord { get; private set; }
		/// <summary>File name</summary>
		public string FileName { get; private set; }
		#endregion
	}
}
