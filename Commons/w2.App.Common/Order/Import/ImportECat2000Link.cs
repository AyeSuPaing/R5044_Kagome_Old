/*
=========================================================================================================
  Module      : e-cat2000(紐付けデータ)取込モジュール(ImportECat2000Link.cs)
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
	/// ImportECat2000Link の概要の説明です
	/// </summary>
	public class ImportECat2000Link : ImportBase
	{
		/// <summary>
		/// インポート
		/// </summary>
		/// <param name="sr">csvファイルストリーム</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>取込結果</returns>
		/// <remarks>
		/// e-cat2000(紐付けデータ)を取込、配送伝票番号を更新
		/// </remarks>
		public override bool Import(StreamReader sr, string loginOperatorName, UpdateHistoryAction updateHistoryAction)
		{
			string strLineBuffer = null;
			int iHeaderColumns = 0;

			// 先頭行ヘッダ項目数取得
			iHeaderColumns = StringUtility.SplitCsvLine(sr.ReadLine()).Length;

			//------------------------------------------------------
			// 配送伝票番号更新処理
			//------------------------------------------------------
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatementOrder = new SqlStatement("OrderFileImport", "UpdateOrderLastChanged"))
			{
				try
				{
					// コネクションオープン＆「コミット済みデータ読み取り可能」でトランザクション開始
					sqlAccessor.OpenConnection();
					sqlAccessor.BeginTransaction(IsolationLevel.ReadCommitted);

					// ２行目以降の配送伝票番号取得
					int iCurrentLine = 1;
					while (sr.Peek() != -1)
					{
						// 処理中行カウンタ＋１
						iCurrentLine++;

						// フィールドに改行が含まれる場合を考慮して行を結合（CSVの行に改行がある場合、「"」は奇数個のはず）
						strLineBuffer = sr.ReadLine();
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

						string[] strOrderId = stringBuffer[0].Split('_');

						// 変更前の配送伝票番号取得
						var order = OrderCommon.GetOrder(strOrderId[0], sqlAccessor);
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
								stringBuffer[1],
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
						htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO, stringBuffer[1]);
						htInput.Add(Constants.FIELD_ORDER_LAST_CHANGED, loginOperatorName);

						var updated = new OrderService().UpdateOrderShippingCheckNo(
							(string)htInput[Constants.FIELD_ORDERSHIPPING_ORDER_ID],
							(int)htInput[Constants.FIELD_ORDERSHIPPING_ORDER_SHIPPING_NO],
							(string)htInput[Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO],
							(string)htInput[Constants.FIELD_ORDER_LAST_CHANGED],
							UpdateHistoryAction.DoNotInsert,
							sqlAccessor);
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
					// 処理行数(先頭行は除く)
					m_iLinesCount = iCurrentLine - 1;
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
	}
}
