/*
=========================================================================================================
  Module      : ショートURLサービス (ShortUrlService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2016 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.ShortUrl.Helper;

namespace w2.Domain.ShortUrl
{
	/// <summary>
	/// ショートURLサービス
	/// </summary>
	public class ShortUrlService : ServiceBase, IShortUrlService
	{
		#region +GetSearchHitCount 検索ヒット件数取得

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(ShortUrlListSearchCondition condition)
		{
			using (var repository = new ShortUrlRepository())
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
		public ShortUrlListSearchResult[] Search(ShortUrlListSearchCondition condition)
		{
			using (var repository = new ShortUrlRepository())
			{
				var results = repository.Search(condition);
				return results;
			}
		}

		#endregion

		#region +GetAll 取得（全て）

		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル</returns>
		public ShortUrlModel[] GetAll(string shopId)
		{
			using (var repository = new ShortUrlRepository())
			{
				var models = repository.GetAll(shopId);
				if (models.Length == 0) return new ShortUrlModel[0];
				return models;
			}
		}

		#endregion

		#region +GetShortUrlForDuplicationShortUrl ショートURL重複ショートURL情報取得（※ワークテーブル参照）
		/// <summary>
		/// 重複ショートURL情報取得（※ワークテーブル参照）
		/// </summary>
		/// <returns>重複ショートURL情報</returns>
		public Dictionary<string, int> GetShortUrlForDuplicationShortUrl()
		{
			using (var repository = new ShortUrlRepository())
			{
				var shortUrlInfo = repository.GetShortUrlForDuplicationShortUrl();
				return shortUrlInfo;
			}
		}
		#endregion

		#region +Insert 登録

		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(ShortUrlModel model)
		{
			using (var repository = new ShortUrlRepository())
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
		/// <returns>影響を受けた件数</returns>
		public int Update(ShortUrlModel model, SqlAccessor accessor)
		{
			using (var repository = new ShortUrlRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="models">モデル</param>
		/// <returns>影響を受けた件数</returns>
		public int Update(IEnumerable<ShortUrlModel> models)
		{
			var result = 0;
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				result += models.Sum(model => this.Update(model, accessor));

				accessor.CommitTransaction();
			}
			return result;
		}

		#endregion

		#region +Delete 削除

		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="surlNo">ショートURL NO</param>
		public void Delete(long surlNo)
		{
			using (var repository = new ShortUrlRepository())
			{
				var result = repository.Delete(surlNo);
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
			using (var repository = new ShortUrlRepository(accessor))
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
			using (var repository = new ShortUrlRepository())
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
				using (var repository = new ShortUrlRepository())
				{
					repository.CheckShortUrlFieldsForGetMaster(
						new Hashtable { { Constants.FIELD_SHORTURL_SHOP_ID, shopId } },
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