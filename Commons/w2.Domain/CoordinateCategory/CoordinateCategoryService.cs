/*
=========================================================================================================
  Module      : コーディネートカテゴリサービス (CoordinateCategoryService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.CoordinateCategory.Helper;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;

namespace w2.Domain.CoordinateCategory
{
	/// <summary>
	/// コーディネートカテゴリサービス
	/// </summary>
	public class CoordinateCategoryService : ServiceBase
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(CoordinateCategoryListSearchCondition condition)
		{
			using (var repository = new CoordinateCategoryRepository())
			{
				var count = repository.GetSearchHitCount(condition);
				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		public CoordinateCategoryListSearchResult[] Search(CoordinateCategoryListSearchCondition condition)
		{
			using (var repository = new CoordinateCategoryRepository())
			{
				var results = repository.Search(condition);
				return results;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="coordinateCategoryId">カテゴリID</param>
		/// <param name="sqlAccessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public CoordinateCategoryModel Get(string coordinateCategoryId, SqlAccessor sqlAccessor = null)
		{
			using (var repository = new CoordinateCategoryRepository(sqlAccessor))
			{
				var model = repository.Get(coordinateCategoryId);
				return model;
			}
		}
		#endregion

		#region +GetCategoryTree カテゴリツリーを取得
		/// <summary>
		/// カテゴリツリーを取得
		/// </summary>
		/// <param name="coordinateCategoryIds">カテゴリID</param>
		/// <returns>モデル</returns>
		public CoordinateCategoryModel[] GetCategoryTree(Hashtable coordinateCategoryIds)
		{
			using (var repository = new CoordinateCategoryRepository())
			{
				var models = repository.GetCategoryTree(coordinateCategoryIds);
				return models;
			}
		}
		#endregion

		#region +GetParentCategories 指定カテゴリIDと、その上位のカテゴリ(TOPカテゴリまで)のリストを取得する
		/// <summary>
		/// カテゴリツリーを取得
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>モデル</returns>
		public CoordinateCategoryModel[] GetParentCategories(string categoryId)
		{
			using (var repository = new CoordinateCategoryRepository())
			{
				var models = repository.GetParentCategories(categoryId);
				return models;
			}
		}
		#endregion

		#region +GetByName 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="coordinateCategoryName">カテゴリID</param>
		/// <returns>モデル</returns>
		public CoordinateCategoryModel GetByName(string coordinateCategoryName)
		{
			using (var repository = new CoordinateCategoryRepository())
			{
				var model = repository.GetByName(coordinateCategoryName);
				return model;
			}
		}
		#endregion

		#region +GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル</returns>
		public CoordinateCategoryModel[] GetAll()
		{
			using (var repository = new CoordinateCategoryRepository())
			{
				var model = repository.GetAll();
				return model;
			}
		}
		#endregion

		#region +GetTopCoordinateCategory 最上位カテゴリ取得
		/// <summary>
		/// 最上位カテゴリ取得
		/// </summary>
		/// <returns>モデル</returns>
		public CoordinateCategoryModel[] GetTopCoordinateCategory()
		{
			using (var repository = new CoordinateCategoryRepository())
			{
				var model = repository.GetTopCoordinateCategory();

				return model;
			}
		}
		#endregion

		#region +GetTopCoordinateCategory コーディネートカテゴリー情報取得(直下の子カテゴリー)
		/// <summary>
		/// コーディネートカテゴリー情報取得(直下の子カテゴリー)
		/// </summary>
		/// <param name="categoryId">カテゴリID</param>
		/// <returns>モデル</returns>
		public CoordinateCategoryModel[] GetCoordinateCategoryFamily(string categoryId)
		{
			using (var repository = new CoordinateCategoryRepository())
			{
				var model = repository.GetCoordinateCategoryFamily(categoryId);
				return model;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(CoordinateCategoryModel model, SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateCategoryRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(CoordinateCategoryModel model, SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateCategoryRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="coordinateCategoryId">カテゴリID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string coordinateCategoryId, SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateCategoryRepository(accessor))
			{
				repository.Delete(coordinateCategoryId);
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
			using (var repository = new CoordinateCategoryRepository(accessor))
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
			string replacePrice,
			int digitsByKeyCurrency)
		{
			using (var repository = new CoordinateCategoryRepository())
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
		/// <returns>チェックOKか</returns>
		public bool CheckFieldsForGetMaster(string sqlFieldNames)
		{
			try
			{
				using (var repository = new CoordinateCategoryRepository())
				{
					repository.CheckFieldsForGetMaster(
						new Hashtable(),
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