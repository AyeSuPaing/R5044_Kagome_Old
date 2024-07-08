/*
=========================================================================================================
  Module      : バッチ処理クラス(BatchProc.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using w2.App.Common.Order;
using w2.App.Common.Order.Import;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.UpdateHistory.Helper;

namespace w2.Commerce.Batch.ImportOrderFile
{
	public class BatchProc
	{
		#region +Execute BatchProcResult
		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="batchArgs">バッチ引数</param>
		/// <returns>実行結果</returns>
		public BatchProcResult Execute(BatchArgs batchArgs)
		{
			try
			{
				batchArgs.ThrowExceptionIfBatchArgsAreIllegal();
			}
			catch (BatchArgsException ex)
			{
				throw new Exception(
					"バッチ引数が不正なため処理を終了しました。\r\n" + batchArgs.CreateArgsInfoMessage(),
					ex);
			}

			// Temp、Compのディレクトリを生成
			if (Directory.Exists(Constants.DIRECTORY_TEMP_PATH) == false)
			{
				Directory.CreateDirectory(Constants.DIRECTORY_TEMP_PATH);
			}
			if (Directory.Exists(Constants.DIRECTORY_COMP_PATH) == false)
			{
				Directory.CreateDirectory(Constants.DIRECTORY_COMP_PATH);
			}

			// ファイルを一時Workへ移動
			string workFilePath = Path.Combine(Constants.DIRECTORY_TEMP_PATH, DateTime.Now.ToString("yyyyMMddhhmmss_") + Path.GetFileName(batchArgs.ImportFilePath));
			File.Move(batchArgs.ImportFilePath, workFilePath);

			ImportBase import = this.CreateImporter(batchArgs);
			ImportPayment importPayment = this.CreatePaymentImporter(batchArgs);
			ImportOrder importOrder = this.CreateOrderImporter(batchArgs);
			ImportShipping importShipping = this.CreateOrderShippingImporter(batchArgs, workFilePath);

			bool result = false;
			StringBuilder resultMessage = new StringBuilder();

			// エンコーダ取得
			byte[] csvByteStream = null;
			using (var csvFileStream = new FileStream(workFilePath, FileMode.Open, FileAccess.Read))
			{
				csvByteStream = new byte[csvFileStream.Length];
				csvFileStream.Read(csvByteStream, 0, csvByteStream.Length);
			}
			var enc = StringUtility.GetCode(csvByteStream);
			if (enc == null)
			{
				throw new Exception(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_GET_ENCODING_ERROR));
			}

			// 注文関連ファイル取込
			using (StreamReader streamReader = new StreamReader(workFilePath, enc))
			{
				// 注文関連ファイル取込インスタンス取得
				switch (batchArgs.ImportFileType)
				{
					// CSV入金データ
					case Constants.KBN_ORDERFILE_PAYMENT_DATA:

						// 取込を実行し結果と件数とエラー情報を受け取る（更新履歴とともに）
						result = importPayment.Import(streamReader, batchArgs.ExecuteOperatorName, Path.GetFileNameWithoutExtension(workFilePath), UpdateHistoryAction.Insert);

						// 処理結果メッセージを作成
						if (result)
						{
							resultMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_IMPORT_SUCCESS)
								.Replace("@@ 1 @@", StringUtility.ToNumeric(importPayment.UpdatedCount))
								.Replace("@@ 2 @@", StringUtility.ToNumeric(importPayment.LinesCount)));
						}
						else
						{
							resultMessage.Append(importPayment.ErrorMessage);
							var errorMassagesDetail = new StringBuilder();
							errorMassagesDetail.Append("<br />").Append("---------------------------------------------------------------").Append("<br />");
							errorMassagesDetail.Append(string.Join(",", importPayment.ErrorList.Select(x => x.Keys).First())).Append("<br />");
							errorMassagesDetail.Append(string.Join("<br />", importPayment.ErrorList.Select(x => x.Values).ToArray().Select(x => string.Join(",", x))));
							errorMassagesDetail.Append("<br />").Append("---------------------------------------------------------------");
							resultMessage.Append(errorMassagesDetail);
						}
						break;

					// 注文データ
					case Constants.KBN_ORDERFILE_IMPORT_ORDER:
						result = importOrder.Import(streamReader, batchArgs.ExecuteOperatorName);

						var fixedPurchaseRegistCountText = "";
						if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
						{
							fixedPurchaseRegistCountText = string.Format("(定期台帳生成数：{0})", StringUtility.ToNumeric(importOrder.FixedPurchaseRegistCount));
						}
						if (importOrder.LineCount != 0)
						{
							resultMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_IMPORT_ORDER_SUCCESS))
								.Replace("@@ 1 @@", StringUtility.ToNumeric(importOrder.OrderCount))
								.Replace("@@ 2 @@", StringUtility.ToNumeric(importOrder.ImportCount))
								.Replace("@@ 3 @@", fixedPurchaseRegistCountText)
								.Replace("@@ 4 @@", StringUtility.ToNumeric(importOrder.OrderCount - importOrder.ImportCount))
								.Replace("@@ 5 @@", StringUtility.ToNumeric(importOrder.LineCount))
								.Replace("@@ 6 @@", StringUtility.ToNumeric(importOrder.OrderItemImportCount))
								.Replace("@@ 7 @@", StringUtility.ToNumeric(importOrder.LineCount - importOrder.OrderItemImportCount));
						}

						resultMessage.Append(importOrder.GetErrorMessage());
						break;

					case Constants.KBN_ORDERFILE_SHIPPING_DATA:
						// Execute import and receive result, number of cases and error information (along with update history)
						result = importShipping.Import(
							streamReader,
							batchArgs.ExecuteOperatorName,
							UpdateHistoryAction.Insert);

						// Create processing result message
						if (result)
						{
							resultMessage.Append(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_IMPORT_SUCCESS)
								.Replace("@@ 1 @@", StringUtility.ToNumeric(importShipping.UpdatedCount))
								.Replace("@@ 2 @@", StringUtility.ToNumeric(importShipping.LinesCount)));
							break;
						}

						resultMessage.Append(importShipping.ErrorMessage);
						var errorMessagesDetail = new StringBuilder();
						errorMessagesDetail.Append("<br />")
							.Append("---------------------------------------------------------------")
							.Append("<br />")
							.Append(string.Join(",", importShipping.ErrorList.Select(item => item.Keys).First()))
							.Append("<br />")
							.Append(string.Join("<br />",
								importShipping.ErrorList.Select(item => item.Values).ToArray().Select(item => string.Join(",", item))))
							.Append("<br />")
							.Append("---------------------------------------------------------------");
						resultMessage.Append(errorMessagesDetail);
						break;

					// 紐付けデータ（更新履歴とともに）
					default:
						result = import.Import(streamReader, batchArgs.ExecuteOperatorName, UpdateHistoryAction.Insert);

						string mailTemplateId = batchArgs.MailTemplateID;

						if (result
							&& (import.SuccessOrderInfos.Count > 0)
							&& (mailTemplateId != string.Empty))
						{
							var errorMessages = SendMail(import.SuccessOrderInfos, mailTemplateId);
							resultMessage.Append(errorMessages);
						}

						// 処理結果メッセージを作成
						if (result)
						{
							string successMessage = string.Format("{0}\r\n\r\n{1}", ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_IMPORT_SUCCESS)
								.Replace("@@ 1 @@", StringUtility.ToNumeric(import.UpdatedCount))
								.Replace("@@ 2 @@", StringUtility.ToNumeric(import.LinesCount)),
								import.ErrorMessage);
							resultMessage.Insert(0, successMessage);
						}
						else
						{
							resultMessage.Append(import.ErrorMessage);
						}
						break;

					//受注情報ステータスの一括更新
					case Constants.KBN_ORDERFILE_IMPORT_ORDER_STATUS:
						//読み込まれるファイルはcsvであるか
						var exe = System.IO.Path.GetExtension(batchArgs.ImportFilePath);
						if (exe.Equals(".csv") == false)
						{
							resultMessage.AppendLine(MessageManager.GetMessages(ImportMessage.ERRMSG_DATA_MIGRATION_FILE_UPLOAD_NOT_CSV));
							break;
						}

						result = import.Import(
							streamReader,
							batchArgs.ExecuteOperatorName,
							UpdateHistoryAction.Insert);

						// 処理結果メッセージを作成
						if (result)
						{
							string successMessage = string.Format(
								"{0}\r\n\r\n{1}",
								ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_IMPORT_SUCCESS)
									.Replace("@@ 1 @@", StringUtility.ToNumeric(import.UpdatedCount))
									.Replace("@@ 2 @@", StringUtility.ToNumeric(import.LinesCount)),
								import.ErrorMessage);
							resultMessage.Insert(0, successMessage);
							if (string.IsNullOrEmpty(import.ErrorMessage) == false)
							{
								resultMessage.AppendLine(import.ErrorMessage);
							}
						}
						else
						{
							resultMessage.Append(import.ErrorMessage);
						}
						break;
				}
			}

			// ファイルの処理
			if (batchArgs.ImportFileType == Constants.KBN_ORDERFILE_IMPORT_ORDER)
			{
				// ファイルを削除する
				File.Delete(workFilePath);
			}
			else
			{
				// ファイルを完了ディレクトリに入れる
				var compFilePath = Path.Combine(Constants.DIRECTORY_COMP_PATH, Path.GetFileName(workFilePath));
				File.Move(workFilePath, compFilePath);
			}

			// ログに落としておく
			var msg = resultMessage.ToString().Replace("<br/>", "\r\n").Replace("<br />", "\r\n");
			FileLogger.WriteInfo(msg);

			if (batchArgs.ImportFileType == Constants.KBN_ORDERFILE_IMPORT_ORDER)
			{
				return new BatchProcResult(result, msg, importOrder.OrderItemImportCount);
			}
			return new BatchProcResult(result, msg);

		}
		#endregion

		#region -CreateOrderImporter 注文取り込みクラス生成
		/// <summary>
		/// 注文取り込みクラス生成
		/// </summary>
		/// <param name="batchArgs">バッチ引数</param>
		/// <returns>受注取り込みクラス</returns>
		private ImportOrder CreateOrderImporter(BatchArgs batchArgs)
		{
			// 注文取り込みインスタンス生成
			ImportOrder importOrder = null;

			switch (batchArgs.ImportFileType)
			{
				case Constants.KBN_ORDERFILE_IMPORT_ORDER:
					importOrder = new ImportOrder(batchArgs.OrderFileSettingValue);
					break;
			}
			return importOrder;
		}
		#endregion

		#region -CreatePaymentImporter 入金系取込クラス生成
		/// <summary>
		/// 入金系取込クラス生成
		/// </summary>
		/// <param name="batchArgs">バッチ引数</param>
		/// <returns>入金ファイル取込クラス</returns>
		private ImportPayment CreatePaymentImporter(BatchArgs batchArgs)
		{
			// 入金系ファイル取込インスタンス生成
			ImportPayment importPayment = null;

			switch (batchArgs.ImportFileType)
			{
				// CSV入金データ
				case Constants.KBN_ORDERFILE_PAYMENT_DATA:
					importPayment = new ImportPayment(batchArgs.OrderFileSettingValue);
					break;
			}
			return importPayment;
		}
		#endregion

		#region +CreateImporter 取込クラス生成
		/// <summary>
		/// 取込クラス生成
		/// </summary>
		/// <param name="batchArgs">バッチ引数</param>
		/// <returns>取込クラス</returns>
		private ImportBase CreateImporter(BatchArgs batchArgs)
		{
			ImportBase import = null;

			// 注文関連ファイル取込インスタンス生成
			switch (batchArgs.ImportFileType)
			{
				// W2C標準 紐付けデータ
				case Constants.KBN_ORDERFILE_SHIPPING_NO_LINK:
					import = new ImportShippingNoLink()
					{
						ExecExternalShipmentEntry = OrderCommon.CanShipmentEntry()
							&& batchArgs.ExecExternalShipmentEntry
					};
					break;

				// e-cat2000(e-cat紐付けデータ)
				case Constants.KBN_ORDERFILE_ECAT2000LINK:
					import = new ImportECat2000Link
					{
						ExecExternalShipmentEntry = OrderCommon.CanShipmentEntry()
							&& batchArgs.ExecExternalShipmentEntry
					};
					break;

				// B2配送伝票紐付けデータ（楽天注文含む）
				case Constants.KBN_B2_RAKUTEN_INCL_LINK:
					import = new ImportB2InclRakutenLink(false)
					{
						ExecExternalShipmentEntry = OrderCommon.CanShipmentEntry()
							&& batchArgs.ExecExternalShipmentEntry
					};
					break;

				// B2配送伝票紐付けデータ（楽天注文含む）（B2クラウド用）
				case Constants.KBN_B2_RAKUTEN_INCL_LINK_CLOUD:
					import = new ImportB2InclRakutenLink(true)
					{
						ExecExternalShipmentEntry = OrderCommon.CanShipmentEntry()
							&& batchArgs.ExecExternalShipmentEntry
					};
					break;

				// 税率毎価格情報紐付けデータ
				case Constants.KBN_ORDERFILE_IMPORT_ORDER_PRICE_BY_TAX_RATE:
					import = new ImportOrderPriceByTaxRate
					{
						ExecExternalShipmentEntry = OrderCommon.CanShipmentEntry()
							&& batchArgs.ExecExternalShipmentEntry
					};
					break;

				// 注文拡張ステータス更新データ
				case Constants.KBN_ORDERFILE_ORDER_EXTEND:
				case Constants.KBN_ORDERFILE_CANCEL_FIXEDPURCHASE:
					// HACK: コンストラクタの引数が意味不明。とりあえず”OrderExtend”を渡したら動いたが…
					import = new ImportOrderExtend(batchArgs.ImportFileType)
					{
						ExecExternalShipmentEntry = false
					};
					break;

				// ２回目未入金者取込
				case Constants.KBN_ORDERFILE_IMPORT_ORDER_SECOND_TIME_NON_DEPOSIT:
					import = new ImportOrderSecondTimeNonDeposit(batchArgs.ImportFileType);
					break;

				// DSK入金データ取込
				case Constants.KBN_ORDERFILE_IMPORT_PAYMENT_DEPOSIT_DSK:
					import = new ImportPaymentDeposit(batchArgs.ImportFileType);
					break;

				// 宅配通配送実績紐付けデータ
				case Constants.KBN_ORDERFILE_PELICAN_RESULT_REPORT_LINK:
					import = new ImportPelicanResultReportLink()
					{
						ExecExternalShipmentEntry = OrderCommon.CanShipmentEntry()
							&& batchArgs.ExecExternalShipmentEntry
					};
					break;

				// 受注情報ステータスの一括更新
				case Constants.KBN_ORDERFILE_IMPORT_ORDER_STATUS:
					import = new ImportOrderStatus(batchArgs.ImportFileType);
					break;
			}

			return import;
		}
		#endregion

		#region +CreateOrderShippingImporter
		/// <summary>
		/// Create order shipping importer
		/// </summary>
		/// <param name="batchArgs">Batch args</param>
		/// <param name="workFilePath">Work file path</param>
		/// <returns>Order shipping update class</returns>
		private ImportShipping CreateOrderShippingImporter(BatchArgs batchArgs, string workFilePath)
		{
			if (batchArgs.ImportFileType != Constants.KBN_ORDERFILE_SHIPPING_DATA) return null;

			var fileName = Path.GetFileNameWithoutExtension(workFilePath);
			var importShipping = new ImportShipping(batchArgs.OrderFileSettingValue, fileName)
			{
				ExecExternalShipmentEntry = (OrderCommon.CanShipmentEntry() && batchArgs.ExecExternalShipmentEntry),
			};
			return importShipping;
		}
		#endregion

		#region +SendMail Send the mail to user and update extend status.
		/// <summary>
		/// Send the mail to user and update extend status.
		/// </summary>
		/// <param name="successInfos">The success infos</param>
		/// <param name="mailTemplateId">The mail template identifier.</param>
		/// <returns>The error messages</returns>
		private string SendMail(List<ImportBase.SuccessInfo> successInfos, string mailTemplateId)
		{
			var errorMessages = new StringBuilder();

			foreach (var succesInfo in successInfos)
			{
				try
				{
					OrderCommon.SendOrderMail(succesInfo.OrderId, mailTemplateId);
				}
				catch (Exception exception)
				{
					AppLogger.WriteError(string.Format("SHIPPING_NO_LINK 行:{0} 注文ID:{1}", succesInfo.LineNo, succesInfo.OrderId), exception);
					errorMessages.AppendFormat(ImportMessage.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_SEND_MAIL_ERROR), succesInfo.LineNo, succesInfo.OrderId);
				}
			}

			return errorMessages.ToString();
		}
		#endregion
	}
}
