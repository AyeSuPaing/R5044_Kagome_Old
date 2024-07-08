/*
=========================================================================================================
  Module      : Lohaco在庫連携クラス(LohacoStockUpdate.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using w2.App.Common.LohacoCreatorWebApi;
using w2.App.Common.LohacoCreatorWebApi.StockEdit;
using w2.App.Common.MallCooperation;
using w2.Common.Util;
using w2.Domain.MallCooperationSetting;
using w2.Domain.MallCooperationUpdateLog;
using w2.Domain.Product;

namespace w2.Commerce.MallBatch.StockUpdate.Lohaco
{
	/// <summary>
	/// Lohaco在庫連携クラス
	/// </summary>
	public class LohacoStockUpdate
	{
		#region デフォルトコンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		/// <param name="setting">Lohacoモール連携設定</param>
		public LohacoStockUpdate(MallCooperationSettingModel setting)
		{
			this.LohacoMallSetting = setting;
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
			if (this.LohacoMallSetting.StockUpdateUseFlg == Constants.FLG_MALLCOOPERATIONSETTING_STOCK_UPDATE_USE_FLG_INVALID)
			{
				new MallCooperationUpdateLogService().UpdateExcludedStockUpdate(Constants.CONST_DEFAULT_SHOP_ID, this.LohacoMallSetting.MallId);

				mallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					this.LohacoMallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
					"Lohaco在庫連携利用無しのため処理対象外");
				return;
			}

			// 在庫連携処理対象取得
			var products = GetTargetStockUpdate();
			if (products.Length == 0)
			{
				mallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					this.LohacoMallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
					string.Format("処理件数：{0}件", products.Length),
					"在庫連携対象件数0件のため処理を終了しました。");
				return;
			}

			// 「処理実行中」に更新
			UpdateActionStatus(Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_STATUS_ACTIVE);

			// 在庫連携
			BaseErrorResponse errorResponse;
			var stockEditResponse = UpdateLohacoProductStock(products, out errorResponse);

			// 処理結果判定
			if ((stockEditResponse != null)
				&& (stockEditResponse.ListResult != null)
				&& (stockEditResponse.ListResult.Results != null)
				&& (stockEditResponse.ListResult.Results.Count > 0))
			{
				var log = new StringBuilder();
				var warningCount = 0;
				var errorCount = 0;
				var errorProductList = new List<string>();
				foreach (var result in stockEditResponse.ListResult.Results)
				{
					switch (result.Status)
					{
						case LohacoConstants.StockStatus.Error:
						case LohacoConstants.StockStatus.Warning:
							errorCount += (result.Status == LohacoConstants.StockStatus.Error) ? 1 : 0;
							warningCount += (result.Status == LohacoConstants.StockStatus.Warning) ? 1 : 0;
							errorProductList.Add(result.ItemCd);
							log.AppendLine(string.Format(
								@"・バリエーションID'{0}'の在庫更新に失敗しました。エラー詳細：エラー内容：{1}、エラーコード：{2}、エラーメッセージ：{3}、データチェック日時：{4}。",
								StringUtility.ToEmpty(result.ItemCd),
								StringUtility.ToEmpty(result.Content),
								StringUtility.ToEmpty(result.ErrorCode),
								StringUtility.ToEmpty(result.ErrorMessage),
								StringUtility.ToDateString(result.CheckDate, Constants.JAPAN_DATE_FORMAT_LONG)));
							break;

						case LohacoConstants.StockStatus.Success:
						default:
							break;
					}
				}
				if (warningCount + errorCount > 0)
				{
					mallWatchingLogManager.Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
						this.LohacoMallSetting.MallId,
						(errorCount > 0) ? Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR : Constants.FLG_MALLWATCHINGLOG_LOG_KBN_WARNING,
						string.Format(
							"処理合計件数：{0}件、成功：{1}件、警告：{2}件、エラー：{3}件。{4}{5}",
							products.Length,
							products.Length - errorCount - warningCount,
							warningCount,
							errorCount,
							Environment.NewLine,
							log));
				}
				else
				{
					// 全成功
					mallWatchingLogManager.Insert(
						Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
						this.LohacoMallSetting.MallId,
						Constants.FLG_MALLWATCHINGLOG_LOG_KBN_SUCCESS,
						string.Format("処理合計件数：{0}件、成功：{1}件", products.Length, products.Length));
				}

				// 処理後更新、警告・エラー商品を除外する
				UpdateActionStatus(Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_STATUS_COMPLETE, errorProductList);
			}
			else
			{
				mallWatchingLogManager.Insert(
					Constants.FLG_MALLWATCHINGLOG_BATCH_ID_STOCKUPDATE,
					this.LohacoMallSetting.MallId,
					Constants.FLG_MALLWATCHINGLOG_LOG_KBN_ERROR,
					string.Format("処理件数：{0}件", products.Length),
					string.Format(
						"Lohaco在庫更新処理に失敗しました。{0}",
						(errorResponse != null) ? string.Format("エラーコード：{0}、エラーメッセージ：{1}", errorResponse.Code, errorResponse.Message) : string.Empty));
			}
		}
		#endregion

		#region -UpdateLohacoProductStock ロハコ商品在庫の更新
		/// <summary>
		/// ロハコ商品在庫の更新
		/// </summary>
		/// <param name="productList">更新対象商品一覧</param>
		/// <param name="errorResponse">基礎エラーレスポンス情報（基礎エラーレスポンスがあれば場合）</param>
		/// <returns>在庫管理APIのレスポンス</returns>
		public StockEditResponse UpdateLohacoProductStock(ProductModel[] productList, out BaseErrorResponse errorResponse)
		{
			var targetList = new List<StockEditRequest.StockEditTarget>();
			foreach (var product in productList)
			{
				var target = new StockEditRequest.StockEditTarget
				{
					SellerId = this.LohacoMallSetting.MallId,
					ItemCd = product.VariationId,
					// バリエーションの在庫が在庫安全基準を下回った場合、「0」として連携
					StockQuantity = (product.Stock > product.StockAlert) ? product.Stock.ToString() : 0.ToString(),
				};
				targetList.Add(target);
			}

			var stockEditRequest = new StockEditRequest(this.LohacoMallSetting.MallId)
			{
				Target = targetList,
			};
			var stockEditCommand = new StockEditCommand();
			return stockEditCommand.OnExecute(
				stockEditRequest,
				this.LohacoMallSetting.MallId,
				this.LohacoMallSetting.LohacoPrivateKey,
				out errorResponse,
				Constants.WRITE_DEBUG_LOG_ENABLED,
				Constants.MASK_PERSONAL_INFO_ENABLED);
		}
		#endregion

		#region -GetTargetStockUpdate 在庫連携対象抽出
		/// <summary>
		/// 在庫連携対象抽出
		/// </summary>
		/// <returns>在庫連携対象</returns>
		public ProductModel[] GetTargetStockUpdate()
		{
			// 処理対象を取得
			var result = new MallCooperationUpdateLogService().GetLohacoTargetStockUpdate(
				this.LohacoMallSetting.ShopId,
				this.LohacoMallSetting.MallId);
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
					Stock = item.Stock,
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
		/// <param name="errorProductList">在庫更新エラー一覧</param>
		public void UpdateActionStatus(string actionStatus, List<string> errorProductList = null)
		{
			var model = new MallCooperationUpdateLogModel
			{
				ShopId = this.LohacoMallSetting.ShopId,
				MallId = this.LohacoMallSetting.MallId,
				MasterKbn = Constants.FLG_MALLCOOPERATIONUPDATELOG_MASTER_KBN_PRODUCTSTOCK,
				ActionKbn = Constants.FLG_MALLCOOPERATIONUPDATELOG_ACTION_KBN_UPDATE,
				ActionStatus = actionStatus
			};
			var updateLogNoList = string.Format(
				"w2_MallCooperationUpdateLog.log_no in ({0})",
				string.Join(",", this.TargetLogNoList.Select(i => i)));
			if (errorProductList != null && errorProductList.Count > 0)
			{
				var excludeProductList = string.Format(
					" AND w2_MallCooperationUpdateLog.variation_id NOT IN ('{0}')",
					string.Join("','", errorProductList.ToArray()));
				updateLogNoList += excludeProductList;
			}
			new MallCooperationUpdateLogService().UpdateActionStatus(model, updateLogNoList);
		}
		#endregion

		#region プロパティ
		/// <summary>Lohacoモール連携設定</summary>
		private MallCooperationSettingModel LohacoMallSetting { get; set; }
		/// <summary>処理対象ログNoリスト</summary>
		private string[] TargetLogNoList { get; set; }
		#endregion
	}
}
