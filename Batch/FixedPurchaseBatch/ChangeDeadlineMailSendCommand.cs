/*
=========================================================================================================
  Module      : 定期購入変更期限案内メール送信クラス(ChangeDeadlineMailSendCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.IO;
using w2.App.Common.Mail;
using w2.App.Common.Order.FixedPurchase;
using w2.App.Common.SendMail;
using w2.Common.Logger;
using w2.Domain.FixedPurchase;
using w2.Domain.MailTemplate;

namespace w2.Commerce.Batch.FixedPurchaseBatch
{
	/// <summary>
	/// 定期購入変更期限案内メール送信クラス
	/// </summary>
	class ChangeDeadlineMailSendCommand
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="fixedPurchaseMailSendTiming">メールの送信タイミング</param>
		public ChangeDeadlineMailSendCommand(FixedPurchaseMailSendTiming fixedPurchaseMailSendTiming)
		{
			this.FixedPurchaseMailSendTiming = fixedPurchaseMailSendTiming;
		}

		/// <summary>
		/// 定期購入変更期限案内メール送信処理
		/// </summary>
		internal void Exec()
		{
			try
			{
				// 最終実行日とメールテンプレートの判定
				if ((IsExcutable() == false) || (IsMailAutoSend() == false)) return;

				// メール送信時間を設定する。メール送信可能時間外であれば、スケジュール登録する。
				this.FixedPurchaseMailSendTiming.SettingTaskSchedule(Constants.FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_CHANGE_DEADLINE_MAIL);

				// 有効な全定期購入に対して変更期限案内メールを送信する
				var fixedPurchaseService = new FixedPurchaseService();
				var fixedPurchaseList = fixedPurchaseService.GetTargetsForSendChangeDeadlineMail();
				foreach (var fixedPurchase in fixedPurchaseList)
				{
					try
					{
						// ユーザーが存在しない or 退会済みの場合は定期購入情報を無効化して、メールを送信しない
						if (FixedPurchaseHelper.CheckFixedPurchaseIsInvalidAndInvalidateFixedPurchase(fixedPurchase, Constants.FLG_LASTCHANGED_BATCH)) continue;

						if (fixedPurchase.NextShippingDate.HasValue == false)
						{
							throw new ApplicationException("次回配送日が設定されていません。");
						}

						// 定期購入変更案内メールの送信日付を設定する（次回配信日 - 配送キャンセル期限 - 定期便変更案内メール送信日）
						var sendMailDate = fixedPurchase.NextShippingDate.Value
							.AddDays(fixedPurchase.CancelDeadline.Value * -1)
							.AddDays(Constants.NEXT_FIXED_PURCHASE_CHANGE_DEADLINE_SEND_DATE * -1);

						// 現在日が定期購入変更案内メールの送信日付でなければメールを送信しない
						if (DateTime.Today.Date != sendMailDate.Date) continue;

						// 定期購入変更期限案内メール送信を送信する
						SendChangeDeadlineMail(fixedPurchase.FixedPurchaseId, fixedPurchase.DispLanguageCode, fixedPurchase.DispLanguageLocaleId);
					}
					catch (Exception ex)
					{
						var logMessage = string.Format("定期購入変更案内メールの送信に失敗しました。(定期購入ID : {0})", fixedPurchase.FixedPurchaseId);
						AppLogger.WriteInfo(logMessage, ex);
					}
				}

			}
			catch (Exception ex)
			{
				AppLogger.WriteInfo("有効な定期購入の取得に失敗しました。", ex);
			}
		}

		/// <summary>
		/// 最終実行日判定
		/// </summary>
		/// <returns>最終実効日が今日ではない</returns>
		private bool IsExcutable()
		{
			if (LastExecDate.GetLastExecDate() == DateTime.Today.Date)
			{
				AppLogger.WriteInfo("本日のバッチ実行が2回目以降であるため、処理を終了します。");
				return false;
			}
			return true;
		}

		/// <summary>
		/// メールテンプレートの自動送信フラグ判定
		/// </summary>
		/// <returns>自動送信ON</returns>
		private bool IsMailAutoSend()
		{
			var mailTemplate = new MailTemplateService().Get(Constants.CONST_DEFAULT_SHOP_ID, Constants.SEND_CHANGE_DEADLINE_MAIL_TEMPLATE_ID);
			if ((mailTemplate == null) || (mailTemplate.AutoSendFlgCheck == false))
			{
				AppLogger.Write("mail", "自動送信フラグが「送信しない」のため、定期購入変更期限案内メールを送信しません。");
				return false;
			}
			return true;
		}

		/// <summary>
		/// 定期購入変更期限案内メール送信を送信する
		/// </summary>
		/// <param name="fixedPurchaseId">定期購入ID</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		private void SendChangeDeadlineMail(string fixedPurchaseId, string languageCode = null, string languageLocaleId = null)
		{
			// メール用定期購入情報を取得する
			var mailData = new MailTemplateDataCreaterForFixedPurchase(true).GetFixedPurchaseMailDatas(fixedPurchaseId);
			mailData[Constants.FIELD_USER_MAIL_ADDR] = mailData[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR];
			mailData[Constants.FIELD_USER_MAIL_ADDR2] = mailData[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2];

			var isPc = (string.IsNullOrEmpty((string)mailData[Constants.FIELD_USER_MAIL_ADDR]) == false);

			// メールを送信できない時間帯なら定期購入者に案内メールを送信する
			if (this.FixedPurchaseMailSendTiming.TimeZoneStatus == FixedPurchaseMailSendTiming.TimeZoneStatusEnum.Ok)
			{
				SendMailCommon.SendMailToUser(
					Constants.SEND_CHANGE_DEADLINE_MAIL_TEMPLATE_ID,
					(string)mailData[Constants.FIELD_USER_USER_ID],
					mailData,
					isPc,
					languageCode,
					languageLocaleId);
			}
			else
			{
				this.FixedPurchaseMailSendTiming.InsertFixedPurchaseBatchMailTmpLog(
					(string)mailData[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID],
					Constants.FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_DEADLINE);
			}
		}

		/// <summary>定期注文メール送信タイミング管理</summary>
		public FixedPurchaseMailSendTiming FixedPurchaseMailSendTiming { get; set; }
	}
}
