/*
=========================================================================================================
  Module      : 商品セールサービス (ProductSaleService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;

namespace w2.Domain.ProductSale
{
	/// <summary>
	/// 商品セールサービス
	/// </summary>
	public class ProductSaleService : ServiceBase, IProductSaleService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productsaleId">商品セールID</param>
		/// <returns>モデル</returns>
		public ProductSaleModel Get(string shopId, string productsaleId)
		{
			using (var repository = new ProductSaleRepository())
			{
				var model = repository.Get(shopId, productsaleId);
				return model;
			}
		}
		#endregion

		#region +GetValidAll 取得（有効）
		/// <summary>
		/// 取得（有効なレコード）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル配列</returns>
		public ProductSaleModel[] GetValidAll(string shopId)
		{
			using (var repository = new ProductSaleRepository())
			{
				return repository.GetValidAll(shopId);
			}
		}
		#endregion

		#region +GetValidAllByProductSaleKbn 取得（商品セール区分が条件）
		/// <summary>
		/// 取得（商品セール区分が条件）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleKbn">商品セール区分</param>
		/// <returns>モデル配列</returns>
		public ProductSaleModel[] GetValidAllByProductSaleKbn(string shopId, string productSaleKbn)
		{
			using (var repository = new ProductSaleRepository())
			{
				return repository.GetValidAllByProductSaleKbn(shopId, productSaleKbn);
			}
		}
		#endregion

		#region +GetProductSaleCount
		/// <summary>
		/// Get product sale count
		/// </summary>
		/// <param name="productSale">Product sale</param>
		/// <returns>Number</returns>
		public int GetProductSaleCount(Hashtable productSale)
		{
			using (var repository = new ProductSaleRepository())
			{
				var count = repository.GetProductSaleCount(productSale);
				return count;
			}
		}
		#endregion

		#region +GetProductSaleList
		/// <summary>
		/// Get product sale list
		/// </summary>
		/// <param name="productSale">Product sale</param>
		/// <returns>Data product sale</returns>
		public DataView GetProductSaleList(Hashtable productSale)
		{
			using (var repository = new ProductSaleRepository())
			{
				return repository.GetProductSaleList(productSale);
			}
		}
		#endregion

		/// <summary>
		/// 期間内に更新された商品セール情報を取得
		/// </summary>
		/// <param name="begin">開始日時</param>
		/// <param name="end">終了日時</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>商品セール情報（店舗IDと商品セールIDのペアの配列）</returns>
		public int RegisterProductSalePriceTmpByDateChanged(
			DateTime begin,
			DateTime end,
			string lastChanged)
		{
			var updated = 0;
			using (var repository = new ProductSaleRepository())
			{
				var kvps = repository.GetUpdatedProductSalePriceByDateChanged(begin, end);
				foreach (var kvp in kvps)
				{
					updated += RegisterProductSalePriceTmp(kvp.Key, kvp.Value, lastChanged);
				}
			}
			return updated;
		}

		#region +商品セール価格取得
		/// <summary>
		/// 商品セール価格取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>新注文ID</returns>
		public decimal? GetProductSalePrice(string shopId, string productSaleId, string productId, string variationId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductSaleRepository(accessor))
			{
				return repository.GetProductSalePrice(shopId, productSaleId, productId, variationId);
			}
		}
		#endregion

		/// <summary>
		/// 商品セール価格テンポラリデータ登録
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <returns>更新件数</returns>
		public int RegisterProductSalePriceTmp(string shopId, string productSaleId, string lastChanged)
		{
			int updated;
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				updated = RegisterProductSalePriceTmp(shopId, productSaleId, lastChanged, accessor);

				accessor.CommitTransaction();
			}

			return updated;
		}
		/// <summary>
		/// 商品セール価格テンポラリデータ登録
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <param name="lastChanged">最終更新者</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int RegisterProductSalePriceTmp(
			string shopId,
			string productSaleId,
			string lastChanged,
			SqlAccessor accessor)
		{
			DeleteProductSalePriceTmpBySaleId(shopId, productSaleId, accessor);

			using (var repository = new ProductSaleRepository(accessor))
			{
				var updated = repository.InsertFromProductSalePriceTmp(shopId, productSaleId, lastChanged);
				return updated;
			}
		}

		/// <summary>
		/// 商品セール価格テンポラリデータ削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productSaleId">商品セールID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>削除件数</returns>
		public int DeleteProductSalePriceTmpBySaleId(string shopId, string productSaleId, SqlAccessor accessor = null)
		{
			using (var repository = new ProductSaleRepository(accessor))
			{
				var deleted = repository.DeleteProductSalePriceTmpBySaleId(shopId, productSaleId);
				return deleted;
			}
		}

		#region +マスタ出力
		/// <summary>
		///  CSVへ出力
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
			using (var repository = new ProductSaleRepository(accessor))
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
			using (var repository = new ProductSaleRepository())
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
				using (var repository = new ProductSaleRepository())
				{
					repository.CheckProductSalePriceFieldsForGetMaster(
						new Hashtable { { Constants.FIELD_PRODUCTSALE_SHOP_ID, shopId } },
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
	}
}
