/*
=========================================================================================================
  Module      : 楽天IDConnectアクション情報クラス(RakutenIDConnectActionInfo.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;
using w2.Domain.User;

namespace w2.App.Common.Auth.RakutenIDConnect
{
	/// <summary>アクション種別</summary>
	public enum ActionType
	{
		/// <summary>ログイン</summary>
		Login,
		/// <summary>ユーザー新規登録</summary>
		UserRegister,
		/// <summary>楽天OpenID連携</summary>
		Connect,
	}

	/// <summary>
	/// 楽天IDConnectアクション情報クラス
	/// </summary>
	[Serializable]
	public class RakutenIDConnectActionInfo
	{
		#region コンストラクタ
		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="type">アクション種別</param>
		/// <param name="beforeUrl">ログイン前のURL</param>
		/// <param name="nextUrl">ログイン後の遷移先URL</param>
		/// <param name="isLandingCart">ランディングカートか</param>
		public RakutenIDConnectActionInfo(ActionType type, string beforeUrl, string nextUrl, bool isLandingCart = false)
		{
			this.Type = type;
			this.BeforeUrl = beforeUrl;
			this.NextUrl = nextUrl;
			this.State = Guid.NewGuid().ToString("N");
			this.Nonce = Guid.NewGuid().ToString("N");
			this.IsLandingCart = isLandingCart;
		}
		#endregion

		#region メソッド
		/// <summary>
		/// ユーザー情報セット
		/// </summary>
		/// <param name="user">ユーザー情報</param>
		public void SetUser(UserModel user)
		{
			this.User = user;
		}

		/// <summary>
		/// 楽天ID会員情報取得レスポンスデータセット
		/// </summary>
		/// <param name="userInfoResponseData">楽天ID会員情報取得レスポンスデータ</param>
		public void SetRakutenIDConnectUserInfoResponseData(RakutenIDConnectUserInfoResponseData userInfoResponseData)
		{
			this.RakutenIdConnectUserInfoResponseData = userInfoResponseData;
		}
		#endregion

		#region プロパティ
		/// <summary>アクション種別</summary>
		public ActionType Type { get; private set; }
		/// <summary>ログイン前のURL</summary>
		public string BeforeUrl { get; private set; }
		/// <summary>ログイン後の遷移先URL</summary>
		public string NextUrl { get; private set; }
		/// <summary>ステート</summary>
		public string State { get; private set; }
		/// <summary>ノンス</summary>
		public string Nonce { get; private set; }
		/// <summary>ユーザー情報</summary>
		public UserModel User { get; private set; }
		/// <summary>楽天ID会員情報取得レスポンスデータ</summary>
		public RakutenIDConnectUserInfoResponseData RakutenIdConnectUserInfoResponseData { get; private set; }
		/// <summary>ランディングカートか</summary>
		public bool IsLandingCart { get; private set; }
		#endregion
	}
}