/*
=========================================================================================================
  Module      : PDF情報出力ページ処理(PdfOutput.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.IO;
using System.Text;

// PDF処理用
using w2.App.Common.Pdf.PdfCreater;
using w2.App.Common.SendMail;
using w2.Domain.Order;

public partial class Form_PdfOutput_PdfOutput : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		//------------------------------------------------------
		// パラメタ取得
		//------------------------------------------------------
		string strSearchKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PDF_OUTPUT]);
		string strPdfKbn = StringUtility.ToEmpty(Request[Constants.REQUEST_KEY_PDF_KBN]);

		//------------------------------------------------------
		// 対象注文情報取得
		//------------------------------------------------------
		Hashtable htParam = (Hashtable)Session[Constants.SESSION_KEY_PARAM];

		try
		{
			//------------------------------------------------------
			// クリア
			//------------------------------------------------------
			Response.Clear();

			// 納品書
			if (strPdfKbn == Constants.KBN_PDF_OUTPUT_ORDER_INVOICE)
			{
				//------------------------------------------------------
				// PDF作成
				//------------------------------------------------------
				OrderInvoiceCreater invoiceCreater = new OrderInvoiceCreater();
				invoiceCreater.Create(Response.OutputStream, strSearchKbn, htParam);

				//------------------------------------------------------
				// PDF情報出力
				//------------------------------------------------------
				// PDFで直接開く場合は「attachment」を「application/pdf」に変更
				Response.ContentType = "application/pdf";
				Response.AppendHeader("Content-Disposition", "attachment; filename=Invoice" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
				Response.Flush();
				Response.End();
			}
			// トータルピッキングリスト
			else if (strPdfKbn == Constants.KBN_PDF_OUTPUT_TOTAL_PICKING_LIST)
			{
				//------------------------------------------------------
				// PDF作成
				//------------------------------------------------------
				TotalPickingListCreater totalPickingListCreater = new TotalPickingListCreater();
				totalPickingListCreater.Create(Response.OutputStream, strSearchKbn, htParam);

				//------------------------------------------------------
				// PDF情報出力
				//------------------------------------------------------
				// PDFで直接開く場合は「attachment」を「application/pdf」に変更
				Response.ContentType = "application/pdf";
				Response.AppendHeader("Content-Disposition", "attachment; filename=PickingList" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
				Response.Flush();
				Response.End();

			}
			// 受注明細書
			else if (strPdfKbn == Constants.KBN_PDF_OUTPUT_ORDER_STATEMENT)
			{
				//------------------------------------------------------
				// PDF作成
				//------------------------------------------------------
				OrderStatementCreater orderstatementCreater = new OrderStatementCreater();
				orderstatementCreater.Create(Response.OutputStream, strSearchKbn, htParam);

				//------------------------------------------------------
				// PDF情報出力
				//------------------------------------------------------
				// PDFで直接開く場合は「attachment」を「application/pdf」に変更
				Response.ContentType = "application/pdf";
				Response.AppendHeader("Content-Disposition", "attachment; filename=OrderStatement" + DateTime.Now.ToString("yyyyMMdd") + ".pdf");
				Response.Flush();
				Response.End();

			}
			// 領収書
			else if (strPdfKbn == Constants.KBN_PDF_OUTPUT_RECEIPT)
			{
				// PDF作成
				new ReceiptCreater().Create(Response.OutputStream, strSearchKbn, htParam);

				//複数領収書をもとめたファイル名「Receip<yyyyMMddHHmmss>.pdf」
				//一件だけのファイル名「Receipt_<ordered_date>_<order_id>_<order_price_total>.pdf」
				var fileName = "";
				if (string.IsNullOrEmpty((string) htParam[Constants.HASH_KEY_ORDER_ID]) == false)
				{
					// 領収書発行メール送信
					SendReceiptMail(htParam, strSearchKbn);

					var order = new OrderService().Get((string)htParam[Constants.HASH_KEY_ORDER_ID]);
					fileName = string.Format(
						"_{0}_{1}_{2}",
						(order.DateCreated).ToString("yyyyMMddHHmmss"),
						(string)htParam[Constants.HASH_KEY_ORDER_ID],
						decimal.ToInt32(order.OrderPriceTotal));
				}
				else
				{
					fileName = DateTime.Now.ToString("yyyyMMddHHmmss");
				}

				// PDF情報出力
				// PDFで直接開く場合は「attachment」を「application/pdf」に変更
				Response.ContentType = "application/pdf";
				Response.AppendHeader("Content-Disposition", "attachment; filename=Receipt" + fileName + ".pdf");
				Response.Flush();
				Response.End();
			}
		}
		catch (ApplicationException ex)
		{
			// システムエラーがある場合はログを出力
			if (ex.InnerException != null)
			{
				w2.Common.Logger.AppLogger.WriteError(ex);
			}

			// エラーページへ
			Session[Constants.SESSION_KEY_ERROR_MSG] = ex.Message;
			Response.Redirect(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR);
		}
	}

	/// <summary>
	/// 領収書発行メール送信
	/// </summary>
	/// <param name="param">検索パラメタ</param>
	/// <param name="strSearchKbn">検索区分</param>
	public void SendReceiptMail(Hashtable param,string strSearchKbn)
	{
		var order = new OrderService().Get((string)param[Constants.HASH_KEY_ORDER_ID]);
		param.Add(Constants.FIELD_ORDER_ORDER_PRICE_TOTAL, order.OrderPriceTotal);
		param.Add(Constants.FIELD_ORDERITEM_DATE_CREATED, order.DateCreated);
		var filePath = new ReceiptCreater().CreateMailFile(strSearchKbn, param, (string)param[Constants.HASH_KEY_ORDER_ID]);
		SendMailCommon.SendReceipFiletMail((string)param[Constants.HASH_KEY_ORDER_ID], filePath);
		if (File.Exists(filePath))
		{
			File.Delete(filePath);
		}
	}

	/// <summary>
	/// ＰＤＦクリエータ起動(外部より直接呼出し）
	/// </summary>
	/// <param name="strSessionId">セッションID</param>
	/// <param name="strSearchKbn">検索区分</param>
	/// <param name="strPdfKbn">PDF区分</param>
	/// <param name="htParam">検索条件</param>
	public static void ExecPdfCreater(string strSessionId, string strSearchKbn, string strPdfKbn, Hashtable htParam)
	{
		//------------------------------------------------------
		// パラメタをシリアライズ化しファイルとして保存
		//------------------------------------------------------
		string strTmpDirPath = OrderInvoiceCreater.TempDirPath;
		ArrayList ParamKeys = new ArrayList();
		ArrayList ParamValues = new ArrayList();
		foreach (string str in htParam.Keys)
		{
			object obj = htParam[str];
			ParamKeys.Add(str);
			ParamValues.Add((obj != DBNull.Value) ? obj : null);
		}

		// ディレクトリが無い場合に作成
		if (Directory.Exists(strTmpDirPath) == false)
		{
			Directory.CreateDirectory(strTmpDirPath + @"\");
		}

		// パラメタキーXMLファイル作成
		using (StringWriter sw = new StringWriter())
		using (System.Xml.XmlTextWriter xw = new System.Xml.XmlTextWriter(sw))
		{
			System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(ArrayList));
			xs.Serialize(xw, ParamKeys);
			File.WriteAllText(strTmpDirPath + @"\ParamKeys.xml", sw.ToString(), Encoding.Unicode);
		}

		// パラメタ値XMLファイル作成
		using (StringWriter sw = new StringWriter())
		using (System.Xml.XmlTextWriter xw = new System.Xml.XmlTextWriter(sw))
		{
			System.Xml.Serialization.XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(ArrayList));
			xs.Serialize(xw, ParamValues);
			File.WriteAllText(strTmpDirPath + @"\ParamValues.xml", sw.ToString(), Encoding.Unicode);
		}

		//------------------------------------------------------
		// ＰＤＦクリエータ設定
		//------------------------------------------------------
		System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo();
		psi.FileName = Constants.PHYSICALDIRPATH_ORDER_PDF_CREATER_EXE;
		StringBuilder sbArgs = new StringBuilder();
		sbArgs.Append("\"").Append(strSessionId.Replace(@"\", @"\\")).Append("\"").Append(" ");
		sbArgs.Append(strSearchKbn).Append(" ");
		sbArgs.Append(strPdfKbn);
		psi.Arguments = sbArgs.ToString();

		//------------------------------------------------------
		// ＰＤＦクリエータプロセス起動
		//------------------------------------------------------
		System.Diagnostics.Process.Start(psi);
	}
}
