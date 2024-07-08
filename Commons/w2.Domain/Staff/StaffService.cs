/*
=========================================================================================================
  Module      : スタッフサービス (StaffService.cs)
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
using w2.Domain.Staff.Helper;

namespace w2.Domain.Staff
{
	/// <summary>
	/// スタッフサービス
	/// </summary>
	public class StaffService : ServiceBase
	{
		#region +GetSearchHitCount 検索ヒット件数取得

		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(StaffListSearchCondition condition)
		{
			using (var repository = new StaffRepository())
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
		public StaffListSearchResult[] Search(StaffListSearchCondition condition)
		{
			using (var repository = new StaffRepository())
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
		/// <param name="staffId">スタッフID</param>
		/// <returns>モデル</returns>
		public StaffModel Get(string staffId)
		{
			using (var repository = new StaffRepository())
			{
				var model = repository.Get(staffId);
				return model;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// オペレータIDで取得
		/// </summary>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>モデル</returns>
		public StaffModel[] GetByOperatorId(string operatorId)
		{
			using (var repository = new StaffRepository())
			{
				var models = repository.GetByOperatorId(operatorId);
				return models;
			}
		}
		#endregion

		#region +GetAllForCoordinate 全取得（コーディネート用）
		/// <summary>
		/// 全取得（コーディネート用）
		/// </summary>
		/// <returns>モデル</returns>
		public StaffModel[] GetAllForCoordinate()
		{
			using (var repository = new StaffRepository())
			{
				var model = repository.GetAllForCoordinate();
				return model;
			}
		}
		#endregion

		#region +GetFollowList フォローリストを取得
		/// <summary>
		/// フォローリストを取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="pageNumber">ページNo</param>
		/// <param name="dispContents">表示数</param>
		/// <returns>モデル</returns>
		public StaffModel[] GetFollowList(string userId, int pageNumber, int dispContents)
		{
			using (var repository = new StaffRepository())
			{
				var models = repository.GetFollowList(userId, pageNumber, dispContents);
				return models;
			}
		}
		#endregion

		#region +GetFollowCount フォローリスト件数を取得
		/// <summary>
		/// フォローリスト件数を取得
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <returns>モデル</returns>
		public int GetFollowCount(string userId)
		{
			using (var repository = new StaffRepository())
			{
				var count = repository.GetFollowCount(userId);
				return count;
			}
		}
		#endregion

		#region +GetAll 全取得
		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデル</returns>
		public StaffModel[] GetAll()
		{
			using (var repository = new StaffRepository())
			{
				var model = repository.GetAll();
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
		public void Insert(StaffModel model, SqlAccessor accessor = null)
		{
			using (var repository = new StaffRepository(accessor))
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
		public int Update(StaffModel model, SqlAccessor accessor = null)
		{
			using (var repository = new StaffRepository(accessor))
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
		/// <param name="modelId">スタッフID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Delete(string modelId, SqlAccessor accessor = null)
		{
			using (var repository = new StaffRepository(accessor))
			{
				var result = repository.Delete(modelId);
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
			using (var repository = new StaffRepository(accessor))
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
			using (var repository = new StaffRepository())
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
				using (var repository = new StaffRepository())
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
