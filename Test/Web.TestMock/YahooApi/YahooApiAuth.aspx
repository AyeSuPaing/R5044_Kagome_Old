<%@ Page Language="C#" AutoEventWireup="true" EnableSessionState="false" ValidateRequest="False" %>
<%--
=========================================================================================================
  Module      : YAHOO API AuthorizationエンドポイントAPI モッククラス(YahooApiAuth.aspx)
 ･･･････････････････････････････････････････････････････････････････････････････････････････････････････
  Copyright   : Copyright w2solution Co.,Ltd. 2023 All Rights Reserved.
=========================================================================================================
--%>

<script runat="server">
	void Main()
	{
		// リクエスト値の取得
		var queryStringValues = RetrieveRequestValues();

		// エラーを返す
		if (queryStringValues.ContainsKey("provokes_error"))
		{
			if (queryStringValues["provokes_error"] != null)
			{
				var errorQueryString = GenerateErrorResponseValues();
				var errorUrl = queryStringValues["redirect_uri"] + errorQueryString;
				Response.Redirect(errorUrl);
			}
		}
		
		// レスポンス値の生成
		// NOTE: https://developer.yahoo.co.jp/yconnect/v2/authorization_code/authorization.html
		var queryString = GenerateResponseValues(
			accessToken: "SlAV32hkKG",
			tokenType: "bearer",
			idToken: "eyJ0eXAiOiJKV1QiLCJhbGciOiJSUzI1NiIsImtpZCI6IjBjYzE3NWI5YzBmMWI2YTgzMWMzOTllMjY5NzcyNjYxIn0.eyJpc3MiOiJodHRwczpcL1wvYXV0aC5sb2dpbi55YWhvby5jby5qcFwveWNvbm5lY3RcL3YyIiwic3ViIjoiU0tBR1FOVlZTVDZYS0c2REFDUlFISFhETUkiLCJhdWQiOlsiZGowMGFpWnBQV013TlhBMlFrdEZXRWx2VENaelBXTnZibk4xYldWeWMyVmpjbVYwSm5nOU4yRS0iXSwiZXhwIjoxNjgwMjU1MDM4LCJpYXQiOjE2Nzc4MzU4MzgsImFtciI6WyJwd2QiXSwibm9uY2UiOiJuLTBTNl9XekEyTWoiLCJjX2hhc2giOiJrQW5ReFdEVGdhSTZOTTVjempoNk1RIn0.o2JJGFuixgD3C9KXhx0wCJykRsjHP5h7pUeRPu4vdbve87dtJsbOlmwTMSYl0DC0-uvqXO1J0GNWIIY1z18fQm3szXWXzespHG9OB2ZWbqDOVHdoGIbBUmaQMR5I5Q50aw6Fqk5gMSpZuXri5ltb7TZyztE_PTZl1uB0wsmNBOuojCq5XZjjRHPhO_R5ZEjwvFfvJiSwViyoQSiYKmhCr7W66qVldO9XjFPpx5VUsCrLDDnPcXpUCHYmRikbQbge2OTiR4SeI7Tjkxe5Xp6IfTWq5NjxAYZDOB__2XbxJ8g3c3toEFgTBdeZoE3LgwQ2t1T1Zmw6b6HU9YuuYsuINA",
			code: "SxlOBeZQ",
			state: "af0ifjsldkj");

		// リダイレクト
		var url = queryStringValues["redirect_uri"] + queryString;
		Response.Redirect(url);
	}

	/// <summary>
	/// リクエストの抽出
	/// </summary>
	/// <returns>リクエスト値セット</returns>
	private Dictionary<string, string> RetrieveRequestValues()
	{
		var queryString = this.Request.QueryString;
		var result = new Dictionary<string, string> { { "response_type", queryString["response_type"] }, { "client_id", queryString["client_id"] }, { "redirect_uri", queryString["redirect_uri"] }, { "bail", queryString["bail"] }, { "scope", queryString["scope"] }, { "state", queryString["state"] }, { "nonce", queryString["nonce"] }, { "display", queryString["display"] }, { "prompt", queryString["prompt"] }, { "max_age", queryString["max_age"] }, { "code_challenge", queryString["code_challenge"] }, { "code_challenge_method", queryString["code_challenge_method"] }, { "provokes_error", queryString["provokes_error"] }, };
		return result;
	}

	/// <summary>
	/// レスポンス値の生成
	/// </summary>
	/// <param name="accessToken">アクセストークン</param>
	/// <param name="tokenType">トークンタイプ</param>
	/// <param name="idToken">ID トークン</param>
	/// <param name="code">認可コード</param>
	/// <param name="state">状態</param>
	/// <returns>レスポンス値</returns>
	/// <see cref="https://developer.yahoo.co.jp/yconnect/v2/authorization_code/authorization.html"/>
	private string GenerateResponseValues(string accessToken, string tokenType, string idToken, string code, string state)
	{
		var resValues = new Dictionary<string, string>
		{
			{ "access_token", accessToken },
			{ "token_type", tokenType },
			{ "id_token", idToken },
			{ "code", code },
			{ "state", state },
		};
		var queryString = Join(resValues);
		return queryString;
	}

	/// <summary>
	/// エラーレスポンス値を生成
	/// </summary>
	/// <returns>エラーレスポンス値</returns>
	private string GenerateErrorResponseValues()
	{
		var resValues = new Dictionary<string, string>
		{
			{ "state", "af0ifjsldkj" },
			{ "error", "invalid_request" },
			{ "error_description", "Unsupported response_type value" },
			{ "error_code", "1000" },
		};
		var queryString = Join(resValues);
		return queryString;
	}

	/// <summary>
	/// レスポンス値の結合
	/// </summary>
	/// <param name="resValues">レスポンス値</param>
	/// <returns>結合したレスポンス値</returns>
	private string Join(Dictionary<string, string> resValues)
	{
		var queryString = "#" + string.Join("&", resValues.Select(v => string.Format("{0}={1}", v.Key, HttpUtility.UrlEncode(v.Value, Encoding.UTF8))));
		return queryString;
	}

</script>
<% this.Main(); %>
