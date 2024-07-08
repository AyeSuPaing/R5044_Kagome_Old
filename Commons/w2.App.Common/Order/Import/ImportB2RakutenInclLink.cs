/*
=========================================================================================================
  Module      : B2配送伝票紐付けデータ（楽天注文含む）取込モジュール(ImportB2InclRakutenLink.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.IO;
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
	/// ImportB2InclRakutenLink の概要の説明です
	/// </summary>
	public class ImportB2InclRakutenLink : ImportBase
	{
		/// <summary>注文IDの列番号</summary>
		private int m_columnNoOrderId = 0; 
		/// <summary>伝票番号の列番号</summary>
		private int m_columnNoShippingNo = 0; 
		/// <summary>B2クラウド用かどうか</summary>
		private bool m_isB2Cloud = false;
		/// <summary>お客様管理コード(注文ID + 個口枝番)</summary>
		private const int CONST_COLUMN_NO_ORDER_ID = 0;
		/// <summary>伝票番号(配送伝票番号)</summary>
		private const int CONST_COLUMN_NO_SHIPPING_NO = 1;
		/// <summary>お客様管理コード(注文ID + 個口枝番)（B2クラウド用）</summary>
		private const int CONST_COLUMN_NO_ORDER_ID_CLOUD = 0;
		/// <summary>伝票番号(配送伝票番号)（B2クラウド用）</summary>
		private const int CONST_COLUMN_NO_SHIPPING_NO_CLOUD = 3;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isCloud">送り状発行システムB2クラウドの取り込みかどうか</param>
		public ImportB2InclRakutenLink(bool isCloud)
		{
			m_isB2Cloud = isCloud;
		}
		/// <summary>
		/// インポート
		/// </summary>
		/// <param name="sr">csvファイルストリーム</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>取込結果</returns>
		/// <remarks>
		/// B2配送伝票紐付けデータ（楽天注文含む）を取込、配送伝票番号を更新
		/// </remarks>
		public override bool Import(StreamReader sr, string loginOperatorName, UpdateHistoryAction updateHistoryAction)
		{
			InitializeColumnNo();
			string strLineBuffer = null;
			int iHeaderColumns = 0;

			// 一行目を取得
			var headerColumns = sr.ReadLine();

			// 先頭行ヘッダ項目数取得
			iHeaderColumns = StringUtility.SplitCsvLine(headerColumns).Length;

			//------------------------------------------------------
			// 配送伝票番号更新処理
			//------------------------------------------------------
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("OrderFileImport", "UpdateOrderShippingCheckNoB2RakutenIncl"))
			using (SqlStatement sqlStatementOrder = new SqlStatement("OrderFileImport", "UpdateOrderLastChangedB2RakutenIncl"))
			{
				try
				{
					// コネクションオープン＆「コミット済みデータ読み取り可能」でトランザクション開始
					sqlAccessor.OpenConnection();
					sqlAccessor.BeginTransaction(IsolationLevel.ReadCommitted);

					// ２行目以降の配送伝票番号取得
					int iCurrentLine = 0;
					var isImportFirstLine = ImportFirstRow(headerColumns);

					// B2クラウドのヘッダ無しでなければ２行目以降の配送伝票番号取得
					if ((isImportFirstLine == false) || (m_isB2Cloud == false))
					{
						iCurrentLine++;
					}

					var errorOrderCount = 0;
					while ((sr.Peek() != -1) || (iCurrentLine == 0))
					{
						// 処理中行カウンタ＋１
						iCurrentLine++;

						// フィールドに改行が含まれる場合を考慮して行を結合（CSVの行に改行がある場合、「"」は奇数個のはず）
						strLineBuffer = (iCurrentLine == 1) ? headerColumns : sr.ReadLine();

						while (((strLineBuffer.Length - strLineBuffer.Replace("\"", "").Length) % 2) != 0)
						{
							strLineBuffer += "\r\n" + sr.ReadLine();
						}

						// １行をCSV分割・フィールド数がヘッダの数と合っているかチェック
						string[] stringBuffer = StringUtility.SplitCsvLine(strLineBuffer);
						if (iHeaderColumns != stringBuffer.Length)
						{
							m_strErrorMessage = MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DEFINITION_MISMATCH)
								.Replace("@@ 1 @@", iCurrentLine.ToString());
							m_strErrorMessage += MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DEFINITION_MISMATCH_EXPLANATION)
									.Replace("@@ 1 @@", iHeaderColumns.ToString())
									.Replace("@@ 2 @@", stringBuffer.Length.ToString());
							this.m_successOrderInfos.Clear();
							return false;
						}

						string[] strOrderId = stringBuffer[m_columnNoOrderId].Split('_');

						// 変更前の配送伝票番号取得
						var order = OrderCommon.GetOrder(strOrderId[0], sqlAccessor);
						if (order.Count == 0)
						{
							errorOrderCount++;
							continue;
						}

						var shippingCheckNoOld = (string)order[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO];

						//------------------------------------------------------
						// 出荷情報登録（外部連携）
						//------------------------------------------------------
						if (this.ExecExternalShipmentEntry)
						{
							var canShipmentEntry = false;
							var errorMessage = ExternalShipmentEntry(
								(string)order[0][Constants.FIELD_ORDER_ORDER_ID],
								(string)order[0][Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
								(string)order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN],
								shippingCheckNoOld,
								stringBuffer[m_columnNoShippingNo],
								(string)order[0][Constants.FIELD_ORDER_CARD_TRAN_ID],
								DeliveryCompanyUtil.GetDeliveryCompanyType(
									(string)order[0][Constants.FIELD_ORDERSHIPPING_DELIVERY_COMPANY_ID],
									(string)order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN]),
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
									(string)order[0][Constants.FIELD_ORDER_ORDER_ID],
									OrderExternalPaymentMemoHelper.CreateOrderPaymentMemo(
										string.IsNullOrEmpty((string)order[0][Constants.FIELD_ORDER_PAYMENT_ORDER_ID])
											? (string)order[0][Constants.FIELD_ORDER_ORDER_ID]
											: (string)order[0][Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
										(string)order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN],
										(string)order[0][Constants.FIELD_ORDER_CARD_TRAN_ID],
										Constants.ACTION_NAME_SHIPPING_REPORT,
										(decimal?)order[0][Constants.FIELD_ORDER_LAST_BILLED_AMOUNT]),
									loginOperatorName,
									UpdateHistoryAction.DoNotInsert,
									sqlAccessor);
							}
						}

						//------------------------------------------------------
						// SQL実行
						//------------------------------------------------------
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_ORDERSHIPPING_ORDER_ID, strOrderId[0]);
						htInput.Add(Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO, (strOrderId.Length == 1 ? 1 : int.Parse(strOrderId[1])));
						htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO, stringBuffer[m_columnNoShippingNo]);
						htInput.Add(Constants.FIELD_ORDER_LAST_CHANGED, loginOperatorName);

						int updated = sqlStatement.ExecStatement(sqlAccessor, htInput);
						m_iUpdatedCount += updated;
						sqlStatementOrder.ExecStatement(sqlAccessor, htInput);

						if (updated == 0)
						{
							AppLogger.WriteError(string.Format(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_UPDATE_SHIPPING_CHECK_NO_ERROR), iCurrentLine, strOrderId[0]));
						}
						else
						{
							this.m_successOrderInfos.Add(new SuccessInfo(iCurrentLine, strOrderId[0], shippingCheckNoOld));
						}

						// 更新履歴登録
						if (updateHistoryAction == UpdateHistoryAction.Insert)
						{
							new UpdateHistoryService().InsertForOrder((string)htInput[Constants.FIELD_ORDERSHIPPING_ORDER_ID], loginOperatorName, sqlAccessor);
						}

						// トランザクションコミット
						sqlAccessor.CommitTransaction();
					}
					// 処理行数(ヘッダー有りの場合は先頭行は除く)
					m_iLinesCount = isImportFirstLine ? iCurrentLine : iCurrentLine - 1;

					if (errorOrderCount > 0)
					{
						m_strErrorMessage += MessageManager
							.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILRIMPORT_ORDER_NOT_EXISTS)
							.Replace("@@ 1 @@", errorOrderCount.ToString());
					}
				}
				catch (Exception ex)
				{
					sqlAccessor.RollbackTransaction();
					this.m_successOrderInfos.Clear();
					throw ex;
				}
			}

			return true;
		}

		/// <summary>
		/// 1行目から取込を行うかどうか
		/// </summary>
		/// <param name="firstRowValues">先頭行</param>
		/// <returns>true：１行目から取り込む、false：１行目を取り込まない</returns>
		private bool ImportFirstRow(string firstRowValues)
		{
			var splitColumns = StringUtility.SplitCsvLine(firstRowValues);
			var result = long.TryParse(splitColumns[CONST_COLUMN_NO_SHIPPING_NO_CLOUD].Trim(), out var orderId);
			return result;
		}

		/// <summary>
		/// 列番号初期化
		/// </summary>
		private void InitializeColumnNo()
		{
			m_columnNoOrderId = m_isB2Cloud
				? CONST_COLUMN_NO_ORDER_ID_CLOUD
				: CONST_COLUMN_NO_ORDER_ID;
			m_columnNoShippingNo = m_isB2Cloud
				? CONST_COLUMN_NO_SHIPPING_NO_CLOUD
				: CONST_COLUMN_NO_SHIPPING_NO;
		}
	}
}
