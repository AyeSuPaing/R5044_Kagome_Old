/*
=========================================================================================================
  Module      : W2C標準 紐付けデータ取込モジュール(ImportShippingNoLink.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Linq;
using w2.App.Common.Uketoru;
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
	/// ImportShippingNoLink の概要の説明です
	/// </summary>
	public class ImportShippingNoLink : ImportBase
	{
		// プロパティ
		private const int m_iColumnsCountMin = 2; // 1行の最小項目数を設定
		private const int m_iColumnsCountMax = 3; // 1行の最大項目数を設定
		private const int m_uketoruColumnsCount = 12; // ウケトル連携の項目数を設定

		/// <summary>
		/// インポート
		/// </summary>
		/// <param name="sr">csvファイルストリーム</param>
		/// <param name="loginOperatorName">ログインオペレータ名</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		/// <returns>取込結果</returns>
		/// <remarks>
		/// 紐付けデータを取込、配送伝票番号を更新
		/// </remarks>
		public override bool Import(StreamReader sr, string loginOperatorName, UpdateHistoryAction updateHistoryAction)
		{
			string strLineBuffer = null;
			string postData = null;

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

					// 各行を読み取る
					int iCurrentLine = 0;
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

						if (this.ExecUketoruCooperation && (iCurrentLine == 1))
						{
							postData += strLineBuffer;
						}

						// １行をCSV分割・フィールド数が正しい項目数(m_iColumnsCount)と合っているかチェック
						string[] stringBuffer = StringUtility.SplitCsvLine(strLineBuffer);
						if (this.ExecUketoruCooperation == false
							? ((m_iColumnsCountMin != stringBuffer.Length) && (m_iColumnsCountMax != stringBuffer.Length))
							: (m_uketoruColumnsCount != stringBuffer.Length))
						{
							m_strErrorMessage = MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DEFINITION_MISMATCH)
								.Replace("@@ 1 @@", iCurrentLine.ToString());
							if (this.ExecUketoruCooperation == false)
							{
								m_strErrorMessage += MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DEFINITION_MISMATCH_DOUBLE_EXPLANATION)
									.Replace("@@ 1 @@", m_iColumnsCountMin.ToString())
									.Replace("@@ 2 @@", m_iColumnsCountMax.ToString())
									.Replace("@@ 3 @@", stringBuffer.Length.ToString());
							}
							else
							{
								m_strErrorMessage += MessageManager.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_DEFINITION_MISMATCH_UKETORU_EXPLANATION)
									.Replace("@@ 1 @@", m_uketoruColumnsCount.ToString())
									.Replace("@@ 2 @@", stringBuffer.Length.ToString());
							}
							this.m_successOrderInfos.Clear();
							return false;
						}

						string[] strOrderId = (this.ExecUketoruCooperation ? stringBuffer[5] : stringBuffer[0]).Split('_');

						// 変更前の配送伝票番号取得
						var order = OrderCommon.GetOrder(strOrderId[0], sqlAccessor);
						if ((order.Count != 0) && (string.IsNullOrEmpty(stringBuffer[0]) == false) && (string.IsNullOrEmpty(stringBuffer[1]) == false))
						{
							var shippingCheckNoOld = (string)order[0][Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO];

							//------------------------------------------------------
							// 出荷情報登録（外部連携）
							//------------------------------------------------------
							if (this.ExecExternalShipmentEntry)
							{
								bool canShipmentEntry;
								var errorMessage = ExternalShipmentEntry(
									(string)order[0][Constants.FIELD_ORDER_ORDER_ID],
									(string)order[0][Constants.FIELD_ORDER_PAYMENT_ORDER_ID],
									(string)order[0][Constants.FIELD_ORDER_ORDER_PAYMENT_KBN],
									shippingCheckNoOld,
									this.ExecUketoruCooperation ? stringBuffer[0] : stringBuffer[1],
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
							htInput.Add(Constants.FIELD_ORDERSHIPPING_SHIPPING_CHECK_NO, this.ExecUketoruCooperation ? stringBuffer[0] : stringBuffer[1]);
							htInput.Add(Constants.FIELD_ORDER_LAST_CHANGED, loginOperatorName);

							var updated =
								new OrderService().UpdateOrderShippingCheckNo(
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

							if (this.ExecUketoruCooperation)
							{
								postData += "\r\n" + strLineBuffer;
							}
						}

						// トランザクションコミット
						sqlAccessor.CommitTransaction();
					}

					// 処理行数
					m_iLinesCount = this.ExecUketoruCooperation ? iCurrentLine - 1 : iCurrentLine;
				}
				catch (Exception ex)
				{
					sqlAccessor.RollbackTransaction();
					this.m_successOrderInfos.Clear();
					throw ex;
				}
			}

			if (this.ExecUketoruCooperation)
			{
				var response = new UketoruTrackersApi().PostHttp(this.CsvFileName, postData);
				FileLogger.Write("uketoru", string.Format(
					"[UKETORU TRACKERS API]ステータス:{0} 追跡番号取込件数:{1} 取込成功件数:{2} 既にDBに登録されていた配送伝票番号:{3}",
					response.Result.Status,
					response.Result.TotalCount,
					response.Result.SuccessCount,
					string.Join(",", response.Result.DuplicatedTrackingNumbers.Select(d => d.ElementAt(0)).Select(d => d.Value))));

				if (response.Result.Status == "error")
				{

					m_strErrorMessage += string.Format(
						ImportMessage.GetMessages(
							ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_UKETORU_COOPERATION_ERROR),
						response.Result.ErrorMessage);

					return false;
				}

				if (response.Result.DuplicatedTrackingNumbers.Count != 0)
				{
					m_strErrorMessage += string.Format(
						ImportMessage.GetMessages(
							ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_UKETORU_COOPERATION_DUPLICATED_TRACKING),
						string.Join(",", response.Result.DuplicatedTrackingNumbers.Select(d => d.ElementAt(0)).Select(d => d.Value)));

					return false;
				}
			}

			return true;
		}
	}
}
