/*
=========================================================================================================
  Module      : Criteo連携ファイル出力(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.Global.Config;
using w2.Common;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Commerce.Batch.ExportCriteoDatas.Proc;

namespace w2.Commerce.Batch.ExportCriteoDatas
{
	/// <summary>
	/// Criteo連携ファイル出力
	/// </summary>
	public class Program
	{	
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		/// <param name="args">コマンドライン引数</param>
		static void Main(string[] args)
		{
			try
			{
				// 初期化
				Initialize();

				// Criteo連携用オブジェクト生成
				CriteoIntegrator integrator = CreateIntegrator();

				try
				{
					// バッチ起動をイベントログ出力
					AppLogger.WriteInfo("起動");

					Export(integrator);

					// バッチ終了をイベントログ出力
					AppLogger.WriteInfo("正常終了");
				}
				catch (Exception ex)
				{
					SendErrorMail(ex.Message, ex);
				}
			}
			catch (Exception ex)
			{
				SendErrorMail("予期せぬエラーが発生しました。詳細は以下のメッセージを参照して下さい。", ex);
			}
		}
		
		/// <summary>
		/// 連携オブジェクト生成
		/// </summary>
		/// <returns>連携オブジェクト</returns>
		private static CriteoIntegrator CreateIntegrator()
		{
			CriteoIntegrationSetting setting = CreateIntegrationSetting();
			CriteoIntegrator integrator = new CriteoIntegrator(setting);

			return integrator;
		}

		/// <summary>
		/// 連携設定を作成する
		/// </summary>
		/// <returns>設定済み連携設定</returns>
		private static CriteoIntegrationSetting CreateIntegrationSetting()
		{
			var config = Properties.Settings.Default;
			CriteoSiteSetting setting = new CriteoSiteSetting(
				config.FileName,
				config.FileUploadDirPath,
				config.SiteRoot,
				config.ProductPage,
				config.ImageRoot);

			return new CriteoIntegrationSetting(setting);
		}

		/// <summary>
		/// 出力処理
		/// </summary>
		/// <param name="integrator">連携オブジェクト</param>
		private static void Export(CriteoIntegrator integrator)
		{
			AppLogger.WriteInfo("ファイル出力開始");
			integrator.Export();

			SendMail(integrator);
		}		

		/// <summary>
		/// 設定初期化
		/// </summary>
		private static void Initialize()
		{
			try
			{
				//------------------------------------------------------
				// SQL接続文字列設定
				//------------------------------------------------------
				Constants.STRING_SQL_CONNECTION = Properties.Settings.Default.SqlConnection;

				//------------------------------------------------------
				// アプリケーション設定読み込み
				//------------------------------------------------------
				// アプリケーション名設定
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;
				Constants.PHYSICALDIRPATH_LOGFILE = Properties.Settings.Default.Directory_LogFilePath;
				SetServerSmtp();

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// ディレクトリ・パス
				Constants.PHYSICALDIRPATH_SQL_STATEMENT = AppDomain.CurrentDomain.BaseDirectory + Constants.DIRPATH_XML_STATEMENTS;
				Constants.FILE_NAME = Properties.Settings.Default.FileName;
				Constants.PHYSICALDIRPATH_UPLOAD_FILE = Properties.Settings.Default.FileUploadDirPath;
				Constants.CRITEO_MAIL_TITLE = Properties.Settings.Default.MailTitle;
				Constants.CRITEO_MAIL_FROM = Properties.Settings.Default.MailFrom;
				Constants.CRITEO_MAIL_TO = Properties.Settings.Default.MailTo;
				Constants.CRITEO_MAIL_CC = Properties.Settings.Default.MailCc;
				Constants.CRITEO_MAIL_BCC = Properties.Settings.Default.MailBcc;
				Constants.PHYSICALDIRPATH_CUSTOMER_RESOUERCE = Properties.Settings.Default.ConfigFileDirPath;

				// アプリケーション共通
				Constants.GLOBAL_CONFIGS = GlobalConfigs.GetInstance();
			}
			catch (Exception ex)
			{
				throw new ApplicationException("設定値の読み込みに失敗しました。\r\n" + ex.ToString());
			}
		}

		/// <summary>
		/// SMTPサーバ設定
		/// </summary>
		private static void SetServerSmtp()
		{
			// SMTPサーバ設定
			// 配列順内容：Server,Port,AuthType,PopServer,PopPort,PopType,UserName,Password
			string[] strSmtpSettings = Properties.Settings.Default.Server_Smtp_Settings.Split(',');
			Constants.SERVER_SMTP = strSmtpSettings[0];
			Constants.SERVER_SMTP_PORT = int.Parse(strSmtpSettings[1]);
			foreach (Enum e in Enum.GetValues(typeof(w2.Common.SmtpAuthType)))
			{
				if (e.ToString().ToUpper() == strSmtpSettings[2].ToUpper())
				{
					Constants.SERVER_SMTP_AUTH_TYPE = (w2.Common.SmtpAuthType)e;
					break;
				}
			}
			if (Constants.SERVER_SMTP_AUTH_TYPE == w2.Common.SmtpAuthType.PopBeforeSmtp)
			{
				Constants.SERVER_SMTP_AUTH_POP_SERVER = strSmtpSettings[3];
				Constants.SERVER_SMTP_AUTH_POP_PORT = strSmtpSettings[4];
				foreach (Enum e in Enum.GetValues(typeof(w2.Common.PopType)))
				{
					if (e.ToString().ToUpper() == strSmtpSettings[5].ToUpper())
					{
						Constants.SERVER_SMTP_AUTH_POP_TYPE = (w2.Common.PopType)e;
						break;
					}
				}
				Constants.SERVER_SMTP_AUTH_USER_NAME = strSmtpSettings[6];
				Constants.SERVER_SMTP_AUTH_PASSOWRD = strSmtpSettings[7];
			}
			else if (Constants.SERVER_SMTP_AUTH_TYPE == w2.Common.SmtpAuthType.SmtpAuth)
			{
				Constants.SERVER_SMTP_AUTH_USER_NAME = strSmtpSettings[6];
				Constants.SERVER_SMTP_AUTH_PASSOWRD = strSmtpSettings[7];
			}
		}

		/// <summary>
		/// エラーメール送信
		/// </summary>
		/// <param name="body">メール本文</param>
		/// <param name="ex">例外</param>
		private static void SendErrorMail(string body, Exception ex)
		{
			AppLogger.WriteError(ex);
			SendMail("【失敗】 " + Constants.CRITEO_MAIL_TITLE, body + "\r\n" + FileLogger.CreateExceptionMessage(ex));
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="integrator">連携オブジェクト</param>
		static void SendMail(CriteoIntegrator integrator)
		{
			string body = string.Format("{0} 件の商品情報の出力に成功しました。", integrator.NumberOf);
			SendMail("【成功】 " + Constants.CRITEO_MAIL_TITLE, body);
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="title">メールタイトル</param>
		/// <param name="body">メール本文</param>
		private static void SendMail(string title, string body)
		{
			using (SmtpMailSender sender = new SmtpMailSender(Constants.SERVER_SMTP))
			{
				var config = Properties.Settings.Default;
				sender.SetFrom(config.MailFrom);
				foreach (string to in config.MailTo) sender.AddTo(to);
				foreach (string cc in config.MailCc) sender.AddTo(cc);
				foreach (string bcc in config.MailBcc) sender.AddTo(bcc);
				sender.SetSubject(title);
				sender.SetBody(body);

				if (sender.SendMail() == false)
				{
					throw new Exception("メールの送信に失敗しました。");
				}
			}
		}
	}
}
