/*
=========================================================================================================
  Module      : 注文ＰＤＦクリエータ処理(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.IO;
using System.Xml.Serialization;
using w2.Common.Logger;
using w2.App.Common;
using w2.App.Common.Pdf.PdfCreater;

namespace w2.Commerce.Batch.OrderPdfCreater
{
	class Program
	{
		int m_iSqlServerTimeout = 30;

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			try
			{
				// 実体作成
				Program program = new Program();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				// アクティヴファイルのフルパスを受け取る
				program.CreatePdfFile(args[0], args[1], args[2]);

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				// エラーイベントログ出力
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			// 初期化
			Iniitialize();
		}

		/// <summary>
		/// 設定初期化
		/// </summary>
		private void Iniitialize()
		{
			try
			{
				//------------------------------------------------------
				// アプリケーション設定読み込み
				//------------------------------------------------------
				// アプリケーション名設定
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定			
				ConfigurationSetting csSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_OrderPdfCreater);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// SQLパス
				Constants.PHYSICALDIRPATH_SQL_STATEMENT = csSetting.GetAppStringSetting("Directory_SqlStatementXml");

				// PDFテンプレートファイルパス
				Constants.PHYSICALDIRPATH_PDF_TEMPLATE = csSetting.GetAppStringSetting("Directory_OrderPdfTemplateFilePath");

				// Valutext file path setting
				Constants.PHYSICALFILEPATH_VALUETEXT = Path.Combine(Constants.PHYSICALDIRPATH_COMMERCE_MANAGER, Constants.FILEPATH_XML_VALUE_TEXT);

				// SQLサーバータイムアウト
				m_iSqlServerTimeout = csSetting.GetAppIntSetting("SqlTimeout_Sec");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// PDFファイル作成
		/// </summary>
		/// <param name="strDirPath">出力先パス</param>
		/// <param name="strBaseName">ファイル名(拡張子なし)</param>
		/// <param name="strSessionId">セッションID</param>
		/// <param name="strSearchKbn">検索区分</param>
		/// <param name="strPdfKbn">PDF区分</param>
		private void CreatePdfFile(string strSessionId, string strSearchKbn, string strPdfKbn)
		{
			//------------------------------------------------------
			// パラメタ復元
			//------------------------------------------------------
			string strDirPath = OrderInvoiceCreater.ExportDirPath;
			string strTempDirPath = OrderInvoiceCreater.TempDirPath;
			Hashtable htParam = new Hashtable();
			ArrayList alParamKeys = null;
			using (FileStream fs1 = new FileStream(strTempDirPath + @"ParamKeys.xml", FileMode.Open))
			{
				XmlSerializer xs = new XmlSerializer(typeof(ArrayList));
				alParamKeys = (ArrayList)xs.Deserialize(fs1);
			}
			File.Delete(strTempDirPath + @"ParamKeys.xml");	// 削除

			ArrayList alParamValues = null;
			using (FileStream fs2 = new FileStream(strTempDirPath + @"ParamValues.xml", FileMode.Open))
			{
				XmlSerializer xs = new XmlSerializer(typeof(ArrayList));
				alParamValues = (ArrayList)xs.Deserialize(fs2);
			}
			File.Delete(strTempDirPath + @"ParamValues.xml");// 削除

			for (int iLoop = 0; iLoop < alParamKeys.Count; iLoop++)
			{
				htParam.Add(alParamKeys[iLoop], (alParamValues[iLoop] == null) ? DBNull.Value : alParamValues[iLoop]);
			}

			//------------------------------------------------------
			// PDF作成
			//------------------------------------------------------
			// 集約エラーハンドラでキャッチできないのでtry-catch
			try
			{
				if (Directory.Exists(strDirPath) == false)
				{
					Directory.CreateDirectory(strDirPath);
				}
				else
				{
					foreach (string str in Directory.GetFiles(strDirPath))
					{
						File.Delete(str);
					}
				}
				if (Directory.Exists(strTempDirPath) == false)
				{
					Directory.CreateDirectory(strTempDirPath);
				}

				// 納品書
				if (strPdfKbn == Constants.KBN_PDF_OUTPUT_ORDER_INVOICE)
				{
					OrderInvoiceCreater invoiceCreater = new OrderInvoiceCreater();
					invoiceCreater.SqlServerTimeout = m_iSqlServerTimeout;
					invoiceCreater.Create(strSearchKbn, htParam, strSessionId);
				}
				// トータルピッキングリスト
				else if (strPdfKbn == Constants.KBN_PDF_OUTPUT_TOTAL_PICKING_LIST)
				{
					TotalPickingListCreater totalPickingListCreater = new TotalPickingListCreater();
					totalPickingListCreater.SqlServerTimeout = m_iSqlServerTimeout;
					totalPickingListCreater.Create(strSearchKbn, htParam, strSessionId);
				}
				// 受注明細書
				else if (strPdfKbn == Constants.KBN_PDF_OUTPUT_ORDER_STATEMENT)
				{
					OrderStatementCreater orderstatementCreater = new OrderStatementCreater();
					orderstatementCreater.SqlServerTimeout = m_iSqlServerTimeout;
					orderstatementCreater.Create(strSearchKbn, htParam, strSessionId);
				}
				// 領収書
				else if (strPdfKbn == Constants.KBN_PDF_OUTPUT_RECEIPT)
				{
					var receiptCreater = new ReceiptCreater
					{
						SqlServerTimeout = m_iSqlServerTimeout
					};
					receiptCreater.Create(strSearchKbn, htParam, strSessionId);
				}
			}
			catch (Exception ex)
			{
				// ログを落としてスルー
				w2.Common.Logger.FileLogger.WriteError(ex);
			}
		}
	}
}
