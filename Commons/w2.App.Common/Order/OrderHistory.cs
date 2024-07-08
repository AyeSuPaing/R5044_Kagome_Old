/*
=========================================================================================================
  Module      : This class is use for update history of order(OrderHistory.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using w2.Common.Sql;
using System;
using System.IO;
using w2.Common.Util;

namespace w2.App.Common.Order
{
	/// <summary>
	/// Use for insert history of order
	/// </summary>
	public class OrderHistory
	{
		/// <summary>
		/// Master used to history
		/// </summary>
		private static readonly List<string> HistoryTable = new List<string>
			{
				"w2_Order",
				"w2_OrderOwner",
				"w2_OrderShipping",
				"w2_OrderItem",
				"w2_OrderCoupon",
				"w2_OrderSetPromotion"
			};

		/// <summary>
		/// Initial OrderHistory object
		/// </summary>
		public OrderHistory()
		{
			this.TimeBegin = DateTime.Now;
		}

		/// <summary>
		/// Get data before update
		/// </summary>
		public void HistoryBegin()
		{
			this.IsNeedAction = false;
			this.DataBefore = GetHistoryData();
		}

		/// <summary>
		/// Get data after update
		/// </summary>
		public void HistoryComplete()
		{
			this.IsNeedAction = true;
			this.DataAfter = GetHistoryData();

			WriteHistoryFile();
		}

		#region Create History
		/// <summary>
		/// Get history data
		/// </summary>
		/// <returns></returns>
		private XElement GetHistoryData()
		{
			if (HistoryTable == null || HistoryTable.Any() == false) return null;

			if (this.Accessor == null)
			{
				using (var sqlAccessor = new SqlAccessor())
				{
					sqlAccessor.OpenConnection();

					return GetHistoryData(sqlAccessor);
				}
			}
			else
			{
				return GetHistoryData(this.Accessor);
			}
		}

		/// <summary>
		/// Get history data
		/// </summary>
		/// <param name="sqlAccessor">SqlAccessor</param>
		/// <returns></returns>
		private XElement GetHistoryData(SqlAccessor sqlAccessor)
		{
			var historyData = new XElement("data");
			var actionData = new StringBuilder();

			foreach (var table in HistoryTable)
			{
				if (CheckOptionEnable(table) == false) continue;

				DataSet data = GetHistoryData(sqlAccessor, table);

				historyData.Add(GetXmlElement(data, table));

				if (this.IsNeedAction
					&& (this.UpdateAction != null)
					&& this.UpdateAction.ContainsKey(table))
				{
					actionData.Append(actionData.Length == 0 ? "" : ", ");
					actionData.Append(GetUpdateAction(data.Tables[0], (List<string>) UpdateAction[table]));
				}
			}

			this.ActionData = actionData.ToString();

			return historyData;
		}

		/// <summary>
		/// Get Xml element of table
		/// </summary>
		/// <param name="sqlAccessor">SqlAccessor</param>
		/// <param name="tableName">Name of table</param>
		/// <returns></returns>
		private DataSet GetHistoryData(SqlAccessor sqlAccessor, string tableName)
		{
			using (SqlStatement sqlStatement = new SqlStatement(GetHistoryStatement(tableName)))
			{
				return sqlStatement.SelectStatement(sqlAccessor, null);
			}
		}

		/// <summary>
		/// Get XmlElement of table
		/// </summary>
		/// <param name="data">DataSet</param>
		/// <param name="tableName">Name of table</param>
		/// <returns></returns>
		private List<XElement> GetXmlElement(DataSet data, string tableName)
		{
			var results = new List<XElement>();

			var document = new XmlDocument();
			document.LoadXml(data.GetXml());

			foreach (XmlNode node in document.SelectNodes("NewDataSet/Table"))
			{
				var element = XElement.Load(new XmlNodeReader(node));
				element.Name = tableName;
				foreach (var elem in element.Elements().Where(elem => string.IsNullOrEmpty(elem.Value) == false))
				{
					elem.Value = StringUtility.RemoveUnavailableControlCode(elem.Value);
				}
				results.Add(element);
			}
			return results;
		}

		/// <summary>
		/// Check Enable Option
		/// </summary>
		/// <param name="tableName">Table Name</param>
		/// <returns></returns>
		private bool CheckOptionEnable(string tableName)
		{
			if ((tableName == Constants.TABLE_ORDERCOUPON) && (Constants.W2MP_COUPON_OPTION_ENABLED == false)) return false;
			if ((tableName == Constants.TABLE_ORDERSETPROMOTION) && (Constants.SETPROMOTION_OPTION_ENABLED == false)) return false;

			return true;
		}

		/// <summary>
		/// Get history query statement
		/// </summary>
		/// <param name="table">Table Name</param>
		/// <returns></returns>
		private string GetHistoryStatement(string table)
		{
			return string.Format("SELECT * FROM {0} WHERE ORDER_ID = '{1}'", table, this.OrderId);
		}
		#endregion

		#region Get Action Info
		/// <summary>
		/// Get update action
		/// </summary>
		/// <param name="data">Data</param>
		/// <param name="fields">Master update</param>
		/// <returns></returns>
		private string GetUpdateAction(DataTable data, List<string> fields)
		{
			if (data == null || fields == null) return null;

			var actionResult = new StringBuilder();

			foreach (DataRow currentRow in data.Rows)
			{
				foreach (var field in fields)
				{
					actionResult.Append(actionResult.Length == 0 ? "" : ", ");
					actionResult.Append(field).Append(":").Append(currentRow[field]);
				}
			}

			return actionResult.ToString();
		}

		/// <summary>
		/// Get list order status action field
		/// </summary>
		/// <param name="status">Status update</param>
		/// <returns>List action field</returns>
		public static List<string> GetOrderStatusAction(string status)
		{
			List<string> actions = new List<string>();
			actions.Add(Constants.FIELD_ORDER_ORDER_STATUS);

			switch (status)
			{
				case Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED:	// 受注承認
					actions.Add(Constants.FIELD_ORDER_ORDER_RECOGNITION_DATE);
					break;

				case Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED:	// 在庫引当済み
					if (Constants.REALSTOCK_OPTION_ENABLED)
					{
						actions.Add(Constants.FIELD_ORDER_ORDER_STOCKRESERVED_DATE);
					}
					break;

				case Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED:	// 出荷手配済み
					actions.Add(Constants.FIELD_ORDER_ORDER_SHIPPING_DATE);
					break;

				case Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP:	// 出荷手配済み	
					actions.Add(Constants.FIELD_ORDER_ORDER_SHIPPED_DATE);
					break;

				case Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP:	// 出荷完了
					actions.Add(Constants.FIELD_ORDER_ORDER_DELIVERING_DATE);
					break;

				case Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED:	// 配送完了
				case Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED:	// キャンセル
					actions.Add(Constants.FIELD_ORDER_ORDER_CANCEL_DATE);
					break;
			}

			return actions;
		}

		/// <summary>
		/// Get list order payment status action field
		/// </summary>
		/// <returns>List action field</returns>
		public static List<string> GetOrderPaymentStatusAction()
		{
			return new List<string>
			{
				Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS,
				Constants.FIELD_ORDER_ORDER_PAYMENT_DATE
			};
		}

		/// <summary>
		/// Get list order demand status action field
		/// </summary>
		/// <returns>List action field</returns>
		public static List<string> GetOrderDemandStatusAction()
		{
			return new List<string>
			{
				Constants.FIELD_ORDER_DEMAND_STATUS,
				Constants.FIELD_ORDER_DEMAND_DATE
			};
		}

		/// <summary>
		/// Get list order return exchange status action field
		/// </summary>
		/// <param name="status">Status update</param>
		/// <returns>List action field</returns>
		public static List<string> GetOrderReturnExchangeStatusAction(string status)
		{
			List<string> actions = new List<string>();
			actions.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_STATUS);

			switch (status)
			{
				case Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_RECEIPT:	// 返品受付
					actions.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_RECEIPT_DATE);
					break;

				case Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_ARRIVAL:// 返品商品到着
					actions.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_ARRIVAL_DATE);
					break;

				case Constants.FLG_ORDER_ORDER_RETURN_EXCHANGE_STATUS_COMPLETE:	// 返品処理計上
					actions.Add(Constants.FIELD_ORDER_ORDER_RETURN_EXCHANGE_COMPLETE_DATE);
					break;
			}

			return actions;
		}

		/// <summary>
		/// Get list order repayment status action field
		/// </summary>
		/// <returns>List action field</returns>
		public static List<string> GetOrderRepaymentStatusAction()
		{
			return new List<string>
			{
				Constants.FIELD_ORDER_ORDER_REPAYMENT_STATUS,
				Constants.FIELD_ORDER_ORDER_REPAYMENT_DATE
			};
		}

		/// <summary>
		/// Get parameters action product real stock change
		/// </summary>
		/// <param name="productRealStockChangeType">Product real stock change type</param>
		/// <returns>Parameters</returns>
		public static Hashtable GetProductRealStockAction(string productRealStockChangeType)
		{
			Hashtable actionInput = new Hashtable();

			switch (productRealStockChangeType)
			{
				// 実在庫引当処理
				case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_RESERVED_STCOK:
					actionInput.Add(Constants.TABLE_ORDER, new List<string>() { Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS });
					actionInput.Add(Constants.TABLE_ORDERITEM,
									new List<string>()
									{
										Constants.FIELD_ORDERITEM_ORDER_ITEM_NO,
										Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED
									});
					break;

				// 実在庫出荷処理
				case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_FORWARD_STCOK:
					actionInput.Add(Constants.TABLE_ORDER, new List<string>() { Constants.FIELD_ORDER_ORDER_SHIPPED_STATUS });
					actionInput.Add(Constants.TABLE_ORDERITEM,
									new List<string>()
									{
										Constants.FIELD_ORDERITEM_ORDER_ITEM_NO,
										Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_SHIPPED
									});
					break;

				// 実在庫戻し処理
				case Constants.FLG_ORDERWORKFLOWSETTING_PRODUCT_REALSTOCK_CHANGE_CANCEL_REALSTCOK:
					actionInput.Add(Constants.TABLE_ORDER, new List<string>() { Constants.FIELD_ORDER_ORDER_STOCKRESERVED_STATUS });
					actionInput.Add(Constants.TABLE_ORDERITEM,
									new List<string>()
									{
										Constants.FIELD_ORDERITEM_ORDER_ITEM_NO,
										Constants.FIELD_ORDERITEM_ITEM_REALSTOCK_RESERVED
									});
					break;
			}

			return actionInput;
		}
		#endregion

		#region Write File
		/// <summary>
		/// Write History Data to file
		/// </summary>
		private void WriteHistoryFile()
		{
			string filePath = string.Format(@"{0}\OrderUpdateHistory\{1}", Constants.PHYSICALDIRPATH_OPERATION_UPDATEHISTORY_LOGFILE, this.TimeBegin.ToString("yyyyMMdd"));
			string fileName = string.Format("{0}_{1}", this.OrderId, this.TimeBegin.ToString("yyyyMMddHHmmss"));

			if (Directory.Exists(filePath) == false)
			{
				Directory.CreateDirectory(filePath);
			}

			WriteHistoryFile(GetTargetHistory(this.DataBefore), filePath, fileName, "0");
			WriteHistoryFile(GetTargetHistory(this.DataAfter), filePath, fileName, "1");
		}

		/// <summary>
		/// Write History Data to file
		/// </summary>
		/// <param name="historyData">History Data</param>
		/// <param name="filePath">File Path</param>
		/// <param name="fileName">File Name</param>
		/// <param name="fileIndex">File Index</param>
		private void WriteHistoryFile(XDocument historyData, string filePath, string fileName, string fileIndex)
		{
			if (historyData == null) return;

			historyData.Save(string.Format(@"{0}\{1}_{2}.xml", filePath, fileName, fileIndex));
		}

		/// <summary>
		/// Get History data
		/// </summary>
		/// <param name="historyData">History Data</param>
		/// <returns></returns>
		private XDocument GetTargetHistory(XElement historyData)
		{
			if (historyData == null) return null;

			return new XDocument(
					new XElement("OrderHistory",
					new XElement("action_type", this.Action),
					new XElement("action_user", this.OpearatorName),
					new XElement("action_detail", ((string.IsNullOrEmpty(this.ExtendAction) == false) ? (this.ExtendAction + ", ") : ""), StringUtility.RemoveUnavailableControlCode(this.ActionData)),
					historyData
				)
			);
		}
		#endregion

		#region 定数・列挙体・プロパティ
		/// <summary>
		/// Action of updating
		/// </summary>
		public enum ActionType
		{
			EcOrderConfirm,
			EcOrderModify,
			EcOrderWorkflow,
			FrontOrderCombine,
			FrontRecommend,
			FrontOrderCancel,
			FrontOrderConfirm
		}

		/// <summary>OrderId</summary>
		public string OrderId { get; set; }
		/// <summary>OpearatorName</summary>
		public string OpearatorName { get; set; }
		/// <summary>Action type</summary>
		public ActionType Action { get; set; }
		/// <summary>TimeBegin</summary>
		private DateTime TimeBegin { get; set; }
		/// <summary>SqlAccessor</summary>
		public SqlAccessor Accessor { get; set; }
		/// <summary>ExtendAction</summary>
		public string ExtendAction { get; set; }
		/// <summary>UpdateAction</summary>
		public Hashtable UpdateAction { get; set; }
		/// <summary>DataBefore</summary>
		private XElement DataBefore { get; set; }
		/// <summary>DataAfter</summary>
		private XElement DataAfter { get; set; }
		/// <summary>Action data</summary>
		private string ActionData { get; set; }
		/// <summary>Need Update Action</summary>
		private bool IsNeedAction { get; set; }
		#endregion
	}
}
