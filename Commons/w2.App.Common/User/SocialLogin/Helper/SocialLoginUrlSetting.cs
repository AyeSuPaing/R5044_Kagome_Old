/*
=========================================================================================================
  Module      : ソーシャルログイン URL設定(SocialLoginUrlSetting.cs)
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
	/// ソーシャルログイン URL設定
	/// </summary>
	public class SocialLoginUrlSetting
	{
		/// <summary>
		/// URLタイプ
		/// </summary>
		public enum UrlType
		{
			/// <summary>本番環境</summary>
			Product,
			/// <summary>検証環境</summary>
			Test,
			/// <summary>w2社内開発環境</summary>
			Develop,
			/// <summary>w2社外開発環境</summary>
			W2Test
		}

		/// <summary>
		/// WebAPIのURL
		/// </summary>
		/// <param name="urlPrefix">URLプレフィックス</param>
		/// <returns>WebAPIのURL</returns>
		private Dictionary<UrlType, Dictionary<SocialLoginApiFunctionType, string>> ApiUrl(string urlPrefix)
		{
			Dictionary<UrlType, Dictionary<SocialLoginApiFunctionType, string>> m_apiUrl =
				new Dictionary<UrlType, Dictionary<SocialLoginApiFunctionType, string>>()
			{
				{
					UrlType.Product,
					new Dictionary<SocialLoginApiFunctionType, string>()
					{
						{SocialLoginApiFunctionType.Authenticate, urlPrefix + "/{0}/{1}/{2}/authenticate"},
						{SocialLoginApiFunctionType.AuthenticateAssociate, urlPrefix + "/{0}/{1}/{2}/authenticate/associate"},
						{SocialLoginApiFunctionType.AuthenticateLogin, urlPrefix + "/" + Constants.SOCIAL_LOGIN_ACCOUNT_ID + "/" + Constants.SOCIAL_LOGIN_SITE_ID + "/{0}/authenticate/login"},
						{SocialLoginApiFunctionType.AuthenticateRegistration, urlPrefix + "/" + Constants.SOCIAL_LOGIN_ACCOUNT_ID + "/" + Constants.SOCIAL_LOGIN_SITE_ID + "/{0}/authenticate/registration"},
						{SocialLoginApiFunctionType.AuthenticatedUser, urlPrefix + "/api/authenticated_user"},
						{SocialLoginApiFunctionType.Map, urlPrefix + "/api/map"},
						{SocialLoginApiFunctionType.Unmap, urlPrefix + "/api/unmap"},
						{SocialLoginApiFunctionType.ProvidersOfUser, urlPrefix + "/api/providers_of_user"},
						{SocialLoginApiFunctionType.UserAttribute, urlPrefix + "/api/user_attribute"},
						{SocialLoginApiFunctionType.Dissociate, urlPrefix + "/api/dissociate"},
						{SocialLoginApiFunctionType.CreateUser, urlPrefix + "/api/create_user"},
						{SocialLoginApiFunctionType.MergeUser, urlPrefix + "/api/merge_user"},
						{SocialLoginApiFunctionType.DeleteUser, urlPrefix + "/api/delete_user"},
						{SocialLoginApiFunctionType.AccessToken, urlPrefix + "/api/access_token"},
						{SocialLoginApiFunctionType.AssociationToken, urlPrefix + "/api/association_token"},
						{SocialLoginApiFunctionType.Users, urlPrefix + "/api/users"},
						{SocialLoginApiFunctionType.Profile, urlPrefix + "/api/profile"},
						{SocialLoginApiFunctionType.ProfileFromProviders, urlPrefix + "/api/profile_from_providers"},
						{SocialLoginApiFunctionType.UpdateProfile, urlPrefix + "/api/update_profile"},
						{SocialLoginApiFunctionType.DeleteProfile, urlPrefix + "/api/delete_profile"},
						{SocialLoginApiFunctionType.GrantProfile, urlPrefix + "/api/grant_profile"},
						{SocialLoginApiFunctionType.ShareLink, urlPrefix + "/api/share_link"},
						{SocialLoginApiFunctionType.Share, urlPrefix + "/api/share"},
						{SocialLoginApiFunctionType.ShareStatus, urlPrefix + "/api/share_status"},
						{SocialLoginApiFunctionType.CheckShared, urlPrefix + "/api/check_shared"},
						{SocialLoginApiFunctionType.Shares, urlPrefix + "/api/shares"},
						{SocialLoginApiFunctionType.ChangeActivityStatus, urlPrefix + "/api/change_activity_status"},
						{SocialLoginApiFunctionType.Comments, urlPrefix + "/api/comments"},
						{SocialLoginApiFunctionType.ChangeCommentStatus, urlPrefix + "/api/change_comment_status"},
						{SocialLoginApiFunctionType.Providers, urlPrefix + "/api/providers"},
						{SocialLoginApiFunctionType.Appinfo, urlPrefix + "/api/appinfo"},
						{SocialLoginApiFunctionType.SecretKey, urlPrefix + "/api/secret_key"},
						{SocialLoginApiFunctionType.Follow, urlPrefix + "/api/follow"},
						{SocialLoginApiFunctionType.Conversion, urlPrefix + "/api/conversion"},
						{SocialLoginApiFunctionType.LineFriends, urlPrefix + "/v2/line/friends"},
					}
				},
			};
			return m_apiUrl;
		}
		
		/// <summary>
		/// APIのURL取得
		/// </summary>
		/// <param name="functionType">機能区分</param>
		/// <param name="providertype">プロバイダ区分</param>
		/// <returns>APIのURL</returns>
		public string GetApiUrl(SocialLoginApiFunctionType functionType, SocialLoginApiProviderType providertype)
		{
			if (providertype == SocialLoginApiProviderType.Apple)
			{
				var urlRefix = Constants.SOCIAL_LOGIN_PROTOCOL_HTTPS + Constants.SOCIAL_LOGIN_FQDN_APPLE;
				return ApiUrl(urlRefix)[SocialLoginUrlSetting.UrlType.Product][functionType];
			}
			else
			{
				var urlRefix = Constants.SOCIAL_LOGIN_PROTOCOL_HTTPS + Constants.SOCIAL_LOGIN_FQDN;
				return ApiUrl(urlRefix)[SocialLoginUrlSetting.UrlType.Product][functionType];
			}
		}
	}
}
