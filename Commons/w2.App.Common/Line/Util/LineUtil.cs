/*
=========================================================================================================
  Module      : LINEユーティリティ(LineUtil.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/

using System.Web.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using w2.Common.Web;
using w2.Domain.UpdateHistory.Helper;
using w2.Domain.User;


namespace w2.App.Common.Line.Util
{
	public class LineUtil
	{
		/// <summary>
		/// LINE直接連携のリクエストURL作成
		/// </summary>
		/// <param name="sessionId">セッションID</param>
		/// <returns>リクエストURL</returns>
		public static string CreateConnectLineUrl(string sessionId)
		{
			var lineUrl = new UrlCreator(Constants.LINE_DIRECT_CONNECT_PATH)
				.AddParam(Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_RESPONSE_TYPE, Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_CODE)
				.AddParam(Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_CLIENT_ID, Constants.LINE_DIRECT_CONNECT_CLIENT_ID)
				.AddParam(Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_STATE, sessionId)
				.AddParam(Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_REDIRECT_URI, Constants.LINE_DIRECT_CONNECT_REQUEST_VALUE_REDIRECT_URI)
				.AddParam(Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_SCOPE, Constants.LINE_DIRECT_CONNECT_REQUEST_VALUE_SCOPE)
				.AddParam(Constants.LINE_DIRECT_CONNECT_REQUEST_KEY_BOT_PROMPT, Constants.LINE_DIRECT_CONNECT_REQUEST_VALUE_BOT_PROMPT)
				.CreateUrl();

			return lineUrl;
		}

		/// <summary>
		/// LINEユーザーIDからユーザー情報取得
		/// </summary>
		/// <param name="lineUserId">LINEユーザーID</param>
		/// <param name="providerId">カラム名</param>
		/// <returns>ユーザーモデル</returns>
		public static UserModel GetUserByLineUserId(string lineUserId, string providerId)
		{
			// 列の存在チェック
			var userService = new UserService();
			if (userService.UserExtendColumnExists(providerId) == false) return null;

			// ユーザーIDから取得
			var user = userService.GetUserByExternalUserId(providerId, lineUserId);

			return user;
		}

		/// <summary>
		/// LINE認証
		/// </summary>
		/// <returns>LINEユーザーID</returns>
		public static string AuthenticationLine(string code)
		{
			var accessToken = GetAccessToken(code);
			var providerUserId = GetProviderUserId(accessToken);
			return providerUserId;
		}

		/// <summary>
		/// アクセストークンの取得
		/// </summary>
		/// <param name="code">認可コード</param>
		/// <returns>アクセストークン</returns>
		public static string GetAccessToken(string code)
		{
			var lineManager = new LineDirectConnect.LineDirectConnectManager();
			var response = lineManager.GetAccessToken(Constants.LINE_DIRECT_CONNECT_REQUEST_ACCESS_TOKEN_PATH, code);

			var requestJson = (JObject)JsonConvert.DeserializeObject(response);

			var accessToken = requestJson[Constants.LINE_DIRECT_CONNECT_JSON_REQUEST_KEY_ACCESS_TOKEN];

			return accessToken.ToString();
		}

		/// <summary>
		/// ユーザーIDの取得
		/// </summary>
		/// <param name="accessToken">アクセストークン</param>
		/// <returns>ユーザーID</returns>
		public static string GetProviderUserId(string accessToken)
		{
			var lineManager = new LineDirectConnect.LineDirectConnectManager();
			var response = lineManager.GetProviderUserId(Constants.LINE_DIRECT_CONNECT_REQUEST_LINE_PROVIDER_ID_PATH, accessToken);

			var requestJson = (JObject)JsonConvert.DeserializeObject(response);

			var providerUserId = requestJson[Constants.LINE_DIRECT_CONNECT_JSON_REQUEST_KEY_USER_ID];

			return providerUserId.ToString();
		}

		/// <summary>
		/// ユーザー拡張項目にLINEユーザーIDをセット
		/// </summary>
		/// <param name="userExtend">ユーザー拡張項目</param>
		/// <param name="w2UserId">ユーザーID</param>
		/// <param name="lineUserId">LINEユーザーID</param>
		public static void SetLineUserIdForUserExtend(UserExtendModel userExtend, string w2UserId, string lineUserId)
		{
			userExtend.UserExtendDataValue[Common.Constants.SOCIAL_PROVIDER_ID_LINE] = lineUserId;
			new UserService().UpdateUserExtend(
				userExtend,
				w2UserId,
				Common.Constants.FLG_LASTCHANGED_USER,
				UpdateHistoryAction.DoNotInsert);
		}

		/// <summary>
		/// ユーザーログインIDからLINEIDの取得
		/// </summary>
		/// <param name="w2UserId">ログインID</param>
		/// <returns>LINEID</returns>
		public static string GetLineUserIdByLoginUserId(string w2UserId)
		{
			var lineUserId = string.Empty;
			var userService = new UserService();
			var user = userService.Get(w2UserId);
			if (string.IsNullOrEmpty(Common.Constants.SOCIAL_PROVIDER_ID_LINE) == false)
			{
				lineUserId = user.GetUserExtend().UserExtendDataValue[Common.Constants.SOCIAL_PROVIDER_ID_LINE];
			}

			return lineUserId;
		}

		/// <summary>
		/// 連携解除
		/// </summary>
		/// <param name="w2UserId">ユーザーID</param>
		public static void UnConnect(string w2UserId)
		{
			new UserService().ModifyUserExtend(
				w2UserId,
				model => { model.UserExtendDataValue[Common.Constants.SOCIAL_PROVIDER_ID_LINE] = string.Empty; },
				UpdateHistoryAction.DoNotInsert);
		}

		/// <summary>
		/// LINEユーザーIDをユーザー拡張項目に更新
		/// </summary>
		/// <param name="userId">ユーザーID</param>
		/// <param name="lineUserId">LINEユーザーID</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public static void UpdateUserExtendForLineUserId(
			string userId,
			string lineUserId,
			UpdateHistoryAction updateHistoryAction)
		{
			if (string.IsNullOrEmpty(Common.Constants.SOCIAL_PROVIDER_ID_LINE)) return;

			new UserService().ModifyUserExtend(
				userId,
				model => { model.UserExtendDataValue[Common.Constants.SOCIAL_PROVIDER_ID_LINE] = lineUserId; },
				updateHistoryAction);
		}

		/// <summary>
		/// ユーザー拡張項目からLINEユーザーIDを除去
		/// </summary>
		/// <param name="userExtend">ユーザー拡張項目</param>
		/// <param name="w2UserId">ユーザーID</param>
		/// <param name="updateHistoryAction">更新履歴アクション</param>
		public static void RemoveLineUserIdFromUserExtend(
			UserExtendModel userExtend,
			string w2UserId,
			UpdateHistoryAction updateHistoryAction)
		{
			userExtend.UserExtendDataValue[Common.Constants.SOCIAL_PROVIDER_ID_LINE] = string.Empty;
			new UserService().UpdateUserExtend(
				userExtend,
				w2UserId,
				w2.App.Common.Constants.FLG_LASTCHANGED_USER,
				updateHistoryAction);
		}
	}
}
