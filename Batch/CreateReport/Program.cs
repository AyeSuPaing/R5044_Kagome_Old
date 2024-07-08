/*
=========================================================================================================
  Module      : メインプログラム(Program.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2009 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.App.Common;
using w2.App.Common.Order;
using w2.App.Common.Util;
using w2.Common.Logger;
using w2.Common.Net.Mail;
using w2.Common.Sql;
using w2.Domain.Coupon;
using w2.Domain.DailyOrderShipmentForecast;
using w2.Domain.FixedPurchase;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.SubscriptionBox;
using w2.Domain.ShopShipping;
using w2.MarketingPlanner.Batch.CreateReport.Action;

namespace w2.MarketingPlanner.Batch.CreateReport
{
	class Program
	{
		/// <summary>
		/// アプリケーションエントリポイント
		/// </summary>
		/// <param name="args">実行引数</param>
		/// <returns>エラーコード</returns>
		static int Main(string[] args)
		{
			// 実行タイプ（DailyOrderShipmentForecast）
			const string EXEC_TYPE_DAILYORDERSHIPMENTFORECAST = "DailyOrderShipmentForecast";

			// エラーコード
			var returnCode = -1;

			try
			{
				Program program = new Program();

				if (args.Length == 0)
				{
					// バッチ起動をイベントログ出力
					AppLogger.WriteInfo("起動");

					program.CreateReport();

					// バッチ終了をイベントログ出力
					AppLogger.WriteInfo("正常終了");

					returnCode = Constants.BATCH_CREATEREPORT_ERRORCODE_COMPLETED;
				}
				else if ((args.Length != 0)
					&& (args[0] == EXEC_TYPE_DAILYORDERSHIPMENTFORECAST))
				{
					// 二重実行禁止
					bool isSuccess = ProcessUtility.ExcecWithProcessMutex(() =>
					{
						// 日別出荷予測
						program.ForecastShipmentData();

						// バッチ終了をログ出力
						FileLogger.Write("ForecastShipmentInfo", "終了");

						returnCode = Constants.BATCH_CREATEREPORT_ERRORCODE_COMPLETED;
					});
					if (isSuccess == false)
					{
						// 二重実行エラー
						returnCode = Constants.BATCH_CREATEREPORT_ERRORCODE_DUPLICATE_EXECUTION;

						throw new Exception("他プロセスが実行中のため、実行が失敗しました。二重実行は禁止されています。");
					}
				}
			}
			catch (Exception ex)
			{
				// メール送信
				SendMail(ex);

				// エラーイベントログ出力
				AppLogger.WriteError(ex);
			}

			return returnCode;
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
					ConfigurationSetting.ReadKbn.C200_CreateReport);

				// メール送信先設定
				Constants.MAIL_SUBJECTHEAD = csSetting.GetAppStringSetting("Mail_SubjectHead");
				Constants.MAIL_FROM = csSetting.GetAppMailAddressSetting("Mail_From");
				Constants.MAIL_TO_LIST = csSetting.GetAppMailAddressSettingList("Mail_To");
				Constants.MAIL_CC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Cc");
				Constants.MAIL_BCC_LIST = csSetting.GetAppMailAddressSettingList("Mail_Bcc");
			}
			catch (Exception ex)
			{
				throw new ApplicationException("Configファイルの読み込みに失敗しました。", ex);
			}
		}

		/// <summary>
		/// レポート作成
		/// </summary>
		private void CreateReport()
		{
			// ポイントレポート作成（月次未回収ポイント）
			CreateUnusedPoint();

			// クーポンレポート作成（月次未回収クーポン）
			CreateUnusedCoupon();

			// Create summary report
			CreateSummary();

			// メール送信
			SendMail();
		}

		/// <summary>
		/// ポイントレポート作成（月次未回収ポイント）
		/// </summary>
		private void CreateUnusedPoint()
		{
			if (Constants.W2MP_POINT_OPTION_ENABLED)
			{
				DateTime dtYesterday = DateTime.Now.AddDays(-1);

				using (var accessor = new SqlAccessor())
				using (var statement = new SqlStatement("CreateReport", "CreateUnusedPointReport") { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
				{
					Hashtable htInput = new Hashtable();
					htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_DEPT_ID, Constants.CONST_DEFAULT_DEPT_ID);
					htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_TGT_YEAR, dtYesterday.ToString("yyyy"));
					htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_TGT_MONTH, dtYesterday.ToString("MM"));
					htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_TGT_DAY, dtYesterday.ToString("dd"));
					htInput.Add(Constants.FIELD_DISPSUMMARYANALYSIS_SUMMARY_KBN, "pnt_unused");

					statement.ExecStatementWithOC(accessor, htInput);
				}
			}
		}

		/// <summary>
		/// クーポンレポート作成（月次未回収クーポン）
		/// </summary>
		private void CreateUnusedCoupon()
		{
			if (Constants.W2MP_COUPON_OPTION_ENABLED)
			{
				DateTime dtYesterday = DateTime.Now.AddDays(-1);

				// モデル作成
				var model = new DispSummaryAnalysisModel();
				model.DeptId = Constants.CONST_DEFAULT_DEPT_ID;
				model.TgtYear = dtYesterday.ToString("yyyy");
				model.TgtMonth = dtYesterday.ToString("MM");
				model.TgtDay = dtYesterday.ToString("dd");

				var couponService = new CouponService();
				// 枚数集計
				model.SummaryKbn = "cpn_unused_cnt";
				couponService.CreateUnusedCouponCountReport(model);

				// 金額集計
				model.SummaryKbn = "cpn_unused_prc";
				couponService.CreateUnusedCouponPriceReport(model);
			}
		}

		/// <summary>
		/// メール送信処理（成功時）
		/// </summary>
		private void SendMail()
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
				smsMailSender.SetSubject(Constants.MAIL_SUBJECTHEAD);
				smsMailSender.SetFrom(Constants.MAIL_FROM.Address);
				Constants.MAIL_TO_LIST.ForEach(mail => smsMailSender.AddTo(mail.Address));
				Constants.MAIL_CC_LIST.ForEach(mail => smsMailSender.AddCC(mail.Address));
				Constants.MAIL_BCC_LIST.ForEach(mail => smsMailSender.AddBcc(mail.Address));

				smsMailSender.SetBody((ex == null) ? "成功" : BaseLogger.CreateExceptionMessage(ex));

				// メール送信
				bool blResult = smsMailSender.SendMail();
				if (blResult == false) FileLogger.WriteError(smsMailSender.MailSendException);
			}
		}

		/// <summary>
		/// Create summary
		/// </summary>
		private void CreateSummary()
		{
			foreach (var action in GetActions())
			{
				action.Execute();
			}
		}

		/// <summary>
		/// Get actions
		/// </summary>
		/// <returns>Actions</returns>
		private IEnumerable<IAction> GetActions()
		{
			yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_ACCESS);
			yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_COUNT);
			yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_ORDER_AMOUNT);
			yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_CONVERSION);
			yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_LTV);
			yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_SENT_MAIL_COUNT);
			yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_MAIL_CLICK_COUNT);

			// For case fixed purchase option enabled
			if (Constants.FIXEDPURCHASE_OPTION_ENABLED)
			{
				yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_REGISTER);
				yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_CANCEL);
				yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_FIXED_PURCHASE_COUNT);
			}

			yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_REGISTER);
			yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_USER_WITHDRAWAL);
			yield return new CreateSummaryReportAction(Constants.FLG_SUMMARYREPORT_DATA_KBN_MEMBERSHIP_COUNT);

			// For case order workflow option enabled
			if (Constants.ORDERWORKFLOW_OPTION_ENABLED)
			{
				yield return new WorkflowTargetCountAggregateAction(ActionBase.CONST_WORKFLOW_TARGET_COUNT_AGGREGATE_ACTION);
			}
		}

		/// <summary>
		/// 日別出荷予測
		/// </summary>
		private void ForecastShipmentData()
		{
			// １ー１オプションがオンか
			if ((Constants.SHIPTMENT_FORECAST_BY_DAYS_ENABLED == false)
				|| (Constants.SCHEDULED_SHIPPING_DATE_OPTION_ENABLE == false)) return;

			FileLogger.Write("ForecastShipmentInfo", "日別出荷予測開始");

			// １ー２当日初回かどうか確認
			var lastExecDate = LastExecDate.GetLastExecDate() > DateTime.Today
				? DateTime.Today
				: LastExecDate.GetLastExecDate();

			var shipmentQuantity = new List<DailyOrderShipmentForecastModel>();

			if ((lastExecDate != DateTime.Today) && (lastExecDate < DateTime.Today))
			{
				// ２－１前日注文の出荷数
				AggregateShipmentsPreviousDays(shipmentQuantity);
				FileLogger.Write("ForecastShipmentInfo", "前日のデータ集計完了");
			}

			// ３－１通常商品の出荷数予測
			AggregateShipmentsYear(shipmentQuantity);
			FileLogger.Write("ForecastShipmentInfo", "通常商品の出荷数予測完了");

			// ３－２定期商品の出荷数予測
			AggregateFBShipmentsYear(shipmentQuantity);
			FileLogger.Write("ForecastShipmentInfo", "定期商品の出荷数予測完了");

			// ４DBに書き込む
			InsertAndUpdateShipmentQuantitytoDataBese(shipmentQuantity);

			// ５最終実行日更新
			LastExecDate.UpdateLastExecDate();
		}

		/// <summary>
		/// 前日の出荷情報集計
		/// </summary>
		/// <param name="shipmentQuantity">集計データリスト</param>
		/// <returns>前日の出荷情報</returns>
		private void AggregateShipmentsPreviousDays(List<DailyOrderShipmentForecastModel> shipmentQuantity)
		{
			var previousDays = DateTime.Today.AddDays(-1);
			var shippedOrders = new OrderService().GetShippingOrderForDate(
				previousDays.Date,
				previousDays.Date.AddDays(1).AddTicks(-1));
			if (shippedOrders == null) return;

			var previousDaysShipmentQuantity = new DailyOrderShipmentForecastModel()
			{
				ShipmentDate = previousDays,
				ShipmentOrderCount = shippedOrders.Count,
				TotalOrderPriceSubtotal = shippedOrders.Cast<DataRowView>().Sum(shippedOrder =>
					(decimal)shippedOrder[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL]),
				DateCreated = DateTime.Now,
				DateChanged = DateTime.Now,
				LastChanged = Constants.FLG_LASTCHANGED_BATCH
			};
		}

		/// <summary>
		/// 本日より半年間の出荷情報集計
		/// </summary>
		/// <param name="shipmentQuantity">集計データリスト</param>
		private void AggregateShipmentsYear(List<DailyOrderShipmentForecastModel> shipmentQuantity)
		{
			var orderService = new OrderService();
			var shippedOrders = orderService.GetShippingOrderForDate(
				DateTime.Today,
				DateTime.Today.AddMonths(6));
			if (shippedOrders == null) return;

			foreach (DataRowView shippedOrder in shippedOrders)
			{
				var isAdded = false;
				shipmentQuantity
					.Where(s => s.ShipmentDate == (DateTime)shippedOrder[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE]).ToList()
						.ForEach(s =>
						{
							s.ShipmentOrderCount++;
							s.TotalOrderPriceSubtotal += (decimal)shippedOrder[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL];
							isAdded = true;
						});

				if (isAdded == false)
				{
					// 新にレコードを追加する
					var shipmentQuantityList = new DailyOrderShipmentForecastModel()
					{
						ShipmentDate = (DateTime)shippedOrder[Constants.FIELD_ORDERSHIPPING_SCHEDULED_SHIPPING_DATE],
						ShipmentOrderCount = 1,
						TotalOrderPriceSubtotal = (decimal)shippedOrder[Constants.FIELD_ORDER_ORDER_PRICE_SUBTOTAL],
						DateCreated = DateTime.Now,
						DateChanged = DateTime.Now,
						LastChanged = Constants.FLG_LASTCHANGED_BATCH
					};
					shipmentQuantity.Add(shipmentQuantityList);
				}
			}
		}

		/// <summary>
		/// 本日より半年間の定期注文出荷情報集計（頒布会も含め）
		/// </summary>
		/// <param name="shipmentQuantity">集計データリスト</param>
		private void AggregateFBShipmentsYear(List<DailyOrderShipmentForecastModel> shipmentQuantity)
		{
			// 定期購入情報ループ開始
			var fixedPurchaseContainerList = new FixedPurchaseService().GetContainerWorking();
			var fixedPurchaseList = fixedPurchaseContainerList.Cast<DataRowView>().GroupBy(item => (string)item[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID])
				.Select(group => group.First())
				.ToArray();
			foreach (DataRowView fixedPurchase in fixedPurchaseList)
			{
				// 一回の注文での合計金額
				var totalPrice = (decimal)0;
				// 出荷数
				var shippingNum = 1;
				// 配送種別
				var shopShipping = new ShopShippingModel();
				// 頒布会？
				var isSubscriptionBox = Constants.SUBSCRIPTION_BOX_OPTION_ENABLED
					&& (string.IsNullOrEmpty((string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID]) == false);
				// 回数(True)? 期間(False)?
				var isSubscriptionBoxTypeCount = true;
				// 定期最大回数（頒布会用）
				var maxOrderCount = 0;
				// 定期期間最終日（頒布会用）
				var lastOrderDate = DateTime.MaxValue;
				// 無限繰り返し？（頒布会用）
				var isAutoRenewal = false;
				// 定額頒布会？（頒布会用）
				var isFixedAount = false;

				// 頒布会？
				if (isSubscriptionBox)
				{
					var subscriptionBoxService = new SubscriptionBoxService();
					var subscriptionBox = subscriptionBoxService.GetByCourseId(
						(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID]);
					if (subscriptionBox == null) continue;

					// 無限繰り返し？
					isAutoRenewal = (subscriptionBox.AutoRenewal == Constants.FLG_SUBSCRIPTIONBOX_AUTO_RENEWAL_TRUE)
						|| (subscriptionBox.IndefinitePeriod == Constants.FLG_SUBSCRIPTIONBOX_INDEFINITE_PERIOD_TRUE);
					// 定額頒布会？
					isFixedAount = (subscriptionBox.FixedAmountFlg == Constants.FLG_SUBSCRIPTIONBOX_FIXED_AMOUNT_TRUE)
						&& (string.IsNullOrEmpty(subscriptionBox.FixedAmount.ToString()) == false);
					// 回数or期間
					isSubscriptionBoxTypeCount = isAutoRenewal
						? true
						: subscriptionBox.OrderItemDeterminationType
							== Constants.FLG_SUBSCRIPTIONBOX_ORDER_ITEM_DETERMINATION_TYPE_NUMBER_TIME
								? true : false;

					// 頒布会デフォルト商品
					var subscriptionBoxDefaultItem = subscriptionBoxService.GetDefaultItemDetails(
						(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_SUBSCRIPTION_BOX_COURSE_ID]);

					// 回数
					if (isSubscriptionBoxTypeCount)
					{
						// 一回目配送商品の合計金額
						totalPrice = isFixedAount
							? (decimal)subscriptionBox.FixedAmount
							: subscriptionBoxDefaultItem.Where(item =>
								item.Count == 1).Sum(item =>
									(item.Price.HasValue
										? item.Price.Value
										: 0) * item.ItemQuantity);

						// 通常頒布会かつ回数の場合最大回数を取得
						if (isAutoRenewal == false) maxOrderCount = subscriptionBoxDefaultItem.Select(subscriptionBoxItem =>
							(int)subscriptionBoxItem.Count).Max();
					}
					// 期間
					else
					{
						// 一回目配送商品の合計金額
						totalPrice = isFixedAount
							? (decimal)subscriptionBox.FixedAmount
							: subscriptionBoxDefaultItem.Where(item =>
								item.IsInTerm((DateTime)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_DATE_CREATED])).Sum(item =>
									(item.Price.HasValue
										? item.Price.Value
										: 0) * item.ItemQuantity);

						// 通常頒布会かつ期間の場合期間最終日を取得
						if (isAutoRenewal == false) lastOrderDate = subscriptionBoxDefaultItem.Select(subscriptionBoxItem =>
							(DateTime)subscriptionBoxItem.TermUntil).Max();
					}

					// 配送種別 
					if (string.IsNullOrEmpty(subscriptionBoxDefaultItem.First().ShippingId)) continue;
					shopShipping = new ShopShippingService().Get(
						(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_SHOP_ID],
						subscriptionBoxDefaultItem.First().ShippingId);
				}
				// 通常定期
				else
				{
					// 出荷数
					shippingNum = fixedPurchaseContainerList.Cast<DataRowView>().Where(fixedPurchaseContainer =>
						(string)fixedPurchaseContainer[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID]
							== (string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID]).Select(fixedPurchaseContainer =>
								(int)fixedPurchaseContainer[Constants.FIELD_FIXEDPURCHASESHIPPING_FIXED_PURCHASE_SHIPPING_NO]).Max();

					// 合計金額
					totalPrice = fixedPurchaseContainerList.Cast<DataRowView>().Where(fixedPurchaseContainer =>
						(string)fixedPurchaseContainer[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID]
								== (string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_ID]).Select(fixedPurchaseContainer =>
									string.IsNullOrEmpty(fixedPurchaseContainer[Constants.FIELD_PRODUCTVARIATION_PRICE].ToString())
										? (decimal)0
										: (decimal)fixedPurchaseContainer[Constants.FIELD_PRODUCTVARIATION_PRICE]).Sum();

					// 配送種別
					var product = new ProductService().GetProductVariation(
						(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_SHOP_ID],
						(string)fixedPurchase[Constants.FIELD_PRODUCTVARIATION_PRODUCT_ID] ?? string.Empty,
						(string)fixedPurchase[Constants.FIELD_PRODUCTVARIATION_VARIATION_ID],
						(string)fixedPurchase[Constants.FIELD_USER_MEMBER_RANK_ID]);
					if ((product == null) || string.IsNullOrEmpty(product.ShippingId)) continue;
					shopShipping = new ShopShippingService().Get(
						(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_SHOP_ID],
						product.ShippingId);
				}

				var fixedPurchaseService = new FixedPurchaseService();
				//次回配送日計算モード
				var calculateMode = fixedPurchaseService.GetCalculationMode(
					(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN],
					Constants.FIXEDPURCHASE_NEXT_SHIPPING_CALCULATION_MODE);

				var baseDate = (DateTime)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_NEXT_SHIPPING_DATE];
				var orderCount = 0;
				while ((baseDate <= DateTime.Today.AddMonths(6))
					&& (orderCount <= maxOrderCount)
					&& (baseDate <= lastOrderDate))
				{
					// 次回出荷日計算
					var nextScheduledShippingDate = OrderCommon.CalculateScheduledShippingDateBasedOnFirstOrderDate(
						(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_SHOP_ID],
						baseDate,
						string.Empty,
						(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASESHIPPING_DELIVERY_COMPANY_ID],
						(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_COUNTRY_ISO_CODE],
						(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ADDR1],
						(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASESHIPPING_SHIPPING_ZIP],
						null);

					// リードタイム使ってない場合は最短配送日数で出荷日を求める
					if (nextScheduledShippingDate.HasValue == false)
					{
						nextScheduledShippingDate = baseDate.AddDays(-shopShipping.FixedPurchaseShippingDaysRequired);
						if (nextScheduledShippingDate < DateTime.Today) nextScheduledShippingDate = DateTime.Today;
					}

					var isAdded = false;
					shipmentQuantity
						.Where(s => s.ShipmentDate == nextScheduledShippingDate).ToList()
							.ForEach(s =>
								{ s.ShipmentOrderCount += shippingNum;
									s.TotalOrderPriceSubtotal += totalPrice;
									isAdded = true; });

					if (isAdded == false)
					{
						// 新にレコードを追加する
						var shipmentQuantityList = new DailyOrderShipmentForecastModel()
						{
							ShipmentDate = (DateTime)nextScheduledShippingDate,
							ShipmentOrderCount = shippingNum,
							TotalOrderPriceSubtotal = totalPrice,
							DateCreated = DateTime.Now,
							DateChanged = DateTime.Now,
							LastChanged = Constants.FLG_LASTCHANGED_BATCH
						};
						shipmentQuantity.Add(shipmentQuantityList);
					}

					if ((isSubscriptionBox == false)
						|| (isSubscriptionBox && isAutoRenewal)
						|| ((isAutoRenewal == false) && (isSubscriptionBoxTypeCount == false) && (baseDate < lastOrderDate))
						|| ((isAutoRenewal == false) && isSubscriptionBoxTypeCount && (orderCount < maxOrderCount)))
					{
						// さらに次回配送日を算出する
						baseDate = fixedPurchaseService.CalculateNextShippingDate(
							(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_KBN],
							(string)fixedPurchase[Constants.FIELD_FIXEDPURCHASE_FIXED_PURCHASE_SETTING1],
							baseDate,
							shopShipping.FixedPurchaseShippingDaysRequired,
							shopShipping.FixedPurchaseMinimumShippingSpan,
							calculateMode);
					}

					if (isSubscriptionBox && (isAutoRenewal == false)) orderCount++;
				}
			}
		}

		/// <summary>
		/// 予測レポートをデータベースに書き込む
		/// </summary>
		/// <param name="shipmentQuantity">集計データリスト</param>
		private void InsertAndUpdateShipmentQuantitytoDataBese(List<DailyOrderShipmentForecastModel> shipmentQuantity)
		{
			if (shipmentQuantity == null) return;

			using (var accessor = new SqlAccessor())
			{
				try
				{
					accessor.OpenConnection();
					accessor.BeginTransaction();

					var dailyOrderShipmentForecast = new DailyOrderShipmentForecastService();

					// DB書き込み実行
					foreach (var shipmentQuantityItem in shipmentQuantity)
					{
						if (string.IsNullOrEmpty(shipmentQuantityItem.ShipmentDate.ToString()) == false)
						{
							dailyOrderShipmentForecast.InsertOrUpdate(shipmentQuantityItem);
						}
					}

					// 古いデータ(2年前)を削除する
					dailyOrderShipmentForecast.DeleteOldShipments();

					accessor.CommitTransaction();
					FileLogger.Write("ForecastShipmentInfo", "データベースに書き込みが成功しました");
				}
				catch(Exception ex)
				{
					accessor.RollbackTransaction();
					FileLogger.Write("ForecastShipmentInfo", "データベースに書き込みが失敗しました");

					// エラーイベントログ出力
					AppLogger.WriteError(ex);
				}
			}
		}
	}
}
