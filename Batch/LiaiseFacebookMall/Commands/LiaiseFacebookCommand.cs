/*
=========================================================================================================
  Module      : Facebook連携コマンドクラス(LiaiseFacebookCommand.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using w2.App.Common.Facebook;
using w2.App.Common.MallCooperation;
using w2.Commerce.Batch.LiaiseFacebookMall.Actions;
using w2.Common.Logger;
using w2.Domain;
using w2.Domain.MallCooperationSetting;

namespace w2.Commerce.Batch.LiaiseFacebookMall.Commands
{
	/// <summary>
	/// Facebook連携コマンドクラス
	/// </summary>
	public class LiaiseFacebookCommand : CommandBase
	{
		/// <summary>
		/// 開始時処理
		/// </summary>
		public override void OnStart()
		{
			// 実行開始時間設定
			this.BeginTime = DateTime.Now;
			FileLogger.WriteInfo(
				string.Format(
					"Facebook連携処理開始[実行開始時間：{0}]",
					this.BeginTime.ToString()));
		}

		/// <summary>
		/// 実行
		/// </summary>
		public override void Exec()
		{
			// Check config option
			if (Constants.FACEBOOK_CATALOG_API_COOPERATION_OPTION_ENABLED == false) return;

			// Get facebook mall cooperation settings
			var facebookMallSettings =
				DomainFacade.Instance.MallCooperationSettingService.GetValidByMallKbn(
					Constants.CONST_DEFAULT_SHOP_ID,
					Constants.FLG_MALLCOOPERATIONSETTING_MALL_KBN_FACEBOOK);
			if (facebookMallSettings.Length == 0) return;

			var facade = new FacebookCatalogApiFacade();
			foreach (var setting in facebookMallSettings
				.Where(mallCoperation => (string.IsNullOrEmpty(mallCoperation.MallExhibitsConfig) == false)))
			{
				// メンテナンス時間帯中はログ出力してスキップする
				if (IsMaintenanceTime(setting)) continue;

				// Call API Facebook Catalog
				var productCatalog = new ProductCatalogAction(facade, setting);
				try
				{
					productCatalog.OnStart();
					productCatalog.Exec();
					productCatalog.OnComplete();
				}
				catch (Exception ex)
				{
					FileLogger.WriteError(ex);
					productCatalog.OnError();
				}
			}
		}

		/// <summary>
		/// 完了時処理
		/// </summary>
		public override void OnComplete()
		{
			FileLogger.WriteInfo(
				string.Format(
					"Facebook連携処理完了[実行終了時間：{0}]",
					DateTime.Now.ToString()));
		}

		/// <summary>
		/// 異常時処理
		/// </summary>
		public override void OnError()
		{
			FileLogger.WriteError("Facebook連携処理でエラー発生");
		}

		/// <summary>
		/// メンテナンス時間帯か
		/// </summary>
		/// <param name="facebookMallSetting">Facebook連携設定</param>
		/// <returns>判定結果（true:メンテナンス時間帯中、false：メンテナンス時間帯外）</returns>
		private bool IsMaintenanceTime(MallCooperationSettingModel facebookMallSetting)
		{
			// メンテナンス時間中はスキップ
			var isMaintenance = false;
			if (facebookMallSetting.MaintenanceDateFrom.HasValue || facebookMallSetting.MaintenanceDateTo.HasValue)
			{
				var maintenanceDateFrom = facebookMallSetting.MaintenanceDateFrom ?? DateTime.MinValue;
				var maintenanceDateTo = facebookMallSetting.MaintenanceDateTo ?? DateTime.MaxValue;

				// 「メンテナンス開始日 <= 現在 < メンテナンス終了日」の時、次のモールの処理へ飛ばす
				if ((maintenanceDateFrom <= DateTime.Now) && (DateTime.Now < maintenanceDateTo))
				{
					// モール監視ログ登録（メンテナンス期間中）
					new MallWatchingLogManager().Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_LIAISE_FACEBOOK_MALL,
						facebookMallSetting.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
						"メンテナンス時間帯のため処理を実行しませんでした。");
					isMaintenance = true;
				}
			}
			return isMaintenance;
		}

		/// <summary>実行開始時間</summary>
		private DateTime BeginTime { get; set; }
	}
}