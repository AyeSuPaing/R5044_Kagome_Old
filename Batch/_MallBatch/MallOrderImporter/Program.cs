/*
=========================================================================================================
  Module      : プログラム(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.App.Common;
using w2.App.Common.Mall.Yahoo.Procedures;
using w2.Common.Net.Mail;
using w2.Common.Logger;

namespace w2.Commerce.MallBatch.MallOrderImporter
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
				// 取込実行
				Program program = new Program();

				AppLogger.WriteInfo("起動");

				var mallId = program.RetrieveMallId(args);
				program.Import(mallId);

				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				AppLogger.WriteError(ex);

				// メール送信
				var sender = new SmtpMailSender();
				sender.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(m => sender.AddTo(m.Address));
				Constants.MAIL_CC_LIST.ForEach(m => sender.AddCC(m.Address));
				sender.SetSubject("[" + Constants.PROJECT_NO + "] " + Properties.Settings.Default.Application_Name + "アラート");
				sender.SetBody(ex.ToString());
				sender.SendMail();
			}
		}

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
				ConfigurationSetting csSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C300_MallOrderImporter);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// メール送信先設定
				Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = csSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = csSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Bcc");

				// Yahoo API - アクセストークンを取得するためにアクセスするエンドポイント(Tokenエンドポイント)
				Constants.YAHOO_API_TOKEN_API_URL = csSetting.GetAppStringSetting("Yahoo_Api_Token_Api_Url");
				// Yahoo API - 注文詳細APIを実行するためのURL
				Constants.YAHOO_API_ORDERINFO_API_URL = csSetting.GetAppStringSetting("Yahoo_Api_OrderInfo_Api_Url");
			}
			catch (Exception ex)
			{
				throw new System.ApplicationException("Config.xmlファイルの読み込みに失敗しました。\r\n" + ex.ToString());
			}
		}

		/// <summary>
		/// モールIDを取得
		/// </summary>
		/// <param name="args">引数</param>
		/// <returns>モールID</returns>
		public string RetrieveMallId(string[] args)
		{
			if (args.Length == 0) { throw new ArgumentNullException("第一引数が不足しています。"); }
			return args[0];
		}

		/// <summary>
		/// 取込処理
		/// </summary>
		/// <param name="mallId">モールID</param>
		public void Import(string mallId)
		{
			// Yahoo注文取込
			new YahooMallOrderDetailImportProcedure().ImportYahooMallOrderDetails(mallId: mallId);
		}
	}
}
