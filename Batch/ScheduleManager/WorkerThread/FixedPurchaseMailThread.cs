/*
=========================================================================================================
  Module      : 定期購入メール送信スレッドクラス (FixedPurchaseMailThread.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using w2.Common.Logger;
using w2.Common.Util;
using w2.Domain.FixedPurchase;
using w2.Domain.TaskSchedule;
using w2.App.Common;
using w2.App.Common.Mail;
using w2.Domain.Order;

namespace w2.MarketingPlanner.Win.ScheduleManager.WorkerThread
{
	/// <summary>
	/// 定期購入メール送信スレッドクラス
	/// </summary>
	class FixedPurchaseMailThread : BaseThread
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="scheduleDate">スケジュール日付</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">定期購入メール実行ID</param>
		/// <param name="actionNo">アクションNO</param>
		public FixedPurchaseMailThread(DateTime scheduleDate, string deptId, string masterId, int actionNo)
			: base(scheduleDate, deptId, Constants.FLG_TACKSCHEDULE_ACTION_KBN_FIXED_PURCHASE_MAIL, masterId, actionNo)
		{
		}

		/// <summary>
		/// スレッド作成（タスクスケジュール実行）
		/// </summary>
		/// <param name="scheduleDate">スケジュール日付</param>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">定期購入メール実行ID</param>
		/// <param name="actionNo">アクションNO</param>
		/// <returns>生成スレッド</returns>
		public static FixedPurchaseMailThread CreateAndStart(DateTime scheduleDate, string deptId, string masterId, int actionNo)
		{
			// スレッド作成
			var fixedPurchaseMaiThread = new FixedPurchaseMailThread(scheduleDate, deptId, masterId, actionNo);
			fixedPurchaseMaiThread.Thread = new Thread(new ThreadStart(fixedPurchaseMaiThread.Work));

			// スレッドスタート
			fixedPurchaseMaiThread.Thread.Start();

			return fixedPurchaseMaiThread;
		}

		/// <summary>
		/// 定期購入メール送信スレッド
		/// </summary>
		public void Work()
		{
			Form1.WriteInfoLogLine("定期バッチメール送信を開始 : ID :" + this.MasterId);
			Form1.WriteInfoLogLine("定期バッチメール送信を開始 ");

			var fixedPurchaseBatchMailTmpLogModels =
				new FixedPurchaseService().SearchFixedPurchaseBatchMailTmpLogs(this.MasterId)
					.Where(m => (m.MasterType == Constants.FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_ORDER)
						|| (m.MasterType == Constants.FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_ERROR))
					.ToArray();

			//定期購入メール実行ID(action_master_id)に紐づくメールを順次送信
			foreach (var fixedPurchaseBatchMailTmpLogModel in fixedPurchaseBatchMailTmpLogModels)
			{
				try
				{
					SendMail(fixedPurchaseBatchMailTmpLogModel.MasterId, fixedPurchaseBatchMailTmpLogModel.MasterType);
				}
				catch (Exception ex)
				{
					FileLogger.WriteError("タスクスケジューラによる定期バッチメールの送信に失敗しました。master_id : " + fixedPurchaseBatchMailTmpLogModel.MasterId, ex);
				}
				finally
				{
					new FixedPurchaseService().DeleteFixedPurchaseBatchMailTmpLog(fixedPurchaseBatchMailTmpLogModel.TmpLogId);
				}
			}

			//全処理が完了した後、実行したタスクスケジュールを削除
			new TaskScheduleService().Delete(this.DeptId, this.ActionKbn, this.MasterId, this.ActionNo);

			Form1.WriteInfoLogLine("定期バッチメール送信を終了 ");
		}

		/// <summary>
		/// メール送信
		/// </summary>
		/// <param name="masterId">注文が成功したメールは"order_id", 注文が失敗したメールは"fixed_purchase_id"</param>
		/// <param name="masterType">注文が成功したメールは"order",注文が失敗したメールは"error"</param>
		private void SendMail(string masterId, string masterType)
		{
			var dataSendMail = (masterType == Constants.FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_ORDER) ?
				new MailTemplateDataCreaterForOrder(true).GetOrderMailDatas(masterId) :
				new MailTemplateDataCreaterForFixedPurchase(true).GetFixedPurchaseMailDatas(masterId);

			//送信先のメールアドレスを確定
			var mailAddr = (string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR];
			var mailAddr2 = (string)dataSendMail[Constants.FIELD_ORDEROWNER_OWNER_MAIL_ADDR2];
			var sendMailPc = (string.IsNullOrEmpty(mailAddr) == false)
				&& IsUnderUserErrorPoint(mailAddr);
			var sendMailMobile = (string.IsNullOrEmpty(mailAddr2) == false)
				&& IsUnderUserErrorPoint(mailAddr2);

			string dispLanguageCode = string.Empty;
			string dispLanguageLocaleId = string.Empty;
			if (Constants.GLOBAL_OPTION_ENABLE)
			{
				if (masterType == Constants.FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_ORDER)
				{
					var order = new OrderService().Get(masterId);
					dispLanguageCode = order.Owner.DispLanguageCode;
					dispLanguageLocaleId = order.Owner.DispLanguageLocaleId;
				}
				else
				{
					var fixedPurchase = new FixedPurchaseService().Get(masterId);
					dispLanguageCode = fixedPurchase.DispLanguageCode;
					dispLanguageLocaleId = fixedPurchase.DispLanguageLocaleId;
				}
			}

			if (Constants.MAIL_SEND_BOTH_PC_AND_MOBILE_ENABLED)
			{
				if (sendMailPc) SendMailProcess(dataSendMail, mailAddr, masterType, dispLanguageCode, dispLanguageLocaleId);
				if (sendMailMobile) SendMailProcess(dataSendMail, mailAddr2, masterType);
			}
			else
			{
				if (sendMailPc) SendMailProcess(dataSendMail, mailAddr, masterType, dispLanguageCode, dispLanguageLocaleId);
				else if (sendMailMobile) SendMailProcess(dataSendMail, mailAddr2, masterType);
			}
		}

		/// <summary>
		/// メール送信の内部処理
		/// </summary>
		/// <param name="dataSendMail">メール配信内容のプロパティ群</param>
		/// <param name="mailAddr">送信メールアドレス</param>
		/// <param name="masterType">注文が成功したメールは"order",注文が失敗したメールは"error"</param>
		/// <param name="languageCode">言語コード</param>
		/// <param name="languageLocaleId">言語ロケールID</param>
		private void SendMailProcess(Hashtable dataSendMail, string mailAddr, string masterType, string languageCode = null, string languageLocaleId = null)
		{
			var userMailTemplate = (masterType == Constants.FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_ORDER) ?
				Constants.CONST_MAIL_ID_ORDER_COMPLETE
				: Constants.CONST_MAIL_ID_FIXEDPURCHASE_FOR_USER;

			// ユーザ対象のメールを送信
			using (var mailSender = new MailSendUtility(
				Constants.CONST_DEFAULT_SHOP_ID,
				userMailTemplate,
				(string)dataSendMail[Constants.FIELD_ORDER_USER_ID],
				dataSendMail,
				(bool)dataSendMail["is_pc"],
				Constants.MailSendMethod.Auto,
				languageCode,
				languageLocaleId,
				mailAddr))
			{
				mailSender.AddTo(StringUtility.ToEmpty(mailAddr));
				if (mailSender.SendMail() == false) throw new Exception("ユーザ対象のメール送信処理に失敗しました。", mailSender.MailSendException);
			}

			// 管理者メールは注文が成功したもののみを送信する
			if (Constants.THANKSMAIL_FOR_OPERATOR_ENABLED && (masterType == Constants.FLG_FIXEDPURCHASEBATCHMAILTMPLOG_MASTER_TYPE_ORDER))
			{
				//管理者対象のメールを送信
				using (var mailSender = new MailSendUtility(Constants.CONST_DEFAULT_SHOP_ID, Constants.CONST_MAIL_ID_ORDER_COMPLETE_FOR_OPERATOR, (string)dataSendMail[Constants.FIELD_ORDER_USER_ID], dataSendMail, (bool)dataSendMail["is_pc"], Constants.MailSendMethod.Auto, userMailAddress: mailAddr))
				{
					// Toが設定されている場合にのみメール送信
					if (string.IsNullOrEmpty(mailSender.TmpTo) == false)
					{
						if (mailSender.SendMail() == false) throw new Exception("管理者対象のメール送信処理に失敗しました。", mailSender.MailSendException);
					}
				}
			}
		}
	}
}
