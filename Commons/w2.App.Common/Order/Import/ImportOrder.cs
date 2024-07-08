/*
=========================================================================================================
  Module      : 注文関連ファイル取込処理(ImportOrder.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Collections;
using System.IO;
using System.Linq;
using System.Text;
using w2.App.Common.Option;
using w2.App.Common.Order.Import.OrderImport.Entity;
using w2.App.Common.Order.Import.OrderImport.ImportAction;
using w2.App.Common.Order.Import.OrderImport.NecessaryCheck;
using w2.App.Common.Order.Payment;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Import
{
	/// <summary>
	/// 注文関連ファイル取込処理
	/// </summary>
	public class ImportOrder
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="serviceId">取込ファイル種別</param>
		public ImportOrder(string serviceId)
		{
			var tmp = serviceId.Split(':');

			this.ImportKbn = tmp[1];
			this.NecessaryCheck = CreateNecessaryCheck();
		}

		/// <summary>
		/// インポート
		/// </summary>
		/// <param name="stream">ストリーム</param>
		/// <param name="operatorName">オペレーター名</param>
		/// <returns>取込結果</returns>
		public bool Import(StreamReader stream, string operatorName)
		{
			var lineBuffer = string.Empty;
			var lineCount = 0;

			OrderData tmpOrder = null;
			var tmpCount = 0;

			while (stream.Peek() >= 0)
			{
				lineCount++;

				// フィールドに改行が含まれる場合を考慮して行を結合（CSVの行に改行がある場合、「"」は奇数個のはず）
				lineBuffer = stream.ReadLine();
				while (((lineBuffer.Length - lineBuffer.Replace("\"", "").Length) % 2) != 0)
				{
					lineBuffer += "\r\n" + stream.ReadLine();
				}
				var csvData = StringUtility.SplitCsvLine(lineBuffer);

				// ヘッダー処理
				if (lineCount == 1)
				{
					this.Header = csvData;

					if (this.NecessaryCheck.Check(this.Header) == false)
					{
						this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NECCESARY_FIELD_LACK));
						this.ErrorMessage.Append(this.NecessaryCheck.NotFoundField);
						return false;
					}

					// 重複項目チェック
					if (CheckDuplicatedField())
					{
						return false;
					}

					continue;
				}

				// 注文データ処理
				var current = new OrderData(GetHashData(csvData));

				// 行のフィールド数が定義と一致されるかのチェック
				if (csvData.Length != this.Header.Length)
				{
					current.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DEFINITION_MISMATCH).Replace("@@ 1 @@", lineCount.ToString()));
				}

				if (tmpOrder == null)
				{
					tmpOrder = current;
					tmpCount = lineCount;
				}
				// 前のデータと同じ注文ID
				else if (tmpOrder.OrderId == current.OrderId)
				{
					tmpOrder.CsvOrderData.Add(current.CsvOrderData[0]);
					if (current.ErrorMessage.Length != 0) tmpOrder.ErrorMessage.Append(current.ErrorMessage);
				}
				// 前のデータと違う注文ID
				else if (tmpOrder.OrderId != current.OrderId)
				{
					ExecImport(tmpCount, tmpOrder, operatorName);
					
					this.OrderCount++;

					tmpOrder = current;
					tmpCount = lineCount;
				}
			}

			ExecImport(tmpCount, tmpOrder, operatorName);

			this.OrderCount++;
			this.LineCount = lineCount - 1;

			return (this.ErrorMessage.Length == 0);
		}

		/// <summary>
		/// データインポート実行
		/// </summary>
		/// <param name="lineCount">行数</param>
		/// <param name="order">注文データ</param>
		/// <param name="operatorName">オペレーター名</param>
		/// <returns>成否</returns>
		private bool ExecImport(int lineCount, OrderData order, string operatorName)
		{
			if (ExecImportAction(lineCount, order) == false)
			{
				if (string.IsNullOrEmpty(this.ApiErrorMessages) == false)
				{
					OrderCommon.AppendExternalPaymentCooperationLog(
						false,
						order.OrderId,
						this.ApiErrorMessages,
						operatorName,
						UpdateHistoryAction.Insert);
				}

				return false;
			}
			
			OrderCommon.AppendExternalPaymentCooperationLog(
					true,
					order.OrderId,
					LogCreator.CreateMessage(order.OrderId, ""),
					operatorName,
					UpdateHistoryAction.Insert);
			this.ImportCount++;

			return true;
		}

		/// <summary>
		/// 注文Hashtableデータ取得
		/// </summary>
		/// <param name="csvData">csvデータ</param>
		/// <returns>注文Hashtableデータ</returns>
		private Hashtable GetHashData(string[] csvData)
		{
			var result = new Hashtable();

			for (var i = 0; i < csvData.Length; i++)
			{
				result.Add(this.Header[i], csvData[i]);
			}

			// 項目補完
			if (result.ContainsKey(Constants.FIELD_ORDER_FIXED_PURCHASE_ID) == false)
			{
				result.Add(Constants.FIELD_ORDER_FIXED_PURCHASE_ID, "");
			}
			if (result.ContainsKey(Constants.FIELD_ORDER_LAST_CHANGED) == false)
			{
				result.Add(Constants.FIELD_ORDER_LAST_CHANGED, Constants.FLG_LASTCHANGED_BATCH);
			}
			if (result.ContainsKey(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_ORG) == false)
			{
				result.Add(Constants.FIELD_ORDERITEM_PRODUCT_PRICE_ORG, result[Constants.FIELD_ORDERITEM_PRODUCT_PRICE]);
			}
			if (result.ContainsKey(Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG) == false)
			{
				result.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_INCLUDED_FLG, TaxCalculationUtility.GetPrescribedOrderItemTaxIncludedFlag());
			}
			if (result.ContainsKey(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE) == false)
			{
				result.Add(Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE, result[Constants.FIELD_ORDERITEM_PRODUCT_TAX_RATE]);
			}
			if (result.ContainsKey(Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE) == false)
			{
				result.Add(Constants.FIELD_ORDERITEM_ITEM_PRICE_SINGLE, result[Constants.FIELD_ORDERITEM_ITEM_PRICE]);
			}
			if (result.ContainsKey(Constants.FIELD_ORDER_RECEIPT_FLG) == false)
			{
				result.Add(Constants.FIELD_ORDER_RECEIPT_FLG, Constants.FLG_ORDER_RECEIPT_FLG_OFF);
			}
			if (result.ContainsKey(Constants.FIELD_ORDER_RECEIPT_ADDRESS) == false)
			{
				result.Add(Constants.FIELD_ORDER_RECEIPT_ADDRESS, string.Empty);
			}
			if (result.ContainsKey(Constants.FIELD_ORDER_RECEIPT_PROVISO) == false)
			{
				result.Add(Constants.FIELD_ORDER_RECEIPT_PROVISO, string.Empty);
			}

			// 領収書対応OPがOFFの場合、初期値とする
			if (Constants.RECEIPT_OPTION_ENABLED == false)
			{
				result[Constants.FIELD_ORDER_RECEIPT_FLG] = Constants.FLG_ORDER_RECEIPT_FLG_OFF;
				result[Constants.FIELD_ORDER_RECEIPT_ADDRESS] = string.Empty;
				result[Constants.FIELD_ORDER_RECEIPT_PROVISO] = string.Empty;
			}

			return result;
		}

		/// <summary>
		/// 項目重複チェック
		/// </summary>
		/// <returns>重複項目があるか？</returns>
		private bool CheckDuplicatedField()
		{
			var duplicatedField = this.Header.Where(item => this.Header.Where(item2 => item2 == item).ToArray().Length > 1).Distinct().ToArray();

			if (duplicatedField.Length > 0)
			{
				this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_FIELD_OVERLAPPING));
				this.ErrorMessage.Append(string.Join(",", duplicatedField));
			}

			return (duplicatedField.Length > 0);
		}

		/// <summary>
		/// 登録処理
		/// </summary>
		/// <param name="lineCount">行数</param>
		/// <param name="order">注文データ</param>
		/// <returns>成否</returns>
		private bool ExecImportAction(int lineCount, OrderData order)
		{
			// 既にエラーがあれば注文を作らない
			if (order.ErrorMessage.Length != 0)
			{
				this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_IRREGULAR_DATA).Replace("@@ 1 @@", lineCount.ToString()).Replace("@@ 2 @@", order.OrderId));
				this.ErrorMessage.Append(order.ErrorMessage);
				return false;
			}

			var importActionOrder = CreateImportAction(order);

			// 取り込み処理クラス作成時エラーチェック
			if (importActionOrder.ErrorMessage.Length != 0)
			{
				this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_IRREGULAR_DATA).Replace("@@ 1 @@", lineCount.ToString()).Replace("@@ 2 @@", order.OrderId));
				this.ErrorMessage.Append(importActionOrder.ErrorMessage);
				return false;
			}

			// データチェック
			if (order.CheckData() == false)
			{
				this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_IRREGULAR_DATA).Replace("@@ 1 @@", lineCount.ToString()).Replace("@@ 2 @@", order.OrderId));
				this.ErrorMessage.Append(order.ErrorMessage);
				return false;
			}

			// 重複注文チェック
			var duplicated = new OrderService().Get(order.OrderId);
			// すでに該当注文IDが登録されている場合
			if (duplicated != null)
			{
				this.DuplicatedErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_REGISTERED_ORDER_DATA).Replace("@@ 1 @@", lineCount.ToString()).Replace("@@ 2 @@", order.OrderId));
				return false;
			}

			// 注文登録処理
			importActionOrder.Import();

			if (importActionOrder.ErrorMessage.Length > 0)
			{
				this.ErrorMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_DATA_ERROR).Replace("@@ 1 @@", lineCount.ToString()).Replace("@@ 2 @@", order.OrderId));
				this.ErrorMessage.Append(importActionOrder.ErrorMessage);
				this.ApiErrorMessages = importActionOrder.ErrorMessage.ToString();
				return false;
			}

			this.OrderItemImportCount += importActionOrder.OrderItemImportCount;
			this.FixedPurchaseRegistCount += importActionOrder.FixedPurchaseRegistCount;

			return true;
		}

		/// <summary>
		/// 必須チェッククラス作成
		/// </summary>
		/// <returns>必須チェッククラス</returns>
		private NecessaryCheckBase CreateNecessaryCheck()
		{
			switch (this.ImportKbn)
			{
				case "neworder":
					return new NecessaryCheckNewOrder();

				case "migration":
					return new NecessaryCheckMigration();

				default:
					return null;
			}
		}

		/// <summary>
		/// 取り込み処理クラス作成
		/// </summary>
		/// <param name="order">注文データ</param>
		/// <returns>取り込み処理クラス</returns>
		private ImportActionBase CreateImportAction(OrderData order)
		{
			switch (this.ImportKbn)
			{
				case "neworder":
					return new ImportActionNewOrder(order);

				case "migration":
					return new ImportActionMigration(order);

				default:
					return null;
			}
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <returns>エラーメッセージ</returns>
		public string GetErrorMessage()
		{
			var result = new StringBuilder();

			if (this.ErrorMessage.Length > 0)
			{
				result.AppendLine("■エラー内容");
				result.AppendLine(this.ErrorMessage.ToString());
			}

			if ((result.Length > 0) && (this.DuplicatedErrorMessage.Length > 0))
			{
				result.AppendLine();
				result.AppendLine();
			}

			if (this.DuplicatedErrorMessage.Length > 0)
			{
				result.AppendLine("■重複内容");
				result.AppendLine(this.DuplicatedErrorMessage.ToString());
			}

			return result.ToString();
		}

		/// <summary>取り込み区分</summary>
		private string ImportKbn { get; set; }
		/// <summary>ヘッダー</summary>
		private string[] Header { get; set; }
		/// <summary>必須チェック</summary>
		private NecessaryCheckBase NecessaryCheck { get; set; }
		/// <summary>エラーメッセージ</summary>
		private StringBuilder ErrorMessage { get { return this.m_errorMessage; } }
		private readonly StringBuilder m_errorMessage = new StringBuilder();
		/// <summary>重複注文エラーメッセージ</summary>
		private StringBuilder DuplicatedErrorMessage { get { return this.m_duplicatedErrorMessage; } }
		private readonly StringBuilder m_duplicatedErrorMessage = new StringBuilder();
		/// <summary>注文全体数</summary>
		public int OrderCount { get; private set; }
		/// <summary>注文生成数</summary>
		public int ImportCount { get; private set; }
		/// <summary>全体行数</summary>
		public int LineCount { get; private set; }
		/// <summary>注文アイテム生成数</summary>
		public int OrderItemImportCount { get; private set; }
		/// <summary>定期台帳生成数</summary>
		public int FixedPurchaseRegistCount { get; private set; }
		/// <summary>APIエラーメッセージ</summary>
		public string ApiErrorMessages { get; private set; }
	}
}
