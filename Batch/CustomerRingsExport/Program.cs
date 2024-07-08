/*
=========================================================================================================
  Module      : カスタマーリングス用出力バッチ(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using w2.Common.Logger;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Commerce.Batch.CustomerRingsExport.Action;

namespace w2.Commerce.Batch.CustomerRingsExport
{
	class Program
	{
		/// <summary>
		/// メイン
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			try
			{
				// 初期化
				Initialize();

				// バッチ起動をイベントログ出力
				EventLogger.WriteInfo("起動");

				bool isSuccess = ProcessUtility.ExcecWithProcessMutex(Execute);

				// バッチ終了をイベントログ出力
				EventLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);

				// メール送信
				SendMail(ex.ToString());
			}
		}

		/// <summary>
		/// 処理実行
		/// </summary>
		private static void Execute()
		{
			var exportFileSettingPath =  Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE + @"CustomerRingsExport\{0}\ExportFileSettings.xml";
			var storeDirectory = File.Exists(string.Format(exportFileSettingPath, Constants.PROJECT_NO)) ? Constants.PROJECT_NO : "Default";
			var defineList = new FileDefines(string.Format(exportFileSettingPath, storeDirectory));
			var actionList = new List<ActionExportBase>();
			foreach (var target in defineList.Defines)
			{
				switch (target.Name)
				{
					case "UserExtend":
						actionList.Add(new ActionExportUserExtend(target));
						break;

					case "ProductTag":
						actionList.Add(new ActionExportProductTag(target));
						break;

					default:
						actionList.Add(new ActionExport(target));
						break;
				}

			}
			actionList.ForEach(ExecAction);

			// メール送信
			SendMail(actionList);
		}

		/// <summary>
		/// アクション実行
		/// </summary>
		/// <param name="action"></param>
		private static void ExecAction(ActionExportBase action)
		{
			action.Execute();
			MailMessages.Append(action.MailMessage);
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="actions">実行するアクション一覧</param>
		private static void SendMail(IEnumerable<ActionExportBase> actions)
		{
			var result = string.Empty;
			var actionResults = actions.Select(item => item.IsSuccess).ToList();

			result = actions.Any(item => item.IsSuccess == false) ? "異常終了" : "正常終了";

			var parameter = new Hashtable()
			{
				{ "result", result },
				{ "message", MailMessages.ToString() },
			};
			
			try
			{
				using (var sender = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, Constants.CONST_MAIL_ID_CUSTOMER_RINGS_EXPORT, "", parameter, true, Constants.MailSendMethod.Auto))
				{
					sender.SendMail();
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
				SendMail(MailMessages.ToString());
			}
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="message">メッセージ</param>
		private static void SendMail(string message)
		{
			using (var sender = new MailSendUtility(Constants.MailSendMethod.Auto))
			{
				// 送信元、送信先設定
				sender.SetSubject(Constants.MAIL_SUBJECTHEAD);
				sender.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => sender.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => sender.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => sender.AddBcc(mail.Address));

				// 本文設定
				sender.SetBody(message);

				if (sender.SendMail() == false)
				{
					// ログ出力
					AppLogger.WriteError("メール送信できませんでした。", sender.MailSendException);
				}
			}
		}
		/// <summary>
		/// 設定初期化
		/// </summary>
		private static void Initialize()
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
					ConfigurationSetting.ReadKbn.C200_CustomerRingsExport);

				// 出力先
				Constants.EXPORT_DIRECTORY = csSetting.GetAppStringSetting("Export_Directory");

				// カスタマーリングス設定
				Constants.CUSTOMER_RINGS_HOST = csSetting.GetAppStringSetting("CustomerRings_Host");
				Constants.CUSTOMER_RINGS_USER_NAME = csSetting.GetAppStringSetting("CustomerRings_UserName");
				Constants.CUSTOMER_RINGS_PASSWORD = csSetting.GetAppStringSetting("CustomerRings_Password");

				// FTPSの設定
				Constants.USE_PASSIVE = csSetting.GetAppBoolSetting("UsePassive");
				Constants.CHECK_SSL = csSetting.GetAppBoolSetting("CheckSsl");

				// メール送信先設定
				Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = csSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = csSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Bcc");

				// 初期化
				MailMessages = new StringBuilder();
			}
			catch (Exception ex)
			{
				throw new ApplicationException("設定ファイルの読み込みに失敗しました。\r\n" + ex);
			}
		}

		/// <summary>メールメッセージ</summary>
		private static StringBuilder MailMessages { get; set; }
	}
}
