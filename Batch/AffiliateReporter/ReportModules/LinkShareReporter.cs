/*
=========================================================================================================
  Module      : リンクシェア成果報告クラス(LinkShareReporter.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using w2.App.Common.Extensions.Currency;
using w2.Common.Sql;
using w2.App.Common.Option;

namespace w2.Commerce.Batch.AffiliateReporter
{
	public class LinkShareReporter : ReportModuleBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public LinkShareReporter()
			: base()
		{
			// なにもしない //
		}

		/// <summary>
		/// レポート報告
		/// </summary>
		public void Report()
		{
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				//------------------------------------------------------
				// 特定注文は対象から外す
				//------------------------------------------------------
				// 未送信 かつ キャンセルされている（同梱子注文でない）注文はフラグを落とす	
				using (SqlStatement sqlStatement = new SqlStatement("LinkShareReporter", "UpdateCancelOrderCoopStatus"))
				{
					int iUpdated = sqlStatement.ExecStatement(sqlAccessor);
				}

				// 未送信 かつ 同梱注文はフラグを落とす	
				using (SqlStatement sqlStatement = new SqlStatement("LinkShareReporter", "UpdateBundleOrderCoopStatus"))
				{
					int iUpdated = sqlStatement.ExecStatement(sqlAccessor);
				}

				//------------------------------------------------------
				// 対象注文取得（未送信 かつ 出荷済み注文取得）
				//------------------------------------------------------
				DataView dvReportItems = null;
				using (SqlStatement sqlStatement = new SqlStatement("LinkShareReporter", "GetTargetOrderItems"))
				{
					dvReportItems = sqlStatement.SelectSingleStatement(sqlAccessor);
				}

				//------------------------------------------------------
				// ファイル作成
				//------------------------------------------------------
				string strFileName = "LinkShareReport" + DateTime.Now.ToString("yyyyMMdd-HHmmss") + ".txt";
				string strTempFilePath = Constants.PHYSICALDIRPATH_TEMPDIR + strFileName;

				// Mutexで排他制御しながらファイル書き込み
				using (System.Threading.Mutex mtx = new System.Threading.Mutex(false, strTempFilePath.ToString().Replace("\\", "_") + ".FileWrite"))
				{
					try
					{
						mtx.WaitOne();

						using (StreamWriter swStreamWriter = new StreamWriter(strTempFilePath, false, Encoding.GetEncoding("Shift_JIS")))
						{
							StringBuilder sbLine = null;
							foreach (DataRowView drvReportItems in dvReportItems)
							{
								sbLine = new StringBuilder();
								sbLine.Append(TrimByBytes((string)drvReportItems[Constants.FIELD_ORDER_ORDER_ID], 40)).Append("\t");
								sbLine.Append(TrimByBytes((string)drvReportItems[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA1], 64)).Append("\t");
								sbLine.Append(DateTime.Parse((string)drvReportItems[Constants.FIELD_AFFILIATECOOPLOG_COOP_DATA2]).ToUniversalTime().ToString("yyyy-MM-dd/HH:mm:ss")).Append("\t");	// GMT(UTC)
								sbLine.Append(((DateTime)drvReportItems[Constants.FIELD_ORDER_ORDER_DATE]).ToUniversalTime().ToString("yyyy-MM-dd/HH:mm:ss")).Append("\t");	// GMT(UTC)
								sbLine.Append(TrimByBytes((string)drvReportItems[Constants.FIELD_ORDERITEM_SHOP_ID] + " " + (string)drvReportItems[Constants.FIELD_ORDERITEM_PRODUCT_ID] + " " + (string)drvReportItems[Constants.FIELD_ORDERITEM_VARIATION_ID], 40)).Append("\t");
								sbLine.Append((int)drvReportItems[Constants.FIELD_ORDERITEM_ITEM_QUANTITY]).Append("\t");
								// ↓売上金額（単価×数量の金額）はの税抜き金額を計算。
								var taxExcludedPrice = TaxCalculationUtility.GetPriceTaxExcluded(
									(decimal)drvReportItems[Constants.FIELD_ORDERITEM_ITEM_PRICE],
									(decimal)drvReportItems[Constants.FIELD_ORDERITEM_ITEM_PRICE_TAX]).ToPriceString();
								sbLine.Append(taxExcludedPrice).Append("\t");
								sbLine.Append("JPY").Append("\t");
								sbLine.Append("").Append("\t");
								sbLine.Append("").Append("\t");
								sbLine.Append(TrimByBytes((string)drvReportItems[Constants.FIELD_ORDEROWNER_OWNER_ZIP], 32)).Append("\t");
								sbLine.Append(TrimByBytes((string)drvReportItems[Constants.FIELD_ORDERITEM_PRODUCT_NAME], 100)).Append("\n");

								swStreamWriter.Write(sbLine.ToString());
							}
						}
					}
					finally
					{
						mtx.ReleaseMutex();	// Dispose()で呼ばれない模様。
					}
				}

				//------------------------------------------------------
				// ファイル送信
				//------------------------------------------------------
				if (Constants.PHYSICALDIRPATH_LINKSHARE_TRANSFER_EXE != "")	// テストの場合は実行ファイルは空
				{
					// コマンド作成
					StringBuilder sbCommandArgs = new StringBuilder();
					sbCommandArgs.Append(@" -dir ").Append(Constants.PHYSICALDIRPATH_LINKSHARE_TRANSFER_EXE);
					sbCommandArgs.Append(@" -file ").Append(strTempFilePath);
					sbCommandArgs.Append(@" -port 8088 ");

					// コマンド送信（リトライあり）
					int iErrorCount = 0;
					while (true)
					{
						// 実行
						Process pProcess = new Process();
						pProcess.StartInfo.FileName = Constants.PHYSICALDIRPATH_LINKSHARE_TRANSFER_EXE + "lstrans.exe";
						pProcess.StartInfo.Arguments = sbCommandArgs.ToString();
						// 以下、出力を読み取る設定
						pProcess.StartInfo.UseShellExecute = false;
						pProcess.StartInfo.RedirectStandardOutput = true;
						pProcess.StartInfo.RedirectStandardError = true;

						// アプリの実行開始
						pProcess.Start();

						// レスポンス取得（p.WaitForExit の前に呼び出さないとデッドロックの可能性あり）
						string strResponse = pProcess.StandardOutput.ReadToEnd().Replace("\r\r\n", "\n"); // 標準出力の読み取り

						// 終了するまで待つ
						pProcess.WaitForExit();

						// レスポンスが取得できたらブレイク
						if (strResponse.ToLower().Contains("successful"))
						{
							break;
						}
						// エラーの場合はカウントを増やし、限界値を超えたらメソッド終了
						else
						{
							iErrorCount++;

							w2.Common.Logger.FileLogger.WriteError(
								"lstrans.exe 実行時にエラーが発生しました(" + iErrorCount + "):" + strFileName
								+ "\r\n" + "\t" + pProcess.StandardError.ReadToEnd() + "\t" + strResponse);

							if (iErrorCount > 2)
							{
								return;
							}
						}
					}
				}

				//------------------------------------------------------
				// 終了ステータスへ更新
				//------------------------------------------------------
				// アフィリエイト区分＆マスタID列取得
				List<string> lAffiliateIds = new List<string>();
				foreach (DataRowView drvOrderItem in dvReportItems)
				{
					string strAffiliateId =
						(string)drvOrderItem[Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN]
						+ " " + (string)drvOrderItem[Constants.FIELD_AFFILIATECOOPLOG_MASTER_ID];

					if (lAffiliateIds.Contains(strAffiliateId) == false)
					{
						lAffiliateIds.Add(strAffiliateId);
					}
				}

				// 更新
				if (lAffiliateIds.Count != 0)
				{
					using (SqlStatement sqlStatement = new SqlStatement("LinkShareReporter", "UpdateReportedCoopStatus"))
					{
						Hashtable htInput = new Hashtable();
						foreach (string strAffiliateId in lAffiliateIds)
						{
							string[] strAffiliateIds = strAffiliateId.Split(' ');
							htInput[Constants.FIELD_AFFILIATECOOPLOG_AFFILIATE_KBN] = strAffiliateIds[0];
							htInput[Constants.FIELD_AFFILIATECOOPLOG_MASTER_ID] = strAffiliateIds[1];

							int iUpdated = sqlStatement.ExecStatement(sqlAccessor, htInput);
						}
					}
				}
			}
		}
	}
}
