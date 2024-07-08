/*
=========================================================================================================
  Module      : 店舗管理者リポジトリ (ShopOperatorRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;
using w2.Domain.ShopOperator.Helper;

namespace w2.Domain.ShopOperator
{
	/// <summary>
	/// 店舗管理者リポジトリ
	/// </summary>
	internal class ShopOperatorRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "ShopOperator";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		internal ShopOperatorRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		internal ShopOperatorRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region ~GetSearchHitCount 検索ヒット件数取得
		/// <summary>
		/// 検索ヒット件数取得
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>件数</returns>
		internal int GetSearchHitCount(ShopOperatorListSearchCondition condition)
		{
			var dv = Get(XML_KEY_NAME, "GetSearchHitCount", condition.CreateHashtableParams());
			return (int)dv[0][0];
		}
		#endregion

		#region ~Search 検索
		/// <summary>
		/// 検索
		/// </summary>
		/// <param name="condition">検索条件</param>
		/// <returns>検索結果列</returns>
		internal ShopOperatorListSearchResult[] Search(ShopOperatorListSearchCondition condition)
		{
			var dv = Get(
				XML_KEY_NAME,
				"Search",
				condition.CreateHashtableParams(),
				replaces: new KeyValuePair<string, string>("@@menu_access_level@@", "menu_access_level" + condition.PkgKbn));
			return dv.Cast<DataRowView>().Select(drv => new ShopOperatorListSearchResult(drv)).ToArray();
		}
		#endregion

		#region ~Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="operatorId">オペレータID</param>
		/// <returns>モデル</returns>
		internal ShopOperatorModel Get(string shopId, string operatorId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPOPERATOR_SHOP_ID, shopId},
				{Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, operatorId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new ShopOperatorModel(dv[0]);
		}
		#endregion

		#region ~GetOperatorListWithTagID タグIDを持つオペレーター情報取得
		/// <summary>
		/// タグIDを持つオペレーター情報取得
		/// </summary>
		/// <returns>オペレーターリスト</returns>
		internal ShopOperatorModel[] GetOperatorListWithTagID()
		{
			var dv = Get(XML_KEY_NAME, "GetOperatorListWithTagID", null);
			if (dv.Count == 0) return null;
			return dv.Cast<DataRowView>().Select(drv => new ShopOperatorModel(drv)).ToArray();
		}
		#endregion

		#region ~GetByLoginId ログイIDから取得
		/// <summary>
		/// ログイIDから取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="loginId">ログインID</param>
		/// <returns>モデル</returns>
		internal ShopOperatorModel GetByLoginId(string shopId, string loginId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPOPERATOR_SHOP_ID, shopId},
				{Constants.FIELD_SHOPOPERATOR_LOGIN_ID, loginId},
			};
			var dv = Get(XML_KEY_NAME, "GetByLoginId", ht);
			if (dv.Count == 0) return null;
			return new ShopOperatorModel(dv[0]);
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
			var dv = Get(XML_KEY_NAME, "GetOperatorList", input);
			return dv.Cast<DataRowView>().Select(drv => new ShopOperatorModel(drv)).ToArray();
		}
		#endregion

		#region ~GetEcOperatorList ECオペレータリスト取得
		/// <summary>
		/// ECオペレータリスト取得
		/// </summary>
		/// <param name="input">パラメータ</param>
		/// <param name="rowNumber">行数</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>オペレータリスト</returns>
		internal ShopOperatorModel[] GetEcOperatorList(
			Hashtable input,
			Dictionary<string, int> rowNumber,
			SqlAccessor accessor = null)
		{
			input.Add(Constants.FIELD_COMMON_BEGIN_NUM, rowNumber[Constants.FIELD_COMMON_BEGIN_NUM]);
			input.Add(Constants.FIELD_COMMON_END_NUM, rowNumber[Constants.FIELD_COMMON_END_NUM]);

			var dv = Get(XML_KEY_NAME, "GetEcOperatorList", input);
			return dv.Cast<DataRowView>().Select(drv => new ShopOperatorModel(drv)).ToArray();
		}
		#endregion

		#region ~GetMpOperatorList MPオペレータリスト取得
		/// <summary>
		/// MPオペレータリスト取得
		/// </summary>
		/// <param name="input">パラメータ</param>
		/// <param name="rowNumber">行数</param>
		/// <param name="accessor">SQLアクセサ</param>
		/// <returns>オペレータリスト</returns>
		internal ShopOperatorModel[] GetMpOperatorList(
			Hashtable input,
			Dictionary<string, int> rowNumber,
			SqlAccessor accessor = null)
		{
			input.Add(Constants.FIELD_COMMON_BEGIN_NUM, rowNumber[Constants.FIELD_COMMON_BEGIN_NUM]);
			input.Add(Constants.FIELD_COMMON_END_NUM, rowNumber[Constants.FIELD_COMMON_END_NUM]);

			var dv = Get(XML_KEY_NAME, "GetMpOperatorList", input);
			return dv.Cast<DataRowView>().Select(drv => new ShopOperatorModel(drv)).ToArray();
		}
		#endregion

		#region ~Insert 登録
		/// <summary>
		/// 登録
		/// </summary>
		/// <param name="model">モデル</param>
		internal void Insert(ShopOperatorModel model)
		{
			Exec(XML_KEY_NAME, "Insert", model.DataSource);
		}
		#endregion

		#region ~Update 更新
		/// <summary>
		/// 更新
		/// </summary>
		/// <param name="model">モデル</param>
		/// <returns>影響を受けた件数</returns>
		internal int Update(ShopOperatorModel model)
		{
			var result = Exec(XML_KEY_NAME, "Update", model.DataSource);
			return result;
		}
		#endregion

		#region ~UpdateOperatorAdvAuthority  閲覧権限更新
		/// <summary>
		/// 閲覧権限更新
		/// </summary>
		/// <param name="input">パラメータ</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateOperatorAdvAuthority(Hashtable input)
		{
			var result = Exec(XML_KEY_NAME, "UpdateOperatorAdvAuthority", input);
			return result;
		}
		#endregion

		#region ~UpdateOperatorTagAuthority 閲覧可能なアフィリエイトタグ更新
		/// <summary>
		/// 閲覧可能なアフィリエイトタグ更新
		/// </summary>
		/// <param name="ht">パラメータ</param>
		/// <returns>影響を受けた件数</returns>
		internal void UpdateOperatorTagAuthority(Hashtable ht)
		{
			Exec(XML_KEY_NAME, "UpdateOperatorTagAuthority", ht);
		}
		#endregion

		#region ~UpdateAuthenticationCode 認証コード更新
		/// <summary>
		/// 認証コード更新
		/// </summary>
		/// <param name="shopId">モデル</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="authenticationCode">認証コード</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateAuthenticationCode(
			string shopId,
			string loginId,
			string authenticationCode)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SHOPOPERATOR_SHOP_ID, shopId },
				{ Constants.FIELD_SHOPOPERATOR_LOGIN_ID, loginId },
				{ Constants.FIELD_SHOPOPERATOR_AUTHENTICATION_CODE, authenticationCode },
			};

			var result = Exec(XML_KEY_NAME, "UpdateAuthenticationCode", input);

			return result;
		}
		#endregion

		#region ~UpdateRemoteAddress リモートIPアドレス更新
		/// <summary>
		/// リモートIPアドレス更新
		/// </summary>
		/// <param name="shopId">モデル</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="remoteAddress">リモートIPアドレス</param>
		/// <returns>影響を受けた件数</returns>
		internal int UpdateRemoteAddress(string shopId, string loginId, string remoteAddress)
		{
			var input = new Hashtable
			{
				{ Constants.FIELD_SHOPOPERATOR_SHOP_ID, shopId },
				{ Constants.FIELD_SHOPOPERATOR_LOGIN_ID, loginId },
				{ Constants.FIELD_SHOPOPERATOR_REMOTE_ADDR, remoteAddress },
			};

			var result = Exec(XML_KEY_NAME, "UpdateRemoteAddress", input);

			return result;
		}
		#endregion

		#region ~Delete 削除
		/// <summary>
		/// 削除
		/// </summary>
		/// <returns>影響を受けた件数</returns>
		/// <param name="shopId">店舗ID</param>
		/// <param name="operatorId">オペレータID</param>
		internal int Delete(string shopId, string operatorId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_SHOPOPERATOR_SHOP_ID, shopId},
				{Constants.FIELD_SHOPOPERATOR_OPERATOR_ID, operatorId},
			};
			var result = Exec(XML_KEY_NAME, "Delete", ht);
			return result;
		}
		#endregion

		#region +マスタ出力
		/// <summary>
		/// マスタをReaderで取得（CSV出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>Reader</returns>
		public SqlStatementDataReader GetMasterWithReader(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var reader = GetWithReader(XML_KEY_NAME, "GetShopOperatorMasterAndRealShopId", input, replaces);
			return reader;
		}

		/// <summary>
		/// マスタ取得（Excel出力用）
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		/// <returns>DataView</returns>
		public DataView GetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			var dv = Get(XML_KEY_NAME, "GetShopOperatorMasterAndRealShopId", input, replaces: replaces);
			return dv;
		}

		/// <summary>
		/// マスタ出力用フィールドチェック
		/// </summary>
		/// <param name="input">検索条件</param>
		/// <param name="replaces">置換値</param>
		public void CheckOperatorFieldsForGetMaster(Hashtable input, params KeyValuePair<string, string>[] replaces)
		{
			Get(XML_KEY_NAME, "CheckOperatorFields", input, replaces: replaces);
		}
		#endregion
	}
}
