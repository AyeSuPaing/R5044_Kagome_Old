/*
=========================================================================================================
  Module      : リアル店舗情報サービス (RealShopService.cs)
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
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;

namespace w2.Domain.RealShop
{
	/// <summary>
	/// リアル店舗情報サービス
	/// </summary>
	public class RealShopService : ServiceBase, IRealShopService
	{
		/*
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(RealShopListSearchCondition condition)
		{
			using (var repository = new RealShopRepository())
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
		public RealShopListSearchResult[] Search(RealShopListSearchCondition condition)
		{
			using (var repository = new RealShopRepository())
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
		/// <param name="realShopId">リアル店舗ID</param>
		/// <returns>モデル</returns>
		public RealShopModel Get(string realShopId)
		{
			using (var repository = new RealShopRepository())
			{
				var model = repository.Get(realShopId);
				return model;
			}
		}
		#endregion

		#region +GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル</returns>
		public RealShopModel[] GetAll()
		{
			using (var repository = new RealShopRepository())
			{
				var model = repository.GetAll();
				return model;
			}
		}
		#endregion

		/*
		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(RealShopModel model, SqlAccessor accessor = null)
		{
			using (var repository = new RealShopRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion
		*/
		/*
		#region +Modify (汎用的に利用)
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="realShopId">リアル店舗ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <returns>影響を受けた件数</returns>
		public void Modify(string realShopId, Action<RealShopModel> updateAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = Modify(string realShopId, updateAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		*/
		/*
		#region +Modify (汎用的に利用)
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="realShopId">リアル店舗ID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public void Modify(string realShopId, Action<RealShopModel> updateAction, SqlAccessor accessor)
		{
			var model = Get(realShopId, accessor);

			updateAction(model);

			var updated = Update(model, accessor);

			return updated;
		}
		#endregion
		*/
		/*
		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public int Update(RealShopModel model, SqlAccessor accessor = null)
		{
			using (var repository = new RealShopRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion
		*/
		/*
		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="realShopId">リアル店舗ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string realShopId, SqlAccessor accessor = null)
		{
			using (var repository = new RealShopRepository(accessor))
			{
				var result = repository.Delete(realShopId);
				return result;
			}
		}
		#endregion
		*/

		#region +GetRealShopProductStockTopForPreview 先頭のリアル店舗商品在庫取得 (プレビュー用)
		/// <summary>
		/// 先頭のリアル店舗商品在庫取得 (プレビュー用)
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>先頭のリアル店舗商品在庫取得 (プレビュー用)</returns>
		public RealShopProductStockModel GetRealShopProductStockTopForPreview(string shopId, SqlAccessor accessor = null)
		{
			using (var repository = new RealShopRepository(accessor))
			{
				var model = repository.GetRealShopProductStockTopForPreview(shopId);
				return model;
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
			using (var repository = new RealShopRepository(accessor))
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
			using (var repository = new RealShopRepository())
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
				using (var repository = new RealShopRepository())
				{
					repository.CheckRealShopFieldsForGetMaster(
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

		#region +GetRealShops
		/// <summary>
		/// Get real shops
		/// </summary>
		/// <param name="areaId">Area id</param>
		/// <param name="brandId">Brand id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Array of real shop model</returns>
		public RealShopModel[] GetRealShops(string areaId, string brandId, SqlAccessor accessor = null)
		{
			using (var repository = new RealShopRepository(accessor))
			{
				var result = repository.GetRealShops(areaId, brandId);
				return result;
			}
		}
		#endregion

		#region +GetRealShopsByAddr1
		/// <summary>
		/// Get real shops by addr1
		/// </summary>
		/// <param name="addr1">addr1</param>
		/// <param name="brandId">Brand id</param>
		/// <param name="accessor">Sql accessor</param>
		/// <returns>Array of real shop model</returns>
		public RealShopModel[] GetRealShopsByAddr1(string addr1, string brandId, SqlAccessor accessor = null)
		{
			using (var repository = new RealShopRepository(accessor))
			{
				var result = repository.GetRealShopsByAddr1(addr1, brandId);
				return result;
			}
		}
		#endregion
	}
}
