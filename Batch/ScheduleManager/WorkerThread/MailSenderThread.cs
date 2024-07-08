/*
=========================================================================================================
  Module      : メール送信スレッドクラス(MailSenderThread.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Xml;
using Newtonsoft.Json;
using w2.App.Common.Line.LineDirectConnect;
using w2.App.Common.Line.LineDirectMessage.MessageType;
using w2.App.Common.ShopMessage;
using w2.App.Common.SMS;
using w2.Common.Logger;
using w2.Common.Net;
using w2.Common.Net.Mail;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.GlobalSMS;
using w2.Domain.MailDist;
using w2.Domain.MailDistSentUser;
using w2.Domain.MailSendLog;
using w2.Domain.MailSendTemp;
using w2.Domain.MessagingAppContents;
using w2.Domain.User;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	partial class MailSenderThread : BaseThread
	{
		// 継承済
		// private string m_strDeptId = null;
		// private string m_strActionKbn = null;
		// private string m_strMastertId = null;
		// private int m_iActionNo = -1;

		/// <summary>キャリアID：docomo</summary>
		public const string CAREER_DOCOMO = "01dc";
		/// <summary>キャリアID：au</summary>
		public const string CAREER_AU = "02au";
		/// <summary>キャリアID：Softbank</summary>
		public const string CAREER_SOFTBANK = "03vd";
		/// <summary>Mail送信結果</summary>
		public const string SEND_MAIL_RESULT = "SendMailResult";
		/// <summary>LINE送信結果</summary>
		public const string SEND_LINE_RESULT = "SendLineResult";

		/// <summary>ロックオブジェクト</summary>
		private readonly object m_lockObject = new object();

		//=========================================================================================
		/// <summary>
		/// スレッド作成
		/// </summary>
		/// <param name="dtScheduleDate">スケジュール日付</param>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strMailDistId">メール配信ID</param>
		/// <param name="iActionNo">アクションNO</param>
		/// <param name="reAction">再アクションか？</param>
		/// <returns>生成スレッド</returns>
		//=========================================================================================
		public static MailSenderThread CreateAndStart(DateTime dtScheduleDate, string strDeptId, string strMailDistId, int iActionNo, bool reAction = false)
		{
			// スレッド作成
			var mailSenderThread = new MailSenderThread(dtScheduleDate, strDeptId, strMailDistId, iActionNo, reAction);
			mailSenderThread.Thread = new Thread(new ThreadStart(mailSenderThread.Work));

			// スレッドスタート
			mailSenderThread.Thread.Start();

			return mailSenderThread;
		}

		//=========================================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strMailDistId">メール配信ID</param>
		//=========================================================================================
		public MailSenderThread(DateTime dtScheduleDate, string strDeptId, string strMailDistId, int iActionNo, bool reAction)
			: base(dtScheduleDate, strDeptId, Constants.FLG_TASKSCHEDULE_ACTION_KBN_MAIL_DIST, strMailDistId, iActionNo, reAction)
		{
		}

		//=========================================================================================
		/// <summary>
		/// メール配信
		/// </summary>
		//=========================================================================================
		public void Work()
		{
			var lMailSendResults = new List<Dictionary<string, string>>();
			long lExtractTotal = 0;
			long lLastErrorExceptCount = 0;
			long lLastMobileMailExceptCount = 0;
			long lSendMailTotal = 0;
			var messageHead = string.Format("メール配信[{0}-{1}]配信", this.MasterId, this.ActionNo);

			try
			{
				//------------------------------------------------------
				// メール配信タスクステータス更新(開始)
				//------------------------------------------------------
				var updateStatus = UpdateTaskStatusBegin(
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT,
					string.Empty);

				//------------------------------------------------------
				// 処理開始宣言
				//------------------------------------------------------
				Form1.WriteInfoLogLine(messageHead + "開始");

				//------------------------------------------------------
				// メール配信設定取得（同時にステータス更新、取得できるまで待つ）
				//------------------------------------------------------
				LoggingDebug("01メール配信設定取得（同時にステータス更新、取得できるまで待つ）");
				var drvMailDistSetting = GetMailDistSettingAndChangeStatus(Constants.FLG_MAILDISTSETTING_STATUS_SEND);

				//------------------------------------------------------
				// メール文章取得
				//------------------------------------------------------
				LoggingDebug("02メール文章取得");
				DataRowView drvMailDistText = null;
				using (var sqlAccessor = new SqlAccessor())
				using (var sqlStatement = new SqlStatement("MailDistText", "GetMailDistText"))
				{
					var htInput = new Hashtable
					{
						{ Constants.FIELD_MAILDISTTEXT_DEPT_ID, this.DeptId },
						{ Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID, drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID] }
					};

					var dvMailDistText = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
					if (dvMailDistText.Count == 0)
					{
						// 異常終了★
					}

					drvMailDistText = dvMailDistText[0];
				}

				// グローバル用のメール設定も取得する
				var mailDistTextSettings = new MailDistTextModel[0];
				if (Constants.GLOBAL_OPTION_ENABLE)
				{
					mailDistTextSettings = new MailDistService().GetMailDistTextContainGlobalSetting(
						this.DeptId,
						(string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID]);
				}

				//------------------------------------------------------
				// メールクリック情報取得
				//------------------------------------------------------
				LoggingDebug("03メールクリック情報取得");
				DataView dvMailClick = null;
				using (var sqlAccessor = new SqlAccessor())
				using (var sqlStatement = new SqlStatement("MailClick", "GetMailClick"))
				{
					var htInput = new Hashtable
					{
						{ Constants.FIELD_MAILCLICK_DEPT_ID, this.DeptId },
						{ Constants.FIELD_MAILCLICK_MAILTEXT_ID, drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID] },
						{ Constants.FIELD_MAILCLICK_MAILDIST_ID, string.Empty },
						{ Constants.FIELD_MAILCLICK_ACTION_NO, 0 }
					};

					dvMailClick = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
				}

				//------------------------------------------------------
				// 抽出/利用ターゲットリスト取得（カンマ区切り）
				//------------------------------------------------------
				// 一応空のターゲットリストIDもＯＫとする（どうせ引っかからない）
				LoggingDebug("04抽出/利用ターゲットリスト取得（カンマ区切り）");
				var sbTargetIdsExtract = new StringBuilder();
				var sbTargetIdsUse = new StringBuilder();
				for (var iLoop = 1; iLoop <= 5; iLoop++)
				{
					var strTargetIdTmp = (string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_TARGET_ID + ((iLoop == 1) ? "" : iLoop.ToString())];
					var strTargetExtractFlgTmp = (string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_TARGET_EXTRACT_FLG + ((iLoop == 1) ? "" : iLoop.ToString())];

					if (strTargetIdTmp.Length != 0)
					{
						// メール配信設定の5つのターゲット中、1つでもエラーになったら全て送らない
						if (IsTargetListStatusNormal(strTargetIdTmp) == false)
						{
							LoggingDebug("04抽出/利用ターゲットリストのステータスがエラーになりました。");
							throw new Exception(
								string.Format(
									"ターゲットリストID:{0}のステータスが「通常」ではないため、メール配信を中止しました。",
									strTargetIdTmp));
						}

						sbTargetIdsUse.Append((sbTargetIdsUse.Length != 0) ? "," : "");
						sbTargetIdsUse.Append("'").Append(strTargetIdTmp).Append("'");

						if (strTargetExtractFlgTmp == Constants.FLG_MAILDISTSETTING_TARGET_EXTRACT_FLG_ON)
						{
							sbTargetIdsExtract.Append((sbTargetIdsExtract.Length != 0) ? "," : "");
							sbTargetIdsExtract.Append("'").Append(strTargetIdTmp).Append("'");
						}
					}
				}

				// ターゲット抽出ステータス更新（「実行中」へ。抽出対象なしの場合は「完了」へ）
				updateStatus = UpdatePrepareTaskStatus(
					(sbTargetIdsExtract.Length == 0) ? Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE : Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT);
				LoggingDebug("05ターゲット抽出ステータス更新（「実行中」へ。抽出対象なしの場合は「完了」へ）:" + updateStatus);

				//------------------------------------------------------
				// 抽出対象ありの場合は抽出（自動消滅スレッドそれぞれ作成）
				//------------------------------------------------------
				LoggingDebug("06抽出対象ありの場合は抽出（自動消滅スレッドそれぞれ作成）");
				if (sbTargetIdsExtract.Length != 0)
				{
					// ターゲットリスト一覧取得
					DataView dvExtractTargetList = null;
					using (var sqlAccessor = new SqlAccessor())
					using (var sqlStatement = new SqlStatement("TargetList", "GetTargetListForAction"))
					{
						sqlStatement.Statement = sqlStatement.Statement.Replace("@@ params @@", sbTargetIdsExtract.ToString());

						var htInput = new Hashtable();
						htInput.Add(Constants.FIELD_MAILDISTSETTING_DEPT_ID, this.DeptId);
						dvExtractTargetList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
					}

					// ターゲットリスト抽出（完了する間で待つ）
					foreach (DataRowView drv in dvExtractTargetList)
					{
						var extractThread = TargetExtractThread.CreateAndStart((string)drv[Constants.FIELD_TARGETLIST_DEPT_ID], (string)drv[Constants.FIELD_TARGETLIST_TARGET_ID]);
						while (extractThread.Thread.IsAlive)
						{
							Thread.Sleep(100);
						}
					}
					
					// ターゲット抽出ステータス更新（抽出対象なしの場合は完了ステータスへ）
					updateStatus = UpdatePrepareTaskStatus(
						Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_DONE,
						Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_EXECUTE);

					// タスクステータス更新（例外エラーにより実行エラーになっている為、実行中へ）
					UpdateTaskStatusEnd(
						Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_EXECUTE,
						Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_ERROR,
						string.Format("{0}/{1}", lSendMailTotal, lExtractTotal));
				}

				//------------------------------------------------------
				// 利用ターゲット決定（複数ターゲットを１つに統合）  ※ひとつでも重複排除のためSQL実行する
				//------------------------------------------------------
				LoggingDebug("07利用ターゲット決定（複数ターゲットを１つに統合）");
				string strTargetKbn = null;
				string strTargetId = null;
				using (var sqlAccessor = new SqlAccessor())
				using (var sqlStatement = new SqlStatement("TargetList", "IntegrationTargetLists"))
				{
					sqlStatement.CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT;
					sqlStatement.Statement = sqlStatement.Statement.Replace("@@ params @@", sbTargetIdsUse.ToString());

					var htInput = new Hashtable
					{
						{ Constants.FIELD_TARGETLISTDATA_DEPT_ID, this.DeptId },
						{ Constants.FIELD_TARGETLISTDATA_MASTER_ID, this.MasterId }
					};
					sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
				}

				strTargetKbn = Constants.FLG_TARGETLISTDATA_TARGET_KBN_MAILDIST;
				strTargetId = this.MasterId;

				//------------------------------------------------------
				// モバイルメール排除用条件作成
				//------------------------------------------------------
				LoggingDebug("08モバイルメール排除用条件作成:");
				var sbWhereExceptMobile = new StringBuilder();
				if ((string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG] == Constants.FLG_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG_ON)
				{
					var xdMobileDomaians = new XmlDocument();
					xdMobileDomaians.Load(AppDomain.CurrentDomain.BaseDirectory + @"Xml\Setting\MobileDomainSetting.xml");
					foreach (XmlNode xnDomain in xdMobileDomaians.SelectNodes("MobileDomainSetting/Domain"))
					{
						sbWhereExceptMobile.Append((sbWhereExceptMobile.Length != 0) ? " AND " : "");
						sbWhereExceptMobile.Append(
							string.Format(" {0}.{1} NOT LIKE '%' + ", Constants.TABLE_MAILSENDTEMP, Constants.FIELD_MAILSENDTEMP_MAIL_ADDR));
						sbWhereExceptMobile.Append("'").Append(StringUtility.SqlLikeStringSharpEscape(xnDomain.InnerText)).Append("'");
						sbWhereExceptMobile.Append(" ESCAPE '#' \n");
					}
				}
				else
				{
					sbWhereExceptMobile.Append("1 = 1");
				}

				DataView dvTargetListData = null;
				using (var sqlAccessor = new SqlAccessor())
				{
					sqlAccessor.OpenConnection();

					//------------------------------------------------------
					// 抽出済ターゲットデータ取得
					//------------------------------------------------------
					LoggingDebug("09抽出済ターゲットデータ取得:開始");
					using (var sqlStatement = new SqlStatement("TargetList", "GetTargetListData"))
					{
						sqlStatement.CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT;
						sqlStatement.Statement = sqlStatement.Statement.Replace("@@ where_except_mobile @@", sbWhereExceptMobile.ToString());

						var htInput = new Hashtable
						{
							{ Constants.FIELD_TARGETLISTDATA_DEPT_ID, this.DeptId },
							{ Constants.FIELD_TARGETLISTDATA_MASTER_ID, strTargetId },
							{ Constants.FIELD_MAILDISTEXCEPTLIST_MAILDIST_ID, this.MasterId },
							{ Constants.FIELD_MAILDISTSETTING_ENABLE_DEDUPLICATION, drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_ENABLE_DEDUPLICATION] }
						};

						dvTargetListData = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
					}
					lExtractTotal = dvTargetListData.Count;
					LoggingDebug("09抽出済ターゲットデータ取得:終了 " + lExtractTotal);

					//------------------------------------------------------
					// メールエラーポイント除外数取得
					//------------------------------------------------------
					LoggingDebug("10メールエラーポイント除外数取得:開始");
					var existsFlg = "";
					using (var statement = new SqlStatement("TargetList", "GetExceptListCount"))
					{
						var input = new Hashtable
						{
							{ Constants.FIELD_MAILDISTEXCEPTLIST_MAILDIST_ID, this.MasterId }
						};
						existsFlg = (((int)statement.SelectSingleStatement(sqlAccessor, input)[0][0] > 0) ? "1" : "");
					}

					DataView dvMailErrorPointExceptCount = null;
					using (var sqlStatement = new SqlStatement("TargetList", "GetMailErrorPointExceptCount"))
					{
						sqlStatement.CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT;

						var htInput = new Hashtable
						{
							{ Constants.FIELD_TARGETLISTDATA_DEPT_ID, this.DeptId },
							{ Constants.FIELD_TARGETLISTDATA_MASTER_ID, strTargetId },
							{ Constants.FIELD_MAILDISTEXCEPTLIST_MAILDIST_ID, this.MasterId },
							{ "exists_flg", existsFlg }
						};
						dvMailErrorPointExceptCount = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
					}
					lLastErrorExceptCount = (int)dvMailErrorPointExceptCount[0][0];
					LoggingDebug("10メールエラーポイント除外数取得:終了 " + lLastErrorExceptCount);

					// 配信済除外数取得
					LoggingDebug("11送信済除外件数取得:開始");
					var lastDuplicateExceptCount
						= ((string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_ENABLE_DEDUPLICATION] == Constants.FLG_MAILDISTSETTING_ENABLE_DEDUPLICATION_ENABLED)
							? new MailDistSentUserService().GetDuplicateExceptCount(
								this.DeptId,
								strTargetId,
								(string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID],
								sbWhereExceptMobile.ToString())
							: 0;
					LoggingDebug("11送信済除外件数取得:終了 " + lastDuplicateExceptCount);

					//------------------------------------------------------
					// モバイル除外数取得
					//------------------------------------------------------
					LoggingDebug("12モバイル除外数取得:開始");
					if ((string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG] == Constants.FLG_MAILDISTSETTING_EXCEPT_MOBILEMAIL_FLG_ON)
					{
						DataView dvMobileMailExceptCount = null;
						using (var sqlStatement = new SqlStatement("TargetList", "GetMobileMailExceptCount"))
						{
							sqlStatement.Statement = sqlStatement.Statement.Replace("@@ where_not_except_mobile @@", " NOT(" + sbWhereExceptMobile.ToString() + ")");

							var htInput = new Hashtable
							{
								{ Constants.FIELD_TARGETLISTDATA_DEPT_ID, this.DeptId },
								{ Constants.FIELD_TARGETLISTDATA_MASTER_ID, strTargetId },
								{ Constants.FIELD_MAILDISTEXCEPTLIST_MAILDIST_ID, this.MasterId },
								{ "exists_flg", existsFlg }
							};

							dvMobileMailExceptCount = sqlStatement.SelectSingleStatement(sqlAccessor, htInput);
						}
						lLastMobileMailExceptCount = (int)dvMobileMailExceptCount[0][0];
					}
					else
					{
						lLastMobileMailExceptCount = 0;
					}
					LoggingDebug("12モバイル除外数取得:終了" + lLastMobileMailExceptCount);

					//------------------------------------------------------
					// メール配信設定の抽出ターゲット数・エラーポイント除外数など更新
					//------------------------------------------------------
					LoggingDebug("13メール配信設定の抽出ターゲット数・エラーポイント除外数など更新:開始");
					using (var sqlStatement = new SqlStatement("MailDistSetting", "UpdateMailDistSettingLastCount"))
					{
						var htInput = new Hashtable
						{
							{ Constants.FIELD_MAILDISTSETTING_DEPT_ID, this.DeptId },
							{ Constants.FIELD_MAILDISTSETTING_MAILDIST_ID, this.MasterId },
							{ Constants.FIELD_MAILDISTSETTING_LAST_COUNT, lExtractTotal },
							{ Constants.FIELD_MAILDISTSETTING_LAST_ERROREXCEPT_COUNT, lLastErrorExceptCount },
							{ Constants.FIELD_MAILDISTSETTING_LAST_MOBILEMAILEXCEPT_COUNT, lLastMobileMailExceptCount },
							{ Constants.FIELD_MAILDISTSETTING_LAST_DUPLICATE_EXCEPT_COUNT, lastDuplicateExceptCount }
						};

						sqlStatement.ExecStatement(sqlAccessor, htInput);
					}
					LoggingDebug("13メール配信設定の抽出ターゲット数・エラーポイント除外数など更新:終了");

					// メールクリック情報格納
					LoggingDebug("14メールクリック情報格納:開始");
					InsertMailInfometion((string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID], dvMailClick);
				}

				//------------------------------------------------------
				// メール配信
				//------------------------------------------------------
				var smsMailSender = new SmtpMailSender();
				smsMailSender.SetFrom((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAIL_FROM], (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAIL_FROM_NAME]);
				smsMailSender.AddCC((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAIL_CC]);
				smsMailSender.AddBcc((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAIL_BCC]);
				if (Constants.ERROR_MAILADDRESS != string.Empty)
				{
					smsMailSender.SetReturnPath(Constants.ERROR_MAILADDRESS);
				}

				// メッセージ変換用
				var strSubjectTmp = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT];
				var strSubjectMobileTmp = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_SUBJECT_MOBILE];
				var strBodyTmp = ((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY]).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
				var strBodyHtmlTmp = ((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML]).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
				var strBodyMobileTmp = ((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE]).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
				var strBodyDecomeTmp = ((string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_DECOME]).Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
				var mcTagSubjectTmp = Regex.Matches(strSubjectTmp, "<@@user:((?!@@>).)*@@>");
				var mcTagSubjectMobileTmp = Regex.Matches(strSubjectMobileTmp, "<@@user:((?!@@>).)*@@>");
				var mcTagBodyTmp = Regex.Matches(strBodyTmp, "<@@user:((?!@@>).)*@@>");
				var mcTagBodyHtmlTmp = Regex.Matches(strBodyHtmlTmp, "<@@user:((?!@@>).)*@@>");
				var mcTagBodyMobileTmp = Regex.Matches(strBodyMobileTmp, "<@@user:((?!@@>).)*@@>");
				var mcTagBodyDecomeTmp = Regex.Matches(strBodyDecomeTmp, "<@@user:((?!@@>).)*@@>");

				var exceptUserId = GetSendUserIdList(); // 再アクション用にログファイルから送信済みユーザIDを取得

				var strTaskScheduleStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE;	// デフォルトは完了へ

				// SMS送信制御
				var useSmsFlg = false;
				var smsDistTexs = new GlobalSMSDistTextModel[] { };
				if (Constants.GLOBAL_SMS_OPTION_ENABLED)
				{
					useSmsFlg = (StringUtility.ToEmpty(drvMailDistText[Constants.FIELD_MAILDISTTEXT_SMS_USE_FLG]) == MailDistTextModel.SMS_USE_FLG_ON);
				}

				if (useSmsFlg)
				{
					var sv = new GlobalSMSService();
					// ループ内で都度SQLたたくのは無駄なため、ここで全キャリア分とってくる
					smsDistTexs = sv.GetSmsDistTexts(this.DeptId, StringUtility.ToEmpty(drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID]));
				}

				var useLineDrect = ((App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED)
					&& (StringUtility.ToEmpty(drvMailDistText[Constants.FIELD_MAILDISTTEXT_LINE_USE_FLG])
						== MailDistTextModel.LINE_USE_FLG_ON));
				var lineDistContents = new string[]{};
				if (useLineDrect)
				{
					var sv = new MessagingAppContentsService();
					var msgAppContents = sv.GetAllContentsEachMessagingAppKbn(
						this.DeptId,
						MessagingAppContentsModel.MASTER_KBN_MAILDISTTEXT,
						StringUtility.ToEmpty(drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID]),
						MessagingAppContentsModel.MESSAGING_APP_KBN_LINE);
					lineDistContents = msgAppContents.Select(model => model.Contents).ToArray();
				}

				SendDebugMail(string.Format("{0}開始：{1}件", messageHead, lExtractTotal.ToString()));
				try // 停止例外捕捉用try
				{
					//------------------------------------------------------
					// 送信ループ開始
					//------------------------------------------------------
					LoggingDebug("15送信ループ開始");
					string strMailSendResult = null;
					var strLineSendResult = string.Empty;
					var hasSmtpError = false;
					foreach (DataRowView targetData in dvTargetListData)
					{
						var addressKbn = (string)targetData[Constants.FIELD_TARGETLISTDATA_MAIL_ADDR_KBN];

						smsMailSender.ClearMailSendException();

						var userDispLanguageCode = string.Empty;
						var userDispLanguageLocaleId = string.Empty;

						if (Constants.GLOBAL_OPTION_ENABLE)
						{
							userDispLanguageCode = (string)targetData[Constants.FIELD_USER_DISP_LANGUAGE_CODE];
							userDispLanguageLocaleId = (string)targetData[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID];

							// 該当の言語コードの設定が存在するか
							var mailDistTextSetting = mailDistTextSettings.FirstOrDefault(
								setting => (setting.LanguageCode == userDispLanguageCode)
									&& (setting.LanguageLocaleId == userDispLanguageLocaleId));

							if (mailDistTextSetting != null)
							{
								SetMailDistTextGlobalSetting(
									mailDistTextSetting,
									ref smsMailSender,
									ref strSubjectTmp,
									ref strBodyTmp,
									ref strBodyHtmlTmp,
									ref mcTagSubjectTmp,
									ref mcTagBodyTmp,
									ref mcTagBodyHtmlTmp);
							}
						}

						if (((addressKbn == Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_PC) && (strBodyTmp.Length + strBodyHtmlTmp.Length == 0))
							|| ((addressKbn == Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_MOBILE) && (strBodyMobileTmp.Length == 0)))
						{
							// 文章無しエラー
							strMailSendResult = Constants.FLG_TASKSCHEDULEHISTORY_ACTION_RESULT_MAILSEND_NOBODY;
							Form1.WriteWarningLogLine("メール配信文章に本文がないため送信スキップ");
							strLineSendResult = "メールの送信が実行できなかったためLINE送信をスキップしました";
						}
						else
						{
							// スケジュール停止フラグチェック＆進捗更新（とりあえず10件ずつチェックする）
							if ((lMailSendResults.Count % 10) == 0)
							{
								CheckScheduleStoppedAndUpdateProgress(lSendMailTotal.ToString() + "/" + lExtractTotal.ToString());
								// HACK:reActionの場合 分子が除外数字分で整数１にならない
							}

							// 後々のOneToOneを考えると一件ずつ配信した方がよい

							var sbSubject = new StringBuilder();
							string body = null;
							string bodyHtml = null;
							MatchCollection mcTagSubject = null;
							MatchCollection mcTagBody = null;
							MatchCollection mcTagBodyHtml = null;
							// メールアドレスかどうかチェック
							var strMailAddr = (string)targetData[Constants.FIELD_TARGETLISTDATA_MAIL_ADDR];
							if ((strMailAddr.Length != 0) && (Validator.IsMailAddress(strMailAddr) == false))
							{
								Form1.WriteWarningLogLine("送信先メールアドレスは不正です。処理をスキップします。メールアドレス（" + strMailAddr + "）はメールアドレスパターンにマッチしません。");
								FileLogger.WriteError(string.Format("ERROR: メールアドレス（{0}）はメールアドレスパターンにマッチしません。\r\n", strMailAddr));
								RegisterInvalidMailAddress(targetData, 999);
								continue;
							}
							else
							{
								smsMailSender.ClearTo();
								smsMailSender.AddTo(strMailAddr);
							}
							switch (addressKbn)
							{
								case Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_PC:

									sbSubject.Append(strSubjectTmp);
									body = strBodyTmp;
									bodyHtml = strBodyHtmlTmp;
									mcTagSubject = mcTagSubjectTmp;
									mcTagBody = mcTagBodyTmp;
									mcTagBodyHtml = mcTagBodyHtmlTmp;
									break;

								case Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_MOBILE:

									sbSubject.Append(strSubjectMobileTmp);
									body = strBodyMobileTmp;
									bodyHtml = strBodyDecomeTmp;
									mcTagSubject = mcTagSubjectMobileTmp;
									mcTagBody = mcTagBodyMobileTmp;
									mcTagBodyHtml = mcTagBodyDecomeTmp;
									break;
							}

							// 送信履歴ログにユーザID含む場合は除外
							if (exceptUserId.Contains((string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID]))
							{
								// 除外ユーザIDをログ出力
								LoggingExcludedUserId(targetData);
								lExtractTotal--;

								// 本来のsleepを飛ばすためここで処理
								Thread.Sleep(5);
								continue;
							}

							var sbBody = new StringBuilder();
							var sbBodyHtml = new StringBuilder();

							//------------------------------------------------------
							// メールクリック変換
							//------------------------------------------------------
							var isPc = (addressKbn == Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_PC);
							if (body.Length != 0)
							{
								sbBody.Append(
									ConvertMailClickUrl(
										body,
										dvMailClick,
										(string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID],
										false,
										isPc));
							}
							if (bodyHtml.Length != 0)
							{
								sbBodyHtml.Append(
									ConvertMailClickUrl(
										bodyHtml,
										dvMailClick,
										(string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID],
										true,
										isPc));
							}

							// メール文章タグ置換情報
							var mailTextReplaceTags = new Dictionary<string, string>();

							// サイト基本情報設定置換
							sbSubject = ShopMessageUtil.ConvertShopMessage(sbSubject, userDispLanguageCode, userDispLanguageLocaleId, false);
							sbBody = ShopMessageUtil.ConvertShopMessage(sbBody, userDispLanguageCode, userDispLanguageLocaleId, false);
							sbBodyHtml = ShopMessageUtil.ConvertShopMessage(sbBodyHtml, userDispLanguageCode, userDispLanguageLocaleId, true);
							lineDistContents = lineDistContents
								.Select(text => ShopMessageUtil.ConvertShopMessage(
									text,
									userDispLanguageCode,
									userDispLanguageLocaleId,
									false)).ToArray();
							
							var user = new UserService().GetUserForMailSend((string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID]);

							if (mcTagSubject.Count + mcTagBody.Count + mcTagBodyHtml.Count > 0)
							{
								// ユーザ情報変換
								SetReplaceTags(mailTextReplaceTags, user, sbSubject, mcTagSubject);
								SetReplaceTags(mailTextReplaceTags, user, sbBody, mcTagBody);
								SetReplaceTags(mailTextReplaceTags, user, sbBodyHtml, mcTagBodyHtml);
							}

							// メール登録解約リンク変換
							SetReplaceMailUnsubscribeTags(user, sbSubject);
							SetReplaceMailUnsubscribeTags(user, sbBody);
							SetReplaceMailUnsubscribeTags(user, sbBodyHtml);

							var compressedReplaceTags = string.Empty;

							if (mailTextReplaceTags.Count > 0)
							{
								// タグ置換情報をJSON型に変換
								var replaceTags = JsonConvert.SerializeObject(mailTextReplaceTags);
								// タグ置換情報を圧縮する
								compressedReplaceTags = StringUtility.CompressString(replaceTags);
							}

							//------------------------------------------------------
							// 端末別設定
							//------------------------------------------------------
							if (addressKbn == Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_MOBILE)
							{
								// モバイル用にエンコーディングリセット
								smsMailSender.SetEncoding(Constants.MOBILE_MAIL_ENCODING, Constants.MOBILE_MAIL_TRANSFER_ENCODING);
								// モバイル用文字コード、絵文字、デコメ画像変換
								DecomeConvert(sbSubject, sbBody, sbBodyHtml, targetData, smsMailSender);
							}
							else
							{
								// PC用にリセット
								smsMailSender.SetEncoding(Constants.PC_MAIL_DEFAULT_ENCODING, Constants.PC_MAIL_DEFAULT_TRANSFER_ENCODING);
								smsMailSender.Message.MultipartRelatedEnable = false;
							}

							//------------------------------------------------------
							// メールセンダーにセット
							//------------------------------------------------------
							smsMailSender.SetSubject(sbSubject.ToString());
							smsMailSender.SetBody(sbBody.ToString());
							smsMailSender.SetBodyHtml(sbBodyHtml.ToString());

							// SMS送信
							if (useSmsFlg)
							{
								var userId = StringUtility.ToEmpty(targetData[Constants.FIELD_TARGETLISTDATA_USER_ID]);
								var carrier = SMSHelper.GetSMSPhoneCarrier(userId);
								// キャリアをもとに本文特定
								var smsTextTmp = smsDistTexs.First(x => x.PhoneCarrier == carrier).SmsText;

								// 特定した本文の各種タグ置換
								var smsTextMatch = Regex.Matches(smsTextTmp, "<@@user:((?!@@>).)*@@>");
								var smsText = smsTextTmp;

								// SMS置換対象がある場合かつ、user取得していない場合はここで取得
								if (smsTextMatch.Count > 0 && user == null)
								{
									user = new UserService().GetUserForMailSend((string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID]);
								}

								// マッチした分だけ置換
								foreach (Match mt in smsTextMatch)
								{
									var strKey = mt.Value.Replace("<@@user:", "").Replace("@@>", "");
									var strValue = (user != null) ? StringUtility.ToEmpty(ConvertMailDistTextUserInfo(user, strKey)) : "";
									smsText = smsText.Replace(mt.Value, strValue);
								}

								// ヘルパ使ってSMS送信
								SMSHelper.SendSms(userId, smsText);
							}

							//------------------------------------------------------
							// メール送信
							//------------------------------------------------------
							if (smsMailSender.SendMail((string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID], (string)targetData[Constants.FIELD_USER_MAIL_ADDR]))
							{
								strMailSendResult = Constants.FLG_TASKSCHEDULEHISTORY_ACTION_RESULT_MAILSEND_OK;
								lSendMailTotal++;

								// 送信済ユーザーとして登録
								if ((string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_ENABLE_DEDUPLICATION] == Constants.FLG_MAILDISTSETTING_ENABLE_DEDUPLICATION_ENABLED)
								{
									new MailDistSentUserService().Insert(
										new MailDistSentUserModel
										{
											MaildistId = (string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID],
											UserId = (string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID],
											LastChanged = Constants.FLG_LASTCHANGED_BATCH
										});
								}
							}
							else
							{
								strMailSendResult = Constants.FLG_TASKSCHEDULEHISTORY_ACTION_RESULT_MAILSEND_NG;

								// 送信アドレスが無効だった場合、エラーアドレス登録＆ポイント追加
								if (ContainsSmtpErrorResponseForInvalidEmailAddress(smsMailSender.MailSendException.ToString())
									&& smsMailSender.MailSendException.ToString().Contains("RCPT TO:<" + (string)targetData[Constants.FIELD_TARGETLISTDATA_MAIL_ADDR] + ">"))
								{
									RegisterInvalidMailAddress(targetData, 1);
								}
								// SMTP接続が行えない場合
								else if ((smsMailSender.MailSendException is NetworkIOException)
									|| (smsMailSender.MailSendException is SocketException))
								{
									FileLogger.WriteError(
										string.Format(
										"・SMTP接続エラー メール配信設定ID：{0}  ユーザーID：", 
										drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID],
										targetData[Constants.FIELD_TARGETLISTDATA_USER_ID]));

									hasSmtpError = true;
									Form1.WriteInfoLogLine(smsMailSender.MailSendException.ToString());
								}
								else
								{
									Form1.WriteInfoLogLine(smsMailSender.MailSendException.ToString());
								}
							}

							// LINE送信
							if (useLineDrect)
							{
								var lineTextMatchArray = lineDistContents.Select(text => Regex.Matches(text, "<@@user:((?!@@>).)*@@>")).ToArray();
								var lineMessagesTemp = lineDistContents.Select(text => new LineMessageText { MessageText = text }).ToArray();
								var isMatch = lineTextMatchArray.Any(match => (match.Count > 0));
								if (isMatch)
								{
									if (user == null)
									{
										user = new UserService().GetUserForMailSend((string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID]);
									}

									for (var i = 0; i < lineTextMatchArray.Length; i++)
									{
										foreach (Match mt in lineTextMatchArray[i])
										{
											var strKey = mt.Value.Replace("<@@user:", string.Empty).Replace("@@>", string.Empty);
											var strValue = (user != null) ? StringUtility.ToEmpty(ConvertMailDistTextUserInfo(user, strKey)) : string.Empty;
											lineMessagesTemp[i].MessageText = lineMessagesTemp[i].MessageText.Replace(mt.Value, strValue);
										}
									}
								}

								var userId = (string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID];
								var extender = new UserService().GetUserExtend(userId);
								var lineId = extender.UserExtendDataValue[Constants.SOCIAL_PROVIDER_ID_LINE];
								if (string.IsNullOrEmpty(lineId) == false)
								{
									// LINE送信実行
									var response = new LineDirectConnectManager().SendPushMessage(lineId, lineMessagesTemp, userId);
									strLineSendResult = (response == null)
										? "成功"
										: string.Format(
											"【{0}】{1}",
											response.Message,
											((response.Details.Length > 0) && (string.IsNullOrEmpty(response.Details[0].Message) == false))
												? string.Format(
													"{0}　- {1}",
													Environment.NewLine,
													string.Join(Environment.NewLine, response.Details.Select(x => x.Message).ToArray()))
												: string.Empty);
								}
							}

							// CSオプションが有効の場合はメール送信ログ登録
							if (Constants.CS_OPTION_ENABLED)
							{
								// 非同期実行時に送信日時を取得したくないのでここに記述
								var dateSendMail = DateTime.Now;

								// 配信時のメール配信文章を登録し、登録時に履歴NOを取得する
								var textHisoryNo = 0;
								textHisoryNo = InsertAndGetTextHistoryNo(drvMailDistText, (string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME], compressedReplaceTags);

								var service = new MailSendLogService();
								var model = new MailSendLogModel
								{
									UserId = (string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID],
									DeptId = (string)drvMailDistText[Constants.FIELD_MAILDISTSETTING_DEPT_ID],
									MailtextId = (string)drvMailDistText[Constants.FIELD_MAILDISTSETTING_MAILTEXT_ID],
									MailtextName = (string)drvMailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_NAME],
									MaildistId = (string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID],
									MaildistName = (string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILDIST_NAME],
									ShopId = string.Empty,
									MailId = string.Empty,
									MailName = string.Empty,
									MailFromName = smsMailSender.Message.From.DisplayName,
									MailFrom = smsMailSender.Message.From.Address,
									MailTo = string.Join(",", smsMailSender.Message.To.Select(m => m.Address)),
									MailCc = string.Join(",", smsMailSender.Message.CC.Select(m => m.Address)),
									MailBcc = string.Join(",", smsMailSender.Message.Bcc.Select(m => m.Address)),
									MailSubject = smsMailSender.Subject,
									MailBody = string.Empty,	// メール本文は「メール配信時文章履歴」テーブルに保存するため、空を設定
									MailBodyHtml = string.Empty,	// メール本文は「メール配信時文章履歴」テーブルに保存するため、空を設定
									ErrorMessage = (smsMailSender.MailSendException != null) ? smsMailSender.MailSendException.ToString() : string.Empty,
									DateSendMail = dateSendMail,
									ReadFlg = Constants.FLG_MAILSENDLOG_READ_FLG_UNREAD,
									DateReadMail = (DateTime?)null,
									TextHistoryNo = textHisoryNo,
									MailAddrKbn = addressKbn
								};
								service.Insert(model);
							}

							//------------------------------------------------------
							// デコメ用添付ファイル削除
							//------------------------------------------------------
							smsMailSender.DecomeAttachmentFile.Clear();
						}

						// 送信結果格納
						var result = new Dictionary<string, string>
						{
							{ SEND_MAIL_RESULT, strMailSendResult },
						};
						if (useLineDrect && (string.IsNullOrEmpty(strLineSendResult) == false))
						{
							result.Add(SEND_LINE_RESULT, strLineSendResult);
						}
						lMailSendResults.Add(result);

						// メール配信結果を随時ログ出力
						SendLoggingResult(targetData, strMailSendResult);

						// 少し休む
						Thread.Sleep(5);
					}

					if (hasSmtpError)
					{
						SendAdministratorSmtpErrorMail((string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_MAILDIST_ID]);
					}

					SendLoggingResult("EOF\r\n");
				}
				catch (ScheduleStopException)
				{
					Form1.WriteInfoLogLine("■メール配信停止要求あり■");

					strTaskScheduleStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP;	// 「停止中」へ
					SendLoggingResult("STOP_EOF\r\n");
				}
				finally
				{
					var messages = new StringBuilder();
					messages.Append(messageHead);

					switch (strTaskScheduleStatus)
					{
						//case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT:
						//case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE:
						//case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_ERROR:
						case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP:
							messages.Append("停止：");
							break;

						case Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE:
							messages.Append("完了：");
							break;
					}

					messages.Append(lSendMailTotal.ToString() + "/" + lExtractTotal.ToString() + "件");
					SendDebugMail(messages.ToString());
					LoggingDebug("15送信ループ:終了");
				}

				//------------------------------------------------------
				// ターゲットリストステータス更新（通常状態へ）
				//------------------------------------------------------
				if (strTargetKbn == Constants.FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST)
				{
					UpdateTargetListStatus(strTargetId, Constants.FLG_TARGETLIST_STATUS_NORMAL);
				}

				//------------------------------------------------------
				// メール配信設定ステータス更新（通常状態へ）
				//------------------------------------------------------
				UpdateMailDistSettingStatus(Constants.FLG_MAILDISTSETTING_STATUS_NORMAL);

				// メール配信用 テンポラリデータの削除
				new MailSendTempService().DeleteAll(this.DeptId, this.MasterId);

				//------------------------------------------------------
				// 処理終了宣言
				//------------------------------------------------------
				Form1.WriteInfoLogLine(messageHead + "完了：" + lSendMailTotal.ToString() + "/" + lExtractTotal.ToString() + "件");

				//------------------------------------------------------
				// メール配信タスクステータス更新（停止 or 完了）
				//------------------------------------------------------
				var lineSendResultFinaly = string.Empty;
				if (useLineDrect)
				{
					var lineSendResultAll = lMailSendResults.Where(result => result.ContainsKey(SEND_LINE_RESULT))
						.Select(result => result[SEND_LINE_RESULT]).ToArray();
					var lineSendResultCounter = new Dictionary<string, int>
					{
						{ "成功", 0 }
					};

					if (lineSendResultAll.Length > 0)
					{
						foreach (var result in lineSendResultAll)
						{
							// 「成功/ エラー等」毎に回数を計測
							if (lineSendResultCounter.ContainsKey(result))
							{
								lineSendResultCounter[result]++;
							}
							else
							{
								lineSendResultCounter.Add(result, 1);
							}
						}
					}

					// 成功件数取得
					lineSendResultFinaly = string.Format(
						"> {0}件：成功{1}",
						lineSendResultCounter["成功"],
						Environment.NewLine);
					// エラー文言・件数取得
					var lineResultError = lineSendResultCounter.Where(result => (result.Key.Equals("成功") == false))
						.ToArray();
					var errorAllCount = lineResultError.Select(error => error.Value).Sum();
					lineSendResultFinaly += string.Format(
						"> {0}件：エラー {1}",
						errorAllCount,
						string.Join(
							"",
							lineResultError.Select(
									error => string.Format("{0}・{1}件：{2}", Environment.NewLine, error.Value, error.Key))
								.ToArray()));
				}

				updateStatus = UpdateTaskStatusEnd(
					strTaskScheduleStatus,
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
					lSendMailTotal + "/" + lExtractTotal);

				//------------------------------------------------------
				// メール配信結果格納
				//------------------------------------------------------
				// 注意：一定時間経過するとスレッドが生きた状態でないと判断するため、今後の処理でもタスクスケジュール履歴へのINSERTは最後にすること
				InsertTaskScheduleHistory(
					(string)drvMailDistSetting[Constants.FIELD_MAILDISTSETTING_TARGET_ID],
					lMailSendResults,
					dvTargetListData);
				// メッセージ配信結果格納
				if (string.IsNullOrEmpty(lineSendResultFinaly) == false) InsertTaskScheduleLog(MessagingAppContentsModel.MESSAGING_APP_KBN_LINE, lineSendResultFinaly);
			}
			catch (Exception ex)
			{
				w2.Common.Logger.FileLogger.WriteError(ex);
				Form1.WriteErrorLogLine(ex.ToString());

				//------------------------------------------------------
				// メール配信設定ステータス更新（エラーへ）
				//------------------------------------------------------
				try
				{
					UpdateMailDistSettingStatus(Constants.FLG_MAILDISTSETTING_STATUS_ERROR);
				}
				catch (Exception ex2)
				{
					w2.Common.Logger.FileLogger.WriteError(ex2);
					Form1.WriteErrorLogLine(ex2.ToString());
				}

				//------------------------------------------------------
				// メール配信タスクステータス更新（エラー）
				//------------------------------------------------------
				try
				{
					UpdateTaskStatusEnd(
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_ERROR,
						null,
						lSendMailTotal.ToString() + "/" + lExtractTotal.ToString());
				}
				catch (Exception ex2)
				{
					w2.Common.Logger.FileLogger.WriteError(ex2);
					Form1.WriteErrorLogLine(ex2.ToString());
				}
				SendDebugMail(messageHead + " エラー発生" + ex.ToString());
			}
		}

		/// <summary>
		/// 無効なEmailAddressによるエラーレスポンスが引数に含まれるかどうかを判定する
		/// </summary>
		/// <param name="response">レスポンス</param>
		/// <returns>無効なEmailAddressによるエラーレスポンスが引数に含まれるならばTrue</returns>
		private bool ContainsSmtpErrorResponseForInvalidEmailAddress(string response)
		{
			var result = Regex.IsMatch(
				response, 
				Constants.PATTERN_SMTP_ERROR_RESPONSE_FOR_INVALID_EMAIL_ADDRESS);
			return result;
		}

		//=========================================================================================
		/// <summary>
		/// スレッドストップチェック
		/// </summary>
		/// <param name="strProgress">進捗</param>
		//=========================================================================================
		private void CheckScheduleStoppedAndUpdateProgress(string strProgress)
		{
			// タスクスケジュール取得
			DataView dvTaskSchedule = null;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("TaskSchedule", "GetTaskScheduleAndUpdateProgress"))
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.DeptId },
					{ Constants.FIELD_TASKSCHEDULE_ACTION_KBN, this.ActionKbn },
					{ Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, this.MasterId },
					{ Constants.FIELD_TASKSCHEDULE_ACTION_NO, this.ActionNo },
					{ Constants.FIELD_TASKSCHEDULE_PROGRESS, strProgress },
				};

				dvTaskSchedule = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
			}

			if (dvTaskSchedule.Count != 0)
			{
				if ((string)dvTaskSchedule[0][Constants.FIELD_TASKSCHEDULE_STOP_FLG] == Constants.FLG_TASKSCHEDULE_STOP_FLG_ON)
				{
					throw new ScheduleStopException();
				}
			}
		}

		//=========================================================================================
		/// <summary>
		/// メール配信設定取得＆ステータス変更
		/// </summary>
		/// <param name="strStatus">変更ステータス</param>
		/// <returns>ターゲットリスト</returns>
		//=========================================================================================
		protected DataRowView GetMailDistSettingAndChangeStatus(string strStatus)
		{
			DataRowView drvMailDistSetting = null;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("MailDistSetting", "GetMailDistSettingAndChangeStatus"))
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_MAILDISTSETTING_DEPT_ID, this.DeptId },
					{ Constants.FIELD_MAILDISTSETTING_MAILDIST_ID, this.MasterId },
					{ Constants.FIELD_MAILDISTSETTING_STATUS, strStatus },
				};

				while (true)
				{
					//------------------------------------------------------
					// 現在のステータス取得・チェック
					//------------------------------------------------------
					DataView dvMailDistSetting;
					lock (m_lockObject)
					{
						dvMailDistSetting = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, htInput);
					}
					if (dvMailDistSetting.Count != 0)
					{
						switch ((string)dvMailDistSetting[0][Constants.FIELD_TARGETLIST_STATUS])
						{
							case Constants.FLG_MAILDISTSETTING_STATUS_NORMAL:
							case Constants.FLG_MAILDISTSETTING_STATUS_ERROR:	// エラーでも実行できる
								// 次へ（ステータスは既に送信中となっている）
								drvMailDistSetting = dvMailDistSetting[0];
								break;

							case Constants.FLG_MAILDISTSETTING_STATUS_SEND:
								Form1.WriteDebugoLogLine("メール送信設定が利用可能ではないので暫く待ちます。：" + this.DeptId + "-" + this.MasterId + "-" + this.ActionNo);
								break;
						}
					}
					else
					{
						// 異常終了★
					}
					if (drvMailDistSetting != null)
					{
						break;	// 無限ループを抜ける
					}

					Thread.Sleep(1000);

					//------------------------------------------------------
					// 停止要求チェック
					//------------------------------------------------------
					try
					{
						CheckScheduleStoppedAndUpdateProgress(null);
					}
					catch (ScheduleStopException)
					{
						// 通常ステータスへ
						var iUpdateStatus = UpdateMailDistSettingStatus(Constants.FLG_MAILDISTSETTING_STATUS_NORMAL);

						// 配信ストップ
						var updateStatusEnd = UpdateTaskStatusEnd(
							Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP,
							null,
							"0/0");
					}
				}
			}

			return drvMailDistSetting;
		}

		//=========================================================================================
		/// <summary>
		/// メール配信設定ステータス更新（強制更新）
		/// </summary>
		/// <param name="strTargetId">ターゲットID</param>
		/// <param name="strStatus">ステータス</param>
		/// <returns></returns>
		//=========================================================================================
		protected int UpdateMailDistSettingStatus(string strStatus)
		{
			var iResult = 0;
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("MailDistSetting", "UpdateMailDistSettingStatus"))
			{
				var htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MAILDISTSETTING_DEPT_ID, this.DeptId);
				htInput.Add(Constants.FIELD_MAILDISTSETTING_MAILDIST_ID, this.MasterId);
				htInput.Add(Constants.FIELD_MAILDISTSETTING_STATUS, strStatus);
				htInput.Add(Constants.FIELD_MAILDISTSETTING_STATUS + "_target", null);

				iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}

			return iResult;
		}

		/// <summary>
		/// メール配信結果格納
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <param name="mailSendList">送信結果情報</param>
		/// <param name="targetListData">ターゲット取得</param>
		private void InsertTaskScheduleHistory(string targetId, List<Dictionary<string, string>> mailSendList, DataView targetListData)
		{
			LoggingDebug("いんさーと開始");
			using (SqlAccessor sqlAccessor = new SqlAccessor())
			{
				using (SqlStatement sqlStatement = new SqlStatement("TaskScheduleHistory", "InsertTaskScheduleHistory"))
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_TASKSCHEDULEHISTORY_DEPT_ID, this.DeptId);
					htInput.Add(Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_KBN, Constants.FLG_TASKSCHEDULEHISTORY_ACTION_KBN_MAIL_DIST);
					htInput.Add(Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID, this.MasterId);
					htInput.Add(Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO, this.ActionNo);
					htInput.Add(Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_STEP, 1);		// ★とりあえず1
					htInput.Add(Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_KBN_DETAIL, Constants.FLG_TASKSCHEDULEHISTORY_ACTION_KBN_DETAIL_MAIL_DIST);
					htInput.Add(Constants.FIELD_TASKSCHEDULEHISTORY_TARGET_ID, targetId);
					for (int index = 0; index < mailSendList.Count; index++)
					{
						htInput[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_RESULT] = mailSendList[index][SEND_MAIL_RESULT];
						htInput[Constants.FIELD_TASKSCHEDULEHISTORY_USER_ID] = targetListData[index][Constants.FIELD_TARGETLISTDATA_USER_ID];
						htInput[Constants.FIELD_TASKSCHEDULEHISTORY_MAIL_ADDR] = targetListData[index][Constants.FIELD_TARGETLISTDATA_MAIL_ADDR];

						sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
					}
				}
			}
			LoggingDebug("いんさーと完了");
		}

		/// <summary>
		/// ターゲットリストのステータス確認する
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>true:通常の場合, false:通常以外の場合</returns>
		private bool IsTargetListStatusNormal(string targetId)
		{
			var targetList = GetTargetList(targetId);
			return (StringUtility.ToEmpty(targetList[0][Constants.FIELD_TARGETLIST_STATUS]) == Constants.FLG_TARGETLIST_STATUS_NORMAL);
		}

		/// <summary>
		/// ターゲットリストIDでターゲットリスト情報を取得する
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>ターゲットリスト情報</returns>
		private DataView GetTargetList(string targetId)
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("TargetList", "GetTargetList"))
			{
				var input = new Hashtable
				{
					{Constants.FIELD_TARGETLIST_DEPT_ID, this.DeptId},
					{Constants.FIELD_TARGETLIST_TARGET_ID, targetId},
				};
				return sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
			}
		}

		/// <summary>
		/// 無効メールアドレス登録、エラーポイント追加
		/// </summary>
		/// <param name="drvTargetData">ターゲットデータ</param>
		/// <param name="addErrorPoint">加算エラーポイント</param>
		private void RegisterInvalidMailAddress(DataRowView drvTargetData, int addErrorPoint)
		{
			using (var accessor = new SqlAccessor())
			using (var statement = new SqlStatement("MailErrorAddr", "AddErrorPoint"))
			{
				var ht = new Hashtable()
				{
					{Constants.FIELD_MAILERRORADDR_MAIL_ADDR, (string)drvTargetData[Constants.FIELD_TARGETLISTDATA_MAIL_ADDR]},
					{Constants.FIELD_MAILERRORADDR_ERROR_POINT, addErrorPoint}
				};
				statement.ExecStatementWithOC(accessor, ht);
			}
		}

		/// <summary>
		/// メール配信時文章履歴登録(登録後、履歴NO取得)
		/// </summary>
		/// <param name="mailDistText">メール配信文章</param>
		/// <param name="mailDistName">メール配信設定名</param>
		/// <param name="compressedReplaceTags">圧縮したタグ置換情報</param>
		/// <returns>登録したメール配信時文章履歴NO</returns>
		private int InsertAndGetTextHistoryNo(DataRowView mailDistText, string mailDistName, string compressedReplaceTags)
		{
			var model = new MailSendTextHistoryModel
			{
				DeptId = this.DeptId,
				MailtextId = (string)mailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_ID],
				MaildistId = this.MasterId,
				MaildistName = mailDistName,
				MailBody = (string)mailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY],
				MailBodyHtml = (string)mailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_HTML],
				MailtextBodyMobile = (string)mailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_BODY_MOBILE],
				MailtextDecome = (string)mailDistText[Constants.FIELD_MAILDISTTEXT_MAILTEXT_DECOME],
				DateCreated = DateTime.Now,
				MailtextReplaceTags = compressedReplaceTags
			};
			var textHistoryNo = new MailSendLogService().InsertTextHistoryAndGetTextHistoryNo(model);
			return textHistoryNo;
		}

		/// <summary>
		/// メール配信文章グローバル用設定
		/// </summary>
		/// <param name="mailDistTextSetting">メール配信文章設定</param>
		/// <param name="smsMailSender">メール送信インスタンス</param>
		/// <param name="subjectTmp">メールタイトル</param>
		/// <param name="bodyTmp">メール文章</param>
		/// <param name="bodyHtmlTmp">メール文章HTML</param>
		/// <param name="tagSubjectTmp">メールタイトル置換用タグ</param>
		/// <param name="tagBodyTmp">メール文章置換用タグ</param>
		/// <param name="tagBodyHtmlTmp">メール文章HTML置換用タグ</param>
		private void SetMailDistTextGlobalSetting(
			MailDistTextModel mailDistTextSetting,
			ref SmtpMailSender smsMailSender,
			ref string subjectTmp,
			ref string bodyTmp,
			ref string bodyHtmlTmp,
			ref MatchCollection tagSubjectTmp,
			ref MatchCollection tagBodyTmp,
			ref MatchCollection tagBodyHtmlTmp)
		{
			// MailSenderの設定を更新(※CCとBCCは先にクリアしておく)
			smsMailSender.ClearCC();
			smsMailSender.ClearBcc();

			smsMailSender.SetFrom(mailDistTextSetting.MailFrom, mailDistTextSetting.MailFromName);
			smsMailSender.AddCC(mailDistTextSetting.MailCc);
			smsMailSender.AddBcc(mailDistTextSetting.MailBcc);

			// 件名、本文、HTML本文
			subjectTmp = mailDistTextSetting.MailtextSubject;
			bodyTmp = mailDistTextSetting.MailtextBody;
			bodyHtmlTmp = mailDistTextSetting.MailtextHtml;

			// タグ置換設定
			tagSubjectTmp = Regex.Matches(subjectTmp, "<@@user:((?!@@>).)*@@>");
			tagBodyTmp = Regex.Matches(bodyTmp, "<@@user:((?!@@>).)*@@>");
			tagBodyHtmlTmp = Regex.Matches(bodyHtmlTmp, "<@@user:((?!@@>).)*@@>");
		}

		#region 暫定対応
		/// <summary>
		/// デバッグ用にログ出力　改善されれば削除する
		/// </summary>
		/// <param name="message">メッセージ</param>
		private void LoggingDebug(string message)
		{
			FileLogger.Write(Constants.INTERIM, this.MasterId + "-" + this.ActionNo + " " + message, true);
		}

		/// <summary>
		/// 除外ユーザIDをログ出力
		/// </summary>
		/// <param name="targetData">ターゲット情報</param>
		private void LoggingExcludedUserId(DataRowView targetData)
		{
			var msg = new StringBuilder();
			msg.Append(this.MasterId + "-" + this.ActionNo).Append(" " + (string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID]).Append(" ユーザID送信済みのため除外");
			Form1.WriteDebugoLogLine(msg.ToString());
			LoggingDebug(msg.ToString());
		}

		/// <summary>
		/// 再アクション用にログファイルから送信済みユーザIDを取得
		/// </summary>
		/// <returns>送信済みユーザIDのリスト</returns>
		/// <remarks>
		/// ログファイルが在りEOFやSTOP_EOFで終わってるなら問題なし。なにもせずステータスだけ変えてスレッド終了（終了処理途中で正常終了できなかった可能性あり？）
		/// 上記でなければファイルからユーザIDを取得しリストへセット。ループ内でユーザIDチェックして一致していれば除外する
		/// </remarks>
		private List<string> GetSendUserIdList()
		{
			var exceptUserId = new List<string>();
			if (File.Exists(this.SendLogFile))
			{
				foreach (var line in File.ReadAllLines(this.SendLogFile))
				{
					var value = line.Trim();
					if (value == "") continue;
					if ((value == "EOF") || (value == "STOP_EOF")) break;

					var val = value.Split(',');
					if ((val.Length > 0) && (exceptUserId.Contains((string)val[1]) == false))
					{
						exceptUserId.Add((string)val[1]);
					}
				}
			}
			return exceptUserId;
		}

		#endregion
	}
}
