/*
=========================================================================================================
  Module      : 商品在庫サービスのインターフェース (IProductStockService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
 */
using System;
using System.Collections;
using System.IO;
using w2.Common.Sql;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.Order;

namespace w2.Domain.ProductStock
{
	/// <summary>
	/// 商品在庫サービスのインターフェース
	/// </summary>
	public interface IProductStockService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">商品バリエーションID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		ProductStockModel Get(
			string shopId,
			string productId,
			string variationId,
			SqlAccessor accessor = null);

		/// <summary>
		/// バリエーション毎含む在庫数を取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>在庫管理する:在庫数 在庫管理しない:半角ハイフン</returns>
		string GetSum(string shopId, string productId, SqlAccessor accessor = null);

		/// <summary>
		/// Is exist
		/// </summary>
		/// <param name="shopId">Shop ID</param>
		/// <param name="productId">Product ID</param>
		/// <param name="variationId">Variation ID</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>True if product stock is existed, otherwise false</returns>
		bool IsExist(
			string shopId,
			string productId,
			string variationId,
			SqlAccessor accessor = null);

		/// <summary>
		/// Insert
		/// </summary>
		/// <param name="model">Model</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int Insert(ProductStockModel model, SqlAccessor accessor);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		int Update(ProductStockModel model, SqlAccessor accessor = null);

		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="variationId">バリエーションID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		int Modify(
			string shopId,
			string productId,
			string variationId,
			Action<ProductStockModel> updateAction,
			SqlAccessor accessor = null);

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
		ProductStockModel UpdateProductStockAndGetStock(
			string shopId,
			string productId,
			string variationId,
			int orderQuantity,
			string lastChanged,
			SqlAccessor accessor = null);

		/// <summary>
		/// 論理在庫のキャンセル
		/// </summary>
		/// <param name="orderItems">注文商品</param>
		/// <param name="productStockHistoryActionStatus">商品在庫履歴アクションステータス</param>
		/// <param name="lastChanged"></param>
		/// <param name="accessor"></param>
		void UpdateProductStockCancel(
			OrderItemModel[] orderItems,
			string productStockHistoryActionStatus,
			string lastChanged,
			SqlAccessor accessor);

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
		int AddProductStock(
			string shopId,
			string productId,
			string variationId,
			int itemQuantity,
			int realStock,
			int realStockB,
			int realStockC,
			int realStockReserved,
			string lastChanged,
			SqlAccessor accessor = null);

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
		bool ExportToCsv(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice);

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
		bool ExportToExcel(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			ExcelTemplateSetting excelTemplateSetting,
			Stream outputStream,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice);

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="shopId">ショップID</param>
		/// <returns>チェックOKか</returns>
		bool CheckFieldsForGetMaster(string sqlFieldNames, string shopId);

		/// <summary>
		/// Delete product stock
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="ignoredVariationIds">Ignored variation ids</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int DeleteProductStock(
			string shopId,
			string productId,
			string[] ignoredVariationIds,
			SqlAccessor accessor);

		/// <summary>
		/// Delete product stock all
		/// </summary>
		/// <param name="shopId">Shop id</param>
		/// <param name="productId">Product id</param>
		/// <param name="accessor">SQL accessor</param>
		/// <returns>Number of rows affected</returns>
		int DeleteProductStockAll(string shopId, string productId, SqlAccessor accessor);
	}
}
