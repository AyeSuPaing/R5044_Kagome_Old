/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2013 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Common.Net.Mail;

namespace w2.CustomerSupport.Batch.CsMailReceiver
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	class Program
	{
		#region -Main メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		/// <param name="args">引数</param>
		/// <returns>取り込み件数、エラー時は-1</returns>
		static int Main(string[] args)
		{
			int returnCode = -1; // エラーコード

			try
			{
				Program program = new Program();

				// 二重起動禁止
				bool isSuccess = ProcessUtility.ExcecWithProcessMutex(() =>
				{
					returnCode = program.ReceiveMails(args);
				});
				if (isSuccess == false)
				{
					returnCode = -2; // 二重起動エラー
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);
				Console.WriteLine(ex);
			}

			return returnCode;
		}
		#endregion

		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		Program()
		{
			// 初期化処理
			Initialize();
		}
		#endregion

		#region -Initialize 初期化処理
		/// <summary>
		/// 初期化処理
		/// </summary>
		private void Initialize()
		{
			try
			{
				// アプリケーション設定読み込み
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定
				var appSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_CsMailReceiver);

				Constants.POP_ACCOUNT_LIST = GetPop3AccountSettings(appSetting);
				Constants.GMAIL_ACCOUNT_LIST = GetGmailAccountSettings(appSetting);
				Constants.EXCHANGE_ACCOUNT_LIST = GetExchangeAccountSettings(appSetting);
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}
		#endregion

		#region -ReceiveMails メール受信
		/// <summary>
		/// メール受信
		/// </summary>
		/// <param name="args">引数</param>
		/// <returns>取り込み件数</returns>
		private int ReceiveMails(string[] args)
		{
			var count = 0;

			// 開始ログ出力
			WriteLog("取り込み開始...");

			if (args.Any())
			{
				count += new MailReceiver().ImportMail(args[0]);
			}
			else
			{
				count += ReceivePop3Mails();
			}

			count += ReceiveGmails();
			count += ReceiveExchangeMails();

			// 終了ログ出力
			WriteLog(string.Format("取り込み終了. {0}通のメールを取り込みました。", count));

			return count;
		}
		#endregion

		#region -WriteLog ログ出力（コンソール＋Infoログ）
		/// <summary>
		/// ログ出力（コンソール＋Infoログ）
		/// </summary>
		/// <param name="message">ログメッセージ</param>
		private void WriteLog(string message)
		{
			Console.WriteLine(message);
			AppLogger.WriteInfo(message);
		}
		#endregion

		#region GetSetting
		/// <summary>
		/// Get POP3 account settings
		/// </summary>
		/// <param name="setting">Configuration setting</param>
		/// <returns>A list of POP3 account settings</returns>
		private static List<Pop3Client.PopAccountSetting> GetPop3AccountSettings(ConfigurationSetting setting)
		{
			// メール受信設定
			var accountSettings = new List<Pop3Client.PopAccountSetting>();
			for (var index = 0; index < Constants.POP_ACCOUNT_MAX_COUNT; index++)
			{
				var settingName = string.Format("PopAccount{0}", index + 1);
				var accountSetting = setting.GetAppStringSetting(settingName).Split(',');
				if (string.IsNullOrEmpty(accountSetting[0]) == false)
				{
					// PopServerのIPアドレスが空でないものだけ追加
					accountSettings.Add(new Pop3Client.PopAccountSetting(accountSetting));
				}
			}

			return accountSettings;
		}

		/// <summary>
		/// Get Gmail account settings
		/// </summary>
		/// <param name="setting">Configuration setting</param>
		/// <returns>A list of Gmail account settings</returns>
		private static List<GmailAccountSetting> GetGmailAccountSettings(ConfigurationSetting setting)
		{
			var physicalDirpath = Path.Combine(
				Properties.Settings.Default.ConfigFileDirPath,
				setting.GetAppStringSetting("PhysicalDirpathGmailApi"));
			var accountSettings = new List<GmailAccountSetting>();

			for (var index = 0; index < Constants.GMAIL_ACCOUNT_MAX_COUNT; index++)
			{
				var settingName = string.Format("GmailAccount{0}", index + 1);
				var accountSetting = setting.GetAppStringSetting(settingName).Split(',');
				if (string.IsNullOrEmpty(accountSetting[0]) == false)
				{
					accountSettings.Add(
						new GmailAccountSetting(
							settingName,
							physicalDirpath,
							accountSetting));
				}
			}

			return accountSettings;
		}

		/// <summary>
		/// Get Exchange account settings
		/// </summary>
		/// <param name="setting">Configuration setting</param>
		/// <returns>A list of Exchange account settings</returns>
		private static List<ExchangeAccountSetting> GetExchangeAccountSettings(ConfigurationSetting setting)
		{
			var physicalDirpath = Path.Combine(
				Properties.Settings.Default.ConfigFileDirPath,
				setting.GetAppStringSetting("PhysicalDirpathExchangeApi"));
			var accountSettings = new List<ExchangeAccountSetting>();

			for (var index = 0; index < Constants.EXCHANGE_ACCOUNT_MAX_COUNT; index++)
			{
				var settingName = string.Format("ExchangeAccount{0}", index + 1);
				var accountSetting = setting.GetAppStringSetting(settingName).Split(',');
				if (string.IsNullOrEmpty(accountSetting[0]) == false)
				{
					accountSettings.Add(
						new ExchangeAccountSetting(
							settingName,
							physicalDirpath,
							accountSetting));
				}
			}

			return accountSettings;
		}
		#endregion

		#region ReceiveMail
		/// <summary>
		/// Receive POP3 mails
		/// </summary>
		/// <returns>Total imported mail successfully</returns>
		private static int ReceivePop3Mails()
		{
			var count = 0;

			foreach (var setting in Constants.POP_ACCOUNT_LIST)
			{
				if (setting == null) continue;

				// Check Gmail && [recent:] is not attached to the beginning of Gmail account.
				if (setting.Server.ToLower().Contains("gmail")
					&& (setting.UserId.ToLower().StartsWith("recent:") == false))
				{
					// ログ出力
					var errorString = "[SYSTEM] Gmailアカウントの先頭にrecent:が付いていないため、メール取り込みを中止しました。";
					Console.WriteLine(errorString);
					AppLogger.WriteError(errorString);
					continue;
				}

				var receiver = new MailReceiver(setting);
				count += receiver.ReceiveAndImport();
			}

			return count;
		}

		/// <summary>
		/// Receive Gmails
		/// </summary>
		/// <returns>Total imported mail successfully</returns>
		private static int ReceiveGmails()
		{
			var count = 0;
			foreach (var setting in Constants.GMAIL_ACCOUNT_LIST)
			{
				if (setting == null) continue;

				var receiver = new GmailReceiver(setting);
				count += receiver.ReceiveAndImport();
			}

			return count;
		}

		/// <summary>
		/// Receive Exchange mails
		/// </summary>
		/// <returns>Total imported mail successfully</returns>
		private static int ReceiveExchangeMails()
		{
			var count = 0;
			foreach (var setting in Constants.EXCHANGE_ACCOUNT_LIST)
			{
				if (setting == null) continue;

				var receiver = new ExchangeReceiver(setting);
				count += receiver.ReceiveAndImport();
			}

			return count;
		}
		#endregion
	}
}
