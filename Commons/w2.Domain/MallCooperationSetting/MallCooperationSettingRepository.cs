/*
=========================================================================================================
  Module      : モール連携設定リポジトリ (MallCooperationSettingRepository.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using w2.Common.Sql;

namespace w2.Domain.MallCooperationSetting
{
	/// <summary>
	/// モール連携設定リポジトリ
	/// </summary>
	public class MallCooperationSettingRepository : RepositoryBase
	{
		/// <returns>影響を受けた件数</returns>
		private const string XML_KEY_NAME = "MallCooperationSetting";

		#region コンストラクタ
		/// <summary>
		/// デフォルトコンストラクタ
		/// </summary>
		public MallCooperationSettingRepository()
		{
		}
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public MallCooperationSettingRepository(SqlAccessor accessor)
			: base(accessor)
		{
		}
		#endregion

		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <returns>モデル</returns>
		public MallCooperationSettingModel Get(string shopId, string mallId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, shopId},
				{Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID, mallId},
			};
			var dv = Get(XML_KEY_NAME, "Get", ht);
			if (dv.Count == 0) return null;
			return new MallCooperationSettingModel(dv[0]);
		}
		#endregion

		#region +GetAll 取得（全て）
		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル列</returns>
		public MallCooperationSettingModel[] GetAll(string shopId)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, shopId}
			};
			var dv = Get(XML_KEY_NAME, "GetAll", ht);
			return dv.Cast<DataRowView>().Select(drv => new MallCooperationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~GetValidByMallKbn モール区分からモール連携設定情報取得
		/// <summary>
		/// モール区分からモール連携設定情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallKbn">モール区分</param>
		/// <returns>モデル列</returns>
		internal MallCooperationSettingModel[] GetValidByMallKbn(string shopId, string mallKbn)
		{
			var ht = new Hashtable
			{
				{Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, shopId},
				{Constants.FIELD_MALLCOOPERATIONSETTING_MALL_KBN, mallKbn}
			};
			var dv = Get(XML_KEY_NAME, "GetValidByMallKbn", ht);
			return dv.Cast<DataRowView>().Select(drv => new MallCooperationSettingModel(drv)).ToArray();
		}
		#endregion

		#region ~UpdateYahooApiTokenSet Yahoo API トークンセット更新
		/// <summary>
		/// Yahoo API トークンセット更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="accessTokenExpirationDateTime">アクセストークン有効期限</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <param name="refreshTokenExpirationDateTime">リフレッシュトークン有効期限</param>
		internal bool UpdateYahooApiTokenSet(
			string shopId,
			string mallId,
			string accessToken,
			DateTime? accessTokenExpirationDateTime,
			string refreshToken,
			DateTime? refreshTokenExpirationDateTime)
		{
			var ht = new Hashtable
			{
				{ Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID, mallId },
				{ Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_ACCESS_TOKEN, accessToken },
				{ Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_ACCESS_TOKEN_EXPIRATION_DATETIME, accessTokenExpirationDateTime },
				{ Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_REFRESH_TOKEN, refreshToken },
				{ Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_REFRESH_TOKEN_EXPIRATION_DATETIME, refreshTokenExpirationDateTime },
			};
			var result = Exec(XML_KEY_NAME, "UpdateYahooApiTokenSet", ht);
			return (result > 0);
		}
		#endregion

		#region ~UpdateYahooApiPublicKey Yahoo API 公開鍵認証更新
		/// <summary>
		/// Yahoo API 公開鍵認証更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="publicKeyAuthorizedDatetime">公開鍵最終認証日時</param>
		/// <returns>影響件数</returns>
		internal bool UpdateYahooApiPublicKey(
			string shopId,
			string mallId,
			DateTime? publicKeyAuthorizedDatetime)
		{
			var parameters = new Hashtable
			{
				{ Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID, mallId },
				{ Constants.FIELD_MALLCOOPERATIONSETTING_YAHOO_API_LAST_PUBLIC_KEY_AUTHORIZED_AT, publicKeyAuthorizedDatetime },
			};
			var result = Exec(XML_KEY_NAME, "UpdateYahooApiPublicKey", parameters);
			return result > 0;
		}
		#endregion

		#region ~DeleteYahooApiTokenSet Yahoo API トークンセット削除
		/// <summary>
		/// Yahoo API 公開鍵認証更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <returns>影響件数</returns>
		internal bool DeleteYahooApiTokenSet(
			string shopId,
			string mallId)
		{
			var parameters = new Hashtable
			{
				{ Constants.FIELD_MALLCOOPERATIONSETTING_SHOP_ID, shopId },
				{ Constants.FIELD_MALLCOOPERATIONSETTING_MALL_ID, mallId },
			};
			var result = Exec(XML_KEY_NAME, "DeleteYahooApiTokenSet", parameters);
			return result > 0;
		}
		#endregion
	}
}
