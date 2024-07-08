/*
=========================================================================================================
  Module      : 基底スレッドクラス(BaseThread.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using w2.App.Common.Util;
using w2.Common.Util;
using w2.Common.Sql;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Domain.User;
using w2.Domain.User.Helper;
using w2.Domain.MailErrorAddr;
using w2.Domain.TargetList;
using w2.Domain.TaskScheduleLog;
using w2.App.Common.MailDist;
using w2.Common.Web;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	class BaseThread
	{
		private DateTime m_dtScheduleDate = new DateTime(0);
		private string m_strActionKbn = null;
		private int m_iActionNo = -1;
		/// <summary>LockObjectsにアクセスするためのロックオブジェクト</summary>
		private static readonly object m_lockObject = new object();
		/// <summary>ロックオブジェクト</summary>
		private static readonly Dictionary<string, object> m_lockObjectKeyPairs = new Dictionary<string, object>();

		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		protected BaseThread()
		{
			// 何もしない //
		}

		/// <summary>
		/// ベースワーカースレッド
		/// </summary>
		/// <param name="dtScheduleDate">スケジュール日付</param>
		/// <param name="strDeptId">識別ID</param>
		/// <param name="strActionKbn">アクション区分</param>
		/// <param name="strMailDistId">メール配信ID</param>
		/// <param name="iActionNo">アクションNO</param>
		/// <param name="reAction">途中から再開するアクションNO</param>
		protected BaseThread(DateTime dtScheduleDate, string strDeptId, string strActionKbn, string strMailDistId, int iActionNo, bool reAction = false)
		{
			m_dtScheduleDate = dtScheduleDate;
			this.DeptId = strDeptId;
			m_strActionKbn = strActionKbn;
			this.MasterId = strMailDistId;
			m_iActionNo = iActionNo;
			this.ReAction = reAction;
		}

		/// <summary>
		/// タスクステータス更新（開始）
		/// </summary>
		/// <param name="strStatus">ステータス</param>
		/// <param name="strTargetStatus">更新対象ステータス</param>
		/// <returns>更新レコード数</returns>
		protected int UpdateTaskStatusBegin(string strStatus, string strTargetStatus, string strProgress)
		{
			return UpdateTaskStatus(strStatus, strTargetStatus, strProgress, "UpdateTaskStatusBegin");
		}
		/// <summary>
		/// タスクステータス更新（終了）
		/// </summary>
		/// <param name="strStatus">ステータス</param>
		/// <param name="strTargetStatus">更新対象ステータス</param>
		/// <returns>更新レコード数</returns>
		protected int UpdateTaskStatusEnd(string strStatus, string strTargetStatus, string strProgress)
		{
			return UpdateTaskStatus(strStatus, strTargetStatus, strProgress, "UpdateTaskStatusEnd");
		}
		/// <summary>
		/// タスクステータス更新
		/// </summary>
		/// <param name="strStatus">ステータス</param>
		/// <param name="strTargetStatus">更新対象ステータス</param>
		/// <param name="strProgress">進捗文字列</param>
		/// <param name="strStatement">実行SQLステートメント名</param>
		/// <returns>更新レコード数</returns>
		private int UpdateTaskStatus(string strStatus, string strTargetStatus, string strProgress, string strStatement)
		{
			int iUpdate = 0;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", strStatement))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE, m_dtScheduleDate);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.DeptId);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, m_strActionKbn);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, this.MasterId);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_NO, m_iActionNo);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS, strStatus);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_EXECUTE_STATUS + "_target", strTargetStatus);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_PROGRESS, strProgress);

				iUpdate = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}

			return iUpdate;
		}

		/// <summary>
		/// タスク準備ステータス更新
		/// </summary>
		/// <param name="strStatus">ステータス</param>
		/// <param name="strTargetStatus">更新対象ステータス</param>
		/// <returns>更新レコード数</returns>
		protected int UpdatePrepareTaskStatus(string strStatus, string strTargetStatus)
		{
			int iUpdate = 0;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("TaskSchedule", "UpdatePrepareTaskStatus"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_TASKSCHEDULE_SCHEDULE_DATE, m_dtScheduleDate);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_DEPT_ID, this.DeptId);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_KBN, m_strActionKbn);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_MASTER_ID, this.MasterId);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_ACTION_NO, m_iActionNo);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_PREPARE_STATUS, strStatus);
				htInput.Add(Constants.FIELD_TASKSCHEDULE_PREPARE_STATUS + "_target", strTargetStatus);

				iUpdate = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}

			return iUpdate;
		}

		/// <summary>
		/// タスクスケジュールログ登録
		/// </summary>
		/// <param name="messagingAppKbn">メッセージアプリ区分</param>
		/// <param name="result">配信結果</param>
		/// <returns>更新レコード数</returns>
		protected int InsertTaskScheduleLog(string messagingAppKbn, string result)
		{
			var model = new TaskScheduleLogModel
			{
				DeptId = this.DeptId,
				ActionKbn = m_strActionKbn,
				ActionMasterId = this.MasterId,
				ActionNo = m_iActionNo,
				MessagingAppKbn = messagingAppKbn,
				Result = result
			};

			var updateRecordCount = new TaskScheduleLogService().InsertLog(model);
			return updateRecordCount;
		}

		/// <summary>
		/// ターゲットリスト取得＆ステータス変更
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <param name="status">変更ステータス</param>
		/// <returns>ターゲットリスト</returns>
		protected DataRowView GetTargetListAndUpdateStatus(string targetId, string status)
		{
			DataRowView result = null;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("TargetList", "GetTargetListAndChangeStatus"))
			{
				sqlStatement.CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT;
				var parameters = new Hashtable()
				{
					{Constants.FIELD_TARGETLIST_DEPT_ID, this.DeptId},
					{Constants.FIELD_TARGETLIST_TARGET_ID, targetId},
					{Constants.FIELD_TARGETLIST_STATUS, status},
				};

				var beginDate = DateTime.Now;
				var alert = 1;
				while (true)
				{
					var targetList = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, parameters);
					if (targetList.Count != 0)
					{
						var targetListStatus = (string)targetList[0][Constants.FIELD_TARGETLIST_STATUS];
						var message = new StringBuilder(string.Format("：ステータス[{0}({1})]{2}-{3}", 
							ValueText.GetValueText(Constants.TABLE_TARGETLIST, Constants.FIELD_TARGETLIST_STATUS, targetListStatus), 
							targetListStatus,
							this.DeptId, 
							targetId));

						switch (targetListStatus)
						{
							case Constants.FLG_TARGETLIST_STATUS_NORMAL:
								// 次へ（ステータスは既に抽出中となっている）
								result = targetList[0];
								break;

							case Constants.FLG_TARGETLIST_STATUS_EXTRACT:
							case Constants.FLG_TARGETLIST_STATUS_USING:
								message = message.Insert(0, "ターゲットリストのステータスが利用可能ではないので暫く待ちます。");
								Form1.WriteDebugoLogLine(message.ToString());

								// ○分毎にアラートメールをシステム管理者に送信
								if ((beginDate + TimeSpan.FromMinutes(alert * Constants.SEND_ALERTMAIL_INTERVAL_MINUTES)) <= DateTime.Now)
								{
									alert++;
									SendAdministratorMail("ターゲットリスト抽出停滞エラー", message.ToString());
								}
								break;

							case Constants.FLG_TARGETLIST_STATUS_ERROR:
								message = message.Insert(0, "対象ターゲットの抽出が失敗しています。");
								throw new ApplicationException(message.ToString());
						}
					}
					if (result != null)
					{
						break;	// 無限ループを抜ける
					}

					Thread.Sleep(1000);
				}
			}

			return result;
		}

		/// <summary>
		/// ターゲットリストステータス更新（強制更新・カウント数更新しない）
		/// </summary>
		/// <param name="strTargetId">ターゲットID</param>
		/// <param name="strStatus">ステータス</param>
		/// <returns></returns>
		protected int UpdateTargetListStatus(string strTargetId, string strStatus)
		{
			int iResult = 0;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("TargetList", "UpdateTargetListStatus2"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_TARGETLIST_DEPT_ID, this.DeptId);
				htInput.Add(Constants.FIELD_TARGETLIST_TARGET_ID, strTargetId);
				htInput.Add(Constants.FIELD_TARGETLIST_STATUS, strStatus);
				htInput.Add(Constants.FIELD_TARGETLIST_STATUS + "_target", null);

				iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}

			return iResult;
		}
		/// <summary>
		/// ターゲットリストステータス更新（強制更新）
		/// </summary>
		/// <param name="strTargetId"></param>
		/// <param name="strStatus"></param>
		/// <param name="objDataCount"></param>
		/// <returns></returns>
		protected int UpdateTargetListStatus(string strTargetId, string strStatus, object objDataCount)
		{
			int iResult = 0;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("TargetList", "UpdateTargetListStatus"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_TARGETLIST_DEPT_ID, this.DeptId);
				htInput.Add(Constants.FIELD_TARGETLIST_TARGET_ID, strTargetId);
				htInput.Add(Constants.FIELD_TARGETLIST_DATA_COUNT, objDataCount);
				htInput.Add(Constants.FIELD_TARGETLIST_DATA_COUNT_DATE, (objDataCount != null) ? DateTime.Now : (object)null);
				htInput.Add(Constants.FIELD_TARGETLIST_STATUS, strStatus);
				htInput.Add(Constants.FIELD_TARGETLIST_STATUS + "_target", null);

				iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}

			return iResult;
		}

		/// <summary>
		/// 会員ランク付与ルールのステータス更新（強制更新・カウント数更新しない）
		/// </summary>
		/// <param name="strMemberRankRuleId">ランク付与ルールID</param>
		/// <param name="strStatus">更新ステータス</param>
		/// <returns></returns>
		protected int UpdateMemberRankRuleStatus(string strMemberRankRuleId, string strStatus)
		{
			int iResult = 0;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "UpdateMemberRankRuleStatusNonCount"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID, strMemberRankRuleId);	// ランク付与ルールID
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_STATUS, strStatus);							// ステータス
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_STATUS + "_target", null);

				iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}

			return iResult;
		}
		/// <summary>
		/// 会員ランク付与ルールのステータス更新（強制更新）
		/// </summary>
		/// <param name="strMemberRankRuleId">ランク付与ルールID</param>
		/// <param name="strStatus">更新ステータス</param>
		/// <param name="objDataCount">抽出完了件数</param>
		/// <returns></returns>
		protected int UpdateMemberRankRuleStatus(string strMemberRankRuleId, string strStatus, object objDataCount)
		{
			int iResult = 0;

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "UpdateMemberRankRuleStatus"))
			{
				Hashtable htInput = new Hashtable();
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_MEMBER_RANK_RULE_ID, strMemberRankRuleId);	// ランク付与ルールID
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_LAST_COUNT, objDataCount);					// 最終付与人数
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_LAST_EXEC_DATE, (objDataCount != null) ? DateTime.Now : (object)null);// 最終付与日時
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_STATUS, strStatus);							// ステータス
				htInput.Add(Constants.FIELD_MEMBERRANKRULE_STATUS + "_target", null);

				iResult = sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
			}

			return iResult;
		}

		/// <summary>
		/// 有効な会員ランクかチェックする
		/// </summary>
		/// <param name="memberRank">会員ランク</param>
		/// <returns>有効な会員ランクか否か</returns>
		protected bool IsValidMemberRank(string memberRank)
		{
			DataView dv = null;
			Hashtable sqlParameter = new Hashtable()
			{
				{Constants.FIELD_MEMBERRANK_MEMBER_RANK_ID, memberRank}
			};

			using (SqlAccessor sqlAccessor = new SqlAccessor())
			using (SqlStatement sqlStatement = new SqlStatement("MemberRankRule", "GetMemberRank"))
			{
				dv = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, sqlParameter);
			}

			if (dv.Count != 0)
			{
				return ((string)dv[0][Constants.FIELD_MEMBERRANK_VALID_FLG] == Constants.FLG_MEMBERRANK_VALID_FLG_VALID);
			}

			return false;
		}

		/// <summary>
		/// システム管理者向けメール（デバッグ用）
		/// </summary>
		/// <param name="message">メッセージ</param>
		protected void SendDebugMail(string message)
		{
			if (Constants.MESSAGE_DEBUG)
			{
				SendAdministratorMail(message, message);
			}
		}

		/// <summary>
		/// システム管理者向けメール送信
		/// </summary>
		/// <param name="subject">件名</param>
		/// <param name="message">メッセージ</param>
		/// <remarks>ブレインサーバを利用して送信する為、キューが溜まっている場合は受信するまで時間かかる</remarks>
		protected void SendAdministratorMail(string subject, string message)
		{
			using (SmtpMailSender mailSender = new SmtpMailSender(Constants.SERVER_SMTP))
			{
				mailSender.SetSubject(Constants.MAIL_SUBJECTHEAD + " " + subject);
				mailSender.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => mailSender.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => mailSender.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => mailSender.AddBcc(mail.Address));

				mailSender.SetBody(DateTime.Now + " " + message);

				if (mailSender.SendMail() == false)
				{
					FileLogger.WriteError(mailSender.MailSendException);
				}
			}
		}

		/// <summary>
		/// システム管理者向けSMTPサーバー接続エラーメール送信
		/// </summary>
		/// <param name="mailDistSettingId">メール配信設定ID</param>
		/// <remarks>ブレインサーバーが落ちているときも想定しているため、利用するSMTPサーバーを切り替える</remarks>
		protected void SendAdministratorSmtpErrorMail(string mailDistSettingId)
		{
			using (var mailSender = new SmtpMailSender(Constants.SERVER_SMTP_FOR_ERROR, Constants.SERVER_SMTP_PORT_FOR_ERROR))
			{
				// 利用SMTPサーバー切り替え
				mailSender.UpdateSmtpAuthInfo(
					Constants.SERVER_SMTP_AUTH_TYPE_FOR_ERROR,
					Constants.SERVER_SMTP_AUTH_POP_SERVER_FOR_ERROR,
					Constants.SERVER_SMTP_AUTH_POP_PORT_FOR_ERROR,
					Constants.SERVER_SMTP_AUTH_POP_TYPE_FOR_ERROR,
					Constants.SERVER_SMTP_AUTH_USER_NAME_FOR_ERROR,
					Constants.SERVER_SMTP_AUTH_PASSOWRD_FOR_ERROR);

				mailSender.SetSubject(Constants.MAIL_SUBJECTHEAD + " " + "SMTPサーバー接続エラー(メール配信設定ID：" + mailDistSettingId + ")");
				mailSender.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => mailSender.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => mailSender.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => mailSender.AddBcc(mail.Address));

				// 件数が非常に多くなる可能性があるため、メールには直接記載しない
				var message = "メール配信時にSMTPサーバーへの接続エラーが発生しました。\nログファイルに対象ユーザーのIDを記載しています。";
				mailSender.SetBody(message);

				if (mailSender.SendMail() == false)
				{
					FileLogger.WriteError(mailSender.MailSendException);
				}
			}
		}

		/// <summary>
		/// エラーポイント未満かどうか？
		/// </summary>
		/// <param name="mailAddr">メールアドレス</param>
		/// <returns>エラーポイント未満：true、エラーポイント以上：false</returns>
		protected bool IsUnderUserErrorPoint(string mailAddr)
		{
			// メールエラーアドレス情報が存在しない場合は未満とする
			var mailErrorAddr = new MailErrorAddrService().Get(mailAddr);
			if (mailErrorAddr == null) return true;

			// エラーポイント未満？
			if (mailErrorAddr.ErrorPoint < Constants.SEND_MAIL_ERROR_POINT) return true;

			// エラーポイント以上の場合はログ出力
			var message =
				string.Format("{0}エラーポイント以上のためメール未送信（mail_addr：{1}, error_point：{2}）",
				Constants.SEND_MAIL_ERROR_POINT,
				mailErrorAddr.MailAddr,
				mailErrorAddr.ErrorPoint);
			FileLogger.WriteError(message);

			return false;
		}

		/// <summary>
		/// メール配信文章タグを置換、置換情報を保存（ユーザー情報）
		/// </summary>
		/// <param name="mailTextReplaceTags">メール文章タグ置換情報</param>
		/// <param name="user">メール配信用のユーザー情報</param>
		/// <param name="text">メール配信用文章</param>
		/// <param name="match">置換タグ</param>
		protected void SetReplaceTags(
			Dictionary<string, string> mailTextReplaceTags,
			UserForMailSend user,
			StringBuilder text,
			MatchCollection match)
		{
			foreach (Match tag in match)
			{
				var key = tag.Value.Replace("<@@user:", "").Replace("@@>", "");
				var value = (user != null) ? StringUtility.ToEmpty(ConvertMailDistTextUserInfo(user, key)) : "";
				text.Replace(tag.Value, value);

				if (mailTextReplaceTags.ContainsKey(key) == false)
				{
					mailTextReplaceTags.Add(key, value);
				}
			}
		}

		/// <summary>
		/// メール配信文章タグを置換、置換情報を保存（メール登録解除リンク）
		/// </summary>
		/// <param name="user">メール配信用のユーザー情報</param>
		/// <param name="text">メール配信用文章</param>
		protected void SetReplaceMailUnsubscribeTags(
			UserForMailSend user,
			StringBuilder text)
		{
			var userId = (string)user.DataSource[Constants.FIELD_USER_USER_ID];
			var mailAddress = (string)user.DataSource[Constants.FIELD_USER_MAIL_ADDR];
			var unsubscribeUrl =
				new UrlCreator($"{Constants.PROTOCOL_HTTPS}{Constants.SITE_DOMAIN}{Constants.PATH_ROOT_FRONT_PC}{Constants.MAIL_LISTUNSUBSCRIBE_URL}")
					.AddParam(Constants.MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_USER_ID, userId)
					.AddParam(Constants.MAIL_LISTUNSUBSCRIBE_REQUEST_KEY_VERIFICATION_KEY, UnsubscribeVarificationHelper.Hash(userId, mailAddress)).CreateUrl();
			text.Replace("<@@ mail_unsubscribe_link @@>", unsubscribeUrl);
		}

		/// <summary>
		/// メール配信文章用にユーザー情報を変換
		/// </summary>
		/// <param name="user">メール配信用のユーザー情報</param>
		/// <param name="tagKey">変換対象ユーザー情報項目</param>
		/// <returns>変換後のユーザー情報</returns>
		protected string ConvertMailDistTextUserInfo(UserForMailSend user, string tagKey)
		{
			var userInfo = ""; // 返却用のユーザー情報
			switch (tagKey)
			{
				// 名前
				case Constants.FIELD_USER_NAME:
					userInfo = UserModel.CreateComplementUserName(
						(string)user.DataSource[tagKey],
						(string)user.DataSource[Constants.FIELD_USER_MAIL_ADDR],
						(string)user.DataSource[Constants.FIELD_USER_MAIL_ADDR2]);
					break;

				// 氏名（姓）
				case Constants.FIELD_USER_NAME1:
					userInfo = (string.IsNullOrEmpty((string)user.DataSource[tagKey]) && string.IsNullOrEmpty((string)user.DataSource[Constants.FIELD_USER_NAME2]))
						? UserModel.CreateComplementUserName(
							(string)user.DataSource[tagKey],
							(string)user.DataSource[Constants.FIELD_USER_MAIL_ADDR],
							(string)user.DataSource[Constants.FIELD_USER_MAIL_ADDR2])
						: (string)user.DataSource[tagKey];
					break;

				// 性別（区分）
				case Constants.FIELD_USER_SEX:
				// メール配信区分（区分）
				case Constants.FIELD_USER_MAIL_FLG:
					userInfo = ValueText.GetValueText(Constants.TABLE_USER, tagKey, (string)user.DataSource[tagKey]);
					break;

				// 生年月日
				case Constants.FIELD_USER_BIRTH:
				// ポイント有効期限
				case Constants.FIELD_USERPOINT_POINT_EXP:
					userInfo = DateTimeUtility.ToString(
						user.DataSource[tagKey],
						DateTimeUtility.FormatType.ShortDate2Letter,
						(string)user.DataSource[Constants.FIELD_USER_DISP_LANGUAGE_LOCALE_ID]);
					break;

				// パスワード
				case Constants.FIELD_USER_PASSWORD:
					// ユーザーパスワードが空文字以外の場合、復号化
					string password = (string)user.DataSource[Constants.FIELD_USER_PASSWORD];
					if (password != "")
					{
						w2.Common.Util.Security.RijndaelCrypto userPassword
							= new w2.Common.Util.Security.RijndaelCrypto(Constants.ENCRYPTION_USER_PASSWORD_KEY, Constants.ENCRYPTION_USER_PASSWORD_IV);
						userInfo = userPassword.Decrypt(password);
					}
					// それ以外の場合、値をそのまま設定
					else
					{
						userInfo = password;
					}
					break;

				// 通常ポイント
				case Constants.FIELD_USERPOINT_POINT:
				// 期間限定本ポイント合計（利用可能なポイントのみ）
				case UserForMailSend.FIELD_USERFORMAILSEND_LIMITED_TERM_POINT:
				// 利用可能ポイント合計
				case UserForMailSend.FIELD_USERFORMAILSEND_POINT_USABLE:
					userInfo = StringUtility.ToNumeric(((decimal)(user.DataSource[tagKey])));
					break;

				default:
					userInfo = ((tagKey.StartsWith(Constants.FLG_USEREXTENDSETTING_PREFIX_KEY))
						? ((user.DataSource.Contains(tagKey)) ? StringUtility.ToEmpty(user.DataSource[tagKey]) : "")
						: (StringUtility.ToEmpty(user.DataSource[tagKey])));
					break;
			}

			return userInfo;
		}

		/// <summary>
		/// メール配信結果を随時ログ出力
		/// </summary>
		/// <param name="targetData">ターゲット情報</param>
		/// <param name="mailSendResult">送信結果</param>
		protected void SendLoggingResult(DataRowView targetData, string mailSendResult)
		{
			SendLoggingResult(
				(string)targetData[Constants.FIELD_TARGETLISTDATA_USER_ID],
				(string)targetData[Constants.FIELD_TARGETLISTDATA_MAIL_ADDR],
				mailSendResult);
		}
		/// <summary>
		/// メール配信結果を随時ログ出力
		/// </summary>
		/// <param name="targetData">ターゲット情報</param>
		/// <param name="mailSendResult">送信結果</param>
		protected void SendLoggingResult(TargetListDataModel targetData, string mailSendResult)
		{
			SendLoggingResult(
				targetData.UserId,
				targetData.MailAddr,
				mailSendResult);
		}
		/// <summary>
		/// メール配信結果を随時ログ出力
		/// </summary>
		/// <param name="userId">ターゲット情報</param>
		/// <param name="mailAddr">送信結果</param>
		/// <param name="mailSendResult">送信結果</param>
		protected void SendLoggingResult(string userId, string mailAddr, string mailSendResult)
		{
			var message = string.Join(
				",",
				DateTime.Now.ToString("yyyy年MM月dd日 HH:mm:ss"),
				userId,
				mailAddr,
				mailSendResult);

			SendLoggingResult(message);
		}
		/// <summary>
		/// メール配信結果を随時ログ出力
		/// </summary>
		/// <param name="message">メッセージ</param>
		/// <remarks>FileLoggerだと日付またぐ場合があるのと、Logger側の仕様変更でこちらも直す必要あるためSystem.IO.Fileで吐き出す。</remarks>
		public void SendLoggingResult(string message)
		{
			try
			{
				// Mutexで排他制御しながらファイル書き込み
				using (var mtx = new Mutex(false, this.SendLogFile.Replace("\\", "_") + ".FileWrite"))
				{
					try
					{
						mtx.WaitOne();

						using (var sw = new StreamWriter(this.SendLogFile, true, Encoding.Default))
						{
							sw.WriteLine(message);
						}
					}
					finally
					{
						mtx.ReleaseMutex();	// Dispose()で呼ばれない模様。
					}
				}
			}
			catch (Exception)
			{
				// 例外の場合はなにもしない
			}
		}

		/// <summary>
		/// ロックオブジェクトの取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="actionKbn">アクション区分</param>
		/// <param name="masterId">マスタId</param>
		/// <returns>ロックオブジェクト</returns>
		protected static object GetLockObject(string deptId, string actionKbn, string masterId)
		{
			lock (m_lockObject)
			{
				var key = string.Join(".", deptId, actionKbn, masterId);;
				if (m_lockObjectKeyPairs.ContainsKey(key) == false)
				{
					m_lockObjectKeyPairs.Add(key, new object());
				}
				var lockObject = m_lockObjectKeyPairs[key];
				return lockObject;
			}
		}

		/// <summary>
		/// メールクリック情報登録
		/// </summary>
		/// <param name="mailTextId">メールID</param>
		/// <param name="dvMailClick">メールクリック情報</param>
		protected void InsertMailInfometion(string mailTextId, DataView dvMailClick)
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("MailClick", "InsertMailClick"))
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				var input = new Hashtable
				{
					{ Constants.FIELD_MAILCLICK_DEPT_ID, this.DeptId },
					{ Constants.FIELD_MAILCLICK_MAILTEXT_ID, mailTextId },
					{
						Constants.FIELD_MAILCLICK_MAILDIST_ID, ((this.ActionKbn == Constants.FLG_TASKSCHEDULE_ACTION_KBN_PUBLISH_COUPON) ? "c" : string.Empty) + this.MasterId
					},
					{ Constants.FIELD_MAILCLICK_ACTION_NO, this.ActionNo },
				};

				foreach (DataRowView drvMailClick in dvMailClick)
				{
					input[Constants.FIELD_MAILCLICK_PCMOBILE_KBN] = (string)drvMailClick[Constants.FIELD_MAILCLICK_PCMOBILE_KBN];
					input[Constants.FIELD_MAILCLICK_MAILCLICK_ID] = (string)drvMailClick[Constants.FIELD_MAILCLICK_MAILCLICK_ID];
					input[Constants.FIELD_MAILCLICK_MAILCLICK_URL] = (string)drvMailClick[Constants.FIELD_MAILCLICK_MAILCLICK_URL];
					input[Constants.FIELD_MAILCLICK_MAILCLICK_KEY] = (string)drvMailClick[Constants.FIELD_MAILCLICK_MAILCLICK_KEY] + this.ActionNo.ToString();

					sqlStatement.ExecStatement(sqlAccessor, input);
				}
				sqlAccessor.CommitTransaction();
			}
		}

		/// <summary>
		/// メールクリック変換
		/// </summary>
		/// <param name="mailText">メールテキスト</param>
		/// <param name="mailClick">メールクリック情報</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="blIsHtml">文書がHTMLか</param>
		/// <param name="isPc">PCメールか</param>
		/// <returns>mc.aspxに変換されたURL</returns>
		protected string ConvertMailClickUrl(string mailText, DataView mailClick, string userId, bool blIsHtml, bool isPc)
		{
			var urlPattern = MailDistTextUtility.GetPatturnUrl(blIsHtml);
			var separatePattern = MailDistTextUtility.GetSeparatePattern(blIsHtml);
			var mailClickUrl = isPc ? Constants.MAILCLICK_URL_PC : Constants.MAILCLICK_URL_MOBILE;

			var sbNewBody = new StringBuilder();
			foreach (var mailTextLine in MailDistTextUtility.CreateMailTextLines(mailText, separatePattern))
			{
				var temp = mailTextLine;

				var mAnchorUrl = Regex.Match(mailTextLine, urlPattern, RegexOptions.Singleline | RegexOptions.IgnoreCase);
				if (mAnchorUrl.Success)
				{
					var anchorUrl = mAnchorUrl.Value;
					var url = anchorUrl;
					if (blIsHtml)
					{
						var mUrl = Regex.Match(url, MailDistTextUtility.PATTURN_URL_TEXT);
						if (mUrl.Success)
						{
							url = mUrl.Value;
						}
					}

					foreach (DataRowView drvMailClick in mailClick)
					{
						if ((url == (string)drvMailClick[Constants.FIELD_MAILCLICK_MAILCLICK_URL])
							&& ((isPc && ((string)drvMailClick[Constants.FIELD_MAILCLICK_PCMOBILE_KBN] == Constants.FLG_MAILCLICK_PCMOBILE_KBN_PC))
								|| ((isPc == false) && ((string)drvMailClick[Constants.FIELD_MAILCLICK_PCMOBILE_KBN] == Constants.FLG_MAILCLICK_PCMOBILE_KBN_MOBILE))))
						{
							var newUrl = mailClickUrl
								+ "?key=" + System.Web.HttpUtility.UrlEncode((string)drvMailClick[Constants.FIELD_MAILCLICK_MAILCLICK_KEY] + this.ActionNo.ToString())
								+ "&uid=" + System.Web.HttpUtility.UrlEncode(userId);

							temp = temp.Replace(url, newUrl);
							if (blIsHtml)
							{
								// <img src="～...のURLが置換された場合、元のURLに戻す。
								temp = temp.Replace("src=\"" + newUrl, "src=\"" + url);
							}

							break;
						}
					}
				}
				sbNewBody.Append(temp);
			}

			return sbNewBody.ToString();
		}

		/// <summary>スケジュール日時</summary>
		public DateTime ScheduleDate
		{
			get { return m_dtScheduleDate; }
		}
		/// <summary>識別ID</summary>
		public string DeptId{ get; protected set;}
		/// <summary>アクション区分</summary>
		public string ActionKbn
		{
			get { return m_strActionKbn; }
		}
		/// <summary>マスタID</summary>
		public string MasterId{ get; protected set;}
		/// <summary>アクションNO</summary>
		public int ActionNo
		{
			get { return m_iActionNo; }
		}
		/// <summary>実行スレッド本体</summary>
		public Thread Thread { get; protected set; }
		/// <summary>再アクションか？ ※暫定対応の為、現状メール送信(Maildist)のみ使用</summary>
		public bool ReAction { get; private set; }
		/// <summary>メール配信配信結果のログ出力先</summary>
		protected string SendLogFile
		{
			get
			{
				return string.Format(
					"{0}mailsend {1}-{2}.log",
					Constants.PHYSICALDIRPATH_LOGFILE,
					this.MasterId,
					this.ActionNo);

			}
		}
	}
}
