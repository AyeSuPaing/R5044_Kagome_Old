/*
=========================================================================================================
  Module      : ソーシャルログイン ログインユーティリティ(SocialLoginUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2017 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using w2.Common.Web;
using w2.Domain.User;
using w2.Domain.UpdateHistory.Helper;
using w2.App.Common.User.SocialLogin.Helper;

namespace w2.App.Common.User.SocialLogin.Util
{
	/// <summary>
	/// ソーシャルログイン ログインユーティリティ
	/// </summary>
	public class SocialLoginUtil
	{
		#region 定数
		/// <summary>キー：ステータス</summary>
		public const string JSON_KEY_STATUS = "status";
		/// <summary>キー：トークン</summary>
		public const string JSON_KEY_TOKEN = "token";
		/// <summary>キー：ユーザ情報</summary>
		public const string JSON_KEY_USER = "user";
		/// <summary>キー：プロバイダ</summary>
		public const string JSON_KEY_PROVIDERS = "providers";
		/// <summary>キー：識別子</summary>
		public const string JSON_KEY_USER_IDENTIFIER = "identifier";
		/// <summary>キー：主キー</summary>
		public const string JSON_KEY_USER_PRIMARY_KEY = "primary_key";

		/// <summary>値：ステータス「ok」</summary>
		public const string STATUS_VALUE_OK = "ok";
		/// <summary>値：ステータス「authorized」</summary>
		public const string STATUS_VALUE_AUTHORIZED = "authorized";
		#endregion

		/// <summary>URL設定</summary>
		public static SocialLoginUrlSetting m_salus = new SocialLoginUrlSetting();

		/// <summary>
		/// クエリパラメータを取得します。
		/// </summary>
		/// <param name="param">パラメータ</param>
		/// <returns>クエリパラメータ</returns>
		public static string GetQueryParam(string[][] param)
		{
			return string.Join("&",
				param.Where(p => p[1] != null)
				.Select(p => string.Format("{0}={1}", p[0], HttpUtility.UrlEncode(p[1], Encoding.UTF8))));
		}

		/// <summary>
		/// ユーザ情報を取得します。
		/// </summary>
		/// <param name="token">WebAPI（association_token）のレスポンスに含まれるトークンを設定</param>
		/// <param name="preserveToken">true：ワンタイムトークンを削除しない</param>
		/// <param name="addProfile">true：個人情報(profile)をレスポンスに含める</param>
		/// <param name="deleteProfile">WebAPI コール後にソーシャルプラス上の個人情報を削除</param>
		/// <returns>ユーザ情報</returns>
		public static SocialLoginModel GetUser(
			string token,
			bool preserveToken,
			bool addProfile,
			bool deleteProfile)
		{
			var authUser = new SocialLoginAuthenticatedUser();
			var response = authUser.Exec(Constants.SOCIAL_LOGIN_API_KEY, token, preserveToken, addProfile, deleteProfile);
			var json = JsonConvert.DeserializeObject<JObject>(response);

			var status = (string)json[JSON_KEY_STATUS];
			if (status == STATUS_VALUE_OK)
			{
				var userData = (JObject)json[JSON_KEY_USER];
				var socialLogin = new SocialLoginModel
				{
					Token = token,
					SPlusUserId = userData[JSON_KEY_USER_IDENTIFIER].ToString(),
					W2UserId = userData[JSON_KEY_USER_PRIMARY_KEY].ToString(),
					RawResponse = response
				};
				return socialLogin;
			}
			return null;
		}

		/// <summary>
		/// ユーザの連携済みプロバイダを取得します。
		/// </summary>
		/// <param name="splusUserId">ソーシャルプラスID</param>
		/// <param name="w2UserId">ユーザID</param>
		/// <returns>ユーザの連携済みプロバイダ</returns>
		public static List<SocialLoginApiProviderType> GetProviders(string splusUserId, string w2UserId)
		{
			var pou = new SocialLoginProvidersOfUser();
			var response = pou.Exec(Constants.SOCIAL_LOGIN_API_KEY, splusUserId, w2UserId);
			var json = JsonConvert.DeserializeObject<JObject>(response);
			var socialProviders = new List<SocialLoginApiProviderType>();

			var status = (string)json[JSON_KEY_STATUS];
			if (status == STATUS_VALUE_OK)
			{
				var providers = (JArray)json[JSON_KEY_PROVIDERS];
				// 対応プロバイダ
				foreach (SocialLoginApiProviderType providerType in Enum.GetValues(typeof(SocialLoginApiProviderType)))
				{
					// 連携済プロバイダ
					foreach (JToken provider in providers)
					{
						if (providerType.ToValue() == provider.ToString())
						{
							socialProviders.Add(providerType);
							break;
						}
					}
				}
			}
			return socialProviders;
		}

		/// <summary>
		/// ユーザーの連携済みソーシャルプロバイダのアカウント情報を取得します。
		/// </summary>
		/// <param name="splusUserId">ソーシャルプラスID</param>
		/// <param name="w2UserId">ユーザーID</param>
		/// <returns>ユーザーの連携済みソーシャルプロバイダのアカウント情報</returns>
		public static ISnsProvider[] GetSocialProviderAccounts(string splusUserId, string w2UserId)
		{
			var response = new SocialLoginUserAttribute().Exec(Constants.SOCIAL_LOGIN_API_KEY, splusUserId, w2UserId);
			var userAttribute = JsonConvert.DeserializeObject<SocialLoginUserAttributeModel>(response);
			if (userAttribute.Status != STATUS_VALUE_OK) return null;

			return userAttribute.GetSocialProviderAccounts();
		}

		/// <summary>
		/// 既存ユーザ、または新規ユーザをソーシャルログイン連携します。
		/// </summary>
		/// <param name="userId">ユーザID</param>
		/// <param name="providerType">プロバイダ区分</param>
		/// <param name="urlAuthority">URLオーソリティ</param>
		/// <returns>リダイレクト先URL</returns>
		public static string AuthenticateOrAssociate(string userId, SocialLoginApiProviderType providerType, string urlAuthority)
		{
			var slat = new SocialLoginAssociationToken();
			var response = slat.Exec(Constants.SOCIAL_LOGIN_API_KEY, null, userId, providerType);
			var json = JsonConvert.DeserializeObject<JObject>(response);

			var status = (string)json[JSON_KEY_STATUS];
			if (status == STATUS_VALUE_OK)
			{
				// 既存ユーザ連携
				var tokenObj = (JObject)json[JSON_KEY_TOKEN];
				var token = (string)tokenObj[JSON_KEY_TOKEN];

				var slaa = new SocialLoginAuthenticateAssociate(providerType);
				var callBackUrl = Constants.PROTOCOL_HTTPS + urlAuthority + Constants.PATH_ROOT + Constants.PAGE_FRONT_SOCIAL_LOGIN_COOPERATION;
				var redirectUrl = slaa.GetUrl(callBackUrl, callBackUrl, token);
				return redirectUrl;
			}
			else
			{
				// 新規ユーザ連携
				var sla = new SocialLoginAuthenticate(providerType);
				var callBackUrl = Constants.PROTOCOL_HTTPS + urlAuthority + Constants.PATH_ROOT + Constants.PAGE_FRONT_SOCIAL_LOGIN_COOPERATION_CALLBACK;
				var redirectUrl = sla.GetUrl(callBackUrl, callBackUrl, true);
				return redirectUrl;
			}
		}

		/// <summary>
		/// ソーシャルプラスの認証用URLを生成します。
		/// </summary>
		/// <param name="providerType">プロバイダ区分</param>
		/// <param name="callBackPath">認証成功時に遷移させたいパス</param>
		/// <param name="errorCallBackPath">認証失敗時に遷移させたいパス</param>
		/// <param name="profile">true：基本情報以外に、追加情報もレスポンスに含める場合</param>
		/// <param name="urlAuthority">URLオーソリティ</param>
		/// <returns>認証用URL</returns>
		public static string GetAuthenticateUrl(
			SocialLoginApiProviderType providerType,
			string callBackPath,
			string errorCallBackPath,
			bool profile,
			string urlAuthority)
		{
			var sla = new SocialLoginAuthenticate(providerType);
			var callBackUrl = Constants.PROTOCOL_HTTPS + urlAuthority + Constants.PATH_ROOT + callBackPath;
			var errorCallBackUrl = Constants.PROTOCOL_HTTPS + urlAuthority + Constants.PATH_ROOT + errorCallBackPath;
			return sla.GetUrl(callBackUrl, errorCallBackUrl, profile);
		}

		/// <summary>
		/// ソーシャルプラスの認証用URLを生成します(ランディングページ用)
		/// </summary>
		/// <param name="providerType">プロバイダ区分</param>
		/// <param name="callBackPath">認証成功時に遷移させたいパス</param>
		/// <param name="errorCallBackPath">認証失敗時に遷移させたいパス</param>
		/// <param name="profile">true：基本情報以外に、追加情報もレスポンスに含める場合</param>
		/// <param name="urlAuthority">URLオーソリティ</param>
		/// <param name="returnPath">戻り先パス</param>
		/// <returns>認証用URL</returns>
		public static string GetAuthenticateUrl(
			SocialLoginApiProviderType providerType,
			string callBackPath,
			string errorCallBackPath,
			bool profile,
			string urlAuthority,
			string returnPath)
		{
			// Hack:V5へマージする際は、上のメソッドと統合したい。
			// パスルート等はGetAuthenticateUrlメソッドでつける
			var callBackUrl = new UrlCreator(callBackPath)
				.AddParam(Constants.REQUEST_KEY_RETURN_URL, returnPath).CreateUrl();
			return GetAuthenticateUrl(providerType, callBackUrl, errorCallBackPath, profile, urlAuthority);
		}

		/// <summary>
		/// エラーメッセージ取得
		/// </summary>
		/// <param name="reason">エラー理由</param>
		/// <returns>エラーメッセージ</returns>
		public static string GetErrorMessage(string reason)
		{
			if (string.IsNullOrEmpty(reason)) return string.Empty;

			var document = XDocument.Parse(Properties.Resources.SocialPlusErrorMessages);
			var errorMessages =
				from elem in document.Root.Elements("ErrorTypes")
				where string.Equals(elem.Attribute("method").Value, "AUTH", StringComparison.OrdinalIgnoreCase)
				select elem;
			var errorMessage = errorMessages.Elements("Message")
				.Where(e => string.Equals(e.Attribute("reason").Value, reason, StringComparison.OrdinalIgnoreCase));

			// 対応するメッセージが存在しない場合は、そのまま表示する
			if (errorMessage.Any() == false) return reason;

			return errorMessage.First().Value;
		}

		/// <summary>
		/// ソーシャルプロバイダ認証情報の同期
		/// </summary>
		/// <param name="splusUserId">ソーシャルプラスID</param>
		/// <param name="w2UserId">ユーザーID</param>
		/// <param name="userExtend">ユーザー拡張項目</param>
		public static void SyncSocialProviderInfo(string splusUserId, string w2UserId, UserExtendModel userExtend = null)
		{
			var service = new UserService();
			var model = userExtend ?? service.GetUserExtend(w2UserId);
			SetSocialProviderInfo(splusUserId, w2UserId, model);

			service.UpdateUserExtend(model, w2UserId, Constants.FLG_LASTCHANGED_USER, UpdateHistoryAction.DoNotInsert);
		}

		/// <summary>
		/// ユーザー拡張項目にソーシャルプロバイダ認証情報をセット
		/// </summary>
		/// <param name="splusUserId">ソーシャルプラスID</param>
		/// <param name="w2UserId">ユーザーID</param>
		/// <param name="userExtend">ユーザー拡張項目</param>
		public static void SetSocialProviderInfo(string splusUserId, string w2UserId, UserExtendModel userExtend)
		{
			var accounts = GetSocialProviderAccounts(splusUserId, w2UserId);
			if (accounts == null) return;

			Constants.SOCIAL_PROVIDER_ID_USEREXTEND_COLUMN_NAMES.Where(
				providerName => userExtend.UserExtendDataValue.ContainsKey(providerName)).ToList().ForEach(
					providerName =>
					{
						var provider = accounts.FirstOrDefault(account => account.ColumnName == providerName);
						userExtend.UserExtendDataValue[providerName] = (provider != null) ? provider.ProviderId : string.Empty;
					});
		}

		/// <summary>
		/// 既存ユーザーの場合にユーザー結合
		/// </summary>
		/// <param name="w2UserId">ユーザーID</param>
		/// <param name="splusUserId">ソーシャルプラスID</param>
		/// <returns>新規ユーザーかどうか</returns>
		public static bool MergeIfExists(string w2UserId, string splusUserId)
		{
			var slua = new SocialLoginUserAttribute();
			var response = slua.Exec(Constants.SOCIAL_LOGIN_API_KEY, null, w2UserId);
			var json = JsonConvert.DeserializeObject<JObject>(response);
			var status = (string)json[JSON_KEY_STATUS];
			if (status == STATUS_VALUE_OK)
			{
				var userData = (JObject)json[JSON_KEY_USER];
				var destSplusUserId = userData[JSON_KEY_USER_IDENTIFIER].ToString();
				// ユーザー結合
				var slmu = new SocialLoginMergeUser();
				slmu.Exec(Constants.SOCIAL_LOGIN_API_KEY, splusUserId, destSplusUserId);
				return false;
			}

			return true;
		}
	}
}