/*
=========================================================================================================
  Module      : 商品カテゴリサービス (ProductCategoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;

namespace w2.Domain.ProductCategory
{
	/// <summary>
	/// 商品カテゴリサービス
	/// </summary>
	public class ProductCategoryService : ServiceBase, IProductCategoryService
	{
		#region +GetAll 全て取得
		/// <summary>
		/// 全て取得
		/// </summary>
		/// <returns>モデル</returns>
		public ProductCategoryModel[] GetAll()
		{
			using (var repository = new ProductCategoryRepository())
			{
				var models = repository.GetAll();
				return models;
			}
		}
		# endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>モデル</returns>
		public ProductCategoryModel Get(string categoryId)
		{
			using (var repository = new ProductCategoryRepository())
			{
				return repository.Get(categoryId);
			}
		}
		#endregion

		#region +GetByCategoryId 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="categoryIds">カテゴリID</param>
		/// <returns>モデル</returns>
		public ProductCategoryModel[] GetByCategoryIds(string[] categoryIds)
		{
			using (var repository = new ProductCategoryRepository())
			{
				var models = repository.GetByCategoryIds(categoryIds);
				return models;
			}
		}
		# endregion

		#region +CheckCategoryByFixedPurchaseMemberFlg
		/// <summary>
		/// Check Category By Fixed Purchase Member Flg
		/// </summary>
		/// <param name="categoryId">The category Id</param>
		/// <param name="fixedPurchaseMemberFlg">The Fixed Purchase Member Flg</param>
		/// <returns>Is Access (True: Allow/ Flase: Deny</returns>
		public bool CheckCategoryByFixedPurchaseMemberFlg(string categoryId, string fixedPurchaseMemberFlg)
		{
			using (var repository = new ProductCategoryRepository())
			{
				return repository.CheckCategoryByFixedPurchaseMemberFlg(categoryId, fixedPurchaseMemberFlg);
			}
		}
		# endregion

		#region +CheckValidProductCategory 有効なカテゴリID存在チェック
		/// <summary>
		/// 有効なカテゴリID存在チェック
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="productCategoryIdsList">商品カテゴリIDリスト</param>
		/// <returns>有効なカテゴリID</returns>
		public string[] CheckValidProductCategory(
			string shopId,
			string[] productCategoryIdsList)
		{
			using (var repository = new ProductCategoryRepository())
			{
				var categoryIds = repository.CheckValidProductCategory(
					shopId,
					productCategoryIdsList);
				return categoryIds;
			}
		}
		# endregion

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
			using (var repository = new ProductCategoryRepository(accessor))
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
			using (var repository = new ProductCategoryRepository())
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
				using (var repository = new ProductCategoryRepository())
				{
					repository.CheckProductCategoryFieldsForGetMaster(
						new Hashtable { { Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, shopId } },
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

		#region +小カテゴリリスト取得
		/// <summary>
		/// 小カテゴリリスト取得
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="shopId">ショップID</param>
		/// <returns>チェックOKか</returns>
		public bool GetAllowedChildCategories(string sqlFieldNames, string shopId)
		{
			try
			{
				using (var repository = new ProductCategoryRepository())
				{
					repository.CheckProductCategoryFieldsForGetMaster(
						new Hashtable { { Constants.FIELD_PRODUCTCATEGORY_SHOP_ID, shopId } },
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
