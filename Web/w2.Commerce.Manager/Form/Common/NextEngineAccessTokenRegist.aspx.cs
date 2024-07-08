/*
=========================================================================================================
  Module      : ネクストエンジンアクセストークン取得＆更新ページ(NextEngineAccessTokenRegist.aspx.cs)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright W2 Co.,Ltd. 2021 All Rights Reserved.
=========================================================================================================
*/
using System;
using System.Linq;
using System.Text;
using w2.App.Common.NextEngine;
using w2.Common.Web;

public partial class Form_NextEngineAccessToken_NextEngineAccessTokenRegist : BasePage
{
	/// <summary>
	/// ページロード
	/// </summary>
	/// <param name="sender"></param>
	/// <param name="e"></param>
	protected void Page_Load(object sender, EventArgs e)
	{
		var accessToken = string.Empty;
		var refreshToken = string.Empty;

		this.IsShowGetButton = false;

		//　DBにアクセストークンがある場合は、有効か確認する。
		if (NextEngineApi.IsExistsToken(out accessToken, out refreshToken))
		{
			var response = NextEngineApi.CallLoginUserApi(accessToken, refreshToken);
			if (response.Result == NextEngineConstants.FLG_RESULT_SUCCESS)
			{
				lNextEngineAccessTokenStatus.Text = HtmlSanitizer.HtmlEncode("アクセストークンは有効です。");
				return;
			}
		}

		var uid = Request[NextEngineConstants.PARAM_UID];
		var state = Request[NextEngineConstants.PARAM_STATE];
		if ((string.IsNullOrEmpty(uid) == false) && (string.IsNullOrEmpty(state) == false))
		{
			var response = NextEngineApi.CallAccessTokenApi(uid, state, Constants.NE_CLIENT_ID, Constants.NE_CLIENT_SECRET);
			if (response.Result == NextEngineConstants.FLG_RESULT_SUCCESS)
			{
				lNextEngineAccessTokenStatus.Text = HtmlSanitizer.HtmlEncode("アクセストークンの取得に成功しました。");
				return;
			}
			lNextEngineAccessTokenStatus.Text = HtmlSanitizer.HtmlEncode("アクセストークンの取得に失敗しました。後程再実行してください。");
		}
		else
		{
			var callBackUrl = new StringBuilder(Constants.PROTOCOL_HTTPS)
				.Append(Constants.SITE_DOMAIN)
				.Append(Constants.PATH_ROOT)
				.Append(Constants.PAGE_MANAGER_NE_ACCESS_TOKEN_REGIST)
				.ToString();
			var nextEngineLoginUrl =
				new UrlCreator(NextEngineConstants.LOGIN_URL)
					.AddParam(NextEngineConstants.PARAM_CLIENT_ID, Constants.NE_CLIENT_ID)
					.AddParam(NextEngineConstants.PARAM_REDIRECT_URI, callBackUrl)
					.CreateUrl();
			Response.Redirect(nextEngineLoginUrl);
		}
	}

	/// <summary>取得ボタンを表示するか？</summary>
	protected bool IsShowGetButton { get; set; }
}