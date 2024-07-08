/*
=========================================================================================================
  Module      : FLAPS連携クラス (FlapsIntegrationFacade.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Linq;
using w2.App.Common.Flaps.Logger;
using w2.App.Common.Flaps.Order;
using w2.App.Common.Flaps.Product;
using w2.App.Common.Flaps.ProductStock;
using w2.App.Common.Order;
using w2.Common.Extensions;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.UpdateHistory.Helper;

namespace w2.App.Common.Flaps
{
	/// <summary>
	/// FLAPS連携クラス
	/// </summary>
	public class FlapsIntegrationFacade
	{
		/// <summary>
		/// 注文連携処理
		/// </summary>
		/// <param name="order">注文</param>
		/// <param name="cart">カート情報</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>注文連携を成功した際に受け取るショップカウンターカード</returns>
		public string ProcessOrder(Hashtable order, CartObject cart, SqlAccessor accessor)
		{
			// 注文情報をマッピング
			var flapsOrderReplication = new FlapsOrderReplication(order, cart);

			var response = new OrderResult();
			try
			{
				foreach (var cartGoods in cart.Items)
				{
					var hasStock = HasStock(cartGoods.VariationId, cartGoods.Count);
					if (hasStock) continue;

					FileLogger.WriteError(
						string.Format(
							"注文数分の在庫がありません。状況に応じて、FLAPS管理画面で在庫を増やしてください。variation_id: {0}, quantity: {1}",
							cartGoods.VariationId,
							cartGoods.Count));
					return "";
				}

				// 注文連携実行
				response = flapsOrderReplication.Post();
				if (string.IsNullOrEmpty(response.Message) == false)
				{
					if (response.Message.Contains(Constants.FLG_FLAPS_ORDER_ERROR_MESSAGE_NO_PRODUCT_FOUND)
						&& (flapsOrderReplication.Products.Count == 1))
					{
						var noProductFoundMsg = string.Format(
							"FLAPSで登録されていない商品が購入されました。FLAPS側で登録してください。order_id: {0}, variation_id: {1}, response message: {2}",
							flapsOrderReplication.Identifier,
							flapsOrderReplication.Products[0].GoodsCode,
							response.Message);
						FileLogger.WriteError(noProductFoundMsg);
						return "";
					}

					var msg = string.Format(
						"FLAPS注文連携処理に失敗しました。order_id: {0}, response message: {1}",
						flapsOrderReplication.Identifier,
						response.Message);
					FileLogger.WriteError(msg);
					return "";
				}

				var debugLog = string.Format(
					"FLAPS注文連携処理に成功しました。order_id: {0}, PosNoSerNo: {1}",
					flapsOrderReplication.Identifier,
					response.PosNoSerNo);
				FileLogger.WriteDebug(debugLog);
			}
			catch (Exception e)
			{
				var variationIds = flapsOrderReplication.Products.Select(p => p.GoodsCode).JoinToString(",");
				var msg = string.Format(
					"FLAPS注文連携処理中に例外が発生しました。order_id: {0}, variation_id: {1}, PosNoSerNo: {2}",
					flapsOrderReplication.Identifier,
					variationIds,
					response.PosNoSerNo);
				FileLogger.WriteError(msg, e);
				
				// 例外スローしない。上の階層(OrderPreorderRegister)にてbool値でエラーハンドルするため。
				return "";
			}

			var stored = flapsOrderReplication.StorePosNoSerNo(response.PosNoSerNo, accessor);
			if (stored == false)
			{
				// PosNoSerNoを更新に失敗した場合、API処理を成功として処理する
				// その代わりログに失敗した情報を記録する
				var msg = string.Format(
					"PosNoSerNoの保持に失敗しました。FLAPS管理画面でその値を確認して管理者に伝えてください。order_id: {0}, PosNoSerNo: {1}",
					flapsOrderReplication.Identifier,
					response.PosNoSerNo);
				FileLogger.WriteError(msg);
			}

			return response.PosNoSerNo;
		}

		/// <summary>
		/// 注文キャンセル (在庫量を元に戻す)
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="posNoSerNo">ショップカウンターカード</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>注文キャンセル結果</returns>
		public bool CancelOrder(string orderId, string posNoSerNo = null, SqlAccessor accessor = null)
		{
			var flapsOrderCancellationReplication = new FlapsOrderCancellationReplication
			{
				Identifier = orderId,
				Date = DateTime.Now.ToString("yyyyMMdd"),
				PosNoSerNo = posNoSerNo,
			};
			var result = flapsOrderCancellationReplication.Post(accessor);
			if ((result != null) && string.IsNullOrEmpty(result.ErrorMessage)) return true;

			var msg = string.Format(
				"注文キャンセル処理に失敗しました。FLAPS管理画面からキャンセルしてください。order_id: {0}, PosNoSerNo: {1}",
				orderId,
				flapsOrderCancellationReplication.PosNoSerNo);
			WriteErrorLogOnConsoleAndFile(msg);
			return false;
		}

		/// <summary>
		/// 商品差分更新
		/// </summary>
		public void CaptureChangedProducts()
		{
			var result = new FlapsReplicationResult(Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT);

			// 最終同期日時の記録用
			var recorder = new LastUpdatedCheckpointRecorder(Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT);
			var now = DateTime.Now;
			var checkpoint = recorder.GetLastUpdatedDateTime();

			// 差分データ取得
			CaptureChangedProducts("", checkpoint, result);
			
			// 最終同期日時を記録
			recorder.Record(now);

			// メール送信
			result.NotifyOnEmail();
		}
		/// <summary>
		/// 商品差分更新
		/// </summary>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="result">結果オブジェクト</param>
		/// <returns>結果オブジェクト</returns>
		public FlapsReplicationResult CaptureChangedProducts(string variationId, FlapsReplicationResult result = null)
		{
			result = result ?? new FlapsReplicationResult(Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT);

			CaptureChangedProducts(
				variationId,
				new LastUpdatedCheckpointRecorder(Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCT)
					.GetLastUpdatedDateTime(),
				result);
			return result;
		}
		/// <summary>
		/// 商品差分更新
		/// </summary>
		/// <param name="checkpoint">差分取得日時(この日時よりも後にFLAPS側で更新された商品を取得する)</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="result">結果オブジェクト</param>
		/// <returns>結果オブジェクト</returns>
		public FlapsReplicationResult CaptureChangedProducts(string variationId, string checkpoint, FlapsReplicationResult result)
		{
			// 差分データ取得
			var fr = new FlapsProductsReplication
			{
				Records = Constants.FLAPS_THE_NUMBER_OF_RECORDS_TO_CAPTURE_AT_ONCE,
				DateThreshold = checkpoint,
			};
			var response = fr.Get();

			// APIエラーチェック
			if (string.IsNullOrEmpty(response.ErrorMessage) == false)
			{
				var msg = string.Format(
					"API同期処理に失敗しました。リクエストの値を確認してください。variation_id: {0}, response message: {1}",
					variationId,
					response.ErrorMessage);
				WriteErrorLogOnConsoleAndFile(msg);

				return result;
			}

			// 更新対象がない場合
			if ((response.Goods.Any() == false)
				|| ((response.Goods.Length <= 1) && string.IsNullOrEmpty(response.Goods[0].StyleCode)))
			{
				var msg = string.Format("更新対象がありません。variation_id: {0}, checkpoint: {1}", variationId, checkpoint);
				WriteErrorLogOnConsoleAndFile(msg);
				
				return result;
			}

			// DB更新処理
			var hasVariationId = (string.IsNullOrEmpty(variationId) == false);
			foreach (var product in response.Goods)
			{
				// 商品ID指定して同期場合は、指定した商品のみDBの値を更新する
				if (hasVariationId && (product.StyleCode != variationId)) continue;

				// 一つの更新に失敗しても処理自体を実行する
				try
				{
					var updated = product.Update();
					if (updated == false)
					{
						var msg = string.Format("商品更新に失敗しました。登録された値が正しいか確認してください。variation_id: {0}", product.Code);
						WriteErrorLogOnConsoleAndFile(msg);

						result.CountupFailure();
						continue;
					}
				}
				catch (Exception e)
				{
					// ログを残して処理を終了させない
					result.CountupFailure();
					var msg = string.Format("DB更新に失敗しました。登録された値が正しいか確認してください。variation_id: {0}", product.Code);
					WriteErrorLogOnConsoleAndFile(msg, e);
					continue;
				}
				
				result.CountupSuccess();
			}

			// まだ取得途中の場合(FinishFetch値が0)、リクエストを続ける (再帰処理)
			if (response.FinishFetch == 0)
			{
				CaptureChangedProducts(hasVariationId ? variationId : "", "", result);
			}

			return result;
		}
		
		/// <summary>
		/// 商品在庫同期処理
		/// </summary>
		/// <returns>結果オブジェクト</returns>
		public void CaptureChangedProductStocks()
		{
			var result = new FlapsReplicationResult(Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCTSTOCK);

			// 最終同期日時の記録用
			var now = DateTime.Now;

			// // 差分データ取得
			var recorder = new LastUpdatedCheckpointRecorder(Constants.FLG_FLAPS_REPLICATION_DATA_PRODUCTSTOCK);
			var checkpoint = recorder.GetLastUpdatedDateTime();
			var variationIds = new ProductService().GetProductVariationIds(Constants.CONST_DEFAULT_SHOP_ID);
			var flapsProductStockReplication = new FlapsProductStockReplication
			{
				GoodsCodes = variationIds,
				RequestDateTime = checkpoint,
			};
			var response = flapsProductStockReplication.Get();

			// レスポンスに何かしらのメッセージがあるか、もしくは在庫データがない場合
			if (((string.IsNullOrEmpty(response.ErrorMessage) == false)
					&& (string.IsNullOrEmpty(response.Message) == false))
				|| ((response.GoodsStock == null) || (response.GoodsStock.Any() == false)))
			{
				var msg = string.Format(
					"API同期処理に失敗しました。リクエストの値を確認してください。variation_id: {0}, response message: {1}",
					string.Join(",", variationIds),
					response.ErrorMessage);
				WriteErrorLogOnConsoleAndFile(msg);

				result.NotifyOnEmail();
				return;
			}

			// DB更新処理
			foreach (var productStock in response.GoodsStock)
			{
				var updated = productStock.Update();
				if (updated == false)
				{
					// ログを残してループ処理を終了させない
					result.CountupFailure();
					var msg = string.Format(
						"DB更新に失敗しました。登録された値が正しいか確認してください。variation_id: {0}, quantity: {1}",
						productStock.GoodsCode,
						productStock.Qty);
					WriteErrorLogOnConsoleAndFile(msg);
					
					result.CountupFailure();
					continue;
				}

				result.CountupSuccess();
			}

			// 最終同期日時を記録
			recorder.Record(now);

			// メール送信
			result.NotifyOnEmail();
		}

		/// <summary>
		/// 在庫があるかどうかを確認する
		/// </summary>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="quantity">数量</param>
		/// <returns>在庫があるかどうか</returns>
		public bool HasStock(string variationId, int quantity)
		{
			// 在庫確認
			var flapsProductStockReplication = new FlapsProductStockReplication
			{
				GoodsCodes = new[] { variationId },
			};
			var response = flapsProductStockReplication.Get();

			// レスポンスに何かしらのメッセージがあるか、もしくは在庫データがない場合
			if (((string.IsNullOrEmpty(response.ErrorMessage) == false)
					&& (string.IsNullOrEmpty(response.Message) == false))
				|| ((response.GoodsStock == null) || (response.GoodsStock.Any() == false)))
			{
				var msg = string.Format(
					"API同期処理に失敗しました。リクエストの値を確認してください。variation_id: {0}, response message: {1}",
					string.Join(",", variationId),
					response.ErrorMessage);
				WriteErrorLogOnConsoleAndFile(msg);

				return false;
			}

			return (response.GoodsStock[0].Qty >= quantity);
		}

		/// <summary>
		/// ERP連携済みフラグ
		/// </summary>
		/// <param name="orderId">注文ID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>結果</returns>
		public bool TurnOnErpIntegrationFlg(string orderId, string lastChanged, SqlAccessor accessor)
		{
			var result = new OrderService().UpdateOrderExtendStatus(
				orderId,
				43,
				Constants.FLG_FIELD_FIXEDPURCHASE_EXTEND_STATUS_ON,
				DateTime.Now,
				lastChanged,
				UpdateHistoryAction.DoNotInsert,
				accessor);
			return (result > 0);
		}

		/// <summary>
		/// コンソール、ファイルにログを出力する
		/// </summary>
		/// <param name="msg">メッセージ</param>
		/// <param name="e">例外</param>
		private void WriteErrorLogOnConsoleAndFile(string msg, Exception e = null)
		{
			Console.WriteLine(msg);
			if (e != null) Console.WriteLine(e.Message);
			FileLogger.WriteError(msg, e);
		}
	}
}
