/*
=========================================================================================================
  Module      : サイトマップXML作成処理(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2010 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.App.Common;
using w2.Common.Logger;
using w2.Common.Net.Mail;

namespace w2.Commerce.Batch.CreateSitemapXml
{
	class Program
	{
		/// <summary>
		/// プログラムのエントリポイント
		/// </summary>
		static void Main()
		{
			string strErrorMessageAll = "";
			string strErrorMessagePc = "";

			try
			{
				// 初期化
				Initialize();

				//------------------------------------------------------
				// バッチ起動をイベントログ出力
				//------------------------------------------------------
				AppLogger.WriteInfo("起動");

				//------------------------------------------------------
				// PC用サイトマップXML作成
				//------------------------------------------------------
				// PCサイトルートディレクトリとPC用サイトマップ設定ファイルが設定されている場合
				if (Constants.PHYSICALDIRPATH_FRONT_PC != "" && Constants.FILE_SITEMAPSETTING_PC != "")
				{
					try
					{
						SitemapXmlCreator smcSitemapXmlCreator = new SitemapXmlCreator();
						// PC用サイトマップXML作成
						smcSitemapXmlCreator.CreateSitemapXml();
						strErrorMessagePc += smcSitemapXmlCreator.AlartMessage;
					}
					catch (Exception ex)
					{
						strErrorMessagePc += ex.Message;

						// ログ出力
						AppLogger.WriteError(ex.ToString());
					}
				}

				//------------------------------------------------------
				// バッチ終了をイベントログ出力
				//------------------------------------------------------
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				strErrorMessageAll += ex.Message;

				// ログ出力
				AppLogger.WriteError(ex.ToString());
			}

			//------------------------------------------------------
			// エラーメッセージがあればメール送信
			// (ただし、管理画面からサイトマップを管理している場合はエラーメッセージの送信はしない)
			//------------------------------------------------------
			var errorMessage = (Constants.SITEMAP_OPTION_ENABLED == false)
				? CreateErrorMessage(strErrorMessageAll, strErrorMessagePc)
				: string.Empty;
			if (string.IsNullOrEmpty(errorMessage) == false)
			{
				try
				{
					// メール送信
					SendMail(errorMessage);
				}
				catch (Exception ex)
				{
					// ログ出力
					AppLogger.WriteError(ex.ToString());
				}
			}
		}

		/// <summary>
		/// 設定初期化
		/// </summary>
		private static void Initialize()
		{
			//------------------------------------------------------
			// アプリケーション設定読み込み
			//------------------------------------------------------
			// アプリケーション名設定
			Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

			// アプリケーション共通の設定
			var csSetting = new ConfigurationSetting(
				Properties.Settings.Default.ConfigFileDirPath,
				ConfigurationSetting.ReadKbn.C000_AppCommon,
				ConfigurationSetting.ReadKbn.C100_SiteCommon,
				ConfigurationSetting.ReadKbn.C100_BatchCommon,
				ConfigurationSetting.ReadKbn.C200_CommonFront,
				ConfigurationSetting.ReadKbn.C300_Pc,
				ConfigurationSetting.ReadKbn.C200_CreateSitemapXml);

			//------------------------------------------------------
			// アプリケーション固有の設定
			//------------------------------------------------------
			// PC用サイトマップ設定ファイルパス
			Constants.FILE_SITEMAPSETTING_PC = csSetting.GetAppStringSetting("File_SitemapSetting_Pc");

			// 結果メール送信先設定
			Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
			Constants.MAIL_FROM = csSetting.GetAppMailAddressSetting("Mail_From");
			Constants.MAIL_TO_LIST = csSetting.GetAppMailAddressSettingList("Mail_To");
			Constants.MAIL_CC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Cc");
			Constants.MAIL_BCC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Bcc");

			// PCルートURL設定
			Constants.URL_FRONT_PC = Constants.PROTOCOL_HTTP + csSetting.GetAppStringSetting("AccessLog_TargetDomain") + Constants.PATH_ROOT_FRONT_PC;
			// フレンドリーURLの有効/無効
			Constants.FRIENDLY_URL_ENABLED = csSetting.GetAppBoolSetting("FriendlyUrlEnabled");
		}

		/// <summary>
		/// メール送信用エラーメッセージ作成
		/// </summary>
		/// <param name="strErrorMessageAll">全般エラーメッセージ</param>
		/// <param name="strErrorMessagePc">PCエラーメッセージ</param>
		/// <returns>エラーメッセージ</returns>
		private static string CreateErrorMessage(string strErrorMessageAll, string strErrorMessagePc)
		{
			StringBuilder sbErrorMessage = new StringBuilder();

			if (strErrorMessageAll != "")
			{
				sbErrorMessage.Append(strErrorMessageAll).Append("\r\n");
			}

			if (strErrorMessagePc != "")
			{
				sbErrorMessage.Append("【PC用サイトマップ作成エラー】").Append("\r\n");
				sbErrorMessage.Append(strErrorMessagePc).Append("\r\n");
			}

			return sbErrorMessage.ToString();
		}

		/// <summary>
		/// メール送信処理
		/// </summary>
		/// <param name="strMessage">メール本文</param>
		private static void SendMail(string strMessage)
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

