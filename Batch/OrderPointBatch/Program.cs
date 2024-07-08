/*
=========================================================================================================
  Module      : メインプログラム(Constants.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Text;
using w2.App.Common.CrossPoint.Helper;
using w2.App.Common.CrossPoint.Point;
using w2.App.Common;
using w2.App.Common.User;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Common.Sql;
using w2.Domain;
using w2.Domain.Coupon;
using w2.Domain.Point;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;

namespace w2.Commerce.Batch.OrderPointBatch
{
	class Program
	{
		private static bool m_changeTempToComp = false;
		private static int m_pointTempToCompDays = 30;
		private static int m_pointTempToCompDaysLimitedTermPoint = 3;

		private static int m_iCountPointGrant = 0;			// ポイント付与時：処理数
		private static int m_iCountPointGrantDelete = 0;	// ポイント付与時：仮ポイント削除数
		private static int m_iCountDeleteExpirePoint = 0;	// 有効期限切れ時：ポイント削除数
		private static int m_iCountDeleteExpireCoupon = 0;	// 有効期限切れ時：クーポン削除数

		/// <summary>
		/// プログラムのエントリポイント
		/// </summary>
		/// <param name="args"></param>
		static void Main(string[] args)
		{
			try
			{
				// 実体作成
				Program objProgram = new Program();

				// バッチ起動をイベントログ出力
				AppLogger.WriteInfo("起動");

				//------------------------------------------------------
				// ポイント付与処理
				//------------------------------------------------------
				if (m_changeTempToComp)
				{
					objProgram.PointGrant(UpdateHistoryAction.Insert);
				}

				//------------------------------------------------------
				// 有効期限切れ削除
				//------------------------------------------------------
				// 有効期限切れポイント削除
				if (Constants.W2MP_POINT_OPTION_ENABLED)
				{
					// 有効期限切れポイント削除
					DeleteExpiredPoint(UpdateHistoryAction.Insert);
				}

				// 有効期限切れクーポン削除
				if (Constants.W2MP_COUPON_OPTION_ENABLED)
				{
					DeleteExpiredCoupon(Constants.FLG_LASTCHANGED_BATCH, UpdateHistoryAction.Insert);
				}

				//------------------------------------------------------
				// メール送信
				//------------------------------------------------------
				SendMail();

				// バッチ終了をイベントログ出力
				AppLogger.WriteInfo("正常終了");
			}
			catch (Exception ex)
			{
				StringBuilder sbErrorMessage = new StringBuilder(ex.ToString());

				//------------------------------------------------------
				// メール送信
				//------------------------------------------------------
				try
				{
					SendMail(ex);
				}
				catch (Exception ex2)
				{
					sbErrorMessage.Append("\r\n\r\n");
					sbErrorMessage.Append(ex2.ToString());
				}
				// エラーイベントログ出力
				AppLogger.WriteError(sbErrorMessage.ToString());
			}
		}

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Program()
		{
			// 初期化
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
				ConfigurationSetting csSetting = new ConfigurationSetting(
					Properties.Settings.Default.ConfigFileDirPath,
					ConfigurationSetting.ReadKbn.C000_AppCommon,
					ConfigurationSetting.ReadKbn.C100_BatchCommon,
					ConfigurationSetting.ReadKbn.C200_OrderPointBatch);

				//------------------------------------------------------
				// アプリケーション固有の設定
				//------------------------------------------------------
				// メール送信先設定
				Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = csSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = csSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Bcc");

				//------------------------------------------------------
				// 各種設定
				//------------------------------------------------------
				// 本ポイント移行設定
				m_changeTempToComp = csSetting.GetAppBoolSetting("OrderPointBatch_ChangeTempToComp_Enabled");
				// 本ポイント移行設定：出荷後何日で本ポイントへ移行するか
				m_pointTempToCompDays = csSetting.GetAppIntSetting("OrderPointBatch_PointTempToCompDays");
				// 本ポイント移行設定：出荷後何日で本ポイントへ移行するか(期間限定ポイント)
				m_pointTempToCompDaysLimitedTermPoint =
					csSetting.GetAppIntSetting("OrderPointBatch_PointTempToCompLimitedTermPointDays");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// ポイント確定処理
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		private void PointGrant(UpdateHistoryAction updateHistoryAction)
		{
			if (Constants.MARKETINGPLANNER_USE_TEMPORARY_POINT == false) return;

			var errors = new StringBuilder();
			var orders = DomainFacade.Instance.OrderService.GetPointGrantOrder(m_pointTempToCompDays);

			var tempPoints = DomainFacade.Instance.PointService.GetTargetUserTempPointToReal(
				m_pointTempToCompDays,
				m_pointTempToCompDaysLimitedTermPoint);

			using (var sqlAccessor = new SqlAccessor())
			{
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				foreach (var tempPoint in tempPoints)
				{
					try
					{
						DomainFacade.Instance.PointService.GetUpdLockForUserPoint(tempPoint.UserId, sqlAccessor);
						DomainFacade.Instance.PointService.GetUpdLockForUserPointHistory(tempPoint.UserId, sqlAccessor);

						var updated = DomainFacade.Instance.PointService.TempToRealPoint(
							tempPoint,
							Constants.FLG_LASTCHANGED_BATCH,
							updateHistoryAction,
							sqlAccessor);

						m_iCountPointGrant += updated;
						m_iCountPointGrantDelete += updated;

						sqlAccessor.CommitTransaction();
					}
					catch (Exception ex)
					{
						sqlAccessor.RollbackTransaction();
						errors.AppendLine(ex.ToString());
					}
				}
			}

			if (Constants.CROSS_POINT_OPTION_ENABLED)
			{
				if (orders == null) return;

				var getInput = new PointApiInput();
				foreach (var order in orders)
				{
					if (order.OrderPointAdd > 0)
					{
						getInput.MemberId = order.UserId;
						getInput.OrderId = order.OrderId;
						getInput.UserCode = Constants.FLG_LASTCHANGED_BATCH;

						var point = new CrossPointPointApiService().Grant(getInput.GetParam(PointApiInput.RequestType.Grant));
						if (point.IsSuccess == false)
						{
							var error = ErrorHelper.CreateCrossPointApiError(point.ErrorMessage, order.UserId);
							errors.AppendLine(error);
							continue;
						}
					}

					DomainFacade.Instance.OrderService.UpdateOrderExtendStatus(
						order.OrderId,
						Constants.ORDER_EXTEND_STATUS_NO_CROSSPOINT_GRANTED,
						Constants.FLG_ORDER_EXTEND_STATUS_ON,
						DateTime.Now,
						Constants.FLG_LASTCHANGED_BATCH,
						UpdateHistoryAction.Insert);

					// Update user info
					var user = DomainFacade.Instance.UserService.Get(order.UserId);
					UserUtility.AdjustPointAndMemberRankByCrossPointApi(user);
				}
			}

			if (errors.Length > 0) throw new Exception("処理でエラーが発生しました。：\r\n" + errors);
		}

		/// <summary>
		/// 有効期限切れポイント削除
		/// </summary>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		private static void DeleteExpiredPoint(UpdateHistoryAction updateHistoryAction)
		{
			using (var sqlAccessor = new SqlAccessor())
			{
				// トランザクション開始
				sqlAccessor.OpenConnection();
				sqlAccessor.BeginTransaction();

				try
				{
					// 期限切れユーザポイント取得
					var sv = new PointService();
					var deletePoints = sv.GetExpiredUserPoints(sqlAccessor);

					// ループして削除＆履歴インサート（更新履歴とともに）
					foreach (var deletePoint in deletePoints)
					{
						var updated = sv.DeleteExpiredPoint(
							deletePoint,
							Constants.FLG_LASTCHANGED_BATCH,
							Constants.CROSS_POINT_OPTION_ENABLED == false,
							updateHistoryAction,
							sqlAccessor);
						m_iCountDeleteExpirePoint += updated;

						// ユーザーテーブルの最終更新者を更新
						var userService = new UserService();
						userService.Modify(
							deletePoint.UserId,
							model =>
							{
								model.LastChanged = Constants.FLG_LASTCHANGED_BATCH;
							},
							UpdateHistoryAction.DoNotInsert,
							sqlAccessor);
						// トランザクションコミット
						sqlAccessor.CommitTransaction();
					}
				}
					catch (Exception ex)
				{
					// トランザクションロールバック
					sqlAccessor.RollbackTransaction();

					throw ex;
				}
			}
		}

		/// <summary>
		/// 有効期限切れクーポン削除（更新履歴とともに）
		/// </summary>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		private static void DeleteExpiredCoupon(string lastChanged, UpdateHistoryAction updateHistoryAction)
		{
			var couponService = new CouponService();
			var update = couponService.DeleteExpiredUserCoupon(lastChanged, updateHistoryAction);
			m_iCountDeleteExpireCoupon += update;
		}

		/// <summary>
		/// メール送信処理（成功時）
		/// </summary>
		private static void SendMail()
		{
			SendMail(null);
		}
		/// <summary>
		/// メール送信処理（失敗時）
		/// </summary>
		/// <param name="ex">例外（NULLなら成功）</param>
		private static void SendMail(Exception ex)
		{
			using (SmtpMailSender smsMailSender = new SmtpMailSender(Constants.SERVER_SMTP))
			{
				// メール送信デフォルト値設定
				smsMailSender.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => smsMailSender.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => smsMailSender.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => smsMailSender.AddBcc(mail.Address));

				if (ex == null)
				{
					StringBuilder sbMessage = new StringBuilder();
					sbMessage.Append("---------------------実行結果---------------------\n");
					sbMessage.Append("ポイント付与時、付与数　　　　　　 　　： ").Append(m_iCountPointGrant).Append(" 件\n");
					sbMessage.Append("ポイント付与時、仮ポイント削除数　　　 ： ").Append(m_iCountPointGrantDelete).Append(" 件\n");
					sbMessage.Append("有効期限切れ時、ポイント削除数　　　　 ： ").Append(m_iCountDeleteExpirePoint).Append(" 件\n");
					sbMessage.Append("有効期限切れ時、クーポン削除数　　　　 ： ").Append(m_iCountDeleteExpireCoupon).Append(" 件\n");

					smsMailSender.SetSubject(Constants.MAIL_SUBJECTHEAD);
					smsMailSender.SetBody(sbMessage.ToString());
				}
				else
				{
					smsMailSender.SetSubject(Constants.MAIL_SUBJECTHEAD + "【エラー】");
					smsMailSender.SetBody(BaseLogger.CreateExceptionMessage(ex));
				}

				// メール送信
				bool blResult = smsMailSender.SendMail();
				if (blResult == false)
				{
					Exception ex2 = smsMailSender.MailSendException;
					FileLogger.WriteError(ex2);
				}
			}
		}
	}
}
