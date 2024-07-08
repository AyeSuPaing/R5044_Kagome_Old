/*
=========================================================================================================
  Module      : ソーシャルログイン プロバイダ区分(SocialLoginApiProviderType.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace w2.App.Common.User.SocialLogin.Helper
{
	/// <summary>
	/// ソーシャルログイン プロバイダ区分
	/// </summary>
	public enum SocialLoginApiProviderType
	{
		/// <summary>プロバイダ: Facebook</summary>
		Facebook = 1,
		/// <summary>プロバイダ: Twitter</summary>
		Twitter = 2,
		/// <summary>プロバイダ: Google</summary>
		Google = 3,
		/// <summary>プロバイダ: Gplus</summary>
		Gplus = 4,
		/// <summary>プロバイダ: Mixi</summary>
		Mixi = 5,
		/// <summary>プロバイダ: Yahoo</summary>
		Yahoo = 6,
		/// <summary>プロバイダ: Rakuten</summary>
		Rakuten = 7,
		/// <summary>プロバイダ: Line</summary>
		Line = 8,
		// PayPalは独自連携を行う
		///// <summary>プロバイダ: Paypal</summary>
		//Paypal = 9,
		/// <summary>プロバイダ: Apple</summary>
		Apple = 10,
	}

	#region 拡張クラス
	/// <summary>
	/// ソーシャルログイン プロバイダ区分（拡張クラス）
	/// </summary>
	static class SocialLoginApiProviderTypeExtension
	{
		/// <summary>
		/// プロバイダ区分からコード値を取得します。
		/// </summary>
		/// <param name="providerType">プロバイダ区分</param>
		/// <returns>コード値</returns>
		public static string ToValue(this SocialLoginApiProviderType providerType)
		{
			return providerType.ToString().ToLower();
		}
	}
	#endregion
}
