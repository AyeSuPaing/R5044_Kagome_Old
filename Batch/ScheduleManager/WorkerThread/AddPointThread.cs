/*
=========================================================================================================
  Module      : ポイント付与スレッドクラス(AddPointThread.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Linq;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using w2.App.Common.CrossPoint.Helper;
using w2.App.Common.Option.CrossPoint;
using w2.App.Common.ShopMessage;
using w2.App.Common.User;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Net.Mail;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.Point;
using w2.Domain.MailSendLog;
using w2.Domain.MailDist;
using w2.Domain.MessagingAppContents;
using w2.Domain.TaskSchedule;
using w2.Domain.TargetList;
using w2.App.Common.Line.LineDirectMessage.MessageType;
using w2.Domain.User.Helper;
using w2.App.Common.Line.LineDirectConnect;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	/// <summary>
	/// ポイント付与スレッドクラス
	/// </summary>
	class AddPointThread : BaseThread
	{
		/// <summary>
		/// スレッド作成（タスクスケジュール実行）
		/// </summary>
		/// <param name="scheduleDate">スケジュール日付</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="pointRuleScheduleId">ポイントルールスケジュールID</param>
		/// <param name="actionNo">アクションNO</param>
		/// <returns>生成スレッド</returns>
		public static AddPointThread CreateAndStart(DateTime scheduleDate, string deptId, string pointRuleScheduleId, int actionNo)
		{
			// スレッド作成
			var addPointThread = new AddPointThread(scheduleDate, deptId, pointRuleScheduleId, actionNo);
			addPointThread.Thread = new Thread(addPointThread.Work);

			// スレッドスタート
			addPointThread.Thread.Start();

			return addPointThread;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="scheduleDate">スケジュール日付</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="pointRuleScheduleId">ポイントルールスケジュールID</param>
		/// <param name="actionNo">アクションNO</param>
		public AddPointThread(DateTime scheduleDate, string deptId, string pointRuleScheduleId, int actionNo)
			: base(scheduleDate, deptId, Constants.FLG_TASKSCHEDULE_ACTION_KBN_ADD_POINT, pointRuleScheduleId, actionNo)
		{
			// 処理しない //
		}

		/// <summary>
		/// ポイント付与
		/// </summary>
		public void Work()
		{
			long extractTotal = 0; // 抽出人数
			long addPointTotal = 0; // 付与人数
			var service = new PointService();

			try
			{
				// ポイント付与タスクステータス更新(開始)
				UpdateTaskStatusBegin(Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE, Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT, "");

				// 処理開始宣言
				Form1.WriteInfoLogLine("ポイント付与[" + this.MasterId + "]付与開始");

				// ポイント付与ルール取得（同時にステータス更新、取得できるまで待つ）
				var pointRuleSchedule = GetPointRuleSchedule(Constants.FLG_MEMBERRANKRULE_STATUS_UPDATE);
				var pointRule = service.GetPointRule(this.DeptId, pointRuleSchedule.PointRuleId);

				// ポイントルール無効
				if ((pointRule == null) || (pointRule.ValidFlg == Constants.FLG_POINTRULE_VALID_FLG_INVALID))
				{
					// ポイント付与ルールのステータス更新（通常状態へ）（※ステータスは同一ステータスで更新し、最終付与人数・日時も更新）
					pointRuleSchedule.Status = Constants.FLG_POINTRULESCHEDULE_STATUS_NORMAL;
					service.UpdatePointRuleScheduleStatus(pointRuleSchedule);

					// 処理終了宣言
					Form1.WriteInfoLogLine("ポイント付与[" + this.MasterId + "]付与完了：" + pointRuleSchedule.PointRuleId + "は無効です。");

					// ポイント付与タスクステータス更新（完了）
					UpdateTaskStatusEnd(Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE, Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE, "");

					return;
				}

				// ターゲット抽出ステータス更新（「実行中」へ）
				UpdatePrepareTaskStatus(Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE, Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT);

				// 実行タイミングを問わずターゲット抽出フラグがONの場合は、ターゲットを取得するために、ポイント付与ユーザー抽出スレッドを作成
				if (pointRuleSchedule.TargetExtractFlg == Constants.FLG_POINTRULESCHEDULE_TARGET_EXTRACT_FLG_ON)
				{
					var targetExtractThread = TargetExtractThread.CreateAndStart(Constants.CONST_DEFAULT_DEPT_ID, pointRuleSchedule.TargetId);
					while (targetExtractThread.Thread.IsAlive)
					{
						Thread.Sleep(50);
					}
				}
				// ターゲット抽出ステータス更新（「実行完了」へ）
				UpdatePrepareTaskStatus(Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_DONE, Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_EXECUTE);

				// 抽出済ターゲットデータ取得
				var targetListData = new TargetListService().GetTargetListDataDeduplicated(this.DeptId, Constants.FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST, pointRuleSchedule.TargetId);
				extractTotal = targetListData.Length;

				// ポイント付与
				var taskScheduleStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE;	// デフォルトは完了へ

				var errorMessage = new StringBuilder();
				try // 停止例外捕捉用try
				{
					// ポイント付与ループ開始
					foreach (var targetData in targetListData)
					{
						// スケジュール停止フラグチェック＆進捗更新（とりあえず10件ずつチェックする）
						if ((addPointTotal % 10) == 0)
						{
							CheckScheduleStoppedAndUpdateProgress(string.Format("{0}/{1}", addPointTotal, extractTotal));
						}

						// 後々のOneToOneを考えると一件ずつ付与した方がよい //

						// ポイント付与情報取得＆設定
						var userId = targetData.UserId;
						var mailId = pointRuleSchedule.MailId;

						var user = new UserService().Get(userId);

						using (var accessor = new SqlAccessor())
						{
							// トランザクション開始
							accessor.OpenConnection();
							accessor.BeginTransaction();

							// ポイント発行
							service.IssuePointByRule(
								pointRule,
								userId,
								string.Empty,
								pointRule.IncNum,
								Constants.FLG_LASTCHANGED_BATCH,
								UpdateHistoryAction.DoNotInsert,
								accessor);

							if (Constants.CROSS_POINT_OPTION_ENABLED
								&& ((pointRule.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE)
									|| (pointRule.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT)))
							{
								// Update point api
								var pointKbn = (pointRule.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_VERSATILE_POINT_RULE)
									? w2.App.Common.Constants.CROSS_POINT_REASON_KBN_POINT_RULE
									: w2.App.Common.Constants.CROSS_POINT_REASON_KBN_BIRTHDAY;
								var result = CrossPointUtility.UpdateCrossPointApi(
									user,
									pointRule.IncNum,
									CrossPointUtility.GetValue(
										Constants.CROSS_POINT_SETTING_ELEMENT_REASON_ID,
										pointKbn),
									accessor);

								if (result.Length > 0)
								{
									var error = ErrorHelper.CreateCrossPointApiError(result, user.UserId);
									errorMessage.AppendLine(error);
									accessor.RollbackTransaction();
									continue;
								}
								else
								{
									UserUtility.AdjustPointAndMemberRankByCrossPointApi(user, accessor: accessor);
								}
							}

							// 誕生日ポイントの場合は誕生日ポイント付与年を更新
							if (pointRule.PointIncKbn == Constants.FLG_POINTRULE_POINT_INC_KBN_BIRTHDAY_POINT)
							{
								user.LastBirthdayPointAddYear = DateTime.Now.Year.ToString();
								new UserService().UpdateUserBirthdayPointAddYear(user, UpdateHistoryAction.Insert, accessor);
							}

							// コミット
							accessor.CommitTransaction();
						}

						// ポイント付与人数をインクリメント
						addPointTotal++;

						// メール送信
						if (mailId != "")
						{
							SendMail(mailId, targetData);
						}

						// 少し休む
						Thread.Sleep(50);
					}
					SendLoggingResult("EOF\r\n");
				}
				catch (ScheduleStopException)
				{
					Form1.WriteInfoLogLine("■ポイント付与停止要求あり■");

					taskScheduleStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP;	// 「停止中」へ
					SendLoggingResult("STOP_EOF\r\n");
				}

				// ポイント付与ルールのステータス更新（通常状態へ）（※ステータスは同一ステータスで更新し、最終付与人数・日時も更新）
				pointRuleSchedule.Status = Constants.FLG_POINTRULESCHEDULE_STATUS_NORMAL;
				pointRuleSchedule.LastCount = addPointTotal.ToString();
				pointRuleSchedule.LastExecDate = DateTime.Now;
				service.UpdatePointRuleScheduleStatus(pointRuleSchedule);

				// 処理終了宣言
				Form1.WriteInfoLogLine(string.Format("ポイント付与[{0}]付与完了：{1}/{2}件", this.MasterId, addPointTotal, extractTotal));

				// Api point update error information
				if (errorMessage.Length > 0)
				{
					FileLogger.WriteError(errorMessage.ToString());
				}

				// ポイント付与タスクステータス更新（停止 or 完了）
				UpdateTaskStatusEnd(
					taskScheduleStatus,
					Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
					string.Format("{0}/{1}", addPointTotal, extractTotal));
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				Form1.WriteErrorLogLine(ex.ToString());
				SendDebugMail(ex.ToString());

				// ポイント付与ルールのステータス更新（エラーへ）
				try
				{
					var model = service.GetPointRuleSchedule(this.MasterId);
					model.Status = Constants.FLG_POINTRULESCHEDULE_STATUS_ERROR;
					model.LastCount = addPointTotal.ToString();
					model.LastExecDate = DateTime.Now;
					service.UpdatePointRuleScheduleStatus(model);
				}
				catch (Exception ex2)
				{
					FileLogger.WriteError(ex2);
					Form1.WriteErrorLogLine(ex2.ToString());
				}

				// ポイント付与タスクステータス更新（エラー）
				try
				{
					UpdateTaskStatusEnd(Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_ERROR, null, string.Format("{0}/{1}", addPointTotal, extractTotal));
				}
				catch (Exception ex2)
				{
					FileLogger.WriteError(ex2);
					Form1.WriteErrorLogLine(ex2.ToString());
				}
			}
		}

		/// <summary>
		/// スレッドストップチェック
		/// </summary>
		/// <param name="progress">進捗</param>
		private void CheckScheduleStoppedAndUpdateProgress(string progress)
		{
			// タスクスケジュール取得
			var taskSchedule = new TaskScheduleService().GetTaskScheduleAndUpdateProgress(this.DeptId, this.ActionKbn, this.MasterId, this.ActionNo, progress);

			if ((taskSchedule != null) && (taskSchedule.StopFlg == Constants.FLG_TASKSCHEDULE_STOP_FLG_ON))
			{
				throw new ScheduleStopException();
			}
		}

		/// <summary>
		/// ポイント付与ルール取得＆ステータス変更
		/// </summary>
		/// <param name="status">変更ステータス</param>
		/// <returns>ポイント付与ルール</returns>
		private PointRuleScheduleModel GetPointRuleSchedule(string status)
		{
			var service = new PointService();
			PointRuleScheduleModel result = null;
			while (true)
			{
				var tmpPointRuleSchedule = service.GetPointRuleSchedule(this.MasterId);

				if (tmpPointRuleSchedule != null)
				{
					switch (tmpPointRuleSchedule.Status)
					{
						// ステータスによらず、取得可能
						case Constants.FLG_POINTRULESCHEDULE_STATUS_NORMAL: // 通常
						case Constants.FLG_POINTRULESCHEDULE_STATUS_EXTRACT: // 抽出中
						case Constants.FLG_POINTRULESCHEDULE_STATUS_UPDATE: // 更新中
						case Constants.FLG_POINTRULESCHEDULE_STATUS_ERROR: // 更新エラー（エラーでも実行できる）
							tmpPointRuleSchedule.Status = status;
							service.UpdatePointRuleScheduleStatus(tmpPointRuleSchedule);
							result = service.GetPointRuleSchedule(this.MasterId);
							break;
					}
				}
				if (result != null) break;

				Thread.Sleep(500);

				// 停止要求チェック
				try
				{
					CheckScheduleStoppedAndUpdateProgress(null);
				}
				catch (ScheduleStopException)
				{
					// 通常ステータスへ
					tmpPointRuleSchedule.Status = Constants.FLG_POINTRULESCHEDULE_STATUS_NORMAL;
					service.UpdatePointRuleScheduleStatus(tmpPointRuleSchedule);

					// 付与ストップ
					UpdateTaskStatusEnd(Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP, null, "0/0");
				}
			}

			return result;
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="mailId">メールID</param>
		/// <param name="targetData">ターゲットデータ</param>
		private void SendMail(string mailId, TargetListDataModel targetData)
		{
			var mailDistText = new MailDistService().GetText(this.DeptId, mailId);
			if (mailDistText == null) return;

			// メール配信
			var mailSender = new SmtpMailSender();
			mailSender.SetFrom(mailDistText.MailFrom, mailDistText.MailFromName);
			mailSender.AddCC(mailDistText.MailCc);
			mailSender.AddBcc(mailDistText.MailBcc);
			if (Constants.ERROR_MAILADDRESS != "")
			{
				mailSender.SetReturnPath(Constants.ERROR_MAILADDRESS);
			}

			// メッセージ変換用
			var subjectTmp = mailDistText.MailtextSubject;
			var subjectMobileTmp = mailDistText.MailtextSubjectMobile;
			var bodyTmp = mailDistText.MailtextBody.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
			var bodyHtmlTmp = mailDistText.MailtextHtml.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
			var bodyMobileTmp = mailDistText.MailtextBodyMobile.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
			var bodyDecomeTmp = mailDistText.MailtextDecome.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", "\r\n");
			var tagSubjectTmp = Regex.Matches(subjectTmp, "<@@user:((?!@@>).)*@@>");
			var tagSubjectMobileTmp = Regex.Matches(subjectMobileTmp, "<@@user:((?!@@>).)*@@>");
			var tagBodyTmp = Regex.Matches(bodyTmp, "<@@user:((?!@@>).)*@@>");
			var tagBodyHtmlTmp = Regex.Matches(bodyHtmlTmp, "<@@user:((?!@@>).)*@@>");
			var tagBodyMobileTmp = Regex.Matches(bodyMobileTmp, "<@@user:((?!@@>).)*@@>");
			var tagBodyDecomeTmp = Regex.Matches(bodyDecomeTmp, "<@@user:((?!@@>).)*@@>");

			List<string> networkIOExceptUserId = new List<string>(); // SMTP接続エラーによりメール配信行えなかったユーザID

			var taskScheduleStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE;// デフォルトは完了へ

			// LINE配信設定
			var useLineDrect = App.Common.Line.Constants.LINE_DIRECT_OPTION_ENABLED
				&& (StringUtility.ToEmpty(mailDistText.LineUseFlg)
					== MailDistTextModel.LINE_USE_FLG_ON);
			var lineDistContents = new string[] { };
			if (useLineDrect)
			{
				var sv = new MessagingAppContentsService();
				var msgAppContents = sv.GetAllContentsEachMessagingAppKbn(
					this.DeptId,
					MessagingAppContentsModel.MASTER_KBN_MAILDISTTEXT,
					StringUtility.ToEmpty(mailDistText.MailtextId),
					MessagingAppContentsModel.MESSAGING_APP_KBN_LINE);
				lineDistContents = msgAppContents.Select(model => model.Contents).ToArray();
			}

			// 停止例外捕捉用try
			try
			{
				var mailSendResult = string.Empty;

				var addressKbn = targetData.MailAddrKbn;
				if (((addressKbn == Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_PC) && (bodyTmp.Length + bodyHtmlTmp.Length == 0))
					|| ((addressKbn == Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_MOBILE) && (bodyMobileTmp.Length == 0)))
				{
					// 文章無しエラー
					mailSendResult = Constants.FLG_TASKSCHEDULEHISTORY_ACTION_RESULT_MAILSEND_NOBODY;
					Form1.WriteWarningLogLine("メール配信文章に本文がないため送信スキップ");
				}
				else if ((addressKbn != Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_PC)
					&& (addressKbn != Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_MOBILE))
				{
					// メール送信区分エラー
					mailSendResult = Constants.FLG_TASKSCHEDULEHISTORY_ACTION_RESULT_MAILSEND_NOTARGET;
					Form1.WriteWarningLogLine("メール配信区分がPC、MBではないためスキップ");
				}
				else
				{
					// 後々のOneToOneを考えると一件ずつ配信した方がよい //

					var sbSubject = new StringBuilder();
					string strBody = null;
					string strBodyHtml = null;
					MatchCollection mcTagSubject = null;
					MatchCollection mcTagBody = null;
					MatchCollection mcTagBodyHtml = null;
					// メールアドレスかどうかチェック
					var mailAddr = targetData.MailAddr;
					if ((mailAddr.Length != 0) && (Validator.IsMailAddress(mailAddr) == false))
					{
						Form1.WriteWarningLogLine(
							"送信先メールアドレスは不正です。処理をスキップします。メールアドレス（" + mailAddr + "）はメールアドレスパターンにマッチしません。");
						FileLogger.WriteError(string.Format("ERROR: メールアドレス（{0}）はメールアドレスパターンにマッチしません。\r\n", mailAddr));

						return;
					}
					else
					{
						mailSender.ClearTo();
						mailSender.AddTo(mailAddr);
					}
					switch (addressKbn)
					{
						case Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_PC:
							sbSubject.Append(subjectTmp);
							strBody = bodyTmp;
							strBodyHtml = bodyHtmlTmp;
							mcTagSubject = tagSubjectTmp;
							mcTagBody = tagBodyTmp;
							mcTagBodyHtml = tagBodyHtmlTmp;
							break;

						case Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_MOBILE:
							sbSubject.Append(subjectMobileTmp);
							strBody = bodyMobileTmp;
							strBodyHtml = bodyDecomeTmp;
							mcTagSubject = tagSubjectMobileTmp;
							mcTagBody = tagBodyMobileTmp;
							mcTagBodyHtml = tagBodyDecomeTmp;
							break;
					}

					var body = new StringBuilder();
					var bodyHtml = new StringBuilder();
					if (strBody.Length != 0)
					{
						body.Append(strBody);
					}
					if (strBodyHtml.Length != 0)
					{
						bodyHtml.Append(strBodyHtml);
					}

					// メール文章タグ置換情報
					var mailTextReplaceTags = new Dictionary<string, string>();

					// サイト基本情報設定置換
					sbSubject = ShopMessageUtil.ConvertShopMessage(
						sbSubject,
						targetData.DispLanguageCode,
						targetData.DispLanguageLocaleId,
						false);
					body = ShopMessageUtil.ConvertShopMessage(
						body,
						targetData.DispLanguageCode,
						targetData.DispLanguageLocaleId,
						false);
					bodyHtml = ShopMessageUtil.ConvertShopMessage(
						bodyHtml,
						targetData.DispLanguageCode,
						targetData.DispLanguageLocaleId,
						true);

					var user = new UserForMailSend();

					// ユーザ情報変換
					if (mcTagSubject.Count + mcTagBody.Count + mcTagBodyHtml.Count > 0)
					{
						user = new UserService().GetUserForMailSend(targetData.UserId);
						SetReplaceTags(mailTextReplaceTags, user, sbSubject, mcTagSubject);
						SetReplaceTags(mailTextReplaceTags, user, body, mcTagBody);
						SetReplaceTags(mailTextReplaceTags, user, bodyHtml, mcTagBodyHtml);
					}

					// メール登録解約リンク変換
					SetReplaceMailUnsubscribeTags(user, sbSubject);
					SetReplaceMailUnsubscribeTags(user, body);
					SetReplaceMailUnsubscribeTags(user, bodyHtml);

					var compressedReplaceTags = string.Empty;
					if (mailTextReplaceTags.Count > 0)
					{
						// タグ置換情報をJSON型に変換
						var replaceTags = JsonConvert.SerializeObject(mailTextReplaceTags);
						// タグ置換情報を圧縮する
						compressedReplaceTags = StringUtility.CompressString(replaceTags);
					}

					// 端末別設定
					if (addressKbn == Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_MOBILE)
					{
						// モバイル用にエンコーディングリセット
						mailSender.SetEncoding(Constants.MOBILE_MAIL_ENCODING, Constants.MOBILE_MAIL_TRANSFER_ENCODING);
					}
					else
					{
						// PC用にリセット
						mailSender.SetEncoding(
							Constants.PC_MAIL_DEFAULT_ENCODING,
							Constants.PC_MAIL_DEFAULT_TRANSFER_ENCODING);
						mailSender.Message.MultipartRelatedEnable = false;
					}

					// メールセンダーにセット
					mailSender.SetSubject(sbSubject.ToString());
					mailSender.SetBody(body.ToString());
					mailSender.SetBodyHtml(bodyHtml.ToString());

					// メール送信
					if (mailSender.SendMail((string)user.DataSource[Constants.FIELD_USER_USER_ID], (string)user.DataSource[Constants.FIELD_USER_MAIL_ADDR]))
					{
						mailSendResult = Constants.FLG_TASKSCHEDULEHISTORY_ACTION_RESULT_MAILSEND_OK;
					}
					else
					{
						mailSendResult = Constants.FLG_TASKSCHEDULEHISTORY_ACTION_RESULT_MAILSEND_NG;

						Form1.WriteInfoLogLine(mailSender.MailSendException.ToString());
					}

					// CSオプションが有効の場合はメール送信ログ登録
					if (Constants.CS_OPTION_ENABLED)
					{
						// 非同期実行時に送信日時を取得したくないのでここに記述
						var dateSendMail = DateTime.Now;

						// 配信時のメール配信文章を登録し、登録時に履歴NOを取得する
						var textHisoryNo = 0;
						textHisoryNo = InsertAndGetTextHistoryNo(mailDistText, "ポイント付与", compressedReplaceTags);

						var service = new MailSendLogService();
						var model = new MailSendLogModel
						{
							UserId = targetData.UserId,
							DeptId = mailDistText.DeptId,
							MailtextId = mailDistText.MailtextId,
							MailtextName = mailDistText.MailtextName,
							MaildistId = "",
							MaildistName = "",
							ShopId = string.Empty,
							MailId = string.Empty,
							MailName = string.Empty,
							MailFromName = mailSender.Message.From.DisplayName,
							MailFrom = mailSender.Message.From.Address,
							MailTo = string.Join(",", mailSender.Message.To.Select(m => m.Address)),
							MailCc = string.Join(",", mailSender.Message.CC.Select(m => m.Address)),
							MailBcc = string.Join(",", mailSender.Message.Bcc.Select(m => m.Address)),
							MailSubject = mailSender.Subject,
							// メール本文は「メール配信時文章履歴」テーブルに保存するため、空を設定
							MailBody = string.Empty,
							// メール本文は「メール配信時文章履歴」テーブルに保存するため、空を設定
							MailBodyHtml = string.Empty,
							ErrorMessage
								=(mailSender.MailSendException != null)
									? mailSender.MailSendException.ToString()
									: string.Empty,
							DateSendMail = dateSendMail,
							ReadFlg = Constants.FLG_MAILSENDLOG_READ_FLG_UNREAD,
							DateReadMail = (DateTime?)null,
							TextHistoryNo = textHisoryNo,
							MailAddrKbn = addressKbn
						};
						service.Insert(model);
					}

					// LINE送信
					if (useLineDrect)
					{
						// サイト基本情報設定置換
						lineDistContents = lineDistContents.Select(
						text => ShopMessageUtil.ConvertShopMessage(
							text,
							targetData.DispLanguageCode,
							targetData.DispLanguageLocaleId,
							false)).ToArray();

						var lineTextMatchArray = lineDistContents
							.Select(text => Regex.Matches(text, "<@@user:((?!@@>).)*@@>")).ToArray();
						var lineMessagesTemp = lineDistContents
							.Select(text => new LineMessageText { MessageText = text }).ToArray();
						var isMatch = lineTextMatchArray.Any(match => (match.Count > 0));
						if (isMatch)
						{
							if (user.DataSource == null)
							{
								user = new UserService().GetUserForMailSend(targetData.UserId);
							}

							for (var i = 0; i < lineTextMatchArray.Length; i++)
							{
								foreach (Match mt in lineTextMatchArray[i])
								{
									var strKey = mt.Value.Replace("<@@user:", string.Empty)
										.Replace("@@>", string.Empty);
									var strValue = (user != null)
										? StringUtility.ToEmpty(ConvertMailDistTextUserInfo(user, strKey))
										: string.Empty;
									lineMessagesTemp[i].MessageText
										= lineMessagesTemp[i].MessageText.Replace(mt.Value, strValue);
								}
							}
						}

						var userId = targetData.UserId;
						var extender = new UserService().GetUserExtend(userId);
						var lineId = extender.UserExtendDataValue[Constants.SOCIAL_PROVIDER_ID_LINE];
						if (string.IsNullOrEmpty(lineId) == false)
						{
							// LINE送信実行
							var response = new LineDirectConnectManager().SendPushMessage(
								lineId,
								lineMessagesTemp,
								userId);
							if (response != null)
							{
								var lineSendResult = string.Format(
									"【{0}】{1}",
									response.Message,
									((response.Details.Length > 0)
										&& (string.IsNullOrEmpty(response.Details[0].Message) == false))
										? string.Format(
											"{0}　- {1}",
											Environment.NewLine,
											string.Join(
												Environment.NewLine,
												response.Details.Select(x => x.Message).ToArray()))
										: string.Empty);
								Form1.WriteInfoLogLine(lineSendResult);
							}
						}
					}
				}

				// メール配信結果を随時ログ出力
				SendLoggingResult(targetData, mailSendResult);

				// 少し休む
				Thread.Sleep(5);
			}
			catch (ScheduleStopException)
			{
				Form1.WriteInfoLogLine("■メール配信停止要求あり■");
			}
		}

		/// <summary>
		/// メール配信時文章履歴登録(登録後、履歴NO取得)
		/// </summary>
		/// <param name="mailDistText">メール配信文章</param>
		/// <param name="mailDistName">メール配信設定名</param>
		/// <param name="compressedReplaceTags">圧縮したタグ置換情報</param>
		/// <returns>登録したメール配信時文章履歴NO</returns>
		private int InsertAndGetTextHistoryNo(MailDistTextModel mailDistText, string mailDistName, string compressedReplaceTags)
		{
			var model = new MailSendTextHistoryModel
			{
				DeptId = this.DeptId,
				MailtextId = mailDistText.MailtextId,
				MaildistId = this.MasterId,
				MaildistName = mailDistName,
				MailBody = mailDistText.MailtextBody,
				MailBodyHtml = mailDistText.MailtextHtml,
				MailtextBodyMobile = mailDistText.MailtextBodyMobile,
				MailtextDecome = mailDistText.MailtextDecome,
				DateCreated = DateTime.Now,
				MailtextReplaceTags = compressedReplaceTags
			};
			var textHistoryNo = new MailSendLogService().InsertTextHistoryAndGetTextHistoryNo(model);
			return textHistoryNo;
		}
	}
}
