/*
=========================================================================================================
  Module      : YAHOO API AuthorizationエンドポイントAPI コールバック ページ(YahooApiAuthCallback.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Web.UI;
using w2.App.Common.Mall.Yahoo.Procedures;
using w2.Common.Web;

/// <summary>
/// YAHOO API AuthorizationエンドポイントAPI コールバック ページ
/// </summary>
public partial class Form_MailLiaise_YahooApiAuthCallback : Page
{
	/// <summary>アクセストークン</summary>
	private const string QUERY_STRING_ACCESS_TOKEN = "access_token";
	/// <summary>トークンタイプ</summary>
	private const string QUERY_STRING_TOKEN_TYPE = "token_type";
	/// <summary>ID トークン</summary>
	private const string QUERY_STRING_ID_TOKEN = "id_token";
	/// <summary>認可コード</summary>
	private const string QUERY_STRING_CODE = "code";
	/// <summary>ステート</summary>
	private const string QUERY_STRING_STATE = "state";
	/// <summary>エラーコード</summary>
	private const string QUERY_STRING_ERROR = "error";
	/// <summary>エラー概要</summary>
	private const string QUERY_STRING_ERROR_DESCRIPTION = "error_description";
	/// <summary>エラー判定用コード</summary>
	private const string QUERY_STRING_ERROR_CODE = "error_code";

	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		if (!IsPostBack)
		{
			var input = RetrieveQueryStringValues();
			input.WriteLog();

			// 検証
			var state = SessionManager.YahooApiAntiForgeryStateCode;
			if (input.Validate(state) == false)
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = "検証に失敗しました。リクエストが不正です。管理者へ問い合わせてください。";
				var errUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR).CreateUrl();
				Response.Redirect(errUrl);
			}
			var mallId = SessionManager.YahooApiMallId;
			if (string.IsNullOrEmpty(mallId))
			{
				Session[Constants.SESSION_KEY_ERROR_MSG] = "セッションが切れました。リトライしてください。";
				var errUrl = new UrlCreator(Constants.PATH_ROOT + Constants.PAGE_MANAGER_ERROR).CreateUrl();
				Response.Redirect(errUrl);
			}

			// トークン更新
			var callbackUrl = Constants.PROTOCOL_HTTPS
				+ Constants.SITE_DOMAIN
				+ Constants.PATH_ROOT_EC
				+ Constants.PAGE_MANAGER_MALL_YAHOO_API_AUTH_CALLBACK;
			new YahooApiTokenProcedure().UpdateAccessTokenWithAuthCode(
				mallId: mallId,
				authCode: input.Code,
				redirectUri: callbackUrl);

			// 元の画面に戻る (モール連携基本設定編集画面)
			var url = new UrlCreator(Constants.PATH_ROOT_EC + Constants.PAGE_MANAGER_MALL_CONFIG)
				.AddParam(Constants.REQUEST_KEY_MALL_ID, mallId)
				.AddParam(Constants.REQUEST_KEY_ACTION_STATUS, Constants.ACTION_STATUS_DETAIL)
				.CreateUrl();
			Response.Redirect(url);
		}
	}

	/// <summary>
	/// クエリ文字列値を取得
	/// </summary>
	/// <returns>クエリ文字列値</returns>
	private YahooApiAuthCallbackInput RetrieveQueryStringValues()
	{
		var queryString = this.Request.QueryString;
		var result = new YahooApiAuthCallbackInput(
			accessToken: StringUtility.ToEmpty(queryString[QUERY_STRING_ACCESS_TOKEN]),
			tokenType: StringUtility.ToEmpty(queryString[QUERY_STRING_TOKEN_TYPE]),
			idToken: StringUtility.ToEmpty(queryString[QUERY_STRING_ID_TOKEN]),
			code: StringUtility.ToEmpty(queryString[QUERY_STRING_CODE]),
			state: StringUtility.ToEmpty(queryString[QUERY_STRING_STATE]),
			error: StringUtility.ToEmpty(queryString[QUERY_STRING_ERROR]),
			errorDescription: StringUtility.ToEmpty(queryString[QUERY_STRING_ERROR_DESCRIPTION]),
			errorCode: StringUtility.ToEmpty(queryString[QUERY_STRING_ERROR_CODE]));
		return result;
	}
}
