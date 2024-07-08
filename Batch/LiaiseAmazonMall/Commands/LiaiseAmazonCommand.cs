/*
=========================================================================================================
  Module      : Amazon連携コマンドクラス(LiaiseAmazonCommand.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using AmazonMarketPlaceWebService;
using w2.Common.Logger;
using w2.Domain.MallCooperationSetting;
using w2.App.Common.MallCooperation;
using w2.Commerce.Batch.LiaiseAmazonMall.Amazon;
using w2.Commerce.Batch.LiaiseAmazonMall.Util;

namespace w2.Commerce.Batch.LiaiseAmazonMall.Commands
{
	/// <summary>
	/// Amazon連携コマンドクラス
	/// </summary>
	public class LiaiseAmazonCommand : CommandBase
	{
		#region +OnStart 開始時処理
		/// <summary>
		/// 開始時処理
		/// </summary>
		public override void OnStart()
		{
			// 実行開始時間設定
			this.BeginTime = DateTime.Now;
			FileLogger.WriteInfo(string.Format("Amazon連携処理開始[実行開始時間：{0}]", this.BeginTime.ToString()));
		}
		#endregion

		#region +Exec 実行
		/// <summary>
		/// 実行
		/// </summary>
		public override void Exec()
		{
			// Amazonモール連携設定取得
			var amazonMallSettings
				= new MallCooperationSettingService().GetValidByMallKbn(Constants.CONST_DEFAULT_SHOP_ID, Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_AMAZON);

			foreach (var amazonMallSetting in amazonMallSettings)
			{
				// メンテナンス時間帯中はログ出力してスキップする
				if (IsMaintenanceTime(amazonMallSetting)) continue;

				AmazonApiService amazonOrderApi = null;
				try
				{
					amazonOrderApi = new AmazonApiService(amazonMallSetting);
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					continue;
				}

				// 前回実行時間取得(初回実行時はNULL)
				var latestExecuteChecker = new LatestExecuteDatetimeChecker();
				var latestExecuteDatetime = latestExecuteChecker.GetLastExecuteDatetime(amazonMallSetting.MallId);

				// 注文取込処理実行
				var orderImportAction = new OrderImportAction(amazonOrderApi, latestExecuteDatetime, amazonMallSetting.MallId);
				try
				{
					orderImportAction.OnStart();
					orderImportAction.Exec();
					orderImportAction.OnComplete();

					// 注文取込実行終了時間出力
					latestExecuteChecker.CreateEndFile(this.BeginTime);
				}
				catch(Exception ex)
				{
					FileLogger.WriteError(ex);
					orderImportAction.OnError();
				}

				// 出荷通知処理実行
				var orderFulfillmentAction = new OrderFulfillmentAction(amazonOrderApi, amazonMallSetting.MallId);
				try
				{
					orderFulfillmentAction.OnStart();
					orderFulfillmentAction.Exec();
					orderFulfillmentAction.OnComplete();
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					orderFulfillmentAction.OnError();
				}
			}
		}
		#endregion

		#region +OnComplete 完了時処理
		/// <summary>
		/// 完了時処理
		/// </summary>
		public override void OnComplete()
		{
			FileLogger.WriteInfo(string.Format("Amazon連携処理完了[実行終了時間：{0}]", DateTime.Now.ToString()));
		}
		#endregion

		#region +OnError 異常時処理
		/// <summary>
		/// 異常時処理
		/// </summary>
		public override void OnError()
		{
			FileLogger.WriteError(string.Concat("Amazon連携処理でエラー発生"));
		}
		#endregion

		#region -IsMaintenanceTime メンテナンス時間帯か
		/// <summary>
		/// メンテナンス時間帯か
		/// </summary>
		/// <param name="amazonMallSetting">Amazon連携設定</param>
		/// <returns>判定結果（true:メンテナンス時間帯中、false：メンテナンス時間帯外）</returns>
		private bool IsMaintenanceTime(MallCooperationSettingModel amazonMallSetting)
		{
			// メンテナンス時間中はスキップ
			var isMaintenance = false;
			if ((amazonMallSetting.MaintenanceDateFrom != null) || (amazonMallSetting.MaintenanceDateTo != null))
			{
				var maintenanceDateFrom = amazonMallSetting.MaintenanceDateFrom ?? DateTime.MinValue;
				var maintenanceDateTo = amazonMallSetting.MaintenanceDateTo ?? DateTime.MaxValue;

				// 「メンテナンス開始日 <= 現在 < メンテナンス終了日」の時、次のモールの処理へ飛ばす
				if ((maintenanceDateFrom <= DateTime.Now) && (DateTime.Now < maintenanceDateTo))
				{
					// モール監視ログ登録（メンテナンス期間中）
					new MallWatchingLogManager().Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISEAMAZONMALL,
						amazonMallSetting.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
						"メンテナンス時間帯のため処理を実行しませんでした。");
					isMaintenance = true;
				}
			}
			return isMaintenance;
		}
		#endregion

		#region プロパティ
		/// <summary>実行開始時間</summary>
		private DateTime BeginTime { get; set; }
		#endregion
	}
}
