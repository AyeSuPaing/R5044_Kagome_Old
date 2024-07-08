/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using System.Collections;
using System.Linq;
using w2.Common.Logger;
using w2.App.Common;
using w2.App.Common.Util;
using w2.Commerce.Batch.UserIntegrationCreator.Commands;

namespace w2.Commerce.Batch.UserIntegrationCreator
{
	/// <summary>
	/// プログラム本体
	/// </summary>
	class Program
	{
		#region +メイン処理
		/// <summary>
		/// メイン処理
		/// </summary>
		public static void Main(string[] args)
		{
			try
			{
				// 本プログラムインスタンス作成
				var program = new Program();

				AppLogger.WriteInfo("起動");

				// 二重起動禁止
				bool isSuccess = ProcessUtility.ExcecWithProcessMutex(program.Start);
				if (isSuccess == false)
				{
					throw new Exception("他プロセスが起動しているため、起動に失敗しました。二重起動は禁止されています。");
				}

				// 結果メール送信
				SendResurtMail(true, program.ExecResultMessage);

				AppLogger.WriteInfo("終了");
			}
			catch (Exception ex)
			{
				var errorMessage = ex.ToString();
				try
				{
					// 結果メール送信
					SendResurtMail(false, BaseLogger.CreateExceptionMessage(ex));
				}
				catch (Exception ex2)
				{
					errorMessage += "\r\n" + ex2.ToString();
				}

				AppLogger.WriteError(ex);
			}
		}
		#endregion

		#region +コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			// 初期化処理
			Initialize();
		}
		#endregion

		#region -初期化処理
		/// <summary>
		/// 初期化処理
		/// </summary>
		private static void Initialize()
		{
			try
			{
				// アプリケーション設定読み込み
				Constants.APPLICATION_NAME = Properties.Settings.Default.Application_Name;

				// アプリケーション共通の設定
				var appSetting
					= new w2.App.Common.ConfigurationSetting(
						Properties.Settings.Default.ConfigFileDirPath,
						w2.App.Common.ConfigurationSetting.ReadKbn.C000_AppCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C100_BatchCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C300_UserIntegrationCreator);

				// アプリケーション固有の設定
				Constants.PARALLEL_WORKERTHREADS = int.Parse(appSetting.GetAppStringSetting("UserIntegration_ParallelWorkerThreads"));
				// メール送信先設定
				Constants.MAIL_SUBJECTHEAD = appSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = appSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = appSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = appSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = appSetting.GetAppMailAddressSettingList("Mail_Bcc");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}
		#endregion

		#region -プログラム開始
		/// <summary>
		/// プログラム開始
		/// </summary>
		private void Start()
		{
			// 最終実行日時取得
			var targetStart = GetLastExec();
			var targetEnd = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:00"));

			this.StartTime = DateTime.Parse(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

			// 24時間処理が続くと１度だけメールを送信する
			var timer = new System.Timers.Timer();
			timer.AutoReset = false;
			timer.Interval = 86400000.0;
			timer.Elapsed += SendMailEvent;
			timer.Enabled = true;

			// ユーザー統合情報作成
			this.ExecResultMessage = new CreateUserIntegrationCommand().Exec(targetStart, targetEnd);

			// 正常終了したら進捗ファイル更新
			UpdateLastExecFile(targetEnd);
		}
		#endregion

		#region -結果メール送信
		/// <summary>
		/// 結果メール送信
		/// </summary>
		/// <param name="isSuccess">成功？</param>
		/// <param name="message">メッセージ</param>
		private static void SendResurtMail(bool isSuccess, string message)
		{
			using (var mailSender = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, Constants.CONST_MAIL_ID_CREATE_USERINTEGRATION, "", new Hashtable(), true, Constants.MailSendMethod.Auto))
			{
				mailSender.SetBody(message.ToString());
				mailSender.SetSubject("【" + (isSuccess ? "成功" : "失敗") + "】" + mailSender.Subject + "[" + DateTime.Now.ToString("MM/dd") + "]");
				if (mailSender.SendMail() == false)
				{
					FileLogger.WriteError(message.ToString(), mailSender.MailSendException);
				}
			}
		}
		#endregion

		#region メール送信
		/// <summary>
		/// メール送信イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SendMailEvent(object sender, System.Timers.ElapsedEventArgs e)
		{
			SendUserIntegrationAlertMail(
				"ユーザー統合バッチアラート",
				"ユーザー統合バッチが1日以上稼働している為、確認してください。\r\n実行開始時間：" + this.StartTime);
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="message">メッセージ</param>
		private static void SendUserIntegrationAlertMail(string subject, string message)
		{
			using (var sender = new MailSendUtility(Constants.MailSendMethod.Auto))
			{
				// 送信元、送信先設定
				sender.SetSubject(Constants.MAIL_SUBJECTHEAD + subject);
				sender.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => sender.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => sender.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => sender.AddBcc(mail.Address));

				// 本文設定
				sender.SetBody(message);

				if (sender.SendMail() == false)
				{
					// ログ出力
					AppLogger.WriteError("ユーザー統合バッチアラートメールが送信できませんでした。", sender.MailSendException);
				}
			}
		}
		#endregion

		#region 最終実行日時関連
		/// <summary>
		/// 最終実行日時取得
		/// </summary>
		/// <returns>最終実行日時</returns>
		private DateTime GetLastExec()
		{
			try
			{
				var lastExec = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, Constants.FILENAME_LASTEXEC_PREFIX + "*");
				if (lastExec.Length > 0)
				{
					var datetime = Path.GetFileName(lastExec[0]).Substring(Constants.FILENAME_LASTEXEC_PREFIX.Length);
					return DateTime.ParseExact(datetime, Constants.FILENAME_LASTEXEC_SUFFIX_DATEFORMAT, null);
				}
			}
			catch (Exception)
			{
			}
			return DateTime.MinValue;
		}

		/// <summary>
		/// 進捗ファイル更新
		/// </summary>
		/// <param name="targetEnd">対象日時（終了）</param>
		private void UpdateLastExecFile(DateTime targetEnd)
		{
			Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, Constants.FILENAME_LASTEXEC_PREFIX + "*")
				.ToList().ForEach(File.Delete);
			File.Create(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
				Constants.FILENAME_LASTEXEC_PREFIX + targetEnd.ToString(Constants.FILENAME_LASTEXEC_SUFFIX_DATEFORMAT)));
		}
		#endregion

		#region プロパティ
		/// <summary>実行結果メッセージ</summary>
		public string ExecResultMessage { get; set; }
		/// <summary>処理実行開始日時</summary>
		private DateTime StartTime { get; set; }
		#endregion
	}
}