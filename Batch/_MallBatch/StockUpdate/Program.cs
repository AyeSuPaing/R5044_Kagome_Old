/*
=========================================================================================================
  Module      : 商品・在庫情報アップロードメイン関数 (Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using AmazonMarketPlaceWebService;
using w2.App.Common;
using w2.App.Common.MallCooperation;
using w2.Commerce.MallBatch.StockUpdate.Amazon;
using w2.Commerce.MallBatch.StockUpdate.Andmall;
using w2.Commerce.MallBatch.StockUpdate.Common;
using w2.Commerce.MallBatch.StockUpdate.Ftp;
using w2.Commerce.MallBatch.StockUpdate.Lohaco;
using w2.Commerce.MallBatch.StockUpdate.Mall;
using w2.Commerce.MallBatch.StockUpdate.Properties;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.MallCooperationSetting;
using w2.SFTPClientWrapper;

namespace w2.Commerce.MallBatch.StockUpdate
{
	///**************************************************************************************
	/// <summary>
	///  商品・在庫情報アップロードメイン関数
	/// </summary>
	///**************************************************************************************
	class Program
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			try
			{
				//------------------------------------------------------
				// アプリケーション設定読み込み
				//------------------------------------------------------
				// アプリケーション名設定
				Constants.APPLICATION_NAME = Settings.Default.Application_Name;

				// アプリケーション共通の設定			
				ConfigurationSetting csSetting
					= new ConfigurationSetting(
						Settings.Default.ConfigFileDirPath,
						ConfigurationSetting.ReadKbn.C000_AppCommon,
						ConfigurationSetting.ReadKbn.C100_BatchCommon,
						ConfigurationSetting.ReadKbn.C300_StockUpdate,
						ConfigurationSetting.ReadKbn.C200_LiaiseAmazonMall);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				Constants.MKADV_OUTPUT_DIR = csSetting.GetAppStringSetting("Mkadv_OutputDir");
				Constants.TMP_SAVE_TO = csSetting.GetAppStringSetting("MailOrderGetter_TmpSaveTo");
				Constants.YAHOO_ADD_MINUTE = csSetting.GetAppIntSetting("StockUpdate_Yahoo_AddMinute");
				Constants.PROGRAMS_MKADV = csSetting.GetAppStringSetting("Program_Mkadv");
				Constants.YAHOO_APIFLG = csSetting.GetAppBoolSetting("StockUpdate_Yahoo_ApiFlg");
				Constants.YAHOO_ATTESTATIONKEY = csSetting.GetAppStringSetting("StockUpdate_Yahoo_AttestationKey");
				Constants.YAHOO_GETURL = csSetting.GetAppStringSetting("StockUpdate_Yahoo_GetUrl");
				Constants.YAHOO_STOREID = csSetting.GetAppStringSetting("StockUpdate_Yahoo_StoreId");
				Constants.YAHOO_TSADDMIN = csSetting.GetAppIntSetting("StockUpdate_Yahoo_TsAddMin");
				Constants.YAHOO_UPDATEURL = csSetting.GetAppStringSetting("StockUpdate_Yahoo_UpdateUrl");
				Constants.AMAZON_MALL_MWS_ENDPOINT = csSetting.GetAppStringSetting("LiaiseAmazonMall_amazon_mws_endpoint");
				Constants.SELLERSKU_FOR_NO_VARIATION = csSetting.GetAppStringSetting("SellerSKU_Link_Column_For_NoVariation");
				Constants.SELLERSKU_FOR_USE_VARIATION = csSetting.GetAppStringSetting("SellerSKU_Link_Column_For_UseVariation");
				Constants.FULFILLMENTLATENCY_FOR_NO_VARIATION = csSetting.GetAppStringSetting("Amazon_FulfillmentLatency_Column_For_NoVariation");
				Constants.FULFILLMENTLATENCY_FOR_USE_VARIATION = csSetting.GetAppStringSetting("Amazon_FulfillmentLatency_Column_For_UseVariation");
				Constants.FULFILLMENTLATENCY_DEFAULT = csSetting.GetAppIntSetting("Amazon_FulfillmentLatency_Default");
				Constants.ANDMALL_CLIENT_TEMP_DIR_PATH = csSetting.GetAppStringSetting("Andmall_Client_Temp_Dir_Path");
				Constants.ANDMALL_CLIENT_TEMP_FILE_BACKUP_DAY = csSetting.GetAppIntSetting("Andmall_Client_Temp_File_Backup_day");
				Constants.ANDMALL_SERVER_DIR_PATH = csSetting.GetAppStringSetting("Andmall_Server_Dir_Path");
				Constants.WRITE_DEBUG_LOG_ENABLED = csSetting.GetAppBoolSetting("LiaiseLohacoMall_WriteDebugLogEnabled");
				Constants.MASK_PERSONAL_INFO_ENABLED = csSetting.GetAppBoolSetting("LiaiseLohacoMall_MaskPersonalInfoEnabled");
			}
			catch (Exception ex)
			{
				throw new System.ApplicationException("config.xmlファイルの読み込みに失敗しました。\r\n" + ex.ToString());
			}
		}

		/// <summary>
		/// メイン処理
		/// </summary>
		/// <param name="strArgs">引数</param>
		static void Main(string[] strArgs)
		{
			try
			{
				// 商品・在庫アップロード処理実行
				Program program = new Program();

				AppLogger.WriteInfo("起動");

				program.UpdateStockInfo();

				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// 商品、在庫情報アップロード
		/// </summary>
		private void UpdateStockInfo()
		{
			// 実行区分
			const string EXE_ACTION_PRODUCT_INSERT = "INSERT";
			const string EXE_ACTION_PRODUCT_UPDATE = "UPDATE";
			const string EXE_ACTION_PRODUCT_STOCK_UPDATE = "STOCK_UPDATE";

			// モール監視ログ登録（バッチ起動）
			MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE, "", Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, "在庫更新処理を開始しました。");

			// 区切り線
			FileLogger.WriteInfo("************************************************************************************************");

			//------------------------------------------------------
			// ディレクトリが存在しなければ作成
			//------------------------------------------------------
			if (Directory.Exists(Constants.TMP_SAVE_TO) == false)
			{
				Directory.CreateDirectory(Constants.TMP_SAVE_TO);
			}

			//------------------------------------------------------
			// Andmall在庫連携処理
			//------------------------------------------------------
			var andmallSettings = new MallCooperationSettingService().GetValidByMallKbn(Constants.CONST_DEFAULT_SHOP_ID, Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL);
			foreach (var andmallSetting in andmallSettings)
			{
				try
				{
					// メンテナンス時間中はスキップ
					if ((andmallSetting.MaintenanceDateFrom != null) || (andmallSetting.MaintenanceDateTo != null))
					{
						var maintenanceDateFrom = andmallSetting.MaintenanceDateFrom ?? DateTime.MinValue;
						var maintenanceDateTo = andmallSetting.MaintenanceDateTo ?? DateTime.MaxValue;

						// 「メンテナンス開始日 <= 現在 < メンテナンス終了日」の時、次のモールの処理へ飛ばす
						if ((maintenanceDateFrom <= DateTime.Now) && (DateTime.Now < maintenanceDateTo))
						{
							// モール監視ログ登録（メンテナンス期間中）
							MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
								andmallSetting.MallId,
								Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
								"メンテナンス時間帯のため処理を実行しませんでした。");
							continue;
						}
					}

					// 在庫情報連携
					var andmallStockUpdate = new AndmallStockUpdate(andmallSetting);
					andmallStockUpdate.Exec();
				}
				catch (Exception ex)
				{
					FileLogger.WriteError("Andmall在庫更新時にエラーが発生したため、処理をスキップします");
					FileLogger.WriteError(ex.Message);

					// モール監視ログ登録（Andmall在庫更新に失敗：例外エラー）
					MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
						andmallSetting.MallId,
						 Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
						"在庫更新エラー：Andmall在庫更新処理に失敗しました。システム管理者にお問い合わせください。");
				}
			}

			//------------------------------------------------------
			// Amazon在庫連携処理
			//------------------------------------------------------
			var amazonMallSettings = new MallCooperationSettingService().GetValidByMallKbn(Constants.CONST_DEFAULT_SHOP_ID, Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON);
			foreach (var amazonMallSetting in amazonMallSettings)
			{
				try
				{
					// メンテナンス時間中はスキップ
					if ((amazonMallSetting.MaintenanceDateFrom != null) || (amazonMallSetting.MaintenanceDateTo != null))
					{
						var maintenanceDateFrom = amazonMallSetting.MaintenanceDateFrom ?? DateTime.MinValue;
						var maintenanceDateTo = amazonMallSetting.MaintenanceDateTo ?? DateTime.MaxValue;

						// 「メンテナンス開始日 <= 現在 < メンテナンス終了日」の時、次のモールの処理へ飛ばす
						if ((maintenanceDateFrom <= DateTime.Now) && (DateTime.Now < maintenanceDateTo))
						{
							// モール監視ログ登録（メンテナンス期間中）
							MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE, amazonMallSetting.MallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, "メンテナンス時間帯のため処理を実行しませんでした。");
							continue;
						}
					}

					// 在庫情報連携
					var amazonOrderApi = new AmazonApiService(amazonMallSetting);
					var amazonStockUpdate = new AmazonStockUpdate(amazonOrderApi, amazonMallSetting);
					amazonStockUpdate.Exec();
				}
				catch (Exception ex)
				{
					FileLogger.WriteError("Amazon在庫更新時にエラーが発生したため、処理をスキップします");
					FileLogger.WriteError(ex.Message);

					// モール監視ログ登録（Amazon在庫更新に失敗：例外エラー）
					MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE, amazonMallSetting.MallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, "在庫更新エラー：Amazon在庫更新処理に失敗しました。システム管理者にお問い合わせください。");
				}
			}

			//------------------------------------------------------
			// Lohaco在庫連携処理
			//------------------------------------------------------
			var lohacoMallSettings = new MallCooperationSettingService().GetValidByMallKbn(Constants.CONST_DEFAULT_SHOP_ID, Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_LOHACO);
			foreach (var lohacoMallSetting in lohacoMallSettings)
			{
				try
				{
					// メンテナンス時間中はスキップ
					if ((lohacoMallSetting.MaintenanceDateFrom != null) || (lohacoMallSetting.MaintenanceDateTo != null))
					{
						var maintenanceDateFrom = lohacoMallSetting.MaintenanceDateFrom ?? DateTime.MinValue;
						var maintenanceDateTo = lohacoMallSetting.MaintenanceDateTo ?? DateTime.MaxValue;

						// 「メンテナンス開始日 <= 現在 < メンテナンス終了日」の時、次のモールの処理へ飛ばす
						if ((maintenanceDateFrom <= DateTime.Now) && (DateTime.Now < maintenanceDateTo))
						{
							// モール監視ログ登録（メンテナンス期間中）
							MallWatchingLogManager.Insert(
								Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
								lohacoMallSetting.MallId,
								Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
								"メンテナンス時間帯のため処理を実行しませんでした。");
							continue;
						}
					}

					// 在庫情報連携
					var lohacoStockUpdate = new LohacoStockUpdate(lohacoMallSetting);
					lohacoStockUpdate.Exec();
				}
				catch (Exception ex)
				{
					FileLogger.WriteError("Lohaco在庫更新時にエラーが発生したため、処理をスキップします");
					FileLogger.WriteError(ex.Message, ex);

					// モール監視ログ登録（Lohaco在庫更新に失敗：例外エラー）
					MallWatchingLogManager.Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
						lohacoMallSetting.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
						"在庫更新エラー：Lohaco在庫更新処理に失敗しました。システム管理者にお問い合わせください。");
				}
			}

			//------------------------------------------------------
			// YahooAPI在庫更新処理
			//------------------------------------------------------
			if (Constants.YAHOO_APIFLG)
			{
				try
				{
					using (SqlAccessor sqlAccessor = new SqlAccessor())
					{
						sqlAccessor.OpenConnection();

						//------------------------------------------------------
						// Yahoo在庫ログ取り込み
						//------------------------------------------------------
						DataView dvMallCooperationSetting = null;
						using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "GetCooperationUpdateLogYahoo"))
						{
							dvMallCooperationSetting = sqlStatement.SelectSingleStatement(sqlAccessor);
						}
						if (dvMallCooperationSetting.Count > 0)
						{
							// ログ書き出し
							FileLogger.WriteInfo(dvMallCooperationSetting.Count + "件のYahooAPI実行開始");

							//------------------------------------------------------
							// YahooAPI設定値取得
							//------------------------------------------------------
							string strKey = Constants.YAHOO_ATTESTATIONKEY;
							string strGetUrl = Constants.YAHOO_GETURL;
							string strStoreId = Constants.YAHOO_STOREID;
							string strTs = DateTime.Now.AddMinutes(Constants.YAHOO_TSADDMIN).ToString("yyyyMMddHHmmss");
							// 在庫チェックを行わないのでコメント化
							// YahooAPI.GetProductStockYahooAPI(strGetUrl, strStoreId, strTs, strKey);

							//------------------------------------------------------
							// 見つかったログの数だけYahooAPI実行
							//------------------------------------------------------
							StringBuilder sbErrorMessage = new StringBuilder();
							StringBuilder sbWarningMessage = new StringBuilder();
							int iFailCount = 0;
							int iDeleteCount = 0;
							string strUpdateUrl = Constants.YAHOO_UPDATEURL;
							foreach (DataRowView drvMallCooperationSetting in dvMallCooperationSetting)
							{
								// ログ書き出し
								FileLogger.WriteDebug("ログNo：" + drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONUPDATELOG_LOG_NO].ToString() + "の処理実行");

								// パラメータ取得
								string strCode = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONUPDATELOG_PRODUCT_ID];
								string strSubCode = (string)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONUPDATELOG_VARIATION_ID];
								string strStock = drvMallCooperationSetting[Constants.FIELD_PRODUCTSTOCK_STOCK].ToString();

								//------------------------------------------------------
								// YahooAPI実行
								//------------------------------------------------------
								bool blRet = false;

								// １か月以内のログ情報をYahoo!API処理対象とする
								if ((DateTime)drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONUPDATELOG_DATE_CREATED] >= DateTime.Now.AddMonths(-1))
								{
									// Yahoo!APIで取得したXMLファイルには在庫未登録の商品在庫情報が取得出来なく、
									// 在庫の新規登録を行うには在庫チェックは不要となるため、在庫チェックをコメント化
									/* 在庫チェック
									if (YahooAPI.CheckProductStockYahooAPI(strCode, strSubCode))
									{
										// 在庫更新
										blRet = YahooAPI.UpdateProductStockYahooAPI(strUpdateUrl, strStoreId, strCode, strSubCode, strStock, strTs, strKey);
									}
									else
									{
										FileLogger.WriteDebug(strCode + ":" + strSubCode + "の商品IDがまだ確認できないため、更新スキップ");
										sbErrorMessage.Append((iFailCount == 0) ? "下記商品のYahooAPI在庫更新の処理結果を確認中です。\r\n" : "");
										sbErrorMessage.Append((iFailCount + 1).ToString());
										sbErrorMessage.Append(":商品ID[");
										sbErrorMessage.Append(strCode);
										sbErrorMessage.Append("] バリエーションID[");
										sbErrorMessage.Append(strSubCode);
										sbErrorMessage.Append("]\r\n");
									} */
									// 在庫更新
									blRet = YahooAPI.UpdateProductStockYahooAPI(strUpdateUrl, strStoreId, strCode, strSubCode, strStock, strTs, strKey);
								}
								// １か月より前のログ情報はYahoo!API対象外とする
								else
								{
									// アクションステータスを「99」に変更する（ログの更新対象から取り除く）
									using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "UpdateCooperationUpdateLogYahooExcept"))
									{
										Hashtable htInput = new Hashtable();
										htInput.Add(Constants.FIELD_MALLCOOPERATIONUPDATELOG_LOG_NO, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONUPDATELOG_LOG_NO]);

										sqlStatement.ExecStatement(sqlAccessor, htInput);
									}

									// ワーニングメッセージ作成
									sbWarningMessage.Append((iDeleteCount == 0) ? "下記商品のYahooAPI在庫更新の処理結果を取得できませんでした。\r\n" : "");
									sbWarningMessage.Append((iDeleteCount + 1).ToString());
									sbWarningMessage.Append(":No[");
									sbWarningMessage.Append(drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONUPDATELOG_LOG_NO].ToString());
									sbWarningMessage.Append("] 商品ID[");
									sbWarningMessage.Append(strCode);
									sbWarningMessage.Append("] バリエーションID[");
									sbWarningMessage.Append(strSubCode);
									sbWarningMessage.Append("]\r\n");

									// Yahoo!API連携対象外になった件数をカウント
									iDeleteCount++;

									continue;
								}

								//------------------------------------------------------
								// 成功ならアクションステータスを更新(失敗なら件数カウント）
								//------------------------------------------------------
								if (blRet)
								{
									using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "UpdateCooperationUpdateLogYahoo"))
									{
										Hashtable htInput = new Hashtable();
										htInput.Add(Constants.FIELD_MALLCOOPERATIONUPDATELOG_LOG_NO, drvMallCooperationSetting[Constants.FIELD_MALLCOOPERATIONUPDATELOG_LOG_NO]);

										sqlStatement.ExecStatement(sqlAccessor, htInput);
									}
								}
								else
								{
									sbErrorMessage.Append((iFailCount == 0) ? "下記商品のYahooAPI在庫更新に失敗しました。\r\n" : "");
									sbErrorMessage.Append((iFailCount + 1).ToString());
									sbErrorMessage.Append(":商品ID[");
									sbErrorMessage.Append(strCode);
									sbErrorMessage.Append("] バリエーションID[");
									sbErrorMessage.Append(strSubCode);
									sbErrorMessage.Append("]\r\n");

									// YahooAPIに失敗した件数をカウント
									iFailCount++;
								}
							}

							//------------------------------------------------------
							// メッセージ出力
							//------------------------------------------------------
							// 全て成功
							if (iFailCount == 0)
							{
								FileLogger.WriteInfo("YahooAPI在庫更新全て成功");

								// モール監視ログ登録（YahooAPI在庫更新に全て成功）
								MallWatchingLogManager.Insert(
									Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
									(string)dvMallCooperationSetting[0][Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID],
									Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
									"YahooAPI在庫更新処理に成功しました。");
							}
							// 失敗あり
							else
							{
								FileLogger.WriteError(iFailCount.ToString() + "件のYahooAPI在庫更新に失敗！！");

								// モール監視ログ登録（YahooAPI在庫更新に失敗）
								MallWatchingLogManager.Insert(
									Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
									(string)dvMallCooperationSetting[0][Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID],
									Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
									iFailCount.ToString() + "件のYahooAPI在庫更新処理に失敗しました。\r\n" + sbErrorMessage.ToString() + "※このメッセージが出力され続ける場合は、Yahoo!と管理サイトの商品情報・在庫管理の設定（在庫更新をする、しない）に不整合がないかご確認ください。");
							}

							// Yahoo!API在庫更新処理を対象外にした
							if (iDeleteCount > 0)
							{
								FileLogger.WriteInfo(iDeleteCount.ToString() + "件の在庫更新ログ情報をYahooAPI在庫更新対象外とした");

								// モール監視ログ登録（YahooAPI在庫更新を対象外とした）
								MallWatchingLogManager.Insert(
									Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
									(string)dvMallCooperationSetting[0][Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID],
									Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
									iDeleteCount.ToString() + "件の在庫更新ログ情報をYahoo!API在庫更新処理の対象外としました。（１か月間Yahoo!API在庫更新に失敗）\r\n" + sbWarningMessage.ToString());
							}
						}
					}
				}
				catch (Exception ex)
				{
					FileLogger.WriteError("YahooAPI在庫更新時にエラーが発生したため、処理をスキップします");
					FileLogger.WriteError(ex.Message);

					// モール監視ログ登録（YahooAPI在庫更新に失敗：例外エラー）
					MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE, "", Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, "在庫更新エラー：YahooAPI在庫更新処理に失敗しました。システム管理者にお問い合わせください。");
				}
			}

			// 楽天及びYahooモール設定取得（※AndMall、AmazonとLohacoは上部で在庫連携の実施済み）
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				//------------------------------------------------------
				// 商品情報との不整合ログを除外する（アクションステータスを「99」へ）
				//------------------------------------------------------
				using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "UpdateInconsistentLogExcept"))
				{
					int iUpdate = sqlStatement.ExecStatement(sqlAccessor);
				}

				//------------------------------------------------------
				// アクション区分決定（商品アップロード 又は 在庫アップロード）
				//------------------------------------------------------
				// モール対象ログ取得
				string strExeKbn = null;
				string strMasterKbn = null;
				string strActionKbn = null;
				bool blUseVariationFlg = true;
				var targetMallLog = new DataView();
				using (var sqlStatement = new SqlStatement("TempTable", "GetTargetLog"))
				{
					targetMallLog = sqlStatement.SelectSingleStatement(sqlAccessor);
				}
				if (targetMallLog.Count > 0)
				{
					// パラメータ取得
					strMasterKbn = StringUtility.ToEmpty(targetMallLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_MASTER_KBN]);
					strActionKbn = StringUtility.ToEmpty(targetMallLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_ACTION_KBN]);
					blUseVariationFlg = (StringUtility.ToEmpty(targetMallLog[0][Constants.FIELD_PRODUCT_USE_VARIATION_FLG]) == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE);

					// 実行処理分岐（在庫更新）
					if ((strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCTSTOCK) && (strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_UPDATE))
					{
						strExeKbn = EXE_ACTION_PRODUCT_STOCK_UPDATE;
						FileLogger.WriteInfo("実行アクションは、「在庫更新」を実行(バリエーション：" + (blUseVariationFlg ? "あり" : "なし") + ")");
					}
					// 実行処理分岐（商品登録）
					else if ((strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCT) && (strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_INSERT))
					{
						strExeKbn = EXE_ACTION_PRODUCT_INSERT;
						FileLogger.WriteInfo("実行アクションは、「商品登録」を実行(バリエーション：" + (blUseVariationFlg ? "あり" : "なし") + ")");
					}
					// 実行処理分岐（商品更新）
					else if ((strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCT) && (strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_UPDATE))
					{
						strExeKbn = EXE_ACTION_PRODUCT_UPDATE;
						FileLogger.WriteInfo("実行アクションは、「商品更新」を実行(バリエーション：" + (blUseVariationFlg ? "あり" : "なし") + ")");
					}
					// 該当なし（同一エラーが出力され続ける可能性があるため、エラーとする。）
					else
					{
						// モール監視ログ登録（不正なログエラー）
						MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE, "", Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, "システムエラー：ログ形式が不正なログが存在します。システム管理者にお問い合わせください。");

						FileLogger.WriteFatal("ログ形式が不正なログが存在します。（終了）【緊急】");
						Environment.Exit(0);
					}
				}
				else
				{
					// モール監視ログ登録（対象更新ログなし）
					MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE, "", Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, "在庫更新対象の商品はありませんでした。処理を終了します。");

					FileLogger.WriteInfo("実行対象ログが存在しませんでした。（終了）");
					Environment.Exit(0);
				}

				//------------------------------------------------------
				// 更新対象テーブルを作成（商品コンバータ側に渡すデータ）
				//------------------------------------------------------
				string strLogTableName = null;
				using (SqlStatement sqlStatement = new SqlStatement("TempTable", "GetHostId"))
				{
					strLogTableName = ((string)sqlStatement.SelectSingleStatement(sqlAccessor)[0][0]).Trim();
				}

				// 対象データ（w2_MallCooperationSetting）を一時テーブルに作成する
				TempTableBuilder tempTableBuilder = new TempTableBuilder(sqlAccessor, strLogTableName, strMasterKbn, strActionKbn, blUseVariationFlg);

				//------------------------------------------------------
				// 楽天及びYahooモール設定取得（※AndMall、AmazonとLohacoは上部で在庫連携の実施済み）
				//------------------------------------------------------
				MallConfig mallConfig = new MallConfig(sqlAccessor);

				//------------------------------------------------------
				// 対象データ詳細実行ログ出力
				//------------------------------------------------------
				foreach (MallConfigTip mallConfigTip in mallConfig.Configs)
				{
					switch (strExeKbn)
					{
						case EXE_ACTION_PRODUCT_STOCK_UPDATE:
							FileLogger.WriteInfo("実行アクションは、「在庫更新」を実行(バリエーション：" + ((blUseVariationFlg) ? "あり" : "なし") + ") 対象は、" + mallConfigTip.MallName + ":" + tempTableBuilder.GetUpdateCount(sqlAccessor, mallConfigTip.MallKbn, mallConfigTip.MallId, blUseVariationFlg) + "件");
							break;

						case EXE_ACTION_PRODUCT_INSERT:
							FileLogger.WriteInfo("実行アクションは、「商品登録」を実行(バリエーション：" + ((blUseVariationFlg) ? "あり" : "なし") + ") 対象は、" + mallConfigTip.MallName + ":" + tempTableBuilder.GetUpdateCount(sqlAccessor, mallConfigTip.MallKbn, mallConfigTip.MallId, blUseVariationFlg) + "件");
							break;

						case EXE_ACTION_PRODUCT_UPDATE:
							FileLogger.WriteInfo("実行アクションは、「商品更新」を実行(バリエーション：" + ((blUseVariationFlg) ? "あり" : "なし") + ") 対象は、" + mallConfigTip.MallName + ":" + tempTableBuilder.GetUpdateCount(sqlAccessor, mallConfigTip.MallKbn, mallConfigTip.MallId, blUseVariationFlg) + "件");
							break;
					}
				}

				//------------------------------------------------------
				// モール設定毎にメンテナンス期間をチェックする
				//------------------------------------------------------
				foreach (MallConfigTip mallConfigTip in mallConfig.Configs)
				{
					//------------------------------------------------------
					// メンテナンス設定チェック
					//------------------------------------------------------
					if ((mallConfigTip.MaintenanceDateFrom != null) || (mallConfigTip.MaintenanceDateTo != null))
					{
						DateTime? dtMaintenanceDateFrom = (mallConfigTip.MaintenanceDateFrom == null) ? DateTime.MinValue : mallConfigTip.MaintenanceDateFrom;
						DateTime? dtMaintenanceDateTo = (mallConfigTip.MaintenanceDateTo == null) ? DateTime.MaxValue : mallConfigTip.MaintenanceDateTo;

						// 「メンテナンス開始日 <= 現在 < メンテナンス終了日」の時、次のモールの処理へ飛ばす
						if ((dtMaintenanceDateFrom <= DateTime.Now) && (DateTime.Now < dtMaintenanceDateTo))
						{
							// モール監視ログ登録（メンテナンス期間中）
							MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE, mallConfigTip.MallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, "メンテナンス設定により処理を実行しませんでした。");

							tempTableBuilder.RollBack(sqlAccessor, mallConfigTip.MallKbn, mallConfigTip.MallId);
							continue;
						}
					}

					//------------------------------------------------------
					// モール設定チェック
					//------------------------------------------------------
					// YahooAPI在庫更新ONの場合
					if (Constants.YAHOO_APIFLG)
					{
						// Yahooの在庫更新は行わない
						if ((mallConfigTip.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO) && (strExeKbn == EXE_ACTION_PRODUCT_STOCK_UPDATE))
						{
							tempTableBuilder.RollBack(sqlAccessor, Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO, mallConfigTip.MallId);
							continue;
						}
					}

					// モール区分がAmazon、Lohaco、Andmallの場合はスキップ
					if ((mallConfigTip.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON)
						|| (mallConfigTip.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_LOHACO)
						|| (mallConfigTip.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_ANDMALL))
					{
						continue;
					}

					// 商品コンバータ出力ディレクトリ設定
					string strMkadvOutputDir = Constants.MKADV_OUTPUT_DIR + mallConfigTip.ShopId + @"\" + mallConfigTip.MallId + @"\";

					// 既存ファイル存在チェック（楽天のみ）
					if (mallConfigTip.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN)
					{
						// ファイルが存在する場合は、ログステータスを戻し次のモールへ進む
						if (FtpFileUpload.CheckRakutenRemoteFileExist(mallConfigTip))
						{
							tempTableBuilder.RollBack(sqlAccessor, Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN, mallConfigTip.MallId);
							FileLogger.WriteInfo("楽天チェック（NG）：アップロードを待機します。モールID[" + mallConfigTip.MallId + "]");
							continue;
						}
						else
						{
							FileLogger.WriteInfo("楽天チェック（OK）モールID[" + mallConfigTip.MallId + "]");
						}
					}

					//------------------------------------------------------
					// 実行用パラメータを取得し商品コンバータ実行
					//------------------------------------------------------
					List<Hashtable> lConvertParams = mallConfigTip.GetConvertParams(sqlAccessor, strMasterKbn, strActionKbn, blUseVariationFlg);
					foreach (Hashtable ht in lConvertParams)
					{
						// 共通の実行用パラメータ作成
						StringBuilder sbParam = new StringBuilder();
						sbParam.Append((string)ht[Constants.FIELD_MALLPRDCNV_ADTO_ID]);
						sbParam.Append(" \"");
						sbParam.Append(mallConfigTip.ShopId);
						sbParam.Append(@"\");
						sbParam.Append(mallConfigTip.MallId);
						sbParam.Append(@"\");
						sbParam.Append((string)ht[Constants.FIELD_MALLPRDCNV_PATH]);
						sbParam.Append((string)ht[Constants.FIELD_MALLPRDCNV_FILENAME]);
						sbParam.Append("\" ");
						sbParam.Append(
							((bool)ht[Constants.USE_VARIATION])
								? (Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION && (mallConfigTip.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN))
									? "w2_ProductViewStockUpdateSku"
									: "w2_ProductViewStockUpdate"
								: "w2_ProductStockUpdate");
						sbParam.Append(" ");

						// モール区分とバリエーション有無を判断し、実行用パラメータにテーブル名セットする
						switch (mallConfigTip.MallKbn)
						{
							case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN:
								sbParam.Append(((bool)ht[Constants.USE_VARIATION]) ? tempTableBuilder.ConvVTableNameRakuten : tempTableBuilder.ConvTableNameRakuten);
								break;

							case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO:
								sbParam.Append(((bool)ht[Constants.USE_VARIATION]) ? tempTableBuilder.ConvVTableNameYahoo : tempTableBuilder.ConvTableNameYahoo);
								break;
						}
						sbParam.Append(" ");
						sbParam.Append(mallConfigTip.MallId);
						sbParam.Append(" \"");
						sbParam.Append(
							(mallConfigTip.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN) && Constants.MALLCOOPERATION_RAKUTEN_SKUMIGRATION
								? blUseVariationFlg
									? mallConfigTip.RakutenSkuManagementIdOutputFormatForVariation.Replace(@"\", @"\\").Replace("\"", "\\\"")
									: mallConfigTip.RakutenSkuManagementIdOutputFormatForNormal.Replace(@"\", @"\\").Replace("\"", "\\\"")
								: string.Empty);
						sbParam.Append("\"");

						// 商品コンバータ実行
						Mkadv(sbParam.ToString());
					}

					//------------------------------------------------------
					// 商品コンバータID設定チェック
					//------------------------------------------------------
					if (lConvertParams.Count == 0)
					{
						StringBuilder sbErrorMessage = new StringBuilder();
						sbErrorMessage.Append("「バリエーション").Append((blUseVariationFlg) ? "あり" : "なし").Append("、");
						sbErrorMessage.Append((strMasterKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCT) ? "商品" : "在庫");
						sbErrorMessage.Append((strActionKbn == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_INSERT) ? "登録" : "更新");
						sbErrorMessage.Append("」の商品コンバータ設定が未設定です。\r\nモール連携設定画面の商品コンバータ設定情報をご確認ください。");

						// モール監視ログ登録（商品コンバータ未設定エラー）
						MallWatchingLogManager.Insert(
							Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
							mallConfigTip.MallId,
							Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
							"商品コンバータ未設定エラー：モール[" + mallConfigTip.MallName + "]の商品コンバータ設定値を取得できませんでした。\r\n" + sbErrorMessage.ToString());

						FileLogger.WriteError("モール[" + mallConfigTip.MallName + "]の商品コンバータ設定値の取得に失敗しました。");
						tempTableBuilder.RollBack(sqlAccessor, mallConfigTip.MallKbn, mallConfigTip.MallId);
						continue;
					}

					//------------------------------------------------------
					// ファイル存在チェック、件数整合性チェック
					//------------------------------------------------------
					// ファイル出力先パスを取得する（対象が1件以上の場合）
					bool blIsUploadTarget1 = false;
					string strMkadvOutPutPath1 = null;
					if (lConvertParams.Count >= 1)
					{
						strMkadvOutPutPath1 = strMkadvOutputDir + (string)lConvertParams[0][Constants.FIELD_MALLPRDCNV_PATH] + (string)lConvertParams[0][Constants.FIELD_MALLPRDCNV_FILENAME];
						FileLogger.WriteInfo("<" + mallConfigTip.MallName + ">：ファイル出力先「" + strMkadvOutPutPath1 + "」");	// モール連携設定
						if (FtpFileUpload.CheckCsvCount(strMkadvOutPutPath1))
						{
							blIsUploadTarget1 = true;
						}
						else
						{
							tempTableBuilder.RollBack(sqlAccessor, mallConfigTip.MallKbn, mallConfigTip.MallId);
							File.Delete(strMkadvOutPutPath1);
							DeleteProductConverterFiles(
								(string)lConvertParams[0][Constants.FIELD_MALLPRDCNV_ADTO_ID],
								(string)lConvertParams[0][Constants.FIELD_MALLPRDCNV_FILENAME],
								mallConfigTip,
								sqlAccessor
								);
							FileLogger.WriteInfo("出力データ件数が0件のため、モール[" + mallConfigTip.MallName + "]のアップロード処理をスキップします。\nファイル「Root/" + strMkadvOutPutPath1 + "」を削除しました。");
							continue;
						}
					}
					else
					{
						strMkadvOutPutPath1 = "";
					}

					// ファイル出力先パスを取得する（対象が2件以上の場合）
					bool blIsUploadTarget2 = false;
					string strMkadvOutPutPath2 = null;
					if (lConvertParams.Count >= 2)
					{
						strMkadvOutPutPath2 = strMkadvOutputDir + (string)lConvertParams[1][Constants.FIELD_MALLPRDCNV_PATH] + (string)lConvertParams[1][Constants.FIELD_MALLPRDCNV_FILENAME];
						FileLogger.WriteInfo("<" + mallConfigTip.MallName + ">：ファイル出力先「" + strMkadvOutPutPath2 + "」");	// モール連携設定
						if (FtpFileUpload.CheckCsvCount(strMkadvOutPutPath2))
						{
							blIsUploadTarget2 = true;
						}
						else
						{
							tempTableBuilder.RollBack(sqlAccessor, mallConfigTip.MallKbn, mallConfigTip.MallId);
							File.Delete(strMkadvOutPutPath2);
							DeleteProductConverterFiles(
								(string)lConvertParams[1][Constants.FIELD_MALLPRDCNV_ADTO_ID],
								(string)lConvertParams[1][Constants.FIELD_MALLPRDCNV_FILENAME],
								mallConfigTip,
								sqlAccessor
								);
							FileLogger.WriteInfo("出力データ件数が0件のため、モール[" + mallConfigTip.MallName + "]のアップロード処理をスキップします。\nファイル「Root/" + strMkadvOutPutPath2 + "」を削除しました。");
							continue;
						}
					}
					else
					{
						strMkadvOutPutPath2 = "";
					}

					// ファイル出力先パスを取得する（更新ターゲット３（楽天カテゴリ）がtrueの場合）
					bool blIsUploadTarget3 = ((mallConfigTip.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN) && (strExeKbn == EXE_ACTION_PRODUCT_INSERT));
					string strMkadvOutPutPath3 = null;
					if (blIsUploadTarget3)
					{
						strMkadvOutPutPath3 = strMkadvOutputDir + (string)lConvertParams[(lConvertParams.Count - 1)][Constants.FIELD_MALLPRDCNV_PATH] + (string)lConvertParams[(lConvertParams.Count - 1)][Constants.FIELD_MALLPRDCNV_FILENAME];
						FileLogger.WriteInfo("<" + mallConfigTip.MallName + ">：ファイル出力先「" + strMkadvOutPutPath3 + "」");	// モール連携設定

						// 楽天カテゴリ設定がない場合、ログを出力
						if (FtpFileUpload.CheckCsvCount(strMkadvOutPutPath3) == false)
						{
							File.Delete(strMkadvOutPutPath3);
							DeleteProductConverterFiles(
								(string)lConvertParams[(lConvertParams.Count - 1)][Constants.FIELD_MALLPRDCNV_ADTO_ID],
								(string)lConvertParams[(lConvertParams.Count - 1)][Constants.FIELD_MALLPRDCNV_FILENAME],
								mallConfigTip,
								sqlAccessor
								);
							FileLogger.WriteInfo("出力データ件数が0件のため、モール[" + mallConfigTip.MallName + "]のカテゴリアップロード処理をスキップします。\nファイル「Root/" + strMkadvOutPutPath3 + "」を削除しました。");
							blIsUploadTarget3 = false;
						}
					}
					else
					{
						strMkadvOutPutPath3 = "";
					}

					//------------------------------------------------------
					// FTPアップロード
					//------------------------------------------------------
					try
					{
						bool blUploadSuccess = false;
						//------------------------------------------------------
						// FTPアップロード実行
						//------------------------------------------------------
						// ターゲット１がアップロード対象の場合
						if (blIsUploadTarget1)
						{
							blUploadSuccess = mallConfigTip.IsSftp
								? UploadStockBySftp(
									mallConfigTip,
									(string)lConvertParams[0][Constants.FIELD_MALLPRDCNV_FILENAME],
									strMkadvOutPutPath1)
								: FtpFileUpload.FtpUpload(
									mallConfigTip.PathFtpUpload + (string)lConvertParams[0][Constants.FIELD_MALLPRDCNV_FILENAME],
									strMkadvOutPutPath1,
									mallConfigTip.FtpUserName,
									mallConfigTip.FtpPassWord,
									true);
						}
						
						// ターゲット２がアップロード対象の場合
						if ((blUploadSuccess) && (blIsUploadTarget2))
						{
							if (mallConfigTip.IsSftp)
							{
								UploadStockBySftp(
									mallConfigTip,
									(string)lConvertParams[1][Constants.FIELD_MALLPRDCNV_FILENAME],
									strMkadvOutPutPath2);
							}
							else
							{
								FtpFileUpload.FtpUpload(
									mallConfigTip.PathFtpUpload + (string)lConvertParams[1][Constants.FIELD_MALLPRDCNV_FILENAME],
									strMkadvOutPutPath2,
									mallConfigTip.FtpUserName,
									mallConfigTip.FtpPassWord,
									true);
							}
						}

						// ターゲット３がアップロード対象の場合
						if ((blUploadSuccess) && (blIsUploadTarget3))
						{
							if (mallConfigTip.IsSftp)
							{
								UploadStockBySftp(
									mallConfigTip,
									(string)lConvertParams[(lConvertParams.Count - 1)][Constants.FIELD_MALLPRDCNV_FILENAME],
									strMkadvOutPutPath3);
							}
							else
							{
								FtpFileUpload.FtpUpload(
									mallConfigTip.PathFtpUpload + (string)lConvertParams[(lConvertParams.Count - 1)][Constants.FIELD_MALLPRDCNV_FILENAME],
									strMkadvOutPutPath3,
									mallConfigTip.FtpUserName,
									mallConfigTip.FtpPassWord,
									true);
							}
						}

						//------------------------------------------------------
						// FTPアップロード後処理
						//------------------------------------------------------
						// FTPアップロード成功判別
						if (blUploadSuccess)
						{
							// アップロード成功
							FileLogger.WriteInfo("モール[" + mallConfigTip.MallName + "]のアップロード成功");
							tempTableBuilder.Complete(sqlAccessor, mallConfigTip.MallKbn, mallConfigTip.MallId);

							// モール監視ログ登録（アップロード成功）
							MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE, mallConfigTip.MallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, "モール[" + mallConfigTip.MallName + "]のFTPアップロードに成功しました。");

							// 楽天の場合、アップロード済みファイルをリネームして履歴を残す
							if (mallConfigTip.MallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN)
							{
								// ファイルリネーム処理
								try
								{
									if (blIsUploadTarget1)
									{
										RenameCopyUploadFile(
											strMkadvOutPutPath1,
											(string)lConvertParams[0][Constants.FIELD_MALLPRDCNV_PATH],
											(string)lConvertParams[0][Constants.FIELD_MALLPRDCNV_FILENAME],
											int.Parse((string)lConvertParams[0][Constants.FIELD_MALLPRDCNV_ADTO_ID]),
											mallConfigTip,
											sqlAccessor);
									}
									if (blIsUploadTarget2)
									{
										RenameCopyUploadFile(
											strMkadvOutPutPath2,
											(string)lConvertParams[1][Constants.FIELD_MALLPRDCNV_PATH],
											(string)lConvertParams[1][Constants.FIELD_MALLPRDCNV_FILENAME],
											int.Parse((string)lConvertParams[1][Constants.FIELD_MALLPRDCNV_ADTO_ID]),
											mallConfigTip,
											sqlAccessor);
									}
									if (blIsUploadTarget3)
									{
										RenameCopyUploadFile(
											strMkadvOutPutPath3,
											(string)lConvertParams[(lConvertParams.Count - 1)][Constants.FIELD_MALLPRDCNV_PATH],
											(string)lConvertParams[(lConvertParams.Count - 1)][Constants.FIELD_MALLPRDCNV_FILENAME],
											int.Parse((string)lConvertParams[(lConvertParams.Count - 1)][Constants.FIELD_MALLPRDCNV_ADTO_ID]),
											mallConfigTip,
											sqlAccessor);
									}
								}
								catch (Exception ex)
								{
									// モール監視ログ登録（ファイルリネームコピー失敗）
									MallWatchingLogManager.Insert(
										Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
										mallConfigTip.MallId,
										Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING, "ファイル保存エラー：モール[" + mallConfigTip.MallName + "]のFTPアップロード済みファイルの履歴保存に失敗しました。");

									FileLogger.WriteError(ex.Message);
								}
							}
						}
						else
						{
							throw new Exception();
						}
					}
					catch (Exception ex)
					{
						// モール監視ログ登録（FTP通信エラー）
						MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE, mallConfigTip.MallId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, "FTP通信エラー：モール[" + mallConfigTip.MallName + "]のアップロードに失敗しました。\r\nモール連携設定メニューにて設定内容を再度ご確認ください。");

						FileLogger.WriteError("モール[" + mallConfigTip.MallName + "]のアップロード中にエラーが発生しました。[" + mallConfigTip.MallName + "]に対するアップロード処理をスキップします。");
						FileLogger.WriteError(ex.Message);
						tempTableBuilder.RollBack(sqlAccessor, mallConfigTip.MallKbn, mallConfigTip.MallId);
						continue;
					}
				}

				//------------------------------------------------------
				// モール監視ログ（ログ停滞期間チェック）
				//------------------------------------------------------
				DataView dvCheckOldestLog = null;
				using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "CheckOldestLog"))
				{
					dvCheckOldestLog = sqlStatement.SelectSingleStatement(sqlAccessor);
				}
				if (dvCheckOldestLog.Count > 0)
				{
					// 最古ログがシステム日付より２日以前のものであればログ出力する
					if ((DateTime)dvCheckOldestLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_DATE_CREATED] < DateTime.Now.AddDays(-2))
					{
						StringBuilder sbLogMessage = new StringBuilder();
						sbLogMessage.Append("ログ停滞エラー：商品と在庫の登録・更新ログが停滞しています。システム管理者にお問い合わせください。\r\n");
						sbLogMessage.Append("----------連携ログ情報----------\r\n");
						sbLogMessage.Append("No：").Append(dvCheckOldestLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_LOG_NO].ToString()).Append("\r\n");
						sbLogMessage.Append("モールID：").Append((string)dvCheckOldestLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_MALL_ID]).Append("\r\n");
						sbLogMessage.Append("区分：").Append(((string)dvCheckOldestLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_MASTER_KBN] == Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCT) ? "商品" : "在庫");
						sbLogMessage.Append(((string)dvCheckOldestLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_ACTION_KBN] == Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_INSERT) ? "登録" : "更新").Append("\r\n");
						sbLogMessage.Append("商品ID：").Append((string)dvCheckOldestLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_PRODUCT_ID]).Append("\r\n");
						sbLogMessage.Append("バリエーションID：").Append((string)dvCheckOldestLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_VARIATION_ID]).Append("\r\n");
						sbLogMessage.Append("バリエーション有無：").Append(((string)dvCheckOldestLog[0][Constants.FIELD_PRODUCT_USE_VARIATION_FLG] == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE) ? "あり" : "なし").Append("\r\n");
						sbLogMessage.Append("作成日：").Append(dvCheckOldestLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_DATE_CREATED].ToString()).Append("\r\n");

						MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE, "", Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, sbLogMessage.ToString());
						FileLogger.WriteError("ログ停滞エラー：ログNo " + dvCheckOldestLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_LOG_NO].ToString() + " ログ作成日 " + dvCheckOldestLog[0][Constants.FIELD_MALLCOOPERATIONUPDATELOG_DATE_CREATED].ToString());
					}
				}

				//------------------------------------------------------
				// モール監視ログ（在庫未更新情報チェック）
				//------------------------------------------------------
				DataView dvProductStockErrorInfo = null;
				using (SqlStatement sqlStatement = new SqlStatement("MallCooperationSetting", "GetProductStockErrorInfo"))
				{
					dvProductStockErrorInfo = sqlStatement.SelectSingleStatement(sqlAccessor);
				}
				if (dvProductStockErrorInfo.Count > 0)
				{
					StringBuilder sbLogMessage = new StringBuilder();
					sbLogMessage.Append(DateTime.Now.ToString()).Append(" 時点、在庫情報が ").Append(dvProductStockErrorInfo.Count.ToString()).Append(" 件未更新です。\r\n");

					int iIndex = 1;
					foreach (DataRowView drvProductStockErrorInfo in dvProductStockErrorInfo)
					{
						sbLogMessage.Append(iIndex.ToString()).Append(" ------------------------------\r\n");
						sbLogMessage.Append("　モールID：").Append((string)drvProductStockErrorInfo[Constants.FIELD_MALLCOOPERATIONUPDATELOG_MALL_ID]).Append("\r\n");
						sbLogMessage.Append("　商品名：").Append((string)drvProductStockErrorInfo[Constants.FIELD_PRODUCT_NAME]).Append(" ").Append((string)drvProductStockErrorInfo[Constants.FIELD_PRODUCTVARIATION_VARIATION_NAME1]).Append("\r\n");
						sbLogMessage.Append("　商品ID：").Append((string)drvProductStockErrorInfo[Constants.FIELD_PRODUCTSTOCK_PRODUCT_ID]).Append("\r\n");
						sbLogMessage.Append("　バリエーションID：").Append((string)drvProductStockErrorInfo[Constants.FIELD_PRODUCTSTOCK_VARIATION_ID]).Append("\r\n");
						sbLogMessage.Append("　バリエーション有無：").Append(((string)drvProductStockErrorInfo[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE) ? "あり" : "なし").Append("\r\n");
						sbLogMessage.Append("　在庫数：").Append(drvProductStockErrorInfo[Constants.FIELD_PRODUCTSTOCK_STOCK].ToString()).Append("\r\n");

						iIndex++;
					}

					MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE, "", Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING, sbLogMessage.ToString());
				}
			}
		}

		/// <summary>
		/// 商品コンバータファイル削除
		/// </summary>
		/// <param name="strAdtoId">商品コンバータID</param>
		/// <param name="strAdtoFileName">ファイル名</param>
		/// <param name="mallConfigTip">モール設定情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private int DeleteProductConverterFiles(
			string strAdtoId,
			string strAdtoFileName,
			MallConfigTip mallConfigTip,
			SqlAccessor sqlAccessor
			)
		{
			using (SqlStatement sqlStatement = new SqlStatement("AdFiles", "DeleteItAdFiles"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLPRDCNVFILES_ADTO_ID, strAdtoId);
				htInput.Add(Constants.FIELD_MALLPRDCNVFILES_PATH, mallConfigTip.ShopId + "/" + mallConfigTip.MallId + "/" + strAdtoFileName);

				return sqlStatement.ExecStatement(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// ファイルリネームコピー
		/// </summary>
		/// <param name="strMkadvOutPutPath">商品コンバータ出力パス</param>
		/// <param name="strAdtoFileName">ファイル名</param>
		/// <param name="strAdtoId">商品コンバータID</param>
		/// <param name="mallConfigTip">モール設定情報</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <remarks>アップロード済みファイルをリネームして履歴保存する</remarks>
		private void RenameCopyUploadFile(
			string strMkadvOutPutPath,
			string strAdtoPath,
			string strAdtoFileName,
			int strAdtoId,
			MallConfigTip mallConfigTip,
			SqlAccessor sqlAccessor
			)
		{
			// ファイルをリネームコピーする
			string strFileName = Path.GetDirectoryName(strMkadvOutPutPath) + "/" + DateTime.Now.ToString("yyyyMMddHHmmss_") + strAdtoFileName;
			File.Copy(strMkadvOutPutPath, strFileName, true);
			
			// 管理側商品コンバータ設定画面から参照可能な履歴として残す
			using (SqlStatement sqlStatement = new SqlStatement("AdFiles", "InsertItAdFiles"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MALLPRDCNVFILES_ADTO_ID, strAdtoId);
				htInput.Add(Constants.FIELD_MALLPRDCNVFILES_PATH, mallConfigTip.ShopId + "/" + mallConfigTip.MallId + "/" + strAdtoPath + Path.GetFileName(strFileName));

				sqlStatement.ExecStatement(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// 商品コンバータ実行
		/// </summary>
		/// <param name="strArg">引数</param>
		/// <remarks>商品コンバータバッチを実行する</remarks>
		private void Mkadv(string strArg)
		{
			FileLogger.WriteInfo("商品コンバータ実行：" + strArg);
			Console.WriteLine("> " + Constants.PROGRAMS_MKADV + " " + strArg);

			try
			{
				string strPathExe = Constants.PROGRAMS_MKADV;

				ProcessStartInfo psInfo = new ProcessStartInfo();
				psInfo.FileName = strPathExe; // 実行するファイル
				psInfo.CreateNoWindow = true; // コンソール・ウィンドウを開かない
				psInfo.UseShellExecute = false; // シェル機能を使用しない
				psInfo.Arguments = strArg;
				psInfo.CreateNoWindow = true; // コマンドプロンプトを出さない
				psInfo.RedirectStandardOutput = true; // 標準出力をリダイレクトする
				psInfo.RedirectStandardInput = true; // 標準入力を　　　〃
				psInfo.RedirectStandardError = true; // 標準エラー出力を　　　〃

				Process process = Process.Start(psInfo);
				process.OutputDataReceived += new DataReceivedEventHandler(FtpFileUpload.ProcessDataReceived);
				process.ErrorDataReceived += new DataReceivedEventHandler(FtpFileUpload.ProcessDataReceived);
				process.BeginOutputReadLine();
				process.BeginErrorReadLine();
				process.WaitForExit();
				if (process.ExitCode != 0) throw new Exception("商品コンバータ実行：異常終了");
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.ToString());
				throw ex;
			}
			FileLogger.WriteInfo("商品コンバータ実行：正常終了");
		}

		/// <summary>
		/// 在庫をSFTPによるファイルアップロードで更新する
		/// </summary>
		/// <param name="mallConfig">メール設定</param>
		/// <param name="uploadFileName">アップロードするファイル名</param>
		/// <param name="fromClientFilePath">クライアント側のファイルパス</param>
		/// <returns>成功 : true, 失敗 : false</returns>
		private bool UploadStockBySftp(MallConfigTip mallConfig,string uploadFileName, string fromClientFilePath)
		{
			var sftpManager = new SFTPManager(mallConfig.SFTPSettings);

			try
			{
				// HACK : hoge/foo\piyo\bar.csvみたいになってるのでそれを直す
				var slashCorrectPath = new FileInfo(fromClientFilePath).FullName;

				// 在庫連携データのアップロード
				sftpManager.CreateSFTPClient().PutFile(Path.Combine("./", mallConfig.FtpUploadDir, uploadFileName), slashCorrectPath);
				return true;
			}
			catch (Exception ex)
			{
				MallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					mallConfig.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
					"SFTP接続に失敗",
					"SFTP接続に失敗しました。");
				
				return false;
			}
		}

		/// <summary>
		/// モール監視ログマネージャ
		/// </summary>
		public static MallWatchingLogManager MallWatchingLogManager
		{
			get { return m_mallWatchingLogManager; }
		}
		public static MallWatchingLogManager m_mallWatchingLogManager = new MallWatchingLogManager();
	}
}
