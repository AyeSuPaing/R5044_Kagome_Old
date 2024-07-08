/*
=========================================================================================================
  Module      : ターゲットリスト設定サービス (TargetListService.cs)
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
using w2.Common.Util;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;

//using w2.Domain.TargetList.Helper;

namespace w2.Domain.TargetList
{
	/// <summary>
	/// ターゲットリスト設定サービス
	/// </summary>
	public class TargetListService : ServiceBase
	{
		/*
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(TargetListListSearchCondition condition)
		{
			using (var repository = new TargetListRepository())
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
		public TargetListListSearchResult[] Search(TargetListListSearchCondition condition)
		{
			using (var repository = new TargetListRepository())
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
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>モデル</returns>
		public TargetListModel Get(string deptId, string targetId)
		{
			using (var repository = new TargetListRepository())
			{
				var model = repository.Get(deptId, targetId);
				return model;
			}
		}
		#endregion

		#region +GetAll 取得(すべて)
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>モデル</returns>
		public TargetListModel[] GetAll(string deptId)
		{
			using (var repository = new TargetListRepository())
			{
				var result = repository.GetAll(deptId);
				return result;
			}
		}
		#endregion

		#region +GetAllValidTargetList 有効なターゲットリストをすべて取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <returns>モデル</returns>
		public TargetListModel[] GetAllValidTargetList(string deptId)
		{
			using (var repository = new TargetListRepository())
			{
				var result = repository.GetAllValidTargetList(deptId);
				return result;
			}
		}
		#endregion

		#region +GetTargetListForAction アクション実行ターゲットリスト取得
		/// <summary>
		/// アクション実行ターゲットリスト取得
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetListIds">ターゲットリストID</param>
		/// <returns>モデルリスト</returns>
		public TargetListModel[] GetTargetListForAction(string deptId, string targetListIds)
		{
			using (var repository = new TargetListRepository())
			{
				var models = repository.GetTargetListForAction(deptId, targetListIds);
				return models;
			}
		}
		#endregion

		/*
		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void Insert(TargetListModel model)
		{
			using (var repository = new TargetListRepository())
			{
				repository.Insert(model);
			}
		}
		#endregion
		*/
		/*
		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int Update(TargetListModel model)
		{
			using (var repository = new TargetListRepository())
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
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>
		public void Delete(string deptId, string targetId)
		{
			using (var repository = new TargetListRepository())
			{
				var result = repository.Delete(deptId, targetId);
				return result;
			}
		}
		#endregion
		*/

		#region +GetTargetListDataDeduplicated ターゲットリストデータ取得（重複除外）
		/// <summary>
		/// ターゲットリストデータ取得（重複除外）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetKbn">ターゲット区分</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="accessor">アクセサー</param>
		/// <returns>ターゲットリスト</returns>
		public TargetListDataModel[] GetTargetListDataDeduplicated(string deptId, string targetKbn, string masterId, SqlAccessor accessor = null)
		{
			using (var repository = new TargetListRepository(accessor) { CommandTimeout = Constants.AGGREGATE_SQL_TIME_OUT })
			{
				var result = repository.GetTargetListDataDeduplicated(deptId, targetKbn, masterId);
				return result;
			}
		}
		#endregion

		#region +GetHitTargetListIds 対象ユーザの有効なターゲットリスト一覧を取得
		/// <summary>
		/// ターゲットリストデータ取得（重複除外）
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="userId">ユーザID</param>
		/// <param name="targetKbn">ターゲット区分</param>
		/// <returns>ターゲットリストID配列</returns>
		public string[] GetHitTargetListId(string deptId, string userId, string targetKbn)
		{
			using (var repository = new TargetListRepository())
			{
				var result = repository.GetHitTargetListIds(deptId, userId, targetKbn);
				return result;
			}
		}
		#endregion

		#region +CheckValidTargetList ターゲットリストID存在チェック
		/// <summary>
		/// ターゲットリストID存在チェック
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>ターゲットリストID存在するか</returns>
		public bool CheckValidTargetList(string deptId, string targetId)
		{
			using (var repository = new TargetListRepository())
			{
				var isValid = repository.CheckValidTargetList(deptId, targetId);
				return isValid;
			}
		}
		#endregion

		#region +CheckUserIdInTargetList ユーザーIDがターゲットリスト内にあるか
		/// <summary>
		/// ユーザーIDがターゲットリスト内にあるか
		/// </summary>
		/// <param name="deptId">識別ID</param>
		/// <param name="masterId">マスタID</param>
		/// <param name="userId">ユーザーID</param>
		/// <returns>ユーザーIDがターゲットリスト内にあるか</returns>
		public bool CheckUserIdInTargetList(string deptId, string masterId, string userId)
		{
			using (var repository = new TargetListRepository())
			{
				var isExisted = repository.CheckUserIdInTargetList(deptId, masterId, userId);
				return isExisted;
			}
		}
		#endregion

		#region +IsTargetListStatusNormal ターゲットリストのステータス確認する
		/// <summary>
		/// ターゲットリストのステータス確認する
		/// </summary>
		/// <param name="targetId">ターゲットリストID</param>
		/// <returns>true:通常の場合, false:通常以外の場合</returns>
		public bool IsTargetListStatusNormal(string targetId)
		{
			var targetList = Get(Constants.CONST_DEFAULT_DEPT_ID, targetId);
			if (targetList == null) return false;
			return (StringUtility.ToEmpty(targetList.Status) == Constants.FLG_TARGETLIST_STATUS_NORMAL);
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <returns>チェックOKか</returns>
		public bool CheckTargetListDataFieldsForGetMaster(string sqlFieldNames)
		{
			try
			{
				using (var repository = new TargetListRepository())
				{
					repository.CheckTargetListDataFieldsForGetMaster(
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
			using (var repository = new TargetListRepository(accessor))
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
			using (var repository = new TargetListRepository())
			{
				var dv = repository.GetMaster(input, new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames));
				if (dv.Count >= 20000) return false;

				new MasterExportExcel().Exec(setting, excelTemplateSetting, dv, outputStream, formatDate, digitsByKeyCurrency, replacePrice);
				return true;
			}
		}
		#endregion
	}
}
