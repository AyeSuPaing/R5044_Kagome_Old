/*
=========================================================================================================
  Module      : ログイン入力クラス(LoginInput.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2019 All Rights Reserved.
=========================================================================================================
*/

using w2.Cms.Manager.Codes;
using w2.Cms.Manager.Codes.Binder;

namespace w2.Cms.Manager.Input
{
	/// <summary>
	/// ログイン入力クラス
	/// </summary>
	public class LoginInput
	{
		/// <summary>ログインID</summary>
		[BindAlias(Constants.REQUEST_KEY_MANAGER_LOGIN_ID)]
		public string LoginId { get; set; }
		/// <summary>パスワード</summary>
		public string Password { get; set; }
		/// <summary>次URL</summary>
		[BindAlias(Constants.REQUEST_KEY_MANAGER_NEXTURL)]
		public string NextUrl { get; set; }
		/// <summary>Login expired flag</summary>
		public string LoginExpiredFlg { get; set; }
	}
}