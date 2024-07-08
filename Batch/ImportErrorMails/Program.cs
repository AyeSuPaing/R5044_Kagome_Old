/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.Common;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Common.Sql;
using w2.App.Common;

namespace w2.MarketingPlanner.Batch.ImportErrorMails
{
	class Program
	{
		/// <summary>
		/// アプリケーションエントリポイント
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			try
			{
				// 実体作成
				var objProgram = new Program();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				if (Constants.SERVER_POP != "")
				{
					using (var pop3 = new Pop3Client())
					{
						pop3.Connect();
						var mailList = pop3.GetMailLists();

						foreach (var mailKey in mailList.Keys)
						{
							var message = pop3.GetMessageByMailKey(mailKey);
							if (message == null) continue;
							objProgram.Import(message);

							pop3.DeleteMessageByMailKey(mailKey);
						}
					}
				}

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
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
					ConfigurationSetting.ReadKbn.C200_ImportErrorMails);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// POPサーバ設定
				Constants.SERVER_POP = csSetting.GetAppStringSetting("ImportErrorMails_PopServer_Server");
				Constants.SERVER_POP_TYPE = (PopType)csSetting.GetAppSetting("ImportErrorMails_PopServer_PopType", typeof(PopType));
				Constants.SERVER_POP_PORT = csSetting.GetAppIntSetting("ImportErrorMails_PopServer_Port");
				Constants.SERVER_POP_AUTH_USER_NAME = csSetting.GetAppStringSetting("ImportErrorMails_PopServer_UserName");
				Constants.SERVER_POP_AUTH_PASSOWRD = csSetting.GetAppStringSetting("ImportErrorMails_PopServer_Password");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// エラーメール取込
		/// </summary>
		/// <param name="pop3Message">メール</param>
		private void Import(Pop3Message pop3Message)
		{
			using (var sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();
				try
				{
					// 取込
					var errorMailImporter = new ErrorMailImporter();
					errorMailImporter.Import(pop3Message, sqlAccessor);
				}
				catch (Exception ex)
				{
					// 例外が発生した場合はログに落とす
					var sbErrorMail = new StringBuilder();
					sbErrorMail.Append("\r\n");
					sbErrorMail.Append(pop3Message.Subject).Append("\r\n");
					sbErrorMail.Append(pop3Message.BodyDecoded).Append("\r\n");
					FileLogger.WriteError(sbErrorMail.ToString(), ex);
				}
			}
		}
	}
}
