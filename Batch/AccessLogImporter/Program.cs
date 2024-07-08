/*
=========================================================================================================
  Module      : プログラム本体(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Common.Sql;

namespace w2.MarketingPlanner.Batch.AccessLogImporter
{
	partial class Program
	{
		// プロセス
		static Process.P01_ImportW3cLog m_p01_W3cLogImporter = new Process.P01_ImportW3cLog();
		static Process.P02_ProcessAccessLog m_p02_ProcessAccessLog = new Process.P02_ProcessAccessLog();

		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		/// <param name="args"></param>
		[STAThread]
		static int Main(string[] args)
		{
			try
			{
				Program program = new Program();

				AppLogger.WriteInfo("起動");

				program.ImportAndProcess();

				AppLogger.WriteInfo("終了");
			}
			catch (Exception ex)
			{
				// エラーログ出力
				AppLogger.WriteError(ex.ToString());

				// メール送信
				SendMail(Constants.MAIL_SUBJECTHEAD + " エラー", ex.ToString());

				return -1;
			}

			// 失敗が含まれている場合はエラーを返す
			if ((m_p01_W3cLogImporter.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.FAILED)
				|| (m_p01_W3cLogImporter.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.FAILED_A_PART)
				|| (m_p02_ProcessAccessLog.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.FAILED)
				|| (m_p02_ProcessAccessLog.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.FAILED_A_PART))
			{
				return -1;
			}

			// 正常終了を返す
			return 0;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			// 初期化処理
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
				w2.App.Common.ConfigurationSetting csSetting
					= new w2.App.Common.ConfigurationSetting(
						Properties.Settings.Default.ConfigFileDirPath,
						w2.App.Common.ConfigurationSetting.ReadKbn.C000_AppCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C100_BatchCommon,
						w2.App.Common.ConfigurationSetting.ReadKbn.C200_AccessLogImporter);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// W3Cログ取得元ディレクトリ格納
				Constants.IMPORTLOG_HEADER = csSetting.GetAppStringSetting("AccessLog_W3CLogHead");
				foreach (string strW3cLogDirectories in csSetting.GetAppStringSettingList("AccessLog_IISLog_GetDirPath"))
				{
					Constants.W3CLOG_DIRECTORIES.Add(strW3cLogDirectories);
				}

				// ログ中の読み込み対象URL
				Constants.TARGETURL_LOGIMPORT = csSetting.GetAppStringSetting("AccessLog_GetlogPath");

				// 対象ログ格納パス
				Constants.PATH_W3CLOG_IMPORT = csSetting.GetAppStringSetting("AccessLog_IISLog_StockDirPath") + @"Import\";
				Constants.PATH_W3CLOG_ACTIVE = csSetting.GetAppStringSetting("AccessLog_IISLog_StockDirPath") + @"Active\";
				Constants.PATH_W3CLOG_COMPLETE = csSetting.GetAppStringSetting("AccessLog_IISLog_StockDirPath") + @"Complete\";
				Constants.PATH_W3CLOG_ERROR = csSetting.GetAppStringSetting("AccessLog_IISLog_StockDirPath") + @"Error\";

				// ログ保持月数
				Constants.LOGSTOCK_MONTHS = csSetting.GetAppIntSetting("AccessLog_IISLog_StockMonths");

				Constants.PROCESSACCESSLOGSETTINGS = csSetting.GetAppStringSettingList("AccessLog_ProcessAccessLogSettings");
				Constants.SQLTIMEOUT_SEC = csSetting.GetAppIntSetting("AccessLog_SqlTimeOutSec");
				Constants.ACCESSLOG_TARGET_PC = csSetting.GetAppBoolSetting("AccessLog_Target_PC");

				// メール送信先設定
				Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = csSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = csSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Bcc");

				//------------------------------------------------------
				// 対象ディレクトリ作成
				//------------------------------------------------------
				if (Directory.Exists(Constants.PATH_W3CLOG_IMPORT) == false)
				{
					Directory.CreateDirectory(Constants.PATH_W3CLOG_IMPORT);
				}
				if (Directory.Exists(Constants.PATH_W3CLOG_ACTIVE) == false)
				{
					Directory.CreateDirectory(Constants.PATH_W3CLOG_ACTIVE);
				}
				if (Directory.Exists(Constants.PATH_W3CLOG_COMPLETE) == false)
				{
					Directory.CreateDirectory(Constants.PATH_W3CLOG_COMPLETE);
				}
				if (Directory.Exists(Constants.PATH_W3CLOG_ERROR) == false)
				{
					Directory.CreateDirectory(Constants.PATH_W3CLOG_ERROR);
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// ログ取り込み＆加工処理
		/// </summary>
		private void ImportAndProcess()
		{
			DataView dvAccessLogStatus = null;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();

				// アクセスログステータス取得
				dvAccessLogStatus = GetAccessLogStatus(sqlAccessor);

				//------------------------------------------------------
				// ０－１．アクセスログ初期化（前日完了していたら翌日へ）
				//------------------------------------------------------
				// 前日完了していたら翌日の初期状態へ
				if ((DateTime.Parse((string)dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_TARGET_DATE]).Date < DateTime.Now.AddDays(-1).Date)
					&& ((string)dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_DAY_STATUS] == Constants.FLG_ACCESSLOGPROCSTAT_DAY_STATUS_END))
				{
					using (SqlStatement sqlStatement = new SqlStatement("AccessLogStatus", "UpdateAccessLogTargetDateAndStatus"))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_ACCESSLOGSTATUS_TARGET_DATE, DateTime.Parse((string)dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_TARGET_DATE]).AddDays(1).ToString("yyyy/MM/dd"));
						htInput.Add(Constants.FIELD_ACCESSLOGSTATUS_DAY_STATUS, Constants.FLG_ACCESSLOGPROCSTAT_DAY_STATUS_INIT);

						sqlStatement.ExecStatement(sqlAccessor, htInput);
					}

					// アクセスログステータス取得
					dvAccessLogStatus = GetAccessLogStatus(sqlAccessor);
				}
				//------------------------------------------------------
				// ０－２．アクセスログステータス戻し(リトライ用）
				//------------------------------------------------------
				// 「取り込み中」だったら初期状態へ戻す
				else if ((string)dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_DAY_STATUS] == Constants.FLG_ACCESSLOGPROCSTAT_DAY_STATUS_IMPORT_RUN)
				{
					using (SqlStatement sqlStatement = new SqlStatement("AccessLogStatus", "UpdateAccessLogStatus"))
					{
						Hashtable htInput = new Hashtable();
						htInput.Add(Constants.FIELD_ACCESSLOGSTATUS_DAY_STATUS, Constants.FLG_ACCESSLOGPROCSTAT_DAY_STATUS_INIT);

						sqlStatement.ExecStatement(sqlAccessor, htInput);
					}

					// アクセスログステータス取得
					dvAccessLogStatus = GetAccessLogStatus(sqlAccessor);
				}

				//------------------------------------------------------
				// １．アクセスログ取り込み＆加工
				//------------------------------------------------------
				// 現在のターゲット日付取得
				DateTime dtTargetDate = DateTime.Parse((string)dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_TARGET_DATE]);

				// 過去のもののみ実行（モバイルのみの案件だとどんどん先に進んでしまうため）
				if ((dtTargetDate.Date < DateTime.Now.Date)
					&& (((string)dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_DAY_STATUS] == Constants.FLG_ACCESSLOGPROCSTAT_DAY_STATUS_INIT)
						|| ((string)dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_DAY_STATUS] == Constants.FLG_ACCESSLOGPROCSTAT_DAY_STATUS_PROC_WAIT)))
				{
					// 開始メール送信
					SendMail(Constants.MAIL_SUBJECTHEAD + " 0:[開始]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd") + " バッチ開始");

					//------------------------------------------------------
					// １－１．アクセスログ取り込み
					//------------------------------------------------------
					if (Constants.ACCESSLOG_TARGET_PC)
					{
						if ((string)dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_DAY_STATUS] == Constants.FLG_ACCESSLOGPROCSTAT_DAY_STATUS_INIT)
						{
							SendMail(Constants.MAIL_SUBJECTHEAD + " 1:[取込]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd") + " 取込開始");

							// アクセスログ取り込み
							AppLogger.WriteInfo("1.W3Cログ取込実行");
							m_p01_W3cLogImporter.ImportAll(Constants.ACCESSLOG_TARGET_PC);

							// 再度ステータス取得
							dvAccessLogStatus = GetAccessLogStatus(sqlAccessor);
						}
					}

					//------------------------------------------------------
					// １－２．アクセスログ加工
					//------------------------------------------------------
					if ((string)dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_DAY_STATUS] == Constants.FLG_ACCESSLOGPROCSTAT_DAY_STATUS_PROC_WAIT)
					{
						SendMail(Constants.MAIL_SUBJECTHEAD + " 2:[加工]", DateTime.Now.ToString("yyyy/MM/dd HH:mm:dd") + " 加工開始");

						// アクセスログ加工
						AppLogger.WriteInfo("2.ログ加工実行");
						m_p02_ProcessAccessLog.ProcessAll(Constants.ACCESSLOG_TARGET_PC);

						// 再度ステータス取得
						dvAccessLogStatus = GetAccessLogStatus(sqlAccessor);
					}

					//------------------------------------------------------
					// １－３．ログファイル圧縮
					//------------------------------------------------------
					if (Constants.ACCESSLOG_TARGET_PC)
					{
						try
						{
							m_p01_W3cLogImporter.CompressCompleteLogFiles();
						}
						catch (Exception ex)
						{
							// 警告メール送信
							SendMail(Constants.MAIL_SUBJECTHEAD + " 3:[エラー](ファイル圧縮失敗)", ex.ToString());

							AppLogger.WriteError(ex);
						}
					}
				}
			}

			//------------------------------------------------------
			// ２．メール送信
			//------------------------------------------------------
			{
				// メールタイトル作成
				string strBatchResultString = null;
				if ((m_p01_W3cLogImporter.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.FAILED)
					|| m_p02_ProcessAccessLog.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.FAILED)
				{
					strBatchResultString = Process.P00_BaseProcess.GetProcessResultString(Process.P00_BaseProcess.PROCESS_RESULT.FAILED);
				}
				else if ((m_p01_W3cLogImporter.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.ALERT)
					|| m_p02_ProcessAccessLog.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.ALERT)
				{
					strBatchResultString = Process.P00_BaseProcess.GetProcessResultString(Process.P00_BaseProcess.PROCESS_RESULT.ALERT);
				}
				else if ((m_p01_W3cLogImporter.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.SUCCESS)
					|| m_p02_ProcessAccessLog.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.SUCCESS)
				{
					strBatchResultString = Process.P00_BaseProcess.GetProcessResultString(Process.P00_BaseProcess.PROCESS_RESULT.SUCCESS);
				}
				else
				{
					strBatchResultString = Process.P00_BaseProcess.GetProcessResultString(Process.P00_BaseProcess.PROCESS_RESULT.NONE);
				}
				string strMailSubject = Constants.MAIL_SUBJECTHEAD + " 3:[" + strBatchResultString + "] " + (string)dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_TARGET_DATE] + " [" + (string)dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_DAY_STATUS] + "]";

				StringBuilder sbMailBody = new StringBuilder();

				// メール本文作成０
				if (dvAccessLogStatus.Count != 0)
				{
					sbMailBody.Append("■終了時ステータス:").Append(dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_TARGET_DATE]);
					sbMailBody.Append("「").Append(dvAccessLogStatus[0][Constants.FIELD_ACCESSLOGSTATUS_DAY_STATUS]).Append("」\r\n");
					sbMailBody.Append("\r\n");
				}

				// メール本文作成１
				if (m_p01_W3cLogImporter.ProcessResult != Process.P00_BaseProcess.PROCESS_RESULT.NONE)
				{
					sbMailBody.Append("■").Append(m_p01_W3cLogImporter.GetType().Name).Append(" ");
					sbMailBody.Append("[").Append(m_p01_W3cLogImporter.GetProcessResultString()).Append("]\r\n");

					sbMailBody.Append("-----------------------------------------------").Append("\r\n");
					sbMailBody.Append(" 成功  ：" + m_p01_W3cLogImporter.SuccessFileNames.Count.ToString() + "件").Append("\r\n");
					sbMailBody.Append(" 失敗  ：" + m_p01_W3cLogImporter.FailedFileNames.Count.ToString() + "件").Append("\r\n");
					sbMailBody.Append(" 対象外：" + m_p01_W3cLogImporter.NotImportFileNames.Count.ToString() + "件").Append("\r\n");
					sbMailBody.Append("-----------------------------------------------").Append("\r\n");
					// 成功ファイル名一覧
					if (m_p01_W3cLogImporter.SuccessFileNames.Count != 0)
					{
						sbMailBody.Append("[成功]").Append("\r\n");
						foreach (string strFiles in m_p01_W3cLogImporter.SuccessFileNames)
						{
							sbMailBody.Append(strFiles).Append("\r\n");
						}
						sbMailBody.Append("\r\n");
					}
					// 失敗ファイル名一覧
					if (m_p01_W3cLogImporter.FailedFileNames.Count != 0)
					{
						sbMailBody.Append("[失敗]").Append("\r\n");
						foreach (string strFailFailedName in m_p01_W3cLogImporter.FailedFileNames)
						{
							sbMailBody.Append(strFailFailedName).Append("\r\n");
						}
						sbMailBody.Append("\r\n");
					}
					// 対象外ファイル名一覧
					if (m_p01_W3cLogImporter.NotImportFileNames.Count != 0)
					{
						sbMailBody.Append("[対象外]").Append("\r\n");
						foreach (string strFiles in m_p01_W3cLogImporter.NotImportFileNames)
						{
							sbMailBody.Append(strFiles).Append("\r\n");
						}
						sbMailBody.Append("\r\n");
					}
				}
				// 例外が発生していれば本文追記
				if (m_p01_W3cLogImporter.ProcessException != null)
				{
					sbMailBody.Append(m_p01_W3cLogImporter.ProcessException.ToString());
					sbMailBody.Append("\r\n");
				}

				// メール本文作成２
				if (m_p02_ProcessAccessLog.ProcessResult != Process.P00_BaseProcess.PROCESS_RESULT.NONE)
				{
					sbMailBody.Append("■").Append(m_p02_ProcessAccessLog.GetType().Name).Append(" ");
					sbMailBody.Append("[").Append(m_p02_ProcessAccessLog.GetProcessResultString()).Append("]\r\n");
				}
				// 例外が発生していれば本文追記
				if (m_p02_ProcessAccessLog.ProcessException != null)
				{
					sbMailBody.Append(m_p02_ProcessAccessLog.ProcessException.ToString());
				}

				// 実行されなかった場合
				if ((m_p01_W3cLogImporter.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.NONE)
					&& (m_p02_ProcessAccessLog.ProcessResult == Process.P00_BaseProcess.PROCESS_RESULT.NONE))
				{
					sbMailBody.Append("\r\n処理は実行されませんでした。");
				}

				// メール送信
				SendMail(strMailSubject, sbMailBody.ToString());
			}
		}

		/// <summary>
		/// アクセスログステータス取得
		/// </summary>
		/// <param name="sqlAccessor"></param>
		/// <returns></returns>
		private static DataView GetAccessLogStatus(SqlAccessor sqlAccessor)
		{
			using (SqlStatement sqlStatement = new SqlStatement("AccessLogStatus", "GetAccessLogStatus"))
			{
				return sqlStatement.SelectSingleStatement(sqlAccessor);
			}
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="strSubject">件名</param>
		/// <param name="strBody">本文</param>
		public static void SendMail(string strSubject, string strBody)
		{
			using (SmtpMailSender smtpMailSender = new SmtpMailSender())
			{
				smtpMailSender.SetSubject(strSubject);
				smtpMailSender.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => smtpMailSender.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => smtpMailSender.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => smtpMailSender.AddBcc(mail.Address));
				smtpMailSender.SetBody(strBody);

				smtpMailSender.SendMail();
			}
		}
	}
}
