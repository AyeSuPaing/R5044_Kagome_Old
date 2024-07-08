/*
=========================================================================================================
  Module      : 入荷通知メール送信処理(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using w2.App.Common;
using w2.App.Common.Extensions.Currency;
using w2.App.Common.Order;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.Favorite;
using w2.Domain.User;

namespace w2.Commerce.Batch.ArrivalMailSend
{
	class program
	{
		// パラメータ格納用
		List<KeyValuePair<string, string>> lkvpTargetArrivalMailKbns = new List<KeyValuePair<string, string>>();
		List<Hashtable> lhtTargetProducts = new List<Hashtable>();
		/// <summary> メール送信方法 ※デフォルト：自動送信 </summary>
		private Constants.MailSendMethod m_mailSendMethod = Constants.MailSendMethod.Auto;

		DateTime begin = new DateTime();	// 処理開始時間
		DateTime end = new DateTime();	// 処理終了時間
		int successCount = 0;	// 送信成功件数
		int failureCount = 0;	// 送信失敗件数

		/// <summary>
		/// プログラムのエントリポイント
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			// 実体作成
			program objProgram = new program();
			try
			{
				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				if (args.Any(arg => arg == Constants.FLG_MAILTEMPLATE_MAIL_CATEGORY_STOCKALERT))
				{
					//在庫状況最新化
					objProgram.BulkUpdateAlertSendFlg();

					 //在庫アラートメール送信
					if (Constants.PRODUCT_STOCK_OPTION_ENABLE && Constants.STOCKALERTMAIL_OPTION_ENABLED
						&& (Constants.STOCK_ALERT_MAIL_THRESHOLD > 0)) objProgram.StockAlertMail();
				}
				else
				{
					// パラメータ取得
					objProgram.GetParameters(args);

					// 入荷通知メール送信
					objProgram.ArrivalMailSend();

					// 結果メール送信
					SendResurtMail(objProgram.GetResultMessage());
				}

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				string strErrorMessage = ex.ToString();

				try
				{
					// メール送信
					SendResurtMail(BaseLogger.CreateExceptionMessage(ex));
				}
				catch (Exception ex2)
				{
					strErrorMessage += "\r\n" + ex2.ToString();
				}

				// エラーイベントログ出力
				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public program()
		{
			// 初期化
			Iniitialize();
		}

		/// <summary>
		/// 設定初期化
		/// </summary>
		private void Iniitialize()
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
				ConfigurationSetting.ReadKbn.C200_ArrivalMailSend);

			//------------------------------------------------------
			// アプリケーション固有の設定
			//------------------------------------------------------
			// 結果メール送信先設定
			Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
			Constants.MAIL_FROM = csSetting.GetAppMailAddressSetting("Mail_From");
			Constants.MAIL_TO_LIST = csSetting.GetAppMailAddressSettingList("Mail_To");
			Constants.MAIL_CC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Cc");
			Constants.MAIL_BCC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Bcc");
		}

		/// <summary>
		/// パラメータ取得
		/// </summary>
		/// <param name="args">パラメータ</param>
		private void GetParameters(string[] args)
		{
			foreach (string arg in args)
			{
				string[] strParam = arg.Split(',');

				if (strParam[0] == "-")
				{
					lkvpTargetArrivalMailKbns.Add(new KeyValuePair<string, string>(strParam[1], strParam[2]));
				}
				else if (strParam[0] == "+")
				{
					if (Enum.TryParse(strParam[1], out m_mailSendMethod) == false)
					{
						m_mailSendMethod = Constants.MailSendMethod.Auto;
					}
				}
				else
				{
					Hashtable htTargetProduct = new Hashtable();
					htTargetProduct.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID, strParam[0]);
					htTargetProduct.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID, strParam[1]);
					htTargetProduct.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID, strParam[2]);
					htTargetProduct.Add("send_arrival_mail_flg", strParam[3]);
					htTargetProduct.Add("send_release_mail_flg", strParam[4]);
					htTargetProduct.Add("send_resale_mail_flg", strParam[5]);
					htTargetProduct.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_LAST_CHANGED, strParam[6]);

					lhtTargetProducts.Add(htTargetProduct);
				}
			}
		}

		/// <summary>
		/// 入荷メール送信処理
		/// </summary>
		private void ArrivalMailSend()
		{
			// 処理開始時間設定
			begin = DateTime.Now;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				foreach (KeyValuePair<string, string> kvpTargetArrivalMailKbn in lkvpTargetArrivalMailKbns)
				{
					foreach (Hashtable htTargetProduct in lhtTargetProducts)
					{
						ArrivalMailSend(
							kvpTargetArrivalMailKbn.Key,
							kvpTargetArrivalMailKbn.Value,
							(string)htTargetProduct[Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID],
							(string)htTargetProduct[Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID],
							(string)htTargetProduct[Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID],
							sqlAccessor);
					}
				}
			}

			// 処理終了時間設定
			end = DateTime.Now;
		}

		/// <summary>
		/// 入荷メール送信処理
		/// </summary>
		/// <param name="strArrivalMailKbn">入荷通知メール区分</param>
		/// <param name="strMailTemplateId">メールテンプレートID</param>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private void ArrivalMailSend(string strArrivalMailKbn, string strMailTemplateId, string strShopId, string strProductId, string strVariationId, SqlAccessor sqlAccessor)
		{
			// メールテンプレートIDが「NOTSEND」の場合、ステータスを「送信なし処理済み」に更新
			if (strMailTemplateId == "NOTSEND")
			{
				UpdateMailSendStatus("", "", strShopId, strProductId, strVariationId, strArrivalMailKbn, Constants.FLG_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS_NOTSEND, sqlAccessor);
			}
			// それ以外の場合メール送信
			else
			{
				bool blSuccess = false;

				//------------------------------------------------------
				// 対象入荷メール情報取得
				//------------------------------------------------------
				DataView dvSendTarget = null;
				using (SqlStatement sqlStatement = new SqlStatement("UserProductArrivalMail", "GetSendTarget"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID, strShopId);
					htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID, strProductId);
					htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID, strVariationId);
					htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN, strArrivalMailKbn);
					dvSendTarget = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
				}

				//------------------------------------------------------
				// 入荷メール送信＆メール配信済みフラグ更新
				//------------------------------------------------------
				foreach (DataRowView drv in dvSendTarget)
				{
					blSuccess = false;

					//------------------------------------------------------
					// 1.入荷メール送信
					//------------------------------------------------------
					// 会員/ゲストの判定
					bool isMember = ((string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID] != Constants.FLG_USERPRODUCTARRIVALMAIL_USER_ID_GUEST);

					// メールテンプレート内タグ変換用データ取得
					Hashtable htMailContents = new Hashtable();
					htMailContents.Add("user_name", isMember ? UserModel.CreateComplementUserName(
						(string)drv["user_name"],
						(string)drv[Constants.FIELD_USER_MAIL_ADDR],
						(string)drv[Constants.FIELD_USER_MAIL_ADDR2]) : "");																						// ユーザ名, ゲストは空文字に
					htMailContents.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID, (string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID]);	// 商品ID
					htMailContents.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID, (string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID]);	// 商品ID
					htMailContents.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID, (string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID]);	// バリエーションID
					htMailContents.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID + "_urlenc", HttpUtility.UrlEncode((string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID]));	// 商品ID（URLエンコード済み）
					htMailContents.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID + "_urlenc", HttpUtility.UrlEncode((string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID]));	// バリエーションID（URLエンコード済み）
					htMailContents.Add("product_name", ProductCommon.CreateProductJointName(drv));	// 商品名+バリエーション名
					htMailContents.Add("price", ProductCommon.GetProductPriceNumeric(drv, (string)drv[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE).ToPriceString());	// 価格
					htMailContents.Add(Constants.FIELD_PRODUCT_SELL_FROM + "_yyyyMMdd", StringUtility.ToDateString(drv[Constants.FIELD_PRODUCT_SELL_FROM], "yyyy/MM/dd"));  // 販売期間(yyyy/MM/dd)
					// 販売期間(置換フォーマットはyyyy/MM/dd HH:mm、メールテンプレートの置換タグはhh:mmのまま)
					htMailContents.Add(Constants.FIELD_PRODUCT_SELL_FROM + "_yyyyMMddhhmm", StringUtility.ToDateString(drv[Constants.FIELD_PRODUCT_SELL_FROM], "yyyy/MM/dd HH:mm"));
					htMailContents.Add(Constants.FIELD_PRODUCT_BRAND_ID1, (string)drv[Constants.FIELD_PRODUCT_BRAND_ID1]);	// ブランドID1
					htMailContents.Add(Constants.FIELD_PRODUCT_BRAND_ID2, (string)drv[Constants.FIELD_PRODUCT_BRAND_ID2]);	// ブランドID2
					htMailContents.Add(Constants.FIELD_PRODUCT_BRAND_ID3, (string)drv[Constants.FIELD_PRODUCT_BRAND_ID3]);	// ブランドID3
					htMailContents.Add(Constants.FIELD_PRODUCT_BRAND_ID4, (string)drv[Constants.FIELD_PRODUCT_BRAND_ID4]);	// ブランドID4
					htMailContents.Add(Constants.FIELD_PRODUCT_BRAND_ID5, (string)drv[Constants.FIELD_PRODUCT_BRAND_ID5]);	// ブランドID5

					htMailContents.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID,
						isMember ? (string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID] : Constants.FLG_USERPRODUCTARRIVALMAIL_USER_ID_GUEST);	// 会員/ゲストの振り分けタグ
					htMailContents.Add(Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID, drv[Constants.FIELD_USER_USER_MANAGEMENT_LEVEL_ID].ToString());	// User Management Level Id
					htMailContents.Add(Constants.FIELD_ORDER_ADVCODE_FIRST, drv[Constants.FIELD_ORDER_ADVCODE_FIRST].ToString());						// 初回広告コード

					// 区分判定, メールアドレス格納
					bool isPc = ((string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN] == Constants.FLG_USERPRODUCTARRIVALMAIL_PCMOBILE_KBN_PC);
					string mailAddr;
					if ((isMember == false) || ((string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_GUEST_MAIL_ADDR] != ""))
					{
						// ゲスト or 会員&ゲストアドレス
						mailAddr = (string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_GUEST_MAIL_ADDR];
					}
					else
					{
						// 会員
						mailAddr = isPc ? (string)drv[Constants.FIELD_USER_MAIL_ADDR] : (string)drv[Constants.FIELD_USER_MAIL_ADDR2];
					}
					var userId = isMember ? (string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID] : "";

					// メールアドレスチェック
					if (Validator.IsMailAddress(mailAddr) == false)
					{
						// ステータスを「処理中」から「送信なし処理済み」に設定する
						UpdateMailSendStatus((string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID],
											 mailAddr,
											 strShopId,
											 strProductId,
											 strVariationId,
											 strArrivalMailKbn,
											 Constants.FLG_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS_NOTSEND,
											 sqlAccessor);

						// メールアドレス不正の場合ログ書き込み
						FileLogger.WriteError("ユーザーID(" + (string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID] + ")のメールアドレス(" + mailAddr + ")が不正です。");
						failureCount++;
						continue;
					}

					// ユーザの言語コード取得
					string languageCode = null;
					string languageLocaleId = null;
					// モバイルユーザにはデフォルトのメールテンプレートを送信する
					if (Constants.GLOBAL_OPTION_ENABLE && isPc)
					{
						languageCode = (drv[Constants.FIELD_USER_DISP_LANGUAGE_CODE] != DBNull.Value)
							? (string)drv[Constants.FIELD_USER_DISP_LANGUAGE_CODE]
							: null;
						languageLocaleId = (drv[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID] != DBNull.Value)
							? (string)drv[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID]
							: null;
					}

					// 送信先メールアドレス
					var userMailAddress = (drv[Constants.FIELD_USER_MAIL_ADDR] != DBNull.Value)
						? (string)drv[Constants.FIELD_USER_MAIL_ADDR]
						: (string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_GUEST_MAIL_ADDR];

					// 指定メールアドレスに送信
					using (MailSendUtility msMailSend = new MailSendUtility(strShopId, strMailTemplateId, userId, htMailContents, isPc, m_mailSendMethod, languageCode, languageLocaleId, userMailAddress))
					{
						msMailSend.AddTo(mailAddr);

						// メール送信
						if (msMailSend.SendMail())
						{
							blSuccess = true;
							successCount++;
						}
						else
						{
							// 送信エラーの場合ログ書き込み
							FileLogger.WriteError(this.GetType().BaseType.ToString() + " : " + msMailSend.MailSendException.ToString());
							failureCount++;
						}
					}

					//------------------------------------------------------
					// 2.メール配信済みフラグ更新
					//------------------------------------------------------
					// 送信成功
					if (blSuccess)
					{
						// ステータスを処理中から送信済みに更新する
						UpdateMailSendStatus(
							(string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID],
							mailAddr,
							strShopId,
							strProductId,
							strVariationId,
							strArrivalMailKbn,
							Constants.FLG_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS_SENT,
							sqlAccessor);
					}
					// 送信対象外or送信失敗
					else
					{
						// ステータスを処理中から未送信に戻す
						UpdateMailSendStatus(
							(string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID],
							mailAddr,
							strShopId,
							strProductId,
							strVariationId,
							strArrivalMailKbn,
							Constants.FLG_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS_UNSENT,
							sqlAccessor);
					}
				}
			}
		}

		/// <summary>
		/// 在庫アラートメール送信処理
		/// </summary>
		private void StockAlertMail()
		{
			// 処理開始時間設定
			begin = DateTime.Now;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				var productStockService = new FavoriteService();
				var alertProducts = productStockService.GetStockAlertProducts(Constants.STOCK_ALERT_MAIL_THRESHOLD);

				sqlAccessor.OpenConnection();

				foreach (FavoriteModel alertProduct in alertProducts)
				{
					StockAlertMail(
						alertProduct,
						sqlAccessor);
				}
			}

			// 処理終了時間設定
			end = DateTime.Now;
		}

		/// <summary>
		/// 在庫アラートメール送信処理
		/// </summary>
		/// <param name="alertProduct">店舗ID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		private void StockAlertMail(FavoriteModel alertProduct, SqlAccessor sqlAccessor)
		{
			// 対象在庫アラートメール情報取得
			var mailTemplateId = Constants.CONST_MAIL_ID_STOCK_ALERT_RESALE;

			var sendTarget = new FavoriteService().GetSendTarget(alertProduct.UserId, alertProduct.ShopId, alertProduct.ProductId, alertProduct.VariationId);

			// 在庫アラートメール送信＆メール配信済みフラグ更新
			foreach (DataRowView drv in sendTarget)
			{
				var success = false;

				// 1.在庫アラートメール送信
				// 会員/ゲストの判定
				var isMember = ((string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID] != Constants.FLG_USERPRODUCTARRIVALMAIL_USER_ID_GUEST);

				// メールテンプレート内タグ変換用データ取得
				var mailContents = new Hashtable
				{
					{
						"user_name", isMember
							? UserModel.CreateComplementUserName(
								(string)drv["user_name"],
								(string)drv[Constants.FIELD_USER_MAIL_ADDR],
								(string)drv[Constants.FIELD_USER_MAIL_ADDR2])
							: ""
					},
					{ Constants.FIELD_FAVORITE_SHOP_ID, (string)drv[Constants.FIELD_FAVORITE_SHOP_ID] },
					{ Constants.FIELD_FAVORITE_PRODUCT_ID, (string)drv[Constants.FIELD_FAVORITE_PRODUCT_ID] },
					{ Constants.FIELD_FAVORITE_VARIATION_ID, (string)drv[Constants.FIELD_FAVORITE_VARIATION_ID] },
					{ Constants.FIELD_FAVORITE_PRODUCT_ID + "_urlenc", HttpUtility.UrlEncode((string)drv[Constants.FIELD_FAVORITE_PRODUCT_ID]) },
					{ Constants.FIELD_FAVORITE_VARIATION_ID + "_urlenc", HttpUtility.UrlEncode((string)drv[Constants.FIELD_FAVORITE_VARIATION_ID]) },
					{
						"product_name", ((string)drv[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE)
						? ProductCommon.CreateProductJointName(drv)
						: (string)drv[Constants.FIELD_PRODUCT_NAME]
					},
					{ Constants.FIELD_PRODUCTVARIATION_PRICE, ProductCommon.GetProductPriceNumeric(drv, (string)drv[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE).ToPriceString(true) },
					{ Constants.FIELD_PRODUCT_SELL_FROM + "_yyyyMMdd", StringUtility.ToDateString(drv[Constants.FIELD_PRODUCT_SELL_FROM], "yyyy/MM/dd") },
					{ Constants.FIELD_PRODUCT_SELL_FROM + "_yyyyMMddhhmm", StringUtility.ToDateString(drv[Constants.FIELD_PRODUCT_SELL_FROM], "yyyy/MM/dd hh:mm") },
					{ "user_name1", (string)drv[Constants.FIELD_USER_NAME1] },
					{ "user_name2", (string)drv[Constants.FIELD_USER_NAME2] },
					{
						Constants.FIELD_PRODUCT_IMAGE_HEAD, ((string)drv[Constants.FIELD_PRODUCT_USE_VARIATION_FLG] == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE)
						? (string)drv[Constants.FIELD_PRODUCTVARIATION_VARIATION_IMAGE_HEAD].ToString()
						: (string)drv[Constants.FIELD_PRODUCT_IMAGE_HEAD].ToString()
					},
					{ Constants.FIELD_PRODUCTSTOCK_STOCK, (string)drv[Constants.FIELD_PRODUCTSTOCK_STOCK].ToString() },
					{ Constants.FIELD_FAVORITE_USER_ID, (string)drv[Constants.FIELD_FAVORITE_USER_ID] }
				};

				// 区分判定, メールアドレス格納
				var isPc = ((string)drv[Constants.FIELD_USER_USER_KBN] == Constants.FLG_USER_USER_KBN_PC_USER);

				var mailAddr =
					isPc
						? string.IsNullOrEmpty((string)drv[Constants.FIELD_USER_MAIL_ADDR])
							? (string)drv[Constants.FIELD_USER_MAIL_ADDR2]
							: (string)drv[Constants.FIELD_USER_MAIL_ADDR]
						: string.IsNullOrEmpty((string)drv[Constants.FIELD_USER_MAIL_ADDR2])
							? (string)drv[Constants.FIELD_USER_MAIL_ADDR]
							: (string)drv[Constants.FIELD_USER_MAIL_ADDR2];
				var userId = isMember ? (string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID] : "";

				// メールアドレスチェック
				if (Validator.IsMailAddress(mailAddr) == false)
				{
					// メールアドレス不正の場合ログ書き込み
					FileLogger.WriteError("ユーザーID(" + (string)drv[Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID] + ")のメールアドレス(" + mailAddr + ")が不正です。");
					failureCount++;
					continue;
				}

				// ユーザの言語コード取得
				var languageCode = "";
				var languageLocaleId = "";
				if (Constants.GLOBAL_OPTION_ENABLE && isPc)
				{
					languageCode = (drv[Constants.FIELD_USER_DISP_LANGUAGE_CODE] != DBNull.Value)
						? (string)drv[Constants.FIELD_USER_DISP_LANGUAGE_CODE]
						: "";
					languageLocaleId = (drv[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID] != DBNull.Value)
						? (string)drv[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID]
						: "";
				}

				// 指定メールアドレスに送信
				using (var mailSend = new MailSendUtility(alertProduct.ShopId, mailTemplateId, userId, mailContents, isPc, m_mailSendMethod, languageCode, languageLocaleId))
				{
					mailSend.AddTo(mailAddr);

					// メール送信
					if (mailSend.SendMail())
					{
						success = true;
						successCount++;
					}
					else
					{
						// 送信エラーの場合ログ書き込み
						FileLogger.WriteError(this.GetType().BaseType?.ToString() + " : " + mailSend.MailSendException.ToString());
						failureCount++;
					}
				}

				// 2.メール配信済みフラグ更新 送信成功
				if (success)
				{
					// ステータスを処理中から送信済みに更新する
					new FavoriteService().UpdateAlertSendFlg(userId, alertProduct.ShopId, alertProduct.ProductId, alertProduct.VariationId, Constants.FLG_FAVORITE_STOCK_ALERT_MAIL_SEND_FLG_SENT);
				}
			}
		}

		/// <summary>
		/// 在庫減少アラートメール送信フラグ更新
		/// </summary>
		private void BulkUpdateAlertSendFlg()
		{
			new FavoriteService().BulkUpdateAlertSendFlg(Constants.FLG_FAVORITE_STOCK_ALERT_MAIL_SEND_FLG_UNSENT, Constants.STOCK_ALERT_MAIL_THRESHOLD);
		}

		/// <summary>
		/// メールテンプレートID取得
		/// </summary>
		/// <param name="strArrivalMailKbn">入荷通知メール区分</param>
		/// <return>メールテンプレートID</return>
		protected string GetMailTemplateId(string strArrivalMailKbn)
		{
			string strMailTemplateId = null;

			foreach (KeyValuePair<string, string> kvp in lkvpTargetArrivalMailKbns)
			{
				if (kvp.Key == strArrivalMailKbn)
				{
					strMailTemplateId = kvp.Value;
					break;
				}
			}

			return strMailTemplateId;
		}

		/// <summary>
		/// 送信フラグ更新
		/// </summary>
		/// <param name="strUserId">ユーザID</param>
		/// <param name="guestmailAddr">メールアドレス</param>
		/// <param name="strShopId">店舗ID</param>
		/// <param name="strProductId">商品ID</param>
		/// <param name="strVariationId">バリエーションID</param>
		/// <param name="strArrivalMailKbn">入荷通知メール区分</param>
		/// <param name="strMailSendStatus">更新後の送信ステータス</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		protected void UpdateMailSendStatus(
			string strUserId,
			string guestmailAddr,
			string strShopId,
			string strProductId,
			string strVariationId,
			string strArrivalMailKbn,
			string strMailSendStatus,
			SqlAccessor sqlAccessor)
		{
			using (SqlStatement sqlStatement = new SqlStatement("UserProductArrivalMail", "UpdateMailSendStatus"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_USER_ID, strUserId);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_GUEST_MAIL_ADDR, guestmailAddr);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_SHOP_ID, strShopId);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_PRODUCT_ID, strProductId);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_VARIATION_ID, strVariationId);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_ARRIVAL_MAIL_KBN, strArrivalMailKbn);
				htInput.Add(Constants.FIELD_USERPRODUCTARRIVALMAIL_MAIL_SEND_STATUS, strMailSendStatus);

				sqlStatement.ExecStatement(sqlAccessor, htInput);
			}
		}

		/// <summary>
		/// 成功時結果メール用メッセージ取得
		/// </summary>
		private string GetResultMessage()
		{
			StringBuilder sbMessage = new StringBuilder();
			sbMessage.Append("入荷通知メール配信完了").Append("\r\n").Append("\r\n");
			sbMessage.Append("処理開始時間：").Append(begin.ToString()).Append("\r\n");
			sbMessage.Append("処理終了時間：").Append(end.ToString()).Append("\r\n");
			sbMessage.Append("送信成功件数：").Append(successCount.ToString()).Append("件\r\n");
			sbMessage.Append("送信失敗件数：").Append(failureCount.ToString()).Append("件\r\n");

			return sbMessage.ToString();
		}

		/// <summary>
		/// 結果メール送信処理
		/// </summary>
		/// <param name="strMessage">メール本文</param>
		private static void SendResurtMail(string strMessage)
		{
			using (SmtpMailSender smsMailSender = new SmtpMailSender(Constants.SERVER_SMTP))
			{
				// メール送信デフォルト値設定
				smsMailSender.SetSubject(Constants.MAIL_SUBJECTHEAD);
				smsMailSender.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => smsMailSender.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => smsMailSender.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => smsMailSender.AddBcc(mail.Address));

				smsMailSender.SetBody(strMessage);

				// メール送信
				bool blResult = smsMailSender.SendMail();
				if (blResult == false)
				{
					Exception ex2 = smsMailSender.MailSendException;
					FileLogger.WriteError(ex2);
				}
			}
		}
	}
}
