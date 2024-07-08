/*
=========================================================================================================
  Module      : クーポン発行スレッドクラス(PublishCouponThread.cs)
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
using w2.App.Common.ShopMessage;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Common.Net.Mail;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;
using w2.Domain.Coupon;
using w2.Domain.Coupon.Helper;
using w2.Domain.MailSendLog;
using w2.Domain.MailDist;
using w2.Domain.TaskSchedule;
using w2.Domain.TargetList;
using System.Data;
using System.Collections;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	class PublishCouponThread : BaseThread
	{
		/// <summary>送信結果一覧</summary>
		private List<string> _mailSendResults = new List<string>();

		/// <summary>
		/// スレッド作成（タスクスケジュール実行）
		/// </summary>
		/// <param name="scheduleDate">スケジュール日付</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponRuleScheduleId">ポイントルールスケジュールID</param>
		/// <param name="actionNo">アクションNO</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>生成スレッド</returns>
		public static PublishCouponThread CreateAndStart(
			DateTime scheduleDate,
			string deptId,
			string couponRuleScheduleId,
			int actionNo,
			string lastChanged)
		{
			// スレッド作成
			var publishCouponThread = new PublishCouponThread(
				scheduleDate,
				deptId,
				couponRuleScheduleId,
				actionNo,
				lastChanged);
			publishCouponThread.Thread = new Thread(publishCouponThread.Work);

			// スレッドスタート
			publishCouponThread.Thread.Start();

			return publishCouponThread;
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="scheduleDate">スケジュール日付</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="couponRuleScheduleId">クーポンスケジュールID</param>
		/// <param name="actionNo">アクションNO</param>
		/// <param name="lastChangedTaskSchedule">Last changed task schedule</param>
		public PublishCouponThread(
			DateTime scheduleDate,
			string deptId,
			string couponRuleScheduleId,
			int actionNo,
			string lastChangedTaskSchedule)
				: base(scheduleDate, deptId, Constants.FLG_TASKSCHEDULE_ACTION_KBN_PUBLISH_COUPON, couponRuleScheduleId, actionNo)
		{
			this.LastChangedTaskSchedule = lastChangedTaskSchedule;
		}

		/// <summary>
		/// クーポン発行
		/// </summary>
		public void Work()
		{
			long extractTotal = 0; // 抽出人数
			long publishCouponTotal = 0; // 付与人数

			try
			{
				// クーポン発行タスクステータス更新(開始)
				var updateStatus = UpdateTaskStatusBegin(Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE, Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT, "");

				// 処理開始宣言
				Form1.WriteInfoLogLine("クーポン発行[" + this.MasterId + "]発行開始");

				// クーポン発行ルール取得（同時にステータス更新、取得できるまで待つ）
				var couponSchedule = GetCouponSchedule(Constants.FLG_COUPONSCHEDULE_STATUS_UPDATE);
				var coupon = new CouponService().GetPublishCouponsById(this.DeptId, couponSchedule.CouponId);

				// クーポン設定無効
				if (coupon == null)
				{
					// クーポン発行ルールのステータス更新（通常状態へ）
					couponSchedule.Status = Constants.FLG_COUPONSCHEDULE_STATUS_NORMAL;
					new CouponService().UpdateCouponScheduleStatus(couponSchedule);

					// 処理終了宣言
					Form1.WriteInfoLogLine(
						string.Format(
							"クーポン発行[{0}]付与完了：{1}は無効です。",
							this.MasterId,
							couponSchedule.CouponId));

					// クーポン発行タスクステータス更新（完了）
					UpdateTaskStatusEnd(Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE, Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE, "");

					return;
				}

				// ターゲット抽出ステータス更新（「実行中」へ）
				UpdatePrepareTaskStatus(Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE, Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_INIT);

				// 実行タイミングを問わずターゲット抽出フラグがONの場合は、ターゲットを取得するために、クーポン発行ユーザー抽出スレッドを作成
				if (couponSchedule.TargetExtractFlg == Constants.FLG_COUPONSCHEDULE_TARGET_EXTRACT_FLG_ON)
				{
					var targetExtractThread = TargetExtractThread.CreateAndStart(Constants.CONST_DEFAULT_DEPT_ID, couponSchedule.TargetId);
					while (targetExtractThread.Thread.IsAlive)
					{
						Thread.Sleep(50);
					}
				}
				// ターゲット抽出ステータス更新（「実行完了」へ）
				UpdatePrepareTaskStatus(Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_DONE, Constants.FLG_TASKSCHEDULE_PREPARE_STATUS_EXECUTE);

				// 抽出済ターゲットデータ取得
				lock (GetLockObject(this.DeptId, this.ActionKbn, this.MasterId))
				{
					TargetListDataModel[] targetListData;
					using (var sqlAccessor = new SqlAccessor())
					{
						targetListData = new TargetListService().GetTargetListDataDeduplicated(this.DeptId, Constants.FLG_TARGETLISTDATA_TARGET_KBN_TARGETLIST, couponSchedule.TargetId, sqlAccessor);
						extractTotal = targetListData.Length;
					}

					// クーポン発行
					var taskScheduleStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE;	// デフォルトは完了へ

					try // 停止例外捕捉用try
					{
						// クーポン発行ループ開始
						foreach (var targetData in targetListData)
						{
							// スケジュール停止フラグチェック＆進捗更新（とりあえず10件ずつチェックする）
							if ((publishCouponTotal % 10) == 0)
							{
								CheckScheduleStoppedAndUpdateProgress(string.Format("{0}/{1}", publishCouponTotal, extractTotal));
							}

							// 後々のOneToOneを考えると一件ずつ付与した方がよい //

							// クーポン発行情報取得＆設定
							var userId = targetData.UserId;
							var mailId = couponSchedule.MailId;
							var deptId = targetData.DeptId;

							var user = new UserService().Get(userId);

							var execByManual = string.IsNullOrEmpty(couponSchedule.ScheduleKbn);
							this.LastChangedTaskSchedule = execByManual ? this.LastChangedTaskSchedule : Constants.FLG_LASTCHANGED_BATCH;

							using (var accessor = new SqlAccessor())
							{
								// トランザクション
								accessor.OpenConnection();
								accessor.BeginTransaction();
								var couponService = new CouponService();

								var publishCouponInfo = new UserCouponDetailInfo
								{
									OrderId = string.Empty,
									UserId = userId,
									DeptId = deptId,
									CouponId = coupon.CouponId,
									CouponCode = coupon.CouponCode,
									DiscountPrice = coupon.DiscountPrice,
									LastChanged = this.LastChangedTaskSchedule,
									UserCouponCount = couponSchedule.PublishQuantity
								};

								couponService.InsertUserCoupon(publishCouponInfo, UpdateHistoryAction.Insert, accessor, false);

								// 誕生日クーポンの場合は誕生日クーポン発行年を更新
								if (coupon.CouponType == Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FOR_REGISTERED_USER
									|| coupon.CouponType == Constants.FLG_COUPONCOUPON_TYPE_LIMITED_BIRTHDAY_FREESHIPPING_FOR_REGISTERED_USER)
								{
									user.LastBirthdayCouponPublishYear = DateTime.Now.Year.ToString();
									new UserService().UpdateUserBirthdayCouponPublishYear(
										user,
										UpdateHistoryAction.Insert,
										accessor,
										this.LastChangedTaskSchedule);
								}

								// コミット
								accessor.CommitTransaction();
							}

							// クーポン発行人数をインクリメント
							publishCouponTotal++;

							// メール送信
							if (mailId != string.Empty)
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
						Form1.WriteInfoLogLine("■クーポン発行停止要求あり■");

						taskScheduleStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_STOP;	// 「停止中」へ
						SendLoggingResult("STOP_EOF\r\n");
					}

					couponSchedule.Status = Constants.FLG_COUPONSCHEDULE_STATUS_NORMAL;
					couponSchedule.LastCount = publishCouponTotal.ToString();
					couponSchedule.LastExecDate = DateTime.Now;
					new CouponService().UpdateCouponSchedule(couponSchedule);

					// 処理終了宣言
					Form1.WriteInfoLogLine("クーポン発行[" + this.MasterId + "]発行完了：" + string.Format("{0}/{1}", publishCouponTotal, extractTotal) + "件");

					// クーポン発行タスクステータス更新（停止 or 完了）
					updateStatus = UpdateTaskStatusEnd(
						taskScheduleStatus,
						Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_EXECUTE,
						string.Format("{0}/{1}", publishCouponTotal, extractTotal));

					InsertTaskScheduleHistory(
						couponSchedule.TargetId,
						_mailSendResults,
						targetListData);
				}
			}
			catch (Exception ex)
			{
				FileLogger.WriteError(ex);
				Form1.WriteErrorLogLine(ex.ToString());
				SendDebugMail(ex.ToString());

				// クーポン発行ルールのステータス更新（エラーへ）
				try
				{
					// 内部処理でステータスを「更新エラー」に更新
					GetCouponSchedule(Constants.FLG_COUPONSCHEDULE_STATUS_NORMAL);
				}
				catch (Exception ex2)
				{
					FileLogger.WriteError(ex2);
					Form1.WriteErrorLogLine(ex2.ToString());
				}

				// クーポン発行タスクステータス更新（エラー）
				try
				{
					UpdateTaskStatusEnd(Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_ERROR, null, string.Format("{0}/{1}", publishCouponTotal, extractTotal));
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
		/// クーポン発行ルール取得＆ステータス変更
		/// </summary>
		/// <param name="status">変更ステータス</param>
		/// <returns>クーポン発行ルール</returns>
		private CouponScheduleModel GetCouponSchedule(string status)
		{
			var service = new CouponService();
			CouponScheduleModel result = null;
			while (true)
			{
				var tmpCouponSchedule = service.GetCouponSchedule(this.MasterId);

				if (tmpCouponSchedule != null)
				{
					switch (tmpCouponSchedule.Status)
					{
						// ステータスによらず、取得可能
						case Constants.FLG_COUPONSCHEDULE_STATUS_NORMAL: // 通常
						case Constants.FLG_COUPONSCHEDULE_STATUS_EXTRACT: // 抽出中
						case Constants.FLG_COUPONSCHEDULE_STATUS_UPDATE: // 更新中
						case Constants.FLG_COUPONSCHEDULE_STATUS_ERROR: // 更新エラー（エラーでも実行できる）
							tmpCouponSchedule.Status = status;
							service.UpdateCouponScheduleStatus(tmpCouponSchedule);
							result = service.GetCouponSchedule(this.MasterId);
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
					tmpCouponSchedule.Status = Constants.FLG_COUPONSCHEDULE_STATUS_NORMAL;
					service.UpdateCouponScheduleStatus(tmpCouponSchedule);

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
			if (Constants.ERROR_MAILADDRESS != string.Empty)
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

			var networkIOExceptUserId = new List<string>(); // SMTP接続エラーによりメール配信行えなかったユーザID

			var taskScheduleStatus = Constants.FLG_TASKSCHEDULE_EXECUTE_STATUS_DONE;	// デフォルトは完了へ
			try // 停止例外捕捉用try
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
					var body = string.Empty;
					var bodyHtml = string.Empty;
					MatchCollection mcTagSubject = null;
					MatchCollection mcTagBody = null;
					MatchCollection mcTagBodyHtml = null;
					// メールアドレスかどうかチェック
					var mailAddr = targetData.MailAddr;
					if ((mailAddr.Length != 0) && (Validator.IsMailAddress(mailAddr) == false))
					{
						Form1.WriteWarningLogLine(string.Format("送信先メールアドレスは不正です。処理をスキップします。メールアドレス（{0}）はメールアドレスパターンにマッチしません。", mailAddr));
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
							body = bodyTmp;
							bodyHtml = bodyHtmlTmp;
							mcTagSubject = tagSubjectTmp;
							mcTagBody = tagBodyTmp;
							mcTagBodyHtml = tagBodyHtmlTmp;
							break;

						case Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_MOBILE:
							sbSubject.Append(subjectMobileTmp);
							body = bodyMobileTmp;
							bodyHtml = bodyDecomeTmp;
							mcTagSubject = tagSubjectMobileTmp;
							mcTagBody = tagBodyMobileTmp;
							mcTagBodyHtml = tagBodyDecomeTmp;
							break;
					}
					var dvMailClick = GetMailInfometion(mailDistText.MailtextId);
					var sbBody = new StringBuilder();
					var sbBodyHtml = new StringBuilder();

					// メールクリック変換
					var isPc = (addressKbn == Constants.FLG_TARGETLISTDATA_MAIL_ADDR_KBN_PC);
					if (string.IsNullOrEmpty(body) == false)
					{
						sbBody.Append(
							ConvertMailClickUrl(
								body,
								dvMailClick,
								targetData.UserId,
								false,
								(isPc)));
					}
					if (string.IsNullOrEmpty(bodyHtml) == false)
					{
						sbBodyHtml.Append(
							ConvertMailClickUrl(
								bodyHtml,
								dvMailClick,
								targetData.UserId,
								true,
								(isPc)));
					}

					// メール文章タグ置換情報
					var mailTextReplaceTags = new Dictionary<string, string>();

					// サイト基本情報設定置換
					sbSubject = ShopMessageUtil.ConvertShopMessage(sbSubject, targetData.DispLanguageCode, targetData.DispLanguageLocaleId, false);
					sbBody = ShopMessageUtil.ConvertShopMessage(sbBody, targetData.DispLanguageCode, targetData.DispLanguageLocaleId, false);
					sbBodyHtml = ShopMessageUtil.ConvertShopMessage(sbBodyHtml, targetData.DispLanguageCode, targetData.DispLanguageLocaleId, true);

					var user = new UserService().GetUserForMailSend(targetData.UserId);

					// ユーザ情報変換
					if (mcTagSubject.Count + mcTagBody.Count + mcTagBodyHtml.Count > 0)
					{
						SetReplaceTags(mailTextReplaceTags, user, sbSubject, mcTagSubject);
						SetReplaceTags(mailTextReplaceTags, user, sbBody, mcTagBody);
						SetReplaceTags(mailTextReplaceTags, user, sbBodyHtml, mcTagBodyHtml);
					}

					// メール登録解約リンク変換
					SetReplaceMailUnsubscribeTags(user, sbSubject);
					SetReplaceMailUnsubscribeTags(user, sbBody);
					SetReplaceMailUnsubscribeTags(user, sbBodyHtml);

					// メールクリック情報登録
					InsertMailInfometion(mailDistText.MailtextId, dvMailClick);

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
						mailSender.SetEncoding(Constants.PC_MAIL_DEFAULT_ENCODING, Constants.PC_MAIL_DEFAULT_TRANSFER_ENCODING);
						mailSender.Message.MultipartRelatedEnable = false;
					}

					// メールセンダーにセット
					mailSender.SetSubject(sbSubject.ToString());
					mailSender.SetBody(sbBody.ToString());
					mailSender.SetBodyHtml(sbBodyHtml.ToString());

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
						textHisoryNo = InsertAndGetTextHistoryNo(mailDistText, "クーポン発行", compressedReplaceTags);

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
							MailBody = string.Empty,	// メール本文は「メール配信時文章履歴」テーブルに保存するため、空を設定
							MailBodyHtml = string.Empty,	// メール本文は「メール配信時文章履歴」テーブルに保存するため、空を設定
							ErrorMessage = (mailSender.MailSendException != null) ? mailSender.MailSendException.ToString() : string.Empty,
							DateSendMail = dateSendMail,
							ReadFlg = Constants.FLG_MAILSENDLOG_READ_FLG_UNREAD,
							DateReadMail = (DateTime?)null,
							TextHistoryNo = textHisoryNo,
							MailAddrKbn = addressKbn
						};
						service.Insert(model);
					}
				}

				// メール配信結果を随時ログ出力
				SendLoggingResult(targetData, mailSendResult);

				_mailSendResults.Add(mailSendResult);

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

		/// <summary>
		/// メールクリック情報取得
		/// </summary>
		/// <param name="mailTextId">メール文書ID</param>
		/// <returns>メールクリック情報</returns>
		private DataView GetMailInfometion(string mailTextId)
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("MailClick", "GetMailClick"))
			{

				var input = new Hashtable
				{
					{ Constants.FIELD_MAILCLICK_DEPT_ID, this.DeptId },
					{ Constants.FIELD_MAILCLICK_MAILTEXT_ID, mailTextId },
					{ Constants.FIELD_MAILCLICK_MAILDIST_ID, string.Empty },
					{ Constants.FIELD_MAILCLICK_ACTION_NO, 0 },
				};

				var mailClick = sqlStatement.SelectSingleStatementWithOC(sqlAccessor, input);
				return mailClick;
			}
		}

		/// <summary>
		/// メール配信結果格納
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <param name="mailSendList">送信結果情報</param>
		/// <param name="targetListData">ターゲット取得</param>
		private void InsertTaskScheduleHistory(string targetId, List<string> mailSendList, TargetListDataModel[] targetListData)
		{
			using (var sqlAccessor = new SqlAccessor())
			using (var sqlStatement = new SqlStatement("TaskScheduleHistory", "InsertTaskScheduleHistory"))
			{
				var htInput = new Hashtable
				{
					{ Constants.FIELD_TASKSCHEDULEHISTORY_DEPT_ID, this.DeptId },
					{ Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_KBN, Constants.FLG_TASKSCHEDULEHISTORY_ACTION_KBN_PUBLISH_COUPON },
					{ Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_MASTER_ID, this.MasterId },
					{ Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_NO, this.ActionNo },
					{ Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_STEP, 1 },
					{ Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_KBN_DETAIL, Constants.FLG_TASKSCHEDULEHISTORY_ACTION_KBN_DETAIL_MAIL_DIST },
					{ Constants.FIELD_TASKSCHEDULEHISTORY_TARGET_ID, targetId }
				};
				for (var index = 0; index < mailSendList.Count; index++)
				{
					htInput[Constants.FIELD_TASKSCHEDULEHISTORY_ACTION_RESULT] = mailSendList[index];
					htInput[Constants.FIELD_TASKSCHEDULEHISTORY_USER_ID] = targetListData[index].UserId;
					htInput[Constants.FIELD_TASKSCHEDULEHISTORY_MAIL_ADDR] = targetListData[index].MailAddr;

					sqlStatement.ExecStatementWithOC(sqlAccessor, htInput);
				}
			}
		}

		/// <summary>Last changed task schedule</summary>
		private string LastChangedTaskSchedule { get; set; }
	}
}
