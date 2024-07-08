/*
=========================================================================================================
  Module      : 注文関連ファイル取込モジュール(OrderFileImportProcess.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Order.Import;
using w2.App.Common.Order.Workflow;
using w2.Common.Logger;
using w2.Domain.UpdateHistory.Helper;

/// <summary>
/// 注文関連ファイル取込共通処理実行クラス
/// </summary>
public class OrderFileImportProcess
{
	/// <summary>
	/// 注文関連ファイル取込共通処理
	/// </summary>
	/// <param name="selectedValue">ファイル種別</param>
	/// <param name="file">ファイルデータ</param>
	/// <param name="fileNamePattern">ファイル名パターン</param>
	/// <param name="mailTemplateId">メールテンプレートID</param>
	/// <param name="isShipmentEntry">出荷情報登録連携の有無</param>
	/// <param name="operatorName">オペレーター名</param>
	/// <returns>Import結果</returns>
	public static ExecResult Start(
		string selectedValue,
		HttpPostedFile file,
		string fileNamePattern,
		string mailTemplateId,
		bool isShipmentEntry,
		string operatorName)
	{
		var resultMessage = new StringBuilder();
		var selectedValueExtend = selectedValue;
		selectedValue = selectedValue.Split(':')[0];
		var importResponse = new ExecResult
		{
			FileName = file.FileName,
			ImportType = selectedValue,
			IsAsyncExec = (Constants.ORDER_FILE_IMPORT_ASYNC
			|| (selectedValue == Constants.KBN_ORDERFILE_IMPORT_ORDER
			|| selectedValue == Constants.KBN_ORDERFILE_IMPORT_ORDER_STATUS))
		};

		// ファイル指定チェック
		if (importResponse.ImportSuccess && (file.FileName == ""))
		{
			importResponse.ImportSuccess = false;
			resultMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_FILE_UNSELECTED));
		}

		// ファイル存在チェック
		var inputStream = file.InputStream;
		if (importResponse.ImportSuccess && (inputStream.Length == 0))
		{
			importResponse.ImportSuccess = false;
			resultMessage.Append(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_FILE_UNFIND));
		}

		// ファイル名フォーマットチェック
		if (importResponse.ImportSuccess && ((selectedValue == Constants.KBN_ORDERFILE_PAYMENT_DATA)
			|| (selectedValue == Constants.KBN_ORDERFILE_RETURNS_ACCEPTED)
			|| (selectedValue == Constants.KBN_ORDERFILE_CANCEL_FIXEDPURCHASE)
			|| (selectedValue == Constants.KBN_ORDERFILE_ORDER_EXTEND)
			|| (selectedValue == Constants.KBN_ORDERFILE_SHIPPING_DATA)
			|| (selectedValue == Constants.KBN_ORDERFILE_IMPORT_ORDER_STATUS)))
		{
			if (ImportPayment.CheckFileName(fileNamePattern, Path.GetFileName(file.FileName)) == false)
			{
				importResponse.ImportSuccess = false;
				resultMessage.Append(
					WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_INVALID_FILE_NAME)
						.Replace("@@ 1 @@", fileNamePattern));
			}
		}

		// バッチ処理
		// 受注取り込みはバッチで必ず実行する
		if (importResponse.IsAsyncExec)
		{
			// 保存先のファイルディレクトリ
			if (Directory.Exists(Constants.DIRECTORY_IMPORTORDERFILE_UPLOAD_PATH) == false)
			{
				// 無ければ作る
				Directory.CreateDirectory(Constants.DIRECTORY_IMPORTORDERFILE_UPLOAD_PATH);
			}

			var saveFilepath = Path.Combine(
				Constants.DIRECTORY_IMPORTORDERFILE_UPLOAD_PATH,
				Path.GetFileName(file.FileName));

			// Up予定のファイルがあれば退避
			if (File.Exists(saveFilepath))
			{
				File.Move(saveFilepath, string.Format("{0}_{1:yyyyMMddhhmmss}", saveFilepath, DateTime.Now));
			}

			// Upしたファイルを保存
			file.SaveAs(saveFilepath);

			// 引数
			// 取り込みファイルパス、取り込みファイルの種別、出荷連携するかしないか、実行オペレータ名、注文ファイル設定値、メールテンプレートIDを渡す
			var args = string.Format(
				"\"{0}\" {1} {2} \"{3}\" {4} {5}",
				saveFilepath,
				selectedValue,
				isShipmentEntry ? "1" : "0",
				operatorName,
				selectedValueExtend,
				mailTemplateId);

			// バッチ実行
			Process.Start(Constants.PHYSICALDIRPATH_ORDERFILEIMPORT_EXE, args);

			return importResponse;
		}

		// エンコーディング取得（取得に失敗した場合エラー）
		// 宅配通配送実績データ取込時はエンコーディングを「Big5」に固定するため、エンコーディング取得処理を行わない
		var encoding = Encoding.GetEncoding(932);
		if (selectedValue != Constants.KBN_ORDERFILE_PELICAN_RESULT_REPORT_LINK)
		{
			var csvByteStream = new byte[inputStream.Length];
			inputStream.Read(csvByteStream, 0, csvByteStream.Length);
			encoding = StringUtility.GetCode(csvByteStream);
			if (encoding == null)
			{
				importResponse.ImportSuccess = false;
				resultMessage.Append(WebMessages.GetMessages(ImportMessage.ERRMSG_MANAGER_ORDERFILEIMPORT_GET_ENCODING_ERROR));
			}
			inputStream.Seek(0, SeekOrigin.Begin);
		}

		// 上記チェックでエラーではない場合
		if (importResponse.ImportSuccess)
		{
			ImportBase import = null; // 標準配送伝票紐づけ
			ImportPayment importPayment = null; // 入金ファイル取込
			// Shipping file import
			ImportShipping importShipping = null;

			// 注文関連ファイル取込インスタンス取得
			switch (selectedValue)
			{
				// W2C標準 紐付けデータ
				case Constants.KBN_ORDERFILE_SHIPPING_NO_LINK:
				// ウケトル 紐付けデータ
				case Constants.KBN_ORDERFILE_UKETORU_LINK:
					import = new ImportShippingNoLink()
					{
						ExecExternalShipmentEntry = (OrderCommon.CanShipmentEntry() && isShipmentEntry),
						CsvFileName = file.FileName,
						ExecUketoruCooperation = (selectedValue == Constants.KBN_ORDERFILE_UKETORU_LINK)
					};
					break;

				// e-cat2000(e-cat紐付けデータ)
				case Constants.KBN_ORDERFILE_ECAT2000LINK:
					import = new ImportECat2000Link
					{
						ExecExternalShipmentEntry = OrderCommon.CanShipmentEntry() && isShipmentEntry
					};
					break;

				// B2配送伝票紐付けデータ（楽天注文含む）
				case Constants.KBN_B2_RAKUTEN_INCL_LINK:
					import = new ImportB2InclRakutenLink(false)
					{
						ExecExternalShipmentEntry = OrderCommon.CanShipmentEntry() && isShipmentEntry
					};
					break;

				// B2配送伝票紐付けデータ（楽天注文含む）（B2クラウド用）
				case Constants.KBN_B2_RAKUTEN_INCL_LINK_CLOUD:
					import = new ImportB2InclRakutenLink(true)
					{
						ExecExternalShipmentEntry = OrderCommon.CanShipmentEntry() && isShipmentEntry
					};
					break;

				// 税率毎価格情報データ
				case Constants.KBN_ORDERFILE_IMPORT_ORDER_PRICE_BY_TAX_RATE:
					import = new ImportOrderPriceByTaxRate()
					{
						ExecExternalShipmentEntry = OrderCommon.CanShipmentEntry() && isShipmentEntry
					};
					break;

				// CSV入金データ
				case Constants.KBN_ORDERFILE_PAYMENT_DATA:
					importPayment = new ImportPayment(selectedValueExtend).SetCreatePopupMessageProc(CreatePopUpUrl);
					break;

				// 返品受付/定期解約/拡張ステータス更新
				case Constants.KBN_ORDERFILE_RETURNS_ACCEPTED:
				case Constants.KBN_ORDERFILE_CANCEL_FIXEDPURCHASE:
				case Constants.KBN_ORDERFILE_ORDER_EXTEND:
					import = new ImportOrderExtend(selectedValueExtend);
					break;

				// ２回目未入金者取込
				case Constants.KBN_ORDERFILE_IMPORT_ORDER_SECOND_TIME_NON_DEPOSIT:
					import = new ImportOrderSecondTimeNonDeposit(selectedValueExtend);
					break;

				// （DSK）入金データ取込
				case Constants.KBN_ORDERFILE_IMPORT_PAYMENT_DEPOSIT_DSK:
					import = new ImportPaymentDeposit(selectedValueExtend);
					break;

				// Csv shipping data
				case Constants.KBN_ORDERFILE_SHIPPING_DATA:
					var fileName = Path.GetFileNameWithoutExtension(file.FileName);
					importShipping = new ImportShipping(selectedValueExtend, fileName)
					{
						ExecExternalShipmentEntry = (OrderCommon.CanShipmentEntry() && isShipmentEntry),
					};
					importShipping.SetCreatePopupMessageProc(CreatePopUpUrl);
					break;

				// 宅配通紐付けデータ
				case Constants.KBN_ORDERFILE_PELICAN_RESULT_REPORT_LINK:
					import = new ImportPelicanResultReportLink
					{
						ExecExternalShipmentEntry = OrderCommon.CanShipmentEntry() && isShipmentEntry
					};
					// エンコーディングを「Big5」に固定
					encoding = Encoding.GetEncoding(950);
					break;
			}

			// 注文関連ファイル取込
			using (var streamReader = new StreamReader(inputStream, encoding))
			{
				// 注文関連ファイル取込インスタンス取得
				switch (selectedValue)
				{
					// CSV入金データ
					case Constants.KBN_ORDERFILE_PAYMENT_DATA:

						// 取込を実行し結果と件数とエラー情報を受け取る（更新履歴とともに）
						importResponse.ImportSuccess = importPayment.Import(
							streamReader,
							operatorName,
							Path.GetFileNameWithoutExtension(file.FileName),
							UpdateHistoryAction.Insert);

						// 処理結果メッセージを作成
						if (importResponse.ImportSuccess)
						{
							resultMessage.Append(
								WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_IMPORT_SUCCESS)
									.Replace("@@ 1 @@", StringUtility.ToNumeric(importPayment.UpdatedCount)).Replace(
										"@@ 2 @@",
										StringUtility.ToNumeric(importPayment.LinesCount)));

							// もし処理対象外行があったらCSVリンクを表示する
							if (string.IsNullOrEmpty(importPayment.FilePathExcludeRecord) == false)
							{
								importResponse.ExcludeMessage = WebMessages
									.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_IMPORT_EXCLUDE_RECORD)
									.Replace("@@ 1 @@", importPayment.FilePathExcludeRecord);
							}
						}
						else
						{
							resultMessage.Append(importPayment.ErrorMessage);
						}

						// エラーデータ表示
						var errorPaymentDataList = importPayment.ErrorList; // エラーデータ一覧
						importResponse.ErrorData = errorPaymentDataList;
						importResponse.TotalCase = importPayment.LinesCount;
						importResponse.ErrorCase = errorPaymentDataList.Count;
						importResponse.SuccessCase = importPayment.LinesCount - errorPaymentDataList.Count;
						break;

					// 受注取り込み
					case Constants.KBN_ORDERFILE_IMPORT_ORDER:
						// ここに来ることはないはず。
						break;

					// Csv shipping data
					case Constants.KBN_ORDERFILE_SHIPPING_DATA:
						// Execute import and receive result, number of cases and error information
						importResponse.ImportSuccess = importShipping.Import(
							streamReader,
							operatorName,
							UpdateHistoryAction.Insert);

						// Create processing result message
						if (importResponse.ImportSuccess)
						{
							resultMessage.Append(
								WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_IMPORT_SUCCESS)
									.Replace("@@ 1 @@", StringUtility.ToNumeric(importShipping.UpdatedCount)).Replace(
										"@@ 2 @@",
										StringUtility.ToNumeric(importShipping.LinesCount)));

							// If there is a line that is not processed, a CSV link will be displayed.
							if ((importShipping.FilePathExcludeRecord != null)
								&& (string.IsNullOrEmpty(importShipping.FilePathExcludeRecord) == false))
							{
								importResponse.ExcludeMessage = WebMessages
									.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_IMPORT_EXCLUDE_RECORD)
									.Replace("@@ 1 @@", importShipping.FilePathExcludeRecord);
							}
						}
						else
						{
							resultMessage.Append(importShipping.ErrorMessage);
						}

						// Error data display
						var errorshippingDataList = importShipping.ErrorList;
						importResponse.ErrorData = errorshippingDataList;
						importResponse.TotalCase = importShipping.LinesCount;
						importResponse.ErrorCase = errorshippingDataList.Count;
						importResponse.SuccessCase = importShipping.LinesCount - errorshippingDataList.Count;
						break;

					// 紐付けデータ（更新履歴とともに）
					default:
						importResponse.ImportSuccess = import.Import(
							streamReader,
							operatorName,
							UpdateHistoryAction.Insert);

						if (importResponse.ImportSuccess
							&& (import.SuccessOrderInfos.Count > 0)
							&& (mailTemplateId != string.Empty))
						{
							var errorMessages = SendMail(import.SuccessOrderInfos, mailTemplateId);
							resultMessage.Append(errorMessages);
						}

						// 処理結果メッセージを作成
						if (importResponse.ImportSuccess)
						{
							importResponse.TotalCase = import.LinesCount;
							importResponse.SuccessCase = import.UpdatedCount;
							importResponse.ErrorCase = (import.LinesCount - import.UpdatedCount);
							var successMessage = string.Format(
									"{0}<br/>",
									WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_IMPORT_SUCCESS)
										.Replace("@@ 1 @@", StringUtility.ToNumeric(import.UpdatedCount))
										.Replace("@@ 2 @@", StringUtility.ToNumeric(import.LinesCount)))
								+ "<br/>"
								+ StringUtility.ChangeToBrTag(import.ErrorMessage);
							resultMessage.Insert(0, successMessage);
						}
						else
						{
							resultMessage.Append(import.ErrorMessage);
						}
						break;
				}
			}
		}
		importResponse.ResultMessage = resultMessage.ToString();
		return importResponse;
	}

	/// <summary>
	/// Send the mail to user and update extend status.
	/// </summary>
	/// <param name="successInfos">The success infos</param>
	/// <param name="mailTemplateId">The mail template identifier.</param>
	/// <returns>The error messages</returns>
	private static string SendMail(List<ImportBase.SuccessInfo> successInfos, string mailTemplateId)
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
				errorMessages.AppendFormat(WebMessages.GetMessages(WebMessages.ERRMSG_MANAGER_ORDERFILEIMPORT_SEND_MAIL_ERROR), succesInfo.LineNo, succesInfo.OrderId);
			}
		}

		return errorMessages.ToString();
	}


	/// <summary>
	/// ポップアップリンクを作成する
	/// </summary>
	/// <param name="orderIdList">注文ID</param>
	/// <returns>注文情報詳細URL</returns>
	private static string CreatePopUpUrl(List<string> orderIdList)
	{
		var popupUrlString = new StringBuilder();
		foreach (var orderId in orderIdList)
		{
			if (popupUrlString.Length != 0)
			{
				popupUrlString.Append(", ");
			}

			// 注文詳細URL作成
			var orderDetailUrl = OrderPage.CreateOrderDetailUrl(orderId, true, false, Constants.PAGE_MANAGER_ORDERFILEIMPORT_LIST);

			// javascript作成
			var orderDetailLink = new StringBuilder();
			orderDetailLink.Append("<a href=\"javascript:open_window('");
			orderDetailLink.Append(WebSanitizer.UrlAttrHtmlEncode(orderDetailUrl.ToString()));
			orderDetailLink.Append("','ordercontact','width=828,height=600,top=110,left=380,status=NO,scrollbars=yes');\" >");
			orderDetailLink.Append(orderId);
			orderDetailLink.Append("</a>");

			popupUrlString.Append(orderDetailLink);
		}

		return popupUrlString.ToString();
	}
}