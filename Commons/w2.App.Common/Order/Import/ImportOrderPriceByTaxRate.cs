/*
=========================================================================================================
  Module      : W2C標準 紐付けデータ取込モジュール(ImportOrderPriceByTaxRate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Order;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Import
{
	/// <summary>
	/// ImportOrderPriceByTaxRate の概要の説明です
	/// </summary>
	public class ImportOrderPriceByTaxRate : ImportBase
	{
		/// <summary>1行の項目数を設定</summary>
		private const int m_iColumnsCount = 8;

		/// <summary>
		/// インポート
		/// </summary>
		/// <param name="sr">csvファイルストリーム</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>取込結果</returns>
		/// <remarks>
		/// 紐付けデータを取込、税率毎価格情報を更新
		/// </remarks>
		public override bool Import(StreamReader sr, string loginOperatorName, UpdateHistoryAction updateHistoryAction)
		{
			//------------------------------------------------------
			// 税率毎価格情報更新処理
			//------------------------------------------------------
			var currentLine = 0;
			var orderPriceByTaxRateService = new OrderPriceByTaxRateService();
			var orderService = new OrderService();
			var tempData = new List<OrderPriceByTaxRateModel>();
			var tempOrderId = "";
			var targetOrder = new OrderModel();
			using (var accessor = new SqlAccessor())
			{

				accessor.OpenConnection();
				accessor.BeginTransaction();
				try
				{
					while (sr.Peek() != -1)
					{
						// 処理中行カウンタ＋１
						currentLine++;

						// フィールドに改行が含まれる場合を考慮して行を結合（CSVの行に改行がある場合、「"」は奇数個のはず）
						var lineBuffer = sr.ReadLine();
						while (((lineBuffer.Length - lineBuffer.Replace("\"", "").Length) % 2) != 0)
						{
							lineBuffer += "\r\n" + sr.ReadLine();
						}

						// １行をCSV分割・フィールド数が正しい項目数(m_iColumnsCount)と合っているかチェック
						var columnDataBuffer = StringUtility.SplitCsvLine(lineBuffer);
						if (m_iColumnsCount != columnDataBuffer.Length)
						{
							m_strErrorMessage = MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DEFINITION_MISMATCH)
								.Replace("@@ 1 @@", currentLine.ToString());
							m_strErrorMessage += MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DEFINITION_MISMATCH_EXPLANATION)
								.Replace("@@ 1 @@", m_iColumnsCount.ToString())
								.Replace("@@ 2 @@", columnDataBuffer.Length.ToString());
							accessor.RollbackTransaction();
							this.m_successOrderInfos.Clear();
							return false;
						}

						var currentData = new OrderPriceByTaxRateModel
						{
							OrderId = columnDataBuffer[0],
							KeyTaxRate = decimal.Parse(columnDataBuffer[1]),
							PriceSubtotalByRate = decimal.Parse(columnDataBuffer[2]),
							PriceShippingByRate = decimal.Parse(columnDataBuffer[3]),
							PricePaymentByRate = decimal.Parse(columnDataBuffer[4]),
							ReturnPriceCorrectionByRate = decimal.Parse(columnDataBuffer[5]),
							PriceTotalByRate = decimal.Parse(columnDataBuffer[6]),
							TaxPriceByRate = decimal.Parse(columnDataBuffer[7]),
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now
						};

						if (tempData.Count == 0)
						{
							tempData.Add(currentData);
							tempOrderId = currentData.OrderId;
						}
						// 前のデータと同じ注文ID
						else if (tempOrderId == currentData.OrderId)
						{
							tempData.Add(currentData);
						}
						// 前のデータと違う注文ID
						else if (tempOrderId != currentData.OrderId)
						{
							targetOrder = orderService.Get(tempOrderId, accessor);
							if (targetOrder == null)
							{
								m_strErrorMessage = MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_NO_FIND)
									.Replace("@@ 1 @@", currentLine.ToString())
									.Replace("@@ 2 @@", tempOrderId);
								accessor.RollbackTransaction();
								this.m_successOrderInfos.Clear();
								return false;
							}
							// 注文IDに紐づいた税率毎価格情報を削除して、読み込んだデータをインサート
							orderPriceByTaxRateService.DeleteAllByOrderId(tempOrderId, accessor);
							tempData.ForEach(data => orderPriceByTaxRateService.Insert(data, accessor));

							tempData.Clear();
							tempData.Add(currentData);
							tempOrderId = currentData.OrderId;
							m_iUpdatedCount += tempData.Count;
						}
					}

					targetOrder = orderService.Get(tempOrderId, accessor);
					if (targetOrder == null)
					{
						m_strErrorMessage = MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ORDER_NO_FIND)
							.Replace("@@ 1 @@", currentLine.ToString())
							.Replace("@@ 2 @@", tempOrderId);
						accessor.RollbackTransaction();
						this.m_successOrderInfos.Clear();
						return false;
					}
					// 注文IDに紐づいた税率毎価格情報を削除して、読み込んだデータをインサート
					orderPriceByTaxRateService.DeleteAllByOrderId(tempOrderId, accessor);
					tempData.ForEach(data => orderPriceByTaxRateService.Insert(data, accessor));
					m_iUpdatedCount += tempData.Count;
				}
				catch (Exception)
				{
					accessor.RollbackTransaction();
					m_strErrorMessage = MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_IRREGULAR_DATA)
						.Replace("@@ 1 @@", currentLine.ToString())
						.Replace("@@ 2 @@", tempOrderId);
					this.m_successOrderInfos.Clear();
					return false;
				}
				accessor.CommitTransaction();
				// 処理行数
				m_iLinesCount = currentLine;
			}

			return true;
		}
	}
}
