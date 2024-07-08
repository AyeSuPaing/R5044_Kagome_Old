/*
=========================================================================================================
  Module      : 受注情報ステータスの一括更新クラス(ImportOrderStatus.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using w2.App.Common.Order.Import.OrderImport.NecessaryCheck;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.Input.Order;
using System.Diagnostics;

namespace w2.App.Common.Order.Import
{
	/// <summary>
	/// 受注情報ステータスの一括更新
	/// </summary>
	public class ImportOrderStatus : ImportBase
	{
		/// <summary>Xmlセットファイル</summary>
		private static ImportSetting _xmlSetting;
		/// <summary>必須チェック</summary>
		private NecessaryCheckBase _necessaryCheck;
		/// <summary>CSVヘッダー</summary>
		private string[] _headerField;
		/// <summary>注文ステータス日時</summary>
		private const string CONST_ORDER_ORDER_STATUS_DATE = "order_status_date";
		/// <summary>ステータス定数</summary>
		private const string CONST_STATUS = "status";
		/// <summary>日時定数</summary>
		private const string CONST_DATE = "date";

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="fileType">ファイルタイプ</param>
		public ImportOrderStatus(string fileType)
		{
			GetImportSetting(fileType);
			_necessaryCheck = new NecessaryCheckUpdateOrderStatus();
		}

		/// <summary>
		/// 設定を取得する
		/// </summary>
		/// <param name="fileType">ファイルタイプ</param>
		private void GetImportSetting(string fileType)
		{
			try
			{
				// 設定ファイルをロード
				var mainElement = XElement.Load(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER + Constants.FILE_XML_ORDERFILEIMPORT_SETTING);
				var serviceNode =
					from serviceNodes in mainElement.Elements("OrderFile")
					where (serviceNodes.Element("Value").Value == fileType)
					select new
					{
						importFileSetting = serviceNodes.Elements("ImportFileSetting")
							.ToDictionary(node => node.Attribute("key").Value, node => node.Attribute("value").Value)
					};

				// 基本的な設定を読む
				var importFileSettings = serviceNode.First().importFileSetting;
				_xmlSetting = new ImportSetting
				{
					HeaderRowCount = int.Parse(importFileSettings["HeaderRowCount"]),
					FooterRowCount = int.Parse(importFileSettings["FooterRowCount"])
				};
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// インポート実行
		/// </summary>
		/// <param name="sr">テキストリーダー</param>
		/// <param name="loginOperatorName">操作者</param>
		/// <param name="updateHistoryAction">動作</param>
		/// <returns>実行結果</returns>
		public override bool Import(StreamReader sr, string loginOperatorName, UpdateHistoryAction updateHistoryAction)
		{
			var csvData = ReadCsvData(sr);

			// 検証失敗したら、エラーを表示、実行しない
			if (string.IsNullOrEmpty(m_strErrorMessage) == false)
			{
				m_successOrderInfos.Clear();
				return false;
			}

			// 更新実行、更新失敗場合は注文IDをエラーメッセージに保存する
			UpdateOrderStatus(csvData, loginOperatorName, updateHistoryAction);
			return true;
		}

		/// <summary>
		/// CSVデータを読込
		/// </summary>
		/// <param name="fileStream">ファイルストリーム</param>
		/// <returns>CSVデータ</returns>
		private List<Dictionary<string, string>> ReadCsvData(StreamReader fileStream)
		{
			var datas = new List<Dictionary<string, string>>();
			var currentLine = 0;
			var updateFailedOrder = new StringBuilder();
			while (fileStream.Peek() != -1)
			{
				currentLine++;
				var lineBuffer = fileStream.ReadLine();
				var csvData = StringUtility.SplitCsvLine(lineBuffer);

				// ヘッダー
				if (currentLine == 1)
				{
					// 注文ID必須検証
					if (_necessaryCheck.Check(csvData) == false)
					{
						m_strErrorMessage = MessageManager
							.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NECCESARY_FIELD_LACK);
						return datas;
					}

					if (CheckDuplicatedColumn(csvData))
					{
						m_strErrorMessage = MessageManager
							.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_FIELD_OVERLAPPING);
						return datas;
					}

					var errorColumnName = CheckColumnName(csvData);
					if (errorColumnName.Length > 0)
					{
						var errorMessages = new StringBuilder();
						foreach (var column in errorColumnName)
						{
							errorMessages.AppendLine(
								ImportMessage.GetMessages(
									ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_STATUS_WRONG_FIELDNAME)
								.Replace("@@ 1 @@", column));
						}

						m_strErrorMessage = errorMessages.ToString();
						return datas;
					}
					_headerField = csvData;
					continue;
				}

				// ヘッダー以外の行にOrderID入力検証 空ならエラー
				if (string.IsNullOrEmpty(csvData[Array.IndexOf(_headerField, Constants.FIELD_ORDER_ORDER_ID)]))
				{
					updateFailedOrder.AppendLine(
						ImportMessage.GetMessages(
							ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_NOORDER_ID));
				}

				var errorInputDataMessage = CheckInputFormat(csvData);
				if (string.IsNullOrEmpty(errorInputDataMessage) == false)
				{
					updateFailedOrder.AppendLine(
						ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_STATUS_WRONG_CONTENT_FORMAT)
						.Replace("@@ 1 @@", currentLine.ToString())
						.Replace("@@ 2 @@", errorInputDataMessage));
				}

				// 検証完了
				var dataWithFields = new Dictionary<string, string>();
				for (var i = 0; i < _headerField.Length; i++)
				{
					dataWithFields.Add(_headerField[i], csvData[i]);
				}
				dataWithFields.Add("line_no", currentLine.ToString());
				datas.Add(dataWithFields);
			}

			if (datas.Count == 0)
			{
				updateFailedOrder.AppendLine(
						ImportMessage.GetMessages(
							ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_STATUS_NO_ROW));
			}

			m_strErrorMessage = updateFailedOrder.ToString();
			return datas;
		}

		/// <summary>
		/// ステータスの更新処理
		/// </summary>
		/// <param name="datas">入力データ</param>
		/// <param name="loginOperatorName">操作者</param>
		/// <param name="updateHistoryAction">動作</param>
		private void UpdateOrderStatus(
			List<Dictionary<string, string>> datas,
			string loginOperatorName,
			UpdateHistoryAction updateHistoryAction)
		{

			var updateFailedOrder = new StringBuilder();
			var count = 0;
			var orderService = new OrderService();
			var defaultDateTime = DateTime.Now;
			var process = new ProcessAfterUpdateOrderStatus();
			foreach (var data in datas)
			{
				var orderModel = orderService.Get(data[Constants.FIELD_ORDER_ORDER_ID]);
				if (orderModel == null)
				{
					updateFailedOrder.AppendLine(
						ImportMessage
							.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_STATUS_WRONG_ORDER_ID)
							.Replace("@@ 1 @@", data["line_no"])
							.Replace("@@ 2 @@", data[Constants.FIELD_ORDER_ORDER_ID]));
				}
				else if ((orderModel.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_ORDER_CANCELED)
					|| (orderModel.OrderStatus == Constants.FLG_ORDER_ORDER_STATUS_TEMP_CANCELED))
				{
					updateFailedOrder.AppendLine(
						ImportMessage
							.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_STATUS_CANCELED_ORDER_ID)
							.Replace("@@ 1 @@", data["line_no"])
							.Replace("@@ 2 @@", data[Constants.FIELD_ORDER_ORDER_ID]));
				}
				else
				{
					using (var accessor = new SqlAccessor())
					{
						accessor.OpenConnection();
						accessor.BeginTransaction();
						var orderData = SetOrderStatusData(data);
						var hasUpdateOrderStatus = orderData.ContainsKey(Constants.FIELD_ORDER_ORDER_STATUS);
						var hasUpdatePaymentStatus = orderData.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS);

						var updated = orderService.Modify(
							orderData[Constants.FIELD_ORDER_ORDER_ID],
							order =>
							{
								if (hasUpdateOrderStatus)
								{
									// 注文ステータス更新日時定義
									var changeDate = orderData.ContainsKey(CONST_ORDER_ORDER_STATUS_DATE)
										? CastDatetime(orderData[CONST_ORDER_ORDER_STATUS_DATE])
										: defaultDateTime;
									order.OrderStatus = orderData[Constants.FIELD_ORDER_ORDER_STATUS];

									// 該当の注文ステータスの更新日時を設定。
									switch (orderData[Constants.FIELD_ORDER_ORDER_STATUS])
									{
										case Constants.FLG_ORDER_ORDER_STATUS_ORDER_RECOGNIZED:
											order.OrderRecognitionDate = changeDate;
											break;

										case Constants.FLG_ORDER_ORDER_STATUS_STOCK_RESERVED:
											order.OrderStockreservedDate = changeDate;
											break;

										case Constants.FLG_ORDER_ORDER_STATUS_SHIP_ARRANGED:
											order.OrderShippingDate = changeDate;
											break;

										case Constants.FLG_ORDER_ORDER_STATUS_SHIP_COMP:
											order.OrderShippedDate = changeDate;
											break;

										case Constants.FLG_ORDER_ORDER_STATUS_DELIVERY_COMP:
											order.OrderDeliveringDate = changeDate;
											break;
									}
								}

								// 入金ステータスを設定
								if (hasUpdatePaymentStatus)
								{
									order.OrderPaymentStatus = orderData[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS];

									// 入金ステータスの更新日時を設定
									if (orderData.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_DATE))
									{
										order.OrderPaymentDate =
											CastDatetime(orderData[Constants.FIELD_ORDER_ORDER_PAYMENT_DATE]);
									}
								}

								// 督促ステータスを設定
								if (orderData.ContainsKey(Constants.FIELD_ORDER_DEMAND_STATUS))
								{
									order.DemandStatus = orderData[Constants.FIELD_ORDER_DEMAND_STATUS];

									//督促ステータスの更新日時を設定
									if (orderData.ContainsKey(Constants.FIELD_ORDER_DEMAND_DATE))
									{
										order.DemandDate = CastDatetime(orderData[Constants.FIELD_ORDER_DEMAND_DATE]);
									}
								}

								// 拡張ステータス1～50項目の設定
								foreach (var item in orderData)
								{
									if (Regex.IsMatch(item.Key, @"extend_status\d+")
										&& (string.IsNullOrEmpty(item.Value) == false))
									{
										order.DataSource[item.Key] = item.Value;
										var dateFieldName = "extend_status_date" + Regex.Match(item.Key, @"\d+").Value;

										// 拡張ステータス1～50項目の更新日時を設定
										if (orderData.ContainsKey(dateFieldName))
										{
											order.DataSource[dateFieldName] = string.IsNullOrEmpty(orderData[dateFieldName])
												? defaultDateTime.ToString("yyyy/MM/dd HH:mm:ss")
												: orderData[dateFieldName];
										}
									}
								}
								order.LastChanged = loginOperatorName;
								order.DateChanged = defaultDateTime;
							},
							updateHistoryAction,
							accessor);
						count = updated;

						if (0 < updated)
						{
							var orderInput = new OrderInput(orderModel);

							// 受注情報ステータス更新時処理
							if (orderData.ContainsKey(Constants.FIELD_ORDER_ORDER_STATUS))
							{
								// ステータス更新による請求書印字処理
								var errorMessage = process.UpdatedInvoiceByOrderStatus(
									orderInput,
									Constants.StatusType.Order,
									orderData[Constants.FIELD_ORDER_ORDER_STATUS],
									accessor);
								if (string.IsNullOrEmpty(errorMessage) == false)
								{
									updateFailedOrder.AppendLine(errorMessage);
								}

								// ステータス更新による定期台帳処理
								process.UpdateFixedPurchaseByOrderStatus(
									updated,
									Constants.StatusType.Order,
									orderData[Constants.FIELD_ORDER_ORDER_STATUS],
									orderInput,
									loginOperatorName,
									accessor);

								// ステータス更新による仮ポイント→本ポイントへ変更
								process.UpdateTempPointToRealPointByOrderStatus(
									orderInput,
									Constants.StatusType.Order,
									orderData[Constants.FIELD_ORDER_ORDER_STATUS],
									accessor,
									orderInput.UserId,
									orderData[Constants.FIELD_ORDER_ORDER_ID],
									loginOperatorName);
							}

							// シリアルキー割り当てを行う
							if(orderData.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS)
								&& orderData.ContainsKey(Constants.FIELD_ORDER_ORDER_STATUS))
							{
								process.DeliverSerialKeyByUpdateStatus(
									hasUpdateOrderStatus || hasUpdatePaymentStatus,
									updated,
									orderModel.OrderId,
									orderModel.DigitalContentsFlg,
									hasUpdateOrderStatus
											? (string)orderData[Constants.FIELD_ORDER_ORDER_STATUS]
											: orderModel.OrderStatus,
									hasUpdatePaymentStatus
											? (string)orderData[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS]
											: orderModel.OrderPaymentStatus,
									loginOperatorName,
									accessor);
							}

							// 入金ステータス更新時処理
							if (orderData.ContainsKey(Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS))
							{
								// ステータス更新による定期購入会員判定条件支払い
								process.UpdateFixPurChaseMemberFlgByPaymentStatus(
									Constants.StatusType.Payment,
									orderData[Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS],
									orderInput,
									loginOperatorName,
									updateHistoryAction,
									accessor);
							}
						}
						accessor.CommitTransaction();
					}
				}
			}
			if (updateFailedOrder.Length > 0)
			{
				updateFailedOrder.Insert(
					0,
					string.Format(
						"{0}\r\n",
						MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_STATUS_WRONG_ORDER)));
			}
			m_strErrorMessage = updateFailedOrder.ToString();
			m_iUpdatedCount = count;
			m_iLinesCount = datas.Count;
		}

		/// <summary>
		/// 重複の列の検証
		/// </summary>
		/// <param name="data">列のデータ</param>
		/// <returns>検証結果</returns>
		private bool CheckDuplicatedColumn(string[] data)
		{
			var isDuplicated = data.GroupBy(name => name).Any(group => group.Count() > 1);
			return isDuplicated;
		}

		/// <summary>
		/// 注文ステータス列名検証
		/// </summary>
		/// <param name="data">列のデータ</param>
		/// <returns>検証結果</returns>
		private string[] CheckColumnName(string[] data)
		{
			var statusList = new string[7]
			{
				Constants.FIELD_ORDER_ORDER_ID,
				Constants.FIELD_ORDER_ORDER_STATUS,
				CONST_ORDER_ORDER_STATUS_DATE,
				Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS,
				Constants.FIELD_ORDER_ORDER_PAYMENT_DATE,
				Constants.FIELD_ORDER_DEMAND_STATUS,
				Constants.FIELD_ORDER_DEMAND_DATE
			};

			// 拡張ステータス1-50まで列名の検証
			var wrongFields = data
				.Where(item => ((statusList.Contains(item) == false)
					&& (Regex.IsMatch(
						item,
					@"^extend_status_date|extend_status(([1-4]{0,1}[1-9])|[1-5]0)$") == false)))
				.ToArray();
			return wrongFields;
		}

		/// <summary>
		/// 入力値のフォーマットを検証する
		/// </summary>
		/// <param name="data">注文データ</param>
		/// <returns>検証失敗のフィールド名</returns>
		private string CheckInputFormat(string[] data)
		{
			for (var i = 0; i < data.Length; i++)
			{
				if (_headerField[i].Contains("id"))
				{
					// 注文IDが返品、交換の場合エラーとする
					if (Regex.IsMatch(data[i], "-[0-9]{3}$")) return _headerField[i];
				}
				// 日付検証
				else if ((_headerField[i].Contains("date")))
				{
					// 空白を検証しない
					if (string.IsNullOrWhiteSpace(data[i])) continue;
					if (Validator.IsDate(data[i]) == false) return _headerField[i];
				}
				// ステータスの入力値のフォーマットを検証する
				else if (_headerField[i].Contains("status"))
				{
					// 設定値以外の値の入力検証
					if (ValueText.Exists(Constants.VALUETEXT_PARAM_FILE_IMPORT_ORDER_STATUS, _headerField[i]) == false) continue;
					var statusValue = ValueText.GetValueKvpArray(
						Constants.VALUETEXT_PARAM_FILE_IMPORT_ORDER_STATUS,
						_headerField[i]);
					if (statusValue.Any(item => (item.Key == data[i])) == false)
					{
						return _headerField[i];
					}
				}
			}
			return string.Empty;
		}

		/// <summary>
		/// 更新する受注ステータスをセット
		/// </summary>
		/// <param name="order">注文データ</param>
		/// <returns>変更完了データ</returns>
		private Dictionary<string, string> SetOrderStatusData(Dictionary<string, string> order)
		{
			var resultDict = new Dictionary<string, string>();
			foreach (var item in order)
			{
				// 設定XMLファイル中にステータスの設定はない場合スキップ
				if (item.Key.Contains(CONST_DATE) || item.Key.Contains(CONST_STATUS))
				{
					var statusKey = item.Key.Contains(Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME)
						? Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME
						: item.Key.Contains(CONST_ORDER_ORDER_STATUS_DATE)
							? Constants.FIELD_ORDER_ORDER_STATUS
							: item.Key.Replace(CONST_DATE, CONST_STATUS);

					if (ValueText.Exists(
						Constants.VALUETEXT_PARAM_FILE_IMPORT_ORDER_STATUS,
						statusKey) == false) continue;
				}

				switch (item.Key)
				{
					case Constants.FIELD_ORDER_ORDER_STATUS:
					case Constants.FIELD_ORDER_ORDER_PAYMENT_STATUS:
					case Constants.FIELD_ORDER_DEMAND_STATUS:
						resultDict.Add(
							item.Key,
							ValueText.GetValueText(
								Constants.VALUETEXT_PARAM_FILE_IMPORT_ORDER_STATUS,
								item.Key,
								item.Value));
						break;
					default:
						if (Regex.IsMatch(item.Key, @"extend_status\d+"))
						{
							resultDict.Add(
								item.Key,
								ValueText.GetValueText(
									Constants.VALUETEXT_PARAM_FILE_IMPORT_ORDER_STATUS,
									Constants.FIELD_ORDER_EXTEND_STATUS_BASENAME,
									item.Value));
						}
						else
						{
							resultDict.Add(item.Key, item.Value);
						}
						break;
				}
			}
			return resultDict;
		}

		/// <summary>
		/// 時間を返す、入力内容DateTimeへ変換失敗の場合は今の時間を返す
		/// </summary>
		/// <param name="dateString">日付データ文字列</param>
		private DateTime CastDatetime(string dateString)
		{
			if (string.IsNullOrEmpty(dateString)) return DateTime.Now;

			DateTime result;
			if (DateTime.TryParse(dateString, out result))
			{
				return result;
			}
			return DateTime.Now;
		}

		/// <summary>
		/// インポート設定クラス
		/// </summary>
		private class ImportSetting
		{
			/// <summary>ヘッダーの行数</summary>
			public int HeaderRowCount { get; set; }
			/// <summary>フッターの行数</summary>
			public int FooterRowCount { get; set; }
		}
	}
}
