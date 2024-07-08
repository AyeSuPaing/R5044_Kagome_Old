/*
=========================================================================================================
  Module      : モール連携設定サービス (MallCooperationSettingService.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2015 All Rights Reserved.
=========================================================================================================
*/

using System;

namespace w2.Domain.MallCooperationSetting
{
	/// <summary>
	/// モール連携設定サービス
	/// </summary>
	public class MallCooperationSettingService : ServiceBase, IMallCooperationSettingService
	{
		#region +Get 取得
		/// <summary>
		/// 取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <returns>モデル</returns>
		public MallCooperationSettingModel Get(string shopId, string mallId)
		{
			using (var repository = new MallCooperationSettingRepository())
			{
				var model = repository.Get(shopId, mallId);
				return model;
			}
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
			using (var repository = new MallCooperationSettingRepository())
			{
				return repository.GetAll(shopId);
			}
		}
		#endregion

		#region +GetValidByMallKbn モール区分からモール連携設定情報取得
		/// <summary>
		/// モール区分からモール連携設定情報取得
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallKbn">モール区分</param>
		/// <returns>モデル列</returns>
		public MallCooperationSettingModel[] GetValidByMallKbn(string shopId, string mallKbn)
		{
			using (var repository = new MallCooperationSettingRepository())
			{
				return repository.GetValidByMallKbn(shopId, mallKbn);
			}
		}
		#endregion

		#region +UpdateYahooApiTokenSet Yahoo API トークンセット更新
		/// <summary>
		/// Yahoo API トークンセット更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="accessToken">アクセストークン</param>
		/// <param name="accessTokenExpirationDateTime">アクセストークン有効期限</param>
		/// <param name="refreshToken">リフレッシュトークン</param>
		/// <param name="refreshTokenExpirationDateTime">リフレッシュトークン有効期限</param>
		public void UpdateYahooApiTokenSet(
			string shopId,
			string mallId,
			string accessToken,
			DateTime? accessTokenExpirationDateTime,
			string refreshToken,
			DateTime? refreshTokenExpirationDateTime)
		{
			using (var repository = new MallCooperationSettingRepository())
			{
				repository.UpdateYahooApiTokenSet(
					shopId,
					mallId,
					accessToken,
					accessTokenExpirationDateTime,
					refreshToken,
					refreshTokenExpirationDateTime);
			}
		}
		#endregion

		#region +UpdateYahooApiPublicKey Yahoo API 公開鍵認証更新
		/// <summary>
		/// Yahoo API 公開鍵認証更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		/// <param name="publicKey">公開鍵</param>
		/// <param name="publicKeyVersion">公開鍵バージョン</param>
		/// <param name="publicKeyAuthorizedDatetime">公開鍵最終認証日時</param>
		public void UpdateYahooApiPublicKey(
			string shopId,
			string mallId,
			DateTime? publicKeyAuthorizedDatetime)
		{
			using (var repository = new MallCooperationSettingRepository())
			{
				repository.UpdateYahooApiPublicKey(
					shopId,
					mallId,
					publicKeyAuthorizedDatetime);
			}
		}
		#endregion

		#region +DeleteYahooApiTokenSet Yahoo API トークンセット削除
		/// <summary>
		/// Yahoo API 公開鍵認証更新
		/// </summary>
		/// <param name="shopId">店舗ID</param>
		/// <param name="mallId">モールID</param>
		public void DeleteYahooApiTokenSet(
			string shopId,
			string mallId)
		{
			using (var repository = new MallCooperationSettingRepository())
			{
				repository.DeleteYahooApiTokenSet(shopId, mallId);
			}
		}
		#endregion
	}
}
