/*
=========================================================================================================
  Module      : ソーシャルログインWebAPI ユーザーのソーシャルプロバイダ認証情報の一括取得(SocialLoginUserAttribute.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using w2.App.Common.User.SocialLogin.Helper;

namespace w2.App.Common.User.SocialLogin
{
	/// <summary>
	/// ユーザーのソーシャルプロバイダ認証情報の一括取得
	/// </summary>
	public class SocialLoginUserAttribute : SocialLoginBase
	{
		/// <summary>
		/// コンストラクタ
		/// </summary>
		public SocialLoginUserAttribute()
			: base(SocialLoginApiFunctionType.UserAttribute)
		{
		}

		/// <summary>
		/// 実行
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="socialPlusId">取得対象ソーシャルPLUS ID</param>
		/// <param name="userId">取得対象ユーザーID</param>
		/// <returns>レスポンス(JSON形式)</returns>
		public string Exec(
			string apiKey,
			string socialPlusId,
			string userId)
		{
			var parameter = CreateParam(apiKey, socialPlusId, userId);
			return GetHttpRequest(parameter);
		}

		/// <summary>
		/// パラメータ作成
		/// </summary>
		/// <param name="apiKey">APIキー</param>
		/// <param name="socialPlusId">取得対象ソーシャルPLUS ID</param>
		/// <param name="userId">取得対象ユーザーID</param>
		/// <returns>パラメータ</returns>
		/// <remarks>ソーシャルPLUS IDとユーザーIDのどちらかを指定していればOK</remarks>
		private string[][] CreateParam(
			string apiKey,
			string socialPlusId,
			string userId)
		{
			var parameter = new []
			{
				new [] { "key", apiKey },
				new [] { "identifier", socialPlusId },
				new [] { "primary_key", userId }
			};
			return parameter;
		}
	}
}
