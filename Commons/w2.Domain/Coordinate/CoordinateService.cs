/*
=========================================================================================================
  Module      : コーディネートサービス (CoordinateService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.ContentsTag;
using w2.Domain.Coordinate.Helper;
using w2.Domain.CoordinateCategory;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.Product;

namespace w2.Domain.Coordinate
{
	/// <summary>
	/// コーディネートサービス
	/// </summary>
	public class CoordinateService : ServiceBase
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(CoordinateListSearchCondition condition)
		{
			using (var repository = new CoordinateRepository())
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
		public CoordinateListSearchResult[] Search(CoordinateListSearchCondition condition)
		{
			using (var repository = new CoordinateRepository())
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
		/// <param name="coordinateId">コーディネートID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public CoordinateModel Get(string coordinateId, SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateRepository(accessor))
			{
				var model = repository.Get(coordinateId);

				return model;
			}
		}
		#endregion

		#region +GetWithChilds 取得(付帯項目付)
		/// <summary>
		/// 取得(付帯項目付)
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <param name="shopId">店舗ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public CoordinateModel GetWithChilds(string coordinateId, string shopId, SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateRepository(accessor))
			{
				var model = repository.Get(coordinateId);

				if (model == null) return null;
				model.TagList = new List<ContentsTagModel>();
				model.CategoryList = new List<CoordinateCategoryModel>();
				model.ProductList = new List<ProductModel>();

				var items = repository.GetItems(coordinateId);

				var categoryService = new CoordinateCategoryService();
				var productService = new ProductService();
				var tagService = new ContentsTagService();
				foreach (var item in items)
				{
					switch (item.ItemKbn)
					{
						case Constants.FLG_COORDINATE_ITEM_KBN_TAG:
							var tag = tagService.Get(Int64.Parse(item.ItemId), accessor);
							if (tag != null) model.TagList.Add(tag);
							break;

						case Constants.FLG_COORDINATE_ITEM_KBN_CATEGORY:
							var category = categoryService.Get(item.ItemId, accessor);
							if (category != null) model.CategoryList.Add(category);
							break;

						case Constants.FLG_COORDINATE_ITEM_KBN_PRODUCT:
							ProductModel product = null;
							if (item.ItemId == item.ItemId2)
							{
								var productInfo = productService.Get(shopId, item.ItemId, accessor);
								if (productInfo != null)
								{
									product = productService.Get(shopId, item.ItemId, accessor);
									product.VariationId = item.ItemId;
								}
							}
							else
							{
								product = productService.GetProductVariation(shopId, item.ItemId, item.ItemId2, string.Empty, accessor);
							}

							if (product != null) model.ProductList.Add(product);
							break;
					}
				}
				return model;
			}
		}
		#endregion

		#region +GetGetForPreview 取得
		/// <summary>
		/// 取得(付帯項目付)
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public DataView GetForPreview(string coordinateId, SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateRepository(accessor))
			{
				var dv = repository.GetForPreview(coordinateId);

				return dv;
			}
		}
		#endregion

		#region +GetCoordinateListByProductId 商品IDで取得
		/// <summary>
		/// 取得(付帯項目付)
		/// </summary>
		/// <param name="productId">商品ID</param>
		/// <param name="shopId"></param>
		/// <returns>モデルリスト</returns>
		public List<CoordinateModel> GetCoordinateListByProductId(string productId, string shopId)
		{
			using (var repository = new CoordinateRepository())
			{
				var items = repository.GetItemsByProductId(productId);
				if (items != null)
				{
					var result = new List<CoordinateModel>();
					foreach (var item in items)
					{
						result.Add(GetWithChilds(item.CoordinateId, shopId));
					}
					return result;
				}
				return null;
			}
		}
		#endregion

		#region +GetAll 全取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <returns>モデル</returns>
		public CoordinateModel[] GetAll()
		{
			using (var repository = new CoordinateRepository())
			{
				var model = repository.GetAll();
				return model;
			}
		}
		#endregion

		#region +GetLikeList いいねリストを取得
		/// <summary>
		/// いいねリストを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pageNumber">ページNo</param>
		/// <param name="dispContents">表示数</param>
		/// <returns>モデル</returns>
		public CoordinateModel[] GetLikeList(string userId, int pageNumber, int dispContents)
		{
			using (var repository = new CoordinateRepository())
			{
				var models = repository.GetLikeList(userId, pageNumber, dispContents);
				return models;
			}
		}
		#endregion

		#region +GetLikeListCount いいねリスト件数を取得
		/// <summary>
		/// いいねリスト件数を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル</returns>
		public int GetLikeListCount(string userId)
		{
			using (var repository = new CoordinateRepository())
			{
				var count = repository.GetLikeListCount(userId);
				return count;
			}
		}
		#endregion

		#region +GetLikeRanking いいねランキング取得
		/// <summary>
		/// いいねランキング取得
		/// </summary>
		/// <param name="countDays">集計日数</param>
		/// <returns>モデル</returns>
		public CoordinateModel[] GetLikeRankingList(int countDays)
		{
			using (var repository = new CoordinateRepository())
			{
				var models = repository.GetLikeRankingList(countDays);

				return models;
			}
		}
		#endregion

		#region +GetContentsSummaryRanking 目次概要ランキング取得
		/// <summary>
		/// 目次概要ランキング取得
		/// </summary>
		/// <param name="reportType">レポートタイプ</param>
		/// <param name="countDays">集計日数</param>
		/// <returns>モデル</returns>
		public CoordinateModel[] GetContentsSummaryRankingList(string reportType, int countDays)
		{
			using (var repository = new CoordinateRepository())
			{
				var models = repository.GetContentsSummaryRankingList(reportType, countDays);

				return models;
			}
		}
		#endregion

		#region +GetCoordinateTopForPreview 先頭のコーディネート取得 (プレビュー用)
		/// <summary>
		/// 先頭のコーディネート取得 (プレビュー用)
		/// </summary>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>先頭の商品 (プレビュー用)</returns>
		public CoordinateModel GetCoordinateTopForPreview(SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateRepository(accessor))
			{
				var model = repository.GetCoordinateTopForPreview();
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
		public void Insert(CoordinateModel model, SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +InsertCoordinateitem 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void InsertCoordinateItem(CoordinateItemModel model, SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateRepository(accessor))
			{
				repository.InsertCoordinateItem(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(CoordinateModel model, SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateRepository(accessor))
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
		/// <param name="coordinateId">コーディネートID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string coordinateId, SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateRepository(accessor))
			{
				repository.Delete(coordinateId);
			}
		}
		#endregion

		#region +DeleteItem 項目削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="coordinateId">コーディネートID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void DeleteItem(string coordinateId, SqlAccessor accessor = null)
		{
			using (var repository = new CoordinateRepository(accessor))
			{
				repository.DeleteItem(coordinateId);
			}
		}
		#endregion

		#region マスタ出力
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
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var accessor = new SqlAccessor())
			using (var repository = new CoordinateRepository(accessor))
			using (var reader = repository.GetMasterWithReader(
				input,
				statementName,
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
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			using (var repository = new CoordinateRepository())
			{
				var dv = repository.GetMaster(input,
					statementName,
					new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				if (dv.Count >= 20000) return false;

				new MasterExportExcel().Exec(setting, excelTemplateSetting, dv, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				return true;
			}
		}

		/// <summary>
		/// マスタ区分で該当するStatementNameを取得
		/// </summary>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>StatementName</returns>
		private string GetStatementNameInfo(string masterKbn)
		{
			switch (masterKbn)
			{
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE: // コーディネート
					return "GetCoordinateMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_COORDINATE_ITEM: // コーディネートアイテム
					return "GetCoordinateItemMaster";
			}
			throw new Exception("未対応のマスタ区分：" + masterKbn);
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="masterKbn">マスタ区分</param>
		/// <returns>チェックOKか</returns>
		public bool CheckFieldsForGetMaster(string sqlFieldNames, string masterKbn)
		{
			try
			{
				using (var repository = new CoordinateRepository())
				{
					repository.CheckFieldsForGetMaster(
						new Hashtable(),
						masterKbn,
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