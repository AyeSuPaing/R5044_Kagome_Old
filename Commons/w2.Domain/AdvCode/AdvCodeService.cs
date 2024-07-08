/*
=========================================================================================================
  Module      : 広告コードサービス (AdvCodeService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using w2.Common.Logger;
using w2.Common.Sql;
using w2.Common.Util;
using w2.Domain.AdvCode.Helper;
using w2.Domain.MasterExportSetting;
using w2.Domain.MasterExportSetting.Helper;
using w2.Domain.ShopOperator;

namespace w2.Domain.AdvCode
{
	/// <summary>
	/// 広告コードサービス
	/// </summary>
	public class AdvCodeService : ServiceBase, IAdvCodeService
	{
		/// <summary>広告コード抽出置換値</summary>
		public const string WHERE_ADVERTISEMENT_CODE = "@@ where_advertisement_code @@";

		#region +Advertisement Code

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// <param name="advertisementCode">閲覧可能広告コード</param>
		/// <returns>件数</returns>
		public int GetAdvCodeSearchHitCount(AdvCodeListSearchCondition condition, string[] advertisementCode = null)
		{
			var whereCondition = string.Empty;
			if ((advertisementCode != null) && (advertisementCode.Length > 0))
			{
				whereCondition = CreateWhereInCondition(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE, advertisementCode);
			}
			var replaces = new KeyValuePair<string, string>(WHERE_ADVERTISEMENT_CODE, whereCondition);

			using (var repository = new AdvCodeRepository())
			{
				var count = repository.GetAdvCodeSearchHitCount(condition, replaces);
				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// /// <param name="advertisementCode">閲覧可能広告コード</param>
		/// <returns>モデル列</returns>
		public AdvCodeListSearchResult[] SearchAdvCode(AdvCodeListSearchCondition condition, string[] advertisementCode = null)
		{
			var whereCondition = string.Empty;
			if ((advertisementCode != null) && (advertisementCode.Length > 0))
			{
				whereCondition = CreateWhereInCondition(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE, advertisementCode);
			}

			var replaces = new KeyValuePair<string, string>(WHERE_ADVERTISEMENT_CODE, whereCondition);

			using (var repository = new AdvCodeRepository())
			{
				var searchResults = repository.SearchAdvCode(condition, replaces);
				return searchResults;
			}
		}
		#endregion

		/// <summary>
		/// IN条件生成
		/// </summary>
		/// <param name="column">カラム名</param>
		/// <param name="value">対象値</param>
		/// <returns>条件</returns>
		public string CreateWhereInCondition(string column, string[] value)
		{
			return string.Format(
				"AND  ({0} IN ({1}))",
				column,
				string.Join(",", value.Select(adv => string.Format("'{0}'", adv.Replace("'", "''")))));
		}

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="advcodeNo">広告コードNO</param>
		/// <returns>モデル</returns>
		public AdvCodeModel GetAdvCode(long advcodeNo)
		{
			using (var repository = new AdvCodeRepository())
			{
				var model = repository.GetAdvCode(advcodeNo);
				return model;
			}
		}

		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="advertisementCode">広告コード</param>
		/// <returns>モデル</returns>
		public AdvCodeModel GetAdvCodeFromAdvertisementCode(string advertisementCode)
		{
			using (var repository = new AdvCodeRepository())
			{
				var model = repository.GetAdvCodeFromAdvertisementCode(advertisementCode);
				return model;
			}
		}

		/// <summary>
		/// Get advertisement code from advertisement code media type id
		/// </summary>
		/// <param name="advCodeMediaType">AdvCode Media Type</param>
		/// <returns>advcode no</returns>
		public int GetAdvCodeFromAdvcodeMediaTypeId(string advCodeMediaType)
		{
			using (var repository = new AdvCodeRepository())
			{
				return repository.GetAdvCodeFromAdvcodeMediaTypeId(advCodeMediaType);
			}
		}

		/// <summary>
		/// 全取得
		/// </summary>
		/// <returns>モデルリスト</returns>
		public AdvCodeModel[] GetAll()
		{
			using (var repository = new AdvCodeRepository())
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
		public void InsertAdvCode(AdvCodeModel model)
		{
			using (var repository = new AdvCodeRepository())
			{
				repository.InsertAdvCode(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		public int UpdateAdvCode(AdvCodeModel model)
		{
			using (var repository = new AdvCodeRepository())
			{
				var result = repository.UpdateAdvCode(model);
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="advcodeNo">広告コードNO</param>
		public void DeleteAdvCode(long advcodeNo)
		{
			using (var repository = new AdvCodeRepository())
			{
				repository.DeleteAdvCode(advcodeNo);
			}
		}
		#endregion

		#endregion

		#region +Advertisement Code Media Type

		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// <returns>件数</returns>
		public int GetAdvCodeMediaTypeSearchHitCount(AdvCodeMediaTypeListSearchCondition condition)
		{
			using (var repository = new AdvCodeRepository())
			{
				var count = repository.GetAdvCodeMediaTypeSearchHitCount(condition);
				return count;
			}
		}
		#endregion

		#region +Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索パラメタ</param>
		/// <returns>モデル列</returns>
		public AdvCodeMediaTypeListSearchResult[] SearchAdvCodeMediaType(AdvCodeMediaTypeListSearchCondition condition)
		{
			using (var repository = new AdvCodeRepository())
			{
				var searchResults = repository.SearchAdvCodeMediaType(condition);
				return searchResults;
			}
		}
		#endregion

		#region +SearchMediaTypeByKeyword キーワードから広告媒体区分を検索
		/// <summary>
		/// キーワードから広告媒体区分を検索
		/// </summary>
		/// <param name="searchWord">検索キーワード</param>
		/// <returns>検索結果列</returns>
		public AdvCodeMediaTypeModel[] SearchMediaTypeByKeyword(string searchWord)
		{
			using (var repository = new AdvCodeRepository())
			{
				var searchResults = repository.SearchMediaTypeByKeyword(searchWord);
				return searchResults;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="advcodeMediaTypeId">区分ID</param>
		/// <returns>モデル</returns>
		public AdvCodeMediaTypeModel GetAdvCodeMediaType(string advcodeMediaTypeId)
		{
			using (var repository = new AdvCodeRepository())
			{
				var model = repository.GetAdvCodeMediaType(advcodeMediaTypeId);
				return model;
			}
		}

		/// <summary>
		/// Get AdvCode Media Type List All
		/// </summary>
		/// <returns>AdvCode Media Type List</returns>
		public AdvCodeMediaTypeModel[] GetAdvCodeMediaTypeListAll()
		{
			using (var repository = new AdvCodeRepository())
			{
				return repository.GetAdvCodeMediaTypeListAll();
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		public void InsertAdvCodeMediaType(AdvCodeMediaTypeModel model)
		{
			using (var repository = new AdvCodeRepository())
			{
				repository.InsertAdvCodeMediaType(model);
			}
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">Accessor</param>
		/// <returns></returns>
		public int UpdateAdvCodeMediaType(AdvCodeMediaTypeModel model, SqlAccessor accessor)
		{
			using (var repository = new AdvCodeRepository(accessor))
			{
				var result = repository.UpdateAdvCodeMediaType(model);
				return result;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="advcodeMediaTypeId">区分ID</param>
		public void DeleteAdvCodeMediaType(string advcodeMediaTypeId)
		{
			using (var repository = new AdvCodeRepository())
			{
				repository.DeleteAdvCodeMediaType(advcodeMediaTypeId);
			}
		}
		#endregion

		#endregion

		#region +マスタ出力
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
				using (var repository = new AdvCodeRepository())
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

		/// <summary>
		///  CSVへ出力
		/// </summary>
		/// <param name="setting">出力設定</param>
		/// <param name="input">検索条件</param>
		/// <param name="sqlFieldNames">SQLフィールド名列</param>
		/// <param name="outputStream">出力ストリーム</param>
		/// <param name="loginOperatorShopId">ログインオペレータ店舗ID</param>
		/// <param name="loginOperatorId">ログインオペレータID</param>
		/// <param name="formatDate">日付形式</param>
		/// <param name="digitsByKeyCurrency">基軸通貨 小数点以下の有効桁数</param>
		/// <param name="replacePrice">Replace文字列</param>
		/// <returns>成功か</returns>
		public bool ExportToCsv(
			MasterExportSettingModel setting,
			Hashtable input,
			string sqlFieldNames,
			Stream outputStream,
			string loginOperatorShopId,
			string loginOperatorId,
			string formatDate,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			var whereCondition = CreateUsableAdvertisementCodesStatement(loginOperatorShopId, loginOperatorId);
			using (var accessor = new SqlAccessor())
			using (var repository = new AdvCodeRepository(accessor))
			using (var reader = repository.GetMasterWithReader(
				input,
				statementName,
				new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames),
				new KeyValuePair<string, string>(WHERE_ADVERTISEMENT_CODE, whereCondition)))
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
		/// <param name="loginOperatorShopId">ログインオペレータ店舗ID</param>
		/// <param name="loginOperatorId">ログインオペレータID</param>
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
			string loginOperatorShopId,
			string loginOperatorId,
			int digitsByKeyCurrency,
			string replacePrice)
		{
			var statementName = GetStatementNameInfo(setting.MasterKbn);
			var whereCondition = CreateUsableAdvertisementCodesStatement(loginOperatorShopId, loginOperatorId);
			using (var repository = new AdvCodeRepository())
			{
				var dv = repository.GetMaster(input,
					statementName,
					new KeyValuePair<string, string>("@@ fields @@", sqlFieldNames),
					new KeyValuePair<string, string>(WHERE_ADVERTISEMENT_CODE, whereCondition));
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
				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE: // 広告コード
					return "GetAdvCodeMaster";

				case Constants.FLG_MASTEREXPORTSETTING_MASTER_KBN_ADVCODE_MEDIA_TYPE: // 広告媒体区分マスタ
					return "GetAdvCodeMediaTypeMaster";
			}
			throw new Exception("未対応のマスタ区分：" + masterKbn);
		}

		/// <summary>
		/// 閲覧可能な広告コードによる絞り込み用の条件句を生成します
		/// </summary>
		/// <param name="loginOperatorShopId">ログインオペレータ店舗ID</param>
		/// <param name="loginOperatorId">ログインオペレータID</param>
		/// <returns>置換用SQLステートメント</returns>
		private string CreateUsableAdvertisementCodesStatement(string loginOperatorShopId, string loginOperatorId)
		{
			var loginOperator = new ShopOperatorService().Get(loginOperatorShopId, loginOperatorId);
			var whereCondition = string.Empty;
			if (loginOperator.UsableAdvcodeNosInReport.Any())
			{
				var updateAdCodeArray = StringUtility.SplitCsvLine(loginOperator.UsableAdvcodeNosInReport).ToArray();
				if (updateAdCodeArray.Any())
				{
					whereCondition = CreateWhereInCondition(Constants.FIELD_ADVCODE_ADVERTISEMENT_CODE, updateAdCodeArray);
				}
			}
			return whereCondition;
		}
		#endregion

		#region +SearchAdvCodesForAutosuggest
		/// <summary>
		/// Search advertisement codes for autosuggest
		/// </summary>
		/// <param name="searchWord">Search word</param>
		/// <returns>Advertisement codes</returns>
		public AdvCodeModel[] SearchAdvCodesForAutosuggest(string searchWord)
		{
			try
			{
				using (var repository = new AdvCodeRepository())
				{
					repository.CommandTimeout = Constants.ORDERREGISTINPUT_SUGGEST_QUERY_TIMEOUT;
					var advCodes = repository.SearchAdvCodesForAutosuggest(searchWord);
					return advCodes;
				}
			}
			catch (Exception ex)
			{
				throw ex;
			}
		}
		#endregion
	}
}
