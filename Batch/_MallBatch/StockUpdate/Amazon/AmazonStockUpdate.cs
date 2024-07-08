/*
=========================================================================================================
  Module      : Amazon在庫連携クラス(AmazonStockUpdate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System.Linq;
using AmazonMarketPlaceWebService;
using w2.App.Common.MallCooperation;
using w2.Domain.MallCooperationSetting;
using w2.Domain.MallCooperationUpdateLog;
using w2.Domain.Product;

namespace w2.Commerce.MallBatch.StockUpdate.Amazon
{
	/// <summary>
	/// Amazon在庫連携クラス
	/// </summary>
	public class AmazonStockUpdate
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="amazonApi">AmazonAPI</param>
		/// <param name="amazonMallSetting">Amazonモール連携設定</param>
		public AmazonStockUpdate(AmazonApiService amazonApi, MallCooperationSettingModel amazonMallSetting)
		{
			this.AmazonApi = amazonApi;
			this.AmazonMallSetting = amazonMallSetting;
		}
		#endregion

		#region +Exec 実行
		/// <summary>
		/// 実行
		/// </summary>
		public void Exec()
		{
			var mallWatchingLogManager = new MallWatchingLogManager();
			// 在庫連携対象外の場合、ステータスを対象外に更新する
			if (this.AmazonMallSetting.StockUpdateUseFlg == Constants.FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_INVALID)
			{
				new MallCooperationUpdateLogService().UpdateExcludedStockUpdate(Constants.CONST_DEFAULT_SHOP_ID, this.AmazonMallSetting.MallId);

				mallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					this.AmazonMallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
					"Amazon在庫連携利用無しのため処理対象外");
				return;
			}

			// 在庫連携処理対象取得
			var products = GetTargetStockUpdate();
			if (products.Count() == 0)
			{
				mallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					this.AmazonMallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
					string.Format("処理件数：{0}件", products.Count()),
					"在庫連携対象件数0件のため処理を終了しました。");
				return;
			}

			// 「処理実行中」に更新
			UpdateActionStatus(Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_STATUS_ACTIVE);

			// 在庫連携
			var submitFeedResponce = this.AmazonApi.InvokeSubmitFeedForProduct(Constants.AMAZON_MALL_FEED_TYPE_INVENTORY_AVAILABILITY, products);

			// 処理結果判定
			if ((submitFeedResponce != null) && submitFeedResponce.IsSetSubmitFeedResult() 
				&& (submitFeedResponce.SubmitFeedResult.FeedSubmissionInfo.FeedProcessingStatus == Constants.FEED_PROCESSING_STATUS_SUBMITTED))
			{
				mallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					this.AmazonMallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
					string.Format("処理件数：{0}件", products.Count()),
					"Amazon在庫更新処理に成功しました。");
			}
			else
			{
				mallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					this.AmazonMallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
					string.Format("処理件数：{0}件", products.Count()),
					"Amazon在庫更新処理に失敗しました。");
			}

			// 処理後更新
			UpdateActionStatus(Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_STATUS_COMPLETE);
		}
		#endregion

		#region -GetTargetStockUpdate 在庫連携対象抽出
		/// <summary>
		/// 在庫連携対象抽出
		/// </summary>
		/// <returns>在庫連携対象</returns>
		private ProductModel[] GetTargetStockUpdate()
		{
			// AmazonSKU取得用SELECT文生成
			var amazonSkuColumn = string.Format(
				"w2_ProductVariationView.{0} as amazon_sku_use_variation, w2_ProductVariationView.variation_{1} as amazon_sku_no_variation",
				Constants.SELLERSKU_FOR_USE_VARIATION,
				Constants.SELLERSKU_FOR_NO_VARIATION);

			// 出荷作業日数取得用SELECT文生成
			var fulfillmentLatencyColumn = string.Concat(
					string.IsNullOrEmpty(Constants.FULFILLMENTLATENCY_FOR_NO_VARIATION)
						? string.Empty : string.Format(",w2_ProductVariationView.variation_{0} as fulfillmentlatency_no_variation", Constants.FULFILLMENTLATENCY_FOR_NO_VARIATION),
					string.IsNullOrEmpty(Constants.FULFILLMENTLATENCY_FOR_USE_VARIATION)
						? string.Empty : string.Format(",w2_ProductVariationView.{0} as fulfillmentlatency_use_variation", Constants.FULFILLMENTLATENCY_FOR_USE_VARIATION));

			// 処理対象を取得
			var result = new MallCooperationUpdateLogService().GetTargetStockUpdate(
				this.AmazonMallSetting.ShopId,
				this.AmazonMallSetting.MallId,
				amazonSkuColumn,
				fulfillmentLatencyColumn);
			this.TargetLogNoList = result.Select(i => (i.LogNo).ToString()).ToArray();

			// 商品の重複を削除してリストにする
			var targetProduct = result.Select(model => model)
				.GroupBy(model => new { model.ProductId, model.VariationId })
				.Select(g => g.FirstOrDefault())
				.ToArray();
			var productList = targetProduct.Select
				(item =>
				new ProductModel
				{
					ProductId = item.ProductId,
					VariationId = item.VariationId,
					AmazonSKU = (item.UseVariationFlg == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE)
						? item.AmazonSkuUseVariation
						: item.AmazonSkuNoVariation,
					FulfillmentLatency = (item.UseVariationFlg == Constants.FLG_PRODUCT_USE_VARIATION_FLG_USE_USE)
						? (string.IsNullOrEmpty(item.FulfillmentLatencyUseVariation) == false) ? int.Parse(item.FulfillmentLatencyUseVariation) : Constants.FULFILLMENTLATENCY_DEFAULT
						: (string.IsNullOrEmpty(item.FulfillmentLatencyNoVariation) == false) ? int.Parse(item.FulfillmentLatencyNoVariation) : Constants.FULFILLMENTLATENCY_DEFAULT,
					Stock = item.Stock
				})
				.ToArray();
			return productList;
		}
		#endregion

		#region -UpdateActionStatus 処理ステータス更新
		/// <summary>
		/// 処理ステータス更新
		/// </summary>
		/// <param name="actionStatus">処理ステータス</param>
		private void UpdateActionStatus(string actionStatus)
		{
			var model = new MallCooperationUpdateLogModel
			{
				ShopId = this.AmazonMallSetting.ShopId,
				MallId = this.AmazonMallSetting.MallId,
				MasterKbn = Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCTSTOCK,
				ActionKbn = Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_UPDATE,
				ActionStatus = actionStatus
			};

			var updateLogNoList = string.Format(
				"w2_MallCooperationUpdateLog.log_no in ({0})",
				string.Join(",", this.TargetLogNoList.Select(i => i)));

			new MallCooperationUpdateLogService().UpdateActionStatus(model, updateLogNoList);
		}
		#endregion

		#region プロパティ
		/// <summary>AmazonAPIサービス</summary>
		private AmazonApiService AmazonApi { get; set; }
		/// <summary>Amazonモール連携設定</summary>
		private MallCooperationSettingModel AmazonMallSetting { get; set; }
		/// <summary>処理対象ログNoリスト</summary>
		private string[] TargetLogNoList { get; set; }
		#endregion
	}
}
