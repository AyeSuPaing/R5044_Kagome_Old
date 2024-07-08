/*
=========================================================================================================
  Module      : 管理画面ログインインターフェース(IManagerLogin.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

namespace w2.Cms.Manager.Codes.ManagerCommon
{
	/// <summary>
	/// 管理画面ログインインターフェース（テストの時に利用）
	/// </summary>
	public interface IManagerLogin
	{
		/// <summary>
		/// ログインアクション
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		/// <returns>エラーメッセージ</returns>
		string LoginAction(string loginId, string password);

		/// <summary>
		/// Login
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="password">パスワード</param>
		/// <returns>A status code or error message</returns>
		string Login(string loginId, string password);

		/// <summary>
		/// Authentication
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <param name="authenticationCode">認証コード</param>
		/// <returns>A status code or error message</returns>
		string Authentication(string loginId, string authenticationCode);

		/// <summary>
		/// Resend authentication code
		/// </summary>
		/// <param name="loginId">ログインID</param>
		/// <returns>An error message or empty string</returns>
		string ResendCode(string loginId);
	}
}
