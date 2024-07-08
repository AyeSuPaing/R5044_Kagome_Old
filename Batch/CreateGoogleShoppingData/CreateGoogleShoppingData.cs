/*
=========================================================================================================
  Module      : Googleショッピング連携用データフィード作成処理(CreateGoogleShoppingData.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Xml;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Logger;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Extensions.Currency;
using w2.Common.Net.Mail;

namespace w2.Commerce.Batch.CreateGoogleShoppingData
{
	public class CreateGoogleShoppingData
	{
		private const string CONST_DIRECTORY_PATH_PRODUCT_DATA_FEED = "Contents/GoogleMerchantDataFeed/";	// データフィード格納ディレクトリ
		private const int CONST_PRODUCT_NAME_MAX_LENGTH = 35;				// 商品名最大文字数
		private const int CONST_PRODUCT_DESCRIPTION_MAX_LENGTH = 5000;		// 商品説明最大文字数
		private const int CONST_FILE_MAX_STREAM_LENGTH = 15000000;			// ファイル最大ストリーム長(バイト単位)
		private const int CONST_FILE_MAX_COUNT = 10;						// 取込み可能最大ファイル数
		private static StringBuilder m_sbErrorMessages = new StringBuilder();

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		/// <param name="args">コマンドライン引数</param>
		static void Main(string[] args)
		{
			try
			{
				// 実体作成
				CreateGoogleShoppingData program = new CreateGoogleShoppingData();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				// 実行
				program.CreateDataFeed();

				// メール送信
				SendMail();

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex) 
			{
				// メール送信
				SendMail(ex);

				AppLogger.WriteError(ex);
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public CreateGoogleShoppingData()
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
					ConfigurationSetting.ReadKbn.C100_SiteCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_CommonFront,
					ConfigurationSetting.ReadKbn.C200_CommonManager,
					ConfigurationSetting.ReadKbn.C300_Pc,
					ConfigurationSetting.ReadKbn.C200_CreateGoogleMerchantDataFeed);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// PCルートパス設定			
				Constants.PATH_ROOT_FRONT_PC = csSetting.GetAppStringSetting("Site_RootPath_PCFront");
				// PCルートURL設定			
				Constants.URL_FRONT_PC = Constants.PROTOCOL_HTTP + csSetting.GetAppStringSetting("AccessLog_TargetDomain") + Constants.PATH_ROOT_FRONT_PC;
				// フレンドリーURL
				Constants.FRIENDLY_URL_ENABLED = csSetting.GetAppBoolSetting("FriendlyUrlEnabled");

				// メール送信先設定
				Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = csSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = csSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Bcc");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// データフィード作成
		/// </summary>
		private void CreateDataFeed()
		{
			//------------------------------------------------------
			// Googleショッピング連携対象商品情報取得
			//------------------------------------------------------
			DataView dvProducts = null;
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("Statements", "GetProductList"))
			{
				dvProducts = sqlStatement.SelectSingleStatementWithOC(sqlAccessor);
			}

			if (dvProducts.Count <= 0)
			{
				// イベントログ出力
				AppLogger.WriteInfo("連携対象商品が0件のため、処理終了");

				return;
			}

			//------------------------------------------------------
			// 商品エントリー作成
			//------------------------------------------------------
			XmlTextWriter xmlTextWriter = null;
			int iFileNo = 1;
			int iProcessedCount = 0;
			int iErrorCount = 0;
			bool blNewFileFlg = true;
			string strEntryNodeTemp = null;

			string directoryGoogleMerchantDataFeed = Path.Combine(Constants.PHYSICALDIRPATH_CONTENTS_ROOT, CONST_DIRECTORY_PATH_PRODUCT_DATA_FEED);
			if (Directory.Exists(directoryGoogleMerchantDataFeed) == false)
			{
				Directory.CreateDirectory(directoryGoogleMerchantDataFeed);
			}

			foreach (DataRowView drvProduct in dvProducts)
			{
				//------------------------------------------------------
				// ストリーム初期化
				//------------------------------------------------------
				// 新規ファイルフラグがtrueの場合は、ストリームを初期化
				if (blNewFileFlg)
				{
					xmlTextWriter = new XmlTextWriter(directoryGoogleMerchantDataFeed + Constants.PROJECT_NO + "_DataFeed_" + iFileNo.ToString() + ".xml", Encoding.GetEncoding("UTF-8"));
					// 出力書式設定
					xmlTextWriter.Formatting = Formatting.Indented;
					// インデント設定
					xmlTextWriter.Indentation = 2;
					// エンコード設定
					xmlTextWriter.WriteProcessingInstruction("xml", "version='1.0' encoding='utf-8'");

					// <feed>ノード開始
					xmlTextWriter.WriteStartElement("feed", "http://www.w3.org/2005/Atom");
					xmlTextWriter.WriteAttributeString("xmlns", "g", null, "http://base.google.com/ns/1.0");

					blNewFileFlg = false;

					// 複数ファイル目の場合は、一時格納しておいた商品エントリーをストリームに書き込み
					if (iFileNo > 1)
					{
						xmlTextWriter.WriteRaw(strEntryNodeTemp);
						iProcessedCount++;
					}
				}

				//------------------------------------------------------
				// 商品エントリー作成
				//------------------------------------------------------
				string strEntryNode = CreateEntryNode(drvProduct);
				if (strEntryNode != null)
				{
					xmlTextWriter.WriteString("\r\n");

					// ファイル最大ストリーム長を超える場合、ファイルを区切る
					if ((xmlTextWriter.BaseStream.Length + Encoding.UTF8.GetByteCount(strEntryNode)) >= CONST_FILE_MAX_STREAM_LENGTH)
					{
						// 取込み可能な最大ファイル数に達している場合は、処理終了
						if (iFileNo >= CONST_FILE_MAX_COUNT)
						{
							m_sbErrorMessages.Append((dvProducts.Count - iProcessedCount) + "件の対象商品のデータフィードが、ファイル数制限を超えたため作成できませんでした。\n");
							break;
						}

						// <feed>ノード終了
						xmlTextWriter.WriteEndElement();

						// ストリームを閉じる
						xmlTextWriter.Close();

						// ファイルNoをカウントアップ
						iFileNo++;
						// 新規ファイルフラグをtrueに設定
						blNewFileFlg = true;
						// 現在のループの商品エントリーを一時格納
						strEntryNodeTemp = strEntryNode;
					}
					// ストリームに商品エントリーを書き込み
					else
					{
						xmlTextWriter.WriteRaw(strEntryNode);
						iProcessedCount++;
					}
				}
				// エラー発生時は、該当商品のエントリーを出力しない
				else
				{
					iErrorCount++;
					continue;
				}
			}
			
			//------------------------------------------------------
			// データフィードXML作成
			//------------------------------------------------------
			xmlTextWriter.WriteString("\r\n");
			// <feed>ノード終了
			xmlTextWriter.WriteEndElement();

			//------------------------------------------------------
			// 処理結果出力
			//------------------------------------------------------
			// エラーが発生した場合
			if (iErrorCount > 0)
			{
				m_sbErrorMessages.Append(iErrorCount + "件の対象商品で、データフィード作成時にエラーが発生しました。\n");
			}
			// 全件正常に処理された場合
			else
			{
				AppLogger.WriteInfo("処理件数：" + iProcessedCount + "　データフィードが正常に作成されました。");
			}
			
			// ストリームを閉じる
			xmlTextWriter.Close();
		}

		/// <summary>
		/// エントリーノード作成
		/// </summary>
		/// <param name="drvProduct">商品情報</param>
		/// <returns>商品情報エントリーノード</returns>
		private string CreateEntryNode(DataRowView drvProduct)
		{
			using (StringWriter sw = new StringWriter())
			using (XmlTextWriter xmlTextWriter = new XmlTextWriter(sw))
			{
				// 出力書式設定
				xmlTextWriter.Formatting = Formatting.Indented;
				// インデント設定
				xmlTextWriter.Indentation = 2;

				// <entry>ノード開始
				xmlTextWriter.WriteStartElement("entry");

				//------------------------------------------------------
				// データマッピング
				//------------------------------------------------------
				// 商品名
				xmlTextWriter.WriteStartElement("title");
				xmlTextWriter.WriteCData((((string)drvProduct[Constants.FIELD_PRODUCT_NAME]).Length > CONST_PRODUCT_NAME_MAX_LENGTH)
					? StringUtility.StrTrim((string)drvProduct[Constants.FIELD_PRODUCT_NAME], CONST_PRODUCT_NAME_MAX_LENGTH)
					: (string)drvProduct[Constants.FIELD_PRODUCT_NAME]);
				xmlTextWriter.WriteEndElement();
					
				// 商品リンク
				xmlTextWriter.WriteStartElement("link");
				xmlTextWriter.WriteCData(Constants.URL_FRONT_PC + ProductCommon.CreateProductDetailUrl(
					(string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID],
					(string)drvProduct[Constants.FIELD_PRODUCT_CATEGORY_ID1],
					"",
					"",
					(string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID],
					(string)drvProduct[Constants.FIELD_PRODUCT_NAME],
					""));
				xmlTextWriter.WriteEndElement();

				// 商品説明(Google仕様では全角5000文字制限があるが、w2の商品概要の最大長の方が短いため文字数チェックは行わない)
				xmlTextWriter.WriteStartElement("description");
				xmlTextWriter.WriteCData(StringUtility.ToValue(StringUtility.ToNull((string)drvProduct[Constants.FIELD_PRODUCT_OUTLINE]), "商品説明なし").ToString());
				xmlTextWriter.WriteEndElement();

				// 商品ID
				string strProductId = (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID];
					
				// 半角英数以外の文字種が含まれている場合は、商品情報を出力しない
				if(Validator.IsHalfwidthAlphNum(strProductId) == false)
				{
					m_sbErrorMessages.Append("商品ID：" + (string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID] + "　商品IDに半角英数以外の文字種が含まれています。\n");
					return null;
				}

				xmlTextWriter.WriteStartElement("g:id");
				xmlTextWriter.WriteCData((string)drvProduct[Constants.FIELD_PRODUCT_PRODUCT_ID]);
				xmlTextWriter.WriteEndElement();

				// 商品価格
				xmlTextWriter.WriteStartElement("g:price");
				xmlTextWriter.WriteString(
					string.IsNullOrEmpty(drvProduct[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE].ToString())
					? drvProduct[Constants.FIELD_PRODUCT_DISPLAY_PRICE].ToPriceString() + " JPY"
					: drvProduct[Constants.FIELD_PRODUCT_DISPLAY_SPECIAL_PRICE].ToPriceString() + " JPY");
				xmlTextWriter.WriteEndElement();

				// 状態(新品、中古品、再生品)
				xmlTextWriter.WriteStartElement("g:condition");
				xmlTextWriter.WriteString("new");
				xmlTextWriter.WriteEndElement();

				// ブランド
				xmlTextWriter.WriteStartElement("g:brand");
				xmlTextWriter.WriteCData((string)drvProduct[Constants.FIELD_PRODUCT_BRAND_ID1]);
				xmlTextWriter.WriteEndElement();

				// 商品画像(LLサイズ画像)
				xmlTextWriter.WriteStartElement("g:image_link");
				if (string.IsNullOrEmpty((string)drvProduct[Constants.FIELD_PRODUCT_IMAGE_HEAD]) == false)
				{
					xmlTextWriter.WriteCData(Constants.URL_FRONT_PC + Constants.PATH_PRODUCTIMAGES + (string)drvProduct[Constants.FIELD_PRODUCT_SHOP_ID] + "/"
						+ (string)drvProduct[Constants.FIELD_PRODUCT_IMAGE_HEAD] + Constants.PRODUCTIMAGE_FOOTER_LL);
				}
				xmlTextWriter.WriteEndElement();

				// 在庫数
				int stock = (int)drvProduct[Constants.FIELD_PRODUCTSTOCK_STOCK];
				xmlTextWriter.WriteStartElement("g:quantity");
				xmlTextWriter.WriteValue(stock);
				xmlTextWriter.WriteEndElement();

				// 在庫状況
				xmlTextWriter.WriteStartElement("g:availability");
				xmlTextWriter.WriteString(GetAvailability(stock));
				xmlTextWriter.WriteEndElement();

				// <entry>ノード終了
				xmlTextWriter.WriteEndElement();

				return sw.ToString();
			}
		}

		/// <summary>
		/// 在庫情報文言取得
		/// </summary>
		/// <remarks>
		/// 在庫あり [in stock] : 実装済み
		/// 在庫なし [out of stock] : 実装済み
		/// 在庫僅少 [limited availability] : 未実装
		/// </remarks>
		/// <param name="stock">在庫数</param>
		/// <returns>在庫状況</returns>
		private string GetAvailability(int stock)
		{
			if (stock == 0) return "out of stock"; // 在庫なしの場合
			return "in stock"; // 在庫がある場合
		}

		/// <summary>
		/// メール送信処理（成功時）
		/// </summary>
		private static void SendMail()
		{
			SendMail(null);
		}

		/// <summary>
		/// メール送信処理（失敗時）
		/// </summary>
		/// <param name="ex">例外（NULLなら成功）</param>
		private static void SendMail(Exception ex)
		{
			using (SmtpMailSender smsMailSender = new SmtpMailSender(Constants.SERVER_SMTP))
			{
				// メール送信デフォルト値設定
				smsMailSender.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => smsMailSender.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => smsMailSender.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => smsMailSender.AddBcc(mail.Address));

				StringBuilder sbMessage = new StringBuilder();
				if ((ex != null) || (m_sbErrorMessages.Length != 0))
				{
					sbMessage.Append("失敗").Append("\n");
					sbMessage.Append(m_sbErrorMessages.ToString()).Append("\n");
					sbMessage.Append(BaseLogger.CreateExceptionMessage(ex)).Append("\n");
				}
				else
				{
					sbMessage.Append("成功").Append("\n");
				}
				smsMailSender.SetSubject(Constants.MAIL_SUBJECTHEAD);
				smsMailSender.SetBody(sbMessage.ToString());

				// メール送信
				if (smsMailSender.SendMail() == false)
				{
					Exception ex2 = smsMailSender.MailSendException;
					FileLogger.WriteError(ex2);
				}
			}
		}

	}
}
