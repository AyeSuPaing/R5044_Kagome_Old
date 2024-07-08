/*
=========================================================================================================
  Module      : 宅配通配送実績データ取込モジュール(ImportPelicanResultReportLink.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.DeliveryCompany.Helper;
using w2.Domain.Order;
using w2.Domain.UpdateHistory;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Order.Import
{
	/// <summary>
	/// 宅配通配送実績データ取込クラス
	/// </summary>
	public class ImportPelicanResultReportLink : ImportBase
	{
		/// <summary>1行の必要バイト数</summary>
		private const int BYTE_LENGTH_MAX = 564;

		/// <summary>
		/// インポート
		/// </summary>
		/// <param name="sr">csvファイルストリーム</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>取込結果</returns>
		/// <remarks>
		/// 宅配通配送実績データを取込、配送伝票番号を更新
		/// </remarks>
		public override bool Import(StreamReader sr, string loginOperatorName, UpdateHistoryAction updateHistoryAction)
		{
			var lineBuffer = string.Empty;

			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("OrderFileImport", "UpdateOrderLastChanged"))
			{
				try
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					// 各行を読み取る
					while (sr.Peek() != -1)
					{
						lineBuffer = sr.ReadLine();

						// 「Total:」が含まれていれば、処理を抜ける
						if (lineBuffer.Contains("Total:")) continue;

						// 処理中行カウンタ＋１
						m_iLinesCount++;

						// フィールドに改行が含まれる場合を考慮して行を結合（CSVの行に改行がある場合、「"」は奇数個のはず）
						while (((lineBuffer.Length - lineBuffer.Replace("\"", string.Empty).Length) % 2) != 0)
						{
							lineBuffer += Environment.NewLine + sr.ReadLine();
						}

						// 1行のバイト数が満たされているかチェックし、エラーが出たらnullを返す
						var encoding = Encoding.GetEncoding(950);
						if (encoding.GetBytes(lineBuffer).Length < BYTE_LENGTH_MAX)
						{
							m_strErrorMessage = string.Format(
								ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_BYTE_LENGTH_ERROR),
								m_iLinesCount,
								BYTE_LENGTH_MAX);
							m_successOrderInfos.Clear();
							return (m_iUpdatedCount > 0);
						}

						// 行バッファから配送実績データを取得
						var resultReport = new PelicanResultReport(lineBuffer, encoding);

						// 注文情報取得
						var order = new OrderService().Get(resultReport.OrderId, accessor);
						if (order != null)
						{
							// 出荷情報登録（外部連携）
							if (this.ExecExternalShipmentEntry)
							{
								var canShipmentEntry = false;
								var errorMessage = ExternalShipmentEntry(
									order.OrderId,
									order.PaymentOrderId,
									order.OrderPaymentKbn,
									order.Shippings[0].ShippingCheckNo,
									resultReport.ShippingCheckNo,
									order.CardTranId,
									DeliveryCompanyUtil.GetDeliveryCompanyType(
										order.Shippings[0].DeliveryCompanyId,
										order.OrderPaymentKbn),
									UpdateHistoryAction.DoNotInsert,
									out canShipmentEntry);
								if (string.IsNullOrEmpty(errorMessage) == false)
								{
									m_strErrorMessage += errorMessage + "<br/>";
									continue;
								}

								if (canShipmentEntry)
								{
									// 決済連携メモ追記
									new OrderService().AddPaymentMemo(
										order.OrderId,
										OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
											string.IsNullOrEmpty(order.PaymentOrderId)
												? order.OrderId
												: order.PaymentOrderId,
											order.OrderPaymentKbn,
											order.CardTranId,
											Constants.ACTION_NAME_SHIPPING_REPORT,
											order.LastBilledAmount),
										loginOperatorName,
										UpdateHistoryAction.DoNotInsert,
										accessor);
								}
							}

							// 更新処理
							var updated = UpdateOrderShipping(resultReport, accessor);
							m_iUpdatedCount += updated;

							if (updated != 0)
							{
								var input = new Hashtable
								{
									{ Constants.FIELD_ORDERSHIPPING_ORDER_ID, resultReport.OrderId },
									{ Constants.FIELD_ORDER_LAST_CHANGED, loginOperatorName }
								};

								statement.ExecStatement(accessor, input);

								m_successOrderInfos.Add(
									new SuccessInfo(m_iLinesCount, resultReport.OrderId, order.Shippings[0].ShippingCheckNo));

								// 更新履歴登録
								if (updateHistoryAction == UpdateHistoryAction.Insert)
								{
									new UpdateHistoryService().InsertForOrder(
										(string)input[Constants.FIELD_ORDERSHIPPING_ORDER_ID],
										loginOperatorName,
										accessor);
								}
							}
							else
							{
								AppLogger.WriteError(
									string.Format(
										ImportMessage.GetMessages(
											ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_UPDATE_SHIPPING_CHECK_NO_ERROR),
										m_iLinesCount,
										resultReport.OrderId));

							}
						}

						accessor.CommitTransaction();
					}
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					m_successOrderInfos.Clear();
					throw;
				}
			}

			if (string.IsNullOrEmpty(m_strErrorMessage) == false)
			{
				m_strErrorMessage += "<br/>" + ImportMessage.GetMessages(
					ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_APPROPRIATE_ERROR_EXIST);
			}

			return (m_iUpdatedCount > 0);
		}

		/// <summary>
		/// 配送先情報更新
		/// </summary>
		/// <param name="resultData">配送実績データ</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		private int UpdateOrderShipping(PelicanResultReport resultData, SqlAccessor accessor)
		{
			var updated = 0;
			var orderService = new OrderService();

			// 配送先情報取得
			var orderShippingNo = 1;
			var orderShipping = orderService.GetShipping(resultData.OrderId, orderShippingNo, accessor);

			// 取り出したデータのチェックを行う
			var checkErrorMessage = CheckResultData(resultData);

			if ((orderShipping != null) && string.IsNullOrEmpty(checkErrorMessage))
			{
				orderShipping.ShippingStatusCode = resultData.PelicanStatusCode;
				orderShipping.ShippingStatus = resultData.PelicanShippingStatus.Replace("0", "");
				orderShipping.ShippingOfficeName = resultData.PelicanOfficeName;
				orderShipping.ShippingCurrentStatus = resultData.PelicanStatus;
				orderShipping.ShippingHandyTime = resultData.PelicanHandyTime;
				orderShipping.ShippingStatusUpdateDate = DateTime.ParseExact(resultData.PelicanCreateDate, "yyyyMMdd", null);
				orderShipping.ShippingCheckNo = resultData.ShippingCheckNo;
				orderShipping.ShippingStatusDetail = resultData.PelicanStatusDetail;

				updated = orderService.UpdateOrderShipping(orderShipping, accessor);
			}
			else if (string.IsNullOrEmpty(checkErrorMessage) == false)
			{
				m_strErrorMessage += string.Format(
					ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_ROW_AND_ORDER_ID),
					m_iLinesCount,
					resultData.OrderId,
					checkErrorMessage);
			}

			return updated;
		}

		/// <summary>
		/// 取り出した配送実績データのチェック
		/// </summary>
		/// <param name="resultReport">配送実績データ</param>
		/// <returns>エラーメッセージ</returns>
		private string CheckResultData(PelicanResultReport resultReport)
		{
			var successDate = DateTime.MinValue;

			// データ生成日が日付形式かチェック
			if (DateTime.TryParseExact(
				resultReport.PelicanCreateDate,
				"yyyyMMdd",
				null,
				DateTimeStyles.AssumeLocal,
				out successDate) == false)
			{
				return MessageManager.GetMessages(
					MessageManager.INPUTCHECK_DATE).Replace("@@ 1 @@", "データ生成日");
			}

			// 完了状態コードが値として存在しているかチェック
			if (ShippingStatusCodeValueItem.ContainsKey(
				resultReport.PelicanStatusCode) == false)
			{
				return MessageManager.GetMessages(
					MessageManager.INPUTCHECK_REGEXP2).Replace("@@ 1 @@", "完了状態コード");
			}

			// 配送状態IDが値として存在しているかチェック
			if (ShippingStatusValueItem.ContainsKey(
				resultReport.PelicanShippingStatus.Replace("0", "")) == false)
			{
				return MessageManager.GetMessages(
					MessageManager.INPUTCHECK_REGEXP2).Replace("@@ 1 @@", "配送状態ID");
			}

			// Handy操作時間が日付形式かチェック
			if (DateTime.TryParseExact(
				resultReport.PelicanHandyTime,
				"yyyyMMddHHmmss",
				null,
				DateTimeStyles.AssumeLocal,
				out successDate) == false)
			{
				return MessageManager.GetMessages(
					MessageManager.INPUTCHECK_DATE).Replace("@@ 1 @@", "Handy操作時間");
			}

			// 現在の状態が値として存在しているかチェック
			if (ShippingCurrentStatusValueItem.ContainsKey(
				resultReport.PelicanStatus) == false)
			{
				return MessageManager.GetMessages(
					MessageManager.INPUTCHECK_REGEXP2).Replace("@@ 1 @@", "現在の状態");
			}

			return string.Empty;
		}

		/// <summary>完了状態コードのDictionary</summary>
		private Dictionary<string, string> ShippingStatusCodeValueItem
		{
			get
			{
				return ValueText.GetValueKvpArray(
					Constants.TABLE_ORDERSHIPPING,
					Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS_CODE).ToDictionary(x => x.Key, x => x.Value);
			}
		}
		/// <summary>配送状態IDのDictionary</summary>
		private Dictionary<string, string> ShippingStatusValueItem
		{
			get
			{
				return ValueText.GetValueKvpArray(
					Constants.TABLE_ORDERSHIPPING,
					Constants.FIELD_ORDERSHIPPING_SHIPPING_STATUS).ToDictionary(x => x.Key, x => x.Value);
			}
		}
		/// <summary>現在の状態のDictionary</summary>
		private Dictionary<string, string> ShippingCurrentStatusValueItem
		{
			get
			{
				return ValueText.GetValueKvpArray(
					Constants.TABLE_ORDERSHIPPING,
					Constants.FIELD_ORDERSHIPPING_SHIPPING_CURRENT_STATUS).ToDictionary(x => x.Key, x => x.Value);
			}
		}

		/// <summary>
		/// 配送実績データクラス
		/// </summary>
		public class PelicanResultReport
		{
			/// <summary>
			/// コンストラクタ
			/// </summary>
			/// <param name="lineBuffer">行バッファ</param>
			/// <param name="encoding">エンコード</param>
			public PelicanResultReport(string lineBuffer, Encoding encoding)
			{
				this.ShippingCheckNo = StringUtility.GetByteLengthString(lineBuffer, 11, 22, encoding);
				this.OrderId = StringUtility.GetByteLengthString(lineBuffer, 26, 57, encoding);
				this.PelicanCreateDate = StringUtility.GetByteLengthString(lineBuffer, 342, 349, encoding);
				this.PelicanStatusCode = StringUtility.GetByteLengthString(lineBuffer, 507, 507, encoding);
				this.PelicanShippingStatus = StringUtility.GetByteLengthString(lineBuffer, 561, 564, encoding);
				this.PelicanOfficeName = StringUtility.GetByteLengthString(lineBuffer, 515, 518, encoding);
				this.PelicanHandyTime = StringUtility.GetByteLengthString(lineBuffer, 519, 532, encoding);
				this.PelicanStatus = StringUtility.GetByteLengthString(lineBuffer, 533, 540, encoding);
				this.PelicanStatusDetail = StringUtility.GetByteLengthString(lineBuffer, 548, 559, encoding);
			}

			/// <summary>配送伝票番号</summary>
			public string ShippingCheckNo { get; set; }
			/// <summary>注文ID</summary>
			public string OrderId { get; set; }
			/// <summary>データ生成日</summary>
			public string PelicanCreateDate { get; set; }
			/// <summary>完了状態コード</summary>
			public string PelicanStatusCode { get; set; }
			/// <summary>配送状態ID</summary>
			public string PelicanShippingStatus { get; set; }
			/// <summary>営業所略称</summary>
			public string PelicanOfficeName { get; set; }
			/// <summary>Handy操作時間</summary>
			public string PelicanHandyTime { get; set; }
			/// <summary>現在の状態</summary>
			public string PelicanStatus { get; set; }
			/// <summary>状況説明</summary>
			public string PelicanStatusDetail { get; set; }
		}
	}
}
