/*
=========================================================================================================
  Module      : ソーシャルログイン ログインモデル(SocialLoginModel.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace w2.App.Common.User.SocialLogin.Helper
{
	/// <summary>
	/// ソーシャルログイン ログインモデル
	/// </summary>
	[Serializable]
	public class SocialLoginModel
	{
		/// <summary>トークン</summary>
		public string Token { get; set; }
		/// <summary>ユーザID</summary>
		public string W2UserId { get; set; }
		/// <summary>ソーシャルプラスID</summary>
		public string SPlusUserId { get; set; }
		/// <summary>レスポンス文字列(JSON)</summary>
		public string RawResponse { get; set; }
		/// <summary>遷移元URL</summary>
		public string TransitionSourcePath { get; set; }
	}
}
