/*
=========================================================================================================
  Module      : 商品レビューサービス (ProductReviewService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;

namespace w2.Domain.ProductReview
{
	/// <summary>
	/// 商品レビューサービス
	/// </summary>
	public class ProductReviewService : ServiceBase
	{
		#region +Get 商品レビュー取得
		/// <summary>
		/// 商品レビューの取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="reviewNo">レビュー番号</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>商品レビュー</returns>
		public ProductReviewModel Get(string shopId, string productId, string reviewNo, SqlAccessor accessor)
		{
			using (var repository = new ProductReviewRepository(accessor))
			{
				return repository.Get(shopId, productId, reviewNo);
			}
		}
		#endregion

		#region 
		/// <summary>
		/// 生成した新しいレビュー番号の取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productId">商品ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>レビュー生成番号</returns>
		public int GetNewReviewNo(string shopId, string productId, SqlAccessor accessor)
		{
			using (var repository = new ProductReviewRepository(accessor))
			{
				return repository.GetNewReviewNo(shopId, productId);
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
			using (var repository = new ProductReviewRepository(accessor))
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
			using (var repository = new ProductReviewRepository())
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
				using (var repository = new ProductReviewRepository())
				{
					repository.CheckProductReviewFieldsForGetMaster(
						new Hashtable { { Constants.FIELD_PRODUCTREVIEW_SHOP_ID, shopId } },
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

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセス</param>
		/// <returns></returns>
		public int Insert(ProductReviewModel model, SqlAccessor accessor)
		{
			using (var repository = new ProductReviewRepository(accessor))
			{
				return repository.Insert(model);
			}
		}
		#endregion
		
		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセス</param>
		/// <returns>更新した件数</returns>
		public int Update(ProductReviewModel model, SqlAccessor accessor)
		{
			using (var repository = new ProductReviewRepository(accessor))
			{
				return repository.Update(model);
			}
		}
		#endregion

	}
}