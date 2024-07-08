/*
=========================================================================================================
  Module      : モール連携設定サービスのインターフェース (IMallCooperationSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Domain.MallCooperationSetting
{
	/// <summary>
	/// モール連携設定サービスのインターフェース
	/// </summary>
	public interface IMallCooperationSettingService : IService
	{
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <returns>モデル</returns>
		MallCooperationSettingModel Get(string shopId, string mallId);

		/// <summary>
		/// 取得（全て）
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <returns>モデル列</returns>
		MallCooperationSettingModel[] GetAll(string shopId);

		/// <summary>
		/// モール区分からモール連携設定情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallKbn">モール区分</param>
		/// <returns>モデル列</returns>
		MallCooperationSettingModel[] GetValidByMallKbn(string shopId, string mallKbn);

		/// <summary>
		/// Yahoo API トークンセット更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="accessTokenExpirationDateTime">アクセストークン有効期限</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <param name="refreshTokenExpirationDateTime">リフレッシュトークン有効期限</param>
		void UpdateYahooApiTokenSet(
			string shopId,
			string mallId,
			string accessToken,
			DateTime? accessTokenExpirationDateTime,
			string refreshToken,
			DateTime? refreshTokenExpirationDateTime);

		/// <summary>
		/// Yahoo API 公開鍵認証更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="publicKeyAuthorizedDatetime">公開鍵最終認証日時</param>
		void UpdateYahooApiPublicKey(
			string shopId,
			string mallId,
			DateTime? publicKeyAuthorizedDatetime);

		/// <summary>
		/// Yahoo API トークンセット削除
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		void DeleteYahooApiTokenSet(string shopId, string mallId);
	}
}
