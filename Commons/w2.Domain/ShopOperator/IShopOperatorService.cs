/*
=========================================================================================================
  Module      : 店舗管理者サービスインターフェース (IShopOperatorService.cs)
  ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2022 All Rights Reserved.
=========================================================================================================
*/
using w2.Common.Sql;

namespace w2.Domain.ShopOperator
{
	/// <summary>
	/// 店舗管理者サービスインターフェース
	/// </summary>
	public interface IShopOperatorService : IService
	{
		/// <summary>
		/// ログインIDから取得
		/// </summary>
		/// <param name="shopId">モデル</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="accessor">SQLアクセサ</param>
		ShopOperatorModel GetByLoginId(string shopId, string loginId, SqlAccessor accessor = null);

		/// <summary>
		/// ログインIDから有効フラグオフ更新
		/// </summary>
		/// <param name="shopId">モデル</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="lastChanged">最終更新者</param>
		int UpdateValidFlgOffByLoginId(string shopId, string loginId, string lastChanged);

		/// <summary>
		/// 認証コード更新
		/// </summary>
		/// <param name="shopId">モデル</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="authenticationCode">認証コード</param>
		int UpdateAuthenticationCode(string shopId, string loginId, string authenticationCode);

		/// <summary>
		/// リモートIPアドレス更新
		/// </summary>
		/// <param name="shopId">モデル</param>
		/// <param name="loginId">ログインID</param>
		/// <param name="remoteAddress">リモートIPアドレス</param>
		int UpdateRemoteAddress(string shopId, string loginId, string remoteAddress);
	}
}
