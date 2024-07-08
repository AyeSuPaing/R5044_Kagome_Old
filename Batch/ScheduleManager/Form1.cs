/*
=========================================================================================================
  Module      : メインフォーム(Form1.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using w2.App.Common;
using w2.Common.Net.Mail;
using w2.Common.Util;

namespace w2.MarketingPlanner.Win.ScheduleManager
{
	public partial class Form1 : Form
	{
		private const int MAX_DISP_CHARS = 50000;		// MAX_DISP_CHARS ～ MAX_DISP_CHARS/2 文字表示

		private static ReaderWriterLock m_rwlDisplayBufferLock = new ReaderWriterLock();
		private static ReaderWriterLock m_rwlWriteBufferLock = new ReaderWriterLock();

		private static StringBuilder m_sbDisplayBuffer = new StringBuilder();
		private static StringBuilder m_sbWriteBuffer = new StringBuilder();

		private static bool m_blSetDispMessage = false;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Form1()
		{
			InitializeComponent();
		}

		// Xボタンでアプリケーションが終了しないで、ただWindowを隠すように設定
		// ただしWindowsの終了時には、アプリケーションを終了

		/// <summary>
		/// WndProcオーバーライド
		/// </summary>
		/// <param name="m"></param>
		protected override void WndProc(ref Message m)
		{
			const int WM_SYSCOMMAND = 0x0112;
			const int WM_DESTROY = 0x0002;
			const int SC_CLOSE = 0xF060;

			if (m.Msg == WM_SYSCOMMAND && m.WParam.ToInt32() == SC_CLOSE)
			{
				// UIでの閉じるがここに来る
				Hide();
				return;
			}

			if (m.Msg == WM_DESTROY)
			{
				// Window破棄がここに来る
				// HACK: 本当はObserverThreadとWorkerThreadをAbort()したいのだが、
				// それを行う手段がなかったのでプロセスごと終了してしまう。
				Environment.Exit(WM_DESTROY);
			}

			base.WndProc(ref m);
		} 

		/// <summary>
		/// フォームロードイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_Load(object sender, EventArgs e)
		{
			WriteInfoLogLine("");
			WriteInfoLogLine("------- アプリケーション開始 -------");
			
			// アプリケーション初期化
			InitializeApplication();

			// メインスレッド生成
			ScheduleObserver.CreateThread();

			WriteInfoLogLine("メインスレッド生成完了");
		}

		/// <summary>
		/// 初期化
		/// </summary>
		private void InitializeApplication()
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
					ConfigurationSetting.ReadKbn.C200_ScheduleManager);

				//------------------------------------------------------
				// コンフィグ監視起動（アプリケーション初期化処理セット）
				//------------------------------------------------------
				// AppConfigフォルダ配下の全てのファイルを対象とする
				FileUpdateObserver.GetInstance().AddObservationWithSubDir(
					Properties.Settings.Default.ConfigFileDirPath + @"AppConfig\",
					"*.*",
					InitializeApplication);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = new MailAddress(csSetting.GetAppStringSetting("Mail_From"));
				Constants.MAIL_TO_LIST = GetMailAddress(csSetting, "Mail_To");
				Constants.MAIL_CC_LIST = GetMailAddress(csSetting, "Mail_Cc");
				Constants.MAIL_BCC_LIST = GetMailAddress(csSetting, "Mail_Bcc");

				// デコメ画像ディレクトリ物理パス
				Constants.PHYSICALDIRPATH_DECOMEIMAGE = Constants.MARKETINGPLANNER_DECOME_MOBILEHTMLMAIL_DIRPATH;

				// メッセージデバッグ
				Constants.MESSAGE_DEBUG = csSetting.GetAppBoolSetting("MailDistribute_MessageDebug_Enabled");

				// ターゲットリスト抽出アラートメールのインターバル(分)
				Constants.SEND_ALERTMAIL_INTERVAL_MINUTES = csSetting.GetAppIntSetting("Send_AlertMail_IntervalMinutes");

				// メールクリックURL
				Constants.MAILCLICK_URL_PC = csSetting.GetAppStringSetting("MailDistribute_MailClickUrl");
				Constants.MAILCLICK_URL_MOBILE = csSetting.GetAppStringSetting("MailDistribute_MailClickUrl");

				// スレッド実行の最大数
				Constants.THREADS_MAX = csSetting.GetAppIntSetting("Threads_Max");

				// メール送信エラーポイント（定期注文＆会員ランク変更メールは指定以上エラーポイントの場合送信しない）
				Constants.SEND_MAIL_ERROR_POINT = csSetting.GetAppIntSetting("Send_Mail_Error_Point");

				// httpsプロトコル
				Constants.PROTOCOL_HTTPS = csSetting.GetAppStringSetting("Site_ProtocolHttps");
				// サイトのドメイン
				Constants.SITE_DOMAIN = csSetting.GetAppStringSetting("Site_Domain");
				// ECのRootPath
				Constants.PATH_ROOT_EC = csSetting.GetAppStringSetting("Site_RootPath_w2cManager");
				// PCサイトRootPath
				Constants.URL_FRONT_PC_SECURE = Constants.PROTOCOL_HTTPS + Constants.SITE_DOMAIN + csSetting.GetAppStringSetting("Site_RootPath_PCFront");
				// 実在庫利用設定有無
				Constants.REALSTOCK_OPTION_ENABLED = csSetting.GetAppBoolSetting("RealStockOption_Enabled");

				// 無効なEmailアドレスによるメール送信エラーレスポンス
				Constants.PATTERN_SMTP_ERROR_RESPONSE_FOR_INVALID_EMAIL_ADDRESS =
					csSetting.GetAppStringSetting("Pattern_Smtp_Error_Response_For_Invalid_EmailAddress");

				// エラーメール送信用SMTPサーバ設定
				// 配列順内容：Server,Port,AuthType,PopServer,PopPort,PopType,UserName,Password
				string[] strSmtpSettings = csSetting.GetAppStringSetting("Server_Smtp_Settings_For_Error").Split(',');
				Constants.SERVER_SMTP_FOR_ERROR = strSmtpSettings[0];
				Constants.SERVER_SMTP_PORT_FOR_ERROR = int.Parse(strSmtpSettings[1]);
				foreach (Enum e in Enum.GetValues(typeof(w2.Common.SmtpAuthType)))
				{
					if (e.ToString().ToUpper() == strSmtpSettings[2].ToUpper())
					{
						Constants.SERVER_SMTP_AUTH_TYPE_FOR_ERROR = (w2.Common.SmtpAuthType)e;
						break;
					}
				}
				if (Constants.SERVER_SMTP_AUTH_TYPE_FOR_ERROR == w2.Common.SmtpAuthType.PopBeforeSmtp)
				{
					Constants.SERVER_SMTP_AUTH_POP_SERVER_FOR_ERROR = strSmtpSettings[3];
					Constants.SERVER_SMTP_AUTH_POP_PORT_FOR_ERROR = strSmtpSettings[4];
					foreach (Enum e in Enum.GetValues(typeof(w2.Common.PopType)))
					{
						if (e.ToString().ToUpper() == strSmtpSettings[5].ToUpper())
						{
							Constants.SERVER_SMTP_AUTH_POP_TYPE_FOR_ERROR = (w2.Common.PopType)e;
							break;
						}
					}
					Constants.SERVER_SMTP_AUTH_USER_NAME_FOR_ERROR = strSmtpSettings[6];
					Constants.SERVER_SMTP_AUTH_PASSOWRD_FOR_ERROR = strSmtpSettings[7];
				}
				else if (Constants.SERVER_SMTP_AUTH_TYPE_FOR_ERROR == w2.Common.SmtpAuthType.SmtpAuth)
				{
					Constants.SERVER_SMTP_AUTH_USER_NAME_FOR_ERROR = strSmtpSettings[6];
					Constants.SERVER_SMTP_AUTH_PASSOWRD_FOR_ERROR = strSmtpSettings[7];
				}

				// 受注ワークフロー対象件数集計間隔
				Constants.WORKFLOW_TARGET_COUNT_AGGREGATE_INTERVAL_HOUR = csSetting.GetAppIntSetting("WorkflowTargetCountAggregateIntervalHour");
			}
			catch (Exception ex)
			{
				throw new System.ApplicationException("Config.xmlファイルの読み込みに失敗しました。\r\n" + ex.ToString());
			}
		}

		/// <summary>
		/// アプリケーション終了
		/// </summary>
		private void ExitApplication()
		{
			// アイコンをトレイから取り除く
			notifyIcon.Visible = false;

			// メッセージ書き込み
			WriteMessage();

			// スケジュール監視スレッド停止
			ScheduleObserver.AbortThread();

			// アプリケーションの終了
			Application.Exit();
		}

		/*
		/// <summary>
		/// フォーム閉じるイベント（隠すだけ）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_FormClosing(object sender, FormClosingEventArgs e)
		{
			// 終了処理のキャンセル
			e.Cancel = true;

			// フォームの非表示
			this.Visible = false;
		}*/

		/// <summary>
		/// 終了イベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ToolStripMenuItemClose_Click(object sender, EventArgs e)
		{
			ExitApplication();
		}

		/// <summary>
		/// 閉じるボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnClose_Click(object sender, EventArgs e)
		{
			if (MessageBox.Show("アプリケーションを終了します。\r\nよろしいですか？", "終了確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				ExitApplication();
			}
		}

		/// <summary>
		/// 隠すボタンクリック
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnHide_Click(object sender, EventArgs e)
		{
			// フォームの非表示
			this.Visible = false;
		}

		/// <summary>
		/// タスクトレイアイコンダブルクリック（表示）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void notifyIcon1_DoubleClick(object sender, EventArgs e)
		{
			// フォームの表示
			this.Visible = true;

			// 最小化されていたら通常サイズに戻す
			if (this.WindowState == FormWindowState.Minimized)
			{
				this.WindowState = FormWindowState.Normal;
			}

			// フォームをアクティブにする
			this.Activate();
		}

		/// <summary>
		/// 表示用タイマイベント（とりあえず1秒おきに実行）
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timerDisplayMessage_Tick(object sender, EventArgs e)
		{
			if (m_blSetDispMessage)
			{
				try
				{
					//--- 書き込みロック取得 ---//
					m_rwlDisplayBufferLock.AcquireWriterLock(Timeout.Infinite);

					//------------------------------------------------------
					// 最大表示文字数を超えていたら表示ログ削減
					//------------------------------------------------------
					if (m_sbDisplayBuffer.Length > MAX_DISP_CHARS)
					{
						m_sbDisplayBuffer.Remove(0, MAX_DISP_CHARS / 2);
					}

					//------------------------------------------------------
					// メッセージ画面へ追記
					//------------------------------------------------------
					// 画面セット
					tbMessage.Text = m_sbDisplayBuffer.ToString();

					// 最終行表示
					tbMessage.SelectionStart = tbMessage.Text.Length;
					tbMessage.ScrollToCaret();

					m_blSetDispMessage = false;
				}
				finally
				{
					//--- 書き込みロック解除 ---//
					m_rwlDisplayBufferLock.ReleaseWriterLock();
				}
			}
		}

		/// <summary>
		/// ファイル書き込み用タイマイベント
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void timerWriteMessage_Tick(object sender, EventArgs e)
		{
			WriteMessage();
		}

		/// <summary>
		/// ログファイル書き込み
		/// </summary>
		private void WriteMessage()
		{
			try
			{
				//--- 書き込みロック取得 ---//
				m_rwlWriteBufferLock.AcquireWriterLock(Timeout.Infinite);

				if (m_sbWriteBuffer.Length != 0)
				{
					//------------------------------------------------------
					// ログ書き込み
					//------------------------------------------------------
					string strLogFilePath = Constants.PHYSICALDIRPATH_LOGFILE + "history_" + DateTime.Now.ToString("yyyyMM") + "." + Constants.LOGFILE_EXTENSION;
					try
					{
						// Mutexで排他制御しながらファイル書き込み
						using (System.Threading.Mutex mtx = new System.Threading.Mutex(false, strLogFilePath.Replace("\\", "_") + ".FileWrite"))
						{
							try
							{
								mtx.WaitOne();

								using (System.IO.StreamWriter sw = new System.IO.StreamWriter(strLogFilePath, true, System.Text.Encoding.Default))
								{
									// ファイル書き込み
									sw.Write(m_sbWriteBuffer.ToString());
									sw.Close();
								}
							}
							finally
							{
								mtx.ReleaseMutex();
							}
						}
					}
					catch (Exception ex)
					{
						// 例外の場合はログ出力してやりすごす
						w2.Common.Logger.FileLogger.WriteError(ex);
					}

					//------------------------------------------------------
					// バッファ初期化
					//------------------------------------------------------
					m_sbWriteBuffer = new StringBuilder();
				}
			}
			finally
			{
				//--- 書き込みロック解除 ---//
				m_rwlWriteBufferLock.ReleaseWriterLock();
			}
		}

		/// <summary>
		/// デバッグ書き込み
		/// </summary>
		/// <param name="strMessage"></param>
		public static void WriteDebugoLogLine(string strMessage)
		{
			if (Constants.MESSAGE_DEBUG)
			{
				WriteLogLine("DEBG", strMessage);
			}
		}
		/// <summary>
		/// 情報書き込み
		/// </summary>
		/// <param name="strMessage"></param>
		public static void WriteInfoLogLine(string strMessage)
		{
			WriteLogLine("INFO", strMessage);
		}
		/// <summary>
		/// エラー書き込み
		/// </summary>
		/// <param name="strMessage"></param>
		public static void WriteErrorLogLine(string strMessage)
		{
			WriteLogLine("ERROR", strMessage);
		}
		/// <summary>
		/// 警告書き込み
		/// </summary>
		/// <param name="strMessage"></param>
		public static void WriteWarningLogLine(string strMessage)
		{
			WriteLogLine("WARN", strMessage);
		}
		/// <summary>
		/// エラー書き込み
		/// </summary>
		/// <param name="strMessage"></param>
		private static void WriteLogLine(string strHeader, string strMessage)
		{
			string strLogMessage = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + " " + strHeader + ": " + strMessage + "\r\n";

			//------------------------------------------------------
			// 表示ログバッファ書き込み
			//------------------------------------------------------
			try
			{
				//--- 書き込みロック取得 ---//
				m_rwlDisplayBufferLock.AcquireWriterLock(Timeout.Infinite);

				// ログ追加
				m_sbDisplayBuffer.Append(strLogMessage);
			}
			finally
			{
				//--- 書き込みロック解除 ---//
				m_rwlDisplayBufferLock.ReleaseWriterLock();
			}

			//------------------------------------------------------
			// ファイルログバッファ書き込み
			//------------------------------------------------------
			try
			{
				//--- 書き込みロック取得 ---//
				m_rwlWriteBufferLock.AcquireWriterLock(Timeout.Infinite);

				// ログ追加
				m_sbWriteBuffer.Append(strLogMessage);
			}
			finally
			{
				//--- 書き込みロック解除 ---//
				m_rwlWriteBufferLock.ReleaseWriterLock();
			}

			m_blSetDispMessage = true;
		}

		/// <summary>
		/// クリップボードコピー
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void btnCopyClipboad_Click(object sender, EventArgs e)
		{
			Clipboard.SetText(tbMessage.Text);

			MessageBox.Show("クリップボードにコピーしました。");
		}

		private void tbMessage_TextChanged(object sender, EventArgs e)
		{

		}

		/// <summary>
		/// システム管理者向けメールアドレスを取得
		/// </summary>
		/// <param name="csSetting">アプリケーション設定値</param>
		/// <param name="settingKey">設定キー</param>
		/// <returns>メールアドレスリスト</returns>
		private List<MailAddress> GetMailAddress(ConfigurationSetting csSetting, string settingKey)
		{
			List<MailAddress> mailAddressList = new List<MailAddress>();
			if (csSetting.GetAppStringSetting(settingKey) != null)
			{
				csSetting.GetAppStringSettingList(settingKey).ForEach(mail => mailAddressList.Add(new MailAddress(mail)));
			}

			return mailAddressList;
		}
	}
}