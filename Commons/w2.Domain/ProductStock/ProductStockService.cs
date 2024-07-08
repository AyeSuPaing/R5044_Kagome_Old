/*
=========================================================================================================
  Module      : 商品在庫サービス (ProductStockService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.Order;
using w2.Domain.Product;
using w2.Domain.ProductStockHistory;

namespace w2.Domain.ProductStock
{
	/// <summary>
	/// 商品在庫サービス
	/// </summary>
	public class ProductStockService : ServiceBase, IProductStockService
	{
		/*
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(ProductStockListSearchCondition condition)
		{
			using (var repository = new ProductStockRepository())
			{
				var count = repository.GetSearchHitCount(condition);
				return count;
			}
		}
		#endregion
		*/
		/*
		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public ProductStockListSearchResult[] Search(ProductStockListSearchCondition condition)
		{
			using (var repository = new ProductStockRepository())
			{
				var results = repository.Search(condition);
				return results;
			}
		}
		#endregion
		*/

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public ProductStockModel Get(string shopId, string productId, string variationId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductStockRepository(accessor))
			{
				var model = repository.Get(shopId, productId, variationId);
				return model;
			}
		}
		#endregion

		#region +GetSum バリエーション毎含む在庫数を取得取得
		/// <summary>
		/// バリエーション毎含む在庫数を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>在庫管理する:在庫数 在庫管理しない:半角ハイフン</returns>
		public string GetSum(string shopId, string productId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductStockRepository(accessor))
			{
				var stock = repository.GetSum(shopId, productId);
				return stock;
			}
		}
		#endregion

		#region +FlapsInsert FLAPS在庫連携による挿入
		/// <summary>
		/// FLAPS在庫連携による挿入
		/// </summary>
		/// <param name="productStock">商品在庫モデル</param>
		/// <returns>挿入結果</returns>
		public bool FlapsInsert(ProductStockModel productStock)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new ProductStockRepository(accessor))
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var insertResult = (repository.Insert(productStock) > 0);
				if (insertResult == false) return false;

				var historyUpdateResult = FlapsUpdateHistory(productStock, accessor);
				if (historyUpdateResult == false) return false;

				accessor.CommitTransaction();
				
				return true;
			}
		}
		#endregion

		#region +FlapsUpdate FLAPS在庫連携による更新
		/// <summary>
		/// FLAPS在庫連携による更新
		/// </summary>
		/// <param name="productStock">モデル</param>
		public bool FlapsUpdate(ProductStockModel productStock)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updateResult = (Update(productStock, accessor) > 0);
				if (updateResult == false) return false;

				var historyUpdateResult = FlapsUpdateHistory(productStock, accessor);
				if (historyUpdateResult == false) return false;
				
				accessor.CommitTransaction();

				return true;
			}
		}
		#endregion

		#region ~FlapsUpdateHistory FLAPS連携後の変更履歴更新
		/// <summary>
		/// FLAPS連携後の変更履歴更新
		/// </summary>
		/// <param name="productStock">モデル</param>
		/// <param name="accessor">アクセサ</param>
		/// <returns>結果</returns>
		internal bool FlapsUpdateHistory(ProductStockModel productStock, SqlAccessor accessor)
		{
			var history = new ProductStockHistoryModel
			{
				ShopId = Constants.CONST_DEFAULT_SHOP_ID,
				OrderId = "",
				ProductId = productStock.ProductId,
				VariationId = productStock.VariationId,
				ActionStatus = Constants.FLG_PRODUCTSTOCKHISTORY_ACTION_STATUS_STOCK_EXTERNAL_API,
				UpdateStock = productStock.Stock,
				UpdateMemo = "",
				LastChanged = productStock.LastChanged
			};
			var historyUpdateResult = (new ProductStockHistoryService().Insert(history, accessor) > 0);
			if (historyUpdateResult == false)
			{
				var msg = string.Format(
					"在庫履歴を更新できませんでした。variation_id: {0}, update_stock: {1}",
					productStock.VariationId,
					productStock.Stock);
				FileLogger.WriteError(msg);
				return false;
			}

			return true;
		}
		#endregion

		#region +IsExist
		/// <summary>
		/// Is exist
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="variationId">Variation ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>True if product stock is existed, otherwise false</returns>
		public bool IsExist(
			string shopId,
			string productId,
			string variationId,
			SqlAccessor accessor = null)
		{
			using (var repository = new ProductStockRepository(accessor))
			{
				var result = repository.IsExist(shopId, productId, variationId);
				return result;
			}
		}
		#endregion

		#region +Insert
		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int Insert(ProductStockModel model, SqlAccessor accessor)
		{
			using (var repository = new ProductStockRepository(accessor))
			{
				var result = repository.Insert(model);
				return result;
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(ProductStockModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ProductStockRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region Modify 更新（汎用的に利用）
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(
			string shopId,
			string productId,
			string variationId,
			Action<ProductStockModel> updateAction,
			SqlAccessor accessor = null)
		{
			// 最新データ取得
			var product = Get(shopId, productId, variationId, accessor);

			// モデル内容更新
			updateAction(product);

			// 更新
			int updated;
			using (var repository = new ProductStockRepository(accessor))
			{
				updated = repository.Update(product);
			}
			return updated;
		}
		#endregion

		#region +UpdateProductStockAndGetStock 在庫数更新(更新後在庫数取得)
		/// <summary>
		/// 在庫数更新(更新後在庫数取得)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="orderQuantity">注文数量</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品在庫モデル</returns>
		public ProductStockModel UpdateProductStockAndGetStock(string shopId, string productId, string variationId, int orderQuantity, string lastChanged, SqlAccessor accessor = null)
		{
			using (var repository = new ProductStockRepository(accessor))
			{
				var updated = Modify(
					shopId,
					productId,
					variationId,
					(ProductStockModel) =>
					{
						ProductStockModel.Stock = ProductStockModel.Stock - orderQuantity;
						ProductStockModel.LastChanged = lastChanged;
					});
				var result = Get(shopId, productId, variationId, accessor);
				return result;
			}
		}
		#endregion

		#region +UpdateProductStockCancel 論理在庫のキャンセル
		/// <summary>
		/// 論理在庫のキャンセル
		/// </summary>
		/// <param name="orderItems">注文商品</param>
		/// <param name="productStockHistoryActionStatus">商品在庫履歴アクションステータス</param>
		/// <param name="lastChanged"></param>
		/// <param name="accessor"></param>
		public void UpdateProductStockCancel(OrderItemModel[] orderItems, string productStockHistoryActionStatus, string lastChanged, SqlAccessor accessor)
		{
			var productService = new ProductService();
			var productStockHistoryService = new ProductStockHistoryService();
			orderItems.Where(item => ((item.IsReturnItem == false)
				|| productService.IsStockManagement(item.ShopId, item.ProductId, item.VariationId, string.Empty, accessor)))
				.ToList()
				.ForEach(item =>
				{
					AddProductStock(
						item.ShopId,
						item.ProductId,
						item.VariationId,
						item.ItemQuantity,
						0,
						0,
						0,
						0,
						lastChanged,
						accessor);
					productStockHistoryService.Insert(
						new ProductStockHistoryModel
						{
							OrderId = item.OrderId,
							ShopId = item.ShopId,
							ProductId = item.ProductId,
							VariationId = item.VariationId,
							ActionStatus = productStockHistoryActionStatus,
							AddStock = item.ItemQuantity,
							AddRealstock = 0,
							AddRealstockB = 0,
							AddRealstockC = 0,
							AddRealstockReserved = 0,
							UpdateMemo = string.Empty,
							LastChanged = lastChanged,
						});
				});
		}
		#endregion

		#region +AddProductStock 商品在庫情報の論理在庫・実在庫・引当済実在庫数更新
		/// <summary>
		/// 商品在庫情報の論理在庫・実在庫・引当済実在庫数更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="itemQuantity">注文数</param>
		/// <param name="realStock">実A在庫数</param>
		/// <param name="realStockB">実B在庫数</param>
		/// <param name="realStockC">実C在庫数</param>
		/// <param name="realStockReserved">引当済実在庫数</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int AddProductStock(
			string shopId,
			string productId,
			string variationId,
			int itemQuantity,
			int realStock,
			int realStockB,
			int realStockC,
			int realStockReserved,
			string lastChanged,
			SqlAccessor accessor = null)
		{
			using (var repository = new ProductStockRepository(accessor))
			{
				return repository.AddProductStock(
					shopId,
					productId,
					variationId,
					itemQuantity,
					realStock,
					realStockB,
					realStockC,
					realStockReserved,
					lastChanged);
			}
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// CSVへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か</returns>
		public bool ExportToCsv(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			using (var accessor = new SqlAccessor())
			using (var repository = new ProductStockRepository(accessor))
			using (var reader = repository.GetMasterWithReader(
				input,
				new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames)))
			{
				new MasterExportCsv().Exec(setting, reader, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
			}
			return true;
		}

		/// <summary>
		/// Excelへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="excelTemplateSetting">Excelテンプレート設定</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か（件数エラーの場合は失敗）</returns>
		public bool ExportToExcel(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			ExcelTemplateSetting excelTemplateSetting,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			using (var repository = new ProductStockRepository())
			{
				var dv = repository.GetMaster(input, new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				if (dv.Count >= 20000) return false;

				new MasterExportExcel().Exec(setting, excelTemplateSetting, dv, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				return true;
			}
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="shopId">ショップID</param>
		/// <returns>チェックOKか</returns>
		public bool CheckFieldsForGetMaster(string sqlFieldNames, string shopId)
		{
			try
			{
				using (var repository = new ProductStockRepository())
				{
					repository.CheckProductStockFieldsForGetMaster(
						new Hashtable { { Constants.FIELD_PRODUCTSTOCK_SHOP_ID, shopId } },
						new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				}
			}
			catch (Exception ex)
			{
				AppLogger.WriteWarn(ex);
				return false;
			}
			return true;
		}
		#endregion

		#region +DeleteProductStock
		/// <summary>
		/// Delete product stock
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="ignoredVariationIds">Ignored variation ids</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int DeleteProductStock(
			string shopId,
			string productId,
			string[] ignoredVariationIds,
			SqlAccessor accessor)
		{
			using (var repository = new ProductStockRepository(accessor))
			{
				var result = repository.DeleteProductStock(
					shopId,
					productId,
					ignoredVariationIds);
				return result;
			}
		}
		#endregion

		#region +DeleteProductStockAll
		/// <summary>
		/// Delete product stock all
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		public int DeleteProductStockAll(string shopId, string productId, SqlAccessor accessor)
		{
			using (var repository = new ProductStockRepository(accessor))
			{
				var result = repository.DeleteProductStockAll(shopId, productId);
				return result;
			}
		}
		#endregion
	}
}
