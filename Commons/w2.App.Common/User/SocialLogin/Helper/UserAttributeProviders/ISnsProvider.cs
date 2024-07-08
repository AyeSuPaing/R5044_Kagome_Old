/*
=========================================================================================================
  Module      : ソーシャルログイン ソーシャルプロバイダ別認証情報インターフェース(ISnsProvider.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2018 All Rights Reserved.
=========================================================================================================
*/
using System;

namespace w2.App.Common.User.SocialLogin.Helper
{
	/// <summary>
	/// ソーシャルプロバイダ別認証情報インターフェース
	/// </summary>
	public interface ISnsProvider
	{
		/// <summary>ソーシャルプロバイダ区分</summary>
		SocialLoginApiProviderType ProviderType { get; }
		/// <summary>ソーシャルプロバイダID</summary>
		string ProviderId { get; }
		/// <summary>ソーシャルプロバイダID登録用カラム名</summary>
		string ColumnName { get; }
	}
}