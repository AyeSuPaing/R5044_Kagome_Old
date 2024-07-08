/*
=========================================================================================================
  Module      : 店舗管理者サービス (ShopOperatorService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
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
using w2.Domain.MenuAuthority;
using w2.Domain.MenuAuthority.Helper;
using w2.Domain.ShopOperator.Helper;

namespace w2.Domain.ShopOperator
{
	/// <summary>
	/// 店舗管理者サービス
	/// </summary>
	public class ShopOperatorService : ServiceBase, IShopOperatorService
	{
		#region +GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		public int GetSearchHitCount(ShopOperatorListSearchCondition condition)
		{
			using (var repository = new ShopOperatorRepository())
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
		public ShopOperatorListSearchResult[] Search(ShopOperatorListSearchCondition condition)
		{
			using (var repository = new ShopOperatorRepository())
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
		/// <param name="shopId">店舗ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public ShopOperatorModel Get(string shopId, string operatorId, SqlAccessor accessor = null)
		{
			using (var repository = new ShopOperatorRepository(accessor))
			{
				var model = repository.Get(shopId, operatorId);
				return model;
			}
		}
		#endregion

		#region +GetOperatorListWithTagID タグIDを持つオペレーターを取得
		/// <summary>
		/// タグIDを持つオペレーターを取得
		/// </summary>
		/// <param name="accessor">Sqlアクセサ</param>
		/// <returns>モデル</returns>
		public ShopOperatorModel[] GetOperatorListWithTagID(SqlAccessor accessor = null)
		{
			using (var repository = new ShopOperatorRepository(accessor))
			{
				var model = repository.GetOperatorListWithTagID();
				return model;
			}
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>モデル</returns>
		public ShopOperatorModel GetWithAuthorityName(string shopId, string operatorId, SqlAccessor accessor = null)
		{
			var shopOperator = Get(shopId, operatorId, accessor);
			return shopOperator;
		}
		#endregion

		#region +GetOperatorList オペレータリスト取得
		/// <summary>
		/// オペレータリスト取得
		/// </summary>
		/// <param name="input">パラメータ</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>オペレータリスト</returns>
		public ShopOperatorModel[] GetOperatorList(Hashtable input, SqlAccessor accessor = null)
		{
			using (var repository = new ShopOperatorRepository(accessor))
			{
				var shopOperator = repository.GetOperatorList(input);
				return shopOperator;
			}
		}
		#endregion

		#region +GetEcOperatorList ECオペレータリスト取得
		/// <summary>
		/// ECオペレータリスト取得
		/// </summary>
		/// <param name="input">パラメータ</param>
		/// <param name="rowNumber">行数</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>オペレータリスト</returns>
		public ShopOperatorModel[] GetEcOperatorList(
			Hashtable input,
			Dictionary<string, int> rowNumber,
			SqlAccessor accessor = null)
		{
			using (var repository = new ShopOperatorRepository(accessor))
			{
				var shopOperator = repository.GetEcOperatorList(input, rowNumber);
				return shopOperator;
			}
		}
		#endregion

		#region +GetMpOperatorList MPオペレータリスト取得
		/// <summary>
		/// MPオペレータリスト取得
		/// </summary>
		/// <param name="input">パラメータ</param>
		/// <param name="rowNumber">行数</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>オペレータリスト</returns>
		public ShopOperatorModel[] GetMpOperatorList(
			Hashtable input,
			Dictionary<string, int> rowNumber,
			SqlAccessor accessor = null)
		{
			using (var repository = new ShopOperatorRepository(accessor))
			{
				var shopOperator = repository.GetMpOperatorList(input, rowNumber);
				return shopOperator;
			}
		}
		#endregion

		#region +GetByLoginId ログインIDから取得
		/// <summary>
		/// ログインIDから取得
		/// </summary>
		/// <param name="shopId">モデル</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="accessor">SQLアクセサ</param>
		public ShopOperatorModel GetByLoginId(string shopId, string loginId, SqlAccessor accessor = null)
		{
			using (var repository = new ShopOperatorRepository(accessor))
			{
				var model = repository.GetByLoginId(shopId, loginId);

				if (model != null)
				{
					var menuService = new MenuAuthorityService();
					model.EcMenuAuthorities = menuService.Get(
						shopId,
						MenuAuthorityHelper.ManagerSiteType.Ec,
						model.GetMenuAccessLevel(MenuAuthorityHelper.ManagerSiteType.Ec) ?? -1);
					model.MpMenuAuthorities = menuService.Get(
						shopId,
						MenuAuthorityHelper.ManagerSiteType.Mp,
						model.GetMenuAccessLevel(MenuAuthorityHelper.ManagerSiteType.Mp) ?? -1);
					model.CsMenuAuthorities = menuService.Get(
						shopId,
						MenuAuthorityHelper.ManagerSiteType.Cs,
						model.GetMenuAccessLevel(MenuAuthorityHelper.ManagerSiteType.Cs) ?? -1);
					model.CmsMenuAuthorities = menuService.Get(
						shopId,
						MenuAuthorityHelper.ManagerSiteType.Cms,
						model.GetMenuAccessLevel(MenuAuthorityHelper.ManagerSiteType.Cms) ?? -1);
				}

				return model;
			}
		}
		#endregion

		#region +UpdateValidFlgOffByLoginId ログインIDから有効フラグオフ更新
		/// <summary>
		/// ログインIDから有効フラグオフ更新
		/// </summary>
		/// <param name="shopId">モデル</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="lastChanged">最終更新者</param>
		public int UpdateValidFlgOffByLoginId(string shopId, string loginId, string lastChanged)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var shopOperator = GetByLoginId(shopId, loginId, accessor);
				if (shopOperator == null) return 0;

				shopOperator.ValidFlg = Constants.FLG_SHOPOPERATOR_VALID_FLG_INVALID;
				shopOperator.LastChanged = lastChanged;
				var updated = Update(shopOperator, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion

		#region +Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		public void Insert(ShopOperatorModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ShopOperatorRepository(accessor))
			{
				repository.Insert(model);
			}
		}
		#endregion

		#region +Modify 更新（汎用的に利用）
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="updateAction">更新操作</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(
			string shopId,
			string operatorId,
			Action<ShopOperatorModel> updateAction)
		{
			using (var accessor = new SqlAccessor())
			{
				accessor.OpenConnection();
				accessor.BeginTransaction();

				var updated = Modify(shopId, operatorId, updateAction, accessor);

				accessor.CommitTransaction();
				return updated;
			}
		}
		#endregion
		#region +Modify 更新（汎用的に利用）
		/// <summary>
		/// 更新（汎用的に利用）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="updateAction">更新操作</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>影響を受けた件数</returns>
		public int Modify(
			string shopId,
			string operatorId,
			Action<ShopOperatorModel> updateAction,
			SqlAccessor accessor)
		{
			// 最新データ取得
			var user = Get(shopId, operatorId, accessor);

			// モデル内容更新
			updateAction(user);

			// 更新
			int updated = Update(user, accessor);

			return updated;
		}
		#endregion

		#region +Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int Update(ShopOperatorModel model, SqlAccessor accessor = null)
		{
			using (var repository = new ShopOperatorRepository(accessor))
			{
				var result = repository.Update(model);
				return result;
			}
		}
		#endregion

		#region +UpdateOperatorAdvAuthority  閲覧権限更新
		/// <summary>
		///  閲覧権限更新
		/// </summary>
		/// <param name="input">パラメータ</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int UpdateOperatorAdvAuthority(Hashtable input, SqlAccessor accessor = null)
		{
			using (var repository = new ShopOperatorRepository(accessor))
			{
				var result = repository.UpdateOperatorAdvAuthority(input);
				return result;
			}
		}
		#endregion

		#region +UpdateOperatorTagAuthority 閲覧可能なアフィリエイトタグ更新
		/// <summary>
		/// 閲覧可能なアフィリエイトタグ更新
		/// </summary>
		/// <param name="ht">パラメータ</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public void UpdateOperatorTagAuthority(Hashtable ht, SqlAccessor accessor = null)
		{
			using (var repository = new ShopOperatorRepository(accessor))
			{
				repository.UpdateOperatorTagAuthority(ht);
			}
		}
		#endregion

		#region +UpdateAuthenticationCode 認証コード更新
		/// <summary>
		/// 認証コード更新
		/// </summary>
		/// <param name="shopId">モデル</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="authenticationCode">認証コード</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateAuthenticationCode(
			string shopId,
			string loginId,
			string authenticationCode)
		{
			using (var repository = new ShopOperatorRepository())
			{
				var updated = repository.UpdateAuthenticationCode(
					shopId,
					loginId,
					authenticationCode);

				return updated;
			}
		}
		#endregion

		#region +UpdateRemoteAddress リモートIPアドレス更新
		/// <summary>
		/// リモートIPアドレス更新
		/// </summary>
		/// <param name="shopId">モデル</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="remoteAddress">リモートIPアドレス</param>
		/// <returns>影響を受けた件数</returns>
		public int UpdateRemoteAddress(string shopId, string loginId, string remoteAddress)
		{
			using (var repository = new ShopOperatorRepository())
			{
				var updated = repository.UpdateRemoteAddress(
					shopId,
					loginId,
					remoteAddress);

				return updated;
			}
		}
		#endregion

		#region +Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>更新件数</returns>
		public int Delete(string shopId, string operatorId, SqlAccessor accessor = null)
		{
			using (var repository = new ShopOperatorRepository(accessor))
			{
				var result = repository.Delete(shopId, operatorId);
				return result;
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
			using (var repository = new ShopOperatorRepository(accessor))
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
			using (var repository = new ShopOperatorRepository())
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
				using (var repository = new ShopOperatorRepository())
				{
					repository.CheckOperatorFieldsForGetMaster(
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
