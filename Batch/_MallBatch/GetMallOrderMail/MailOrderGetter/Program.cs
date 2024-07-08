/*
=========================================================================================================
  Module      : メール注文取得／注文メール取込 (program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using w2.App.Common;
using w2.App.Common.Mall.Rakuten;
using w2.Commerce.MallBatch.MailOrderGetter.MailAnalyze;
using w2.Commerce.MallBatch.MailOrderGetter.Process.Base;
using w2.Commerce.MallBatch.MailOrderGetter.Process.Rakuten;
using w2.Commerce.MallBatch.MailOrderGetter.Process.Yahoo;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;

namespace w2.Commerce.MallBatch.MailOrderGetter
{
	///**************************************************************************************
	/// <summary>
	/// プログラムクラス
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
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定			
				ConfigurationSetting csSetting
					= new ConfigurationSetting(
						Properties.Settings.Default.ConfigFileDirPath,
						ConfigurationSetting.ReadKbn.C000_AppCommon,
						ConfigurationSetting.ReadKbn.C100_BatchCommon,
						ConfigurationSetting.ReadKbn.C300_MailOrderGetter);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// メール送信先設定
				Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = csSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = csSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Bcc");

				// 商品ID
				Constants.PRODUCT_KEY = csSetting.GetAppStringSetting("MailOrderGetter_ProductKey");

				Constants.TAX_ROUNDTYPE = csSetting.GetAppStringSetting("MailOrderGetter_Tax_RoundType");

				// 予約注文ご確認メール取込機能
				Constants.RAKUTEN_RESERVATION_ORDER_MAIL_ENABLED = csSetting.GetAppBoolSetting("MailOrderGetter_Rakuten_ReservationOrderMail_Enabled");
				// Rakuten overseas order mail enabled
				Constants.RAKUTEN_OVERSEA_ORDER_MAIL_ENABLED = csSetting.GetAppBoolSetting("MailOrderGetter_Rakuten_OverseaOrderMail_Enabled");
				// Rakuten oversea order extend status field
				Constants.RAKUTEN_OVERSEAS_ORDER_EXTEND_STATUS_FIELD = csSetting.GetAppStringSetting("MailOrderGetter_Rakuten_OverseasOrderMail_ExtendStatus");
				// Rakuten shipping tomorrow order extend status field
				Constants.RAKUTEN_SHIPPING_TOMORROW_ORDER_EXTEND_STATUS_FIELD = csSetting.GetAppStringSetting("MailOrderGetter_Rakuten_ShippingTomorrowOrderMail_ExtendStatus");
				// 楽天：注文通知メール件名
				Constants.RAKUTEN_ORDER_MAIL_SUBJECT = csSetting.GetAppStringSetting("MailOrderGetter_Rakuten_Order_Mail_Subject");
				// 楽天：注文通知メール件名(RakutenGlobalMarket)
				Constants.RAKUTEN_GLOBAL_MARKET_ORDER_MAIL_SUBJECT = csSetting.GetAppStringSetting("MailOrderGetter_Rakuten_Global_Market_Order_Mail_Subject");
				// 楽天：予約購入の受注確定時メール件名
				Constants.RAKUTEN_RESERVATION_ORDER_MAIL_SUBJECT = csSetting.GetAppStringSetting("MailOrderGetter_Rakuten_Reservation_Order_Mail_Subject");
				// 楽天:注文キャンセルメール件名
				Constants.RAKUTEN_CANCEL_ORDER_MAIL_SUBJECT = csSetting.GetAppStringSetting("MailOrderGetter_Rakuten_Cancel_Order_Mail_Subject");
				// 取り込み保留対象ステータス
				Constants.RAKUTEN_RETRY_IMPORT_ORDER_PROGRESS = csSetting.GetAppStringSetting("MailOrderGetter_Rakuten_RetryImport_OrderProgress").Split(',');
				// 取り込み保留対象のリトライ期間(時間)
				Constants.RAKUTEN_RETRY_IMPORT_PERIOD = csSetting.GetAppIntSetting("MailOrderGetter_Rakuten_RetryImport_Period");
				// 取り込み対象ステータス
				Constants.RAKUTEN_IMPORT_ORDER_PROGRESS = csSetting.GetAppStringSetting("MailOrderGetter_Rakuten_Import_OrderProgress").Split(',');
				// 取り込み対象ステータス
				Constants.RAKUTEN_CANCEL_ORDER_PROGRESS = csSetting.GetAppStringSetting("MailOrderGetter_Rakuten_Cancel_OrderProgress").Split(',');
				// 楽天：注文メール内の受注番号を含む行の正規表現
				Constants.RAKUTEN_REGULAR_EXPRESSION_FOR_LINE = csSetting.GetAppStringSetting("MailOrderGetter_Rakuten_Regular_Expression_For_Line");
				// 楽天：注文メール内の受注番号の正規表現
				Constants.RAKUTEN_REGULAR_EXPRESSION_FOR_ORDER_NO = csSetting.GetAppStringSetting("MailOrderGetter_Rakuten_Regular_Expression_For_Order_No");
				// 楽天：APIの配送希望時間帯の指定なしの文言
				Constants.RAKUTEN_API_SHIPPING_TERM_NONE_COMMENT = csSetting.GetAppStringSettingList("MailOrderGetter_Rakuten_Api_Shipping_Term_None_Comment").ToArray(); ;
				
				/// <summary>Yahoo：注文IDの取込フォーマット(TRUE：ｗ２モールID-yahoo注文ID FALSE：yahoo注文ID)</summary>
				Constants.YAHOO_MALL_ORDERID_FORMAT = csSetting.GetAppBoolSetting("MailOrderGetter_Yahoo_OrderId_Format");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// アプリケーションエントリポイント
		/// </summary>
		/// <param name="args">
		/// [0] メールファイルのベースパス
		/// [1] 店舗ID
		/// [2] モールID
		/// </param>
		static void Main(string[] strArgs)
		{
			Program program = null;

			try
			{
				program = new Program();
			}
			catch (Exception ex)
			{
				Console.WriteLine("MailOrderGetter初期化中にエラー発生");
				Console.WriteLine(ex.ToString());
				Environment.Exit(-1);
			}

			try
			{
				AppLogger.WriteInfo("起動");

				if (strArgs.Length == 3)
				{
					Constants.SHOP_ID = strArgs[1];
					Constants.MALL_ID = strArgs[2];
					program.ImportOrderMail(strArgs[0], strArgs[1], strArgs[2]);
				}
				else
				{
					FileLogger.WriteError("引数が足りません。");
				}

				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteInfo("エラー終了\r\n" + ex);
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// 初期化
		/// </summary>
		/// <param name="mallDir">モールディレクトリ</param>
		/// <param name="shopId">ショップID</param>
		/// <param name="mallId">モールID</param>
		private void Init(string mallDir, string shopId, string mallId)
		{
			this.ShopId = shopId;
			this.MailId = mallId;

			// ３か月前以前のモール監視ログを削除する（大量ログ蓄積回避）
			Program.MallWatchingLogManager.Delete();

			// モール監視ログ登録（バッチ起動）
			Program.MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER, this.MailId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS, "注文メールの取込処理を開始しました。", "");

			Constants.PATH_SUCCESS = mallDir + @"\Success";
			Constants.PATH_ACTIVE = mallDir + @"\Active";
			Constants.PATH_ERROR = mallDir + @"\Error";
			Constants.PATH_STOCK_ERROR = mallDir + @"\StockError";
			Constants.PATH_UNKNOWN = mallDir + @"\Unknown";
			Constants.PATH_RETRY = mallDir + @"\Retry";

			//------------------------------------------------------
			// ディレクトリが存在しなければ作成
			//------------------------------------------------------
			if (Directory.Exists(Constants.PATH_SUCCESS) == false)
			{
				Directory.CreateDirectory(Constants.PATH_SUCCESS);
			}
			if (Directory.Exists(Constants.PATH_ACTIVE) == false)
			{
				Directory.CreateDirectory(Constants.PATH_ACTIVE);
			}
			if (Directory.Exists(Constants.PATH_ERROR) == false)
			{
				Directory.CreateDirectory(Constants.PATH_ERROR);
			}
			if (Directory.Exists(Constants.PATH_STOCK_ERROR) == false)
			{
				Directory.CreateDirectory(Constants.PATH_STOCK_ERROR);
			}
			if (Directory.Exists(Constants.PATH_UNKNOWN) == false)
			{
				Directory.CreateDirectory(Constants.PATH_UNKNOWN);
			}
			if (Directory.Exists(Constants.PATH_RETRY) == false)
			{
				Directory.CreateDirectory(Constants.PATH_RETRY);
			}
		}

		/// <summary>
		/// 注文メール取込
		/// </summary>
		/// <param name="mallDir">モールディレクトリ</param>
		/// <param name="shopId">ショップID</param>
		/// <param name="mallId">モールID</param>
		public void ImportOrderMail(string mallDir, string shopId, string mallId)
		{
			Init(mallDir, shopId, mallId);
			ImportOrderMail(Constants.ExecTypes.Retry);
			ImportOrderMail(Constants.ExecTypes.Active);
		}
		/// <summary>
		/// 注文メール取込
		/// </summary>
		/// <param name="execType">実行タイプ（パス）</param>
		private void ImportOrderMail(Constants.ExecTypes execType)
		{
			//------------------------------------------------------
			// モール区分取得
			//------------------------------------------------------
			var mallKbn = "";
			DataView mallCooperationSetting = null;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("Mall", "GetMallSetingFromMallId"))
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID, StringUtility.ToEmpty(this.MailId) }
				};

				mallCooperationSetting = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}
			if (mallCooperationSetting.Count > 0)
			{
				mallKbn = (string)mallCooperationSetting[0][Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN];
			}
			else
			{
				FileLogger.WriteError("モールIDの取得に失敗しました MallId:" + this.MailId);
				throw new Exception();
			}

			//------------------------------------------------------
			// ファイルからメールを読み込む
			//------------------------------------------------------
			var mailFiles = (Directory.GetFiles(
					(execType == Constants.ExecTypes.Active)
						? Constants.PATH_ACTIVE
						: Constants.PATH_RETRY));

			Hashtable htMails = new Hashtable();
			foreach (var strMailFile in mailFiles)
			{
				// 別プロジェクトのメールを取込対象外とする
				if (Path.GetFileName(strMailFile).StartsWith(Constants.PROJECT_NO) == false)
				{
					try
					{
						File.Move(strMailFile, Constants.PATH_UNKNOWN + @"\" + Path.GetFileName(strMailFile));
						FileLogger.WriteError("別プロジェクトのメールが取込対象となりました。ファイル名の先頭にProjectNo「" + Constants.PROJECT_NO + "」を入れてください。 \r\n" + Constants.PATH_UNKNOWN + @"\" + Path.GetFileName(strMailFile));
					}
					catch (Exception ex)
					{
						FileLogger.WriteError("ファイルの移動に失敗しました key:" + strMailFile, ex);
					}

					continue;
				}

				// メール読込
				using (TextReader trReader = new StreamReader(strMailFile, Encoding.GetEncoding(932)))
				{
					var mailText = trReader.ReadToEnd();

					try
					{
						if (mallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO
							|| mallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN)
						{
							//ヘッダーからcharsetを取得
							var contentTypeHeader = mailText.Substring(0, mailText.IndexOf("\r\n\r\n"));
							var match = Regex.Match(contentTypeHeader, @"charset=([""'])?([^\s"";]+)([""'])?([;|\s]|$)");
							if (match.Success)
							{
								//デコードが不要なら何もしない
								if (match.Groups[2].Value != "iso-2022-jp")
								{
									// 本文をBase64・UTF8デコードする
									var headerEndIndex = mailText.IndexOf(Constants.ORDER_MAIL_DELIMITER_HEADER_AND_BODY)
										+ Constants.ORDER_MAIL_DELIMITER_HEADER_AND_BODY.Length;
									var enc = Encoding.GetEncoding(Constants.YAHOO_ORDER_MAIL_BODY_ENCODE_TYPE_STRING);
									mailText = mailText.Substring(0, headerEndIndex)
										+ Common.Net.Mail.EncodeHelper.DecodeBase64(enc, mailText.Substring(headerEndIndex));
								}
							}
						}
					}
					catch (Exception ex)
					{
						// 本文デコード失敗
						FileLogger.WriteError("メール本文のデコードに失敗しました。key:" + strMailFile, ex);
					}

					htMails[strMailFile] = mailText;
				}
			}

			//------------------------------------------------------
			// メールオブジェクトを構成
			//------------------------------------------------------
			Hashtable htMailMessages = new Hashtable();
			foreach (string strKey in htMails.Keys)
			{
				try
				{
					htMailMessages[strKey] = new MailMessage((string)htMails[strKey]);
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					Console.WriteLine(ex.ToString());

					// ファイルをErrorに移動
					try
					{
						File.Move(strKey, Constants.PATH_ERROR + @"\" + Path.GetFileName(strKey));
						// エラーメール送信
						BaseProcess.ErrorMailSender("[メールの解析に失敗しました。]\r\n" + Constants.PATH_ERROR + @"\" + Path.GetFileName(strKey));
						string[] strParams = strKey.ToString().Split('\\');

						// モール監視ログ登録（メール解析エラー）
						Program.MallWatchingLogManager.Insert(Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER, this.MailId, Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR, "解析エラー：システム管理者にお問い合わせください。\r\n取込に失敗した注文メールは詳細1にてご確認頂けます。\r\nファイル名[" + strParams[strParams.Length - 1] + "]", (string)htMails[strKey]);
					}
					catch (Exception ex2)
					{
						// ファイル移動失敗
						FileLogger.WriteError("ファイルの移動に失敗しました key:" + strKey, ex2);
					}
				}
			}

			//------------------------------------------------------
			// スクレーピング処理
			//------------------------------------------------------
			Hashtable htMallMails = new Hashtable();
			foreach (string key in htMailMessages.Keys)
			{
				FileLogger.WriteInfo("[" + key + "]解析");
				try
				{
					MailMessage mmMailMessage = (MailMessage)htMailMessages[key];

					// メール件名取得
					string subject = "";
					if ((mmMailMessage.Header.HeaderParts["subject"] != null)
						&& ((List<string>)mmMailMessage.Header.HeaderParts["subject"]).Count != 0)
					{
						subject = BaseProcess.GetMailHeaderString(((List<string>)mmMailMessage.Header.HeaderParts["subject"])[0]);
					}

					// 取込対象モール判別
					switch (mallKbn)
					{
						// 楽天メール
						case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN:
							// 取り込み対象（メール件名から取込対象か判断する）
							if (RakutenProcess.CheckMailImportPossible(subject, key))
							{
								// 楽天注文メールを解析・設定する(解析するのは受注IDのみ)
								var rakutenProcess = new RakutenProcess(this.ShopId, key, mmMailMessage, mallCooperationSetting[0]);
								htMallMails[key] = rakutenProcess;
							}
							break;

						// ヤフーメール
						case Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO:
							// 取り込み対象（メール件名から取込対象か判断する）
							if (YahooOrder.CheckMailImportPossible(subject, key))
							{
								// ヤフー注文メールを解析・設定する
								YahooOrder yahooOrder = new YahooOrder(mmMailMessage.Body, key, this.ShopId);
								htMallMails[key] = yahooOrder;
							}
							break;
					}
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					Console.WriteLine(ex.ToString());
					try
					{
						// ファイルをErrorに移動
						File.Move(key, Constants.PATH_ERROR + @"\" + Path.GetFileName(key));

						// エラーメール送信
						BaseProcess.ErrorMailSender("[メールの解析に失敗しました。]\r\n" + ((ex.InnerException != null) ? ex.InnerException.Message : ex.Message) + "\r\nファイル名：" + Constants.PATH_ERROR + @"\" + Path.GetFileName(key));
						string[] strParams = key.ToString().Split('\\');
						if (StringUtility.ToEmpty(Constants.DISP_ERROR_MESSAGE) == "")
						{
							// モール監視ログ登録（メール解析エラー）
							Program.MallWatchingLogManager.Insert(
								Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
								this.MailId,
								Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
								"解析エラー：システム管理者にお問い合わせください。\r\n取込に失敗した注文メールは詳細1にてご確認頂けます。\r\nファイル名[" + strParams[strParams.Length - 1] + "]",
								htMails[key].ToString());
						}
						else
						{
							// モール監視ログ登録（特定のエラーメッセージを登録する）
							Program.MallWatchingLogManager.Insert(
								Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
								this.MailId,
								(Constants.DISP_ERROR_KBN != null) ? Constants.DISP_ERROR_KBN : Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
								Constants.DISP_ERROR_MESSAGE + "\r\nファイル名[" + strParams[strParams.Length - 1] + "]",
								htMails[key].ToString());
						}
					}
					catch (Exception ex2)
					{
						// ファイル移動失敗
						FileLogger.WriteError("ファイル移動失敗 key：" + key, ex2);
					}
				}
			}

			//------------------------------------------------------
			// 注文メール解析済み情報をDB取込
			//------------------------------------------------------
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				foreach (string strKey in htMallMails.Keys)
				{
					FileLogger.WriteInfo("[" + strKey + "]ＤＢ投入");
					try
					{
						// トランザクション開始
						sqlAccessor.BeginTransaction();
						if (mallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_RAKUTEN)
						{
							((RakutenProcess)htMallMails[strKey]).InsertRakutenOrder(sqlAccessor);
						}
						else if (mallKbn == Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_YAHOO)
						{
							((YahooOrder)htMallMails[strKey]).InsertOrder(sqlAccessor, this.MailId, strKey, (string)htMails[strKey]);
						}

						// トランザクションコミット
						sqlAccessor.CommitTransaction();
					}
					catch (Exception ex1)
					{
						//------------------------------------------------------
						// 登録エラー時にロールバック＆ファイル移動を行う
						//------------------------------------------------------
						// .NetFramework2.0のバグでSqlTransaction.Zombie()を実行するとNullReferenceExceptionになってしまい、
						// 本来であればArgumentNullExceptionを返すべきだがArgumentNullExceptionをキャッチできないと、
						// SQLTransactionのコンストラクタが起動されSqlConnectionがNULLで初期化され、
						// トランザクションが完了したというイレギュラーエラーが発生するため下記try～catch処理を追加。

						// トランザクションロールバック
						try
						{
							sqlAccessor.RollbackTransaction();
						}
						catch (Exception ex)
						{
							FileLogger.WriteError("トランザクションロールバックに失敗:", ex);
						}

						// SQLタイムアウトの場合はファイルの移動と例外のスローは行わない。（次回実行時に再処理するため）
						if ((ex1.InnerException is SqlException)
							&& (((SqlException)ex1.InnerException).Number == -2)) // TdsEnums.TIMEOUT_EXPIRED（private定義）
						{
							// モール監視ログ登録
							string[] strParams = strKey.ToString().Split('\\');
							Program.MallWatchingLogManager.Insert(
								Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
								this.MailId,
								Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
								"メール取り込みエラー：DB接続時にタイムアウトしました。次回起動時に再度取り込みます。\r\n取込に失敗した注文メールは詳細1にてご確認頂けます。\r\nファイル名[" + strParams[strParams.Length - 1] + "]",
								htMails[strKey].ToString());

							// エラーログ追加
							AppLogger.WriteError(ex1);
						}
						else
						{
							// ファイルをErrorに移動
							try
							{
								var isRetry = IsRetry(ex1, execType);
								var movePath = isRetry ? Constants.PATH_RETRY : Constants.PATH_ERROR;
								File.Move(strKey, movePath + @"\" + Path.GetFileName(strKey));

								if (isRetry == false)
								{
									BaseProcess.ErrorMailSender(
										"DB更新でエラーが発生しました。処理を終了します。\r\nファイル名："
										+ Constants.PATH_ERROR + @"\" + Path.GetFileName(strKey));
								}

								var strParams = strKey.ToString().Split('\\');

								if (isRetry)
								{
									// モール監視ログ登録（楽天API情報取得失敗）
									Program.MallWatchingLogManager.Insert(
										Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
										this.MailId,
										Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
										"メール取り込みエラー：楽天APIで情報取得に失敗しました。次回起動時に再度取り込みます。\r\n取込に失敗した注文メールは詳細1にてご確認頂けます。\r\nファイル名[" + strParams[strParams.Length - 1] + "]",
										htMails[strKey].ToString(),
										Constants.DISP_ERROR_MESSAGE);
									continue;
								}
								else if (StringUtility.ToEmpty(Constants.DISP_ERROR_MESSAGE) == "")
								{
									// モール監視ログ登録（メール解析エラー）
									Program.MallWatchingLogManager.Insert(
										Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
										this.MailId,
										Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
										"例外エラー：システム管理者にお問い合わせください。\r\n取込に失敗した注文メールは詳細1にてご確認頂けます。\r\nファイル名[" + strParams[strParams.Length - 1] + "]",
										htMails[strKey].ToString());
								}
								else
								{
									// モール監視ログ登録（特定のエラーメッセージを登録する）
									Program.MallWatchingLogManager.Insert(
										Constants.FLG_MALLWATCHINGLOG_BATCH_ID_MAILORDERGETTER,
										this.MailId,
										(Constants.DISP_ERROR_KBN != null) ? Constants.DISP_ERROR_KBN : Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
										Constants.DISP_ERROR_MESSAGE,
										htMails[strKey].ToString());
								}
							}
							catch (Exception ex)
							{
								FileLogger.WriteError("ファイル移動失敗 key:" + strKey, ex);
							}
							throw;
						}
					}
				}
			}
		}

		/// <summary>
		/// リトライ対象か
		/// </summary>
		/// <param name="ex">例外</param>
		/// <param name="execType">実行タイプ</param>
		/// <returns>リトライ対象か</returns>
		public bool IsRetry(Exception ex, Constants.ExecTypes execType)
		{
			if ((ex is RakutenApiCoopException) == false) return false;

			var result = ((ex.Message.StartsWith(RakutenApiCoopException.ORDER_GET_PROGRESS_RETRY))
				|| ((execType == Constants.ExecTypes.Active)
					&& ex.Message.StartsWith(RakutenApiCoopException.ORDER_GET_ERROR)));
			return result;
		}

		/// <summary>
		/// モール監視ログマネージャ
		/// </summary>
		public static w2.App.Common.MallCooperation.MallWatchingLogManager MallWatchingLogManager
		{
			get { return m_mallWatchingLogManager; }
		}
		public static w2.App.Common.MallCooperation.MallWatchingLogManager m_mallWatchingLogManager = new App.Common.MallCooperation.MallWatchingLogManager();
		/// <summary>ショップID</summary>
		public string ShopId { get; set; }
		/// <summary>モールID</summary>
		public string MailId { get; set; }
	}
}
